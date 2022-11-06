import os
import pysftp

from dotenv import load_dotenv

load_dotenv()

# Grabbing environment vars
SFTP_HOST = os.getenv("SFTP_HOST")
SFTP_USER = os.getenv("SFTP_USER")
SFTP_PASS = os.getenv("SFTP_PASS")

cnopts = pysftp.CnOpts()
cnopts.hostkeys = None

with pysftp.Connection(host=SFTP_HOST, username=SFTP_USER, password=SFTP_PASS, port=23, cnopts=cnopts) as conn:

    # Upload build files (will have to be changed later)
    conn.put_d("C:/Workspaces/Visual-Studio/Exalted-Sage/New/bot/bin/x64\Release/net5.0", "./")

