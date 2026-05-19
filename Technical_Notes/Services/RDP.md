# Main Connect
```powershell
xfreerdp /u:Fiona /p:'48Ns72!bns74@S84NNNSl' /d:inlanefreight.htb /v:10.129.203.10 /cert:ignore \
/dynamic-resolution "/drive:Labaratory,/home/sako/Labaratory"
```

```
==============================
RDP (Remote Desktop Protocol)
Cheat Sheet – Professional
==============================

PORT:
3389/TCP – RDP (default)
Custom ports common

Transport:
TCP / UDP

Purpose:
Remote GUI access to Windows systems

--------------------------------
1. BASIC ENUMERATION
--------------------------------

nmap -p3389 -sV <IP>
nmap -p3389 --script rdp-* <IP>
nmap -p3389 --script rdp-ntlm-info <IP>

rdp-sec-check.pl <IP>
nmap --script rdp-ntlm-info -p3389 <IP>

# Valid user check (NLA off)
xfreerdp /v:<IP> /u:test /p:test
remmina -c rdp://<IP>

hydra -L users.txt -P passwords.txt rdp://<IP>
crowbar -b rdp -s <IP>/32 -u user -C passwords.txt

xfreerdp /v:<IP> /u:user /pth:<NTLM_HASH>
xfreerdp /v:<IP> /u:user /p:password
xfreerdp /v:<IP> /u:domain\\user /p:password
xfreerdp /v:<IP> /u:user /p:pass +clipboard +drives
xfreerdp /v:<IP> /u:<USER> /p:'<PASSWORD>' /compression /gfx /network:modem /bpp:16 /themes /fonts /wallpaper

xfreerdp /u:Fiona /p:'48Ns72!bns74@S84NNNSl' /d:inlanefreight.htb /v:10.129.203.10 /cert:ignore \
/dynamic-resolution "/drive:Labaratory,/home/sako/Labaratory"


nmap --script rdp-vuln-ms12-020 -p3389 <IP>
searchsploit rdp

query user
tscon <ID> /dest:console

Event Viewer:
Security Logs
Microsoft-Windows-TerminalServices-*

nmap
xfreerdp
hydra
crowbar
metasploit
searchsploit

FULL RDP Check

#!/bin/bash
# rdp_enum.sh - Comprehensive RDP Enumeration Script
# Authorized Pentest Tool - Linux Target Enumeration

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

echo -e "${BLUE}[*] RDP Comprehensive Enumeration Started${NC}"

# 1. RDP Service Status
echo -e "\n${YELLOW}[+] 1. RDP Service Status${NC}"
ss -tlnp | grep :3389 || netstat -tlnp | grep :3389
systemctl status xrdp 2>/dev/null || echo "xrdp service not found"
ps aux | grep -E "(xrdp|rdesktop|rdp)" | grep -v grep

# 2. RDP Configuration Files
echo -e "\n${YELLOW}[+] 2. RDP Configuration Files${NC}"
find /etc -name "*rdp*" -o -name "*xrdp*" 2>/dev/null
find /etc/x11 -name "*rdp*" 2>/dev/null
cat /etc/xrdp/xrdp.ini 2>/dev/null || echo "xrdp.ini not found"
ls -la /etc/xrdp/ 2>/dev/null

# 3. RDP Users & Groups
echo -e "\n${YELLOW}[+] 3. RDP Users & Groups${NC}"
getent group rdp 2>/dev/null || echo "rdp group not found"
getent passwd | grep -i rdp
id -Gn | grep -i rdp
cat /etc/group | grep -i rdp

# 4. Listening Ports & Processes
echo -e "\n${YELLOW}[+] 4. Network & Processes${NC}"
lsof -i :3389 2>/dev/null || ss -tulpn | grep 3389
netstat -tulpn | grep 3389 2>/dev/null
ps aux | grep -E "xrdp|rdp|3389" | grep -v grep

# 5. Log Files Analysis
echo -e "\n${YELLOW}[+] 5. RDP Log Files${NC}"
find /var/log -name "*rdp*" -o -name "*xrdp*" 2>/dev/null
tail -50 /var/log/xrdp.log 2>/dev/null || echo "xrdp.log not found"
tail -50 /var/log/xrdp-sesman.log 2>/dev/null
journalctl -u xrdp -n 50 2>/dev/null

# 6. RDP Sessions & Connections
echo -e "\n${YELLOW}[+] 6. Active RDP Sessions${NC}"
w | grep rdp 2>/dev/null
who | grep -i rdp
loginctl list-sessions | grep -i rdp 2>/dev/null

# 7. Firewall & Network Config
echo -e "\n${YELLOW}[+] 7. Firewall Rules${NC}"
iptables -L -n | grep 3389
iptables -t nat -L -n | grep 3389
ufw status 2>/dev/null | grep 3389
firewalld-cmd --list-all 2>/dev/null | grep 3389

# 8. Installed RDP Packages
echo -e "\n${YELLOW}[+] 8. Installed RDP Packages${NC}"
dpkg -l | grep -E "(xrdp|rdp)" 2>/dev/null
rpm -qa | grep -E "(xrdp|rdp)" 2>/dev/null
yum list installed | grep rdp 2>/dev/null

# 9. User Home Directories & RDP Configs
echo -e "\n${YELLOW}[+] 9. User RDP Configs${NC}"
find /home -name "*rdp*" -o -name "*.rdp" 2>/dev/null 2>/dev/null
find /root -name "*rdp*" 2>/dev/null 2>/dev/null
find /tmp -name "*rdp*" 2>/dev/null

# 10. Credential Files & History
echo -e "\n${YELLOW}[+] 10. Credential Artifacts${NC}"
find /home /root /var -name "*password*" -o -name "*cred*" 2>/dev/null 2>/dev/null
grep -r "3389" /home/*/.ssh/ 2>/dev/null
cat /home/*/.bash_history | grep -i rdp 2>/dev/null

# 11. xrdp Specific Enumeration
echo -e "\n${YELLOW}[+] 11. xrdp Specific${NC}"
ls -la /var/run/xrdp/ 2>/dev/null
ls -la /var/lib/xrdp/ 2>/dev/null
cat /etc/pam.d/xrdp-sesman 2>/dev/null

# 12. Network Connections
echo -e "\n${YELLOW}[+] 12. RDP Network Connections${NC}"
netstat -an | grep 3389 2>/dev/null
ss -an | grep 3389

# 13. Summary
echo -e "\n${GREEN}[+] SUMMARY${NC}"
echo "RDP Port Status: $(ss -tlnp | grep :3389 >/dev/null && echo 'OPEN' || echo 'CLOSED')"
echo "xrdp Service: $(systemctl is-active xrdp 2>/dev/null || echo 'NOT FOUND')"
echo "RDP Users: $(getent group rdp >/dev/null && echo 'FOUND' || echo 'NONE')"
echo "Log Files: $(find /var/log -name '*rdp*' | wc -l) files found"

# 14. Export Results
RESULTS_FILE="rdp_enum_$(date +%Y%m%d_%H%M%S).txt"
{
    echo "=== RDP Enumeration - $(date) ===" >> $RESULTS_FILE
    ss -tlnp | grep 3389 >> $RESULTS_FILE 2>/dev/null
    systemctl status xrdp >> $RESULTS_FILE 2>/dev/null
    cat /etc/xrdp/xrdp.ini >> $RESULTS_FILE 2>/dev/null
    tail -100 /var/log/xrdp* >> $RESULTS_FILE 2>/dev/null
} 2>/dev/null

echo -e "\n${GREEN}[+] Results saved: $RESULTS_FILE${NC}"

echo -e "${BLUE}[*] RDP Enumeration Complete!${NC}"
```


