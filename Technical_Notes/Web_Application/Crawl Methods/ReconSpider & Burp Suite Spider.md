

# ReconSpider

### Quraşdırma

```bash
git clone https://github.com/bhavsec/reconspider.git
cd reconspider
pip3 install -r requirements.txt
python3 reconspider.py
```

### İşə Salma

```bash
python3 reconspider.py
```

### Əsas Əmrlər (Menyu)

| Seçim | Funksiya                          |
|-------|-----------------------------------|
| `1`   | IP Ünvan Araşdırması              |
| `2`   | Domain Araşdırması                |
| `3`   | Email Araşdırması                 |
| `4`   | Telefon Nömrəsi Araşdırması       |
| `5`   | URL / Sayt Araşdırması            |
| `6`   | DNS Araşdırması                   |
| `7`   | WHOIS Sorğusu                     |
| `8`   | Shodan Sorğusu (API tələb olunur) |
| `9`   | VirusTotal Yoxlaması              |
| `0`   | Çıxış                             |

### IP Araşdırması

```
Seçim: 1
Hədəf IP: 192.168.1.1
```

Qaytarılan məlumatlar:
- Ölkə / Region / Şəhər
- ISP / Təşkilat
- ASN
- Koordinatlar

### Domain Araşdırması

```
Seçim: 2
Hədəf Domain: example.com
```

Qaytarılan məlumatlar:
- DNS qeydləri (A, MX, NS, TXT)
- IP ünvanları
- Subdomain siyahısı
- WHOIS məlumatı

### Email Araşdırması

```
Seçim: 3
Hədəf Email: user@example.com
```

Qaytarılan məlumatlar:
- Email mövcudluğu
- Breach yoxlaması (HaveIBeenPwned)
- Email sahibinin məlumatı

### Shodan İnteqrasiyası

```bash
# API key konfiqurasiyası
nano reconspider/config.py
SHODAN_API_KEY = "your_api_key_here"
```

Sorğu nümunəsi:
```
Seçim: 8
Hədəf IP/Domain: 8.8.8.8
```

### DNS Araşdırması

```
Seçim: 6
Hədəf Domain: example.com
```

Qaytarılan qeyd növləri:
- `A` — IPv4 ünvanı
- `AAAA` — IPv6 ünvanı
- `MX` — Mail serveri
- `NS` — Ad serveri
- `TXT` — SPF, DKIM və s.
- `CNAME` — Alias qeydi

### Faydalı İpuçları

```bash
# Nəticəni fayla yaz
python3 reconspider.py | tee output.txt

# Verbose rejim (əgər dəstəklənir)
python3 reconspider.py -v

# Bütün modul nəticəsini JSON kimi saxla
python3 reconspider.py > results.json
```

---

# Burp Suite Spider (Crawler)

Burp Suite Spider — veb tətbiqinin strukturunu xəritələmək üçün istifadə olunan avtomatik crawler-dır.

### Aktivləşdirmə (Burp Suite Pro / Community)

```
Target → Site Map → Sağ klik → Spider this host
           və ya
Spider tab → Spider is running [ON]
```

### Spider Konfiqurasiyası

```
Spider → Options
```

| Parametr               | Tövsiyə Olunan Dəyər         | Açıqlama                          |
|------------------------|------------------------------|-----------------------------------|
| Max depth              | 5–10                         | Səhifə dərinliyi limiti           |
| Max requests           | 3000–10000                   | Maksimum sorğu sayı               |
| Max unique requests    | 1000                         | Unikal URL limiti                 |
| Request throttling     | 50–100 ms                    | Serveri aşırı yükləməmək üçün    |
| Follow redirects       | ✅ ON                        | Yönləndirmələri izlə              |
| Process forms          | ✅ ON                        | Formaları doldur və göndər        |
| Submit login forms     | ⚠️ Ehtiyatla                | Autentifikasiya lazımdırsa açın   |

### Scope Tənzimlənməsi (Vacibdir!)

```
Target → Scope → Include in scope
```

```
Protocol: https
Host/IP: .*\.example\.com
Port: ^443$
File: .*
```

> ⚠️ **XƏBƏRDARLIQ:** Spider-i işə salmadan əvvəl mütləq Scope təyin edin!
> Əks halda kənar saytlar da taranacaq.

### Spider-i İşə Salma

```
1. Proxy → Intercept → Intercept is off
2. Brauzerinizdə hədəf saytı açın
3. Target → Site Map → Hədəfi sağ klik edin
4. "Spider this host" seçin
5. Spider → Toggle Spider Status
```

### Form İşləmə

```
Spider → Options → Form Submission
```

| Parametr              | Dəyər                   |
|-----------------------|-------------------------|
| Don't submit forms    | ❌ (submit etmək üçün)  |
| Prompt for guidance   | ✅ (manual doldurma)    |
| Automatically submit  | ⚠️ (test mühitində)    |

### Login Forması Üçün

```
Spider → Options → Application Login
├── Handle application login: ✅
├── Username: testuser
└── Password: testpass123
```

### Active Scan ilə Birlikdə İstifadə

```
Site Map → Hədəfi seç → Sağ klik
→ "Actively scan this host"
```

İş axını:
```
Spider (xəritəlmə) → Active Scan (zəiflik axtarışı) → Reporting
```

### Site Map Analizi

```
Target → Site Map
```

Filtrlər:
```
Filter: Show only in-scope items
Filter: Show only requested items  
Filter: Hide not-found items (404)
```

Məzmun növləri:
- 🟢 Tapılmış səhifələr
- 🔵 Parametrli URL-lər
- 🟡 Formalar
- 🔴 Xətalı cavablar

### Faydalı Klaviatura Qısayolları

| Qısayol     | Funksiya                       |
|-------------|-------------------------------|
| `Ctrl+I`    | Intercept ON/OFF               |
| `Ctrl+F`    | Axtarış                        |
| `Ctrl+A`    | Hamısını seç                   |
| `Ctrl+R`    | Repeater-ə göndər              |
| `Ctrl+S`    | Scanner-ə göndər               |

### Spider Nəticələrinin İxracı

```
Target → Site Map → Sağ klik → "Save selected items"
           → Formatlar: XML, JSON, CSV
```

---

## ⚖️ Müqayisə: ReconSpider vs Burp Suite Spider

| Xüsusiyyət         | ReconSpider              | Burp Suite Spider           |
|--------------------|--------------------------|------------------------------|
| İstifadə sahəsi    | Kənar OSINT              | Daxili veb tətbiq            |
| Protokol           | OSINT API-ları           | HTTP/HTTPS                   |
| GUI                | ❌ Terminal              | ✅ Qrafik interfeys          |
| Aktiv scan         | ❌                       | ✅                           |
| Ödənişli versiya   | ❌ Pulsuz                | Pro versiya (ödənişli)       |
| Autentifikasiya    | API key (Shodan, VT)     | Cookie / Login form          |
| Çıxış formatı      | Terminal / txt           | XML, JSON, HTML Report       |

---

## 🛡️ Etik İstifadə Qaydaları

> - Yalnız **icazə verilmiş** sistemlərdə istifadə edin
> - **Bug Bounty** proqramlarının qaydalarına riayət edin  
> - Production mühitini **test mühiti** ilə qarışdırmayın
> - Bütün fəaliyyətlər üçün **yazılı icazə** alın
> - **Məlumatları məxfi** saxlayın

---

*Hazırlandı: ReconSpider v1.0+ | Burp Suite v2024+*
