# Step 17 — Tuple & ValueTuple

---

## Why Tuples?

```csharp
// Without tuple — can only return one value
string GetHost() => "10.0.0.1";    // can't also return port

// With tuple — return multiple values without a class
(string host, int port) GetTarget() => ("10.0.0.1", 443);

var t = GetTarget();
Console.WriteLine(t.host);          // → 10.0.0.1
Console.WriteLine(t.port);          // → 443
```

---

## Tuple vs ValueTuple — Key Difference

| | `Tuple<T>` (old) | `ValueTuple` (modern) |
|---|---|---|
| Syntax | `Tuple.Create(1, "a")` | `(1, "a")` |
| Access | `.Item1`, `.Item2` | Named fields or `.Item1` |
| Type | Reference type (heap) | Value type (stack) — faster |
| Mutable | ❌ No | ✅ Yes |
| Use case | Legacy / interop | Preferred in modern C# |

**Always prefer ValueTuple** (parenthesis syntax) in modern C#.

---

## ValueTuple — Named Fields (Preferred)

```csharp
// Declare with named fields
(string host, int port, bool open) result = ("10.0.0.1", 443, true);

Console.WriteLine(result.host);     // → 10.0.0.1
Console.WriteLine(result.port);     // → 443
Console.WriteLine(result.open);     // → True

// var works too
var scan = (host: "192.168.1.1", port: 80, service: "HTTP");
Console.WriteLine(scan.service);    // → HTTP

// Modify fields
scan.port = 8080;                   // ValueTuple is mutable
Console.WriteLine(scan.port);       // → 8080
```

---

## ValueTuple — Without Names (Positional)

```csharp
// No names — access via .Item1, .Item2, etc.
(string, int) pair = ("10.0.0.1", 22);

Console.WriteLine(pair.Item1);      // → 10.0.0.1
Console.WriteLine(pair.Item2);      // → 22

// Named names are just aliases — Item1/Item2 still work
(string host, int port) target = ("10.0.0.1", 443);
Console.WriteLine(target.Item1);    // → 10.0.0.1 (same as target.host)
```

---

## Returning Tuples from Methods

```csharp
// Named return type
(string service, string version) ParseBanner(string raw)
{
    if (raw.Contains("SSH"))
        return ("SSH", raw.Split('_')[1].Split(' ')[0]);
    if (raw.StartsWith("HTTP"))
        return ("HTTP", raw.Split(' ')[1]);
    return ("UNKNOWN", "");
}

var (svc, ver) = ParseBanner("OpenSSH_8.9p1 Ubuntu");
Console.WriteLine($"Service: {svc}, Version: {ver}");
// → Service: SSH, Version: 8.9p1

// Arrow syntax — single expression
(string host, int port, bool open) ScanPort(string ip, int p)
    => (ip, p, IsPortOpen(ip, p));

// Return multiple scan values
(int open, int closed, int filtered) ScanSubnet(string[] hosts, int port)
{
    int o = 0, c = 0, f = 0;
    foreach (string h in hosts)
    {
        if (IsPortOpen(h, port))   o++;
        else                       c++;
    }
    return (o, c, f);
}

var (open, closed, filtered) = ScanSubnet(hosts, 80);
Console.WriteLine($"Open: {open}, Closed: {closed}");
```

---

## Deconstruction — Unpacking Tuples

```csharp
// Deconstruct into new variables
(string host, int port) = ("10.0.0.1", 443);
Console.WriteLine(host);            // → 10.0.0.1
Console.WriteLine(port);            // → 443

// Deconstruct method return
var (service, version) = ParseBanner("OpenSSH_8.9p1");

// Discard with _ — ignore values you don't need
var (h, _, open) = ScanPort("10.0.0.1", 80);
Console.WriteLine($"{h} → {open}");

// Deconstruct into existing variables
string s;
int    p;
(s, p) = ("10.0.0.1", 22);

// In foreach — deconstruct tuples in a list
var results = new List<(string host, int port, bool open)>
{
    ("10.0.0.1", 22,  true),
    ("10.0.0.1", 80,  true),
    ("10.0.0.1", 443, false)
};

foreach (var (host, port, isOpen) in results)
{
    Console.WriteLine($"{host}:{port} → {(isOpen ? "OPEN" : "CLOSED")}");
}
```

---

## Tuples in Variables & Collections

```csharp
// Single variable
var target = (host: "10.0.0.1", port: 443);

// Array of tuples
(string host, int port)[] targets = {
    ("10.0.0.1", 22),
    ("10.0.0.2", 80),
    ("10.0.0.3", 443)
};

// List of tuples
var scanList = new List<(string host, int port, bool open)>();
scanList.Add(("10.0.0.1", 22, true));
scanList.Add(("10.0.0.1", 80, false));

// Dictionary with tuple value
var services = new Dictionary<int, (string name, string protocol)>
{
    { 22,  ("SSH",   "TCP") },
    { 53,  ("DNS",   "UDP") },
    { 80,  ("HTTP",  "TCP") },
    { 443, ("HTTPS", "TCP") }
};

foreach (var (port, (name, proto)) in services)
    Console.WriteLine($"{port,-6} {name,-8} {proto}");
```

---

## Tuple (Classic) — Old Syntax

```csharp
// System.Tuple — reference type, old style
Tuple<string, int> pair = new Tuple<string, int>("10.0.0.1", 443);
Tuple<string, int> pair2 = Tuple.Create("10.0.0.1", 443);

// Access — .Item1, .Item2 only (no named fields)
Console.WriteLine(pair.Item1);      // → 10.0.0.1
Console.WriteLine(pair.Item2);      // → 443

// Immutable — cannot modify
// pair.Item1 = "newvalue";         // ❌ compile error

// Up to 8 items — beyond 8 use TRest
Tuple<int, int, int, int, int, int, int, Tuple<int>> big =
    Tuple.Create(1, 2, 3, 4, 5, 6, 7, Tuple.Create(8));
```

---

## Pentest Context

```csharp
// Port scanner returning structured results
(string host, int port, string service, bool open) ScanPort(string ip, int p)
{
    bool isOpen = IsPortOpen(ip, p);
    string svc  = GetServiceName(p);
    return (ip, p, svc, isOpen);
}

// Collect all results
var scanResults = new List<(string host, int port, string service, bool open)>();

string[] hosts = { "10.0.0.1", "10.0.0.2" };
int[]    ports = { 22, 80, 443, 3389 };

foreach (string host in hosts)
    foreach (int port in ports)
        scanResults.Add(ScanPort(host, port));

// Filter and print open ports
foreach (var (host, port, service, open) in scanResults)
    if (open)
        Console.WriteLine($"[+] {host}:{port,-6} {service}");

// Banner parser — return service + version + extra info
(string service, string version, string os) ParseSSHBanner(string raw)
{
    // "SSH-2.0-OpenSSH_8.9p1 Ubuntu-3ubuntu0.6"
    string[] parts   = raw.Replace("SSH-2.0-", "").Split(' ');
    string[] svcParts= parts[0].Split('_');
    return (
        service: svcParts[0],
        version: svcParts.Length > 1 ? svcParts[1] : "",
        os:      parts.Length > 1 ? parts[1] : ""
    );
}

var (svc, ver, os) = ParseSSHBanner("SSH-2.0-OpenSSH_8.9p1 Ubuntu-3ubuntu0.6");
Console.WriteLine($"Service : {svc}");   // → OpenSSH
Console.WriteLine($"Version : {ver}");   // → 8.9p1
Console.WriteLine($"OS      : {os}");    // → Ubuntu-3ubuntu0.6

// Credential testing — return success + session info
(bool success, string token, string role) TryLogin(string user, string pass)
{
    // ... auth logic ...
    if (user == "admin" && pass == "admin")
        return (true, "tok_abc123", "administrator");
    return (false, "", "");
}

var (ok, token, role) = TryLogin("admin", "admin");
if (ok)
    Console.WriteLine($"[+] Logged in as {role}, token: {token}");

// Network topology — edge list as tuple list
var edges = new List<(string from, string to, int latency)>
{
    ("10.0.0.1", "10.0.0.2", 2),
    ("10.0.0.2", "10.0.0.3", 5),
    ("10.0.0.1", "10.0.0.3", 8)
};

foreach (var (from, to, lat) in edges)
    Console.WriteLine($"{from} → {to} ({lat}ms)");
```

---

## Quick Reference

```csharp
// ── Create ──────────────────────────────────────────
(string, int) t1    = ("host", 443);          // positional
var t2              = (host: "host", port: 443); // named
var t3              = ("host", 443);           // inferred

// ── Access ──────────────────────────────────────────
t1.Item1;                          // positional access
t2.host;                           // named access

// ── Modify (ValueTuple only) ─────────────────────────
t2.port = 8080;                    // mutable

// ── Deconstruct ─────────────────────────────────────
var (host, port)   = t2;           // into new vars
(host, port)       = t2;           // into existing vars
var (h, _, open)   = t3;           // discard with _

// ── In methods ──────────────────────────────────────
(string a, int b) MyMethod()  => ("value", 42);
var (a, b) = MyMethod();

// ── In collections ───────────────────────────────────
var list = new List<(string host, int port)>();
list.Add(("10.0.0.1", 80));
foreach (var (h, p) in list) { }

// ── Classic Tuple (legacy) ───────────────────────────
var old = Tuple.Create("host", 443);
old.Item1;                         // read-only
```
