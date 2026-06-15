# Information Gathering
```powershell
# Fayl tipini yoxla
file application.exe

# .NET yoxla
strings application.exe | grep -i ".NET\|framework\|csharp"

# Java yoxla
file application.jar
jar tf application.jar

# Windows - Strings ilə
.\strings64.exe .\application.exe

# Linux
strings application.exe | grep -iE "pass|user|admin|key|secret|token"
```

# Static Analysis
```powershell
# De4Dot ilə obfuscation-ı sil
de4dot.exe application.exe
# → application-cleaned.exe yaradılır

# DnSpy ilə source kodu oxu
# application-cleaned.exe → DnSpy-a sürükle

# JADX ilə decompile et
jadx -d output_dir application.jar

# JAR məzmununu çıxar
jar xf application.jar
```

# Dynamic Analysis
```powershell
1. ProcMon64.exe aç
2. Filter: Process Name → application.exe
3. Tətbiqi işlət
4. İzlə:
   - Hansı fayllar yaradılır?
   - Hansı registry açarları sorğulanır?
   - Hansı network bağlantıları qurulur?
```

# Hardcoded Credential Axtarışı
```powershell
SMB → NETLOGON share → .exe tapıldı
        ↓
ProcMon → temp qovluğunda .bat yaradır
        ↓
Silinmə icazəsi ləğv edildi → .bat saxlandı
        ↓
.bat məzmunu:
  - Base64 kodlu məlumat → oracle.txt
  - PowerShell script → monta.ps1
  - oracle.txt decode → restart-service.exe
        ↓
x64dbg → Memory dump → .NET binary aşkarlandı
        ↓
de4dot → obfuscation silindi
        ↓
DnSpy → hardcoded credentials tapıldı ✅
```

# Credential Axtarışı üçün Komandalar
```powershell
# Strings ilə axtarış
strings.exe" .\Restart-OracleService.exe | Select-String -Pattern "pass|user|admin|web|login|secret|key" -CaseSensitive:$false

# PowerShell ilə
Select-String -Path .\*.exe -Pattern "password|username|secret" -Encoding Byte

# Registry-də hardcoded məlumat
reg query HKLM /f "password" /t REG_SZ /s
reg query HKCU /f "password" /t REG_SZ /s
```

# Procmon Analyze
```powershell
.\Restart-OracleService.exe

Procmon --> Filter --> Process Name (Restart-OracleService.exe) --> Operation (CreateFile)
```
<img width="932" height="41" alt="image" src="https://github.com/user-attachments/assets/c2f83141-363e-4004-8ef1-7d0c9321e8df" />
---

# With GUI
```powershell
C:\Users\cybervaca\AppData\Local\Temp

Properties -> Security -> Advanced -> cybervaca -> Disable inheritance -> Convert inherited permissions into explicit permissions on this object -> Edit -> Show advanced permissions, we deselect the Delete subfolders and files, and Delete checkboxes.
```

# With Powershell
```powershell
$sourcePath = "C:\Users\cybervaca\AppData\Local\Temp"
$destinationPath = "C:\TempKopyalar"
if (!(Test-Path -Path $destinationPath)) {
    New-Item -ItemType Directory -Path $destinationPath
}
$watcher = New-Object System.IO.FileSystemWatcher
$watcher.Path = $sourcePath
$watcher.Filter = "*.*"
$watcher.IncludeSubdirectories = $true
$watcher.EnableRaisingEvents = $true
$action = {
    $path = $Event.SourceEventArgs.FullPath
    $name = $Event.SourceEventArgs.Name
    $changeType = $Event.SourceEventArgs.ChangeType
    if ($changeType -eq "Created") {
        try {
            Start-Sleep -Seconds 1
            $destFile = Join-Path $destinationPath $name
            Copy-Item -Path $path -Destination $destFile -Force
        } catch {}
    }
}
Register-ObjectEvent $watcher "Created" -Action $action
while ($true) { Start-Sleep -Seconds 5 }
```
<img width="935" height="422" alt="image" src="https://github.com/user-attachments/assets/6034a9fa-7151-4de9-91f0-d5934ff94deb" />

https://forum.hackthebox.com/t/exploiting-web-vulnerabilities-in-thick-client-applications/276823/85

