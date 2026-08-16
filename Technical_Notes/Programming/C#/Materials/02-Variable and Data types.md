# Step 2 — Variables & Data Types

---

## Value Types
```csharp
int age = 25;                   // -2,147,483,648 → 2,147,483,647
long bigNum = 9999999999L;      // very large integers
short small = 32767;            // -32,768 → 32,767
byte b = 255;                   // 0 → 255 (unsigned)

float pi = 3.14f;               // 7 digit precision
double precise = 3.14159265;    // 15 digit precision
decimal money = 99.99m;         // financial (28 digit precision)

bool isActive = true;           // true / false
char letter = 'A';              // single character (single quote)
```

---

## Reference Types
```csharp
string name = "Anar";          // text (double quote)
string empty = null;            // can be null
string blank = string.Empty;    // same as ""

int[] numbers = { 1, 2, 3 };  // array (reference type)
object anything = 42;          // accepts any value
```

---

## Type Conversion
```csharp
// Implicit (automatic — no data loss)
int x = 100;
long y = x;
double d = x;

// Explicit cast (forced — data loss possible)
double pi = 3.99;
int rounded = (int)pi;          // → 3 (decimal part lost)

// String → Number
int parsed = int.Parse("42");
bool ok = int.TryParse("abc", out int result); // does not crash

// Number → String
string s = age.ToString();
string s2 = Convert.ToString(age);
```

---

## var — Type Inference
```csharp
var age = 25;           // compiler treats as int
var name = "Anar";      // compiler treats as string
var pi = 3.14;          // compiler treats as double
// type cannot change — just makes writing easier
```

---

## const & readonly
```csharp
const int MAX_PORT = 65535;      // compile-time constant
const string VERSION = "1.0.0";

// readonly — assigned once at runtime
readonly int startTime = Environment.TickCount;
```

---

## Nullable Types
```csharp
int? port = null;               // nullable int
bool? flag = null;              // nullable bool

// Null check
if (port == null) Console.WriteLine("Port not set");
int realPort = port ?? 80;      // if null, use 80
```

---

## String Features
```csharp
string ip = "192.168.1.1";

int len      = ip.Length;               // 11
bool has192  = ip.Contains("192");
string upper = ip.ToUpper();
string[] parts = ip.Split('.');         // ["192","168","1","1"]
string sub   = ip.Substring(0, 3);      // "192"

// Interpolation
string msg = $"[+] Target: {ip}, Port: {port}";

// Verbatim — backslash is treated as literal
string path = @"C:\Users\Anar\Desktop";
```

---

## Value vs Reference — The Difference
```csharp
// Value Type — copied
int a = 10;
int b = a;
b = 99;
Console.WriteLine(a); // → 10 (unchanged)

// Reference Type — points to the same location
int[] arr1 = { 1, 2, 3 };
int[] arr2 = arr1;
arr2[0] = 99;
Console.WriteLine(arr1[0]); // → 99 (changed!)
```

---

## Pentest Context Usage
```csharp
// Port scanner variables
string target    = "192.168.1.1";
int startPort    = 1;
int endPort      = 1024;
bool verbose     = false;
int timeout      = 500;

// Recon data
string hostname  = Environment.MachineName;
string username  = Environment.UserName;
bool isDomain    = hostname != Environment.UserDomainName;
long uptime      = Environment.TickCount64;

// Sandbox checks
bool lowUptime   = uptime < 3_600_000;  // less than 1 hour
bool lowCores    = Environment.ProcessorCount < 2;
bool isSandbox   = lowUptime || lowCores;
```

---
