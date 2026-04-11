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

# Data Types
---
![C-data-types](https://github.com/user-attachments/assets/ae8201f6-63d8-49f1-9ba8-162cb789dc77)

```
LONG      |   8 Byte
DOUBLE    |   8 Byte 
INT       |   4 Byte
FLOAT     |   4 Byte
CHAR      |   1 Byte
BOOL      |   1 Byte
STRING    |   Dynamic
VOID      |   NULL
``` 
---

# Operators
```

```
---
