# Wordpress Enumeration
```powershell
# Version
curl -s http://blog.inlanefreight.local | grep WordPress

# Login Page
curl -s http://blog.inlanefreight.local/wp-login.php

# Plugins
curl -s http://blog.inlanefreight.local/wp-content/plugins/

# Themes
curl -s http://blog.inlanefreight.local/wp-content/themes/
```

# WPScan
```powershell
sudo gem install wpscan
sudo wpscan --update

wpscan --url https://target.com --api-token YOUR_TOKEN_HERE
echo "cli_options:\n  api_token: YOUR_TOKEN" > ~/.wpscan/scan.yml
```

# Detection Mode
```powershell
--detection-mode passive
--detection-mode aggressive
--detection-mode mixed
```

# Version and Core Enumeration
```powershell
wpscan --url https://target.com --detection-mode aggressive
```

# Plugin Enumeration
```powershell
wpscan --url https://target.com --enumerate vp

wpscan --url https://target.com --enumerate ap

wpscan --url https://target.com --enumerate p

wpscan --url https://target.com --enumerate ap --plugins-detection aggressive
```

# Theme Enumeration
```powershell
wpscan --url https://target.com --enumerate vt

wpscan --url https://target.com --enumerate at

wpscan --url https://target.com --enumerate t
```

# User Enumeration
```powershell
wpscan --url https://target.com --enumerate u

wpscan --url https://target.com --enumerate u[1-50]
```

# Brute Force
```powershell
# Tək user, wordlist ilə
wpscan --url https://target.com \
       --username admin \
       --passwords rockyou.txt \
       --threads 50

# User listindən
wpscan --url https://target.com \
       --usernames users.txt \
       --passwords /usr/share/wordlists/rockyou.txt

# Login page ilə
wpscan --url https://target.com \
       --username admin \
       --passwords rockyou.txt \
       --password-attack wp-login
```

# Full Enumeration
```powershell
# MƏRHƏLƏ 1: Kəşf
wpscan --url https://target.com \
       --detection-mode passive \
       --api-token TOKEN

# MƏRHƏLƏ 2: Tam enumeration
wpscan --url https://target.com \
       --enumerate ap,at,u,cb,dbe \
       --detection-mode aggressive \
       --api-token TOKEN \
       -o enum_results.json -f json

# MƏRHƏLƏ 3: İstifadəçi tapıldıqdan sonra brute force
wpscan --url https://target.com \
       --usernames found_users.txt \
       --passwords /usr/share/wordlists/rockyou.txt \
       --password-attack xmlrpc \
       --threads 20 \
       -o bruteforce_results.txt
```
