# Active Directory Certificate Services (AD CS) Pentesting Roadmap

This comprehensive, 50-step roadmap is designed to take you from a fundamental understanding of Public Key Infrastructure (PKI) to an advanced level of executing, analyzing, and mitigating AD CS vulnerabilities within an enterprise Active Directory environment.

---

## Phase 1: Foundations & Architecture (Steps 1 - 10)

The goal of this phase is to master the mathematical, cryptographic, and architectural blueprints behind AD CS and its deep integration with Active Directory.

1. **Master Public Key Infrastructure (PKI):** Study asymmetric cryptography, public/private key pairs, and the mathematical trust mechanics of digital signatures.
2. **Deconstruct the X.509 Certificate Structure:** Learn to read and parse fields within an X.509 certificate, focusing on `Subject`, `Issuer`, `Validity Period`, and `Subject Alternative Name (SAN)`.
3. **Analyze Extended Key Usages (EKUs) & OIDs:** Understand Object Identifiers (OIDs) and map out critical EKUs such as Client Authentication (`1.3.6.1.5.5.7.3.2`), Smartcard Logon (`1.3.6.1.4.1.311.20.2.2`), and Any Purpose (`2.5.29.37.0`).
4. **Differentiate Certificate Encodings:** Learn the practical differences between `.DER`, `.PEM`, `.CRT`, `.PFX`, and `.P12` formats. Practice converting between them using `openssl`.
5. **Dive into ASN.1 & BER/DER Encoding:** Understand how certificate data is structured at the byte level using Abstract Syntax Notation One (ASN.1) and Distinguished Encoding Rules (DER).
6. **Map AD CS Enterprise Architecture:** Study the layout of an Enterprise CA vs. a Standalone CA, and the trust hierarchy of Root CAs and Subordinate CAs.
7. **Understand Certificate Templates:** Analyze how blueprints for certificates are defined, managed, and replicated across the Active Directory Configuration Naming Context (Configuration NC).
8. **Inspect AD CS LDAP Object Classes:** Query and visualize the directory objects that store PKI data, such as `pKIEnrollmentService`, `pKIAuthority`, and `certificateTemplate`.
9. **Deconstruct Certificate-Based Authentication:** Trace the step-by-step process of how a user presents a certificate to the KDC (Domain Controller) to request a Kerberos TGT via the PKINIT protocol.
10. **Analyze Revocation Mechanics:** Study how systems verify certificate validity using Certificate Revocation Lists (CRLs) and the Online Certificate Status Protocol (OCSP).

---

## Phase 2: Lab Construction & Reconnaissance (Steps 11 - 20)

Build your specialized testing environment and master the techniques required to discover AD CS infrastructure within a target network.

11. **Build a Multi-Domain Lab Environment:** Set up a local Windows Server lab featuring a Root Domain and at least one Child Domain using Active Directory Domain Services (AD DS).
12. **Install and Configure an Enterprise CA:** Deploy the AD CS role on a dedicated member server and configure it as an Enterprise Root CA linked to your Active Directory forest.
13. **Enable HTTP Enrollment Interfaces:** Configure Web Enrollment (`/certsrv/`) and the Certificate Enrollment Web Service (CES) to simulate real-world legacy exposure.
14. **Deploy Flawed Templates:** Purposefully create and publish custom templates mimicking misconfigurations for vulnerabilities ESC1 through ESC8.
15. **Assemble the Attack Toolkit:** Install and configure modern AD CS auditing tools including `Certipy`, `Certify`, `SharpHound`, and `BloodHound`.
16. **Conduct Passive Network Reconnaissance:** Utilize network capture tools (e.g., Wireshark) and port scanners (`nmap`) to map out active CA servers via RPC (port 135), SMB (port 445), and HTTP/HTTPS.
17. **Perform Low-Level LDAP Queries:** Practice identifying CA servers and templates using native tools like `ldapsearch` (Linux) or `ADSI Edit` (Windows) without running noisy specialized tools.
18. **Master Certipy Auditing:** Run `certipy find -vulnerable` against your domain and analyze every parameter of the generated text and clean output.
19. **Execute Internal Audits with Certify:** Simulate an unprivileged insider attack path by running `Certify.exe find /vulnerable` from a standard domain user session.
20. **Map Paths with BloodHound:** Ingest collected AD CS data into BloodHound to visualize attack graphs extending from low-privilege accounts to the Certificate Authority and Domain Admin groups.

---

## Phase 3: Misconfigured Template Exploitation (ESC1 - ESC4) (Steps 21 - 30)

Focus on the classic template-level misconfigurations identified by SpecterOps that allow immediate privilege escalation.



21. **Analyze ESC1 (Enrollee Supplies Subject):** Learn the exact preconditions where a template allows the `CT_PRIVATE_FLAG_ENROLLEE_SUPPLIES_SUBJECT` flag alongside an authentication EKU.
22. **Execute ESC1 Weaponization:** Use `Certipy` or `Certify` to request a certificate against an ESC1 template, specifying a Domain Administrator's User Principal Name (UPN) in the SAN field.
23. **Extract TGT & NTLM Hashes via PKINIT:** Request a Kerberos TGT using the generated `.pfx` certificate via `Rubeus` or `Certipy auth`, and extract the Administrator's NTLM hash.
24. **Deconstruct ESC2 (Any Purpose / No EKU):** Study how templates containing the "Any Purpose" EKU or an empty EKU can be abused for any authentication type across the domain.
25. **Deconstruct ESC3 (Certificate Request Agent):** Understand the mechanics of the Enrollment Agent EKU (`1.3.6.1.4.1.311.20.2.1`), which allows an attacker to request certificates on behalf of other directory users.
26. **Execute the Two-Stage ESC3 Attack:** Request an Enrollment Agent certificate first, then use that certificate to co-sign a secondary request for a target Administrator certificate.
27. **Analyze ESC4 (Template Access Control Abuse):** Identify templates where weak Active Directory Access Control Entries (ACEs) grant permissions like `WriteDacl` or `WriteOwner` to low-privileged groups.
28. **Perform Template Modification & Rollback:** Use `Certipy template -config` to overwrite an ESC4 template's parameters to turn it into an ESC1 template, exploit it, and restore the original configuration cleanly.
29. **Target Cross-Domain Templates:** Abuse misconfigured templates published by the root domain that allow enrollment access to users within a compromised child domain.
30. **Review Event Logs for ESC1-ESC4:** Analyze Windows Security Logs (specifically Event ID 4886 "Certificate request received" and Event ID 4887 "Certificate issued") to map the defensive footprint of template exploitation.

---

## Phase 4: Architecture, Access Control & Relay Attacks (ESC5 - ESC8) (Steps 31 - 40)

Expand your perimeter to encompass weaknesses found in CA permissions, configuration containers, and network authentication protocols.

31. **Analyze ESC5 (PKI Object ACL Abuse):** Audit security descriptors on the CA server's AD objects, the `CN=Public Key Services` container, and the CA computer object itself to identify unauthorized write paths.
32. **Deconstruct ESC6 (CA Configuration Abuse):** Study the impact of the `EDITF_ATTRIBUTESUBJECTALTNAME2` flag enabled on the CA server, which forces the CA to honor user-supplied SANs on *any* template.
33. **Analyze ESC7 (CA Administrator / Officer Privileges):** Map out the attack chain when an account holds `ManageCA` or `ManageCertificates` rights over the CA server.
34. **Execute Pending Certificate Release:** Use ESC7 rights to issue a malicious certificate request that was automatically held for approval, then remotely approve and retrieve it.
35. **Deconstruct ESC8 (NTLM Relay to Web Enrollment):** Master the theoretical mechanics of relaying incoming NTLM authentication to unencrypted HTTP AD CS Web Enrollment endpoints (`/certsrv/certfnsh.asp`).
36. **Execute ESC8 via Coerced Authentication:** Set up `ntlmrelayx` pointing to the CA Web Enrollment portal, use a tool like `PetitPotam` (MS-EFSR) to coerce authentication from a Domain Controller, and relay that request to capture the DC's machine certificate.
37. **Perform Pass-the-Certificate & DCSync:** Take the relayed Domain Controller machine certificate, perform PKINIT authentication to act as the DC, and execute a `DCSync` operation to dump the domain NTDS database.
38. **Examine ESC9 & ESC10 (Strong Certificate Mapping):** Research modern update patches (such as KB5014754) and learn how explicit UPN mappings and weak `StrongCertificateBindingEnforcement` configurations can be bypassed.
39. **Exploit Certificate Renewal Policies:** Learn to abuse valid, expiring user certificates to sign a renewal request that extends persistence or modifies attributes under specific template constraints.
40. **Establish External Persistence:** Use a compromised certificate to authenticate against corporate VPNs or internal federated portals that rely on AD CS for authentication, circumventing traditional password-reset procedures.

---

## Phase 5: Advanced Engineering, Tool Crafting & Defense (Steps 41 - 50)

Transition from using automated attack scripts to understanding low-level memory extraction, building customized logic, and establishing enterprise-grade mitigations.

41. **Extract Certificates from Memory:** Use post-exploitation tools like `Mimikatz` (`crypto::certificates`) to harvest private keys and certificates stored within the LSASS process space or local certificate stores.
42. **Decrypt Certificates via DPAPI:** Trace how private keys are protected on disk using the Data Protection API (DPAPI) and practice decrypting them using extracted user master keys.
43. **Design Certificate-Based Persistence:** Implement long-lived, stealthy access by creating client certificates with 10+ year validities on obscure templates that escape routine cleanups.
44. **Architect Custom AD CS Decision Engines:** Analyze raw LDAP properties programmatically. Map out the conditions required to build an automated, zero-dependency engine capable of calculating AD CS exploit chains from raw JSON or LDAP dumps.
45. **Deconstruct Bit-Level PKINIT Packets:** Use packet analysis tools to look at the exact ASN.1 structure of the `AS-REQ` and `AS-REP` packets when injecting a certificate into a Kerberos pre-authentication flow.
46. **Implement AD CS Hardening:** Practice securing your lab by disabling Web Enrollment HTTP endpoints, enforcing Extended Protection for Authentication (EPA), disabling unused templates, and enforcing SSL.
47. **Deploy Strong Template ACLs:** Remove broad groups like `Authenticated Users`, `Domain Users`, or `Everyone` from template enrollment rights, replacing them with explicit, minimized tier-based security groups.
48. **Construct Honey-Templates:** Create highly attractive but non-functional certificate templates (e.g., named `DomainAdmin-Logon` with ESC1 indicators) to act as canary objects that log and alert on unauthorized enrollment attempts.
49. **Write Professional-Grade Mitigation Reports:** Practice translating complex AD CS exploitations into structured, business-ready remediation documentation complete with risk scoring (CVSS) and step-by-step group policy fixes.
50. **Simulate Full-Scope Active Directory Engagements:** Combine all phases inside a complex, hardened lab environment. Detect the CA, bypass standard detection filters, pivot cross-domain via certificate injection, and secure the domain infrastructure against your own attack footprint.
