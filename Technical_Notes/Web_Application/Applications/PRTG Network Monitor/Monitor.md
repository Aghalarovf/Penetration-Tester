# Reconnaissance
```powershell
# Tam port skanı + servis versiyası
sudo nmap -sV -p- --open -T4 <TARGET_IP>

# Yalnız ümumi web portları
sudo nmap -sV -p 80,443,8080,8443 <TARGET_IP>

Indy httpd 17.x.xx.xxxx (Paessler PRTG bandwidth monitor)
```

# Version
```powershell
curl -s http://<TARGET_IP>:8080/index.htm \
  -A "Mozilla/5.0 (compatible; MSIE 7.01; Windows NT 5.0)" \
  | grep version
```

# Credential Hunting
```powershell
eyewitness --web -f targets.txt --timeout 10
# Default credentials avtomatik göstərilir: prtgadmin:prtgadmin
```

# Default Credentials
```powershell
prtgadmin:prtgadmin
prtgadmin:Password123
prtgadmin:prtg
```
