module Server.SocketSetup

open System.Net.Sockets
open System.Net

[<Literal>]
let LOCAL_IP_STRING = "192.168.0.143"

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

let sendMessage (socket : Socket) (message : string) =
    let messageByteArray = System.Text.Encoding.ASCII.GetBytes(message)
    let bytesSent = socket.Send(messageByteArray)
    printfn $"Number of bytes sent: %d{bytesSent}"
    
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
        let bytesResponse = [|for i in 0..256 -> byte(i)|] 
        let response = connectedSocket.Receive(bytesResponse)
        let message = System.Text.Encoding.ASCII.GetString bytesResponse[0 .. response]
        printfn $"Number of bytes received from the client: %d{response}"
        printfn $"Message from client: %s{message}"
        printfn $"End of loop"
    
    