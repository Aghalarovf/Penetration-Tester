# LDAP Pentesting Learning Roadmap

---

## LDAP Request
```
&      AND
!      NOT
*      Wildcard

( OPERATOR (IF1)(IF2) ... )

(&(objectClass=user)(givenname=A*))

LDAP ( userAccountControl )    |
LDAP ( groupType )             |   1.2.840.113556.1.4.803   
LDAP ( sAMAccountType )        |
```

## LDAPSEARCH

---

### RootDSE
```bash
ldapsearch -x -H ldap://DC-IP/ -s base
```

### Naming Contexts
```bash
ldapsearch -x -H ldap://DC-IP/ -s base namingcontexts
```

### SASL Mechanism
```bash
ldapsearch -x -H ldap://DC-IP/ -s base supportedsaslmechanisms
```

### Schema Information
```bash
ldapsearch -x -H ldap://192.168.0.239 -s base subschemaSubentry
```

### Objects
```bash
ldapsearch -x -H ldap://192.168.0.239 -D "Administrator@warzone.oxsium.local" -w "sako2005!" -b "DC=warzone,DC=oxsium,DC=local" "(objectClass=*)"
ldapsearch -x -H ldap://192.168.0.239 -D "Administrator@warzone.oxsium.local" -w "sako2005!" -b "DC=warzone,DC=oxsium,DC=local" "(objectClass=user)"
ldapsearch -x -H ldap://192.168.0.239 -D "Administrator@warzone.oxsium.local" -w 'sako2005!' -b "DC=warzone,DC=oxsium,DC=local" '(&(objectClass=user)(samaccountname=Administrator))'

ldapsearch -x -H ldap://192.168.0.239 -D "Administrator@warzone.oxsium.local" -w "sako2005!" -b "DC=warzone,DC=oxsium,DC=local" "(objectClass=group)"
ldapsearch -x -H ldap://192.168.0.239 -D "Administrator@warzone.oxsium.local" -w "sako2005!" -b "DC=warzone,DC=oxsium,DC=local" "(objectClass=computer)"
ldapsearch -x -H ldap://192.168.0.239 -D "Administrator@warzone.oxsium.local" -w "sako2005!" -b "DC=warzone,DC=oxsium,DC=local" "(objectClass=organizationalunit)"
ldapsearch -x -H ldap://192.168.0.239 -D "Administrator@warzone.oxsium.local" -w "sako2005!" -b "DC=warzone,DC=oxsium,DC=local" "(objectClass=grouppolicycontainer)"
```

### LDAP_MATCHING_RULE_BIT_AND
```bash
ldapsearch -x -H ldap://DC-IP -D "Administrator@warzone.oxsium.local" -w 'sako2005!'  -b "DC=warzone,DC=oxsium,DC=local" "(useraccountcontrol=:1.2.840.113556.1.4.803:=X)"

X = 2          | ACCOUNTDISABLE
X = 16         | LOCKOUT
X = 32         | PASSWD_NOT_REQD
X = 64         | PASSWD_CANT_CHANGE
X = 65536	    | DONT_EXPIRE_PASSWORD
X = 524288	   | TRUSTED_FOR_DELEGATION
X = 16777216   | TRUSTED_TO_AUTH_FOR_DELEGATION
```



