 # 🔎 GAU — Çıxış Filterleri Cheatsheet

> `gau` ilə toplanan URL-lərdən maksimum dəyər çıxarmaq üçün filterlər.  
> Bütün filterlər `gau_all.txt` faylı üzərindən işləyir.

---

## 📥 Əvvəlcə topla

```bash
gau target.com | tee gau_all.txt
```

---

## 🧹 0. Deduplikasiya — İlk Olaraq Bunu Et

```bash
cat gau_all.txt | uro > gau_clean.txt
```

> `uro` eyni strukturlu URL-ləri tək saxlayır: `?id=1` və `?id=2`-dən yalnız birini götürür.  
> Əks halda minlərlə dublikat URL ilə vaxt itirirsən. Bütün aşağıdakı filterləri `gau_clean.txt` üzərindən işlət.

---

## 💉 1. Parametrli URL-lər (Injection Hədəfləri)

```bash
cat gau_clean.txt | grep "?" | grep "=" > params.txt
```

---

## 🎯 2. SQLi / XSS Hədəfi Olan Parametr Adları

```bash
cat gau_clean.txt | grep -iE "(\?|&)(id|uid|user|name|search|query|q|page|cat|item|product|order|ref|url|redirect|next|lang|debug)="
```

---

## 🔑 3. API Endpoint-ləri

```bash
cat gau_clean.txt | grep -iE "/api/|/v1/|/v2/|/v3/|/graphql|/rest/|/json|/rpc"
```

---

## 📁 4. Həssas Fayl Uzantıları

```bash
cat gau_clean.txt | grep -iE "\.(sql|db|bak|backup|old|zip|tar|gz|7z|rar|log|env|config|conf|cfg|ini|pem|key|p12|pfx|csv|xls|xlsx|xml|json|yaml|yml)$"
```

---

## 🚪 5. Admin / Giriş Panelləri

```bash
cat gau_clean.txt | grep -iE "(admin|panel|dashboard|login|signin|portal|manage|backend|cpanel|wp-admin|phpmyadmin)"
```

---

## 🔀 6. Open Redirect Hədəfləri

```bash
cat gau_clean.txt | grep -iE "(\?|&)(redirect|url|next|return|goto|dest|destination|forward|target|redir|location|link|ref|callback|continue|from|to)="
```

---

## 🌐 7. SSRF Hədəfləri

```bash
cat gau_clean.txt | grep -iE "(\?|&)(url|uri|src|source|path|fetch|load|host|endpoint|proxy|request|feed|file|image|img|page|data)="
```

---

## 📂 8. Path Traversal Hədəfləri

```bash
cat gau_clean.txt | grep -iE "(\?|&)(file|path|dir|folder|include|page|doc|document|template|view|load|read)="
```

---

## 🔐 9. Auth Token-lər / API Key-lər URL-də

```bash
cat gau_clean.txt | grep -iE "(token|api_key|apikey|access_token|auth|secret|key|password|passwd|pwd|session|sess)="
```

---

## ⚙️ 10. JS Faylları (Gizli Endpoint-lər üçün)

```bash
cat gau_clean.txt | grep "\.js$" | grep -v "min.js" | sort -u > js_files.txt
```

---

## 🔢 11. Versiyalanmış Endpoint-lər

```bash
cat gau_clean.txt | grep -iE "/v[0-9]+/" | sort -u
```

---

## 🚀 Master Pipeline — Hamısını Birlikdə İşlət

```bash
cat gau_all.txt | uro | grep "?" | grep "=" | \
grep -iE "(id|url|redirect|file|page|src|path|query|token|key)=" | \
sort -u > high_value_params.txt

wc -l high_value_params.txt
```

---

## 📊 Prioritet Sırası

| Prioritet | Parametr Növü | Potensial Zəiflik |
|:---------:|--------------|------------------|
| 🔴 1 | `token`, `auth`, `api_key`, `session` | Credential exposure |
| 🔴 2 | `redirect`, `url`, `next`, `goto` | Open Redirect / SSRF |
| 🟠 3 | `file`, `path`, `include`, `page` | LFI / Path Traversal |
| 🟠 4 | `id`, `uid`, `user`, `order` | SQLi / IDOR |
| 🟡 5 | `search`, `query`, `q`, `name` | XSS / SQLi |
| 🟡 6 | `src`, `image`, `feed`, `load` | SSRF |
| 🟢 7 | `.bak`, `.sql`, `.env`, `.log` | Sensitive file exposure |
| 🟢 8 | `/admin`, `/panel`, `/dashboard` | Auth bypass |

---

*GAU Filters Cheatsheet v1.0*
    → Wayback + CommonCrawl + OTX mənbələri birləşdirilir
