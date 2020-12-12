import discord
import json
import os
import pymongo
import sys

sys.path.insert(1, "/bot/")

from settings import MONGO_CONNECT
from discord.ext import commands
from requests_futures.sessions import FuturesSession

INSUFF_ARGS = "Insufficient arguments!"
NO_UNDERSTAND = "I do not understand what you are asking me to do."

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

# def update_data(entry_list, path):
#     """
#         Takes the given list of strings to write-thru the data
#         in the file pointed at by the 'path' parameter
#     """

#     with open(path, "w") as daily_file:

#         for entry in entry_list:
#             daily_file.write(entry + '\n')

# def create_new(entry, path):
#     """
#         Takes the given 'entry' and writes it to the file
#         pointed at by the 'path' parameter
#     """

#     with open(path, "w") as daily_file:
#         daily_file.write(entry + '\n')

# def read_data(file_path):
#     """
#         Returns a list of entries found within the .txt file
#         pointed at by the 'file_path' parameter
#     """

#     data = []

#     with open(file_path, "r") as f:
        
#         while True:
#             line = f.readline()

#             if not line:
#                 break

#             data.append(line.strip())

#     return data

# def obtain_description(achieve_id):

#     session = FuturesSession()

#     request = session.get("https://api.guildwars2.com/v2/achievements/" + str(achieve_id))
#     request_result = request.result()

#     result = json.loads(request_result.text)

#     return result['requirement']

class DailyAlertCog(commands.Cog):
    """
        Currently a WiP
    """

    @commands.command(name="daily-alert", help=" - Series of commands relating to the addition or removal of dailies to be alerted to!", pass_context = True)
    async def daily(self, ctx, *args):
        """
            Some stuff in here
        """

        if (args):

            if (args[0] == "clear-list"):

                # if (os.path.isfile(DAILY_LIST_PATH)):
                #     open(DAILY_LIST_PATH, "w").close()

                await ctx.send("Daily Watchlist has been cleared!")

            elif (args[0] == "add"):

                if (len(args) < 2):
                    await ctx.send(INSUFF_ARGS)
                    return

                dailyToAdd = "Daily " + tupToSentence(args[1:])
                dailyToAdd = dailyToAdd.strip()

                print(dailyToAdd)

                # with open(DAILY_PATH, "r") as daily_file:
                #     daily_achieve = json.load(daily_file)

                # if (dailyToAdd not in daily_achieve):
                #     await ctx.send("Cannot recognize the achievement!")
                #     return

                # if (os.path.isfile(DAILY_LIST_PATH)):

                #     dailyList = read_data(DAILY_LIST_PATH)

                #     if dailyToAdd not in dailyList:
                #         dailyList.append(dailyToAdd)
                #         update_data(dailyList, DAILY_LIST_PATH)                    
                #     else:
                #         await ctx.send("Entry already exists!")
                #         return

                # else:
                #     create_new(dailyToAdd, DAILY_LIST_PATH)

                await ctx.send("Daily has been added to the Watchlist!")
                return

            elif (args[0] == "remove"):

                if (len(args) < 2):
                    await ctx.send(INSUFF_ARGS)
                    return

                dailyToRemove = "Daily " + tupToSentence(args[1:])
                dailyToRemove = dailyToRemove.strip()

                # if (os.path.isfile(DAILY_LIST_PATH)):

                #     dailyList = read_data(DAILY_LIST_PATH)

                #     if dailyToRemove in dailyList:
                #         dailyList.remove(dailyToRemove)
                #         update_data(dailyList, DAILY_LIST_PATH)
                #     else:
                #         await ctx.send("Entry does not exist!")
                #         return

                #     await ctx.send("Entry has been removed!")

                # else:
                #     await ctx.send("The Watchlist is currently empty!")


            # elif (args[0] == "display"):

            #     with open(DAILY_PATH, "r") as daily_file:
            #         daily_achieve = json.load(daily_file)

            #     response = "Here is the list of daily achievements I am currently keeping track of..."
            #     embed_msg = discord.Embed(color=0xffee05)

            #     dailyList = read_data(DAILY_LIST_PATH)

            #     for entry in dailyList:

            #         daily_id = daily_achieve[entry]

            #         embed_msg.add_field(
            #             name=entry,
            #             value=obtain_description(daily_id),
            #             inline=False
            #         )

            #     await ctx.send(response, embed=embed_msg)
            #     return

            # elif (args[0] == "notify"):

            #     if (len(args) < 2):
            #         await ctx.send(INSUFF_ARGS)
            #         return

            #     # How to make this so that only officers can use this?
            #     if (args[1] == "role"):

            #         max_role = ctx.message.author.roles[len(ctx.message.author.roles) - 1]

            #         if ((max_role.name != "Exalted") or (max_role != "Ascended")):
            #             await ctx.send("It seems that you do not have the required permissions to run this command...")
            #             return

            #         with open(AO_PATH, "r") as guild_file:
            #             auric_oasis = json.load(guild_file)

            #         mentions_list = auric_oasis["role_mentions"]
            #         mentions_list.append(args[2])
            #         auric_oasis["role_mentions"] = mentions_list

            #         with open(AO_PATH, "w") as guild_file:
            #             json.dump(auric_oasis, guild_file)

            #         await ctx.send("This role will now be notified of any daily alerts!")
            #         return

            #     elif (args[1] == "me"):

            #         with open(AO_PATH, "r") as guild_file:
            #             auric_oasis = json.load(guild_file)

            #         mentions_list = auric_oasis["user_mentions"]
            #         mentions_list.append(str(ctx.message.author))
            #         auric_oasis["user_mentions"] = mentions_list

            #         with open(AO_PATH, "w") as guild_file:
            #             json.dump(auric_oasis, guild_file)
                    
            #         await ctx.send("You will now be notified of any daily alerts!")
            #         return

            else:
                await ctx.send("I do not understand what you are asking me to do.")
                return

        else:

            await ctx.send(INSUFF_ARGS)
            return