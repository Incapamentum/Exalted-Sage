import discord

from discord.ext import commands
from settings import PREFIX

def setup(bot):
    """
        Add this cog to the bot
    """

    bot.add_cog(HelpCog(bot))

class HelpCog(commands.Cog):
    """

    """

    @commands.command(name="help", help=" - Produces a modest help menu listing available commands", pass_context = True)
    async def help(self, ctx, *args):
        """
            Displays an embedded help menu
        """

        if (args):

           cmd = args[0]

           response = "Here are additional details on the %s command" % (cmd)

           if (cmd == "daily-alert"):

               embed = "```\nPossible arguments:\n\tclear-list    Clears the Daily Watchlist \n\tadd           Adds a Daily Achievement to the watchlist \n\tremove        Removes a Daily Achievement from the watchlist \n\tdisplay       Displays the list of Daily Achievements on the Watchlist \n\tnotify        Adds specific roles or yourself to be pinged for the Daily Watchlist\n```"

        else:

            response = "Here's a list of instructions that I can help with. If you wish to know more about a specific command, please use `%shelp [command-name]`" % (PREFIX)

            embed = "```\n%sdaily-alert [args...]\n\tSeries of commands used in managing daily alert notifications regarding Daily Achievements\n```" % (PREFIX)

        await ctx.send(response + embed)

