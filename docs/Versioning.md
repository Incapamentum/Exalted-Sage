# Versioning

This project has undergone major changes in the past. Originally, the project was written in Python, using `Discord.py`. When news broke out that it would no longer be maintained, an alternative was sought for. In addition, the use of Python was detrimental due to it being a dynamic language.

The second revision of the bot came along in the form of C#, chosen due to the .NET runtime environment and development supported via Visual Studio. This ensured that development was the same across the board. However, an issue in the form of the target framework lifetime cycle came along. For compatibility reasons with a VPS service, the bot targeted the .NET 5.0 framework, but this soon reached EOL. Therefore, the bot was later changed to target .NET 6.0. Soon after was when the three-year LTS for even-numbered releases was encountered, with the shorter support for odd-numbered releases. Assuming that the different release versions are fundamentally different, it has become necessary to now version the bot.

## Semantic Versioning

Semantic versioning is a system that numbers software releases. It comes in the form of **MAJOR.MINOR.PATCH**, with additional labels for pre-release and build metadata being available as extensions to the format.

According to the [official standard](https://semver.org/), for this versioning system to work, a public API must first be declared. In the context of the bot, this can easily be dictated by the frontend interface of it to the server, and to a more precise extent the version of [Discord.NET](https://discordnet.dev/). Once this is identified, changes are communicated to it with specific increments to the version number.

For example: bug fixes that don't affect the API increment the patch version. Backwards compatible API additions/changes increment the minor version, and this can be specific to any new functionality that's introduced to the bot. Any change to the frontend interface, or the major update of Discord.NET, the major version is incremented.

### Versioning Guidelines

1. **MAJOR** version when incompatible API changes are made
2. **MINOR** version when functionality is added in a backwards compatible manner
3. **PATCH** version when backwards compatible bug fixes are made.

As previously mentioned, additional labels for pre-release and build metadata are available as extensions to the **MAJOR.MINOR.PATCH** format.

## Assemblies

Versioning in C# can be done in a couple of ways, but all fundamentally depend on .NET assemblies. According to an [official doc](https://learn.microsoft.com/en-us/dotnet/standard/assembly/), "assemblies are the fundamental units of deployment, version control, reuse, activation scoping, and security permissions for .NET-based applications."

In general, assemblies have the following properties:

- Implemented as *.exe* or *.dll* files
- For libraries that target the .NET Framework, assemblies can be shared between applications by putting them in the [global assembly cache](https://learn.microsoft.com/en-us/dotnet/framework/app-domains/gac). Assemblies must be [strong-named](https://learn.microsoft.com/en-us/dotnet/standard/assembly/strong-named) before they can be included.
- Assemblies are only loaded into memory if they're required. If they aren't used, they aren't loaded. Therefore, assemblies can be an efficient way to manage resources in larger projects.
- Information about an assembly can be programmatically obtained by using [reflection](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/reflection).
- An assembly can be loaded for inspection by using the [MetadataLoadContext](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.metadataloadcontext) class on .NET and .NET Framework.

### AssemblyInfo.cs

The project contains an `AssemblyInfo.cs` file, which provides two attributes to set two different types of versions.

The first is the `AssemblyVersion` attribute, which is the version number used by the framework during build and at runtime to locate, link, and load assemblies. When a reference is added to any assembly in a project, it's this version number that gets embedded.

The other is the `AssemblyFileVersion`, and this is strictly a version number given to a file in the file system. it's displayed by Windows Explorer, and never used by the .NET framework or runtime for referencing.

```c#
// Version information for an assembly consists of the following four values:
// Major Version
// Minor Version
// Build Number
// Revision
[assembly: AssemblyVersion("1.0.0.0")]  
[assembly: AssemblyFileVersion("1.0.0.0")]
```

Providing a (*) in place of absolute values makes the compiler increase the number by one every time the project is built.

To access the info written in the `AssemblyInfo.cs` file at runtime, use `System.Reflection.Assembly`. Use `System.Reflection.Assembly.GetExecutingAssembly()` to get the assembly **from where this line of code is in**. Alternatively, use `System.Reflection.Assembly.GetEntryAssembly()` to get the assembly the project started with.

Below is a basic example:

```C#
System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
var fieVersionInfo = FileVersionInfo.GetVersionInfo(executingAssembly .Location);
var version = fieVersionInfo.FileVersion;
```

