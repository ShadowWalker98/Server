module Server.SocketSetup
open OperationsManager
open System.Net.Sockets
open System.Net

[<Literal>]
let LOCAL_IP_STRING = "192.168.0.143"

[<Literal>]
let INIT_MESSAGE = "Hello!"

let socketSetup : Socket =
    let serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
    let endPoint = IPEndPoint(IPAddress.Loopback, 1234)
    serverSocket.Bind(endPoint)
    printfn $"socket type is %s{serverSocket.ToString()}"
    printfn $"socket info is %s{serverSocket.LocalEndPoint.ToString()}"
    serverSocket.Listen(100)
    serverSocket

let sendMessage (socket : Socket) (message : string) : unit =
    let messageByteArray = System.Text.Encoding.ASCII.GetBytes(message)
    let bytesSent = socket.Send(messageByteArray)
    ()
    
let receiveMessage (socket :Socket) : string =
     let bytesResponse = [|for i in 0..256 -> byte(i)|]
     let response = socket.Receive(bytesResponse)
     let message = System.Text.Encoding.ASCII.GetString bytesResponse[0 .. response]
     message
    
let acceptConnections (serverSocket : Socket) : Socket =
    let acceptedSocket = serverSocket.Accept()
    printfn $"The connected client's end point is %s{acceptedSocket.RemoteEndPoint.ToString()}"
    sendMessage acceptedSocket INIT_MESSAGE
    acceptedSocket
    
let socketHandler =
    let serverSocket = socketSetup
    let connectedSocket = acceptConnections serverSocket
 
    let mutable keepOpen = true
    
    while keepOpen do
        
        let command = receiveMessage connectedSocket
        printfn $"Message from client: %s{command}"
        let result = operationsManager command 
        printfn $"Result of operation is: %d{result}"
        let resultString = result.ToString()
        sendMessage connectedSocket resultString
        printfn $"End of loop"
    
    