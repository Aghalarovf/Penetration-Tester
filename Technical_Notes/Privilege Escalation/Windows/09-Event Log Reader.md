# Group Enumeration
```powershell
net localgroup "Event Log Readers"
```

# Searching Security Logs Using wevtutil
```powershell
wevtutil qe Security /rd:true /f:text | Select-String "/user"
```

# Passing Credentials to wevtutil
```powershell
wevtutil qe Security /rd:true /f:text /r:share01 /u:julie.clay /p:Welcome1 | findstr "/user"
```

# Searching Security Logs Using Get-WinEvent
```powershell
Get-WinEvent -LogName security | where { $_.ID -eq 4688 -and $_.Properties[8].Value -like '*/user*'} | Select-Object @{name='CommandLine';expression={ $_.Properties[8].Value }}
```
