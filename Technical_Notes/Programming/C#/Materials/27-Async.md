```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        string host = "127.0.0.1";

        Console.WriteLine("--- 1. Parallel Port Scan ---");
        List<int> openPorts = await ScanPortsAsync(host, 1, 100);
        foreach (int port in openPorts)
        {
            Console.WriteLine($"Port {port} is OPEN");
        }

        Console.WriteLine("\n--- 2. Timeout Pattern (Task.WhenAny) ---");
        bool isOpen = await CheckPortWithTimeoutAsync(host, 80, 500);
        Console.WriteLine($"Port 80 status: {(isOpen ? "Open" : "Closed/Timeout")}");

        Console.WriteLine("\n--- 3. Cancellation Token ---");
        using CancellationTokenSource cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately for demonstration

        try
        {
            await ScanPortWithTokenAsync(host, 443, cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Scan task was canceled.");
        }
    }

    // Day 1 & 2: Task, Task<T> and Task.WhenAll for parallel scanning
    static async Task<List<int>> ScanPortsAsync(string host, int startPort, int endPort)
    {
        List<Task<int>> tasks = new List<Task<int>>();

        for (int port = startPort; port <= endPort; port++)
        {
            tasks.Add(CheckSinglePortAsync(host, port));
        }

        int[] results = await Task.WhenAll(tasks);
        return results.Where(port => port != -1).ToList();
    }

    static async Task<int> CheckSinglePortAsync(string host, int port)
    {
        using TcpClient client = new TcpClient();
        try
        {
            await client.ConnectAsync(host, port).ConfigureAwait(false);
            return port;
        }
        catch
        {
            return -1;
        }
    }

    // Day 3: Task.WhenAny and Timeout Pattern
    static async Task<bool> CheckPortWithTimeoutAsync(string host, int port, int timeoutMs)
    {
        using TcpClient client = new TcpClient();
        Task connectTask = client.ConnectAsync(host, port);
        Task timeoutTask = Task.Delay(timeoutMs);

        Task completedTask = await Task.WhenAny(connectTask, timeoutTask).ConfigureAwait(false);

        return completedTask == connectTask && client.Connected;
    }

    // Day 3: CancellationToken
    static async Task ScanPortWithTokenAsync(string host, int port, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        using TcpClient client = new TcpClient();
        await client.ConnectAsync(host, port).WaitAsync(token).ConfigureAwait(false);
    }
}
```
