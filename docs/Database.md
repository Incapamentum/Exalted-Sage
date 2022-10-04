# Database

Description of the collections found in the database. The structure of the docs found in each category is also briefly explained.

## Collections

Below are descriptions of the different collections found in the database.

### achievemenets

The docs found in this collection follow a schema pattern as shown in [this model](/New/bot/Models/AchievementDoc.cs). The primary feature of these docs is a (ID, achievement name) mapping, represented as a `Dictionary<ulong, string>` in the model. The primary purpose of such docs are to filter out information obtained from performing a HTTP GET request at the API for daily achievements (please double-check this information).

```json
{
    "Title": "Doc-Name",
    "Data": "Creation-Date",
    "Achievements": {
        1234: "Achievement 1",
        4321: "Achievmenet 2",
        ...
    }
}
```

### categories

This is a relatively new collection, pursued as an inclusive solution in attempting to modularize certain aspects of code. The docs found in this collection follow a schema pattern as shown in [this model](/New/bot/Models/CategoryDoc.cs). The primary features of this doc are as follows: the ID of the category, represented as a `ulong` in the model, and two (name, ID) mappings, represented as a `Dictionary<string, ulong>` in the model. One mapping is for text channels, and the other is for voice channels.

The purpose of pursuing it in such a way is to modularize method calls by category origin as opposed to having multiple docs in a collection representing different groupings of channels without any kind of association to the server category it belongs to.

```json
{
    "Title": "Category-Name",
    "CatId": 1234567890,
    "Text": {
        "Text-Channel-1": 9876543210,
        ...
    },
    "Voice": {
        "Voice-Channel-1": 4567891230,
        ...
    }
}
```

### channels \[DEPRECATED\]

Currently this collection can be considered as deprecated and will eventually be removed in a future build.

The docs found in this collection follow a schema pattern as shown in [this model](/New/bot/Models/ChannelsDoc.cs). The primary feature of these docs is a (channel name, ID) mapping, represented as a `Dictionary<string, ulong>` in the model.

This approach may be a more frustrating approach than pursuing a categorical grouping due to the channels not being directly related to one another outside of purpose, and may cause overall issues in pursuing modularization due to it.

```json
{
    "Title": "Grouped Channels",
    "Channels": {
        "channel-name-1": 1234567890,
        ...
    }
}
```

### responses

The docs found in this collection follow a schema pattern as shown in [this model](/New/bot/Models/ResponseDoc.cs). The primary feature of these docs is a list of strings, represented as a `string[]` in the model.

```json
{
    "Title": "Responses 1",
    "Data": "Created-Date",
    "Responses": [
        "Response 1",
        "Response 2",
        ...
    ]
}
```

### roles

The docs found in this collection currently do not have a schema pattern shown in any model source file. This is due to the feature not currently being used.

The future intent on this may be to group roles based on perceived categories, i.e. if it's a role to be pinged, if it's a guild member role, etc.

```json
{
    "Title": "Sample Role IDs",
    "Date": "Created-Date",
    "Roles": {
        "Role 1": 1234567890,
        "Role 2": 9876543210,
        ...
    }
}
```

### watchlists

The docs found in this collection follow a schema pattern as shown in [this model](/New/bot/Models/WatchlistDoc.cs). The primary feature of this doc is a list of strings, represented as a `string[]` in the model.

The purpose of this collection is to maintain different lists meant to be watched for. For example, one doc found in this collection is a watchlist for daily achievements. It is used as a way of filtering through the daily achievements obtained from an HTTP GET request to the official GW2 API.

```json
{
    "Title": "Watchlist-1",
    "Watchlist": [
        "Watch Item 1",
        "Watch Item 2",
        ...
    ]
}
```