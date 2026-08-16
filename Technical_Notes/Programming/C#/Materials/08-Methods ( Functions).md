# Step 10 — Methods (Functions)

---

## Defining & Calling a Method
```csharp
// Definition
void SayHello()
{
    Console.WriteLine("[*] Hello, Hacker!");
}

// Call
SayHello();   // → [*] Hello, Hacker!

// Definition order doesn't matter in C# — call before or after
Greet();

void Greet()
{
    Console.WriteLine("[*] Greet called");
}
```

---

## void — No Return Value
```csharp
// void = method does something but returns nothing
void PrintBanner(string host)
{
    Console.WriteLine($"[*] Target : {host}");
    Console.WriteLine($"[*] Time   : {DateTime.Now}");
}

PrintBanner("192.168.1.1");
// → [*] Target : 192.168.1.1
// → [*] Time   : ...

// Early exit with return (no value)
void CheckPort(int port)
{
    if (port < 1 || port > 65535)
    {
        Console.WriteLine("[-] Invalid port");
        return;                    // exit early
    }
    Console.WriteLine($"[+] Port {port} is valid");
}
```

---

## return — Return a Value
```csharp
// Return type declared before method name
string GetVersion(string banner)
{
    int idx = banner.IndexOf("_");
    return banner.Substring(idx + 1, 5);
}

string ver = GetVersion("OpenSSH_8.9p1");
Console.WriteLine(ver);           // → 8.9p1

// Return bool — flag check
bool IsPrivateIP(string ip)
{
    return ip.StartsWith("192.168.") ||
           ip.StartsWith("10.")      ||
           ip.StartsWith("172.16.");
}

bool priv = IsPrivateIP("192.168.1.5");  // → true

// Return int
int Add(int a, int b)
{
    return a + b;
}

int sum = Add(40, 2);             // → 42
```

---

## Parameters & Arguments
```csharp
// Single parameter
void Scan(string host)
{
    Console.WriteLine($"[*] Scanning {host}");
}

// Multiple parameters
void Connect(string host, int port)
{
    Console.WriteLine($"[*] Connecting to {host}:{port}");
}

Connect("10.0.0.1", 4444);        // → [*] Connecting to 10.0.0.1:4444

// Calling with named arguments — order doesn't matter
Connect(port: 8080, host: "10.0.0.2");
```

---

## Default Parameters
```csharp
// Default value assigned in signature
void Scan(string host, int port = 80, bool verbose = false)
{
    if (verbose)
        Console.WriteLine($"[*] Scanning {host}:{port}");
    else
        Console.WriteLine($"[+] {host}:{port}");
}

Scan("10.0.0.1");                 // uses port=80, verbose=false
Scan("10.0.0.1", 443);           // uses verbose=false
Scan("10.0.0.1", 443, true);     // all explicit

// Default string
void Encode(string data, string charset = "UTF-8")
{
    Console.WriteLine($"[*] Encoding as {charset}: {data}");
}
```

---

## Method Overloading — Same Name, Different Params
```csharp
// Same method name — different signatures
void Log(string message)
{
    Console.WriteLine($"[*] {message}");
}

void Log(string message, string level)
{
    Console.WriteLine($"[{level}] {message}");
}

void Log(string message, string level, bool timestamp)
{
    string ts = timestamp ? $"[{DateTime.Now:HH:mm:ss}]" : "";
    Console.WriteLine($"{ts}[{level}] {message}");
}

// C# picks the correct version automatically
Log("Host is up");                        // → [*] Host is up
Log("Port open", "+");                    // → [+] Port open
Log("Access denied", "!", true);          // → [14:22:01][!] Access denied

// Overload — different parameter types
int Multiply(int a, int b)     => a * b;
double Multiply(double a, double b) => a * b;

int    r1 = Multiply(3, 4);        // → 12
double r2 = Multiply(3.5, 2.0);   // → 7.0
```

---

## Expression-Body Methods (Arrow Syntax)
```csharp
// Single expression — use => instead of { return ... }
string ToHex(byte b)          => b.ToString("X2");
bool   IsOpen(int port)       => port > 0 && port < 65536;
int    Square(int n)          => n * n;
string Prompt(string host)    => $"[*] Attacking {host}";

// void arrow method
void Banner(string t)         => Console.WriteLine($"=== {t} ===");

// Usage
Console.WriteLine(ToHex(255));     // → FF
Banner("RECON PHASE");             // → === RECON PHASE ===
```

---

## Returning Multiple Values — Tuples
```csharp
// Return a tuple
(string host, int port, bool open) ScanPort(string ip, int p)
{
    bool isOpen = TryConnect(ip, p);
    return (ip, p, isOpen);
}

var result = ScanPort("10.0.0.1", 80);
Console.WriteLine($"{result.host}:{result.port} → {result.open}");

// Deconstruct directly
var (host, port, open) = ScanPort("10.0.0.1", 443);
Console.WriteLine($"{host}:{port} = {(open ? "OPEN" : "CLOSED")}");

// Simple tuple
(int, string) GetService(int port)
{
    if (port == 22)  return (22, "SSH");
    if (port == 80)  return (80, "HTTP");
    return (port, "UNKNOWN");
}
```

---

## ref & out — Pass by Reference
```csharp
// out — method must assign the value (no input)
bool TryParsePort(string input, out int port)
{
    return int.TryParse(input, out port);
}

if (TryParsePort("8080", out int p))
    Console.WriteLine($"[+] Port: {p}");      // → [+] Port: 8080

// ref — pass existing variable, method can modify it
void Increment(ref int counter)
{
    counter++;
}

int hits = 0;
Increment(ref hits);
Increment(ref hits);
Console.WriteLine(hits);                       // → 2

// Multiple out values
bool TryParseBanner(string banner,
                    out string service,
                    out string version)
{
    service = version = "";
    if (!banner.Contains("_")) return false;
    service = banner.Split('_')[0];
    version = banner.Split('_')[1];
    return true;
}
```

---

## params — Variable Number of Arguments
```csharp
// Accept any number of arguments
void ScanHosts(params string[] hosts)
{
    foreach (string h in hosts)
        Console.WriteLine($"[*] Scanning {h}");
}

ScanHosts("10.0.0.1");
ScanHosts("10.0.0.1", "10.0.0.2", "10.0.0.3");

// params + fixed params
void Log(string level, params string[] messages)
{
    foreach (string m in messages)
        Console.WriteLine($"[{level}] {m}");
}

Log("+", "SSH open", "HTTP open", "RDP open");
// → [+] SSH open
// → [+] HTTP open
// → [+] RDP open
```

---

## Recursive Methods
```csharp
// Method that calls itself
int Factorial(int n)
{
    if (n <= 1) return 1;          // base case — stop recursion
    return n * Factorial(n - 1);   // recursive call
}

Console.WriteLine(Factorial(5));   // → 120

// Recursive directory traversal
void WalkDir(string path, int depth = 0)
{
    string indent = new string(' ', depth * 2);
    Console.WriteLine($"{indent}[>] {path}");

    foreach (string dir in Directory.GetDirectories(path))
        WalkDir(dir, depth + 1);   // recurse into subdirectory
}

WalkDir(@"C:\Users\Anar");
```

---

## Pentest Context Usage
```csharp
// Reusable port scanner method
bool IsPortOpen(string host, int port, int timeout = 1000)
{
    try
    {
        using var client = new System.Net.Sockets.TcpClient();
        return client.ConnectAsync(host, port)
                     .Wait(timeout);
    }
    catch { return false; }
}

// Overloaded scanner
void ScanPort(string host, int port)
{
    bool open = IsPortOpen(host, port);
    Console.WriteLine($"{host}:{port,-6} {(open ? "[+] OPEN" : "[-] CLOSED")}");
}

void ScanPort(string host, int[] ports)
{
    foreach (int p in ports)
        ScanPort(host, p);
}

void ScanPort(string[] hosts, int port)
{
    foreach (string h in hosts)
        ScanPort(h, port);
}

// Usage
ScanPort("10.0.0.1", 80);
ScanPort("10.0.0.1", new[] { 22, 80, 443, 3389 });
ScanPort(new[] { "10.0.0.1", "10.0.0.2" }, 445);

// Encode / decode helpers
string Base64Encode(string input)
{
    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
    return Convert.ToBase64String(bytes);
}

string Base64Decode(string b64)
{
    byte[] bytes = Convert.FromBase64String(b64);
    return System.Text.Encoding.UTF8.GetString(bytes);
}

string enc = Base64Encode("whoami");   // → "d2hvYW1p"
string dec = Base64Decode(enc);        // → "whoami"

// XOR cipher method
byte[] XorEncrypt(byte[] data, byte key)
{
    byte[] result = new byte[data.Length];
    for (int i = 0; i < data.Length; i++)
        result[i] = (byte)(data[i] ^ key);
    return result;
}

byte[] payload   = { 0x90, 0x90, 0xCC, 0xC3 };
byte[] encrypted = XorEncrypt(payload, 0x41);
byte[] decrypted = XorEncrypt(encrypted, 0x41);  // XOR is reversible

// Banner grabber — returns structured result
(string service, string version) ParseBanner(string raw)
{
    if (string.IsNullOrEmpty(raw)) return ("UNKNOWN", "");
    if (raw.Contains("SSH"))
    {
        string ver = raw.Split('_').ElementAtOrDefault(1)
                        ?.Split(' ')[0] ?? "";
        return ("SSH", ver);
    }
    if (raw.StartsWith("HTTP"))   return ("HTTP",  raw.Split(' ')[1]);
    if (raw.Contains("FTP"))      return ("FTP",   raw.Split(' ')[0]);
    return ("UNKNOWN", raw.Substring(0, Math.Min(20, raw.Length)));
}

var (svc, ver) = ParseBanner("OpenSSH_8.9p1 Ubuntu");
Console.WriteLine($"Service: {svc} | Version: {ver}");
// → Service: SSH | Version: 8.9p1
```
