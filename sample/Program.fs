// Learn more about F# at http://fsharp.org

open System
open Suave
open Suave.SerilogExtensions
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Serilog
open Serilog.Formatting.Json

type Maybe<'a> =
    | Nothing
    | Just of 'a

type Rec = { First: string; Job: Maybe<string> }
let app = choose [
    GET >=> path "/index" >=> OK "Index"
    POST >=> path "/echo" >=> OK "Echo"
    GET >=> path "/destructure" >=> context (fun ctx ->
        let logger = ctx.Logger()
        let genericUnion =  Maybe.Just { First = "Zaid"; Job = Nothing }
        logger.Information("Generic Union with Record {@Union}", genericUnion)

        let result = Ok (Just "for now!")
        logger.Information("Result {@Value}", result)

        let simpleList = [1;2;3;4;5]
        logger.Information("Simple list {@List}", simpleList)

        let complexList = [ box (Just "this?"); box ({ First = "Zaid"; Job = Nothing }) ]
        logger.Information("Complex list {@List}", complexList)

        OK "Done"
    )

    GET >=> path "/internal" >=> context (fun ctx ->
        let logger = ctx.Logger()
        logger.Information("Using internal logger")
        OK "Internal"
    )

    GET >=> path "/fail" >=> context (fun ctx -> failwith "Fail miserably")
]

let simpleApp = choose [
    GET >=> path "/" >=> OK "Home"
    GET >=> path "/other" >=> OK "Other route"
]
let serilogConfig =
    { SerilogConfig.defaults
        with IgnoredRequestFields =
            Ignore.fromRequest
            |> Field.host
            |> Field.requestBody }
let appWithLogger = SerilogAdapter.Enable(app)

Log.Logger <-
  LoggerConfiguration()
    .Destructure.FSharpTypes()
    .WriteTo.Console()
    .CreateLogger()

startWebServer defaultConfig appWithLogger
