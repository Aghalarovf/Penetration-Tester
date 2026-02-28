# Enumeration with FFUF

```
ffuf -w WORDLIST -u http://example.com/FUZZ
```

# Directory Traversal

```
ffuf -w /usr/share/wordlists/SecLists/Discovery/Web-Content/common.txt \
     -u http://target.com/FUZZ \
     -fs 0 -fc 404,403 \
     -t 100 -timeout 10 \
     -o dirscan.json

ffuf -w common.txt:PATH,/usr/share/wordlists/SecLists/Discovery/Web-Content/raft-large-directories.txt:EXT \
     -u "http://target.com/FUZZ.PATH" \
     -H "User-Agent: Mozilla/5.0..." \
     -mc 200,301,302 \
     -fs 0,1234 \
     -recursion -recursion-depth 2 \
     -o pro_dir.json

-fs 0: 0 byte response filtrele
-fc 404,403: False positive filtrele
-t 100: 100 thread
-o: JSON output
-H Custom Header
-mc Status Code
```

# Page Fuzzing

```
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

# SUBDOMAIN FUZZING

```
ffuf -w /usr/share/wordlists/SecLists/Discovery/DNS/subdomains-top1million-5000.txt \
     -u https://FUZZ.target.com \
     -H "Host: FUZZ.target.com" \
     -mc 200,403 \
     -fw 0 \
     -t 200 \
     -o subdomains.json
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


