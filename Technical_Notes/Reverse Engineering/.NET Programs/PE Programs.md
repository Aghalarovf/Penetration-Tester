# File Identifier
```powershell
file target.exe
# PE32 / PE32+ (64-bit) / .NET assembly

xxd target.exe | head -4
# MZ başlığı: 4D 5A
```

# Entropy Identify
```powershell
import pefile
pe = pefile.PE("target.exe")
for section in pe.sections:
    print(section.Name, hex(section.VirtualAddress), section.get_entropy())

> 7.0 --> Encrypted
```

# Strings Analysis
```powershell
strings -n 8 target.exe | grep -iE "pass|pwd|key|token|secret|auth|bearer|apikey"
strings -el target.exe   # Unicode (UTF-16LE) stringlər
```

# Deobfuscation
```powershell
floss target.exe
# Stack strings, tight strings, decoded strings ayrı-ayrı çıxarır
```

# Disassembly
```powershell
1. File → Import File → target.exe
2. Auto-analysis işlət (öntəyin parametrlər)
3. Symbol Tree → Functions bölməsini aç
4. Axtarış: Search → For Strings → "pass", "token"
5. Tapılan stringe sağ klik → References → Show References to Address
```

# Decompile
```powershell
1. dnSpy-ə target.exe sürüklə
2. Tam C# kodu görünəcək
3. Edit → Search → "password" / "apiKey"
```

# Process Monitor
```powershell
Process Name is --> Program_Name.exe
```
