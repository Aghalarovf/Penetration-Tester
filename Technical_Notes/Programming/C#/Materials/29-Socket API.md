## Server
```csharp
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class SocketServer
{
    static void Main(string[] args)
    {
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Loopback, 5000);
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        serverSocket.Bind(localEndPoint);
        serverSocket.Listen(10);
        Console.WriteLine("Socket server is listening on port 5000...");

        Socket clientSocket = serverSocket.Accept();
        Console.WriteLine("Client connected via low-level socket.");

        byte[] buffer = new byte[1024];
        int bytesRead = clientSocket.Receive(buffer);
        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine($"Received: {message}");

        clientSocket.Send(buffer, 0, bytesRead, SocketFlags.None);
        Console.WriteLine("Echoed message back to client.");

        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
        serverSocket.Close();
        Console.WriteLine("Server closed.");
    }
}
```

## Client
```csharp
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class SocketClient
{
    static void Main(string[] args)
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Loopback, 5000);
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        Console.WriteLine("Attempting to connect to server using low-level socket...");
        clientSocket.Connect(remoteEndPoint);
        Console.WriteLine("Connected to server!");

        string message = "Hello Raw Socket!";
        byte[] data = Encoding.UTF8.GetBytes(message);
        clientSocket.Send(data);
        Console.WriteLine("Message sent.");

        byte[] buffer = new byte[1024];
        int bytesRead = clientSocket.Receive(buffer);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine($"Echo response: {response}");

        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
        Console.WriteLine("Client closed.");
    }
}
```
