# 🔴 OSEP İmtahanı — 100 Addımlı Tam Hazırlıq Roadmap

> **PEN-300: Evasion Techniques and Breaching Defenses**
> Bu roadmap OSEP sertifikatı üçün tam hazırlıq proqramıdır. Hər addım real imtahan mövzularına əsaslanır.

---

## 📋 Ümumi Məlumat

| Xüsusiyyət | Detal |
|---|---|
| **Sertifikat** | OSEP (OffSec Experienced Pentester) |
| **Kurs** | PEN-300 |
| **İmtahan müddəti** | 48 saat |
| **Format** | Flags toplamaq (Active Directory + Linux) |
| **Əsas fokus** | AV/EDR bypass, lateral movement, AD hücumları |

---

## 🗓️ MƏRHƏLƏ 1: Əsas Hazırlıq (Addım 1–15)

### Addım 1 — Mühit Qurulması
- Kali Linux (son versiya) VM qur
- Windows 10/11 + Windows Server 2019 VM-lər qur
- VMware Workstation Pro və ya VirtualBox seç
- Snapshot sistemi ilə çalış (hər eksperimentdən əvvəl snapshot al)

### Addım 2 — PEN-300 Kurs Materialını Əldə Et
- OffSec Learn platformasına daxil ol
- PDF + video materialları yüklə
- Hər fəsil üçün lab mühitini aktivləşdir
- Qeyd dəftəri (Obsidian və ya CherryTree) hazırla

### Addım 3 — C# Proqramlaşdırmaya Giriş
- .NET Framework və .NET Core fərqini öyrən
- Visual Studio 2022 Community qur
- Console application, Class library quruluşunu öyrən
- `csc.exe` ilə command-line compile etməyi öyrən

### Addım 4 — C# Əsas Sintaksis
- Variables, data types, loops, conditionals
- Classes, methods, namespaces
- Exception handling (try/catch/finally)
- P/Invoke: Windows API çağırışları üçün `DllImport`

### Addım 5 — Windows API Əsasları
- Win32 API necə işləyir
- `kernel32.dll`, `ntdll.dll`, `advapi32.dll`
- MSDN sənədlərini oxumağı öyrən
- `PInvoke.net` resursundan istifadə et

### Addım 6 — PowerShell Dərinləşmə
- PowerShell execution policy mexanizmlərini öyrən
- `AMSI` (Anti-Malware Scan Interface) nədir
- PowerShell logging: ScriptBlock, Module, Transcription
- `Constrained Language Mode` nədir və necə bypass edilir

### Addım 7 — Python ilə Tooling
- Socket proqramlaşdırma əsasları
- HTTP requests ilə exploit yazma
- Shellcode manipulation Python ilə
- `impacket` kitabxanasına giriş

### Addım 8 — Networking Əsasları (Penetration Testing perspektivindən)
- TCP/UDP port scanning mexanizmi
- HTTP/HTTPS, SMB, RDP, WinRM protokolları
- Proxy zəncirləri: SOCKS4/SOCKS5
- Pivoting konsepsiyası: niyə lazımdır

### Addım 9 — Active Directory Əsasları
- AD components: Domain, Forest, Tree, OU
- LDAP sorğuları ilə obyektləri axtarmaq
- Kerberos authentication axını
- NTLM authentication axını

### Addım 10 — Antivirus Mexanizmləri
- Signature-based detection
- Heuristic/behavioral detection
- Sandbox analysis
- EDR (Endpoint Detection & Response) nədir

### Addım 11 — Lab: İlk C# Shellcode Runner
- `msfvenom` ilə shellcode yarat
- C# proqramı içinə shellcode yerləşdir
- `VirtualAlloc` + `CreateThread` istifadə et
- Windows Defender-in reaksiyasını müşahidə et

### Addım 12 — Metasploit Framework Dərinləşmə
- `msfconsole` advanced istifadəsi
- Payload types: staged vs stageless
- `multi/handler` konfiqurasiyası
- Meterpreter post-exploitation modülləri

### Addım 13 — Burp Suite və Web Hücumları (OSEP kontekstində)
- Client-side hücumlar üçün web server qurma
- `SimpleHTTPServer` ilə payload çatdırma
- HTA, DOCX, JScript payload-larını host etmə
- HTTPS ilə C2 kanalı simulyasiyası

### Addım 14 — Wireshark ilə Traffic Analizi
- Shellcode traffic-i necə görünür
- Beaconing pattern-ləri tanı
- Encrypted C2 vs plaintext C2
- Network-based detection qaçınma

### Addım 15 — Qeyd Sistemi Qurulması
- Obsidian ilə structured note-taking
- Hər texnika üçün şablon yarat: Texnika → Necə işləyir → Bypass → Kod
- `tmux` ilə terminal idarəetməsi
- Screenshot avtomatlaşdırması

---

## 🗓️ MƏRHƏLƏ 2: Client-Side Hücumlar (Addım 16–28)

### Addım 16 — HTML Applications (HTA)
- HTA faylı nədir və necə işləyir
- `mshta.exe` ilə execution
- VBScript içərisindən shellcode execution
- HTA-nın AV-dan necə qaçdığı

### Addım 17 — Microsoft Office Makrolar
- Word/Excel makro yaratma
- `AutoOpen` / `Document_Open` event-ləri
- VBA ilə shellcode execution
- Makro obfuscation texnikaları

### Addım 18 — Office Macro Delivery
- `.doc` vs `.docm` format fərqi
- Email attachment ssenariləri
- Protected view bypass texnikaları
- Makro-suz Office hücumları (DDE, Field codes)

### Addım 19 — Windows Script Host (WSH)
- JScript və VBScript ilə payload
- `wscript.exe` vs `cscript.exe`
- `.js`, `.vbs`, `.wsf` faylları
- WSH-dən shellcode execution

### Addım 20 — PowerShell Payload-ları
- Base64 encoded PowerShell commands
- `IEX (New-Object Net.WebClient).DownloadString()`
- PowerShell download cradles
- AMSI bypass texnikaları (memory patching)

### Addım 21 — AMSI Bypass Texnikaları
- AMSI necə işləyir (memory-dən)
- `amsi.dll` patch etmə
- Reflection ilə AMSI bypass
- String obfuscation ilə AMSI bypass

### Addım 22 — Shellcode Obfuscation — Giriş
- XOR encryption shellcode üçün
- Caesar cipher tətbiqi
- Custom decryption stub yazmaq
- Runtime decryption konsepsiyası

### Addım 23 — Lab: Macro-based Payload
- Word sənədə VBA makro yaz
- Shellcode-u makro içinə yerləşdir
- Windows Defender testini apar
- Obfuscation ilə bypass et

### Addım 24 — Phishing Infrastruktur
- GoPhish ilə phishing kampaniyası
- Credential harvesting səhifəsi
- Email spoofing əsasları
- Domain reputation idarəetməsi

### Addım 25 — Living off the Land Binaries (LOLBins)
- `certutil.exe` ilə fayl yükləmə
- `msiexec.exe` ilə payload execution
- `regsvr32.exe` (Squiblydoo) texnikası
- `rundll32.exe` ilə execution

### Addım 26 — regsvr32 / Squiblydoo
- COM scriptlet yaratma (`.sct` faylı)
- `regsvr32 /u /n /s /i:http://` pattern
- AppLocker bypass imkanı
- WDAC bypass imkanı

### Addım 27 — Microsoft Teams / Slack Hücumları (kontekst)
- Phishing vektorları olaraq Teams
- Attachment delivery Teams vasitəsilə
- Social engineering taktikalar
- Initial access vektorlarını map etmək

### Addım 28 — Lab: Full Client-Side Chain
- HTA → Shellcode → Meterpreter zənciri
- VBA Makro → PowerShell → C# loader zənciri
- Hər zənciri sıfırdan qur və test et
- Problematik nöqtələri debug et

---

## 🗓️ MƏRHƏLƏ 3: Process Injection (Addım 29–42)

### Addım 29 — Process Injection Nədir
- Process injection-ın məqsədi: legitimasiya
- Injection target seçimi (explorer.exe, svchost.exe)
- Handle açma: `OpenProcess`
- Memory allokasiyas: `VirtualAllocEx`

### Addım 30 — Classic DLL Injection
- `WriteProcessMemory` ilə DLL path yazma
- `CreateRemoteThread` + `LoadLibrary`
- Handle icazələri: `PROCESS_ALL_ACCESS`
- Lab: notepad.exe-yə DLL inject et

### Addım 31 — Reflective DLL Injection
- Disk-less DLL yükləmə
- `ReflectiveDLLInjection` GitHub proyekti
- Özünü yükləyən DLL strukturu
- Memory-only execution üstünlükləri

### Addım 32 — Process Hollowing
- Suspended process yaratmaq
- Legitimate binary-nin memory-sini boşaltmaq
- Shellcode ilə doldurma
- `ResumeThread` ilə işə salma

### Addım 33 — Process Hollowing — Kod
- `CreateProcess` `CREATE_SUSPENDED` flag
- `NtUnmapViewOfSection` istifadəsi
- `WriteProcessMemory` + `SetThreadContext`
- C# ilə tam process hollowing implementasiyası

### Addım 34 — Thread Hijacking
- Mövcud thread-i suspend etmək
- `GetThreadContext` / `SetThreadContext`
- RIP/EIP registr manipulyasiyası
- `ResumeThread` ilə execution yönləndirmə

### Addım 35 — APC Injection
- Asynchronous Procedure Calls (APC)
- Alertable thread nədir
- `QueueUserAPC` funksiyası
- Early Bird APC injection texnikası

### Addım 36 — Early Bird APC Injection
- Suspended process-də APC queue etmək
- Process başlamadan əvvəl shellcode çalışdırmaq
- AV hook-larından əvvəl execution
- C# implementasiyası

### Addım 37 — Shellcode Runner — AV Bypass Texnikalar
- Sleep + timing check bypass
- Sandbox detection texnikaları
- User interaction check (mouse movement)
- Process parent check

### Addım 38 — Syscall-based Injection
- Direct syscall nədir
- `NtAllocateVirtualMemory` birbaşa çağırışı
- EDR hook-larını bypass etmə
- `SysWhispers2` tool istifadəsi

### Addım 39 — Injection Texnikalarının Müqayisəsi
- Hər injection metodunun AV detection riski
- Hansı situasiyada hansı metod seçilməlidir
- EDR vs AV davranış fərqləri
- Forensic artifact-lar

### Addım 40 — Lab: Process Injection Chain
- Önce `calc.exe`-yə shellcode inject et
- Sonra `explorer.exe`-yə yönləndir
- Meterpreter sessionu qoru
- Migrate komandası ilə process dəyiş

### Addım 41 — DLL Proxying / DLL Hijacking
- DLL search order nədir
- `LoadLibrary` axtarma sırası
- Zəif DLL yükləmə tapan tool-lar
- Proxy DLL yaratmaq (orijinalı saxlamaqla)

### Addım 42 — Lab: Tam Injection Ssenari
- Spear phishing ilə initial access
- Process injection ilə persistence
- AV bypass ilə uzun müddət qalma
- Bütün addımları sıfırdan icra et

---

## 🗓️ MƏRHƏLƏ 4: AV/EDR Bypass (Addım 43–55)

### Addım 43 — AV Bypass Metodologiyası
- Signature detection bypass: string dəyişdirmə
- Behavioral detection bypass: slow execution
- Memory scanning bypass: encryption
- Heuristic bypass: benign context

### Addım 44 — Shellcode Encryption
- AES-128/256 encryption C# ilə
- Runtime key generation
- Key-in binary içərisindən gizlədilməsi
- XOR multi-byte key

### Addım 45 — Payload Segmentation
- Shellcode-u parçalara bölmək
- Ayrı thread-lərdə hissələri birləşdirmək
- Sleep aralarında execution
- Memory-də shellcode yığımı

### Addım 46 — Donut Framework
- Position-independent shellcode generator
- .NET assembly-ni shellcode-a çevirmək
- DLL-i shellcode-a çevirmək
- Donut ilə AMSI/ETW bypass

### Addım 47 — ETW (Event Tracing for Windows) Bypass
- ETW nədir və niyə vacibdir
- `EtwEventWrite` patch etmək
- ETW provider-ları deaktiv etmək
- EDR-in ETW-dən istifadəsi

### Addım 48 — PPID Spoofing
- Parent Process ID saxtalaşdırması
- Legitim parent göstərmək (Word → explorer)
- `STARTUPINFOEX` strukturu
- `UpdateProcThreadAttribute` istifadəsi

### Addım 49 — Stomping Texnikaları
- Module Stomping (DLL hollowing)
- Mövcud DLL-in memory-sini dəyişdirmək
- Thread Stack Spoofing
- Call Stack manipulyasiyası

### Addım 50 — Obfuscation Tool-ları
- `Confuser` / `ConfuserEx` .NET obfuscation
- String encryption, control flow obfuscation
- `InvisibilityCloak` tool
- Custom obfuscator yazmaq

### Addım 51 — Living off the Land ilə AV Bypass
- LOLBins vasitəsilə payload execution
- `msbuild.exe` ilə C# kodu çalışdırmaq
- `installutil.exe` bypass texnikası
- `csc.exe` ilə runtime compile

### Addım 52 — WDAC (Windows Defender Application Control) Bypass
- WDAC policy-lərini anlamaq
- Trusted binary-lərin istifadəsi
- `.NET` trust bypass texnikaları
- COM object bypass

### Addım 53 — AppLocker Bypass
- AppLocker rule types (Path, Publisher, Hash)
- Writable whitelisted path-lar
- `C:\Windows\Tasks\`, `C:\Windows\Temp\`
- Scripting engine bypass-ları

### Addım 54 — Defender Sandbox Evasion
- VM/Sandbox aşkarlama texnikalar
- Sleep-based evasion
- Registry, filesystem artifact check
- Network connectivity check

### Addım 55 — Lab: Tam AV Bypass Chain
- Defender + EDR aktiv mühitdə
- Encryted shellcode + process injection
- Bütün bypass texnikalarını birləşdir
- VirusTotal-da yoxlama (private scan)

---

## 🗓️ MƏRHƏLƏ 5: Lateral Movement (Addım 56–68)

### Addım 56 — Lateral Movement Əsasları
- Lateral movement nədir
- Hansı protokollar istifadə edilir
- Credential reuse vs pass-the-hash
- Network segmentation bypass

### Addım 57 — Pass-the-Hash (PtH)
- NTLM hash ilə authentication
- `sekurlsa::pth` (Mimikatz)
- PtH ilə SMB/WMI/PSExec
- Restricted Admin mode

### Addım 58 — Overpass-the-Hash
- NTLM hash → Kerberos TGT
- `sekurlsa::pth /ntlm /user /domain`
- Kerberos ticket ilə hərəkət
- Pass-the-Ticket ilə fərq

### Addım 59 — Pass-the-Ticket (PtT)
- TGT / TGS fərqi
- Ticket export etmək (Mimikatz, Rubeus)
- Ticket import etmək
- Golden Ticket ilə fərqi

### Addım 60 — WMI ilə Lateral Movement
- `Win32_Process.Create` metodu
- `wmic /node` ilə remote execution
- C# WMI class-lardan istifadə
- WMI event subscription (persistence)

### Addım 61 — WinRM ilə Lateral Movement
- PowerShell Remoting protokolu
- `New-PSSession` / `Invoke-Command`
- `evil-winrm` tool istifadəsi
- WinRM ilə fayl transfer

### Addım 62 — PSExec ilə Lateral Movement
- PSExec necə işləyir (SMB + service)
- `Impacket psexec.py`
- C# PSExec implementasiyası
- Detection artifact-lar

### Addım 63 — DCOM ilə Lateral Movement
- Distributed COM nədir
- `MMC20.Application` COM object
- `ShellWindows` / `ShellBrowserWindow`
- `Impacket dcomexec.py`

### Addım 64 — Scheduled Task ilə Lateral Movement
- Remote scheduled task yaratmaq
- `schtasks /create /s <remotehost>`
- COM `ITaskService` interface
- Cleanup: task silmək

### Addım 65 — SMB ilə Fayl Transfer
- `$C`, `$ADMIN` share-ləri
- Payload-ı remote host-a kopyalamaq
- UNC path-lar vasitəsilə execution
- SMB relay hücumları

### Addım 66 — SSH Pivoting (Linux)
- Local Port Forwarding
- Remote Port Forwarding
- Dynamic Port Forwarding (SOCKS)
- `ProxyChains` konfiqurasiyası

### Addım 67 — Chisel ilə Tunneling
- Chisel server + client qurulumu
- SOCKS5 tunnel yaratmaq
- Firewall-dan keçmək
- Multi-hop pivoting

### Addım 68 — Lab: Lateral Movement Chain
- İlk host-dan ikinci host-a PtH
- İkincidən üçüncüyə WMI
- Hər hostda foothold saxla
- Network map yenilə

---

## 🗓️ MƏRHƏLƏ 6: Active Directory Hücumları (Addım 69–82)

### Addım 69 — AD Reconnaissance
- `PowerView` ilə AD enumeration
- `SharpHound` ilə BloodHound data collect
- `ldapsearch` / `LDAP` sorğuları
- `net` commands ilə quick enum

### Addım 70 — BloodHound Analizi
- BloodHound qurulumu (Neo4j + GUI)
- Attack path-ları vizualizasiya
- "Shortest path to Domain Admin" sorğusu
- Custom Cypher sorğuları

### Addım 71 — Kerberoasting
- SPN (Service Principal Name) nədir
- TGS ticket request etmək
- Offline hash cracking (hashcat)
- `Rubeus kerberoast` / `GetUserSPNs.py`

### Addım 72 — AS-REP Roasting
- Pre-authentication deaktiv hesablar
- `Rubeus asreproast`
- `GetNPUsers.py` impacket
- Offline crack: `hashcat -m 18200`

### Addım 73 — DCSync Hücumu
- Replication rights ilə hash dump
- `mimikatz lsadump::dcsync /user:krbtgt`
- `secretsdump.py` impacket
- DCSync detection mexanizmi

### Addım 74 — Golden Ticket
- krbtgt hash-in əhəmiyyəti
- Forged TGT yaratmaq
- Domain-wide persistence
- `mimikatz kerberos::golden`

### Addım 75 — Silver Ticket
- Service account hash ilə TGS forgery
- CIFS, HOST, HTTP service ticket-ları
- Spesifik servis hücumları
- DC ilə əlaqə olmadan işləmək

### Addım 76 — Constrained Delegation
- msDS-AllowedToDelegateTo attribute
- `Rubeus s4u` komandası
- S4U2Self + S4U2Proxy zənciri
- Hücum ssenarisinin tamamlanması

### Addım 77 — Unconstrained Delegation
- Printer Bug (SpoolSample) ilə kombinasiya
- TGT capture edilməsi
- `Rubeus monitor` + `SpoolSample`
- DC computer account-un TGT-si

### Addım 78 — Resource-Based Constrained Delegation (RBCD)
- `msDS-AllowedToActOnBehalfOfOtherIdentity`
- Zəif DACL-ları istifadə etmək
- Machine account yaratmaq
- `Rubeus s4u` ilə impersonation

### Addım 79 — ACL/DACL Abuse
- GenericAll, GenericWrite, WriteDACL
- ForceChangePassword hücumu
- Shadow Credentials (KeyCredentialLink)
- BloodHound ilə ACL tapma

### Addım 80 — Domain Trust Hücumları
- Forest vs Domain trust
- Trust relationship exploitation
- Inter-forest TGT forgery
- SID History abuse

### Addım 81 — LAPS (Local Administrator Password Solution) Bypass
- LAPS nədir
- LAPS şifrəsini oxuma icazəsi
- `LAPSToolkit` istifadəsi
- LAPS olmayan sistemlər

### Addım 82 — Lab: AD Full Compromise Chain
- Low-privileged user ilə başla
- Kerberoasting → lateral movement
- DCSync → Golden Ticket
- Domain Admin qazanmaq

---

## 🗓️ MƏRHƏLƏ 7: Linux Post-Exploitation (Addım 83–90)

### Addım 83 — Linux Privilege Escalation — Ümumi Baxış
- SUID/SGID binary-lər
- Sudo misconfiguration
- Cron job abuse
- Writable `/etc/passwd`

### Addım 84 — Linux-da Credential Hunting
- `/etc/shadow` oxumaq
- SSH private key-lər
- History faylları (`.bash_history`)
- Config fayllarında plaintext credentials

### Addım 85 — Linux Pivoting
- SSH dynamic forwarding
- `socat` ilə port forwarding
- `netsh` əvəzinə `iptables`
- `/proc/net/tcp` ilə port enum

### Addım 86 — Linux AD İnteqrasiyası (Kerberos)
- Linux hostda Kerberos ticket (`klist`)
- `.ccache` faylını oğurlamaq
- `KRB5CCNAME` environment variable
- Impacket ilə `.ccache` istifadəsi

### Addım 87 — Linux-da Persistence
- Cron job backdoor
- `.bashrc` / `.profile` trojan
- systemd service backdoor
- SSH authorized_keys

### Addım 88 — Docker Escape Texnikaları
- Privileged container hücumu
- `/proc/sysrq-trigger` metodu
- Volume mount abuse
- `--cap-add=SYS_ADMIN` exploit

### Addım 89 — Lab: Linux Privilege Escalation
- Zəif SUID binary tap
- Sudo misconfiguration exploit
- Cron job hijack
- Root əldə et

### Addım 90 — Lab: Linux → Windows AD
- Linux-dan AD-yə Kerberos attack
- `.ccache` ilə Windows resource-a çatmaq
- Impacket toolset-in tam istifadəsi
- Cross-platform attack zənciri

---

## 🗓️ MƏRHƏLƏ 8: Tam Ssenari Hazırlığı (Addım 91–100)

### Addım 91 — OSEP İmtahan Formatını Anlamaq
- 48 saatlıq imtahan strukturu
- Flags: Local + Proof flags
- Sıfırdan Domain Admin-ə gedən yol
- Hesabat tələbləri (imtahan sonrası)

### Addım 92 — C2 Framework Mənimsəmə (Covenant/Havoc)
- Covenant C2: Grunt yaratmaq
- Havoc C2: Agent deployment
- Listener konfiqurasiyası (HTTP/HTTPS)
- Malleable C2 profile-lar

### Addım 93 — Sliver C2 Framework
- Sliver qurulumu
- HTTP/HTTPS/DNS implant-lar
- mTLS istifadəsi
- BOF (Beacon Object File) dəstəyi

### Addım 94 — Hesabat Yazma Texnikası
- Executive summary yazmaq
- Texniki tapıntıları sənədləşdirmək
- Screenshot ilə hər addımı əsaslandırmaq
- Remediation tövsiyələri

### Addım 95 — Lab: Tam OSEP Ssenari #1
- Phishing → Initial Access
- AV bypass + Process injection
- Lateral movement (3 host)
- Domain compromise
- Bütün flagları topla

### Addım 96 — Lab: Tam OSEP Ssenari #2
- Fərqli initial access vektoru
- Linux pivot nöqtəsi
- AD-yə Kerberos vasitəsilə
- Golden Ticket persistence
- Ssenarini 8 saat ərzində tamamla

### Addım 97 — Zəif Nöqtə Analizi
- Kəşf etdiyin çətin mövzuları siyahıla
- Hər zəif nöqtə üçün əlavə məşq et
- Kurs laboratoriyalarını təkrar et
- Challenge labs-ı bitir

### Addım 98 — Speed Drilling
- Hər texnikani timer ilə məşq et
- Cheat sheet hazırla (personal reference)
- Tool cheatsheets: Mimikatz, Rubeus, Impacket, Netsh
- Ən çox istifadə olunan komandaları əzbərlə

### Addım 99 — Mock Exam
- 48 saatlıq simulyasiya apar
- Real imtahan şəraitini yarat (interruption yoxdur)
- Hər 2 saatdan bir progress qeyd et
- Post-exam debriefing: nə çətin oldu?

### Addım 100 — İmtahana Hazırlıq (Son 48 saat)
- Bütün tool-ların işlədiyini yoxla
- VPN bağlantısını test et
- Cheat sheet-ləri print et
- Rest al — 48 saat fiziki dözümlülük tələb edir
- Qeyd: Flags tapanda dərhal screenshot al!

---

## 🛠️ Əsas Tool-lar Siyahısı

| Kateqoriya | Tool-lar |
|---|---|
| **C2 Framework** | Covenant, Havoc, Sliver, Metasploit |
| **AD Enumeration** | BloodHound, PowerView, SharpHound, ADSearch |
| **Credential Dumping** | Mimikatz, Rubeus, Impacket Secretsdump |
| **Lateral Movement** | PsExec, Evil-WinRM, CrackMapExec |
| **Pivoting** | Chisel, Ligolo-ng, SSH, Netsh |
| **AV Bypass** | Donut, SysWhispers, Confuser, Scarecrow |
| **Kerberos Attack** | Rubeus, Impacket (GetUserSPNs, GetNPUsers) |
| **Shellcode** | msfvenom, Donut, Custom C# |
| **Recon (Linux)** | LinPEAS, LinEnum |
| **Recon (Windows)** | WinPEAS, PowerUp, Seatbelt |

---

## 📚 Tövsiyə Edilən Əlavə Resurslar

- **Hack The Box** — Windows & AD məşqlər üçün
- **Proving Grounds Practice** — OSCP/OSEP üçün OffSec laboratoriyalar
- **TryHackMe** — AV bypass və AD path-ları
- **VX-Underground** — Malware texnikaları üçün
- **GitHub: S3cur3Th1sSh1t** — AV bypass PoC-ları
- **GitHub: GhostPack** — Rubeus, Seatbelt, SharpDPAPI
- **ired.team** — Texniki deep-dive məqalələr
- **Sektor7** — Malware development kursları

---

## ⚠️ Vacib Qeydlər

> 🔑 **Ən Vacib Mövzular (İmtahanda çıxma ehtimalı yüksək):**
> 1. Process Injection (xüsusilə Early Bird APC)
> 2. AMSI + ETW bypass
> 3. Kerberoasting + AS-REP Roasting
> 4. DCSync + Golden/Silver Ticket
> 5. Pivoting (Chisel/Ligolo)
> 6. Constrained/Unconstrained Delegation

> 💡 **Strategiya:** İmtahanda hər zaman minimal artifact qoy. Log-ları nəzərə al, cleanup et, AV-dan qaç.

> ⏱️ **Vaxt İdarəetməsi:** 48 saatı 6 × 8 saatlıq bloklara böl. İlk 8 saatda network map tamamlanmalıdır.

---

*Bu roadmap PEN-300 kurs proqramına əsaslanır. Bütün texnikalar yalnız qanuni, icazəli mühitlərdə tətbiq edilməlidir.*
