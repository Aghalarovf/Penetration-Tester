# Target Profiling (FFUF-dan əvvəl)

```
# Default response size
curl -s http://target.com/asdasdasd | wc -w

# Default word count
curl -s http://target.com/asdasdasd | wc -c

ffuf -w WORDLIST -u http://example.com/FUZZ -fs -fw
-fs <baseline_size>
-fw <baseline_word> 

```

# SUBDOMAIN FUZZING

```
/etc/hosts yazılmalıdır
10.10.10.10 academy.htb

ffuf -w /usr/share/wordlists/seclists/Discovery/DNS/subdomains-top1million-5000.txt \
     -u http://FUZZ.academy.htb:31996 \
     -t 50 -fs 985

archive, faculty, test
```

# VHOST FUZZING

```
ffuf -w /usr/share/wordlists/seclists/Discovery/DNS/subdomains-top1million-5000.txt \
     -u http://154.57.164.78:31996 \
     -H "Host: FUZZ.academy.htb" -t 50 -fs 985

archive, faculty, test
```

# Extension Fuzzing

```
ffuf -w /usr/share/seclists/Discovery/Web-Content/web-extensions.txt:FUZZ -u http://archive.academy.htb:31996/indexFUZZ
ffuf -w /usr/share/seclists/Discovery/Web-Content/web-extensions.txt:FUZZ -u http://test.academy.htb:31996/indexFUZZ
ffuf -w /usr/share/seclists/Discovery/Web-Content/web-extensions.txt:FUZZ -u http://faculty.academy.htb:31996/indexFUZZ
```

# Directory Discovery

```
ffuf -w common.txt -u http://target/FUZZ -ac -t 40
Auto calibration false positive 20–30% azaldır.

ffuf -w /usr/share/wordlists/seclists/Discovery/Web-Content/directory-list-2.3-small.txt \
     -u http://faculty.academy.htb:31996/FUZZ \
     -fs 0 -fc 404,403 \
     -t 1000 -timeout 3

ffuf -w /usr/share/wordlists/seclists/Discovery/Web-Content/directory-list-2.3-small.txt \
     -u http://faculty.academy.htb:31996/courses/FUZZ \
     -fs 0 -fc 404,403 \
     -t 1000 -timeout 3 -e .php,.phps,.php7

ffuf -w common.txt:PATH,/usr/share/wordlists/SecLists/Discovery/Web-Content/raft-large-directories.txt:FUZZ /usr/share/wordlists/seclist/extension.txt:EXT

-H "User-Agent: Mozilla/5.0..." \
-recursion -recursion-depth 2 \
-e .php,.asp,.aspx,.jsp,.bak,.old,.html,.txt,.info \
-o pro_dir.json
-fs 0: 0 byte response filtrele
-fc 404,403: False positive filtrele
-t 100: 100 thread
-o: JSON output
-H Custom Header
-mc Status Code
-e Extensions
```

# Page Fuzzing

```
ffuf -w /opt/useful/seclists/Discovery/Web-Content/web-extensions.txt:FUZZ -u http://SERVER_IP:PORT/blog/indexFUZZ

# GET Parameter Fuzzing
ffuf -w params.txt:PARAM,/usr/share/wordlists/SecLists/Discovery/Web-Content/burp-parameter-names.txt:VAL \
     -u "http://target.com/page?FUZZ=VAL" \
     -H "Cookie: session=abc123" \
     -mc 200,302 \
     -fw 10 \
     -o params.json

# POST Form Fuzzing
ffuf -w users.txt:USER,passes.txt:PASS \
     -u http://target.com/login \
     -X POST \
     -d "username=FUZZ&password=FUZZ" \
     -H "Content-Type: application/x-www-form-urlencoded" \
     -mc 200,302 \
     -fr "Invalid|Error" \
     -o brute.json
```

# RECURSIVE FUZZING (Deep Scan)

```
ffuf -w common.txt \
     -u http://target.com/FUZZ \
     -recursion \
     -recursion-depth 3 \
     -recursion-threads 50 \
     -mc all \
     -fs 0 \
     -o recursive.json

ffuf -w raft-large-directories.txt \
     -u http://target.com/FUZZ \
     -recursion -recursion-depth 2 \
     -match-extension txt,php,asp,jsp,aspx \
     -o deepscan.json
```

# VHOST FUZZING

```
ffuf -w /usr/share/wordlists/SecLists/Discovery/DNS/subdomains-top1million-20000.txt \
     -u http://target.com \
     -H "Host: FUZZ.target.com" \
     -H "User-Agent: Mozilla/5.0" \
     -mc 200,403,401 \
     -fw 12 \
     -ac \
     -o vhosts.json

ffuf -w /opt/useful/seclists/Discovery/DNS/subdomains-top1million-5000.txt:FUZZ -u http://academy.htb:PORT/ -H 'Host: FUZZ.academy.htb'
```

# Parameter Discovery

```
# GET
ffuf -w /usr/share/wordlists/seclists/Discovery/Web-Content/burp-parameter-names.txt:FUZZ -u http://admin.academy.htb:30979/admin/admin.php?FUZZ=key -fs xxx

# POST
ffuf -w /usr/share/wordlists/seclists/Discovery/Web-Content/burp-parameter-names.txt:FUZZ -u http://admin.academy.htb:30979/admin/admin.php -X POST -d 'FUZZ=key' -H 'Content-Type: application/x-www-form-urlencoded' -fs 798

for i in $(seq 1 1000); do echo $i >> ids.txt; done

ffuf -w ids.txt:FUZZ -u http://admin.academy.htb:30979/admin/admin.php -X POST -d 'id=FUZZ' -H 'Content-Type: application/x-www-form-urlencoded' -fs 768

curl http://admin.academy.htb:30979/admin/admin.php -X POST -d 'id=<VALUE>' -H 'Content-Type: application/x-www-form-urlencoded'
```

# DNS RECORDS (Zone Transfer Style)

```
ffuf -w /usr/share/wordlists/SecLists/Discovery/DNS/fierce-namelist.txt \
     -u http://dns.target.com/FUZZ \
     -H "Host: FUZZ.target.com" \
     -mc 200,403 \
     -o dns_records.json
```

# VALUE FUZZING (Parameter Pollution)

```
# LFI/RFI Fuzz
ffuf -w /usr/share/wordlists/SecLists/Fuzzing/LFI/LFI-grid-5.txt \
     -u "http://target.com/page?file=../FUZZ" \
     -fs 1234 \
     -mc 200 \
     -o lfi.json

# XSS Payload Fuzz
ffuf -w /usr/share/wordlists/SecLists/Fuzzing/XSS/xss_final.txt \
     -u "http://target.com/search?q=FUZZ" \
     -fr "Filtered|Blocked" \
     -o xss.json

# SQLi Fuzz
ffuf -w /usr/share/wordlists/SecLists/Fuzzing/SQL-Injection/sqli-5.txt \
     -u "http://target.com/user?id=1'FUZZ" \
     -mc 500 \
     -o sqli.json
```

# Important Flags

```
Flag,Ne İşe Yarar,Örnek
-fs SIZE,Size filtre,"-fs 0,1234"
-fc CODE,Status filtre,"-fc 404,403"
-mc CODE,Match code,"-mc 200,301"
-fw LEN,Word count filtre,-fw 10
-fr REGEX,Response filtre,"-fr ""Error"""
-t N,Thread say,-t 100
-o FILE,JSON output,-o results.json
-ac,Auto calibration,-ac
-H HEADER,Custom header,"-H ""X-Forwarded-For: 127.0.0.1"""
```


