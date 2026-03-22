# Snaffler
```
Snaffler.exe -d <domain_adı> -o snaffler_output.txt

Snaffler.exe -d <domain> -i C:\Snaffler\Rules -s

Snaffler.exe -n <DC_IP_və_ya_Adı> -o dc_scan.log
```

# PowerHuntShares
```
Import-Module .\PowerHuntShares.psm1
New-Item -Path "C:\Tools\HuntResults" -ItemType Directory -Force

Invoke-HuntSMBShares -Threads 20 -OutputDirectory "C:\Tools\HuntResults"
```
