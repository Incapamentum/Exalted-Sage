import os

from dotenv import load_dotenv

load_dotenv()

PREFIX = "!"

DISPATCHER = ["daily_alert", "help"]

MONGO_CONNECT = os.getenv("MONGO_URI")
TOKEN = os.getenv("DISCORD_TOKEN")
# BOT_ID = os.getenv("BOT_ID")
