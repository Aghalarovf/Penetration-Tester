# Checks
```powershell
sudo -l
sudo -V | head -n1
```

[SUDO Abuse 1](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/05-Sudo%20Rights.md)
[SUDO Abuse 2](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/15-SUDO.md)
(Shared Libraries)(https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/12-Shared%20Libraries.md)
---

```powershell
id
```
(https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/06-Privileged%20Groups.md)[Group Abuse]
---

```powershell
history
```
[]()
---

```powershell
echo $PATH
```
[Path Abuse](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/02-Path%20Abuse.md)
---

```powershell
echo $SHELL
echo $0
```
[Restricted Environment](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/03-Restricted%20Shell.md)
---

```powershell
find / -user root -perm -2000 -writable 2>/dev/null
find / -user root -perm -4000  2>/dev/null
find / -user root -perm -6000  2>/dev/null
```
[SUID/SGID](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/04-Special%20Permissions.md)
---


```powershell
getcap -r / 2>/dev/null | grep --color=always -E 'cap_sys_admin|cap_setuid|cap_setgid|cap_dac_override|cap_sys_ptrace|cap_sys_module|cap_sys_chroot|$'
```
[https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/07-Capabilities.md](Capabilities)
---

```powershell
find / -type f -name "*.sh" 2>/dev/null | grep -v "src\|snap\|share"
ls -l ~/.ssh

ls -l /etc/shadow
ls -l /etc/passwd

cat /etc/crontab

find / -path /proc -prune -o -type d -perm -o+w 2>/dev/null
find / -path /proc -prune -o -type f -perm -o+w 2>/dev/null

grep -rn "Password" / 2>/dev/null
```
[https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/01-Critical%20Files.md](Critical Files and Configures)
---

```powershell
screen -v
```
[https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/08-Vulnerability%20Services.md](Screen Abuse)
---

```powershell
lsblk
```
[]()
---

```powershell
logrotate --help
find /var/log -type f -writable 2>/dev/null
```
[https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/09-Logrotate.md](Logrotate Abuse)
---

```powershell
cat /etc/exports
```
[https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/10-Network%20Shares.md](NFS Abuse)
---

```powershell
./linux-exploit-suggester.sh
msf exploit(multi/handler) > use post/multi/recon/local_exploit_suggester
```
[https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/11-Kernel%20Exploit.md](Kernel Exploit)
---

```powershell
./linpeas.sh | grep -A5 "Shared Library"
```
[https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/12-Shared%20Libraries.md](Shared Library Abuse)
[https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/13-Shared%20Object%20Hijacking.md](Shared Object Hijacking)
---

```powershell
uname -r       5.8 <= version < 5.17
```
[https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/17-DirtyPipe.md](DirtyPipe)
---

```powershell
pkexec --version      0.120 > Dangerous
```
[https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/16-Polkit.md](Polkit)
---

```powershell
sudo cat /etc/sudoers | grep -v "#" | sed -r '/^\s*$/d'
```
---

```powershell
python3 pcredz.py -i eth0
```
[]()
---

```powershell
Python Library Abuse
```
[https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/14-Python%20Library%20Hijacking.md](Python Library Abuse)
```
---
