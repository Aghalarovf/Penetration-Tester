# List Process
```powershell
ps aux | grep root

find /proc -name cmdline -exec cat {} \; 2>/dev/null | tr " " "\n"
```

# List Services
```powershell
apt list --installed | tr "/" " " | cut -d" " -f1,3 | sed 's/[0-9]://g' | tee -a installed_pkgs.list

for i in $(curl -s https://gtfobins.org/api.json | jq -r '.executables | keys[]'); do if grep -q "$i" installed_pkgs.list; then echo "Check for GTFO: $i";fi; done
```

# SSH Files
```powershell
ls -l ~/.ssh
```

# Bash History
```powershell
history
find / -type f \( -name *_hist -o -name *_history \) -exec ls -l {} \; 2>/dev/null
```

# Critical Files and Pattern
```powershell
find / -type f \( -name *.conf -o -name *.config \) -exec ls -l {} \; 2>/dev/null

grep 'DB_USER\|DB_PASSWORD' wp-config.php
find / ! -path "*/proc/*" -iname "*config*" -type f 2>/dev/null

ls -l /etc/shadow   -rw-r----- 1 root shadow 1643 Apr  6 18:20 /etc/shadow  ( Read or Write == CRITICAL )
ls -l /etc/passwd   -rw-r----- 1 root shadow 1643 Apr  6 18:20 /etc/shadow  ( Read HASH or Write == CRITICAL )

grep -rn "Password" / 2>/dev/null
```

# Find Writable Files and Directory
```powershell
find / -path /proc -prune -o -type d -perm -o+w 2>/dev/null

find / -path /proc -prune -o -type f -perm -o+w 2>/dev/null
```
