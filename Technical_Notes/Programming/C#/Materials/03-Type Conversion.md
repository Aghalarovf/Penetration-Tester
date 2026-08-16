# Step 3 — Type Conversion

---

## Implicit Conversion (Automatic)
No data loss — compiler allows it automatically.
```csharp
byte  b   = 100;
short s   = b;       // byte → short   ✓
int   i   = s;       // short → int    ✓
long  l   = i;       // int → long     ✓
float f   = l;       // long → float   ✓
double d  = f;       // float → double ✓
```

### Implicit Conversion Chain
```
byte → short → int → long → float → double
                          ↘
                        decimal
```

---

## Explicit Conversion (Cast)
Data loss possible — you must force it manually.
```csharp
double pi      = 3.99;
int rounded    = (int)pi;        // → 3  (decimal part lost)

long bigNum    = 9999999999L;
int truncated  = (int)bigNum;    // → overflow, wrong value!

double d       = 1.7;
float f        = (float)d;       // → slight precision loss
```

### When to Use Cast
```csharp
// Safe — you know the value fits
double score = 95.7;
int display  = (int)score;       // → 95, acceptable

// Dangerous — value may overflow
long fileSize = 999999999999L;
int bad       = (int)fileSize;   // → garbage value, avoid this
```

---

## Convert Class
Handles null safely. Throws on invalid input.
```csharp
Convert.ToInt32("42");           // → 42
Convert.ToInt32(3.99);           // → 4  (rounds, not truncates!)
Convert.ToInt32(true);           // → 1
Convert.ToInt32(false);          // → 0
Convert.ToInt32(null);           // → 0  (no exception)

Convert.ToDouble("3.14");        // → 3.14
Convert.ToString(255);           // → "255"
Convert.ToBoolean(1);            // → true
Convert.ToBoolean(0);            // → false
Convert.ToChar(65);              // → 'A'
```

### Convert vs Cast — Key Difference
```csharp
double pi = 3.99;

int a = (int)pi;              // → 3  (truncates)
int b = Convert.ToInt32(pi);  // → 4  (rounds)
```

---

## int.Parse()
Converts string to int. Crashes if input is invalid.
```csharp
int port    = int.Parse("8080");      // → 8080
int age     = int.Parse("25");        // → 25
int broken  = int.Parse("abc");       // → FormatException!
int empty   = int.Parse("");          // → FormatException!
int nullVal = int.Parse(null);        // → ArgumentNullException!
```

### Parse Variants
```csharp
double.Parse("3.14");
float.Parse("3.14");
long.Parse("9999999999");
bool.Parse("true");           // → true
bool.Parse("True");           // → true (case-insensitive)
bool.Parse("yes");            // → FormatException!
```

---

## int.TryParse()
Safe version — never crashes. Returns bool.
```csharp
bool ok1 = int.TryParse("8080", out int port);
// ok1 → true, port → 8080

bool ok2 = int.TryParse("abc", out int bad);
// ok2 → false, bad → 0 (default)

bool ok3 = int.TryParse(null, out int n);
// ok3 → false, n → 0
```

### TryParse Pattern (Recommended)
```csharp
string input = Console.ReadLine();

if (int.TryParse(input, out int port))
{
    Console.WriteLine($"[+] Valid port: {port}");
}
else
{
    Console.WriteLine("[-] Invalid port number");
}
```

---

## Parse vs TryParse vs Convert

| Method | Null Input | Invalid Input | Rounds? | Use When |
|---|---|---|---|---|
| `(int)` cast | crash | N/A | no (truncates) | you trust the type |
| `int.Parse()` | crash | crash | no | input is guaranteed valid |
| `int.TryParse()` | false | false | no | input may be invalid |
| `Convert.ToInt32()` | → 0 | crash | yes | null is possible |

---

## ToString() — Any Type to String
```csharp
int port       = 8080;
double score   = 3.14;
bool flag      = true;

string s1 = port.ToString();          // → "8080"
string s2 = score.ToString();         // → "3.14"
string s3 = flag.ToString();          // → "True"
string s4 = port.ToString("X");       // → "1F90" (hex)
string s5 = score.ToString("F2");     // → "3.14" (2 decimal places)
string s6 = 255.ToString("X2");       // → "FF"
```

---

## Number Base Conversions
```csharp
// Decimal → Hex / Binary / Octal
int value = 255;
string hex    = Convert.ToString(value, 16);  // → "ff"
string binary = Convert.ToString(value, 2);   // → "11111111"
string octal  = Convert.ToString(value, 8);   // → "377"

// Hex string → int
int fromHex = Convert.ToInt32("ff", 16);      // → 255
int fromBin = Convert.ToInt32("11111111", 2); // → 255

// int → hex string (uppercase)
string upper = value.ToString("X2");           // → "FF"
string lower = value.ToString("x2");           // → "ff"
```

---

## Boxing & Unboxing
```csharp
// Boxing — value type stored as object (reference type)
int num    = 42;
object box = num;         // boxed

// Unboxing — object back to value type
int unbox  = (int)box;    // unboxed

// Wrong unbox — crashes
object o   = 42;
double d   = (double)o;   // → InvalidCastException!
double safe = (double)(int)o;  // correct: unbox first, then cast
```

---

## Pentest Context Usage
```csharp
// CLI argument → int (safe)
string portArg = args[0];
if (!int.TryParse(portArg, out int port) || port < 1 || port > 65535)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("[-] Invalid port. Use 1–65535.");
    Console.ResetColor();
    return;
}

// Uptime → hours/minutes
long ms      = Environment.TickCount64;
long seconds = ms / 1000;
long minutes = seconds / 60;
long hours   = minutes / 60;
Console.WriteLine($"[*] Uptime: {hours}h {minutes % 60}m");

// Sandbox check threshold
bool sandbox = hours < 1;

// Hex shellcode bytes
string[] hexBytes = { "90", "90", "90", "C3" };
byte[] shellcode  = hexBytes.Select(h => Convert.ToByte(h, 16)).ToArray();
Console.WriteLine($"[+] Shellcode: {shellcode.Length} bytes");

// PID from string
string pidInput = "1234";
int pid = int.TryParse(pidInput, out int p) ? p : -1;
```
