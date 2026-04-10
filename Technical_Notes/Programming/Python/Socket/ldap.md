# 🔬 LDAP Protocol — Low-Level Deep Dive (Down to the Byte)

> **LDAP** = Lightweight Directory Access Protocol  
> RFC 4511 (LDAPv3) · Runs over TCP port **389** (plain) / **636** (LDAPS/TLS)  
> Encoding: **ASN.1 BER** (Basic Encoding Rules)

---

## 1. Architecture Overview

```
┌─────────────────────────────────────────┐
│            LDAP Client                  │
│  (ldapsearch, Python ldap3, AD, etc.)   │
└────────────────┬────────────────────────┘
                 │ TCP/IP  (port 389 or 636)
┌────────────────▼────────────────────────┐
│            LDAP Server                  │
│  (OpenLDAP, Active Directory, 389-DS)   │
│                                         │
│  DIT (Directory Information Tree)       │
│  dc=example,dc=com                      │
│    ├── ou=users                         │
│    │     └── cn=alice                   │
│    └── ou=groups                        │
│          └── cn=admins                  │
└─────────────────────────────────────────┘
```

---

## 2. The Encoding Engine: ASN.1 BER

Every single byte in LDAP is encoded using **ASN.1 BER (Basic Encoding Rules)**.  
Each value is a **TLV triplet**:

```
┌──────────┬──────────┬──────────────────┐
│   TAG    │  LENGTH  │      VALUE       │
│ (1+ byte)│ (1+ byte)│   (N bytes)      │
└──────────┴──────────┴──────────────────┘
```

### 2.1 TAG Byte Structure

```
Bit:  7  6  5  4  3  2  1  0
      ├──┤  ├──┤  ├────────────┤
      Class  P/C    Tag Number
```

| Bits 7-6 | Class | Meaning |
|----------|-------|---------|
| `00` | Universal | Standard ASN.1 types (INTEGER, SEQUENCE…) |
| `01` | Application | LDAP-specific types |
| `10` | Context | Context-dependent (varies by position) |
| `11` | Private | Not used in LDAP |

| Bit 5 | Primitive/Constructed |
|-------|-----------------------|
| `0` | Primitive (raw value) |
| `1` | Constructed (contains nested TLVs) |


| Tag Number 4-0 | Hex (Primitive) | Binary (Bits 4–0) | Data Type | Notes |
|:---:|:---:|:---:|---|---|
| 0 | `0x00` | `00000` | **End-of-Content (EOC)** | Marks the end of indefinite-length encodings |
| 1 | `0x01` | `00001` | **BOOLEAN** | True = `0xFF`, False = `0x00` |
| 2 | `0x02` | `00010` | **INTEGER** | Signed integers — e.g. `02 01 05` → Integer 5 |
| 3 | `0x03` | `00011` | **BIT STRING** | Sequence of bits — e.g. flags, public keys |
| 4 | `0x04` | `00100` | **OCTET STRING** | Arbitrary byte sequence — heavily used in LDAP |
| 5 | `0x05` | `00101` | **NULL** | Empty value — used for operations with no parameters |
| 6 | `0x06` | `00110` | **OBJECT IDENTIFIER** | OID — e.g. `1.2.840.113554...` |
| 10 | `0x0A` | `01010` | **ENUMERATED** | A single value chosen from a predefined list |
| 12 | `0x0C` | `01100` | **UTF8String** | UTF-8 encoded text |
| 16 | `0x30`* | `10000` | **SEQUENCE** | Constructed — ordered list of elements |
| 17 | `0x31`* | `10001` | **SET** | Constructed — unordered collection of elements |
| 18 | `0x12` | `10010` | **NumericString** | Digits and space only (`0–9`, ` `) |
| 19 | `0x13` | `10011` | **PrintableString** | Restricted ASCII subset (no `@`, `&`, etc.) |
| 20 | `0x14` | `10100` | **TeletexString** | T.61 character set (legacy) |
| 22 | `0x16` | `10110` | **IA5String** | Standard ASCII — International Alphabet No. 5 |
| 23 | `0x17` | `10111` | **UTCTime** | Timestamp format: `YYMMDDHHMMSSZ` |
| 24 | `0x18` | `11000` | **GeneralizedTime** | Timestamp format: `YYYYMMDDHHMMSSZ` (higher precision) |
| 30 | `0x1E` | `11110` | **BMPString** | Two-byte Unicode characters (UCS-2) |
| 31 | — | `11111` | **Long Form Escape** | Tag number ≥ 31 — additional bytes carry the actual tag number |

### 2.2 Common Universal Tags

| Tag Byte | Hex | ASN.1 Type |
|----------|-----|-----------|
| `0x01` | 01 | BOOLEAN |
| `0x02` | 02 | INTEGER |
| `0x04` | 04 | OCTET STRING (raw bytes / UTF-8 strings) |
| `0x05` | 05 | NULL |
| `0x0A` | 0A | ENUMERATED |
| `0x30` | 30 | SEQUENCE (constructed) |
| `0x31` | 31 | SET (constructed) |

### 2.3 LENGTH Encoding

#### Short Form (value ≤ 127 bytes)
```
┌────────────────────┐
│  0  L  L  L  L  L  │   bit 7 = 0, bits 6-0 = actual length
└────────────────────┘
Example: length=5  →  0x05
```

#### Long Form (value > 127 bytes)
```
┌──────────────────────────────────────────────┐
│  1  N  N  N  N  N  N  N  │  byte1 │  byte2 … │
└──────────────────────────────────────────────┘
  bit7=1, bits 6-0 = how many following bytes hold the actual length

Example: length=300 (0x012C)
  0x82  0x01  0x2C
  │      └──────┴── 300 in 2 bytes (big-endian)
  └─ 0x82 = 1000_0010 → 2 length bytes follow
```

#### Indefinite Form (rarely used in LDAP)
```
0x80 ... content ... 0x00 0x00
```

---

## 3. LDAPMessage — The Top-Level Envelope

**Every** LDAP PDU (Protocol Data Unit) is wrapped in a `LDAPMessage`:

```asn1
LDAPMessage ::= SEQUENCE {
     messageID   MessageID,          -- INTEGER (0..maxInt)
     protocolOp  CHOICE { ... },     -- The actual operation
     controls    [0] Controls OPTIONAL
}
```

### Wire Layout

```
30 LL                        ← SEQUENCE (0x30 = Universal, Constructed, 16)
   02 xx [messageID bytes]   ← INTEGER  (messageID)
   [tag] LL [op bytes]       ← protocolOp (Application class tag)
   (a0 LL [controls])        ← optional Controls
```

### MessageID

```
Byte stream:  02  01  01
              │   │   └── value = 1
              │   └─────── length = 1 byte
              └─────────── tag = INTEGER (0x02)
```

---

## 4. Application Tags — All LDAP Operations

```
Tag   Hex   Operation                 Direction
 0    60    BindRequest               Client → Server
 1    61    BindResponse              Server → Client
 2    62    UnbindRequest             Client → Server
 3    63    SearchRequest             Client → Server
 4    64    SearchResultEntry         Server → Client
 5    65    SearchResultDone          Server → Client
 6    66    ModifyRequest             Client → Server
 7    67    ModifyResponse            Server → Client
 8    68    AddRequest                Client → Server
 9    69    AddResponse               Server → Client
10    6A    DelRequest                Client → Server
11    6B    DelResponse               Server → Client
12    6C    ModifyDNRequest           Client → Server
13    6D    ModifyDNResponse          Server → Client
14    6E    CompareRequest            Client → Server
15    6F    CompareResponse           Server → Client
16    70    AbandonRequest            Client → Server
19    73    SearchResultReference     Server → Client
23    77    ExtendedRequest           Client → Server
24    78    ExtendedResponse          Server → Client
25    79    IntermediateResponse      Server → Client
```

> **How to read Application tags:**  
> `0x60` = `0110_0000`  
> Bits 7-6: `01` = Application class  
> Bit 5: `1` = Constructed  
> Bits 4-0: `00000` = tag number 0 → BindRequest  

---

## 5. LDAPResult — The Common Response Body

Most responses share this structure:

```asn1
LDAPResult ::= SEQUENCE {
     resultCode    ENUMERATED { success(0), operationsError(1), ... },
     matchedDN     LDAPDN,
     diagnosticMsg LDAPString,
     referral      [3] Referral OPTIONAL
}
```

### Result Codes (ENUMERATED)

| Code | Hex | Meaning |
|------|-----|---------|
| 0 | 00 | success |
| 1 | 01 | operationsError |
| 2 | 02 | protocolError |
| 3 | 03 | timeLimitExceeded |
| 4 | 04 | sizeLimitExceeded |
| 7 | 07 | authMethodNotSupported |
| 8 | 08 | strongerAuthRequired |
| 10 | 0A | referral |
| 11 | 0B | adminLimitExceeded |
| 16 | 10 | noSuchAttribute |
| 17 | 11 | undefinedAttributeType |
| 20 | 14 | attributeOrValueExists |
| 21 | 15 | invalidAttributeSyntax |
| 32 | 20 | noSuchObject |
| 34 | 22 | invalidDNSyntax |
| 48 | 30 | inappropriateAuthentication |
| 49 | 31 | invalidCredentials |
| 50 | 32 | insufficientAccessRights |
| 51 | 33 | busy |
| 52 | 34 | unavailable |
| 53 | 35 | unwillingToPerform |
| 64 | 40 | namingViolation |
| 65 | 41 | objectClassViolation |
| 80 | 50 | other |

---

## 6. BindRequest — Byte-by-Byte Dissection

### 6.1 Simple Bind

A client authenticates with DN + password.

```asn1
BindRequest ::= [APPLICATION 0] SEQUENCE {
     version    INTEGER (1..127),     -- always 3 for LDAPv3
     name       LDAPDN,               -- OCTET STRING
     authentication  CHOICE {
          simple  [0] OCTET STRING,   -- Context tag 0, primitive
          sasl    [3] SaslCredentials
     }
}
```

**Example:** Bind as `cn=admin,dc=example,dc=com` with password `secret`

```
Byte offset   Hex   Meaning
──────────────────────────────────────────────────────────────────
 0            30    SEQUENCE (LDAPMessage)
 1            37    Length = 55 bytes

 2            02    INTEGER (messageID)
 3            01    Length = 1
 4            01    Value = 1

 5            60    APPLICATION 0, Constructed (BindRequest)
 6            32    Length = 50 bytes

 7            02    INTEGER (version)
 8            01    Length = 1
 9            03    Value = 3 (LDAPv3)

10            04    OCTET STRING (DN)
11            1A    Length = 26
12..37        63 6E 3D 61 64 6D 69 6E 2C 64 63 3D 65 78 61 6D
              70 6C 65 2C 64 63 3D 63 6F 6D
              ASCII: "cn=admin,dc=example,dc=com"

38            80    Context [0] Primitive (simple password)
39            06    Length = 6
40..45        73 65 63 72 65 74
              ASCII: "secret"
```

**Full hex dump:**
```
30 37
  02 01 01
  60 32
    02 01 03
    04 1A 63 6E 3D 61 64 6D 69 6E 2C 64 63 3D 65 78
          61 6D 70 6C 65 2C 64 63 3D 63 6F 6D
    80 06 73 65 63 72 65 74
```

### 6.2 BindResponse — Success

```
30 0C           LDAPMessage SEQUENCE, length=12
  02 01 01      messageID = 1
  61 07         APPLICATION 1 (BindResponse), length=7
    0A 01 00    ENUMERATED resultCode = 0 (success)
    04 00       matchedDN = "" (empty OCTET STRING)
    04 00       diagnosticMessage = "" (empty OCTET STRING)
```

---

## 7. SearchRequest — Byte-by-Byte

### 7.1 ASN.1 Definition

```asn1
SearchRequest ::= [APPLICATION 3] SEQUENCE {
     baseObject   LDAPDN,
     scope        ENUMERATED { baseObject(0), singleLevel(1), wholeSubtree(2) },
     derefAliases ENUMERATED { neverDerefAliases(0), derefInSearching(1),
                               derefFindingBaseObj(2), derefAlways(3) },
     sizeLimit    INTEGER (0..maxInt),  -- 0 = no limit
     timeLimit    INTEGER (0..maxInt),  -- 0 = no limit
     typesOnly    BOOLEAN,
     filter       Filter,
     attributes   AttributeSelection
}
```

### 7.2 Filter Encoding

```asn1
Filter ::= CHOICE {
     and              [0] SET OF Filter,       -- 0xA0
     or               [1] SET OF Filter,       -- 0xA1
     not              [2] Filter,              -- 0xA2
     equalityMatch    [3] AttributeValueAssertion,  -- 0xA3
     substrings       [4] SubstringFilter,     -- 0xA4
     greaterOrEqual   [5] AttributeValueAssertion,  -- 0xA5
     lessOrEqual      [6] AttributeValueAssertion,  -- 0xA6
     present          [7] AttributeDescription,     -- 0x87 (primitive!)
     approxMatch      [8] AttributeValueAssertion,  -- 0xA8
     extensibleMatch  [9] MatchingRuleAssertion      -- 0xA9
}
```

| Filter tag | Hex | Example LDAP filter |
|------------|-----|---------------------|
| `[0]` AND  | A0  | `(&(...)(...))`     |
| `[1]` OR   | A1  | `(\|(...)(...))`    |
| `[2]` NOT  | A2  | `(!(...))` |
| `[3]` EQ   | A3  | `(uid=alice)` |
| `[4]` SUB  | A4  | `(cn=ali*)` |
| `[5]` GE   | A5  | `(age>=18)` |
| `[6]` LE   | A6  | `(age<=65)` |
| `[7]` PRES | 87  | `(objectClass=*)` |
| `[8]` APPROX | A8 | `(cn~=alice)` |
| `[9]` EXT  | A9  | `(cn:caseExactMatch:=Alice)` |

### 7.3 Full SearchRequest Example

Search: base=`dc=example,dc=com`, scope=wholeSubtree, filter=`(uid=alice)`, attributes=[`cn`,`mail`]

```
30 41           LDAPMessage SEQUENCE, len=65
  02 01 02      messageID = 2
  63 3C         APPLICATION 3 Constructed (SearchRequest), len=60

    04 11       OCTET STRING (baseObject), len=17
    64 63 3D 65 78 61 6D 70 6C 65 2C 64 63 3D 63 6F 6D
    ── "dc=example,dc=com"

    0A 01 02    ENUMERATED scope = 2 (wholeSubtree)
    0A 01 00    ENUMERATED derefAliases = 0 (neverDeref)
    02 01 00    INTEGER sizeLimit = 0
    02 01 00    INTEGER timeLimit = 0
    01 01 00    BOOLEAN typesOnly = FALSE

    A3 0D       Filter [3] equalityMatch, len=13
      04 03 75 69 64        attr = "uid"
      04 05 61 6C 69 63 65  value = "alice"

    30 0C       AttributeSelection SEQUENCE, len=12
      04 02 63 6E           "cn"
      04 04 6D 61 69 6C     "mail"
```

### 7.4 SearchResultEntry — Byte Layout

```
30 ??           LDAPMessage
  02 01 02      messageID = 2
  64 ??         APPLICATION 4 Constructed (SearchResultEntry)

    04 ??       objectName (DN of matched entry)
    ...

    30 ??       PartialAttributeList SEQUENCE OF
      30 ??       PartialAttribute SEQUENCE
        04 ??     type = attribute name (e.g. "cn")
        31 ??     SET OF values
          04 ??   value (e.g. "Alice Smith")
      30 ??       next attribute ...
```

### 7.5 SearchResultDone

```
30 0C
  02 01 02      messageID = 2
  65 07         APPLICATION 5 (SearchResultDone)
    0A 01 00    resultCode = 0 (success)
    04 00       matchedDN = ""
    04 00       diagnosticMessage = ""
```

---

## 8. AddRequest — Byte Layout

```asn1
AddRequest ::= [APPLICATION 8] SEQUENCE {
     entry      LDAPDN,
     attributes AttributeList
}
AttributeList ::= SEQUENCE OF attribute SEQUENCE {
     type    AttributeDescription,
     vals    SET OF value AttributeValue
}
```

```
30 ??
  02 01 03        messageID = 3
  68 ??           APPLICATION 8 (AddRequest)

    04 ??         entry DN
    e.g.: "cn=bob,dc=example,dc=com"

    30 ??         AttributeList SEQUENCE OF
      30 ??         Attribute
        04 0B 6F 62 6A 65 63 74 43 6C 61 73 73  type="objectClass"
        31 ??         SET
          04 06 70 65 72 73 6F 6E               "person"
      30 ??
        04 02 63 6E                             type="cn"
        31 ??
          04 03 62 6F 62                        "bob"
```

---

## 9. ModifyRequest — Byte Layout

```asn1
ModifyRequest ::= [APPLICATION 6] SEQUENCE {
     object   LDAPDN,
     changes  SEQUENCE OF change SEQUENCE {
          operation  ENUMERATED { add(0), delete(1), replace(2) },
          modification PartialAttribute
     }
}
```

```
30 ??
  02 01 04        messageID = 4
  66 ??           APPLICATION 6 (ModifyRequest)

    04 ??         DN of entry to modify

    30 ??         SEQUENCE OF changes
      30 ??         change #1
        0A 01 02    operation = 2 (replace)
        30 ??       PartialAttribute
          04 04 6D 61 69 6C   type = "mail"
          31 ??
            04 0F 62 6F 62 40 65 78 61 6D 70 6C 65 2E 63 6F 6D
            value = "bob@example.com"
```

---

## 10. DeleteRequest & ModifyDNRequest

### DeleteRequest
```
30 ??
  02 01 05
  4A ??           APPLICATION 10 PRIMITIVE (DelRequest)
  [DN bytes directly as OCTET STRING content]

Example — delete "cn=bob,dc=example,dc=com":
30 20
  02 01 05
  4A 1A 63 6E 3D 62 6F 62 2C 64 63 3D 65 78 61 6D 70 6C 65 2C 64 63 3D 63 6F 6D
```

> Note: DelRequest uses APPLICATION 10 **primitive** (`0x4A`), NOT constructed.

### ModifyDNRequest (rename/move)
```asn1
ModifyDNRequest ::= [APPLICATION 12] SEQUENCE {
     entry         LDAPDN,
     newrdn        RelativeLDAPDN,
     deleteoldrdn  BOOLEAN,
     newSuperior   [0] LDAPDN OPTIONAL
}
```

---

## 11. ExtendedRequest / ExtendedResponse

Used for operations like **StartTLS**, **Password Modify**.

```asn1
ExtendedRequest ::= [APPLICATION 23] SEQUENCE {
     requestName  [0] LDAPOID,
     requestValue [1] OCTET STRING OPTIONAL
}
```

### StartTLS Request Wire Bytes

```
30 1D
  02 01 01
  77 18           APPLICATION 23 (ExtendedRequest)
    80 16          Context [0] primitive (requestName OID)
    31 2E 33 2E 36 2E 31 2E 34 2E 31 2E 31 34 36 36
    2E 32 30 30 33 37
    ── OID "1.3.6.1.4.1.1466.20037" (startTLS)
```

### Common Extended Operation OIDs

| OID | Operation |
|-----|-----------|
| `1.3.6.1.4.1.1466.20037` | StartTLS |
| `1.3.6.1.4.1.4203.1.11.1` | Password Modify |
| `1.3.6.1.4.1.4203.1.11.3` | Who Am I? |
| `1.3.6.1.1.8` | Cancel Operation |

---

## 12. Controls

Controls are optional extensions attached to any LDAPMessage.

```asn1
Controls ::= SEQUENCE OF control Control

Control ::= SEQUENCE {
     controlType   LDAPOID,
     criticality   BOOLEAN DEFAULT FALSE,
     controlValue  OCTET STRING OPTIONAL
}
```

Controls are wrapped in **Context [0] Constructed** = `0xA0`.

```
A0 ??             Controls wrapper
  30 ??           Control SEQUENCE
    04 ??         controlType OID (as string bytes)
    01 01 FF      criticality = TRUE  (01=BOOLEAN, 01=len, FF=true)
    04 ??         controlValue
```

### Common Control OIDs

| OID | Control |
|-----|---------|
| `1.2.840.113556.1.4.319` | Simple Paged Results |
| `1.3.6.1.4.1.4203.1.10.1` | Subentries |
| `2.16.840.1.113730.3.4.2` | ManageDSAIT |
| `1.2.840.113556.1.4.473` | Server-side Sort |
| `1.3.6.1.1.13.1` | Pre-read |
| `1.3.6.1.1.13.2` | Post-read |

---

## 13. SASL Authentication (Kerberos / DIGEST-MD5)

```asn1
SaslCredentials ::= SEQUENCE {
     mechanism   LDAPString,
     credentials OCTET STRING OPTIONAL
}
```

Tag for SASL in BindRequest: Context **[3] Constructed** = `0xA3`

```
30 ??
  02 01 01      messageID
  60 ??         BindRequest
    02 01 03    version=3
    04 00       name="" (anonymous base DN for SASL)
    A3 ??       [3] SASL credentials
      04 06 47 53 53 41 50 49   mechanism = "GSSAPI"
      04 ??                     (Kerberos token bytes)
```

---

## 14. Full TCP Conversation — Step by Step

```
CLIENT                                        SERVER
  │                                             │
  │──── TCP SYN ──────────────────────────────►│
  │◄─── TCP SYN-ACK ───────────────────────────│
  │──── TCP ACK ──────────────────────────────►│
  │                                             │
  │──── BindRequest (msgID=1) ────────────────►│
  │◄─── BindResponse (success) ────────────────│
  │                                             │
  │──── SearchRequest (msgID=2) ──────────────►│
  │◄─── SearchResultEntry (msgID=2) ───────────│  ← may repeat N times
  │◄─── SearchResultEntry (msgID=2) ───────────│
  │◄─── SearchResultDone (msgID=2, success) ───│
  │                                             │
  │──── ModifyRequest (msgID=3) ──────────────►│
  │◄─── ModifyResponse (msgID=3, success) ──────│
  │                                             │
  │──── UnbindRequest (msgID=4) ──────────────►│
  │──── TCP FIN ──────────────────────────────►│
  │◄─── TCP FIN ───────────────────────────────│
```

> Multiple requests can be **pipelined** (sent without waiting for response).  
> The **messageID** correlates requests to responses asynchronously.

---

## 15. LDAP URL Format

```
ldap://host:port/baseDN?attributes?scope?filter?extensions

Examples:
ldap://ldap.example.com/dc=example,dc=com
ldap://ldap.example.com:389/dc=example,dc=com?cn,mail?sub?(uid=alice)
ldaps://ldap.example.com:636/dc=example,dc=com
```

---

## 16. OID Encoding in BER (inside LDAP)

OIDs are written as dotted strings in LDAP (e.g. `"1.3.6.1.4.1.4203.1.11.1"`),  
but when encoded as BER OID (`0x06`) inside SNMP or X.509 they use a compact binary form.  
**In LDAP, OIDs are always carried as plain OCTET STRINGs** (UTF-8 dotted notation).

---

## 17. Distinguished Name (DN) Encoding Rules

DNs are plain **UTF-8 OCTET STRINGs** on the wire.  
Comma-separated RDNs, most-specific first:

```
cn=Alice Smith,ou=users,dc=example,dc=com
└─── RDN ────┘ └─RDN──┘ └── RDN ──┘└RDN┘
```

Special characters that MUST be escaped with `\`:

| Char | Escape |
|------|--------|
| `,`  | `\,`   |
| `=`  | `\=`   |
| `+`  | `\+`   |
| `<`  | `\<`   |
| `>`  | `\>`   |
| `#`  | `\#` (at start) |
| `;`  | `\;`   |
| `\`  | `\\`   |
| `"`  | `\"`   |
| NUL  | `\00`  |

---

## 18. Attribute Syntax OIDs (Common)

| OID | Syntax | Example value |
|-----|--------|---------------|
| `1.3.6.1.4.1.1466.115.121.1.7` | Boolean | `TRUE` |
| `1.3.6.1.4.1.1466.115.121.1.15` | Directory String (UTF-8) | `Alice Smith` |
| `1.3.6.1.4.1.1466.115.121.1.26` | IA5 String | `alice@example.com` |
| `1.3.6.1.4.1.1466.115.121.1.27` | Integer | `42` |
| `1.3.6.1.4.1.1466.115.121.1.24` | Generalized Time | `20240101120000Z` |
| `1.3.6.1.4.1.1466.115.121.1.40` | Octet String (binary) | `\x89PNG...` |
| `1.3.6.1.4.1.1466.115.121.1.12` | DN | `cn=admin,dc=example,dc=com` |

---

## 19. Python — Raw Socket LDAP (No Library)

```python
import socket
import struct

def encode_length(n: int) -> bytes:
    if n < 0x80:
        return bytes([n])
    elif n < 0x100:
        return bytes([0x81, n])
    else:
        b = n.to_bytes(2, 'big')
        return bytes([0x82]) + b

def tlv(tag: int, value: bytes) -> bytes:
    return bytes([tag]) + encode_length(len(value)) + value

def ldap_string(s: str) -> bytes:
    return tlv(0x04, s.encode('utf-8'))

def ldap_integer(n: int) -> bytes:
    length = max(1, (n.bit_length() + 8) // 8)
    return tlv(0x02, n.to_bytes(length, 'big'))

def ldap_enum(n: int) -> bytes:
    return tlv(0x0A, bytes([n]))

def build_bind_request(msg_id: int, dn: str, password: str) -> bytes:
    version    = ldap_integer(3)
    name       = ldap_string(dn)
    simple_pw  = tlv(0x80, password.encode('utf-8'))  # [0] primitive
    bind_body  = version + name + simple_pw
    bind_req   = tlv(0x60, bind_body)                  # APPLICATION 0
    msg_id_enc = ldap_integer(msg_id)
    return tlv(0x30, msg_id_enc + bind_req)            # SEQUENCE

def parse_result_code(response: bytes) -> int:
    # Minimal parser: find ENUMERATED (0x0A) after APPLICATION tag
    i = 0
    while i < len(response):
        if response[i] == 0x0A and response[i+1] == 0x01:
            return response[i+2]
        i += 1
    return -1

# ── Connect and Bind ────────────────────────────────
HOST = 'ldap.example.com'
PORT = 389

pkt = build_bind_request(1, 'cn=admin,dc=example,dc=com', 'secret')

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.connect((HOST, PORT))
    s.sendall(pkt)
    resp = s.recv(4096)

result = parse_result_code(resp)
print(f"Result code: {result} ({'success' if result == 0 else 'error'})")
print(f"Raw response: {resp.hex(' ')}")
```

---

## 20. Wireshark Filter Reference

```
ldap                    # all LDAP traffic
ldap.messageID == 1     # specific message
ldap.protocolOp == 0    # BindRequest
ldap.protocolOp == 3    # SearchRequest
ldap.resultCode != 0    # errors only
ldap.filter             # search filters
tcp.port == 389         # plain LDAP
tcp.port == 636         # LDAPS
```

---

## 21. Complete Tag/Byte Quick Reference Card

```
── Universal (always same meaning) ─────────────────────
02  INTEGER
04  OCTET STRING
05  NULL
0A  ENUMERATED
30  SEQUENCE (constructed)
31  SET (constructed)

── Application (LDAP operations) ───────────────────────
60  BindRequest           (App 0,  constructed)
61  BindResponse          (App 1,  constructed)
62  UnbindRequest         (App 2,  primitive)
63  SearchRequest         (App 3,  constructed)
64  SearchResultEntry     (App 4,  constructed)
65  SearchResultDone      (App 5,  constructed)
66  ModifyRequest         (App 6,  constructed)
67  ModifyResponse        (App 7,  constructed)
68  AddRequest            (App 8,  constructed)
69  AddResponse           (App 9,  constructed)
4A  DelRequest            (App 10, PRIMITIVE)
6B  DelResponse           (App 11, constructed)
6C  ModifyDNRequest       (App 12, constructed)
6D  ModifyDNResponse      (App 13, constructed)
6E  CompareRequest        (App 14, constructed)
6F  CompareResponse       (App 15, constructed)
50  AbandonRequest        (App 16, PRIMITIVE)
73  SearchResultReference (App 19, constructed)
77  ExtendedRequest       (App 23, constructed)
78  ExtendedResponse      (App 24, constructed)
79  IntermediateResponse  (App 25, constructed)

── Context (meaning depends on position) ───────────────
80  simple password in BindRequest       [0] primitive
A0  AND filter / Controls wrapper        [0] constructed
A1  OR filter                            [1] constructed
A2  NOT filter                           [2] constructed
A3  equalityMatch filter / SASL creds   [3] constructed
A4  substrings filter                    [4] constructed
A5  greaterOrEqual filter                [5] constructed
A6  lessOrEqual filter                   [6] constructed
87  present filter                       [7] PRIMITIVE
A8  approxMatch filter                   [8] constructed
A9  extensibleMatch filter               [9] constructed
```

---

*Based on RFC 4511 (LDAPv3) · RFC 4512 (Schema) · RFC 4513 (Auth)*  
*All byte values in hexadecimal · Big-endian byte order throughout*
