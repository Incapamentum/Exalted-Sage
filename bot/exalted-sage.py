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
from discord.ext import commands
from pathlib import Path
from requests_futures.sessions import FuturesSession

# def obtain_description(achieve_id):

#     session = FuturesSession()

#     request = session.get("https://api.guildwars2.com/v2/achievements/" + str(achieve_id))
#     request_result = request.result()

#     result = json.loads(request_result.text)

#     return result['requirement']

# async def on_reset(ctx):

#     session = FuturesSession()

#     while True:

#         utc_now = datetime.utcnow()

#         # Checks the watchlist of daily achievements and determines if it will
#         # make an announcement about one being available or not
#         if (utc_now.hour == 0):

#             embed_msg = discord.Embed(color=0xffee05)
#             report = False

#             request = session.get("https://api.guildwars2.com/v2/achievements/daily/tomorrow")
#             request_result = request.result()

#             result = json.loads(request_result.text)

#             # Obtains a list of the PVE daily achievements for tomorrow.
#             # Each entry in the list is a dictionary
#             pve_daily = result['pve']

#             # Obtaining the list of dailies we are watching for
#             # Each entry in the list is a Daily name
#             daily_list = read_data(DAILY_LIST_PATH)

#             # Obtaining the dictionary associated each Daily name with its
#             # respective ID to be compared against the watchlist
#             with open(DAILY_PATH, "r") as daily_file:
#                 daily_achieves = json.load(daily_file)

#             daily_names = []

#             for entry in daily_list:

#                 daily_id = daily_achieves[entry]

#                 for daily in pve_daily:

#                     if ((daily['id'] == daily_id) and (entry not in daily_names)):

#                         report = True
#                         daily_names.append(entry)

#                         embed_msg.add_field(
#                             name=entry,
#                             value=obtain_description(daily_id),
#                             inline=False
#                         )

#             if (report):

#                 ping = ""

#                 with open(AO_PATH, "r") as guild_file:
#                     auric_oasis = json.load(guild_file)

#                 mentions = auric_oasis['role_mentions'] + auric_oasis['user_mentions']

#                 for role in auric_oasis['roles']:

#                     if (role in mentions):
#                         ping += (auric_oasis['roles'][role] + ' ')

#                 for member in auric_oasis['members']:

#                     if (member in mentions):
#                         ping += (auric_oasis['members'][member] + ' ')

#                 response = "Attention! A daily achievement that is being monitored will appear tomorrow!\n"
#                 await ctx.send(response + ping, embed=embed_msg)
#                 return

#             await ctx.send("Nothing to report...")

#             # Full 24hrs (86400) + 1min
#             time_left = 86460
#         else:
#             hour = 23 - utc_now.hour
#             minute = 59 - utc_now.minute
#             sec = 59 - utc_now.second

#             time = timedelta(hours=hour, minutes=minute, seconds=sec)
#             time_left = time.total_seconds() + 60

#         await asyncio.sleep(time_left)

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

# @sage.command()
# async def init_tasks(ctx):

#     exalted = discord.utils.get(ctx.guild.roles, name="Exalted")
#     ascended = discord.utils.get(ctx.guild.roles, name="Ascended")

#     if ((exalted in ctx.author.roles) or (ascended in ctx.author.roles)):
#         sage.loop.create_task(on_reset(ctx))
#         await ctx.send("I shall uphold the tasks that I have been assigned!")
#         return

#     await ctx.send("It seems that you do not have the required permissions to run this command...")
#     return

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

    if ((exalted in ctx.author.roles) or (ascended in ctx.author.roles) or (creator == ctx.author.name)):

        if (db.roles.find_one({"Title" : "Auric Oasis Roles"}) is None):

            roles_doc = {}
            roles_list = {}

            roles_doc["Title"] = "Auric Oasis Roles"

            # Collecting all roles in the guild
            for role in guild.roles:
                roles_list[role.name] = role.mention

            roles_doc["roles"] = roles_list

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
