#r "nuget: Akka.FSharp" 
#r "nuget: Akka.TestKit"
open System
open Akka.Actor
open Akka.Configuration
open Akka.FSharp

let square msg =
  msg * msg

let actorOfConvert f outputRef =
  actorOf2 (fun _ msg -> outputRef <! f msg)

let actorOfConvertToChild f spawnChild (mailbox : Actor<'a>) =

  let rec imp state =
    actor {
      let newstate =
        match state with
        | Some s -> s
        | None -> spawnChild mailbox

      let! msg = mailbox.Receive()
      newstate <! f msg
      return! imp (Some newstate)
    }

  imp None

let squareWithChildRef =
  actorOfConvertToChild print (spawnChild square "print-actor")
  |> spawn system "square-with-child-actor"