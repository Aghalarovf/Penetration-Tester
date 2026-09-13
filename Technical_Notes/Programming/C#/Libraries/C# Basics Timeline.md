# C# Red Team — Study Plan (Step 19 → Step 75)
> A detailed day-by-day schedule from Step 19 to the end of the roadmap.
> Based on 2 hours of focused study per day.

---

## How to Use This Plan

Each day follows this structure:
- **Theory** — understand the concept (30–40 min)
- **Code** — write it yourself, no copy-pasting (60–70 min)
- **Test** — run it, break it, fix it (remaining time)

---

## 🟣 Stage 3 — OOP (Starting from Step 19)

### Step 19 — Interfaces
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Understand what `interface` is and why it exists. Write the `IC2Channel` example. Implement `HttpChannel` and `TcpChannel` |
| 2 | Write additional interfaces — `IScanner`, `IExfil`. Call them polymorphically through a `List<IC2Channel>` |

---

### Step 20 — Polymorphism & Virtual Methods
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Understand `virtual` / `override` difference. Write a base `C2Channel` class and override it in `HttpChannel` |
| 2 | Test the difference between `abstract` class and `interface`. Understand the `sealed` keyword |

---

### Step 21 — Static Members & Static Classes
**Duration: 1 day**

| Day | Goal |
|-----|------|
| 1 | Write a `static class Utils` with `XorEncrypt`, `ToBytes`, `ToHex` helper methods. Understand `const` vs `static readonly` |

---

## 🟡 Stage 4 — LINQ (Steps 22–24)

### Step 22 — Where & Select
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Filter a port list using `Where`. Transform a host list using `Select`. Method syntax vs query syntax |
| 2 | Write your own recon result class, filter it with LINQ. Understand `ToList()` vs `ToArray()` |

---

### Step 23 — OrderBy, GroupBy, Distinct
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Sort a port list with `OrderBy`. Remove duplicate IPs with `Distinct` |
| 2 | Group ports by service type using `GroupBy` (80/443 → HTTP, 22 → SSH) |

---

### Step 24 — First, Any, All, Count
**Duration: 1 day**

| Day | Goal |
|-----|------|
| 1 | Test `Any`, `All`, `Count`, `FirstOrDefault`, `SingleOrDefault` — each with a real offensive scenario |

---

## 🔴 Stage 5 — Essentials (Steps 25–31)

### Step 25 — Exception Handling
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `try/catch/finally` — handle TCP connection failures. Work with `SocketException`, `TimeoutException` |
| 2 | Write a custom exception class. Use `when` filter for specific exception handling. Nested try-catch |

---

### Step 26 — Nullable Types & Null Safety
**Duration: 1 day**

| Day | Goal |
|-----|------|
| 1 | Use `string?`, `int?`, `?.`, `??`, `??=` — all in a single project |

---

### Step 27 — Delegates, Lambda, Func/Action
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Write a `delegate` type manually. Then simplify with `Func<>` and `Action<>` |
| 2 | Lambda expressions — use in a LINQ chain. Write a callback pattern (log handler) |

---

### Step 28 — using & IDisposable
**Duration: 1 day**

| Day | Goal |
|-----|------|
| 1 | Implement `IDisposable`. Understand `using` statement vs `using` declaration. Write a `ScanSession` class |

---

### Step 29 — Encoding & Byte Conversion
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `UTF8.GetBytes`, `Convert.ToBase64String`, `FromBase64String` — complete string ↔ bytes roundtrip |
| 2 | Write XOR encrypt/decrypt. Implement hex string ↔ byte array conversion. Use `BitConverter` |

---

### Step 30 — File I/O
**Duration: 1 day**

| Day | Goal |
|-----|------|
| 1 | Read a wordlist, write results. Understand `File` vs `StreamReader` vs `StreamWriter`. Use the `Path` class |

---

### Step 31 — Async / Await
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Understand what `async/await` is — synchronous vs asynchronous. `Task`, `Task<T>` |
| 2 | Write an async port scanner — scan ports 1–1024 in parallel with `Task.WhenAll` |
| 3 | `CancellationToken`, `Task.WhenAny`, timeout pattern. When to use `ConfigureAwait(false)` |

---

## 🌐 Stage 5.5 — Network Fundamentals (Steps 32–34)

> These steps bridge the gap between C# essentials and Windows Internals networking code.

### Step 32 — TCP/IP & Networking Concepts
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Understand IP addressing, subnets, gateway, DNS. TCP vs UDP differences. OSI model layers 3–7. Well-known ports (22, 53, 80, 443, 445, 3389) |
| 2 | TCP 3-way handshake (SYN → SYN-ACK → ACK). Connection teardown. What happens during a port scan. Wireshark — capture a real TCP handshake |

---

### Step 33 — Socket API Basics
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Understand what a socket is. `IPAddress`, `IPEndPoint` classes. Low-level `Socket` class — bind, listen, accept, connect flow |
| 2 | Write a raw `Socket`-based echo server and client. Understand blocking vs non-blocking sockets. Compare `Socket` vs `TcpClient` abstraction level |

---

### Step 34 — NetworkStream & Framing
**Duration: 1 day**

| Day | Goal |
|-----|------|
| 1 | `NetworkStream` read/write. `BinaryReader` / `BinaryWriter` over a stream. Length-prefixed message framing — send structured data without splitting issues |

---

## ⚫ Stage 6 — Windows Internals & API (Steps 35–40)

> ⚠️ A **Windows VM** is mandatory from this stage onward.

### Step 35 — P/Invoke Basics
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `DllImport` syntax. Call `MessageBox` from `kernel32.dll`. Understand `SetLastError`, `CharSet` parameters |
| 2 | Write `OpenProcess` + `CloseHandle`. Obtain a process handle and close it. Error handling via `Marshal.GetLastWin32Error` |

---

### Step 36 — Windows Data Types
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Memorize the mapping: `HANDLE→IntPtr`, `DWORD→uint`, `BOOL→bool`, `LPVOID→IntPtr` |
| 2 | Define `STARTUPINFO` and `PROCESS_INFORMATION` structs in C#. Understand the `StructLayout` attribute |

---

### Step 37 — Process Enumeration
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | List all processes with `Process.GetProcesses()`. Print PID, name, memory usage |
| 2 | Find `lsass`, `winlogon`, `explorer`. Read process owner. Compare `System.Diagnostics` vs P/Invoke approach |

---

### Step 38 — Handle & Memory Basics
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `ReadProcessMemory` — read bytes from another process's memory (use your own test process) |
| 2 | `WriteProcessMemory` — write to memory. Understand access rights (`PROCESS_VM_READ`, `PROCESS_VM_WRITE`) |

---

### Step 39 — Token & Privilege Basics
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `OpenProcessToken` — get your own process token. Understand token types |
| 2 | Enable `SeDebugPrivilege` with `AdjustTokenPrivileges`. Enumerate current privileges |

---

### Step 40 — unsafe & Pointers
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | `unsafe` context — create pointers, dereference. Work with `int*`, `byte*` |
| 2 | `fixed` statement — pin a managed array. Pointer arithmetic |
| 3 | `stackalloc` — allocate array on the stack. Write NOP sled bytes to a byte array via pointer |

---

## 🔥 Stage 7 — Memory Manipulation (Steps 41–43)

### Step 41 — Marshal Class
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `Marshal.AllocHGlobal` / `FreeHGlobal`. `Marshal.Copy` — managed ↔ unmanaged memory transfer |
| 2 | Serialize a struct to unmanaged memory. Use `Marshal.SizeOf` and `PtrToStructure` |

---

### Step 42 — VirtualAlloc & Memory Protection
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Allocate RW memory with `VirtualAlloc`. Understand `MEM_COMMIT` vs `MEM_RESERVE` |
| 2 | Change protection from RW → RX with `VirtualProtect`. Memorize memory protection constants |
| 3 | `VirtualAllocEx` — allocate in a remote process. Clean up with `VirtualFree` |

---

### Step 43 — CreateThread & Shellcode Execution
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Create a new thread with `CreateThread`. Wait for it with `WaitForSingleObject` |
| 2 | Write a NOP sled (`0x90`) shellcode to memory and execute via thread (spawn calc.exe) |
| 3 | Generate calc.exe shellcode with `msfvenom` → execute from C#. Complete cycle: `VirtualAlloc → Copy → CreateThread` |

---

## 🟠 Stage 8 — Reflection & In-Memory Loading (Steps 44–47)

### Step 44 — Reflection Basics
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `Assembly.GetExecutingAssembly()` — list types and methods. `GetType`, `GetMethod` |
| 2 | Call a method dynamically with `MethodInfo.Invoke`. Call a private method via reflection |

---

### Step 45 — Assembly.Load() — In-Memory Execution
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Build a simple C# DLL. Load it with `Assembly.Load(byte[])` |
| 2 | Find and invoke the DLL's method via reflection. Pass arguments |
| 3 | Download the DLL over HTTP and load it in-memory (`HttpClient` → `byte[]` → `Assembly.Load`) |

---

### Step 46 — Dynamic Invocation
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Replace P/Invoke with `GetProcAddress` + `Marshal.GetDelegateForFunctionPointer` |
| 2 | Call `VirtualAlloc` via dynamic invocation. Achieve the same result without a static `DllImport` |

---

### Step 47 — PowerShell Runspace (AppLocker Bypass)
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Add `System.Management.Automation` reference. Create a basic runspace and execute `whoami` |
| 2 | Set `LanguageMode` via `InitialSessionState`. Capture and display script output |

---

## 🔴 Stage 9 — Networking & C2 (Steps 48–52)

### Step 48 — TCP Client & Server
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Write a local `TcpListener` + `TcpClient`. Send and receive messages |
| 2 | Build an async TCP reverse shell — stream `cmd.exe` output over the socket |

---

### Step 49 — HTTP Beaconing
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `HttpClient` GET/POST. Set up a local HTTP server (Python `http.server`), beacon to it from C# |
| 2 | Add jitter. Set custom User-Agent and headers. Parse the response |

---

### Step 50 — DNS over HTTPS (DoH) Beaconing
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Understand the DoH concept. Send a manual query to `1.1.1.1/dns-query` endpoint |
| 2 | Base64url-encode data and embed it as a subdomain label |
| 3 | Parse the TXT record response. Complete DoH beacon cycle: encode → query → decode |

---

### Step 51 — Named Pipes & Impersonation
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Write a local IPC channel with `NamedPipeServerStream` + `NamedPipeClientStream` |
| 2 | Send and receive commands over the pipe. Async pipe communication |
| 3 | `ImpersonateNamedPipeClient` — steal the client's token on the server side. Verify the identity |

---

### Step 52 — Agent Architecture & Command Dispatcher
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Design a modular agent — `ICommand` interface, command registry using `Dictionary<string, Action<string[]>>` |
| 2 | Implement built-in commands: `shell`, `sleep`, `exit`. Wire them to the beacon loop |
| 3 | Add a task queue — agent polls the C2, receives a task ID + command, executes, returns output |

---

## 🟤 Stage 10 — AV/EDR Evasion (Steps 53–59)

> ⚠️ Windows Defender must be tested on a **real VM** from this stage onward.

### Step 53 — AMSI Bypass
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | AMSI architecture — `amsi.dll`, `AmsiScanBuffer` function, when it gets called |
| 2 | Locate `AmsiScanBuffer` via `LoadLibrary + GetProcAddress`. Make it writable with `VirtualProtect` |
| 3 | Write the patch bytes (`0xB8 0x57 0x00 0x07 0x80 0xC3`). Verify AMSI is disabled inside a PowerShell runspace |

---

### Step 54 — ETW Patching
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Understand ETW — `EtwEventWrite`, Provider/Consumer model. How EDRs use ETW for telemetry |
| 2 | Locate `EtwEventWrite` in `ntdll.dll`. Apply the same pattern as AMSI — `VirtualProtect` + patch |
| 3 | Apply the `{ 0xC3 }` ret patch. Verify ETW events are suppressed |

---

### Step 55 — Unhooking via Fresh NTDLL
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Understand EDR hooking — `jmp` instruction injection. PE format basics: DOS header, NT header, sections |
| 2 | Read `ntdll.dll` from disk. Parse PE headers, locate the `.text` section |
| 3 | Overwrite the in-process ntdll `.text` section with the fresh copy |

---

### Step 56 — Direct Syscalls
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Syscall mechanics — user mode → kernel mode transition. What a Syscall ID (SSN) is |
| 2 | Generate a syscall stub with SysWhispers3. Integrate it into a C# project |
| 3 | Call `NtAllocateVirtualMemory` via direct syscall. Compare with the hooked `VirtualAlloc` |

---

### Step 57 — Payload Obfuscation
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | XOR encrypt/decrypt — embed shellcode, decrypt at runtime |
| 2 | AES-256 encryption — `Aes.Create()`, key/IV management. Test encrypted shellcode against offline AV (do not upload) |

---

### Step 58 — Process Injection — Classic
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | `OpenProcess → VirtualAllocEx → WriteProcessMemory` — write shellcode into a remote process |
| 2 | Execute shellcode with `CreateRemoteThread`. Inject into `notepad.exe` |
| 3 | Optimize access rights. Close handles after injection. Analyze what AV detects |

---

### Step 59 — Process Hollowing
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Hollowing concept — `CREATE_SUSPENDED`, `NtUnmapViewOfSection`. Deep dive into PE format |
| 2 | Create a suspended process, unmap its image, write the payload |
| 3 | Update the entry point register (RCX/EIP) with `SetThreadContext`. Resume with `ResumeThread` |

---

## 🟣 Stage 11 — Credential Access (Steps 60–62)

### Step 60 — Token Impersonation
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Token types — Primary vs Impersonation. Get a token with `OpenProcessToken` |
| 2 | Clone the token with `DuplicateTokenEx`. Understand impersonation levels |
| 3 | Impersonate `winlogon.exe` token with `ImpersonateLoggedOnUser`. Verify with `whoami` |

---

### Step 61 — Custom MiniDump (LSASS)
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | `MiniDumpWriteDump` API. Open a handle to LSASS (`SeDebugPrivilege` required) |
| 2 | Write the dump file. Analyze offline with Mimikatz |
| 3 | Understand AV detection vectors. Research `ReadProcessMemory`-based manual dump as an alternative |

---

### Step 62 — SAM & Registry Extraction
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Save SAM + SYSTEM hives with `reg save`. Write the `RegSaveKey` P/Invoke version |
| 2 | Parse offline with Impacket `secretsdump`. Understand SYSKEY encryption |

---

### Step 63 — Kerberos Ticket Manipulation
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Kerberos flow — AS-REQ, TGT, TGS-REQ, TGS. What the LSA is |
| 2 | List tickets with `LsaConnectUntrusted + LsaCallAuthenticationPackage` |
| 3 | Extract a TGT with `KERB_RETRIEVE_TKT_REQUEST`. Inject it with `KERB_SUBMIT_TKT_REQUEST` (Pass-the-Ticket) |

---

## 🔵 Stage 12 — Active Directory (Steps 64–69)

> ⚠️ An **Active Directory lab** is required for this stage: Domain Controller + 2 Windows VMs.

### Step 64 — LDAP Enumeration
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `DirectoryEntry + DirectorySearcher` — list all users. Understand filter syntax |
| 2 | Query group membership, AdminCount=1, disabled accounts. Find users with SPNs |

---

### Step 65 — Kerberoasting
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | What Kerberoasting is — SPN, TGS, RC4 encryption. Find Kerberoastable accounts via LDAP |
| 2 | Request a TGS with `KerberosRequestorSecurityToken`. Extract raw ticket bytes |
| 3 | Crack offline with `hashcat -m 13100`. Crack a service account password in the lab |

---

### Step 66 — DACL / ACL Enumeration
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | ACL/DACL/ACE concepts. Read object permissions with `ActiveDirectorySecurity` |
| 2 | Find `GenericAll`, `WriteDACL`, `GenericWrite`. Build a privilege escalation path in the lab |

---

### Step 67 — COM Object Lateral Movement
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | COM/DCOM basics. `MMC20.Application` ProgID. `Type.GetTypeFromProgID` |
| 2 | Execute a command on a remote host via `ExecuteShellCommand` (in lab) |
| 3 | Test `ShellWindows` and `ShellBrowserWindow` COM objects |

---

### Step 68 — DCSync
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | DCSync principle — MS-DRSR protocol. Which privileges are required |
| 2 | Execute Mimikatz `lsadump::dcsync` through a PowerShell runspace |
| 3 | Extract the `krbtgt` hash. Understand the Golden Ticket concept |

---

### Step 69 — Pass-the-Hash
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | PTH mechanics — NTLM auth flow. `LogonUser` with `LOGON32_LOGON_NEW_CREDENTIALS` |
| 2 | Authenticate to SMB, WMI, WinRM via PTH in the lab. Compare with Impacket |

---

## ⚪ Stage 13 — AppLocker & CLM Bypass (Steps 70–72)

### Step 70 — AppLocker Enumeration
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Read AppLocker policy from the registry. Check enforcement mode |
| 2 | Find writable paths (`%TEMP%`, `%APPDATA%`). Identify gaps in the whitelist |

---

### Step 71 — MSBuild Inline Task Execution
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Write a `.csproj` file with an inline C# task. Execute it with `MSBuild.exe` |
| 2 | Embed shellcode execution inside the MSBuild task. Test under AppLocker enforcement |

---

### Step 72 — CLM Detection & Bypass
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | What CLM is, the `__PSLockdownPolicy` env var. Check `LanguageMode` |
| 2 | Force `FullLanguage` mode via a custom runspace |
| 3 | Test PSv2 downgrade bypass. Configure `InitialSessionState` |

---

## 🟢 Stage 14 — Advanced Post-Exploitation (Steps 73–75)

### Step 73 — Registry Persistence
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | HKCU `Run` key — user-level persistence. Verify it survives a reboot |
| 2 | HKLM `Run` key — system-level. Add WMI subscription persistence |

---

### Step 74 — Situational Awareness
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `IsSystem`, domain check, AV process detection — consolidate everything into a single class |
| 2 | Sandbox detection — RAM, CPU cores, username checks. `SeDebugPrivilege` verification |

---

### Step 75 — Agent Hardening & Operational Security
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Sleep masking — encrypt agent in memory during sleep intervals. Detect and evade common sandbox triggers |
| 2 | Malleable C2 profiles — randomize beacon intervals, URI paths, headers. String obfuscation at compile time |

---
