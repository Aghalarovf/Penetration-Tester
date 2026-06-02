# Special Permissions
```powershell
find / -user root -perm -2000 -writable 2>/dev/null    SGID
find / -user root -perm -4000  2>/dev/null          SUID
find / -user root -perm -6000  2>/dev/null          SUID + SGID

find / -perm -4000 -type f 2>/dev/null | awk -F'/' '{print $NF}' | sort -u > suid_binaries.list
for i in $(curl -s https://gtfobins.org/api.json | jq -r '.executables | keys[]'); do 
    if grep -q "^$i$" suid_binaries.list; then 
        echo "[!!!] Dangerous SUID Binary -> This Method has in GTFOBin: $i";
    fi; 
done

find / -perm -6000 -type f 2>/dev/null | awk -F'/' '{print $NF}' | sort -u > critical_binaries.list
for i in $(curl -s https://gtfobins.org/api.json | jq -r '.executables | keys[]'); do 
    if grep -q "^$i$" critical_binaries.list; then 
        echo "[!!!] Dangerous SGID Binary -> This Method has in GTFOBin: $i";
    fi; 
done

bash.sh
#!/bin/bash
id
```
