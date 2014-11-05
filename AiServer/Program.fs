
open System
open System.Net
open System.IO
open System.Text
open System.Diagnostics

open AiLib.Framework
open AiLib.Net
open AiLib.Net.Request
open AiLib.Net.Response

let encoding = new UTF8Encoding()

let handler: HttpHandler = fun req resp ->
    async {
        //printfn "Got request of %A" req.Url
        RespContentEncoding encoding resp 

        match req with
        | Accepts [ Path "/getGreeting" ; AcceptHttpMethod "GET" ] req ->
            Respond resp [ 
                RespContentType "text/plain";
                RespBytes("Hello"B)
            ]

        | _ -> 
            Respond resp [  
                RespExn (new Exception("BadRequest"))
            ]
            
    }   

let httpGet() =
    let uri = "http://localhost:3410/getGreeting"
    let req = WebRequest.CreateHttp(uri)
    req.Method <- "GET"

    use resp = req.GetResponse()
    use stream = resp.GetResponseStream()
    let buffer = Array.zeroCreate (int resp.ContentLength)
    let nBytes = stream.Read(buffer, 0, buffer.Length)
    encoding.GetString(buffer)

[<EntryPoint>]
let main argv = 
    let server = Ai.Scavenger.Server(Array2D.zeroCreate 0 0, encoding)
    let webServer = HttpServer(
        {   host = "http://localhost:3410/";
            handler = server.Handler
        })
    webServer.Start()

    Console.ReadLine() |> ignore

    0 