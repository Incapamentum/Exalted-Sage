## Table of Contents

- [Developing Exalted Sage](#developing-exalted-sage)
  - [Development Environment and Toolchains](#development-environment-and-toolchains)
  - [Resource Settings](#resource-settings)
- [Logging Practices & Guidelines](#logging-practices-&-guidelines)

## Developing Exalted Sage

The new project is written entirely in C#, as it can execute on any machine with a .NET runtime (similar to how Java first gets compiled to bytecode and executed in the JVM). The language is statically- and strongly-typed, with support for type inference, and making use of object-oriented programming and generics. There is also a vast amount of third-party support through the NuGet package manager, which extends its overall functionality.

### Development Environment and Toolchains

Due to the choice of C#, all development has so far been done in [Visual Studio](https://visualstudio.microsoft.com/). Although not a requirement, it is helpful to understand the relationship between [solutions and projects](https://learn.microsoft.com/en-us/visualstudio/ide/solutions-and-projects-in-visual-studio?view=vs-2022). The use of Visual Studio is beneficial as it manages the setup of the entirety of the project through the `.csproj` file that exists at the root.

At the time of writing this document, Visual Studio Community 2022 is being used. During its installation, there will be a number of different options to choose from, as the IDE supports development in multiple languages and target development. Ensure the `.NET desktop development` option is checked, which can be found under the `Desktop & Mobile` workloads. On the right-side to the different install options, there are additional ones as well. Ensure that `.NET Framework 4.8 development tools` is checked as well. At the top of the installer should be a tab for `Individual components`. Clicking that will take you to a list with an expansive list of different options. Here, ensure that `.NET 6.0 Runtime (Long Term Support)` is checked, along with the `.NET SDK`. Now, go ahead and install.

Once installation has completed, launch Visual Studio. In the main screen of the IDE, click on `Help -> About Microsoft Visual Studio`. Clicking this will open up a new window, which lists some information. Now, confirm that these values are what's expected for the specified tool:

- Microsoft .NET Framework, v4.8
- Microsoft Visual Studio Community 2022, v17.4.x
  - C# Tools v4.4.x

Once the above is confirmed, there is but one more step before you can begin development.

### Resource Settings

As this project is a Discord bot, it requires a token. One can be generated for development use by going to the [Discord Developer Portal](https://discord.com/developers/applications) and creating a new application. Currently, I have not been able to find an official guide written by Discord, but if you were to Google a guide on how to do so, you'd be able to find one.

Generating the token is but half of what needs to be done. The second half involves setting up a local [MongoDB server](https://www.mongodb.com/). Like with the above, looking up a guide on how to do this is perhaps the best approach. After setting it up, what will be needed are two things: the name of the database, and the connection URI. These should be available to you through the app client of the local server.

Now, how does the bot make use of the above information? Well, they're saved in a `settings.json` file that's found in the `Config/` directory. The format should looks a bit like this:

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

As a contributor, you may not have access to the production resource settings, so for the time being focus on the `DevSettings`. All values should be strings, as such:

```json
{
    "DummySettings": {
        "Token": "sK5aMMgI8A6WZQi7M3gf",
        "DatabaseName": "Name-of-database",
        "ConnectionUri": "//localhost/:2002"
    }
}
```

Please note that the above are ***strictly*** sample/dummy values, and ***NOT*** actual values. **DO NOT USE THESE DURING DEVELOPMENT!**

## Logging Practices & Guidelines

As the bot is event-driven, such as processing any incoming messages or VC state changes, it is helpful to be able to keep track of these for the purposes of debugging. However, a question may arise as to what constitutes a good log? Should that even be logged? This section serves as a way to help answer some of these questions following these guidelines:

- Is the log needed? Does it relay important info that couldn't otherwise be received from other logs?
- Will an object be logged? Is the object in question huge? What aspects of the object should be logged?
- Will the log help debug and provide understanding to the state and flow of the bot?

### Log Levels

When writing a log, please note the following log levels, which are useful in knowing the type of information it's attempting to provide:

- **ERROR** - reserved to points of failures, where execution is halted
- **WARNING** - not necessarily a point of failure, but rather unexpected behavior came up
- **INFO** - should record major events in flow and state that help in understanding what's being executed
- **DEBUG** - similar in nature to INFO, but includes detailed information about the state of objects and contents of data structures

### Log Frugality

Don't go overboard with the information. Only log what should be of utmost important. As an example: don't actually log a huge object. Keep it super simple:

- Pick the most important and useful attributes, such as those necessary in flow execution
- Often, errors occur due to something being NULL

### Log Uniqueness

Each log being produced should be unique. Having to sift through multiple similar logs is useless when an error occurs.

The following is a good rule-of-thumbs to keep logs unique:

- Denote the service/function name as a prefix