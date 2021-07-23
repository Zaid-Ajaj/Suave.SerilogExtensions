namespace Suave.SerilogExtensions

open System.Diagnostics
open Suave
open Serilog.Core
open Serilog.Events

type ErrorLogEnricher(context: HttpContext, stopwatch: Stopwatch, requestId: string) = 
    interface ILogEventEnricher with 
        member this.Enrich(logEvent: LogEvent, _: ILogEventPropertyFactory) = 
            stopwatch.Stop()

            stopwatch.ElapsedMilliseconds
            |> Enrichers.eventProperty "Duration"
            |> logEvent.AddOrUpdateProperty

            requestId
            |> Enrichers.eventProperty "RequestId"
            |> logEvent.AddOrUpdateProperty    

            context.request.url.PathAndQuery
            |> Enrichers.eventProperty "FullPath"
            |> logEvent.AddOrUpdateProperty

            context.request.method
            |> string 
            |> Enrichers.eventProperty "Method"
            |> logEvent.AddOrUpdateProperty

            Enrichers.eventProperty "StatusCode" 500
            |> logEvent.AddOrUpdateProperty

            Enrichers.eventProperty "Reason" "Internal Server Error"
            |> logEvent.AddOrUpdateProperty

            Enrichers.eventProperty "Type" "ServerError"
            |> logEvent.AddOrUpdateProperty
