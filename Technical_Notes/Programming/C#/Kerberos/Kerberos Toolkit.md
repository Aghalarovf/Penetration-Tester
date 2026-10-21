# Kerberos Expert Tool Kit — C# Implementation

## 📋 Table of Contents

- [Common Kerberos](#-common-kerberos)
- [Protocol & Ticket Analysis](#-protocol--ticket-analysis)
- [Enumeration](#-enumeration)
- [Attacks — User Accounts](#-attacks--user-accounts)
- [Attacks — Service Accounts](#-attacks--service-accounts)
- [Delegation Attacks](#-delegation-attacks)
- [Certificate & PKINIT Attacks](#-certificate--pkinit-attacks)
- [DC & Infrastructure Attacks](#-dc--infrastructure-attacks)
- [Ticket Forge & Manipulation](#-ticket-forge--manipulation)
- [Authentication & Abuse](#-authentication--abuse)
- [Mitigation & Hardening Analysis](#-mitigation--hardening-analysis)
- [Post-Exploitation](#-post-exploitation)
- [Intelligence & Analysis](#-intelligence--analysis)
- [Recommended Starting Sequence](#-recommended-starting-sequence)

---

## 🔵 Common Kerberos

> First contact at the protocol level — KDC reachability, AS-REQ/TGS-REQ structural analysis, fingerprinting.

| # | Tool Name | Technical Depth | What It Does |
|---|---|---|---|
| 1 | **KerberosPortProber** | TCP 88 + UDP 88 TcpClient/Socket | Checks KDC reachability over TCP/UDP — detects UDP fragmentation issues |
| 2 | **ASREQProber** | Raw AS-REQ (PA-ENC-TIMESTAMP) | Receives `KDC_ERR_PREAUTH_REQUIRED` from the KDC — confirms KDC activity + pre-auth policy |
| 3 | **KerberosRealmDiscovery** | DNS SRV + kinit analog | Realm/DC discovery from `_kerberos._tcp.dc._msdcs.domain.com` SRV records |
| 4 | **KerberosEncryptionDetector** | AS-REQ etype listing | Finds supported etypes — rc4-hmac (23), aes128 (17), aes256 (18) — RC4 default signals legacy weakness |
| 5 | **KDCErrorInterpreter** | KRB-ERROR parsing | Translates `KDC_ERR_` codes — `PREAUTH_REQUIRED(25)`, `CLIENT_NOT_FOUND(6)`, `PRINCIPAL_UNKNOWN` — basis for username enumeration |
| 6 | **KerberosClockSkewChecker** | AS-REQ timestamp | Measures the local ↔ DC time difference — exceeding the 5-minute skew limit will break tickets |
| 7 | **KerberosPACParser** | PAC structure decoding | Parses `PAC_LOGON_INFO`, `PAC_SIGNATURE_DATA`, `PAC_CREDENTIALS_INFO` fields |
| 8 | **KerberosPrincipalFormatter** | user@REALM / NT principals | Converts principal name formats — enterprise, NT, canonical name normalization |
| 9 | **KerberosReferralAnalyzer** | Cross-realm referral tickets | Parses `KDC_ERR_WRONG_REALM` + referral TGTs — maps trust topology |
| 10 | **KerberosUDPFragmentProber** | UDP 88 max packet size | Detects `KRB5KDC_ERR_RESPONSE_TOO_BIG` — determines whether TCP fallback is required |

---

## 🟡 Protocol & Ticket Analysis

> Structural analysis of tickets, crypto fields, and PAC signatures.

| # | Tool Name | Technical Depth | What It Does |
|---|---|---|---|
| 11 | **ASREPParser** | KRB-AS-REP ASN.1 DER | Extracts the ticket, encrypted part, and key fields from the AS-REP |
| 12 | **TGSREPParser** | KRB-TGS-REP ASN.1 | Extracts the service ticket, session key, and ad-tickets from the TGS-REP |
| 13 | **TicketKvnoExtractor** | Ticket kvno field | Reads the ticket version number — detects key rotation events |
| 14 | **KerberosTicketFlagAnalyzer** | Ticket flags bitfield | Decodes forwardable, renewable, initial, pre-authent, ok-as-delegate flags |
| 15 | **PACSigVerifier** | PAC_SIGNATURE_DATA | Verifies Server/Privilege Server signatures — determines whether forging is possible if the key is held |
| 16 | **SessionKeyExtractor** | AS-REP enc-part | Extracts the session key from the AS-REP enc-part — for service ticket requests |
| 17 | **KerberosAPREQInspector** | AP-REQ + authenticator | Analyzes the authenticator structure and sub-session key of existing tickets |
| 18 | **KerberosTicketLifetimeChecker** | endtime/renew-till | Calculates ticket lifetime/renewal windows — finds long-lived tickets |
| 19 | **KerberosCrossRealmTicketAnalyzer** | Cross-realm TGT | Checks the SID filtering/status of tickets obtained via cross-realm trust |
| 20 | **KerberosCipherStrengthAnalyzer** | Etype + key length | Evaluates the strength of ciphers in use — RC4 signals NT hash reuse risk |

---

## 🟠 Enumeration

> Discovery of users, services, SPNs, and delegation configurations via Kerberos.

| # | Tool Name | Technical Depth | What It Does |
|---|---|---|---|
| 21 | **KerberosUserEnum** | AS-REQ status codes | Username enumeration via the `KDC_ERR_PREAUTH_REQUIRED` vs `KDC_ERR_CLIENT_NOT_FOUND` difference (no credentials needed) |
| 22 | **KerberosUserEnumPasswordSpray** | AS-REQ + `KDC_ERR_PREAUTH_FAILED` | Pre-auth testing for username + password spray — lockout-aware |
| 23 | **SPNEnumerator** | LDAP — `servicePrincipalName` | Enumerates all SPNs in the domain — finds Kerberoasting targets |
| 24 | **SPNToUserMapper** | LDAP + sAMAccountName | Maps the SPN ↔ user account relationship — which service account carries which SPN |
| 25 | **DelegationConfigEnumerator** | LDAP — `msDS-AllowedToDelegateTo` | Finds constrained delegation configurations — along with target SPNs |
| 26 | **UnconstrainedDelegationFinder** | LDAP — `userAccountControl` & `TRUSTED_FOR_DELEGATION` | Finds accounts/DCs with unconstrained delegation active — TGT capture targets |
| 27 | **RBCDFinder** | LDAP — `msDS-AllowedToActOnBehalfOfOtherIdentity` | Enumerates Resource-Based Constrained Delegation configurations |
| 28 | **TrustedToAuthForDelegationFinder** | `TRUSTED_TO_AUTH_FOR_DELEGATION` flag | Finds accounts with protocol transition enabled — for S4U2Self |
| 29 | **KerberosServiceAccountAudit** | LDAP combination | Audits service accounts (accounts with SPNs) for password age, hash type, pre-auth status |
| 30 | **KerberosGroupMembershipMapper** | LDAP — `memberOf` | Group membership of Kerberos target accounts — how privileged accounts are reachable via Kerberos |
| 31 | **ASREPAccountFinder** | LDAP — `UF_DONT_REQUIRE_PREAUTH` | Finds accounts that don't require pre-auth — AS-REP Roasting targets |
| 32 | **KerberosTrustEnumerator** | LDAP — `trustedDomainObject` | Finds the Kerberos direction of trusts, transitivity, and forest trust status |

---

## 🔴 Attacks — User Accounts

> Kerberos weaknesses against user accounts.

| # | Tool Name | Technique | What It Does |
|---|---|---|---|
| 33 | **ASREP Roaster** | AS-REQ (no pre-auth) → enc-part | Extracts the AS-REP enc-part hash of accounts with pre-auth disabled — for offline cracking (mode 18200) |
| 34 | **Kerberoast** | TGS-REQ → service ticket | Collects TGSes for all accounts with an SPN — RC4 tickets = NT hash (mode 13100) |
| 35 | **KerberoastRC4Focus** | Etype 23 filter | Requests only RC4 tickets — maximizes cracking value |
| 36 | **KerberoastAESFocus** | Etype 17/18 | Requests AES tickets — slow to crack, but valuable for hash reuse analysis |
| 37 | **KerberosPasswordSpray** | AS-REQ pre-auth | Lockout-aware password spray consistent with the domain password policy |
| 38 | **KerberosBruteForce** | AS-REQ | Slow brute-force — Kerberos login failures on honeypots/hosting environments |
| 39 | **KerberosUserEnumerationTiming** | Response timing analysis | Username enumeration via response time differences — when status codes are masked |
| 40 | **KerberosPasswordPolicyReader** | LDAP | Reads lockout threshold/duration before spraying — a SAMR analog over LDAP |

---

## 🟣 Attacks — Service Accounts

> Capturing and exploiting service account hashes.

| # | Tool Name | Technique | What It Does |
|---|---|---|---|
| 41 | **KerberoastCracker** | Hashcat mode 13100/19600 | Hashcat orchestrator for cracking TGS tickets — both RC4 and AES |
| 42 | **KerberosServiceTicketHarvester** | TGS-REQ + krb5cc parse | Collects all service tickets and converts them to hash format |
| 43 | **SPNSquattingProber** | LDAP + SPN registration test | Finds empty SPNs — attacker can register an SPN to draw Kerberoasting to users |
| 44 | **KerberosSilverTicketForge** | Service account NT hash | Forges a TGS using the service hash rather than krbtgt — LANMAN/RC4 + AES |
| 45 | **KerberosTicketCacheParser** | LSASS / KRB5CCNAME | Reads the local ticket cache — exports existing tickets (kirbi format) |
| 46 | **KerberosKRBCCConverter** | kirbi ↔ ccache | Ticket format conversions — for Impacket/Rubeus compatibility |
| 47 | **KerberosServiceAccountPivot** | Cracked service account | Builds a PtH/PtT and TGT request chain from a service account password |
| 48 | **KerberosWeakPasswordScanner** | Service account wordlist | Spray against service accounts with a targeted wordlist (service, sql, svc prefixes) |

---

## ⚔️ Delegation Attacks

> Exploiting delegation configurations.

| # | Tool Name | Technique | What It Does |
|---|---|---|---|
| 49 | **S4U2SelfAbuser** | S4U2Self + S4U2Proxy | Obtains a ticket for any user on its own behalf via constrained delegation |
| 50 | **S4U2ProxyPivot** | S4U2Proxy + forwardable TGT | Obtains a service ticket to the delegation SPN with a forwardable TGS — admin impersonation |
| 51 | **RBCDAttacker** | Writes `msDS-AllowedToActOnBehalfOfOtherIdentity` | Writes RBCD configuration (if write access exists) — machine account control → impersonation |
| 52 | **RBCDMachineAccountForge** | Machine account SPN + S4U | Registers a created machine account SPN and completes the RBCD relay |
| 53 | **UnconstrainedDelegationListener** | TGT capture (DC + Tier-0) | Monitors an unconstrained delegation host — captures incoming TGTs (chain up to DCSync) |
| 54 | **KerberosResourceConstrainedFinder** | LDAP + ACL analysis | Finds accounts with RBCD write access — builds an attack path graph |
| 55 | **S4UTicketRogue** | S4U2Self + PAC manipulation | PAC manipulation in S4U tickets — user SID override |
| 56 | **DelegationAttackPathPlanner** | Graph analysis | Builds an attack path graph from delegation configs — who can impersonate whom |

---

## 🔶 Certificate & PKINIT Attacks

> The ADCS + PKINIT + Kerberos intersection.

| # | Tool Name | Technique | What It Does |
|---|---|---|---|
| 57 | **PKINITProber** | PA-PK-AS-REQ | Checks DC support for PKINIT — for certificate-based auth |
| 58 | **UnPACTheHash** | PKINIT U2U → PA-PAC-REQUEST | Extracts an NT hash from a certificate — used with Shadow Credentials and ESC cases |
| 59 | **ShadowCredentialsAbuser** | Writes `msDS-KeyCredentialLink` | Writes a KeyCredential, extracts the NT hash via PKINIT, and cleans up traces |
| 60 | **ESC1Abuser** | ADCS `ENROLLEE_SUPPLIES_SUBJECT` | Obtains a certificate for any user from a misconfigured template → Kerberos auth |
| 61 | **ESC2-ESC6Scanner** | ADCS template flags | Scans for dangerous template flags — AnyPurpose, EKU-less, `EDITF_ATTRIBUTESUBJECTALTNAME2` |
| 62 | **ESC8RelayProber** | ADCS NTLM relay | Checks ADCS HTTP endpoint vulnerability to NTLM relay |
| 63 | **CertToKerberosBridge** | Cert + PKINIT | Bridges an obtained certificate to a TGT — a PtH analog |

---

## 🎯 DC & Infrastructure Attacks

> Attacks via the DC, krbtgt, and infrastructure.

| # | Tool Name | Technique | What It Does |
|---|---|---|---|
| 64 | **DCSync** | MS-DRSR `DRSGetNCChanges` | Extracts all user hashes — including krbtgt — using replication rights |
| 65 | **DCSyncSingleUser** | `DRSGetNCChanges` + pmsgOut filter | Extracts only a single user's hash — reduces noise |
| 66 | **DCShadow** | MS-DRSR `DRSAddEntry` | Writes objects via a fake DC registration — SIDHistory, delegation config changes |
| 67 | **ZeroLogonChecker** | `NetrServerReqChallenge` / `NetrServerAuthenticate3` | CVE-2020-1472 — checks whether the DC account password can be reset (exploits, but with a restore option) |
| 68 | **KerberosGoldenTicketForge** | krbtgt NT hash | Forges a Golden Ticket — any user, any group, any domain |
| 69 | **KerberosDiamondTicketForge** | DCSync + krbtgt AES key | Signs a TGT with the AES key — combined with S4U for a "diamond" ticket |
| 70 | **KerberosSapphireTicketForge** | krbtgt AES key + S4U | Sapphire ticket — PAC refreshed via S4U2Self and forged |
| 71 | **krbtgtKeyIDAnalyzer** | Key rotation monitoring | Monitors krbtgt kvno and key age — detects lack of rotation in the Golden Ticket window |
| 72 | **KerberosRIDCyclingAnalyzer** | RID analysis | User enumeration on the DC via RID cycling — without DCSync |
| 73 | **KerberosGPPRefresher** | SYSVOL + gPCFileSysPath | Finds delegation/SPN configuration changes from GPOs |

---

## 🛠️ Ticket Forge & Manipulation

> Creating, modifying, and manipulating tickets and PACs.

| # | Tool Name | Technique | What It Does |
|---|---|---|---|
| 74 | **TicketPACInjector** | PAC construct + signature | Creates/modifies a PAC — GroupSID, UserSID manipulation |
| 75 | **TicketSIDHistoryInjector** | `PAC_SID_HISTORY` | Adds an Enterprise Admin SID via SIDHistory — for forged tickets |
| 76 | **TicketGroupEditor** | `PAC_LOGON_INFO` | Modifies a ticket's group membership — adds the Domain Admins SID |
| 77 | **TicketLifetimeExtender** | endtime/renew-till patch | Extends ticket lifetime — the 10-year Golden Ticket pattern |
| 78 | **TicketEncoderKRB** | ASN.1 DER encoder | Encodes tickets/AP-REQ/TGS-REQs — a raw ASN.1 builder in C# |
| 79 | **TicketPassthruValidator** | AP-REQ + service | Tests whether forged tickets are accepted by a real service |
| 80 | **TicketCrossRealmForge** | Cross-realm trust key | Forges cross-realm tickets when the inter-realm trust key is in hand |

---

## ⚔️ Authentication & Abuse

> Access and lateral movement using tickets.

| # | Tool Name | Technique | What It Does |
|---|---|---|---|
| 81 | **KerberosSMBAuth** | AP-REQ + SMB | SMB auth with a ticket — Pass-the-Ticket |
| 82 | **KerberosWinRMPTT** | AP-REQ + WinRM | WinRM access via Pass-the-Ticket |
| 83 | **KerberosHTTPPTT** | AP-REQ + HTTP | Ticket auth over HTTP — Exchange EWS |
| 84 | **KerberosLDAPPacers** | LDAP + PAC | Ticket auth over LDAP — reading domain objects |
| 85 | **KerberosMSSQLPTT** | TDS + Kerberos | Ticket auth to MSSQL Server — xp_cmdshell pivot |
| 86 | **KerberosSilverTicketSMB** | Silver TGS + SMB | CIFS auth with a Silver Ticket — local admin-level access |
| 87 | **KerberosPassTheCache** | ccache/kirbi export | Direct auth from the ticket cache — Rubeus/Impacket compatible |

---

## 🔶 Mitigation & Hardening Analysis

> Checking the protection status against Kerberos exploitation.

| # | Tool Name | Technique | What It Does |
|---|---|---|---|
| 88 | **KerberosAESOnlyChecker** | LDAP + `msDS-SupportedEncryptionTypes` | RC4 disabling status — checks AES-only policy |
| 89 | **KerberosPACSigningChecker** | PAC signature validation | PAC signature enforcement status (CVE-2021-42287 hardening) |
| 90 | **KerberosPreAuthPolicyReader** | LDAP — `userAccountControl` | Number of accounts with pre-auth disabled — AS-REP Roast risk |
| 91 | **KerberosDelegationAudit** | LDAP | Full audit report of all delegation configurations — with risk scoring |
| 92 | **KerberosADCSAudit** | LDAP + ADCS templates | Audits ADCS template risks — ESC cases |
| 93 | **KerberosPrivilegedAccountExposure** | LDAP combination | Scans privileged accounts for SPN/pre-auth/delegation exposure |
| 94 | **KerberosTier0ExposureMapper** | Graph + LDAP | Kerberos attack surface map of Tier-0 objects |
| 95 | **KerberosMitigationRecommender** | Policy templates | Kerberos hardening recommendations — AES-only, delegation restrictions, krbtgt rotation |

---

## 🔧 Post-Exploitation

> Lateral movement and persistence via Kerberos after establishing a foothold.

| # | Tool Name | Technique | What It Does |
|---|---|---|---|
| 96 | **KerberosGoldenTicketPersistence** | Storing the krbtgt hash | Creates two Golden Tickets — persists indefinitely with a backup even if krbtgt is rotated twice |
| 97 | **KerberosRODCTicketForge** | RODC + krbtgt | Forges tickets with the RODC krbtgt hash — from the RODC key cache |
| 98 | **KerberosCrossRealmGoldenTicket** | Trust key + forge | Cross-realm Golden Ticket — when the trust key is in hand |
| 99 | **KerberosADCSMasterBridge** | ADCS + PKINIT + Golden | Full chain: ADCS admin cert → DCSync → krbtgt → Golden Ticket |
| 100 | **KerberosSilverTicketFarm** | Service hash reuse | When the same hash reuse is found across multiple services — farm-wide Silver Ticket generator |
