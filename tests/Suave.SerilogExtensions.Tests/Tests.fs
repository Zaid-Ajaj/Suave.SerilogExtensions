module Tests

open Expecto
open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful
open Suave.Testing
open System.Linq
open System.Collections.Generic
open Serilog.Sinks.TestCorrelator
open Suave.SerilogExtensions
open Serilog
open Serilog.Formatting.Json

let app =
  choose [ path "/index" >=> OK "Index"
           path "/manyLogs" 
           >=> context (fun ctx ->
            let logger = ctx.Logger()
            logger.Information("Read my {RequestId}")
            OK "Many logs")

           OK "Not Found" ]

let appWithLogger = SerilogAdapter.Enable(app)

Log.Logger <-
  LoggerConfiguration()
     .WriteTo.TestCorrelator()
     .WriteTo.Console()
     .WriteTo.Console(JsonFormatter())
     .CreateLogger()

let pass() = Expect.isTrue true "Passed"
let fail() = Expect.isTrue false "Failed"

let rnd = System.Random()
let rndConfig() = { defaultConfig with bindings = [ HttpBinding.createSimple Protocol.HTTP "127.0.0.1" (rnd.Next(2000, 9000)) ] }

let run app = runWith (rndConfig()) app

[<Tests>]
let tests =
  testList "Serilog Extensions" [
    testCase "Logger works" <| fun () ->

      use context = TestCorrelator.CreateContext()

      run appWithLogger
      |> req HttpMethod.GET "/index?hello=there&included=true" None
      |> ignore

      TestCorrelator.GetLogEventsFromContextGuid(context.Guid)
      |> Seq.length
      |> fun count -> Expect.equal count 2 "There is two log events, one for request and one for response"

    testCase "Reading logger from context works" <| fun _ ->
      use context = TestCorrelator.CreateContext()
      
      run appWithLogger 
      |> req HttpMethod.GET "/manyLogs" None
      |> ignore 

      TestCorrelator.GetLogEventsFromContextGuid(context.Guid)
      |> Seq.map (fun logEvent -> (logEvent.Properties.Item "RequestId").ToString())
      |> Seq.length 
      |> fun count -> Expect.equal count 3 "There are three log event properties with RequestId"

    testCase "Ingoring request fields" <| fun _ ->
      let ignoredFields =
        Ignore.fromRequest
        |> Field.path
        |> Field.method
        |> Field.host
        |> fun (Choser fields) -> fields
        |> Array.ofList

      Expect.equal "Request.Path" ignoredFields.[2] "Path is correct"
      Expect.equal "Request.Method" ignoredFields.[1] "Method is correct"
      Expect.equal "Request.Host" ignoredFields.[0] "Host is correct"

  ]
