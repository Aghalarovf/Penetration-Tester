# Get current username
```powershell
echo %USERNAME% || whoami
$env:username
```

# List user privilege
```powershell
whoami /priv
whoami /groups
```

# List all users
```powershell
net user
whoami /all
Get-LocalUser | ft Name,Enabled,LastLogon
Get-ChildItem C:\Users -Force | select Name
```

# List logon requirements; useable for bruteforcing
```powershell
$env:usernadsc
net accounts
```

# Get details about a user
```powershell
net user administrator
net user admin
net user %USERNAME%
```

# Entry Types
```powershell
Get-WinEvent -LogName Security -FilterXPath "*[System[EventID=4624]]" -MaxEvents 50 | ForEach-Object {
    $xml = [xml]$_.ToXml()
    $TargetUserName = ($xml.Event.EventData.Data | Where-Object {$_.Name -eq "TargetUserName"})."#text"
    $LogonType = ($xml.Event.EventData.Data | Where-Object {$_.Name -eq "LogonType"})."#text"
    $IpAddress = ($xml.Event.EventData.Data | Where-Object {$_.Name -eq "IpAddress"})."#text"
    
    # Sistem hesablarını süzgəcdən keçiririk ki, yalnız real istifadəçilər qalsın
    if ($TargetUserName -notmatch "SYSTEM|ANONYMOUS|LOCAL SERVICE|NETWORK SERVICE|Window$") {
        [PSCustomObject]@{
            "Time"             = $_.TimeCreated
            "Username"    = $TargetUserName
            "Entry Type"  = $LogonType
            "IP Address"         = $IpAddress
        }
    }
} | Format-Table -AutoSize
```

# List all local groups
```powershell
net localgroup
Get-LocalGroup | ft Name
```

# Get details about a group
```powershell
net localgroup administrators
Get-LocalGroupMember Administrators | ft Name, PrincipalSource
Get-LocalGroupMember Administrateurs | ft Name, PrincipalSource
```

# Get Domain Controllers
```powershell
nltest /DCLIST:DomainName
nltest /DCNAME:DomainName
nltest /DSGETDC:DomainName
```
