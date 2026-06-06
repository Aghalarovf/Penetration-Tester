# Reconnaissance
```powershell
# URL-ə bax → GitLab logo + login səhifəsi
http://<TARGET>/users/sign_in

# Versiya (yalnız giriş etdikdən sonra)
http://<TARGET>/help

sudo nmap -sV -p 80,443,8080,8081 <TARGET_IP>
# Yalnız HTTP serveri göstərir, GitLab-a spesifik deyil

curl -s http://<TARGET>/help | grep version
searchsploit gitlab

# Tipik subdomain adları:
gitlab.<domain>
git.<domain>
repo.<domain>
code.<domain>
dev.<domain>
```

# User Enumeration
```powershell
http://<TARGET>/users/sign_up

→ "Username is already taken" xətası → username mövcuddur
→ Boş sahə → username mövcud deyil

root, admin, administrator, git, gitlab, deploy

Qeydiyyat formasında mövcud email daxil et:
→ "Email has already been taken" → email mövcuddur
→ Bu texnika ən son GitLab versiyasında da işləyir
```

# Enumeration without Authentication
```powershell
http://<TARGET>/explore

Axtarılacaqlar:
- Public layihələr
- Commit tarixçəsi (tarix = versiya ipucu)
- Hardcoded credentials
- Config faylları
- SSH private key-lər
- API key-lər
- İnfrastruktur məlumatları

# Autentifikasiyasız axtarış (əgər icazə verilibsə):
http://<TARGET>/search?search=password
http://<TARGET>/search?search=secret
http://<TARGET>/search?search=api_key
http://<TARGET>/search?search=private_key
```

# Credential Hunting
```powershell
# gitleaks ilə repo tara
gitleaks detect --source=<REPO_PATH>

# truffleHog ilə
trufflehog git http://<TARGET>/<user>/<repo>
```
