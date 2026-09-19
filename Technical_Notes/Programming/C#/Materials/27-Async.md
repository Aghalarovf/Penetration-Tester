### Basic Async / Await and Task
```csharp
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Process started...");
        int result = await CalculateAsync(5);
        Console.WriteLine($"Result: {result}");
    }

    static async Task<int> CalculateAsync(int number)
    {
        await Task.Delay(1000);
        return number * number;
    }
}
```

---

### Task.WhenAll and Task.WhenAny
```csharp
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Task<string> task1 = GetDataAsync("Server 1", 1500);
        Task<string> task2 = GetDataAsync("Server 2", 500);

        Task<string> firstFinished = await Task.WhenAny(task1, task2);
        Console.WriteLine($"First finished: {await firstFinished}");

        string[] allResults = await Task.WhenAll(task1, task2);
        Console.WriteLine($"All results received: {allResults[0]}, {allResults[1]}");
    }

    static async Task<string> GetDataAsync(string name, int delay)
    {
        await Task.Delay(delay);
        return $"{name} data";
    }
}
```

---

### CancellationToken and ConfigureAwait
```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        cts.CancelAfter(1000);

        try
        {
            await LongRunningTaskAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation canceled due to timeout!");
        }
    }

    static async Task LongRunningTaskAsync(CancellationToken token)
    {
        for (int i = 0; i < 5; i++)
        {
            token.ThrowIfCancellationRequested();
            Console.WriteLine($"Working... {i + 1}");
            await Task.Delay(500).ConfigureAwait(false);
        }
    }
}
```

---
