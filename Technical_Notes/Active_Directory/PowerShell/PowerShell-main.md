# Basic Enumeration
```

```

# AD Enumeration

## Users, Groups, Computers, OUs, GPOs and etc.
```
# AD Module
Get-ADUser -Filter * | Select samaccountname
Get-ADUser -Identity "admin"
Get-ADUser -Identity "admin" -Properties "Description" -like "*pass*" | Select samaccountname, description
Get-ADUser -Identity "admin" | Select SID
Get-ADUser -Filter 'AdminCount -eq 1' | Select samaccountname,sid
Get-ADUser -Filter 'ServicePrincipalName -ne $null' -Properties serviceprincipalname | Select samaccountname,serviceprincipalname
Get-ADUser -Filter 'PasswordNotRequired -eq $true' | Select samaccountname
Get-ADUser -Filter 'Enabled -eq $false' | Select samaccountname
Get-ADUser -Filter 'LockedOut -eq $true' | Select samaccountname
Get-ADUser -Filter 'DoesNotRequirePreAuth -eq $true' | Select samaccount

Get-ADGroup -Filter * | Select name,Distinguishedname,sid
Get-ADGroup -Filter * -Properties Description | Select name,distinguishedname,description,sid
Get-ADGroup -Filter * -Properties managedby | Select name,distinguishedname,description,sid,managedby
Get-ADGroup -Identity "Group Name" -Properties MemberOf | Select memberof
Get-ADGroup -Filter 'AdminCount -eq 1' | Select name,sid
Get-ADGroupmember -Identity "Schema Admin" | Select name,samaccountname,sid

Get-ADComputer -Filter * | Select name,distinguishedname,sid
Get-ADComputer -Filter * -Properties Description | Select name,properties
Get-ADComputer -Filter * -Properties OperatingSystem | Select name,operatingsystem
Get-ADComputer -Filter * -Properties OperatingSystemversion | Select name,operatingsystemversion
Get-ADComputer -Filter * -Properties IPv4Address | Select name,ipv4address
Get-ADDomainController -Filter * | Select name,ipv4address
Get-ADComputer -Filter 'Operatingsystem -like "*sql*"' | Select name,operatingsystem
Get-ADComputer -Filter 'ServicePrincipalName -like "*mssql*"' | Select name,serviceprincipalname



```

# 
