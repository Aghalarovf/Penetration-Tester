# C# Red Team Roadmap — Filterlənmiş Versiya

> **Hədəf:** Red Team tooling üçün lazım olan C# bazasını qurmaq. Lazımsız mövzular çıxarılmışdır.

---

## 🟢 Stage 1 — Language Basics (Steps 1–10)
> Hamısı mütləqdir. Sintaksis bazası olmadan heç nə yazılmaz.

### Step 1 — Hello World & Program Structure
`namespace`, `class`, `Main()` — proqramın giriş nöqtəsi.
```csharp
Console.WriteLine("Hello, World!");
```

### Step 2 — Variables & Data Types
`int`, `double`, `float`, `bool`, `char`, `string`. Value type vs reference type fərqi.
```csharp
int port = 4444;
string host = "10.0.0.1";
bool isOpen = true;
```

### Step 3 — Type Conversion
`Convert.ToInt32()`, `int.Parse()`, `int.TryParse()`. Network data parse edəndə daim lazım olur.
```csharp
string input = "4444";
int port = int.Parse(input);
```

### Step 4 — Operators
Arithmetic (`+`, `-`, `*`, `/`, `%`), comparison (`==`, `!=`, `>`, `<`), logical (`&&`, `||`, `!`), assignment (`+=`, `-=`).

### Step 5 — String Operations
`Length`, `ToUpper()`, `ToLower()`, `Trim()`, `Replace()`, `Contains()`, `Split()`, `Substring()`, string interpolation.
```csharp
string beacon = $"HOST={host};PORT={port}";
string[] parts = "10.0.0.1:4444".Split(':');
```

### Step 6 — Conditional Statements
`if`, `else if`, `else`, `switch`.
```csharp
if (isOpen) Console.WriteLine("[+] Port open");
else Console.WriteLine("[-] Filtered");
```

### Step 7 — Ternary & Null Coalescing
`? :` və `??` — qısa şərtlər üçün.
```csharp
string status = isOpen ? "OPEN" : "CLOSED";
string target = input ?? "127.0.0.1";
```

### Step 8 — Loops: for & while
Loop dəyişənləri, `break`, `continue`.
```csharp
for (int port = 1; port <= 1024; port++)
    Scan(host, port);
```

### Step 9 — Loops: foreach & do-while
Collection-lar üzərində iterate etmək.
```csharp
foreach (string host in liveHosts)
    Console.WriteLine($"[+] {host}");
```

### Step 10 — Methods
Parametrlər, return tipləri, `void`, overloading.
```csharp
bool IsPortOpen(string host, int port) => TcpConnect(host, port);
```

---

## 🔵 Stage 2 — Collections (Seçilmiş Steps)

### Step 11 — Arrays
Fixed-size. Shellcode byte array-ləri, sabit port siyahıları.
```csharp
byte[] shellcode = { 0x90, 0x90, 0xCC };
int[] commonPorts = { 22, 80, 443, 3389 };
```

### Step 13 — List\<T\>
Dynamic collection. Live host-lar, açıq portlar, loot toplamaq.
```csharp
var liveHosts = new List<string>();
liveHosts.Add("10.0.0.1");
liveHosts.Sort();
```

### Step 14 — Dictionary\<TKey, TValue\>
Key-value. Credential store, port→service mapping, recon nəticələri.
```csharp
var creds = new Dictionary<string, string>();
creds["admin"] = "Password123!";
var portMap = new Dictionary<int, string> { {22, "SSH"}, {3389, "RDP"} };
```

### Step 15 — HashSet\<T\>
Unikal elementlər. Scan edilmiş IP-ləri dedup etmək.
```csharp
var scanned = new HashSet<string>();
scanned.Add("10.0.0.1");
scanned.Add("10.0.0.1"); // ignore edilir
```

### Step 16 — Queue\<T\> & Stack\<T\>
C2 task queue (FIFO), execution history (LIFO).
```csharp
var taskQueue = new Queue<string>();
taskQueue.Enqueue("whoami");
taskQueue.Enqueue("ipconfig");
string next = taskQueue.Dequeue(); // → "whoami"
```

---

## 🟣 Stage 3 — OOP (Seçilmiş Steps)

### Step 21 — Classes & Objects
Tool-ları strukturlaşdırmaq üçün — Scanner, Beacon, Implant class-ları.
```csharp
class PortScanner
{
    public string Target { get; set; }
    public List<int> OpenPorts { get; set; } = new();
}
```

### Step 22 — Properties & Access Modifiers
`public`, `private`, `internal`. Implant config-lərini encapsulate etmək.
```csharp
class BeaconConfig
{
    public string C2Host { get; set; }
    private int _sleepInterval = 60;
    public int SleepInterval => _sleepInterval;
}
```

### Step 23 — Constructors
Parameterli constructor — tool initialization.
```csharp
class ReverseShell
{
    private string _host;
    private int _port;

    public ReverseShell(string host, int port)
    {
        _host = host;
        _port = port;
    }
}
```

### Step 25 — Polymorphism & Virtual Methods
Fərqli C2 channel-ları üçün eyni interface — HTTP, TCP, SMB Pipe.
```csharp
class C2Channel
{
    public virtual string Receive() => "";
}

class HttpChannel : C2Channel
{
    public override string Receive() => PollHttp();
}

class TcpChannel : C2Channel
{
    public override string Receive() => ReadSocket();
}
```

### Step 27 — Static Members & Static Classes
Utility class-ları — helper metodlar, sabit config dəyərləri.
```csharp
static class Utils
{
    public static string XorEncrypt(string data, byte key) { ... }
    public static byte[] ToBytes(string hex) { ... }
}
```

---

## 🟡 Stage 4 — LINQ (Seçilmiş Steps)

### Step 30 — Where & Select
Şərtə görə filter + transform. Port/host siyahılarını emal etmək.
```csharp
var highPorts = ports.Where(p => p > 1024).ToList();
var hostnames = results.Select(r => r.Hostname).ToList();
```

### Step 31 — OrderBy, GroupBy, Distinct
Nəticələri sırala, qruplaşdır, təkrarları sil.
```csharp
var sorted = openPorts.OrderBy(p => p).ToList();
var unique = foundHosts.Distinct().ToList();
```

### Step 32 — First, Any, All, Count
Sürətli yoxlamalar — hər hansı admin var mı, port açıqdırmı.
```csharp
bool hasAdmin = users.Any(u => u.Contains("admin"));
int openCount = ports.Count(p => p < 1024);
string first = liveHosts.FirstOrDefault();
```

---

## 🔴 Stage 5 — Essentials (Seçilmiş Steps)

### Step 35 — Exception Handling
Network tooling-də mütləq lazım — connection fail, timeout, access denied.
```csharp
try
{
    using TcpClient tc = new TcpClient();
    tc.Connect(host, port);
    // ...
}
catch (SocketException ex)
{
    Console.WriteLine($"[-] {host}:{port} — {ex.Message}");
}
finally
{
    // cleanup
}
```

### Step 37 — Nullable Types & Null Safety
Null check olmadan tool-lar crash edir.
```csharp
string? response = GetC2Response();
string cmd = response ?? "sleep";
int? pid = FindProcess("lsass")?.Id;
```

### Step 38–39 — Delegates, Lambda, Func/Action
Callback-lər, async operation-lar, LINQ chain-ləri üçün mütləq lazım.
```csharp
Action<string> log = msg => Console.WriteLine($"[*] {msg}");

Func<string, int, bool> isOpen = (host, port) => TcpConnect(host, port);

// Parallel scan
var tasks = ports.Select(p => Task.Run(() => ScanPort(host, p)));
await Task.WhenAll(tasks);
```

---

## 📌 Oxuma Sırası

| Mərhələ | Mövzu | Vaxt |
|---|---|---|
| 1 | Stage 1 — Bütün Steps 1-10 | 3-4 gün |
| 2 | Stage 2 — Steps 11, 13, 14, 15, 16 | 2-3 gün |
| 3 | Stage 3 — Steps 21, 22, 23, 25, 27 | 3-4 gün |
| 4 | Stage 4 — Steps 30, 31, 32 | 1-2 gün |
| 5 | Stage 5 — Steps 35, 37, 38-39 | 2 gün |
| 6 | **Red Team Library Roadmap-a keç** | — |

---

## ⚡ Sonra Keçəcəyin Mövzular

Bu roadmap bitdikdən sonra birbaşə:

```
System.Net.Sockets     → Reverse shell, C2 channel
System.Diagnostics     → Process execution, enumeration  
System.Net             → HTTP beacon, payload download
System.Runtime.InteropServices → P/Invoke, shellcode injection
System.Reflection      → In-memory execution, AV bypass
System.Security.Cryptography  → C2 traffic encryption
```
