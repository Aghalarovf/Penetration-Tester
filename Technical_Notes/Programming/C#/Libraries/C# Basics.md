# C# Red Team Roadmap — Tam Versiya

> **Hədəf:** Red Team tooling üçün lazım olan C# bazasını qurmaq.

---

## 🟢 Stage 1 — Language Basics
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

## 🔵 Stage 2 — Collections

### Step 11 — Arrays
Fixed-size. Shellcode byte array-ləri, sabit port siyahıları.
```csharp
byte[] shellcode = { 0x90, 0x90, 0xCC };
int[] commonPorts = { 22, 80, 443, 3389 };
```

### Step 12 — List\<T\>
Dynamic collection. Live host-lar, açıq portlar, loot toplamaq.
```csharp
var liveHosts = new List<string>();
liveHosts.Add("10.0.0.1");
liveHosts.Sort();
```

### Step 13 — Dictionary\<TKey, TValue\>
Key-value. Credential store, port→service mapping, recon nəticələri.
```csharp
var creds = new Dictionary<string, string>();
creds["admin"] = "Password123!";
var portMap = new Dictionary<int, string> { {22, "SSH"}, {3389, "RDP"} };
```

### Step 14 — HashSet\<T\>
Unikal elementlər. Scan edilmiş IP-ləri dedup etmək.
```csharp
var scanned = new HashSet<string>();
scanned.Add("10.0.0.1");
scanned.Add("10.0.0.1"); // ignore edilir
```

### Step 15 — Queue\<T\> & Stack\<T\>
C2 task queue (FIFO), execution history (LIFO).
```csharp
var taskQueue = new Queue<string>();
taskQueue.Enqueue("whoami");
taskQueue.Enqueue("ipconfig");
string next = taskQueue.Dequeue(); // → "whoami"
```

---

## 🟣 Stage 3 — OOP

### Step 16 — Classes & Objects
Tool-ları strukturlaşdırmaq üçün — Scanner, Beacon, Implant class-ları.
```csharp
class PortScanner
{
    public string Target { get; set; }
    public List<int> OpenPorts { get; set; } = new();
}
```

### Step 17 — Properties & Access Modifiers
`public`, `private`, `internal`. Implant config-lərini encapsulate etmək.
```csharp
class BeaconConfig
{
    public string C2Host { get; set; }
    private int _sleepInterval = 60;
    public int SleepInterval => _sleepInterval;
}
```

### Step 18 — Constructors
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

### Step 19 — Interface ⬅️ YENİ
`interface` — fərqli C2 channel-larını eyni contract altında birləşdirmək.
Virtual/override inheritance-dan fərqli olaraq, interface **nə edəcəyini** müəyyən edir, **necə** deyil.
```csharp
interface IC2Channel
{
    string Receive();
    void Send(string data);
}

class HttpChannel : IC2Channel
{
    public string Receive() => PollHttp();
    public void Send(string data) => PostHttp(data);
}

class TcpChannel : IC2Channel
{
    public string Receive() => ReadSocket();
    public void Send(string data) => WriteSocket(data);
}

// İstifadəsi — hansı channel olduğu fərq etmir
IC2Channel channel = new HttpChannel();
string task = channel.Receive();
```

### Step 20 — Polymorphism & Virtual Methods
Fərqli C2 channel-ları üçün base class davranışını override etmək.
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

### Step 21 — Static Members & Static Classes
Utility class-ları — helper metodlar, sabit config dəyərləri.
```csharp
static class Utils
{
    public static string XorEncrypt(string data, byte key) { ... }
    public static byte[] ToBytes(string hex) { ... }
}
```

---

## 🟡 Stage 4 — LINQ

### Step 22 — Where & Select
Şərtə görə filter + transform. Port/host siyahılarını emal etmək.
```csharp
var highPorts = ports.Where(p => p > 1024).ToList();
var hostnames = results.Select(r => r.Hostname).ToList();
```

### Step 23 — OrderBy, GroupBy, Distinct
Nəticələri sırala, qruplaşdır, təkrarları sil.
```csharp
var sorted = openPorts.OrderBy(p => p).ToList();
var unique = foundHosts.Distinct().ToList();
```

### Step 24 — First, Any, All, Count
Sürətli yoxlamalar — hər hansı admin var mı, port açıqdırmı.
```csharp
bool hasAdmin = users.Any(u => u.Contains("admin"));
int openCount = ports.Count(p => p < 1024);
string first = liveHosts.FirstOrDefault();
```

---

## 🔴 Stage 5 — Essentials

### Step 25 — Exception Handling
Network tooling-də mütləq lazım — connection fail, timeout, access denied.
```csharp
try
{
    using TcpClient tc = new TcpClient();
    tc.Connect(host, port);
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

### Step 26 — Nullable Types & Null Safety
Null check olmadan tool-lar crash edir.
```csharp
string? response = GetC2Response();
string cmd = response ?? "sleep";
int? pid = FindProcess("lsass")?.Id;
```

### Step 27 — Delegates, Lambda, Func/Action
Callback-lər, async operation-lar, LINQ chain-ləri üçün mütləq lazım.
```csharp
Action<string> log = msg => Console.WriteLine($"[*] {msg}");
Func<string, int, bool> isOpen = (host, port) => TcpConnect(host, port);
```

### Step 28 — using & IDisposable ⬅️ YENİ
Network connection-lar, file stream-lər mütləq dispose edilməlidir.
`using` — scope bitəndə avtomatik `Dispose()` çağırır. Memory leak və connection leak qarşısını alır.
```csharp
// using statement — scope bitəndə avtomatik bağlanır
using (TcpClient tc = new TcpClient())
{
    tc.Connect(host, port);
    // scope bitdi → tc.Dispose() avtomatik çağrılır
}

// Modern C# — using declaration
using FileStream fs = File.OpenRead("wordlist.txt");
// method bitəndə avtomatik dispose olur

// Öz class-ında implement etmək
class ScanSession : IDisposable
{
    private TcpClient _client = new TcpClient();

    public void Dispose()
    {
        _client?.Close();
        Console.WriteLine("[*] Session closed");
    }
}
```

### Step 29 — Encoding & Byte Conversion ⬅️ YENİ
Shellcode, beacon data, XOR encryption, Base64 payload — hamısı byte əməliyyatlarıdır.
```csharp
// String → Byte
byte[] bytes = Encoding.UTF8.GetBytes("whoami");

// Byte → String
string text = Encoding.UTF8.GetString(bytes);

// Base64 encode/decode — payload obfuscation
string b64     = Convert.ToBase64String(bytes);
byte[] decoded = Convert.FromBase64String(b64);

// Hex string → byte array — shellcode
string hex     = "90 90 CC";
byte[] shellcode = hex.Split(' ')
                      .Select(h => Convert.ToByte(h, 16))
                      .ToArray();

// XOR encryption — basic obfuscation
byte key = 0x41;
byte[] encrypted = bytes.Select(b => (byte)(b ^ key)).ToArray();
```

### Step 30 — File I/O ⬅️ YENİ
Wordlist oxumaq, loot yazmaq, config faylı — hər tool-da lazımdır.
```csharp
// Wordlist oxu — brute force
string[] passwords = File.ReadAllLines("wordlist.txt");

// Loot fayla yaz
File.WriteAllText("loot.txt", "admin:Password123!");

// Nəticələri append et — hər scan-dan sonra
File.AppendAllText("results.txt", $"[+] {host}:{port} OPEN\n");

// Faylın mövcudluğunu yoxla
if (File.Exists("config.txt"))
{
    string config = File.ReadAllText("config.txt");
}

// Bütün sətirləri List-ə yüklə
List<string> targets = File.ReadAllLines("targets.txt").ToList();
```

### Step 31 — Async / Await ⬅️ YENİ
Network tool-ların ən kritik mövzusu. Sinxron scan — yavaş. Async scan — sürətli.
```csharp
// Sinxron — port 1 bitməmiş port 2 başlamır (yavaş)
for (int p = 1; p <= 1024; p++)
    ScanPort(host, p);

// Async — hamısı paralel işləyir (sürətli)
async Task ScanPortAsync(string host, int port)
{
    try
    {
        using TcpClient tc = new TcpClient();
        await tc.ConnectAsync(host, port);
        Console.WriteLine($"[+] {port} OPEN");
    }
    catch { /* closed */ }
}

// 1024 portu paralel scan et
var tasks = Enumerable.Range(1, 1024)
    .Select(p => ScanPortAsync(host, p));

await Task.WhenAll(tasks);

// HTTP beacon — async
async Task<string> BeaconAsync(string c2Url)
{
    using HttpClient client = new HttpClient();
    return await client.GetStringAsync(c2Url);
}
```

---

## 📌 Oxuma Sırası

| Mərhələ | Mövzu | Vaxt |
|---|---|---|
| 1 | Stage 1 — Steps 1–10 | 3–4 gün |
| 2 | Stage 2 — Steps 11–15 | 2–3 gün |
| 3 | Stage 3 — Steps 16–21 | 3–4 gün |
| 4 | Stage 4 — Steps 22–24 | 1–2 gün |
| 5 | Stage 5 — Steps 25–31 | 3–4 gün |
| 6 | **Red Team Library Roadmap-a keç** | — |

---

## ⚡ Sonra Keçəcəyin Mövzular

Bu roadmap bitdikdən sonra birbaşa:

```
System.Net.Sockets              → Reverse shell, C2 channel
System.Diagnostics              → Process execution, enumeration
System.Net                      → HTTP beacon, payload download
System.Runtime.InteropServices  → P/Invoke, shellcode injection
System.Reflection               → In-memory execution, AV bypass
System.Security.Cryptography    → C2 traffic encryption
```

---

## 🆕 Əlavə Edilən Mövzular

| Step | Mövzu | Niyə Vacibdir |
|---|---|---|
| 19 | Interface | C2 channel abstraction, pluggable architecture |
| 28 | using & IDisposable | Connection leak, memory leak qarşısını alır |
| 29 | Encoding & Bytes | Shellcode, XOR, Base64 — hər tool-da lazım |
| 30 | File I/O | Wordlist, loot, config — hər tool-da lazım |
| 31 | Async / Await | Paralel scan — tool-ların ən kritik mövzusu |
