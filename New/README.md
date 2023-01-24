# Exalted Sage - C# Rewrite

Something about how Discord.py is no longer being actively maintained, being rewritten in a different language, yada yada

## Table of Contents

- [Dependencies](#dependencies)
- [Development Environment](#development-environment)
- [Application Structure](#application-structure)

## Dependencies

The following is a complete list of the dependencies the application makes use of, along with the version of it. No testing has been done in checking at which versions the application breaks.

- Discord.Net v3.9.0
- Json.Net v1.0.33
- Microsoft.Extensions.Configuration v7.0.0
- Microsoft.Extensions.Configuration.Binder v7.0.2
- Microsoft.Extensions.Configuration.EnvironmentVariables v7.0.0
- Microsoft.Extensions.Configuration.Json v7.0.0
- MongoDB.Driver v2.18.0
- NLog v5.1.1

In cases where this document isn't frequently updated, the most up-to-date information can always be found in the C# project file found in the root of the `bot` project.

## Application Structure

Explains how the application is structured.

### Handlers & Services

The application makes use of `Handlers` and `Services` (defined below) to function.

**Handlers** - responds to specific internal requests and executes logic, processing any data it receives

**Services** - provides actions that have an effect on the state of the application; forms part of an operational process

### Interfacing

Although not explicit in the application structure, the bot supports an admin-facing interface, which is command-driven, and a user-facing interface, which is event-driven. This is to allow for separation of different functionality and to prevent any misuse from users.