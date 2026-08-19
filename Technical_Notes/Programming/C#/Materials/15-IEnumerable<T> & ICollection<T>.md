# Step 18 — IEnumerable\<T\> & ICollection\<T\>

---

## The Collection Interface Hierarchy

```
IEnumerable<T>          ← base of ALL iteration
    └── ICollection<T>  ← adds Count, Add, Remove, Contains
            ├── IList<T>       → List<T>, arrays
            └── ISet<T>        → HashSet<T>
            └── IDictionary<TKey,TValue> → Dictionary<T,T>
```

Every collection in C# implements `IEnumerable<T>` at minimum.
That is why `foreach` works on **all** of them.

---

## IEnumerable\<T\> — The Iteration Interface

```csharp
// IEnumerable<T> has exactly ONE method:
// IEnumerator<T> GetEnumerator();

// This is why foreach works on everything
// foreach internally calls GetEnumerator()

// All of these are IEnumerable<string>
List<string>    list    = new List<string>    { "a", "b", "c" };
string[]        array   = { "a", "b", "c" };
HashSet<string> hashset = new HashSet<string> { "a", "b", "c" };
Queue<string>   queue   = new Queue<string>   (new[]{"a","b","c"});

// foreach works on all of them — same interface
foreach (string item in list)    Console.WriteLine(item);
foreach (string item in array)   Console.WriteLine(item);
foreach (string item in hashset) Console.WriteLine(item);
foreach (string item in queue)   Console.WriteLine(item);
```

---

## Why Use IEnumerable\<T\> as a Parameter Type?

```csharp
// ❌ Too restrictive — only accepts List<string>
void PrintHosts(List<string> hosts)
{
    foreach (string h in hosts)
        Console.WriteLine(h);
}

// ✅ Flexible — accepts ANY collection
void PrintHosts(IEnumerable<string> hosts)
{
    foreach (string h in hosts)
        Console.WriteLine(h);
}

// Now all of these work
PrintHosts(new List<string>    { "10.0.0.1", "10.0.0.2" });
PrintHosts(new string[]        { "10.0.0.1", "10.0.0.2" });
PrintHosts(new HashSet<string> { "10.0.0.1", "10.0.0.2" });
PrintHosts(new Queue<string>   (new[]{ "10.0.0.1" }));

// Rule: accept the least specific type that meets your needs
// If you only iterate  → IEnumerable<T>
// If you need Count    → ICollection<T>
// If you need indexing → IList<T>
```

---

## IEnumerable\<T\> — Lazy Evaluation

```csharp
// IEnumerable is LAZY — values computed only when requested
// No work happens until you iterate

IEnumerable<int> GetPorts()
{
    Console.WriteLine("Starting...");
    yield return 22;
    Console.WriteLine("After 22...");
    yield return 80;
    Console.WriteLine("After 80...");
    yield return 443;
}

// Nothing printed yet — iterator not started
IEnumerable<int> ports = GetPorts();

// Iteration starts only here
foreach (int p in ports)
{
    Console.WriteLine($"Got: {p}");
}
// → Starting...
// → Got: 22
// → After 22...
// → Got: 80
// → After 80...
// → Got: 443
```

---

## yield return — Custom Iterators

```csharp
// yield return lets a method produce a sequence lazily
IEnumerable<string> GenerateIPs(string prefix, int start, int end)
{
    for (int i = start; i <= end; i++)
        yield return $"{prefix}.{i}";
}

// Nothing computed until iterated
foreach (string ip in GenerateIPs("10.0.0", 1, 254))
    Console.WriteLine(ip);
// → 10.0.0.1
// → 10.0.0.2
// → ...
// → 10.0.0.254

// yield break — stop the sequence early
IEnumerable<int> OpenPortsOnly(string host, int[] ports)
{
    foreach (int p in ports)
    {
        if (IsPortOpen(host, p))
            yield return p;
        // skips closed ports automatically
    }
}
```

---

## ICollection\<T\> — Adds Mutation

```csharp
// ICollection<T> extends IEnumerable<T> with:
// int  Count       { get; }
// bool IsReadOnly  { get; }
// void Add(T item);
// void Clear();
// bool Contains(T item);
// void CopyTo(T[] array, int arrayIndex);
// bool Remove(T item);

// Accept any writable collection
void AddScanResult(ICollection<string> results, string host)
{
    if (!results.Contains(host))
        results.Add(host);
    Console.WriteLine($"[*] Total: {results.Count}");
}

// Works with List, HashSet — both implement ICollection
List<string>    list    = new List<string>();
HashSet<string> hashset = new HashSet<string>();

AddScanResult(list,    "10.0.0.1");  // works
AddScanResult(hashset, "10.0.0.1");  // works

// Arrays are IEnumerable but NOT ICollection (fixed size)
// string[] array — cannot Add/Remove
```

---

## IList\<T\> — Adds Index Access

```csharp
// IList<T> extends ICollection<T> with:
// T    this[int index] { get; set; }
// int  IndexOf(T item);
// void Insert(int index, T item);
// void RemoveAt(int index);

void SwapFirst(IList<string> items)
{
    if (items.Count < 2) return;
    (items[0], items[1]) = (items[1], items[0]);
}

List<string> hosts = new List<string> { "10.0.0.1", "10.0.0.2", "10.0.0.3" };
SwapFirst(hosts);
Console.WriteLine(hosts[0]);        // → 10.0.0.2

// Arrays also implement IList<T>
string[] arr = { "a", "b", "c" };
SwapFirst(arr);
Console.WriteLine(arr[0]);          // → b
```

---

## Interface Comparison

| Interface | Iterate | Count | Add/Remove | Index `[i]` | Implementors |
|---|---|---|---|---|---|
| `IEnumerable<T>` | ✅ | ❌ | ❌ | ❌ | All collections |
| `ICollection<T>` | ✅ | ✅ | ✅ | ❌ | List, HashSet |
| `IList<T>` | ✅ | ✅ | ✅ | ✅ | List, Array |
| `IDictionary<K,V>` | ✅ | ✅ | ✅ | `[key]` | Dictionary |

---

## Pentest Context

```csharp
// Accept any collection as input — maximum flexibility
bool ContainsPrivateIP(IEnumerable<string> ips)
{
    foreach (string ip in ips)
    {
        if (ip.StartsWith("192.168.") ||
            ip.StartsWith("10.")      ||
            ip.StartsWith("172.16."))
            return true;
    }
    return false;
}

// Works with any collection type
List<string>    fromList    = new List<string>    { "8.8.8.8", "192.168.1.1" };
string[]        fromArray   = { "10.0.0.1", "1.1.1.1" };
HashSet<string> fromHashset = new HashSet<string> { "172.16.0.1" };

Console.WriteLine(ContainsPrivateIP(fromList));    // → True
Console.WriteLine(ContainsPrivateIP(fromArray));   // → True
Console.WriteLine(ContainsPrivateIP(fromHashset)); // → True

// Lazy port range generator — memory efficient, no array needed
IEnumerable<int> PortRange(int start, int end)
{
    for (int p = start; p <= end; p++)
        yield return p;
}

// Scans ports 1-1024 without allocating an array
foreach (int port in PortRange(1, 1024))
{
    if (IsPortOpen("10.0.0.1", port))
        Console.WriteLine($"[+] {port} open");
}

// Filter with IEnumerable — build pipeline
IEnumerable<string> FilterPrivate(IEnumerable<string> ips)
{
    foreach (string ip in ips)
        if (ip.StartsWith("10.") || ip.StartsWith("192.168."))
            yield return ip;
}

IEnumerable<string> FilterAlive(IEnumerable<string> ips)
{
    foreach (string ip in ips)
        if (Ping(ip))
            yield return ip;
}

// Chain filters — lazy, no intermediate lists
string[] allIPs = { "10.0.0.1", "8.8.8.8", "10.0.0.2", "1.1.1.1" };

foreach (string ip in FilterAlive(FilterPrivate(allIPs)))
    Console.WriteLine($"[+] Live private host: {ip}");

// Accept writable collection — populate from scan
void CollectLiveHosts(string[] subnet, ICollection<string> output)
{
    foreach (string host in subnet)
        if (Ping(host))
            output.Add(host);
}

List<string>    liveList = new List<string>();
HashSet<string> liveSet  = new HashSet<string>();

CollectLiveHosts(new[]{"10.0.0.1","10.0.0.2"}, liveList);
CollectLiveHosts(new[]{"10.0.0.1","10.0.0.2"}, liveSet);
```

---

## Quick Reference

```csharp
// ── Parameter type selection ─────────────────────────
void Scan(IEnumerable<string> hosts)   { }  // read-only iteration
void Fill(ICollection<string> output)  { }  // need Add/Count
void Swap(IList<string> items)         { }  // need index access

// ── yield return iterator ────────────────────────────
IEnumerable<int> Range(int a, int b)
{
    for (int i = a; i <= b; i++)
        yield return i;
}

// ── yield break ──────────────────────────────────────
IEnumerable<int> TakeWhile(IEnumerable<int> src, int max)
{
    foreach (int x in src)
    {
        if (x > max) yield break;
        yield return x;
    }
}

// ── Accepted everywhere IEnumerable is expected ──────
IEnumerable<string> e = new List<string>();
e = new string[] { };
e = new HashSet<string>();
e = new Queue<string>();
```
