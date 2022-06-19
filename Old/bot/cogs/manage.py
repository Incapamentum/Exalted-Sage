import discord

from discord.ext import commands

def setup(bot):
    """
        Add this cog to the bot
    """

    bot.add_cog(ManageCog(bot))

class ManageCog(commands.Cog):
    """
    """

    @commands.command(name="generate-channel-ids", pass_context = True)
    async def generate_channel_ids(self, ctx):
        """
            Only used to figure out some channels
        """

        creator = "Goose"

        if (ctx.author.name == creator):

            guild = ctx.message.guild
            print(guild.channels)