open System
open Server
open SocketSetup


[<EntryPoint>]
let main args =
    
    let serverSocket = socketSetup
    Async.Start(socketHandler serverSocket)
    
    Console.ReadLine() |> ignore
    
    0
    
    
    
    
