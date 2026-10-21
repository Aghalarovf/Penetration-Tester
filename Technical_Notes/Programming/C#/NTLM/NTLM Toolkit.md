NTLM Expert Tool Kit — C# Implementation
📋 Mündəricat
Common NTLM
Protocol & Challenge Analysis
Capture & Collection
Cracking & Recovery
Authentication & Abuse
Relay Attacks
Coercion Engines
Kerberos ↔ NTLM Interop
Mitigation & Hardening Analysis
Post-Exploitation
Intelligence & Analysis
Başlamaq üçün ardıcıllıq
🔵 Common NTLM
Protokol səviyyəsində ilk təmas — negotiate, challenge, response strukturunun analizi.



#	Alət adı	Texniki dərinlik	Nə edir
1	NTLMProtocolProber	Raw NEGOTIATE_MESSAGE → CHALLENGE_MESSAGE handshake	NTLM v1/v2 dəstəyini, negotiate flag-larını müəyyənləşdirir
2	NTLMVersionDetector	ntlm security blob parsing	NTLMv1 vs NTLMv2 fərqini — NTLMSSP_NEGOTIATE_NTLM vs NEGOTIATE_NTLM2_KEY flag-ları ilə ayrı-ayrı report edir
3	NTLMFlagAnalyzer	NTLMSSP_MESSAGE_SIGNATURE bit field-ları	Bütün negotiate flag-larını decode edir — signing, sealing, key exchange, 128-bit encryption dəstəyi
4	NTLMChallengeExtractor	CHALLENGE_MESSAGE → ServerChallenge	8 baytlıq challenge dəyərini, TargetInfo (AV_PAIR) sahələrini çıxarır
5	NTLMTargetInfoParser	AV_PAIR array parsing	MsvAvNbDomainName, MsvAvNbComputerName, MsvAvDnsDomainName, MsvAvTimestamp — hədəf haqqında meta çıxarır
6	NTLMSSPBlobInspector	SPNEGO / NTLMSSP GSS-API wrapping	SMB, HTTP (401), LDAP bind, MSSQL TDS içindəki NTLM blob-larını parse edir
7	NTLMUnicodeConverter	UTF-16LE encoding	Username/domain/password-un NTLM spesifikasiyasına uyğun encoding-ini hazırlayır — MD4 hash hesablama əsası
8	NTLMAnonymousProber	ANONYMOUS flag ilə session setup	Anonymous NTLM session-un qəbul olunub-olunmadığını test edir — null session analoqu
9	NTLMSPNInspector	MsvAvTargetName / SPN field	Challenge-də qaytarılan SPN-in yoxlanması — alias/SPOF host aşkarlamaq üçün
10	NTLMSessionKeyProber	NEGOTIATE_KEY_EXCHANGE flag	Key exchange support-unu yoxlayır — signed/sealed session qurulub-qurulmayacağını müəyyən edir
🟡 Protocol & Challenge Analysis
Challenge/response dəyərlərinin struktur təhlili, crypto primitivləri və hash formatları.



#	Alət adı	Texniki dərinlik	Nə edir
11	NTLMv1ResponseBuilder	DES(LM hash, challenge)	NTLMv1 response qurur — LAN Manager + NT response kombinasiyası
12	NTLMv2ResponseBuilder	HMAC-MD5 + blob	NTLMv2 hash = HMAC-MD5(NT hash, user+domain) → challenge + blob ilə response qurur
13	NTProofStrCalculator	HMAC-MD5	NTProofStr = HMAC-MD5(NTLMv2Hash, challenge+blob) — offline verification üçün
14	LMHashLegacyGenerator	DES + uppercase truncation	Köhnə LM hash-i hesablayır — 14 char limit, case-insensitive zəifliyi nümayiş etdirir
15	NTHashMD4Calculator	MD4(UTF-16LE(password))	NT hash-i hesablayır — Pass-the-Hash və cracking üçün əsas format
16	NTLMHashFormatConverter	Format parser	user:domain:lm:ntlm::: (John) ↔ user::domain:hash:hash:hash:: (NTDS/hashcat) çevirmələri
17	NTLMv2ClientChallengeBuilder	Blob construction	Timestamp, client nonce, AV_PAIRs ilə tam client challenge blob qurur — relay/replay testləri üçün
18	NTLMNonceEntropyChecker	Challenge sampling	Server challenge-larının təsadüfilik statistikasını yoxlayır — weak RNG hallarda replay imkanını qiymətləndirir
19	NTLMSessionSecurityValidator	SIGNKEY derivation	Session base key → client/server signing key derivation-inı yoxlayır — MIC manipulation testləri üçün
20	NTLMMessageIntegrityChecker	MIC field	NTLMv2 MIC (HMAC-MD5 üç mesaj üzərində) hesablayır/yoxlayır — MIC bypass hallarını test edir
🟠 Capture & Collection
NTLM autentifikasiya cəhdlərinin real vaxtda tutulması və saxlanması.



#	Alət adı	Texniki dərinlik	Nə edir
21	NTLMSMBListener	Raw TCP 445 + SMB Negotiate/Session Setup	Gələn SMB NTLM auth-larını capture edir, hash formatında saxlayır
22	NTLMHTTPListener	TCP 80/8080 — WWW-Authenticate: NTLM	HTTP üzərindən gələn NTLM Negotiate/ Authenticate header-larını tutur
23	NTLMChallengeServer	Custom challenge mode + --static-challenge	Sabit challenge ilə hash-ları rainbow table-a uyğun tutmaq üçün
24	NTLMHashSaver	Hashcat/John format output	Capture olunmuş hash-ları hashcat formatında (mode 5600 NTLMv2, 5500 NetLM/NetNTLMv1) fayla yazır
25	NTLMDowngradeInjector	NEGOTIATE response manipulation	Client-in NTLMv2-dən NTLMv1/LM-ə düşməsinə çalışır — zəif response format tutmaq üçün
26	NTLMEAPListener	802.1X / EAP	IEEE 802.1X/EAP üzərindən MSCHAPv2 (NTLM əsasında) hash tutmaq üçün
27	NTLMRRASProber	VPN / RAS dial-in protocol	RRAS authentication-la NTLM hash cəlbi
28	NTLMProxyWarrior	HTTP proxy + Proxy-Authenticate: NTLM	Proxy auth üzərindən NTLM hash capture
29	NTLMFTPListener	FTP AUTH NTLM	FTP üzərindən NTLM auth tutmaq üçün
30	NTLMDatabaseIndexer	SQLite backend	Bütün capture-ların təkrarsız hash, user, domain istiqaməti ilə indexlənməsi — uzunmüddətli kampaniya analizi
🟣 Cracking & Recovery
Capture olunmuş hash-ların offline bərpası.



#	Alət adı	Texniki dərinlik	Nə edir
31	NTLMHashCracker	Hashcat/John wrapper	NT hash (mode 1000) dictionary, mask, rule-larla crack etmək üçün
32	NTLMv1Cracker	mode 5500	NetNTLMv1 hash-ları crack etmək üçün — DES gözləyən xüsusi harness
33	NTLMv2Cracker	mode 5600	NetNTLMv2 hash-ları üçün hashcat orchestrator — job queue, resume support
34	NTLMWordlistManager	Dictionary + rules	rockyou.txt + dive.rule kombinasiyaları ilə avtomatik crack session-ları qurur
35	NTLMRainbowTableProber	Rainbow table lookup	Kidbə köhnə LM/NTLMv1 hash-lar üçün rainbow table lookup (pre-computed)
36	NTLMPassTheHashValidator	Hash-based auth test	Crack edilmiş hash ilə SMB/HTTP auth-u test edir — PTHash işləyir mi
37	NTLMPwdumpParser	NTDS.dit / SAM dump parser	pwdump formatında dump-ların index-lənməsi, unique hash sayı, reuse analizi
38	NTLMHashReuseAnalyzer	Cross-host hash comparison	Birdən çox host-dan tutulan hash-ların eyni local admin parolunu paylaşdığını tapır
39	NTLMPBKDF2BruteForcer	GPU/CPU adaptive	Yüksək entropy-lu hash-lar üçün hardware-ə adaptiv brute-force planlayıcı
⚔️ Authentication & Abuse
Hash-ların yenidən istifadəsi, delegation, key robbing.



#	Alət adı	Protokol / Texnika	Nə edir
40	NTLMPassTheHash	NTLM session setup ilə hash	NT hash ilə SMB/HTTP auth — password olmadan giriş
41	NTLMPassTheHashHTTP	HTTP Authorization: NTLM	HTTP üzərindən PTH — Exchange, MSSQL, WinRM üzrə
42	NTLMPassTheHashMSSQL	TDS + NTLM auth	MSSQL Server-ə hash ilə auth — xp_cmdshell lateral movement
43	NTLMUnPACTheHash	PKINIT U2U	Kerberos PKINIT-dən NT hash çıxarmaq — cert-dən hash recovery
44	NTLMDomainTrustAbuse	lsarpc — trust key	Trust hesab hash-ları ilə cross-domain auth — SIDHistory + trust kombinasiyası
45	NTLMMachineAccountAuth	$MACHINE.ACC	Machine hesab hash-i ilə auth — KrbRelayUp-analog və ADCS rolları üçün
46	NTLMUnicodeSmuggling	MsvAvTargetName manipulation	Unicode normalization istismarı — Cross-Protocol / hostname smuggle istismarı
🔴 Relay Attacks
NTLM autentifikasiyasının başqa host-a yönləndirilməsi.



#	Alət adı	Protokol / Texnika	Nə edir
47	NTLMSMBRelay	SMB ↔ SMB	Gələn SMB NTLM auth-unu başqa SMB host-a relay etmək — ən klassik relay
48	NTLMSMBtoLDAPRelay	SMB → LDAP	SMB-dən LDAP-ya relay — signing disabled DC-də admin əməliyyatları
49	NTLMSMBtoLDAPSRelay	SMB → LDAPS	LDAPS (signing-lə) — channel binding bypass cəhdləri ilə
50	NTLMSMBtoHTTPRelay	SMB → HTTP/Exchange	Exchange EWS, Autodiscover üzərindən relay — signed HTTP öhdəlikləri
51	NTLMSMBtoMSSQLRelay	SMB → TDS	MSSQL Server üzərindən relay — xp_cmdshell RCE-ya qədər
52	NTLMSMBtoIMAPRelay	SMB → IMAP	IMAP STARTTLS relay variantı
53	NTLMDNSZombieRelay	DNS poisoning + multi-relay	MDNS/LLMNR poisoning ilə çoxlu auth-un toplandığı relay farm
54	NTLMRelayMachineAccount	Relay → Self	Machine hesab-a relay edib domain obyekti yaradmaq (Resource-Based Constrained Delegation istismarı)
55	NTLMSigningEnforcementBypass	SMB signing = required bypass cəhdləri	Signing məcburiyyəti altında xüsusi implement-larla relay cəhdləri test etmək
56	NTLMEPAChannelBindingProber	Channel Binding Token	Extended Protection for Authentication CBT-dən (binding bypass yoxlanışı)
⚡ Coercion Engines
Hədəf host-un attacker-ə NTLM auth göndərməsinə məcbur edilməsi.



#	Alət adı	Protokol / Texnika	Nə edir
57	NTLMSpoolerCoercer	MS-RPRN — RpcRemoteFindFirstPrinterChangeNotification	Print Spooler coercion — klasik yanaşma
58	NTLMPetitPotam	MS-EFSRPC — EfsRpcOpenFileRaw	EFSRPC coercion — ADCS (NTLM relay) ilə kombinasiya üçün güclü
59	NTLMDFSCoercer	MS-DFSNM — NetrDfsAddStdRoot	DFS coercion — Spooler-da patch olunduqda alternativ
60	NTLMShadowCoercer	MS-FSRVP — IsPathSupported	File Server Remote VSS protocol coercion
61	NTLMCertCoercer	MS-CES — ADCS endpoint	ADCS endpoint-ləri üzərindən coercion
62	NTLMSmartCardCoercer	MS-PAR / MS-RPRN varian	Print System Asynchronous Remoting coercion
63	NTLMLLMNRSpoofer	UDP 5355 LLMNR poisoning	LLMNR query-lərə spoof cavab — hədəf attacker-host-a NTLM auth göndərir
64	NTLMNBNSResponder	UDP 137 NBNS poisoning	NetBIOS Name Service poisoning
65	NTLMDHCPv6Spoofer	DHCPv6 + DNS takeover	IPv6 DHCP spoof edib DNS server kimi özünü qeyd etmək — poisoning ilə birlikdə
66	NTLMWPADProxySpoofer	WPAD discovery hijack	WPAD hostname tutub proxy auth ilə NTLM hash cəlbi
67	NTLMDNSPoisoner	Raw UDP 53	Poisoned DNS records — hədəf üçün attacker IP-yə resolve
68	NTLMDot11Captiver	Wi-Fi captive portal + NTLM	Wireless client-lar üçün captive portal NTLM capture
🟢 Kerberos ↔ NTLM Interop
Kerberos-NTLM əlaqələri, delegation və cross-protocol təhlil.



#	Alət adı	Texniki dərinlik	Nə edir
69	NTLMKerberosDowngradeProber	SPNEGO negotiation	Kerberos → NTLM fallback-ı test edir — KDC çatmazlığında NTLM aktivləşir mi
70	NTLMUnconstrainedDelegationMonitor	TGT capture	Unconstrained delegation-la gələn TGT-ləri tutur (NTLM əlaqəli olaraq hash reuse üçün)
71	NTLMS4U2ProxyAnalyzer	Resource-Based Constrained Delegation	RBCD-nin NTLM relay ilə birlikdə işlədiyi halları yoxlayır
72	NTLMKerbRoastInterceptor	AS-REP roasting	AS-REP response-larda hash-sız NTLM auth-da pre-auth olmayan hesablar
73	NTLMUsernameEnumeration	NTLM status kodları	STATUS_LOGON_FAILURE vs STATUS_ACCOUNT_DISABLED fərqi ilə istifadəçi adı enumerate edir
74	NTLMDomainTrustDelegation	LsarQueryTrustedDomainInfo	Trust-ların NTLM istiqamətliliyini yoxlayır — allowed/deallowed
75	NTLMCrossProtocolRelayAnalyzer	Protocol compatibility matrix	Hansı protocol-ların hansına relay edilə bilməyəcəyini hesablayır (SMB→HTTP, HTTP→LDAP və s.)
🔶 Mitigation & Hardening Analysis
Domain üzərində NTLM istismarına qarşı qorunma statusunun yoxlanması.



#	Alət adı	Texniki dərinlik	Nə edir
76	NTLMSigningChecker	srvsvc + SMB Negotiate	Domain üzrə SMB signing required olub-olmadığını yoxlayır
77	NTLMLDAPSigningChecker	LDAP bind options	LDAP/LDAPS signing və channel binding-ə məcburiyyət statusu
78	NTLMRestrictionPolicyReader	LsarQueryInformationPolicy — PolicyNtAuthentication	RestrictNTLM policy-lərin yoxlanması — domain/DC səviyyəsində
79	NTLMAuditingPolicyReader	Audit policy	NTLM istismarı üçün audit event-lərin (4624, 4625) sazlanması
80	NTLMChannelBindingChecker	LsapQueryDomainInformation	Extended Protection / Channel Binding policy-ləri
81	NTLMSMBv1DisabledChecker	SMB Negotiate	SMBv1-in disabled olub-olmadığı — NTLMv1 istismarının prerişiti
82	NTLMPrivilegedAccountFinder	LDAP + hash reuse analysis	Privileged hesabların NTLM istismarına açıq olduğunu tapır
83	NTLMMitigationRecommender	Policy templates	Tapılan zəifliklər üçün NTLM blocking, SMB signing, EP Auth rekomendasiyaları yaradır
🔧 Post-Exploitation
Foothold sonrası NTLM üzərindən lateral movement və kalıcılıq.



#	Alət adı	Protokol / Texnika	Nə edir
84	NTLMLocalAdminPivot	Pass-the-Hash + svcctl	Local admin hash ilə başqa host-a servis yaradaraq lateral move
85	NTLMTokenImpersonation	LogonUser + DuplicateTokenEx	NTLM session token impersonation — user context-lə kod icra etmək
86	NTLMCredentialHarvester	lsass dump + NT hash extraction	Local NT hash-ların yığılması — PTH üçün hazırlıq
87	NTLMPasswordSprayValidator	Lockout-aware spray	NTLM auth spray — lockout policy-ə uyğun təhlükəsiz sürət
88	NTLMPersistentRelay	Scheduled task / service	Relay listener-in perzistent saxlanması
89	NTLMADCSAbuse	MS-WCCE + relay	NTLM relay ilə ADCS cert request — admin-equivalent cert əldə etmək
90	NTLMSilverTicketBridge	krbtgt-dən NT hash	NT hash-dən TGS forgo — Silver Ticket yaradır
91	NTLMGoldenCredentialForge	krbtgt NT hash	Golden Ticket yaradır — NTLM/NT hash reuse üçün
92	NTLMShadowCredentialsAbuse	msDS-KeyCredentialLink	Shadow Credentials yazmaq + UnPACTheHash ilə NT hash çıxarmaq
📊 Intelligence & Analysis
Toplanmış NTLM data-larının təhlili, parol reuse modeli, hücum path qrafikası.



#	Alət adı	Texniki dərinlik	Nə edir
93	NTLMHashReuseMapper	Graph analysis	Hansı istifadəçilər hansı host-larda eyni local admin parolu istifadə edir — attack path qrafikası qurur
94	NTLMChallengeDistributionAnalyzer	Statistical analysis	Challenge-ların istiqaməti üzrə yayılma analizi — hansı server-lar NTLM-i qəbul edir
95	NTLMRelayAttackPathPlanner	Graph + rules engine	Relay attack üçün avtomatik path hesablayır — coercion → relay → ADCS/RBCD zənciri
96	NTLMEventLogCorrelator	Windows Event Log parsing	4624/4625/4648/4776 event-lərində NTLM istismarı izlərini korrelyasiya edir
97	NTLMReportGenerator	Markdown/HTML export	Bütün tapıntıların professional pentest report formatında çıxarışı
98	NTLMHoneypotDetector	Fingerprint analysis	Hədəfin honeypot olub-olmadığını — challenge davranışı və response vaxtları ilə yoxlayır
99	NTLMNoiseSimulator	Fake auth generator	Blue team üçün detection testləri — fake NTLM auth-ların SIEM alert-lərini tetikləyir mi
100	NTLMKillChainMapper	MITRE ATT&CK mapping	Tapılan zəiflikləri MITRE ATT&CK texnika ID-ləri ilə map-ləyir (T1550.002 Pass the Hash, T1187 Forced Authentication, T1557.001 LLMNR/NBT-NS Poisoning)
