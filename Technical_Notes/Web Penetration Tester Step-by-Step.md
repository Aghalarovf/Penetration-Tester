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
