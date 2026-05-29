# List Process
```powershell
ps aux | grep root
```

# List Current Terminal-Attached Processes
```powershell
ps au
```

# SSH Directory
```powershell
ls -l ~/.ssh
```

# Bash History
```powershell
history
```

# Sudo - List User's Privileges
```powershell
sudo -l
```

# Critical Files
```powershell
find / -name '*conf*' 2>/dev/null
```

# Shadow / Passwd
```powershell
ls -l /etc/shadow   -rw-r----- 1 root shadow 1643 Apr  6 18:20 /etc/shadow  ( Read or Write == CRITICAL )
ls -l /etc/passwd   -rw-r----- 1 root shadow 1643 Apr  6 18:20 /etc/shadow  ( Read HASH or Write == CRITICAL )
```

# Cron Jobs
```powershell
ls -la /etc/cron.*
```

# File Systems & Additional Drives
```powershell
lsblk
```

# Find Writable Directories
```powershell
find / -path /proc -prune -o -type d -perm -o+w 2>/dev/null
```

# Find Writable Files
```powershell
find / -path /proc -prune -o -type f -perm -o+w 2>/dev/null
```

# Search Pattern Credentials
```powershell
grep -rn "Password" / 2>/dev/null
```
