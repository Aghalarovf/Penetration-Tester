# Step 20 — Collection Initialization Patterns

---

## Object Initializers

```csharp
// Without object initializer — verbose
class Host
{
    public string IP      { get; set; } = "";
    public int    Port    { get; set; }
    public bool   IsAlive { get; set; }
    public string OS      { get; set; } = "";
}

// ❌ Old way — constructor + property assignments
Host h = new Host();
h.IP      = "10.0.0.1";
h.Port    = 22;
h.IsAlive = true;
h.OS      = "Linux";

// ✅ Object initializer — clean one-expression syntax
Host h2 = new Host
{
    IP      = "10.0.0.1",
    Port    = 22,
    IsAlive = true,
    OS      = "Linux"
};

// var + target-typed new (C# 9+)
var h3 = new Host { IP = "10.0.0.2", Port = 80 };

// Trailing comma is allowed
var h4 = new Host
{
    IP   = "10.0.0.3",
    Port = 443,          // ← trailing comma OK
};
```

---

## Collection Initializers

```csharp
// List initializer
List<string> hosts = new List<string>
{
    "10.0.0.1",
    "10.0.0.2",
    "10.0.0.3"
};

// Array initializer
string[] targets = { "10.0.0.1", "10.0.0.2" };
int[]    ports   = { 22, 80, 443, 3389 };

// HashSet initializer
HashSet<int> openPorts = new HashSet<int> { 22, 80, 443 };

// Queue / Stack initializer — via IEnumerable constructor
Queue<string> queue = new Queue<string>(new[] { "host1", "host2" });
Stack<int>    stack = new Stack<int>(new[] { 1, 2, 3 });
```

---

## Dictionary Initializers

```csharp
// Style 1 — classic { key, value } pairs
Dictionary<string, int> services = new Dictionary<string, int>
{
    { "SSH",   22  },
    { "HTTP",  80  },
    { "HTTPS", 443 },
    { "RDP",   3389 }
};

// Style 2 — index initializer (C# 6+) — cleaner, preferred
Dictionary<string, int> services2 = new Dictionary<string, int>
{
    ["SSH"]   = 22,
    ["HTTP"]  = 80,
    ["HTTPS"] = 443,
    ["RDP"]   = 3389
};

// var + target-typed new
var creds = new Dictionary<string, string>
{
    ["admin"] = "admin",
    ["root"]  = "toor",
    ["sa"]    = "sa"
};
```

---

## List of Objects — Combined Pattern

```csharp
class ScanResult
{
    public string Host    { get; set; } = "";
    public int    Port    { get; set; }
    public string Service { get; set; } = "";
    public bool   IsOpen  { get; set; }
}

// List of objects using object initializers inside collection initializer
List<ScanResult> results = new List<ScanResult>
{
    new ScanResult { Host = "10.0.0.1", Port = 22,   Service = "SSH",   IsOpen = true  },
    new ScanResult { Host = "10.0.0.1", Port = 80,   Service = "HTTP",  IsOpen = true  },
    new ScanResult { Host = "10.0.0.1", Port = 443,  Service = "HTTPS", IsOpen = false },
    new ScanResult { Host = "10.0.0.1", Port = 3389, Service = "RDP",   IsOpen = true  }
};

// Filter with LINQ
var openOnly = results.Where(r => r.IsOpen).ToList();
```

---

## Nested Object Initializers

```csharp
class PortInfo
{
    public int    Number   { get; set; }
    public string Protocol { get; set; } = "TCP";
    public bool   IsOpen   { get; set; }
}

class Target
{
    public string          IP       { get; set; } = "";
    public string          Hostname { get; set; } = "";
    public List<PortInfo>  Ports    { get; set; } = new();
    public List<string>    Tags     { get; set; } = new();
}

// Nested initializer — objects inside objects
Target target = new Target
{
    IP       = "10.0.0.1",
    Hostname = "server01",
    Ports    = new List<PortInfo>
    {
        new PortInfo { Number = 22,  Protocol = "TCP", IsOpen = true  },
        new PortInfo { Number = 80,  Protocol = "TCP", IsOpen = true  },
        new PortInfo { Number = 443, Protocol = "TCP", IsOpen = false }
    },
    Tags = new List<string> { "linux", "web", "internal" }
};

Console.WriteLine(target.IP);
Console.WriteLine(target.Ports[0].Number);   // → 22
Console.WriteLine(target.Tags[1]);           // → web
```

---

## Target-Typed new (C# 9+)

```csharp
// When the type is already declared, new() infers the type
List<string> hosts = new();                  // same as new List<string>()
Dictionary<string, int> map = new();        // same as new Dictionary<string, int>()
HashSet<int> portSet = new() { 22, 80 };    // with initializer

// Works in any context where type is known
class Target
{
    public List<int> Ports { get; set; } = new();    // inferred from property type
    public List<string> Tags { get; set; } = new();
}

// Method parameter
void Scan(List<string> hosts) { }

Scan(new() { "10.0.0.1", "10.0.0.2" });    // type inferred from parameter
```

---

## with Expression — Non-Destructive Copy (Records)

```csharp
// record — immutable by default, perfect for data
record Host(string IP, int Port, bool IsAlive, string OS = "Unknown");

// Create original
Host original = new Host("10.0.0.1", 22, true, "Linux");

// Copy with one field changed — original untouched
Host modified = original with { Port = 443 };

Console.WriteLine(original.Port);  // → 22
Console.WriteLine(modified.Port);  // → 443

// with works with multiple fields
Host other = original with { IP = "10.0.0.2", IsAlive = false };

// Records also get equality, ToString, Deconstruct for free
Console.WriteLine(original);       // → Host { IP = 10.0.0.1, Port = 22, IsAlive = True, OS = Linux }
bool same = original == modified;  // → False (value equality)
```

---

## Span and Inline Arrays (Performance)

```csharp
// Stackalloc — allocate array on stack (no GC pressure)
Span<int> ports = stackalloc int[] { 22, 80, 443 };

foreach (int p in ports)
    Console.WriteLine(p);

// Inline array init — common ports, no heap allocation
ReadOnlySpan<byte> shellcode = new byte[] { 0x90, 0x90, 0xCC, 0xC3 };
```

---

## Common Patterns Summary

```csharp
// Pattern 1 — Simple list of primitives
var ports = new List<int> { 22, 80, 443, 3389 };

// Pattern 2 — List of objects
var hosts = new List<Host>
{
    new() { IP = "10.0.0.1", Port = 22  },
    new() { IP = "10.0.0.2", Port = 80  }
};

// Pattern 3 — Dictionary with index syntax
var services = new Dictionary<int, string>
{
    [22]   = "SSH",
    [80]   = "HTTP",
    [443]  = "HTTPS"
};

// Pattern 4 — Nested collections
var topology = new Dictionary<string, List<int>>
{
    ["10.0.0.1"] = new() { 22, 80, 443 },
    ["10.0.0.2"] = new() { 3389, 445   }
};

// Pattern 5 — Record with defaults
record ScanConfig(
    string   Host,
    int[]    Ports   = null!,
    int      Timeout = 1000,
    bool     Verbose = false
);

var cfg = new ScanConfig("10.0.0.1") with { Verbose = true };
```

---

## Pentest Context

```csharp
// Clean scan config object
class ScanOptions
{
    public string   Host     { get; set; } = "";
    public int[]    Ports    { get; set; } = { 22, 80, 443, 3389 };
    public int      Timeout  { get; set; } = 1000;
    public bool     Verbose  { get; set; } = false;
    public string   Output   { get; set; } = "console";
}

// Initialize with defaults — override only what you need
var quickScan = new ScanOptions
{
    Host    = "10.0.0.1"
};

var deepScan = new ScanOptions
{
    Host    = "10.0.0.1",
    Ports   = new[] { 21, 22, 23, 25, 53, 80, 110, 143, 443, 445, 3389, 8080 },
    Timeout = 2000,
    Verbose = true,
    Output  = "file"
};

// Clean credential list as Dictionary
var credList = new Dictionary<string, string>
{
    ["admin"]       = "admin",
    ["admin"]       = "password",    // duplicate key — last wins
    ["root"]        = "toor",
    ["administrator"] = "P@ssw0rd"
};

// Service fingerprint database
var fingerprints = new Dictionary<int, (string name, string proto)>
{
    [21]   = ("FTP",    "TCP"),
    [22]   = ("SSH",    "TCP"),
    [23]   = ("Telnet", "TCP"),
    [53]   = ("DNS",    "UDP"),
    [80]   = ("HTTP",   "TCP"),
    [443]  = ("HTTPS",  "TCP"),
    [445]  = ("SMB",    "TCP"),
    [3389] = ("RDP",    "TCP")
};

foreach (var (port, (name, proto)) in fingerprints)
    Console.WriteLine($"{port,-6} {name,-8} {proto}");

// Scan result list — built inline
record PortResult(string Host, int Port, string Service, bool Open);

var scanResults = new List<PortResult>
{
    new("10.0.0.1", 22,  "SSH",   true),
    new("10.0.0.1", 80,  "HTTP",  true),
    new("10.0.0.1", 443, "HTTPS", false),
    new("10.0.0.2", 22,  "SSH",   false),
    new("10.0.0.2", 3389,"RDP",   true)
};

// Filter open ports grouped by host
var grouped = scanResults
    .Where(r => r.Open)
    .GroupBy(r => r.Host);

foreach (var group in grouped)
{
    Console.WriteLine($"\n[+] {group.Key}");
    foreach (var r in group)
        Console.WriteLine($"    {r.Port} {r.Service}");
}
```

---

## Quick Reference

```csharp
// ── Object initializer ───────────────────────────────
var obj = new MyClass { Prop1 = val1, Prop2 = val2 };

// ── Collection initializer ───────────────────────────
var list = new List<int>    { 1, 2, 3 };
var set  = new HashSet<int> { 1, 2, 3 };
var arr  = new int[]        { 1, 2, 3 };

// ── Dictionary initializer ───────────────────────────
var dict = new Dictionary<string, int>
{
    ["key1"] = 1,
    ["key2"] = 2
};

// ── Target-typed new (C# 9+) ─────────────────────────
List<string>              hosts = new();
Dictionary<string, int>   map   = new();
MyClass                   obj2  = new() { Prop = val };

// ── Nested ───────────────────────────────────────────
var nested = new MyClass
{
    Inner = new InnerClass { Value = 42 },
    Items = new List<int> { 1, 2, 3 }
};

// ── Record + with ────────────────────────────────────
record Cfg(string Host, int Port = 80);
var base_ = new Cfg("10.0.0.1");
var copy  = base_ with { Port = 443 };
```
