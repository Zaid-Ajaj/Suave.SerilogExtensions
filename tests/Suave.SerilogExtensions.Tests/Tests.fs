module Tests

open Expecto
open Suave
open Suave.SerilogExtensions

let pass() = Expect.isTrue true "Passed"
let fail() = Expect.isTrue false "Failed"

let rnd = System.Random()
let rndConfig() = { defaultConfig with bindings = [ HttpBinding.createSimple Protocol.HTTP "127.0.0.1" (rnd.Next(2000, 9000)) ] }

[<Tests>]
let tests =
  testList "Serilog Extensions" [
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
