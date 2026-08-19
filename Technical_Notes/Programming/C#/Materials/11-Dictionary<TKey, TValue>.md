# Step 14 — Dictionary\<TKey, TValue\>

---

## What is Dictionary\<TKey, TValue\>?

```csharp
// List — access by index (position)
List<string> hosts = new List<string> { "10.0.0.1", "10.0.0.2" };
hosts[0];                           // → "10.0.0.1"

// Dictionary — access by key (name)
Dictionary<string, int> ports = new Dictionary<string, int>();
ports["SSH"]  = 22;
ports["HTTP"] = 80;
ports["SSH"];                       // → 22
```

`TKey` = key type, `TValue` = value type. Keys must be **unique**.

---

## Declaring & Initializing

```csharp
// Empty dictionary
Dictionary<string, int> services = new Dictionary<string, int>();

// With initial values
Dictionary<string, int> ports = new Dictionary<string, int>
{
    { "SSH",   22  },
    { "HTTP",  80  },
    { "HTTPS", 443 },
    { "RDP",   3389 }
};

// var shorthand
var creds = new Dictionary<string, string>
{
    { "admin", "admin123" },
    { "root",  "toor"     }
};

// Any type as key or value
Dictionary<int, string>   portService = new Dictionary<int, string>();
Dictionary<string, bool>  scanResult  = new Dictionary<string, bool>();
Dictionary<string, List<int>> hostPorts = new Dictionary<string, List<int>>();
```

---

## Add() — Add Entries

```csharp
Dictionary<string, int> services = new Dictionary<string, int>();

// Method 1 — Add()
services.Add("SSH",   22);
services.Add("HTTP",  80);
services.Add("HTTPS", 443);

// Method 2 — index assignment
services["RDP"]   = 3389;
services["SMB"]   = 445;

// Add() throws if key exists — index assignment overwrites
services.Add("SSH", 2222);          // ❌ throws ArgumentException
services["SSH"] = 2222;             // ✅ overwrites silently

// TryAdd — safe add (no exception)
bool added = services.TryAdd("FTP", 21);
Console.WriteLine(added);           // → True (added)

bool again = services.TryAdd("FTP", 21);
Console.WriteLine(again);           // → False (already exists)
```

---

## Remove() — Remove Entries

```csharp
Dictionary<string, int> services = new Dictionary<string, int>
{
    { "SSH", 22 }, { "HTTP", 80 }, { "RDP", 3389 }
};

// Remove by key
bool removed = services.Remove("RDP");
Console.WriteLine(removed);         // → True

// Remove non-existent key — no exception, returns false
bool miss = services.Remove("FTP");
Console.WriteLine(miss);            // → False

// Remove and get the value at the same time
if (services.Remove("HTTP", out int port))
    Console.WriteLine($"Removed HTTP on port {port}");
// → Removed HTTP on port 80

// Clear all entries
services.Clear();
Console.WriteLine(services.Count);  // → 0
```

---

## ContainsKey() — Check Key Existence

```csharp
Dictionary<string, int> services = new Dictionary<string, int>
{
    { "SSH", 22 }, { "HTTP", 80 }, { "HTTPS", 443 }
};

bool hasSSH = services.ContainsKey("SSH");
Console.WriteLine(hasSSH);          // → True

bool hasFTP = services.ContainsKey("FTP");
Console.WriteLine(hasFTP);          // → False

// ContainsValue — check value
bool has80 = services.ContainsValue(80);
Console.WriteLine(has80);           // → True

// ❌ Don't access key directly without checking — throws KeyNotFoundException
// Console.WriteLine(services["FTP"]);

// ✅ Always check first
if (services.ContainsKey("FTP"))
    Console.WriteLine(services["FTP"]);
```

---

## TryGetValue() — Safe Value Access

```csharp
Dictionary<string, int> services = new Dictionary<string, int>
{
    { "SSH", 22 }, { "HTTP", 80 }
};

// ❌ Risky — throws if key missing
int port = services["FTP"];

// ✅ Safe — returns false if missing, no exception
if (services.TryGetValue("SSH", out int sshPort))
    Console.WriteLine($"SSH port: {sshPort}");    // → SSH port: 22

if (!services.TryGetValue("FTP", out int ftpPort))
    Console.WriteLine("FTP not found");            // → FTP not found

// ftpPort = 0 (default) when not found

// GetValueOrDefault — returns default if missing
int rdpPort = services.GetValueOrDefault("RDP", 0);
Console.WriteLine(rdpPort);         // → 0
```

---

## Iterating with foreach

```csharp
Dictionary<string, int> services = new Dictionary<string, int>
{
    { "SSH", 22 }, { "HTTP", 80 }, { "HTTPS", 443 }, { "RDP", 3389 }
};

// Iterate key-value pairs
foreach (KeyValuePair<string, int> pair in services)
{
    Console.WriteLine($"{pair.Key,-8} → {pair.Value}");
}
// → SSH      → 22
// → HTTP     → 80
// → HTTPS    → 443
// → RDP      → 3389

// Deconstruct — cleaner syntax
foreach (var (service, port) in services)
{
    Console.WriteLine($"{service}: {port}");
}

// Keys only
foreach (string key in services.Keys)
    Console.WriteLine(key);

// Values only
foreach (int val in services.Values)
    Console.WriteLine(val);

// LINQ on dictionary
var highPorts = services.Where(p => p.Value > 100);
foreach (var (svc, port) in highPorts)
    Console.WriteLine($"{svc}: {port}");
```

---

## Count & Other Properties

```csharp
Dictionary<string, int> services = new Dictionary<string, int>
{
    { "SSH", 22 }, { "HTTP", 80 }, { "HTTPS", 443 }
};

Console.WriteLine(services.Count);           // → 3

// Keys and Values as collections
ICollection<string> keys   = services.Keys;
ICollection<int>    values = services.Values;

Console.WriteLine(keys.Count);               // → 3
Console.WriteLine(string.Join(", ", keys));  // → SSH, HTTP, HTTPS
```

---

## Nested Dictionary

```csharp
// Dictionary of lists — host → open ports
Dictionary<string, List<int>> hostPorts = new Dictionary<string, List<int>>();

hostPorts["10.0.0.1"] = new List<int> { 22, 80, 443 };
hostPorts["10.0.0.2"] = new List<int> { 3389, 445 };

foreach (var (host, ports) in hostPorts)
{
    Console.Write($"{host}: ");
    Console.WriteLine(string.Join(", ", ports));
}
// → 10.0.0.1: 22, 80, 443
// → 10.0.0.2: 3389, 445

// Dictionary of dictionaries
Dictionary<string, Dictionary<string, string>> db = new();
db["10.0.0.1"] = new Dictionary<string, string>
{
    { "os",    "Linux"  },
    { "state", "up"     }
};
```

---

## Pentest Context

```csharp
// Service fingerprint map
Dictionary<int, string> fingerprints = new Dictionary<int, string>
{
    { 21,   "FTP"   },
    { 22,   "SSH"   },
    { 23,   "Telnet"},
    { 80,   "HTTP"  },
    { 443,  "HTTPS" },
    { 445,  "SMB"   },
    { 3389, "RDP"   }
};

// Identify service by port
int scanned = 443;
if (fingerprints.TryGetValue(scanned, out string svc))
    Console.WriteLine($"[+] Port {scanned} → {svc}");
// → [+] Port 443 → HTTPS

// Credential spray — user:pass pairs
Dictionary<string, string> creds = new Dictionary<string, string>
{
    { "admin",  "admin"    },
    { "root",   "toor"     },
    { "guest",  "guest"    },
    { "sa",     "sa"       }
};

foreach (var (user, pass) in creds)
{
    bool ok = TryLogin("10.0.0.1", user, pass);
    if (ok)
    {
        Console.WriteLine($"[+] Valid creds: {user}:{pass}");
        break;
    }
}

// Scan result tracker — host → open ports
Dictionary<string, List<int>> scanResults = new Dictionary<string, List<int>>();

string[] hosts = { "10.0.0.1", "10.0.0.2", "10.0.0.3" };
int[]    ports = { 22, 80, 443, 3389 };

foreach (string host in hosts)
{
    scanResults[host] = new List<int>();
    foreach (int port in ports)
    {
        if (IsPortOpen(host, port))
            scanResults[host].Add(port);
    }
}

// Print summary
foreach (var (host, openPorts) in scanResults)
{
    if (openPorts.Count > 0)
        Console.WriteLine($"[+] {host}: {string.Join(", ", openPorts)}");
}

// Blacklist — skip known safe hosts
Dictionary<string, string> whitelist = new Dictionary<string, string>
{
    { "10.0.0.254", "gateway" },
    { "10.0.0.1",  "router"  }
};

foreach (string host in hosts)
{
    if (whitelist.ContainsKey(host))
    {
        Console.WriteLine($"[-] Skipping {host} ({whitelist[host]})");
        continue;
    }
    // scan...
}
```

---

## Quick Reference

```csharp
var d = new Dictionary<string, int> { { "SSH", 22 } };

// ── Add ─────────────────────────────────────────────
d.Add("HTTP", 80);                 // throws if key exists
d["RDP"] = 3389;                   // overwrites if exists
d.TryAdd("FTP", 21);               // safe — no exception

// ── Remove ──────────────────────────────────────────
d.Remove("RDP");                   // by key
d.Remove("HTTP", out int p);       // remove + get value
d.Clear();                         // remove all

// ── Search ──────────────────────────────────────────
bool has  = d.ContainsKey("SSH");
bool hasV = d.ContainsValue(22);
d.TryGetValue("SSH", out int port);          // safe read
int val = d.GetValueOrDefault("FTP", 0);     // default if missing

// ── Info ────────────────────────────────────────────
int count = d.Count;
ICollection<string> keys   = d.Keys;
ICollection<int>    values = d.Values;

// ── Iterate ─────────────────────────────────────────
foreach (var (key, value) in d) { }
foreach (string key in d.Keys)   { }
foreach (int   val in d.Values)  { }
```
