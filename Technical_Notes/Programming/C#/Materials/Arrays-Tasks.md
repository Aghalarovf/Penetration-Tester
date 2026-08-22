# C# Collections — Red Team Practice Tasks
> **Topics:** `Array` · `List<T>` · `Dictionary<TKey,TValue>` · `HashSet<T>` · `Queue<T>` · `Stack<T>`  
> **Theme:** Red Team tooling — port scanning, credential checking, C2 simulation, recon

---

## 🟢 Easy (Tasks 1–3)

---

### Task 1 — Port-Service Mapper
**Collections:** `int[]` · `Dictionary<int, string>`

Store these ports in a fixed `int[]` array: `21, 22, 80, 443, 3306, 3389`.  
Create a `Dictionary<int, string>` mapping each port to its service name.  
Loop through the array and print each port with its service.  
If a port is not in the dictionary, print `Unknown Service`.

**Expected Output:**
```
[+] 21   → FTP
[+] 22   → SSH
[+] 80   → HTTP
[+] 443  → HTTPS
[+] 3306 → MySQL
[+] 3389 → RDP
[-] 9999 → Unknown Service
```

---

### Task 2 — Duplicate IP Remover
**Collections:** `string[]` · `List<string>` · `HashSet<string>`

You have a raw IP list with duplicates (simulating multiple scan tools):
```
"10.0.0.1", "10.0.0.5", "10.0.0.12",
"10.0.0.1", "10.0.0.5", "10.0.0.20"
```
- Add all to a `List<string>`
- Deduplicate using `HashSet<string>`
- Sort and print unique IPs
- Show how many duplicates were removed

**Expected Output:**
```
[*] Raw count      : 6
[*] Unique count   : 4
[*] Duplicates     : 2
[*] Unique hosts   :
    10.0.0.1
    10.0.0.5
    10.0.0.12
    10.0.0.20
```

---

### Task 3 — Weak Credential Detector
**Collections:** `Dictionary<string, string>`

Store 5 username/password pairs in a `Dictionary<string, string>`.  
Check each password:
- Length `< 8` → **WEAK**
- Otherwise → **OK**

Print each user with their status. At the end print total weak count.

**Expected Output:**
```
[!] admin    : pass      → WEAK
[!] root     : toor      → WEAK
[!] operator : Secure99! → OK
[*] Weak credentials: 2/5
```

---

## 🟡 Intermediate (Tasks 4–6)

---

### Task 4 — C2 Task Queue
**Collections:** `Queue<string>` · `Stack<string>`

Enqueue these commands: `"whoami"`, `"ipconfig"`, `"net user"`, `"netstat"`.  
Dequeue and "execute" each one (just print it).  
Push each executed command to a `Stack<string>` as history.  
After all commands run, print history from Stack (newest → oldest).

**Expected Output:**
```
[>] Executing: whoami
[>] Executing: ipconfig
[>] Executing: net user
[>] Executing: netstat

[*] Execution history (latest first):
[H] netstat
[H] net user
[H] ipconfig
[H] whoami
```

---

### Task 5 — Subnet Host Classifier
**Collections:** `string[]` · `List<string>` · `Dictionary<string, int>`

Given 8 IPs, split each with `.Split('.')` and check the last octet:
- Even → `liveHosts` List
- Odd  → `deadHosts` List

Also count how many IPs belong to each `/24` subnet using  
`Dictionary<string, int>` (key = first 3 octets, e.g. `"10.0.0"`).

**Expected Output:**
```
[+] LIVE : 10.0.0.12, 10.0.0.44, 10.0.1.20
[-] DEAD : 10.0.0.1, 10.0.0.31

[*] Subnet groups:
    10.0.0.x → 6 hosts
    10.0.1.x → 2 hosts

[*] Alive rate: 37.5%
```

---

### Task 6 — Recon Loot Collector
**Collections:** `Dictionary<string, List<string>>`

Create a `Dictionary<string, List<string>>` where:
- Key   = IP address
- Value = list of loot items found on that host

Manually populate loot for 4 hosts.  
Then print all loot per host, find the host with the most loot,  
and count total loot items across all hosts.

**Expected Output:**
```
[*] 10.0.0.1 → 3 items:
    - admin:Password123
    - SSH private key
    - /etc/shadow

[*] 10.0.0.5 → 2 items:
    - RDP credentials
    - browser cookies

[*] Richest target : 10.0.0.1 (3 items)
[*] Total loot     : 8 items
```

---

## 🔴 Hard (Tasks 7–10)

---

### Task 7 — Brute Force Simulator
**Collections:** `Queue<string>` · `HashSet<string>` · `List<string>` · `Dictionary<string, int>`

Define a password wordlist in a `Queue<string>` (8 passwords).  
Target user: `"admin"`. Correct password: `"Password123!"`.

For each attempt:
- Skip if already tried (use `HashSet<string>`)
- Track attempt count in `Dictionary<string, int>`
- Add failed attempts to `List<string>`
- Stop when correct password is found

**Expected Output:**
```
[-] Trying: admin123   → FAIL
[-] Trying: 123456     → FAIL
[+] Trying: Password123! → SUCCESS (attempt #4)

[*] Total attempts  : 4
[*] Failed attempts : 3
[*] Unique tried    : 4
```

---

### Task 8 — C2 Agent with Command Results
**Collections:** `Queue<string>` · `Stack<string>` · `Dictionary<string, string>`

Build a `Dictionary<string, string>` mapping commands to fake output:
```
"whoami"   → "NT AUTHORITY\SYSTEM"
"ipconfig" → "10.0.0.5"
"net user" → "admin, guest"
"netstat"  → "TCP 0.0.0.0:445 LISTENING"
```

Enqueue all 4 commands. Dequeue each, look up its result from Dictionary,  
print `command → result`, and push to history Stack.

Then ask user for `int n` — re-execute last `n` commands from Stack.

**Expected Output:**
```
[>] whoami   → NT AUTHORITY\SYSTEM
[>] ipconfig → 10.0.0.5
[>] net user → admin, guest
[>] netstat  → TCP 0.0.0.0:445 LISTENING

Re-execute how many? 2

[!] Re-running: netstat  → TCP 0.0.0.0:445 LISTENING
[!] Re-running: net user → admin, guest

[*] Total executed   : 4
[*] Re-executed      : 2
```

---

### Task 9 — Port Scan with Open Port Tracker
**Collections:** `string[]` · `List<int>` · `HashSet<int>` · `Dictionary<string, List<int>>`

Given 5 target IPs, simulate a port scan for each.  
For each IP, randomly select 3 ports from `commonPorts` array as "open".  
- Store open ports per host in `Dictionary<string, List<int>>`
- Track all unique open ports across all hosts in `HashSet<int>`

At the end:
- Print each host with its open ports and service names
- Print total unique open ports found across all hosts

**Expected Output:**
```
[+] 10.0.0.1 → 22 (SSH), 80 (HTTP), 3389 (RDP)
[+] 10.0.0.5 → 22 (SSH), 443 (HTTPS), 3306 (MySQL)
[+] 10.0.0.12 → 80 (HTTP), 3306 (MySQL), 5432 (PostgreSQL)

[*] Unique open ports across all hosts: 6
    22, 80, 443, 3306, 3389, 5432
```

---

### Task 10 — Mini Recon Framework
**Collections:** ALL — `Array` · `List<T>` · `Dictionary<K,V>` · `HashSet<T>` · `Queue<T>` · `Stack<T>`

Build a small recon tool that runs a full pipeline:

1. Load 6 IPs into a `Queue<string>`
2. Dequeue each IP:
   - Skip if already scanned (`HashSet<string>`)
   - Add to `liveHosts` List
   - Assign 2 random open ports from `int[]` commonPorts
   - Store ports in `Dictionary<string, List<int>>`
   - Push scan log to `Stack<string>`
3. Print final report with audit log (newest first from Stack)

**Expected Output:**
```
[>] Scanning: 10.0.0.1 → Ports: 22, 80
[>] Scanning: 10.0.0.5 → Ports: 443, 3389
[!] 10.0.0.1 already scanned — skip
[>] Scanning: 10.0.0.12 → Ports: 22, 3306
...

[*] ===== FINAL REPORT =====
[*] Hosts scanned    : 5
[*] Skipped (dedup)  : 1
[*] Total open ports : 10

[*] Audit log (latest first):
[LOG] 10.0.0.12 → Port 3306 open
[LOG] 10.0.0.5  → Port 3389 open
[LOG] 10.0.0.1  → Port 22 open
```

---

## 📊 Summary Table

| #  | Difficulty | Collections Used                              | Tool Type              |
|----|------------|-----------------------------------------------|------------------------|
| 1  | 🟢 Easy    | `Array` + `Dictionary`                        | Port-service mapper    |
| 2  | 🟢 Easy    | `List` + `HashSet`                            | IP deduplicator        |
| 3  | 🟢 Easy    | `Dictionary`                                  | Credential checker     |
| 4  | 🟡 Medium  | `Queue` + `Stack`                             | C2 task runner         |
| 5  | 🟡 Medium  | `Array` + `List` + `Dictionary`               | Subnet classifier      |
| 6  | 🟡 Medium  | `Dictionary<string, List<string>>`            | Loot collector         |
| 7  | 🔴 Hard    | `Queue` + `HashSet` + `List` + `Dictionary`   | Brute force simulator  |
| 8  | 🔴 Hard    | `Queue` + `Stack` + `Dictionary`              | C2 agent               |
| 9  | 🔴 Hard    | `Array` + `List` + `HashSet` + `Dictionary`   | Port scan tracker      |
| 10 | 🔴 Hard    | **All 6**                                     | Mini recon framework   |

---

> 💡 Go in order — Task 10 uses everything from Tasks 1–9.
