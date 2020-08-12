import asyncio
import cogs
import discord
import importlib
import json
import os
import random
import re
import settings
import shutil
import sys
import utilities

from datetime import datetime, timedelta
from discord.ext import commands

sys.path.insert(1, settings.COGS_PATH)
import dispatch

async def on_reset(ctx):

    while True:

        utc_now = datetime.utcnow()

        if (utc_now.hour == 0):
            await ctx.send("It is reset time my dudes!")
            time_left = 86400
        else:
            hour = 23 - utc_now.hour
            minute = 59 - utc_now.minute
            sec = 59 - utc_now.second

            time = timedelta(hours=hour, minutes=minute, seconds=sec)
            time_left = time.total_seconds()

        await asyncio.sleep(time_left)

def init_cogs(bot, cog_list):
    """Add all the cogs in the given list of available cogs"""
    
    for cog in cog_list:
        importlib.import_module(cog)
        bot.add_cog(dispatch.dispatcher[cog]())

def dir_check(path):

    if not os.path.isdir(path):
        os.mkdir(path)

sage = commands.Bot(command_prefix=settings.PREFIX)

@sage.command()
async def init_tasks(ctx):
    sage.loop.create_task(on_reset(ctx))

# @cumberlan.event
# async def on_guild_join(guild):
#     """
#         Creates a dedicated directory for newly joined server, and adds it to a list.
#         Also generates a list of members associated with the server.
#     """

#     GUILD_ID = settings.GUILDS_PATH + str(guild.id)
#     MEMBER_PATH = GUILD_ID + "/members_list.json"

#     if not os.path.isdir(settings.GUILDS_PATH):
#         os.mkdir(settings.GUILDS_PATH)

#     if not os.path.isdir(GUILD_ID):
#         os.mkdir(GUILD_ID)

#     # Creates a guilds_list.json file with newly joined server info
#     if not os.path.isfile(settings.GUILDS_LIST):

#         guilds_list = {str(guild.id) : guild.name}

#         with open(settings.GUILDS_LIST, "w") as guilds_file:
#             json.dump(guilds_list, guilds_file)
#     # Modifies the guilds_list.json file with newly joined server info
#     else:

#         with open(settings.GUILDS_LIST, "w") as guilds_file:
#             guilds_list = json.load(guilds_file)

#         if str(guild.id) in guilds_list:
#             return

#         guilds_list[str(guild.id)] = guild.name

#         with open(settings.GUILDS_LIST, "w") as guilds_file:
#             json.dump(guilds_list, guilds_file)

#     # Generates a list of members belonging to the server
#     member_list = {}

#     for member in guild.members:

#         member_id = "<@!%s>" % (member.id)
#         member_list[member_id] = member.display_name

#     with open(MEMBER_PATH, "w") as member_file:
#         json.dump(member_list, member_file)

# @cumberlan.event
# async def on_guild_remove(guild):
#     """
#         Removes all information associated with the server.
#     """

#     GUILD_ID = settings.GUILDS_PATH + str(guild.id)

#     with open(settings.GUILDS_LIST, "r") as guilds_file:
#         guilds_list = json.load(guilds_file)

#     if str(guild.id) in guilds_list:
#         del guilds_list[str(guild.id)]

#     with open(settings.GUILDS_LIST, "w") as guilds_file:
#         json.dump(guilds_list, guilds_file)

#     shutil.rmtree(GUILD_ID)

# Does two important actions:
#   1) Does a quick housekeeping action
#   2) Serves as a sanity check to display if bot is online
@sage.event
async def on_ready():

    # dir_check(settings.GUILDS_PATH)

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
with open(settings.DISPATCHER_PATH, "r") as settings_file:
    data = json.load(settings_file)

cogs_list = data["cogs"]

init_cogs(sage, cogs_list)

# Initializing the bot
sage.run(utilities.load_token(settings.TOKEN_PATH))
