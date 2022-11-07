import os
import pysftp

# Grabbing environment vars
SFTP_HOST = os.getenv("host")
SFTP_USER = os.getenv("username")
SFTP_PASS = os.getenv("password")

cnopts = pysftp.CnOpts()
cnopts.hostkeys = None

# DEBUG
print(f'Current directory: {os.getcwd()}')

with pysftp.Connection(host=SFTP_HOST, username=SFTP_USER, password=SFTP_PASS, port=23, cnopts=cnopts) as conn:

    # Upload build files (will have to be changed later)
    conn.put_d("build/", "/")

