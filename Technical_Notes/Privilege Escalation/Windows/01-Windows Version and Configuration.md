# System Info
```powershell
systeminfo | findstr /B /C:"OS Name" /C:"OS Version"
```

# Kernel Update
```powershell
wmic qfe
Get-HotFix | ft -AutoSize
```

# OS Information
```powershell
wmic os get osarchitecture || echo %PROCESSOR_ARCHITECTURE%
```

# Environment
```powershell
set
Get-ChildItem Env: | ft Key,Value
```

# Installed Programs
```powershell
wmic product get name
Get-WmiObject -Class Win32_Product |  select Name, Version
```

# Drives
```powershell
wmic logicaldisk get caption || fsutil fsinfo drives
wmic logicaldisk get caption,description,providername
Get-PSDrive | where {$_.Provider -like "Microsoft.PowerShell.Core\FileSystem"}| ft Name,Root
```
