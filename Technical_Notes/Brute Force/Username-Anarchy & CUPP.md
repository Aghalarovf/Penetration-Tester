# 👤 Username Anarchy

### Quraşdırma

```bash
sudo apt install ruby -y
git clone https://github.com/urbanadventurer/username-anarchy.git
cd username-anarchy
chmod +x username-anarchy
```

### Ümumi Sintaksis

```bash
./username-anarchy [SEÇIMLƏR] "Ad Soyad"
./username-anarchy [SEÇIMLƏR] -i fayl.txt
```

---

### Əsas Parametrlər

| Parametr             | Açıqlama                                            |
|----------------------|-----------------------------------------------------|
| `"Ad Soyad"`         | Hədəfin adı birbaşa                                 |
| `-i FILE`            | Adlar siyahısı olan fayl                            |
| `-o FILE`            | Nəticəni fayla yaz                                  |
| `-l`                 | Hər şeyi kiçik hərflə yaz                           |
| `-u`                 | Plugin siyahısını göstər                            |
| `--plugin NAME`      | Yalnız müəyyən plugin(lər)i istifadə et             |
| `--plugin-help`      | Pluginlər haqqında məlumat                          |
| `-a`                 | Bütün formatları göstər (duplikatlarla birlikdə)    |
| `-F FORMAT`          | Xüsusi format tətbiq et                             |

---

### Tək Ad ilə İstifadə

```bash
# Əsas istifadə
./username-anarchy "Jane Smith"

# Nəticəni fayla yaz
./username-anarchy "Jane Smith" > jane_usernames.txt

# Kiçik hərflə
./username-anarchy -l "Jane Smith" > jane_usernames.txt
```

### Fayl ilə İstifadə (çoxlu ad)

```bash
# names.txt məzmunu:
# Jane Smith
# John Doe
# Thomas Edison

./username-anarchy -i names.txt > all_usernames.txt
```

### Xüsusi Format

```bash
# Format simvolları:
# F = Ad (First)    f = Ad kiçik hərf
# L = Soyad (Last)  l = Soyad kiçik hərf
# i = Adın ilk hərfi

./username-anarchy -F 'f.l' "Jane Smith"     # jane.smith
./username-anarchy -F 'f_l' "Jane Smith"     # jane_smith
./username-anarchy -F 'lf' "Jane Smith"      # smithjane
./username-anarchy -F 'fl' "Jane Smith"      # janesmith
./username-anarchy -F 'f.L' "Jane Smith"     # jane.Smith
```

## 🔑 CUPP

### Quraşdırma

```bash
# Paket meneceri ilə
sudo apt install cupp -y

# Mənbə koddan
git clone https://github.com/Mebus/cupp.git
cd cupp
python3 cupp.py --help
```

### Ümumi Sintaksis

```bash
cupp [SEÇIM]
python3 cupp.py [SEÇIM]
```

---

### Parametrlər

| Parametr | Açıqlama                                               |
|----------|--------------------------------------------------------|
| `-i`     | İnteraktiv rejim — sorğu-cavab ilə siyahı yarat        |
| `-w FILE`| Mövcud siyahını CUPP qaydaları ilə genişləndir         |
| `-l`     | Böyük siyahıları internetdən yüklə                     |
| `-a`     | Alecto verilənlər bazasından istifadəçi adı+şifrə al  |
| `-v`     | Versiya məlumatı                                       |

---

### İnteraktiv Rejim (`-i`)

```bash
cupp -i
```

#### CUPP-un Soruşduğu Məlumatlar

| Sahə                     | Nümunə Məlumat        |
|--------------------------|-----------------------|
| First Name               | `Jane`                |
| Surname                  | `Smith`               |
| Nickname                 | `Janey`               |
| Birthdate (DDMMYYYY)     | `11121990`            |
| Partner's name           | `Jim`                 |
| Partner's nickname       | `Jimbo`               |
| Partner's birthdate      | `12121990`            |
| Child's name             | *(boş burax)*         |
| Child's nickname         | *(boş burax)*         |
| Child's birthdate        | *(boş burax)*         |
| Pet's name               | `Spot`                |
| Company name             | `AHI`                 |
| Keywords                 | `hacker,blue,pizza`   |
| Special chars at end?    | `Y`                   |
| Random numbers at end?   | `Y`                   |
| Leet mode?               | `Y`                   |

---

### Onlayn Siyahıları Yükləmək (`-l`)

```bash
cupp -l
```

Yüklənən siyahılar:
- `500-worst-passwords.txt`
- `common-passwords-win.txt`
- `facebook-phished-passwords.txt`
- `john.txt`
- və digərləri

---

### Alecto Verilənlər Bazası (`-a`)

```bash
cupp -a
# Default istifadəçi adı + şifrə cütlərini çıxarır
# Məs: admin:admin, root:root, test:test
```

---

## 🔗 Birlikdə İstifadə

### Tam İş Axını

```bash
# 1. OSINT məlumatlarını topla (sosial media, LinkedIn, şirkət saytı)

# 2. Username Anarchy ilə istifadəçi adları yarat
cd username-anarchy
./username-anarchy "Jane Smith" > ~/jane_usernames.txt
echo "$(wc -l < ~/jane_usernames.txt) istifadəçi adı yaradıldı"

# 3. CUPP ilə şifrə siyahısı yarat
cupp -i
# Sualları cavablandır → jane.txt yaranır
echo "$(wc -l < jane.txt) şifrə yaradıldı"

# 4. Şirkətin şifrə siyasətinə uyğun filtrə et
grep -E '^.{8,}$' jane.txt \
  | grep -E '[A-Z]' \
  | grep -E '[a-z]' \
  | grep -E '[0-9]' \
  | grep -E '[!@#$%^&*]' \
  > jane_filtered.txt
echo "$(wc -l < jane_filtered.txt) şifrə filtrdən keçdi"

# 5. Hydra ilə hücum et
hydra -L ~/jane_usernames.txt -P jane_filtered.txt \
  TARGET_IP -s PORT -f \
  http-post-form "/:username=^USER^&password=^PASS^:Invalid"
```

---

### Fayl Strukturu

```
pentest/
├── recon/
│   └── jane_smith_osint.txt      # Toplanan məlumatlar
├── wordlists/
│   ├── jane_usernames.txt        # Username Anarchy çıxışı
│   ├── jane_passwords.txt        # CUPP çıxışı (ham)
│   └── jane_filtered.txt         # Filtrdən keçmiş şifrələr
└── results/
    └── hydra_result.txt          # Hücum nəticəsi
```

---

## 🔍 Grep ilə Filtrasiya

### Şifrə Siyasəti Nümunələri

#### Minimum 6 simvol, böyük+kiçik hərf, rəqəm, 2 xüsusi simvol

```bash
grep -E '^.{6,}$' jane.txt \
  | grep -E '[A-Z]' \
  | grep -E '[a-z]' \
  | grep -E '[0-9]' \
  | grep -E '([!@#$%^&*].*){2,}' \
  > jane_filtered.txt
```

#### Minimum 8 simvol, böyük+kiçik hərf, rəqəm, 1 xüsusi simvol

```bash
grep -E '^.{8,}$' jane.txt \
  | grep -E '[A-Z]' \
  | grep -E '[a-z]' \
  | grep -E '[0-9]' \
  | grep -E '[!@#$%^&*]' \
  > jane_filtered.txt
```

#### Yalnız rəqəm və hərfdən ibarət (xüsusi simvol yoxdur)

```bash
grep -E '^[a-zA-Z0-9]{6,}$' jane.txt > jane_simple.txt
```

#### Maksimum uzunluq məhdudiyyəti (6-12 simvol)

```bash
grep -E '^.{6,12}$' jane.txt > jane_length.txt
```

#### Boşluq olmayan şifrələr

```bash
grep -v ' ' jane.txt > jane_nospace.txt
```

### Nəticə Sayını Yoxlamaq

```bash
wc -l jane.txt           # Cəmi şifrə sayı
wc -l jane_filtered.txt  # Filtrdən keçən sayı

# Faiz hesabla
echo "$(wc -l < jane_filtered.txt) / $(wc -l < jane.txt) * 100" | bc
```

---

## ⚡ Hydra ilə İnteqrasiya

### HTTP POST Form

```bash
hydra -L jane_usernames.txt -P jane_filtered.txt \
  192.168.1.100 -s 8080 -f -v \
  http-post-form "/login:user=^USER^&pass=^PASS^:Incorrect"
```

### SSH

```bash
hydra -L jane_usernames.txt -P jane_filtered.txt \
  -t 4 -f ssh://192.168.1.100
```

### FTP

```bash
hydra -L jane_usernames.txt -P jane_filtered.txt \
  -f ftp://192.168.1.100
```

### Nəticəni Fayla Yaz

```bash
hydra -L jane_usernames.txt -P jane_filtered.txt \
  192.168.1.100 ssh -o results/hydra_output.txt
```
