# Hydra

### Main Parameters

| Parametr        | Açıqlama                                          |
|-----------------|---------------------------------------------------|
| `-l USER`       | Tək istifadəçi adı                                |
| `-L FILE`       | İstifadəçi adları siyahısı (fayl)                 |
| `-p PASS`       | Tək şifrə                                         |
| `-P FILE`       | Şifrə siyahısı (fayl)                             |
| `-C FILE`       | `user:pass` formatında birləşmiş fayl             |
| `-t N`          | Paralel tapşırıq sayı (default: 16)               |
| `-T N`          | Ümumi paralel bağlantı sayı                       |
| `-s PORT`       | Xüsusi port nömrəsi                               |
| `-f`            | İlk tapılan cütdə dayan                           |
| `-F`            | Hər hədəf üçün ilk tapılanda dayan                |
| `-v / -V`       | Verbose / Hər cəhdi göstər                        |
| `-d`            | Debug modu                                        |
| `-o FILE`       | Nəticəni fayla yaz                                |
| `-R`            | Əvvəlki sessiyadan davam et                       |
| `-I`            | Əvvəlki sessiyanı sil, yenidən başla              |
| `-M FILE`       | Çoxlu hədəf (fayl)                                |
| `-w N`          | Cavab gözləmə vaxtı (saniyə)                      |
| `-W N`          | Bağlantılar arası gözləmə vaxtı                   |
| `-e nsr`        | `n`=boş şifrə, `s`=user=pass, `r`=tərsinə         |

---

### Protokol Nümunələri

#### 🌐 HTTP Form (POST)

```bash
hydra -L users.txt -P passwords.txt 192.168.1.1 \
  http-post-form "/login:username=^USER^&password=^PASS^:Invalid credentials"
```

#### 🌐 HTTP Form (GET)

```bash
hydra -L users.txt -P passwords.txt 192.168.1.1 \
  http-get-form "/login:user=^USER^&pass=^PASS^:Error"
```

#### 🔒 HTTP Basic Auth

```bash
hydra -L users.txt -P passwords.txt 192.168.1.1 http-get /admin
```

#### 🔑 SSH

```bash
hydra -l root -P passwords.txt ssh://192.168.1.1
hydra -l root -P passwords.txt 192.168.1.1 -s 2222 ssh
```

#### 📁 FTP

```bash
hydra -L users.txt -P passwords.txt ftp://192.168.1.1
```

#### 🗄️ MySQL

```bash
hydra -l root -P passwords.txt 192.168.1.1 mysql
```

#### 🗃️ MSSQL

```bash
hydra -l sa -P passwords.txt 192.168.1.1 mssql
```

#### 📧 SMTP

```bash
hydra -l admin@mail.com -P passwords.txt smtp://mail.example.com
```

#### 📬 POP3

```bash
hydra -L users.txt -P passwords.txt pop3://mail.example.com
```

#### 📨 IMAP

```bash
hydra -L users.txt -P passwords.txt imap://mail.example.com
```

#### 🖥️ RDP (Windows Remote Desktop)

```bash
hydra -L users.txt -P passwords.txt rdp://192.168.1.1
```

#### 📡 SMB

```bash
hydra -L users.txt -P passwords.txt smb://192.168.1.1
```

#### 🔐 Telnet

```bash
hydra -l admin -P passwords.txt telnet://192.168.1.1
```

#### 🔌 VNC

```bash
hydra -P passwords.txt 192.168.1.1 vnc
```

#### 🗝️ LDAP

```bash
hydra -L users.txt -P passwords.txt ldap3://192.168.1.1
```

---

### Qabaqcıl İstifadə

#### Çoxlu hədəf

```bash
hydra -L users.txt -P passwords.txt -M targets.txt ssh
```

#### Nəticəni fayla yaz

```bash
hydra -L users.txt -P passwords.txt ssh://192.168.1.1 -o result.txt
```

#### Sessiyadan davam et

```bash
hydra -R
```

#### Xüsusi vaxt intervalı

```bash
hydra -L users.txt -P passwords.txt -w 5 -W 3 ssh://192.168.1.1
```

#### Proxy ilə istifadə

```bash
hydra -L users.txt -P passwords.txt -p socks5://127.0.0.1:9050 ssh://192.168.1.1
```

---

## 🐍 Medusa

### Ümumi Sintaksis

```bash
medusa [SEÇIMLƏR] -h HƏDƏF -u USER -p PASS -M MODUL
```

### Main Parameters

| Parametr        | Açıqlama                                          |
|-----------------|---------------------------------------------------|
| `-h HOST`       | Tək hədəf IP/hostname                             |
| `-H FILE`       | Hədəflər siyahısı (fayl)                          |
| `-u USER`       | Tək istifadəçi adı                                |
| `-U FILE`       | İstifadəçi adları siyahısı (fayl)                 |
| `-p PASS`       | Tək şifrə                                         |
| `-P FILE`       | Şifrə siyahısı (fayl)                             |
| `-C FILE`       | `host:user:pass` formatında birləşmiş fayl        |
| `-M MODULE`     | İstifadə ediləcək modul (protokol)                |
| `-m OPTS`       | Modulun əlavə parametrləri                        |
| `-t N`          | Paralel hədəf sayı (default: 1)                   |
| `-T N`          | Ümumi paralel tapşırıq sayı                       |
| `-f`            | İlk tapılanda dayan (hər hədəf üçün)              |
| `-F`            | Bütün hədəflər üçün ilk tapılanda dayan           |
| `-b`            | Verbose olmayan (sıxışdırılmış) çıxış             |
| `-v LEVEL`      | Verbose səviyyəsi (0-6)                           |
| `-w N`          | Hər cəhd arasında gözləmə vaxtı (saniyə)          |
| `-g N`          | Bağlantı gözləmə vaxtı (saniyə)                   |
| `-r N`          | Uğursuz bağlantıda yenidən cəhd sayı             |
| `-o FILE`       | Nəticəni fayla yaz                                |
| `-n PORT`       | Xüsusi port nömrəsi                               |
| `-s`            | SSL istifadə et                                   |
| `-e ns`         | `n`=boş şifrə, `s`=user=pass                      |

---

### Modul Nümunələri

#### 🔑 SSH

```bash
medusa -h 192.168.1.1 -U users.txt -P passwords.txt -M ssh | grep "ACCOUNT FOUND"
```

#### 📁 FTP

```bash
medusa -h 192.168.1.1 -u admin -P passwords.txt -M ftp | grep "ACCOUNT FOUND"
```

#### 🌐 HTTP (Basic Auth)

```bash
medusa -h 192.168.1.1 -U users.txt -P passwords.txt -M http -m DIR:/admin | grep "ACCOUNT FOUND"
```

#### 🌐 HTTP Form (POST)

```bash
medusa -h 192.168.1.1 -U users.txt -P passwords.txt -M web-form \
  -m "FORM:username=&password=:F=Login failed" | grep "ACCOUNT FOUND"
```

#### 🗄️ MySQL

```bash
medusa -h 192.168.1.1 -u root -P passwords.txt -M mysql | grep "ACCOUNT FOUND"
```

#### 🗃️ MSSQL

```bash
medusa -h 192.168.1.1 -u sa -P passwords.txt -M mssql | grep "ACCOUNT FOUND"
```

#### 📧 SMTP

```bash
medusa -h mail.example.com -U users.txt -P passwords.txt -M smtp | grep "ACCOUNT FOUND"
```

#### 📡 SMB

```bash
medusa -h 192.168.1.1 -U users.txt -P passwords.txt -M smbnt | grep "ACCOUNT FOUND"
```

#### 🖥️ RDP

```bash
medusa -h 192.168.1.1 -U users.txt -P passwords.txt -M rdp | grep "ACCOUNT FOUND"
```

#### 🔐 Telnet

```bash
medusa -h 192.168.1.1 -u admin -P passwords.txt -M telnet | grep "ACCOUNT FOUND"
```

#### 📬 POP3

```bash
medusa -h mail.example.com -U users.txt -P passwords.txt -M pop3 | grep "ACCOUNT FOUND"
```

#### 📨 IMAP

```bash
medusa -h mail.example.com -U users.txt -P passwords.txt -M imap | grep "ACCOUNT FOUND"
```

#### 🔌 VNC

```bash
medusa -h 192.168.1.1 -u admin -P passwords.txt -M vnc
```

#### 🗝️ LDAP

```bash
medusa -h 192.168.1.1 -U users.txt -P passwords.txt -M ldap
```

#### 🐘 PostgreSQL

```bash
medusa -h 192.168.1.1 -u postgres -P passwords.txt -M postgres
```

---

### Qabaqcıl İstifadə

#### Çoxlu hədəf

```bash
medusa -H targets.txt -U users.txt -P passwords.txt -M ssh
```

#### Paralel tapşırıqlar

```bash
medusa -H targets.txt -U users.txt -P passwords.txt -M ssh -t 5 -T 20
```

#### Nəticəni fayla yaz

```bash
medusa -h 192.168.1.1 -U users.txt -P passwords.txt -M ssh -o result.txt
```

#### SSL ilə

```bash
medusa -h 192.168.1.1 -U users.txt -P passwords.txt -M imap -s
```

#### Mövcud modulların siyahısı

```bash
medusa -d
```

---

## Compare Table

| Xüsusiyyət              | Hydra                    | Medusa                    |
|-------------------------|--------------------------|---------------------------|
| **Sürət**               | Çox sürətli              | Sürətli                   |
| **Paralel iş**          | `-t` / `-T`              | `-t` / `-T`               |
| **HTTP Form dəstəyi**   | ✅ Güclü                 | ✅ Var (məhdud)            |
| **Modul sistemi**       | Protokol adı ilə          | `-M MODUL` ilə            |
| **SSL dəstəyi**         | ✅ Avtomatik              | ✅ `-s` ilə               |
| **Sessiya bərpası**     | ✅ `-R`                  | ❌                        |
| **Çoxlu hədəf**         | ✅ `-M FILE`             | ✅ `-H FILE`              |
| **IPv6 dəstəyi**        | ✅                       | Məhdud                    |
| **Aktivlik**            | Aktiv inkişaf             | Az aktiv                  |
| **Platforma**           | Linux/macOS/Windows       | Linux/macOS               |


---

*Son yenilənmə: 2025 | Yalnız icazəli mühitlərdə istifadə edin.*
