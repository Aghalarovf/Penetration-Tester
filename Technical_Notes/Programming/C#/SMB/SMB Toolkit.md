# SMB Expert Tool List — C# Implementation

> **Məqsəd:** SMB protokolunun reconnaissance, enumeration və post-exploitation mərhələlərini əhatə edən expert-level C# alətlər toplusu.  
> **Ardıcıllıq:** Hər kateqoriya əvvəlkinin üzərinə qurulur. Sırayla get.

---

## 📋 Mündəricat

- [Common SMB](#-common-smb)
- [System Information](#-system-information)
- [Domain Information](#-domain-information)
- [DC (Domain Controller)](#-dc-domain-controller)
- [Enumeration](#-enumeration)
- [Object Enumeration](#-object-enumeration)
- [Vulnerabilities](#-vulnerabilities)
- [Share & File Reconnaissance](#-share--file-reconnaissance)
- [Authentication & Relay](#-authentication--relay)
- [Post-Exploitation](#-post-exploitation)
- [Intelligence & Analysis](#-intelligence--analysis)
- [Başlamaq üçün ardıcıllıq](#-başlamaq-üçün-ardıcıllıq)

---

## 🔵 Common SMB

> Heç bir credential olmadan protokol səviyyəsində ilk təmas — dialect, signing, capability, fingerprint.

| # | Alət adı | Texniki dərinlik | Nə edir |
|---|---|---|---|
| 1 | **SMBDialectProber** | `TcpClient` → manual SMB Negotiate paketi | SMBv1/v2/v3 dialect dəstəyini, max dialect-i müəyyənləşdirir |
| 2 | **SMBSigningAuditor** | SMB Negotiate response flag-ları | `SecurityMode` flag-ını oxuyur — signing required/enabled/disabled ayrı-ayrı report edir |
| 3 | **SMBCapabilityScanner** | SMBv3 Negotiate response | Compression, encryption, multi-channel capability-lərini tapır |
| 4 | **SMBv1Detector** | `TcpClient` → yalnız `PC NETWORK PROGRAM 1.0` dialect | SMBv1-in aktiv olub-olmadığını müəyyən edir — EternalBlue/WannaCry üçün ilk şərt |
| 5 | **SMBEncryptionChecker** | SMBv3 `GLOBAL_CAP_ENCRYPTION` flag | Encryption required vs. supported fərqini ayırır — relay hücumuna qarşı qorunmanı müəyyən edir |
| 6 | **SMBPortMapper** | TCP 445 + TCP 139 | Hər iki SMB portunu xəritələyir — NetBIOS over TCP/IP (139) vs Direct SMB (445) |
| 7 | **SMBDialectDowngradeChecker** | `NEGOTIATE` request-ə yalnız köhnə dialektlər | Server-i daha zəif dialektə downgrade etmək mümkündürmü yoxlayır |
| 8 | **GuestAccessProber** | `username: Guest`, `password: boş` | Guest hesabı ilə autentifikasiya cəhdi — admin share-lərə çatmaq mümkündürmü |
| 9 | **SMBServerStringExtractor** | `NEGOTIATE` response → `ServerString` sahəsi | OS versiyası, service pack, build number çıxarır — honeypot aşkarlamaq üçün də istifadə olunur |
| 10 | **SMBTimeoutBehaviorProber** | Yarımçıq `NEGOTIATE` paketi | Timeout dəyərləri, TCP RST vs FIN davranışı ilə passiv OS fingerprint |

---

## 🟡 System Information

> Host səviyyəsində məlumat toplama — OS, servis, şəbəkə, patch vəziyyəti, RPC infrastrukturu.

| # | Alət adı | Texniki dərinlik | Nə edir |
|---|---|---|---|
| 11 | **NetBIOSNameScanner** | UDP 137 — raw `NetBIOS Name Service` query | NBNS sorğusu göndərib hostname, domain, MAC, workgroup alır |
| 12 | **SMBFingerprintMapper** | Negotiate + Session Setup error kodları | Credential olmadan OS versiyası, build number, domain adı çıxarır |
| 13 | **SMBNullSessionProber** | `\\host\IPC$` anonymous bind | Null session mümkündürmü yoxlayır, RPC endpoint-lərini siyahıya alır |
| 14 | **WKSSVCInfoGatherer** | `wkssvc` — `NetWkstaGetInfo` (Level 102) | Kompüterin adı, domain, OS versiyası, LAN kanalı sayı |
| 15 | **RPCEndpointMapper** | `epmapper` — port 135 → SMB pipe | Bütün RPC endpoint-lərini, UUID-lərini, annotasiyalarını çıxarır |
| 16 | **NamedPipeEnumerator** | `\\host\IPC$` — `NtQueryDirectoryFile` | Mövcud named pipe-ları listə alır (`svcctl`, `samr`, `lsarpc`, `netlogon`...) |
| 17 | **SystemTimeReader** | `srvsvc` — `NetRemoteTOD` | Uzaq kompüterin sistem saatını çıxarır — Kerberos 5 dəqiqəlik skew üçün vacibdir |
| 18 | **ComputerDescriptionReader** | `srvsvc` — `NetServerGetInfo` Level 101 | Admin tərəfindən yazılmış server açıqlamasını oxuyur — host rolunu dərhal açıqlayır |
| 19 | **InstalledServicesLister** | `svcctl` — `EnumServicesStatusExW` | Bütün servisləri adı, vəziyyəti, start tipi ilə listə alır |
| 20 | **AntivirusDetector** | `svcctl` + məlum AV servis adları siyahısı | Windows Defender, CrowdStrike, SentinelOne, Carbon Black, Sophos — aktiv AV/EDR tapır |
| 21 | **UACConfigReader** | `winreg` — `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System` | `EnableLUA`, `LocalAccountTokenFilterPolicy` dəyərlərini oxuyur |
| 22 | **HotfixEnumerator** | `winreg` — `Component Based Servicing\Packages` | Quraşdırılmış KB patch-lərini listə alır — PrintNightmare, ZeroLogon, PetitPotam patch-lərini yoxlayır |
| 23 | **NetworkInterfaceEnumerator** | `winreg` — `HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces` | Bütün network interface-ləri IP, subnet, gateway, DNS ilə çıxarır — dual-homed host-ları tapır |
| 24 | **WMIAccessProber** | `\\host\root\cimv2` — `IWbemServices::ExecQuery` | WMI-nin uzaqdan əlçatan olub-olmadığını test edir — lateral movement vektoru kimi qiymətləndirir |

---

## 🟠 Domain Information

> Domain siyasəti, SID məlumatları, functional level, trust və LSA üzərindən domain-ə aid kəşfiyyat.

| # | Alət adı | RPC Interface / Protokol | Nə edir |
|---|---|---|---|
| 25 | **LSARPolicyReader** | `lsarpc` — `LsarQueryInformationPolicy` | Audit policy, domain SID, LSA secrets adları oxunur |
| 26 | **LSARSIDResolver** | `lsarpc` — `LsarLookupNames` / `LsarLookupSids` | SID ↔ Username qarşılıqlı çevirmə (bulk) |
| 27 | **DomainPasswordPolicyReader** | `samr` — `SamrQueryInformationDomain` (Level 1) | Min uzunluq, lockout threshold, lockout duration — spray etməzdən əvvəl mütləq lazımdır |
| 28 | **DomainFunctionalLevelReader** | `LDAP` — `domainFunctionality`, `forestFunctionality` | Domain/Forest/DC functional level — köhnə level = RC4 default, NTLM v1 aktiv ola bilər |
| 29 | **TrustRelationshipAnalyzer** | `lsarpc` — `LsarEnumerateTrustedDomains` + `LsarQueryTrustedDomainInfo` | Trust tipi, istiqamət, SID filtering, transitivity — cross-forest attack path analizi |
| 30 | **DomainSIDExtractor** | `lsarpc` — `LsarQueryInformationPolicy` (PolicyAccountDomainInformation) | Domain SID-ini `S-1-5-21-X-X-X` formatında çıxarır — Golden/Silver Ticket üçün mütləq lazımdır |
| 31 | **LSASecretNameDumper** | `lsarpc` — `LsarEnumeratePrivileges` + secret key adları | LSA secret adlarını listə alır — `_SC_*` servis credential-larını, `$MACHINE.ACC` machine account-u göstərir |

---

## 🔴 DC (Domain Controller)

> Domain Controller-i locate etmə, rol müəyyənləşdirmə, servis vəziyyəti və replikasiya analizi.

| # | Alət adı | Texniki dərinlik | Nə edir |
|---|---|---|---|
| 32 | **DCLocatorSMB** | UDP 389 LDAP ping + SMB portcheck | Domain Controller-i SMB üzərindən locate edir (`_ldap._tcp.dc._msdcs`) |
| 33 | **LSARPolicyReader** | `lsarpc` — `LsarQueryInformationPolicy` | DC-nin audit policy-sini, domain SID-ini, LSA secrets adlarını oxuyur |
| 34 | **FSMORoleIdentifier** | `LDAP` — `fSMORoleOwner` atributu | 5 FSMO rolunun hansı DC-də olduğunu tapır — PDC Emulator hücumda ilk hədəfdir |
| 35 | **DCRoleChecker** | `LDAP` — `userAccountControl` + `msDS-IsGC`, `msDS-IsRODC` | DC rolunu müəyyən edir — Primary DC, Global Catalog, RODC fərqini ayırır |
| 36 | **DCReplicationPartnerEnum** | `MS-DRSR` — `DsReplicaGetInfo` | DC replication partner-lərini, son replikasiya vaxtını çıxarır |
| 37 | **NetlogonSecureChannelProber** | `netlogon` — `NetrServerAuthenticate3` | ZeroLogon (CVE-2020-1472) üçün ön yoxlama — exploit etmir, yalnız vulnerable olub-olmadığını yoxlayır |
| 38 | **DCServiceStatusChecker** | `svcctl` — `QueryServiceStatusEx` | NTDS, KDC, Netlogon, DFSR, DNS, W32Time servis vəziyyətlərini yoxlayır |
| 39 | **PrintSpoolerStatusChecker** | `svcctl` — `QueryServiceStatus` → `Spooler` | DC-də Print Spooler aktiv mi — PrintNightmare və coercion hücumları üçün ilk şərt |

---

## 🟢 Enumeration

> SMB üzərindən aktiv sessiyalar, share-lər və bağlantıların siyahıya alınması.

| # | Alət adı | RPC Interface | Nə edir |
|---|---|---|---|
| 40 | **SRVSVCShareEnum** | `srvsvc` — `NetShareEnumAll` (Level 2) | Share-ləri adı, yolu, şərhi, max/current connection sayı ilə çıxarır |
| 41 | **SRVSVCSessionEnum** | `srvsvc` — `NetSessionEnum` (Level 10) | Aktiv SMB sessiyaları — kim, hansı kompüterdən, neçə saniyədir |
| 42 | **SRVSVCConnectionEnum** | `srvsvc` — `NetConnectionEnum` | Hansı share-ə kim connected-dir, PID ilə birlikdə |
| 43 | **DFSShareMapper** | `netdfs` — `NetrDfsEnum` | Distributed File System share-lərini və referral target-larını tapır |

---

## 🟣 Object Enumeration

> İstifadəçi, qrup və hesab obyektlərinin RPC üzərindən siyahıya alınması.

| # | Alət adı | RPC Interface | Nə edir |
|---|---|---|---|
| 44 | **WKSSVCUserEnum** | `wkssvc` — `NetWkstaUserEnum` | Kompüterdə hal-hazırda login olan bütün istifadəçilər (domain + local) |
| 45 | **SAMRUserEnumerator** | `samr` — `SamrEnumerateUsersInDomain` | SAM RPC üzərindən lokal istifadəçiləri enumerate edir (null session-la da işləyə bilər) |
| 46 | **SAMRGroupEnumerator** | `samr` — `SamrEnumerateGroupsInDomain` | Lokal qrupları enumerate edir |
| 47 | **SAMRLocalAdminDumper** | `samr` — `SamrGetMembersInGroup` | Administrators qrupunun üzvlərini tam məlumatla çıxarır |

---

## 🔶 Vulnerabilities

> Patch edilməmiş zəifliklərin SMB protokolu üzərindən yoxlanması.

| # | Alət adı | CVE / Texnika | Nə edir |
|---|---|---|---|
| 48 | **SMBGhostChecker** | CVE-2020-0796 — compression header | SMBv3.1.1 `NEGOTIATE_CONTEXT` — CompressionCapabilities — vulnerable host-u tapır |

---

## 🗂️ Share & File Reconnaissance

> Share-lərə daxil olub məzmunu kəşf etmə, credential axtarışı.

| # | Alət adı | API / Metod | Nə edir |
|---|---|---|---|
| 49 | **ShareAccessTester** | `WNetAddConnection2` + `CreateFile` | Hər share-ə read/write/no-access testini edir, nəticəni matris kimi göstərir |
| 50 | **SensitiveFileHunter** | Recursive `FindFirstFile/FindNextFile` | Share-lərdə `*.txt, *.xml, *.config, *.kdbx, web.config, unattend.xml` axtarır |
| 51 | **SYSVOLCrawler** | `\\domain\SYSVOL` recursive walk | `Groups.xml` (GPP cpassword), login script-lər, scheduled task XML-lərini tapır |
| 52 | **GPPPasswordExtractor** | `Groups.xml` parser + AES-256 decrypt | SYSVOL-dakı `cpassword` dəyərini Microsoft-un public AES key-i ilə deşifrə edir |
| 53 | **NETLOGONScriptAnalyzer** | `\\domain\NETLOGON` crawler | Logon script-ləri yükləyir, içindəki credential-ları və UNC path-ları analiz edir |
| 54 | **ShareWriteProber** | Test file create/delete | Yazıla bilən share-lərə test faylı yazıb pozur — real write icazəsini təsdiqləyir |
| 55 | **FileMetadataExtractor** | `GetFileInformationByHandle` | Fayl tarixlərini, sahibini, ACL-ini, alternate data stream-lərini oxuyur |
| 56 | **SMBFileSearcher** | Pattern-based recursive search | Share-lərdə regex pattern ilə axtarış — məsələn `password`, `admin`, `secret` |

---

## ⚔️ Authentication & Relay

> NTLM, Pass-the-Hash, coercion və relay texnikaları.

| # | Alət adı | Protokol / Texnika | Nə edir |
|---|---|---|---|
| 57 | **NTLMAuthTester** | `WNetAddConnection2` + NTLM | NTLM hash ilə SMB autentifikasiyasını test edir (Pass-the-Hash) |
| 58 | **SMBRelayListener** | Raw TCP 445 + SMB handshake | Gələn NTLM autentifikasiyasını başqa host-a relay edir |
| 59 | **PrintSpoolerCoercer** | `MS-RPRN` — `RpcRemoteFindFirstPrinterChangeNotification` | Print spooler-i attacker host-a NTLM auth göndərməyə məcbur edir |
| 60 | **PetitPotamCoercer** | `MS-EFSRPC` — `EfsRpcOpenFileRaw` | EFSRPC üzərindən DC-ni autentifikasiya etməyə coerce edir |
| 61 | **DFSCoercer** | `MS-DFSNM` — `NetrDfsAddStdRoot` | DFS namespace RPC üzərindən coercion (PrintSpooler-ə alternativ) |
| 62 | **SMBAuthCapturer** | TCP listener + NTLMv2 challenge/response | Gələn NTLMv2 hash-ləri capture edib `hashcat` formatında saxlayır |

---

## 🔧 Post-Exploitation

> Sistemdə foothold əldə etdikdən sonra SMB üzərindən lateral movement və kalıcılıq.

| # | Alət adı | API / Protokol | Nə edir |
|---|---|---|---|
| 63 | **RemoteServiceCreator** | `svcctl` RPC — `CreateServiceW` | Uzaq kompüterdə servis yaradır, shellcode-u servis kimi işlədir, silinir |
| 64 | **RemoteServiceManager** | `svcctl` — `QueryServiceStatus/ChangeServiceConfig` | Mövcud servisləri listə alır, başladır, dayandırır, konfiqurasiya dəyişir |
| 65 | **RemoteRegistryReader** | `winreg` RPC — `RegConnectRegistry` | Uzaq kompüterin registry-sini SMB üzərindən oxuyur |
| 66 | **RemoteRegistryWriter** | `winreg` — `RegSetValueEx` | Uzaq registry-yə dəyər yazır (persistence, config dəyişikliyi) |
| 67 | **SAMHiveExtractor** | `winreg` — `RegSaveKey` via SMB | SAM + SYSTEM hive-larını uzaqdan çıxarır, secretsdump üçün hazırlayır |
| 68 | **RemoteFileDeployer** | `CreateFile` UNC + `WriteFile` | Uzaq share-ə fayl yazır — payload deploy etmək üçün |
| 69 | **RemoteFileEraser** | `DeleteFile` over SMB | İz silmək üçün uzaq host-dan fayl silir |
| 70 | **VSShadowAccessor** | `\\host\\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy*` | Volume Shadow Copy üzərindən `ntds.dit` + SYSTEM hive-ını kopyalayır |
| 71 | **TaskSchedulerSMB** | `atsvc` / `ITaskSchedulerV2` RPC | Uzaq kompüterdə scheduled task yaradır — lateral movement üçün |
| 72 | **SCCMShareExplorer** | `\\SCCM\SMSPKG*` share crawler | SCCM deployment share-lərini axtarır, içindəki credential-ları tapır |

---

## 🧠 Intelligence & Analysis

> Yığılmış bütün məlumatları birləşdirib hücum yolu çəkmə və hesabat hazırlama.

| # | Alət adı | Nə edir |
|---|---|---|
| 73 | **SMBAttackSurfaceMapper** | Bütün kateqoriyaların nəticələrini birləşdirib attack surface xəritəsi çıxarır — signing yoxdur + null session = relay candidate |
| 74 | **LateralMovementPlanner** | Session enum + local admin tapıntılarını birləşdirib "user X, host A-da login-dir və host B-də local admin-dir" zənciri qurur |
| 75 | **SMBTimestampAnalyzer** | Share-dəki faylların yaradılma/dəyişdirilmə tarixlərini analiz edib ən son aktiv faylları tapır |
| 76 | **CredentialCorrelator** | GPP passwords, NETLOGON script-lər, sensitive file-lardakı tapıntıları birləşdirib credential siyahısı hazırlayır |
| 77 | **SMBReconReporter** | Bütün tool-ların JSON output-larını oxuyub HTML report + attack path summary çıxarır |

---

## 📊 Statistika

| Kateqoriya | Tool sayı |
|---|---|
| 🔵 Common SMB | 10 |
| 🟡 System Information | 14 |
| 🟠 Domain Information | 7 |
| 🔴 DC | 8 |
| 🟢 Enumeration | 4 |
| 🟣 Object Enumeration | 4 |
| 🔶 Vulnerabilities | 1 |
| 🗂️ Share & File | 8 |
| ⚔️ Auth & Relay | 6 |
| 🔧 Post-Exploitation | 10 |
| 🧠 Intelligence | 5 |
| **Yekun** | **77** |

---

## 🚀 Başlamaq üçün ardıcıllıq

### Bu həftə — Common SMB + System Info
```
#1  SMBDialectProber          → raw TCP + manual negotiate paketi
#2  SMBSigningAuditor         → SecurityMode flag-ları
#4  SMBv1Detector             → yalnız PC NETWORK PROGRAM 1.0 dialect
#11 NetBIOSNameScanner        → UDP 137 raw socket
#17 SystemTimeReader          → NetRemoteTOD
```

### Növbəti həftə — Domain + DC
```
#25 LSARPolicyReader          → lsarpc bind + policy oxu
#27 DomainPasswordPolicyReader→ lockout threshold — spray əvvəli
#30 DomainSIDExtractor        → S-1-5-21-X-X-X formatında
#34 FSMORoleIdentifier        → 5 FSMO rolu
#37 NetlogonSecureChannelProber → ZeroLogon ön yoxlama
```

### Sonra — Enumeration + Share Intel
```
#40 SRVSVCShareEnum           → IPC$ bind + NetShareEnumAll
#51 SYSVOLCrawler             → Groups.xml tapıcı
#52 GPPPasswordExtractor      → cpassword AES decrypt
#20 AntivirusDetector         → AV/EDR servis adları
```

### Daha sonra — Auth & Relay
```
#57 NTLMAuthTester            → Pass-the-Hash SMB testi
#59 PrintSpoolerCoercer       → MS-RPRN coercion
#62 SMBAuthCapturer           → NTLMv2 hash capture
```

### Final — Post-Exploitation
```
#63 RemoteServiceCreator      → SCM lateral movement
#67 SAMHiveExtractor          → uzaqdan SAM dump
#70 VSShadowAccessor          → ntds.dit extraction
#77 SMBReconReporter          → HTML report generator
```

---

## 📚 Faydalı Referanslar

| Mənbə | Protokol |
|---|---|
| MS-SMB2 Spesifikasiyası | `[MS-SMB2]` — docs.microsoft.com |
| MS-RPCE (RPC over SMB) | `[MS-RPCE]` — docs.microsoft.com |
| MS-SRVS (srvsvc) | `[MS-SRVS]` — docs.microsoft.com |
| MS-WKST (wkssvc) | `[MS-WKST]` — docs.microsoft.com |
| MS-SAMR | `[MS-SAMR]` — docs.microsoft.com |
| MS-LSAD / MS-LSAT | `[MS-LSAD]` — docs.microsoft.com |
| MS-RPRN (Print Spooler) | `[MS-RPRN]` — docs.microsoft.com |
| MS-DRSR (Replication) | `[MS-DRSR]` — docs.microsoft.com |
| Impacket (Python reference) | github.com/fortra/impacket |

---

> **Qeyd:** Bütün alətlər yalnız icazəniz olan mühitlərdə — öz lab-ınızda, CTF-də və ya rəsmi pentest müqaviləsi olan sistemlərdə istifadə edilməlidir.
