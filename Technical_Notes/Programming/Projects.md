# Pentest Tool Projects
### 5 C# Tools · 5 Python Tools
#### Real-world assignments — build tools you'll actually use

---

# 🔵 C# TOOLS

---

## C# Tool 1 — PortSweep
### *TCP Port Scanner with Banner Grabbing*

**Purpose:** Scan a target host or subnet for open TCP ports, identify running services, and grab banners to detect software versions.

**Pentest use case:** Initial reconnaissance — find attack surface before exploitation.

---

**Modules / Classes:**

```
PortSweep/
├── Program.cs
├── Scanner/
│   ├── TcpScanner.cs       — TCP connect scan logic
│   ├── BannerGrabber.cs    — Read first response bytes from open port
│   └── SubnetParser.cs     — Parse "192.168.1.0/24" into IP list
├── Models/
│   ├── ScanTarget.cs       — host, port range, timeout settings
│   └── ScanResult.cs       — host, port, status, banner, service name
├── Output/
│   ├── ConsoleReporter.cs  — colored table output
│   └── CsvExporter.cs      — save results to .csv
└── Utils/
    ├── ServiceMapper.cs     — port → service name dictionary
    └── IpHelper.cs          — IP validation, range expansion
```

**Required Methods:**

```csharp
// TcpScanner.cs
bool IsPortOpen(string host, int port, int timeoutMs)
List<ScanResult> ScanHost(string host, int[] ports)
List<ScanResult> ScanSubnet(string cidr, int[] ports)

// BannerGrabber.cs
string GrabBanner(string host, int port, int timeoutMs = 2000)

// SubnetParser.cs
List<string> ExpandCIDR(string cidr)          // "192.168.1.0/24" → 254 IPs
bool ValidateIP(string ip)

// ServiceMapper.cs
string GetServiceName(int port)               // 22 → "SSH", 80 → "HTTP"
string GuessServiceFromBanner(string banner)  // "OpenSSH" → "SSH"

// ConsoleReporter.cs
void PrintTable(List<ScanResult> results)
void PrintSummary(List<ScanResult> results)

// CsvExporter.cs
void Export(List<ScanResult> results, string filePath)
```

**C# Concepts Practiced:**
- `TcpClient` with `ConnectAsync().Wait(timeout)`
- `async/await` (optional for parallel scanning)
- `Dictionary<int, string>` for service map
- `StringBuilder` for report generation
- Tuples, List, foreach, try/catch

**Expected Output:**
```
[*] Scanning 192.168.1.1 — ports 1-1024
PORT     STATE    SERVICE    BANNER
22       OPEN     SSH        OpenSSH_8.9p1 Ubuntu
80       OPEN     HTTP       Apache/2.4.49
443      OPEN     HTTPS      -
3306     CLOSED   MySQL      -

[*] Scan complete — 3 open / 1021 closed
[*] Results saved → scan_192.168.1.1.csv
```

---

## C# Tool 2 — CreepCrawl
### *HTTP Directory & File Bruteforcer*

**Purpose:** Brute-force hidden directories and files on a web server using a wordlist. Detect sensitive endpoints, admin panels, backup files, and config leaks.

**Pentest use case:** Web application enumeration — find what the server is hiding.

---

**Modules / Classes:**

```
CreepCrawl/
├── Program.cs
├── Core/
│   ├── Crawler.cs           — main brute-force engine
│   ├── RequestSender.cs     — HTTP GET with configurable headers/timeout
│   └── WordlistLoader.cs    — read wordlist file, filter blanks/comments
├── Models/
│   ├── CrawlConfig.cs       — target URL, wordlist path, threads, extensions
│   └── CrawlResult.cs       — url, status code, content length, redirect target
├── Detection/
│   ├── StatusFilter.cs      — filter by status codes (e.g. show only 200, 301, 403)
│   └── SensitivityChecker.cs — flag paths like /admin, /.git, /backup, /config
├── Output/
│   ├── LivePrinter.cs       — real-time console output with color
│   └── ReportWriter.cs      — write findings to .txt file
└── Utils/
    └── UrlBuilder.cs         — combine base URL + path + extensions
```

**Required Methods:**

```csharp
// Crawler.cs
void Start(CrawlConfig config)
List<CrawlResult> RunWordlist(string baseUrl, List<string> words)

// RequestSender.cs
(int statusCode, long contentLength, string redirectUrl) SendGet(string url)

// WordlistLoader.cs
List<string> Load(string filePath)
List<string> AppendExtensions(List<string> words, string[] exts)
// e.g. "admin" → ["admin", "admin.php", "admin.bak", "admin.txt"]

// StatusFilter.cs
bool ShouldReport(int statusCode, int[] allowedCodes)

// SensitivityChecker.cs
string GetSensitivityLevel(string path)   // "CRITICAL" / "HIGH" / "MEDIUM" / "LOW"

// UrlBuilder.cs
string Build(string baseUrl, string path)
```

**C# Concepts Practiced:**
- `HttpClient` with custom headers and timeout
- `string.Join`, `Path.Combine`, URL building
- File I/O: `File.ReadAllLines()`
- `List<T>`, `Dictionary`, `foreach`
- Enum for sensitivity levels
- `Console.ForegroundColor` for colored output

**Expected Output:**
```
[*] Target  : http://10.0.0.1
[*] Wordlist: /usr/share/wordlists/dirb/common.txt (4614 words)
[*] Exts    : .php .txt .bak

[200] /index.php              (1842 bytes)
[301] /admin          →       /admin/
[403] /admin/config.php       (CRITICAL)
[200] /backup.zip             (HIGH)
[200] /.git/HEAD              (CRITICAL)

[*] Done — 5 findings in 4614 requests
```

---

## C# Tool 3 — LogHound
### *Log File Analyzer & Threat Hunter*

**Purpose:** Parse large log files (auth.log, access.log, Windows event logs in text form) and identify brute force attempts, privilege escalation, suspicious IPs, and anomalous patterns.

**Pentest use case:** Post-exploitation / blue team simulation — understand what you left behind or what to look for during an engagement.

---

**Modules / Classes:**

```
LogHound/
├── Program.cs
├── Parsers/
│   ├── AuthLogParser.cs     — parse Linux /var/log/auth.log
│   ├── ApacheLogParser.cs   — parse Apache/Nginx access.log (Combined Log Format)
│   └── GenericLogParser.cs  — keyword search on any plain text log
├── Detectors/
│   ├── BruteForceDetector.cs  — N failed logins from same IP within T seconds
│   ├── PrivEscDetector.cs     — detect "sudo", "su root", "NOPASSWD" patterns
│   └── AnomalyDetector.cs    — rare user agents, unusual hours, large responses
├── Models/
│   ├── LogEntry.cs          — timestamp, ip, user, action, raw line
│   └── ThreatEvent.cs       — severity, type, description, source IP, timestamp
├── Output/
│   ├── ThreatPrinter.cs     — print threats grouped by severity
│   └── HtmlReporter.cs      — generate simple HTML threat report
└── Utils/
    ├── IpGrouper.cs          — group events by source IP
    └── TimeParser.cs         — parse various timestamp formats
```

**Required Methods:**

```csharp
// AuthLogParser.cs
List<LogEntry> Parse(string filePath)
List<LogEntry> FilterByKeyword(List<LogEntry> entries, string keyword)

// BruteForceDetector.cs
List<ThreatEvent> Detect(List<LogEntry> entries, int threshold = 5)
Dictionary<string, int> CountFailedByIP(List<LogEntry> entries)

// PrivEscDetector.cs
List<ThreatEvent> Detect(List<LogEntry> entries)

// IpGrouper.cs
Dictionary<string, List<LogEntry>> GroupByIP(List<LogEntry> entries)
List<string> GetTopIPs(Dictionary<string, List<LogEntry>> grouped, int top = 10)

// ThreatPrinter.cs
void Print(List<ThreatEvent> threats)
void PrintSummary(List<ThreatEvent> threats)

// HtmlReporter.cs
void Generate(List<ThreatEvent> threats, string outputPath)
```

**C# Concepts Practiced:**
- `File.ReadAllLines()`, `StreamReader`
- `string.Split()`, `Contains()`, `StartsWith()`
- `Dictionary<string, List<T>>` grouping
- `DateTime.Parse()` for timestamps
- `StringBuilder` for HTML generation
- LINQ: `OrderByDescending`, `GroupBy`, `Where`, `Count`

**Expected Output:**
```
====== LOGHOUND THREAT REPORT ======

[CRITICAL] Brute Force — 10.0.0.99
  → 47 failed SSH attempts in 3 minutes
  → Targeted users: root, admin, ubuntu

[HIGH] Privilege Escalation — 192.168.1.5
  → sudo su executed at 03:14 AM
  → User: www-data → root

[MEDIUM] Suspicious Access — 10.0.0.50
  → 3 requests to /etc/passwd via LFI
  → Response size: 2847 bytes

TOP ATTACKER IPs:
  10.0.0.99    →  47 events
  192.168.1.5  →  12 events
```

---

## C# Tool 4 — HashCrackr
### *Hash Identifier & Dictionary Attack Tool*

**Purpose:** Identify hash type from format/length, then attempt to crack it using a wordlist with optional rules (append numbers, toggle case, add common suffixes).

**Pentest use case:** Post-exploitation password cracking after dumping `/etc/shadow`, NTLM hashes, or database password columns.

---

**Modules / Classes:**

```
HashCrackr/
├── Program.cs
├── Core/
│   ├── HashIdentifier.cs    — detect MD5, SHA1, SHA256, NTLM, bcrypt by length/format
│   ├── HashComputer.cs      — compute MD5, SHA1, SHA256, SHA512 from plaintext
│   └── CrackEngine.cs       — dictionary attack loop with rule mutations
├── Rules/
│   ├── MutationEngine.cs    — apply rules: append 123, leetspeak, toggle case
│   └── RuleSet.cs           — define and load mutation rule configs
├── Models/
│   ├── HashTarget.cs        — raw hash string, detected type, status
│   └── CrackResult.cs       — hash, plaintext, attempts, time taken
├── Output/
│   ├── ProgressPrinter.cs   — show attempts/sec, ETA, current candidate
│   └── ResultExporter.cs    — save cracked hashes to file
└── Utils/
    └── WordlistReader.cs     — buffered reading for large wordlists
```

**Required Methods:**

```csharp
// HashIdentifier.cs
string Identify(string hash)         // → "MD5" / "SHA256" / "NTLM" / "UNKNOWN"
bool IsMD5(string hash)
bool IsSHA256(string hash)
bool IsNTLM(string hash)             // NTLM = 32 hex chars, same as MD5 — note caveat

// HashComputer.cs
string ComputeMD5(string plaintext)
string ComputeSHA1(string plaintext)
string ComputeSHA256(string plaintext)
string ComputeSHA512(string plaintext)

// CrackEngine.cs
CrackResult? TryCrack(HashTarget target, List<string> wordlist)
CrackResult? TryCrackWithRules(HashTarget target, List<string> wordlist)

// MutationEngine.cs
IEnumerable<string> Mutate(string word)
// yields: word, Word, WORD, w0rd, word1, word123, word!, wordword

// ProgressPrinter.cs
void Update(long attempts, long total, string current)
void PrintResult(CrackResult result)
```

**C# Concepts Practiced:**
- `System.Security.Cryptography` — `MD5`, `SHA256`, `SHA512`
- `Convert.ToHexString()` / `BitConverter.ToString()`
- `IEnumerable<string>` with `yield return`
- `Stopwatch` for timing
- `StreamReader` for large file reading
- `string.ToLower()`, `ToUpper()`, `Replace()` for mutations

**Expected Output:**
```
[*] Hash   : 5f4dcc3b5aa765d61d8327deb882cf99
[*] Type   : MD5
[*] Wordlist: rockyou.txt (14,344,391 words)
[*] Rules  : enabled (x8 mutations per word)

[*] Trying... 142,880 attempts/sec
[+] CRACKED!
    Hash      : 5f4dcc3b5aa765d61d8327deb882cf99
    Plaintext : password
    Attempts  : 3,241
    Time      : 0.02s
```

---

## C# Tool 5 — PayloadForge
### *Payload Encoder & Obfuscator*

**Purpose:** Encode, obfuscate, and transform payloads for AV evasion and WAF bypass testing. Support multiple encoding chains, XOR encryption, and format conversions.

**Pentest use case:** Payload delivery — transform shellcode, commands, or scripts to evade filters and signature-based detection.

---

**Modules / Classes:**

```
PayloadForge/
├── Program.cs
├── Encoders/
│   ├── Base64Encoder.cs     — encode/decode Base64
│   ├── HexEncoder.cs        — encode/decode hex (\\x41 or 41 42 43 format)
│   ├── XorEncoder.cs        — XOR with single byte or rolling key
│   ├── UrlEncoder.cs        — URL encode/decode (%41, %20, double encoding)
│   └── Rot13Encoder.cs      — ROT13 / ROT47 character substitution
├── Obfuscators/
│   ├── CaseRandomizer.cs    — rAnDoM cAsE to bypass case-sensitive filters
│   ├── CommentInserter.cs   — insert /**/ between SQL/JS tokens
│   ├── UnicodeConverter.cs  — convert chars to \u0041 unicode escapes
│   └── ChainBuilder.cs      — apply multiple encoders sequentially
├── Generators/
│   ├── ShellcodeFormatter.cs — format raw bytes as C#/Python/PS array
│   └── PayloadTemplates.cs  — common payloads: reverse shell, cmd exec
├── Models/
│   ├── Payload.cs           — raw bytes, encoding chain, output format
│   └── EncodingStep.cs      — encoder name, key, options
└── Output/
    └── OutputFormatter.cs    — print as hex dump, C# array, Python bytes, raw
```

**Required Methods:**

```csharp
// Base64Encoder.cs
string Encode(string input)
string Decode(string b64)
byte[] EncodeBytes(byte[] data)

// XorEncoder.cs
byte[] Encrypt(byte[] data, byte key)
byte[] Encrypt(byte[] data, byte[] rollingKey)
string ToHexString(byte[] data)

// HexEncoder.cs
string ToHex(byte[] data, string format = "\\x")   // \x41\x42 or 41 42
byte[] FromHex(string hex)

// UrlEncoder.cs
string Encode(string input, bool doubleEncode = false)
string Decode(string input)

// ChainBuilder.cs
byte[] ApplyChain(byte[] input, List<EncodingStep> steps)
string DescribeChain(List<EncodingStep> steps)

// ShellcodeFormatter.cs
string FormatAsCSharp(byte[] shellcode)    // byte[] buf = new byte[] { 0x90, ... };
string FormatAsPython(byte[] shellcode)    // buf = b"\x90\x90..."
string FormatAsHexDump(byte[] shellcode)   // 0000: 90 90 CC C3  ....
```

**C# Concepts Practiced:**
- `byte[]` manipulation, bitwise XOR `^`
- `Convert.ToBase64String()`, `Convert.FromBase64String()`
- `Encoding.UTF8.GetBytes()` / `GetString()`
- `string.Format()`, `StringBuilder`
- Method chaining pattern
- `switch` expression for encoder selection
- LINQ: `Select()` on `byte[]`

**Expected Output:**
```
[*] Input      : whoami
[*] Chain      : UTF8 → XOR(0x41) → Base64 → URL-encode

[STEP 1] XOR(0x41)  : 36 29 2E 20 28 24
[STEP 2] Base64      : NiksICgk
[STEP 3] URL-encode  : NiksICgk   (no special chars — unchanged)

[OUTPUT]
  Raw       : NiksICgk
  C# array  : byte[] buf = new byte[] { 0x4E, 0x69, 0x6B, 0x73, 0x49, 0x43, 0x67, 0x6B };
  Python    : buf = b"\x4E\x69\x6B\x73\x49\x43\x67\x6B"
```

---
---

# 🟡 PYTHON TOOLS

---

## Python Tool 1 — NetRecon
### *Network Reconnaissance & Host Discovery Scanner*

**Purpose:** Discover live hosts on a network using ICMP ping + TCP probing, then perform OS fingerprinting based on TTL values and open port signatures.

**Pentest use case:** Pre-attack reconnaissance — map the network before targeted scanning.

---

**Modules / Files:**

```
net_recon/
├── main.py
├── scanner/
│   ├── host_discovery.py    — ICMP ping + TCP SYN probe to discover live hosts
│   ├── port_prober.py       — quick TCP connect on common ports
│   └── os_fingerprint.py    — guess OS from TTL, open ports, banner patterns
├── utils/
│   ├── ip_utils.py          — parse CIDR, validate IPs, expand ranges
│   ├── subnet_gen.py        — generate all IPs in a subnet
│   └── geo_lookup.py        — (optional) map IP to country via offline DB
├── output/
│   ├── table_printer.py     — formatted terminal table with colors
│   └── json_exporter.py     — save results as JSON
└── models/
    └── host.py              — Host dataclass: ip, hostname, os_guess, open_ports, ttl
```

**Required Functions:**

```python
# host_discovery.py
def ping(ip: str, timeout: float = 1.0) -> bool
def tcp_probe(ip: str, ports: list[int], timeout: float = 0.5) -> bool
def discover_hosts(subnet: str) -> list[str]

# port_prober.py
def probe_common_ports(ip: str, top_n: int = 20) -> list[int]
COMMON_PORTS = [21, 22, 23, 25, 53, 80, 110, 443, 445, 3306, 3389, 8080]

# os_fingerprint.py
def guess_os_from_ttl(ttl: int) -> str          # 64→Linux, 128→Windows, 255→Cisco
def guess_os_from_ports(open_ports: list) -> str
def fingerprint(ip: str, open_ports: list, ttl: int) -> str

# ip_utils.py
def expand_cidr(cidr: str) -> list[str]
def validate_ip(ip: str) -> bool
def ip_to_int(ip: str) -> int

# table_printer.py
def print_host_table(hosts: list[Host]) -> None
def print_summary(hosts: list[Host]) -> None
```

**Python Concepts Practiced:**
- `socket` module — TCP connect, timeout
- `subprocess` — call system `ping`
- `ipaddress` module — `IPv4Network`, `ip_network(cidr)`
- `dataclasses.dataclass`
- `concurrent.futures.ThreadPoolExecutor` — parallel scanning
- List comprehensions, f-strings, type hints

**Expected Output:**
```
[*] Scanning 192.168.1.0/24 ...

IP             HOSTNAME        OS GUESS    OPEN PORTS        TTL
192.168.1.1    router.local    Cisco       80, 443           255
192.168.1.5    DESKTOP-A1B2    Windows     135, 445, 3389    128
192.168.1.10   ubuntu-srv      Linux       22, 80            64

[*] Live hosts: 3 / 254 scanned
```

---

## Python Tool 2 — WebProbe
### *Web Application Fingerprinter & Vulnerability Spotter*

**Purpose:** Fingerprint a web application — detect framework, CMS, server software, interesting headers, and flag known vulnerable versions or misconfigurations.

**Pentest use case:** Web recon — understand the target stack and identify low-hanging fruit before manual testing.

---

**Modules / Files:**

```
web_probe/
├── main.py
├── probes/
│   ├── header_analyzer.py   — parse response headers for tech stack clues
│   ├── cms_detector.py      — detect WordPress, Joomla, Drupal by paths/patterns
│   ├── waf_detector.py      — detect WAF presence (Cloudflare, ModSecurity, etc.)
│   └── vuln_checker.py      — flag known vulnerable versions from banner info
├── crawl/
│   ├── link_extractor.py    — extract all hrefs from HTML response
│   └── form_finder.py       — find and list all HTML forms + input fields
├── utils/
│   ├── http_client.py       — requests wrapper with retry, custom UA, proxy support
│   ├── url_utils.py         — URL normalizer, path joiner, domain extractor
│   └── pattern_db.py        — dict of CMS fingerprints, vuln version signatures
├── output/
│   ├── report_printer.py    — colored terminal output by finding severity
│   └── markdown_report.py   — generate .md report of all findings
└── models/
    └── finding.py           — Finding dataclass: type, severity, evidence, url
```

**Required Functions:**

```python
# header_analyzer.py
def analyze(headers: dict) -> list[Finding]
def extract_server(headers: dict) -> str           # "Apache/2.4.49"
def extract_tech_stack(headers: dict) -> list[str] # ["PHP/7.4", "WordPress"]
def check_security_headers(headers: dict) -> list[Finding]
# flags missing: X-Frame-Options, CSP, HSTS, X-Content-Type-Options

# cms_detector.py
def detect(url: str, html: str) -> str | None
def check_wordpress(url: str) -> bool    # /wp-login.php, /wp-content/, generator meta
def check_joomla(url: str) -> bool
def check_drupal(url: str) -> bool

# waf_detector.py
def detect_waf(response) -> str | None   # check headers + body for WAF signatures

# vuln_checker.py
def check_version(software: str, version: str) -> list[Finding]
# e.g. Apache 2.4.49 → CVE-2021-41773 Path Traversal

# form_finder.py
def find_forms(html: str) -> list[dict]  # [{"action":"/login","inputs":["user","pass"]}]

# http_client.py
def get(url: str, timeout: int = 5, headers: dict = None) -> requests.Response | None
def get_with_retry(url: str, retries: int = 3) -> requests.Response | None
```

**Python Concepts Practiced:**
- `requests` library — headers, redirects, timeout
- `re` — regex for version extraction, meta tag parsing
- `BeautifulSoup` — HTML parsing for forms and links
- `dataclasses`, type hints
- `dict` comprehensions for header analysis
- `any()`, `all()` for pattern matching

**Expected Output:**
```
[*] Target: http://10.0.0.1

[SERVER]
  Software  : Apache/2.4.49
  Language  : PHP/7.4.3
  CMS       : WordPress 5.8.1

[CRITICAL] CVE-2021-41773 — Apache 2.4.49 Path Traversal / RCE
[HIGH]     WordPress 5.8.1 — outdated, multiple known CVEs
[HIGH]     WAF: Not detected — direct exploitation possible
[MEDIUM]   Missing header: Content-Security-Policy
[MEDIUM]   Missing header: X-Frame-Options (Clickjacking risk)
[LOW]      X-Powered-By header exposed: PHP/7.4.3

[FORMS FOUND]
  /wp-login.php   — inputs: log, pwd, wp-submit
  /contact        — inputs: name, email, message
```

---

## Python Tool 3 — CredzHarvest
### *Credential & Secret Hunter in Files and Source Code*

**Purpose:** Recursively scan files, directories, or git repositories to find hardcoded credentials, API keys, tokens, private keys, and sensitive configuration values.

**Pentest use case:** Post-exploitation / code review — extract credentials from compromised systems, source code dumps, or backup archives.

---

**Modules / Files:**

```
credz_harvest/
├── main.py
├── scanners/
│   ├── file_scanner.py      — scan individual file content
│   ├── dir_scanner.py       — recursive directory walk with extension filter
│   └── git_scanner.py       — scan git history (commit diffs) for leaked secrets
├── patterns/
│   ├── pattern_db.py        — regex patterns for 30+ secret types
│   └── entropy.py           — Shannon entropy calculator for high-entropy strings
├── filters/
│   ├── false_positive.py    — ignore placeholder values like "your_key_here"
│   └── extension_filter.py  — only scan relevant file types
├── output/
│   ├── finding_printer.py   — print findings with file path, line number, context
│   └── sarif_exporter.py    — export in SARIF format (compatible with GitHub Security)
└── models/
    └── secret_finding.py    — SecretFinding dataclass: type, value, file, line, entropy
```

**Required Functions:**

```python
# pattern_db.py
PATTERNS = {
    "AWS_ACCESS_KEY"    : r"AKIA[0-9A-Z]{16}",
    "AWS_SECRET_KEY"    : r"(?i)aws.{0,20}secret.{0,20}['\"][0-9a-zA-Z/+]{40}['\"]",
    "GITHUB_TOKEN"      : r"ghp_[0-9a-zA-Z]{36}",
    "PRIVATE_KEY"       : r"-----BEGIN (RSA|EC|OPENSSH) PRIVATE KEY-----",
    "GENERIC_PASSWORD"  : r"(?i)(password|passwd|pwd)\s*[=:]\s*['\"]?.{6,}",
    "BEARER_TOKEN"      : r"(?i)bearer\s+[a-zA-Z0-9\-_.~+/]{20,}",
    "DB_CONNECTION"     : r"(?i)(mysql|postgres|mongodb):\/\/\w+:\w+@",
    # ... 23 more patterns
}

# file_scanner.py
def scan_file(path: str) -> list[SecretFinding]
def scan_line(line: str, line_num: int, path: str) -> list[SecretFinding]

# dir_scanner.py
def scan_directory(root: str, extensions: list[str] = None) -> list[SecretFinding]
def should_skip(path: str) -> bool   # skip node_modules, .git objects, binaries

# entropy.py
def shannon_entropy(data: str) -> float
def is_high_entropy(value: str, threshold: float = 4.5) -> bool

# false_positive.py
def is_placeholder(value: str) -> bool   # "your_key_here", "xxxx", "example"
def filter_findings(findings: list) -> list[SecretFinding]

# git_scanner.py
def scan_git_history(repo_path: str) -> list[SecretFinding]
def get_commit_diffs(repo_path: str) -> list[str]
```

**Python Concepts Practiced:**
- `re` — compile and match patterns at scale
- `os.walk()` for recursive directory traversal
- `math.log2()` for entropy calculation
- `pathlib.Path`
- `dataclasses`, `@dataclass`
- Generator functions with `yield` for memory efficiency
- Exception handling: `UnicodeDecodeError` for binary files

**Expected Output:**
```
[*] Scanning /var/www/html ... (1,847 files)

[CRITICAL] AWS_ACCESS_KEY
  File  : /var/www/html/config/aws.php (line 14)
  Value : AKIAIOSFODNN7EXAMPLE
  Entropy: 4.89

[CRITICAL] PRIVATE_KEY
  File  : /var/www/html/keys/id_rsa (line 1)
  Value : -----BEGIN OPENSSH PRIVATE KEY-----

[HIGH] GENERIC_PASSWORD
  File  : /var/www/html/.env (line 8)
  Value : DB_PASSWORD="Sup3rS3cr3t!"

[HIGH] DB_CONNECTION
  File  : /var/www/html/config/database.py (line 3)
  Value : mysql://root:toor@localhost/prod_db

[*] Scan complete — 4 findings in 1,847 files
```

---

## Python Tool 4 — BruteBox
### *Modular Authentication Brute Forcer*

**Purpose:** Brute-force authentication for multiple protocols — SSH, FTP, HTTP Basic Auth, and web login forms — using credential lists with rate limiting and proxy support.

**Pentest use case:** Credential attacks during authorized engagements — test password strength across services.

---

**Modules / Files:**

```
brute_box/
├── main.py
├── modules/
│   ├── ssh_brute.py         — SSH brute force using paramiko
│   ├── ftp_brute.py         — FTP login attempts via ftplib
│   ├── http_basic_brute.py  — HTTP Basic Auth via requests
│   └── form_brute.py        — POST-based web login form brute force
├── core/
│   ├── engine.py            — main attack loop, threading, rate limit
│   ├── credential_loader.py — load and combine usernames + passwords
│   └── result_tracker.py    — track attempts, hits, errors, speed
├── evasion/
│   ├── rate_limiter.py      — delay between requests (fixed / jitter / adaptive)
│   └── proxy_rotator.py     — rotate through SOCKS5/HTTP proxy list
├── output/
│   ├── live_display.py      — progress bar + attempts/sec + found credentials
│   └── hit_logger.py        — log valid credentials to file immediately on find
└── models/
    └── credential.py        — Credential dataclass: username, password, status
```

**Required Functions:**

```python
# engine.py
def run(module: str, target: str, credentials: list, threads: int = 5) -> list[Credential]
def worker(cred: Credential, attack_func: callable) -> Credential

# credential_loader.py
def load_userlist(path: str) -> list[str]
def load_passlist(path: str) -> list[str]
def combine(users: list[str], passwords: list[str]) -> list[Credential]
def load_combo_file(path: str) -> list[Credential]   # "user:pass" format

# ssh_brute.py
def try_login(host: str, port: int, username: str, password: str) -> bool

# ftp_brute.py
def try_login(host: str, username: str, password: str) -> bool

# http_basic_brute.py
def try_login(url: str, username: str, password: str) -> bool

# form_brute.py
def try_login(url: str, username: str, password: str,
              user_field: str, pass_field: str,
              failure_string: str) -> bool

# rate_limiter.py
def wait(mode: str = "fixed", delay: float = 0.5) -> None
# modes: "fixed", "jitter" (random ±50%), "adaptive" (slow on errors)

# result_tracker.py
def update(attempt: int, total: int, found: int, speed: float) -> None
def print_summary(results: list[Credential]) -> None
```

**Python Concepts Practiced:**
- `paramiko` — SSH client
- `ftplib.FTP` — FTP login
- `requests` — HTTP with auth, session, cookies
- `threading.Thread`, `queue.Queue`
- `time.sleep()`, `random.uniform()` for jitter
- `dataclasses`, type hints
- `try/except` for network errors

**Expected Output:**
```
[*] Module   : SSH Brute Force
[*] Target   : 10.0.0.1:22
[*] Wordlist : 3 users × 10,000 passwords = 30,000 combos
[*] Threads  : 10 | Rate: 0.3s delay | Jitter: ON

[*] Progress : ████████░░ 8,241/30,000 (27%) — 41 attempts/sec

[+] VALID CREDENTIAL FOUND!
    User     : root
    Password : toor
    Protocol : SSH

[*] Attack complete
    Duration : 3m 22s
    Attempts : 8,241
    Found    : 1
```

---

## Python Tool 5 — PktWatch
### *Packet Capture Analyzer & Protocol Dissector*

**Purpose:** Capture live network traffic or parse existing `.pcap` files to extract credentials, interesting headers, DNS queries, HTTP requests, and detect anomalous patterns.

**Pentest use case:** Network sniffing on compromised segments — extract in-transit credentials, map internal traffic, detect C2 beacons.

---

**Modules / Files:**

```
pkt_watch/
├── main.py
├── capture/
│   ├── live_capture.py      — sniff live traffic on an interface using scapy
│   └── pcap_reader.py       — read and parse .pcap / .pcapng files
├── dissectors/
│   ├── http_dissector.py    — extract HTTP requests, responses, cookies, forms
│   ├── dns_dissector.py     — extract DNS queries and responses
│   ├── ftp_dissector.py     — extract FTP USER/PASS commands
│   └── tcp_stream.py        — reassemble TCP streams from raw packets
├── detectors/
│   ├── cred_detector.py     — find credentials in cleartext protocols
│   ├── beacon_detector.py   — detect periodic C2 beacon patterns
│   └── scan_detector.py     — detect port scan patterns (SYN flood, sequential)
├── output/
│   ├── stream_printer.py    — print reassembled TCP conversations
│   └── pcap_summary.py      — top talkers, protocols breakdown, timeline
└── models/
    ├── packet_info.py        — PacketInfo dataclass: src, dst, proto, payload
    └── finding.py            — NetworkFinding: type, severity, evidence, timestamp
```

**Required Functions:**

```python
# live_capture.py
def start_capture(interface: str, packet_count: int = 0, bpf_filter: str = "") -> list
def on_packet(packet) -> None   # scapy callback

# pcap_reader.py
def read_pcap(path: str) -> list[PacketInfo]
def extract_tcp_streams(packets: list) -> dict[tuple, list]

# http_dissector.py
def extract_requests(stream: bytes) -> list[dict]    # method, url, headers, body
def extract_credentials(request: dict) -> list[str]  # find user/pass in POST body
def extract_cookies(response: dict) -> list[str]

# dns_dissector.py
def extract_queries(packets: list) -> list[dict]   # [{"name":"evil.com","type":"A"}]
def detect_dns_tunneling(queries: list) -> bool    # long subdomains = tunneling?

# ftp_dissector.py
def extract_credentials(stream: bytes) -> tuple[str, str] | None  # (user, pass)

# cred_detector.py
def scan_stream(data: bytes, protocol: str) -> list[NetworkFinding]

# beacon_detector.py
def analyze_intervals(connections: list[dict]) -> list[NetworkFinding]
def is_beaconing(timestamps: list[float], jitter_tolerance: float = 0.1) -> bool

# pcap_summary.py
def top_talkers(packets: list, top_n: int = 10) -> dict
def protocol_breakdown(packets: list) -> dict
def print_summary(packets: list) -> None
```

**Python Concepts Practiced:**
- `scapy` — packet sniffing and crafting
- `struct` — binary data parsing
- `collections.Counter`, `defaultdict`
- Generator functions — process packets lazily
- `statistics.stdev()` — beacon interval analysis
- `re` — credential pattern matching in raw streams
- `datetime` — packet timestamp handling

**Expected Output:**
```
[*] Reading: capture.pcap (12,847 packets)

===== PROTOCOL BREAKDOWN =====
  TCP    : 8,421 (65.5%)
  DNS    : 2,103 (16.4%)
  UDP    : 1,891 (14.7%)
  ICMP   :   432 ( 3.4%)

===== FINDINGS =====

[CRITICAL] FTP Credentials in Cleartext
  Stream : 10.0.0.5:51234 → 10.0.0.1:21
  USER   : ftpuser
  PASS   : F!leS3rver2024

[HIGH] HTTP POST — Possible Login Form
  URL    : http://10.0.0.1/login
  Body   : username=admin&password=admin123

[HIGH] DNS Tunneling Suspected
  Host   : 10.0.0.99
  Query  : 119-104-111-97-109-105.evil.com
  Reason : High-entropy subdomain, 47 queries in 60s

[MEDIUM] C2 Beacon Pattern Detected
  Host   : 10.0.0.50 → 185.220.101.5
  Interval: every ~5000ms (±3% jitter)
  Count  : 23 connections over 2 minutes

===== TOP TALKERS =====
  10.0.0.99   →  4,221 packets (attacker?)
  10.0.0.5    →  2,841 packets
  10.0.0.1    →  1,993 packets
```
