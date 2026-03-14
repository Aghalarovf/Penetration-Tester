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
Get-DomainUser * | Select-Object samaccountname,description |Where-Object {$_.Description -ne $null}

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

Import-Module .\PowerView.ps1
Import-Module .\PowerUpSQL.ps1

# CanRDP (Remote Desktop Access)
## Konkret bir hostda RDP icazəsi olanları tapmaq
Get-NetLocalGroupMember -ComputerName ACADEMY-EA-MS01 -GroupName "Remote Desktop Users"
## Bütün domendə GPO vasitəsilə RDP hüququ verilmiş istifadəçilər
Get-DomainGPOUserLocalGroupMapping -LocalGroup "Remote Desktop Users" | select ObjectIdentifier, MemberName, GPODisplayName

# CanPSRemote (PowerShell Remoting / WinRM)
## Konkret hostda WinRM icazəsi olanlar
Get-NetLocalGroupMember -ComputerName ACADEMY-EA-MS01 -GroupName "Remote Management Users"
## "Remote Management Users" qrupunun üzvlərini tapmaq
Get-DomainGroupMember -Identity "Remote Management Users" | select MemberName, GroupName

## Konkret Userin bütün hostlar üzərində PSRemote icazəsi
(Get-DomainComputer).dnshostname | Out-File -FilePath "hosts.txt" -Encoding ascii

Get-Content .\hosts.txt | Where-Object { $_ -ne "" } | ForEach-Object { Get-NetLocalGroupMember -ComputerName $_.Trim() -GroupName "Remote Management Users" -ErrorAction SilentlyContinue } | Where-Object { $_.MemberName -match "bdavis" }


# SQLAdmin (Database Access)
## Şəbəkədəki bütün SQL serverləri (SPN vasitəsilə) tapmaq
Get-DomainUser | Where-Object {$_.serviceprincipalname -ne $null} | Select-Object samaccountname, serviceprincipalname

## SQL Admins qrupunun üzvlərini siyahılamaq
Get-DomainGroupMember -Identity "SQL Admins" | select MemberName

# Müəyyən bir istifadəçinin şəbəkədə harada Local Admin olduğunu tapmaq
Find-LocalAdminAccess -UserName "Hədəf_İstifadəçi"

# PowerShell Remoting (WinRM) - Windows-dan
$password = ConvertTo-SecureString "Klmcargo2" -AsPlainText -Force
$cred = new-object System.Management.Automation.PSCredential ("INLANEFREIGHT\forend", $password)
## Sessiyanı başlatmaq
Enter-PSSession -ComputerName ACADEMY-EA-MS01 -Credential $cred
## Sessiyadan çıxmaq
Exit-PSSession

# Evil-WinRM - Linux-dan (Kali)
## Quraşdırma
gem install evil-winrm

## Qoşulma
evil-winrm -i 10.129.201.234 -u forend -p 'Klmcargo2'

# PowerUpSQL (Windows):
## SQL instansiyalarını tapmaq
Get-SQLInstanceDomain
## SQL sorğusu icra etmək (Versiyanı yoxlamaq)
Get-SQLQuery -Instance "172.16.5.150,1433" -username "inlanefreight\damundsen" -password "SQL1234!" -query "Select @@version" -Verbose

Get-SQLQuery -Instance "172.16.5.150,1433" -username "inlanefreight\damundsen" -password "SQL1234!" -query "EXEC sp_configure 'show advanced options', 1; RECONFIGURE; EXEC sp_configure 'xp_cmdshell', 1; RECONFIGURE;" -Verbose

# Impacket (Linux):
## Windows Authentication istifadə edərək SQL-ə qoşulmaq
mssqlclient.py INLANEFREIGHT/DAMUNDSEN@172.16.5.150 -windows-auth

```
