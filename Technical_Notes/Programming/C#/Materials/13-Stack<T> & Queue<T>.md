# Step 16 — Stack\<T\> & Queue\<T\>

---

## Stack\<T\> — LIFO (Last In, First Out)

```
Push →  [ 3 | 2 | 1 ]  ← TOP
Pop  ←  [ 3 | 2 | 1 ]  ← TOP

Last item pushed is first item popped.
Think: a stack of plates — you take from the top.
```

---

### Declaring & Initializing

```csharp
// Empty stack
Stack<string> callStack = new Stack<string>();
Stack<int>    history   = new Stack<int>();

// With initial values — first item ends up at bottom
Stack<string> commands = new Stack<string>(new[] { "cmd1", "cmd2", "cmd3" });
// TOP → cmd3, cmd2, cmd1 → BOTTOM
```

---

### Push() — Add to Top

```csharp
Stack<string> visited = new Stack<string>();

visited.Push("10.0.0.1");
visited.Push("10.0.0.2");
visited.Push("10.0.0.3");

// Stack (top to bottom): 10.0.0.3 | 10.0.0.2 | 10.0.0.1
Console.WriteLine(visited.Count);   // → 3
```

---

### Pop() — Remove from Top

```csharp
Stack<string> visited = new Stack<string>();
visited.Push("10.0.0.1");
visited.Push("10.0.0.2");
visited.Push("10.0.0.3");

string last = visited.Pop();
Console.WriteLine(last);            // → 10.0.0.3  (last pushed)
Console.WriteLine(visited.Count);   // → 2

// Pop() throws if stack is empty
// ✅ Safe pop
if (visited.Count > 0)
    visited.Pop();
```

---

### Peek() — View Top Without Removing

```csharp
Stack<int> ports = new Stack<int>();
ports.Push(22);
ports.Push(80);
ports.Push(443);

int top = ports.Peek();
Console.WriteLine(top);             // → 443
Console.WriteLine(ports.Count);     // → 3 (still 3 — not removed)

// TryPeek / TryPop — safe, no exception
if (ports.TryPeek(out int peeked))
    Console.WriteLine($"Top: {peeked}");

if (ports.TryPop(out int popped))
    Console.WriteLine($"Popped: {popped}");
```

---

### Contains() & Iterating

```csharp
Stack<string> hosts = new Stack<string>();
hosts.Push("10.0.0.1");
hosts.Push("10.0.0.2");
hosts.Push("10.0.0.3");

bool has = hosts.Contains("10.0.0.2");
Console.WriteLine(has);             // → True

// foreach — iterates top to bottom
foreach (string h in hosts)
    Console.WriteLine(h);
// → 10.0.0.3
// → 10.0.0.2
// → 10.0.0.1

// Convert to array — top to bottom order
string[] arr = hosts.ToArray();

// Clear
hosts.Clear();
```

---

### Pentest Context — Stack

```csharp
// Undo/redo command history
Stack<string> cmdHistory = new Stack<string>();

cmdHistory.Push("nmap -sV 10.0.0.1");
cmdHistory.Push("netcat 10.0.0.1 4444");
cmdHistory.Push("whoami");

Console.WriteLine("[*] Last command:");
Console.WriteLine(cmdHistory.Peek());          // → whoami

// Undo last command
string undone = cmdHistory.Pop();
Console.WriteLine($"[*] Undone: {undone}");   // → whoami

// Backtrack through visited hosts (DFS-style)
Stack<string> toVisit = new Stack<string>();
HashSet<string> seen  = new HashSet<string>();

toVisit.Push("10.0.0.1");

while (toVisit.Count > 0)
{
    string host = toVisit.Pop();

    if (!seen.Add(host)) continue;             // skip already visited
    Console.WriteLine($"[*] Visiting: {host}");

    // Discover neighbors and push
    foreach (string neighbor in GetNeighbors(host))
        if (!seen.Contains(neighbor))
            toVisit.Push(neighbor);
}

// Call stack simulation — track method depth
Stack<string> execStack = new Stack<string>();

execStack.Push("Main()");
execStack.Push("ScanNetwork()");
execStack.Push("IsPortOpen()");

Console.WriteLine($"[*] Current depth: {execStack.Count}");
Console.WriteLine($"[*] Executing: {execStack.Peek()}");
// → Executing: IsPortOpen()
```

---

---

## Queue\<T\> — FIFO (First In, First Out)

```
Enqueue → [ 1 | 2 | 3 ] → Dequeue

First item enqueued is first item dequeued.
Think: a queue of people — first in line is served first.
```

---

### Declaring & Initializing

```csharp
// Empty queue
Queue<string> targets  = new Queue<string>();
Queue<int>    jobQueue = new Queue<int>();

// With initial values — first item is at front
Queue<string> hosts = new Queue<string>(new[] { "10.0.0.1", "10.0.0.2", "10.0.0.3" });
// FRONT → 10.0.0.1 | 10.0.0.2 | 10.0.0.3 → BACK
```

---

### Enqueue() — Add to Back

```csharp
Queue<string> targets = new Queue<string>();

targets.Enqueue("10.0.0.1");
targets.Enqueue("10.0.0.2");
targets.Enqueue("10.0.0.3");

// Queue (front to back): 10.0.0.1 | 10.0.0.2 | 10.0.0.3
Console.WriteLine(targets.Count);   // → 3
```

---

### Dequeue() — Remove from Front

```csharp
Queue<string> targets = new Queue<string>();
targets.Enqueue("10.0.0.1");
targets.Enqueue("10.0.0.2");
targets.Enqueue("10.0.0.3");

string first = targets.Dequeue();
Console.WriteLine(first);           // → 10.0.0.1  (first enqueued)
Console.WriteLine(targets.Count);   // → 2

// Dequeue() throws if queue is empty
// ✅ Safe dequeue
if (targets.Count > 0)
    targets.Dequeue();
```

---

### Peek() — View Front Without Removing

```csharp
Queue<int> jobs = new Queue<int>();
jobs.Enqueue(1);
jobs.Enqueue(2);
jobs.Enqueue(3);

int next = jobs.Peek();
Console.WriteLine(next);            // → 1
Console.WriteLine(jobs.Count);      // → 3 (still 3)

// TryDequeue / TryPeek — safe, no exception
if (jobs.TryPeek(out int peeked))
    Console.WriteLine($"Next: {peeked}");

if (jobs.TryDequeue(out int job))
    Console.WriteLine($"Processing: {job}");
```

---

### Contains() & Iterating

```csharp
Queue<string> hosts = new Queue<string>();
hosts.Enqueue("10.0.0.1");
hosts.Enqueue("10.0.0.2");
hosts.Enqueue("10.0.0.3");

bool has = hosts.Contains("10.0.0.2");
Console.WriteLine(has);             // → True

// foreach — iterates front to back
foreach (string h in hosts)
    Console.WriteLine(h);
// → 10.0.0.1
// → 10.0.0.2
// → 10.0.0.3

// Convert to array
string[] arr = hosts.ToArray();

// Clear
hosts.Clear();
```

---

### Pentest Context — Queue

```csharp
// Target scan queue — process hosts in order
Queue<string> scanQueue = new Queue<string>();

string[] subnet = { "10.0.0.1", "10.0.0.2", "10.0.0.3", "10.0.0.4" };
foreach (string ip in subnet)
    scanQueue.Enqueue(ip);

Console.WriteLine($"[*] Queued {scanQueue.Count} targets\n");

while (scanQueue.Count > 0)
{
    string host = scanQueue.Dequeue();
    Console.WriteLine($"[*] Scanning: {host}  ({scanQueue.Count} remaining)");
    // scan...
}

// Job/task queue — rate-limited requests
Queue<string> requestQueue = new Queue<string>();

requestQueue.Enqueue("GET /admin");
requestQueue.Enqueue("GET /login");
requestQueue.Enqueue("POST /upload");

while (requestQueue.Count > 0)
{
    string req = requestQueue.Dequeue();
    Console.WriteLine($"[>] Sending: {req}");
    Thread.Sleep(500);              // rate limit
}

// BFS (Breadth-First Search) — network discovery
Queue<string> bfsQueue = new Queue<string>();
HashSet<string> visited = new HashSet<string>();

bfsQueue.Enqueue("10.0.0.1");      // start node

while (bfsQueue.Count > 0)
{
    string host = bfsQueue.Dequeue();
    if (!visited.Add(host)) continue;

    Console.WriteLine($"[*] Discovered: {host}");

    foreach (string neighbor in GetNeighbors(host))
        if (!visited.Contains(neighbor))
            bfsQueue.Enqueue(neighbor);
}
```

---

## Stack\<T\> vs Queue\<T\>

| Feature | `Stack<T>` | `Queue<T>` |
|---|---|---|
| Order | LIFO — Last In First Out | FIFO — First In First Out |
| Add | `Push()` | `Enqueue()` |
| Remove | `Pop()` (from top) | `Dequeue()` (from front) |
| Peek | `Peek()` — views top | `Peek()` — views front |
| Analogy | Stack of plates | Queue of people |
| Algorithm | DFS (Depth-First Search) | BFS (Breadth-First Search) |
| Use case | Undo/redo, backtracking | Job queues, ordered processing |

---

## Quick Reference

```csharp
// ── Stack<T> ────────────────────────────────────────
var stack = new Stack<int>();

stack.Push(1);                     // add to top
int top    = stack.Pop();          // remove from top (throws if empty)
int peeked = stack.Peek();         // view top (throws if empty)
stack.TryPop(out int v);           // safe pop
stack.TryPeek(out int p);          // safe peek
bool has   = stack.Contains(1);
int  count = stack.Count;
stack.Clear();

// ── Queue<T> ────────────────────────────────────────
var queue = new Queue<int>();

queue.Enqueue(1);                  // add to back
int front = queue.Dequeue();       // remove from front (throws if empty)
int next   = queue.Peek();         // view front (throws if empty)
queue.TryDequeue(out int dv);      // safe dequeue
queue.TryPeek(out int dp);         // safe peek
bool qhas  = queue.Contains(1);
int  qcount= queue.Count;
queue.Clear();
```
