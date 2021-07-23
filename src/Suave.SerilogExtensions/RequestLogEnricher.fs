namespace Suave.SerilogExtensions

open Suave
open Serilog.Core
open Serilog.Events
open System.Diagnostics

type RequestLogEnricher(context: HttpContext, config: SerilogConfig, requestId: string) =
    interface ILogEventEnricher with
        member this.Enrich(logEvent: LogEvent, _: ILogEventPropertyFactory) =
            let (Choser ignoredRequestFields) = config.IgnoredRequestFields
            let included field = not (List.exists ((=) ("Request." + field)) ignoredRequestFields)
            let anyOf xs = fun x -> List.exists ((=) x) xs

            requestId
            |> Enrichers.eventProperty "RequestId"
            |> logEvent.AddOrUpdateProperty

            Enrichers.eventProperty "Type" "Request"
            |> logEvent.AddOrUpdateProperty

            if included "Path" then
                context.request.path
                |> Enrichers.eventProperty "Path"
                |> logEvent.AddOrUpdateProperty

            if included "FullPath" then
                context.request.url.PathAndQuery
                |> Enrichers.eventProperty "FullPath"
                |> logEvent.AddOrUpdateProperty

            if included "Method" then
                context.request.method
                |> string
                |> Enrichers.eventProperty "Method"
                |> logEvent.AddOrUpdateProperty

            if included "Host" then
                context.request.host
                |> Enrichers.eventProperty "Host"
                |> logEvent.AddOrUpdateProperty

            if included "QueryString" then
                context.request.rawQuery
                |> Enrichers.eventProperty "QueryString"
                |> logEvent.AddOrUpdateProperty

            if included "Query" then
                context.request.query
                |> List.map (fun (prop, value) -> prop, Option.defaultValue "" value)
                |> Map.ofList
                |> Enrichers.eventProperty "Query"
                |> logEvent.AddOrUpdateProperty

            if included "UserIPAddress" then
                context.connection.ipAddr
                |> string
                |> Enrichers.eventProperty "UserIPAddress"
                |> logEvent.AddOrUpdateProperty

            if included "Headers" then
                context.request.headers
                |> Map.ofList
                |> Map.remove "host"
                |> Map.remove "user-agent"
                |> Enrichers.eventProperty "RequestHeaders"
                |> logEvent.AddOrUpdateProperty

            if included "UserAgent" then
                context.request.headers
                |> List.tryFind (fun (key, _) -> anyOf ["user-agent"; "useragent"] (key.ToLowerInvariant()))
                |> Option.map snd
                |> Option.iter (Enrichers.eventProperty "UserAgent" >> logEvent.AddOrUpdateProperty)

            if included "Body" then
                context.request.rawForm
                |> System.Text.Encoding.UTF8.GetString
                |> Enrichers.eventProperty "Body"
                |> logEvent.AddOrUpdateProperty

            if included "ContentType" then
                context.request.headers
                |> List.tryFind (fun (key, _) -> anyOf ["content-type"; "contenttype"] (key.ToLowerInvariant()))
                |> Option.map snd
                |> Option.iter (Enrichers.eventProperty "ContentType" >> logEvent.AddOrUpdateProperty)

            if included "ContentLength" then
                context.request.headers
                |> List.tryFind (fun (key, _) -> anyOf ["content-length"; "content-length"] (key.ToLowerInvariant()))
                |> Option.map snd
                |> Option.iter (Enrichers.eventProperty "ContentLength" >> logEvent.AddOrUpdateProperty)
