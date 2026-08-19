# Step 13 — List\<T\>

---

## What is List\<T\>?

```csharp
// Array — fixed size, cannot grow
int[] ports = new int[3];          // always 3 elements

// List<T> — dynamic size, grows and shrinks at runtime
List<int> ports = new List<int>(); // starts empty, no size limit
```

`T` is the type placeholder — replace it with any type:

```csharp
List<string>  hosts    = new List<string>();
List<int>     ports    = new List<int>();
List<bool>    results  = new List<bool>();
List<byte>    payload  = new List<byte>();
```

---

## Declaring & Initializing

```csharp
// Empty list
List<string> hosts = new List<string>();

// With initial values
List<string> targets = new List<string> { "10.0.0.1", "10.0.0.2" };

// var shorthand
var ports = new List<int> { 22, 80, 443, 3389 };

// With initial capacity (optimization — not a size limit)
List<string> results = new List<string>(capacity: 100);
```

---

## Add() — Add Elements

```csharp
List<string> hosts = new List<string>();

hosts.Add("10.0.0.1");
hosts.Add("10.0.0.2");
hosts.Add("10.0.0.3");

// hosts → { "10.0.0.1", "10.0.0.2", "10.0.0.3" }

// AddRange — add multiple at once
hosts.AddRange(new[] { "192.168.1.1", "192.168.1.2" });

// Insert at specific index
hosts.Insert(0, "172.16.0.1");    // insert at beginning

// hosts → { "172.16.0.1", "10.0.0.1", "10.0.0.2", ... }
```

---

## Remove() — Remove Elements

```csharp
List<int> ports = new List<int> { 22, 80, 443, 3389, 8080 };

// Remove by value — removes first match
ports.Remove(80);
// → { 22, 443, 3389, 8080 }

// Remove by index
ports.RemoveAt(0);
// → { 443, 3389, 8080 }

// Remove range — start index, count
List<int> nums = new List<int> { 1, 2, 3, 4, 5 };
nums.RemoveRange(1, 3);           // remove 3 elements from index 1
// → { 1, 5 }

// Remove with condition
ports.RemoveAll(p => p > 1000);
// removes 3389 and 8080
```

---

## Contains() — Check Existence

```csharp
List<string> hosts = new List<string> { "10.0.0.1", "10.0.0.2", "192.168.1.1" };

bool found = hosts.Contains("10.0.0.2");
Console.WriteLine(found);             // → True

bool notFound = hosts.Contains("8.8.8.8");
Console.WriteLine(notFound);          // → False

// Check with condition
bool hasPrivate = hosts.Exists(h => h.StartsWith("192.168."));
Console.WriteLine(hasPrivate);        // → True

// Find index
int idx = hosts.IndexOf("10.0.0.2");
Console.WriteLine(idx);               // → 1

// Returns -1 if not found
int missing = hosts.IndexOf("8.8.8.8");
Console.WriteLine(missing);           // → -1
```

---

## Count — Element Count

```csharp
List<string> hosts = new List<string> { "10.0.0.1", "10.0.0.2", "10.0.0.3" };

Console.WriteLine(hosts.Count);       // → 3

hosts.Add("10.0.0.4");
Console.WriteLine(hosts.Count);       // → 4

hosts.Remove("10.0.0.1");
Console.WriteLine(hosts.Count);       // → 3

// Count is a property, not a method — no ()
// Array uses .Length, List uses .Count
```

---

## Clear() — Remove All Elements

```csharp
List<string> results = new List<string> { "SSH open", "HTTP open", "RDP open" };

Console.WriteLine(results.Count);     // → 3

results.Clear();

Console.WriteLine(results.Count);     // → 0
// List still exists, just empty
```

---

## Sort() — Sort Elements

```csharp
List<int> ports = new List<int> { 443, 22, 3389, 80, 8080 };

ports.Sort();
// → { 22, 80, 443, 3389, 8080 }

// Sort strings alphabetically
List<string> hosts = new List<string> { "beta.local", "alpha.local", "gamma.local" };
hosts.Sort();
// → { "alpha.local", "beta.local", "gamma.local" }

// Sort descending with custom comparer
ports.Sort((a, b) => b.CompareTo(a));
// → { 8080, 3389, 443, 80, 22 }

// Reverse after sort
ports.Sort();
ports.Reverse();
// → { 8080, 3389, 443, 80, 22 }
```

---

## Accessing Elements

```csharp
List<string> hosts = new List<string> { "10.0.0.1", "10.0.0.2", "10.0.0.3" };

// By index — same as array
Console.WriteLine(hosts[0]);          // → 10.0.0.1
Console.WriteLine(hosts[2]);          // → 10.0.0.3

// First and Last (using index)
Console.WriteLine(hosts[0]);               // first
Console.WriteLine(hosts[hosts.Count - 1]); // last

// Index from end
Console.WriteLine(hosts[^1]);         // → 10.0.0.3

// Modify by index
hosts[1] = "192.168.1.1";

// Loop — for
for (int i = 0; i < hosts.Count; i++)
    Console.WriteLine($"[{i}] {hosts[i]}");

// Loop — foreach
foreach (string host in hosts)
    Console.WriteLine($"[*] Scanning {host}");
```

---

## Useful Extra Methods

```csharp
List<int> ports = new List<int> { 22, 80, 443, 3389 };

// Find first match
int found = ports.Find(p => p > 100);
Console.WriteLine(found);             // → 443

// Find all matches
List<int> highPorts = ports.FindAll(p => p > 100);
// → { 443, 3389 }

// Convert to array
int[] arr = ports.ToArray();

// Check if any match
bool anyHigh = ports.Exists(p => p > 1000);
Console.WriteLine(anyHigh);           // → True

// Get a sub-range
List<int> sub = ports.GetRange(1, 2); // start=1, count=2
// → { 80, 443 }

// Min / Max
int min = ports.Min();                // → 22  (needs using System.Linq)
int max = ports.Max();                // → 3389
```

---

## List\<T\> vs Array

| Feature | `int[]` Array | `List<int>` |
|---|---|---|
| Size | Fixed at creation | Dynamic — grows/shrinks |
| Length/Count | `.Length` | `.Count` |
| Add elements | Not possible | `Add()`, `AddRange()` |
| Remove elements | Not possible | `Remove()`, `RemoveAt()` |
| Search | `Array.BinarySearch()` | `Contains()`, `Find()` |
| Performance | Slightly faster | Slightly more overhead |
| Use case | Known fixed size | Unknown or changing size |

```csharp
// Use array when size is fixed and known
int[] commonPorts = { 22, 80, 443 };

// Use List<T> when size changes at runtime
List<int> openPorts = new List<int>();
// — add ports as scan discovers them
```

---

## Pentest Context

```csharp
// Dynamic host collector
List<string> liveHosts = new List<string>();

string[] subnet = { "10.0.0.1", "10.0.0.2", "10.0.0.3", "10.0.0.4" };

foreach (string host in subnet)
{
    if (Ping(host))                   // assume Ping() returns bool
    {
        liveHosts.Add(host);
        Console.WriteLine($"[+] {host} is up");
    }
}

Console.WriteLine($"\n[*] Live hosts: {liveHosts.Count}");

// Port scan results stored in list
List<int> openPorts = new List<int>();
int[] targets = { 21, 22, 23, 80, 443, 3389, 8080 };

foreach (int port in targets)
{
    if (IsPortOpen("10.0.0.1", port))
        openPorts.Add(port);
}

openPorts.Sort();

Console.WriteLine("[+] Open ports:");
foreach (int p in openPorts)
    Console.WriteLine($"    {p}");

// Blacklist — remove known false positives
List<string> results = new List<string> { "SSH", "HTTP", "NetBIOS", "HTTP", "RDP" };

results.RemoveAll(r => r == "NetBIOS");    // strip noise
Console.WriteLine(results.Contains("SSH")); // → True
Console.WriteLine(results.Count);           // → 4

// Build shellcode dynamically
List<byte> shellcode = new List<byte>();

shellcode.AddRange(new byte[] { 0x90, 0x90, 0x90 });  // NOP sled
shellcode.AddRange(new byte[] { 0xCC, 0xC3 });         // INT3, RET

byte[] final = shellcode.ToArray();
Console.WriteLine(BitConverter.ToString(final));
// → 90-90-90-CC-C3

// Credential list — dedup and sort
List<string> creds = new List<string> { "admin:admin", "root:toor", "admin:1234", "root:root" };

creds.RemoveAll(c => c.StartsWith("admin"));  // drop admin entries
creds.Sort();

Console.WriteLine($"[*] Remaining creds: {creds.Count}");
foreach (string c in creds)
    Console.WriteLine($"    {c}");
// → root:root
// → root:toor
```

---

## Quick Reference

```csharp
var list = new List<string> { "a", "b", "c" };

// ── Add ─────────────────────────────────────────────
list.Add("d");                     // append
list.AddRange(new[]{"e","f"});     // append multiple
list.Insert(0, "z");               // insert at index

// ── Remove ──────────────────────────────────────────
list.Remove("a");                  // by value (first match)
list.RemoveAt(0);                  // by index
list.RemoveAll(x => x == "b");     // by condition
list.RemoveRange(0, 2);            // by index range
list.Clear();                      // remove all

// ── Search ──────────────────────────────────────────
bool has  = list.Contains("c");    // true / false
int  idx  = list.IndexOf("c");     // index or -1
bool any  = list.Exists(x => x.Length > 1);
string f  = list.Find(x => x == "c");
List<string> all = list.FindAll(x => x != "b");

// ── Info ────────────────────────────────────────────
int count = list.Count;            // element count

// ── Order ───────────────────────────────────────────
list.Sort();                       // ascending
list.Reverse();                    // flip order
list.Sort((a, b) => b.CompareTo(a)); // descending

// ── Convert ─────────────────────────────────────────
string[] arr  = list.ToArray();
List<string> sub = list.GetRange(1, 2); // start, count
```
