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

## 1. Trust Fundamentals

### 1.1 What Is a Trust?
An AD trust is a configured relationship between two domains/forests that allows users in one domain to authenticate and be authorized to resources in another. Trusts are foundational to multi-domain and multi-forest enterprise environments and are a primary lateral-movement / privilege-escalation vector.

### 1.2 Trust Direction
| Direction | Meaning |
|---|---|
| **One-way** | Users in Domain B can access resources in Domain A, but not vice versa. The "trusting" domain extends trust to the "trusted" domain. |
| **Two-way (bidirectional)** | Users in both domains can access resources in each other's domain. |

> Rule of thumb: trust flows the *opposite* direction of access. If Domain A trusts Domain B, users *from* B can access resources *in* A.

### 1.3 Trust Transitivity
| Type | Meaning |
|---|---|
| **Transitive** | Trust extends beyond the two directly-linked domains (e.g., A trusts B, B trusts C → A trusts C). Default within a forest. |
| **Non-transitive** | Trust applies only to the two explicitly linked domains. Default for external trusts. |

### 1.4 Trust Types

| Trust Type | Transitivity | Direction | Description |
|---|---|---|---|
| **Parent-Child** | Transitive | Two-way | Automatically created between a parent domain and a new child domain in the same forest. |
| **Tree-Root** | Transitive | Two-way | Automatically created when a new domain tree is added to a forest. |
| **External** | Non-transitive | One-way or two-way | Manually created between two domains in different forests, or between an NT4 domain and an AD domain. Uses SID filtering by default. |
| **Forest** | Transitive | One-way or two-way | Connects two forest root domains, allowing all domains in both forests to trust each other. |
| **Shortcut** | Transitive | One-way or two-way | Manually created to shorten the authentication path between two domains in a large/complex forest (improves logon performance). |
| **Realm** | Transitive or Non-transitive | One-way or two-way | Connects an AD domain to a non-Windows Kerberos realm (e.g., MIT Kerberos/Unix). |

### 1.5 Key Security Attributes
| Attribute | Description |
|---|---|
| **SID Filtering** | Security control on external/forest trusts that strips SIDs not belonging to the trusted domain from a Kerberos ticket's SID history, mitigating SID-History injection abuse. Often disabled or weakened for legacy/migration reasons (`quarantine=No`). |
| **Selective Authentication** | When enabled, requires explicit "Allowed to Authenticate" permissions on target computer objects, instead of allowing trust-wide authentication. |
| **SID History** | Attribute (`sIDHistory`) that can carry over a user's SID from a migrated domain — can be abused for privilege escalation if SID filtering is disabled. |
| **TRUST_ATTRIBUTES (trustAttributes)** | Bitmask flags stored on the `trustedDomain` object describing trust properties (see §6.3). |
| **TGT Delegation across trusts** | Cross-trust ticket flow that can be abused (e.g., Kerberoasting, "trust ticket" forging, golden/diamond tickets when trust keys are compromised). |

---

## 2. Native Windows Built-in Tools (No Extra Install)

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

### 2.4 `dsquery` / `dsget` (legacy but still present)
```cmd
dsquery * "cn=System,dc=target,dc=local" -filter "(objectClass=trustedDomain)" -attr *
```

### 2.5 ADSI / `[adsi]` via PowerShell (no RSAT required)
```powershell
$domain = [System.DirectoryServices.ActiveDirectory.Domain]::GetCurrentDomain()
$domain.GetAllTrustRelationships()

# Or target a specific domain
$context = New-Object System.DirectoryServices.ActiveDirectory.DirectoryContext('Domain','target.local')
$d = [System.DirectoryServices.ActiveDirectory.Domain]::GetDomain($context)
$d.GetAllTrustRelationships()

# Forest trust enumeration
$forest = [System.DirectoryServices.ActiveDirectory.Forest]::GetCurrentForest()
$forest.GetAllTrustRelationships()
$forest.Domains
```

### 2.6 Raw LDAP Query (System container)
```powershell
([adsisearcher]"(objectClass=trustedDomain)").FindAll() | ForEach-Object { $_.Properties }
```
Trust objects live at: `CN=System,DC=<domain>,DC=<tld>` with objectClass `trustedDomain`.

---

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

### 5.7 Kerbrute (validate cross-domain accounts via trust)
```bash
kerbrute userenum -d target.local --dc <DC_IP> users.txt
```

---

## 6. Manual LDAP Attribute Reference

### 6.1 Trust Object Location
```
CN=System,DC=<domain-component>,...
objectClass: trustedDomain
```

### 6.2 Key Attributes on `trustedDomain` Objects
| Attribute | Description |
|---|---|
| `trustPartner` | FQDN of the trusted/trusting domain |
| `flatName` | NetBIOS name of the partner domain |
| `trustDirection` | 1=Inbound, 2=Outbound, 3=Bidirectional |
| `trustType` | 1=Downlevel(NT4), 2=Windows AD, 3=MIT/Kerberos realm, 4=DCE |
| `trustAttributes` | Bitmask flags (see below) |
| `securityIdentifier` | SID of the trusted domain |
| `whenCreated` / `whenChanged` | Timestamps |

### 6.3 `trustAttributes` Bitmask Values
| Value (hex) | Flag | Meaning |
|---|---|---|
| `0x1` | NON_TRANSITIVE | Trust is non-transitive |
| `0x2` | UPLEVEL_ONLY | Only Windows 2000+ clients can use trust |
| `0x4` | QUARANTINED_DOMAIN (SID Filtering) | SID filtering is enabled |
| `0x8` | FOREST_TRANSITIVE | Forest trust |
| `0x10` | CROSS_ORGANIZATION | External trust to org outside the forest (no SID history allowed in claims) |
| `0x20` | WITHIN_FOREST | Trust between domains in the same forest |
| `0x40` | TREAT_AS_EXTERNAL | Forest trust treated as external (disables transitive forest-wide auth) |
| `0x200` | CROSS_ORGANIZATION_NO_TGT_DELEGATION | Restricts TGT delegation |
| `0x400` | PIM_TRUST | Privileged Identity Management trust |
| `0x800` | CROSS_ORGANIZATION_ENABLE_TGT_DELEGATION | Enables S4U2Proxy-style TGT delegation across cross-org trust |

### 6.4 Checking Selective Authentication / SID Filtering State
```powershell
# PowerShell AD module
Get-ADTrust -Identity target.local -Properties * | Select-Object Name, Direction, ForestTransitive, SIDFilteringQuarantined, SelectiveAuthentication, TGTDelegation
```
```cmd
:: Native command-line view
netdom trust target.local /domain:current.local /verify /verbose
```

---

## 7. Domain & Forest Functional Level / Topology Recon (Context for Trusts)

```powershell
# Forest-wide topology, schema, GC servers, root domain
Get-ADForest | Format-List Name, RootDomain, Domains, GlobalCatalogs, ForestMode, SchemaMaster

# Domain functional level
Get-ADDomain | Select-Object DomainMode, PDCEmulator, InfrastructureMaster

# List all DCs in a trusted domain (needed to target queries cross-domain)
Get-ADDomainController -Filter * -Server target.local

# DNS-based forest/domain discovery (often reveals trust topology via SRV records)
nslookup -type=SRV _ldap._tcp.dc._msdcs.target.local
nslookup -type=SRV _kerberos._tcp.target.local
```

---

## 8. Putting It Together — Suggested Enumeration Workflow

1. **Establish foothold** — obtain any valid domain credential (even low-privileged) or unauthenticated SMB/LDAP access.
2. **Map current domain trusts**:
   - `nltest /domain_trusts /all_trusts /v`
   - `Get-ADTrust -Filter *`
   - `Get-DomainTrust` (PowerView)
3. **Recursively map the full forest/trust graph**:
   - `Get-DomainTrustMapping` (PowerView)
   - `bloodhound-python -c Trusts` or `SharpHound.exe -c Trusts`
4. **Visualize in BloodHound** — identify which domains are reachable, in which direction, and whether transitive.
5. **Check trust security posture** per link:
   - SID filtering (`QUARANTINED_DOMAIN`) status
   - Selective Authentication enabled/disabled
   - TGT delegation flags
6. **Enumerate foreign principals**:
   - `Get-DomainForeignUser`, `Get-DomainForeignGroupMember` (PowerView)
   - BloodHound foreign membership edges
7. **Identify cross-trust privilege escalation paths** (informational — exploitation is out of scope of pure enumeration):
   - SID History abuse potential (if filtering disabled)
   - Unconstrained delegation hosts that span trust boundaries
   - Kerberoastable accounts visible across trust
8. **Document findings**: trust direction, transitivity, type, SID filtering state, and any foreign privileged memberships — this becomes the core of the AD trust risk assessment in your report.

---

## 9. Quick Reference — Command Cheat Table

| Goal | Tool | Command |
|---|---|---|
| List trusts (local domain) | nltest | `nltest /domain_trusts /all_trusts /v` |
| List trusts (PS AD module) | PowerShell | `Get-ADTrust -Filter * -Properties *` |
| List trusts | netdom | `netdom query /domain:target.local trust` |
| Recursive trust map | PowerView | `Get-DomainTrustMapping` |
| Forest trust info | PowerView | `Get-ForestTrust` |
| Graph visualization | BloodHound | `SharpHound.exe -c Trusts` → import to BloodHound GUI |
| LDAP raw query | ldapsearch | `ldapsearch -x -b "CN=System,DC=..." "(objectClass=trustedDomain)"` |
| RPC enum | rpcclient | `rpcclient -U user%pass <IP> -c enumdomtrusts` |
| SMB/RPC enum (Samba) | net rpc | `net rpc trustdom list -S <IP> -U user%pass` |
| Cross-platform recon | enum4linux-ng | `enum4linux-ng -A <IP>` |
| Cross-domain auth check | CrackMapExec/NetExec | `nxc smb <IP> -u user -p pass -d target.local` |

---

## 10. Notes & Caveats

- Always confirm you have **explicit written authorization** before enumerating or testing trust relationships in any environment you do not own.
- Trust enumeration is typically **passive/low-noise** for read operations (LDAP queries, `nltest`), but tools like SharpHound generate significant traffic and may trigger EDR/SIEM alerts — scope and pace accordingly.
- Trust attribute values and tool output can differ slightly between OS/AD schema versions (Server 2008 R2 → 2022) — always cross-validate with at least two tools when in doubt.
- SID filtering and selective authentication settings are **the most security-relevant facts to extract** from trust enumeration, since they determine real-world exploitability of a given trust edge.

---

*Prepared as a technical reference for authorized security assessments of Active Directory environments.*
