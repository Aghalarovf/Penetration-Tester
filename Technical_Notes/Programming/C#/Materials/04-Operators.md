# Step 4 — Operators

---

## Arithmetic Operators
```csharp
int a = 10, b = 3;

int sum   = a + b;    // → 13
int diff  = a - b;    // → 7
int prod  = a * b;    // → 30
int div   = a / b;    // → 3   (integer division, decimal lost)
int mod   = a % b;    // → 1   (remainder)

double exact = (double)a / b;  // → 3.333... (cast first)
```

### Increment & Decrement
```csharp
int x = 5;

x++;   // → 6  (post-increment: use then add)
x--;   // → 5  (post-decrement: use then subtract)
++x;   // → 6  (pre-increment: add then use)
--x;   // → 5  (pre-decrement: subtract then use)

// Difference matters in expressions
int a = 5;
int b = a++;   // b → 5, a → 6  (old value assigned)
int c = ++a;   // c → 7, a → 7  (new value assigned)
```

---

## Comparison Operators
```csharp
int port = 8080;

bool eq  = port == 8080;   // → true   (equal)
bool neq = port != 443;    // → true   (not equal)
bool gt  = port > 1024;    // → true   (greater than)
bool lt  = port < 65535;   // → true   (less than)
bool gte = port >= 8080;   // → true   (greater than or equal)
bool lte = port <= 9000;   // → true   (less than or equal)
```

---

## Logical Operators
```csharp
bool isAdmin   = true;
bool isElevated = false;

bool and = isAdmin && isElevated;   // → false (both must be true)
bool or  = isAdmin || isElevated;   // → true  (one is enough)
bool not = !isAdmin;                // → false (inverts)

// Short-circuit evaluation
// && stops at first false
// || stops at first true
bool result = IsAlive(host) && ScanPorts(host);  // ScanPorts skipped if dead
```

### Combined Logic
```csharp
int port    = 8080;
bool isOpen = true;
bool isHTTP = port == 80 || port == 8080;
bool valid  = isOpen && isHTTP && port < 65535;
```

---

## Assignment Operators
```csharp
int count = 0;

count += 5;    // count = count + 5  → 5
count -= 2;    // count = count - 2  → 3
count *= 4;    // count = count * 4  → 12
count /= 3;    // count = count / 3  → 4
count %= 3;    // count = count % 3  → 1
```

---

## Bitwise Operators
```csharp
int a = 0b_1010;   // 10
int b = 0b_1100;   // 12

int and  = a & b;   // → 0b_1000 = 8  (AND)
int or   = a | b;   // → 0b_1110 = 14 (OR)
int xor  = a ^ b;   // → 0b_0110 = 6  (XOR)
int not  = ~a;      // → bitwise NOT
int left = a << 1;  // → 20 (shift left = multiply by 2)
int right= a >> 1;  // → 5  (shift right = divide by 2)
```

### XOR — Pentest Usage
```csharp
// XOR encryption (symmetric — same key to encrypt & decrypt)
byte[] payload = { 0x90, 0x90, 0xC3 };
byte   key     = 0xAA;

// Encrypt
byte[] encrypted = payload.Select(b => (byte)(b ^ key)).ToArray();

// Decrypt (same operation)
byte[] decrypted = encrypted.Select(b => (byte)(b ^ key)).ToArray();
```

---

## Operator Precedence
```csharp
// High → Low priority
// 1. ()  parentheses
// 2. ++, --
// 3. *, /, %
// 4. +, -
// 5. >, <, >=, <=
// 6. ==, !=
// 7. &&
// 8. ||
// 9. =, +=, -=

int result = 2 + 3 * 4;        // → 14  (not 20)
int safe   = (2 + 3) * 4;      // → 20
bool check = 5 > 3 && 2 < 4;   // → true
```

---

## Pentest Context Usage
```csharp
// Port range validation
int port = 8080;
bool validPort = port >= 1 && port <= 65535;

// Timeout calculation with jitter
int sleepMs  = 30000;
int jitter   = 20;
int variance = sleepMs * jitter / 100;  // 6000
// random sleep between 24000 and 36000

// Sandbox detection scoring
int score = 0;
if (Environment.ProcessorCount < 2)    score += 1;
if (Environment.TickCount64 < 3600000) score += 1;
if (Environment.UserName == "sandbox") score += 1;
bool isSandbox = score >= 2;

// Byte XOR for payload obfuscation
byte b         = 0x90;
byte key       = 0xAA;
byte encoded   = (byte)(b ^ key);   // → 0x3A
byte decoded   = (byte)(encoded ^ key); // → 0x90 (original)

// Hex byte check
int value      = 255;
string hex     = value.ToString("X2");  // → "FF"
bool isNOP     = (value & 0xFF) == 0x90;
```

---


// Clean command output
string rawOutput = "  NT AUTHORITY\\SYSTEM\r\n";
string clean     = rawOutput.Trim().Replace("\r\n", "").Replace("\\", "/");
// → "NT AUTHORITY/SYSTEM"
```
