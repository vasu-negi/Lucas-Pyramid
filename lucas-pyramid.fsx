#time "on"
#r "nuget: Akka.FSharp" 
#r "nuget: Akka.TestKit" 


open System
open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open System.Diagnostics
let mutable inc = 0 
let system = System.create "MySystem" (Configuration.defaultConfig())

let bigint (x:int) = bigint(x)
let n=100000000
let k=24

let getActorCount number : int= 
    let mutable res = n 
    if n < Environment.ProcessorCount then
        res <-n
    else
        res <- Environment.ProcessorCount
    res
    
let numberofCores = getActorCount n

type Message = 
    | Success of string
    | Parent of string


let check message = 
    let mutable res=0 |> bigint
    let mutable msg = message |> bigint
    let mutable k2 = k |> bigint
    let mutable i=msg
    let mutable increment = 1 |> bigint

    while (i<(msg+k2)) do
        res <- res+ (i*i)
        i<- i + increment

    let root= sqrt (float res) 
    ()

    if root % 1.000000 = 0.000000 then
        printfn "%i" message
         

let myActor (mailbox: Actor<_>) = 
    let rec loop() = actor {
        let rand = System.Random()
        let! message = mailbox.Receive()
        let sender = mailbox.Sender()
        match message with 
            | (a,b) -> 
                for i=a to b do
                    check i
                sender <! Success "Success"
        return! loop()
    } 
    loop()


let myMonitor (mailbox: Actor<_>) =
    let rec loop()= actor{
        let! msg = mailbox.Receive();
        match msg with 
        | Success s -> inc <- inc + 1
        | Parent m -> 
            printfn "%i" n
            let actorArray =  [for i in 1 .. (numberofCores+1) do yield spawn system ("myActor" + (string) i) myActor]

            let workUnit=n/numberofCores
            let mutable startLimit= 0
            let mutable endLimit = 0
            
            for i=1 to numberofCores  do
                startLimit <- ((i-1)*workUnit)+1
                if i<numberofCores then
                    endLimit <- i*workUnit
                    actorArray.[i] <! (startLimit,endLimit)
                else
                    endLimit <- n
                    actorArray.[i] <! (startLimit,endLimit)
        return! loop()
    }            
    loop()


let mon = spawn system "myMon" myMonitor
mon <! Parent "parent" 
while(inc <> numberofCores) do
((*do*))



