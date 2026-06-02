# Checks
```powershell
sudo -l
sudo -V | head -n1

id

history

echo $PATH

echo $SHELL
echo $0

find / -user root -perm -2000 -writable 2>/dev/null
find / -user root -perm -4000  2>/dev/null
find / -user root -perm -6000  2>/dev/null

getcap -r / 2>/dev/null | grep --color=always -E 'cap_sys_admin|cap_setuid|cap_setgid|cap_dac_override|cap_sys_ptrace|cap_sys_module|cap_sys_chroot|$'

find / -type f -name "*.sh" 2>/dev/null | grep -v "src\|snap\|share"
ls -l ~/.ssh

ls -l /etc/shadow
ls -l /etc/passwd

cat /etc/crontab

find / -path /proc -prune -o -type d -perm -o+w 2>/dev/null
find / -path /proc -prune -o -type f -perm -o+w 2>/dev/null

grep -rn "Password" / 2>/dev/null

screen -v

lsblk

logrotate --help
find /var/log -type f -writable 2>/dev/null

cat /etc/exports

./linux-exploit-suggester.sh

./linpeas.sh | grep -A5 "Shared Library"

uname -r       5.8 <= version < 5.17

pkexec --version      0.120 > Dangerous

sudo cat /etc/sudoers | grep -v "#" | sed -r '/^\s*$/d'

python3 pcredz.py -i eth0
```
