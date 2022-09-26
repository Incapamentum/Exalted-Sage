# Database

## Collections

- achievements
  - Doc(s) consist of a (ID, string) mapping of achievements.
- categories
  - Doc(s) consist of three main data points: the ID of the server category, a `Text` object containing (string, ID) mappings of text channels, and a `Voice` object containing (string, ID) mappings of voice channels. For some categories, the text/voice channels may be empty.
- channels
  - This category is being considered to be deleted.
- responses
  - Contains a list of different responses that are categorized by doc. Doc(s) consist of an array of strings.
- roles
  - Currently WiP
- watchlists (this might have to be renamed to be more general)
  - Doc(s) consist of an array of strings containing names of items that are being tracked.