import os

from dotenv import load_dotenv

load_dotenv()

PREFIX = "!"

DISPATCHER = ["daily_alert", "help", "manage"]

MONGO_CONNECT = os.getenv("MONGO_URI")
TOKEN = os.getenv("DISCORD_TOKEN")
BROADCAST_CHANNEL = os.getenv("MAIN_CHANNEL_ID")
# BOT_ID = os.getenv("BOT_ID")
COGS_PATH = "./cogs"
