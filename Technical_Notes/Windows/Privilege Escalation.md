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
