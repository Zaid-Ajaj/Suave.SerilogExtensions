// Learn more about F# at http://fsharp.org

open System
open Suave 
open Suave.SerilogExtensions
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Serilog
open Serilog.Formatting.Json

let app = 
  choose [ GET >=> path "/index" >=> OK "Index"
           POST >=> path "/echo" >=> OK "Echo" 
           GET >=> path "/internal" 
               >=> context (fun ctx ->
                     let logger = ctx.Logger()
                     logger.Information("Using internal logger")
                     OK "Internal") 
           GET >=> path "/fail" >=> context (fun ctx -> failwith "Fail miserably") ]

let serilogConfig = 
  { SerilogConfig.defaults 
      with IgnoredRequestFields = 
             Ignore.fromRequest
             |> Field.host
             |> Field.requestBody }
let appWithLogger = SerilogAdapter.Enable(app)

Log.Logger <- 
  LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Console(JsonFormatter())
    .CreateLogger()

startWebServer defaultConfig appWithLogger