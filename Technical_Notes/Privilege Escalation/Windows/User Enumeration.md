# Get current username
```powershell
echo %USERNAME% || whoami
$env:username
```

# List user privilege
```powershell
whoami /priv
whoami /groups
```

# List all users
```powershell
net user
whoami /all
Get-LocalUser | ft Name,Enabled,LastLogon
Get-ChildItem C:\Users -Force | select Name
```

# List logon requirements; useable for bruteforcing
```powershell
$env:usernadsc
net accounts
```

# Get details about a user
```powershell
net user administrator
net user admin
net user %USERNAME%
```

# List all local groups
```powershell
net localgroup
Get-LocalGroup | ft Name
```

# Get details about a group
```powershell
net localgroup administrators
Get-LocalGroupMember Administrators | ft Name, PrincipalSource
Get-LocalGroupMember Administrateurs | ft Name, PrincipalSource
```

# Get Domain Controllers
```powershell
nltest /DCLIST:DomainName
nltest /DCNAME:DomainName
nltest /DSGETDC:DomainName
```
