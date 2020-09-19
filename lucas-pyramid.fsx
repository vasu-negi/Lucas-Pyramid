


#r "nuget: Akka.FSharp" 
#r "nuget: Akka.TestKit" 
// #load "Bootstrap.fsx"
//let stopWatch = System.Diagnostics.Stopwatch.StartNew()

open System
open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open System.Diagnostics
let mutable inc = 0 
let system = System.create "MySystem" (Configuration.defaultConfig())


let bigint (x:int) = bigint(x)
let n=100000 
let k=2

let check message = 
    //printfn "Hello%i " message
    let mutable res=0 |> bigint
    let mutable msg = message |> bigint
    let mutable k2 = k |> bigint
    let mutable i=msg
    let mutable increment = 1 |> bigint
    //i |> bigint
    //printfn "Type message : %A" (msg.GetType())
    //printfn "Type k2 : %A" (k2.GetType())
    //printfn "Type n : %A" (n.GetType())
    //printfn "Type : %A" (i.GetType())
   // printfn "Type res : %A" (res.GetType())
    while (i<(msg+k2)) do
        res <- res+ (i*i)
        i<- i + increment

   // for i=message |> bigint to (message+k-1) |> bigint do
        //printfn "result= %i" res

    

    let root= sqrt (float res) 
    ()
    //()
    //if message = 46564 then
        //printfn "result= %i" res
        //printfn "root :%f" root
    //printfn "message=%A" msg
    //printfn "root :%f" root
    //printfn "Type ROOT : %A" (root.GetType())
    
    if root % 1.000000 = 0.000000 then
        printfn "Perfect Square: %i" message
         



let myActor (mailbox: Actor<_>) = 
    actor {
        let rand = System.Random()
        printf "HERE   "
        let! message = mailbox.Receive()
        let sender = mailbox.Sender()
        match message with 
        | (a,b) -> 
            //printfn " %i  %i bello" a b
            for i=a to b do
                //printf " YYYY "
                //printfn "index%i" i
                //printfn "Value of a %i" a
                check i

        sender <! message
        inc <- inc + 1
    }

type Msg =
    { starting: int
      ending: int 
    }



let myMonitor (mailbox: Actor<_>) =
    //let mutable i = 11
    printfn "%i" Environment.ProcessorCount
    let numberofCores = Environment.ProcessorCount
    let actorArray = Array.create (numberofCores+1) (spawn system "myActor" myActor)
    {1..10} |> Seq.iter (fun a ->
        actorArray.[a] <- spawn system (string a) myActor
        ()
    )
   


    // First group
    //{1..10} |> Seq.iter(fun a ->
     //   actorArray.[a] <! a 
    //    ()
   // )
    let workUnit=n/10
    let mutable startLimit= 0
    let mutable endLimit = 0
    for i=1 to numberofCores  do


        startLimit <- ((i-1)*workUnit)+1
        if i<numberofCores then
            endLimit <- i*workUnit
            //printfn "%i %i sdfasd" startLimit endLimit
            actorArray.[i] <! (startLimit,endLimit)
            //printf "HELLO "
            
        else
            endLimit <- n
            actorArray.[i] <! (startLimit,endLimit)

    (*for i=1 to 10 do
        actorArray.[i] <! starting ending
        for k=b to (b+comp) do
            actorArray.[i] <! b  
            //printf "b=%i" b
            b<-b+1*)

    let rec loop() =
        actor {
            let! message = mailbox.Receive()
            match message with
            |_ -> 
                //actorArray.[message] <! i  
               // i<-i+1 
            
             
              
            return! loop()
        } 
    loop()


let time f = 
    let proc = Process.GetCurrentProcess()
    let cpu_time_stamp = proc.TotalProcessorTime
    let timer = new Stopwatch()
    timer.Start()
    try
        f()
    finally
        let cpu_time = (proc.TotalProcessorTime-cpu_time_stamp).TotalMilliseconds
        printfn "CPU time = %dms" (int64 cpu_time)
        printfn "Absolute time = %dms" timer.ElapsedMilliseconds
        
#time
let mon = spawn system "myMon" myMonitor
#time