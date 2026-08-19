# Step 19 — Sorting & Searching Collections

---

## List.Sort() — Sort a List In-Place

```csharp
// Default sort — ascending (uses IComparable<T>)
List<int> ports = new List<int> { 443, 22, 3389, 80, 8080 };
ports.Sort();
// → { 22, 80, 443, 3389, 8080 }

List<string> hosts = new List<string> { "beta", "alpha", "gamma" };
hosts.Sort();
// → { "alpha", "beta", "gamma" }

// Sort modifies the list in-place — no return value
// If you need a sorted copy, use LINQ .OrderBy() instead

// Descending — reverse after sort
ports.Sort();
ports.Reverse();
// → { 8080, 3389, 443, 80, 22 }

// Sort with lambda comparer
ports.Sort((a, b) => b.CompareTo(a));   // descending in one step
// → { 8080, 3389, 443, 80, 22 }

// Sort strings by length
List<string> services = new List<string> { "SSH", "HTTP", "FTP", "HTTPS", "RDP" };
services.Sort((a, b) => a.Length.CompareTo(b.Length));
// → { SSH, FTP, RDP, HTTP, HTTPS }
```

---

## Array.Sort() — Sort an Array In-Place

```csharp
int[] ports = { 443, 22, 3389, 80, 8080 };

// Sort ascending
Array.Sort(ports);
// → { 22, 80, 443, 3389, 8080 }

// Sort descending — reverse after
Array.Sort(ports);
Array.Reverse(ports);
// → { 8080, 3389, 443, 80, 22 }

// Sort parallel arrays — sort ports and keep service names aligned
int[]    portNums  = { 443, 22, 80,   3389 };
string[] portNames = { "HTTPS", "SSH", "HTTP", "RDP" };

Array.Sort(portNums, portNames);    // sorts portNums, moves portNames to match
// portNums  → { 22, 80, 443, 3389 }
// portNames → { SSH, HTTP, HTTPS, RDP }

// Sort sub-range only — index, length
int[] nums = { 5, 3, 1, 4, 2 };
Array.Sort(nums, 1, 3);             // sort only index 1..3
// → { 5, 1, 3, 4, 2 }

// Sort with comparison delegate
string[] words = { "banana", "Apple", "cherry" };
Array.Sort(words, StringComparer.OrdinalIgnoreCase);
// → { Apple, banana, cherry }
```

---

## Array.BinarySearch() — Fast Sorted Search

```csharp
// ⚠️ Array MUST be sorted first — BinarySearch assumes sorted input
int[] ports = { 22, 80, 443, 3389, 8080 };
Array.Sort(ports);

// Returns index if found, negative number if not found
int idx = Array.BinarySearch(ports, 443);
Console.WriteLine(idx);            // → 2  (found at index 2)

int missing = Array.BinarySearch(ports, 9999);
Console.WriteLine(missing);        // → negative (not found)

// Safe check
int result = Array.BinarySearch(ports, 80);
if (result >= 0)
    Console.WriteLine($"Found 80 at index {result}");
else
    Console.WriteLine("Not found");

// BinarySearch on sub-range
int[] arr = { 1, 3, 5, 7, 9, 11, 13 };
int found = Array.BinarySearch(arr, 1, 4, 7); // search index 1..4
Console.WriteLine(found);          // → 3

// BinarySearch vs IndexOf
// Array.IndexOf — O(n) linear, works unsorted
// Array.BinarySearch — O(log n) fast, needs sorted array
```

---

## IComparer\<T\> — Custom Sorting Logic

```csharp
// Implement IComparer<T> for reusable, named sort logic
// CompareTo rules:
//   return < 0  → a comes before b
//   return 0    → a equals b
//   return > 0  → a comes after b

// Sort hosts by last octet of IP
class IPOctetComparer : IComparer<string>
{
    public int Compare(string? a, string? b)
    {
        int lastA = int.Parse(a!.Split('.')[3]);
        int lastB = int.Parse(b!.Split('.')[3]);
        return lastA.CompareTo(lastB);
    }
}

List<string> ips = new List<string>
{
    "10.0.0.5", "10.0.0.1", "10.0.0.22", "10.0.0.3"
};

ips.Sort(new IPOctetComparer());
// → 10.0.0.1, 10.0.0.3, 10.0.0.5, 10.0.0.22

// Sort by port priority — common pentest ports first
class PentestPortComparer : IComparer<int>
{
    private static readonly int[] priority = { 22, 80, 443, 3389, 445 };

    public int Compare(int a, int b)
    {
        int rankA = Array.IndexOf(priority, a);
        int rankB = Array.IndexOf(priority, b);

        // Not in priority list → rank = int.MaxValue (goes to end)
        if (rankA == -1) rankA = int.MaxValue;
        if (rankB == -1) rankB = int.MaxValue;

        return rankA.CompareTo(rankB);
    }
}

int[] ports = { 8080, 22, 3389, 443, 9090, 80 };
Array.Sort(ports, new PentestPortComparer());
// → { 22, 80, 443, 3389, 8080, 9090 }
```

---

## IComparable\<T\> — Make Your Class Sortable

```csharp
// Implement IComparable<T> so Sort() works natively on your type
class ScanResult : IComparable<ScanResult>
{
    public string Host    { get; set; } = "";
    public int    Port    { get; set; }
    public bool   IsOpen  { get; set; }

    // Define default sort order — by port ascending
    public int CompareTo(ScanResult? other)
    {
        if (other == null) return 1;
        return Port.CompareTo(other.Port);
    }

    public override string ToString() =>
        $"{Host}:{Port} [{(IsOpen ? "OPEN" : "CLOSED")}]";
}

List<ScanResult> results = new List<ScanResult>
{
    new ScanResult { Host = "10.0.0.1", Port = 443,  IsOpen = true  },
    new ScanResult { Host = "10.0.0.1", Port = 22,   IsOpen = true  },
    new ScanResult { Host = "10.0.0.1", Port = 3389, IsOpen = false },
    new ScanResult { Host = "10.0.0.1", Port = 80,   IsOpen = true  }
};

results.Sort();                     // uses CompareTo — sort by port
results.ForEach(r => Console.WriteLine(r));
// → 10.0.0.1:22 [OPEN]
// → 10.0.0.1:80 [OPEN]
// → 10.0.0.1:443 [OPEN]
// → 10.0.0.1:3389 [CLOSED]
```

---

## Comparison\<T\> Delegate — Inline Comparer

```csharp
List<string> hosts = new List<string>
{
    "192.168.1.10", "10.0.0.1", "172.16.0.5", "192.168.1.2"
};

// Inline sort logic via Comparison<T> delegate
hosts.Sort((a, b) =>
{
    // Sort by network class: 10.x < 172.x < 192.x
    string prefixA = string.Join(".", a.Split('.').Take(2));
    string prefixB = string.Join(".", b.Split('.').Take(2));
    return string.Compare(prefixA, prefixB, StringComparison.Ordinal);
});

// Multikey sort — open ports first, then by port number
List<(string host, int port, bool open)> scanData = new()
{
    ("10.0.0.1", 443,  true),
    ("10.0.0.1", 22,   true),
    ("10.0.0.1", 9090, false),
    ("10.0.0.1", 80,   true),
    ("10.0.0.1", 8080, false)
};

scanData.Sort((a, b) =>
{
    // Open ports first
    int byOpen = b.open.CompareTo(a.open);
    if (byOpen != 0) return byOpen;

    // Then by port ascending
    return a.port.CompareTo(b.port);
});

foreach (var (host, port, open) in scanData)
    Console.WriteLine($"{host}:{port,-6} {(open ? "[+]" : "[-]")}");
// → 10.0.0.1:22   [+]
// → 10.0.0.1:80   [+]
// → 10.0.0.1:443  [+]
// → 10.0.0.1:8080 [-]
// → 10.0.0.1:9090 [-]
```

---

## LINQ Sorting — OrderBy / ThenBy

```csharp
using System.Linq;

List<int> ports = new List<int> { 443, 22, 3389, 80, 8080 };

// OrderBy — ascending (returns new collection, no in-place change)
var sorted   = ports.OrderBy(p => p).ToList();
var desc     = ports.OrderByDescending(p => p).ToList();

// ThenBy — multi-key sort
var data = new List<(string host, int port)>
{
    ("10.0.0.2", 80), ("10.0.0.1", 443), ("10.0.0.1", 22), ("10.0.0.2", 22)
};

var ordered = data
    .OrderBy(x => x.host)
    .ThenBy(x => x.port)
    .ToList();

foreach (var (h, p) in ordered)
    Console.WriteLine($"{h}:{p}");
// → 10.0.0.1:22
// → 10.0.0.1:443
// → 10.0.0.2:22
// → 10.0.0.2:80
```

---

## Searching Methods Compared

| Method | Collection | Sorted? | Speed | Returns |
|---|---|---|---|---|
| `Array.BinarySearch()` | Array | ✅ Required | O(log n) | index or negative |
| `Array.IndexOf()` | Array | ❌ No | O(n) | index or -1 |
| `List.IndexOf()` | List | ❌ No | O(n) | index or -1 |
| `List.BinarySearch()` | List | ✅ Required | O(log n) | index or negative |
| `List.Find()` | List | ❌ No | O(n) | element or default |
| `List.FindIndex()` | List | ❌ No | O(n) | index or -1 |
| `HashSet.Contains()` | HashSet | ❌ No | O(1) | bool |

```csharp
// List also has BinarySearch
List<int> sorted = new List<int> { 22, 80, 443, 3389 };
int idx = sorted.BinarySearch(443);   // → 2
```

---

## Quick Reference

```csharp
// ── List.Sort() ──────────────────────────────────────
list.Sort();                           // default ascending
list.Sort((a, b) => b.CompareTo(a));  // descending lambda
list.Sort(new MyComparer());           // custom IComparer<T>
list.Reverse();                        // flip order in-place

// ── Array.Sort() ─────────────────────────────────────
Array.Sort(arr);                       // ascending
Array.Sort(arr, new MyComparer());     // custom comparer
Array.Sort(keys, values);             // parallel arrays
Array.Sort(arr, startIndex, length);  // sub-range
Array.Reverse(arr);                   // reverse in-place

// ── Array.BinarySearch() — array MUST be sorted ──────
int idx = Array.BinarySearch(arr, target);
if (idx >= 0) { /* found at idx */ }
else          { /* not found     */ }

// ── IComparer<T> ────────────────────────────────────
class MyComparer : IComparer<int>
{
    public int Compare(int a, int b) => a.CompareTo(b);
}

// ── IComparable<T> — make type self-sortable ─────────
class MyType : IComparable<MyType>
{
    public int Value { get; set; }
    public int CompareTo(MyType? other) =>
        Value.CompareTo(other?.Value ?? 0);
}

// ── LINQ (non-mutating) ──────────────────────────────
var asc  = list.OrderBy(x => x).ToList();
var desc = list.OrderByDescending(x => x).ToList();
var multi= list.OrderBy(x => x.A).ThenBy(x => x.B).ToList();
```
