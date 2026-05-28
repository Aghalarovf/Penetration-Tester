# Querying Current Patch Level
```powershell
wmic qfe
```

# Running Sherlock
```powershell
Set-ExecutionPolicy bypass -Scope process

Import-Module .\Sherlock.ps1
Find-AllVulns
```

# Metasploit
```powershell
msf6 exploit(windows/smb/smb_delivery) > search smb_delivery

set ForceExploit true
```
