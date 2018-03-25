namespace Suave.SerilogExtensions

open Suave
open Suave.Successful
open Suave.Operators

type FieldChoser<'t> = Choser of string list

type RequestLogData = RequestLogData
type ResponseLogData = ResponseLogData

/// Configuration to define what fields to ignore from either request or response and the error handler in case an exception is thrown
type SerilogConfig = 
    { IgnoredRequestFields : FieldChoser<RequestLogData>
      IgnoredResponseFields : FieldChoser<ResponseLogData>
      RequestMessageTemplate : string
      ResponseMessageTemplate : string
      ErrorMessageTemplate : string
      ErrorHandler : System.Exception -> HttpContext -> WebPart }

    /// The default config with no ignored log event fields and a generic "Internal Server Error" 500 error handler.
    static member defaults = 
        { IgnoredRequestFields = Choser [ ] 
          IgnoredResponseFields = Choser [ ]
          RequestMessageTemplate = "{Method} Request at {FullPath}"
          ResponseMessageTemplate = "{Method} Response (StatusCode {StatusCode}) at {FullPath} took {Duration} ms"
          ErrorMessageTemplate = "Error at {FullPath} took {Duration} ms"
          ErrorHandler = 
           fun ex httpContext -> 
              OK "Internal Server Error"
              >=> Writers.setStatus HttpCode.HTTP_500  }

module Ignore = 
    let fromRequest : FieldChoser<RequestLogData> = Choser [ ]
    let fromResponse : FieldChoser<ResponseLogData> = Choser [ ]

module Field = 
    let private ignoreReq (input: string) = 
        fun ((Choser ignored): FieldChoser<RequestLogData>) ->
            (Choser (("Request." + input) :: ignored)) : FieldChoser<RequestLogData>
    let private ignoreRes (input: string) = 
        fun ((Choser ignored): FieldChoser<ResponseLogData>) ->
            (Choser (("Response." + input) :: ignored)) : FieldChoser<ResponseLogData>

    let path = ignoreReq "Path"
    let host = ignoreReq "Host"
    let method = ignoreReq "Method"
    let requestHeaders = ignoreReq "Headers"
    let requestBody = ignoreReq "Body"
    let userAgent = ignoreReq "UserAgent"
    let contentType = ignoreReq "ContentType"
    let contentLength = ignoreReq "ContentLength"
    let queryString = ignoreReq "QueryString"
    let query = ignoreReq "Query"
    let requestBodyContent = ignoreReq "BodyContent" 
    let userIpAddress = ignoreReq "UserIPAddress"
    let responseContentLength = ignoreRes "ContentLength"
    let responseContentType = ignoreRes "ContentType"
    let reasonPhrase = ignoreRes "ReasonPhrase"