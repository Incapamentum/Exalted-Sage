"""
    Author: Incapamentum

    File name: gen_daily_list.py

    Generates a list of dailies at the time
    of script execution.

    The list of daily achievements changes
    upon server reset, which occurs at
    00:00 UTC.
"""

import json
import os

from datetime import datetime
from requests_futures.sessions import FuturesSession

LOG_PATH = "logs/"

# Setting up for logging purposes
utc = datetime.utcnow()
yyyy = utc.year
mm = utc.month
dd = utc.day

# fileName = "daily_list-%d-%d-%d.log" % (yyyy, mm, dd)
fileName = f'daily_list-{yyyy}-{mm}-{dd}.log'
FILE_PATH = LOG_PATH + fileName

# Checking for existence of directory
if (not os.path.isdir(LOG_PATH)):
    os.mkdir(LOG_PATH)

session = FuturesSession()

request = session.get("https://api.guildwars2.com/v2/achievements/daily")
request_result = request.result()

daily = json.loads(request_result.text)
dailyPVE = daily['pve']

id_List = []

# Collecting the IDs of today's daily achievements
for entry in dailyPVE:
    id_List.append(entry['id'])

fp = open(FILE_PATH, "w")

# Grabbing name of the achievement associated with the ID, then
# creating string connecting both to be written to a log file
for ID in id_List:

    request = session.get("https://api.guildwars2.com/v2/achievements/" + str(ID))
    request_result = request.result()

    result = json.loads(request_result.text)

    entry = f'ID: {ID}, Name: {result["name"]}\n'

    fp.write(entry)

fp.close()
print("List of today's dailies have been generated and saved within 'logs' subdirectory.")