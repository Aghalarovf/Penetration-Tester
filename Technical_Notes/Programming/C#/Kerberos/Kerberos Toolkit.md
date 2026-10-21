Kerberos Expert Tool Kit — C# Implementation
📋 Mündəricat
Common Kerberos
Protocol & Ticket Analysis
Enumeration
Attacks — User Accounts
Attacks — Service Accounts
Delegation Attacks
Certificate & PKINIT Attacks
DC & Infrastructure Attacks
Ticket Forge & Manipulation
Authentication & Abuse
Mitigation & Hardening Analysis
Post-Exploitation
Intelligence & Analysis
Başlamaq üçün ardıcıllıq
🔵 Common Kerberos
Protokol səviyyəsində ilk təmas — KDC çatma, AS-REQ/TGS-REQ struktur analizi, fingerprint.



#	Alət adı	Texniki dərinlik	Nə edir
1	KerberosPortProber	TCP 88 + UDP 88 TcpClient/Socket	KDC-nin TCP/UDP üzərindən əlçatanlığını yoxlayır — UDP fragmentation issue-ları aşkarlayır
2	ASREQProber	Raw AS-REQ (PA-ENC-TIMESTAMP)	KDC-dən KDC_ERR_PREAUTH_REQUIRED cavabı alır — KDC aktivliyi + pre-auth policy təsdiqi
3	KerberosRealmDiscovery	DNS SRV + kinit analog	_kerberos._tcp.dc._msdcs.domain.com SRV record-larından realm/DC discovery
4	KerberosEncryptionDetector	AS-REQ etype listing	Dəstəklənən etype-ları tapır — rc4-hmac (23), aes128 (17), aes256 (18) — RC4 default = KRBPASS-ın köhnəliyi
5	KDCErrorInterpreter	KRB-ERROR parsing	KDC_ERR_ kodlarını tərcümə edir — PREAUTH_REQUIRED(25), CLIENT_NOT_FOUND(6), PRINCIPAL_UNKNOWN — username enumeration əsası
6	KerberosClockSkewChecker	AS-REQ timestamp	Local ↔ DC vaxt fərqini ölçür — 5 dəqiqəlik skew limitini aşarsa ticket-lar işləməz
7	KerberosPACParser	PAC structure decoding	PAC_LOGON_INFO, PAC_SIGNATURE_DATA, PAC_CREDENTIALS INFO sahələrini parse edir
8	KerberosPrincipalFormatter	user@REALM / NT principals	Principal name format-larını çevirir — enterprise, NT, canonical name normalization
9	KerberosReferralAnalyzer	Cross-realm referral tickets	KDC_ERR_WRONG_REALM + referral TGT-lərini parse edir — trust topologiyasını xəritələyir
10	KerberosUDPFragmentProber	UDP 88 max packet size	KRB5KDC_ERR_RESPONSE_TOO_BIG aşkarlanır — TCP-yə fallback lazımlığını müəyyənləşdirir
🟡 Protocol & Ticket Analysis
Ticket-ların struktur təhlili, crypto sahələri, PAC签名.



#	Alət adı	Texniki dərinlik	Nə edir
11	ASREPParser	KRB-AS-REP ASN.1 DER	AS-REP içindəki ticket, encrypted part, key sahələrini çıxarır
12	TGSREPParser	KRB-TGS-REP ASN.1	TGS-REP-dən service ticket, session key, adticket-ləri çıxarır
13	TicketKvnoExtractor	Ticket kvno field	Ticket version number-ı oxuyur — key rotation hadisələrini aşkarlayır
14	KerberosTicketFlagAnalyzer	Ticket flags bitfield	forwardable, renewable, initial, pre-authent, ok-as-delegate flag-larını decode edir
15	PACSigVerifier	PAC_SIGNATURE_DATA	Server/Privilege Server signature-ları yoxlayır — key-ə sahib olsaq forge etmək mümkündürmü
16	SessionKeyExtractor	AS-REP enc-part	AS-REP enc-part-dan session key çıxarır — service ticket request-lər üçün
17	KerberosAPREQInspector	AP-REQ + authenticator	Mövcud ticket-lərin authenticator strukturunu, sub-session key-i təhlil edir
18	KerberosTicketLifetimeChecker	endtime/renew-till	Ticket-ların lifetime/renewal window-unu hesablayır — long-lived ticket-ları tapır
19	KerberosCrossRealmTicketAnalyzer	Cross-realm TGT	Cross-realm trust üzərindən alınan ticket-lərin SID filtering/statusunu yoxlayır
20	KerberosCipherStrengthAnalyzer	Etype + key length	istifadə olunan cipher-ların gücünü qiymətləndirir — RC4 = NT hash reuse riski
🟠 Enumeration
Kerberos üzərindən istifadəçi, servis, SPN və delegation konfiqurasiyasının kəşfi.



#	Alət adı	Texniki dərinlik	Nə edir
21	KerberosUserEnum	AS-REQ status codes	KDC_ERR_PREAUTH_REQUIRED vs KDC_ERR_CLIENT_NOT_FOUND fərqi ilə username enumeration (credsiz!)
22	KerberosUserEnumPasswordSpray	AS-REQ + KDC_ERR_PREAUTH_FAILED	Username + password spray üçün pre-auth testing — lockout-aware
23	SPNEnumerator	LDAP — servicePrincipalName	Domain-dəki bütün SPN-ləri enumerate edir — Kerberoasting hədəflərini tapır
24	SPNToUserMapper	LDAP + sAMAccountName	SPN ↔ user account bağlantısını qurur — hansı servis hesab hansı SPN-i daşıyır
25	DelegationConfigEnumerator	LDAP — msDS-AllowedToDelegateTo	Constrained delegation konfiqurasiyalarını tapır — hədəf SPN-ləri ilə birlikdə
26	UnconstrainedDelegationFinder	LDAP — userAccountControl & TRUSTED_FOR_DELEGATION	Unconstrained delegation-ı aktiv hesabları/DC-ləri tapır — TGT capture hədəfləri
27	RBCDFinder	LDAP — msDS-AllowedToActOnBehalfOfOtherIdentity	Resource-Based Constrained Delegation konfiqurasiyalarını enumerate edir
28	TrustedToAuthForDelegationFinder	TRUSTED_TO_AUTH_FOR_DELEGATION flag	Protocol transition-enabled hesabları tapır — S4U2Self üçün
29	KerberosServiceAccountAudit	LDAP kombinasiyası	Servis hesabların (spn-lə hesablar) parol yaşı, hash tipi, pre-auth statusu audit-i
30	KerberosGroupMembershipMapper	LDAP — memberOf	Kerberos hədəf hesabların qrup üzvlüyü — privileged hesabların Kerberos-dan çatılması
31	ASREPAccountFinder	LDAP — UF_DONT_REQUIRE_PREAUTH	Pre-auth tələb etməyən hesabları tapır — AS-REP Roasting hədəfləri
32	KerberosTrustEnumerator	LDAP — trustedDomainObject	Trust-ların Kerberos istiqamətini, transitivity, forest trust statusunu tapır
🔴 Attacks — User Accounts
İstifadəçi hesablarına qarşı Kerberos zəiflikləri.



#	Alət adı	Texnika	Nə edir
33	ASREP Roaster	AS-REQ (no pre-auth) → enc-part	Pre-auth disabled hesabların AS-REP enc-part hash-ini çıxarır — offline crack üçün (mode 18200)
34	Kerberoast	TGS-REQ → service ticket	SPN-i olan bütün hesabların TGS-lərini toplayır — RC4 ticket-lar = NT hash (mode 13100)
35	KerberoastRC4Focus	Etype 23 filter	Yalnız RC4 ticket-ları request edir — cracking dəyərini maksimuma çatdırır
36	KerberoastAESFocus	Etype 17/18	AES ticket-larını request edir — yavaş crack, amma hash reuse analizi üçün dəyərli
37	KerberosPasswordSpray	AS-REQ pre-auth	Domain password policy-ə uyğun lockout-aware password spray
38	KerberosBruteForce	AS-REQ	Yavaş brute-force — honeypot/hosting-lərdə kerberos login failure-ları
39	KerberosUserEnumerationTiming	Response timing analysis	Response vaxt fərqləri ilə username enumeration — status code-lar maskalandıqda
40	KerberosPasswordPolicyReader	LDAP	Spray etməzdən əvvəl lockout threshold/duration-u oxuyur — SAMR analoqu LDAP üstündə
🟣 Attacks — Service Accounts
Servis hesab hash-larının tutulması və istismarı.



#	Alət adı	Texnika	Nə edir
41	KerberoastCracker	Hashcat mode 13100/19600	TGS ticket-ları crack etmək üçün hashcat orchestrator — RC4 + AES hər ikisi
42	KerberosServiceTicketHarvester	TGS-REQ + krb5cc parse	Bütün servis ticket-larını yığıb hash formatına çevirir
43	SPNSquattingProber	LDAP + SPN registration test	Boş SPN-ləri tapır — attacker SPN qeyd edib Kerberoast-ı user-lara cəlb edə bilər
44	KerberosSilverTicketForge	Service account NT hash	krbtgt deyil — service hash ilə TGS forge edir — LANMAN/RC4 + AES
45	KerberosTicketCacheParser	LSASS / KRB5CCNAME	Local ticket cache-i oxuyur — mövcud ticket-ları export edir (kirbi format)
46	KerberosKRBCCConverter	kirbi ↔ ccache	Ticket format çevirmələri — Impacket/Rubeus uyğunluğu üçün
47	KerberosServiceAccountPivot	Crack edilmiş servis hesab	Servis hesab parolundan PTH/PSH və TGT request zənciri qurur
48	KerberosWeakPasswordScanner	Service account wordlist	Servis hesablarına xüsusi wordlist (service, sql, svc prefix-lər) ilə spray
⚔️ Delegation Attacks
Delegation konfiqurasiyalarının istismarı.



#	Alət adı	Texnika	Nə edir
49	S4U2SelfAbuser	S4U2Self + S4U2Proxy	Constrained delegation üzərindən öz adına istənilən user üçün ticket almaq
50	S4U2ProxyPivot	S4U2Proxy + forwardable TGT	Delegation SPN-inə forwardable TGS ile service ticket almaq — admin impersonation
51	RBCDAttacker	msDS-AllowedToActOnBehalfOfOtherIdentity yazır	RBCD konfiqurasiya yazır (write access varsa) — machine account control → impersonation
52	RBCDMachineAccountForge	Machine account SPN + S4U	Yaradılmış machine account SPN qeyd edib RBCD relay-ini tamamlamayır
53	UnconstrainedDelegationListener	TGT capture (DC + Tier-0)	Unconstrained delegation host-unu monitor edir — gələn TGT-ləri tutur (DCSync-ə qədər zəncir)
54	KerberosResourceConstrainedFinder	LDAP + ACL analysis	RBCD yazma hüququ olan hesabları tapır — attack path qrafikası
55	S4UTicketRogue	S4U2Self + PAC manipulation	S4U ticket-larında PAC manipulation — user SID override
56	DelegationAttackPathPlanner	Graph analysis	Delegation config-lərindən attack path qrafikası qurur — kim kimə impersonation edə bilər
🔶 Certificate & PKINIT Attacks
ADCS + PKINIT + Kerberos kəsiyi.



#	Alət adı	Texnika	Nə edir
57	PKINITProber	PA-PK-AS-REQ	DC-nin PKINIT dəstəyini yoxlayır — cert-based auth üçün
58	UnPACTheHash	PKINIT U2U → PA-PAC-REQUEST	Cert-dən NT hash çıxarır — Shadow Credentials, ESC halları ilə istifadə
59	ShadowCredentialsAbuser	msDS-KeyCredentialLink yazır	KeyCredential yazır, PKINIT ilə NT hash çıxarır, izi təmizləyir
60	ESC1Abuser	ADCS ENROLLEE_SUPPLIES_SUBJECT	Misconfigured template-dən istənilən user üçün cert alır → Kerberos auth
61	ESC2-ESC6Scanner	ADCS template flags	Dangerous template flag-larını skan edir — AnyPurpose, EKU-less, EDITF_ATTRIBUTESUBJECTALTNAME2
62	ESC8RelayProber	ADCS NTLM relay	ADCS HTTP endpoint-i NTLM relay-ə qarşı həssaslığını yoxlayır
63	CertToKerberosBridge	Cert + PKINIT	Əldə edilmiş cert-dən TGT-yə keçid zənciri — PSH-analog
🎯 DC & Infrastructure Attacks
DC, krbtgt və infrastruktura üzərindən hücumlar.



#	Alət adı	Texnika	Nə edir
64	DCSync	MS-DRSR DRSGetNCChanges	Replication hüququ ilə bütün user hash-larını çıxarır — krbtgt daxil
65	DCSyncSingleUser	DRSGetNCChanges + pmsgOut filter	Yalnız tək user-in hash-ini çıxarır — noise azaldır
66	DCShadow	MS-DRSR DRSAddEntry	Sahte DC qeydiyyatı ilə obyekt yazır — SIDHistory, delegation config dəyişikliyi
67	ZeroLogonChecker	NetrServerReqChallenge/NetrServerAuthenticate3	CVE-2020-1472 — DC hesab parolunu sıfırlamaq mümkündürmü (exploit edir, amma restore option)
68	KerberosGoldenTicketForge	krbtgt NT hash	Golden Ticket forge edir — istənilən user, istənilən qrup, istənilən domain
69	KerberosDiamondTicketForge	DCSync + krbtgt AES key	AES key ilə TGT sign edir — S4U ilə birləşərək "diamond" ticket
70	KerberosSapphireTicketForge	krbtgt AES key + S4U	Sapphire ticket — S4U2Self ilə PAC yenilənərək forge
71	krbtgtKeyIDAnalyzer	Key rotation monitoring	krbtgt kvno və key age monitoring — Golden Ticket ictimaiyyətində rotation yoxluğu
72	KerberosRIDCyclingAnalyzer	RID analysis	RID cycling ilə DC-də user enumeration — DCSync olmadan
73	KerberosGPPRefresher	SYSVOL + gPCFileSysPath	GPO-lardan delegation/SPN konfiqurasiya dəyişikliklərini tapır
🛠️ Ticket Forge & Manipulation
Ticket-ların yaradılması, dəyişdirilməsi, PAC manipulation.



#	Alət adı	Texnika	Nə edir
74	TicketPACInjector	PAC construct + signature	PAC yaratmaq/dəyişmək — GroupSID, UserSID manipulation
75	TicketSIDHistoryInjector	PAC_SID_HISTORY	SIDHistory ilə enterprise admin SID əlavə edir — forge edilmiş ticket-larda
76	TicketGroupEditor	PAC_LOGON_INFO	Ticket-in qrup üzvlüyünü dəyişir — Domain Admins SID əlavə etmək
77	TicketLifetimeExtender	endtime/renew-till patch	Ticket lifetime-nı uzadır — 10 il Golden Ticket pattern
78	TicketEncoderKRB	ASN.1 DER encoder	Ticket/AP-REQ/TGS-REQ-ləri encode edir — C#-da raw ASN.1 builder
79	TicketPassthruValidator	AP-REQ + service	Forge edilmiş ticket-ların real service-də qəbul olunub-olunmadığını test edir
80	TicketCrossRealmForge	Cross-realm trust key	Inter-realm trust key-ə sahib olsaq cross-realm ticket forge edir
⚔️ Authentication & Abuse
Ticket-lardan istifadə edərək giriş/lateral movement.



#	Alət adı	Texnika	Nə edir
81	KerberosSMBAuth	AP-REQ + SMB	Ticket ilə SMB auth — Pass-the-Ticket
82	KerberosWinRMPTT	AP-REQ + WinRM	Pass-the-Ticket ilə WinRM giriş
83	KerberosHTTPPTT	AP-REQ + HTTP	HTTP üzərindən ticket auth — Exchange EWS
84	KerberosLDAPPacers	LDAP + PAC	LDAP üzərindən ticket auth — domain obyekt oxuma
85	KerberosMSSQLPTT	TDS + Kerberos	MSSQL Server-ə ticket auth — xp_cmdshell pivot
86	KerberosSilverTicketSMB	Silver TGS + SMB	Silver ticket ilə CIFS auth — local admin tip access
87	KerberosPassTheCache	ccache/kirbi export	Ticket cache-dən birbaşa auth — Rubeus/Impacket uyğun
🔶 Mitigation & Hardening Analysis
Kerberos istismarına qarşı qorunma statusunun yoxlanması.



#	Alət adı	Texnika	Nə edir
88	KerberosAESOnlyChecker	LDAP + msDS-SupportedEncryptionTypes	RC4 disabling statusu — AES-only policy yoxlanışı
89	KerberosPACSigningChecker	PAC signature validation	PAC signature enforcement statusu (CVE-2021-42287 hardening)
90	KerberosPreAuthPolicyReader	LDAP — userAccountControl	Pre-auth disabled hesabların sayı — AS-REP Roast risk
91	KerberosDelegationAudit	LDAP	Bütün delegation konfiqurasiyalarının audit reportu — risk scoring ilə
92	KerberosADCSAudit	LDAP + ADCS templates	ADCS template risklərinin audit-i — ESC halları
93	KerberosPrivilegedAccountExposure	LDAP kombinasiyası	Privileged hesabların SPN/pre-auth/delegation exposure-larını skan edir
94	KerberosTier0ExposureMapper	Graph + LDAP	Tier-0 obyektlərin Kerberos attack surface map-i
95	KerberosMitigationRecommender	Policy templates	Kerberos hardening rekomendasiyaları — AES-only, delegation restriction, krbtgt rotation
🔧 Post-Exploitation
Kerberos foothold sonrası lateral movement və kalıcılıq.



#	Alət adı	Texnika	Nə edir
96	KerberosGoldenTicketPersistence	krbtgt hash saklamaq	Golden Ticket 2x yaradılıb — krbtgt hash-ı ikicə rotate edilse də, backup ilə daima qalır
97	KerberosRODCTicketForge	RODC + krbtgt	RODC krbtgt hash-ı ilə ticket forge — RODC key cache-dən
98	KerberosCrossRealmGoldenTicket	Trust key + forge	Cross-realm Golden Ticket — trust key-ə sahib olsaq
99	KerberosADCSMasterBridge	ADCS + PKINIT + Golden	ADCS admin cert → DCSync → krbtgt → Golden zənciri
100	KerberosSilverTicketFarm	Service hash reuse	Birdən çox servisdə eyni hash reuse tapılarsa — farm-wide Silver Ticket generator
