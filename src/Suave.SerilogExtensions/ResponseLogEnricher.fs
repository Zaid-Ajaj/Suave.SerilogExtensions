namespace Suave.SerilogExtensions

open System.Diagnostics
open Suave
open Serilog.Core
open Serilog.Events

type ResponseLogEnricher(context: HttpContext, config: SerilogConfig, stopwatch: Stopwatch, requestId: string) = 
    interface ILogEventEnricher with 
        member this.Enrich(logEvent: LogEvent, _: ILogEventPropertyFactory) = 
            let (Choser ignoredRequestFields) = config.IgnoredRequestFields
            let included field = not (List.exists ((=) ("Response." + field)) ignoredRequestFields)
            let anyOf xs = fun x -> List.exists ((=) x) xs 
            
            stopwatch.Stop()
            stopwatch.ElapsedMilliseconds
            |> int
            |> Enrichers.eventProperty "Duration"
            |> logEvent.AddOrUpdateProperty

            requestId
            |> Enrichers.eventProperty "RequestId"
            |> logEvent.AddOrUpdateProperty

            Enrichers.eventProperty "Type" "Response"
            |> logEvent.AddOrUpdateProperty            

            if included "Method" then 
                context.request.method
                |> string
                |> Enrichers.eventProperty "Method"
                |> logEvent.AddOrUpdateProperty

            if included "StatusCode" then 
                context.response.status.code
                |> Enrichers.eventProperty "StatusCode"
                |> logEvent.AddOrUpdateProperty

            if included "ReasonPhrase" then 
                context.response.status.reason
                |> Enrichers.eventProperty "ReasonPhrase"
                |> logEvent.AddOrUpdateProperty

            if included "ContentType" then 
                context.response.headers
                |> List.tryFind (fun (key, _) -> anyOf ["content-type"; "contenttype"] (key.ToLowerInvariant()))
                |> Option.map snd
                |> Option.iter (Enrichers.eventProperty "ContentType" >> logEvent.AddOrUpdateProperty)
            
            if included "FullPath" then 
                context.request.url.PathAndQuery
                |> Enrichers.eventProperty "FullPath"
                |> logEvent.AddOrUpdateProperty
            
            if included "ContentLength" then 
                context.response.headers
                |> List.tryFind (fun (key, _) -> anyOf ["content-length"; "content-length"] (key.ToLowerInvariant()))
                |> Option.map snd
                |> Option.iter (Enrichers.eventProperty "ContentLength" >> logEvent.AddOrUpdateProperty)            
            
            ()