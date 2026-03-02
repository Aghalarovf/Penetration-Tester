# Bloodhound Setup

```
cd /opt
sudo mkdir bloodhound
cd bloodhound

sudo apt update
sudo apt install docker.io docker-compose -y

sudo usermod -aG docker $USER
newgrp docker

curl -L https://ghst.ly/getbhce > docker-compose.yml

docker-compose pull && docker-compose up -d

http://localhost:8080/ui/login

bloodhound-python -u USER -p PASS -d domain.local -ns DC_IP -c All
bloodhound-python -u USER -hashes LMHASH:NTHASH -d domain.local -ns DC_IP -c All
SharpHound.exe -c All
```
