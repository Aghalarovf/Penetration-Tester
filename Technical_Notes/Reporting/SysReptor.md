# SysReptor Sıfırdan Quraşdırılma və HTB Şablonlarının Sazlanması

### 1. Addım: Repository-ni Klonlamaq və Qovluğa Keçid
```bash
git clone [https://github.com/SysLifters/sysreptor.git](https://github.com/SysLifters/sysreptor.git)
cd sysreptor
./install.sh

cd deploy

# app.env faylına xarici qoşulma dəyişənini əlavə edirik
echo "BIND_PORT=0.0.0.0:8000:8000" >> ../app.env

# docker-compose.yml faylında localhost məhdudiyyətini tamamilə silirik
sed -i 's/${BIND_PORT:-127.0.0.1:8000:8000}/"8000:8000"/g' docker-compose.yml

sudo ufw allow 8000/tcp

docker compose down
docker compose up -d

# Portun 0.0.0.0:8000 olaraq dinlənildiyini təsdiqləyin
sudo ss -tlnp | grep 8000

# Kali Linux-un İP ünvanını öyrənin (Windows-dan girmək üçün)
ip a

# HTB CPTS rəsmi hesabat dizaynını import edin
curl -s "[https://docs.sysreptor.com/assets/htb-designs.tar.gz](https://docs.sysreptor.com/assets/htb-designs.tar.gz)" | docker compose exec --no-TTY app python3 manage.py importdemodata --type=design

# Nümunə HTB CPTS maşın hesabatını import edin
curl -s "[https://docs.sysreptor.com/assets/htb-demo-projects.tar.gz](https://docs.sysreptor.com/assets/htb-demo-projects.tar.gz)" | docker compose exec --no-TTY app python3 manage.py importdemodata --type=project
```

# Start
```powershell
cd /home/sako/sysreptor/deploy
docker compose up -d
```

<img width="1457" height="833" alt="image" src="https://github.com/user-attachments/assets/eaba413f-d080-4f01-8952-ff7c2cd7d583" />
