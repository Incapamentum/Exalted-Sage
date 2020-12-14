import asyncio
import discord
import dns
import dotenv
import importlib
import json
import os
import pymongo
import random
import re
import settings
import shutil

from datetime import datetime, timedelta
from discord.ext import commands, tasks
from dotenv import load_dotenv
from pathlib import Path
from requests_futures.sessions import FuturesSession

def get_id(daily_achieves, daily):
    """
        Returns the key/index corresponding to the given
        daily achieve
    """

    for i_d, val in daily_achieves.items():
        if daily == val:
            return i_d

def obtain_description(achieve_id):

    session = FuturesSession()

    request = session.get("https://api.guildwars2.com/v2/achievements/" + str(achieve_id))
    request_result = request.result()

    result = json.loads(request_result.text)

    return result['requirement']

def init_cogs(bot, cog_list):
    """
        Add all the cogs in the given list of available cogs
    """

    for cog in cog_list:
        bot.load_extension(settings.COGS_PATH.strip("./") + "." + cog)

# Connecting to the MongoDB cluster and accessing the Auric_Oasis
# database
mongo_client = pymongo.MongoClient(settings.MONGO_CONNECT)
db = mongo_client.Auric_Oasis

# Bot initialization
sage = commands.Bot(command_prefix=settings.PREFIX)
sage.remove_command("help")

@tasks.loop(minutes=50)
async def on_reset():

    broadcast_channel = sage.get_channel(int(settings.BROADCAST_CHANNEL))
    session = FuturesSession()

    # This will be needed for later
    utc_now = datetime.utcnow()

    embed_msg = discord.Embed(color=0xffee05)
    repot = False

    # This will definitely be needed for later
    if (utc_now.hour == 0):

        request = session.get("https://api.guildwars2.com/v2/achievements/daily/tomorrow")
        request_result = request.result()

        result = json.loads(request_result.text)

        # Obtain PVE dailies that will be available tomorrow
        # Each entry in this is a dictionary
        pve_daily = result['pve']

        # Obtaining the collection of daily achieves
        dailyAchieve_doc = db.daily_achievements.find_one({"Title" : "List of Daily Achievements"})
        daily_list = dailyAchieve_doc["achievements"]

        # Obtaining the daily watchlist
        watchlist_doc = db.daily_achievements.find_one({"Title" : "Daily Watchlist"})
        watchlist = watchlist_doc["watchlist"]

        if (watchlist):

            report = False

            for entry in watchlist:

                daily_id = int(get_id(daily_list, entry))

                for daily in pve_daily:

                    if ((daily_id == daily['id'])):
                        
                        report = True

                        embed_msg.add_field(
                            name=entry,
                            value=obtain_description(daily_id),
                            inline=False
                        )

            if (report):

                ping = ""

                # Obtaining the notify list
                roles_doc = db.roles.find_one({"Title" : "Auric Oasis Roles"})
                notify_list = roles_doc["notify_list"]

                for role in notify_list:

                    ping += role + ' '

                response = "Attention! A daily achievement that is being monitored will appear tomorrow!\n"
                await broadcast_channel.send(response + ping, embed=embed_msg)
                return

            await broadcast_channel.send("Nothing to report...")

@sage.command()
async def init_guild(ctx):
    """
        Used to generate a database for the guild, filling it with roles that
        exist in the guild
    """

    creator = "Goose"
    exalted = discord.utils.get(ctx.guild.roles, name="Exalted")
    ascended = discord.utils.get(ctx.guild.roles, name="Ascended")
    guild = ctx.guild

    if ((exalted in ctx.author.roles) or (ascended in ctx.author.roles) or (ctx.author.name == creator)):

        if (db.roles.find_one({"Title" : "Auric Oasis Roles"}) is None):

            roles_doc = {}
            roles_list = {}

            roles_doc["Title"] = "Auric Oasis Roles"

            # Collecting all roles in the guild
            for role in guild.roles:
                roles_list[role.name] = role.mention

            roles_doc["roles"] = roles_list
            roles_doc["notify_list"] = []

            db.roles.insert_one(roles_doc)

            await ctx.send("My database regarding the guild hierarchy has been initialized!")
        
        else:
            await ctx.send("My database regarding the guild hierarchy does not need to be initialized.")

    else:
        await ctx.send("My apologies, but you are not authorized to issue this command.")

@sage.event
async def on_guild_join(guild):
    """
        Obtains the list of all roles found in the server
    """

    roles_doc = {}
    roles_list = {}

    roles_doc["Title"] = "Auric Oasis Roles"

    # Collecting all roles in the guild
    for role in guild.roles:
        roles_list[role.name] = role.mention

    roles_doc["roles"] = roles_list
    roles_doc["notify_list"] = []

    # Inserting the 'roles' document to the Auric_Oasis database
    db.roles.insert_one(roles_doc)

# Figure out a nice way of updating the database
@sage.event
async def on_guild_role_create(role):
    """
        Updates the 'roles with the newly created role
    """

    roles_doc = db.roles.find_one({"Title" : "Auric Oasis Roles"})

    roles_list = roles_doc["roles"]
    roles_list[role.name] = role.mention

    db.roles.update_one({"Title" : "Auric Oasis Roles"}, {"$set": {"roles" : roles_list}})

@sage.event
async def on_guild_role_update(before, after):
    """
        Updates the 'roles' with the updated role
    """

    roles_doc = db.roles.find_one({"Title" : "Auric Oasis Roles"})

    roles_list = roles_doc["roles"]

    # Check to see the name of the updated role has not changed
    if (after.name not in roles_list):

        # Removes the old role from roles_list
        roles_list.pop(before.name, None)

        # Add the updated role to roles_list
        roles_list[after.name] = after.mention

        db.roles.update_one({"Title" : "Auric Oasis Roles"}, {"$set": {"roles" : roles_list}})

@sage.event
async def on_guild_role_delete(role):
    """
        Updates the 'roles' by removing the deleted role
    """

    roles_doc = db.roles.find_one({"Title" : "Auric Oasis Roles"})

    roles_list = roles_doc["roles"]
    roles_list.pop(role.name, None)

    db.roles.update_one({"Title" : "Auric Oasis Roles"}, {"$set": {"roles" : roles_list}})

# This is currently scuffed
@sage.event
async def on_guild_remove(guild):
    """
        Removes all information associated with the server
    """

    mongo_client.drop_database("Auric_Oasis")

# Does two important actions:
#   1) Does a quick housekeeping action
#   2) Serves as a sanity check to display if bot is online
@sage.event
async def on_ready():

    print("Logged in as")
    print(sage.user.name)
    print(sage.user.id)
    print("------")

    on_reset.start()

    # Shows that the bot is doing a listening 'activity' for help
    helpActivity = discord.Activity(name = settings.PREFIX + "help", type = discord.ActivityType.listening)
    await sage.change_presence(activity=helpActivity)

@sage.event
async def on_message(msg):

    if msg.author == sage.user:
        return

    if msg.content.startswith(settings.PREFIX):
        await sage.process_commands(msg)
        return

    message = msg.content.lower()
    msg_split = message.split()

    if (("tarir" in msg_split) or ("ab" in msg_split)):
        await msg.channel.send("We have protected Tarir for over a century. It will not fall this day.")
        return

# Obtaining list of commands/cogs to include in the bot
cogs = settings.DISPATCHER

init_cogs(sage, cogs)

# Initializing the bot
sage.run(settings.TOKEN)
