# 🔐 Medium Level Penetration Tester — AD + WEB 100 Layihə / Tapşırıq

> **Səviyyə:** Orta (Medium)  
> **Sahə:** Active Directory (AD) + Web Application Penetration Testing  
> **Məqsəd:** Praktiki bacarıqların inkişafı üçün strukturlaşdırılmış yol xəritəsi

---

## 📁 BÖLMƏ 1 — Active Directory (AD) Penetration Testing

### 🔵 1.1 Kəşfiyyat və Siyahıya Alma (Reconnaissance & Enumeration)

| # | Tapşırıq | Alətlər | Çətinlik |
|---|----------|---------|----------|
| 1 | BloodHound ilə AD mühitini tam siyahıya al, attack path-ları aşkar et | BloodHound, SharpHound | ⭐⭐ |
| 2 | LDAP vasitəsilə bütün istifadəçi, qrup və kompüterləri enumerate et | ldapsearch, ADExplorer | ⭐⭐ |
| 3 | Kerberoastable hesabları aşkar et və SPN-ləri siyahıya al | GetUserSPNs.py, Rubeus | ⭐⭐ |
| 4 | AS-REP Roasting üçün preauthentication tələb etməyən hesabları tap | GetNPUsers.py, Rubeus | ⭐⭐ |
| 5 | SMB vasitəsilə paylaşılan qovluqları enumerate et | CrackMapExec, smbclient | ⭐ |
| 6 | Domain Controller-ı, DNS server-ı və site topology-ni kəşf et | nmap, nltest, nslookup | ⭐ |
| 7 | GPO (Group Policy Object) siyahılarını çıxar, zəiflikləri axtar | PowerView, Get-GPO | ⭐⭐ |
| 8 | ACL/DACL zəifliklərini enumerate et (WriteDACL, GenericAll, etc.) | BloodHound, PowerView | ⭐⭐⭐ |
| 9 | Domain Trust əlaqələrini xəritələşdir | nltest, PowerView | ⭐⭐ |
| 10 | AdminSDHolder qorunan hesabları müəyyən et | PowerView, ADSI | ⭐⭐ |

---

### 🟡 1.2 Credential Attacks

| # | Tapşırıq | Alətlər | Çətinlik |
|---|----------|---------|----------|
| 11 | Kerberoasting hücumu həyata keçir və TGS biletlərini offline crack et | Hashcat, John the Ripper | ⭐⭐ |
| 12 | AS-REP Roasting ilə hash əldə et və crack et | Hashcat, GetNPUsers.py | ⭐⭐ |
| 13 | Password Spraying hücumu həyata keçir (lockout-dan qaçaraq) | CrackMapExec, Kerbrute | ⭐⭐ |
| 14 | NTLM hash-ları SAM database-dən dump et | Mimikatz, secretsdump.py | ⭐⭐ |
| 15 | LSASS prosesindən credential-ları dump et | Mimikatz, Pypykatz | ⭐⭐⭐ |
| 16 | DCSync hücumu ilə Domain Admin hash-larını əldə et | Mimikatz, secretsdump.py | ⭐⭐⭐ |
| 17 | Pass-the-Hash (PtH) hücumu həyata keçir | CrackMapExec, Mimikatz | ⭐⭐ |
| 18 | Pass-the-Ticket (PtT) hücumu həyata keçir | Rubeus, Mimikatz | ⭐⭐⭐ |
| 19 | Overpass-the-Hash / Pass-the-Key hücumu tətbiq et | Mimikatz, Rubeus | ⭐⭐⭐ |
| 20 | DPAPI vasitəsilə saxlanmış credential-ları deşifrə et | Mimikatz, DPAPImk2john | ⭐⭐⭐ |

---

### 🟠 1.3 Privilege Escalation (AD)

| # | Tapşırıq | Alətlər | Çətinlik |
|---|----------|---------|----------|
| 21 | GenericAll/GenericWrite ACL-dən istifadə edərək hesab hüquqlarını artır | PowerView, BloodHound | ⭐⭐⭐ |
| 22 | WriteDACL icazəsi ilə özünü DA qrupuna əlavə et | PowerView, net | ⭐⭐⭐ |
| 23 | Constrained Delegation zəifliyindən istifadə et (S4U2Self/S4U2Proxy) | Rubeus, getST.py | ⭐⭐⭐ |
| 24 | Unconstrained Delegation olan server-ı aşkar et və TGT çal | Rubeus, BloodHound | ⭐⭐⭐ |
| 25 | Resource-Based Constrained Delegation (RBCD) hücumu həyata keçir | PowerMad, Rubeus | ⭐⭐⭐ |
| 26 | SeBackupPrivilege ilə NTDS.dit-i kopyala | SeBackupPrivilege exploit | ⭐⭐⭐ |
| 27 | SeImpersonatePrivilege ilə token impersonation et (Potato hücumları) | PrintSpoofer, GodPotato | ⭐⭐ |
| 28 | AdminSDHolder abuse edərək persistent backdoor yarat | PowerView, ADSI | ⭐⭐⭐ |
| 29 | GPO abuse ilə scheduled task vasitəsilə kod icra et | SharpGPOAbuse | ⭐⭐⭐ |
| 30 | Domain-dən Forest-ə keçid üçün SID History Injection həyata keçir | Mimikatz | ⭐⭐⭐⭐ |

---

### 🔴 1.4 Lateral Movement (AD)

| # | Tapşırıq | Alətlər | Çətinlik |
|---|----------|---------|----------|
| 31 | WMI vasitəsilə uzaq maşında kod icra et | CrackMapExec, Invoke-WMIMethod | ⭐⭐ |
| 32 | PSRemoting (WinRM) ilə lateral movement et | Evil-WinRM, CrackMapExec | ⭐⭐ |
| 33 | PsExec ilə uzaq sistem üzərində əmr icra et | Impacket psexec.py | ⭐⭐ |
| 34 | SMB vasitəsilə service yaratmaq üçün SCM-i istismar et | smbexec.py | ⭐⭐ |
| 35 | RDP üzərindən credential ile giriş et, session hijack sına | xfreerdp, tscon | ⭐⭐ |
| 36 | DCOM protokolundan istifadə edərək lateral movement et | Impacket dcomexec.py | ⭐⭐⭐ |
| 37 | Token impersonation ilə başqa istifadəçi kimi hərəkət et | Mimikatz, incognito | ⭐⭐⭐ |
| 38 | Active Directory Certificate Services (ADCS) ESC1 zəifliyini istismar et | Certipy, Certify | ⭐⭐⭐ |
| 39 | ADCS ESC8 (NTLM Relay to AD CS HTTP Endpoint) hücumunu tətbiq et | ntlmrelayx.py, Certipy | ⭐⭐⭐⭐ |
| 40 | MS14-068 Kerberos Privilege Escalation zəifliyini sına | PyKEK, Metasploit | ⭐⭐⭐ |

---

### ⚫ 1.5 Persistence və Post-Exploitation (AD)

| # | Tapşırıq | Alətlər | Çətinlik |
|---|----------|---------|----------|
| 41 | Golden Ticket hücumu həyata keçir (KRBTGT hash ilə) | Mimikatz, Rubeus | ⭐⭐⭐ |
| 42 | Silver Ticket yaratmaq üçün service account hash-ını istifadə et | Mimikatz | ⭐⭐⭐ |
| 43 | Diamond Ticket ilə daha stealth persistence yarat | Rubeus | ⭐⭐⭐⭐ |
| 44 | Skeleton Key hücumu ilə DC-yə backdoor yarat | Mimikatz | ⭐⭐⭐ |
| 45 | DCShadow hücumu ilə AD replikasiya axınına müdaxilə et | Mimikatz | ⭐⭐⭐⭐ |
| 46 | Yeni gizli Domain Admin hesabı yarat | net, PowerShell | ⭐⭐ |
| 47 | Scheduled Task vasitəsilə persistence qur | schtasks, CrackMapExec | ⭐⭐ |
| 48 | Registry Run Key ilə startup persistence əlavə et | reg, PowerShell | ⭐⭐ |
| 49 | WMI Event Subscription ilə fileless persistence yarat | PowerShell, WMI | ⭐⭐⭐ |
| 50 | NTDS.dit-i dump et və offline-da bütün domain hash-larını əldə et | secretsdump.py, ntdsutil | ⭐⭐⭐ |

---

## 📁 BÖLMƏ 2 — Web Application Penetration Testing

### 🔵 2.1 Kəşfiyyat və Siyahıya Alma (Web Recon)

| # | Tapşırıq | Alətlər | Çətinlik |
|---|----------|---------|----------|
| 51 | Subdomain enumeration həyata keçir | Subfinder, Amass, dnsx | ⭐ |
| 52 | Directory və fayl bruteforce et | Feroxbuster, Gobuster, dirsearch | ⭐ |
| 53 | JavaScript fayllarından endpoint-lər çıxar | LinkFinder, JSParser | ⭐⭐ |
| 54 | Virtual host (VHost) kəşfi həyata keçir | Gobuster vhost, ffuf | ⭐⭐ |
| 55 | Web texnologiyalarını fingerprint et | Wappalyzer, WhatWeb, Nuclei | ⭐ |
| 56 | Google Dorking ilə açıq sənədlər və konfiqurasiya faylları tap | Google, Shodan | ⭐ |
| 57 | Wayback Machine ilə köhnə endpoint-lər tap | waybackurls, gau | ⭐ |
| 58 | API endpoint-lərini Swagger/OpenAPI spesifikasiyasından çıxar | Postman, Burp Suite | ⭐⭐ |
| 59 | S3 bucket-ları enumerate et, açıq olanları tap | s3scanner, awsbucketdump | ⭐⭐ |
| 60 | CMS aşkar et (WordPress, Joomla, Drupal) və versiya tap | WPScan, CMSMap | ⭐ |

---

### 🟡 2.2 Injection Zəiflikləri

| # | Tapşırıq | Alətlər | Çətinlik |
|---|----------|---------|----------|
| 61 | SQL Injection aşkar et və database-i dump et | sqlmap, manual | ⭐⭐ |
| 62 | Blind SQL Injection (Boolean/Time-based) əl ilə istismar et | manual, sqlmap | ⭐⭐⭐ |
| 63 | NoSQL Injection (MongoDB) həyata keçir | manual, Burp Suite | ⭐⭐⭐ |
| 64 | OS Command Injection tapıb RCE əldə et | manual, Burp Suite | ⭐⭐ |
| 65 | Server-Side Template Injection (SSTI) aşkar et və RCE əldə et | tplmap, manual | ⭐⭐⭐ |
| 66 | XPath Injection tətbiq et | manual, Burp Suite | ⭐⭐⭐ |
| 67 | LDAP Injection həyata keçir | manual | ⭐⭐⭐ |
| 68 | XML External Entity (XXE) ilə /etc/passwd oxu | manual, Burp Suite | ⭐⭐ |
| 69 | Blind XXE ilə Out-of-Band data exfiltration et | Burp Collaborator, XXEinjector | ⭐⭐⭐ |
| 70 | HTTP Header Injection (CRLF) həyata keçir | manual, Burp Suite | ⭐⭐ |

---

### 🟠 2.3 Authentication və Session Zəiflikləri

| # | Tapşırıq | Alətlər | Çətinlik |
|---|----------|---------|----------|
| 71 | Broken Authentication — rate limiting olmayan login formunu bruteforce et | Hydra, Burp Intruder | ⭐⭐ |
| 72 | JWT token-i alqoritm konfusion (RS256→HS256) ilə saxtalaşdır | jwt_tool, manual | ⭐⭐⭐ |
| 73 | JWT "none" alqoritm zəifliyini istismar et | manual, jwt_tool | ⭐⭐ |
| 74 | Session fixation hücumu həyata keçir | manual, Burp Suite | ⭐⭐ |
| 75 | CSRF token bypass metodlarını sına | manual, Burp Suite | ⭐⭐⭐ |
| 76 | Password reset funksiyasındakı zəiflikləri istismar et (token leak, host header) | manual, Burp Suite | ⭐⭐⭐ |
| 77 | OAuth 2.0 misconfiguration-larını tap (state param bypass, open redirect) | manual, Burp Suite | ⭐⭐⭐ |
| 78 | Cookie security atributlarını (HttpOnly, Secure, SameSite) analiz et | Burp Suite, browser | ⭐ |
| 79 | Multi-factor authentication bypass metodlarını sına | manual | ⭐⭐⭐ |
| 80 | Account takeover üçün HTTP response manipulation sına | Burp Suite | ⭐⭐⭐ |

---

### 🔴 2.4 Server-Side Zəifliklər

| # | Tapşırıq | Alətlər | Çətinlik |
|---|----------|---------|----------|
| 81 | Server-Side Request Forgery (SSRF) ilə daxili servislərə çat | manual, Burp Suite | ⭐⭐⭐ |
| 82 | Blind SSRF ilə out-of-band interaksiya aşkar et | Burp Collaborator, interactsh | ⭐⭐⭐ |
| 83 | Path Traversal ilə həssas faylları oxu (../../etc/passwd) | manual, Burp Suite | ⭐⭐ |
| 84 | Local File Inclusion (LFI) ilə log poisoning et, RCE əldə et | manual, Burp Suite | ⭐⭐⭐ |
| 85 | Remote File Inclusion (RFI) ilə uzaqdan kod icra et | manual | ⭐⭐ |
| 86 | Insecure Deserialization — PHP/Java/Python object injection sına | ysoserial, PHPGGC | ⭐⭐⭐⭐ |
| 87 | File Upload zəifliyi — WebShell yüklə, RCE əldə et | manual, Burp Suite | ⭐⭐ |
| 88 | Business Logic zəifliyi tap (price manipulation, workflow bypass) | manual, Burp Suite | ⭐⭐⭐ |
| 89 | HTTP Request Smuggling (CL.TE/TE.CL) həyata keçir | Burp Suite, smuggler | ⭐⭐⭐⭐ |
| 90 | GraphQL introspection açıqdırsa, gizli field-ləri aşkar et | InQL, Altair, manual | ⭐⭐ |

---

### ⚫ 2.5 Client-Side Zəifliklər və İleri Səviyyə Web

| # | Tapşırıq | Alətlər | Çətinlik |
|---|----------|---------|----------|
| 91 | Reflected XSS tapıb cookie steal et | manual, XSStrike, Burp | ⭐⭐ |
| 92 | Stored XSS ilə admin session-ını oğurla | manual, Burp Suite | ⭐⭐ |
| 93 | DOM-based XSS aşkar et (source-sink analizi) | manual, DOMinator | ⭐⭐⭐ |
| 94 | CORS misconfiguration ilə cross-origin data oxu | manual, Burp Suite | ⭐⭐⭐ |
| 95 | Clickjacking hücumu sına, PoC hazırla | manual, Burp Suite | ⭐ |
| 96 | Open Redirect tap və phishing zənciri qur | manual, Burp Suite | ⭐⭐ |
| 97 | Subdomain Takeover həyata keçir (dangling DNS) | Subjack, nuclei | ⭐⭐⭐ |
| 98 | Web Cache Poisoning hücumu tətbiq et | Burp Suite, manual | ⭐⭐⭐⭐ |
| 99 | API Rate Limiting bypass metodlarını sına (header manipulation) | manual, Burp Suite | ⭐⭐⭐ |
| 100 | Full attack chain qur: Web RCE → Internal SSRF → AD lateral movement | Hər şey | ⭐⭐⭐⭐⭐ |

---

## 🛠️ Tövsiyə Olunan Alətlər Siyahısı

### Active Directory
```
BloodHound / SharpHound    → AD attack path analizi
Impacket Suite             → Python tabanlı AD hücum alətləri  
Mimikatz                   → Credential dump
Rubeus                     → Kerberos attacks
CrackMapExec (CME/NetExec) → Swiss army knife for AD
PowerView                  → AD enumeration (PowerShell)
Certipy                    → ADCS attacks
Evil-WinRM                 → WinRM shell
Kerbrute                   → Kerberos brute/enum
```

### Web Application
```
Burp Suite Pro             → Ana proxy və testing platforması
ffuf / Gobuster            → Fuzzing
Nuclei                     → Template-based scanning
sqlmap                     → SQL Injection automation
jwt_tool                   → JWT testing
XSStrike                   → XSS testing
Subfinder / Amass          → Recon
interactsh / OAST          → Out-of-band testing
ysoserial / PHPGGC         → Deserialization payloads
```

---

## 📚 Öyrənmə Resursları

### Praktiki Platformalar
- **HackTheBox** — Active Directory maşınları (Forest, Monteverde, Cascade, Sauna)
- **TryHackMe** — AD Learning Paths, Web Fundamental Path
- **PentesterLab** — Web application exercise-ləri
- **PortSwigger Web Security Academy** — Pulsuz, ən yaxşı web pentesting labs
- **VulnHub** — Offline AD lab maşınları

### Oxunacaq Materiallar
- "The Hacker Playbook 3" — Red Team bölmələri
- "Real-World Bug Hunting" — Web zəiflikləri
- SpecterOps BloodHound documentation
- HackTricks (book.hacktricks.xyz) — Hərtərəfli cheat sheet

### Sertifikatlar
- **OSCP** (OffSec Certified Professional) — Ən əsas
- **CRTE** (Certified Red Team Expert) — AD üzrə
- **BSCP** (Burp Suite Certified Practitioner) — Web üzrə
- **eJPT / eCPPT** — Başlanğıc üçün

---

## 📊 Tamamlama Meyarları

| Səviyyə | Tamamlanan Tapşırıq | Status |
|---------|---------------------|--------|
| Başlanğıc | 1-30 | 🟢 Əsas bacarıqlar |
| Orta | 31-70 | 🟡 İnkişaf mərhələsi |
| İleri | 71-99 | 🔴 Expert səviyyəsi |
| Master | 100 | ⭐ Full chain |

---

> **Qeyd:** Bu tapşırıqlar yalnız **qanuni** mühitlərdə (öz lab-ınızda, CTF platformalarında, icazə verilmiş sistemlərdə) tətbiq edilməlidir. İcazəsiz sistemlərə hücum cinayət sayılır.

*Hazırlandı: Medium Level Penetration Tester üçün Strukturlaşdırılmış Öyrənmə Yolu*
