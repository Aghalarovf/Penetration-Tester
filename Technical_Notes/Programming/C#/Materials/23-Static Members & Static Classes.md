# Step 21 — Static Members & Static Classes

---

## What is `static`?

A `static` member belongs to the **class itself**, not to any instance. There is exactly one copy, shared across everything.

```csharp
class Counter
{
    public static int Total = 0;   // one copy for all instances
    public int Id;

    public Counter()
    {
        Total++;
        Id = Total;
    }
}

Counter a = new Counter();
Counter b = new Counter();
Console.WriteLine(Counter.Total);   // 2  — accessed on the class, not on a/b
Console.WriteLine(a.Total);         // error — don't access static via instance
```

---

## Static Methods

Called on the class, not on an object. Cannot access instance members (`this` doesn't exist).

```csharp
class Utils
{
    public static string XorEncrypt(string data, byte key)
    {
        char[] result = new char[data.Length];
        for (int i = 0; i < data.Length; i++)
            result[i] = (char)(data[i] ^ key);
        return new string(result);
    }

    public static byte[] ToBytes(string hex)
    {
        int len = hex.Length / 2;
        byte[] bytes = new byte[len];
        for (int i = 0; i < len; i++)
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        return bytes;
    }
}

string encrypted = Utils.XorEncrypt("hello", 0x41);
byte[] raw       = Utils.ToBytes("90 90 CC C3".Replace(" ", ""));
```

---

## Static Class

A class marked `static` can only contain static members. Cannot be instantiated at all. Used for pure utility / helper collections.

```csharp
static class NetworkUtils
{
    public static bool IsPrivateIp(string ip)
    {
        return ip.StartsWith("10.")      ||
               ip.StartsWith("192.168.") ||
               ip.StartsWith("172.");
    }

    public static string RandomUserAgent()
    {
        string[] agents = {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64)",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7)"
        };
        return agents[Random.Shared.Next(agents.Length)];
    }
}

bool priv = NetworkUtils.IsPrivateIp("192.168.1.1");   // true
string ua  = NetworkUtils.RandomUserAgent();
// NetworkUtils n = new NetworkUtils();   // error — cannot instantiate
```

---

## Static Fields — Shared State

```csharp
class ImplantManager
{
    private static List<string> _activeAgents = new();
    public  static int          AgentCount    => _activeAgents.Count;

    public static void Register(string agentId)
    {
        _activeAgents.Add(agentId);
    }

    public static void Deregister(string agentId)
    {
        _activeAgents.Remove(agentId);
    }
}

ImplantManager.Register("agent-001");
ImplantManager.Register("agent-002");
Console.WriteLine(ImplantManager.AgentCount);   // 2
```

---

## Static Constructor

Runs once, automatically, before the class is used for the first time. Used to initialize static fields.

```csharp
static class Config
{
    public static readonly string C2Host;
    public static readonly int    Port;

    static Config()   // no access modifier, no parameters
    {
        C2Host = Environment.GetEnvironmentVariable("C2_HOST") ?? "10.0.0.1";
        Port   = int.Parse(Environment.GetEnvironmentVariable("C2_PORT") ?? "443");
    }
}

Console.WriteLine(Config.C2Host);   // reads from env or uses default
```

---

## `const` vs `static readonly`

| | `const` | `static readonly` |
|---|---|---|
| Set when | Compile time | Runtime (constructor) |
| Can use variable/expression | No | Yes |
| Implicitly static | Yes | Must add `static` |

```csharp
static class Limits
{
    public const    int MaxRetries   = 3;                          // compile-time constant
    public static readonly int Timeout = GetTimeout();             // set at runtime
    
    private static int GetTimeout() => 
        int.Parse(Environment.GetEnvironmentVariable("TIMEOUT") ?? "5000");
}
```

---

## Instance vs Static — Side by Side

```csharp
class Beacon
{
    // instance — each Beacon has its own
    public string Host  { get; set; }
    public int    Sleep { get; set; }

    // static — shared across all Beacons
    public static int TotalBeacons  { get; private set; } = 0;
    public static int DefaultSleep  = 60;

    public Beacon(string host)
    {
        Host         = host;
        Sleep        = DefaultSleep;
        TotalBeacons++;
    }
}

Beacon b1 = new Beacon("10.0.0.1");
Beacon b2 = new Beacon("10.0.0.2");

Console.WriteLine(b1.Host);             // 10.0.0.1   — instance
Console.WriteLine(Beacon.TotalBeacons); // 2          — static
```

---

## Quick Reference

| Concept | Syntax | Use when |
|---|---|---|
| Static method | `public static void Foo() { }` | Utility, no instance needed |
| Static field | `public static int X = 0;` | Shared state across all instances |
| Static property | `public static int X { get; }` | Shared computed value |
| Static class | `static class Foo { }` | Pure helper — no instantiation |
| Static constructor | `static Foo() { }` | One-time init of static members |
| `const` | `public const int X = 5;` | Compile-time fixed value |
| `static readonly` | `public static readonly int X;` | Runtime fixed value |
