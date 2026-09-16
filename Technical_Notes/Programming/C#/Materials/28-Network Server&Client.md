## Server
```csharp
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class ServerApp
{
    static async Task Main(string[] args)
    {
        int port = 5000;
        TcpListener server = new TcpListener(IPAddress.Loopback, port);
        
        server.Start();
        Console.WriteLine("Server is listening on port 5000...");
        Console.WriteLine("Waiting for a client...");

        TcpClient client = await server.AcceptTcpClientAsync();
        Console.WriteLine("Client connected! (TCP Handshake established)");

        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];

        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine($"Received from client: {message}");

        string responseMessage = "Hello Client, your message was received!";
        byte[] responseBytes = Encoding.UTF8.GetBytes(responseMessage);
        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
        Console.WriteLine("Response sent to client.");

        client.Close();
        server.Stop();
        Console.WriteLine("Connection closed. Press any key to exit...");
        Console.ReadKey();
    }
}
```

## Client
```csharp
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class ClientApp
{
    static async Task Main(string[] args)
    {
        int port = 5000;
        TcpClient client = new TcpClient();

        Console.WriteLine("Attempting to connect to the server...");
        
        await client.ConnectAsync(IPAddress.Loopback, port);
        Console.WriteLine("Successfully connected to the server!");

        NetworkStream stream = client.GetStream();

        string message = "Hello Server, I am the Client!";
        byte[] data = Encoding.UTF8.GetBytes(message);
        await stream.WriteAsync(data, 0, data.Length);
        Console.WriteLine("Message sent to server.");

        byte[] buffer = new byte[1024];
        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine($"Server response: {response}");

        client.Close();
        Console.WriteLine("Connection closed. Press any key to exit...");
        Console.ReadKey();
    }
}
```
