module Server.SocketSetup
open OperationsManager
open System.Net.Sockets
open System.Net

// change this IP when running on your local machine
[<Literal>]
let LOCAL_IP_STRING = "192.168.0.137"

[<Literal>]
let INIT_MESSAGE = "Hello!"

let socketSetup : Socket =
    let serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
    let endPoint = IPEndPoint(IPAddress.Parse(LOCAL_IP_STRING), 1234)
    serverSocket.Bind(endPoint)
    printfn $"socket type is %s{serverSocket.ToString()}"
    printfn $"socket info is %s{serverSocket.LocalEndPoint.ToString()}"
    serverSocket.Listen(100)
    serverSocket

let sendMessage (socket : Socket) (message : string) : unit =
    let messageByteArray = System.Text.Encoding.ASCII.GetBytes(message)
    socket.Send(messageByteArray) |> ignore
    ()
    
let receiveMessage (socket :Socket) : string =
     let bytesResponse = [|for i in 0..256 -> byte(i)|]
     let response = socket.Receive(bytesResponse)
     let message = System.Text.Encoding.ASCII.GetString bytesResponse[0 .. response]
     message
    
let acceptConnections (serverSocket : Socket) =
    
    let acceptedSocket = serverSocket.Accept()
    printfn $"The connected client's end point is %s{acceptedSocket.RemoteEndPoint.ToString()}"
    sendMessage acceptedSocket INIT_MESSAGE
    acceptedSocket
    
let closeSocket (socket : Socket) =
    socket.Shutdown(SocketShutdown.Both)
    socket.Close()

let socketHandler (serverSocket : Socket) =
    async {
        let mutable keepServerOpen = true
        let clientSockets = new ResizeArray<Socket>()


        while keepServerOpen do
            let connectedSocket = acceptConnections serverSocket
            clientSockets.Add(connectedSocket)
        
            let handleClientAsync (connectedSocket : Socket) =
                async {
                    try
                        let mutable keepOpen = true
                        while keepOpen do
                            let command = receiveMessage connectedSocket
                            printfn $"Message from client: %s{command}"

                            let result = operationsManager command
                            printfn $"Result of operation is: %d{result}"

                            let resultString = result.ToString()
                            sendMessage connectedSocket resultString
                            if command.Trim().Equals(TERMINATE) then
                                    printfn "request for termination received"
                                    keepOpen <- false
                                    // Notify all clients to terminate
                                    for clientSocket in clientSockets do
                                        printfn $"clientSocket %s{clientSocket.RemoteEndPoint.ToString()}"
                                        if clientSocket <> connectedSocket then
                                            sendMessage clientSocket TERMINATE
                                    // Close all client sockets
                                    for clientSocket in clientSockets do
                                        closeSocket clientSocket
                                    keepServerOpen <- false
                            if command.ToLower() = EXIT_COMMAND then
                                keepOpen <- false
                                connectedSocket.Shutdown(SocketShutdown.Both)
                                connectedSocket.Close()
                    with
                    | ex ->
                        printfn $"stack trace: $s{ex.StackTrace}"
                        printfn $"exception message: $s{ex.Message}" 
                        connectedSocket.Shutdown(SocketShutdown.Both)
                        connectedSocket.Close()
                    }  

            Async.Start(handleClientAsync connectedSocket)
 
        // Terminate all client connections when the server is asked to terminate
        while keepServerOpen do
            if clientSockets.Count > 0 then
                for clientSocket in clientSockets do
                    closeSocket clientSocket
            else
                keepServerOpen <- false

        // Shutdown and close the server socket
        closeSocket serverSocket
    }
  
