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
mysql -u ubuntu 
