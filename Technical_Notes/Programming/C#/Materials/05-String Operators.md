# Step 5 — String Operations

---

## Basic Properties
```csharp
string ip = "  192.168.1.1  ";

int  len     = ip.Length;          // → 16 (includes spaces)
bool isEmpty = ip == "";
bool isNull  = ip == null;
bool blank   = string.IsNullOrWhiteSpace(ip);  // → false (has content)
```

---

## Case Operations
```csharp
string cmd = "WhoAmI";

string upper = cmd.ToUpper();    // → "WHOAMI"
string lower = cmd.ToLower();    // → "whoami"

// Case-insensitive comparison
bool match = cmd.Equals("whoami", StringComparison.OrdinalIgnoreCase); // → true
```

---

## Trim — Remove Whitespace
```csharp
string raw = "  192.168.1.1  ";

string trimmed      = raw.Trim();        // → "192.168.1.1"
string trimStart    = raw.TrimStart();   // → "192.168.1.1  "
string trimEnd      = raw.TrimEnd();     // → "  192.168.1.1"

// Custom character trim
string dirty = "###target###";
string clean = dirty.Trim('#');          // → "target"
```

---

## Contains, StartsWith, EndsWith
```csharp
string path = @"C:\Users\Anar\password.txt";

bool hasPass  = path.Contains("password");      // → true
bool isC      = path.StartsWith(@"C:\");        // → true
bool isTxt    = path.EndsWith(".txt");          // → true

// Case-insensitive check
bool hasPwd   = path.Contains("PASSWORD",
                StringComparison.OrdinalIgnoreCase); // → true
```

---

## IndexOf & Substring
```csharp
string banner = "OpenSSH_8.9p1 Ubuntu";

int idx    = banner.IndexOf("_");         // → 7
int last   = banner.LastIndexOf(" ");     // → 13

// Extract from position
string ver = banner.Substring(7, 6);     // → "_8.9p1"
string os  = banner.Substring(14);       // → "Ubuntu" (to end)

// Safe extraction
if (banner.Contains("OpenSSH"))
{
    int start = banner.IndexOf("_") + 1;
    string version = banner.Substring(start, 5); // → "8.9p1"
}
```

---

## Replace
```csharp
string cmd = "cmd.exe /c whoami";

string replaced  = cmd.Replace("whoami", "ipconfig");
// → "cmd.exe /c ipconfig"

string cleaned   = cmd.Replace(".exe", "");
// → "cmd /c whoami"

// Chain replacements
string sanitized = cmd
    .Replace("/c", "")
    .Replace("  ", " ")
    .Trim();
```

---

## Split
```csharp
// IP splitting
string ip      = "192.168.1.1";
string[] parts = ip.Split('.');
// → ["192", "168", "1", "1"]

int octet = int.Parse(parts[3]);  // → 1

// Multi-char delimiter
string csv  = "target,8080,open,http";
string[] fields = csv.Split(',');
// → ["target", "8080", "open", "http"]

// Split with limit
string line     = "admin:password:1000:1000";
string[] cols   = line.Split(':', 2);
// → ["admin", "password:1000:1000"]

// Split lines
string output   = "line1\nline2\nline3";
string[] lines  = output.Split('\n');
```

---

## Join
```csharp
string[] parts = { "192", "168", "1", "1" };
string ip      = string.Join(".", parts);   // → "192.168.1.1"

string[] cmds  = { "whoami", "hostname", "ipconfig" };
string chain   = string.Join(" && ", cmds);
// → "whoami && hostname && ipconfig"
```

---

## String Interpolation & Format
```csharp
string host = "192.168.1.1";
int    port = 8080;
bool   open = true;

// Interpolation (recommended)
string msg1 = $"[+] {host}:{port} is {(open ? "OPEN" : "CLOSED")}";

// Format
string msg2 = string.Format("[+] {0}:{1}", host, port);

// Padding — table alignment
Console.WriteLine($"{"PORT",-10}{"STATUS",-10}{"SERVICE",-15}");
Console.WriteLine($"{8080,-10}{"OPEN",-10}{"HTTP",-15}");
Console.WriteLine($"{443,-10}{"OPEN",-10}{"HTTPS",-15}");
```

---

## String Builder — Large Strings
```csharp
// Bad — creates new string each iteration
string result = "";
for (int i = 0; i < 1000; i++)
    result += i + ",";         // slow — 1000 allocations

// Good — single buffer
var sb = new System.Text.StringBuilder();
for (int i = 0; i < 1000; i++)
    sb.Append(i).Append(",");  // fast

string final = sb.ToString();
```

---

## Useful Conversions
```csharp
// String → byte array
byte[] bytes  = System.Text.Encoding.UTF8.GetBytes("whoami");

// Byte array → string
string text   = System.Text.Encoding.UTF8.GetString(bytes);

// String → Base64
string b64    = Convert.ToBase64String(bytes);      // → "d2hvYW1p"

// Base64 → String
byte[] decoded = Convert.FromBase64String(b64);
string plain   = System.Text.Encoding.UTF8.GetString(decoded); // → "whoami"

// Char → ASCII value
int ascii     = (int)'A';          // → 65
char letter   = Convert.ToChar(65); // → 'A'
```

---

## Pentest Context Usage
```csharp
// Parse banner for service detection
string banner  = "OpenSSH_8.9p1 Ubuntu-3ubuntu0.6";
bool isSSH     = banner.Contains("SSH", StringComparison.OrdinalIgnoreCase);
string version = banner.Split('_')[1].Split(' ')[0];  // → "8.9p1"

// Build DNS covert channel payload
string cmd     = "whoami";
string encoded = string.Join("-",
    cmd.Select(c => ((int)c).ToString()));
// → "119-104-111-97-109-105"
string subdomain = encoded + ".evil.com";

// Decode on agent side
string received  = "119-104-111-97-109-105";
string decoded   = string.Join("",
    received.Split('-')
            .Select(n => Convert.ToChar(int.Parse(n))));
// → "whoami"

// Credential file detection
string filename = "backup_password_2024.txt";
string[] keywords = { "password", "creds", "secret", "token", "key" };
bool isSensitive = keywords.Any(k =>
    filename.Contains(k, StringComparison.OrdinalIgnoreCase));
