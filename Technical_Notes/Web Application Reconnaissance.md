# 🔎 Web Enumeration — 100 Addımlı Dərin Bələdçi

> **Fokus:** Hədəf web səhifə üzərində tam kəşfiyyat — passiv və aktiv texnikalar  
> **Struktur:** İlk 60 addım **Passiv**, son 40 addım **Aktiv**  
> **Qeyd:** Yalnız yazılı icazə alınmış hədəflərə tətbiq et.

---

## 🕵️ HİSSƏ I — PASİV ENUMERATİON (Addım 1–60)
> Hədəf sistemə birbaşa toxunmadan məlumat toplama. Bütün sorğular 3-cü tərəf xidmətlər vasitəsilə gedir.

---

### 📌 BLOK 1 — Domain və WHOIS Kəşfi (1–8)

1. **WHOIS sorğusu** — domain qeydiyyat məlumatlarını topla  
   ```bash
   whois target.com
   whois -h whois.arin.net target.com
   ```
   → Registrar, qeydiyyat tarixi, nameserver-lər, admin email

2. **WHOIS tarixi** — köhnə qeydiyyat məlumatlarını araşdır  
   → `https://whoishistory.com` | `https://domaintools.com`  
   → Əvvəlki sahiblər, köhnə email-lər, IP dəyişiklikləri

3. **Reverse WHOIS** — eyni sahibin digər domainlərini tap  
   → `https://viewdns.info/reversewhois/`  
   → Eyni şəxsə/şirkətə aid bütün domain-lər

4. **ASN (Autonomous System Number) təyini**  
   ```bash
   whois -h whois.radb.net target.com
   curl https://api.bgpview.io/search?query_term=target.com
   ```
   → Şirkətin sahib olduğu bütün IP blokları

5. **IP-dən BGP məlumatı**  
   → `https://bgp.he.net` → ASN, prefix, peering məlumatları  
   → Şirkətin bütün şəbəkə infrastrukturu xəritəsi


7. **Domain expiry yoxlama**  
   → Bitmək üzrə olan domain-lər = subdomain takeover riski  
   → `https://expireddomains.net`

8. **TLD variations** — eyni ad altında başqa TLD-ləri yoxla  
   → `target.net`, `target.org`, `target.io`, `target.co`  
   → Eyni korporasiyaya aid parallel infrastruktur

---

### 📌 BLOK 2 — DNS Dərin Analiz (9–17)

9. **Bütün DNS qeyd növlərini sorğula**  
   ```bash
   dig target.com ANY
   dig target.com A
   dig target.com AAAA
   dig target.com MX
   dig target.com NS
   dig target.com TXT
   dig target.com SOA
   dig target.com CAA
   ```

14. **DNS SOA qeydi analizi**  
    ```bash
    dig target.com SOA
    ```
    → Primary nameserver, admin email (@ → .), serial number (deployment tarixi)

17. **DNS-over-HTTPS (DoH) ilə bypass test**  
    ```bash
    curl "https://cloudflare-dns.com/dns-query?name=target.com&type=A" -H "accept: application/dns-json"
    ```
    → Bəzi DNS qeydlərini CDN/Firewall gizlədir, DoH ilə real IP görünə bilər

---

### 📌 BLOK 3 — SSL/TLS Sertifikat Kəşfi (18–24)

18. **crt.sh ilə sertifikat şəffaflıq loqları**  
    ```bash
    curl "https://crt.sh/?q=%.target.com&output=json" | jq '.[].name_value' | sort -u
    ```
    → Heç vaxt indekslənməmiş subdomainlər burada görünə bilər

19. **Censys.io ilə sertifikat axtarışı**  
    → `https://search.censys.io/certificates?q=parsed.names%3Atarget.com`  
    → Sertifikat sahibi, issuer, SAN sahələri

20. **sslscan ilə TLS analiz** *(passiv — public scan DB-dən)*  
    → `https://www.ssllabs.com/ssltest/analyze.html?d=target.com`  
    → TLS versiyası, cipher suite-lər, zəifliklər (POODLE, BEAST, Heartbleed)

21. **SAN (Subject Alternative Names) analizi**  
    → Bir sertifikat birdən çox domain əhatə edə bilər  
    → Qardaş domenləri (sibling domains) tapılır  
    ```bash
    openssl s_client -connect target.com:443 </dev/null 2>/dev/null | openssl x509 -noout -text | grep DNS:
    ```

23. **Köhnə sertifikatları araşdır**  
    → `https://certspotter.com/api/v1/issuances?domain=target.com&include_subdomains=true`  
    → Köhnə sertifikatlarda artıq istifadə edilməyən subdomain-lər

---

### 📌 BLOK 4 — OSINT: Axtarış Motorları və Dorklar (25–33)

25. **Google Dorking — əsas**  
    ```
    site:target.com
    site:target.com filetype:pdf
    site:target.com filetype:xls OR filetype:xlsx
    site:target.com filetype:sql OR filetype:db
    site:target.com inurl:admin
    site:target.com inurl:login
    site:target.com inurl:config
    site:target.com intitle:"index of"
    ```

26. **Google Dorking — həssas fayllar**  
    ```
    site:target.com ext:log
    site:target.com ext:bak OR ext:old OR ext:backup
    site:target.com ext:env
    site:target.com ext:xml inurl:config
    site:target.com "api_key" OR "apikey" OR "api key"
    site:target.com "password" filetype:txt
    ```

27. **Google Dorking — xəta mesajları**  
    ```
    site:target.com "SQL syntax"
    site:target.com "Warning: mysql"
    site:target.com "Fatal error"
    site:target.com "stack trace"
    site:target.com "500 Internal Server Error"
    ```

28. **Bing Dorking**  
    ```
    site:target.com
    ip:X.X.X.X          ← eyni IP-dəki digər saytlar
    ```

29. **DuckDuckGo — caching**  
    → `!g cache:target.com` — Google cache  
    → Sayt offline olsa belə köhnə versiyaya bax

30. **Shodan.io — passiv infrastruktur kəşfi**  
    ```
    hostname:target.com
    org:"Target Company"
    ssl.cert.subject.cn:target.com
    http.title:"Target"
    ```
    → Açıq portlar, servis versiyaları, default səhifələr, CVE-lər

31. **Shodan API ilə avtomatik sorğu**  
    ```bash
    shodan host target-ip
    shodan search "hostname:target.com" --fields ip_str,port,org,hostnames
    ```

32. **Censys.io — host kəşfi**  
    ```
    services.tls.certificates.leaf_data.names: target.com
    dns.reverse_dns.reverse_dns: target.com
    ```

---

### 📌 BLOK 5 — Arxiv və Tarix Analizi (34–40)

34. **Wayback Machine — arxiv URL-ləri**  
    ```bash
    waybackurls target.com | tee wayback_urls.txt
    ```
    → Silinmiş səhifələr, köhnə endpointlər, köhnə parametrlər

35. **gau (GetAllUrls) — multi-source URL toplama**  
   [GAU ilə maksimum verimlilik](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Web_Application/Enumeration/GAU.md)

36. **CommonCrawl indeks sorğusu**  
    ```bash
    curl "http://index.commoncrawl.org/CC-MAIN-2024-10-index?url=*.target.com&output=json" | jq '.url'
    ```

37. **Arxiv URL-lərindən parametr çıxarma**  
    ```bash
    cat wayback_urls.txt gau_urls.txt | sort -u | grep "?" | \
    grep "=" | sed 's/=.*/=/' | sort -u > unique_params.txt
    ```
    → Köhnə, artıq istifadə edilməyən parametrlər = unpatched zəifliklər

38. **JS fayllarının tarixçəsi**  
    ```bash
    waybackurls target.com | grep "\.js$" | sort -u
    ```
    → Köhnə JS versiyaları köhnə API endpoint-ləri ehtiva edə bilər

39. **Robots.txt tarixçəsi**  
    → `https://web.archive.org/web/*/target.com/robots.txt`  
    → Əvvəl gizlənmiş, indi silinmiş direktoriləri tap

40. **Sitemap.xml arxiv analizi**  
    → `https://web.archive.org/web/*/target.com/sitemap.xml`  
    → Köhnə URL strukturu, silinmiş səhifələr

---

### 📌 BLOK 6 — Kod Depozitləri və Məlumat Sızması (41–48)

41. **GitHub — şirkət adı ilə axtarış**  
    ```
    org:targetcompany
    "target.com" password
    "target.com" secret
    "target.com" api_key
    "target.com" private_key
    ```

42. **GitHub Dorking — credentials**  
    ```
    "target.com" "BEGIN RSA PRIVATE KEY"
    "target.com" "db_password"
    "target.com" "AWS_SECRET"
    filename:.env "target.com"
    filename:config.php "target.com"
    ```

43. **truffleHog ilə git tarixçəsi skani**  
    ```bash
    trufflehog git https://github.com/targetorg/repo --json
    ```

44. **gitleaks ilə secret aşkarlama**  
    ```bash
    gitleaks detect --source ./cloned-repo --report-format json
    ```

45. **GitLab, Bitbucket axtarışı**  
    → `https://gitlab.com/search?search=target.com`  
    → `https://bitbucket.org/repo/all?name=target`

46. **Pastebin və snippet saytları**  
    ```bash
    pwnbin -k "target.com"
    ```
    → `https://pastebin.com/search?q=target.com`  
    → `https://gist.github.com/search?q=target.com`

47. **npm / PyPI / Maven paketlərini yoxla**  
    ```bash
    npm search target
    pip search target
    ```
    → Açıq mənbəli paketlərə qatılmış internal kod

48. **Docker Hub-da şirkət image-ləri**  
    → `https://hub.docker.com/search?q=target`  
    → Hardcoded credentials, internal IP-lər, gizli endpoint-lər

---

### 📌 BLOK 7 — Texnologiya Müəyyənləşdirməsi (49–54)

49. **BuiltWith — tam texnologiya profili**  
    → `https://builtwith.com/target.com`  
    → CMS, analytics, CDN, email servis, payment gateway, hosting

50. **Wappalyzer CLI**  
    ```bash
    wappalyzer https://target.com --pretty
    ```
    → Framework, JS kitabxana versiyaları, server stack

51. **retire.js — köhnəlmiş JS kitabxanaları**  
    ```bash
    retire --js --path ./downloaded-js/
    ```
    → Bilinen CVE-ləri olan köhnə jQuery, Bootstrap, Angular versiyaları

52. **WhatWeb — passiv fingerprinting**  
    ```bash
    whatweb -a 1 target.com    # aggressive level 1 (passiv)
    ```

53. **HTTP Security Headers analizi**  
    → `https://securityheaders.com/?q=target.com`  
    → CSP, HSTS, X-Frame-Options, Referrer-Policy

54. **Mozilla Observatory**  
    → `https://observatory.mozilla.org/analyze/target.com`  
    → TLS, header, redirect konfigurasiyanın qiymətləndirilməsi

---

### 📌 BLOK 8 — Sosial Mühəndislik OSINT (55–60)

55. **LinkedIn — işçi profili**  
    → Texniki işçilərin rolları → istifadə olunan texnologiyalar  
    → İş elanları → "Django developer axtarırıq" = backend texnologiyası

56. **theHarvester — email və subdomain**  
    ```bash
    theHarvester -d target.com -b google,linkedin,bing,censys,dnsdumpster
    ```

57. **Hunter.io — email format**  
    → `https://hunter.io/search/target.com`  
    → `{first}.{last}@target.com` formatı → admin hesabları üçün əsas

58. **Gravatar / Avatar servisləri**  
    → Tapılan email-ləri MD5 hash edib `gravatar.com/avatar/{hash}` yoxla  
    → Hesabın mövcudluğunu təsdiqləyir

59. **Breached data yoxlama**  
    → `https://haveibeenpwned.com/api/v3/breachedaccount/{email}`  
    → Sızmış şifrə bazalarında hədəf şirkət hesabları varmı?

60. **Dehashed / IntelX — credential bazaları**  
    → `https://dehashed.com/search?query=target.com`  
    → Credential stuffing üçün potensial material (yalnız qanuni testdə istifadə)

---
---

## ⚡ HİSSƏ II — AKTİV ENUMERATİON (Addım 61–100)
> Hədəf sistemə birbaşa sorğu göndərməni tələb edən texnikalar. **Mütləq yazılı icazə tələb olunur.**

---

### 📌 BLOK 9 — Port Skan və Servis Kəşfi (61–67)

61. **Nmap — əsas TCP skanı**  
    ```bash
    nmap -sV -sC -T4 -p- target.com -oA nmap_full
    ```
    → `-sV` servis versiyası, `-sC` default scriptlər, `-p-` bütün portlar

62. **Nmap — UDP skanı**  
    ```bash
    nmap -sU -T4 --top-ports 200 target.com -oA nmap_udp
    ```
    → DNS (53), SNMP (161), TFTP (69), NTP (123)

63. **Masscan — ultra sürətli port skanı**  
    ```bash
    masscan -p1-65535 target.com --rate=5000 -oJ masscan.json
    ```

64. **Nmap NSE scriptləri — web xüsusi**  
    ```bash
    nmap --script http-enum,http-headers,http-methods,http-title target.com -p80,443
    nmap --script ssl-enum-ciphers target.com -p443
    nmap --script http-auth-finder target.com
    ```

65. **testssl.sh — TLS aktiv analiz**  
    ```bash
    testssl.sh --full target.com
    ```
    → Heartbleed, POODLE, BEAST, ROBOT, zəif cipher-lər, sertifikat

66. **Nikto — web server ilkin skan**  
    ```bash
    nikto -h https://target.com -output nikto_report.txt -Format txt
    ```
    → Default fayllar, server misconfigurasiya, köhnə versiyalar

67. **HTTPx — canlı web host yoxlama**  
    ```bash
    cat subdomains.txt | httpx -status-code -title -tech-detect -content-length -json > httpx_results.json
    ```

---

### 📌 BLOK 10 — Subdomain Aktiv Enumeration (68–73)

68. **Amass aktiv skan**  
    ```bash
    amass enum -active -d target.com -o amass_active.txt
    ```

69. **dnsx — DNS brute-force**  
    ```bash
    dnsx -d target.com -w /opt/SecLists/Discovery/DNS/subdomains-top1million-5000.txt -o dnsx_results.txt
    ```

70. **PureDNS — yüksək performanslı subdomain brute-force**  
    ```bash
    puredns brute /opt/SecLists/Discovery/DNS/subdomains-top1million-110000.txt target.com -r resolvers.txt
    ```

71. **Gobuster DNS**  
    ```bash
    gobuster dns -d target.com -w /opt/SecLists/Discovery/DNS/bitquark-subdomains-top100000.txt -t 50
    ```

72. **Virtual Host (VHost) skanı**  
    ```bash
    gobuster vhost -u https://target.com -w /opt/SecLists/Discovery/DNS/subdomains-top1million-5000.txt --append-domain
    ffuf -w subdomains.txt -u https://target.com -H "Host: FUZZ.target.com" -fs 4242
    ```

73. **Subdomain Takeover yoxlama**  
    ```bash
    subjack -w subdomains.txt -t 100 -timeout 30 -ssl -o takeover_results.txt
    nuclei -l subdomains.txt -t ~/nuclei-templates/http/takeovers/
    ```

---

### 📌 BLOK 11 — Kataloq və Fayl Fuzzing (74–80)

74. **FFuf — sürətli directory fuzzing**  
    ```bash
    ffuf -u https://target.com/FUZZ -w /opt/SecLists/Discovery/Web-Content/raft-large-directories.txt \
         -mc 200,201,301,302,401,403 -t 50 -o ffuf_dirs.json -of json
    ```

75. **Gobuster dir — kataloq skanı**  
    ```bash
    gobuster dir -u https://target.com -w /opt/SecLists/Discovery/Web-Content/common.txt \
                 -x php,html,js,txt,bak,old,zip -t 30 -o gobuster_results.txt
    ```

76. **Dirsearch — əhatəli fayl skanı**  
    ```bash
    dirsearch -u https://target.com -e php,asp,aspx,jsp,html,js,json,txt,bak,sql \
              -w /opt/SecLists/Discovery/Web-Content/directory-list-2.3-big.txt \
              --output=dirsearch_report.txt
    ```

77. **Feroxbuster — rekursiv fuzzing**  
    ```bash
    feroxbuster -u https://target.com -w /opt/SecLists/Discovery/Web-Content/raft-medium-words.txt \
                --depth 3 --auto-tune -o feroxbuster.txt
    ```

78. **Gizli fayl axtarışı**  
    ```bash
    ffuf -u https://target.com/FUZZ -w /opt/SecLists/Discovery/Web-Content/raft-large-files.txt \
         -mc 200 -t 40
    # Xüsusi hədəflər:
    # /.git/HEAD
    # /.env
    # /config.php.bak
    # /wp-config.php
    # /web.config
    # /.htaccess
    ```

79. **API endpoint fuzzing**  
    ```bash
    ffuf -u https://target.com/api/FUZZ -w /opt/SecLists/Discovery/Web-Content/api/actions.txt \
         -mc 200,201,400,401,403 -t 30
    ffuf -u https://target.com/api/v1/FUZZ -w /opt/SecLists/Discovery/Web-Content/api/objects.txt
    ```

80. **Parametr fuzzing (arjun)**  
    ```bash
    arjun -u https://target.com/search -m GET
    arjun -u https://target.com/api/user -m POST
    ```
    → Gizli GET/POST parametrlərini kəşf edir

---

### 📌 BLOK 12 — JavaScript Analizi (81–86)

81. **LinkFinder — JS-dən endpoint çıxarma**  
    ```bash
    python3 linkfinder.py -i https://target.com -d -o results.html
    ```

82. **JSParser — URL scraping**  
    ```bash
    python3 jsparser.py https://target.com
    ```

83. **getJS — JS fayllarını topla**  
    ```bash
    getJS --url https://target.com --complete --output js_files.txt
    ```

84. **Nuclei — JS secret skanı**  
    ```bash
    nuclei -l js_files.txt -t ~/nuclei-templates/exposures/tokens/ -o js_secrets.txt
    ```

85. **JS fayllarında hardcoded credentials manuallı axtarış**  
    ```bash
    cat js_files.txt | while read url; do
      curl -s "$url" | grep -iE "(api_key|apikey|secret|password|token|auth)" 
    done
    ```

86. **Source map fayllarını tap**  
    ```bash
    ffuf -u https://target.com/static/FUZZ.js.map -w js_names.txt -mc 200
    ```
    → `.map` faylları orijinal minify edilməmiş kodu ehtiva edir

---

### 📌 BLOK 13 — CMS və Framework Spesifik Skan (87–91)

87. **WPScan — WordPress**  
    ```bash
    wpscan --url https://target.com --enumerate u,p,t,tt --plugins-detection aggressive \
           --api-token YOUR_TOKEN -o wpscan_report.json --format json
    ```

88. **Droopescan — Drupal**  
    ```bash
    droopescan scan drupal -u https://target.com -t 32
    ```

89. **CMSeek — universal CMS kəşfi**  
    ```bash
    python3 cmseek.py -u https://target.com --follow-redirect
    ```

90. **Nuclei — texnologiya spesifik CVE skanı**  
    ```bash
    nuclei -u https://target.com -t ~/nuclei-templates/ \
           -tags cve,exposure,misconfig,default-login \
           -severity critical,high,medium \
           -o nuclei_results.json -jsonl
    ```

91. **Metabigor — şirkət infrastruktur kəşfi**  
    ```bash
    echo "target.com" | metabigor web --json
    ```

---

### 📌 BLOK 14 — Burp Suite ilə Dərin Analiz (92–97)

92. **Burp Suite — passiv crawl**  
    → Brauzerdə hədəf saytı gəz, Burp Proxy hər sorğunu qeyd edir  
    → Site Map-i tam doldurmaq üçün bütün funksiyaları istifadə et

93. **Burp Scanner — aktiv tarama**  
    ```
    → Burp Suite Pro → Dashboard → New Scan
    → Crawl and Audit seçimi
    → Scan configuration: thorough
    ```

94. **Burp Intruder — parametr kəşfi**  
    → Sorğunu Intruder-ə göndər  
    → Payload position-larını işarələ  
    → Sniper rejimində header/param fuzzing et

95. **Burp Comparer — response müqayisəsi**  
    → İki fərqli response-u müqayisə et  
    → Auth/unauth fərqini analiz et

96. **Burp Decoder/Encoder**  
    → Base64, URL encoding, HTML encoding analizləri  
    → Token strukturunu anlamaq üçün

97. **Burp Collaborator — out-of-band kəşf**  
    ```
    → Burp Collaborator server aç
    → URL-i SSRF, blind XSS, XXE payload-larında istifadə et
    → DNS/HTTP sorğularını izlə
    ```

---

### 📌 BLOK 15 — Yekun Analiz və Xəritələmə (98–100)

98. **Bütün tapıntıları birləşdir — attack surface xəritəsi**  
    ```bash
    cat amass_active.txt dnsx_results.txt | sort -u > all_subdomains.txt
    cat ffuf_dirs.json gobuster_results.txt | anew > all_endpoints.txt
    cat wayback_urls.txt gau_urls.txt | sort -u | anew >> all_endpoints.txt
    ```

99. **EyeWitness — vizual screenshot toplama**  
    ```bash
    eyewitness --web -f all_subdomains.txt --timeout 30 --threads 10 \
               --prepend-https -d eyewitness_report
    ```
    → Hər subdomain üçün screenshot + texnologiya məlumatı  
    → Admin panel, login səhifəsi, default page-ləri vizual aşkarla

100. **Attack surface yekun hesabatı**  
     ```bash
     # Statistika çıxar
     echo "=== ENUMERATION SUMMARY ===" > summary.txt
     echo "Total Subdomains: $(wc -l < all_subdomains.txt)" >> summary.txt
     echo "Live Hosts: $(wc -l < httpx_results.json)" >> summary.txt
     echo "Total Endpoints: $(wc -l < all_endpoints.txt)" >> summary.txt
     echo "JS Files: $(wc -l < js_files.txt)" >> summary.txt
     echo "Open Ports (unique): $(sort -u nmap_full.gnmap | grep 'open' | wc -l)" >> summary.txt
     cat summary.txt
     ```
     → Bu hesabat exploitation mərhələsi üçün əsas olacaq  
     → Hər tapıntını prioritetlər üzrə sırala: Critical → High → Medium

---

## 🧰 Tool Cədvəli — Tez Başvuru

| # | Tool | Tip | Funksiya | Link |
|---|------|-----|----------|------|
| 1 | **Amass** | Aktiv/Passiv | Subdomain enum | github.com/owasp-amass/amass |
| 2 | **Subfinder** | Passiv | Subdomain OSINT | github.com/projectdiscovery/subfinder |
| 3 | **dnsx** | Aktiv | DNS brute-force | github.com/projectdiscovery/dnsx |
| 4 | **httpx** | Aktiv | HTTP probe | github.com/projectdiscovery/httpx |
| 5 | **ffuf** | Aktiv | Fast fuzzer | github.com/ffuf/ffuf |
| 6 | **gobuster** | Aktiv | Dir/DNS/VHost | github.com/OJ/gobuster |
| 7 | **feroxbuster** | Aktiv | Rekursiv fuzzer | github.com/epi052/feroxbuster |
| 8 | **dirsearch** | Aktiv | Web path bruteforce | github.com/maurosoria/dirsearch |
| 9 | **nuclei** | Aktiv | Template-based scan | github.com/projectdiscovery/nuclei |
| 10 | **gau** | Passiv | URL toplama | github.com/lc/gau |
| 11 | **waybackurls** | Passiv | Arxiv URL-ləri | github.com/tomnomnom/waybackurls |
| 12 | **arjun** | Aktiv | Param kəşfi | github.com/s0md3v/Arjun |
| 13 | **LinkFinder** | Aktiv | JS endpoint kəşfi | github.com/GerbenJavado/LinkFinder |
| 14 | **getJS** | Aktiv | JS toplama | github.com/003random/getJS |
| 15 | **truffleHog** | Passiv | Secret aşkarlama | github.com/trufflesecurity/trufflehog |
| 16 | **gitleaks** | Passiv | Git secret scan | github.com/gitleaks/gitleaks |
| 17 | **subjack** | Aktiv | Subdomain takeover | github.com/haccer/subjack |
| 18 | **testssl.sh** | Aktiv | TLS analiz | github.com/drwetter/testssl.sh |
| 19 | **nikto** | Aktiv | Web server skan | github.com/sullo/nikto |
| 20 | **eyewitness** | Aktiv | Screenshot | github.com/RedSiege/EyeWitness |
| 21 | **wpscan** | Aktiv | WordPress skan | github.com/wpscanteam/wpscan |
| 22 | **theHarvester** | Passiv | OSINT toplama | github.com/laramies/theHarvester |
| 23 | **Shodan CLI** | Passiv | İnfra kəşfi | shodan.io |
| 24 | **PureDNS** | Aktiv | DNS bruteforce | github.com/d3mondev/puredns |
| 25 | **Burp Suite** | Aktiv | Proxy/Scanner | portswigger.net |

---

## 📂 Wordlist Mənbələri

```bash
# SecLists — ən böyük wordlist kolleksiyası
git clone https://github.com/danielmiessler/SecLists /opt/SecLists

# Assetnote Wordlists — API/tech-spesifik
# https://wordlists.assetnote.io/

# Fuzz.txt — endpoint fuzzing
# https://github.com/Bo0oM/fuzz.txt

# Resolvers — DNS skanlar üçün
# https://github.com/trickest/resolvers
```

---

## 🔁 Tövsiyə Olunan İş Axını

```
Passiv (1-60)
    ↓
WHOIS + DNS + SSL + Shodan + crt.sh
    ↓
Wayback + GAU + GitHub + LinkedIn
    ↓
Bütün subdomainlər + parametrlər + teknologiyalar siyahısı
    ↓
Aktiv (61-100)
    ↓
Port skan (Nmap + Masscan)
    ↓
Subdomain brute-force (PureDNS + Amass aktiv)
    ↓
Directory fuzzing (FFuf + Feroxbuster)
    ↓
JS analiz + API endpoint kəşfi
    ↓
Nuclei + WPScan + CMSeek
    ↓
Burp Suite — tam xəritə
    ↓
EyeWitness — vizual hesabat
    ↓
Attack Surface Xəritəsi → Exploitation mərhələsinə keç
```

---

> ⚠️ **Hüquqi Xəbərdarlıq:** Aktiv enumeration hədəf sistemin loglarında iz buraxır.  
> Bütün testlər **yazılı icazə** çərçivəsində aparılmalıdır.

---
*Web Enumeration Deep Dive v1.0 | 100 Addım | Passiv (1-60) + Aktiv (61-100)*
