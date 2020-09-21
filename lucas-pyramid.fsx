#time "on"
#r "nuget: Akka.FSharp" 
#r "nuget: Akka.TestKit" 

open System
open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open System.Diagnostics

///////////////////////////Initialization////////////////////////////////////////
let system = System.create "MySystem" (Configuration.defaultConfig())
let mutable numactors = 0 
let convertToBigInt (x:int) = bigint(x)

type Message = 
    | Success of string
    | BossCommand of string
    
let n= 100000
let k=2

let one = bigint(1) 
let divideBy2 = bigint(2)

let getActorCount (number : int) = 
    let mutable res = number 
    if number < Environment.ProcessorCount then
        res <-number
    else
        res <- Environment.ProcessorCount
    res
let numberofCores = getActorCount n
let workActor = n/numberofCores
///////////////////////////Initialization////////////////////////////////////////

let isPerfectSquare n =
    let rec binarySearch l h = 
        let mid = (h + l) / divideBy2
        let midSq = (mid * mid)
        if l > h then false
        elif n = midSq then true
        else if n < midSq then binarySearch l (mid - one)
        else binarySearch (mid + one) h
    binarySearch one n

let squareFunction (sqNumber:int) = 
    let startindex = bigint(sqNumber)
    let mutable start=startindex
    let mutable square = bigint(0)
    let mutable inc_step = bigint(1)
    let bigIntK = bigint(k)
     
    while (start<(startindex+bigIntK)) do
        square <- square + (start*start)
        start<- start + inc_step
    let sq = isPerfectSquare square
    if sq then printfn "%i" sqNumber
    
let Worker (mailbox: Actor<_>) = 
    let rec loop() = actor {
        let! recievedCommand = mailbox.Receive()
        let sender = mailbox.Sender()
        match recievedCommand with 
            | (a,b) -> 
                for x = a to b do
                    squareFunction x
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
            let actorArray =  [for i in 1 .. (numberofCores+1) do yield spawn system ("Worker" + (string) i) Worker]
            
            
            for i = 1 to numberofCores  do
                left <- ((i-1)*workActor)+1 
                if i <= numberofCores then
                    right <- i*workActor
                    actorArray.[i] <! (left,right)
                else
                    right <- n
                    actorArray.[i] <! (left,right)
        return! loop()
    }            
    loop()


let boss = spawn system "Boss" Boss
boss <! BossCommand "BossCommand" 

while(numactors <> numberofCores) do
()



