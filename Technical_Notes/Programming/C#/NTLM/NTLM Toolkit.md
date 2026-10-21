# NTLM Expert Tool Kit — C# Implementation

## 📋 Table of Contents

- [Common NTLM](#-common-ntlm)
- [Protocol & Challenge Analysis](#-protocol--challenge-analysis)
- [Capture & Collection](#-capture--collection)
- [Cracking & Recovery](#-cracking--recovery)
- [Authentication & Abuse](#-authentication--abuse)
- [Relay Attacks](#-relay-attacks)
- [Coercion Engines](#-coercion-engines)
- [Kerberos ↔ NTLM Interop](#-kerberos--ntlm-interop)
- [Mitigation & Hardening Analysis](#-mitigation--hardening-analysis)
- [Post-Exploitation](#-post-exploitation)
- [Intelligence & Analysis](#-intelligence--analysis)
- [Recommended Starting Sequence](#-recommended-starting-sequence)

---

## 🔵 Common NTLM

> First contact at the protocol level — analysis of the negotiate, challenge, response structure.

| # | Tool Name | Technical Depth | What It Does |
|---|---|---|---|
| 1 | **NTLMProtocolProber** | Raw NEGOTIATE_MESSAGE → CHALLENGE_MESSAGE handshake | Determines NTLM v1/v2 support and negotiate flags |
| 2 | **NTLMVersionDetector** | NTLM security blob parsing | Distinguishes NTLMv1 vs NTLMv2 — reports separately via `NTLMSSP_NEGOTIATE_NTLM` vs `NEGOTIATE_NTLM2_KEY` flags |
| 3 | **NTLMFlagAnalyzer** | `NTLMSSP_MESSAGE_SIGNATURE` bit fields | Decodes all negotiate flags — signing, sealing, key exchange, 128-bit encryption support |
| 4 | **NTLMChallengeExtractor** | `CHALLENGE_MESSAGE` → `ServerChallenge` | Extracts the 8-byte challenge value and TargetInfo (AV_PAIR) fields |
| 5 | **NTLMTargetInfoParser** | AV_PAIR array parsing | Extracts metadata about the target — `MsvAvNbDomainName`, `MsvAvNbComputerName`, `MsvAvDnsDomainName`, `MsvAvTimestamp` |
| 6 | **NTLMSSPBlobInspector** | SPNEGO / NTLMSSP GSS-API wrapping | Parses NTLM blobs within SMB, HTTP (401), LDAP bind, MSSQL TDS |
| 7 | **NTLMUnicodeConverter** | UTF-16LE encoding | Prepares username/domain/password encoding per the NTLM specification — the basis for MD4 hash calculation |
| 8 | **NTLMAnonymousProber** | Session setup with the ANONYMOUS flag | Tests whether an anonymous NTLM session is accepted — a null-session analog |
| 9 | **NTLMSPNInspector** | `MsvAvTargetName` / SPN field | Checks the SPN returned in the challenge — for detecting alias/SPOF hosts |
| 10 | **NTLMSessionKeyProber** | `NEGOTIATE_KEY_EXCHANGE` flag | Checks key exchange support — determines whether a signed/sealed session will be established |

---

## 🟡 Protocol & Challenge Analysis

> Structural analysis of challenge/response values, crypto primitives, and hash formats.

| # | Tool Name | Technical Depth | What It Does |
|---|---|---|---|
| 11 | **NTLMv1ResponseBuilder** | DES(LM hash, challenge) | Builds the NTLMv1 response — a combination of LAN Manager + NT response |
| 12 | **NTLMv2ResponseBuilder** | HMAC-MD5 + blob | NTLMv2 hash = HMAC-MD5(NT hash, user+domain) → builds the response with challenge + blob |
| 13 | **NTProofStrCalculator** | HMAC-MD5 | NTProofStr = HMAC-MD5(NTLMv2Hash, challenge+blob) — for offline verification |
| 14 | **LMHashLegacyGenerator** | DES + uppercase truncation | Computes the legacy LM hash — demonstrates the 14-char limit and case-insensitivity weakness |
| 15 | **NTHashMD4Calculator** | MD4(UTF-16LE(password)) | Computes the NT hash — the base format for Pass-the-Hash and cracking |
| 16 | **NTLMHashFormatConverter** | Format parser | Converts between `user:domain:lm:ntlm:::` (John) ↔ `user::domain:hash:hash:hash::` (NTDS/hashcat) |
| 17 | **NTLMv2ClientChallengeBuilder** | Blob construction | Builds a complete client challenge blob with timestamp, client nonce, AV_PAIRs — for relay/replay testing |
| 18 | **NTLMNonceEntropyChecker** | Challenge sampling | Checks server challenge randomness statistics — assesses replay potential in weak RNG cases |
| 19 | **NTLMSessionSecurityValidator** | SIGNKEY derivation | Checks session base key → client/server signing key derivation — for MIC manipulation testing |
| 20 | **NTLMMessageIntegrityChecker** | MIC field | Calculates/verifies the NTLMv2 MIC (HMAC-MD5 over three messages) — tests MIC bypass cases |

---

## 🟠 Capture & Collection

> Real-time capture and storage of NTLM authentication attempts.

| # | Tool Name | Technical Depth | What It Does |
|---|---|---|---|
| 21 | **NTLMSMBListener** | Raw TCP 445 + SMB Negotiate/Session Setup | Captures incoming SMB NTLM auths, saves them in hash format |
| 22 | **NTLMHTTPListener** | TCP 80/8080 — `WWW-Authenticate: NTLM` | Captures incoming NTLM Negotiate/Authenticate headers over HTTP |
| 23 | **NTLMChallengeServer** | Custom challenge mode + `--static-challenge` | Uses a fixed challenge to make hashes match a rainbow table |
| 24 | **NTLMHashSaver** | Hashcat/John format output | Writes captured hashes to a file in hashcat format (mode 5600 NTLMv2, 5500 NetLM/NetNTLMv1) |
| 25 | **NTLMDowngradeInjector** | NEGOTIATE response manipulation | Attempts to force the client down from NTLMv2 to NTLMv1/LM — to capture a weaker response format |
| 26 | **NTLMEAPListener** | 802.1X / EAP | For capturing MSCHAPv2 (NTLM-based) hashes over IEEE 802.1X/EAP |
| 27 | **NTLMRRASProber** | VPN / RAS dial-in protocol | NTLM hash collection via RRAS authentication |
| 28 | **NTLMProxyWarrior** | HTTP proxy + `Proxy-Authenticate: NTLM` | NTLM hash capture via proxy auth |
| 29 | **NTLMFTPListener** | FTP `AUTH NTLM` | For capturing NTLM auth over FTP |
| 30 | **NTLMDatabaseIndexer** | SQLite backend | Indexes all captures by unique hash, user, and domain direction — for long-term campaign analysis |

---

## 🟣 Cracking & Recovery

> Offline recovery of captured hashes.

| # | Tool Name | Technical Depth | What It Does |
|---|---|---|---|
| 31 | **NTLMHashCracker** | Hashcat/John wrapper | Cracks NT hashes (mode 1000) using dictionary, mask, and rules |
| 32 | **NTLMv1Cracker** | mode 5500 | For cracking NetNTLMv1 hashes — a dedicated harness that handles DES |
| 33 | **NTLMv2Cracker** | mode 5600 | Hashcat orchestrator for NetNTLMv2 hashes — job queue, resume support |
| 34 | **NTLMWordlistManager** | Dictionary + rules | Builds automated crack sessions using rockyou.txt + dive.rule combinations |
| 35 | **NTLMRainbowTableProber** | Rainbow table lookup | Rainbow table lookup (pre-computed) for legacy LM/NTLMv1 hashes |
| 36 | **NTLMPassTheHashValidator** | Hash-based auth test | Tests SMB/HTTP auth with a cracked hash — checks whether PtH works |
| 37 | **NTLMPwdumpParser** | NTDS.dit / SAM dump parser | Indexes dumps in pwdump format, counts unique hashes, analyzes reuse |
| 38 | **NTLMHashReuseAnalyzer** | Cross-host hash comparison | Finds hashes captured from multiple hosts that share the same local admin password |
| 39 | **NTLMPBKDF2BruteForcer** | GPU/CPU adaptive | A hardware-adaptive brute-force planner for high-entropy hashes |

---

## ⚔️ Authentication & Abuse

> Hash reuse, delegation, key robbing.

| # | Tool Name | Protocol / Technique | What It Does |
|---|---|---|---|
| 40 | **NTLMPassTheHash** | Hash-based NTLM session setup | SMB/HTTP auth with an NT hash — login without a password |
| 41 | **NTLMPassTheHashHTTP** | `Authorization: NTLM` over HTTP | PtH over HTTP — for Exchange, MSSQL, WinRM |
| 42 | **NTLMPassTheHashMSSQL** | TDS + NTLM auth | Authenticates to MSSQL Server with a hash — xp_cmdshell lateral movement |
| 43 | **NTLMUnPACTheHash** | PKINIT U2U | Extracts the NT hash from Kerberos PKINIT — hash recovery from a certificate |
| 44 | **NTLMDomainTrustAbuse** | `lsarpc` — trust key | Cross-domain auth using trust account hashes — SIDHistory + trust combination |
| 45 | **NTLMMachineAccountAuth** | `$MACHINE.ACC` | Auth using the machine account hash — KrbRelayUp analog, for ADCS roles |
| 46 | **NTLMUnicodeSmuggling** | `MsvAvTargetName` manipulation | Exploits Unicode normalization — Cross-Protocol / hostname smuggling exploitation |

---

## 🔴 Relay Attacks

> Forwarding NTLM authentication to another host.

| # | Tool Name | Protocol / Technique | What It Does |
|---|---|---|---|
| 47 | **NTLMSMBRelay** | SMB ↔ SMB | Relays incoming SMB NTLM auth to another SMB host — the classic relay |
| 48 | **NTLMSMBtoLDAPRelay** | SMB → LDAP | Relays SMB to LDAP — admin operations on a DC with signing disabled |
| 49 | **NTLMSMBtoLDAPSRelay** | SMB → LDAPS | LDAPS (with signing) — with channel binding bypass attempts |
| 50 | **NTLMSMBtoHTTPRelay** | SMB → HTTP/Exchange | Relay over Exchange EWS, Autodiscover — signed HTTP obligations |
| 51 | **NTLMSMBtoMSSQLRelay** | SMB → TDS | Relay via MSSQL Server — up to xp_cmdshell RCE |
| 52 | **NTLMSMBtoIMAPRelay** | SMB → IMAP | IMAP STARTTLS relay variant |
| 53 | **NTLMDNSZombieRelay** | DNS poisoning + multi-relay | A relay farm collecting multiple auths via MDNS/LLMNR poisoning |
| 54 | **NTLMRelayMachineAccount** | Relay → Self | Relays to a machine account to create a domain object (Resource-Based Constrained Delegation exploitation) |
| 55 | **NTLMSigningEnforcementBypass** | Attempts to bypass SMB signing = required | Tests relay attempts with custom implementations under enforced signing |
| 56 | **NTLMEPAChannelBindingProber** | Channel Binding Token | Checks for Extended Protection for Authentication CBT (binding bypass check) |

---

## ⚡ Coercion Engines

> Forcing a target host to send NTLM auth to the attacker.

| # | Tool Name | Protocol / Technique | What It Does |
|---|---|---|---|
| 57 | **NTLMSpoolerCoercer** | `MS-RPRN` — `RpcRemoteFindFirstPrinterChangeNotification` | Print Spooler coercion — the classic approach |
| 58 | **NTLMPetitPotam** | `MS-EFSRPC` — `EfsRpcOpenFileRaw` | EFSRPC coercion — powerful in combination with ADCS (NTLM relay) |
| 59 | **NTLMDFSCoercer** | `MS-DFSNM` — `NetrDfsAddStdRoot` | DFS coercion — an alternative when Spooler is patched |
| 60 | **NTLMShadowCoercer** | `MS-FSRVP` — `IsPathSupported` | File Server Remote VSS protocol coercion |
| 61 | **NTLMCertCoercer** | `MS-CES` — ADCS endpoint | Coercion via ADCS endpoints |
| 62 | **NTLMSmartCardCoercer** | `MS-PAR` / `MS-RPRN` variant | Print System Asynchronous Remoting coercion |
| 63 | **NTLMLLMNRSpoofer** | UDP 5355 LLMNR poisoning | Spoofs responses to LLMNR queries — target sends NTLM auth to the attacker host |
| 64 | **NTLMNBNSResponder** | UDP 137 NBNS poisoning | NetBIOS Name Service poisoning |
| 65 | **NTLMDHCPv6Spoofer** | DHCPv6 + DNS takeover | Spoofs IPv6 DHCP to register itself as a DNS server — combined with poisoning |
| 66 | **NTLMWPADProxySpoofer** | WPAD discovery hijack | Hijacks the WPAD hostname to collect NTLM hashes via proxy auth |
| 67 | **NTLMDNSPoisoner** | Raw UDP 53 | Poisoned DNS records — resolves to the attacker's IP for the target |
| 68 | **NTLMDot11Captiver** | Wi-Fi captive portal + NTLM | Captive portal NTLM capture for wireless clients |

---

## 🟢 Kerberos ↔ NTLM Interop

> Kerberos-NTLM relationships, delegation, and cross-protocol analysis.

| # | Tool Name | Technical Depth | What It Does |
|---|---|---|---|
| 69 | **NTLMKerberosDowngradeProber** | SPNEGO negotiation | Tests the Kerberos → NTLM fallback — whether NTLM activates when the KDC is unreachable |
| 70 | **NTLMUnconstrainedDelegationMonitor** | TGT capture | Captures TGTs arriving via unconstrained delegation (for NTLM-related hash reuse) |
| 71 | **NTLMS4U2ProxyAnalyzer** | Resource-Based Constrained Delegation | Checks cases where RBCD works together with NTLM relay |
| 72 | **NTLMKerbRoastInterceptor** | AS-REP roasting | Accounts without pre-auth in hash-less NTLM auth from AS-REP responses |
| 73 | **NTLMUsernameEnumeration** | NTLM status codes | Enumerates usernames via the `STATUS_LOGON_FAILURE` vs `STATUS_ACCOUNT_DISABLED` difference |
| 74 | **NTLMDomainTrustDelegation** | `LsarQueryTrustedDomainInfo` | Checks the NTLM directionality of trusts — allowed/disallowed |
| 75 | **NTLMCrossProtocolRelayAnalyzer** | Protocol compatibility matrix | Calculates which protocols can be relayed to which others (SMB→HTTP, HTTP→LDAP, etc.) |

---

## 🔶 Mitigation & Hardening Analysis

> Checking the protection status against NTLM exploitation across the domain.

| # | Tool Name | Technical Depth | What It Does |
|---|---|---|---|
| 76 | **NTLMSigningChecker** | `srvsvc` + SMB Negotiate | Checks whether SMB signing is required across the domain |
| 77 | **NTLMLDAPSigningChecker** | LDAP bind options | Status of LDAP/LDAPS signing and channel binding enforcement |
| 78 | **NTLMRestrictionPolicyReader** | `LsarQueryInformationPolicy` — `PolicyNtAuthentication` | Checks RestrictNTLM policies — at the domain/DC level |
| 79 | **NTLMAuditingPolicyReader** | Audit policy | Configuration of audit events (4624, 4625) for NTLM exploitation |
| 80 | **NTLMChannelBindingChecker** | `LsapQueryDomainInformation` | Extended Protection / Channel Binding policies |
| 81 | **NTLMSMBv1DisabledChecker** | SMB Negotiate | Whether SMBv1 is disabled — a prerequisite for NTLMv1 exploitation |
| 82 | **NTLMPrivilegedAccountFinder** | LDAP + hash reuse analysis | Finds privileged accounts exposed to NTLM exploitation |
| 83 | **NTLMMitigationRecommender** | Policy templates | Generates recommendations for NTLM blocking, SMB signing, EP Auth based on found weaknesses |

---

## 🔧 Post-Exploitation

> Lateral movement and persistence via NTLM after establishing a foothold.

| # | Tool Name | Protocol / Technique | What It Does |
|---|---|---|---|
| 84 | **NTLMLocalAdminPivot** | Pass-the-Hash + `svcctl` | Lateral move by creating a service on another host using a local admin hash |
| 85 | **NTLMTokenImpersonation** | `LogonUser` + `DuplicateTokenEx` | NTLM session token impersonation — executing code in the user's context |
| 86 | **NTLMCredentialHarvester** | lsass dump + NT hash extraction | Collects local NT hashes — preparation for PtH |
| 87 | **NTLMPasswordSprayValidator** | Lockout-aware spray | NTLM auth spraying — at a safe rate consistent with lockout policy |
| 88 | **NTLMPersistentRelay** | Scheduled task / service | Keeps the relay listener persistent |
| 89 | **NTLMADCSAbuse** | `MS-WCCE` + relay | ADCS cert request via NTLM relay — obtaining an admin-equivalent certificate |
| 90 | **NTLMSilverTicketBridge** | NT hash from krbtgt | Forges a TGS from an NT hash — creates a Silver Ticket |
| 91 | **NTLMGoldenCredentialForge** | krbtgt NT hash | Creates a Golden Ticket — for NTLM/NT hash reuse |
| 92 | **NTLMShadowCredentialsAbuse** | `msDS-KeyCredentialLink` | Writes Shadow Credentials + extracts the NT hash via UnPACTheHash |

---

## 📊 Intelligence & Analysis

> Analysis of collected NTLM data, password reuse modeling, attack path graphing.

| # | Tool Name | Technical Depth | What It Does |
|---|---|---|---|
| 93 | **NTLMHashReuseMapper** | Graph analysis | Builds an attack path graph showing which users reuse the same local admin password across which hosts |
| 94 | **NTLMChallengeDistributionAnalyzer** | Statistical analysis | Analyzes the distribution of challenges by direction — which servers accept NTLM |
| 95 | **NTLMRelayAttackPathPlanner** | Graph + rules engine | Automatically computes a path for a relay attack — coercion → relay → ADCS/RBCD chain |
| 96 | **NTLMEventLogCorrelator** | Windows Event Log parsing | Correlates traces of NTLM exploitation in events 4624/4625/4648/4776 |
| 97 | **NTLMReportGenerator** | Markdown/HTML export | Exports all findings in a professional pentest report format |
| 98 | **NTLMHoneypotDetector** | Fingerprint analysis | Checks whether the target is a honeypot — via challenge behavior and response times |
| 99 | **NTLMNoiseSimulator** | Fake auth generator | Detection tests for blue teams — whether fake NTLM auths trigger SIEM alerts |
| 100 | **NTLMKillChainMapper** | MITRE ATT&CK mapping | Maps found weaknesses to MITRE ATT&CK technique IDs (T1550.002 Pass the Hash, T1187 Forced Authentication, T1557.001 LLMNR/NBT-NS Poisoning) |
