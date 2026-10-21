# SMB Expert Tool List — C# Implementation

---

## 📋 Table of Contents

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
- [Recommended Starting Sequence](#-recommended-starting-sequence)

---

## 🔵 Common SMB

> First contact at the protocol level without any credentials — dialect, signing, capability, fingerprint.

| # | Tool Name | Technical Depth | What It Does |
|---|---|---|---|
| 1 | **SMBDialectProber** | `TcpClient` → manual SMB Negotiate packet | Determines SMBv1/v2/v3 dialect support and the maximum dialect |
| 2 | **SMBSigningAuditor** | SMB Negotiate response flags | Reads the `SecurityMode` flag — reports signing required/enabled/disabled separately |
| 3 | **SMBCapabilityScanner** | SMBv3 Negotiate response | Finds compression, encryption, and multi-channel capabilities |
| 4 | **SMBv1Detector** | `TcpClient` → only `PC NETWORK PROGRAM 1.0` dialect | Determines whether SMBv1 is active — the first condition for EternalBlue/WannaCry |
| 5 | **SMBEncryptionChecker** | SMBv3 `GLOBAL_CAP_ENCRYPTION` flag | Distinguishes encryption required vs. supported — determines protection against relay attacks |
| 6 | **SMBPortMapper** | TCP 445 + TCP 139 | Maps both SMB ports — NetBIOS over TCP/IP (139) vs Direct SMB (445) |
| 7 | **SMBDialectDowngradeChecker** | `NEGOTIATE` request with only older dialects | Checks whether the server can be downgraded to a weaker dialect |
| 8 | **GuestAccessProber** | `username: Guest`, `password: blank` | Attempts authentication with the guest account — checks whether admin shares can be reached |
| 9 | **SMBServerStringExtractor** | `NEGOTIATE` response → `ServerString` field | Extracts OS version, service pack, build number — also used for honeypot detection |
| 10 | **SMBTimeoutBehaviorProber** | Incomplete `NEGOTIATE` packet | Passive OS fingerprinting via timeout values, TCP RST vs FIN behavior |

---

## 🟡 System Information

> Host-level information gathering — OS, services, network, patch status, RPC infrastructure.

| # | Tool Name | Technical Depth | What It Does |
|---|---|---|---|
| 11 | **NetBIOSNameScanner** | UDP 137 — raw `NetBIOS Name Service` query | Sends an NBNS query to retrieve hostname, domain, MAC, workgroup |
| 12 | **SMBFingerprintMapper** | Negotiate + Session Setup error codes | Extracts OS version, build number, domain name without credentials |
| 13 | **SMBNullSessionProber** | `\\host\IPC$` anonymous bind | Checks whether a null session is possible, lists RPC endpoints |
| 14 | **WKSSVCInfoGatherer** | `wkssvc` — `NetWkstaGetInfo` (Level 102) | Computer name, domain, OS version, number of LAN channels |
| 15 | **RPCEndpointMapper** | `epmapper` — port 135 → SMB pipe | Extracts all RPC endpoints, their UUIDs, and annotations |
| 16 | **NamedPipeEnumerator** | `\\host\IPC$` — `NtQueryDirectoryFile` | Lists available named pipes (`svcctl`, `samr`, `lsarpc`, `netlogon`...) |
| 17 | **SystemTimeReader** | `srvsvc` — `NetRemoteTOD` | Retrieves the remote computer's system clock — important due to Kerberos' 5-minute skew tolerance |
| 18 | **ComputerDescriptionReader** | `srvsvc` — `NetServerGetInfo` Level 101 | Reads the server description written by an admin — immediately reveals the host's role |
| 19 | **InstalledServicesLister** | `svcctl` — `EnumServicesStatusExW` | Lists all services with name, status, start type |
| 20 | **AntivirusDetector** | `svcctl` + list of known AV service names | Finds active AV/EDR — Windows Defender, CrowdStrike, SentinelOne, Carbon Black, Sophos |
| 21 | **UACConfigReader** | `winreg` — `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System` | Reads `EnableLUA`, `LocalAccountTokenFilterPolicy` values |
| 22 | **HotfixEnumerator** | `winreg` — `Component Based Servicing\Packages` | Lists installed KB patches — checks patches for PrintNightmare, ZeroLogon, PetitPotam |
| 23 | **NetworkInterfaceEnumerator** | `winreg` — `HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces` | Extracts all network interfaces with IP, subnet, gateway, DNS — finds dual-homed hosts |
| 24 | **WMIAccessProber** | `\\host\root\cimv2` — `IWbemServices::ExecQuery` | Tests whether WMI is remotely accessible — evaluates it as a lateral movement vector |

---

## 🟠 Domain Information

> Domain policy, SID information, functional level, trust, and domain-related reconnaissance via LSA.

| # | Tool Name | RPC Interface / Protocol | What It Does |
|---|---|---|---|
| 25 | **LSARPolicyReader** | `lsarpc` — `LsarQueryInformationPolicy` | Reads audit policy, domain SID, LSA secret names |
| 26 | **LSARSIDResolver** | `lsarpc` — `LsarLookupNames` / `LsarLookupSids` | Bulk SID ↔ Username mutual conversion |
| 27 | **DomainPasswordPolicyReader** | `samr` — `SamrQueryInformationDomain` (Level 1) | Minimum length, lockout threshold, lockout duration — essential before spraying |
| 28 | **DomainFunctionalLevelReader** | `LDAP` — `domainFunctionality`, `forestFunctionality` | Domain/Forest/DC functional level — an older level may mean RC4 is default, NTLMv1 may be active |
| 29 | **TrustRelationshipAnalyzer** | `lsarpc` — `LsarEnumerateTrustedDomains` + `LsarQueryTrustedDomainInfo` | Trust type, direction, SID filtering, transitivity — cross-forest attack path analysis |
| 30 | **DomainSIDExtractor** | `lsarpc` — `LsarQueryInformationPolicy` (PolicyAccountDomainInformation) | Extracts the domain SID in `S-1-5-21-X-X-X` format — essential for Golden/Silver Ticket |
| 31 | **LSASecretNameDumper** | `lsarpc` — `LsarEnumeratePrivileges` + secret key names | Lists LSA secret names — shows `_SC_*` service credentials, `$MACHINE.ACC` machine account |

---

## 🔴 DC (Domain Controller)

> Locating the Domain Controller, determining its role, service status, and replication analysis.

| # | Tool Name | Technical Depth | What It Does |
|---|---|---|---|
| 32 | **DCLocatorSMB** | UDP 389 LDAP ping + SMB port check | Locates the Domain Controller via SMB (`_ldap._tcp.dc._msdcs`) |
| 33 | **LSARPolicyReader** | `lsarpc` — `LsarQueryInformationPolicy` | Reads the DC's audit policy, domain SID, LSA secret names |
| 34 | **FSMORoleIdentifier** | `LDAP` — `fSMORoleOwner` attribute | Finds which DC holds each of the 5 FSMO roles — PDC Emulator is the first target in an attack |
| 35 | **DCRoleChecker** | `LDAP` — `userAccountControl` + `msDS-IsGC`, `msDS-IsRODC` | Determines DC role — distinguishes Primary DC, Global Catalog, RODC |
| 36 | **DCReplicationPartnerEnum** | `MS-DRSR` — `DsReplicaGetInfo` | Extracts DC replication partners and last replication time |
| 37 | **NetlogonSecureChannelProber** | `netlogon` — `NetrServerAuthenticate3` | Preliminary check for ZeroLogon (CVE-2020-1472) — does not exploit, only checks vulnerability |
| 38 | **DCServiceStatusChecker** | `svcctl` — `QueryServiceStatusEx` | Checks NTDS, KDC, Netlogon, DFSR, DNS, W32Time service statuses |
| 39 | **PrintSpoolerStatusChecker** | `svcctl` — `QueryServiceStatus` → `Spooler` | Whether Print Spooler is active on the DC — the first condition for PrintNightmare and coercion attacks |

---

## 🟢 Enumeration

> Listing active sessions, shares, and connections over SMB.

| # | Tool Name | RPC Interface | What It Does |
|---|---|---|---|
| 40 | **SRVSVCShareEnum** | `srvsvc` — `NetShareEnumAll` (Level 2) | Extracts shares with name, path, comment, max/current connection count |
| 41 | **SRVSVCSessionEnum** | `srvsvc` — `NetSessionEnum` (Level 10) | Active SMB sessions — who, from which computer, for how many seconds |
| 42 | **SRVSVCConnectionEnum** | `srvsvc` — `NetConnectionEnum` | Which share is connected to by whom, along with PID |
| 43 | **DFSShareMapper** | `netdfs` — `NetrDfsEnum` | Finds Distributed File System shares and referral targets |
| 44 | **ShareSecurityDescriptorReader** | `srvsvc` — `NetrShareGetInfo` (Level 502) | Reads a share's security descriptor (DACL) via RPC without physically connecting — shows who has which permission |
| 45 | **PrinterShareEnum** | `spoolss` — `EnumPrinters` | Lists printers shared over SMB — identifies PrintNightmare/coercion targets |

---

## 🟣 Object Enumeration

> Listing user, group, and account objects via RPC.

| # | Tool Name | RPC Interface | What It Does |
|---|---|---|---|
| 46 | **WKSSVCUserEnum** | `wkssvc` — `NetWkstaUserEnum` | All users (domain + local) currently logged in on the computer |
| 47 | **SAMRUserEnumerator** | `samr` — `SamrEnumerateUsersInDomain` | Enumerates local users via SAM RPC (can also work with a null session) |
| 48 | **SAMRGroupEnumerator** | `samr` — `SamrEnumerateGroupsInDomain` | Enumerates local groups |
| 49 | **SAMRLocalAdminDumper** | `samr` — `SamrGetMembersInGroup` | Extracts full information on Administrators group members |

---

## 🔶 Vulnerabilities

> Checking for unpatched vulnerabilities via the SMB protocol.

| # | Tool Name | CVE / Technique | What It Does |
|---|---|---|---|
| 50 | **SMBGhostChecker** | CVE-2020-0796 — compression header | SMBv3.1.1 `NEGOTIATE_CONTEXT` — CompressionCapabilities — finds vulnerable hosts |
| 51 | **EternalBlueChecker** | CVE-2017-0144 (MS17-010) — SMBv1 `Trans2 SESSION_SETUP` | Checks whether SMBv1 is patched against EternalBlue from a crafted packet's response (without exploiting) |
| 52 | **SMBLorisDetector** | Connection exhaustion (DoS) — partial SMB connections | Opens many partial SMB sessions to test the server's connection table limit and DoS susceptibility |
| 53 | **SMBCompoundRequestChecker** | CVE-2022-24500 class — compound request flood | Sends multiple SMB2 compound requests to check the server's vulnerability to this type of request |

---

## 🗂️ Share & File Reconnaissance

> Accessing shares to discover content and search for credentials.

| # | Tool Name | API / Method | What It Does |
|---|---|---|---|
| 54 | **ShareAccessTester** | `WNetAddConnection2` + `CreateFile` | Tests read/write/no-access for each share, displays results as a matrix |
| 55 | **SensitiveFileHunter** | Recursive `FindFirstFile/FindNextFile` | Searches shares for `*.txt, *.xml, *.config, *.kdbx, web.config, unattend.xml` |
| 56 | **SYSVOLCrawler** | `\\domain\SYSVOL` recursive walk | Finds `Groups.xml` (GPP cpassword), logon scripts, scheduled task XML files |
| 57 | **GPPPasswordExtractor** | `Groups.xml` parser + AES-256 decrypt | Decrypts the `cpassword` value in SYSVOL using Microsoft's public AES key |
| 58 | **NETLOGONScriptAnalyzer** | `\\domain\NETLOGON` crawler | Downloads logon scripts, analyzes credentials and UNC paths within them |
| 59 | **ShareWriteProber** | Test file create/delete | Writes and deletes a test file on writable shares — confirms actual write permission |
| 60 | **FileMetadataExtractor** | `GetFileInformationByHandle` | Reads file dates, owner, ACL, alternate data streams |
| 61 | **SMBFileSearcher** | Pattern-based recursive search | Regex pattern search across shares — e.g. `password`, `admin`, `secret` |

---

## ⚔️ Authentication & Relay

> NTLM, Pass-the-Hash, coercion, and relay techniques.

| # | Tool Name | Protocol / Technique | What It Does |
|---|---|---|---|
| 62 | **NTLMAuthTester** | `WNetAddConnection2` + NTLM | Tests SMB authentication with an NTLM hash (Pass-the-Hash) |
| 63 | **SMBRelayListener** | Raw TCP 445 + SMB handshake | Relays incoming NTLM authentication to another host |
| 64 | **PrintSpoolerCoercer** | `MS-RPRN` — `RpcRemoteFindFirstPrinterChangeNotification` | Forces the print spooler to send NTLM auth to the attacker's host |
| 65 | **PetitPotamCoercer** | `MS-EFSRPC` — `EfsRpcOpenFileRaw` | Coerces the DC into authenticating via EFSRPC |
| 66 | **DFSCoercer** | `MS-DFSNM` — `NetrDfsAddStdRoot` | Coercion via DFS namespace RPC (an alternative to PrintSpooler) |
| 67 | **SMBAuthCapturer** | TCP listener + NTLMv2 challenge/response | Captures incoming NTLMv2 hashes and saves them in `hashcat` format |

---

## 🔧 Post-Exploitation

> Lateral movement and persistence via SMB after a foothold has been established on a system.

| # | Tool Name | API / Protocol | What It Does |
|---|---|---|---|
| 68 | **RemoteServiceCreator** | `svcctl` RPC — `CreateServiceW` | Creates a service on the remote computer, runs shellcode as a service, then deletes it |
| 69 | **RemoteServiceManager** | `svcctl` — `QueryServiceStatus/ChangeServiceConfig` | Lists existing services, starts, stops, and reconfigures them |
| 70 | **RemoteRegistryReader** | `winreg` RPC — `RegConnectRegistry` | Reads the remote computer's registry via SMB |
| 71 | **RemoteRegistryWriter** | `winreg` — `RegSetValueEx` | Writes a value to the remote registry (persistence, config changes) |
| 72 | **SAMHiveExtractor** | `winreg` — `RegSaveKey` via SMB | Remotely extracts SAM + SYSTEM hives, prepares them for secretsdump |
| 73 | **RemoteFileDeployer** | `CreateFile` UNC + `WriteFile` | Writes a file to a remote share — for deploying a payload |
| 74 | **RemoteFileEraser** | `DeleteFile` over SMB | Deletes a file from the remote host to erase traces |
| 75 | **VSShadowAccessor** | `\\host\\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy*` | Copies `ntds.dit` + the SYSTEM hive via Volume Shadow Copy |
| 76 | **TaskSchedulerSMB** | `atsvc` / `ITaskSchedulerV2` RPC | Creates a scheduled task on the remote computer — for lateral movement |
| 77 | **SCCMShareExplorer** | `\\SCCM\SMSPKG*` share crawler | Searches SCCM deployment shares, finds credentials within them |

---

---

> **Note:** All tools must be used only in environments you are authorized to test — your own lab, a CTF, or systems covered by a formal penetration-testing agreement.
