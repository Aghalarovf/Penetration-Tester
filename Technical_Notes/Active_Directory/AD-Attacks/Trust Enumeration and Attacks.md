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

---

### 2.1 `nltest` (part of Windows / RSAT)
```cmd
:: Enumerate all domain trusts visible from current domain
nltest /domain_trusts

:: Verbose, includes trust attributes and SIDs
nltest /domain_trusts /all_trusts /v

:: Discover trusted DC for a given domain
nltest /dsgetdc:targetdomain.local

:: Get info about a specific trust
nltest /server:dc01.target.local /trusted_domains
```

### 2.2 `netdom` (RSAT - Active Directory Domain Services Tools)
```cmd
:: Query trust relationships of a domain
netdom query /domain:target.local trust

:: Query domain controllers
netdom query /domain:target.local dc

:: Query workstations/servers
netdom query /domain:target.local workstation
```

### 2.3 PowerShell Active Directory Module (`RSAT-AD-PowerShell`)
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

## 3. PowerView (PowerSploit / Dev-Branch)

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

### 3.1 Interpreting `Get-DomainTrust` Output
| Field | Meaning |
|---|---|
| `SourceName` | Domain initiating the query |
| `TargetName` | Trusted/trusting domain |
| `TrustType` | WINDOWS_ACTIVE_DIRECTORY / MIT / DCE |
| `TrustAttributes` | Bitmask (see §6.3) |
| `TrustDirection` | Inbound / Outbound / Bidirectional |

---

## 4. BloodHound / SharpHound

BloodHound visualizes trust relationships as graph edges and is the most efficient way to find abusable cross-domain/cross-forest paths.

```powershell
# Collect data including trust info (CollectionMethod Trusts / All)
SharpHound.exe -c All
SharpHound.exe -c Trusts
SharpHound.exe -c Trusts,DCOnly --domain target.local

# Python collector (cross-platform, works over network without agent)
bloodhound-python -u user -p pass -d target.local -ns <DC_IP> -c All
bloodhound-python -u user -p pass -d target.local -ns <DC_IP> -c Trusts
```

In the BloodHound GUI:
- **Analysis tab → "Map Domain Trusts"** for a visual trust graph.
- Node type: `Domain` — edges labeled `TrustedBy` indicate direction.
- Query for **foreign group membership / SID history abuse paths** to identify exploitable trust links.

Useful built-in/custom Cypher queries:
```cypher
// All trust relationships
MATCH p=(d1:Domain)-[:TrustedBy]->(d2:Domain) RETURN p

// Find unconstrained delegation across a trust boundary
MATCH (c:Computer {unconstraineddelegation:true}) RETURN c

// Find foreign users with privileged group membership
MATCH (u:User)-[:MemberOf]->(g:Group) WHERE u.domain <> g.domain RETURN u,g
```

---

## 5. Linux-Based Tooling (External / Black-Box Enumeration)

### 5.1 Impacket Suite
```bash
# Enumerate domain trusts via LDAP
python3 windapsearch.py -d target.local -u 'user@target.local' -p 'Password1' --trusts

# lookupsid.py — brute SIDs across a trusted domain
lookupsid.py target.local/user:password@<DC_IP>

# GetADUsers.py — enumerate users (cross-check across trusted domains)
GetADUsers.py target.local/user:password -all -dc-ip <DC_IP>

# secretsdump for trust keys (post-DA, used to forge inter-realm TGTs)
secretsdump.py -just-dc target.local/user:password@<DC_IP>
```

### 5.2 `ldapsearch`
```bash
# Anonymous / authenticated bind to enumerate trustedDomain objects
ldapsearch -x -H ldap://<DC_IP> -D "user@target.local" -w 'Password1' \
  -b "CN=System,DC=target,DC=local" "(objectClass=trustedDomain)" \
  trustPartner trustDirection trustType trustAttributes securityIdentifier

# Full forest-wide search using Global Catalog (port 3268)
ldapsearch -x -H ldap://<DC_IP>:3268 -D "user@target.local" -w 'Password1' \
  -b "DC=target,DC=local" "(objectClass=trustedDomain)"
```

### 5.3 `rpcclient` (SMB/RPC, classic but reliable)
```bash
rpcclient -U "user%password" <DC_IP>
rpcclient $> enumdomtrusts
rpcclient $> lsaenumtrustdom
rpcclient $> querydominfo
```

### 5.4 `enum4linux` / `enum4linux-ng`
```bash
enum4linux -a <DC_IP>
enum4linux-ng -A <DC_IP> -oA output

# Look specifically for trust-related output in the "Domain/Trust" section
```

### 5.5 `smbclient` / `net rpc` (Samba tools)
```bash
net rpc trustdom list -S <DC_IP> -U user%password
net rpc lsaquery -S <DC_IP> -U user%password
```

### 5.6 `CrackMapExec` / `NetExec`
```bash
# Enumerate trusts (recent NetExec versions)
nxc ldap <DC_IP> -u user -p password --trusted-for-delegation
nxc smb <DC_IP> -u user -p password --shares

# Pull domain info and pass-the-hash to enumerate across trust boundary
nxc smb <target_domain_DC_IP> -u user -H <NTLM_hash> -d target.local
```

