# Suave.SerilogExtensions [![Build Status](https://travis-ci.org/Zaid-Ajaj/Suave.SerilogExtensions.svg?branch=master)](https://travis-ci.org/Zaid-Ajaj/Suave.SerilogExtensions) [![Nuget](https://img.shields.io/nuget/v/Suave.SerilogExtensions.svg?colorB=green)](https://www.nuget.org/packages/Suave.SerilogExtensions)

[Suave](https://github.com/SuaveIO/suave) plugin to use [Serilog](https://github.com/serilog/serilog) as the logger for your application

## Builds

![Build History](https://buildstats.info/travisci/chart/Zaid-Ajaj/Suave.SerilogExtensions)


### Building


Make sure the following **requirements** are installed in your system:

* [dotnet SDK](https://www.microsoft.com/net/download/core) 2.0 or higher
* [Mono](http://www.mono-project.com/) if you're on Linux or macOS.

```
> build.cmd // on windows
$ ./build.sh  // on unix
```

### Watch Tests

The `WatchTests` target will use [dotnet-watch](https://github.com/aspnet/Docs/blob/master/aspnetcore/tutorials/dotnet-watch.md) to watch for changes in your lib or tests and re-run your tests on all `TargetFrameworks`

```
./build.sh WatchTests
```