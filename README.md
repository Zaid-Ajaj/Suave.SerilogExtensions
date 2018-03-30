# Suave.SerilogExtensions [![Build Status](https://travis-ci.org/Zaid-Ajaj/Suave.SerilogExtensions.svg?branch=master)](https://travis-ci.org/Zaid-Ajaj/Suave.SerilogExtensions) [![Nuget](https://img.shields.io/nuget/v/Suave.SerilogExtensions.svg?colorB=green)](https://www.nuget.org/packages/Suave.SerilogExtensions)

[Suave](https://github.com/SuaveIO/suave) plugin to use the awesome [Serilog](https://github.com/serilog/serilog) library as the logger for your application

### Install
Install from Nuget:
```bash
# using nuget client
dotnet add package Suave.SerilogExtensions
# using Paket
mono .paket/paket.exe add Suave.SerilogExtensions --project path/to/Your.fsproj
```

### Usage
Wrap an existing `WebPart` with `SerilogAdapter.Enable`:
```fs
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.SerilogExtensions
open Serilog 

let webApp = GET >=> path "/" >=> OK "Home"

// webAppWithLogging : WebPart
let webAppWithLogging = SerilogAdapter.Enable(webApp)

// Configure serilog 
Log.Logger <- 
  LoggerConfiguration()
    .Destructure.FSharpTypes()
    .WriteTo.Console() // from Serilog.Sinks.Console
    .CreateLogger() 

startWebServer defaultConfig webAppWithLogging
```
Now `dotnet run` and `curl http://localhost:8080` to get the following logs:
```fs
[20:33:37 INF] Smooth! Suave listener started in 185.966 with binding 127.0.0.1:8080
[20:35:42 INF] GET Request at /
[20:35:42 INF] GET Response (StatusCode 200) at / took 121 ms
```
These request and response log events contain many properties that are extracted from the `HttpContext`, enable a detailed console sink to see this in action:
```fs
open Serilog.Formatting.Json
(*
  ...
*)
Log.Logger <- 
  LoggerConfiguration()
    .Destructure.FSharpTypes()
    .WriteTo.Console() // from Serilog.Sinks.Console
    .WriteTo.Console(JsonFormatter())
    .CreateLogger() 
``` 
Now there logs become:
```fs
[20:39:48 INF] Smooth! Suave listener started in 146.37 with binding 127.0.0.1:8080
[20:39:54 INF] GET Request at /
{"Timestamp":"2018-03-25T20:39:54.4942292+02:00","Level":"Information","MessageTemplate":"{Method} Request at {FullPath}","Properties":{"RequestId":"9817639f-fcc6-45c5-9c41-593f707a0649","Type":"Request","Path":"/","FullPath":"/","Method":"GET","Host":"localhost","QueryString":"","Query":{},"UserIPAddress":"127.0.0.1","RequestHeaders":{"accept":"*/*"},"UserAgent":"curl/7.55.1","Body":""}}
[20:39:54 INF] GET Response (StatusCode 200) at / took 140 ms
{"Timestamp":"2018-03-25T20:39:54.6245586+02:00","Level":"Information","MessageTemplate":"{Method} Response (StatusCode {StatusCode}) at {FullPath} took {Duration} ms","Properties":{"Duration":140,"RequestId":"9817639f-fcc6-45c5-9c41-593f707a0649","Type":"Response","Method":"GET","StatusCode":200,"ReasonPhrase":"OK","FullPath":"/"}}
```
Logs from the same roundtrip will include a `RequestId` property that is the same for these logs to trace them back using your favorite log server. 

### Use the logger from inside the WebPart
You can get a reference for a logger with the `RequestId` attached to it from inside a `WebPart`:
```fs
let webApp = 
  choose [ GET >=> path "/" >=> OK "Home"
           GET >=> path "/index" 
               >=> context (fun ctx ->
                     // get the contextual logger
                     let logger = ctx.Logger() 
                     logger.Information("Read my {RequestId}")
                     OK "Some response") ]
```
the `Logger()` method is an extension method to `HttpContext`. 

### Ignore log fields
As you can see, there many fields being logged from the request and response. You can configure the logger to ignore some fields:
```fs
let serilogConfig = 
  { SerilogConfig.defaults with
      IgnoredRequestFields = 
        Ignore.fromRequest
        |> Field.host
        |> Field.userAgent
        |> Field.queryString
      IgnoredResponseFields = 
        Ignore.fromResponse
        |> Field.contentType
        |> Field.reasonPhrase }

let webAppWithLogging = SerilogAdapter.Enable(webApp, serilogConfig)
```
### Error Handling
Error handling within the Serilog `WebPart` is also handled by Serilog and not Suave's internal logger. The error handler is of type: `Exception -> HttpContext -> WebPart` with the default handler returning a generic error message from the server:
```fs
let errorHandler = 
 fun ex httpContext -> 
    OK "Internal Server Error"
    >=> Writers.setStatus HttpCode.HTTP_500
```
You can override this error handler from the config:
```fs
let serilogConfig = 
 { SerilogConfig.defaults with 
    ErrorHandler = 
      fun ex httpContext -> 
        // NancyFx-style apologetic message :D
        OK "Sorry, something went terribly wrong!"
        >=> Writers.setStatus HttpCode.HTTP_500 }

let webAppWithLogging = SerilogAdapter.Enable(webApp, serilogConfig)
```

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