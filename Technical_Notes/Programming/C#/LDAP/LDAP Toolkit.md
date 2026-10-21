LDAP Expert Tool Kit — C# Implementation
📋 Mündəricat
Common LDAP
Protocol & Connection Analysis
Enumeration — Domain Objects
Enumeration — Users, Groups & ACLs
LDAP Injection & Security Testing
Credential & Hash Attacks
ADCS Attacks
Delegation & RBCD
GPO & Policy Attacks
DC & Infrastructure Attacks
BloodHound / Graph Analysis
Authentication & Abuse
Mitigation & Hardening Analysis
Post-Exploitation
Intelligence & Analysis
Başlamaq üçün ardıcıllıq
🔵 Common LDAP
Protokol səviyyəsində ilk təmas — bağlantı, rootDSE, bind, anonym/i̇ce.



#	Alət adı	Texniki dərinlik	Nə edir
1	LDAPPortProber	TCP 389/636 — TcpClient	LDAP/LDAPS port-larının əlçatanlığını yoxlayır — StartTLS-capable 389 vs 636
2	RootDSEProber	LDAP://rootDSE anonymous bind	rootDSE oxuyur — defaultNamingContext, domainFunctionality, forestFunctionality, dcAttributes
3	LDAPAnonymousBindProber	Anonymous bind + base search	Anonymous bind mümkündürmü — hansı obyektlər anonymous-la görünür
4	LDAPStartTLSProber	StartTLS extended op	389 port-luq bağlantının TLS-ə upgrade edə bildiyini yoxlayır — plaintext bind riski
5	LDAPFunctionalityReader	rootDSE — domainFunctionality	Domain/Forest functional level-ları oxuyur — köhnə level = NTLM/RC4 default riski
6	LDAPNamingContextMapper	rootDSE — namingContexts	Bütün naming context-ləri tapır — schema, config, domain NC-ləri
7	LDAPSchedulerProber	LDAP paged search limits	Paged search limit-lərini (MaxValRange, MaxPool) tapır — enumerate sürəti planlamaq üçün
8	LDAPSchemaDumper	schemaNamingContext	AD schema obyektlərini listə alır — custom attribute-lar, mayContain struktur
9	LDAPServerFingerprinter	rootDSE + vendor attributes	LDAP server məhsulunu (AD, OpenLDAP, eDirectory) fingerprint edir — attack vector seçimi
10	LDAPReferralChaser	LDAPResult referral parsing	Referral-ları izləyir — cross-domain enumeration, trust hopping
🟡 Protocol & Connection Analysis
Bağlantı təhlükəsizliyi, signing/sealing statusu, bind metodu təhlili.



#	Alət adı	Texniki dərinlik	Nə edir
11	LDAPSigningChecker	rootDSE — supportedSASLMechanisms	LDAP signing requirement statusu — Require Signing GPO yoxlanışı
12	LDAPSChannelBindingChecker	TLS + channel binding	LDAPS channel binding token yoxlanışı — Starbleed/EPA bypass testləri
13	LDAPSASLBindAnalyzer	SASL mechanism listing	Dəstəklənən SASL mechanism-ları tapır — GSSAPI, NTLM, DIGEST-MD5, EXTERNAL
14	LDAPNegotiateAuthProber	SASL NTLM / GSSAPI	Negotiate auth ilə bind — Kerberos vs NTLM fallback yoxlanışı
15	LDAPSimpleBindTester	DN + password bind	Simple bind testi — credential validity, data 533 disabled account detection
16	LDAPBindErrorInterpreter	LDAP result codes	data 525 (user not found), 530 (not permitted), 532 (password expired), 775 (locked) — rich enumeration
17	LDAPAnonymousDumper	Anonymous bind + paged search	Anonymous-la oxuna bilən obyektləri çıxarır — default AD-də çoxlu info çıxır
18	LDAPConnectionPoolManager	Multi-DC connection pooling	Yük balanslı multi-DC bağlantıları — enumerate sürəti + reliability
19	LDAPTLS Cert Inspector	LDAPS certificate	LDAPS cert-in subject, SAN, validity, issuer-ini oxuyur — cert tamper testing
20	LDAPTimeoutBehaviorAnalyzer	Slow query / abandoned search	Timeout dəyərləri, abandoned search-davranışı — fingerprint və DoS limit qiymətləndirməsi
🟠 Enumeration — Domain Objects
Domain strukturunun LDAP üzərindən kəşfi.



#	Alət adı	Texniki dərinlik	Nə edir
21	DomainObjectCounter	(objectClass=*) paged search	Bütün obyektlərin sayını hesablayır — paged search chunk-ları ilə
22	OUStructureMapper	(objectClass=organizationalUnit)	OU ağacını çıxarır — GPO link-ləri ilə birlikdə
23	DomainControllerFinder	(userAccountControl:1.2.840.113556.1.4.803:=8192)	Domain-dəki bütün DC-ləri tapır
24	GCPortDiscoverer	rootDSE + port 3268/3269	Global Catalog port-larını tapır — forest-wide search
25	DomainTrustEnumerator	(objectClass=trustedDomain)	Bütün trust-ları listə alır — tip, istiqamət, transitivity
26	ExchangeServerFinder	(objectClass=msExchExchangeServer)	Exchange server-ləri tapır — lateral movement hədəfləri
27	SQLServerSPNFinder	(servicePrincipalName=MSSQLSvc*)	MSSQL SPN-ləri olan hesabları tapır
28	LAPSReader	ms-Mcs-AdmPwd / msLAPS-Password	LAPS parollarını oxuyur — read hüququ varsa hər host-un local admin parolu
29	GMSADecrypter	msDS-GroupMSAMembership + AES key	gMSA hesablarının parolunu oxuyur — bcrypt/AES decrypt zənciri
30	DomainSIDExtractor	objectSid — S-1-5-21-*	Domain SID çıxarır — Golden/Silver Ticket, well-known SID mapping
31	PasswordPolicyReader	cn=PasswordPolicy,cn=System,...	PSO-lar, min length, complexity, lockout — spray əvvəli mütləq addım
32	DomainFunctionalLevelReader	rootDSE — domainFunctionality	Functional level oxuyur — Kerberos hardening statusu ilə bağlı qərarlar
🟣 Enumeration — Users, Groups & ACLs
İstifadəçi, qrup və ACL təhlili — BloodHound-un raw data mənbəyi.



#	Alət adı	Texniki dərinlik	Nə edir
33	LDAPUserEnumerator	(objectClass=user)	Bütün user-ları — sAMAccountName, description, lastLogon, pwdLastSet
34	LDAPDescriptionHunter	(description=*pass*)	Description sahələrində credential axtarışı — admin-lər burada parol yazır
35	LDAPGroupEnumerator	(objectClass=group)	Bütün qrupları — member list-ləri, nested membership zəncirləri
36	LDAPNestedGroupResolver	Recursive memberOf chase	Nested qrup üzvlüyünü resolve edir — Domain Admins-ə dolayısıyla çatan user-lar
37	LDAPAdminCountFinder	(adminCount=1)	adminCount=1 olan hesablar — privileged account-ların dəqiq siyahısı
38	LDAPDACLReader	nTSecurityDescriptor	ACL/DACL strukturunu oxuyur — kim hansı obyekt üzərində nə edə bilər
39	LDAPACLRightsMapper	Security descriptor + Validated Write	Extended right-ları map-ləyir — DCSync, Reset Password, WriteDACL kimi kritik right-lar
40	LDAPDangerousRightsFinder	ACL filter	Təhlükəli right-ları tapır — GenericAll, WriteDACL, WriteOwner, AllExtendedRights
41	LDAPOwnedReaderFinder	ACL owner analysis	Owner-i current user olan obyektləri tapır — owner right → DACL dəyişmə
42	LDAPGPOPathMapper	gPCFileSysPath	GPO SYSVOL path-larını map-ləyir — GPP password crawl-una
43	LDAPSIDHistoryAuditor	sIDHistory	SIDHistory olan obyektləri tapır — migration leftovers, hidden admin access
44	LDAPBitFieldDecoder	userAccountControl bitfield	UAC flag-larını decode edir — DONT_REQ_PREAUTH, TRUSTED_FOR_DELEGATION, NOT_DELEGATED
45	LDAPExchangeMailboxFinder	homeMDB filter	Mailbox-ları tapır — delegation rights, mailbox ACL zəiflikləri
🔴 LDAP Injection & Security Testing
LDAP sorğularına injection, filter manipulation.



#	Alət adı	Texnika	Nə edir
46	LDAPInjectionTester	Filter syntax manipulation	Web app LDAP filter-lərinə injection testləri — *)(objectClass=*) klassik payload-lar
47	LDAPWildcardEnumerator	* wildcard brute	Wildcard-based blind LDAP injection — character-by-character data extraction
48	LDAPBooleanInjectionProber	&(|) logic manipulation	Boolean-based blind LDAP injection — TRUE/FALSE response fərqi ilə data çıxarma
49	LDAPANDORLogicTester	&, |, ! operator abuse	Filter logic operator-ları ilə data leakage testləri
50	LDAPFilterLengthAbuser	Long filter + truncation	Filter length limit-lərini test edir — truncation-based leakage
51	LDAPBlindTimingInjector	Slow query construction	Timing-based blind LDAP injection — data extraction vaxt fərqi ilə
52	LDAPANDSPNExtractor	Injection + SPN queries	Injection-lar vasitəsilə SPN-ləri çıxarmaq — Kerberoast zənciri qidalanması
53	LDAPInjectionWebProbe	HTTP + LDAP backend	Web form-ların LDAP backend-inin olub-olmadığını fingerprint etmək
54	LDAPANDNestedFilterAbuser	Nested filter construction	Deeply nested filter-larla server-side processing overload + data leakage
55	LDAPResponseFormatAnalyzer	Bind/Search result structure	Injection nəticələrinin response-dakı ayırıcıları — visible data, error code, response time
⚔️ Credential & Hash Attacks
LDAP üzərindən credential/hash əldə etmə.



#	Alət adı	Texnika	Nə edir
56	LDAPKerberoastFeed	SPN query + TGS requests	LDAP-dan SPN-ləri çıxarıb Kerberoast-lara feed edir — zəncir automatizasiyası
57	LDAPASREPFeed	DONT_REQ_PREAUTH filter	Pre-auth disabled hesabları tapıb AS-REP Roast-a feed edir
58	LAPSPasswordExtractor	ms-Mcs-AdmPwd + msLAPS-Password	LAPS read right-i olan hər host-un local admin parolunu çıxarır
59	LAPSACLEnumerator	ms-Mcs-AdmPwd ACL	Kimin LAPS parolunu oxuya bildiyini tapır — RBCD attack prep
60	GMSAPasswordReader	msDS-GroupMSAMembership + AES decrypt	gMSA hesab parollarını oxuyur — PrincipalAllowedToRetrievePrincipals
61	LDAPUserPasswordSprayer	Simple bind + policy-aware	LDAP bind ilə password spray — lockout threshold-aware rate limiting
62	LDAPASREQTimingEnumerator	Bind error codes	data 525 vs 533 vs 775 fərqi ilə username + account status enumeration
63	LDAPServiceAccountAudit	SPN + password age	Servis hesablarının köhnə parollarını tapır — Kerberoast dəyərini artırır
64	LDAPPasswordNeverExpiresFinder	DONT_EXPIRE_PASSWORD	Parolu heç vaxt bitməyən hesablar — yüksək dəyərli crack hədəfləri
🔶 ADCS Attacks
ADCS template-lərinin LDAP üzərindən kəşfi və istismarı.



#	Alət adı	Texnika	Nə edir
65	ADCSObjectEnumerator	pKIEnrollmentService	Enterprise CA-ları tapır — CA name, DNS hostname, cert template-ləri
66	ADCSTemplateDumper	pKICertificateTemplate	Bütün template-ləri çıxarır — flags, EKU, enrollment rights
67	ADCSDangerousTemplateFinder	Template flag analysis	ESC1-ESC13 template zəifliklərini avtomatik tapır
68	ADCSEnrollmentRightsReader	msPKI-Certificate-Name-Flag	Enrollment rights-ları oxuyur — kimin hansı template-dən cert ala bildiyini
69	ADCSManageCAInterpreter	ManageCA right	ManageCA hüququ olan istifadəçiləri tapır — ESC7 attack path
70	ADCSCAConfigAbuser	EDITF_ATTRIBUTESUBJECTALTNAME2	CA flag-də SAN istismarını tapır — ESC6
71	ADCSOwnerAbuse	Template ownership	Template owner-lik right-i olan user-lar — DACL modification → ESC
72	ADCSWebEnrollmentProber	HTTP endpoint check	Web Enrollment / CES endpoint-lərinin aktivliyini yoxlayır — NTLM relay hədəfləri
🟢 Delegation & RBCD
LDAP-dan delegation konfiqurasiyalarının tapılması və istismar yoluna hazırlıq.



#	Alət adı	Texnika	Nə edir
73	LDAPSAllowedToDelegateFinder	msDS-AllowedToDelegateTo	Constrained delegation konfiqurasiyalarını tapır
74	LDAPRBCDTargetFinder	msDS-AllowedToActOnBehalfOfOtherIdentity	RBCD yazılmış obyektləri tapır
75	LDAPRBCDWriteRightsFinder	ACL + msDS-AllowedToAct...	RBCD attribute-unu yazma hüququ olan hesabları tapır
76	LDAPUnconstrainedDelegationFinder	UAC TRUSTED_FOR_DELEGATION	Unconstrained delegation-ı aktiv hesabları/DC-ləri tapır
77	LDAPResourceConstrainedDelegationMapper	RBCD + machine account ACL	RBCD zəncirini map-ləyir — machine account-ların kimlərə yazma hüququ
78	LDAPDelegationAttackPlanner	Graph + rules engine	Delegation config-lərindən attack path qrafikası qurur
79	LDAPSAllowedToActWriteAbuser	RBCD attribute write	RBCD attribute-unu yazır (write right varsa) — impersonation path
80	LDAPNotDelegatedBypassFinder	NOT_DELEGATED flag	Not-delegated hesabları tapır — təhlükəli istisnalar
🟡 GPO & Policy Attacks
GPO, script və policy obyektlərinin LDAP+SYSVOL kombinasiyası ilə təhlili.



#	Alət adı	Texnika	Nə edir
81	GPOObjectEnumerator	(objectClass=groupPolicyContainer)	Bütün GPO-ları listə alır — display name, gPCFileSysPath
82	GPOApplyRightsMapper	gPLink + ACL analysis	GPO-ların hansı OU-lara tətbiq edildiyini tapır — Tier-0-a tətbiq edilən GPO-lar
83	GPOLocalGroupPolicyReader	SYSVOL + GptTmpl.inf	Restricted Groups, Local Admin config-lərini oxuyur — hidden admin paths
84	GPOLogonScriptAnalyzer	SYSVOL + script files	Logon script-ləri tapır — içindəki credential və UNC path-lar
85	GPOMSIDTInstallerAbuser	SYSVOL + script təhlil	GPO-dan deploy edilən MSI-ları tapır — malicious install vector
86	GPOPathTraversalProber	SYSVOL UNC path	GPO file path-larında symlink/path traversal cəhdləri — write hüquq varsa malicious GPO
87	GPOResourceBasedACLFinder	GPO ACL	GPO obyektləri üzərində yazma hüququ olan user-ları tapır — malicious GPO deploy
88	GPOLAPSDeployFinder	SYSVOL + LAPS schema	LAPS deployment statusu — hansı OU-larda LAPS aktivdir
🎯 DC & Infrastructure Attacks
DC obyektləri və infrastrukturanın LDAP üzərindən hədəflənməsi.



#	Alət adı	Texnika	Nə edir
89	LDAPDCSyncRightFinder	ACL — DS-Replication-Get-Changes-All	DCSync hüququ olan istifadəçiləri tapır — replication rights mapping
90	LDAPAdminSDHolderAnalyzer	cn=AdminSDHolder,cn=System,...	AdminSDHolder ACL-ini oxuyur — protected accounts DACL-ın mənbəyi
91	LDAPDSHeuristicsReader	cn=Directory Service,cn=Windows NT,...	dsHeuristics dəyərini oxuyur — anonymous restrictions, SID filtering statusu
92	LDAPKrbtgtACLAnalyzer	krbtgt obyekt ACL	krbtgt obyekti üzərində yazma hüquqları — Golden Ticket path
93	LDAPDomainAdminsMemberReader	cn=Domain Admins	Domain Admins üzvlərini dəqiq siyahı ilə çıxarır
94	LDAPForeignSecurityPrincipalFinder	cn=ForeignSecurityPrincipals	Cross-domain principal-ları tapır — trust zəiflikləri
95	LDAPBuiltinAdminInterpreter	cn=Builtin	Builtin qrupların üzvlərini, daxili hüquqları təhlil edir
96	LDAPRODCPolicyReader	msDS-RevealedList	RODC password replication policy-ni oxuyur — RODC-də cache-lənən hesablar
97	LDAPSchemaAdminAttributeFinder	schema + custom attrs	Custom attribute-ları tapır — bəzən credential-lar burada saxlanılır
98	LDAPDnsZoneReader	cn=MicrosoftDNS	DNS zone-ları LDAP-dan oxuyur — AD-integrated DNS records, hidden hosts
📊 BloodHound / Graph Analysis
LDAP data-larından attack path qrafikası qurulması.



#	Alət adı	Texniki dərinlik	Nə edir
99	BloodHoundDataCollector	LDAP paged search + JSON builder	BloodHound-compatible JSON data collector — users, groups, ACL, sessions
100	BloodHoundEdgeBuilder	Graph edge logic	LDAP-dan toplanmış data-dan edge-lər qurur — MemberOf, GenericAll, DCSync, CanRDP
101	BloodHoundShortestPathFinder	Graph traversal	Current user-dan Domain Admins-ə shortest path hesablayır
102	BloodHoundTier0Mapper	Tier-0 obyekt analizi	Tier-0 obyektlərini tapır, çatma yollarını qrafikada işarələyir
103	BloodHoundACLAbusePathPlanner	ACL abuse rules engine	ACL right-larından avtomatik attack zənciri qurur — WriteDACL → DCSync → Golden
104	BloodHoundOwnedNodeTracker	Session state	Hansı obyektlərin "owned" olduğunu izləyir — incremental collection
105	BloodHoundReportExporter	Graph export	Attack path-ların vizual report çıxarışı — Markdown, PNG qrafika
⚔️ Authentication & Abuse
LDAP-lardan əldə edilən data-ların istismarı.



#	Alət adı	Texnika	Nə edir
106	LDAPPassTheHashBind	SASL NTLM + hash	NTLM hash ilə LDAP bind — PTH LDAP variant
107	LDAPKerberosBind	SASL GSSAPI + ticket	Kerberos ticket ilə LDAP bind — Pass-the-Ticket LDAP variant
108	LDAPWriteObjectAbuser	ldapmodify analog	İstənilən obyekt attribute-unu yazır — delegation, SPN, description dəyişikliyi
109	LDAPAddUserCreator	ldapadd analog	Yeni user/qrup obyekti yaradır — persistence
110	LDAPDACLAEditor	Security descriptor write	DACL edit — öz user-inə GenericAll əlavə etmək
111	LDAPSAMRNameMapping	SID ↔ name	SID ↔ sAMAccountName çevirmələri — bulk resolve
112	LDAPShadowCredentialWrite	msDS-KeyCredentialLink write	Shadow Credentials attribute yazır — PKINIT → NT hash
113	LDAPSPNRegisterAbuser	servicePrincipalName write	SPN qeyd edir — Kerberoast hədəfi yaratmaq / RBCD prep
🔶 Mitigation & Hardening Analysis
LDAP istismarına qarşı qorunma statusunun yoxlanması.



#	Alət adı	Texniki dərinlik	Nə edir
114	LDAPSigningEnforcementChecker	rootDSE + bind test	LDAP signing enforcement statusu — 2020 hardening dəyişikliyi yoxlanışı
115	LDAPChannelBindingChecker	LDAPS + EPA	Channel binding requirement statusu
116	LDAPAnonymousRestrictionChecker	Anonymous bind test	Anonymous bind məhdudiyyətləri — dsHeuristics 7-ci karakter
117	LDAPACLPrivilegeAuditor	ACL audit	Təhlükəli ACL-lərin audit reportu — kim nəyə çatır
118	LDAPObjectPermissionAnalyzer	Permission matrix	Domain obyektləri üzrə permission matrisi — risk scoring ilə
119	LDAPEscalationPathFinder	Graph + rules engine	LDAP data-dan privilege escalation yollarını avtomatik tapır
120	LDAPADCSHardeningChecker	Template + CA config	ADCS hardening statusu — dangerous template-ların disabling statusu
121	LDAPMitigationRecommender	Policy templates	LDAP hardening rekomendasiyaları — signing, anonymous bind, ACL cleanup
🔧 Post-Exploitation
LDAP foothold sonrası kalıcılıq və lateral movement.



#	Alət adı	Texnika	Nə edir
122	LDAPHiddenObjectCreator	Low-visibility object creation	AdminCount=0, description boş obyektlər yaratmaq — stealthy persistence
123	LDAPSIDHistoryInjector	sIDHistory write	Privileged SID əlavə etmək — migration-analog persistence
124	LDAPAdminSDHolderWrite	AdminSDHolder ACL write	AdminSDHolder ACL dəyişmə — SDProp gedişi ilə privileged ACL yayma
125	LDAPRODCPasswordFilterAbuse	msDS-RevealedList manip.	RODC password filter manipulasiyası — hash replication yönləndirmə
126	LDAPObfuscatedAttributeSaver	Custom attribute write	Custom attribute-a data yazmaq — hidden storage
127	LDAPSchemaExtensionAbuser	Schema update	Schema attribute əlavə etmək — persistence + data hiding
128	LDAPDCSyncPivotPlanner	DCSync + graph	DCSync hüququ əldə edilib edilmədiyini yoxlayır, zənciri planlayır
📊 Intelligence & Analysis
Toplanmış LDAP data-larının təhlili, report, detection testləri.



#	Alət adı	Texniki dərinlik	Nə edir
129	LDAPQueryPerformanceAnalyzer	Query timing statistics	LDAP query vaxtlarını ölçür — server load, page size optimizasiyası
130	LDAPAttackPathPlanner	Graph + rules engine	LDAP-specific attack path qrafikası — LAPS → local admin → RBCD → DCSync zənciri
131	LDAPEventLogCorrelator	Windows Event Log	2886/2887/2889 (LDAP binding) event-lərindən istismar izləri korrelyasiyası
132	LDAPReconReportGenerator	Markdown/HTML export	Bütün tapıntıların pentest report formatında çıxarışı
133	LDAPNoiseSimulator	Fake LDAP queries	Blue team detection testləri — LDAP enumeration SIEM alert-ləri tetiklənir mi
134	LDAPObjectChangeMonitor	invokeChangeNotifications	LDAP change notification monitor — real vaxtda obyekt dəyişikliklərini izləmək
135	LDAPKillChainMapper	MITRE ATT&CK mapping	Tapılan zəiflikləri ATT&CK texnika ID-ləri ilə map-ləyir (T1018 Remote System Discovery, T1087 Account Discovery, T1482 Domain Trust Discovery, T1098 Account Manipulation)
