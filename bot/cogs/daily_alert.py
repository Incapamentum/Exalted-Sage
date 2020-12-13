import discord
import json
import pymongo
import sys

sys.path.insert(1, "/bot/")

from settings import MONGO_CONNECT
from discord.ext import commands
from requests_futures.sessions import FuturesSession

INSUFF_ARGS = "Insufficient arguments!"
NO_UNDERSTAND = "I do not understand what you are asking me to do."

mongo_client = pymongo.MongoClient(MONGO_CONNECT)
db = mongo_client.Auric_Oasis

def setup(bot):
    """
        Add this cog to the given bot
    """

    bot.add_cog(DailyAlertCog(bot))

def tupToSentence(tup):
    """
        Takes a tuple, converts it to a string
    """

    words = len(tup)
    sentence = ""

    for i in range(words):

        sentence += tup[i]

        if (i < words):
            sentence += ' '

    return sentence

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

class DailyAlertCog(commands.Cog):
    """
        Currently a WiP
    """

    @commands.command(name="daily-alert", help=" - Series of commands relating to the addition or removal of dailies to be alerted to!", pass_context = True)
    async def daily(self, ctx, *args):
        """
            Collection of commands specific for 'daily-alert'
        """

        creator = "Goose"
        exalted = discord.utils.get(ctx.guild.roles, name="Exalted")
        ascended = discord.utils.get(ctx.guild.roles, name="Ascended")

        if ((exalted in ctx.author.roles) or (ascended in ctx.author.roles) or (ctx.author.name == creator)):

            if (args):

                # Obtaining daily achieves info
                dailyAchieves_doc = db.daily_achievements.find_one({"Title" : "List of Daily Achievements"})
                daily_list = dailyAchieves_doc["achievements"]

                # Obtaining watchlist info
                watchlist_doc = db.daily_achievements.find_one({"Title" : "Daily Watchlist"})
                watchlist = watchlist_doc["watchlist"]

                # Obtaining info on roles and notify
                roles_doc = db.roles.find_one({"Title" : "Auric Oasis Roles"})
                roles_list = roles_doc["roles"]
                notify_list = roles_doc["notify_list"]

                # Clears the watchlist
                if (args[0] == "clear-list"):

                    # Checks to see if the watchlist is empty or not
                    if (not watchlist):
                        await ctx.send("There is no Watchlist to clear!")
                        return

                    watchlist.clear()

                    db.daily_achievements.update_one({"Title" : "Daily Watchlist"}, {"$set": {"watchlist" : watchlist}})

                    await ctx.send("Daily Watchlist has been cleared!")

                # If it is not present, inserts the daily to the watchlist
                elif (args[0] == "add-daily"):

                    if (len(args) < 2):
                        await ctx.send(INSUFF_ARGS)
                        return

                    dailyToAdd = "Daily " + tupToSentence(args[1:])
                    dailyToAdd = dailyToAdd.strip()

                    # Check to see if the achieve is valid
                    if (dailyToAdd not in daily_list.values()):
                        await ctx.send("Cannot recognize this achievement!")
                        return

                    # Makes sure no repeat is being added to the watchlist
                    if (dailyToAdd not in watchlist):

                        watchlist.append(dailyToAdd)
                        db.daily_achievements.update_one({"Title" : "Daily Watchlist"}, {"$set": {"watchlist": watchlist}})

                        await ctx.send("%s has been added to the Watchlist!" % (dailyToAdd))
                        return
                    else:
                        await ctx.send("Daily is already being monitored!")
                        return
                
                elif (args[0] == "remove-daily"):

                    if (len(args) < 2):
                        await ctx.send(INSUFF_ARGS)
                        return

                    dailyToRemove = "Daily " + tupToSentence(args[1:])
                    dailyToRemove = dailyToRemove.strip()

                    # Check to see if the achieve is valid
                    if (dailyToRemove not in daily_list.values()):
                        await ctx.send("Cannot recognize this achievement!")

                    if (watchlist):

                        # Makes sure the achieve exists in the watchlist
                        if (dailyToRemove in watchlist):

                            watchlist.remove(dailyToRemove)
                            db.daily_achievements.update_one({"Title" : "Daily Watchlist"}, {"$set": {"watchlist": watchlist}})

                            await ctx.send("%s has been removed from the Watchlist!" % (dailyToRemove))
                            return
                        else:
                            await ctx.send("This daily is not in the watchlist!")
                            return

                    else:
                        await ctx.send("Nothing to remove from the watchlist!")
                        return

                # Displays the watchlist
                elif (args[0] == "display-watchlist"):

                    response = "Here is the list of dailies I am currently keeping track of..."
                    embed_msg = discord.Embed(color=0xffee05)

                    for entry in watchlist:

                        daily_id = get_id(daily_list, entry)

                        embed_msg.add_field(
                            name=entry,
                            value=obtain_description(daily_id),
                            inline=False
                        )

                    await ctx.send(response, embed=embed_msg)
                    return

                # Adds a role to the notify list
                elif (args[0] == "add-notify"):

                    if (len(args) < 2):
                        await ctx.send(INSUFF_ARGS)
                        return

                    roleToAdd = tupToSentence(args[1:])
                    roleToAdd = roleToAdd.strip()

                    # Makes sure role is valid within the guild, and not in the notify list
                    if ((roleToAdd in roles_list.values()) and (roleToAdd not in notify_list)):

                        notify_list.append(roleToAdd)
                        db.roles.update_one({"Title" : "Auric Oasis Roles"}, {"$set": {"notify_list" : notify_list}})

                        await ctx.send("%s has been added to the notify list!" % (roleToAdd))
                        return

                    else:
                        await ctx.send("Role either does not exist in the guild, or has already been added!")
                        return

                elif (args[0] == "remove-notify"):

                    if (len(args) < 2):
                        await ctx.send(INSUFF_ARGS)
                        return

                    if (notify_list):

                        roleToRemove = tupToSentence(args[1:])
                        roleToRemove = roleToRemove.strip()

                        # Makes sure role is valid within the guild, and in the notify list
                        if ((roleToRemove in roles_list.values()) and (roleToRemove in notify_list)):

                            notify_list.remove(roleToRemove)
                            db.roles.update_one({"Title" : "Auric Oasis Roles"}, {"$set": {"notify_list" : notify_list}})

                            await ctx.send("%s has been removed from the notify list!" % (roleToRemove))
                            return

                        else:
                            await ctx.send("Role either does not exist in the guild, or is not in the notify list!")
                            return

                    else:
                        await ctx.send("Nothing to remove from the notify list!")
                        return

                elif (args[0] == "display-notify"):

                    response = "Here is the list of roles I am tasked to notify..."
                    embed_msg = discord.Embed(color=0xffee05)

                    for role in notify_list:

                        role_name = get_id(roles_list, role)

                        embed_msg.add_field(
                            name=role_name,
                            value='\u200b',
                            inline=False
                        )

                    await ctx.send(response, embed=embed_msg)
                    return

                else:
                    await ctx.send("I do not understand what you are asking me to do!")
                    return
            
            else:
                await ctx.send(INSUFF_ARGS)
                return

        else:
            await ctx.send("My apologies, but you are not authorized to issue this command.")