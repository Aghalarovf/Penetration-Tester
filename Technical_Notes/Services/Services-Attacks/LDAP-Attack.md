# Enumeration
```powershell
ldapsearch -x -H ldap://<DC_IP> -b "DC=domain,DC=local"

ldapsearch -x -H ldap://<DC_IP> -D "user@domain.local" -w "password" -b "DC=domain,DC=local"

ldapsearch -x -H ldap://<DC_IP> -b "DC=domain,DC=local" "(objectClass=user)" sAMAccountName
```
