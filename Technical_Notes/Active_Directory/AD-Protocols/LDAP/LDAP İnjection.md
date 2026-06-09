# LDAP Injection
```powershell
(&(objectClass=user)(sAMAccountName=$username)(userPassword=$password))

(&(objectClass=user)(sAMAccountName=$username = "*")(userPassword=$password))

(&(objectClass=user)(sAMAccountName=$username)(userPassword=$password = "*"))

```
