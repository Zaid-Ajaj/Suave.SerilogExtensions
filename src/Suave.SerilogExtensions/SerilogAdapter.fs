namespace Suave.SerilogExtensions

open System
open System.Diagnostics
open Suave
open Serilog

[<AutoOpen>]
module Extensions = 
    type HttpContext with 
        /// Returns a logger with the RequestId attached to it, if present.
        member ctx.Logger() : ILogger = 
            ctx.userState
            |> Map.tryFind "RequestId"
            |> function 
                | None -> Log.Logger
                | Some requestId -> Log.ForContext("RequestId", requestId)   

type SerilogAdapter() = 
    static member Enable(app: WebPart, config: SerilogConfig) : WebPart = 
        
        fun ctx -> async { 
            let requestId = Guid.NewGuid().ToString()
            let stopwatch = Stopwatch.StartNew()

            let requestLogger = Log.ForContext(RequestLogEnricher(ctx, config, requestId))
            requestLogger.Information("{Method} Request at {FullPath}")
            
            try
              let contextWithRequestId = 
                { ctx with 
                    userState = Map.add "RequestId" (box requestId) ctx.userState }

              let! result = app contextWithRequestId
              match result with 
              | Some resultContext ->
                  let responseLogger = Log.ForContext(ResponseLogEnricher(resultContext, config, stopwatch, requestId))
                  responseLogger.Information("{Method} Response at {FullPath} took {Duration} ms")
                  return Some resultContext
              | None -> 
                  let passThrough = Log.ForContext(PassThroughLogEnricher(ctx))
                  passThrough.Information("Passing through logger WebPart")
                  return None
            with 
            | ex -> 
                let errorLogger = Log.ForContext(ErrorLogEnricher(ctx, stopwatch, requestId))
                errorLogger.Error(ex, "Error at {FullPath} took {Duration} ms")
                let! errorHandlerResult = ((config.ErrorHandler ex ctx) ctx) 
                return errorHandlerResult
        }

    static member Enable(app: WebPart) = SerilogAdapter.Enable(app, SerilogConfig.defaults)