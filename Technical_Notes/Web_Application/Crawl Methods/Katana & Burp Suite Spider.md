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

## 2. Katana Flags Reference

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

## 3. Katana → BurpSuite Integration

### Basic Proxy Passthrough

```powershell
# All Katana traffic → Burp (HTTP)
katana -u https://target.com -proxy http://127.0.0.1:8080

# HTTPS with Burp CA trust (export cert first)
katana -u https://target.com \
  -proxy http://127.0.0.1:8080 \
  -headless
```

### 
```powershell
katana -u https://www.inlanefreight.com/ \
  -d 5 \
  -jc \
  -headless \
  -xhr \
  -known-files all \
  -proxy http://127.0.0.1:8080 \
  -o inlanefreight_full.txt \
  -silent \
  -c 15 \
  -rl 30
```

# Burp Suite Spider
<img width="592" height="262" alt="image" src="https://github.com/user-attachments/assets/4e4b125e-3d0a-43cf-ad54-f452af9a128a" />
```powershell
Target --> Sitemap --> www.inlanefreight.com --> Scan --> 
```
