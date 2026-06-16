# Common Applications
```powershell

```
<img width="938" height="784" alt="image" src="https://github.com/user-attachments/assets/20045431-31e4-4494-92b3-58dae24f33db" />
<img width="935" height="577" alt="image" src="https://github.com/user-attachments/assets/ec56e8f0-bb5e-4496-b3cd-39dc9d74d5dd" />

# CMS Detection & Fingerprinting Cheat Sheet (Enumeration Only)

> Goal: identify with certainty which CMS (WordPress, Joomla, Drupal, Magento, etc.) a target website is running, using passive and active reconnaissance techniques. No exploitation included.

---

## 1. Automated CMS Fingerprinting Tools

These tools combine multiple detection techniques (headers, paths, hashes, meta tags) into a single scan.

```bash
# WhatWeb — broad tech fingerprinting (CMS, server, JS libs, etc.)
whatweb -v https://target.com

# Wappalyzer (CLI version)
wappalyzer https://target.com

# WPScan — WordPress-specific, but also confirms WordPress presence
wpscan --url https://target.com --enumerate

# CMSeeK — dedicated CMS detection across 170+ CMSs
python3 cmseek.py -u https://target.com

# BuiltWith CLI / Web UI
# https://builtwith.com/  (browser-based lookup)

# Nikto — flags CMS-related paths and known files during a general scan
nikto -h https://target.com
```

---

## 2. HTTP Response Header Analysis

```bash
curl -I https://target.com
curl -sI https://target.com | grep -i -E "x-powered-by|server|set-cookie|x-generator|x-drupal|x-pingback"
```

What to look for:
- `X-Powered-By` — sometimes reveals PHP version or framework
- `X-Generator` — common in Drupal (`Drupal 9`), some Joomla setups
- `X-Pingback` — almost always indicates **WordPress** (`xmlrpc.php` endpoint)
- `Set-Cookie` naming — `wp-settings-*` (WordPress), `JSESSIONID` (Java-based), `PHPSESSID` (generic PHP), `CMSSESSID*` (concrete CMS variants)
- `Server` header — Apache/Nginx/LiteSpeed version (not CMS-specific, but useful context)

---

## 3. HTML Source Code Inspection

```bash
curl -s https://target.com | grep -i -E "wp-content|wp-includes|/sites/default|/sites/all|joomla|media/system|skin/frontend|components/com_|Drupal.settings"
```

Key signatures inside HTML:
| CMS | Signature in source |
|---|---|
| WordPress | `/wp-content/`, `/wp-includes/`, `wp-json` REST link, `generator` meta = `WordPress X.X` |
| Joomla | `/media/system/`, `/components/com_*`, `Joomla! - Open Source Content Management` in generator meta |
| Drupal | `/sites/default/`, `/sites/all/`, `Drupal.settings`, `X-Generator: Drupal` |
| Magento | `/skin/frontend/`, `/static/version*/frontend/`, `Mage.Cookies`, `var/cookie` references |
| Shopify | `cdn.shopify.com`, `Shopify.theme` JS object |
| Wix | `static.wixstatic.com`, `wix-warmup-data` |
| Squarespace | `static1.squarespace.com`, `Squarespace.afterBodyLoad` |
| TYPO3 | `typo3conf/`, `typo3temp/` |
| PrestaShop | `/themes/`, `prestashop` JS variables |
| OpenCart | `index.php?route=`, `catalog/view/theme` |

**Generator meta tag (quick win when present):**
```bash
curl -s https://target.com | grep -i '<meta name="generator"'
```

---

## 4. robots.txt and sitemap.xml Analysis

```bash
curl -s https://target.com/robots.txt
curl -s https://target.com/sitemap.xml
```

What to look for:
- WordPress: `Disallow: /wp-admin/`, `Sitemap: .../wp-sitemap.xml`
- Joomla: `Disallow: /administrator/`, `Disallow: /components/`
- Drupal: `Disallow: /core/`, `Disallow: /node/`, `Disallow: /modules/`
- Magento: `Disallow: /catalog/`, `Disallow: /checkout/`

---

## 5. Well-Known Paths & File Probing

Probe for CMS-specific files/directories that typically remain accessible (404 vs 200 status comparison):

```bash
for path in wp-login.php wp-admin wp-json/ administrator/ /components/ /sites/default/settings.php /core/CHANGELOG.txt /xmlrpc.php /skin/frontend /app/etc/local.xml /typo3conf/ /user/login; do
  code=$(curl -s -o /dev/null -w "%{http_code}" "https://target.com/$path")
  echo "$path -> $code"
done
```

| Path | CMS |
|---|---|
| `/wp-login.php`, `/wp-admin/`, `/wp-json/` | WordPress |
| `/administrator/`, `/components/`, `/modules/mod_*` | Joomla |
| `/user/login`, `/core/CHANGELOG.txt`, `/node/1` | Drupal |
| `/app/etc/local.xml`, `/downloader/`, `/skin/frontend/` | Magento |
| `/typo3conf/`, `/typo3temp/` | TYPO3 |
| `/admin/login.php`, `/index.php?route=common/home` | OpenCart |

---

## 6. CHANGELOG / README / Version Disclosure Files

Many CMSs ship changelog or readme files that disclose exact version numbers:

```bash
curl -s https://target.com/readme.html          # WordPress core
curl -s https://target.com/CHANGELOG.txt         # Drupal
curl -s https://target.com/core/CHANGELOG.txt    # Drupal 8/9/10
curl -s https://target.com/language/en-GB/en-GB.xml   # Joomla version
curl -s https://target.com/administrator/manifests/files/joomla.xml
```

---

## 7. JavaScript & CSS Asset Path Analysis

```bash
curl -s https://target.com | grep -oE '(src|href)="[^"]+\.(js|css)"'
```

Look at asset URL structure: `wp-content/themes/`, `wp-content/plugins/` (WordPress), `sites/default/files/` (Drupal), `media/jui/`, `templates/` (Joomla), `static/version*/frontend/` (Magento 2), `cdn.shopify.com/s/files/` (Shopify).

---

## 8. Cookie Naming Conventions

```bash
curl -sI https://target.com | grep -i set-cookie
```

| Cookie pattern | CMS |
|---|---|
| `wordpress_*`, `wp-settings-*` | WordPress |
| `joomla_user_state`, `*_csrf_token` (Joomla style) | Joomla |
| `SESS*` (hashed), `Drupal.visitor.*` | Drupal |
| `frontend`, `PHPSESSID` + `X-Magento-*` headers | Magento |
| `_shopify_*`, `cart_currency` | Shopify |

---

## 9. Favicon Hashing (Shodan/Censys-style fingerprinting)

Default CMS installs often keep their default favicon, which has a stable hash usable for fingerprinting at scale:

```bash
curl -s https://target.com/favicon.ico | md5sum
# Compare hash against known CMS default favicon hash databases (e.g., via Shodan's http.favicon.hash search)
```

---

## 10. DNS, WHOIS, and Passive Recon

```bash
whois target.com
dig target.com ANY
dig TXT target.com          # SPF/DKIM records sometimes reveal hosted CMS platform (e.g., Shopify, Wix)
```

Passive sources:
- `https://builtwith.com/target.com`
- `https://www.wappalyzer.com/lookup/target.com/`
- Wayback Machine (`web.archive.org`) — check historic snapshots for old CMS signatures that may still be present in cached assets

---

## 11. CMS-Specific API / Endpoint Probing

```bash
# WordPress REST API
curl -s https://target.com/wp-json/

# Drupal JSON:API
curl -s https://target.com/jsonapi

# Joomla API (if com_api or Joomla 4+ webservices enabled)
curl -s https://target.com/api/index.php/v1/users

# Ghost CMS
curl -s https://target.com/ghost/api/v3/content/

# Magento GraphQL
curl -s https://target.com/graphql -X POST -H "Content-Type: application/json" -d '{"query":"{ storeConfig { code } }"}'
```

A valid structured JSON response from any of these strongly confirms the underlying CMS.

---

## 12. Server-Side Technology Cross-Referencing

CMS choice often correlates with backend stack — useful for narrowing down candidates before deeper probing:

```bash
curl -sI https://target.com | grep -i "x-powered-by\|server"
```

- PHP-based: WordPress, Joomla, Drupal (often), Magento, OpenCart, PrestaShop, MODX
- Ruby-based: Spree, RefineryCMS
- Node.js-based: Ghost, Strapi, KeystoneJS
- .NET-based: Umbraco, Sitefinity, DNN (DotNetNuke)
- Java-based: Magnolia, Liferay, AEM (Adobe Experience Manager)

---

## 13. Manual Visual/Behavioral Indicators

- Login page URL pattern and styling (`/wp-login.php` login form vs. Joomla's `/administrator` Bootstrap-styled login)
- Default error pages (404/403) sometimes retain CMS branding or unique templates
- URL slug structure: `?p=123` (WordPress legacy), `/node/123` (Drupal), `index.php?option=com_content&view=article&id=123` (Joomla)
- Admin panel URL conventions: `/wp-admin`, `/administrator`, `/user/login`, `/admin`

---

## 14. Consolidated Decision Checklist

1. Run `whatweb` / `cmseek` for an automated first pass
2. Check HTTP headers for `X-Generator`, `X-Pingback`, `X-Drupal-*`
3. Inspect HTML source for generator meta tag and asset path patterns (`wp-content`, `sites/default`, `components/com_*`)
4. Pull `robots.txt` and `sitemap.xml` for CMS-specific disallow rules
5. Probe well-known CMS paths and compare HTTP status codes
6. Pull changelog/readme files for exact version disclosure
7. Inspect cookie names set by the server
8. Hash the favicon and compare against known CMS defaults
9. Hit CMS-specific API endpoints (`wp-json`, `jsonapi`, `graphql`, `ghost/api`) to get a definitive JSON-based confirmation
10. Cross-reference backend language/server stack to narrow down candidate CMSs if signatures are ambiguous or stripped


