# Bloodhound Setup

```
cd /opt
sudo mkdir bloodhound
cd bloodhound

sudo apt update
sudo apt install docker.io docker-compose -y

sudo usermod -aG docker $USER
newgrp docker

curl -L https://ghst.ly/getbhce > docker-compose.yml

docker-compose pull && docker-compose up -d

http://localhost:8080/ui/login

bloodhound-python -u USER -p PASS -d domain.local -ns DC_IP -c All
bloodhound-python -u USER -hashes LMHASH:NTHASH -d domain.local -ns DC_IP -c All
SharpHound.exe -c All
```

# BloodHound Cheat Sheet

> BloodHound uses graph theory to reveal hidden relationships in Active Directory.
> Ingests data collected by **SharpHound** (C#) or **BloodHound.py** (Python).

---

## Data Collection

### RustHound-CE
```powershell
rusthound-ce --domain fluffy.htb -u user -p password
```

### BloodyAD
```powershell
python3 bloodyAD -u user -p password -d domain.local --host 10.10.10.10 add groupMember 'service accounts' user
```

### SharpHound (Windows)
```powershell
# Collect everything
.\SharpHound.exe -c All

# All + GPO local group mapping
.\SharpHound.exe -c All,GPOLocalGroup

# Stealth mode (slower, less noisy)
.\SharpHound.exe -c All --Stealth

# Target a specific domain
.\SharpHound.exe -c All --Domain target.local

# Specify a DC
.\SharpHound.exe -c All --DomainController dc01.domain.local

# Custom output path and prefix
.\SharpHound.exe -c All --OutputDirectory C:\temp\ --OutputPrefix "lab"

# Loop collection (repeat every N minutes)
.\SharpHound.exe -c All --Loop --LoopDuration 02:00:00 --LoopInterval 00:05:00

# Use alternate credentials
.\SharpHound.exe -c All --LdapUsername user --LdapPassword pass
```

### BloodHound.py (Linux / remote)
```bash
# Install
pip install bloodhound

# Collect all data remotely
bloodhound-python -u user -p 'password' -d domain.local -ns 192.168.1.1 -c All

# With NTLM hash
bloodhound-python -u user --hashes :ntlmhash -d domain.local -ns 192.168.1.1 -c All

# Kerberos auth (needs ccache)
export KRB5CCNAME=user.ccache
bloodhound-python -u user -k --no-pass -d domain.local -ns 192.168.1.1 -c All

# Collect only sessions (stealthy)
bloodhound-python -u user -p 'password' -d domain.local -ns 192.168.1.1 -c Session

# Output to specific directory
bloodhound-python -u user -p 'password' -d domain.local -ns 192.168.1.1 -c All --zip -o /tmp/bh/
```

### Collection Methods
| Method | What it collects |
|--------|-----------------|
| `Default` | Sessions, LocalAdmins, Trusts |
| `All` | Everything below |
| `DCOnly` | Users, Groups, Computers, Trusts, ACLs (no agent needed) |
| `Session` | Logged-on users per machine |
| `LocalAdmin` | Local admin memberships |
| `DCOM` | DCOM rights |
| `RDP` | RDP access rights |
| `PSRemote` | WinRM access rights |
| `Trusts` | Domain/forest trusts |
| `ACL` | Access Control Lists |
| `ObjectProps` | Object property details |
| `Container` | OU/container structure |
| `GPOLocalGroup` | GPO-applied local group memberships |

---

## Starting BloodHound

```bash
# Start Neo4j first
sudo neo4j start
# or
sudo neo4j console

# Launch BloodHound GUI
bloodhound &

# Default Neo4j credentials (change on first login)
# URL:  http://localhost:7474
# User: neo4j
# Pass: neo4j
```

### Upload Data
1. Open BloodHound GUI
2. Click **Upload Data** (top right)
3. Select the `.zip` or individual `.json` files from SharpHound output
4. Wait for ingestion to complete

---

## Pre-Built Queries (GUI)

Access via **Analysis** tab → select a query.

### Domain Info
```
Find all Domain Admins
Find Shortest Paths to Domain Admins
Find Principals with DCSync Rights
List all Kerberoastable Accounts
Find AS-REP Roastable Users
Find all unconstrained delegation systems
```

### Attack Paths
```
Shortest Paths to Domain Admins
Shortest Paths to High Value Targets
Shortest Paths from Kerberoastable Users
Shortest Paths from AS-REP Roastable Users
Shortest Paths to Domain Admins from Owned Principals
```

### Dangerous Permissions
```
Find Principals with DCSync Rights
Find all Paths from Domain Users to High Value Targets
Transitive Object Control
```

---

## Cypher Queries (Raw)

Run in the **Raw Query** bar at the bottom of BloodHound.

### Domain & Environment
```cypher
// All domains
MATCH (d:Domain) RETURN d.name

// All DCs
MATCH (c:Computer {unconstraineddelegation:true})-[:MemberOf*1..]->(g:Group) 
WHERE g.name =~ "(?i)domain controllers.*" 
RETURN c.name

// Count all objects
MATCH (n) RETURN labels(n), count(n)
```

### High Value Targets
```cypher
// Find all Domain Admins
MATCH (u:User)-[:MemberOf*1..]->(g:Group {name:"DOMAIN ADMINS@DOMAIN.LOCAL"}) 
RETURN u.name

// Find all members of high-value groups
MATCH (u)-[:MemberOf*1..]->(g:Group) 
WHERE g.highvalue = true 
RETURN u.name, g.name

// Find all high-value targets
MATCH (n {highvalue:true}) RETURN n.name, labels(n)
```

### Kerberos Attacks
```cypher
// Kerberoastable accounts
MATCH (u:User {hasspn:true}) 
RETURN u.name, u.serviceprincipalnames

// Kerberoastable accounts with DA path
MATCH (u:User {hasspn:true}), (g:Group {name:"DOMAIN ADMINS@DOMAIN.LOCAL"}),
p=shortestPath((u)-[*1..]->(g)) 
RETURN p

// AS-REP roastable users
MATCH (u:User {dontreqpreauth:true}) 
RETURN u.name

// AS-REP roastable with path to DA
MATCH (u:User {dontreqpreauth:true}), (g:Group {name:"DOMAIN ADMINS@DOMAIN.LOCAL"}),
p=shortestPath((u)-[*1..]->(g)) 
RETURN p
```

### Delegation
```cypher
// Unconstrained delegation (computers)
MATCH (c:Computer {unconstraineddelegation:true}) 
RETURN c.name, c.operatingsystem

// Unconstrained delegation (users)
MATCH (u:User {unconstraineddelegation:true}) 
RETURN u.name

// Constrained delegation
MATCH (c:Computer) 
WHERE c.allowedtodelegate IS NOT NULL 
RETURN c.name, c.allowedtodelegate

// RBCD — resource-based constrained delegation
MATCH p=(u)-[:AllowedToAct]->(c:Computer) 
RETURN u.name, c.name
```

### ACL / Permission Abuse
```cypher
// DCSync rights
MATCH p=(u)-[:DCSync|GetChanges|GetChangesAll]->(d:Domain) 
RETURN u.name, d.name

// GenericAll on any object
MATCH p=(u)-[:GenericAll]->(n) 
RETURN u.name, n.name, labels(n)

// GenericWrite on users
MATCH p=(u)-[:GenericWrite]->(v:User) 
RETURN u.name, v.name

// WriteOwner on high value targets
MATCH p=(u)-[:WriteOwner]->(n {highvalue:true}) 
RETURN u.name, n.name

// ForceChangePassword
MATCH p=(u)-[:ForceChangePassword]->(v:User) 
RETURN u.name, v.name

// AddMember to groups
MATCH p=(u)-[:AddMember]->(g:Group) 
RETURN u.name, g.name

// WriteDACL on domain
MATCH p=(u)-[:WriteDacl]->(d:Domain) 
RETURN u.name

// All ACL paths to DA
MATCH p=shortestPath((u:User)-[:GenericAll|GenericWrite|WriteOwner|WriteDacl|AddMember*1..]->(g:Group {name:"DOMAIN ADMINS@DOMAIN.LOCAL"})) 
RETURN p
```

### Session & Access
```cypher
// Where are DAs logged in?
MATCH (u:User)-[:MemberOf*1..]->(g:Group {name:"DOMAIN ADMINS@DOMAIN.LOCAL"}),
(u)-[:HasSession]->(c:Computer) 
RETURN u.name, c.name

// All local admin rights for a user
MATCH (u:User {name:"JDOE@DOMAIN.LOCAL"})-[:AdminTo]->(c:Computer) 
RETURN c.name

// All users with local admin somewhere
MATCH (u:User)-[:AdminTo]->(c:Computer) 
RETURN u.name, count(c) ORDER BY count(c) DESC

// Computers with most local admins
MATCH (u)-[:AdminTo]->(c:Computer) 
RETURN c.name, count(u) ORDER BY count(u) DESC LIMIT 10
```

### Shortest Paths
```cypher
// Shortest path from owned user to DA
MATCH (u:User {name:"JDOE@DOMAIN.LOCAL"}), (g:Group {name:"DOMAIN ADMINS@DOMAIN.LOCAL"}),
p=shortestPath((u)-[*1..]->(g)) 
RETURN p

// All shortest paths to DA
MATCH (u:User), (g:Group {name:"DOMAIN ADMINS@DOMAIN.LOCAL"}),
p=shortestPath((u)-[*1..]->(g)) 
RETURN p

// Shortest path between two specific nodes
MATCH (a {name:"JDOE@DOMAIN.LOCAL"}), (b {name:"DC01.DOMAIN.LOCAL"}),
p=shortestPath((a)-[*1..]->(b)) 
RETURN p

// Paths through owned nodes
MATCH (o {owned:true}), (g:Group {name:"DOMAIN ADMINS@DOMAIN.LOCAL"}),
p=shortestPath((o)-[*1..]->(g)) 
RETURN p
```

### Trusts
```cypher
// All domain trusts
MATCH p=(d:Domain)-[:TrustedBy|Trusts]->(n) RETURN p

// Bidirectional trusts
MATCH p=(d:Domain)-[:TrustedBy]->(n:Domain)-[:TrustedBy]->(d) RETURN p

// Paths across trust boundaries
MATCH p=shortestPath((u:User)-[*1..]->(d:Domain {name:"TARGET.LOCAL"})) 
WHERE u.domain <> "TARGET.LOCAL" 
RETURN p
```

### LAPS
```cypher
// Computers with LAPS enabled
MATCH (c:Computer {haslaps:true}) RETURN c.name

// Who can read LAPS passwords?
MATCH p=(u)-[:ReadLAPSPassword]->(c:Computer) 
RETURN u.name, c.name

// Shortest path using LAPS read
MATCH (u)-[:ReadLAPSPassword]->(c:Computer),
(g:Group {name:"DOMAIN ADMINS@DOMAIN.LOCAL"}),
p=shortestPath((c)-[*1..]->(g)) 
RETURN p
```

### GPO
```cypher
// GPOs with interesting permissions
MATCH p=(u)-[:GenericAll|GenericWrite|Owns|WriteDacl|WriteOwner]->(g:GPO) 
RETURN u.name, g.name

// All GPO links
MATCH p=(g:GPO)-[:GPLink]->(o) 
RETURN g.name, o.name, labels(o)
```

---

## Mark Nodes as Owned / High Value

### In GUI
- Right-click a node → **Mark as Owned** (adds skull icon)
- Right-click a node → **Mark as High Value** (adds target icon)

### Via Cypher
```cypher
// Mark a user as owned
MATCH (u:User {name:"JDOE@DOMAIN.LOCAL"}) SET u.owned = true

// Mark multiple users as owned
MATCH (u:User) WHERE u.name IN ["USER1@DOMAIN.LOCAL","USER2@DOMAIN.LOCAL"] 
SET u.owned = true

// Mark a computer as owned
MATCH (c:Computer {name:"WS01.DOMAIN.LOCAL"}) SET c.owned = true

// Mark a group as high value
MATCH (g:Group {name:"IT ADMINS@DOMAIN.LOCAL"}) SET g.highvalue = true

// Clear owned flag
MATCH (n {owned:true}) SET n.owned = false
```

---

## Database Management

```cypher
// Count nodes by type
MATCH (n) RETURN labels(n), count(n) ORDER BY count(n) DESC

// Count relationships by type
MATCH ()-[r]->() RETURN type(r), count(r) ORDER BY count(r) DESC

// Delete all data (wipe the database)
MATCH (n) DETACH DELETE n

// Delete specific domain data
MATCH (n {domain:"DOMAIN.LOCAL"}) DETACH DELETE n

// Find nodes with no relationships (orphans)
MATCH (n) WHERE NOT (n)-[]-() RETURN n.name, labels(n)
```

---

## BloodHound CE (Community Edition)

```bash
# Run with Docker
curl -L https://ghst.ly/getbhce | docker compose -f - up

# Default login
# URL:  http://localhost:8080
# User: admin
# Pass: (shown in docker output on first run)

# Upload via CLI (bhce)
bhce upload -u admin -p 'password' --url http://localhost:8080 *.json
```

---

## Tips & OPSEC

| Tip | Detail |
|-----|--------|
| Use `DCOnly` first | Queries only the DC — no lateral movement to machines |
| Avoid `Session` collection in prod | Requires connecting to every computer |
| Use `--Stealth` | Skips systems likely to trigger AV/EDR |
| Use `--Loop` off-hours | Captures more session data with less noise |
| Combine with owned nodes | Mark compromised accounts to find next-hop paths |
| Filter by `enabled=true` | Removes disabled accounts from attack paths |
| Use `shortestPath` not `allShortestPaths` | Faster for large environments |

---

## Useful Edge Types (Relationships)

| Edge | Meaning |
|------|---------|
| `MemberOf` | User/Computer is in a group |
| `AdminTo` | Has local admin rights |
| `HasSession` | User has active session on computer |
| `GenericAll` | Full control over object |
| `GenericWrite` | Can write arbitrary attributes |
| `WriteOwner` | Can change object owner |
| `WriteDacl` | Can modify object ACL |
| `ForceChangePassword` | Can reset password |
| `AddMember` | Can add members to a group |
| `DCSync` | Can replicate domain secrets |
| `AllowedToDelegate` | Constrained delegation |
| `AllowedToAct` | Resource-based constrained delegation |
| `ReadLAPSPassword` | Can read LAPS local admin password |
| `GPLink` | GPO is linked to OU/Site |
| `TrustedBy` | Domain trust direction |
| `Owns` | Owns the object |
| `GetChanges` / `GetChangesAll` | DCSync-related extended rights |
