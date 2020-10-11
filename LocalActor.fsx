#r "nuget: Akka.FSharp" 
#r "nuget: Akka.Remote"

open System
open Akka.Actor
open Akka.Configuration
open Akka.FSharp

let configuration = 
    ConfigurationFactory.ParseString(
        @"akka {
            actor {
                provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                deployment {
                    
                }
            }
            remote {
                helios.tcp {
                    port = 0
                    hostname = ""10.20.62.11""
                }
            }
        }")

let system = ActorSystem.Create("RemoteFSharp", configuration)
let echoClient1 = system.ActorSelection("akka.tcp://RemoteFSharp@10.20.62.15:6565/user/EchoServer")
let echoClient2 = system.ActorSelection("akka.tcp://RemoteFSharp@10.20.62.11:6565/user/EchoServer")
let echoClient3 = system.ActorSelection("akka.tcp://RemoteFSharp@10.20.62.18:6565/user/EchoServer")


printfn "Command line arguments: %A " fsi.CommandLineArgs

match fsi.CommandLineArgs with
    | [| fileName; n; k |] ->
        printfn "running %s: with n = %s , k = %s" fileName n k
        let start1 = 0
        let end1 = int(n) / 3

        let start2 = int(n) / 3 + 1
        let end2 = 2 * int(n) / 3

        let start3 = 2 * int(n) / 3 + 1
        let end3 = int(n)
        
        let task1 = echoClient1 <? sprintf "%d %s %d" end1 k start1
        let response1 = Async.RunSynchronously (task1)
        printfn "Task running on remote with %s" (string(response1))

        let task2 = echoClient2 <? sprintf "%d %s %d" end2 k start2
        let response2 = Async.RunSynchronously (task2)
        printfn "Task running on remote with %s" (string(response2))

        let task3 = echoClient3 <? sprintf "%d %s %d" end3 k start3
        let response3 = Async.RunSynchronously (task3)
        printfn "Task running on remote with %s" (string(response3))
    | _ ->
        printfn "USAGE: LocalActor.fsx 3 2"
