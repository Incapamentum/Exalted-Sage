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

fileName = "daily_list-%d-%d-%d.log" % (yyyy, mm, dd)
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

for entry in dailyPVE:

    if entry['id'] not in id_List:
        id_List.append(entry['id'])

fp = open(FILE_PATH, "w")

for ID in id_List:

    request = session.get("https://api.guildwars2.com/v2/achievements/" + str(ID))
    request_result = request.result()

    result = json.loads(request_result.text)

    entry = "ID: %d, Name: %s\n" % (ID, result['name'])

    fp.write(entry)

fp.close()
print("List of today's dailies have been generated and saved within 'logs' subdirectory.")