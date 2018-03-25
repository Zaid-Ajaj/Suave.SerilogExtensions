# Suave.SerilogExtensions

[Suave](https://github.com/SuaveIO/suave) plugin to use [Serilog](https://github.com/serilog/serilog) as the logger for your application

---

## Builds

MacOS/Linux | Windows
--- | ---
[![Travis Badge](https://travis-ci.org/Zaid-Ajaj/Suave.SerilogExtensions.svg?branch=master)](https://travis-ci.org/Zaid-Ajaj/Suave.SerilogExtensions) | [![Build status](https://ci.appveyor.com/api/projects/status/github/Zaid-Ajaj/Suave.SerilogExtensions?svg=true)](https://ci.appveyor.com/project/Zaid-Ajaj/Suave.SerilogExtensions)
[![Build History](https://buildstats.info/travisci/chart/Zaid-Ajaj/Suave.SerilogExtensions)](https://travis-ci.org/Zaid-Ajaj/Suave.SerilogExtensions/builds) | [![Build History](https://buildstats.info/appveyor/chart/Zaid-Ajaj/Suave.SerilogExtensions)](https://ci.appveyor.com/project/Zaid-Ajaj/Suave.SerilogExtensions)  


## Nuget 

Stable | Prerelease
--- | ---
[![NuGet Badge](https://buildstats.info/nuget/Suave.SerilogExtensions)](https://www.nuget.org/packages/Suave.SerilogExtensions/) | [![NuGet Badge](https://buildstats.info/nuget/Suave.SerilogExtensions?includePreReleases=true)](https://www.nuget.org/packages/Suave.SerilogExtensions/)

---

### Building


Make sure the following **requirements** are installed in your system:

* [dotnet SDK](https://www.microsoft.com/net/download/core) 2.0 or higher
* [Mono](http://www.mono-project.com/) if you're on Linux or macOS.

```
> build.cmd // on windows
$ ./build.sh  // on unix
```

#### Environment Variables

* `CONFIGURATION` will set the [configuration](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build?tabs=netcore2x#options) of the dotnet commands.  If not set it will default to Release.
  * `CONFIGURATION=Debug ./build.sh` will result in things like `dotnet build -c Debug`
* `GITHUB_TOKEN` will be used to upload release notes and nuget packages to github.
  * Be sure to set this before releasing

### Watch Tests

The `WatchTests` target will use [dotnet-watch](https://github.com/aspnet/Docs/blob/master/aspnetcore/tutorials/dotnet-watch.md) to watch for changes in your lib or tests and re-run your tests on all `TargetFrameworks`

```
./build.sh WatchTests
```

* [Add your nuget API key to paket](https://fsprojects.github.io/Paket/paket-config.html#Adding-a-NuGet-API-key)

```
paket config add-token "https://www.nuget.org" 4003d786-cc37-4004-bfdf-c4f3e8ef9b3a
```

* [Create a GitHub OAuth Token](https://help.github.com/articles/creating-a-personal-access-token-for-the-command-line/)
    * You can then set the `GITHUB_TOKEN` to upload release notes and artifacts to github
    * Otherwise it will fallback to username/password


* Then update the `RELEASE_NOTES.md` with a new version, date, and release notes [ReleaseNotesHelper](https://fsharp.github.io/FAKE/apidocs/fake-releasenoteshelper.html)

* You can then use the `Release` target.  This will:
    * make a commit bumping the version:  `Bump version to 0.2.0` and add the release notes to the commit
    * publish the package to nuget
    * push a git tag
```
./build.sh Release
```
