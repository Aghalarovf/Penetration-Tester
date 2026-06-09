# General Process
```powershell
Hədəfi tap
    ↓
Binary / DLL faylını müəyyənləşdir
    ↓
Static Analysis (strings, file, objdump)
    ↓
Dynamic Analysis (GDB, ltrace, strace)
    ↓
Connection String / Credential çıxar
    ↓
Servislərə qoşul / Şifrəni yenidən sına
```

# First Reconnaissance
```powershell
file ./binary           # ELF, DLL, PE, Mach-O?
file ./library.dll      # .NET Assembly?

strings ./binary                    # bütün string-lər
strings ./binary | grep -i pass     # şifrə axtarışı
strings ./binary | grep -i server   # server axtarışı
strings ./binary | grep -i uid      # username axtarışı
strings ./binary | grep "DRIVER"    # ODBC connection string
strings ./binary | grep "Server="   # MSSQL connection string

xxd ./binary | less
hexdump -C ./binary | grep -A2 -B2 "pass"
```

# ELF Binary — GDB ilə Dynamic Analysis
```powershell
# PEDA extension ilə (tövsiyə edilir)
git clone https://github.com/longld/peda.git ~/peda
echo "source ~/peda/peda.py" >> ~/.gdbinit

gdb ./binary
```
<img width="786" height="572" alt="image" src="https://github.com/user-attachments/assets/0d214974-1a5c-4ca5-9023-a174c8b16394" />

# 
```powershell
gdb ./octopus_checker

gdb-peda$ set disassembly-flavor intel
gdb-peda$ disas main

# Axtarılacaq funksiyalar:
# SQLDriverConnect  → MSSQL/ODBC
# mysql_real_connect → MySQL
# PQconnectdb       → PostgreSQL
# sqlite3_open      → SQLite

gdb-peda$ b *0x5555555551b0    # SQLDriverConnect ünvanı
gdb-peda$ run

gdb-peda$ x/s $rdx
# Nəticə:
# "DRIVER={ODBC Driver 17 for SQL Server};SERVER=localhost,1401;UID=admin;PWD=secret123;"
```

# Connection String Formatları
```powershell
# MSSQL / ODBC
DRIVER={ODBC Driver 17 for SQL Server};SERVER=host,port;UID=user;PWD=pass;

# MySQL
Server=host;Database=db;Uid=user;Pwd=pass;

# PostgreSQL
Host=host;Port=5432;Database=db;Username=user;Password=pass;

# SQLite
Data Source=/path/to/file.db;
```

# DLL Files
```powershell
# Windows-da
Get-FileMetaData .\MultimasterAPI.dll

# Linux-da (mono lazımdır)
file MultimasterAPI.dll
# → PE32 executable (DLL) ... Mono/.Net assembly

github.com/dnSpyEx/dnSpy
File → Open → DLL faylını seç

MultimasterAPI.dll
    └── MultimasterAPI
        └── Controllers
            └── ColleagueController    ← bura bax
                └── Get()
                └── GetColleagues()    ← connection string burada

// Kod içində bu kimi görünür:
SqlConnection conn = new SqlConnection(
    "Server=localhost;Database=Multimaster;User Id=sa;Password=secret123;"
);
```

# DLL Analysis in Linux
```powershell
# strings ilə sürətli bax
strings MultimasterAPI.dll | grep -i "password\|pwd\|server\|database"

# monodis ilə
monodis --output=output.txt MultimasterAPI.dll

# ilspy (Linux versiyası)
ilspycmd MultimasterAPI.dll > decompiled.cs
```
