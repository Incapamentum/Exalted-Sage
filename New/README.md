# Exalted Sage - C# Rewrite

Something about how Discord.py is no longer being actively maintained, being rewritten in a different language, yada yada

## Dependencies

List of dependencies the application makes use of

## Development Environment

Information like what IDE, version, etc

## Application Structure

Anytime a message is fired, it quickly parses it to then redirect it to the appropriate channel

- [ ] Bot should have two interfaces:
  - [ ] An admin interface
    - [ ] Is command-driven
  - [ ] A user interface
    - [ ] Is event-driven



Admin commands should manage different aspect of the bot, such as keeping track of which messages to listen for (maybe store message IDs in the database?) Among other things