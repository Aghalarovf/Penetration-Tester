# 🕷️ Web Pentesting & Bug Bounty Roadmap: From $200/month to $10K–$20K/month
### 300 Steps — The Complete Offensive Web Security Blueprint

---

> **How to use this roadmap:** Follow the phases in order. Each phase builds on the last. Don't skip ahead. The difference between someone who earns $200/month and someone who earns $10K+/month is almost never talent — it's depth, methodology, and consistency.

---

## PHASE 1 — FOUNDATIONS (Steps 1–40)
### *"You can't break what you don't understand"*

---

### 🖥️ Networking & Protocol Basics

1. Understand the OSI model — know what happens at each layer during an HTTP request
2. Learn how TCP/IP works: handshakes, packets, ports, and sockets
3. Study DNS in depth: A, CNAME, MX, TXT, NS records; zone transfers; DNS over HTTPS
4. Understand how HTTP/1.1, HTTP/2, and HTTP/3 differ and why it matters for recon
5. Learn HTTPS: TLS handshake, certificate chains, SNI, HSTS, certificate transparency logs
6. Study IPv4 vs IPv6 addressing, subnetting, CIDR notation
7. Learn how CDNs work (Cloudflare, Akamai, Fastly) and how to identify/bypass them
8. Understand reverse proxies, load balancers, and WAFs at a conceptual level
9. Learn about WebSockets: upgrade headers, frames, masking, and ws:// vs wss://
10. Study CORS: preflight requests, `Access-Control-Allow-*` headers, wildcard origins

---

### 🌐 Web Application Architecture

11. Learn how cookies work: `Secure`, `HttpOnly`, `SameSite`, `Domain`, `Path`, `Expires`
12. Understand sessions: server-side sessions, JWT, opaque tokens, refresh tokens
13. Study how browsers enforce the Same-Origin Policy (SOP)
14. Learn about iframes, `X-Frame-Options`, and Content Security Policy (CSP)
15. Understand authentication flows: form-based, OAuth 2.0, OpenID Connect, SAML
16. Study API architectures: REST, GraphQL, gRPC, SOAP — their structures and attack surfaces
17. Learn about web caches: browser cache, CDN cache, reverse proxy cache, cache poisoning concepts
18. Understand the difference between stateless and stateful applications
19. Study microservices vs monoliths from an attacker's perspective
20. Learn about cloud infrastructure: AWS, GCP, Azure metadata services and exposed endpoints

---

### 🛠️ Environment Setup

21. Set up Kali Linux or Parrot OS in a VM (VirtualBox/VMware) or use WSL2 on Windows
22. Install and configure Burp Suite Community Edition — learn every panel
23. Set up FoxyProxy in Firefox and configure Burp as your proxy
24. Install the Burp CA certificate in your browser to intercept HTTPS
25. Learn Burp Suite's core tabs: Proxy, Repeater, Intruder, Decoder, Comparer, Scanner
26. Install and learn `curl` — practice crafting raw HTTP requests from the terminal
27. Set up a local vulnerable lab: install DVWA (Damn Vulnerable Web Application)
28. Install OWASP WebGoat for guided vulnerability learning
29. Set up PortSwigger Web Security Academy account (free, best resource on earth)
30. Install essential CLI tools: `nmap`, `ffuf`, `nuclei`, `httpx`, `subfinder`, `amass`

---

### 📚 Core Vulnerability Concepts

31. Read the OWASP Top 10 (2021 version) — understand each category deeply
32. Read the OWASP Testing Guide (OTG) cover to cover — bookmark it
33. Read the PortSwigger Web Security Academy learning paths for all major vuln classes
34. Understand the CIA Triad (Confidentiality, Integrity, Availability) in web context
35. Learn the difference between vulnerabilities, weaknesses, and misconfigurations
36. Study CVSS scoring — understand how severity is calculated
37. Learn what CVEs, CWEs, and CAPECs are and how to reference them
38. Read HackerOne's Hacktivity feed — filter by disclosed reports and read 50+ of them
39. Read Bugcrowd's vulnerability taxonomy document
40. Study at least 20 public bug bounty write-ups before touching a real target

---

## PHASE 2 — RECONNAISSANCE MASTERY (Steps 41–90)
### *"Recon is where money is made or lost"*

---

### 🔍 Passive Reconnaissance

41. Learn OSINT fundamentals: what's passive vs active recon and why it matters legally
42. Use `whois` and `RDAP` to enumerate domain registrations and ownership history
43. Use Certificate Transparency logs (crt.sh, censys.io) to find subdomains
44. Learn Google Dorking: `site:`, `inurl:`, `intitle:`, `filetype:`, `ext:`, `-www` operators
45. Use Shodan to find exposed infrastructure: search by ASN, org name, SSL cert
46. Use Censys.io for certificate and host enumeration
47. Learn Wayback Machine (web.archive.org) and `waybackurls` to find old endpoints
48. Study `gau` (GetAllURLs) to pull URLs from multiple passive sources
49. Learn `theHarvester` for email and domain harvesting
50. Use LinkedIn, GitHub, Glassdoor, and job postings to fingerprint tech stack
51. Search GitHub for leaked credentials, API keys, config files using `git-secrets` and `truffleHog`
52. Learn `github-search` operators: `org:`, `filename:`, `extension:`, `path:`
53. Study Pastebin, Gist, and similar paste sites for leaked data
54. Use BuiltWith and Wappalyzer to identify technologies used by targets
55. Learn to read DNS records manually and interpret SPF, DKIM, DMARC for misconfigs
56. Study BGP routing and ASN enumeration with `bgp.he.net` and RIPE NCC

---

### 🕵️ Active Reconnaissance

57. Learn `subfinder` and `amass` for automated subdomain enumeration
58. Learn `dnsx` for DNS resolution and brute-forcing
59. Use `httpx` to probe live hosts, grab titles, status codes, and tech headers
60. Learn `nmap`: service detection (`-sV`), OS detection (`-O`), script engine (`-sC`, `--script`)
61. Study common nmap NSE scripts relevant to web pentesting
62. Use `masscan` for high-speed port scanning on large scopes
63. Learn subdomain brute-forcing with `ffuf` and wordlists from SecLists
64. Practice virtual host discovery (`ffuf -H "Host: FUZZ.target.com"`)
65. Learn `gobuster` and `feroxbuster` for directory and file brute-forcing
66. Understand recursive fuzzing — go 3–4 levels deep on interesting paths
67. Study JavaScript file analysis: use `LinkFinder`, `JSParser`, `SecretFinder` on `.js` files
68. Learn to manually read JavaScript for hidden endpoints, keys, and logic flaws
69. Use `gf` (grep-fu) with patterns to filter URLs for potential vulnerabilities
70. Learn `hakrawler` and `katana` for automated crawling of web applications
71. Study `aquatone` and `gowitness` for visual recon (screenshots of all subdomains)
72. Learn to use `nuclei` with community templates for automated initial recon scans
73. Build a personal recon automation pipeline using bash or Python
74. Learn to use `notify` to get alerts when new subdomains or changes are detected
75. Understand continuous monitoring: set up recon to run on a schedule (cron jobs)

---

### 📋 Scope & Target Selection

76. Learn to read bug bounty program scopes on HackerOne, Bugcrowd, Intigriti, YesWeHack
77. Understand in-scope vs out-of-scope rules — violating scope can get you banned
78. Learn to identify high-value targets within a scope (admin panels, APIs, auth endpoints)
79. Prioritize targets with large attack surfaces over niche, locked-down ones
80. Study how to find "forgotten" assets: old subdomains, staging environments, dev servers
81. Learn about acquisitions: companies that recently acquired other companies often inherit vulnerable assets
82. Study how to use Crunchbase and LinkedIn to find acquisitions and subsidiaries
83. Learn to identify out-of-scope assets that border in-scope ones (useful for pivot logic)
84. Read 10+ bug bounty program policies and learn to spot permissive vs restrictive programs
85. Understand VDPs (Vulnerability Disclosure Programs) vs paid BBPs — when to choose each
86. Learn the concept of "low-hanging fruit" programs vs "high signal" programs
87. Study how new programs (just launched) often have more bugs waiting to be found
88. Learn to track programs you've already tested to revisit after updates
89. Understand asset expansion: how new acquisitions or product launches create new attack surface
90. Build a spreadsheet to track all programs, their scope, payouts, and your submissions

---

## PHASE 3 — CORE VULNERABILITIES (Steps 91–175)
### *"Know your weapons cold"*

---

### 💉 Injection Vulnerabilities

91. **SQL Injection (SQLi)** — understand error-based, blind boolean, blind time-based, and UNION-based SQLi
92. Learn SQLi detection: single quote `'`, comment sequences, boolean payloads
93. Master `sqlmap`: learn flags `-u`, `--dbs`, `--tables`, `--dump`, `--level`, `--risk`, `--tamper`
94. Learn manual SQLi exploitation without tools — critical for WAF bypass situations
95. Study second-order SQL injection: payloads stored and executed later
96. Learn NoSQL injection: MongoDB operators like `$gt`, `$where`, `$regex` in JSON bodies
97. Study GraphQL injection: field injection, introspection abuse, batch query attacks
98. Learn XML injection and XXE (XML External Entity): file read, SSRF, blind XXE via OOB
99. Practice XXE with different parsers: libxml2, Xerces, .NET XML
100. Learn LDAP injection: authentication bypass and data extraction via LDAP queries
101. Study XPATH injection: similar logic to SQLi but for XML databases
102. Learn OS Command Injection: `;`, `&&`, `||`, `|`, backticks, `$()` — both Unix and Windows
103. Study blind OS command injection: time delays, DNS callbacks via Burp Collaborator
104. Learn Server-Side Template Injection (SSTI): identify template engines (Jinja2, Twig, FreeMarker, Velocity)
105. Practice SSTI exploitation to RCE in different languages (Python, PHP, Java)

---

### 🔗 Cross-Site Scripting (XSS)

106. Understand the three types: Reflected, Stored, and DOM-based XSS
107. Learn HTML context XSS: `<script>alert(1)</script>` and why it's just the beginning
108. Study XSS in attribute context: `" onmouseover="alert(1)`, breaking out of quotes
109. Learn XSS in JavaScript context: breaking out of strings, template literals
110. Study XSS in CSS context and URL context (href, src attributes)
111. Learn DOM XSS: sources (`location.hash`, `document.URL`) and sinks (`innerHTML`, `eval()`)
112. Master Burp Suite's DOM Invader tool for DOM XSS hunting
113. Learn CSP bypass techniques: `unsafe-inline`, JSONP endpoints, Angular sandboxes
114. Study XSS filter evasion: encoding tricks, tag/attribute mutation, polyglots
115. Learn how to escalate XSS to session hijack, CSRF, keylogging, phishing
116. Learn to build a professional XSS PoC that demonstrates real impact beyond `alert(1)`
117. Study XSS in JSON responses, SVG files, and PDF generation endpoints
118. Learn mutation XSS (mXSS) and how browser parsers introduce vulnerabilities
119. Understand the difference between self-XSS and exploitable XSS for bounty purposes

---

### 🔀 Request Forgery & Logic Flaws

120. Learn CSRF: how it works, same-site vs cross-site requests, token bypass techniques
121. Study CSRF bypass: token leakage via Referer, weak token patterns, token reuse
122. Learn SSRF (Server-Side Request Forgery): what servers you can reach internally
123. Study SSRF to AWS metadata: `http://169.254.169.254/latest/meta-data/`
124. Learn blind SSRF detection using Burp Collaborator / interactsh
125. Study SSRF bypass techniques: redirects, DNS rebinding, URL parsers confusion
126. Learn SSRF to internal service exploitation (Redis, Elasticsearch, internal APIs)
127. Study Open Redirect: parameter-based, header-based, path-based redirects
128. Learn how Open Redirect chains with OAuth, phishing, and SSRF
129. Study business logic vulnerabilities: price manipulation, quantity abuse, coupon stacking
130. Learn race conditions: concurrent requests leading to TOCTOU bugs
131. Study account takeover logic: password reset flaws, token predictability, email case sensitivity
132. Learn 2FA bypass: response manipulation, code reuse, backup code abuse, rate limit absence
133. Study insecure direct object references (IDOR) as a logic/access control flaw

---

### 🔓 Access Control & Authentication

134. Learn IDOR (Insecure Direct Object Reference): numeric IDs, GUIDs, sequential enumeration
135. Study horizontal vs vertical privilege escalation via IDOR
136. Learn mass assignment: adding `isAdmin: true`, `role: "admin"` in request bodies
137. Study broken access control: forced browsing, parameter tampering, missing auth checks
138. Learn JWT attacks: `alg: none`, weak secret brute-force, RS256 to HS256 confusion
139. Study JWT header injection: embedding a malicious JWK or `kid` path traversal
140. Learn OAuth attacks: `redirect_uri` bypass, `state` parameter CSRF, token leakage
141. Study OAuth implicit flow attacks and authorization code interception
142. Learn password reset vulnerabilities: predictable tokens, host header injection, long token validity
143. Study session fixation: forcing a victim to use an attacker-controlled session ID
144. Learn authentication bypass: default credentials, type juggling in PHP (`"0" == false`)
145. Study multi-step authentication flaws: skipping steps, reordering requests

---

### 📁 File & Path Vulnerabilities

146. Learn Path Traversal: `../../../etc/passwd`, URL encoding, double encoding, null bytes
147. Study path traversal on Windows: `..\`, URL encoding, alternate data streams
148. Learn Local File Inclusion (LFI): log poisoning, `/proc/self/environ`, PHP wrappers
149. Study Remote File Inclusion (RFI): loading attacker-controlled scripts
150. Learn unrestricted file upload: bypassing extension checks, MIME type checks, magic bytes
151. Study file upload to RCE: uploading PHP shells, `.htaccess` tricks, SVG XSS
152. Learn zip slip vulnerabilities in file upload endpoints that process archives
153. Study file download vulnerabilities: IDOR in download endpoints, path traversal in filenames

---

### 🌐 HTTP-Level Vulnerabilities

154. Learn HTTP Host Header attacks: password reset poisoning, cache poisoning, SSRF
155. Study HTTP Request Smuggling: CL.TE and TE.CL desync attacks
156. Learn HTTP Response Splitting and header injection
157. Study Web Cache Poisoning: unkeyed inputs, cache deception attacks
158. Learn HTTP parameter pollution: duplicate parameters in PHP, ASP, Node.js
159. Study CRLF injection: `%0d%0a` in headers leading to header injection and XSS
160. Learn clickjacking: iframe embedding, `X-Frame-Options` bypass, UI redressing
161. Study CORS misconfigurations: reflected origin, `null` origin, subdomain trust
162. Learn subdomain takeover: dangling DNS records pointing to unclaimed cloud services
163. Study prototype pollution: `__proto__`, `constructor.prototype` in JavaScript objects

---

### ☁️ API & Modern App Vulnerabilities

164. Learn GraphQL recon: introspection queries, schema extraction, type enumeration
165. Study GraphQL vulnerabilities: injection, IDOR, excessive data exposure, batching attacks
166. Learn REST API vulnerabilities: verb tampering (GET vs POST vs PUT), version disclosure
167. Study API key exposure: in JS files, headers, error messages, documentation pages
168. Learn JWT in API contexts: common misconfigurations in `Authorization: Bearer` flows
169. Study WebSocket vulnerabilities: lack of origin check, message manipulation, CSRF via WS
170. Learn GraphQL DoS: deeply nested queries, circular fragments, field duplication
171. Study Server-Side Request Forgery in API contexts: webhook endpoints, URL fetchers
172. Learn broken object-level authorization (BOLA/IDOR) in REST and GraphQL APIs
173. Study broken function-level authorization: accessing admin endpoints without admin role
174. Learn excessive data exposure: APIs returning more fields than the UI displays
175. Study mass assignment in API contexts: sending extra fields in POST/PUT requests

---

## PHASE 4 — ADVANCED TECHNIQUES (Steps 176–230)
### *"This is where top earners separate from the pack"*

---

### 🚀 Exploitation & Chaining

176. Learn vulnerability chaining: combining low/medium bugs for critical impact
177. Study Self-XSS + CSRF = exploitable XSS (classic chain)
178. Learn Open Redirect + OAuth = account takeover chain
179. Study SSRF + IMDSv1 = credential theft on AWS
180. Learn Blind XSS: payload fires in admin panels — use XSS Hunter or interactsh
181. Study stored XSS to account takeover via cookie theft or CSRF token capture
182. Learn privilege escalation chains: IDOR → mass assignment → admin panel access
183. Study information disclosure → credential reuse → full account takeover
184. Learn subdomain takeover → stored XSS on the main domain via postMessage
185. Study XXE → SSRF → internal network pivot

---

### 🔒 Authentication Deep Dive

186. Learn OAuth 2.0 in exhaustive detail: all grant types and their attack surfaces
187. Study OpenID Connect attacks: `nonce` reuse, `id_token` leakage, issuer confusion
188. Learn SAML attacks: XML signature wrapping, comment injection, `InResponseTo` reuse
189. Study Kerberos basics for internal pentest context
190. Learn API authentication: HMAC signatures, timestamp validation bypass
191. Study certificate-based authentication and client cert weaknesses
192. Learn multi-factor authentication bypass at scale: SIM swap context, session reuse

---

### 🛡️ WAF & Filter Bypass

193. Learn WAF detection: response differences, `Server` header, error pages
194. Study WAF bypass for SQLi: `/**/`, URL encoding, case mixing, time delays
195. Learn WAF bypass for XSS: HTML entity encoding, Unicode escapes, tag/attribute mutation
196. Study WAF bypass for path traversal: null bytes, double encoding, Unicode normalization
197. Learn to use `wafwoof` for WAF fingerprinting
198. Study how to test WAF rules systematically and document bypass payloads
199. Learn header-based WAF bypass: `X-Forwarded-For`, `X-Real-IP`, `X-Originating-IP` spoofing
200. Study JSON and XML parser confusion to bypass WAF input filtering

---

### 🧪 Source Code Analysis

201. Learn to read JavaScript source code for secrets, endpoints, and logic flaws
202. Study deobfuscation of minified/obfuscated JavaScript
203. Learn to use browser DevTools profiler and debugger for dynamic JS analysis
204. Study mobile app analysis for APIs: use Apktool, jadx, and Frida for Android
205. Learn iOS app analysis: use `objection`, Frida, and `class-dump`
206. Study white-box web app testing: PHP, Node.js, Python (Django/Flask), Java (Spring)
207. Learn to use `semgrep` for static analysis of source code
208. Study common insecure coding patterns in each major web framework
209. Learn to use `retire.js` and `snyk` to find vulnerable dependencies
210. Study deserialization vulnerabilities: Java, PHP, Python pickle, Node.js `serialize`

---

### 🔭 Advanced Recon

211. Learn to build a full asset inventory from scratch for large enterprise targets
212. Study cloud asset discovery: S3 buckets, Azure blobs, GCP storage — open buckets
213. Learn S3 bucket enumeration: `aws s3 ls s3://target-bucket --no-sign-request`
214. Study Firebase misconfigs: open `.json` endpoints with sensitive data
215. Learn Google Cloud Storage enumeration and misconfiguration hunting
216. Study Elasticsearch and Kibana open instances — search Shodan with `product:elastic`
217. Learn MongoDB open instances and how to find them via Shodan/Censys
218. Study GitHub Actions and CI/CD pipeline secret exposure
219. Learn Docker registry exposure: unauthenticated `/v2/_catalog` endpoints
220. Study Kubernetes API server exposure: `kubectl` against unauthenticated clusters

---

### 📱 Mobile API & Thick Client

221. Learn to set up Burp with Android (both emulator and physical device)
222. Study certificate pinning bypass using Frida scripts or apk patching
223. Learn deep link hijacking in Android/iOS applications
224. Study WebView vulnerabilities: `addJavascriptInterface`, file:// access
225. Learn to extract hardcoded secrets from mobile APKs using MobSF
226. Study thick client vulnerabilities: DLL hijacking, insecure storage, traffic analysis
227. Learn to test Electron apps: Node.js integration, contextIsolation bypass
228. Study PWA (Progressive Web App) specific attack surface
229. Learn API versioning attacks: testing v1 when app uses v2 (older versions less protected)
230. Study gRPC security testing: protobuf inspection, authentication bypass

---

## PHASE 5 — BUG BOUNTY OPERATIONS (Steps 231–270)
### *"Being a great hacker isn't enough — you need to run a business"*

---

### 📝 Report Writing

231. Understand that a great report is the difference between accepted and rejected
232. Learn the anatomy of a perfect bug bounty report: Title, Severity, Summary, Steps to Reproduce, Impact, Remediation
233. Write titles that are clear, specific, and include the vulnerability class and affected endpoint
234. Write step-by-step reproduction instructions assuming the reader knows nothing
235. Always include a working PoC — video, screenshot, or curl command
236. Learn to articulate business impact beyond "XSS works" — what data can be stolen? what accounts can be taken over?
237. Study how CVSS scoring works and learn to justify your severity ratings
238. Reference CVE/CWE numbers where appropriate
239. Learn to write remediation advice — it shows professionalism and builds rapport
240. Study triage team psychology: be respectful, clear, and patient in communications
241. Learn to escalate severity through chaining — show the full attack path
242. Always attach Burp Suite HTTP request/response captures as evidence
243. Never report duplicate bugs — do research before submitting
244. Learn to write a "N-Day" report: documenting a known CVE on a live target
245. Study how to communicate around out-of-scope edge cases diplomatically

---

### 💰 Earnings Strategy

246. Start on PortSwigger Web Security Academy — complete all labs before real targets
247. Begin on HackerOne and Bugcrowd public programs: no invite required
248. Target programs with "safe harbor" clauses for legal protection
249. Focus your first 3 months on one program — go deep, not wide
250. Track your submission-to-bounty ratio per program to identify where you convert best
251. Learn to identify "seasonal" bugs: new features, acquisitions, infrastructure migrations
252. Study leaderboards — top earners focus on critical/high bugs, not quantity of lows
253. Learn when to upgrade from public programs to private programs (invitation-based)
254. Build a reputation on HackerOne: signal score, thanks, and disclosure count matter
255. Apply to private programs via reputation milestones — they pay more and have less competition
256. Learn about retesting: programs pay for verifying fixes on your old bugs
257. Study "regression" bugs: patches that reintroduce old vulnerabilities
258. Learn to combine passive income (write-up ads, YouTube, blog) with bounty hunting
259. Consider building automation that finds a class of bugs at scale across multiple programs
260. Learn about BBSS (Bug Bounty as a Service) platforms: Synack, Cobalt, HackerOne Response

---

### ⚖️ Legal & Ethics

261. Always read the program policy before testing — not reading it is not an excuse
262. Never test out-of-scope assets — it can lead to legal consequences
263. Understand the Computer Fraud and Abuse Act (CFAA) and equivalent laws in your country
264. Never access, copy, or download user data beyond what's needed to prove the bug
265. Do not disclose vulnerabilities publicly before the program's disclosure timeline (usually 90 days)
266. Learn to handle "no bounty" programs ethically — report anyway if it protects users
267. Understand coordinated disclosure vs full disclosure
268. Never use automated scanners in a way that causes service disruption (DoS)
269. Keep detailed logs of every test — timestamped notes and HTTP captures
270. Understand that some programs have strict rules against automation — read the policy

---

## PHASE 6 — SCALING TO $10K+ (Steps 271–300)
### *"Automation, specialization, and compound knowledge"*

---

### ⚙️ Automation & Tooling

271. Build a custom recon automation pipeline: subfinder → httpx → nuclei → notify
272. Write custom `nuclei` templates for vulnerabilities you find repeatedly
273. Build a subdomain monitoring system: alert when new subdomains appear
274. Automate screenshot capture and change detection on all discovered assets
275. Write custom `ffuf` wordlists based on targets you've studied deeply
276. Build a personal knowledge base of all bugs found, techniques used, and payloads
277. Learn Python: automate HTTP requests with `requests`, parse JS with `BeautifulSoup`
278. Study `burp extensions` (BApp Store): turbo intruder, logger++, JS Miner, Active Scan++
279. Build custom Burp extensions in Python or Java for recurring manual tasks
280. Learn to use `interactsh` (self-hosted Collaborator alternative) for blind testing
281. Deploy a personal VPS (DigitalOcean/Linode) for running long recon jobs
282. Learn Docker: containerize your tools for consistent, reproducible environments
283. Build a bug bounty dashboard to track programs, submissions, and earnings

---

### 🎯 Specialization Paths

284. **Path A — API Specialist**: Master REST/GraphQL/gRPC, focus on fintech and SaaS programs
285. **Path B — Authentication Expert**: Specialize in OAuth, SAML, SSO — critical severity territory
286. **Path C — Cloud Security**: AWS/GCP/Azure misconfigs, S3 buckets, metadata service abuse
287. **Path D — Mobile + API**: Android/iOS testing plus their backend APIs
288. **Path E — Business Logic**: Focus on e-commerce, banking, and healthcare logic flaws
289. **Path F — Recon Automation**: Build the best recon pipeline and monetize through faster coverage
290. Study the top 10 publicly disclosed reports in your chosen specialization area
291. Follow and study hackers who specialize in your chosen area (Twitter/X, blogs, write-ups)

---

### 🌍 Community & Continuous Learning

292. Follow security researchers: @NahamSec, @TomNomNom, @jobertabma, @LiveOverflow, @PortSwigger
293. Watch NahamSec's live hacking streams on Twitch/YouTube for real-world methodology
294. Join bug bounty Discord servers: NahamSec's Discord, Bug Bounty World, Hacking Hub
295. Attend virtual/in-person conferences: DEF CON, Black Hat, BSides, NahamCon
296. Read new CVEs and public disclosures weekly — subscribe to security newsletters
297. Practice new techniques on HackTheBox, TryHackMe, and PortSwigger labs constantly
298. Write public write-ups for bugs you've disclosed — builds reputation and passive income
299. Mentor newer hunters — teaching forces deeper understanding and builds your network
300. **Never stop learning.** The web evolves every year. New frameworks = new attack surfaces. The $10K/month hunter is simply the one who never stopped when it got hard.

---

## 📊 Earnings Milestones Reference

| Stage | Monthly Earnings | What You Need |
|---|---|---|
| Beginner | $0–$200 | Foundations done, first bugs found (info disc, low severity) |
| Intermediate | $200–$1,000 | Consistent medium bugs, good report writing, 1–2 programs |
| Advanced | $1,000–$5,000 | High/critical bugs, chaining, automation basics |
| Expert | $5,000–$10,000 | Private programs, specialization, scale and speed |
| Elite | $10,000–$20,000+ | Automation, specialization, continuous learning, network |

---

## 🧰 Essential Tools List

| Category | Tools |
|---|---|
| Proxy | Burp Suite Pro, OWASP ZAP |
| Recon | subfinder, amass, httpx, dnsx, nuclei |
| Fuzzing | ffuf, gobuster, feroxbuster |
| Scanning | nmap, masscan, shodan-cli |
| JS Analysis | LinkFinder, JSParser, SecretFinder |
| Crawling | hakrawler, katana, gau, waybackurls |
| Exploitation | sqlmap, dalfox (XSS), jwt_tool |
| Collab | Burp Collaborator, interactsh |
| Monitoring | notify, gowitness, aquatone |
| OSINT | theHarvester, shodan, censys |

---

## 📚 Essential Resources

- **PortSwigger Web Security Academy** — free, best structured learning available
- **HackerOne Hacktivity** — read disclosed reports daily
- **OWASP Testing Guide (OTG)** — your bible
- **The Web Application Hacker's Handbook** (book)
- **Real-World Bug Hunting** by Peter Yaworski (book)
- **Bug Bounty Bootcamp** by Vickie Li (book)
- **NahamSec on YouTube** — real methodology, real bugs
- **LiveOverflow on YouTube** — deep technical content
- **PentesterLab** — code-based learning with pro subscription

---

*Built for serious hunters. Follow the path. Do the reps. The money follows the skill.*

*Last updated: 2026 | 300 Steps | Web Pentesting & Bug Bounty Roadmap*
