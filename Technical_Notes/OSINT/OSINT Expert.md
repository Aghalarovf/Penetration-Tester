# 🔍 OSINT → Expert+ Red Team Intelligence Roadmap
## 200 Steps from Zero to Professional-Grade Intelligence Operator

> **Disciplines Covered:** OSINT · SIGINT · SOCINT · HUMINT (digital) · GEOINT · FININT · DARKINT · TECHINT · CYBINT
> **Target Level:** Absolute beginner → Expert+ / Red Team Intelligence Professional
> **Philosophy:** *Underrated methods first. Operational security always. Think like an adversary.*

---

## 📋 ROADMAP OVERVIEW

| Phase | Steps | Focus |
|-------|-------|-------|
| **Phase 0** | 1–10 | Mindset & OpSec Foundation |
| **Phase 1** | 11–30 | OSINT Core Fundamentals |
| **Phase 2** | 31–55 | SOCINT – Social Intelligence |
| **Phase 3** | 56–75 | SIGINT – Signals Intelligence |
| **Phase 4** | 76–95 | GEOINT – Geospatial Intelligence |
| **Phase 5** | 96–115 | TECHINT / CYBINT – Technical Intelligence |
| **Phase 6** | 116–135 | FININT – Financial Intelligence |
| **Phase 7** | 136–155 | DARKINT – Dark Web & Underground Intel |
| **Phase 8** | 156–170 | Advanced Red Team OSINT Operations |
| **Phase 9** | 171–185 | AI-Augmented Intelligence |
| **Phase 10** | 186–200 | Expert+ Tradecraft & Mastery |

---

## ⚠️ LEGAL & ETHICAL DISCLAIMER

All techniques in this roadmap must be applied only within legal frameworks, with proper authorization, or in controlled lab environments. OSINT is lawful only when targeting public data. Unauthorized interception of signals (SIGINT) is illegal in most jurisdictions. This roadmap is for **defensive intelligence, red team engagements, and authorized penetration testing only**.

---

## PHASE 0 — MINDSET & OPERATIONAL SECURITY FOUNDATION
*Steps 1–10 | "You cannot collect intelligence if you leave footprints."*

---

### Step 1 — Develop the Intelligence Mindset
Stop thinking like a researcher. Start thinking like an analyst. Intelligence work is not about *finding* information — it's about *what the information means*. Practice structured analytic techniques (SATs). Read "Psychology of Intelligence Analysis" by Richards Heuer (CIA declassified — free online). Understand cognitive biases: anchoring, confirmation bias, mirror imaging. These biases kill analysts.

### Step 2 — Understand the Intelligence Cycle
Master the 6-stage intelligence cycle: **Planning → Collection → Processing → Analysis → Dissemination → Feedback**. Every OSINT operation must follow this cycle. Never start collecting without a defined intelligence requirement (IR). Write your IR before touching a keyboard.

### Step 3 — Build an Air-Gapped Analysis Environment
Create a dedicated OSINT workstation. Use a separate physical machine or a VM (VirtualBox/VMware) with a fresh OS for each investigation. Never mix personal browsing with operational work. Snapshot your VM before each session so you can roll back. Use Whonix or Tails OS for high-sensitivity work.

### Step 4 — Master Operational Security (OpSec) from Day One
OpSec is not a feature — it is a discipline. Apply the 5-step OpSec process: identify critical information → analyze threats → analyze vulnerabilities → assess risk → apply countermeasures. Read the NSA's OPSEC manual. Your digital exhaust (browser fingerprint, timing patterns, metadata) can expose you and your target simultaneously.

### Step 5 — Build a Sock Puppet Infrastructure
Create believable alternate personas for OSINT collection. Each persona needs: aged social media accounts (3–6 months minimum), consistent backstory, unique email address (ProtonMail/Tutanota), unique phone number (VoIP services: MySudo, TextNow), and a plausible geographic location. Never reuse personas across operations.

### Step 6 — Understand Browser Fingerprinting & Anti-Detection
Your browser is a fingerprint. Learn what canvas fingerprinting, WebGL fingerprinting, and font enumeration reveal about you. Use tools: **Whoer.net**, **BrowserLeaks.com**, **Coveryourtracks.eff.org**. Use Firefox with uBlock Origin + Canvas Blocker + Random Agent Spoofer for OSINT work. Never use your personal browser for investigations.

### Step 7 — Master VPN, Proxy, and Tor Tradecraft
Understand the difference between these tools and their failure modes. VPNs log. Tor is slow but powerful. Proxies can be compromised. Build a layered approach: **VPN → Tor → Target** for sensitive collection. Learn about exit node attacks on Tor. Use residential proxies for bypassing geo-blocks without triggering bot detection.

### Step 8 — Learn Metadata Analysis
Every file leaks metadata. A photo can reveal GPS coordinates, camera model, and timestamp. A Word document can reveal the author, revision history, and tracked changes. Tools: **ExifTool**, **FOCA**, **Metagoofil**, **MAT2**. Before submitting any file in an operation, strip all metadata. When analyzing targets, extract everything.

### Step 9 — Establish a Note-Taking & Case Management System
Professional intelligence work requires structured documentation. Use **Maltego** for link analysis, **Obsidian** for knowledge graphs, **CherryTree** for hierarchical notes, or **Hunchly** for web investigation capture. Create an evidence log for every step: URL, timestamp, screenshot, analysis note. Chain of custody matters.

### Step 10 — Set Up Your Core OSINT Platform
Install **Kali Linux** or **Buscador OSINT VM**. Install key tools: `theHarvester`, `Recon-ng`, `Maltego CE`, `Shodan CLI`, `Amass`, `SpiderFoot`, `Photon`, `metagoofil`. Learn the Recon-ng module system — it is one of the most underrated OSINT frameworks. Build a tool inventory list and document what each tool does *before* using it.

---

## PHASE 1 — OSINT CORE FUNDAMENTALS
*Steps 11–30 | "Google is not OSINT. Google is just the beginning."*

---

### Step 11 — Advanced Google Dorking (Beyond the Basics)
Most people know `site:`, `filetype:`, and `inurl:`. Go deeper. Learn: `cache:`, `related:`, `AROUND(n)` proximity operator, and how to combine operators for surgical queries. **Underrated dork:** `intitle:"index of" "parent directory" filetype:xlsx` finds exposed spreadsheets. Build a dork library for common targets: HR databases, backup files, config files, login portals.

**Critical dorks to master:**
```
"@gmail.com" filetype:xls site:target.com
intext:"password" filetype:log
intitle:"phpMyAdmin" intext:"Welcome to phpMyAdmin"
ext:sql "INSERT INTO" "VALUES"
inurl:/wp-content/uploads/ filetype:pdf
```

### Step 12 — Master Bing, DuckDuckGo, and Yandex for OSINT
Google is not indexed everywhere. **Yandex** is significantly better for facial recognition and Russian-language content. **Bing** indexes different content than Google — test the same dork on both. **DuckDuckGo** doesn't personalize results, giving you raw data. **Baidu** for Chinese-language targets. Build a multi-engine search workflow.

### Step 13 — Certificate Transparency Logs
Every SSL certificate issued is logged publicly. This is massively underused. Tools: **crt.sh**, **Censys.io**, **Facebook CT Monitor**. Search `crt.sh/?q=%.target.com` to find every subdomain that has ever had an SSL certificate — including internal ones, test environments, and forgotten infrastructure. Many bug bounty hunters ignore this. Red teamers should not.

### Step 14 — DNS Intelligence & Passive DNS
DNS records are a goldmine. Learn: A, AAAA, MX, NS, TXT, CNAME, SOA records and what each reveals. **Passive DNS** databases store historical DNS resolution data — even after records are deleted. Tools: **SecurityTrails**, **PassiveDNS**, **DNSDB** (Farsight), **Robtex**. Historical MX records can reveal old email providers with weaker security. Historical A records show IP address history.

### Step 15 — WHOIS & Domain Intelligence (Deep Dive)
WHOIS has become less useful with GDPR, but the tradecraft around it remains vital. Learn: registrar history, nameserver pivoting, registrant email pivoting. **Underrated:** Use `DomainTools` reverse WHOIS to find all domains registered by the same email. Historical WHOIS (before GDPR) is still stored in databases like **DomainEye** and **WhoisXMLAPI**.

### Step 16 — IP Address Intelligence
An IP address tells a story. Tools: **IPinfo.io**, **Shodan**, **Censys**, **GreyNoise**, **AbuseIPDB**. Learn: ASN (Autonomous System Number) lookups, BGP routing tables (route-views.oregon-ix.net), and how to identify CDNs masking the real origin IP. **Underrated technique:** Look up the ASN of a company and enumerate all IP ranges they own. This reveals infrastructure they never publicized.

### Step 17 — Shodan Mastery (The Most Underrated OSINT Tool)
Shodan indexes internet-connected devices. It sees your webcam, your router, your industrial control system. Learn Shodan filters: `org:`, `port:`, `product:`, `before/after:`, `ssl.cert.subject.cn:`, `http.title:`. **Critical Shodan dorks:**
```
org:"Target Corp" port:22
ssl:"Target Corp" 200
http.title:"Dashboard" org:"Target"
product:"Apache" version:"2.2"
```
Shodan Monitor lets you track your own (or authorized target's) exposure over time.

### Step 18 — Censys & ZoomEye Intelligence
Censys is Shodan's academic rival with stronger certificate analysis. Use Censys for: TLS certificate analysis, finding misconfigured cloud storage, exposed databases. **ZoomEye** is the Chinese equivalent with different coverage — particularly useful for APAC targets. Both use different scanning methodologies and will find different things than Shodan.

### Step 19 — Google Cache & Wayback Machine Intelligence
Content gets deleted. Intelligence doesn't disappear. **Wayback Machine** (archive.org) stores snapshots of web pages going back to 1996. **Underrated technique:** Look at a target's website from 2–5 years ago. Old employee directories, old product lists, old job postings reveal infrastructure and personnel that still exist. Use `waybackurls` tool to extract all URLs ever archived for a domain.

```bash
waybackurls target.com | grep "\.php?id=" | sort -u
```

### Step 20 — Search Engine Operators for File Intelligence
Most exposed sensitive files are indexed. Build systematic file-type searches:
```
site:target.com filetype:pdf "confidential"
site:target.com filetype:xls "username" "password"
site:target.com filetype:doc "internal use only"
site:target.com ext:env OR ext:cfg OR ext:conf
site:target.com ext:bak OR ext:backup OR ext:old
```
**Underrated:** `ext:pptx site:target.com` — PowerPoint presentations often contain internal architecture diagrams, personnel lists, and strategy documents.

### Step 21 — GitHub & Code Repository Intelligence (CRITICAL)
GitHub is one of the most valuable OSINT sources. Developers accidentally commit: API keys, passwords, private keys, database connection strings, internal URLs. Tools: **GitDorker**, **TruffleHog**, **Gitleaks**, **GitHub Dork Search**. Search GitHub directly:
```
"target.com" password
"target.com" api_key
"target.com" BEGIN RSA PRIVATE KEY
org:TargetOrg filename:.env
```
Also search: GitLab, Bitbucket, SourceForge, Pastebin, Gist.

### Step 22 — Cloud Storage OSINT (S3, Azure Blob, GCS)
Misconfigured cloud storage is epidemic. Learn to find exposed buckets:
- **AWS S3:** `target-backup.s3.amazonaws.com`, use `GrayhatWarfare`, `BucketFinder`, `AWSBucketDump`
- **Azure Blob:** `target.blob.core.windows.net`
- **GCS:** `storage.googleapis.com/target-bucket`
- **DigitalOcean Spaces:** `target.nyc3.digitaloceanspaces.com`

Tool: **CloudEnum** automates discovery across all major cloud providers.

### Step 23 — Pastebin & Paste Site Intelligence
Sensitive data gets pasted. Monitor: **Pastebin**, **Ghostbin**, **Hastebin**, **PrivateBin**, **dpaste**. Use **PasteHunter** to monitor for keywords. **Underrated sources:** Telegram channels that dump leaked data, Rentry.co, bin.bz. Set up Google Alerts for `"target.com" site:pastebin.com`.

### Step 24 — Job Posting OSINT (Massively Underrated)
Job postings reveal internal technology stacks, team structures, security tools used, and strategic initiatives. Search: LinkedIn Jobs, Indeed, Glassdoor, Dice, and target company careers pages. A job posting for "Senior AWS Security Engineer familiar with Palo Alto Networks and CrowdStrike" tells you their entire security stack. This is legally public information that most OSINT practitioners ignore.

### Step 25 — Email Address Discovery & Verification
Finding and verifying email addresses is a core OSINT skill. Tools: **Hunter.io**, **Phonebook.cz**, **Skymem**, **Voila Norbert**, **EmailRep.io**. Learn email format patterns: firstname.lastname@, f.lastname@, firstnamelastname@. Verify without sending: **email-verify CLI**, **verify-email.org**. **Underrated:** MX Toolbox SMTP testing — manually test SMTP server responses to verify email existence without sending a message.

### Step 26 — Document & Report OSINT (SEC, Court, Patents)
Public records are massively underutilized. Sources:
- **SEC EDGAR**: Public company filings, executive compensation, subsidiaries
- **PACER**: US federal court records — lawsuits reveal vendors, partnerships, internal disputes
- **USPTO/EPO**: Patent filings reveal R&D roadmaps and proprietary technologies
- **OpenCorporates**: Company registry data across 130+ jurisdictions
- **Dun & Bradstreet**: Business intelligence (some free data available)

### Step 27 — Breach Data Intelligence
Data breaches expose credentials, PII, and infrastructure details. **Legal access methods:** **Have I Been Pwned (HIBP) API**, **IntelX (Intelligence X)**, **DeHashed** (requires subscription), **Snusbase**. Use breach data defensively: identify if target organization's credentials are exposed. Cross-reference usernames across breaches to build persona profiles. **Underrated:** Look for older breaches from 2012–2016 — many organizations never changed credentials.

### Step 28 — Shodan Dorking for Internal Exposure
Beyond basic Shodan, learn to find internally-exposed services:
```
http.html:"Internal use only" org:"Target"
"Set-Cookie: PHPSESSID" http.title:"Admin"
ssl.cert.subject.cn:"*.target.com" port:8443
http.component:"jQuery" version:"1.6" org:"Target"
```
VPN login pages exposed on the internet often reveal VPN product and version — enabling targeted exploitation.

### Step 29 — Build a Custom OSINT Workflow Template
Create a reusable reconnaissance checklist for every target type: individual person, company, infrastructure, social media account. Document your workflows in a runbook format. Every professional operator has standard operating procedures (SOPs). Without them, you miss things. Your SOP should cover: passive recon → active recon → analysis → reporting phases.

### Step 30 — Understand OSINT Legal Boundaries Globally
OSINT laws vary by country. **CFAA (US)** — unauthorized access. **GDPR (EU)** — data collection limits. **Computer Misuse Act (UK)**. Know the difference between: passive OSINT (no interaction with target systems), active OSINT (DNS queries, sending emails), and intrusive OSINT (port scanning, crawling). For red team engagements, get written authorization before any active collection.

---

## PHASE 2 — SOCINT: SOCIAL INTELLIGENCE
*Steps 31–55 | "People are the most exploitable vulnerability."*

---

### Step 31 — Understand SOCINT as a Discipline
Social Intelligence (SOCINT) is the collection of intelligence from social networks, forums, communities, and human behavioral patterns online. It goes beyond "Facebook stalking" — it involves behavioral analysis, network mapping, influence operations awareness, and persona research. The goal is to understand human targets: their relationships, routines, motivations, and vulnerabilities.

### Step 32 — LinkedIn Deep Dive (The Underrated Corporate Intelligence Source)
LinkedIn is the most valuable social OSINT platform for corporate intelligence. Techniques:
- Export company employee list via Google: `site:linkedin.com/in/ "works at Target Corp"`
- Identify organizational hierarchy from endorsements
- Track employee movement to spot talent exodus (indicates company problems)
- Read posts for technology stack, project hints
- **Underrated:** LinkedIn Sales Navigator free trial gives access to advanced search filters for 30 days

### Step 33 — Twitter/X OSINT Advanced Techniques
Twitter/X has the highest signal density of any social platform for real-time intelligence. Advanced techniques:
- **Advanced Search:** `(from:target) until:2024-01-01 since:2020-01-01 lang:en`
- **Geolocation tweets:** Search for tweets near a specific location
- **Shadow account discovery:** Check who they follow, who follows them — map their real network
- Tools: **Twint** (archived), **snscrape**, **Twitter API v2 free tier**, **Tweetdeck**
- **Underrated:** Analyze tweet timing patterns to infer time zones and routines

### Step 34 — Instagram OSINT (Visual Intelligence)
Instagram is undervalued as an OSINT source. Techniques:
- **Geo-tagged photos:** Extract location from photos even without explicit geotag (background analysis)
- **Tagged users:** Find connected accounts through photo tags
- **Story archives:** Use web.archive.org for saved public stories
- Tools: **Osintgram**, **InstaLooter**, **Imginn** (for anonymous viewing)
- **Underrated:** Analyze background environments in photos for location confirmation (building signs, landmarks, vegetation type)

### Step 35 — Facebook OSINT (Still Massive, Often Ignored)
Facebook Graph search no longer works but Facebook OSINT is still rich. Techniques:
- **Facebook Watch** public video comments reveal real identities
- **Group membership** reveals affiliations and beliefs
- **Event attendance** reveals location and schedule
- Tools: **Sowsearch**, **Facebook ID finder**, **Lookup-ID.com**
- URL trick: `facebook.com/[username]/friends_mutual` for mutual friend enumeration
- **Underrated:** Facebook Marketplace reveals location, photos of home interiors, and behavioral patterns

### Step 36 — TikTok OSINT
TikTok is the fastest-growing social OSINT source and most analysts ignore it. Every TikTok video contains metadata and social signals:
- Account creation dates (visible in profile links)
- Follower/following network analysis
- Comments reveal associated accounts
- Duet videos connect real-world relationships
- Tools: **TikTok OSINT** (GitHub), **Tikbuddy**
- **Underrated:** TikTok video metadata sometimes includes GPS data in older uploads

### Step 37 — Telegram OSINT (Critical for Underground Intelligence)
Telegram is the primary communication platform for: threat actors, cybercriminals, hacktivists, journalists, and activists. Techniques:
- Search public channels: `t.me/s/channelname`
- Tools: **Telepathy** (Telegram OSINT tool), **TGStat**, **Telemetr.io**
- **Username enumeration:** `t.me/username` — 200 = exists, 404 = doesn't
- Telegram bots leak member information
- **Underrated:** Forward Telegram messages to `@getidsbot` to get user IDs, then search those IDs across databases

### Step 38 — Discord OSINT
Discord servers contain valuable intelligence on hacker groups, gaming communities, and activist networks. Tools: **DISINT**, **Discord History Tracker**, **DiscordLeaks** (public database of leaked servers). Server invite links can be found via Google dorking: `site:discord.gg "invite" "target keyword"`. **Underrated:** Deleted Discord messages may still be cached in third-party bots (MEE6, Carl-bot logs).

### Step 39 — Reddit OSINT
Reddit users often provide highly personal information across pseudonymous accounts. Tools: **Redective**, **Pushshift API** (historical data), **Reddit Search** (pullpush.io). Technique: Analyze posting history across subreddits to build a behavioral profile. Look for: work-related posts (reveals employer), location posts, health discussions, and relationship mentions. **Underrated:** Sort by "controversial" to find the target's emotional triggers.

### Step 40 — Username OSINT & Cross-Platform Pivoting
A single username can unlock a target's entire digital life. Tools: **Sherlock**, **WhatsMyName**, **Namechk**, **UserSearch.org**, **Maigret** (most powerful). Maigret checks 2,000+ websites and generates detailed reports. **Critical technique:** Targets often use the same username with slight variations — search for variations: `username`, `username1`, `_username_`, `username_official`.

### Step 41 — Reverse Image Search (Multi-Engine Approach)
Single-engine reverse image search misses too much. Build a workflow:
1. **Google Images** — broadest index
2. **Yandex Images** — best for faces, strongest facial recognition
3. **TinEye** — tracks image appearance over time
4. **Bing Visual Search** — different index
5. **PimEyes** — face search engine (subscription, powerful)
6. **FaceCheck.ID** — free facial search
**Underrated:** Search cropped sections of a photo, not just the full image.

### Step 42 — Phone Number OSINT
Phone numbers are identity anchors. Tools: **Numverify API**, **TrueCaller** (crowdsourced name database), **Getcontact**, **Phone Validator**. Reverse lookup a mobile number across: WhatsApp (check profile photo & last seen via WhatsApp Web), Telegram (search by phone), Signal (check via app). **Underrated:** Call recording apps often store metadata in public Google Drive links — search for target phone number in quotes on Google.

### Step 43 — Email-to-Social Profile Pivoting
An email address can reveal connected social accounts. Techniques:
- Upload email to **HaveIBeenPwned** to see breach associations
- Enter email in Facebook "Forgot Password" to see masked phone
- Try logging into Google/Apple with the email to confirm account existence
- **Gravatar**: `gravatar.com/[md5_of_email]` — reveals profile photo and linked accounts
- Tools: **Holehe** (checks email registration on 120+ sites), **EmailRep**

### Step 44 — Social Network Mapping & Link Analysis
Intelligence value comes from relationships, not individuals. Use **Maltego** to visualize relationship networks. Import: Twitter followers, LinkedIn connections, email communications, and domain registrations into a single graph. Identify: key influencers, relationship bridges (people connecting otherwise separate groups), and isolated clusters (potential covert cells). **Underrated:** Map the target's *second-degree network* — who do their contacts know?

### Step 45 — Behavioral Pattern Analysis from Social Data
Go beyond data collection into analysis. Identify patterns:
- **Posting time analysis:** What time of day does the target post? This reveals time zone and schedule
- **Content sentiment:** Is content getting more negative over time? (Stress, job loss, personal issues)
- **Engagement patterns:** Who does the target consistently interact with?
- **Vocabulary analysis:** Language patterns reveal education, region, age group
- Tool: **Wordsmith** for text pattern analysis

### Step 46 — Online Forum OSINT (Underrated Goldmine)
Forums contain massive amounts of unguarded information. Sources:
- **Craigslist** — personal ads, services, reveals location and schedule
- **Nextdoor** — neighborhood-level location data (publicly accessible posts)
- **Quora** — users often answer questions using real expertise and identity
- **Medium** — professional publishing with personal details
- **Substack** — subscription newsletters reveal expertise and real identity
- Specialty forums: **DeviantArt**, **Flickr**, **Behance** — creative professionals expose portfolios

### Step 47 — Dating App OSINT (Ethical Research Only, With Authorization)
In authorized investigations (law enforcement, corporate fraud, red team targeting): dating apps reveal real photos (cross-referenceable), location (within meters), workplace (often listed), and physical description. Tools: **SwipeLeft OSINT scripts**, **Bumble location spoofing for investigation**. **Underrated:** Bumble shows exact distance without login for some app versions — triangulation possible.

### Step 48 — Gaming Platform OSINT
Gamers leak significant amounts of information. Platforms: **Steam** (public profiles show play history, friends), **Xbox Live** (Gamertag lookup reveals real name via linked accounts), **PlayStation Network**, **Battle.net**, **Epic Games**. Tools: **SteamSpy**, **GamersGlobal**. **Underrated:** Steam game purchase history is sometimes public — reveals disposable income level, interests, and time zone from play hours.

### Step 49 — OSINT on Organizations via Social Media
Corporate OSINT through employee social media:
- Aggregate LinkedIn profiles → build org chart without the website
- Employee Instagram/Twitter posts → physical office photos, badge readers visible, security equipment
- Facebook check-ins → office locations, client visit locations
- Glassdoor reviews → internal culture, security policy complaints, technology frustrations
- **Underrated:** Employee fitness apps (Strava, MapMyRun) with public profiles reveal commute routes and office locations

### Step 50 — SOCMINT for Geolocation via Social Posts
Photos contain geolocation intelligence even without GPS metadata. Techniques:
- Analyze sky color for approximate time of day and latitude
- Vegetation type narrows geographic region
- Visible road signs, license plates, architecture
- Sun angle calculation using **SunCalc.org** and timestamp
- **Bellingcat geolocation method:** Use shadow length + direction to calculate exact sun position and confirm location

### Step 51 — Influence Operations & Inauthentic Behavior Detection
Learn to identify coordinated inauthentic behavior (CIB) — essential for both detecting threat actors and understanding how they operate:
- Bot detection: posting frequency (>50 posts/day), uniform language patterns, account age vs follower count
- Amplification networks: clusters of accounts all sharing the same content within minutes
- Tools: **Botometer**, **OSoMe Bot Analysis**, **Inauthentic.info**
- Study Meta's CIB takedown reports for real-world examples

### Step 52 — Dark Social OSINT (WhatsApp, Signal, iMessage Intelligence)
"Dark social" platforms seem opaque but leak intelligence at the edges:
- **WhatsApp:** Profile photo visible without contact (some privacy settings), status visible, "last seen" timing
- **Signal:** Phone number required — use reverse phone lookup after finding the number
- WhatsApp group links shared on forums/websites can be joined for monitoring
- **Underrated:** WhatsApp Business accounts often show business name, website, and location

### Step 53 — Podcast & Audio OSINT
Podcasts are a massively underused intelligence source. CEOs, executives, and experts speak candidly on podcasts. Search: **Listen Notes**, **Podchaser**, **Spotify Podcast Search**. Use speech-to-text tools (**Whisper AI**) to transcribe podcasts and search transcripts for keywords. **Underrated:** Podcast audio background noise analysis can reveal location (traffic, accents, environment sounds).

### Step 54 — YouTube & Video Platform OSINT
YouTube channels reveal: metadata (upload time, geolocation tags, description links), comments (real identity connections), subscriber/subscription lists. Tools: **YouTube Data Tools** (Bernhard Rieder), **4K Video Downloader** for metadata extraction. **Underrated:** YouTube Live streams sometimes reveal unguarded conversations and real-time location. Analyze video backgrounds for environment intelligence.

### Step 55 — Build a SOCINT Persona Analysis Report
Synthesize all SOCINT collection into a structured analysis report. Sections:
1. Subject Overview (identity, aliases, platforms)
2. Relationship Map (key contacts, influence network)
3. Behavioral Profile (posting patterns, interests, topics)
4. Geographic Intelligence (locations visited, home area)
5. Psychological Profile (linguistic analysis, sentiment trends)
6. Security Vulnerabilities (oversharing, potential phishing hooks)
7. Gaps & Uncertainties

---

## PHASE 3 — SIGINT: SIGNALS INTELLIGENCE
*Steps 56–75 | "The air is full of information. Learn to listen."*

---

### Step 56 — SIGINT Fundamentals & Legal Framework
SIGINT (Signals Intelligence) is the interception and analysis of electronic signals. Legal SIGINT includes: listening to unencrypted radio transmissions (legal in most jurisdictions), analyzing RF emissions passively, monitoring public Wi-Fi traffic (on authorized networks). **ILLEGAL without authorization:** intercepting encrypted communications, wiretapping, GSM interception. Always operate within legal boundaries.

### Step 57 — Software-Defined Radio (SDR) Setup
SDR is the gateway to SIGINT. Buy a **RTL-SDR Blog V3 dongle** (~$30). This allows you to receive radio signals from 500 kHz to 1.75 GHz. Install: **SDR#** (Windows), **GQRX** (Linux/Mac), **SDR++**. Learn the basics: frequency, bandwidth, modulation (AM, FM, SSB, CW). Start listening to: FM radio, air traffic control (118–136 MHz), weather broadcasts (162 MHz), maritime VHF.

### Step 58 — Air Traffic Control & Aviation OSINT
ATC communications are unencrypted and public. Listen to live ATC at **LiveATC.net** or with your SDR. Learn: ICAO aircraft identifiers, squawk codes, phraseology. Track flights with **Flightradar24**, **ADS-B Exchange** (unfiltered — shows military and private jets that FR24 hides), **ADSB.fi**. **Critical for OSINT:** ADS-B Exchange shows flights blocked from commercial trackers, including government and corporate aircraft.

### Step 59 — Maritime Signal Intelligence
Ships broadcast AIS (Automatic Identification System) signals publicly. Tools: **MarineTraffic**, **VesselFinder**, **AISHub**. Learn to: identify vessel type, track historical routes, correlate vessel movements with corporate activities (cargo ships for supply chain intelligence, private yachts for executive tracking). **Underrated:** Dark vessel detection — ships that turn off AIS during suspicious activities.

### Step 60 — Weather Satellite Signal Reception
NOAA weather satellites broadcast imagery in real time on 137 MHz. With an RTL-SDR and **WXtoImg** software, receive actual satellite imagery showing cloud cover. **Underrated for OSINT:** GOES satellite imagery (received via 1.7 GHz with an LNB) can show large-scale environmental events, which correlates with open-source environmental intelligence.

### Step 61 — APRS & Amateur Radio OSINT
APRS (Automatic Packet Reporting System) is a real-time tactical digital communications system. Trackers are broadcast on 144.390 MHz (North America). View data at **APRS.fi**. APRS reveals: vehicle locations, weather station data, and ham radio operator positions. **Underrated:** APRS data is archived and searchable by callsign — track historical movement patterns.

### Step 62 — Wi-Fi Intelligence (Passive Collection)
Wi-Fi probe requests reveal devices searching for previously connected networks. A device broadcasts the names of all networks it has connected to — these reveal travel history, workplace, and home. **Underrated technique:** With a passive Wi-Fi capture setup (monitor mode NIC), capture probe requests in a physical location to identify what networks are present. Tools: **Kismet**, **airodump-ng** (for authorized testing only).

### Step 63 — Bluetooth Signal Intelligence
Bluetooth devices broadcast their presence (in discoverable mode) and device names often include owner names or device types. Tools: **BlueZ** (Linux), **Ubertooth One** (hardware, for BLE sniffing). **Underrated:** BLE (Bluetooth Low Energy) beacons broadcast continuously — retail stores, hospitals, and offices use them for location tracking. Passive BLE scanning reveals device types and sometimes identifiers.

### Step 64 — IMSI Catchers & Cellular Intelligence (Legal & Authorized)
Understanding how IMSI catchers (Stingrays) work is essential for red teamers. They impersonate cell towers to capture mobile device identifiers. **For authorized red team operations:** Understanding IMSI catcher capability helps assess physical security. Legal cellular OSINT: tower location databases (**CellMapper**, **OpenCellID**), signal triangulation concepts. **Never deploy IMSI catchers without proper legal authority.**

### Step 65 — RF Spectrum Analysis for Physical Security
Physical red teamers use RF analysis to detect: wireless cameras (2.4 GHz bursts), wireless alarm sensors (433 MHz), card readers (125 kHz RFID, 13.56 MHz NFC), and building management systems. Tools: **HackRF One** (transmit + receive), **RTL-SDR** (receive only), **YARD Stick One** (sub-GHz). **Underrated:** Many commercial alarm systems use 433 MHz with no encryption — replay attacks are possible (authorized testing only).

### Step 66 — Satellite Internet OSINT
Satellite internet terminals (Starlink, ViaSat, HughesNet) have unique radio signatures. Satellite dish orientation can be used to determine geographic latitude. **Underrated:** Starlink dishes automatically point at satellites — the dish orientation can be photographed and analyzed to narrow down geographic location of a satellite internet user. Starlink terminal IP addresses can sometimes be geolocated more precisely than typical IP geolocation.

### Step 67 — Open-Source Radio Monitoring (Global)
Multiple online platforms archive radio transmissions:
- **RadioReference.com** — US scanner frequencies and trunking systems
- **Broadcastify** — Live and archived scanner audio
- **GlobalTuners** — Remote control of SDR receivers worldwide
- **WebSDR** — Access to software-defined radios around the world
- **Underrated:** University research SDRs with public access — some allow you to listen to local frequencies from countries you can't reach physically.

### Step 68 — SIGINT for Network Intelligence (Passive Network Monitoring)
On authorized networks, passive monitoring tools reveal tremendous intelligence:
- **Zeek (formerly Bro)**: Network behavior analysis
- **Wireshark**: Deep packet inspection of unencrypted traffic
- **NetworkMiner**: Passive network forensics tool
- **ngrep**: Network grep for pattern matching in traffic
- **Underrated:** Passive DNS monitoring on your own network reveals all DNS queries made — can identify shadow IT, personal device usage, and data exfiltration patterns.

### Step 69 — Electromagnetic Emanations Intelligence (TEMPEST)
TEMPEST is the study of unintentional electromagnetic emissions from electronic devices. Classified origin, but concepts are public. Research: **Van Eck phreaking** (receiving CRT/LCD screen emissions to reconstruct displayed content), **power line analysis** (keystroke inference from power consumption). Academic paper: "Compromising Electromagnetic Emanations of Wired and Wireless Keyboards" (Vuagnoux & Pasini). Relevant for high-security physical assessments.

### Step 70 — Radio Frequency Identification (RFID) Intelligence
RFID is everywhere: access cards, passports, credit cards, inventory systems. Learn:
- **125 kHz** (LF): Old HID cards, animal tags — easily readable from 30cm
- **13.56 MHz** (HF): MIFARE Classic, NFC — readable at 10–20 cm
- **860–960 MHz** (UHF): Long-range supply chain RFID
- Tools: **Proxmark3** (the gold standard RFID research tool), **ACR122U** (NFC)
- **Underrated:** Reading RFID tags from everyday items in a target's photo (Amazon packaging in background) reveals shipping labels, addresses, and identifiers.

### Step 71 — Acoustic Intelligence (ACOUSTINT)
Sound carries intelligence. Techniques:
- **Laser microphone** concepts: glass vibration from sound inside a room
- **Ultrasonic device tracking:** Some websites and apps use ultrasonic beacons for cross-device tracking
- **Background noise analysis:** Keyboard sounds can reveal what's being typed (acoustic emanation attacks — see Cambridge research)
- **Underrated:** Restaurant background noise in a phone call recording can be matched to specific restaurants by their music, noise profile, and ambient sounds

### Step 72 — Signals Analysis with GNU Radio
GNU Radio is a free & open-source SDR framework for building signal processing systems. It allows you to: decode custom protocols, analyze unknown signals, build signal intelligence workflows. Learn the **GNU Radio Companion** (graphical flowgraph tool). **Underrated for OSINT:** Decode proprietary IoT device signals to understand what data they transmit — relevant for smart home/smart office security assessments.

### Step 73 — Internet-of-Things (IoT) Signal Intelligence
IoT devices use diverse radio protocols: Zigbee, Z-Wave, LoRa, Sigfox, DECT, 6LoWPAN. Learning to receive and analyze these reveals significant intelligence in smart building environments. Tools: **YARD Stick One** for 433/915 MHz, **HackRF** for broadband analysis, **Zigbee sniffers** (Texas Instruments CC2531 dongle). Smart building systems (HVAC, lighting, access control) often transmit without encryption.

### Step 74 — SIGINT Documentation & Reporting
Build a signal intelligence log format:
- Frequency
- Signal type (AM/FM/SSB/Digital)
- Protocol (if identified)
- Location of receiver
- Timestamp (UTC)
- Content summary (for legal transmissions)
- Significance assessment
- Follow-up collection requirements

### Step 75 — Build an SDR SIGINT Lab
Recommended hardware collection for a serious SIGINT analyst:
- **RTL-SDR Blog V3** ($30) — General reception
- **HackRF One** ($300) — 1 MHz–6 GHz, transmit + receive (with authorization)
- **Proxmark3 RDV4** ($400) — RFID/NFC
- **YARD Stick One** (~$100) — Sub-GHz
- **Ubertooth One** ($120) — Bluetooth research
- Various antennas (dipole, yagi, discone)
- Total investment: ~$1,000 for serious capability

---

## PHASE 4 — GEOINT: GEOSPATIAL INTELLIGENCE
*Steps 76–95 | "Every image has a location. Find it."*

---

### Step 76 — GEOINT Fundamentals
GEOINT is intelligence derived from imagery and geospatial data. Used by: military, intelligence agencies, journalists, OSINT investigators. Core concepts: coordinate systems (WGS84, UTM), map projections, satellite imagery interpretation, change detection. Primary consumer: Bellingcat (study ALL of their investigations — they are the gold standard of open-source GEOINT).

### Step 77 — Google Earth Pro Mastery
Google Earth Pro is free and one of the most powerful GEOINT tools available. Features: historical imagery (right-click → "See older images"), sun position simulation, 3D terrain, measuring tools, KML import/export. **Underrated:** Historical imagery slider reveals when structures were built, when vegetation changed, when new vehicles appeared. This has been used to date photos from unknown timeframes.

### Step 78 — Sentinel Hub & Satellite Imagery Sources
Free satellite imagery access:
- **Sentinel Hub** (Copernicus): 10m resolution, updated weekly, free
- **Planet Labs**: High-resolution (3m), commercial but has a free tier
- **NASA Worldview**: Multi-spectral imagery
- **USGS Earth Explorer**: Landsat imagery archive since 1972
- **Zoom.Earth**: Near-real-time satellite imagery
- **Underrated:** Sentinel-1 SAR (Synthetic Aperture Radar) imagery works through clouds and at night — shows ground features invisible to optical sensors

### Step 79 — Image Geolocation (The Bellingcat Method)
Geolocating images is a core GEOINT skill. Method:
1. Extract all visible clues: signage language, architecture style, vegetation, sky color
2. Identify unique features: distinctive buildings, road markings, utility infrastructure
3. Cross-reference with satellite imagery
4. Use **SunCalc.org** for shadow analysis to confirm direction
5. Use **GeoSpy.ai** (AI-powered geolocation)
6. Practice on **GeoGuessr** — seriously improves geolocation intuition

### Step 80 — Chronolocation (Time Dating of Images)
When was a photo taken? Method:
- **Shadow analysis:** Sun position → time of day (use SunCalc)
- **Seasonal indicators:** Foliage state, snow cover, daylight duration
- **Lunar phase:** Moon visible in image → use moonphase calculator
- **Event correlation:** Events visible in image (construction stages, vehicle models)
- **Metadata:** EXIF data if not stripped (but rarely reliable for public images)
- Tool: **Suncalc.org** + **Google Earth** historical imagery

### Step 81 — OpenStreetMap for OSINT
OpenStreetMap (OSM) is crowdsourced and contains details that Google Maps often doesn't: building layouts, indoor maps, hiking trails, rural infrastructure. Tools: **Overpass Turbo** (query OSM data with custom searches), **osmand**, **JOSM**. **Underrated:** OSM edits are timestamped and attributed — you can see when a building was added to the map and by whom, revealing when construction was completed.

### Step 82 — Google Street View OSINT
Street View is time-stamped imagery at street level. Techniques:
- Use the clock icon to access historical Street View (back to ~2007)
- Check if a business exists / when it opened (street view date vs. first appearance)
- Look for security cameras, access points, badge readers in publicly visible areas
- **Underrated:** Street View interior imagery exists for many businesses — see inside restaurants, hotels, museums for physical security assessment

### Step 83 — Wikimapia & Yandex Maps Intelligence
**Wikimapia** is crowdsourced place annotation — users have tagged military bases, restricted areas, and sensitive facilities that don't appear on Google Maps. Check any sensitive site on Wikimapia for user-contributed intelligence. **Yandex Maps** has strong coverage of Russia, Central Asia, and Eastern Europe, often with more detail than Google for these regions.

### Step 84 — Vehicle & Aircraft Tracking
Track vehicles and aircraft with public data:
- **Flightradar24** / **ADS-B Exchange** — real-time aircraft (ADS-B Exchange shows unfiltered data)
- **MarineTraffic** — ship tracking via AIS
- **Strava heatmap** — shows where cyclists and runners travel (revealed secret military base locations in 2018)
- **Google Maps Timeline** — if you have authorized access to a target's Google account data
- License plate databases (ALPR data): **OpenALPR** (open source), state DMV records (legal inquiry process)

### Step 85 — Satellite Change Detection
Identifying what changed between two satellite images is powerful intelligence:
- Before/after comparison for physical security assessment
- Detecting construction, equipment movement, personnel activity
- Tools: **Sentinel Hub EO Browser** (free change detection), **Planet Labs** (commercial)
- **Underrated:** Use **Normalized Difference Vegetation Index (NDVI)** imagery to detect underground construction (soil disturbance changes vegetation patterns)

### Step 86 — Light Pollution & Night Imagery
VIIRS satellite data captures nighttime light emissions. Uses:
- Map populated areas and industrial activity
- Detect unreported facilities (unexpected light sources)
- Identify power outages (for disaster/conflict intelligence)
- **Source:** NASA Black Marble / VIIRS Day/Night Band imagery — free
- **Underrated:** Flares from oil fields and gas wells are visible from space — correlate with oil company production claims

### Step 87 — 3D Terrain Analysis for Physical Intelligence
Understanding terrain gives you physical security insight:
- **Google Earth 3D**: Visualize line of sight, chokepoints, approach vectors
- **SRTM terrain data**: Free 30m resolution elevation data worldwide
- **Relief visualization:** Shadow hillshade maps reveal terrain that flat maps hide
- For red team physical assessments: use terrain analysis to plan approach routes, identify blind spots in surveillance coverage

### Step 88 — Maritime & Port Intelligence
Ports and shipping lanes contain rich intelligence:
- **Import Genius / Panjiva**: US import records (public) reveal supplier relationships
- **ImportYeti** (free): US customs import data
- Container tracking via carrier websites reveals supply chain
- **Underrated:** Bill of Lading databases contain shipper name, consignee name, cargo description, weight, origin, and destination — all public record in the US

### Step 89 — Infrastructure Mapping (Power Grid, Pipeline, Telecom)
Critical infrastructure is often mapped in public databases:
- **EIA.gov**: US energy infrastructure, power plants, pipelines, transmission lines
- **FCC Antenna Search**: Tower locations, frequencies, owner information
- **OpenInfraMap**: Global pipeline, power line, and telecom infrastructure (from OSM)
- **Underrated:** FCC license database reveals exact antenna locations, frequencies, and authorized power levels for every licensed transmitter in the US

### Step 90 — Fire & Environmental Intelligence
Environmental events have intelligence value:
- **NASA FIRMS**: Active fire data updated within hours
- **Global Forest Watch**: Deforestation detection
- **Windy.com**: Real-time wind, precipitation, and ocean current data
- **Copernicus Emergency Management Service**: Satellite damage assessment after disasters
- **Underrated:** Wildfire smoke plumes visible in satellite imagery can be tracked to determine wind direction and speed — useful for chemical incident analysis

### Step 91 — Photogrammetry & 3D Reconstruction
Using multiple photos to reconstruct a 3D model of a location:
- Tools: **Meshroom** (free), **Agisoft Metashape** (commercial)
- Technique: Use photos from different angles (Google Street View, social media) to create a 3D model of a target building
- **Real-world application:** Create a 3D walkthrough of a target facility from publicly available images before a physical red team assessment

### Step 92 — Drone Intelligence (Commercial Drone Data)
Drones are increasingly used for infrastructure inspection, agriculture, and deliveries:
- **DroneBase** / **Hangar**: Commercial drone data marketplaces
- **FAA LAANC**: Drone authorization data reveals where drones are flying
- **Underrated:** Drone inspection footage of bridges, power lines, and buildings is sometimes uploaded publicly on YouTube and Vimeo — provides aerial intelligence of sensitive infrastructure

### Step 93 — GEOINT Analysis: Camouflage & Deception Detection
State actors and facilities use camouflage. Learn to detect it:
- Unusual straight lines in natural environments (tent ridgelines, vehicle covers)
- Uniform vegetation color (painted camouflage netting)
- Shadow inconsistencies (objects covered but casting shadows)
- Thermal signatures (heat from engines/equipment visible on IR imagery)
- **Study:** IHS Markit / Maxar intelligence reports on North Korean nuclear facilities for camouflage detection examples

### Step 94 — Build a GEOINT Collection Plan
Before conducting GEOINT analysis, build a collection plan:
1. Define the intelligence question
2. Identify target location (known coordinates or search area)
3. Select imagery sources (free vs commercial)
4. Define time period of interest
5. Identify what indicators to look for
6. Document findings with UTM/lat-long coordinates, image timestamps, source

### Step 95 — GEOINT Reporting Standards
GEOINT reports follow military/intelligence community standards:
- **Date-Time-Group (DTG)** for all imagery timestamps
- **Source & method** for all imagery (satellite, aerial, street level)
- **Confidence assessment** for every finding (confirmed, probable, possible)
- **Map products** with scale bars and north arrows
- **Change detection tables**: Before → After comparisons with dates

---

## PHASE 5 — TECHINT / CYBINT: TECHNICAL INTELLIGENCE
*Steps 96–115 | "Technology always leaves traces."*

---

### Step 96 — Network Architecture Reconnaissance
Map a target's network architecture from external sources:
- **BGP Routing Tables:** `bgp.he.net` — ASN ownership, IP ranges, peering relationships
- **Traceroute analysis:** Reveals intermediate network hops and hosting relationships
- **Shodan + Censys:** Combined gives 90%+ of externally visible infrastructure
- **Hurricane Electric BGP Toolkit** — free, comprehensive ASN/prefix data
- **Underrated:** BGP hijacking history (BGPMON) reveals when a target's IP space was accidentally or deliberately misrouted

### Step 97 — Web Technology Fingerprinting
Identify technology stacks without active scanning:
- **BuiltWith.com** — passive technology profiling of any website
- **WhatCMS.org** — identify CMS from public signals
- **Wappalyzer** — browser extension for tech detection
- **Netcraft** — hosting history and tech profiling
- **SecurityHeaders.io** — reveals misconfigurations from HTTP headers
- **Underrated:** HTTP response headers often reveal backend language, framework version, and server type — critical for vulnerability assessment

### Step 98 — SSL/TLS Certificate Intelligence
Certificates reveal information beyond basic validation:
- Organization name, department, location (in older certificates)
- Subject Alternative Names (SANs) — reveals all domains on one certificate
- Certificate pinning detection
- Tools: **crt.sh**, **Censys certificate search**, **SSLScan**, **testssl.sh**
- **Underrated:** Self-signed certificates on internal services sometimes appear in CT logs — revealing internal infrastructure names

### Step 99 — Email Header Analysis & Mail Server Intelligence
Email headers are a TECHINT goldmine:
- **X-Originating-IP**: Real sender IP (often exposed in older email systems)
- **Received:** chain reveals all mail servers the email passed through
- **DKIM signature**: Reveals signing domain (may differ from displayed sender)
- **SPF/DKIM/DMARC analysis**: Reveals email authentication posture (misconfiguration = spoofing possible)
- Tools: **MXToolbox Email Header Analyzer**, **Google Admin Toolbox**

### Step 100 — Dark Pattern Detection in Web Applications
Sophisticated TECHINT includes identifying deceptive UI patterns:
- Tracking pixel detection (1x1 invisible images that report back opens/views)
- Browser fingerprinting scripts embedded in web pages
- Third-party data brokers embedded as JavaScript
- **Tools:** **Privacy Badger**, **uBlock Origin** — analyze network requests made by target websites
- Map all third-party connections a website makes → reveals business relationships, ad networks, analytics providers

### Step 101 — Mobile App Intelligence (APK/IPA Analysis)
Mobile apps leak intelligence before they even run:
- Download APK from **APKPure**, **APKMirror** without installing
- Decompile with **jadx**, **apktool**, **MobSF** (Mobile Security Framework)
- Find: hardcoded API keys, backend server URLs, analytics endpoints, employee email addresses, S3 buckets
- **Underrated:** `strings` command on an APK reveals hardcoded secrets that developers forgot to remove before release

### Step 102 — API Endpoint Discovery
Modern applications expose APIs that often have less security than web interfaces:
- Discover APIs via: **Postman API Network**, **RapidAPI**, **APIs.guru**
- Find undocumented APIs via: JavaScript file analysis, mobile app decompilation, Burp Suite passive scanning
- **Google dork:** `site:target.com inurl:api`
- Test for: unauthenticated access, excessive data exposure (OWASP API Security Top 10)
- **Underrated:** Developer documentation portals (`docs.target.com`, `developer.target.com`) reveal API endpoints that aren't linked from the main site

### Step 103 — Source Code Intelligence
Beyond GitHub: find code in unexpected places:
- **Grep.app** — searches across millions of GitHub repos simultaneously
- **PublicWWW** — searches HTML/JS source code of millions of websites
- **Source Code Search Engine (searchcode.com)** — searches multiple code platforms
- **NerdyData** — HTML source code search engine
- **Underrated:** `npm` package names can be squatted — check if target's internal npm packages have public versions

### Step 104 — DNS Zone Transfer Attempts
While modern servers block zone transfers, testing is still worthwhile:
```bash
dig axfr @ns1.target.com target.com
```
Also: **DNS brute-forcing** with wordlists (`dnsrecon`, `fierce`, `Sublist3r`). Subdomain permutation: `dev`, `staging`, `test`, `admin`, `api`, `vpn`, `mail`, `webmail`, `remote`, `portal` — these are almost always present and often less secure than production.

### Step 105 — Web Application Fingerprinting & Error Page Intelligence
Error pages reveal version information:
- Default error pages (Apache 404, nginx 500, IIS 403) reveal exact server version
- **Stack traces** expose: programming language, framework, database, file paths, internal hostnames
- **Debug mode left on:** Exposes environment variables, configuration, and code
- **Underrated:** Access `/robots.txt` — directories disallowed from crawling often contain the most sensitive paths (`/admin`, `/backup`, `/api/v1`)

### Step 106 — IoT Device Intelligence via Shodan
IoT OSINT focuses on finding:
- Default credentials (Google: `[device model] default password`)
- Exposed admin panels
- Publicly visible webcams (**Insecam.org** aggregates them)
- Industrial control systems (SCADA/ICS exposed on internet)
- Medical devices on public internet
- **Critical Shodan searches:**
  ```
  port:502 (Modbus — industrial protocol)
  port:102 (Siemens S7 — industrial PLC)
  "Server: yawcam" has_screenshot:true
  ```

### Step 107 — Cloud Infrastructure Intelligence
Cloud environments are often misconfigured. Beyond S3 buckets:
- **Azure Storage:** `target.blob.core.windows.net/container?restype=container&comp=list`
- **GCP Storage:** `storage.googleapis.com/[bucket-name]`
- **Firebase databases:** `[project-id]-default-rtdb.firebaseio.com/.json`
- **Elasticsearch clusters:** Shodan query: `port:9200 "cluster_name"`
- **Kibana dashboards:** `port:5601 "kibana"` — Kibana without auth exposes log data
- **Underrated:** AWS metadata service — if you ever have SSRF on an AWS-hosted app, `169.254.169.254` gives you instance credentials

### Step 108 — Supply Chain Intelligence
A target's supply chain is their weakest link:
- Identify vendors from: job postings (requires experience with Vendor X), SEC filings (material contracts), employee LinkedIn posts
- Map software supply chain: `package.json`, `requirements.txt`, `go.mod` in public repos
- Check if vendors have public vulnerabilities (Shodan + vendor product searches)
- **SolarWinds-style intelligence:** Find common software across many targets — a single vendor breach can cascade

### Step 109 — Vulnerability Intelligence Integration
Correlate infrastructure findings with vulnerability databases:
- **CVE/NVD (nvd.nist.gov)** — authoritative CVE database
- **Exploit-DB** — public exploits
- **Vulhub** — Docker environments for vulnerability testing
- **Shodan CVEs filter:** `vuln:CVE-2021-44228` (Log4j)
- **GreyNoise** — tells you if an IP is scanning the internet (threat intelligence)
- **Underrated:** **VulnDB** correlation with Shodan data can identify targets running software with known critical vulnerabilities before they're patched

### Step 110 — Container & Orchestration Intelligence
Kubernetes and Docker misconfigurations are epidemic:
- **Kubernetes API server:** `port:6443 "Kubernetes"` on Shodan — sometimes unauthenticated
- **Docker API:** `port:2375 "Docker"` — unauthenticated Docker daemons
- **Kubernetes dashboard:** `http.title:"Kubernetes Dashboard"` on Shodan
- **etcd:** `port:2379` on Shodan — Kubernetes' database, often exposed without auth
- These misconfigurations grant full cluster control

### Step 111 — Firmware Intelligence
IoT and embedded device firmware is often public:
- Download from manufacturer websites or **OpenWrt**, **FirmwareBinaries.com**
- Extract with **Binwalk**: `binwalk -e firmware.bin`
- Find: hardcoded credentials, private keys, backend URLs, encryption keys
- **Underrated:** Firmware changelog documents reveal what vulnerabilities were fixed — which means what vulnerabilities older versions still have

### Step 112 — Windows Active Directory OSINT (External)
Even without internal access, AD artifacts leak externally:
- **Azure AD / Entra ID enumeration:** `https://login.microsoftonline.com/target.com/v2.0/.well-known/openid-configuration` — reveals tenant information
- **Autodiscover endpoint:** `https://autodiscover.target.com/autodiscover/autodiscover.xml`
- **M365 user enumeration:** Tools like **o365creeper** test if email accounts exist in M365 tenant
- **Underrated:** Azure AD Connect servers, if internet-facing, reveal internal domain names

### Step 113 — Payment System & PCI Intelligence
Payment infrastructure is often exposed:
- Look for: payment gateway names in web source code, PCI compliance certificates (public), tokenization providers
- Third-party payment scripts in JavaScript reveals: payment provider, potential for Magecart attacks
- **Underrated:** PCI DSS compliance certificates issued by QSAs are sometimes published and reveal scoping information (which systems are in scope for PCI)

### Step 114 — Industrial Control System (ICS/SCADA) Intelligence
ICS/SCADA systems are increasingly internet-exposed. Shodan queries:
```
product:"Siemens S7-300"
"CODESYS Control"
"schneider-electric"
port:20000 "DNCS"
"GE Fanuc"
```
**Critical:** Do not attempt to access or interact with ICS systems without authorization — these control physical infrastructure and unauthorized access can cause physical harm.

### Step 115 — Build a TECHINT Infrastructure Map
After completing technical collection, build a comprehensive infrastructure map:
1. IP ranges (ASN analysis)
2. Domain and subdomain inventory
3. Technology stack by service
4. SSL/TLS certificate map
5. Cloud service providers used
6. Third-party vendor dependencies
7. Known vulnerabilities (correlate CVEs with identified technologies)
8. Exposure score (number of services × severity of exposure)

---

## PHASE 6 — FININT: FINANCIAL INTELLIGENCE
*Steps 116–135 | "Money always leaves a trail."*

---

### Step 116 — Financial Intelligence Fundamentals
FININT (Financial Intelligence) is the analysis of financial transactions, corporate structures, beneficial ownership, and money flows. Legitimate FININT uses: law enforcement (following illicit funds), corporate due diligence, investigative journalism, and red team reconnaissance (identifying target organization's financial health and relationships).

### Step 117 — SEC & Regulatory Filing OSINT
Public companies must disclose enormous amounts of information:
- **EDGAR (sec.gov/edgar)**: 10-K (annual), 10-Q (quarterly), 8-K (material events), DEF 14A (proxy — executive compensation)
- **Form 4**: Insider transactions — executives buying/selling stock
- **13D/13G filings**: Who owns 5%+ of the company
- **Underrated:** Read the *Risk Factors* section of 10-K filings — companies are legally required to disclose their security vulnerabilities, ongoing litigation, and operational weaknesses

### Step 118 — Corporate Registry Intelligence
Company registrations reveal ownership structure:
- **OpenCorporates.com** — 200M+ company records from 140 jurisdictions, free
- **Companies House (UK)** — director information, filing history, accounts
- **EDGAR** — US public companies
- **OffshoreLeaks** — ICIJ database of offshore entities (Panama Papers, Paradise Papers, Pandora Papers)
- **Beneficial Ownership Databases:** EU's AMLA registry (upcoming), UK PSC register

### Step 119 — Cryptocurrency & Blockchain Intelligence
Blockchain is a public ledger. Every transaction is permanently recorded and traceable:
- **Blockchain.com** — Bitcoin explorer
- **Etherscan.io** — Ethereum explorer
- **Chainalysis Reactor** (commercial) / **Breadcrumbs.app** (free) — transaction graph analysis
- **Technique:** Trace cryptocurrency wallets from known addresses (darknet markets, ransomware payments) to identify connected wallets
- **Underrated:** Transaction timing patterns and amounts can cluster wallets belonging to the same entity even across different coins

### Step 120 — Dark Pool & Alternative Trading System Intelligence
Beyond stock exchanges, alternative trading venues reveal institutional activity:
- FINRA **Trade Reporting Facility (TRF)** data — dark pool volume by security
- **SEC Form ATS**: Alternative Trading Systems must register with SEC
- **CFTC Large Trader Reports**: Who holds large commodity positions
- **Underrated:** Unusual option activity before announcements (SEC tracks this) can indicate insider knowledge — follow SEC enforcement actions for patterns

### Step 121 — Real Estate & Property Intelligence
Property records are public in most jurisdictions:
- **Zillow/Redfin** — home value, sale history, size
- **County assessor websites** — direct access to property tax records (free)
- **FINCEN Geographic Targeting Orders** — identifies anonymous shell company real estate purchases
- **Underrated:** **PACER** court records contain foreclosure filings that reveal property addresses and mortgage servicer details for any US property

### Step 122 — Grant & Contract Intelligence
Government spending data is public:
- **USASpending.gov** — all US federal contracts and grants
- **SAM.gov** — federal contractor registrations (includes NAICS codes, cage codes, points of contact)
- **FPDS (Federal Procurement Data System)** — detailed contract data
- **EU TED** — European public procurement database
- **Underrated:** A company's government contract history reveals their primary business activities, security clearance levels, and key personnel (contracting officers of record)

### Step 123 — Sanctions & AML Database Screening
Screening individuals and entities against sanctions lists:
- **OFAC SDN List** (US Treasury) — sanctions list
- **EU Consolidated Sanctions List**
- **UN Sanctions List**
- **World Bank Debarment** — firms banned from World Bank projects
- Tools: **OpenSanctions.org** — free, comprehensive, machine-readable sanctions database
- **Underrated:** Screening second and third-degree associates of sanctioned entities — compliance programs miss this

### Step 124 — Bankruptcy & Litigation Intelligence
Court filings are open records and reveal sensitive business information:
- **PACER** — all US federal court filings
- **State court systems** — most now have online access
- Bankruptcy filings (Chapter 11): reveal creditor lists, assets, liabilities, key contracts
- **Litigation discovery**: In civil cases, companies must disclose documents — these sometimes become public record
- **Underrated:** Expert witness lists in litigation reveal what technologies a company uses (the expert is hired because of their expertise with the company's specific systems)

### Step 125 — Import/Export Trade Intelligence
US customs data is public:
- **ImportGenius**, **ImportYeti** (free for some data), **Panjiva** (commercial)
- Bill of Lading data: shipper, consignee, cargo description, weight, origin
- Use to: map supply chains, identify vendor relationships, track commodity volumes
- **Underrated:** Correlate import data with SEC filings — discrepancies between stated revenue and import volume can indicate financial irregularities

### Step 126 — Lobbying & Political Contribution Intelligence
Corporate political activity is publicly disclosed:
- **OpenSecrets.org** — campaign finance and lobbying (US)
- **LobbyFacts.eu** — EU lobbying transparency
- **FARA (Foreign Agents Registration Act)** database — foreign influence activities
- Political contributions: **FEC.gov** (US), **Electoral Commission (UK)**
- **Underrated:** Correlate lobbying expenditure timing with regulatory decisions — identifies what policy outcomes a company is purchasing

### Step 127 — Insurance & Risk Intelligence
Insurance filings contain valuable intelligence:
- **EDGAR** 10-K filings disclose insurance coverage amounts
- State insurance commissioner databases (US) — policy data for regulated entities
- **Lloyd's of London** market intelligence — specialty insurance often covers sensitive risks
- **Underrated:** Workers' compensation claims data (public in many US states) reveals employee injuries, job classifications, and workplace hazards at specific locations

### Step 128 — M&A & Private Equity Intelligence
Corporate deals are goldmines of intelligence:
- **Crunchbase** — startup funding rounds, investors, acquisitions
- **PitchBook** (commercial) — comprehensive deal data
- **Mergermarket** (commercial) — M&A intelligence
- **SEC Form S-1** — IPO prospectus contains the most detailed company description required by law
- **Underrated:** Failed acquisition attempts (disclosed in SEC filings) reveal who wanted to buy a company and at what price — revealing competitive dynamics

### Step 129 — Financial Data APIs for OSINT Automation
Build automated financial intelligence collection:
- **SEC EDGAR API** — free, comprehensive
- **Alpha Vantage** — free financial data API
- **Quandl** — economic and financial data
- **OpenFIGI** — financial instrument identification
- Build Python scripts to monitor SEC filings for specific keywords and alert when they appear

### Step 130 — Beneficial Ownership & Shell Company Investigation
Following money through shell companies:
- **Pandora Papers / ICIJ OffshoreLeaks database** — free search
- **OpenCorporates API** — cross-jurisdictional corporate search
- **UK Companies House** — PSC (Persons of Significant Control) register
- **Delaware Secretary of State** — but provides minimal beneficial ownership data (notorious secrecy)
- **Underrated:** Nevada and Wyoming LLC registrations are the US equivalent of offshore secrecy — extremely limited public disclosure

### Step 131 — Dark Web Financial Intelligence (Legal Monitoring)
Monitor cryptocurrency-related criminal activity:
- **Bitcoin Abuse Database** — reported malicious Bitcoin addresses
- **Ransomwhere.org** — ransomware payment tracking
- **Chainabuse.com** — multi-chain abuse reports
- **CipherTrace / Chainalysis** blog posts — publicly released threat intelligence
- These resources are legitimate FININT for threat intelligence professionals

### Step 132 — Economic Sanctions Evasion Detection
Understanding how sanctions are evaded helps identify them:
- Flags of convenience (ships switching flags)
- Free trade zone transshipment
- Front company networks
- Virtual currency layering
- **Study:** US Treasury OFAC enforcement actions (free, detailed) and FINCEN advisories

### Step 133 — Insurance Fraud & Financial Crime Indicators
Red flags used by financial intelligence analysts:
- Round-number transactions
- Just-below-reporting-threshold structuring
- Rapid movement through multiple accounts
- Unusual geographic routing
- Mismatched business activity and transaction volume

### Step 134 — Corporate Intelligence Report Structure
A FININT report should include:
1. Entity identification (legal name, registration, jurisdictions)
2. Ownership structure (corporate chart, beneficial owners)
3. Financial health (revenue, debt, key financial ratios)
4. Key relationships (clients, vendors, investors, partners)
5. Litigation & regulatory history
6. Political & lobbying activity
7. Red flags (inconsistencies, sanction proximity, shell company indicators)
8. Source list with confidence ratings

### Step 135 — Integrate FININT with Other Intelligence Disciplines
FININT is most powerful when combined:
- **FININT + SOCINT**: Correlate executive spending patterns with social media posts
- **FININT + GEOINT**: Match property records with satellite imagery
- **FININT + TECHINT**: Correlate contract wins with infrastructure changes
- **FININT + OSINT**: Use financial triggers (funding rounds, contract awards) to time collection efforts

---

## PHASE 7 — DARKINT: DARK WEB & UNDERGROUND INTELLIGENCE
*Steps 136–155 | "Understanding the dark web is table stakes for modern threat intelligence."*

---

### Step 136 — Dark Web Architecture & Access
The dark web is a collection of networks not indexed by standard search engines:
- **Tor network**: .onion sites, accessible via Tor Browser
- **I2P (Invisible Internet Project)**: .i2p sites, different architecture
- **Freenet**: Censorship-resistant network
- **ZeroNet**: Blockchain-based
- **Safety first**: Always use Tor Browser in a VM (Whonix preferred), never with a standard OS browser

### Step 137 — Safe Dark Web Investigation Setup
For professional dark web OSINT:
1. Use **Whonix** (Gateway + Workstation VM pair) or **Tails OS**
2. No personal accounts, no personal devices
3. Dedicated hardware or isolated VM
4. VPN before Tor (for some threat models) or not (depends on operational requirement)
5. Take VM snapshots before each session
6. Never download files to your host machine
7. Use **Dangerzone** to safely open suspicious documents

### Step 138 — Dark Web Search Engines & Directories
Finding content on the dark web:
- **Ahmia.fi** — Tor search engine (clearnet-accessible)
- **DarkSearch** — dark web search engine
- **OnionSearch** — searches multiple dark web engines
- **Hidden Wiki variants** — directories (quality varies)
- **Torch** — oldest Tor search engine
- **Recon.cx** — dark web forum index

### Step 139 — Ransomware Intelligence (Threat Actor Tracking)
Ransomware groups maintain leak sites (DLS — Data Leak Sites) on Tor:
- Monitor DLS sites for new victims
- Track: timing patterns, victim industry, ransom amounts demanded, data exfiltration claims
- Legitimate monitoring tools: **RansomLook.io** (free), **Ransomwatch** (GitHub)
- Read threat intelligence reports from: **Mandiant**, **CrowdStrike**, **Recorded Future**, **Sophos**
- **Underrated:** Ransomware negotiation chat logs sometimes get leaked — revealing attacker TTPs, internal argument about targets, and operational security failures

### Step 140 — Dark Web Forum Intelligence
Underground forums are where cybercriminals trade tools, techniques, and data:
- **Types:** General cybercrime (cracking, carding), nation-state sponsored, specialized (malware-as-a-service)
- **Historically significant forums:** RaidForums (seized), BreachForums (seized), Exploit.in, XSS.is
- **Collection approach:** Monitor via RSS/atom feeds where available, use Ahmia searches
- **Underrated:** Forum registration dates and posting history of usernames can be correlated with surface web activity

### Step 141 — Credential Market Intelligence
Stolen credentials are sold on dark web markets:
- **Current markets:** Genesis Market (seized), 2easy, Russian Market
- **Monitor without purchasing:** Screenshot and document listings for threat intelligence
- Use **Have I Been Pwned API** for legitimate credential exposure monitoring
- **Legal note:** Purchasing stolen credentials is illegal — collection is for intelligence purposes only

### Step 142 — Malware Repository Intelligence
Malware samples are shared in dark web communities and legitimate threat intelligence platforms:
- **VirusTotal** — upload/search malware samples (legal, used by security community)
- **MalwareBazaar** — free malware sample database (abuse.ch)
- **ANY.RUN** — interactive malware sandbox
- **Hybrid Analysis** — free sandbox analysis
- Analyze: C2 server domains/IPs, mutex names, file paths, registry keys — each is an IOC (Indicator of Compromise)

### Step 143 — Threat Actor Attribution (OSINT Approach)
Attribution is hard. Good attribution follows a structured process:
1. **Technical indicators**: Malware code reuse, infrastructure overlap, TTPs (MITRE ATT&CK)
2. **Linguistic analysis**: Language in malware, ransom notes, forum posts
3. **Operational patterns**: Time zones (active hours), targeting preferences, preferred industries
4. **OSINT correlation**: Connect handles across forums, telegram, surface web
5. **Critical caution:** Attribution confidence levels matter — state "low/medium/high confidence" explicitly

### Step 144 — MITRE ATT&CK Framework Mastery
ATT&CK is the universal language of offensive cyber operations. Every OSINT professional must know it:
- **Tactics (Why):** Reconnaissance, Initial Access, Execution, Persistence, Privilege Escalation, Defense Evasion, Credential Access, Discovery, Lateral Movement, Collection, Exfiltration, Command and Control
- **Techniques (How):** Specific methods for each tactic
- Use ATT&CK Navigator to map threat actor TTPs
- **Underrated:** ATT&CK for ICS and ATT&CK for Mobile are separate matrices — know all three

### Step 145 — Dark Web Data Leak Analysis
When breach data appears on dark web, analyze it for intelligence:
- Identify affected accounts (email domains)
- Assess sensitivity (credentials, PII, financial data)
- Look for metadata: database structure reveals system type, timestamps reveal collection method
- **Critical skill:** Determine if a posted dataset is real or fabricated (compare sample records against known public data)

### Step 146 — Carding & Financial Fraud Forum Intelligence
Carding forums trade stolen payment card data:
- Understand the ecosystem: dumps (full track data), CVV shops, fullz (complete identity packages)
- **Do not purchase** — observe and document for threat intelligence
- Correlate card BINs (Bank Identification Numbers) with issuing bank to assess exposure
- Use **BINList.net** for BIN lookup

### Step 147 — Darknet Market Intelligence
Darknet markets trade in illegal goods but their existence and operation are intelligence:
- Monitor for: new markets, exit scams, law enforcement seizures
- Track market longevity (average market lifespan: 8–12 months)
- Read: Europol/DEA press releases for takedown intelligence (free, public)
- **Underrated:** Market seizure affidavits (public court documents) contain detailed investigation methodology — valuable for understanding how law enforcement tracks dark web activity

### Step 148 — Paste Site & Leak Monitoring (Advanced)
Beyond Pastebin, monitor sophisticated leak channels:
- **Anonfiles / Mega.nz / Gofile** — file hosting services used for data dumps
- **Telegram leak channels** — often more current than dark web forums
- **Matrix chat rooms** — increasingly used by threat actors
- Set up automated monitoring with: **PasteHunter**, custom RSS monitoring, **IFTTT webhooks**
- **Underrated:** Many leaks are first shared on Discord servers before hitting dark web — monitor relevant Discord servers

### Step 149 — Operational Security Analysis of Threat Actors
Study how threat actors fail at OpSec — it's where attribution happens:
- Reused infrastructure (same IP/domain across campaigns)
- Consistent naming conventions in malware
- Operational timing patterns revealing time zone
- Language/locale artifacts in malware (error messages, keyboard layout artifacts)
- Accidental personal account connections
- **Study:** Bellingcat's attribution of GRU officers via OSINT is the gold standard

### Step 150 — Nation-State Threat Intelligence (OSINT Sources)
Tracking state-sponsored cyber actors via public sources:
- **US Cyber Command/NSA public disclosures** — malware samples with attribution
- **CISA Advisories** — technical indicators for nation-state TTPs
- **Recorded Future**, **CrowdStrike**, **Mandiant** public reports (free tier)
- **Underrated:** Taiwan CERT, Japan NISC, South Korean KISA publish detailed threat intelligence rarely covered by Western media

### Step 151 — Dark Web Infrastructure Investigation
Tracing dark web infrastructure:
- .onion addresses are base32-encoded public keys — no IP information
- But: operational failures leak infrastructure (server misconfigurations, TLS certificate on both clearnet and .onion)
- **Technique:** Search Shodan and Censys for HTTP response headers unique to dark web services
- Historical DNS: if a .onion service ever had a clearnet mirror, find it via Wayback Machine

### Step 152 — Hacktivist & Social Movement Intelligence
Hacktivist groups announce operations publicly:
- **Twitter/X, Telegram**: Operation announcements (#OpXXX)
- **Pastebin**: Target lists, proof of access
- **Underrated sources**: **AnonOps IRC** (still active), local political forums in target countries
- Track: claimed victims, methodologies, political motivations

### Step 153 — Dark Web Persona Management for Intelligence Collection
Collecting intelligence from underground forums requires established personas:
- Build reputation by contributing (never contribute illegal content)
- Match persona to forum culture (language, interests, technical level)
- Compartmentalize personas — never let them cross-pollinate
- Use separate hardware, browsers, and connectivity for each persona
- Document all interactions for potential evidence purposes (even in authorized operations)

### Step 154 — Counter-Intelligence Awareness in Dark Web Operations
Threat actors run counter-intelligence operations:
- Honeypot forums designed to deanonymize researchers
- Fake "researcher" accounts gathering intelligence from real researchers
- False flag leak postings to misdirect attribution
- **Always verify**: Is this forum/channel/persona what it claims to be?

### Step 155 — Synthesize DARKINT into Actionable Intelligence
Dark web intelligence is only valuable when actioned:
- **CTI (Cyber Threat Intelligence) report format**: Executive summary, technical indicators, MITRE ATT&CK mapping, recommended mitigations
- Share through: ISACs (Information Sharing and Analysis Centers), US-CERT, partner organizations
- **STIX/TAXII format**: Machine-readable threat intelligence sharing standard
- Tools: **MISP** (Malware Information Sharing Platform) — open source CTI platform

---

## PHASE 8 — ADVANCED RED TEAM OSINT OPERATIONS
*Steps 156–170 | "Intelligence is the force multiplier for every red team operation."*

---

### Step 156 — Red Team OSINT vs Penetration Testing OSINT
Red team OSINT is different from pentest recon:
- **Pentest:** Find vulnerabilities to demonstrate exploitability
- **Red team:** Collect intelligence to simulate a real threat actor — stealth, persistence, and realistic TTPs matter
- Red team OSINT must be completely passive until authorized active testing begins
- Red team intelligence informs: phishing lure design, physical access approach, social engineering pretext, technical entry vectors

### Step 157 — Building a Target Profile (Intelligence Dossier)
Every red team operation begins with a comprehensive target dossier:
- **Organization profile**: Size, locations, subsidiaries, leadership, technology stack
- **Perimeter map**: External IP ranges, domains, cloud services, exposed services
- **Personnel profile**: Key targets, department structure, security team members
- **Physical security assessment**: Visible security measures, access control, guard schedules
- **Technology intelligence**: Products used, versions, known vulnerabilities
- **Relationship intelligence**: Key vendors, partners (potential supply chain attack vectors)

### Step 158 — Pretext Development from OSINT
Use collected intelligence to build convincing social engineering pretexts:
- **IT vendor impersonation**: Identify actual vendors from job postings + LinkedIn → "I'm calling from Cisco TAC about your ASA firmware"
- **Recruiter pretext**: LinkedIn shows employee is looking for new job → approach with job offer (builds rapport for information extraction)
- **Internal employee pretext**: Know the organization's internal terminology from breach data / job postings
- **Physical pretext**: Uniform of delivery company that actually delivers to target (verify via surveillance)

### Step 159 — Phishing Campaign Intelligence
Effective phishing requires deep target knowledge:
- **Identify email platform**: O365, Google Workspace, or on-premise Exchange
- **Map email format** from breach data or HunterIO
- **Collect employee names** for targeting
- **Identify internal systems** referenced in job postings (VPN product, ticketing system, HR platform)
- **Find recent events** (merger, product launch, security incident) to use as lure theme
- **Underrated:** Find the ticketing system name (ServiceNow, Jira, Zendesk) — a fake ticket notification is highly convincing

### Step 160 — Physical Security OSINT (Pre-Assessment)
Before physical red team assessment:
- **Google Maps/Street View**: Identify entry points, parking, vehicle access
- **Employee social media**: Badge photos (reveals card type + reader brand), office interior photos
- **Glassdoor**: Employee complaints about security (broken locks, unmonitored areas, no ID checking)
- **Job postings**: Security guard company name, shift schedules implied by "nights and weekends" requirements
- **LinkedIn**: Security team size and experience level
- **OSINT of CCTV cameras**: Street-level visible camera positions from Street View

### Step 161 — Voice Intelligence & Social Engineering Preparation
Pre-call intelligence collection for vishing (voice phishing):
- **Name pronunciation**: Check LinkedIn audio feature or YouTube appearances
- **Organizational vocabulary**: Industry-specific terms, internal product names from job postings
- **Recent company events**: Earnings calls (public for listed companies), press releases
- **Target's timezone and schedule**: Social media posting patterns
- Practice: Know the target's holding music, IVR options (call as a customer first)

### Step 162 — Insider Threat Intelligence
OSINT can identify potential insider threat indicators:
- Sudden financial distress (public bankruptcy, foreclosure records)
- Recent job searches (LinkedIn "Open to Work", activity on job boards)
- Expressions of grievance on social media
- Recent disciplinary actions (mentioned in litigation, Glassdoor reviews)
- **Underrated:** Employees who recently lost a promotion (visible from LinkedIn title changes) are statistically more receptive to recruitment

### Step 163 — Supply Chain Attack Reconnaissance
Identify the softest supply chain targets:
- **Map software dependencies**: Public GitHub repos, package.json files
- **Identify software update mechanisms**: Does the target auto-update? (Pivots to vendor compromise)
- **Physical supply chain**: Identify hardware vendors from job postings + procurement databases
- **Managed service providers**: IT MSPs are high-value targets (compromise MSP → access all clients)
- **Underrated:** Look for the HR/payroll software vendor — ADP, Paychex breaches affect all clients

### Step 164 — Password Spraying Intelligence Preparation
Before any authorized password spraying:
- **Identify email format**: Required for username construction
- **Identify authentication platform**: O365, Okta, Azure AD, on-premise AD
- **Collect employee names** for target list
- **Research password policies**: Job posting "experience with enforcing password policies" or Glassdoor "forced to change password every 30 days"
- **Identify MFA solution**: "Experience with Duo Security" in job postings
- **Build custom wordlist**: Organization name, location, founding year, products, sports teams, seasonal patterns

### Step 165 — Cloud Attack Surface Reconnaissance
Cloud environments have unique OSINT attack surfaces:
- **Azure AD tenant enumeration**: `login.microsoftonline.com/getuserrealm.srf?login=user@target.com`
- **AWS account ID discovery**: Error messages in misconfigured S3 buckets contain Account IDs
- **GCP project enumeration**: Error responses reveal project names
- **Snowflake/data warehouse exposure**: Search for BI tools (Tableau, Power BI) connecting to cloud data warehouses
- **Underrated:** Terraform state files committed to GitHub often contain active cloud credentials and full infrastructure state

### Step 166 — Competitive Intelligence for Red Team Context
Understanding who the real threat actors are for your target:
- What industry? → What threat groups target that industry? (MITRE ATT&CK Groups by sector)
- What geopolitical exposure? → Nation-state actors with interest in that region
- What data do they hold? → What criminal groups would value that data type
- **Simulate the realistic adversary**, not a generic attacker — this is what separates red teams from pentesters

### Step 167 — Exfiltration Channel Reconnaissance
Identify how data could be exfiltrated from target before the assessment:
- **Outbound filtering**: Do they block Dropbox, Google Drive, OneDrive, Mega?
- **DNS monitoring**: Do they have DNS security (Cisco Umbrella, Cloudflare Gateway)?
- **Email DLP**: What email security gateway do they use?
- **DLP solutions**: Are they using Symantec, Forcepoint, Microsoft Purview?
- Find answers from: job postings, LinkedIn employee profiles (security team skills), Shodan/BuiltWith headers

### Step 168 — Post-Engagement OSINT Validation
After a red team engagement, use OSINT to validate findings:
- Verify that vulnerabilities you found are not already publicly known/exploited
- Check if credentials you found in breach data are still active (test only with authorization)
- Validate that exposed services you found are truly external (not behind VPN)
- Use OSINT to contextualize risk: Is the exposed database being actively scanned by threat actors? (GreyNoise data)

### Step 169 — Red Team Report Writing with OSINT Evidence
Intelligence-grade red team reports use OSINT to demonstrate real-world risk:
- Show that each vulnerability was found using the same methods a real attacker would use
- Include "how we found this" sections — demonstrates realistic attack path
- Use OSINT to establish threat actor context: "Ransomware group X has targeted similar companies using this initial access vector"
- Cite breach data showing target's employees' credentials are already for sale

### Step 170 — Continuous OSINT Monitoring (Persistent Intelligence)
Intelligence is not a one-time event:
- Set up **Google Alerts** for target name + "breach", "data leak", "incident"
- **Shodan Monitoring** — alerts when new services appear on target's IP ranges
- **Certificate Transparency monitoring** (crt.sh RSS feeds) — new subdomains appear
- **GitHub monitoring** — watch target organization for new repository commits
- **Dark web monitoring** — automated alerts when target domain appears in underground forums

---

## PHASE 9 — AI-AUGMENTED INTELLIGENCE
*Steps 171–185 | "AI is not replacing analysts. Analysts using AI are replacing analysts who don't."*

---

### Step 171 — AI as an Intelligence Force Multiplier
AI fundamentally changes OSINT scale. Tasks that took hours now take minutes:
- **Text analysis at scale**: Processing thousands of social media posts for patterns
- **Image analysis**: Automated geolocation, object detection in photos
- **Translation**: Breaking language barriers in intelligence collection
- **Entity extraction**: Named entity recognition (NER) from unstructured text
- **Link analysis**: Automated relationship mapping from large datasets

### Step 172 — LLM-Assisted OSINT (ChatGPT, Claude, Gemini)
Using large language models for OSINT tasks:
- **Query refinement**: LLMs generate better search queries from vague intelligence requirements
- **Report drafting**: Transform raw notes into structured intelligence reports
- **Summarization**: Process long documents quickly
- **Code generation**: Write custom OSINT scripts on demand
- **Critical limitation**: LLMs hallucinate — never use LLM output as factual intelligence without verification

### Step 173 — AI-Powered Image Analysis
Computer vision for OSINT:
- **GeoSpy.ai** — AI geolocation of photos
- **Amazon Rekognition** — object, scene, and face detection API
- **Google Vision API** — text extraction, landmark detection, label detection
- **Tesseract OCR** — extract text from images (license plates, signs, documents)
- **FaceCheck.ID / PimEyes** — facial recognition search engines
- **CLIP (OpenAI)** — zero-shot image classification for custom categories

### Step 174 — Whisper AI for Audio Intelligence
OpenAI's Whisper is a free, highly accurate speech-to-text model:
- Transcribe podcasts, videos, interview recordings, court hearings
- Supports 99 languages — process non-English audio automatically
- Create searchable transcripts of entire YouTube channels
- **Underrated:** Run Whisper on years of executive speeches to build a comprehensive intelligence database of a leader's stated positions, contradictions, and priorities

### Step 175 — Natural Language Processing for OSINT
NLP tools extract intelligence from text at scale:
- **SpaCy**: Named entity recognition — extracts names, organizations, locations from text
- **VADER / BERT-based sentiment**: Emotional analysis of social content
- **Topic modeling (LDA)**: Identify themes in large document collections
- **Graph of entity relationships**: Build knowledge graphs from news articles automatically
- **Tool:** **NLTK**, **HuggingFace Transformers** — build custom OSINT NLP pipelines

### Step 176 — AI-Powered Link Analysis
Traditional link analysis tools struggle with scale. AI improves this:
- **Neo4j + GPT**: Natural language queries on relationship graphs
- **Palantir Gotham** (enterprise) — AI-augmented link analysis
- **Open alternative:** **Gephi** + Python NLP pipeline for automated relationship mapping
- **Underrated:** Use community detection algorithms (Louvain, Girvan-Newman) on social network data to automatically identify clusters and key nodes

### Step 177 — Automated OSINT Pipelines
Build automated intelligence collection and processing:
```python
# Example pipeline concept
1. Shodan API → collect target infrastructure
2. crt.sh API → collect all subdomains
3. theHarvester → collect emails
4. HaveIBeenPwned API → check breach exposure
5. SpaCy NER → extract entities from results
6. Neo4j → store relationships
7. GPT-4 API → generate analysis summary
```
Tools: **n8n** (workflow automation), **Apache Airflow** (data pipelines), **Python** with requests/BeautifulSoup

### Step 178 — AI-Generated Fake Detection
AI generates convincing fake content — intelligence analysts must detect it:
- **Deepfake video detection**: **FakeCatcher** (Intel), **Sensity**, **Deepware Scanner**
- **AI-generated text detection**: **GPTZero**, **Originality.ai**, **Binoculars** (research tool)
- **GAN-generated face detection**: Check **ThisPersonDoesNotExist.com** for understanding artifacts
- **Audio deepfake**: **Hiya.ai**, **Resemble Detect**
- **Underrated:** Metadata analysis of AI-generated content often reveals creation tools

### Step 179 — AI for Multilingual Intelligence
Language is no longer a barrier with AI:
- **DeepL** — highest quality neural translation (supports 30+ languages)
- **Google Translate API** — broader language support (automated processing)
- **Whisper** — multilingual speech recognition
- **Build intelligence pipelines that automatically translate** non-English content from: Russian Telegram channels, Chinese forums, Arabic social media, Spanish dark web forums

### Step 180 — Machine Learning for Anomaly Detection
ML identifies patterns humans miss:
- **Time series anomaly detection**: Unusual web traffic patterns (Isolation Forest, LSTM)
- **Graph anomaly detection**: Identifying unusual nodes in relationship networks
- **Natural language anomaly**: Identifying AI-generated text in supposed human communications
- **Tool:** **PyOD** — Python library for anomaly detection with 40+ algorithms

### Step 181 — AI-Powered Facial Recognition in OSINT
Facial recognition expands OSINT capability significantly:
- **PimEyes** — facial search engine (paid subscription)
- **FaceCheck.ID** — free alternative with limitations
- **Amazon Rekognition** — API-based with collection management
- **Legal and ethical considerations**: Many jurisdictions restrict facial recognition use — know your legal framework
- **Underrated:** Reverse image search on face crops (not full image) dramatically improves matching accuracy

### Step 182 — Custom AI Model Training for OSINT
At expert level, train custom models for specific intelligence tasks:
- **Logo detection**: Fine-tune YOLO for detecting specific company logos in images (identifies target companies in social media photos)
- **Uniform detection**: Classify security personnel, military, police in images
- **License plate region classification**: Country/state identification from plate design
- Tools: **Roboflow** (dataset management), **Ultralytics YOLO** (training), **HuggingFace** (deployment)

### Step 183 — AI Ethics and Bias in Intelligence
AI introduces new failure modes for analysts:
- **Training data bias**: AI trained on skewed data produces skewed intelligence
- **Automation bias**: Over-relying on AI outputs without verification
- **Adversarial inputs**: Threat actors can craft inputs that fool AI systems
- **Always apply the analyst's verification standard to AI outputs: treat AI as a junior analyst whose work must be reviewed**

### Step 184 — AI-Powered Competitive Intelligence
AI enables competitive intelligence at enterprise scale:
- **News monitoring at scale**: Process thousands of news articles daily for competitive signals
- **Patent analysis**: AI extracts technology trends from patent databases
- **Job posting analysis**: Track competitor hiring patterns to predict strategic moves
- **Earnings call analysis**: NLP on all competitor earnings call transcripts to track strategic themes over time

### Step 185 — Build an AI-Augmented OSINT Platform
Integrate AI into your OSINT workflow:
1. **Collection**: Automated scrapers + API integrations
2. **Processing**: NLP pipelines for entity extraction, translation, sentiment
3. **Storage**: Graph database (Neo4j) + vector database (Chroma/Pinecone) for semantic search
4. **Analysis**: LLM-augmented analysis and report generation
5. **Alerting**: Automated alerts when new intelligence matches collection requirements
6. **Visualization**: Dashboards (Grafana, Kibana) for intelligence overview

---

## PHASE 10 — EXPERT+ TRADECRAFT & MASTERY
*Steps 186–200 | "The difference between good and great is tradecraft."*

---

### Step 186 — Structured Analytic Techniques (SATs)
Professional intelligence analysts use formal methods to counter cognitive bias:
- **ACH (Analysis of Competing Hypotheses)**: Evaluate multiple explanations against evidence
- **Key Assumptions Check**: Explicitly identify and challenge assumptions
- **What-If Analysis**: Force consideration of alternative futures
- **Devil's Advocacy**: Deliberately argue against the prevailing assessment
- **Red Cell Analysis**: Adopt the adversary's perspective to anticipate their actions
- Study: **"Critical Thinking for Strategic Intelligence"** (Moore & Krizan)

### Step 187 — Intelligence Writing Standards
Intelligence products are useless if not communicated effectively:
- **Bottom Line Up Front (BLUF)**: Lead with the assessment, not the evidence
- **Key Judgments format** (IC style): Lead paragraph summarizes all key findings
- **Confidence levels**: Always express analytic certainty ("We assess with high confidence...")
- **Source attribution**: Every factual claim has a cited source
- **Dissemination control**: Mark sensitivity level (FOUO, TLP:RED, etc.)
- Study CIA's **"Style Manual & Writers Guide for Intelligence Publications"** (declassified)

### Step 188 — Traffic Light Protocol (TLP) for Intelligence Sharing
TLP is the standard for intelligence sharing sensitivity:
- **TLP:RED**: Not for disclosure (personal conversation only)
- **TLP:AMBER**: Limited disclosure (organization + clients)
- **TLP:AMBER+STRICT**: Organization only
- **TLP:GREEN**: Community sharing
- **TLP:CLEAR**: Unlimited public sharing
- Always mark your intelligence products with appropriate TLP — this is professional practice

### Step 189 — Counter-OSINT (Protecting Against What You Do)
An expert knows how to hide from the techniques they use:
- **Digital footprint reduction**: Remove personal data from data brokers (Deleteme, manual opt-outs)
- **Domain privacy**: WHOIS protection for all registered domains
- **Social media hardening**: Minimize public exposure, regularly audit
- **Metadata stripping**: Remove from all published files
- **Network architecture**: Prevent reverse DNS on internal systems, minimize certificate SAN disclosures
- **Job posting hygiene**: Avoid disclosing security tools, internal product names, or team structure

### Step 190 — Cross-Discipline Intelligence Fusion
Expert OSINT practitioners fuse intelligence from all disciplines:
- OSINT surface → reveals TECHINT targets (exposed services) → SIGINT confirms activity → GEOINT shows physical footprint → FININT reveals financial health → SOCINT reveals key personnel → DARKINT shows threat actor interest
- Build intelligence matrices that cross-reference findings across disciplines
- Look for **convergence**: When multiple independent sources point to the same conclusion, confidence increases
- Look for **divergence**: Inconsistencies between sources indicate deception, error, or incomplete picture

### Step 191 — Intelligence Gap Analysis
What you don't know is as important as what you do:
- Maintain a **Gap Matrix**: Each intelligence requirement vs. current collection coverage
- Prioritize collection against highest-priority gaps
- Acknowledge gaps explicitly in intelligence products
- **Critical intelligence gap indicators**: Periods of no activity (could mean OpSec, could mean collection failure), contradictory indicators (could mean deception)

### Step 192 — Source Evaluation Framework
Not all sources are equal. Evaluate every source:
- **Reliability**: History of accuracy (A=reliable, B=usually reliable, C=fairly reliable, D=usually unreliable, E=unreliable, F=unknown)
- **Credibility**: Plausibility of this specific information (1=confirmed, 2=probably true, 3=possibly true, 4=doubtful, 5=improbable, 6=unknown)
- Apply this **NATO STANAG 2511** matrix to every piece of collected intelligence
- Aggregate confidence = combination of source reliability + information credibility

### Step 193 — Deception Detection & Disinformation Analysis
Adversaries plant false intelligence. Detect it:
- **Too perfect**: Information that perfectly confirms your hypothesis should raise suspicion
- **Single-source dependency**: One source providing the only evidence for a major conclusion
- **Timing anomalies**: Information appearing at exactly the right time is suspicious
- **Provenance gaps**: Information with unclear or untraceable origin
- Study: **"Active Measures"** — Russian information operations (declassified studies available)
- **Underrated:** Study magic and stage illusions to understand how deception exploits cognitive architecture

### Step 194 — Legal & Compliance Expert Knowledge
Expert-level OSINT practitioners know the law:
- **CFAA (US)**: Unauthorized access implications, "exceeds authorized access" clause
- **ECPA (Electronic Communications Privacy Act)**: Wiretapping, stored communications
- **GDPR**: Data collection and processing of EU citizens
- **CCPA**: California privacy law
- **International**: Cybercrime laws vary dramatically — know the laws of every jurisdiction you operate in
- **Evidence handling**: Chain of custody for intelligence that may be used in legal proceedings

### Step 195 — Building & Leading an OSINT Team
Expert practitioners build and lead intelligence capabilities:
- **Team structure**: Collection, processing, analysis, reporting — separate roles
- **Tool standardization**: Consistent toolsets with documented procedures
- **Training program**: Certifications (OSCP, GCIH, OSINT.guru OSINT Essentials, GOSI)
- **Quality control**: All intelligence products reviewed before dissemination
- **Community engagement**: Share intelligence with appropriate communities (ISACs, peer organizations)

### Step 196 — Advanced Certification Path
Professional certifications to pursue (in order):
1. **CompTIA Security+** — baseline security credential
2. **eJPT** (eLearnSecurity Junior Penetration Tester) — practical intro
3. **OSCP** (OffSec Certified Professional) — gold standard offensive credential
4. **GPEN** (GIAC Penetration Tester)
5. **GCIH** (GIAC Certified Incident Handler) — for blue team context
6. **GOSI** (GIAC Open Source Intelligence) — dedicated OSINT certification
7. **CPTC** (Certified Penetration Testing Consultant)
8. **OSWE** (OffSec Web Expert)
9. **CRTE** (Certified Red Team Expert) — Active Directory red teaming
10. **OSED** (OffSec Exploit Developer) — Expert+ territory

### Step 197 — Open Source Intelligence Community Engagement
Join the global OSINT community:
- **OSINT.team** — community forum
- **Bellingcat Discord** — active investigation community
- **TraceLabs CTF** — missing persons OSINT competitions
- **OSINT CTFs**: **Gralhix OSINT CTF**, **Sofia Santos' challenges**, **Ozint CTF**
- **Twitter/X OSINT community**: Follow @sector035, @Gralhix, @OSINTtechniques, @bellingcat
- **Conferences**: DEF CON (OSINT Village), BSides events, SANS OSINT Summit

### Step 198 — Build Your Personal Intelligence Collection
An expert analyst has a personal research library:
- **Books**: "The Art of Intelligence" (Hank Crumpton), "Intelligence-Driven Computer Network Defense" (Hutchins et al.), "Countdown to Zero Day" (Zetter), "Sandworm" (Greenberg), "This Is Not Propaganda" (Pomerantsev)
- **Papers**: Read every Bellingcat investigation. All available free at bellingcat.com
- **Reports**: Subscribe to: Mandiant M-Trends, CrowdStrike Global Threat Report, Verizon DBIR, CISA Advisories
- **Courses**: TCM Security OSINT, SANS FOR578 (Cyber Threat Intelligence), Intel Techniques courses

### Step 199 — Personal OSINT Lab & Continuous Learning Framework
Sustain expert-level competence:
- Maintain a home lab with: Kali Linux, Maltego CE, Shodan CLI, Recon-ng, Neo4j
- Practice monthly on: OSINT CTFs, Hack The Box, TryHackMe OSINT rooms
- Document every new technique in your personal knowledge base
- Contribute back: write blog posts, create GitHub repos, participate in community challenges
- Follow CVE feeds daily: NVD, CISA KEV (Known Exploited Vulnerabilities)
- Read threat intelligence feeds: Mandiant, CrowdStrike, Recorded Future (free tiers)

### Step 200 — Develop Your Intelligence Philosophy
At Expert+ level, you transcend tools and techniques. Develop a personal intelligence philosophy:

**The three laws of expert intelligence:**

1. **Intelligence serves decisions, not curiosity.** Every collection effort must answer a specific question. Never collect for its own sake — collection without a consumer is waste.

2. **Uncertainty is honest. False certainty is dangerous.** The most valuable thing an analyst can say is "I don't know, and here's what it would take to find out." Confidence that exceeds the evidence is the original sin of intelligence analysis.

3. **The adversary is adaptive.** Your techniques become known. Your targets harden. The methods that worked yesterday will be detected tomorrow. Continuously develop new approaches, challenge your assumptions, and study failure as aggressively as you study success.

> *"The purpose of intelligence is to reduce uncertainty, not to eliminate it."*
> — Anonymous CIA Analyst

---

## 📚 ESSENTIAL RESOURCE LIBRARY

### Core Tools Reference
| Category | Tool | Cost | Purpose |
|----------|------|------|---------|
| Reconnaissance | Recon-ng | Free | Modular recon framework |
| OSINT | Maltego CE | Free/Paid | Visual link analysis |
| Scanning | Shodan | Free/Paid | Internet device search |
| Social | Maigret | Free | Username OSINT |
| Email | theHarvester | Free | Email/domain harvesting |
| SDR | GQRX + RTL-SDR | $30 HW | Signal reception |
| RFID | Proxmark3 | $400 HW | RFID research |
| Network | Zeek | Free | Network analysis |
| Blockchain | Breadcrumbs.app | Free | Crypto tracing |
| AI | Whisper | Free | Audio transcription |

### Critical Websites
- **Shodan.io** — internet device search
- **Censys.io** — certificate and host intelligence
- **crt.sh** — certificate transparency
- **SecurityTrails** — DNS intelligence
- **IntelX** — intelligence X (breach data)
- **OpenCorporates.com** — company registry
- **ImportYeti.com** — trade intelligence
- **ADS-B Exchange** — unfiltered aircraft tracking
- **OpenSanctions.org** — sanctions database
- **MalwareBazaar.abuse.ch** — malware intelligence

### Recommended Reading (Priority Order)
1. Richards Heuer — *Psychology of Intelligence Analysis* (FREE, CIA declassified)
2. Bellingcat — *All public investigations* (FREE, bellingcat.com)
3. Andy Greenberg — *Sandworm*
4. Kim Zetter — *Countdown to Zero Day*
5. Robert Baer — *See No Evil*
6. Hutchins, Cloppert & Amin — *Intelligence-Driven Computer Network Defense* (FREE PDF)
7. CIA Style Manual (declassified, FREE via FOIA)

---

## ⚡ QUICK START: FIRST 30 DAYS

| Week | Focus | Key Actions |
|------|-------|------------|
| **Week 1** | OpSec & Environment | Set up Kali VM, build sock puppets, install core tools |
| **Week 2** | Search & Discovery | Master Google dorking, Shodan, crt.sh, DNS tools |
| **Week 3** | Social Intelligence | Sherlock, Maigret, username pivot, Maltego basics |
| **Week 4** | Technical Intel | GitHub OSINT, cloud exposure, tech stack fingerprinting |

---

*Last Updated: 2025 | This document is for educational and authorized security research purposes only.*
*Follow all applicable laws in your jurisdiction. Unauthorized interception and unauthorized computer access are serious crimes.*
