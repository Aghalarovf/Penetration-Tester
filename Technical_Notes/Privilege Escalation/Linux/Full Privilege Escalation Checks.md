# Linux Privilege Escalation Checklist

---

## 1. Sudo

```bash
sudo -l
sudo -V | head -n1
```

- [Sudo Abuse (Metod 1)](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/05-Sudo%20Rights.md)
- [Sudo Abuse (Metod 2)](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/15-SUDO.md)
- [Shared Libraries](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/12-Shared%20Libraries.md)

---

## 2. Qrup İmtiyazları

```bash
id
```

- [Group Abuse](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/06-Privileged%20Groups.md)

---

## 3. Tarix (History)

```bash
history
```

---

## 4. PATH Dəyişəni

```bash
echo $PATH
```

- [Path Abuse](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/02-Path%20Abuse.md)

---

## 5. Məhdud Shell Mühiti

```bash
echo $SHELL
echo $0
```

- [Restricted Shell](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/03-Restricted%20Shell.md)

---

## 6. SUID / SGID Fayllar

```bash
find / -user root -perm -2000 -writable 2>/dev/null
find / -user root -perm -4000  2>/dev/null
find / -user root -perm -6000  2>/dev/null
```

- [SUID/SGID](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/04-Special%20Permissions.md)

---

## 7. Capabilities

```bash
getcap -r / 2>/dev/null | grep --color=always -E \
  'cap_sys_admin|cap_setuid|cap_setgid|cap_dac_override|cap_sys_ptrace|cap_sys_module|cap_sys_chroot|$'
```

- [Capabilities](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/07-Capabilities.md)

---

## 8. Kritik Fayllar və Konfiqurasiyalar

```bash
# Shell skriptlər
find / -type f -name "*.sh" 2>/dev/null | grep -v "src\|snap\|share"

# SSH açarları
ls -l ~/.ssh

# Shadow və Passwd
ls -l /etc/shadow
ls -l /etc/passwd

# Cron tapşırıqları
cat /etc/crontab

# Yazıla bilən qovluqlar və fayllar
find / -path /proc -prune -o -type d -perm -o+w 2>/dev/null
find / -path /proc -prune -o -type f -perm -o+w 2>/dev/null

# Şifrə axtarışı
grep -rn "Password" / 2>/dev/null
```

- [Critical Files and Configurations](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/01-Critical%20Files.md)

---

## 9. Screen

```bash
screen -v
```

- [Screen Abuse](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/08-Vulnerability%20Services.md)

---

## 10. Disk Bölmələri

```bash
lsblk
```

---

## 11. Logrotate

```bash
logrotate --help
find /var/log -type f -writable 2>/dev/null
```

- [Logrotate Abuse](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/09-Logrotate.md)

---

## 12. NFS Paylaşımları

```bash
cat /etc/exports
```

- [NFS Abuse](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/10-Network%20Shares.md)

---

## 13. Kernel Exploit

```bash
./linux-exploit-suggester.sh

# Metasploit ilə
msf > use post/multi/recon/local_exploit_suggester
```

- [Kernel Exploit](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/11-Kernel%20Exploit.md)

---

## 14. Shared Library / Object Hijacking

```bash
./linpeas.sh | grep -A5 "Shared Library"
```

- [Shared Library Abuse](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/12-Shared%20Libraries.md)
- [Shared Object Hijacking](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/13-Shared%20Object%20Hijacking.md)

---

## 15. DirtyPipe

> **Şərt:** `5.8 <= kernel < 5.17`

```bash
uname -r
```

- [DirtyPipe](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/17-DirtyPipe.md)

---

## 16. Polkit (CVE-2021-4034)

> **Şərt:** `pkexec version <= 0.120` — Təhlükəli

```bash
pkexec --version
```

- [Polkit](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/16-Polkit.md)

---

## 17. Sudoers Faylı

```bash
sudo cat /etc/sudoers | grep -v "#" | sed -r '/^\s*$/d'
```

---

## 18. Şəbəkə Etimadnaməsi Tutma

```bash
python3 pcredz.py -i eth0
```

---

## 19. Python Kitabxana Hijacking

- [Python Library Abuse](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Linux/14-Python%20Library%20Hijacking.md)
