# Katana + BurpSuite Spider — Professional Cheat Sheet

## 1. Installation & Setup

### Katana

```bash
# Install via Go
go install github.com/projectdiscovery/katana/cmd/katana@latest

# Install via apt (ProjectDiscovery repo)
sudo apt install katana

# Verify
katana -version
```

### BurpSuite

```bash
# Download from official site
https://portswigger.net/burp/releases

# Start listener (default proxy)
# Proxy → Options → 127.0.0.1:8080

# Export CA certificate for HTTPS interception
# Proxy → Options → Import / export CA certificate
```

### Chrome/Chromium (for Katana headless mode)

```bash
# Required for JS rendering (-headless flag)
sudo apt install chromium-browser
# or
which google-chrome  # Verify path
```

---

## 2. Core Concepts

| Concept | Katana | BurpSuite Spider |
|---|---|---|
| Crawl Type | CLI-based, fast passive + active | GUI-based, manual trigger |
| JS Rendering | ✅ Headless Chromium | ✅ Built-in browser |
| Scope Control | `-field`, `-cs`, `-fs` flags | Suite Scope → Target tab |
| Output Format | JSON, JSONL, TXT | Site Map (GUI), export |
| Proxy Integration | `-proxy` flag → BurpSuite | Native — IS the proxy |
| Best Used For | Discovery & feeding Burp | Deep analysis, auth flows |

**Recommended Workflow:**

```
Katana (crawl) ──proxy──► BurpSuite (intercept + passive scan)
                                  │
                                  └──► Active Scan / Intruder / Repeater
```

---

## 3. Katana Flags Reference

### Essential Flags

| Flag | Description | Example |
|---|---|---|
| `-u` | Single target URL | `-u https://target.com` |
| `-list` | File with multiple targets | `-list urls.txt` |
| `-d` | Crawl depth (default: 3) | `-d 5` |
| `-jc` | Enable JS crawling (parse JS files) | `-jc` |
| `-headless` | Headless browser mode (JS rendering) | `-headless` |
| `-proxy` | Send traffic through proxy | `-proxy http://127.0.0.1:8080` |
| `-o` | Output file | `-o results.txt` |
| `-json` | JSON output format | `-json` |
| `-silent` | Suppress banner/info | `-silent` |
| `-nc` | No color output | `-nc` |

### Scope Control

| Flag | Description | Example |
|---|---|---|
| `-cs` | Crawl scope (regex) | `-cs target\.com` |
| `-fs` | Field scope (path, fqdn, rdn, etc.) | `-fs fqdn` |
| `-field` | Extract specific fields | `-field url,method,body` |
| `-or` | Include out-of-scope in output | `-or` |
| `-rl` | Rate limit (req/sec) | `-rl 50` |
| `-rd` | Redirect following depth | `-rd 3` |
| `-ct` | Crawl timeout per URL (sec) | `-ct 10` |
| `-timeout` | Global timeout (sec) | `-timeout 60` |

### Headless / Browser Options

| Flag | Description | Example |
|---|---|---|
| `-headless` | Enable headless mode | `-headless` |
| `-system-chrome` | Use system Chrome binary | `-system-chrome` |
| `-headless-options` | Pass Chrome flags | `-headless-options --disable-gpu` |
| `-no-sandbox` | Disable Chrome sandbox | `-no-sandbox` |
| `-xhr` | Include XHR requests | `-xhr` |
| `-known-files` | Crawl robots.txt / sitemap.xml | `-known-files all` |

### Output Formatting

| Flag | Description |
|---|---|
| `-json` | JSON line-by-line output |
| `-jsonl` | Newline-delimited JSON |
| `-output` | Write to file |
| `-store-response` | Save raw HTTP responses |
| `-store-response-dir` | Directory for saved responses |
| `-display-out-scope` | Show out-of-scope discovered URLs |
| `-field url` | Extract only URLs |
| `-field method` | Extract HTTP methods |
| `-field body` | Extract request bodies |

### Concurrency & Performance

| Flag | Default | Description |
|---|---|---|
| `-c` | 10 | Concurrency (parallel goroutines) |
| `-p` | 10 | Parallelism (crawl workers) |
| `-rl` | 150 | Rate limit (req/sec) |
| `-rlt` | 10s | Rate limit timeout |

---

## 4. BurpSuite Spider Reference

### Spider / Crawler Settings

```
Target → Site Map → Right-click → Spider this host
Dashboard → New Scan → Crawl only
```

| Setting | Location | Recommended Value |
|---|---|---|
| Crawl depth | Scan config → Crawl | 7–10 |
| Max links | Scan config → Crawl | Unlimited (BBH) |
| Render JS | Scan config → Crawl | Enabled |
| Login form handling | Scan config → Crawl | Automatic |
| Scope check | Target → Scope | Include regex |

### Passive Spider (Proxy-based)

BurpSuite passively maps every request flowing through its proxy:

```
Proxy → Intercept ON/OFF
 └── All traffic logged to: Target → Site Map
```

Enable passive discovery:
```
Proxy → Options → Miscellaneous
  ✅ Use Burp's browser
  ✅ Parse and store requests automatically
```

### Scope Configuration

```
Target → Scope → Add
  ┌────────────────────────────────────┐
  │ Protocol: https                    │
  │ Host/IP: .*\.target\.com           │  ← regex allowed
  │ Port: ^(443|8443)$                 │
  │ File: .*                           │
  └────────────────────────────────────┘
```

---

## 5. Katana → BurpSuite Integration

### Basic Proxy Passthrough

```bash
# All Katana traffic → Burp (HTTP)
katana -u https://target.com -proxy http://127.0.0.1:8080

# HTTPS with Burp CA trust (export cert first)
katana -u https://target.com \
  -proxy http://127.0.0.1:8080 \
  -headless
```

> **Note:** For HTTPS targets in headless mode, install Burp's CA cert into the system/Chrome store.

### Install Burp CA for Headless Chrome

```bash
# Export cert from Burp: Proxy → Options → CA Certificate → Export DER
openssl x509 -inform DER -in burp_ca.der -out burp_ca.crt

# Install system-wide (Linux)
sudo cp burp_ca.crt /usr/local/share/ca-certificates/
sudo update-ca-certificates

# Pass to Chrome headless
katana -u https://target.com \
  -headless \
  -proxy http://127.0.0.1:8080 \
  -headless-options "--ignore-certificate-errors"
```

### Feeding Katana URLs Back into Burp

```bash
# Save all discovered URLs to file
katana -u https://target.com -o discovered_urls.txt -silent

# Then import into Burp:
# Target → Site Map → right-click → Load from file
```

### Automated Site Map Population

```bash
# Run Katana silently and funnel everything into Burp passively
katana -u https://target.com \
  -d 7 \
  -jc \
  -headless \
  -proxy http://127.0.0.1:8080 \
  -rl 30 \
  -silent \
  -nc
# Watch BurpSuite Target → Site Map fill up in real-time
```

---

## 6. Crawling Strategies

### Strategy 1 — Quick Surface Scan

```bash
katana -u https://target.com \
  -d 3 \
  -silent \
  -proxy http://127.0.0.1:8080 \
  -o quick_scan.txt
```

### Strategy 2 — Deep JS-Rendered Crawl

```bash
katana -u https://target.com \
  -d 7 \
  -jc \
  -headless \
  -xhr \
  -proxy http://127.0.0.1:8080 \
  -c 15 \
  -rl 40 \
  -o deep_scan.jsonl \
  -json
```

### Strategy 3 — Authenticated Crawl (with cookies)

```bash
# Extract session cookie from Burp, then:
katana -u https://target.com \
  -H "Cookie: session=abc123; csrf=xyz" \
  -H "Authorization: Bearer <token>" \
  -d 5 \
  -headless \
  -proxy http://127.0.0.1:8080
```

### Strategy 4 — Multi-Target Bulk Scan

```bash
# urls.txt: one URL per line
katana -list urls.txt \
  -d 4 \
  -jc \
  -proxy http://127.0.0.1:8080 \
  -o bulk_results.jsonl \
  -json \
  -silent \
  -c 20 \
  -p 5
```

### Strategy 5 — Stealth / Rate-Limited Scan

```bash
katana -u https://target.com \
  -d 5 \
  -rl 5 \
  -rlt 30 \
  -timeout 15 \
  -proxy http://127.0.0.1:8080 \
  -headless \
  -H "User-Agent: Mozilla/5.0 (compatible; Googlebot/2.1)"
```

---

## 7. Output & Filtering

### Extract Specific Fields

```bash
# URLs only
katana -u https://target.com -field url -silent

# Methods + URLs
katana -u https://target.com -field url,method -silent

# Full detail (url, method, body, headers)
katana -u https://target.com -field url,method,body,header -json
```

### Filter by Extension

```bash
# Include only interesting endpoints
katana -u https://target.com \
  -extension-filter js,php,aspx,jsp,json,xml \
  -silent

# Exclude static assets
katana -u https://target.com \
  -extension-filter png,jpg,gif,svg,css,woff,woff2 \
  -silent
```

### Post-Processing with Common Tools

```bash
# Extract unique URLs
katana -u https://target.com -silent | sort -u > urls.txt

# Filter only endpoints with params
katana -u https://target.com -silent | grep "?"

# Extract JS files
katana -u https://target.com -silent | grep "\.js$"

# Find API endpoints
katana -u https://target.com -silent | grep -E "/api/|/v[0-9]+/"

# Count discovered endpoints
katana -u https://target.com -silent | wc -l

# Parse JSON output
katana -u https://target.com -json | jq '.request.url'
katana -u https://target.com -json | jq 'select(.request.method == "POST")'
```

### Store Full Responses

```bash
katana -u https://target.com \
  -store-response \
  -store-response-dir ./responses/ \
  -proxy http://127.0.0.1:8080
```

---

## 8. Automation & Pipelines

### Full Recon Pipeline

```bash
#!/bin/bash
TARGET=$1
OUTPUT_DIR="./recon_$TARGET"
mkdir -p $OUTPUT_DIR

echo "[*] Starting Katana crawl for $TARGET"

katana \
  -u "https://$TARGET" \
  -d 6 \
  -jc \
  -headless \
  -xhr \
  -known-files all \
  -proxy http://127.0.0.1:8080 \
  -c 15 \
  -rl 30 \
  -json \
  -silent \
  -o "$OUTPUT_DIR/raw_crawl.jsonl"

echo "[*] Extracting endpoints..."
cat "$OUTPUT_DIR/raw_crawl.jsonl" \
  | jq -r '.request.url' \
  | sort -u \
  > "$OUTPUT_DIR/urls.txt"

echo "[*] Extracting JS files..."
grep "\.js" "$OUTPUT_DIR/urls.txt" > "$OUTPUT_DIR/js_files.txt"

echo "[*] Extracting API paths..."
grep -E "/api/|/v[0-9]+/|/graphql" "$OUTPUT_DIR/urls.txt" \
  > "$OUTPUT_DIR/api_endpoints.txt"

echo "[+] Done. Results in $OUTPUT_DIR/"
echo "[+] Total URLs discovered: $(wc -l < $OUTPUT_DIR/urls.txt)"
```

### Integration with Other Tools

```bash
# Katana → gf (grep patterns) → vuln hunting
katana -u https://target.com -silent | gf xss
katana -u https://target.com -silent | gf sqli
katana -u https://target.com -silent | gf lfi
katana -u https://target.com -silent | gf ssrf

# Katana → httpx (live check + tech detect)
katana -u https://target.com -silent | httpx -silent -tech-detect

# Katana → nuclei (template-based scanning)
katana -u https://target.com -silent \
  | nuclei -t exposures/ -silent

# Katana → ffuf (param fuzzing on discovered endpoints)
katana -u https://target.com -silent \
  | grep "?" \
  | while read url; do
      ffuf -u "$url&FUZZ=test" -w wordlist.txt -mc 200,302 -silent
    done
```

---

## 9. One-Liners & Real-World Scenarios

### Bug Bounty Quick Start

```bash
katana -u https://target.com -d 5 -jc -headless \
  -proxy http://127.0.0.1:8080 -silent | tee all_urls.txt | wc -l
```

### Find Hidden Admin Panels

```bash
katana -u https://target.com -silent \
  | grep -iE "admin|dashboard|panel|console|manage|control"
```

### Extract All Parameters

```bash
katana -u https://target.com -silent \
  | grep "?" \
  | sed 's/=.*/=/g' \
  | sort -u
```

### Discover File Upload Endpoints

```bash
katana -u https://target.com -json -silent \
  | jq -r 'select(.request.method == "POST") | .request.url'
```

### JS File Analysis

```bash
katana -u https://target.com -silent \
  | grep "\.js$" \
  | xargs -I{} curl -s {} \
  | grep -oP '(https?://[^\s"'"'"']+|/[a-zA-Z0-9_/-]+)'
```

### Discover Subdomains via Crawl

```bash
katana -u https://target.com -d 5 -silent \
  | grep -oP '[a-zA-Z0-9._-]+\.target\.com' \
  | sort -u
```

### Crawl with Custom Headers (WAF Bypass)

```bash
katana -u https://target.com \
  -H "X-Forwarded-For: 127.0.0.1" \
  -H "X-Real-IP: 127.0.0.1" \
  -H "X-Originating-IP: 127.0.0.1" \
  -H "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64)" \
  -proxy http://127.0.0.1:8080 \
  -silent
```

---

## 10. Tips, Tricks & Gotchas

### ⚡ Performance Tips

- Use `-c 20 -p 5` for broader concurrent crawls without hammering the server
- Combine `-rl` with `-rlt` to stay stealthy under WAF thresholds
- Use `-headless` only when necessary — it's 3–5× slower than standard mode
- For large scopes, split targets into chunks and run parallel sessions

### 🔐 Authentication Tips

- Use Burp's **session handling rules** to maintain auth during active scanning
- Katana doesn't natively handle OAuth flows — log in via Burp's browser first, then extract cookies for `-H` flag
- For token refresh: run Katana with `-timeout 300` and short `-rl` to avoid token expiry

### 🛡️ Avoid Detection

```bash
# Randomize User-Agent
katana -u https://target.com \
  -H "User-Agent: $(shuf -n1 /usr/share/seclists/Fuzzing/User-Agents.txt)" \
  -rl 3 \
  -timeout 10
```

### ⚠️ Common Gotchas

| Issue | Cause | Fix |
|---|---|---|
| SSL errors with Burp | CA cert not trusted | Install Burp CA cert system-wide |
| Katana misses SPA content | Headless not enabled | Add `-headless -xhr` flags |
| Empty Burp Site Map | Scope not set in Burp | Add target to `Target → Scope` |
| Crawl loops forever | Circular redirects | Set `-rd 3` (redirect depth) |
| Too many false paths | Dynamic URL generation | Use `-cs` regex to restrict scope |
| Rate limited / blocked | Too aggressive | Reduce `-c`, `-p`, increase `-rl` |

### 📁 Useful Wordlists (SecLists)

```bash
/usr/share/seclists/Discovery/Web-Content/common.txt
/usr/share/seclists/Discovery/Web-Content/raft-large-files.txt
/usr/share/seclists/Discovery/Web-Content/api/api-endpoints.txt
/usr/share/seclists/Fuzzing/User-Agents.txt
```

### 🔗 Burp Extension Recommendations

| Extension | Use Case |
|---|---|
| **JS Miner** | Extract secrets from JS files |
| **Param Miner** | Discover hidden parameters |
| **Retire.js** | Detect vulnerable JS libraries |
| **Logger++** | Advanced request logging |
| **Autorize** | Broken access control testing |

---

## Quick Reference Card

```
┌──────────────────────────────────────────────────────────────────┐
│  KATANA QUICK FLAGS                                              │
│                                                                  │
│  -u <url>       Target URL                                       │
│  -d <int>       Depth (default 3)                                │
│  -jc            Parse JavaScript                                 │
│  -headless      JS rendering (slow but thorough)                 │
│  -xhr           Capture XHR requests                             │
│  -proxy <url>   Send to BurpSuite (127.0.0.1:8080)              │
│  -c <int>       Concurrency (default 10)                         │
│  -rl <int>      Rate limit req/sec (default 150)                 │
│  -cs <regex>    Restrict crawl scope                             │
│  -json          JSON output                                      │
│  -o <file>      Output file                                      │
│  -silent        No banner                                        │
│  -H "K: V"      Custom header                                    │
│                                                                  │
│  BURP INTEGRATION                                                │
│  Start Burp proxy → 127.0.0.1:8080                              │
│  Add target to Scope → Target → Scope                            │
│  Run Katana with -proxy http://127.0.0.1:8080                    │
│  Watch Site Map populate in real-time                            │
└──────────────────────────────────────────────────────────────────┘
```

---

> **Legal Disclaimer:** Use these techniques only on systems you own or have explicit written permission to test. Unauthorized scanning is illegal and unethical.
