# Reconaissance

## Whois Domain/IP
```powershell
whois target.com
whois 10.10.10.10
```

## ASN / Owner
```powershell
dig +short hackthebox.com

whois -h whois.cymru.com "109.176.239.69"

curl -s https://ipinfo.io/109.176.239.69/json
```

## Zone Transfer AXFR/IXFR
```powershell
dig NS +short target.com
dig @ns1.target.com target.com AXFR

dig SOA +short target.com --> 2026090301
dig @ns1.target.com IXFR=2026090300 target.com

dnsrecon -d target.com
```

## Whois History
```powershell
https://www.bigdomaindata.com/whois-history/
```

## Dorks
```powershell
39. site:target.com — bütün indekslənmiş səhifələr
40. site:target.com filetype:pdf — sənədlər, hesabatlar
41. site:target.com filetype:xls OR xlsx — spreadsheetlər
42. site:target.com filetype:sql — database dump?
43. site:target.com filetype:log — log faylları
44. site:target.com inurl:admin — admin panelləri
45. site:target.com inurl:login — login səhifələri
46. site:target.com inurl:config — konfiqurasiya faylları
47. site:target.com intext:"password" — credential sızması
48. site:target.com intext:"api_key" OR "secret_key" — API açarları
49. "target.com" inurl:pastebin — pastebinə sızmış məlumat
50. "@target.com" filetype:xls — email siyahıları
```

## Technology Fingerprint
```powershell
whatweb target.com

curl -I target.com — HTTP başlıqları, Server, X-Powered-By

Cookie adları — PHPSESSID=PHP, JSESSIONID=Java, ASP.NET_SessionId=ASP

JavaScript faylları — framework (React/Vue/Angular), version

https://builtwith.com/baku.tv
```

## SSL / TLS Analyze
```powershell
testssl.sh target.com
```

## Web Content
```powershell
75. robots.txt — gizlədilməyə çalışılan path-lər
76. sitemap.xml — bütün indekslənmiş URL-lər
77. /.well-known/ — security.txt, openid-configuration
78. /security.txt — responsible disclosure kontaktı
79. /.git/ açıqdırmı? — source code sızması!
80. /.env açıqdırmı? — environment variables, credentials!
81. /backup/, /bak/, /old/ — köhnə fayllar
82. /.DS_Store — Mac metadata, path disclosure
83. /crossdomain.xml, /clientaccesspolicy.xml — Flash policy
84. /api/, /graphql, /swagger.json — API endpoints
```

## Javascript & Frontend
```powershell
Bütün JS fayllarını yığ: waybackurls target.com | grep "\.js"

LinkFinder ilə JS-dən endpoint-lər çıxart

Source map faylları: .js.map — original source code açılır!

Console-da error mesajları — path, stack, internal info
```

## Email harvesting
```powershell
theHarvester -d target.com -b all — email, subdomain, host
Hunter.io — email format və əməkdaş emailləri
GitHub — org:targetcompany — şirkətin repo-ları
GitHub search: "target.com" password — sızmış credential
Pastebin, Ghostbin, Hastebin — sızmış data axtarışı

```

# Burp Suite Pro Penetration Testing Roadmap
### Passive Reconnaissance → Post-Exploitation (One Step per Vulnerability)

> Only run this methodology against systems you are explicitly authorized to test (your own lab, PortSwigger Academy, or an in-scope engagement). Set a strict Target Scope in Burp before Step 1 and keep it enabled throughout.

---

## Phase 0 — Setup

- Create a project, define **Target → Scope**.
- Install: `Logger++`, `Hackvertor`, `Active Scan++`, `Param Miner`, `Turbo Intruder`, `Collaborator Everywhere` / `Taborator`, `JS Miner`, `JS Link Finder`, `AutoVader`, `Backslash Powered Scanner`, `Autorize`, `AuthMatrix`, `AutoRepeater`, `Auth Analyzer`, `CSRF Token Tracker`, `Upload Scanner`, `Content Type Converter`, `JWT Editor`, `HTTP Request Smuggler`, `NoSQLi Scanner`, `Java Deserialization Scanner`, `Freddy`, `InQL`, `GraphQL Raider`, `Additional CORS Checks`, `Web Cache Deception Scanner`, `OAUTHScan`, `EsPReSSO`, `SAML Raider`, `Retire.js`, `Software Vulnerability Scanner`, `Backup Finder`, `Burp Bounty`, `Server-Side Prototype Pollution Scanner`, `SQLiPy` / `CO2`, `XSS Validator`, `Reflector`, `WebSocket Turbo Intruder`, `Wsdler`.
- Start **Logger++** and leave it running for the whole engagement.

### Step 1 — Information Disclosure
**Extensions:** `JS Miner`, `Retire.js`, `Software Vulnerability Scanner`, `Backup Finder`, `Param Miner`
Review JS for secrets/source maps, check for outdated libraries, probe for `.bak`/`.old`/`.zip` leftovers, and check verbose error messages and headers via Logger++.

---

### Step 2 — Clickjacking
**Extensions:** `Burp Clickbandit` (built-in)
Record a clickjacking PoC on sensitive pages lacking `X-Frame-Options` / CSP `frame-ancestors`.

---

### Step 3 — CORS Misconfiguration
**Extensions:** `Additional CORS Checks`
Auto-flags permissive `Access-Control-Allow-Origin` combined with `Access-Control-Allow-Credentials: true`.

```powershell
Target --> SiteMap --> Extension --> Extensions --> CORS*
```

---

### Step 4 — HTTP Host Header Attacks
**Extensions:** `Param Miner`, `Collaborator Everywhere` / `Taborator`, `Active Scan++`
Test password-reset/email links and routing logic for Host header injection, confirm via Collaborator callback.

### Step 5 — Web Cache Deception
**Extensions:** `Web Cache Deception Scanner`, `Param Miner`
Append path confusion (`/profile/nonexistent.css`) to cacheable sensitive endpoints and check if the response gets cached for other users.

### Step 6 — Web Cache Poisoning
**Extensions:** `Param Miner`, `Turbo Intruder`, `Active Scan++`
Probe unkeyed headers (`X-Forwarded-Host`, etc.) that influence the response but aren't part of the cache key.

### Step 7 — CSRF
**Extensions:** `CSRF Token Tracker`, built-in "Generate CSRF PoC"
Check token presence, entropy, and server-side validation on every state-changing request.

### Step 8 — Authentication Weaknesses
**Extensions:** `Auth Analyzer`, `Turbo Intruder`, `Stepper`
Test rate limiting on login/OTP/password reset, session fixation, token predictability, and logout/invalidation.

### Step 9 — Cross-Site Scripting (Reflected / Stored)
**Extensions:** `Reflector`, `XSS Validator`, `Hackvertor`, `Active Scan++`
Mark injection points, generate encoded payload variants, confirm execution with XSS Validator's headless browser.

### Step 10 — DOM-Based Vulnerabilities
**Extensions:** `DOM Invader` (built-in), `AutoVader`
Enable DOM Invader sources/sinks in the browser; run AutoVader for automated DOM XSS and postMessage testing.

### Step 11 — Prototype Pollution
**Extensions:** `DOM Invader` (client-side mode), `Server-Side Prototype Pollution Scanner`
Test client-side merge/clone sinks and server-side object merging for `__proto__` injection.

### Step 12 — Path Traversal
**Extensions:** `Active Scan++`, `Burp Bounty`, `Hackvertor`, `Turbo Intruder`
Fuzz file-path parameters with encoded traversal sequences using SecLists wordlists through Turbo Intruder.

### Step 13 — Access Control / IDOR
**Extensions:** `Autorize`, `AuthMatrix`, `AutoRepeater`, `Auth Analyzer`
Replay low-privilege session tokens against every captured request automatically; diff responses for unauthorized access.

### Step 14 — Business Logic Vulnerabilities
**Extensions:** `Stepper`, `Logger++`
Manually reorder, skip, or repeat steps in multi-stage flows (checkout, password reset, coupon redemption).

### Step 15 — Race Conditions
**Extensions:** `Turbo Intruder`, Repeater "Send group in parallel" (built-in)
Fire single-packet parallel requests at limited-use resources (coupons, balances, one-time tokens).

### Step 16 — API Testing (REST)
**Extensions:** `Wsdler` (SOAP/WSDL), OpenAPI/Swagger import, `Param Miner`, `Autorize`
Enumerate undocumented versions/methods and test authorization on every operation.

### Step 17 — GraphQL API Vulnerabilities
**Extensions:** `InQL`, `GraphQL Raider`, `Param Miner`
Run introspection, test batching attacks, and check field-level authorization.

### Step 18 — JWT
**Extensions:** `JWT Editor`
Test `alg: none`, RS256→HS256 key confusion, `kid` header injection, and signature stripping.

### Step 19 — OAuth / SSO Authentication
**Extensions:** `OAUTHScan`, `EsPReSSO`, `SAML Raider`
Test redirect_uri manipulation, state/PKCE bypass, and SAML signature wrapping/stripping.

### Step 20 — WebSockets
**Extensions:** `WebSocket Turbo Intruder`, built-in WebSockets history
Test origin validation on the upgrade handshake, message tampering, and authorization on each message type.

### Step 21 — NoSQL Injection
**Extensions:** `NoSQLi Scanner`, `Backslash Powered Scanner`
Test operator injection (`$ne`, `$gt`, `$where`) and type-juggling in JSON-based parameters.

### Step 22 — SQL Injection
**Extensions:** `Backslash Powered Scanner`, `SQLiPy` / `CO2`, `Active Scan++`
Let Backslash Powered Scanner flag anomalous input handling, confirm manually, then hand off to sqlmap via SQLiPy for exploitation.

### Step 23 — Server-Side Template Injection (SSTI)
**Extensions:** `Backslash Powered Scanner`, `Burp Bounty`, `Active Scan++`
Send polyglot payloads (`${{<%[%'"}}%\`), then escalate with template-engine-specific RCE payloads once the engine is identified.

### Step 24 — OS Command Injection
**Extensions:** `Backslash Powered Scanner`, `Collaborator Everywhere` / `Taborator`
Inject shell metacharacters and confirm execution via out-of-band DNS/HTTP callback to Collaborator.

### Step 25 — SSRF
**Extensions:** `Collaborator Everywhere` / `Taborator`, `Param Miner`
Test every URL-accepting parameter and header (webhooks, avatar-from-URL, PDF/export features) against your Collaborator server, then pivot to cloud metadata endpoints if confirmed.

### Step 26 — XXE Injection
**Extensions:** `Content Type Converter`, `Collaborator Everywhere` / `Taborator`
Convert JSON requests to XML, inject external entities in SVG uploads, SOAP bodies, SAML responses, and Office file XML parts.

### Step 27 — Insecure Deserialization
**Extensions:** `Java Deserialization Scanner`, `Freddy`, `PHP Object Injection Check`, `ViewState Editor` (.NET)
Fingerprint the serialization format, then generate and send gadget-chain payloads (ysoserial integration where applicable).

### Step 28 — HTTP Request Smuggling
**Extensions:** `HTTP Request Smuggler`, `Turbo Intruder`
Run CL.TE / TE.CL / TE.TE desync probes against the front-end/back-end server pair.

### Step 29 — File Upload → RCE
**Extensions:** `Upload Scanner`, `Content Type Converter`
Test extension bypass, polyglot files, MIME-type confusion, and path traversal in the filename; chain with XXE for XML/SVG-based formats.

### Step 30 — Web LLM Attacks
**Extensions:** `Repeater`, `Intruder`, `Collaborator`
Test direct and indirect prompt injection, check if the LLM can be made to leak system prompts or call tools/APIs on an attacker's behalf, and use Collaborator to confirm data exfiltration via the model's outbound requests.

---

