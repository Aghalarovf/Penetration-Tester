# Privileged Groups
```powershell
Privileged Groups:
root
sudo
wheel
docker
lxd
shadow
disk
kmem
mem
port
adm
systemd-journal
tty
audio/video

grep -v -E '^#' /etc/group | grep -E ':.*(root|sudo|docker|lxd|disk|shadow)'
id
groups

cat /etc/group | grep istifadəçi_adı
getent group | grep istifadəçi_adı
```
