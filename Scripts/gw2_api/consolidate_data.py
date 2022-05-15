import json
import os
import sys

DATA_PATH = "data/"
DEFAULT_DAY = 7
LOG_PATH = "logs/"
NUM_FILES = 4

consolidated_data = []

# Housekeeping
if (not os.path.isdir(DATA_PATH)):
    os.mkdir(DATA_PATH)

if (not os.path.isdir(LOG_PATH)):
    print("No 'logs' directory to be found.")
    sys.exit(-1)

for i in range(NUM_FILES):

    daily = "daily_list-2020-8-%d.log" % (DEFAULT_DAY + i)

    with open(LOG_PATH + daily, "r") as data_file:

        while True:
            line = data_file.readline().strip()

            if not line:
                break

            if line not in consolidated_data:
                consolidated_data.append(line)

with open(DATA_PATH + "data.log", "w") as data_file:

    for line in consolidated_data:
        data_file.write(line + '\n')

print("Completed! Results can be viewed within the 'data' directory.")