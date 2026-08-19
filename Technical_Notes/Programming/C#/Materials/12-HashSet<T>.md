# Step 15 — HashSet\<T\>

---

## What is HashSet\<T\>?

```csharp
// List — allows duplicates
List<string> hosts = new List<string> { "10.0.0.1", "10.0.0.1", "10.0.0.2" };
Console.WriteLine(hosts.Count);     // → 3 (duplicate kept)

// HashSet — unique elements only, duplicates silently ignored
HashSet<string> unique = new HashSet<string> { "10.0.0.1", "10.0.0.1", "10.0.0.2" };
Console.WriteLine(unique.Count);    // → 2 (duplicate removed)
```

**Key properties:**
- No duplicates — every element is unique
- No guaranteed order — elements are not sorted
- Very fast lookups — `O(1)` for `Contains()`

---

## Declaring & Initializing

```csharp
// Empty HashSet
HashSet<string> hosts   = new HashSet<string>();
HashSet<int>    ports   = new HashSet<int>();

// With initial values
HashSet<string> targets = new HashSet<string> { "10.0.0.1", "10.0.0.2", "192.168.1.1" };

// var shorthand
var seen = new HashSet<int> { 22, 80, 443 };

// From existing List — auto deduplicates
List<string> raw = new List<string> { "admin", "root", "admin", "guest", "root" };
HashSet<string> users = new HashSet<string>(raw);
Console.WriteLine(users.Count);     // → 3
```

---

## Add() — Add Elements

```csharp
HashSet<int> ports = new HashSet<int>();

// Add returns bool — true if added, false if duplicate
bool r1 = ports.Add(22);
Console.WriteLine(r1);              // → True  (added)

bool r2 = ports.Add(80);
Console.WriteLine(r2);              // → True  (added)

bool r3 = ports.Add(22);
Console.WriteLine(r3);              // → False (duplicate, ignored)

Console.WriteLine(ports.Count);     // → 2

// No AddRange — use UnionWith or constructor
ports.UnionWith(new[] { 443, 3389, 8080 });

// Or use constructor
var more = new HashSet<int>(new[] { 21, 22, 23 });
```

---

## Contains() — Check Existence

```csharp
HashSet<string> hosts = new HashSet<string> { "10.0.0.1", "10.0.0.2", "192.168.1.1" };

bool found = hosts.Contains("10.0.0.2");
Console.WriteLine(found);           // → True

bool miss = hosts.Contains("8.8.8.8");
Console.WriteLine(miss);            // → False

// HashSet.Contains is O(1) — much faster than List.Contains for large sets
// List searches linearly O(n), HashSet uses hashing O(1)
```

---

## Remove Elements

```csharp
HashSet<int> ports = new HashSet<int> { 22, 80, 443, 3389 };

// Remove by value — returns bool
bool removed = ports.Remove(3389);
Console.WriteLine(removed);         // → True

bool miss = ports.Remove(9999);
Console.WriteLine(miss);            // → False

// Remove all matching condition
ports.RemoveWhere(p => p > 100);
// → { 22, 80 }

// Clear all
ports.Clear();
Console.WriteLine(ports.Count);     // → 0
```

---

## Set Operations

### UnionWith() — combine two sets (OR)

```csharp
// All elements from both sets
HashSet<int> setA = new HashSet<int> { 22, 80, 443 };
HashSet<int> setB = new HashSet<int> { 80, 443, 3389 };

setA.UnionWith(setB);
// setA → { 22, 80, 443, 3389 }
```

### IntersectWith() — common elements (AND)

```csharp
// Only elements that exist in BOTH sets
HashSet<int> setA = new HashSet<int> { 22, 80, 443 };
HashSet<int> setB = new HashSet<int> { 80, 443, 3389 };

setA.IntersectWith(setB);
// setA → { 80, 443 }
```

### ExceptWith() — subtract a set (difference)

```csharp
// Elements in setA but NOT in setB
HashSet<int> setA = new HashSet<int> { 22, 80, 443, 3389 };
HashSet<int> setB = new HashSet<int> { 80, 443 };

setA.ExceptWith(setB);
// setA → { 22, 3389 }
```

### SymmetricExceptWith() — elements in one but not both (XOR)

```csharp
// Elements that are in EITHER set but NOT BOTH
HashSet<int> setA = new HashSet<int> { 22, 80, 443 };
HashSet<int> setB = new HashSet<int> { 80, 443, 3389 };

setA.SymmetricExceptWith(setB);
// setA → { 22, 3389 }
```

---

## Set Comparison Methods

```csharp
HashSet<int> setA = new HashSet<int> { 22, 80, 443 };
HashSet<int> setB = new HashSet<int> { 22, 80, 443, 3389 };
HashSet<int> setC = new HashSet<int> { 22, 80, 443 };

// IsSubsetOf — is A fully inside B?
Console.WriteLine(setA.IsSubsetOf(setB));       // → True

// IsSupersetOf — does A contain all of B?
Console.WriteLine(setB.IsSupersetOf(setA));     // → True

// SetEquals — same elements?
Console.WriteLine(setA.SetEquals(setC));        // → True

// Overlaps — any common elements?
Console.WriteLine(setA.Overlaps(new[] { 22, 9999 }));  // → True
```

---

## Iterating

```csharp
HashSet<string> hosts = new HashSet<string> { "10.0.0.1", "10.0.0.2", "192.168.1.1" };

// foreach — no index, order not guaranteed
foreach (string host in hosts)
    Console.WriteLine($"[*] {host}");

// Count
Console.WriteLine(hosts.Count);     // → 3

// Convert to sorted list when order matters
List<string> sorted = new List<string>(hosts);
sorted.Sort();
foreach (string h in sorted)
    Console.WriteLine(h);
```

---

## HashSet\<T\> vs List\<T\>

| Feature | `List<T>` | `HashSet<T>` |
|---|---|---|
| Duplicates | Allowed | Not allowed |
| Order | Preserved | Not guaranteed |
| `Contains()` speed | O(n) slow | O(1) fast |
| Index access | `list[i]` ✅ | Not supported ❌ |
| Add return value | void | bool (true if new) |
| Use case | Ordered data, duplicates OK | Unique values, fast lookup |

---

## Pentest Context

```csharp
// Dedup discovered hosts
List<string> rawHosts = new List<string>
{
    "10.0.0.1", "10.0.0.2", "10.0.0.1", "10.0.0.3", "10.0.0.2"
};

HashSet<string> uniqueHosts = new HashSet<string>(rawHosts);
Console.WriteLine($"[*] Unique hosts: {uniqueHosts.Count}");  // → 3

// Track already-scanned hosts (avoid rescanning)
HashSet<string> scanned = new HashSet<string>();

string[] targets = { "10.0.0.1", "10.0.0.2", "10.0.0.1" };
foreach (string host in targets)
{
    if (!scanned.Add(host))             // Add returns false if already present
    {
        Console.WriteLine($"[-] Already scanned: {host}");
        continue;
    }
    Console.WriteLine($"[*] Scanning: {host}");
    // scan...
}

// Compare two scan runs — find new open ports
HashSet<int> lastScan    = new HashSet<int> { 22, 80, 443 };
HashSet<int> currentScan = new HashSet<int> { 22, 80, 443, 3389, 8080 };

HashSet<int> newPorts = new HashSet<int>(currentScan);
newPorts.ExceptWith(lastScan);

Console.WriteLine($"[!] New open ports: {string.Join(", ", newPorts)}");
// → [!] New open ports: 3389, 8080

// Find ports open on ALL hosts (intersection)
HashSet<int> host1Ports = new HashSet<int> { 22, 80, 443, 3389 };
HashSet<int> host2Ports = new HashSet<int> { 22, 80, 8080 };
HashSet<int> host3Ports = new HashSet<int> { 22, 443, 3389 };

HashSet<int> common = new HashSet<int>(host1Ports);
common.IntersectWith(host2Ports);
common.IntersectWith(host3Ports);

Console.WriteLine($"[*] Common open ports: {string.Join(", ", common)}");
// → [*] Common open ports: 22

// Blacklist filter — remove known good IPs
HashSet<string> whitelist = new HashSet<string> { "10.0.0.1", "10.0.0.254" };
HashSet<string> allHosts  = new HashSet<string> { "10.0.0.1", "10.0.0.2", "10.0.0.3", "10.0.0.254" };

allHosts.ExceptWith(whitelist);
Console.WriteLine($"[*] Targets after filter: {string.Join(", ", allHosts)}");
// → [*] Targets after filter: 10.0.0.2, 10.0.0.3

// Unique user accounts from multiple sources
HashSet<string> adUsers   = new HashSet<string> { "admin", "john", "jane" };
HashSet<string> localUsers= new HashSet<string> { "admin", "root", "guest" };

adUsers.UnionWith(localUsers);
Console.WriteLine($"[*] All unique users: {string.Join(", ", adUsers)}");
// → admin, john, jane, root, guest
```

---

## Quick Reference

```csharp
var s = new HashSet<int> { 1, 2, 3 };

// ── Add ─────────────────────────────────────────────
bool added = s.Add(4);             // true if new, false if duplicate
s.UnionWith(new[] { 5, 6 });       // add multiple

// ── Remove ──────────────────────────────────────────
s.Remove(1);                       // by value
s.RemoveWhere(x => x > 4);        // by condition
s.Clear();                         // remove all

// ── Search ──────────────────────────────────────────
bool has = s.Contains(3);          // O(1) fast

// ── Set Operations ───────────────────────────────────
s.UnionWith(other);                // A ∪ B  (OR)
s.IntersectWith(other);            // A ∩ B  (AND)
s.ExceptWith(other);               // A - B  (subtract)
s.SymmetricExceptWith(other);      // A △ B  (XOR)

// ── Comparison ──────────────────────────────────────
s.IsSubsetOf(other);               // A ⊆ B
s.IsSupersetOf(other);             // A ⊇ B
s.SetEquals(other);                // A == B
s.Overlaps(other);                 // any common?

// ── Info ────────────────────────────────────────────
int count = s.Count;
```
