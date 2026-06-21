# PowerView Cheat Sheet — Active Directory Enumeration

> Load PowerView first:
> ```powershell
> Import-Module .\PowerView.ps1
> # or bypass execution policy:
> powershell -ep bypass -c "Import-Module .\PowerView.ps1"
> ```

---

# Domain Information

### 1. Retrieve domain information
```powershell
Get-Domain
Get-Domain -Domain target.local
```

### 2. Identify Domain Controllers
```powershell
Get-DomainController
Get-DomainController -Domain target.local | Select Name, IPAddress, OSVersion
```

### 3. Pull domain password policy
```powershell
Get-DomainPolicyData | Select -ExpandProperty SystemAccess
(Get-DomainPolicyData).SystemAccess
```

### 4. Resolve domain SID
```powershell
Get-DomainSID
Get-DomainSID -Domain target.local
```

### 5. Retrieve forest information
```powershell
Get-Forest
Get-Forest -Forest target.local
```

### 6. Enumerate all domains in the forest
```powershell
Get-ForestDomain
Get-ForestDomain -Forest target.local
```

### 7. Discover AD subnets
```powershell
Get-DomainSubnet
Get-DomainSubnet | Select Name, SiteObject
```

---

# User Enumeration

### 8. List all domain users
```powershell
Get-DomainUser
Get-DomainUser | Select SamAccountName, MemberOf, LastLogon
```

### 9. Get detailed info on a specific user
```powershell
Get-DomainUser -Identity jdoe
Get-DomainUser -Identity jdoe | Select *
```

### 10. Find accounts with AdminCount=1
```powershell
Get-DomainUser -AdminCount | Select SamAccountName, AdminCount
```

### 11. Find SPN-enabled accounts (Kerberoasting)
```powershell
Get-DomainUser -SPN | Select SamAccountName, ServicePrincipalName
```

### 12. Find accounts not requiring pre-authentication (AS-REP Roasting)
```powershell
Get-DomainUser -PreauthNotRequired | Select SamAccountName, UserAccountControl
```

### 13. Find active (enabled) users
```powershell
Get-DomainUser -UACFilter NOT_ACCOUNTDISABLE | Select SamAccountName
```

---

# Group Enumeration

### 14. List all groups
```powershell
Get-DomainGroup
Get-DomainGroup | Select Name, Description
```

### 15. Enumerate group members
```powershell
Get-DomainGroupMember -Identity "Domain Admins"
Get-DomainGroupMember -Identity "Domain Admins" | Select MemberName, MemberObjectClass
```

### 16. Resolve nested group membership
```powershell
Get-DomainGroupMember -Identity "Domain Admins" -Recurse
```

### 17. Find all groups a specific user belongs to
```powershell
Get-DomainGroup -UserName jdoe
```

### 18. Find groups with "admin" in the name
```powershell
Get-DomainGroup -Identity *admin* | Select Name, Description
```

### 19. Enumerate local groups on a host
```powershell
Get-NetLocalGroup -ComputerName DC01
Get-NetLocalGroupMember -ComputerName DC01 -GroupName Administrators
```

### 20. Check membership of the Protected Users group
```powershell
Get-DomainGroupMember -Identity "Protected Users" -Recurse | Select MemberName
```

---

# Computer Enumeration

### 21. List all computers in the domain
```powershell
Get-DomainComputer
Get-DomainComputer | Select Name, OperatingSystem, DNSHostName
```

### 22. Filter for servers only
```powershell
Get-DomainComputer -OperatingSystem "*Server*" | Select Name, OperatingSystem
```

### 23. Identify outdated OS versions
```powershell
Get-DomainComputer | Where-Object { $_.OperatingSystem -match "2003|2008|XP|Vista|7" } | Select Name, OperatingSystem
```

### 24. Identify live (pingable) computers
```powershell
Get-DomainComputer | ForEach-Object { if (Test-Connection $_.DNSHostName -Count 1 -Quiet) { $_.Name } }
```

### 25. Retrieve full details on a specific computer
```powershell
Get-DomainComputer -Identity DC01 | Select *
```

---

# Delegation

### 26. Find computers and users with Unconstrained Delegation
```powershell
Get-DomainComputer -Unconstrained | Select Name, DNSHostName
Get-DomainUser -Unconstrained | Select-Object SamAccountName, ServicePrincipalName
```

### 27. Find computers and users with Constrained Delegation
```powershell
Get-DomainComputer -LDAPFilter "(msDS-AllowedToDelegateTo=*)" | Select-Object Name, msDS-AllowedToDelegateTo
Get-DomainUser -LDAPFilter "(msDS-AllowedToDelegateTo=*)" | Select-Object SamAccountName, msDS-AllowedToDelegateTo
```

### 28. Find RBCD
```powershell
function Get-RBCDAudit {
    [CmdletBinding()]
    param(
        [switch]$ExpandGroups,
        [switch]$IncludeDisabled
    )
    function Get-RBCDVerdict {
        param($TargetEnabled, $AttackerEnabled, $HasSPN, $Resolved, $ObjClass)
        if (-not $Resolved) {
            return "UNKNOWN (unresolved/cross-forest SID — verify manually)"
        }
        if (-not $TargetEnabled) {
            return "UNLIKELY (target computer is disabled)"
        }
        if ($AttackerEnabled -eq $false) {
            return "UNLIKELY (attacker account is disabled)"
        }
        if ($HasSPN) {
            return "CONFIRMED (attacker has SPN — S4U2Self/S4U2Proxy directly usable)"
        }
        if ($ObjClass -eq "computer") {
            # Computers normally carry an SPN by default; absence here is unusual but the
            # object class itself supports self-SPN registration (validated write).
            return "LIKELY (computer object, no SPN listed — confirm SPN/dNSHostName)"
        }
        if ($ObjClass -eq "user") {
            return "LIKELY (conditional — user has no SPN; exploitable only if attacker can self-add an SPN, e.g. via owned machine account or write rights on self)"
        }
        return "LIKELY (conditional — verify SPN-add capability)"
    }
    $Targets = Get-DomainObject -LDAPFilter "(msDS-AllowedToActOnBehalfOfOtherIdentity=*)" `
        -Properties samaccountname,distinguishedname,objectclass,useraccountcontrol,msds-allowedtoactonbehalfofotheridentity
    foreach ($Target in $Targets) {
        $TargetEnabled = -not ([int]$Target.useraccountcontrol -band 2)  # ACCOUNTDISABLE bit
        if (-not $IncludeDisabled -and -not $TargetEnabled) { continue }
        $RawData = $Target."msds-allowedtoactonbehalfofotheridentity"
        if (-not $RawData) { continue }
        $SD = New-Object System.Security.AccessControl.RawSecurityDescriptor($RawData, 0)
        foreach ($ACE in $SD.DiscretionaryAcl) {
            if ($ACE.AceType -ne "AccessAllowed") { continue }
            $SID = $ACE.SecurityIdentifier.Value
            $ResolvedObj = $null
            try {
                $ResolvedObj = Get-DomainObject -Identity $SID -Properties samaccountname,objectclass,useraccountcontrol,serviceprincipalname -ErrorAction Stop
            } catch {}
            $Resolved        = [bool]$ResolvedObj
            $IdentityName    = if ($ResolvedObj) { $ResolvedObj.samaccountname } else { "UNRESOLVED ($SID)" }
            $ObjClass        = if ($ResolvedObj) { $ResolvedObj.objectclass[-1] } else { "unknown" }
            $HasSPN          = if ($ResolvedObj) { [bool]$ResolvedObj.serviceprincipalname } else { $false }
            $AttackerEnabled = if ($ResolvedObj) { -not ([int]$ResolvedObj.useraccountcontrol -band 2) } else { $null }

            $Verdict = Get-RBCDVerdict -TargetEnabled $TargetEnabled -AttackerEnabled $AttackerEnabled `
                                        -HasSPN $HasSPN -Resolved $Resolved -ObjClass $ObjClass
            [PSCustomObject]@{
                "Target Computer (Victim)"      = $Target.samaccountname
                "Target Enabled"                = $TargetEnabled
                "Allowed Identity (Attacker)"   = $IdentityName
                "Attacker SID"                  = $SID
                "Attacker Object Type"          = $ObjClass
                "Attacker Enabled"              = $AttackerEnabled
                "Attacker Has SPN (S4U-ready)"  = $HasSPN
                "RBCD Exploitability"           = $Verdict
                "Direct or Group?"               = "Direct"
            }
            if ($ExpandGroups -and $ResolvedObj -and $ResolvedObj.objectclass -contains "group") {
                $Members = Get-DomainGroupMember -Identity $SID -Recurse -ErrorAction SilentlyContinue
                foreach ($Member in $Members) {
                    $MemberObj = $null
                    try {
                        $MemberObj = Get-DomainObject -Identity $Member.MemberSID -Properties objectclass,useraccountcontrol,serviceprincipalname -ErrorAction Stop
                    } catch {}
                    $MemberResolved = [bool]$MemberObj
                    $MemberClass    = if ($MemberObj) { $MemberObj.objectclass[-1] } else { "unknown" }
                    $MemberHasSPN   = if ($MemberObj) { [bool]$MemberObj.serviceprincipalname } else { $false }
                    $MemberEnabled  = if ($MemberObj) { -not ([int]$MemberObj.useraccountcontrol -band 2) } else { $null }
                    $MemberVerdict = Get-RBCDVerdict -TargetEnabled $TargetEnabled -AttackerEnabled $MemberEnabled `
                                                      -HasSPN $MemberHasSPN -Resolved $MemberResolved -ObjClass $MemberClass
                    [PSCustomObject]@{
                        "Target Computer (Victim)"      = $Target.samaccountname
                        "Target Enabled"                = $TargetEnabled
                        "Allowed Identity (Attacker)"   = $Member.MemberName
                        "Attacker SID"                  = $Member.MemberSID
                        "Attacker Object Type"          = $MemberClass
                        "Attacker Enabled"              = $MemberEnabled
                        "Attacker Has SPN (S4U-ready)"  = $MemberHasSPN
                        "RBCD Exploitability"           = $MemberVerdict
                        "Direct or Group?"               = "Via group: $IdentityName"
                    }
                }
            }
        }
    }
    Write-Host "`n[*] Note: This function only audits RBCD." -ForegroundColor Yellow
    Write-Host "[*] For unconstrained delegation, run: Get-DomainComputer -Unconstrained" -ForegroundColor Yellow
    Write-Host "[*] For classic constrained delegation, run: Get-DomainComputer -TrustedToAuth" -ForegroundColor Yellow
}
Get-RBCDAudit -ExpandGroups -IncludeDisabled |
    Sort-Object "RBCD Exploitability" |
    Out-GridView
```

---

# AD CS, LAPS & SPNs

### 29. Enumerate Certificate Services (AD CS)
```powershell
Get-DomainObject -LDAPFilter "(objectClass=pKIEnrollmentService)" | Select Name, DNSHostName
Get-DomainObject -SearchBase "CN=Configuration,DC=domain,DC=local" -LDAPFilter "(objectClass=pKIEnrollmentService)"
```

### 30. Find computers with LAPS enabled
```powershell
Get-DomainComputer | Where-Object { $_.ms-mcs-admpwdexpirationtime } | Select Name
Get-DomainComputer -LDAPFilter "(ms-mcs-admpwd=*)" | Select Name
```

### 31. Read LAPS passwords
```powershell
Get-DomainComputer -LDAPFilter "(ms-mcs-admpwd=*)" | Select Name, 'ms-mcs-admpwd'
Get-DomainComputer -Identity TARGET01 | Select Name, 'ms-mcs-admpwd'
```

### 32. Enumerate SPNs on computer accounts
```powershell
Get-DomainComputer -SPN | Select Name, ServicePrincipalName
```

---

# ACL Enumeration

### 33. Retrieve ACLs on a specific user
```powershell
Get-DomainObjectAcl -Identity jdoe -ResolveGUIDs
```

### 34. Find users with a specific interesting permission
```powershell
Get-DomainObjectAcl -Identity jdoe -ResolveGUIDs | Where-Object { $_.ActiveDirectoryRights -match "WriteProperty|GenericAll|GenericWrite" }
```

### 35. Find ForceChangePassword permissions
```powershell
Get-DomainObjectAcl -ResolveGUIDs | Where-Object { $_.ObjectAceType -eq "User-Force-Change-Password" } | Select ObjectDN, SecurityIdentifier
```

### 36. Find interesting ACLs across the domain
```powershell
Find-InterestingDomainAcl -ResolveGUIDs
Find-InterestingDomainAcl -ResolveGUIDs | Select ObjectDN, AceType, SecurityIdentifier
```

### 37. Enumerate ACLs on the domain root
```powershell
Get-DomainObjectAcl -SearchBase "DC=domain,DC=local" -ResolveGUIDs | Select SecurityIdentifier, ActiveDirectoryRights
```

### 38. Find DCSync permissions
```powershell
Get-DomainObjectAcl -SearchBase "DC=domain,DC=local" -ResolveGUIDs | Where-Object {
    ($_.ObjectAceType -match "DS-Replication-Get-Changes") -or
    ($_.ActiveDirectoryRights -match "ExtendedRight")
} | Select SecurityIdentifier, ObjectAceType
```

### 39. Retrieve ACLs on a specific group
```powershell
Get-DomainObjectAcl -Identity "Domain Admins" -ResolveGUIDs
```

### 40. Find AddMember permissions
```powershell
Get-DomainObjectAcl -ResolveGUIDs | Where-Object { $_.ObjectAceType -eq "Self-Membership" -or $_.ActiveDirectoryRights -match "WriteProperty" } | Select ObjectDN, SecurityIdentifier, ActiveDirectoryRights
```

---

# Trust Enumeration

### 41. Enumerate domain trusts
```powershell
Get-DomainTrust
Get-DomainTrust | Select SourceName, TargetName, TrustType, TrustDirection
```

### 42. Enumerate forest trusts
```powershell
Get-ForestTrust
Get-ForestTrust | Select TopLevelNames, TrustDirection, TrustType
```

### 43. Identify external trusts
```powershell
Get-DomainTrust | Where-Object { $_.TrustType -eq "External" }
```

### 44. Check SID Filtering status
```powershell
Get-DomainTrust | Select SourceName, TargetName, SIDFilteringForestAware, SIDFilteringQuarantined
```

### 45. Find bidirectional trusts
```powershell
Get-DomainTrust | Where-Object { $_.TrustDirection -eq "Bidirectional" }
```

### 46. Map the full trust relationships
```powershell
Get-ForestDomain | ForEach-Object { Get-DomainTrust -Domain $_.Name } | Select SourceName, TargetName, TrustDirection, TrustType
```

### 47. Count all users across the forest
```powershell
Get-ForestDomain | ForEach-Object { $d = $_.Name; $c = (Get-DomainUser -Domain $d).Count; "$d : $c users" }
```

### 48. Identify child domains
```powershell
Get-ForestDomain | Where-Object { $_.Parent -ne $null } | Select Name, Parent
```

---

# GPO Enumeration

### 49. List all GPOs
```powershell
Get-DomainGPO | Select DisplayName, Name, GPCFileSysPath
```

### 50. Find GPOs applied to a specific computer
```powershell
Get-DomainGPO -ComputerIdentity DC01 | Select DisplayName
```

### 51. Find GPOs applied to a specific user
```powershell
Get-DomainGPO -UserIdentity jdoe | Select DisplayName
```

### 52. Retrieve ACLs on a GPO
```powershell
Get-DomainObjectAcl -Identity "GPO_DisplayName" -ResolveGUIDs
```

### 53. Find GPOs linked to an OU
```powershell
Get-DomainOU | Select Name, GPLink
Get-DomainGPOLocalGroup | Select GPODisplayName, GroupName, GroupMemberOf
```

---

# Advanced Enumeration

### 54. Enumerate Resource-Based Constrained Delegation (RBCD)
```powershell
Get-DomainComputer | Where-Object { $_.msDS-AllowedToActOnBehalfOfOtherIdentity } | Select Name, 'msDS-AllowedToActOnBehalfOfOtherIdentity'
```

### 55. Retrieve krbtgt account information
```powershell
Get-DomainUser -Identity krbtgt | Select SamAccountName, PasswordLastSet, PasswordNeverExpires
```

### 56. Identify high-value SPNs
```powershell
Get-DomainUser -SPN | Where-Object { $_.ServicePrincipalName -match "MSSQLSvc|http|ldap|cifs|host" } | Select SamAccountName, ServicePrincipalName
```

### 57. Find services eligible for delegation
```powershell
Get-DomainUser -TrustedToAuth | Select SamAccountName, msds-AllowedToDelegateTo
Get-DomainComputer -TrustedToAuth | Select Name, msds-AllowedToDelegateTo
```

### 58. Review Kerberos policy
```powershell
(Get-DomainPolicyData).KerberosPolicy
```

---

# Session & Login Enumeration

### 59. Enumerate active network sessions
```powershell
Get-NetSession -ComputerName DC01
Get-NetSession -ComputerName DC01 | Select CName, UserName
```

### 60. Identify currently logged-on users
```powershell
Get-NetLoggedon -ComputerName DC01
Get-NetLoggedon -ComputerName WS01 | Select UserName, LogonDomain
```

### 61. Find machines where you have local admin
```powershell
Find-LocalAdminAccess
Find-LocalAdminAccess -ComputerFile computers.txt
```

### 62. Stealthily locate Domain Admin sessions
```powershell
Find-DomainUserLocation -UserGroupIdentity "Domain Admins" -Stealth
Find-DomainUserLocation -UserGroupIdentity "Domain Admins" | Select UserName, SessionFromName
```

---

# Share Enumeration

### 63. Enumerate SMB shares
```powershell
Get-NetShare -ComputerName DC01
Get-DomainComputer | Get-NetShare
```

### 64. Identify accessible shares
```powershell
Find-DomainShare
Find-DomainShare -CheckShareAccess | Select Name, ComputerName
```

### 65. Search shares for interesting files
```powershell
Find-InterestingDomainShareFile
Find-InterestingDomainShareFile -Include "*.txt","*.xml","*.config","*.ps1","*.bat"
Find-InterestingDomainShareFile -SharePath "\\DC01\SYSVOL"
```

---

# Miscellaneous

### 66. Find accounts protected by AdminSDHolder
```powershell
Get-DomainObjectAcl -SearchBase "CN=AdminSDHolder,CN=System,DC=domain,DC=local" -ResolveGUIDs | Select SecurityIdentifier, ActiveDirectoryRights
```

### 67. Identify the Schema Admins group
```powershell
Get-DomainGroup -Identity "Schema Admins" | Select Name
Get-DomainGroupMember -Identity "Schema Admins" | Select MemberName
```

### 68. Check the ms-DS-MachineAccountQuota value
```powershell
Get-DomainObject -Identity "DC=domain,DC=local" | Select 'ms-DS-MachineAccountQuota'
```

### 69. Check for Shadow Credentials attribute
```powershell
Get-DomainUser | Where-Object { $_.'msDS-KeyCredentialLink' } | Select SamAccountName, 'msDS-KeyCredentialLink'
Get-DomainComputer | Where-Object { $_.'msDS-KeyCredentialLink' } | Select Name, 'msDS-KeyCredentialLink'
```

---

# BloodHound & Statistics

### 70. Collect data for BloodHound
```powershell
# SharpHound (preferred)
.\SharpHound.exe -c All --zipfilename bloodhound.zip

# PowerView-based collection
Invoke-BloodHound -CollectionMethod All -OutputDirectory C:\temp\ -OutputPrefix "domain"
Invoke-BloodHound -CollectionMethod All,GPOLocalGroup -Stealth
```

### 71. Pull overall domain statistics
```powershell
$users     = (Get-DomainUser).Count
$computers = (Get-DomainComputer).Count
$groups    = (Get-DomainGroup).Count
$admins    = (Get-DomainGroupMember -Identity "Domain Admins").Count

Write-Host "Users:     $users"
Write-Host "Computers: $computers"
Write-Host "Groups:    $groups"
Write-Host "DA Count:  $admins"
```

---

## Quick Reference — Common Flags

| Flag | Description |
|------|-------------|
| `-Domain` | Target a specific domain |
| `-Server` / `-DomainController` | Specify a DC to query |
| `-Credential` | Use alternate credentials (`New-Object PSCredential`) |
| `-LDAPFilter` | Raw LDAP filter string |
| `-Properties` | Return only specified attributes |
| `-SearchBase` | Start LDAP search from a specific OU/DN |
| `-ResolveGUIDs` | Translate GUIDs to human-readable names in ACLs |
| `-Recurse` | Recursively resolve nested groups |
| `-AdminCount` | Filter for objects with AdminCount=1 |
| `-SPN` | Filter for objects with SPNs set |
| `-Unconstrained` | Filter for unconstrained delegation |
| `-TrustedToAuth` | Filter for constrained delegation |
| `-UACFilter` | Filter by UserAccountControl flags |
| `-PreauthNotRequired` | AS-REP roastable accounts |

---

## Alternate Credentials Example

```powershell
$cred = New-Object System.Management.Automation.PSCredential("domain\user", (ConvertTo-SecureString "password" -AsPlainText -Force))

Get-DomainUser -Credential $cred -Domain target.local
Get-DomainComputer -Credential $cred
Get-DomainController -Credential $cred -Domain target.local
```
