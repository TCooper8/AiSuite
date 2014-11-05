namespace AiLib

open System
open System.IO

open AiLib.Actor

module Logging =
    type Msg =
        | Info of string
        | Debug of string
        | Error of string * exn

    type LoggerConfig = 
        {   name: string;
            infoWriter: TextWriter;
            debugWriter: TextWriter;
            errorWriter: TextWriter;
            formatter: string -> string;
            errorFormatter: string -> exn -> string;
        }

    type Logger(config: LoggerConfig) =
        inherit Actor<Msg>(
            {   name = config.name;
            }
        ) with 

        member this.Info msg =
            Info msg |> this.Agent.Post 

        member this.Debug msg =
            Debug msg |> this.Agent.Post 

        member this.ErrorMsg msg =
            Error(msg, null) |> this.Agent.Post 

        member this.Error msg e =
            Error(msg, e) |> this.Agent.Post 

        member this.DebugWriter = config.debugWriter
        member this.ErrorFormatter = config.errorFormatter
        member this.ErrorWriter = config.errorWriter
        member this.Formatter = config.formatter
        member this.InfoWriter = config.infoWriter

        override this.Receive msg = 
            match msg with
            | Info msg -> this.InfoWriter.WriteLine(this.Formatter msg)
            | Debug msg ->  this.DebugWriter.WriteLine(this.Formatter msg)
            | Error (msg, e) -> this.ErrorWriter.WriteLine(this.ErrorFormatter msg e)

    type BasicLogger(name:string) =
        inherit Logger(
            {   name = name;
                infoWriter = Console.Out;
                debugWriter = Console.Out;
                errorWriter = Console.Error;
                // Formatter function => Date | Name | Msg
                formatter = fun msg -> 
                    sprintf "%A\t|%s\t\t|%s" DateTime.Now name msg;
                errorFormatter = fun msg e -> 
                    sprintf "%A\t|%s\t\t| Error: %s, %A" DateTime.Now name msg e
            }
        )