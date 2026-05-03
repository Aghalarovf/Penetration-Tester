# MinGW Build System
```
cd build
cmake --version
cmake .. -G "MinGW Makefiles"
cmake --build .
g++ -Os first_program.cpp -o first_program.exe
strip first_program.exe
```
---

# Ninja Build System
```
pacman -S mingw-w64-ucrt-x86_64-ninja
ninja --version
cmake .. -G "Ninja"
cmake --build .
```
---

# C++ compilation process diagram
---
<img width="436" height="471" alt="image" src="https://github.com/user-attachments/assets/7a1bf545-e07f-4dbf-82c4-355d24c91857" />

```
1. Pre-Processor: Kodundakı #include və #define kimi sətirləri emal edir. Məsələn, #include <iostream> yazmısansa, həmin kitabxananın içindəki minlərlə sətir kodu sənin faylına "yapışdırır".

2. Compiler (məsələn, GCC və ya MSVC): Sənin yazdığın yüksək səviyyəli C++ kodunu Assembly koduna (prosessorun başa düşdüyü aşağı səviyyəli təlimatlar) çevirir.

3. Assembler: Assembly kodunu maşın dilinə, yəni Object file (.o və ya .obj) şəklində olan sıfırlara və birlərə çevirir. Amma bu fayl hələ təkbaşına işləyə bilmir.

4. Linker: Ən vacib son mərhələdir. Sənin yaratdığın obyekt fayllarını və istifadə etdiyin xarici kitabxanaları (Libraries) bir araya gətirib vahid bir Executable (.exe) faylı yaradır.
```

# Data Types
---
![C-data-types](https://github.com/user-attachments/assets/ae8201f6-63d8-49f1-9ba8-162cb789dc77)

```
LONG LONG  |   8 Byte
DOUBLE     |   8 Byte 
INT        |   4 Byte
LONG       |   4 Byte
FLOAT      |   4 Byte
SHORT INT  |   2 Byte
CHAR       |   1 Byte
BOOL       |   1 Byte
STRING     |   Dynamic   std::string ad = "Oxsium";
AUTO       |   Auto Byte
VOID       |   NULL
``` 
---

# Signed vs Unsigned Data
```
Signed char        |      01111111      |      +127
Signed char        |      11111111      |      −1 (2's complement)
Unsigned char      |      11111111      |      255

signed int a = -1;
unsigned int b = a;

#include <iostream>
#include <climits> // Show Limits
int main() {
    unsigned short say = 65535;
    std::cout << "Ilk deyer: " << say << std::endl;
    say = say + 1; // OVERFLOW 
    std::cout << "Overflow-dan sonra (65535 + 1): " << say << std::endl; 
    unsigned short eded = 1;
    std::cout << "Ilk deyer: " << eded << std::endl;
    eded = eded - 2; // UNDERFLOW
    std::cout << "Underflow-dan sonra (1 - 2): " << eded << std::endl;
    return 0;
}
```
---

# Operators
```
name = "Alice"

sizeof(name)     |
name.length()    |
name.size()      |


```
---

# Typedef and Using ( Aliases )
```
typedef unsigned long long ull;
ull user_id = 5005;

using ull = unsigned long long;
ull system_key = 0xABCDE;
```
---

#
