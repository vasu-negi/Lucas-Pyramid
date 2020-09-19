

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
let n=1000000 
let k=24

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
        printfn "Perfect Square: %i" message
         

let myActor (mailbox: Actor<_>) = 
    let rec loop() = 
        actor {
        let rand = System.Random()
        printf "HERE   "
        let! message = mailbox.Receive()
        let sender = mailbox.Sender()
        match message with 
        | (a,b) -> 
            for i=a to b do
                check i
        inc <- inc + 1
        sender <! message
        return! loop()
        } 
    loop()
        
let myMonitor (mailbox: Actor<_>) =
    let rec loop()= 
        actor{
            printfn "%i" Environment.ProcessorCount
            let numberofCores = Environment.ProcessorCount
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
                }
    loop()

let mon = spawn system "myMon" myMonitor
 

