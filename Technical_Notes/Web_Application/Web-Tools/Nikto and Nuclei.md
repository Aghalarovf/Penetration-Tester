# Nikto & Nuclei Cheat Sheet

> Web vulnerability scanner reference for penetration testers and security researchers.

---

## NIKTO

### Installation

```bash
apt install nikto          # Debian/Ubuntu
git clone https://github.com/sullo/nikto  # From source
```

### Basic Usage

| Command | Purpose |
|--------|---------|
| `nikto -h <target>` | Basic scan against a host |
| `nikto -h https://target.com` | Scan HTTPS target |
| `nikto -h target.com -p 8080` | Scan on a specific port |
| `nikto -h target.com -p 80,443,8080` | Scan multiple ports |

### Output & Reporting

| Command | Purpose |
|--------|---------|
| `nikto -h target.com -o report.html -Format html` | Save output as HTML |
| `nikto -h target.com -o report.txt -Format txt` | Save output as plain text |
| `nikto -h target.com -o report.xml -Format xml` | Save output as XML |
| `nikto -h target.com -o report.csv -Format csv` | Save output as CSV |

### Authentication

| Command | Purpose |
|--------|---------|
| `nikto -h target.com -id admin:password` | HTTP basic authentication |
| `nikto -h target.com -C all` | Check all CGI directories |

### Proxy & Evasion

| Command | Purpose |
|--------|---------|
| `nikto -h target.com -useproxy http://127.0.0.1:8080` | Route traffic through proxy (e.g. Burp) |
| `nikto -h target.com -evasion 1` | URL encoding evasion |
| `nikto -h target.com -evasion 2` | Random URI encoding |
| `nikto -h target.com -evasion 8` | Use Windows directory separator |

> Combine evasion modes: `-evasion 1,2,3`

### Tuning & Plugins

| Command | Purpose |
|--------|---------|
| `nikto -h target.com -Tuning 1` | Scan for interesting files |
| `nikto -h target.com -Tuning 2` | Scan for misconfigurations |
| `nikto -h target.com -Tuning 9` | SQL injection checks |
| `nikto -h target.com -Tuning x` | Reverse tuning (exclude a type) |
| `nikto -list-plugins` | List all available plugins |
| `nikto -h target.com -Plugins headers` | Run specific plugin only |

**Tuning Reference:**

| Code | Category |
|------|----------|
| `1` | Interesting files |
| `2` | Misconfiguration |
| `3` | Information disclosure |
| `4` | Injection (XSS/Script) |
| `5` | Remote file retrieval (inside web root) |
| `6` | Denial of service |
| `7` | Remote file retrieval (server wide) |
| `8` | Command execution / Remote shell |
| `9` | SQL injection |
| `0` | File upload |
| `a` | Authentication bypass |
| `b` | Software identification |

### SSL & Timing

| Command | Purpose |
|--------|---------|
| `nikto -h target.com -ssl` | Force SSL mode |
| `nikto -h target.com -nossl` | Disable SSL |
| `nikto -h target.com -timeout 10` | Set request timeout (seconds) |
| `nikto -h target.com -T 5` | Pause between requests (seconds) |

### Multiple Targets

```bash
nikto -h targets.txt              # Scan list of hosts from file
nikto -h 192.168.1.0/24          # Scan CIDR range (use with nmap output)
nmap -p80 192.168.1.0/24 -oG - | nikto -h -   # Pipe nmap results into nikto
```

---

## NUCLEI

### Installation

```bash
go install -v github.com/projectdiscovery/nuclei/v3/cmd/nuclei@latest
nuclei -update-templates    # Download/update template library
```

### Basic Usage

| Command | Purpose |
|--------|---------|
| `nuclei -u https://target.com` | Scan single URL with all templates |
| `nuclei -l urls.txt` | Scan list of URLs from file |
| `nuclei -u target.com -as` | Automatic scan (smart template selection) |

### Template Selection

| Command | Purpose |
|--------|---------|
| `nuclei -u target.com -t cves/` | Run all CVE templates |
| `nuclei -u target.com -t exposures/` | Check for exposed files/configs |
| `nuclei -u target.com -t misconfiguration/` | Check for misconfigurations |
| `nuclei -u target.com -t technologies/` | Detect technologies in use |
| `nuclei -u target.com -t fuzzing/` | Run fuzzing templates |
| `nuclei -u target.com -t network/` | Network-level checks |
| `nuclei -t /path/to/custom.yaml -u target.com` | Run a custom template |

### Severity Filtering

| Command | Purpose |
|--------|---------|
| `nuclei -u target.com -s critical,high` | Only critical and high severity |
| `nuclei -u target.com -s medium` | Only medium severity findings |
| `nuclei -u target.com -es info` | Exclude informational results |

### Tags & Filtering

| Command | Purpose |
|--------|---------|
| `nuclei -u target.com -tags sqli` | Run only SQL injection templates |
| `nuclei -u target.com -tags xss,lfi` | Run XSS and LFI templates |
| `nuclei -u target.com -tags cve` | Run all CVE-tagged templates |
| `nuclei -u target.com -exclude-tags dos` | Exclude denial-of-service templates |
| `nuclei -u target.com -id CVE-2021-44228` | Run a specific template by ID |

### Output & Reporting

| Command | Purpose |
|--------|---------|
| `nuclei -u target.com -o output.txt` | Save results to text file |
| `nuclei -u target.com -json -o output.json` | Save results as JSON |
| `nuclei -u target.com -markdown-export ./report/` | Export results as Markdown |
| `nuclei -u target.com -silent` | Show only findings, no banner |
| `nuclei -u target.com -v` | Verbose output |

### Rate Limiting & Performance

| Command | Purpose |
|--------|---------|
| `nuclei -u target.com -rl 100` | Limit to 100 requests/second |
| `nuclei -u target.com -c 50` | Set concurrency to 50 threads |
| `nuclei -u target.com -timeout 10` | Set request timeout (seconds) |
| `nuclei -u target.com -retries 3` | Retry failed requests 3 times |
| `nuclei -u target.com -bs 25` | Set bulk size (templates per batch) |

### Proxy & Headers

| Command | Purpose |
|--------|---------|
| `nuclei -u target.com -proxy http://127.0.0.1:8080` | Route through proxy (e.g. Burp) |
| `nuclei -u target.com -H "Cookie: session=abc123"` | Add custom header |
| `nuclei -u target.com -H "Authorization: Bearer <token>"` | Add auth header |

### Authentication

| Command | Purpose |
|--------|---------|
| `nuclei -u target.com -pa` | Use passive mode (no active fuzzing) |
| `nuclei -u target.com -auth-strategies bearer` | Use bearer token auth |

### Template Management

| Command | Purpose |
|--------|---------|
| `nuclei -update-templates` | Update template library |
| `nuclei -tl` | List all available templates |
| `nuclei -validate -t template.yaml` | Validate a custom template |
| `nuclei -u target.com -nt` | Skip template update check |

### Writing a Basic Template

```yaml
id: example-check

info:
  name: Example Header Check
  author: yourname
  severity: info
  tags: custom

requests:
  - method: GET
    path:
      - "{{BaseURL}}"
    matchers:
      - type: word
        words:
          - "X-Powered-By"
        part: header
```

---

## Quick Comparison

| Feature | Nikto | Nuclei |
|--------|-------|--------|
| Speed | Slower | Fast |
| Templates | Built-in plugins | Community YAML templates |
| CVE coverage | Limited | Extensive |
| Custom checks | Limited | Fully customizable |
| Output formats | HTML, XML, CSV, TXT | JSON, Markdown, TXT |
| Active fuzzing | Yes | Yes (with fuzzing templates) |

---

*Use responsibly. Only scan systems you have explicit permission to test.*
