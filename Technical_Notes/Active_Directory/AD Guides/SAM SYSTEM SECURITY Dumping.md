# File Location
```powershell
C:\Windows\System32\config\
```

# Privileged Account Check
```powershell
whoami /priv

SeBackupPrivilege
SeRestorePrivilege
```

# Defender
```powershell
dir /s sam.bak system.bak security.bak
```

# Defender Status
```powershell
Get-MpComputerStatus | select RealTimeProtectionEnabled
```
