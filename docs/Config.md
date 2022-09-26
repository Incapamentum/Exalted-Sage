# Settings Structure

Currently, the application settings are handled via a `settings.json` file that is located in this directory. This file contains two objects: an `DevSettings` object and a `ProdSettings` object. Both are discussed below.

Sample view of the JSON file:

```json
{
    "DevSettings": {
        ...
    },
	"ProdSettings": {
        ...
    }
}
```

Both objects support similar mappings. The only differences are the values of the mappings:

```
{
	"DummySettings": {
		"Token": "Discord-client-token",
		"DatabaseName": "Name-of-database",
		"ConnectionUri": "URI-connection-string"
	}
}
```

