# SUDO
```powershell
sudo cat /etc/sudoers | grep -v "#" | sed -r '/^\s*$/d'

sudo -V | head -n1 --> Search SUDO PoC Exploit

msf exploit(multi/handler) > use post/multi/recon/local_exploit_suggester
set AutoCheck false
set ForceExploit true
```
