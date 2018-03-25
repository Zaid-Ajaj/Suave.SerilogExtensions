namespace Suave.SerilogExtensions

open System.Diagnostics
open Suave
open Serilog.Core
open Serilog.Events

type PassThroughLogEnricher(context: HttpContext) = 
    interface ILogEventEnricher with 
        member this.Enrich(logEvent: LogEvent, _: ILogEventPropertyFactory) = 
            let anyOf xs = fun x -> List.exists ((=) x) xs 

            context.userState
            |> Map.find "Stopwatch"
            |> unbox<Stopwatch> 
            |> fun stopwatch -> 
                stopwatch.Stop()
                stopwatch.ElapsedMilliseconds
                |> int
                |> Enrichers.eventProperty "Duration"
                |> logEvent.AddOrUpdateProperty

            context.userState
            |> Map.find "RequestId"
            |> unbox<string> 
            |> Enrichers.eventProperty "RequestId"
            |> logEvent.AddOrUpdateProperty            