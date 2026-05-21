# Enumerate antivirus
```powershell
WMIC /Node:localhost /Namespace:\\root\SecurityCenter2 Path AntivirusProduct Get displayName
```

# AppLocker
```powershell
Get-MpComputerStatus
Get-AppLockerPolicy -Effective | select -ExpandProperty RuleCollections

# Test Applocker Policy
Get-AppLockerPolicy -Local | Test-AppLockerPolicy -path C:\Windows\System32\cmd.exe -User Everyone
```

# AppLocker Show Executable Files
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
