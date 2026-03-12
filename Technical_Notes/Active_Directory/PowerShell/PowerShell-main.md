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

## Privileged Access
```
# Remote Desktop
Import-Module .\PowerView.ps1
Get-NetLocalGroupMember -ComputerName ACADEMY-EA-MS01 -GroupName "Remote Desktop Users"
Get-NetLocalGroupMember -ComputerName ACADEMY-EA-MS01 -GroupName "Remote Management Users"

$password = ConvertTo-SecureString "Klmcargo2" -AsPlainText -Force
$cred = new-object System.Management.Automation.PSCredential ("INLANEFREIGHT\forend", $password)
Enter-PSSession -ComputerName ACADEMY-EA-MS01 -Credential $cred
Exit-PSSession

# WINRM
gem install evil-winrm
evil-winrm -i 10.129.201.234 -u forend

# SQL Server
cd .\PowerUpSQL\
Import-Module .\PowerUpSQL.ps1
Get-SQLInstanceDomain
Get-SQLQuery -Verbose -Instance "172.16.5.150,1433" -username "inlanefreight\damundsen" -password "SQL1234!" -query 'Select @@version'

mssqlclient.py INLANEFREIGHT/DAMUNDSEN@172.16.5.150 -windows-auth
```
