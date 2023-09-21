
open System.IO
open System.Net
open System.Net.Sockets

let socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
let endPoint = IPEndPoint(IPAddress.Parse("192.168.0.143"), 1234)
socket.Bind(endPoint)
printfn $"socket type is %s{socket.ToString()}"
printfn $"socket info is %s{socket.LocalEndPoint.ToString()}"
socket.Listen(100)

let mutable keepOpen = true

let newSocket = socket.Accept()
printfn $"The connected client's end point is %s{newSocket.RemoteEndPoint.ToString()}"
    
let greeting = "Hello!"
let greetingBytes = System.Text.Encoding.ASCII.GetBytes(greeting)
let bytesSent = newSocket.Send(greetingBytes)
printfn $"Number of bytes sent: %d{bytesSent}"

while keepOpen do
    let bytesResponse = [|for i in 0..256 -> byte(i)|] 
    let response = newSocket.Receive(bytesResponse)
    let message = System.Text.Encoding.ASCII.GetString bytesResponse[0 .. response]
    printfn $"Number of bytes received from the client: %d{response}"
    printfn $"Message from client: %s{message}"
    
    
    
    
    printfn $"End of loop"
    
    
    
