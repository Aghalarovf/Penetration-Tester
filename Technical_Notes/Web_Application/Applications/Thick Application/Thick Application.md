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
