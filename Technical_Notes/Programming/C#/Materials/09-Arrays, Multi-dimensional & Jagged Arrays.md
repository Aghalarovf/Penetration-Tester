# Step 11 & 12 — Arrays, Multi-dimensional & Jagged Arrays

---

## Step 11 — Single-Dimensional Arrays

### Declaring & Initializing

```csharp
// Method 1 — inline initializer
int[] ports = { 22, 80, 443, 3389, 8080 };

// Method 2 — specify size, fill later
string[] hosts = new string[3];
hosts[0] = "10.0.0.1";
hosts[1] = "10.0.0.2";
hosts[2] = "10.0.0.3";

// Method 3 — new keyword with initializer
byte[] shellcode = new byte[] { 0x90, 0x90, 0xCC, 0xC3 };

// Method 4 — type inferred
var targets = new[] { "192.168.1.1", "192.168.1.2" };
```

---

### Accessing Elements by Index

```csharp
int[] ports = { 22, 80, 443, 3389 };

// Index starts at 0
Console.WriteLine(ports[0]);      // → 22
Console.WriteLine(ports[2]);      // → 443

// Last element
Console.WriteLine(ports[ports.Length - 1]);  // → 3389

// Index from end (^ operator)
Console.WriteLine(ports[^1]);     // → 3389  (last)
Console.WriteLine(ports[^2]);     // → 443   (second to last)

// Modify an element
ports[1] = 8080;
Console.WriteLine(ports[1]);      // → 8080
```

---

### array.Length

```csharp
string[] hosts = { "10.0.0.1", "10.0.0.2", "10.0.0.3" };

Console.WriteLine(hosts.Length);  // → 3

// Loop using Length
for (int i = 0; i < hosts.Length; i++)
{
    Console.WriteLine($"[{i}] {hosts[i]}");
}
// → [0] 10.0.0.1
// → [1] 10.0.0.2
// → [2] 10.0.0.3

// foreach — simpler when index not needed
foreach (string host in hosts)
{
    Console.WriteLine($"[*] Scanning {host}");
}
```

---

### Useful Array Methods

```csharp
int[] ports = { 443, 22, 3389, 80, 8080 };

// Sort
Array.Sort(ports);
// → { 22, 80, 443, 3389, 8080 }

// Reverse
Array.Reverse(ports);
// → { 8080, 3389, 443, 80, 22 }

// Search (array must be sorted first)
int[] sorted = { 22, 80, 443, 3389 };
int idx = Array.BinarySearch(sorted, 443);
Console.WriteLine(idx);           // → 2

// Check if value exists
bool found = Array.Exists(ports, p => p == 80);
Console.WriteLine(found);         // → true

// Copy
int[] copy = new int[ports.Length];
Array.Copy(ports, copy, ports.Length);

// Fill
int[] mask = new int[5];
Array.Fill(mask, 0xFF);
// → { 255, 255, 255, 255, 255 }

// Slice with Range
int[] slice = ports[1..3];        // index 1 and 2 only
```

---

### Pentest Context — Single Array

```csharp
// Port scanner over array
int[] targets = { 21, 22, 23, 80, 443, 3389, 8080 };
string host   = "10.0.0.1";

foreach (int port in targets)
{
    bool open = IsPortOpen(host, port);
    Console.WriteLine($"{host}:{port,-6} {(open ? "[+] OPEN" : "[-] CLOSED")}");
}

// XOR shellcode array
byte[] payload = { 0x90, 0x90, 0xCC, 0xC3 };
byte   key     = 0x41;

for (int i = 0; i < payload.Length; i++)
    payload[i] ^= key;

Console.WriteLine(BitConverter.ToString(payload));
// → D1-D1-8D-82
```

---

## Step 12 — Multi-dimensional & Jagged Arrays

### 2D Array — `int[,]`

```csharp
// Declaration — [rows, columns]
int[,] matrix = new int[3, 4];    // 3 rows, 4 columns

// Inline initializer
int[,] grid = {
    { 1, 2, 3 },
    { 4, 5, 6 },
    { 7, 8, 9 }
};

// Access — [row, column]
Console.WriteLine(grid[0, 0]);    // → 1  (top-left)
Console.WriteLine(grid[1, 2]);    // → 6
Console.WriteLine(grid[2, 2]);    // → 9  (bottom-right)

// Dimensions
Console.WriteLine(grid.GetLength(0));  // → 3  (rows)
Console.WriteLine(grid.GetLength(1));  // → 3  (columns)

// Loop through 2D array
for (int row = 0; row < grid.GetLength(0); row++)
{
    for (int col = 0; col < grid.GetLength(1); col++)
    {
        Console.Write($"{grid[row, col]} ");
    }
    Console.WriteLine();
}
// → 1 2 3
// → 4 5 6
// → 7 8 9
```

---

### Jagged Array — `int[][]`

```csharp
// Array of arrays — rows can have different lengths
int[][] jagged = new int[3][];

jagged[0] = new int[] { 1, 2 };
jagged[1] = new int[] { 3, 4, 5, 6 };
jagged[2] = new int[] { 7 };

// Inline initializer
string[][] hostPorts = {
    new[] { "10.0.0.1", "22", "80" },
    new[] { "10.0.0.2", "443" },
    new[] { "10.0.0.3", "8080", "3389", "22" }
};

// Access — [row][column]
Console.WriteLine(jagged[1][2]);      // → 5
Console.WriteLine(hostPorts[0][1]);   // → "22"

// Each row has its own Length
Console.WriteLine(jagged[0].Length);  // → 2
Console.WriteLine(jagged[1].Length);  // → 4

// Loop through jagged array
for (int i = 0; i < jagged.Length; i++)
{
    for (int j = 0; j < jagged[i].Length; j++)
    {
        Console.Write($"{jagged[i][j]} ");
    }
    Console.WriteLine();
}
// → 1 2
// → 3 4 5 6
// → 7
```

---

### 2D vs Jagged — When to Use Each

| Feature | 2D `int[,]` | Jagged `int[][]` |
|---|---|---|
| Shape | Fixed rectangular grid | Rows can differ in length |
| Memory | Contiguous block | Each row separate allocation |
| Performance | Slightly faster iteration | Slightly slower |
| Use case | Matrices, scan grids | Variable-length rows |
| Access syntax | `arr[row, col]` | `arr[row][col]` |
| Row length | `arr.GetLength(1)` | `arr[row].Length` |

```csharp
// Use 2D when all rows are the same length
// e.g. a 4x4 network map
string[,] networkMap = new string[4, 4];

// Use jagged when rows vary
// e.g. each host has different open ports
string[][] openPorts = new string[hostCount][];
openPorts[0] = new[] { "22", "80" };
openPorts[1] = new[] { "443", "8080", "3389" };
```

---

### 3D Array

```csharp
// Three dimensions — [x, y, z]
int[,,] cube = new int[2, 3, 4];

cube[0, 1, 2] = 99;
Console.WriteLine(cube[0, 1, 2]);  // → 99

Console.WriteLine(cube.GetLength(0));  // → 2
Console.WriteLine(cube.GetLength(1));  // → 3
Console.WriteLine(cube.GetLength(2));  // → 4
```

---

### Pentest Context — Multi-dim & Jagged

```csharp
// 2D scan results grid — [hostIndex, portIndex]
string[,] scanGrid = {
    { "10.0.0.1", "22:OPEN",  "80:OPEN",   "443:CLOSED" },
    { "10.0.0.2", "22:CLOSED","80:CLOSED", "443:OPEN"   },
    { "10.0.0.3", "22:OPEN",  "80:OPEN",   "443:OPEN"   }
};

// Print results for each host
for (int h = 0; h < scanGrid.GetLength(0); h++)
{
    Console.WriteLine($"\n[*] Host: {scanGrid[h, 0]}");
    for (int p = 1; p < scanGrid.GetLength(1); p++)
        Console.WriteLine($"    {scanGrid[h, p]}");
}

// Jagged — each host has variable open ports
string[][]  results = {
    new[] { "ssh", "http" },
    new[] { "https", "rdp", "smb" },
    new[] { "ftp" }
};

for (int i = 0; i < results.Length; i++)
{
    Console.Write($"Host {i + 1}: ");
    Console.WriteLine(string.Join(", ", results[i]));
}
// → Host 1: ssh, http
// → Host 2: https, rdp, smb
// → Host 3: ftp

// XOR encrypt a 2D payload matrix
byte[,] payloads = {
    { 0x90, 0x90, 0xCC },
    { 0xEB, 0xFE, 0x00 }
};
byte key = 0x41;

for (int r = 0; r < payloads.GetLength(0); r++)
    for (int c = 0; c < payloads.GetLength(1); c++)
        payloads[r, c] ^= key;
```

---

## Quick Reference

```csharp
// ── Single-dimensional ──────────────────────────────
int[]    arr    = { 1, 2, 3 };
int      len    = arr.Length;         // element count
int      first  = arr[0];
int      last   = arr[^1];
int[]    slice  = arr[0..2];          // { 1, 2 }

Array.Sort(arr);
Array.Reverse(arr);
Array.Fill(arr, 0);
bool exists = Array.Exists(arr, x => x == 2);

// ── 2D (rectangular) ────────────────────────────────
int[,]   grid   = new int[3, 4];
int      rows   = grid.GetLength(0);  // 3
int      cols   = grid.GetLength(1);  // 4
int      val    = grid[1, 2];
grid[0, 0]      = 99;

// ── Jagged ──────────────────────────────────────────
int[][]  jag    = new int[3][];
jag[0]          = new int[] { 1, 2 };
int      rowLen = jag[0].Length;      // 2
int      elem   = jag[0][1];          // 2
```
