# Step 6 — Conditional Statements (if / else / switch)

---

## Basic if / else if / else
```csharp
int port = 443;

if (port == 80)
    Console.WriteLine("HTTP");
else if (port == 443)
    Console.WriteLine("HTTPS");
else if (port == 22)
    Console.WriteLine("SSH");
else
    Console.WriteLine("Unknown port");

// Multiple conditions in one block
int statusCode = 403;

if (statusCode >= 200 && statusCode < 300)
    Console.WriteLine("Success");
else if (statusCode >= 300 && statusCode < 400)
    Console.WriteLine("Redirect");
else if (statusCode >= 400 && statusCode < 500)
    Console.WriteLine("Client Error");
else if (statusCode >= 500)
    Console.WriteLine("Server Error");
```

---

## Logical Operators in Conditions
```csharp
bool isAdmin   = true;
bool isLoggedIn = true;
int  failCount  = 3;

// AND — both must be true
if (isAdmin && isLoggedIn)
    Console.WriteLine("[+] Admin panel access granted");

// OR — at least one must be true
if (failCount > 5 || !isLoggedIn)
    Console.WriteLine("[!] Suspicious activity detected");

// NOT — invert the condition
if (!isLoggedIn)
    Console.WriteLine("[-] Access denied");

// Combined
if (isAdmin && isLoggedIn && failCount < 5)
    Console.WriteLine("[+] Proceed with elevated privileges");
```

---

## Nested if Statements
```csharp
string host = "192.168.1.1";
int    port = 22;
bool   auth = true;

if (host != null && host != "")
{
    if (port == 22)
    {
        if (auth)
            Console.WriteLine($"[+] SSH connected to {host}");
        else
            Console.WriteLine("[-] SSH auth failed");
    }
    else
    {
        Console.WriteLine($"[*] Port {port} is not SSH");
    }
}
else
{
    Console.WriteLine("[!] No host specified");
}
```

---

## switch Statement
```csharp
int port = 21;

switch (port)
{
    case 21:
        Console.WriteLine("FTP");
        break;
    case 22:
        Console.WriteLine("SSH");
        break;
    case 23:
        Console.WriteLine("Telnet");
        break;
    case 80:
        Console.WriteLine("HTTP");
        break;
    case 443:
        Console.WriteLine("HTTPS");
        break;
    case 3306:
        Console.WriteLine("MySQL");
        break;
    case 3389:
        Console.WriteLine("RDP");
        break;
    default:
        Console.WriteLine("Unknown service");
        break;
}
```

---

## switch — Multiple Cases per Block
```csharp
int port = 8443;

switch (port)
{
    case 80:
    case 8080:
    case 8000:
        Console.WriteLine("HTTP variant");
        break;

    case 443:
    case 8443:
    case 4443:
        Console.WriteLine("HTTPS variant");
        break;

    case 22:
    case 2222:
        Console.WriteLine("SSH variant");
        break;

    default:
        Console.WriteLine($"[?] Unknown port: {port}");
        break;
}
```

---

## switch Expression (C# 8+) — Cleaner Syntax
```csharp
int port = 443;

string service = port switch
{
    21         => "FTP",
    22         => "SSH",
    23         => "Telnet",
    25         => "SMTP",
    53         => "DNS",
    80         => "HTTP",
    443        => "HTTPS",
    3306       => "MySQL",
    3389       => "RDP",
    _          => "Unknown"      // default case
};

Console.WriteLine($"Port {port} → {service}");
// → "Port 443 → HTTPS"
```

---

## Pattern Matching with switch (C# 9+)
```csharp
object response = 200;

string result = response switch
{
    int code when code >= 200 && code < 300 => "Success",
    int code when code >= 400 && code < 500 => "Client Error",
    int code when code >= 500               => "Server Error",
    string msg                              => $"Message: {msg}",
    null                                    => "No response",
    _                                       => "Unexpected type"
};

Console.WriteLine(result);  // → "Success"
```

---

## is — Type & Pattern Check
```csharp
object payload = "admin:password123";

// Type check
if (payload is string s)
    Console.WriteLine($"String payload: {s.Length} chars");

// Null check
string input = null;
if (input is not null)
    Console.WriteLine("Has value");

// Condition with is
object value = 443;
if (value is int port && port < 1024)
    Console.WriteLine($"[*] Well-known port: {port}");
```

---

## Conditional with Enum
```csharp
enum ScanStatus { Open, Closed, Filtered, Unknown }

ScanStatus status = ScanStatus.Open;

if (status == ScanStatus.Open)
    Console.WriteLine("[+] Port is open — proceed");
else if (status == ScanStatus.Filtered)
    Console.WriteLine("[*] Port filtered — possible firewall");
else
    Console.WriteLine("[-] Port closed");

// Cleaner with switch
string msg = status switch
{
    ScanStatus.Open     => "[+] Open",
    ScanStatus.Closed   => "[-] Closed",
    ScanStatus.Filtered => "[*] Filtered",
    _                   => "[?] Unknown"
};
```

---

## Guard Clauses — Fail Fast Pattern
```csharp
// Bad — deeply nested
void Connect(string host, int port)
{
    if (host != null)
    {
        if (port > 0 && port < 65535)
        {
            if (!host.Contains(" "))
            {
                Console.WriteLine($"Connecting to {host}:{port}");
            }
        }
    }
}

// Good — guard clauses (early return)
void ConnectClean(string host, int port)
{
    if (string.IsNullOrWhiteSpace(host)) return;
    if (port <= 0 || port > 65535)       return;
    if (host.Contains(" "))              return;

    Console.WriteLine($"Connecting to {host}:{port}");
}
```

---

## Conditional Chaining with String Checks
```csharp
string banner = "Apache/2.4.49 (Unix)";

if (banner.Contains("Apache", StringComparison.OrdinalIgnoreCase))
{
    if (banner.Contains("2.4.49"))
        Console.WriteLine("[!] CVE-2021-41773 — Path Traversal possible!");
    else if (banner.Contains("2.4.50"))
        Console.WriteLine("[!] CVE-2021-42013 — Path Traversal variant!");
    else
        Console.WriteLine("[*] Apache detected — version check needed");
}
else if (banner.Contains("nginx", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("[*] Nginx detected");
}
else if (banner.Contains("IIS", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("[*] Microsoft IIS detected");
}
else
{
    Console.WriteLine("[?] Unknown server software");
}
```

---

## Ternary Inside Complex Logic
```csharp
int    port    = 443;
string host    = "10.0.0.1";
bool   isHttps = port == 443 || port == 8443;

string url     = $"{(isHttps ? "https" : "http")}://{host}:{port}";
// → "https://10.0.0.1:443"

string portLabel = port < 1024
    ? "well-known"
    : port < 49152
        ? "registered"
        : "dynamic/private";
// → "well-known"
```

---

## Null Checks — Defensive Conditions
```csharp
string input = null;

// Classic null check
if (input != null)
    Console.WriteLine(input.ToUpper());

// Null-conditional (safe access)
Console.WriteLine(input?.ToUpper());       // → nothing, no crash

// Null-coalescing (provide default)
string safe = input ?? "N/A";
Console.WriteLine(safe);                   // → "N/A"

// Null-coalescing assignment
input ??= "default_host";                  // assigns only if null
Console.WriteLine(input);                  // → "default_host"
```

---

## Pentest Context Usage
```csharp
// Vulnerability triage by status code
int code = 403;

if (code == 200)
    Console.WriteLine("[+] Endpoint accessible — check for data");
else if (code == 401)
    Console.WriteLine("[*] Auth required — attempt brute force");
else if (code == 403)
    Console.WriteLine("[*] Forbidden — check for bypass techniques");
else if (code == 500)
    Console.WriteLine("[!] Server error — possible injection point");
else if (code == 301 || code == 302)
    Console.WriteLine("[*] Redirect — follow and retest");

// OS detection from TTL
int ttl = 64;

string os = ttl switch
{
    <= 64  => "Linux / Unix",
    <= 128 => "Windows",
    <= 255 => "Cisco / Network Device",
    _      => "Unknown"
};
Console.WriteLine($"[*] Likely OS: {os}");

// Privilege check
string user = "NT AUTHORITY\\SYSTEM";

if (user.Contains("SYSTEM", StringComparison.OrdinalIgnoreCase) ||
    user.Contains("root",   StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("[!] SYSTEM/root level — full compromise");
}
else if (user.Contains("admin", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("[+] Admin rights — proceed to domain enum");
}
else
{
    Console.WriteLine("[-] Low priv — check for PrivEsc vectors");
}

// File sensitivity classifier
string filename = "db_backup_prod.sql";

string[] critical = { "password", "secret",  "token",  "private", "key"  };
string[] high     = { "backup",   "config",  "sql",    "env",     "conf" };
string[] medium   = { "log",      "temp",    "cache",  "bak"             };

bool isCritical = critical.Any(k => filename.Contains(k, StringComparison.OrdinalIgnoreCase));
bool isHigh     = high.Any(k =>     filename.Contains(k, StringComparison.OrdinalIgnoreCase));
bool isMedium   = medium.Any(k =>   filename.Contains(k, StringComparison.OrdinalIgnoreCase));

string severity = isCritical ? "🔴 CRITICAL"
                : isHigh     ? "🟠 HIGH"
                : isMedium   ? "🟡 MEDIUM"
                :               "⚪ LOW";

Console.WriteLine($"[{severity}] {filename}");
// → [🟠 HIGH] db_backup_prod.sql
```
