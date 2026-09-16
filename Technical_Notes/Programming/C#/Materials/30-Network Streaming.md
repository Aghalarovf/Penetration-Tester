## Server
```csharp
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class FramingServer
{
    static async Task Main(string[] args)
    {
        TcpListener server = new TcpListener(IPAddress.Loopback, 5000);
        server.Start();
        Console.WriteLine("Server listening...");

        using TcpClient client = await server.AcceptTcpClientAsync();
        using NetworkStream stream = client.GetStream();
        using BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
        using BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);

        int length = reader.ReadInt32();
        byte[] data = reader.ReadBytes(length);
        string message = Encoding.UTF8.GetString(data);
        Console.WriteLine($"Received framed message: {message}");

        string response = "Message received successfully!";
        byte[] responseData = Encoding.UTF8.GetBytes(response);
        writer.Write(responseData.Length);
        writer.Write(responseData);

        server.Stop();
    }
}
```

## Client
```csharp
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class FramingClient
{
    static async Task Main(string[] args)
    {
        using TcpClient client = new TcpClient();
        await client.ConnectAsync(IPAddress.Loopback, 5000);
        using NetworkStream stream = client.GetStream();
        using BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
        using BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);

        string message = "Hello through framed TCP stream!";
        byte[] data = Encoding.UTF8.GetBytes(message);

        writer.Write(data.Length);
        writer.Write(data);
        Console.WriteLine("Framed message sent.");

        int responseLength = reader.ReadInt32();
        byte[] responseData = reader.ReadBytes(responseLength);
        string response = Encoding.UTF8.GetString(responseData);
        Console.WriteLine($"Server response: {response}");
    }
}
```
