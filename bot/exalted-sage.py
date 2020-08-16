import asyncio
import discord
import dotenv
import importlib
import json
import os
import random
import re
import settings
import shutil

from datetime import datetime, timedelta
from discord.ext import commands
from pathlib import Path
from requests_futures.sessions import FuturesSession

DAILY_LIST_PATH = "./data/achievements/daily_list.txt"
DAILY_PATH = "./data/achievements/daily_achievements.json"
AO_PATH = "./data/guild/auric_oasis.json"

def read_data(file_path):
    """
        Returns a list of entries found within the .txt file
        pointed at by the 'file_path' parameter
    """

    data = []

    with open(file_path, "r") as f:

        while True:
            line = f.readline()

            if not line:
                break

            data.append(line.strip())

    return data

def obtain_description(achieve_id):

    session = FuturesSession()

    request = session.get("https://api.guildwars2.com/v2/achievements/" + str(achieve_id))
    request_result = request.result()

    result = json.loads(request_result.text)

    return result['requirement']

async def on_reset(ctx):

    session = FuturesSession()

    while True:

        utc_now = datetime.utcnow()

        # Checks the watchlist of daily achievements and determines if it will
        # make an announcement about one being available or not
        if (utc_now.hour == 0):

            embed_msg = discord.Embed(color=0xffee05)
            report = False

            request = session.get("https://api.guildwars2.com/v2/achievements/daily/tomorrow")
            request_result = request.result()

            result = json.loads(request_result.text)

            # Obtains a list of the PVE daily achievements for tomorrow.
            # Each entry in the list is a dictionary
            pve_daily = result['pve']

            # Obtaining the list of dailies we are watching for
            # Each entry in the list is a Daily name
            daily_list = read_data(DAILY_LIST_PATH)

            # Obtaining the dictionary associated each Daily name with its
            # respective ID to be compared against the watchlist
            with open(DAILY_PATH, "r") as daily_file:
                daily_achieves = json.load(daily_file)

            daily_names = []

            for entry in daily_list:

                daily_id = daily_achieves[entry]

                for daily in pve_daily:

                    if ((daily['id'] == daily_id) and (entry not in daily_names)):

                        report = True
                        daily_names.append(entry)

                        embed_msg.add_field(
                            name=entry,
                            value=obtain_description(daily_id),
                            inline=False
                        )

            if (report):

                ping = ""

                with open(AO_PATH, "r") as guild_file:
                    auric_oasis = json.load(guild_file)

                mentions = auric_oasis['role_mentions'] + auric_oasis['user_mentions']

                for role in auric_oasis['roles']:

                    if (role in mentions):
                        ping += (auric_oasis['roles'][role] + ' ')

                for member in auric_oasis['members']:

                    if (member in mentions):
                        ping += (auric_oasis['members'][member] + ' ')

                response = "Attention! A daily achievement that is being monitored will appear tomorrow!\n"
                await ctx.send(response + ping, embed=embed_msg)
                return

            await ctx.send("Nothing to report...")

            # Full 24hrs (86400) + 1min
            time_left = 86460
        else:
            hour = 23 - utc_now.hour
            minute = 59 - utc_now.minute
            sec = 59 - utc_now.second

            time = timedelta(hours=hour, minutes=minute, seconds=sec)
            time_left = time.total_seconds() + 60

        await asyncio.sleep(time_left)

def init_cogs(bot, cog_list):
    """
        Add all the cogs in the given list of available cogs
    """

    for cog in cog_list:
        bot.load_extension(settings.COGS_PATH.strip("./") + "." + cog)

def dir_check(path):

    if not os.path.isdir(path):
        os.mkdir(path)

sage = commands.Bot(command_prefix=settings.PREFIX)
sage.remove_command("help")

@sage.command()
async def init_tasks(ctx):

    exalted = discord.utils.get(ctx.guild.roles, name="Exalted")
    ascended = discord.utils.get(ctx.guild.roles, name="Ascended")

    if ((exalted in ctx.author.roles) or (ascended in ctx.author.roles)):
        sage.loop.create_task(on_reset(ctx))
        await ctx.send("I shall uphold the tasks that I have been assigned!")
        return

    await ctx.send("It seems that you do not have the required permissions to run this command...")
    return

@sage.event
async def on_guild_join(guild):
    """
        Generates a list of members by mapping their unique member mention ID
        with their display names, and a list of roles by mapping their role
        names with their unique role mention ID. Also creates a 'roles_mention'
        that will hold an empty list
    """

    auric_oasis = {}
    members_list = {}
    roles_list = {}

    for member in guild.members:
        members_list[str(member)] = member.mention

    for role in guild.roles:
        roles_list[role.name] = role.mention

    auric_oasis["members"] = members_list
    auric_oasis["roles"] = roles_list
    auric_oasis["role_mentions"] = []
    auric_oasis["user_mentions"] = []

    with open(AO_PATH, "w") as guild_file:
        json.dump(auric_oasis, guild_file)

@sage.event
async def on_member_join(member):
    """
        Updates the member list whenever a new member joins the server by
        adding their unique member ID and their display name
    """

    with open(AO_PATH, "r") as guild_file:
        auric_oasis = json.load(guild_file)

    members_list = auric_oasis["members"]
    members_list[str(member)] = member.mention

    auric_oasis["members"] = members_list

    with open(AO_PATH, "w") as guild_file:
        json.dump(auric_oasis, guild_file)

@sage.event
async def on_guild_role_create(role):
    """
        Updates the roles_list with the newly created role
    """

    with open(AO_PATH, "r") as guild_file:
        auric_oasis = json.load(guild_file)

    roles_list = auric_oasis["roles"]
    roles_list[role.name] = role.mention

    auric_oasis["roles"] = roles_list

    with open(AO_PATH, "w") as guild_file:
        json.dump(auric_oasis, guild_file)

@sage.event
async def on_member_remove(member):
    """
        Updates the member list whenever a member leaves the server by
        removing their information from the list
    """

    with open(AO_PATH, "r") as guild_file:
        auric_oasis = json.load(guild_file)

    members_list = auric_oasis["members"]
    mentions_list = auric_oasis["user_mentions"]

    if (str(member) in mentions_list):
        mentions_list.remove(member.mention)
        auric_oasis["user_mentions"] = mentions_list

    del members_list[str(member)]

    auric_oasis["members"] = members_list

    with open(AO_PATH, "w") as guild_file:
        json.dump(auric_oasis, guild_file)

@sage.event
async def on_guild_role_delete(role):
    """
    """

    with open(AO_PATH, "r") as guild_file:
        auric_oasis = json.load(guild_file)

    roles_list = auric_oasis["roles"]
    mentions_list = auric_oasis["role_mentions"]

    if (role.name in mentions_list):

        mentions_list.remove(role.name)
        auric_oasis["role_mentions"] = mentions_list

    del roles_list[role.mention]

    auric_oasis["roles"] = roles_list

    with open(AO_PATH, "w") as guild_file:
        json.dump(auric_oasis, guild_file)

@sage.event
async def on_member_update(before, after):
    """
        Updates the member list whenever a member updates their profile, which
        could be their status, activity, nickname, or roles
    """

    with open(AO_PATH, "r") as guild_file:
        auric_oasis = json.load(guild_file)

    members_list = auric_oasis["members"]
    members_list[str(after)] = after.mention

    auric_oasis["members"] = members_list

    with open(AO_PATH, "w") as guild_file:
        json.dump(auric_oasis, guild_file)

@sage.event
async def on_guild_remove(guild):
    """
        Removes all information associated with the server
    """

    os.remove(AO_PATH)

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
with open(settings.DISPATCHER_PATH, "r") as settings_file:
    cogs = json.load(settings_file)

init_cogs(sage, cogs['cogs'])

# Initializing the bot
sage.run(settings.TOKEN)
