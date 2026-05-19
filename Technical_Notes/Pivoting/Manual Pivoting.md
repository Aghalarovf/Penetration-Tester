# PIVOT1
```powershell
sudo sysctl -w net.ipv4.ip_forward=1

# 1. Gələn paketlərin keçişinə (Forwarding) tam icazə ver
sudo iptables -A FORWARD -j ACCEPT

# 2. Cavab paketlərinin geri qayıtmasına icazə ver
sudo iptables -A FORWARD -m state --state ESTABLISHED,RELATED -j ACCEPT

# 3. Bütün çıxış interfeysləri üçün NAT-ı aktiv et
sudo iptables -t nat -A POSTROUTING -j MASQUERADE

sudo iptables -A FORWARD -p tcp --tcp-flags SYN,RST SYN -j TCPMSS --clamp-mss-to-pmtu
```

# Attacker
```
# 1. Hack The Box Fortresses/Pro Labs daxili şəbəkəsi üçün (10.10.14.0/23 bloku):
route add 10.10.14.0 mask 255.255.254.0 192.168.0.241

# 2. Ümumi Hack The Box Maşınları şəbəkəsi üçün (10.129.0.0/16 bloku):
route add 10.129.0.0 mask 255.255.0.0 192.168.0.241
```
