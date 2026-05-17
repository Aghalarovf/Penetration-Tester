# Fundamental
```powershell
Certification Authority (CA)
    Root CA - is the ultimate, self-signed authority at the top of the PKI hierarchy that is typically kept offline to protect the entire trust network.
    Subordinate CA - is authorized by the Root CA to operate online and handle the day-to-day issuance of certificates to end-users, devices, and servers.

Certificate Templates
    Validity Period
    Cryptography
    Subject Name
    Extensions (Purpose / EKU)

    Template Permission
         Read
         Enroll
         Autoenroll

    Template Version
         Version 1
         Version 2
         Version 3
         Version 4
```

# Template
```powershell
certutil -v -template

cn                                         Common Name
displayName                                Display name
msPKI-Cert-Template-OID                    Template Object Identifier    
msPKI-Certificate-Application-Policy
msPKI-Certificate-Name-Flag
msPKI-Enrollment-Flag
msPKI-Minimal-Key-Size
msPKI-Private-Key-Flag
msPKI-RA-Application-Policies
msPKI-RA-Signature
msPKI-Supersede-Templates
msPKI-Template-Minor-Revision
msPKI-Template-Schema-Version              Version
nTSecurityDescriptor                       Access Control List (ACL)
objectGUID                                 Unical İdentifier
pKIExtendedKeyUsage                        1.3.6.1.5.5.7.3.2 - Client Authentication,   1.3.6.1.5.5.7.3.1 - Server Authentication
pKICriticalExtensions
pKIDefaultKeySpec
pKIMaxIssuingDepth
flags
revision
CT_FLAG_SUBJECT_ALT_REQUIRE_UPN
CT_FLAG_SUBJECT_REQUIRE_DIRECTORY_PATH
CT_FLAG_ENROLLEE_SUPPLIES_SUBJECT         ESC1
CTPRIVATEKEY_FLAG_EXPORTABLE_KEY
```
