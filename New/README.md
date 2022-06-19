# Exalted Sage - C# Rewrite

Something about how Discord.py is no longer being actively maintained, being rewritten in a different language, yada yada

## Dependencies

The following is a complete list of the dependencies the application makes use of, along with the version of it. No testing has been done in checking at which versions the application breaks.

- Discord.Net v3.4.0
- Json.Net v1.0.33
- MongoDB.Driver v2.14.1
- Microsoft.Extensions.Configuration v6.0.0
- Microsoft.Extensions.Configuration.Binder v6.0.0
- Microsoft.Extensions.Configuration.EnvironmentVariables v6.0.1
- Microsoft.Extensions.Configuration.Json v6.0.0

## Development Environment

The application was developed with the following IDEs and frameworks:

- Microsoft .NET Framework, v4.8
- Microsoft Visual Studio Community 2019, v16.11
  - C# Tools v3.11

## Application Structure

Explains how the application is structured.

### Handlers & Services

The application makes use of `Handlers` and `Services` (defined below) to function.

**Handlers** - responds to specific internal requests and executes logic, processing any data it receives

**Services** - provides actions that have an effect on the state of the application; forms part of an operational process

### Interfacing

Although not explicit in the application structure, the bot supports an admin-facing interface, which is command-driven, and a user-facing interface, which is event-driven. This is to allow for separation of different functionality and to prevent any misuse from users.