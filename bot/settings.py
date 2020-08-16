import os

from dotenv import load_dotenv

load_dotenv()

PREFIX = "!"
COGS_PATH = "./cogs"
BOT_ID = os.getenv("BOT_ID")
# GUILDS_PATH = "data/guilds/"
DISPATCHER_PATH = "./data/dispatcher.json"
TOKEN = os.getenv("DISCORD_TOKEN")
# GUILDS_LIST = GUILDS_PATH + "guilds_list.json"
