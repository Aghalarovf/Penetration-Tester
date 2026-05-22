# Process
```powershell
tasklist /v
net start
sc query
Get-Service
Get-Process
Get-WmiObject -Query "Select * from Win32_Process" | where {$_.Name -notlike "svchost*"} | Select Name, Handle, @{Label="Owner";Expression={$_.GetOwner().User}} | ft -AutoSize
```

# Run as SYSTEM
```powershell
tasklist /v /fi "username eq system"
```

# Installed Programs
```powershell
Get-ChildItem 'C:\Program Files', 'C:\Program Files (x86)' | ft Parent,Name,LastWriteTime
Get-ChildItem -path Registry::HKEY_LOCAL_MACHINE\SOFTWARE | ft Name
```

# List Services
```powershell
net start
wmic service list brief
tasklist /SVC
```

# Enumerate Scheduled Tasks
```powershell
schtasks /query /fo LIST 2>nul | findstr TaskName
schtasks /query /fo LIST /v > schtasks.txt; cat schtask.txt | grep "SYSTEM\|Task To Run" | grep -B 1 SYSTEM
Get-ScheduledTask | where {$_.TaskPath -notlike "\Microsoft*"} | ft TaskName,TaskPath,State
```

# Startup Tasks
```powershell
wmic startup get caption,command
reg query HKLM\Software\Microsoft\Windows\CurrentVersion\R
reg query HKCU\Software\Microsoft\Windows\CurrentVersion\Run
reg query HKCU\Software\Microsoft\Windows\CurrentVersion\RunOnce
dir "C:\Documents and Settings\All Users\Start Menu\Programs\Startup"
dir "C:\Documents and Settings\%username%\Start Menu\Programs\Startup"
```

# Pipe Files
```powershell
pipelist.exe /accepteula

# With Powershell
gci \\.\pipe\

# Konkret pipe
accesschk.exe /accepteula \\.\Pipe\lsass -v

# Hamısını yoxla
accesschk.exe /accepteula \pipe\* -v

# Yalnız yazılabilənləri göstər
accesschk.exe -w \pipe\* -v

FILE_READ_DATA     Pipe-dan oxu
FILE_WRITE_DATA    Pipe-a yaz
FILE_ALL_ACCESS    Tam giriş (təhlükəli!)
READ_CONTROL       DACL-ı oxu
SYNCHRONIZE        Sinxronizasiya
```
