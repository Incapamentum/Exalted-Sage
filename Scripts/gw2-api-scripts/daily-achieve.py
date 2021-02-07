# This script aggregates all of the Daily achievements
# into a daily_achievements.json file

import json
import time

from requests_futures.sessions import FuturesSession

session = FuturesSession()

request = session.get("https://api.guildwars2.com/v2/achievements")
request_result = request.result()

achieve_list = json.loads(request_result.text)

daily_achieve = {}

count = 0

print(achieve_list[3304])

for achieve in achieve_list:

    time.sleep(0.5)
    print("I'm still running! Count: %d" % (count + 1))
    count += 1

    request = session.get("https://api.guildwars2.com/v2/achievements/" + str(achieve))
    request_result = request.result()

    result = json.loads(request_result.text)

    item_keys = result.keys()

    if (('id' in item_keys) and ('name' in item_keys)):

        name_split = result['name'].split()

        if (name_split[0] == 'Daily'):
            daily_achieve[result['id']] = result['name']

with open("data/daily_achievements.json", "w") as daily_file:
    json.dump(daily_achieve, daily_file)