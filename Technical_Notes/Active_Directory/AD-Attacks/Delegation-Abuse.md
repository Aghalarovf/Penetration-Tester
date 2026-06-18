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

### 2.2 Unconstrained Delegation
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

### 2.3 
