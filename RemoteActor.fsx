#r "nuget: Akka.FSharp" 
#r "nuget: Akka.Remote"

open System
open Akka.FSharp
let mutable inc = 0 

let config =
    Configuration.parse
        @"akka {
            log-config-on-start : on
            stdout-loglevel : DEBUG
            loglevel : ERROR
            actor.provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
            remote.helios.tcp {
                hostname = ""10.20.62.15""
                port = 6565
            }
        }"

let system = System.create "RemoteFSharp" config
let mutable numactors = 0 
let convertToBigInt (x:int) = bigint(x)

type Message = 
    | Success of string
    | BossCommand of string
    


let one = bigint(1) 
let divideBy2 = bigint(2)

let getActorCount (number : int) = 
    let mutable res = number 
    if number < Environment.ProcessorCount then
        res <-number
    else
        res <- Environment.ProcessorCount
    res
    

///////////////////////////Initialization////////////////////////////////////////

let isSquare n =
    let rec binarySearch l h = 
        let midElement = (h + l) / divideBy2
        let midSq = (midElement * midElement)
        if n = midSq then true
        elif l > h then false
        else if n < midSq then binarySearch l (midElement - one)
        else binarySearch (midElement + one) h
    binarySearch one n

let squareFunction (sqNumber:int) (k: int) = 
    let startindex = bigint(sqNumber)
    let mutable start = startindex
    let mutable square = bigint(0)
    let mutable inc_step = bigint(1)
    let bigIntK = bigint(k)
     
    while (start<(startindex+bigIntK)) do
        square <- square + (start*start)
        start<- start + inc_step

    let sq = isSquare square
    if sq then printfn "%i" sqNumber
    
let Worker (mailbox: Actor<_>) = 
    let rec loop() = actor {
        let! recievedCommand = mailbox.Receive()
        let sender = mailbox.Sender()
        match recievedCommand with 
            | (a,b,k)  -> 
                for x = a to b do
                    squareFunction x k
                sender <! Success "Success"
        return! loop()
    } 
    loop()

let Boss (mailbox: Actor<_>) =
    let rec loop()= actor{
        let! msg = mailbox.Receive();
        match msg with 
        | Success s -> numactors <- numactors + 1
        | BossCommand m -> 
            let mutable left= 0
            let mutable right = 0
            let vals : string[]=  m.Split[|' '|]
            let n = int(vals.[0])
            let k = int(vals.[1])
            let offset = int(vals.[2])
            let numberofCores = getActorCount n
            let workActor = n /numberofCores
            let workerName =  Guid.NewGuid() |> string
            let actorArray =  [for i in 1 .. (numberofCores+1) do yield spawn system (workerName + (string) i) Worker]
            
            for i = 1 to numberofCores  do
                left <- ((i-1)*workActor) + offset + 1
                if i <= numberofCores then
                    right <- i*workActor
                    actorArray.[i] <! (left,right, k)
                else
                    right <- n
                    actorArray.[i] <! (left,right, k)
        return! loop()
    }            
    loop()


let echoServer = 
    spawn system "EchoServer"
    <| fun mailbox ->
        let rec loop() =
            actor {
                let! message = mailbox.Receive()
                let name = Guid.NewGuid() |> string
                let sender = mailbox.Sender()
                let msg: string = message
                printfn "echoServer called"
                let vals : string[]=  message.Split[|' '|]
                let start = 0
                let  n = int(vals.[0])
                let k = int(vals.[1])
                let offset = int(vals.[2])
                printfn "Got args n = %d k = %d" n k 
                let numberofCores = getActorCount n
                let boss = spawn system name Boss
                let input = sprintf "%d %d %d" n k offset
                boss <! BossCommand input
                while(numactors <> numberofCores) do
                ()
                sender <! sprintf "Echo: %s" message
                return! loop()                
            }
        loop()

Console.ReadLine() |> ignore