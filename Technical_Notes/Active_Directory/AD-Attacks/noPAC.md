# Enumeration
```
# MachineAccountQuota (MAQ) Yoxlanılması
Get-ADDomain | Select-Object -Property MachineAccountQuota

Default olaraq bu dəyər 10-dur. Əgər 0-dırsa, yeni hesab yarada bilməyəcəyiniz üçün hücum çətinləşir.

# Domain Controller-in Müəyyən Edilməsi
Get-ADDomainController | Select-Object Name, IPv4Address

# Cari İstifadəçi Hüquqları
whoami /groups

# Patch (Yeniləmə) Səviyyəsinin Yoxlanılması
Get-HotFix | Where-Object {$_.HotFixID -match "KB5008380" -or $_.HotFixID -match "KB5008602"}
Qeyd: Əgər bu komanda heç bir nəticə vermirsə, deməli sistem böyük ehtimalla zəifdir (vulnerable).

# Avtomatlaşdırılmış Enumerate (noPAC skriptləri ilə)
python3 noPac.py inlanefreight.local/damundsen:'SQL1234!' -dc-ip 172.16.5.150 -use-ldap -check
```
