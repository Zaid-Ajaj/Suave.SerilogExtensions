namespace Suave.SerilogExtensions

open System.Diagnostics
open Suave
open Serilog.Core
open Serilog.Events

type PassThroughLogEnricher(context: HttpContext) = 
    interface ILogEventEnricher with 
        member this.Enrich(logEvent: LogEvent, _: ILogEventPropertyFactory) = 
            let (stopwatchExists, stopwatchValue) = context.userState.TryGetValue "Stopwatch"
            if stopwatchExists then
                stopwatchValue
                |> unbox<Stopwatch> 
                |> fun stopwatch -> 
                    stopwatch.Stop()
                    stopwatch.ElapsedMilliseconds
                    |> Enrichers.eventProperty "Duration"
                    |> logEvent.AddOrUpdateProperty

            let (hasRequestId, requestId) = context.userState.TryGetValue "RequestId"
            if hasRequestId then
                requestId
                |> unbox<string> 
                |> Enrichers.eventProperty "RequestId"
                |> logEvent.AddOrUpdateProperty
