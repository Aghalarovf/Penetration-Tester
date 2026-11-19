# OSED Prep — Phase 0: Python Fundamentals

## Overview

Phase 0 builds the minimum Python fluency required before OSED-specific scripting (byte handling, sockets, fuzzing, debugger automation) begins in Phase 1. OffSec's own prerequisite for EXP-301 is "familiarity with writing Python3 code" — not computer-science depth. This phase is deliberately scoped to procedural, script-level Python: no object-oriented design, no advanced language features.

**Target duration:** 1 week at 2–3 hours/day.

## 1. Environment & Execution

- Installing Python 3 and verifying the interpreter version
- Running a script from the command line (`python3 script.py`)
- Basic script structure and the `if __name__ == "__main__":` entry point
- Using a text editor / IDE (VS Code recommended) with a terminal side by side

## 2. Variables & Scalar Types

- Core types: `int`, `str`, `float`, `bool`
- Type conversion: `int()`, `str()`, `float()`, `hex()`, `bin()`
- Arithmetic operators: `+ - * / // % **`
- Comparison and boolean operators: `== != < > <= >=`, `and or not`
- Variable naming and dynamic typing basics

Note: this is the grammar layer only. `bytes`/`bytearray` — the type actually used to build exploit payloads — is deferred to Phase 1, where it belongs alongside `struct` and endianness.

## 3. Strings

- Indexing and slicing (`s[0:4]`, `s[::-1]`)
- Concatenation and repetition (`"A" * 100` — the core buffer-building idiom)
- `len()`
- f-string formatting (`f"offset: {x}"`)
- Common methods: `.strip()`, `.replace()`, `.split()`, `.join()`, `.upper()/.lower()`
- Recognizing where `str` stops being sufficient (anything sent over a socket or written as raw data needs `bytes`, covered in Phase 1)

## 4. Control Flow

- Conditionals: `if` / `elif` / `else`
- `for` loops with `range()`
- `while` loops (needed later for reading from a socket until a condition is met)
- `break` and `continue`
- Building a simple incrementing loop (the conceptual seed of a fuzzing loop: grow a buffer size each iteration)

## 5. Data Structures

- `list`: creation, indexing, `.append()`, iteration — used to accumulate payload pieces or scan results
- `dict`: key-value pairs, `.get()`, iteration — used for offset tables and configuration data
- `tuple`: recognizing the form, since `socket.connect((host, port))` requires one
- Deliberately skipped: `set` and comprehensions. They rarely appear in exploit scripts and are not worth the time here.

## 6. Functions

- `def`, parameters, `return`
- Default parameter values
- Returning multiple values (`return a, b`)
- Refactoring a script into functions (e.g., turning the control-flow fuzzing loop into a reusable function that accepts min/max/step)

## 7. File I/O

- `open()`, `.read()`, `.write()`, the `with` block
- Text mode vs. binary mode (`"rb"` / `"wb"`) — binary mode is required for shellcode files and any raw byte output
- Writing simple logs (useful for recording fuzzing results and crash offsets)

## 8. Error Handling

- `try` / `except` / `finally`
- `except Exception as e:` to catch and inspect errors
- Why this matters specifically: a target crashing mid-exploit must not also crash the exploit script — socket errors and connection resets need to be caught gracefully

## 9. Modules & Imports

- `import socket`, `import struct`, `import sys`, `import time`
- `sys.argv` for reading command-line parameters (target IP, port, offsets)
- Recognizing these four modules now so Phase 1 (bytes/sockets) starts from familiar syntax, not new syntax plus new concepts at once

---

---

# OSED Phase 0 — 15 Practical Tasks

Each task maps directly to an OSED exploit-dev workflow element: buffer building, offset/address math, config tables, fuzzing logic, logging, and crash resilience. Ordered from easy to hard.

---

## Task 1 — Ünvan Hesablayıcı
**Mövzular:** dəyişənlər, hex/bin çevirmə, arifmetika, f-string

Base ünvanı (`0x10000000`) və bir neçə offset verilib. Proqram hər offset üçün yekun ünvanı hesablamalı və `0x` prefiksli, 8 rəqəmli hex formatında (`f"{addr:#010x}"`) çap etməlidir.

**Tələblər:**
- Offset-lər siyahı şəklində verilsin: `offsets = [0x1a4, 0x2b8, 0x3f0]`
- Hər nəticə ayrı sətirdə: `"Base: ... + Offset: ... = Address: ..."`
- Heç bir hazır kitabxana işlədilməsin

---

## Task 2 — Təkrarlanan Bufer Generatoru
**Mövzular:** strings, funksiyalar, loop

`build_buffer(char, count)` funksiyası yaz — verilən simvolu `count` dəfə təkrarlayıb qaytarsın.

100-dən 1000-ə qədər 100-lük addımlarla bufer yarat, hər addımda uzunluğunu gözlənilən ölçü ilə müqayisə et.

**Tələblər:**
- Təkrarlama yalnız funksiya daxilində, inline yox
- Nəticə `if` ilə yoxlanılsın: `"Size: {X} | Built: {Y} | Match: {True/False}"`

---

## Task 3 — Qarışıq Payload Qurucusu
**Mövzular:** strings, concatenation, siyahı

Bir neçə hissədən ibarət payload qur: `"A" * 100` + `"B" * 4` + `"C" * 50`. Hər hissəni siyahıya `.append()` et, sonda `"".join()` ilə birləşdir.

**Tələblər:**
- Hər hissənin uzunluğunu ayrıca çap et
- Yekun bufer uzunluğunun düzgünlüyünü yoxla (`100+4+50` ilə müqayisə)

---

## Task 4 — Unikal Cyclic Pattern Generatoru
**Mövzular:** strings, loop, modulo

`generate_pattern(length)` funksiyası yaz: böyük hərf + kiçik hərf + rəqəm ardıcıllığı ilə (`Aa0Aa1Aa2...Ab0Ab1...`) verilən uzunluqda unikal pattern qaytarsın.

**Tələblər:**
- Hərf indeksləri `%` (modulo) ilə hesablansın, hardcode olunmasın
- Pattern-in uzunluğu dəqiq istənilən `length`-ə bərabər olsun (artıq hissə kəsilsin)

---

## Task 5 — Offset Axtarış Simulyasiyası
**Mövzular:** strings, slicing, find/index

Verilmiş pattern (Task 4-dən) daxilində 4 hərflik "crash marker" (`"Ab1A"`) harada yerləşdiyini tap.

**Tələblər:**
- `.find()` metodundan istifadə et, əl ilə döngü yazma (burada `.find()` real OSED iş axınında istifadə olunan metoddur)
- Tapılmazsa aydın mesaj ver (`-1` nəticəsini emal et)
- Tapılan indeksi "bu, EIP-yə qədər olan offset-dir" izahı ilə çap et

---

## Task 6 — Fuzzing Döngüsü (prototip)
**Mövzular:** while loop, funksiyalar, break

`fuzz_sizes(min_size, max_size, step)` funksiyası yaz — ölçülərin siyahısını qaytarsın (hələ heç nə göndərmir).

Əsas proqramda bu siyahı üzərində gəz, hər ölçüdə bufer yarat (Task 2-dəki funksiyadan istifadə et) və ölçü müəyyən həddi (`300`) keçəndə `break` ilə dayan.

**Tələblər:**
- `while` istifadə et, `for` yox (sonrakı fazada socket-dən oxuma məntiqinin eyni forması)
- Dayanma şərti ayrıca dəyişənlə idarə olunsun (`crash_threshold`)

---

## Task 7 — Offset Konfiqurasiya Cədvəli
**Mövzular:** dict, tuple, funksiyalar

Lüğət qur: `offsets = {"EIP": 524, "ESP": 528, "SEH": 1024}`. Dəyərlər əslində `(offset, description)` tuple-ı olsun: `{"EIP": (524, "instruction pointer")}`.

`get_offset(table, name)` funksiyası yaz: `.get()` ilə axtarsın, tapılmasa `None` qaytarsın.

**Tələblər:**
- `for key, value in table.items():` ilə bütün cədvəli çap et
- Mövcud olmayan açar üçün `get_offset` None qaytaranda əsas proqram bunu idarə etsin

---

## Task 8 — Badchar Yoxlama Simulyasiyası
**Mövzular:** strings, loop, list, conditional

Fərz et ki, bu badchar siyahısı var: `bad_chars = ["\x00", "\x0a", "\x0d"]`. Bir "payload" string-i (`"\x01\x02\x00\x03"`) daxilində bu simvollardan hansının mövcud olduğunu tap.

**Tələblər:**
- Hər badchar üçün `in` operatoru ilə yoxla, tapılanları siyahıya yığ
- Nəticəni "Tapılan badchar-lar: [...]" formasında çap et, heç biri tapılmasa aydın mesaj ver

---

## Task 9 — JMP/Gadget Ünvan Cədvəli
**Mövzular:** dict, list, funksiyalar, loop

Modul adlarını gadget ünvanlarına uyğunlaşdıran lüğət qur: `{"user32.dll": [0x77D1A1A1, 0x77D1B2B2], "kernel32.dll": [0x76F1C3C3]}` (hər modulun bir neçə namizəd ünvanı var).

`find_gadgets(table, module)` funksiyası yaz: verilən modulun bütün ünvanlarını hex formatında siyahı kimi qaytarsın.

**Tələblər:**
- Ünvanlar çap olunanda `f"{addr:#010x}"` formatında göstərilsin
- Mövcud olmayan modul adı üçün boş siyahı qaytarılsın, xəta verilməsin

---

## Task 10 — Fayla Crash Log Yazma
**Mövzular:** file I/O, with block, funksiyalar

`log_crash(filename, size, address)` funksiyası yaz: fayla `"[CRASH] size={size} eip={address:#010x}"` formatında sətir əlavə etsin (append rejimi).

Task 6-dakı fuzzing döngüsünü bununla birləşdir: hər "crash" halında (ölçü həddi keçəndə) bu funksiyanı çağır.

**Tələblər:**
- `with open(..., "a")` istifadə olunsun
- Hər çağırışda vaxt əlavə olunsun (`time.strftime`)

---

## Task 11 — Binary Fayl Yazma/Oxuma
**Mövzular:** file I/O, binary mode

Bufer massivini (`[0x41, 0x42, 0x43, 0x44]`) `bytes()` ilə çevir və `payload.bin` faylına `"wb"` rejimində yaz.

Sonra faylı `"rb"` ilə geri oxu və `.hex()` metodu ilə hex string kimi çap et.

**Tələblər:**
- `bytes([...])` sintaksisini işlət (Phase 1-ə keçid üçün ilk tanışlıq)
- Yazılan və oxunan datanın eyni olduğunu `if` ilə yoxla

---

## Task 12 — Xətaya Davamlı Konvertasiya
**Mövzular:** try/except, funksiyalar

`safe_hex_to_int(value)` funksiyası yaz: `"0x41414141"` kimi string-i `int(value, 16)` ilə çevirsin, səhv formatda olduqda `None` qaytarsın (çökməsin).

Qarışıq siyahını (`["0x41414141", "invalid", "0xDEADBEEF", ""]`) emal et, uğurlu/uğursuzları ayrı siyahılara yığ.

**Tələblər:**
- `except ValueError:` spesifik tutulsun
- `try/except` yalnız funksiya daxilində olsun, əsas döngüdə yox

---

## Task 13 — Komanda Sətri ilə Exploit Konfiqurasiyası
**Mövzular:** sys.argv, try/except, conditional

Proqram üç arqument qəbul etsin: hədəf ünvan (string kimi, hələ IP formatı yoxlanmır), port və bufer ölçüsü (`python script.py 127.0.0.1 9999 500`).

**Tələblər:**
- Arqument sayı səhvdirsə `"Usage: script.py <host> <port> <size>"` göstər və `sys.exit(1)`
- Port və ölçünün `int()`-ə çevrilməsi `try/except` ilə qorunsun
- Doğru parametrlər daxil olanda hamısını f-string ilə çap et: `"Target: {host}:{port} | Buffer size: {size}"`

---

## Task 14 — Struct Pack Sintaksisinə Giriş
**Mövzular:** import struct, funksiyalar

`pack_address(addr)` funksiyası yaz: `struct.pack("<I", addr)` istifadə edərək 32-bitlik ünvanı little-endian bytes-a çevirsin.

Bir neçə ünvan üçün (`0x41424344`, `0x7FFDF000`) nəticəni çevir və `.hex()` ilə çap et.

**Tələblər:**
- `"<I"` (little-endian) ilə `">I"` (big-endian) arasındakı fərqi eyni ünvan üçün çap edib müqayisə et
- Sintaksisə alışmaq məqsədlidir — endianness-in **niyə** vacib olduğunu izah etməyə çalışma, bu Phase 1 mövzusudur

---

## Task 15 — Capstone: Mini Exploit Skeleton
**Mövzular:** HAMISI birlikdə (sys.argv, dict/tuple, funksiyalar, loop, try/except, file I/O, struct)

Aşağıdakı axını izləyən tam skript yaz (real şəbəkə əlaqəsi YOXDUR, hər şey simulyasiyadır):

1. `sys.argv`-dən host, port, max bufer ölçüsü oxu (default dəyərlərlə)
2. Offset cədvəli (Task 7-dəki kimi) təyin et
3. `fuzz_sizes()` (Task 6) ilə ölçü siyahısı yarat
4. Hər ölçü üçün bufer qur (Task 2), `simulate_send(size)` çağır — bu funksiya ölçü müəyyən həddi keçəndə `Exception` atsın
5. Çökmə baş verəndə: offset cədvəlindən EIP dəyərini oxu, `pack_address()` (Task 14) ilə bytes-a çevir, nəticəni `log_crash()` (Task 10) ilə fayla yaz
6. Bütün proses `try/except` ilə qorunsun, heç bir halda skript çökməsin

**Tələblər:**
- Bütün funksiyalar qlobal dəyişənə müraciət etməsin, hər şey parametr/return ilə işləsin
- Əsas məntiq `if __name__ == "__main__":` daxilində olsun
- Bu skripti başdan sona izah edə bilirsənsə, Phase 1-ə (bytes, socket, real struct işi) keçməyə hazırsan

---

## Qeyd

Task 11 və 14 artıq `bytes` və `struct`-a toxunur — bu, Phase 0-ın "qrammatika" sərhədini bir az aşır, amma OSED-ə keçiddə bu iki alət o qədər mərkəzidir ki, onlarsız siyahı "sırf OSED üçün" sayıla bilməzdi.

