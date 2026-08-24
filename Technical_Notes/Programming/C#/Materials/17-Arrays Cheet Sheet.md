# C# Collections Cheat Sheet

---

## Array

Fixed-size, index-based collection.

| Method / Property | What it does | Example |
|---|---|---|
| `Array.Sort(arr)` | Sort ascending | `Array.Sort(ports)` → `[22, 80, 443]` |
| `Array.Reverse(arr)` | Reverse in place | `Array.Reverse(ports)` → `[443, 80, 22]` |
| `Array.BinarySearch(arr, val)` | Find index (sorted array) | `Array.BinarySearch(ports, 443)` → `2` |
| `Array.Exists(arr, predicate)` | Any element matches? | `Array.Exists(ports, p => p == 80)` → `true` |
| `arr.Length` | Element count | `ports.Length` → `5` |
| `arr[i]` | Access by index | `ports[0]` → `22` |

```csharp
int[] ports = { 22, 80, 443 };
Array.Sort(ports);
bool found = Array.Exists(ports, p => p == 80); // true
```

---

## List\<T\>

Dynamic array — grows/shrinks at runtime.

| Method / Property | What it does | Example |
|---|---|---|
| `.Add(item)` | Append to end | `hosts.Add("10.0.0.1")` |
| `.AddRange(items)` | Append multiple | `hosts.AddRange(new[] { "192.168.1.1" })` |
| `.Insert(i, item)` | Insert at index | `hosts.Insert(0, "172.16.0.1")` |
| `.Remove(item)` | Remove first match | `ports.Remove(80)` |
| `.RemoveAt(i)` | Remove by index | `ports.RemoveAt(0)` |
| `.RemoveRange(i, count)` | Remove slice | `ports.RemoveRange(1, 3)` |
| `.RemoveAll(predicate)` | Remove all matches | `ports.RemoveAll(p => p > 1000)` |
| `.Contains(item)` | Exact match exists? | `hosts.Contains("10.0.0.2")` → `true` |
| `.Exists(predicate)` | Any match? | `hosts.Exists(h => h.StartsWith("192."))` |
| `.IndexOf(item)` | First match index | `hosts.IndexOf("10.0.0.2")` → `1` |
| `.Sort()` | Sort ascending | `ports.Sort()` |
| `.Reverse()` | Reverse in place | `ports.Reverse()` |
| `.Count` | Element count | `hosts.Count` → `3` |

```csharp
List<string> hosts = new List<string> { "10.0.0.1", "10.0.0.2" };
hosts.Add("192.168.1.1");
hosts.RemoveAll(h => h.StartsWith("192."));
bool found = hosts.Contains("10.0.0.2"); // true
```

---

## Dictionary\<TKey, TValue\>

Key-value store — O(1) lookup.

| Method / Property | What it does | Example |
|---|---|---|
| `dict[key] = val` | Add or update | `ports["SSH"] = 22` |
| `.Add(key, val)` | Add (throws if exists) | `services.Add("SSH", 22)` |
| `.Remove(key)` | Delete by key | `services.Remove("RDP")` → `true` |
| `.ContainsKey(key)` | Key exists? | `services.ContainsKey("SSH")` → `true` |
| `.ContainsValue(val)` | Value exists? | `services.ContainsValue(80)` → `true` |
| `.TryGetValue(key, out val)` | Safe get (no exception) | `services.TryGetValue("HTTP", out int p)` |
| `.Keys` | All keys | `services.Keys` → `["SSH", "HTTP"]` |
| `.Values` | All values | `services.Values` → `[22, 80]` |
| `.Count` | Pair count | `services.Count` → `4` |

```csharp
var services = new Dictionary<string, int> { { "SSH", 22 }, { "HTTP", 80 } };
foreach (var (service, port) in services)
    Console.WriteLine($"{service}: {port}");
```

---

## HashSet\<T\>

Unique elements only — no duplicates, no order.

| Method / Property | What it does | Example |
|---|---|---|
| `.Add(item)` | Add (ignores duplicate) | `ports.Add(22)` |
| `.Remove(item)` | Remove element | `ports.Remove(22)` |
| `.RemoveWhere(predicate)` | Remove all matches | `ports.RemoveWhere(p => p > 100)` |
| `.Contains(item)` | Exists? | `hosts.Contains("10.0.0.2")` → `true` |
| `.UnionWith(other)` | Add all from other | `setA.UnionWith(setB)` → `{22, 80, 443, 3389}` |
| `.IntersectWith(other)` | Keep common only | `setA.IntersectWith(setB)` → `{80, 443}` |
| `.ExceptWith(other)` | Remove other's items | `setA.ExceptWith(setB)` → `{22, 3389}` |
| `.Count` | Element count | `unique.Count` → `2` |

```csharp
HashSet<int> setA = new HashSet<int> { 22, 80, 443 };
HashSet<int> setB = new HashSet<int> { 80, 443, 3389 };
setA.IntersectWith(setB); // setA → { 80, 443 }
```

---

## Stack\<T\>

LIFO — last in, first out.

| Method / Property | What it does | Example |
|---|---|---|
| `.Push(item)` | Add to top | `commands.Push("cmd4")` |
| `.Pop()` | Remove & return top | `commands.Pop()` → `"cmd4"` |
| `.Peek()` | View top (no remove) | `commands.Peek()` → `"cmd3"` |
| `.Contains(item)` | Exists? | `commands.Contains("cmd1")` → `true` |
| `.Count` | Element count | `commands.Count` → `3` |

```csharp
Stack<string> commands = new Stack<string>(new[] { "cmd1", "cmd2", "cmd3" });
// TOP → cmd3, cmd2, cmd1 → BOTTOM
commands.Push("cmd4");
string last = commands.Pop(); // "cmd4"
```

---

## Queue\<T\>

FIFO — first in, first out.

| Method / Property | What it does | Example |
|---|---|---|
| `.Enqueue(item)` | Add to back | `targets.Enqueue("10.0.0.3")` |
| `.Dequeue()` | Remove & return front | `targets.Dequeue()` → `"10.0.0.1"` |
| `.Peek()` | View front (no remove) | `targets.Peek()` → `"10.0.0.1"` |
| `.Contains(item)` | Exists? | `targets.Contains("10.0.0.2")` → `true` |
| `.Count` | Element count | `targets.Count` → `3` |

```csharp
Queue<string> targets = new Queue<string>();
targets.Enqueue("10.0.0.1");
targets.Enqueue("10.0.0.2");
string first = targets.Dequeue(); // "10.0.0.1"
```
