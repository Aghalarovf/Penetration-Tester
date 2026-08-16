# C# Red Team Libraries — 460 Tasks / 46 Modules

> Red Team / C2 development üçün tam tədris kurrikulumu.
> Hər modulda 10 tapşırıq, asandan çətinə doğru sıralanıb.
> 12 faza, 46 modul, 460 tapşırıq.

---

## Phase 1 — Foundation

### 1. `System` — Console, Math, String, DateTime, Environment

1. **Hello Recon** — Print `MachineName`, `UserName`, `OSVersion`, and `CurrentDirectory` to the console with colored labels (green for keys, white for values).
2. **Env Var Dump** — Iterate all environment variables and print them in `KEY=VALUE` format. Count how many contain the word "PATH".
3. **Token Hunter** — Check for `AWS_ACCESS_KEY_ID`, `GITHUB_TOKEN`, `AZURE_CLIENT_SECRET`, `GOOGLE_APPLICATION_CREDENTIALS` in environment variables. Print found ones in red, missing ones in gray.
4. **Uptime Sandbox Check** — Read `Environment.TickCount64`, convert to hours and minutes, and print. If uptime is under 1 hour, print "Possible sandbox detected" and exit.
5. **String Encoder** — Accept a string from `Console.ReadLine()`, then print its `Base64` encoding, `ToUpper`, `Length`, and character frequency (count each unique char).
6. **Timestamp Logger** — Every 5 seconds print the current timestamp in `yyyy-MM-dd HH:mm:ss` format along with how many seconds have elapsed since the program started. Run for 30 seconds total.
7. **Random Beacon ID Generator** — Generate 20 unique beacon IDs in the format `BEA-{MachineName}-{Random6DigitHex}`. Ensure no duplicates using `HashSet<string>`.
8. **Path Resolver** — Print all `Environment.SpecialFolder` values that exist on the current machine. Show the full path for each, and flag writable ones with a `[W]` marker.
9. **Domain vs Workgroup Detector** — Compare `UserDomainName` vs `MachineName`. If they differ, print "Domain joined: {domain}". Also print OS bitness and processor count.
10. **Mini C2 Pre-flight** — Combine all checks: uptime, domain, user, OS, writable paths, and cloud tokens. Output a formatted recon report with a pass/fail status per check.

### 2. `System.IO`

1. **File Finder** — Search `C:\Users` (or `$HOME` on Linux) recursively for all `.txt` files. Print the full path and file size of each.
2. **Temp Stager** — Write the string `"payload staged"` to a randomly named file in the system temp directory using `Path.GetTempFileName()`. Read it back and verify content.
3. **Credential File Hunter** — Recursively search a given directory for files whose names contain: `password`, `creds`, `secret`, `token`, `key`. Print matches with their full paths.
4. **Line-by-Line Reader** — Read a large text file line by line using `StreamReader` (not `ReadAllLines`). Count lines, words, and the top 5 most frequent words.
5. **Append Logger** — Create a logger that appends timestamped entries to `log.txt` using `StreamWriter(append: true)`. Write 50 entries with random messages, then read and display the last 10.
6. **Directory Loot Mapper** — For a given root path, recursively build a tree structure of directories and files (name, size, last modified). Output it as an indented text report.
7. **In-Memory File Processor** — Read a binary file into a `MemoryStream`. XOR every byte with `0xAA`. Write the result to a new file. Verify by XORing again to get the original.
8. **SSH Key Hunter** — Search the user's home directory recursively for files named `id_rsa`, `id_ed25519`, `.pem`, `.ppk`. For each found, print the first line to identify key type.
9. **Safe File Dropper** — Write a payload (any byte array) to `AppData\Local\Temp` only if disk free space exceeds 50MB, the file doesn't already exist, and the process has write access. Handle all exceptions.
10. **Atomic File Exfil Prep** — Read all `.txt` and `.docx` files from a folder, combine their content into one `MemoryStream`, compute its SHA256 hash, and write a manifest file listing each source file and its individual hash.

### 3. `System.IO.Compression`

1. **Zip Creator** — Create a zip archive from a given folder using `ZipFile.CreateFromDirectory`. Print the archive size before and after.
2. **Zip Reader** — Open a `.zip` file and list all entries: name, compressed size, uncompressed size, and compression ratio.
3. **In-Memory GZip** — Compress a string (e.g., a 1000-char fake recon report) using `GZipStream` into a `MemoryStream`. Print original vs compressed byte counts.
4. **In-Memory Decompress** — Accept a Base64-encoded GZip blob, decode it, decompress it in memory, and print the plaintext — no files written to disk.
5. **Selective Zip** — Given a folder, create a zip that only includes files modified in the last 7 days and larger than 1KB.
6. **Zip Entry Rename** — Open a zip in `Update` mode. Rename all entries by adding a `_backup` suffix before the extension. Save the modified archive.
7. **Multi-Format Compressor** — Compress the same input using `GZipStream` and `DeflateStream`. Compare output sizes, then decompress both and verify they match the original.
8. **Streamed Exfil Simulator** — Read files from a folder one by one, compress each with GZip into memory, Base64-encode the result, and print it as if sending to a C2 (simulate with `Console.WriteLine`).
9. **Archive Integrity Checker** — Open a zip, extract each entry into memory, compute SHA256 hash of each, and write a `manifest.txt` inside the archive listing `filename:sha256hash`.
10. **In-Memory Payload Unpacker** — Simulate a dropper: start with a Base64+GZip encoded fake "payload", decode → decompress → load bytes into a `MemoryStream` → print the first 16 bytes as hex. No disk writes.

### 4. `System.Text.Json`

1. **JSON Parser** — Parse a hardcoded JSON string `{"task":"shell","args":["whoami"],"sleep":30}` and print each field with its type.
2. **Beacon Serializer** — Serialize an anonymous object containing `hostname`, `username`, `os`, `pid`, `arch`, and `timestamp` to a compact JSON string.
3. **C2 Task Deserializer** — Define a `record C2Task(string Task, string[] Args, int Sleep)` and deserialize a sample JSON string into it. Print each field.
4. **Beacon Check-in Simulator** — Build a full check-in payload: gather real system info (`MachineName`, `UserName`, `OSVersion`, process ID), serialize to JSON, Base64-encode it, and print the result.
5. **C2 Response Parser** — Parse a JSON array of tasks: `[{"task":"shell","cmd":"whoami"},{"task":"upload","path":"C:\\loot.txt"}]`. Dispatch each to a handler function based on `task` type.
6. **Config File Loader** — Write a JSON config file `{"sleep":60,"jitter":10,"c2":"http://localhost","useragent":"Mozilla/5.0"}`. Load it at runtime with `JsonSerializer.Deserialize` and use the values.
7. **Nested JSON Parser** — Parse a complex response: `{"status":"ok","data":{"tasks":[...],"implant":{"id":"abc","version":"1.2"}}}`. Navigate nested properties using `JsonDocument`.
8. **JSON Diff Tool** — Parse two JSON objects (e.g., two system recon snapshots). Compare all keys and report which values changed, which are new, and which were removed.
9. **Streaming JSON Parser** — Use `Utf8JsonReader` to parse a large JSON array (1000 items) without loading it all into memory. Count items matching a filter (e.g., `"status":"active"`).
10. **Encrypted JSON Channel** — Serialize a C2 task to JSON, AES-256 encrypt it (using `System.Security.Cryptography`), Base64-encode the ciphertext, then reverse the whole process and verify the original task is recovered.

---

## Phase 2 — Core Red Team

### 5. `System.Net`

1. **HTTP GET** — Download the HTML of `http://example.com` using `HttpClient` and print its length and first 200 characters.
2. **Custom User-Agent** — Make an HTTP GET request with a custom `User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64)` header. Print response headers.
3. **DNS Resolver** — Accept a hostname from the user. Use `Dns.GetHostEntry()` to resolve it. Print all returned IP addresses and the canonical hostname.
4. **HTTP Downloader** — Download a file from a URL, save it to the temp directory, and print download speed (bytes/second) and total time.
5. **HTTP POST Beacon** — Simulate a beacon check-in: POST a JSON body `{"host":"PC01","user":"admin","os":"Win10"}` to `http://httpbin.org/post`. Parse and print the response.
6. **Proxy-Aware Downloader** — Extend the downloader to support an optional HTTP proxy (configurable via environment variable `HTTP_PROXY`). Fall back to direct if not set.
7. **Retry HTTP Client** — Build an `HttpClient` wrapper that retries failed requests up to 3 times with exponential backoff (1s, 2s, 4s). Log each attempt.
8. **Multi-URL Parallel Downloader** — Accept a list of URLs. Download all concurrently using `Task.WhenAll`. Report which succeeded, which failed, and total bytes downloaded.
9. **HTTP C2 Simulator** — Build a simulated C2 loop: GET `http://httpbin.org/get` (pretend it's a task endpoint). Parse response. POST fake results to `http://httpbin.org/post`. Repeat every 10 seconds.
10. **Domain Fronting Skeleton** — Build an `HttpClient` that sets the `Host` header to a target domain but sends the request to a CDN IP. Log the request details. (Use a real public URL to test the mechanics.)

### 6. `System.Net.Sockets`

1. **TCP Connect Check** — Try to connect to `google.com:80` with a 2-second timeout. Print whether the port is open or closed.
2. **Port Scanner** — Scan ports 1–1024 on `127.0.0.1` using `TcpClient.ConnectAsync`. Print all open ports.
3. **TCP Echo Client** — Connect to a local TCP echo server (you can use `ncat -l 9999 -e cat`), send a message, and print the response.
4. **Bind Shell Skeleton** — Create a `TcpListener` on port 9001. Wait for one connection. Read a line of text from the client, print it, and send back `"received"`.
5. **Reverse Shell Skeleton** — Connect to `127.0.0.1:4444` via `TcpClient`. Read a command string from the stream. Execute it with `Process.Start`. Send stdout back over the socket.
6. **UDP Beacon** — Send a UDP packet containing `hostname|username|timestamp` to `127.0.0.1:5555` every 5 seconds. Build a matching UDP listener to receive and print the beacons.
7. **Async Port Scanner** — Scan all 65535 ports on a target using async tasks with a semaphore limiting concurrency to 500. Print open ports sorted numerically.
8. **Banner Grabber** — Connect to open ports (e.g., 21, 22, 25, 80, 443) and read the first 256 bytes. Print service banners to identify running services.
9. **Multi-client TCP C2** — Build a TCP server that handles multiple clients simultaneously using `Task.Run` per connection. Each client sends commands; the server responds with uppercase versions.
10. **ICMP Skeleton** — Using a raw socket with `ProtocolType.Icmp`, craft and send a minimal ICMP echo request to `127.0.0.1`. Receive the reply and print round-trip time.

### 7. `System.Net.NetworkInformation`

1. **NIC Lister** — Print all network interfaces: name, type, and operational status.
2. **IP & Mask Printer** — For each active interface, print all unicast IP addresses and their subnet masks.
3. **Gateway & DNS Dump** — Print the default gateway and DNS servers for each interface that has them.
4. **MAC Address Collector** — List all interface MAC addresses. For each, identify the vendor prefix (first 3 bytes) and guess if it's a VM (VMware: `00:0C:29`, VirtualBox: `08:00:27`).
5. **Ping Single Host** — Ping a hostname entered by the user. Print: reply status, round-trip time, TTL, and whether it's likely Windows or Linux (based on TTL value).
6. **Ping Sweep** — Perform an async ping sweep of `192.168.1.1`–`192.168.1.254` (or any /24 subnet) with a 500ms timeout. Print all alive hosts sorted by IP.
7. **Network Topology Report** — Combine interface enumeration, gateway detection, and DNS discovery into a formatted report. Detect if there are multiple active interfaces (possible dual-homed host).
8. **ARP Cache Reader** — Run `arp -a` via `Process.Start`, parse the output, and build a `Dictionary<string, string>` of IP → MAC. Print the table sorted by IP.
9. **Subnet Calculator** — Given an IP and subnet mask, calculate: network address, broadcast address, first/last usable host, and total host count. Validate with at least 5 different subnets.
10. **Full Network Recon Report** — Produce a complete network snapshot: all interfaces, IPs, MACs, gateways, DNS servers, ARP cache, active connections (via `netstat` output parsing), and a live ping sweep of the local subnet.

### 8. `System.Diagnostics`

1. **Process Lister** — Print all running processes: PID, name, and memory usage (working set in MB).
2. **Command Runner** — Run `whoami`, `hostname`, and `ipconfig /all` (or Linux equivalents) using `ProcessStartInfo`. Capture and print stdout for each.
3. **Hidden Window Executor** — Run `cmd.exe /c dir C:\` with `CreateNoWindow = true` and `RedirectStandardOutput = true`. Print the captured output.
4. **Process Killer** — Accept a process name from the user. Find all matching processes and kill them. Print how many were killed.
5. **Process Watcher** — Every 2 seconds, take a snapshot of running process names. Print any new processes that appeared or disappeared since the last snapshot.
6. **Injection Target Finder** — List processes that match common injection targets: `explorer.exe`, `svchost.exe`, `notepad.exe`, `RuntimeBroker.exe`. Print PID, session ID, and whether they are 64-bit.
7. **Command Output Capture with Timeout** — Run an arbitrary command with a 5-second timeout. If it doesn't finish, kill it and print "Timeout". Otherwise print stdout and stderr separately.
8. **Event Log Reader** — Read the last 50 entries from the `Security` event log. Filter for event IDs `4624` (logon) and `4625` (failed logon). Print timestamp, account name, and logon type.
9. **Performance Monitor** — Sample CPU usage (`% Processor Time`) and available memory (`Available MBytes`) every second for 30 seconds using `PerformanceCounter`. Print min, max, and average for each.
10. **Process Ancestry Tree** — Build and print the parent-child process tree for all running processes. Use WMI or `Process` info to find `ParentProcessId`. Display as an indented tree.

### 9. `System.Threading` & `System.Threading.Tasks`

1. **Delayed Start** — Use `Thread.Sleep(5000)` and `Task.Delay(5000)`. Measure actual elapsed time with `Stopwatch` and print both.
2. **Sandbox Sleep Check** — Sleep for 10 seconds. If `Stopwatch` shows elapsed < 8 seconds, print "Sandbox detected" and exit. Otherwise print "Real system confirmed".
3. **Background Beacon Loop** — Start a background thread that prints `[BEACON] {timestamp}` every 5 seconds. Let main thread run for 30 seconds, then cancel cleanly.
4. **Async Parallel Port Scanner** — Async scan ports 1–1024 with `ConnectAsync` + `WaitAsync(500ms)`. Limit concurrency to 100 using `SemaphoreSlim`. Print open ports.
5. **Mutex Single Instance** — Use a named `Mutex` to ensure only one instance of your program runs. If a second instance is launched, it should print "Already running" and exit.
6. **CancellationToken Auto-Expiry** — Create a beacon loop that runs until a `CancellationToken` fires after 60 seconds. Print a clean shutdown message. Simulate implant self-expiry.
7. **Jitter Calculator** — Implement a beacon sleep with jitter: given `sleepMs=30000` and `jitter=20`, sleep for a random time between `24000ms` and `36000ms`. Run 10 iterations, print each sleep duration.
8. **Parallel Reconnaissance** — Run 5 recon tasks concurrently using `Task.WhenAll`: ping sweep, port scan, process list, env var harvest, and network interface enum. Collect all results into a report.
9. **Thread Pool Rate Limiter** — Submit 500 tasks to `ThreadPool`. Each task sleeps 100ms. Use `SemaphoreSlim(50)` to limit to 50 concurrent. Track start/finish times and print throughput.
10. **Sleep Obfuscation Simulator** — Implement a beacon that: records start time → XOR-encodes its own config string in memory → sleeps with jitter → XOR-decodes config → verifies integrity before continuing. Simulate the full cycle 5 times.

### 10. `Microsoft.Win32` — Registry

1. **Registry Reader** — Read and print `HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProductName` and `CurrentBuild`.
2. **Run Key Enumerator** — List all entries under `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` and `HKLM\...\Run`. Print name and value for each.
3. **Persistence Writer** — Write a test value `"TestPersistence" = "C:\Temp\test.exe"` to the HKCU Run key. Verify it was written by reading it back.
4. **Persistence Cleaner** — Delete the value written in task 3. Verify deletion by checking it no longer exists.
5. **Installed Software Lister** — Enumerate `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall`. Print the `DisplayName` and `DisplayVersion` of each installed program.
6. **Security Product Detector** — Check registry paths used by common AV/EDR tools (Windows Defender: `HKLM\SOFTWARE\Microsoft\Windows Defender`). Print their configuration values.
7. **Service Enumerator** — Enumerate all keys under `HKLM\SYSTEM\CurrentControlSet\Services`. For each, read `ImagePath` and `Start` (startup type). Flag services with non-standard paths.
8. **Registry Persistence Detector** — Check all 5 common persistence registry locations. For each entry found, flag it as: known system binary, known application, or suspicious (unknown path).
9. **Hive Snapshot Differ** — Take a snapshot of all Run key entries. Wait 30 seconds. Take another snapshot. Report any additions, deletions, or modifications.
10. **Autorun Audit Tool** — Combine: Run keys (HKCU + HKLM), `RunOnce` keys, Winlogon entries (`Shell`, `Userinit`), and Services startup paths. Produce a formatted persistence audit report with risk ratings.

### 11. `System` — `Environment` (Advanced)

1. **Special Folder Map** — Enumerate and print all `Environment.SpecialFolder` values. For each, print the resolved path and whether it exists.
2. **Cloud Token Scanner** — Check 15 common cloud/CI environment variables (`AWS_*`, `AZURE_*`, `GITHUB_*`, `GCP_*`, `DOCKER_*`). Print found tokens (first 8 chars + `...`).
3. **Writable Staging Path Finder** — Test all special folders for write access by attempting to create a temp file. Print a `[WRITABLE]` or `[DENIED]` status for each.
4. **Uptime-Based Sandbox Check** — Read `TickCount64`. Print uptime in days/hours/minutes. Exit if under 1 hour. Also check processor count (< 2 = likely sandbox).
5. **32 vs 64-Bit Detector** — Print `Is64BitOperatingSystem`, `Is64BitProcess`, and `ProcessorCount`. Suggest whether to use x86 or x64 shellcode based on the result.
6. **Command Line Argument Parser** — Read `GetCommandLineArgs()`. Support flags: `--sleep N`, `--output path`, `--verbose`. Print parsed values or defaults if not provided.
7. **Network Share Path Probe** — Build a list of `Environment.GetEnvironmentVariable("USERPROFILE")` + common paths (`Desktop`, `Documents`, `Downloads`). Check each for interesting file extensions.
8. **Anti-Analysis Checklist** — Combine: uptime check, processor count check, screen resolution (via WinForms/P/Invoke), username check (is it `sandbox`, `malware`, `virus`?). Score 0–5 and decide to proceed or exit.
9. **Environment Snapshot Serializer** — Collect all environment variables, all special folder paths, machine info, and uptime. Serialize to JSON. Save to a timestamped file in `%TEMP%`.
10. **Full Pre-Execution Gauntlet** — Implement a checklist that runs all Phase 1 sandbox/recon checks in sequence. Each check returns pass/fail. If more than 2 fail, exit silently. Otherwise print "Execution approved" and continue.

---

## Phase 3 — Post-Exploitation

### 12. `System.Management` (WMI)

1. **OS Info Query** — Run `SELECT * FROM Win32_OperatingSystem`. Print OS name, version, architecture, and last boot time.
2. **Process Lister via WMI** — Query `Win32_Process`. Print PID, name, and `ExecutablePath` for each process.
3. **Local User Enumerator** — Query `Win32_UserAccount`. Print username, SID, account type, and whether the account is disabled.
4. **Service Enumerator** — Query `Win32_Service`. Print name, state, start mode, and path for all services. Highlight ones that are running with a non-system path.
5. **AV/EDR Detector** — Query `AntiVirusProduct` from `root\SecurityCenter2`. Print product name, state, and signature update date.
6. **Network Config Dump** — Query `Win32_NetworkAdapterConfiguration WHERE IPEnabled = True`. Print adapter name, IP, subnet, gateway, DNS, and DHCP status.
7. **Logical Disk Mapper** — Query `Win32_LogicalDisk`. Print drive letter, type, total size, free space, and file system. Flag drives under 10% free space.
8. **WMI Event Watcher** — Subscribe to `Win32_ProcessStartTrace`. Print the name of every new process created while your watcher runs. Run for 30 seconds.
9. **Remote WMI Connector** — Accept a target hostname, username, and password. Connect to the remote WMI namespace and run `SELECT * FROM Win32_OperatingSystem`. Print results. Handle auth failures gracefully.
10. **Remote Process Creation via WMI** — Connect to a remote host (use `localhost` for testing). Use `Win32_Process.Create` to run `cmd.exe /c whoami > C:\Temp\wmi_out.txt`. Read back the output file.

### 13. `System.DirectoryServices`

1. **LDAP Connector** — Connect to `LDAP://DC=corp,DC=local` (or a test AD). Print the root domain properties: `name`, `dc`, `distinguishedName`.
2. **User Enumerator** — Search for all user objects. Print `sAMAccountName`, `displayName`, and `mail` for each.
3. **Computer Enumerator** — Search for all computer objects. Print name, OS version, and last logon timestamp.
4. **Group Membership Finder** — Accept a username. Find all groups they belong to using `memberOf` attribute. Print group names.
5. **Domain Admin Finder** — Search for all users who are members of `CN=Domain Admins,CN=Users,DC=corp,DC=local`. Print their usernames and last logon.
6. **Kerberoastable Account Finder** — Search for users with `servicePrincipalName=*` and enabled accounts. Print username and all SPNs.
7. **ASREPRoastable Account Finder** — Search for users with the `DONT_REQUIRE_PREAUTH` flag set (`userAccountControl` bit `0x400000`). Print usernames.
8. **Credential Validator** — Accept a domain, username, and password. Use `PrincipalContext.ValidateCredentials`. Print success or failure. Handle locked accounts and bad password errors.
9. **Privileged Account Auditor** — Enumerate members of: Domain Admins, Enterprise Admins, Schema Admins, Backup Operators, and Account Operators. Build a combined privileged account report.
10. **Full AD Recon Report** — Combine all above: users, computers, groups, Kerberoastable accounts, ASREPRoastable accounts, privileged users. Serialize to JSON and save to a file.

### 14. `System.Security.Cryptography`

1. **AES Key Generator** — Generate a random 256-bit AES key and 128-bit IV using `RandomNumberGenerator`. Print both as hex strings.
2. **AES Encryptor** — Encrypt the string `"Hello, C2!"` with AES-256-CBC. Print the ciphertext as Base64.
3. **AES Round-Trip** — Encrypt a string, then decrypt it. Verify the output matches the input. Print PASS or FAIL.
4. **SHA256 File Hasher** — Compute SHA256 hash of a given file. Print the hash as a lowercase hex string. Verify against a known hash.
5. **HMAC Signer** — Compute an HMAC-SHA256 signature over a message using a 32-byte random key. Print the signature. Then verify it by recomputing and comparing.
6. **RSA Key Pair Generator** — Generate a 2048-bit RSA key pair. Export both keys as Base64. Print their lengths.
7. **RSA Encrypt/Decrypt** — Encrypt a 32-byte AES key with RSA-OAEP-SHA256. Decrypt it and verify the result matches the original.
8. **Encrypted Beacon Payload** — Build a full pipeline: generate AES key/IV → serialize recon data to JSON → AES-encrypt → HMAC-sign → Base64-encode. Print the final payload.
9. **Encrypted Beacon Decoder** — Reverse the pipeline from task 8: Base64-decode → verify HMAC → AES-decrypt → deserialize JSON → print the recon data.
10. **RSA + AES Hybrid Encryption** — Implement a full C2 handshake: client generates AES key → encrypts it with server RSA public key → server decrypts with private key → both sides use the AES key for subsequent messages. Simulate both sides in one program.

### 15. `System.Security` — DPAPI / `ProtectedData`

1. **DPAPI Encrypt String** — Protect the string `"SuperSecret123"` using `ProtectedData.Protect` with `CurrentUser` scope. Print the result as Base64.
2. **DPAPI Decrypt String** — Decrypt the Base64 blob from task 1. Verify the original string is recovered.
3. **Machine-Scope Encryption** — Encrypt and decrypt using `LocalMachine` scope. Discuss (in a comment) why this is weaker than `CurrentUser` scope.
4. **Entropy Usage** — Encrypt with an optional entropy byte array. Show that decryption fails if the entropy is wrong (different or absent).
5. **Chrome Local State Parser** — Read `%LOCALAPPDATA%\Google\Chrome\User Data\Local State`. Parse the JSON and extract the `os_crypt.encrypted_key` field.
6. **Chrome Master Key Extractor** — From the extracted key: Base64-decode → strip the `DPAPI` prefix (first 5 bytes) → call `ProtectedData.Unprotect`. Print the 32-byte master key as hex.
7. **Chrome Login DB Path Finder** — Locate the Chrome `Login Data` SQLite file. Print its path and size. (Do not open it yet — just locate all profile copies.)
8. **DPAPI Blob Analyzer** — Write a function that accepts any DPAPI blob and prints its size and whether `CurrentUser` or `LocalMachine` scope decryption succeeds.
9. **Credential Manager Enumerator** — Use P/Invoke to `CredEnumerateW`. Print the count of stored credentials, their target names and types. (No passwords — just metadata.)
10. **Full Browser Cred Pipeline** — Parse Chrome `Local State` → extract master key via DPAPI → open `Login Data` SQLite (using `Microsoft.Data.Sqlite` NuGet) → for each row, print URL, username, and the length of the encrypted password blob.

### 16. `System.IO.Pipes`

1. **Basic Named Pipe Server** — Create a named pipe server, wait for one client connection, read a string message, print it, and close.
2. **Basic Named Pipe Client** — Connect to the server from task 1. Send the string `"Hello from client"`. Confirm connection success.
3. **Bidirectional Pipe** — Extend the server to read a message, process it (uppercase it), and send the response back to the client.
4. **Pipe-Based Command Runner** — Server reads a command string over the pipe, executes it with `Process.Start`, and sends stdout back to the client over the same pipe.
5. **Multi-Message Session** — Client sends 5 messages in a loop. Server responds to each. Both sides print sent/received messages with timestamps.
6. **Named Pipe C2 Simulator** — Server acts as a C2: sends a JSON task `{"task":"whoami"}` to client. Client executes it and returns the result as JSON. Server prints the parsed result.
7. **Anonymous Pipe Parent-Child** — Parent creates an `AnonymousPipeServerStream`. Spawns a child process passing the client handle as an argument. Child connects and sends back its PID.
8. **Pipe Reconnect Logic** — Client attempts to connect to a named pipe every 2 seconds for up to 30 seconds (retry loop). Server starts after 10 seconds. Demonstrate successful delayed connection.
9. **Remote Pipe Connector** — Connect to a named pipe on `localhost` using full UNC path `\\.\pipe\PipeName`. Then modify to target a remote hostname (even if the remote just mirrors localhost in testing).
10. **SMB C2 Channel Simulator** — Full simulation: server manages a queue of tasks. Client connects, authenticates with a shared secret, receives a task, executes it, returns results. Server logs all interactions with timestamps.

### 17. `System.Windows.Forms`

1. **Screenshot Capture** — Capture the primary screen using `Graphics.CopyFromScreen`. Save as PNG to `%TEMP%\screen_{timestamp}.png`.
2. **Clipboard Reader** — Read and print current clipboard text content. Handle the case where the clipboard is empty or contains non-text data.
3. **Key State Checker** — Check and print the current state of specific keys (Caps Lock, Num Lock, Shift) using `GetAsyncKeyState` via P/Invoke.
4. **Clipboard Monitor** — Check the clipboard every 2 seconds for 60 seconds. Print any new text content that appears (detect changes).
5. **Screenshot to Memory** — Capture the screen but save it to a `MemoryStream` instead of a file. Print the byte count and a SHA256 hash of the image data.
6. **Keylogger Prototype** — Log key presses using `GetAsyncKeyState` in a background loop. Write each key name and timestamp to a log file. Run for 30 seconds.
7. **Fake Input Box** — Display a `MessageBox` claiming "Your Windows session has expired." Then show an `InputBox` prompting for credentials. Print whatever the user typed.
8. **Periodic Screenshot Loop** — Take a screenshot every 10 seconds for 2 minutes. Save each with a timestamped filename. Print total disk space used by the screenshots.
9. **Keylogger with Special Keys** — Extend the keylogger to properly handle: Backspace (remove last char), Enter (add newline), Space. Output clean readable text instead of key names.
10. **Combined Surveillance Agent** — Combine: periodic screenshots (every 30s), clipboard monitoring (every 5s), keylogging. Write all captured data to a structured log file with timestamps and data type labels.

### 18. `System.Drawing`

1. **Screenshot Basics** — Capture the primary monitor using `SystemInformation.PrimaryMonitorSize` and `Graphics.CopyFromScreen`. Save as JPEG.
2. **JPEG vs PNG Size Comparison** — Capture the screen. Save as both JPEG (quality 50) and PNG. Print both file sizes and the compression ratio.
3. **In-Memory Screenshot** — Capture screen to a `MemoryStream` as PNG. Print byte count. Do not write any file to disk.
4. **Multi-Monitor Capture** — Capture all monitors individually. Save each as a separate JPEG with the monitor index in the filename.
5. **Pixel Color Sampler** — Capture the screen. Sample 10 random pixel coordinates. Print `X,Y → R,G,B` for each.
6. **LSB Steganography — Hide** — Given a message string (max 256 chars), hide it in the LSB of the blue channel of a bitmap image pixel by pixel. Save the modified image.
7. **LSB Steganography — Extract** — Read a bitmap modified by task 6. Extract the hidden message from the LSB of the blue channel. Print the recovered string.
8. **Screenshot Diff** — Take two screenshots 5 seconds apart. Compare them pixel by pixel. Output the percentage of pixels that changed (motion detection).
9. **Image Exfil Simulator** — Capture screen → save to `MemoryStream` as JPEG → Base64-encode → split into 512-byte chunks → print each chunk as if sending to C2.
10. **Stego Exfil Channel** — Embed a 200-byte recon JSON payload into the LSBs of a screenshot. Base64-encode the stego image. Then decode: Base64 → bitmap → extract hidden bytes → deserialize JSON → verify the payload.

---

## Phase 4 — Advanced / EDR Evasion

### 19. `System.Reflection`

1. **Assembly Inspector** — Load `mscorlib.dll` from disk. List all public types and how many public methods each has.
2. **Dynamic Method Invoker** — Load any `.exe` or `.dll` from disk. Use Reflection to invoke a public static method by name, passing arguments from the command line.
3. **In-Memory Assembly Loader** — Read a compiled `.exe` into a byte array. Load it with `Assembly.Load(bytes)`. Invoke its entry point with `new string[]{"--test"}`.
4. **Private Field Accessor** — Create a test class with a private field `_secret = "hidden"`. Use Reflection with `BindingFlags.NonPublic` to read and print the private field from outside the class.
5. **Loaded Assembly Lister** — Enumerate all assemblies currently loaded in `AppDomain.CurrentDomain`. Print name, version, location, and whether it is a GAC assembly.
6. **Download and Execute in Memory** — Download a `.dll` from a URL (use a local test server) into a byte array. Load with `Assembly.Load`. Find a specific type and invoke a method. No file written to disk.
7. **Method Hijacker** — Use Reflection to enumerate all methods of a loaded type. Find any method that accepts a `string` parameter. Invoke them all with the input `"test"`. Print results or exceptions.
8. **Dynamic P/Invoke via Reflection** — Instead of a static `[DllImport]`, use Reflection to call `Marshal.GetDelegateForFunctionPointer` and dynamically invoke `MessageBoxW` from `user32.dll`.
9. **Assembly Dependency Walker** — Load an assembly and recursively list all referenced assemblies (by walking `GetReferencedAssemblies()`). Print as an indented dependency tree.
10. **In-Memory Chain Loader** — Simulate a staged loader: Stage 1 is a small C# program that downloads Stage 2 (another `.exe`) into memory, loads it via `Assembly.Load`, invokes its entry point, which then downloads and loads Stage 3. Implement all 3 stages.

### 20. `System.Runtime.InteropServices`

1. **P/Invoke Hello** — Declare and call `MessageBoxW` from `user32.dll`. Display a message box with a title and message. Confirm it returns 1 (OK).
2. **Struct Marshalling** — Define a `SYSTEMTIME` struct with `[StructLayout(LayoutKind.Sequential)]`. P/Invoke `GetSystemTime` from `kernel32.dll`. Print all fields.
3. **VirtualAlloc Test** — Allocate 4096 bytes using `VirtualAlloc` with `PAGE_READWRITE`. Write a byte pattern. Read it back. Confirm. Free with `VirtualFree`.
4. **Marshal Copy Demo** — Allocate memory with `VirtualAlloc`. Use `Marshal.Copy` to write a byte array into it. Read back the bytes by marshalling. Confirm round-trip.
5. **Bitness Detector** — Use `Marshal.SizeOf(typeof(IntPtr))` to detect 32 vs 64-bit. Also call `IsWow64Process` via P/Invoke and print whether the current process is WOW64.
6. **GetModuleHandle + GetProcAddress** — Use P/Invoke to call `GetModuleHandle("kernel32.dll")`. Then call `GetProcAddress` to get the address of `CreateFileW`. Print the pointer value.
7. **VirtualAlloc + ExecuteNOP Sled** — Allocate memory with `PAGE_EXECUTE_READWRITE`. Write 16 NOP bytes (`0x90`) followed by a `RET` (`0xC3`). Create a delegate to the memory and call it. Confirm it returns without crashing.
8. **Remote Memory Read** — Open a target process by PID with `PROCESS_VM_READ`. Use `ReadProcessMemory` to read the first 64 bytes of its base address. Print as hex.
9. **Dynamic API Resolution** — Write a function that accepts a DLL name and export name. Use `LoadLibrary` + `GetProcAddress` to resolve the function pointer. Test with 5 different API functions.
10. **Classic Shellcode Injection** — Write a C# program that: allocates memory in the current process with `VirtualAlloc(PAGE_EXECUTE_READWRITE)`, copies a NOP sled + `RET` shellcode stub, creates a thread pointing to it, waits for it to finish. (Use safe test shellcode only — NOP+RET.)

### 21. `System.Security.Principal`

1. **Identity Printer** — Print the current `WindowsIdentity`: name, authentication type, `IsSystem`, `IsGuest`, and `IsAnonymous`.
2. **Admin Check** — Use `WindowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator)`. Print whether the process is elevated. Also check `IsInRole("BUILTIN\\Users")`.
3. **Group Membership Lister** — Enumerate all SIDs in `WindowsIdentity.GetCurrent().Groups`. Translate each to an `NTAccount` (human-readable name). Print all group names.
4. **Token Info Dumper** — P/Invoke `OpenProcessToken` on the current process. P/Invoke `GetTokenInformation` to get `TokenUser`. Print the SID and translated account name.
5. **Privilege Checker** — Enumerate token privileges using `GetTokenInformation(TokenPrivileges)`. Print each privilege name and whether it is enabled, disabled, or disabled-by-default.
6. **SeDebugPrivilege Enabler** — Use `LookupPrivilegeValue` and `AdjustTokenPrivileges` to enable `SeDebugPrivilege` on the current process token. Verify it is now enabled.
7. **Token Handle Opener** — Accept a PID. P/Invoke `OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION)` then `OpenProcessToken`. Print the token's integrity level (Low/Medium/High/System).
8. **Token Duplicator** — Open a process token with `TOKEN_DUPLICATE`. Duplicate it with `DuplicateTokenEx`. Print the type and impersonation level of the new token.
9. **Token Impersonation** — Duplicate a token from a target process. Call `ImpersonateLoggedOnUser`. Print `WindowsIdentity.GetCurrent().Name` — it should reflect the target. Call `RevertToSelf`.
10. **Token Stealing Chain** — Full chain: find a SYSTEM process (e.g., `winlogon.exe`) → enable `SeDebugPrivilege` → `OpenProcess` → `OpenProcessToken` → `DuplicateToken` → `ImpersonateLoggedOnUser` → verify running as SYSTEM → `RevertToSelf`.

### 22. Advanced Injection Techniques

1. **OpenProcess Test** — Accept a PID. Call `OpenProcess(PROCESS_ALL_ACCESS)`. Print whether the handle is valid. Close the handle. Test on your own process first.
2. **VirtualAllocEx in Remote Process** — Open another process. Call `VirtualAllocEx` to allocate 4096 bytes in it. Print the allocated address. Free it with `VirtualFreeEx`.
3. **WriteProcessMemory Test** — Allocate memory in a target process. Write a known byte pattern. Use `ReadProcessMemory` to verify the bytes were written correctly.
4. **CreateRemoteThread Test** — Write a NOP+RET shellcode stub to a remote process. Create a thread pointing to it with `CreateRemoteThread`. Verify it executes without crashing the target.
5. **DLL Path Injection** — Allocate space in a target process for a DLL path string. Write it. Get `LoadLibraryA` address. `CreateRemoteThread` with it. Verify the DLL loads (use a test DLL that writes a file on load).
6. **Process Spawn Suspended** — Use `CreateProcess` with `CREATE_SUSPENDED`. Capture the `PROCESS_INFORMATION`. Print the PID and thread ID. Resume with `ResumeThread`. Confirm process starts.
7. **Thread Context Read** — Suspend a thread. Call `GetThreadContext`. Print `RIP` (instruction pointer), `RSP` (stack pointer), and `RAX`. Resume the thread.
8. **Thread Hijack Simulation** — Spawn a test process suspended. Read its initial thread context. Write NOP+RET shellcode at a `VirtualAllocEx` address. Set `RIP` to point to it. Resume. Observe behavior.
9. **APC Queue Injection** — Allocate shellcode (NOP+RET) in a target process. Use `OpenThread` on each thread and call `QueueUserAPC`. Trigger by waiting for the thread to enter an alertable state.
10. **Process Hollowing Skeleton** — Spawn `svchost.exe -k LocalService` suspended. Read its PEB to find image base. Unmap with `NtUnmapViewOfSection`. Allocate the same size. Write a test PE header pattern. Attempt resume. Document all steps and handle errors.

### 23. EDR / AV Evasion

1. **AMSI Test Baseline** — Run a known AMSI-triggering string through PowerShell and observe the block. Document the error message.
2. **AMSI DLL Locator** — Use `GetModuleHandle("amsi.dll")` and `GetProcAddress` to find `AmsiScanBuffer`. Print the function's memory address and first 16 bytes as hex.
3. **ETW Function Locator** — Locate `EtwEventWrite` in `ntdll.dll`. Print its address and first 16 bytes. Verify the function signature matches expectations.
4. **AMSI Patch** — Write the 6-byte `ret-error` patch to `AmsiScanBuffer` (change it to `mov eax, 0x80070057; ret`). Use `VirtualProtect` to make it writable first. Verify the patch by reading back the bytes.
5. **ETW Patch** — Write a single `0xC3` (ret) byte to `EtwEventWrite`. Use `VirtualProtect`. Verify. Document the impact on event tracing.
6. **XOR String Obfuscator** — Write a build-time tool that takes strings like `"amsi.dll"` and XOR-encodes them with a key. Generate C# source code with the encoded byte arrays. Verify the runtime decoder produces the original strings.
7. **Syscall Number Resolver** — For the functions `NtAllocateVirtualMemory`, `NtWriteVirtualMemory`, `NtCreateThreadEx` — read the syscall number from ntdll's in-memory stub (byte offset 4). Print each syscall number.
8. **Direct Syscall Stub Executor** — Allocate executable memory. Write the syscall stub bytes for `NtAllocateVirtualMemory` (using the resolved number from task 7). Cast to a delegate and call it to allocate memory. Verify allocation succeeded.
9. **ntdll Unhooker** — Read `ntdll.dll` fresh from disk. Compare its `.text` section bytes to the in-memory version. Print any bytes that differ (EDR hook locations). Optionally restore the original bytes.
10. **Sleep Obfuscation Cycle** — Implement a full sleep obfuscation cycle: allocate a "config" buffer in memory → XOR-encrypt it → sleep with jitter → XOR-decrypt → verify integrity before proceeding. Repeat 5 times and print each cycle's timing.

### 24. Credential Access — LSASS & SAM

1. **LSASS PID Finder** — Find the PID of `lsass.exe` using `Process.GetProcessesByName`. Print its PID, session ID, and handle count.
2. **SeDebugPrivilege Enabler** — Enable `SeDebugPrivilege` using `AdjustTokenPrivileges`. Verify it's enabled. (Required before LSASS access.)
3. **LSASS Handle Opener** — Attempt `OpenProcess(PROCESS_ALL_ACCESS, lsassPid)`. Print whether the handle is valid. Immediately close it. (Just verify access — do not read memory.)
4. **MiniDump API Locator** — Use `GetProcAddress` to find `MiniDumpWriteDump` in `dbghelp.dll`. Print the function address to confirm the library loaded.
5. **LSASS MiniDump Writer** — Call `MiniDumpWriteDump` on the LSASS process. Write the dump to `%TEMP%\lsass.dmp`. Print the file size. (Run as administrator.)
6. **comsvcs.dll Dump** — Use `Process.Start` to run `rundll32 comsvcs.dll,MiniDump {pid} C:\Temp\lsass2.dmp full`. Capture stderr. Verify the dump file was created.
7. **SAM Hive Path Finder** — Check if `C:\Windows\System32\config\SAM` exists and is accessible. Print its size and last modified date. Document why direct copy fails while Windows is running.
8. **VSS Shadow Copy Creator** — Use `System.Management` to call `Win32_ShadowCopy.Create`. Print the resulting shadow copy device path.
9. **SAM Hive Extractor via VSS** — Using the shadow copy from task 8, copy `SAM`, `SYSTEM`, and `SECURITY` hives to `C:\Temp\`. Verify all three files are present and non-empty.
10. **Credential Manager Enumerator** — P/Invoke `CredEnumerateW`. Marshal the returned `CREDENTIAL` structs. Print all stored credential target names, types, and usernames. (No password decryption — metadata only.)

### 25. COM Interop & DCOM

1. **WScript.Shell via COM** — Instantiate `WScript.Shell` using `Type.GetTypeFromProgID`. Run `cmd.exe /c echo hello`. Verify execution by checking output.
2. **WScript.Shell Hidden Run** — Use `wsh.Run("cmd.exe /c whoami > C:\\Temp\\out.txt", 0, true)`. Wait for completion. Read and print `C:\Temp\out.txt`.
3. **Shell.Application Object** — Instantiate `Shell.Application`. Call `ShellExecute` to open `notepad.exe`. Verify the process appears in the process list.
4. **WMI via COM** — Instantiate `WbemScripting.SWbemLocator`. Connect to `root\cimv2`. Run `SELECT * FROM Win32_Process`. Print the first 5 process names.
5. **Scheduled Task via COM — Local** — Use `Schedule.Service` COM object to create a scheduled task that runs `cmd.exe /c echo hello > C:\Temp\schtask_test.txt` on boot. Verify the task appears in Task Scheduler.
6. **Scheduled Task Enumerator** — Use `Schedule.Service` to enumerate all registered tasks in `\`. Print task name, path, and next run time for each.
7. **Scheduled Task Cleanup** — Delete the task created in task 5 using `ITaskFolder.DeleteTask`. Verify it no longer appears.
8. **DCOM MMC20 Local Test** — Instantiate `MMC20.Application` targeting `"."` (localhost). Call `ExecuteShellCommand` to run `cmd.exe /c whoami > C:\Temp\mmc_out.txt`. Read and print the output.
9. **DCOM ShellWindows Test** — Instantiate the `ShellWindows` DCOM object (CLSID `9BA05972-...`). Navigate the object hierarchy to call `ShellExecute`. Run `calc.exe`.
10. **Excel COM Macro Runner** — Instantiate `Excel.Application`. Open a test `.xlsm` file containing a macro that writes `"macro executed"` to a file. Call `excel.Run("TestMacro")`. Verify the file was created.

### 26. `System.IdentityModel` — Kerberos & Token Concepts

1. **Auth Type Detector** — Print `WindowsIdentity.GetCurrent().AuthenticationType`. Determine if it is `Kerberos`, `NTLM`, or `Negotiate`.
2. **Domain Membership Check** — Confirm whether the machine is domain-joined by comparing `UserDomainName` vs `MachineName`. Print domain name if joined.
3. **SPN Enumerator** — Using `DirectoryServices`, search for all accounts with `servicePrincipalName=*`. Print each account and its SPNs.
4. **Kerberoastable Account Report** — Combine SPN search with account status (enabled only). Print a Kerberoasting candidate list with username, SPN, and password last set date.
5. **NTLM Hash Calculator** — Implement MD4 using BouncyCastle NuGet (or a pure C# MD4 implementation). Compute the NTLM hash of `"Password123"`. Verify it matches the known value `58A478135A93AC3BF058A5EA0E8FDB71`.
6. **LSA Connection Test** — P/Invoke `LsaConnectUntrusted`. Print the returned LSA handle value. Immediately close it with `LsaDeregisterLogonProcess`. Confirm no errors.
7. **Kerberos Package Lookup** — After connecting to LSA, call `LsaLookupAuthenticationPackage` with the string `"Kerberos"`. Print the returned package ID.
8. **S4U2Self Simulation** — Use `new WindowsIdentity("user@domain.local")` (S4U2Self constructor). Print the identity name and authentication type. Document what privileges are required.
9. **Pass-the-Ticket Skeleton** — Research the `KERB_SUBMIT_TKT_REQUEST` structure. Write the struct definition with correct `[StructLayout]`. Write a function stub that would call `LsaCallAuthenticationPackage` to submit a ticket. Document each field.
10. **Full Kerberos Attack Reference Implementation** — Build a tool that: (1) enumerates Kerberoastable accounts via LDAP, (2) requests a TGS for each SPN using `KerberosRequestorSecurityToken` from `System.IdentityModel`, (3) extracts the ticket bytes, and (4) saves them in a format compatible with hashcat `-m 13100`. Document every step.

---

## Phase 5 — Privilege Escalation (YENİ)

### 27. Token Privilege Abuse — `System.Security.Principal` (Advanced)

1. **Token Privilege Dumper** — Enumerate all privileges on the current token using `GetTokenInformation(TokenPrivileges)`. Flag `SeImpersonatePrivilege`, `SeAssignPrimaryTokenPrivilege`, `SeBackupPrivilege`, `SeRestorePrivilege`, `SeTakeOwnershipPrivilege`, `SeDebugPrivilege` as `[ATTACK-RELEVANT]`.
2. **SeImpersonate Check** — If `SeImpersonatePrivilege` is present, call `DuplicateTokenEx(SecurityImpersonation)` on a SYSTEM token (from an elevated parent) and verify impersonation with `WindowsIdentity.GetCurrent().Name`.
3. **Named Pipe Impersonation (PrintSpoofer Style)** — Create a named pipe (`CreateNamedPipeW`), spawn `cmd.exe /c whoami` as SYSTEM via a service trigger, accept the pipe connection, call `ImpersonateNamedPipeClient`, and print the impersonated identity.
4. **Potato Family Guide** — Document the differences between JuicyPotato / PrintSpoofer / GodPotato / EfsPotato (COM vs pipe vs impersonation + trigger). Implement one core mechanism: pipe → impersonate → trigger SYSTEM callback.
5. **SeBackupPrivilege File Read** — Open any file (including `C:\Windows\System32\config\SAM`) using `CreateFileW` with `FILE_FLAG_BACKUP_SEMANTICS`. Read and print the first 16 bytes as hex.
6. **SeRestorePrivilege DLL Swap** — With `SeRestorePrivilege`, overwrite a test service's DLL/EXE with a marker-writing payload using raw file write. Restart the service and verify.
7. **SeTakeOwnership Exploit** — Use `SetNamedSecurityInfo` / `TakeOwnershipEx` to take ownership of a protected registry key or file, grant `FULL_CONTROL` to yourself, then read the protected content.
8. **Auto-Logon Credential Check** — Read `HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon` for `DefaultUserName` / `DefaultPassword` / `DefaultDomainName`. Print found credentials.
9. **PrintSpoofer Full Chain** — Full chain: spawn trigger → pipe connect → impersonate → `whoami` as SYSTEM → `RevertToSelf` → verify return to original identity. Log every step with timestamps.
10. **Privilege Escalation Path Report** — Combine all token findings into a ranked attack-path report: privilege → technique → expected result → detection surface.

### 28. UAC Bypass Techniques

1. **UAC Level Detector** — Read `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System\ConsentPromptBehaviorAdmin` and `EnableLUA`. Classify the level: `AlwaysNotify`, `Default`, `PromptForConsent`, `AutoDeny`.
2. **AutoElevate Scanner** — Scan `C:\Windows\System32` for executables whose manifest contains `autoElevate=true` (parse embedded manifest or use `mt.exe`). Print the candidate list.
3. **FodHelper Bypass** — Write `HKCU\Software\Classes\ms-settings\Shell\Open\command\(Default)` = path to a test exe, then launch `fodhelper.exe`. Verify the test exe runs with High integrity (`TokenIntegrityLevel`).
4. **EventVwr Bypass** — Repeat the registry redirect technique with `eventvwr.exe`. Verify high-IL execution and clean up the registry value.
5. **ComputerDefaults Bypass** — Repeat with `computerdefaults.exe` (ms-settings scheme). Compare trigger reliability with the other two.
6. **CMSTPLUA COM Bypass** — Instantiate the CMSTPLUA COM object (`CLSID 3E5FC7F9-9A51-4367-9063-A120244FBEC7`) and call `ICMLuaUtil.ShellExec` with an elevated command. Verify execution at high IL.
7. **Mock Folder UAC Bypass** — Implement the trusted-directory DLL planting variant: drop a hijack DLL into a folder that auto-elevated binaries load from, trigger the binary, and verify execution.
8. **UAC Context Check** — Before/after each bypass, print the process integrity level (Low/Medium/High/System) using `GetTokenInformation(TokenIntegrityLevel)` to prove elevation occurred.
9. **Bypass Comparison Report** — Build a table: technique, UAC levels it works at, dependencies (folders/CLSIDs), and detection surface (Event IDs, Sysmon rule hits).
10. **UAC Detection Notes** — Document Event IDs (e.g., 4688 for `fodhelper.exe` launch, 4698 for scheduled tasks) and Sysmon rule suggestions to detect each bypass.

### 29. Service & Binary Privesc

1. **Unquoted Service Path Finder** — Enumerate `HKLM\SYSTEM\CurrentControlSet\Services` for `ImagePath` values containing spaces without quotes. Print the vulnerable service, path, and writable folder candidates.
2. **Writable Service Binary Check** — For every service, test whether its binary/DLL folder is writable by the current user (ACL check via `GetEffectiveRightsFromAcl` or a write test). Flag matches.
3. **Weak Service Permissions** — Use `QueryServiceObjectSecurity` to read each service's DACL. Flag services granting `SERVICE_CHANGE_CONFIG`, `SERVICE_START`, or `SERVICE_ALL_ACCESS` to non-admins.
4. **Unquoted Path Exploit** — Drop a marker-writing test exe into the first writable folder of a vulnerable unquoted path. Stop/start the service (`sc stop/start` via P/Invoke `ControlService`/`StartService`). Verify execution.
5. **Service Binary Replacement** — Back up a test service's binary, replace it with a payload that writes a marker file, restart the service, verify, and restore the original binary.
6. **AlwaysInstallElevated Exploit** — Check `HKCU\...\Installer\AlwaysInstallElevated` and `HKLM\...\AlwaysInstallElevated` = 1. If set, build a malicious MSI with WiX (`candle.exe` + `light.exe`) that writes a file as SYSTEM, install it, and verify.
7. **DLL Hijack on Services** — Find a service that loads a DLL from a writable path. Craft an export-forwarding proxy DLL (forwarding to the legitimate DLL) that also writes a marker. Place it and restart the service.
8. **Injection-Based Privesc** — With `SeDebugPrivilege`, open an elevated SYSTEM process (e.g., `winlogon.exe`), duplicate its token, impersonate, and verify SYSTEM identity (ties into M21/M27).
9. **Service Control Wrapper** — Write a complete `sc`-style P/Invoke wrapper: `OpenSCManager`, `OpenService`, `QueryServiceConfig`, `ChangeServiceConfig`, `ControlService`, `StartService`, `DeleteService`.
10. **Service Exploit Report** — Combine all service findings into a ranked report: service → vulnerability → exact exploit steps → cleanup steps.

### 30. Registry & Filesystem Privesc

1. **Writable PATH Folder Scan** — Split `%PATH%` and test each folder for write access. Flag DLL-planting candidates (folders searched before System32 by a target app).
2. **Startup Folder Check** — Write-test `%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup` and the `ProgramData` equivalent. Place a test `.lnk` and verify it survives.
3. **HKCU Autorun Abuses** — Enumerate `HKCU\...\Run` and `RunOnce` entries; flag values pointing to writable, missing, or user-modifiable paths.
4. **DLL Search Order Hijack** — Pick a known application, enumerate its DLL search order (app dir → System32 → PATH), find a missing DLL it loads, and place an export-forwarding proxy DLL.
5. **Scheduled Task Overwrite** — Enumerate scheduled tasks (via `schtasks` or COM) whose `Action\Execute` points to a writable path. Replace the binary and trigger the task.
6. **PATH Hijack via HKCU\Environment** — Add a writable folder to the user's `Path` registry value, place a hijack DLL matching a commonly invoked command, and verify it gets loaded first.
7. **Writable Service Registry Keys** — Check service registry keys (`HKLM\SYSTEM\CurrentControlSet\Services\<svc>`) for non-admin write access. Abuse `FailureCommand` or `ImagePath` to run a payload at next failure/restart.
8. **Folder ACL Checker** — Build a reusable helper: given a path, return effective permissions for the current user using `GetEffectiveRightsFromAcl`. Print full/modify/write/read flags.
9. **Exploit Chain Builder** — Automatically combine all findings (PATH, Startup, Autorun, tasks, service keys) into a ranked exploit plan with exact steps and success criteria.
10. **Filesystem Privesc Report** — Final report: each finding with CVSS-style severity, exploitation steps, and remediation (remove write ACL, quote service paths, etc.).

### 31. Patch Audit & Exploit Suggester

1. **Patch Level Audit** — Enumerate installed KB updates via `wmic qfe list` or `HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\HotfixID`. Print count and newest KB.
2. **Kernel Version Fingerprint** — Build a full fingerprint string: `OS ProductName + CurrentBuild + UBR + architecture + hotfix list hash`. Print it for correlation.
3. **Exploit Candidate Research** — Given the fingerprint, research public exploits for missing patches (search exploit-db/GitHub). Build a candidate table: CVE, affected builds, PoC availability.
4. **Local Exploit Suggester** — Write a tool that matches the fingerprint + hotfix list against an embedded CVE database (mirror of `windows-exploit-suggester` logic) and prints applicable candidates.
5. **CVE PoC Lab** — Set up an isolated lab VM (Windows 7 / Server 2008 R2, EOL build). Compile and test only lab-approved PoCs. Never run against production.
6. **Exploit Reliability Scoring** — Rate each candidate: impact (code exec vs DoS), reliability (public PoC quality), and detection risk (AV/EDR signatures, crash likelihood).
7. **Fallback Strategy** — Document non-kernel fallbacks when no kernel exploit applies: token abuse (M27), service misconfig (M29), UAC (M28), credentials (M33).
8. **Lab Validation Matrix** — Track which candidates were verified in the lab, with success/failure and crash notes. Mark candidates to avoid in real ops.
9. **Mitigation Notes** — For each candidate, write impact assessment and remediation (patch KB number, workaround, config hardening).
10. **Final Privesc Report** — Merge kernel + config-based findings into one prioritized escalation plan: expected success rate, detection risk, and time-to-exploit per path.

---

## Phase 6 — Lateral Movement (YENİ)

### 32. Remote Execution

1. **WMI Remote Exec (Extended)** — Extend M12 task 10: accept target, username, password; use `ConnectionOptions` + `ManagementScope`; run `Win32_Process.Create`; capture return value and read back output files with exit codes.
2. **PSExec-Style SMB Exec** — Implement the core PSExec mechanism: copy a service binary to `\\target\ADMIN$`, call `OpenSCManager(\\target)` → `CreateService` → `StartService` → wait → `DeleteService`. Clean up the binary.
3. **WinRM / PowerShell Remoting** — Use `WSManConnectionInfo` + `RunspaceFactory` to run `Invoke-Command` on a remote host with credentials. Print stdout/stderr and exit code.
4. **Scheduled Task Remote** — Use Task Scheduler COM (`Schedule.Service`) with remote host + credentials to register a task that runs a command immediately, then delete it.
5. **DCOM MMC20 Lateral** — Extend M25 task 8 to a remote hostname: instantiate `MMC20.Application` on `\\target`, call `ExecuteShellCommand`, and retrieve output via a file share.
6. **DCOM ShellWindows Lateral** — Extend M25 task 9 to a remote host: instantiate `ShellWindows` on `\\target` and execute a command via the item's `Document.Application.ShellExecute`.
7. **DCOM Excel Lateral** — Instantiate `Excel.Application` on a remote host and run a macro or `DDEInitiate` command. Verify output file creation on a share.
8. **RDP Session Hijack** — Use `WTSOpenServer`, `WTSEnumerateSessions`, and `WTSConnectSession` P/Invoke to hijack an existing RDP session (tscon equivalent, requires SYSTEM). Lab only.
9. **SCM Remote Service Create** — Raw API path: `OpenSCManager(\\target\IPC$)` → `CreateServiceW` (marker binary path on ADMIN$) → `StartService` → `DeleteService`. Print each Win32 error code.
10. **Execution Method Comparison** — Build a table: method (WMI/PSExec/WinRM/DCOM/task), required ports (135/445/5985/5986), privileges, artifacts left, and detection events (Sysmon 1, 13, 4698).

### 33. Credential-Based Movement

1. **NTLM Hash → WMI (Pass-the-Hash)** — Connect to remote WMI using an NTLM hash instead of a password (`ConnectionOptions.Password` = hash, `Authentication = PacketPrivacy`). Document why NTLM PTH works (challenge-response, no plaintext needed).
2. **Over-Pass-the-Hash** — Use the RC4/AES hash of a user to request a TGT via `KERB_ECRYPT` concepts; perform the operation with Rubeus (`asktgt /rc4`) and document the in-memory ticket.
3. **Pass-the-Ticket** — Build `KERB_SUBMIT_TKT_REQUEST` and call `LsaCallAuthenticationPackage` to inject a ticket into the current logon session. Use a Rubeus-obtained `.kirbi` in the lab.
4. **SMB Share Enumeration** — Use `WNetOpenEnum` / `WNetEnumResource` to enumerate shares on `\\target` (IPC$, ADMIN$, C$). Print share names, types, and accessibility.
5. **Share Loot Hunter** — Recursively search enumerated shares for high-value files: `.txt`, `.xls`, `.kdbx`, `.rdp`, `.config`, `.ps1`. Print paths and sizes. Don't download — just map.
6. **Remote Credential Dump** — Trigger a `comsvcs.dll` MiniDump on a remote host via WMI/SCM (`rundll32 comsvcs.dll,MiniDump <pid> \\attacker\share\lsass.dmp full`), then pull the dump back over SMB.
7. **NTLM Relay Concepts** — Set up a lab with Responder + ntlmrelayx. Write a C# SMB signing probe (`NEGOTIATE` + `SESSION_SETUP` check) that reports whether targets enforce SMB signing.
8. **LAPS Password Reader** — If LAPS is deployed, read `ms-Mcs-AdmPwd` for computer accounts via LDAP with a permitted account. Print passwords (lab only). Document the `LAPS-CSE` detection side.
9. **GPP Password Decryptor** — Locate `SYSVOL\...\Groups.xml` with `cpassword`, extract the value, and decrypt it with the well-known AES key (32-byte static key, CBC). Print the plaintext password.
10. **Credential Movement Report** — Combine findings: which credentials/hashes work against which targets, and the recommended movement order.

### 34. Lateral Movement Advanced

1. **RDP Check & Enable** — Test `TCP 3389` on targets; if admin and RDP disabled, enable it remotely via registry (`fDenyTSConnections = 0`) and start the `TermService` via SCM.
2. **WinRM Check & Enable** — Test `5985/5986`; if disabled, enable WinRM remotely via WMI (`Set-WSManQuickConfig` equivalent) and configure trusted hosts.
3. **SMB Relay Lab** — Document the full chain: Responder (capture) → ntlmrelayx (relay to SMB) → SMB exec. For each stage, note the equivalent C# APIs you would use (`SspiCli`, `NtlmSession`, etc.).
4. **DCOM Lateral Hunter** — Scan targets for open `RPC 135` (and `DCOM 49152+` dynamic ports), then test which DCOM objects are instantiable remotely on each host.
5. **Multi-Hop Chain** — Move A → B → C: from host A, use WMI to reach B; from B, use SCM to reach C. Print the path taken, credentials used per hop, and any artifacts left behind.
6. **Hidden Remote Service** — Create a service on a remote host that mimics a legitimate Windows service name/description (e.g., "Windows Time Service" with a plausible binary path in System32) running your payload. Lab only.
7. **Target Enumeration** — Before moving: enumerate live hosts (ping sweep), open admin ports (135/139/445/5985), admin shares, and local admin rights on each. Output a movement-ready target list.
8. **Firewall-Friendly Movement** — Document which methods work through the default Windows Firewall profile: WinRM (5985, enabled by default in domain), SMB (445, usually open internally), RPC (135 + dynamic).
9. **Offline Lab Automation** — Write a script that automates the entire lab lateral chain (deploy targets → execute → verify → clean) so you can repeat tests deterministically.
10. **Lateral Movement Report** — Final report: movement graph (nodes = hosts, edges = method + credential used), detection events per hop (4624 logon type 3, 4698, Sysmon 1), and mitigations.

---

## Phase 7 — Modern AD Attacks (YENİ)

### 35. Delegation Attacks

1. **Unconstrained Delegation Hunter** — LDAP search for `(userAccountControl:1.2.840.113556.1.4.803:=524288)` (0x80000). Print computers and users with unconstrained delegation.
2. **Constrained Delegation Hunter** — LDAP search for `(msDS-AllowedToDelegateTo=*)`. Print each account and its allowed SPN list. Note `TrustedToAuthForDelegation` (0x80000 on userAccountControl + msDS-AllowedToDelegateTo).
3. **RBCD Hunter** — LDAP search for `(msDS-AllowedToActOnBehalfOfOtherIdentity=*)`. Print targets and the principals allowed to act on their behalf.
4. **Constrained Abuse (S4U2Self + S4U2Proxy)** — For a controlled constrained-delegation account, request a TGT (S4U2Self) then a service ticket (S4U2Proxy) for the target SPN using `LsaCallAuthenticationPackage` or Rubeus `s4u`. Print the resulting ticket details.
5. **RBCD Abuse** — Create a computer object (or use a controlled one), set `msDS-AllowedToActOnBehalfOfOtherIdentity` on the target to your computer's SID, request an admin-level service ticket, and authenticate. Lab only.
6. **Silver Ticket** — With a service account's hash, forge a TGS for that SPN (`Rubeus silver` or manual KERB_ECRYPT construction). Access the service as any user. Document PAC contents.
7. **Golden Ticket** — With the `krbtgt` hash, forge a TGT (`Rubeus golden`). Print lifetime, SIDs, and what resources it unlocks. Document mitigations (krbtgt rotation, TGT lifetime policy).
8. **SID History Injection** — Add an Enterprise Admin SID to the forged ticket's SID history. Explain the effect on access checks (membership-based, not PAC-validated in all paths).
9. **Delegation Detection Review** — Document Event IDs: 4768 (TGT), 4769 (TGS, note `S4U2Self` = 0x12 encryption, service-only ticket flag), 4624 (logon type 3). List Sysmon rules for delegation abuse.
10. **Delegation Attack Report** — Rank all delegation paths found: target → technique → credentials needed → impact (which services compromised).

### 36. ADCS (Active Directory Certificate Services)

1. **CA Discovery** — LDAP query `(objectClass=pKIEnrollmentService)` and `certutil -config` enumeration. Print CA names, DNS names, and whether they're root or subordinate.
2. **Template Enumeration** — Enumerate all certificate templates: EKUs, enrollment rights (`nTSecurityDescriptor`), `mspki-certificate-name-flag` (SAN allowed), and `pKIExtendedKeyUsage`. Print a full template list.
3. **ESC1 Hunter** — Find templates with: Client Auth EKU, low-privilege enrollment rights, and SAN allowed (`ENROLLEE_SUPPLIES_SUBJECT`). Print candidate template names — these allow domain escalation.
4. **ESC2/ESC3 Hunter** — Find ESC2 templates (Any Purpose EKU, low-priv enrollment) and ESC3 (enrollment agent templates + a subordinate client-auth template). Print candidates.
5. **ESC4/ESC6/ESC8 Notes** — Document: ESC4 (write access to template ACL → change flags), ESC6 (`EDITF_ATTRIBUTESSUBJECTALTNAME2` CA flag), ESC8 (NTLM relay to CA web enrollment `/certsrv`).
6. **ESC1 Exploit** — Using `ICertRequest` COM (`CertRequest.Submit`), request a certificate with SAN = a Domain Admin user. Export the PFX. Verify the certificate's SAN. Lab only.
7. **ESC6 Exploit** — If the CA has the SAN flag set, request a certificate with SAN via `certreq -attrib "SAN:..."`. Verify the issued cert contains the target SAN.
8. **Certificate → Ticket** — Use the PFX with `Rubeus asktgt /certificate:cert.pfx /password:...` to request a TGT as the SAN user. Use the TGT to access a DC share and verify Domain Admin rights.
9. **ADCS Detection Review** — Document Event IDs: 4886/4887 (cert services issued/denied), 4768/4769 (Kerberos after cert auth), 4624. List anomalies (unusual SAN requests, non-standard enrollment times).
10. **ADCS Attack Report** — Produce a full report: CA inventory, vulnerable templates (ESC1–8), proof-of-concept steps, and remediation (disable vulnerable templates, require CA manager approval).

### 37. ACL Abuse & BloodHound

1. **BloodHound Data Collector** — Build a SharpHound-lite: collect users, groups, computers, sessions, and ACLs via LDAP. Output JSON in a format BloodHound can ingest (or CSV for manual graphing).
2. **GenericAll Hunter** — For the current user's SID, search `nTSecurityDescriptor` ACEs for `GenericAll` / `GenericWrite` / `WriteDacl` / `WriteOwner` rights over other objects. Print the attack paths.
3. **ForceChangePassword** — With `GenericAll` over a target user, reset their password via LDAP `Modify` (`unicodePwd` attribute). Verify by authenticating as that user. Lab only.
4. **AddMember** — With `GenericWrite` over a group (or `GenericAll` on a user in it), add yourself to the group via LDAP `member` attribute modification. Verify with `net group`.
5. **WriteDACL** — With `WriteDacl` over a target object, modify its DACL to grant yourself full control, then perform the follow-on attack (password reset / group add).
6. **GPO Abuse** — Find GPOs with writable ACLs for your user. Modify the GPO's `scripts.ini` or `UserScripts` to add a logon script payload in SYSVOL. Trigger and verify. Lab only.
7. **Path Finder** — Implement mini-BloodHound pathfinding: model principals/objects as graph nodes, ACL edges as directed edges, and run BFS from your user to Domain Admins. Print the shortest paths.
8. **Session Hunting** — Use `NetWkstaUserEnum` (or `NetSessionEnum` on DCs) to enumerate active sessions. Build a user → machine map and flag high-value sessions (admins logged into servers).
9. **Trust Enumeration** — Enumerate domain trusts via `DsEnumerateDomainTrusts` (`GetTrust` equivalent). Print trust type (parent/child/external/forest) and direction. Flag exploitable bidirectional trusts.
10. **ACL Detection Review** — Document Event IDs: 4738 (user changed), 4728/4729 (group add/remove member), 5136 (LDAP modification), 4662 (object access). List detection rules for each abuse.

### 38. Kerberos Deep

1. **TGT via SSPI** — Extend M26: acquire a service ticket via `KerberosRequestorSecurityToken` and parse the `AP-REQ`/ticket flags, realm, sname, and validity times. Print all fields.
2. **RC4 vs AES** — Request tickets with RC4-HMAC vs AES256-CTS etypes; print the `KERB_ECRYPT` etype value in the returned ticket. Document why RC4 is deprecated but still attack-relevant.
3. **Ticket Structure Parser** — Write a minimal ASN.1/DER parser for ticket files: extract realm, sname, flags, enc-part key type, and validity times. Print a human-readable summary of any `.kirbi`.
4. **PTT Library** — Wrap `KERB_SUBMIT_TKT_REQUEST` + `KERB_RETRIEVE_TKT_REQUEST` into a reusable class (`TicketStore.Submit(byte[] kirbi)`, `Retrieve()`). Add error handling per LSA return code.
5. **Kerberoast with RC4** — Extend M26 task 10: request TGS with RC4 etype and export the hash in `$krb5tgs$23$*` format compatible with hashcat `-m 13100`. Verify cracking in the lab.
6. **ASREProast Rubeus-Compatible** — For ASREProastable users (M13 task 7), request an AS-REP and export `$krb5asrep$23$*` hashes compatible with `-m 18200`. Verify in lab.
7. **Unconstrained Delegation Coercion** — Use the MS-RPRN `RpcRemoteFindFirstPrinterChangeNotification` (SpoolSample/PrinterBug) to coerce a DC to authenticate to a machine with unconstrained delegation. Capture the TGT and document each step. Lab only.
8. **PAC Validation** — Document the PAC (Privilege Attribute Certificate) structure, how KDC validates it, and why MS14-068 (PAC forgery) was fixed. Explain modern constraints (PAC signature with krbtgt key).
9. **Kerberos Detection Review** — Document anomalies: unusual etypes (RC4), TGS requests outside work hours, service tickets to unusual SPNs, and Event IDs 4768/4769/4771 patterns.
10. **Kerberos Toolkit Report** — Combine everything into a toolkit: SPN enumeration → roast candidates → TGS/AS-REP capture → hash export → offline cracking results → PTT. Print the full workflow output.

---

## Phase 8 — Advanced EDR Evasion (YENİ)

### 39. Indirect Syscalls

1. **Syscall Primer** — Document with code comments: user/kernel transition, ntdll syscall stubs, EDR hook placement (first bytes), direct syscalls vs indirect syscalls, and the `syscall` instruction flow (x64, r10).
2. **HellsGate Resolver** — Parse ntdll's export table, scan each stub for the `0x4C 0x8B 0xD1 0xB8` (mov r10, rcx; mov eax, <num>) pattern, and resolve syscall numbers for `NtAllocateVirtualMemory`, `NtProtectVirtualMemory`, `NtWriteVirtualMemory`, `NtCreateThreadEx`.
3. **Direct Syscall Executor** — Build x64 stubs (`mov r10, rcx; mov eax, <number>; syscall; ret`) in executable memory. Call `NtAllocateVirtualMemory` directly to allocate RWX memory. Print the returned base address.
4. **Indirect Syscall** — Instead of executing your own stub, jump into ntdll's actual `syscall` instruction (`ret2syscall`): set `r10`/`eax`, then `jmp` to the ntdll `syscall; ret` gadget. Verify with a debugger that execution passes through ntdll.
5. **Syscall Stub Builder** — Generate all four API stubs with correct x64 ABI (rcx→r10 shuffle, argument order). Encapsulate them in a class with `delegate* unmanaged` or `Marshal.GetDelegateForFunctionPointer`.
6. **Full Unhooked Execution Chain** — Chain: indirect `NtAllocateVirtualMemory` → `NtWriteVirtualMemory` (copy shellcode) → `NtProtectVirtualMemory` (RX) → `NtCreateThreadEx`. Execute a safe test shellcode (NOP+RET) and confirm no crash.
7. **Argument Validation** — Add pre-call validation to prevent crashes: `NtAllocateVirtualMemory` region size checks, `NtProtectVirtualMemory` base alignment, and status-code checks after each call.
8. **Syscall Benchmarks** — Time each API called via hooked ntdll vs indirect syscall (100 iterations each). Print the timing delta — a consistent gap can indicate hooking.
9. **Syscall Detection Review** — Document detection vectors: ETW kernel telemetry, instrumented syscalls (Windows 11 24H2 / Redmond), `call stack spoofing` counters, and KPP (PatchGuard) constraints.
10. **Evasion Module Documentation** — Write a technical README for the module: when to use indirect syscalls vs unhooking vs obfuscation, and the expected trade-offs per EDR family.

### 40. Advanced Injection

1. **Module Stomping** — Load a legitimate DLL (e.g., `amsi.dll`), locate its `.text` section, `VirtualProtect` to RWX, overwrite with shellcode, and execute via a callback or thread. Restore original bytes afterwards.
2. **Callback Execution** — Implement shellcode execution through 3+ callback APIs: `EnumFonts`, `EnumDateFormats`, `CertEnumSystemStore`, `TpAllocWork`, or `CreateTimerQueueTimer`. Compare which are most stealthy.
3. **Early Bird APC** — Create a process suspended (`CREATE_SUSPENDED`), allocate + write shellcode with `VirtualAllocEx`/`WriteProcessMemory`, `QueueUserAPC` on the main thread, then `ResumeThread`. Verify execution.
4. **Threadless Injection** — Use `SetWindowsHookEx` with a payload in a shared memory region so the hook callback triggers without creating a thread. Verify the hook fires on the target window's message loop.
5. **Phantom DLL Hollowing** — Map a DLL image with `NtCreateSection` + `NtMapViewOfSection`, write shellcode into an image gap (between sections), and execute from a new thread. Document each step and handle status codes.
6. **Fiber Execution** — `ConvertThreadToFiber` → allocate shellcode → `CreateFiber` pointing at it → `SwitchToFiber`. Print fiber state before/after. Verify the shellcode runs in the fiber context.
7. **TLS Callback Execution** — Craft a DLL with a TLS callback, load it in memory (`Assembly.Load`-style manual mapping or `NtMapViewOfSection`), and let the loader invoke the TLS callback before `DllMain`. Verify the callback executed.
8. **Injection Telemetry Map** — Build a matrix: technique → events generated (Thread creation 4688/Sysmon 8, Image load 7, Memory allocation) → which EDR products flag it.
9. **Injection Method Selector** — Write a decision-tree helper: given (EDR present, target privilege, need for stealth), recommend the best injection technique and explain why.
10. **Advanced Injection Report** — Lab-verify each technique in the same environment; produce a comparison report with success rate, crash rate, and detection events observed.

### 41. Unhooking & Obfuscation

1. **Full Unhooker** — Read ntdll.dll fresh from disk (or a known-good copy), compare `.text` section bytes with the in-memory module, and restore any differing bytes via `VirtualProtect` + `memcpy`. Print the count of restored bytes.
2. **DLL Hollowing Unhook** — Create a suspended process, map a fresh ntdll into it (`NtMapViewOfSection`), and copy pristine `.text` bytes over the hooked module. Verify with a byte-diff.
3. **DInvoke API Hashing** — Implement DJB2/MurmurHash hashing for API names. Resolve exports by walking the PEB → export table and comparing hashes — no `GetProcAddress` (which EDRs hook). Test with 5 APIs.
4. **String Encryption Framework** — Write a build-time string encrypter: encrypt all sensitive strings (DLL names, API names, URLs) with AES + random IV, emit C# source with encrypted byte arrays, and decrypt at runtime.
5. **Payload Obfuscation** — Embed shellcode as AES-encrypted bytes; derive the master key from machine-specific data (e.g., MAC + volume serial via `Environment`/`DriveInfo`). Decrypt at runtime only.
6. **HoneyTokens** — Add decoy strings and fake API call sequences (e.g., benign WMI queries, innocuous file reads) to confuse static analysis. Verify the decoys never execute.
7. **Stack Spoofing Skeleton** — Implement the concept: save the real stack pointer, overwrite return addresses with a legitimate frame (e.g., `WaitForSingleObject`), then restore. Document limitations and provide the skeleton.
8. **Obfuscation Detection Review** — Document static/dynamic analysis limits: entropy-based detection (high entropy = suspicious), string scanning, API call graph analysis, and behavior analysis.
9. **Static Analysis Resistance Test** — Run your obfuscated binary through `strings`, `floss`, and a quick IDA/Ghidra review. Document which sensitive strings/APIs remain visible and improve until clean.
10. **Final Evasion Toolchain** — Combine: XOR string obfuscation (M23) + API hashing + AES payload + indirect syscalls (M39) into one compiled sample. Document the layered defense-in-depth of the toolchain.

---

## Phase 9 — Persistence (YENİ)

### 42. WMI & COM Persistence

1. **WMI Event Subscription** — Create `__EventFilter` (`Win32_ProcessStartTrace` on `notepad.exe`), `__ActiveScriptEventConsumer` (PowerShell one-liner writing a marker), and a filter-consumer binding. Trigger with notepad and verify.
2. **WMI Consumer Enumeration** — Enumerate all `__EventConsumer`, `__EventFilter`, and `__FilterToConsumerBinding` instances via WMI. Flag suspicious ones (non-Microsoft script consumers, unusual queries).
3. **WMI Cleanup** — Delete the binding, consumer, and filter objects you created (in that order). Verify no instances remain — leave the lab clean.
4. **COM Hijack Discovery** — Scan `HKCU\Software\Classes\CLSID` for values where `InprocServer32` is missing or writable. Cross-reference with the list of CLSIDs that `explorer.exe` loads at logon.
5. **COM Hijack Persistence** — Set a hijacked CLSID's `InprocServer32` to a test DLL that writes a marker on `DllMain`. Restart explorer.exe and verify the DLL loaded. Lab only.
6. **TreatAs Hijack** — Use the `TreatAs` registry key to redirect a COM object to your implementation (the system checks TreatAs before the real CLSID). Verify redirection with a test instantiation.
7. **WMI Persistence Detection Review** — Document detection: Sysmon Event 1 (WMI consumer process), Event 13/12 (registry value set for COM hijack), WMI activity telemetry, and Microsoft Defender's `PersistenceHunting` alerts.
8. **COM Hijack Trigger Verification** — Write a trigger harness: enumerate CLSIDs, instantiate each via COM, and report which ones actually load (verifying hijack viability without waiting for logon).
9. **Persistence Reliability Test** — For each persistence method, simulate reboot/lock/unlock/logoff and verify the payload fires every time. Print pass/fail per method.
10. **WMI/COM Persistence Report** — Final report: each method with trigger, payload requirements, detection surface, and cleanup steps.

### 43. Advanced Persistence

1. **IFEO Debugger Persistence** — Set `HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\notepad.exe\Debugger` to a payload path. Launch notepad and verify the payload runs instead. Lab only — restore afterwards.
2. **AppInit_DLLs** — Set `HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows\AppInit_DLLs` + `LoadAppInit_DLLs = 1`. Document x86/x64 implications, SecureBoot restrictions, and which processes load it.
3. **DLL Search Order Hijack (Persistence)** — Identify a persistently-running app (e.g., a scheduled/boot service), find a missing DLL in its search path, and plant an export-forwarding proxy DLL that writes a marker on load.
4. **BITS Job** — Create a Background Intelligent Transfer Service job via COM (`BackgroundCopyManager`), set a `NotifyCmdLine` that runs your payload on job completion. Verify the notification fires.
5. **Scheduled Task OnLogon** — Use `Schedule.Service` to create a task that runs at logon as SYSTEM with a hidden window. Trigger by logoff/logon and verify execution. Clean up afterwards.
6. **Startup .lnk** — Drop a `.lnk` with a legitimate icon into the Startup folder pointing to your payload (or a benign binary with a side-loaded DLL). Verify it fires at next logon.
7. **Print Monitor** — Write `HKLM\SYSTEM\CurrentControlSet\Control\Print\Monitors\<name>\Driver` = your DLL path. The spooler loads it at startup. Verify with a marker DLL. Lab only.
8. **Service Persistence** — Create a service with a legitimate-looking name/description (e.g., "Windows Update Service") and an ImagePath pointing to your payload with plausible arguments. Start and verify. Clean up.
9. **Multi-Persistence Chain** — Install 3 different persistence mechanisms (e.g., WMI event + IFEO + scheduled task). Verify all three fire, then remove all three and confirm a clean system.
10. **Persistence Cleanup Tool** — Write a C# tool that removes all artifacts created by this module: registry values, DLLs, tasks, services, WMI objects. Verify removal and report failures.

---

## Phase 10 — C2 Framework & BOF (YENİ)

### 44. BOF Development & C2 Integration

1. **BOF Toolchain** — Set up mingw-w64 (`x86_64-w64-mingw32-gcc`). Write a minimal `hello-world` BOF using `beacon.h` (include from Cobalt Strike/Sliver BOF headers). Compile to a `.o` and verify it loads in your C2 with `inline-execute`.
2. **BOF Arguments** — Implement `BeaconDataParse`, `BeaconDataInt`, `BeaconDataShort`, `BeaconDataLength`, `BeaconDataExtract`, and `BeaconPrintf`. Build a BOF that accepts an argument (e.g., target path) and prints it back.
3. **BOF Port Scanner** — Write a BOF that scans a port range using raw Winsock (`WSAStartup`, `socket`, `connect` with timeouts) and prints open ports via `BeaconPrintf(CALLBACK_OUTPUT, ...)`.
4. **BOF Netstat** — Implement a BOF using `GetTcpTable`/`GetUdpTable` to list listening ports and connections, printing them formatted. Handle `ERROR_INSUFFICIENT_BUFFER` retry logic.
5. **Aggressor Script** — Write an Aggressor script (`.cna`) that loads your BOFs with custom aliases, e.g., `bof-scan 10.0.0.5 1-1024`, and formats output. Document command registration and argument passing.
6. **External C2 Spec** — Implement a minimal External C2 client/server pair: a C# client connects to your C2's external C2 pipe (named pipe), relays tasks to an implant, and returns output. Document the message framing (length-prefixed, little-endian).
7. **Custom C2 Profile** — Write a Malleable C2 profile: HTTP GET/POST URIs (`/api/ping`, `/api/data`), custom User-Agent, jitter, sleep, data transforms (base64url, netbios). Explain each block's purpose.
8. **Sliver Integration** — Generate a Sliver implant (`generate --mtls` or HTTP), compile your BOFs for Sliver (`sliver-bof` format), load them (`bofs load`), and run an operator workflow: recon → privesc → lateral using your BOFs.
9. **BOF Detection Review** — Document BOF detection: ETW thread start telemetry, new thread creation (Sysmon 8), module loads (Sysmon 7), and how in-memory BOF execution differs from file-based payloads.
10. **BOF Development Report** — Write a technical report: toolchain setup, BOF API reference (your implementations), example outputs, and integration notes for CS/Sliver/Havoc.

---

## Phase 11 — OPSEC & Anti-Forensics (YENİ)

### 45. OPSEC & Artifact Hygiene

1. **Event Log Access** — Enumerate event log names and read permissions (`EventLog.GetEventLogs()`, security descriptor check). Document who can read the Security log (usually admins only) and why that matters.
2. **Log Clearing** — P/Invoke `ClearEventLog` on a test log (lab only). Document what artifacts clearing leaves behind (cleared marker, gaps in 1102/104 events).
3. **PowerShell Log Evasion** — Patch `System.Management.Automation.Utils` (ScriptBlock logging) in memory via reflection. Run a test script and verify no ScriptBlock log entry is created. Lab only.
4. **Timestomping** — Use `SetFileTime` to change a file's creation/modified/access times. Verify with `dir` and PowerShell `Get-Item`. Document $UsnJrnl-based detection caveats.
5. **ADS Usage** — Write a payload to a file's Alternate Data Stream (`type payload.exe > legit.txt:payload.exe` or P/Invoke). Execute from the ADS. Verify with `streams.exe` / `dir /r`.
6. **Prefetch Awareness** — Document how `.pf` files in `C:\Windows\Prefetch` reveal executed binaries. In the lab, disable prefetch (`NtfsDisableLastAccessUpdate` / `EnablePrefetcher = 0`) and test whether your test binary leaves a trace.
7. **ShimCache / AmCache Awareness** — Document `HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\AppCompatCache` and `C:\Windows\AppCompat\Programs\Amcache.hve` — they record every executed binary even after deletion.
8. **MFT / $UsnJrnl Awareness** — Document how `$UsnJrnl` records file creation/deletion/timestamp changes and why timestomping is detectable via USN journal deltas.
9. **Artifact Cleanup Tool** — Write a C# tool that removes all test artifacts from a lab machine: temp files, log entries, registry values, prefetch copies, ADS streams. Handle permission failures gracefully and report leftovers.
10. **OPSEC Checklist** — Build a pre-execution checklist: confirm local admin, check EDR presence (M12 task 5), verify time window, ensure cleanup plan exists, use crash-safe design (wrap payloads in try/catch with no default execution paths).

---

## Phase 12 — Capstone (YENİ)

### 46. Full Chain Capstone

1. **Scenario Design** — Design a realistic lab: 3 hosts (Workstation, DC, File Server) in a domain (`corp.local`). Objective: reach Domain Admin and exfil a file. Document the attack plan before starting.
2. **Initial Access** — Build a C# dropper using evasion techniques from M23/M39/M41 (XOR strings, indirect syscalls, AES-encrypted stage). Deploy it to the Workstation and verify it survives AV/EDR in the lab.
3. **Recon** — Run modules 1, 7, 12, 13 in sequence: environment recon, network recon, WMI enumeration, AD enumeration. Produce a consolidated recon report with findings and next-step recommendations.
4. **Privilege Escalation** — Apply M27–M31 techniques based on recon findings (token abuse, service misconfig, UAC, patch audit). Document which path succeeded and why.
5. **Persistence** — Install persistence from M42–M43 (e.g., WMI event subscription + scheduled task). Verify it survives a reboot and document the detection surface.
6. **Credential Access** — Dump credentials: LSASS MiniDump (M24), DPAPI + browser creds (M15), SAM via VSS (M24). Extract hashes and crack weak ones offline.
7. **Lateral Movement** — Use M32–M34: from Workstation to File Server (WMI or PSExec), then to DC (credential-based movement). Verify each hop with a whoami check.
8. **AD Attacks** — Apply M35–M38: Kerberoast candidates, ADCS ESC1 if applicable, delegation abuse. Achieve Domain Admin and print proof (e.g., `net group "Domain Admins"` output).
9. **Exfiltration** — Staged exfil pipeline: compress (GZip) → encrypt (AES, M14) → chunk (512-byte) → send over HTTP or DNS covert channel (M5/M6 style). Verify the server reassembles the original file.
10. **Cleanup, Detection Review & Final Report** — Remove all artifacts with your cleanup tool (M45). Review Sysmon/Event Logs for your actions. Write a professional final report: timeline, findings with CVSS, IoCs, and mitigations — generated by a C# Markdown/HTML report tool.

---

## Ümumi Statistika / Summary

| Phase | Modules | Tasks |
|:---:|---|:---:|
| 1 — Foundation | 1–4 | 40 |
| 2 — Core Red Team | 5–11 | 70 |
| 3 — Post-Exploitation | 12–18 | 70 |
| 4 — Advanced / EDR Evasion | 19–26 | 80 |
| 5 — Privilege Escalation | 27–31 | 50 |
| 6 — Lateral Movement | 32–34 | 30 |
| 7 — Modern AD Attacks | 35–38 | 40 |
| 8 — Advanced EDR Evasion | 39–41 | 30 |
| 9 — Persistence | 42–43 | 20 |
| 10 — C2 & BOF | 44 | 10 |
| 11 — OPSEC & Anti-Forensics | 45 | 10 |
| 12 — Capstone | 46 | 10 |
| **Total** | **46 modules** | **460 tasks** |
