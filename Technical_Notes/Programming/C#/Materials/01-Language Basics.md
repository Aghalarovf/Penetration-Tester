# Output Functions
```csharp
Console.Clear();
Console.Write("hello);
Console.WriteLine("Hello");

```

# Input Functions
```csharp
string text = Console.ReadLine();
int symbol = Console.Read();
ConsoleKeyInfo key = Console.ReadKey();
```

# Color Functions
```csharp
Console.ForegroundColor = ConsoleColor.Green;
Console.ResetColor();     
```

# Argument Read
```csharp
static void Main(string[] args)
{
   string target = "";
   int port = 80;

   for (int i = 0; i < args.Length; i++)
    {
        if (args[i] == "--target") target = args[i + 1];
        if (args[i] == "--port") port = int.Parse(args[i + 1]);
    }
}
```

# Silent Mode
```csharp
bool verbose = args.Contains("--verbose");

void Log(string msg, bool onlyVerbose = false)
{
    if (!onlyVerbose || verbose)
        Console.WriteLine(msg);
}

Log("[+] Host found: " + ip);
Log("[*] Debug: Socket open", onlyVerbose: true);
```
