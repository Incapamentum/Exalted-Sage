# Settings Structure

Currently, the application settings are handled via a `settings.json` file that is located in this directory. This file contains two objects: an `AppSettings` object and a `Commands` object. Both are discussed below.

Sample view of the JSON file:

```json
{
    "AppSettings": {
        ...
    },
	"Commands": {
        ...
    }
}
```



## AppSettings

This object contains some basic settings used in the configuration of the app, such as the secret token for both the Discord and MongoDB clients. In addition, it also supports a list of guilds that the bot is approved to interact in.

Sample view of the JSON object:

```json
"AppSettings": {
    "Token": "token-value",
    "Guilds": {
        "Guild-1": ulong-ID,
        "Guild-2": ulong-ID
    }
}
```



## Commands

This JSON object comes with only one key associated with a list of objects. This is still a WiP, but the basic idea is to allow for command extensibility through JSON objects that describe different aspects of the command. What it does _not_ do is handle the functionality of the command. That is a different process.

Sample view of the JSON object:

```json
"Commands": {
    "CommandsList": [
        {
            "Name": "command-one",
            "Description": "Sample description of command one",
            "Options":
        }
    ]
}
```

At the moment, the `"Options"` key is still under a WiP, as the command may support different options and may be necessary to have it support a list of objects as well.