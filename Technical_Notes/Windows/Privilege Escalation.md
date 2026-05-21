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
