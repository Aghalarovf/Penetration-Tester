# RPC Ports
```powershell
TCP 135   — RPC Endpoint Mapper (EPM)
TCP 139   — NetBIOS Session
TCP 445   — SMB (RPC over SMB)
TCP 49152-65535 — Dynamic RPC Ports
UDP 137   — NetBIOS Name Service
UDP 138   — NetBIOS Datagram
```

# RPC Implementation
```powershell
MS-SAMR     |     12345778-1234-abcd-ef00-0123456789ac    |     SAM veritabanı (users, groups)
MS-LSAD     |     12345778-1234-abcd-ef00-0123456789ab    |     LSA Policy
MS-DRSR     |     e3514235-4b06-11d1-ab04-00c04fc2dcd2    |     Directory Replication (DCSync)
MS-SRVS     |     4b324fc8-1670-01d3-1278-5a47bf6ee188    |     Server Service (shares)
MS-WKST     |     6bffd098-a112-3610-9833-46c3f87e345a    |     Workstation Service
MS-RRP      |     338cd001-2244-31f1-aaaa-900038001003    |     Remote Registry
MS-TSCH     |     86d35949-83c9-4044-b424-db363231fd0c    |     Task Scheduler
MS-SCMR     |     367abb81-9844-35f1-ad32-98f038001003    |     Service Control Manager
```

# Enumeration
```bash
nmap -sV -sC -p 135,139,445,49152-65535 $TARGET_IP \
  -oA $OUTPUT_DIR/nmap_full

nmap -p 135,445 --script msrpc-enum $TARGET_IP \
  -oA $OUTPUT_DIR/nmap_rpc

nmap -p 445 --script smb-enum-shares,smb-enum-users,smb-os-discovery,\
smb-security-mode,smb2-security-mode,smb-vuln-ms17-010 \
  $TARGET_IP -oA $OUTPUT_DIR/nmap_smb

sudo nmap -sU -p 137,138 $TARGET_IP -oA $OUTPUT_DIR/nmap_udp
```

# RPCDUMP
```bash
rpcdump.py $TARGET_IP | tee $OUTPUT_DIR/rpcdump/rpcdump_anon.txt

rpcdump.py -port 445 $TARGET_IP | tee $OUTPUT_DIR/rpcdump/rpcdump_smb.txt

rpcdump.py $TARGET_IP | grep "UUID\|Protocol" | tee $OUTPUT_DIR/rpcdump/uuids.txt

rpcdump.py -u "$USERNAME" -p "$PASSWORD" $DOMAIN/$USERNAME@$TARGET_IP \
  | tee $OUTPUT_DIR/rpcdump/rpcdump_auth.txt

rpcdump.py -hashes "$HASH" $DOMAIN/$USERNAME@$TARGET_IP \
  | tee $OUTPUT_DIR/rpcdump/rpcdump_pth.txt

# Bütün unique interfeyslər
rpcdump.py $TARGET_IP | grep "Interface" | sort -u \
  | tee $OUTPUT_DIR/rpcdump/interfaces.txt

# Named pipe-lar
rpcdump.py $TARGET_IP | grep "ncacn_np" \
  | tee $OUTPUT_DIR/rpcdump/named_pipes.txt

# Dinamik portlar
rpcdump.py $TARGET_IP | grep "ncacn_ip_tcp" \
  | tee $OUTPUT_DIR/rpcdump/dynamic_ports.txt

# MS-DRSR (DCSync mümkünlüyü) axtarışı
rpcdump.py $TARGET_IP | grep "e3514235-4b06-11d1-ab04-00c04fc2dcd2"
echo "[!] Yuxarıda nəticə varsa DCSync mümkündür"
```

# 
