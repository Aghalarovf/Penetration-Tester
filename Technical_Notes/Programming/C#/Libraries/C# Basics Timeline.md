# C# Red Team — Study Plan (Step 19 → Step 135)
> Updated plan — SMB Basics + AD + Windows PrivEsc modules added.
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


### Step 24 — First, Any, All, Count
**Duration: 1 day**

| Day | Goal |
|-----|------|
| 1 | Test `Any`, `All`, `Count`, `FirstOrDefault`, `SingleOrDefault` — each with a real offensive scenario |

---

## 🔴 Stage 5 — Essentials (Steps 25–31)

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

## 🔵 Stage 5.6 — SMB Basics (Steps 34.1 → 34.10)

> Start after Step 34 is complete. Build the protocol from scratch before touching any pentest tools.
>
> ⚠️ **Cross-platform requirement:** All code in this stage must run on both **Linux and Windows**.
> No `DllImport`, no Windows-only APIs. Use only `TcpClient`, `NetworkStream`, `BinaryReader/Writer`,
> `Span<byte>`, `BitConverter` and `StructLayout` — all available in .NET on any OS.
> Test every tool on Linux first; if it runs there, it will run on Windows too.

---

### Step 34.1 — NetBIOS Session Layer
**Duration: 1 day**

| Day | Goal |
|-----|------|
| 1 | Connect to TCP port 445. Understand the 4-byte NetBIOS Session Service header that wraps every SMB packet — `0x00` + 3-byte big-endian length. Write `SendAsync` / `ReceiveAsync` using `NetworkStream`. Verify framing by sending a raw buffer and reading it back with Wireshark |

```csharp
// NetBIOS Session Service framing
// [00][00][00][XX] — first byte is always 0x00, remaining 3 bytes are the payload length (big-endian)
public async Task SendAsync(byte[] smbPacket, CancellationToken ct = default)
{
    var frame = new byte[4 + smbPacket.Length];
    frame[0] = 0x00;
    frame[1] = (byte)(smbPacket.Length >> 16);
    frame[2] = (byte)(smbPacket.Length >> 8);
    frame[3] = (byte)(smbPacket.Length);
    smbPacket.CopyTo(frame, 4);
    await _stream.WriteAsync(frame, ct);
}

public async Task<byte[]> ReceiveAsync(CancellationToken ct = default)
{
    var header = new byte[4];
    await _stream.ReadExactlyAsync(header, ct);
    int length = (header[1] << 16) | (header[2] << 8) | header[3];
    var data = new byte[length];
    await _stream.ReadExactlyAsync(data, ct);
    return data;
}
```

---

### Step 34.2 — SMB2 Header Structure
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Open the MS-SMB2 specification — read the SMB2 Header section. Map every field to a C# struct using `StructLayout(LayoutKind.Sequential, Pack = 1)`. Fields: `ProtocolId`, `StructureSize`, `CreditCharge`, `Status`, `Command`, `CreditRequest`, `Flags`, `NextCommand`, `MessageId`, `Reserved`, `TreeId`, `SessionId`, `Signature` |
| 2 | Capture a real SMB2 packet in Wireshark. Match each byte offset in the capture to the corresponding struct field. Manually parse the raw bytes in C# and print each field value |

```csharp
// Pure .NET — no P/Invoke, runs on Linux and Windows
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SMB2Header
{
    public uint   ProtocolId;      // 0xFE534D42 (\xFESMB)
    public ushort StructureSize;   // always 64
    public ushort CreditCharge;
    public uint   Status;
    public ushort Command;
    public ushort CreditRequest;
    public uint   Flags;
    public uint   NextCommand;
    public ulong  MessageId;
    public uint   Reserved;
    public uint   TreeId;
    public ulong  SessionId;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] Signature;
}

// Serialize struct → byte[] without any Windows API
public static byte[] StructToBytes<T>(T structure) where T : struct
{
    int size = Marshal.SizeOf<T>();
    var buffer = new byte[size];
    var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
    try
    {
        Marshal.StructureToPtr(structure, handle.AddrOfPinnedObject(), false);
        return buffer;
    }
    finally { handle.Free(); }
}
```

---

### Step 34.3 — NEGOTIATE Request & Response
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Build a NEGOTIATE request from scratch — SMB2 Header + Negotiate body. Fill `DialectCount`, `SecurityMode`, `Capabilities`, and `Dialects[]`. Send it to a live target (Samba on Linux or Windows share) |
| 2 | Receive and parse the NEGOTIATE response. Extract `DialectRevision`, `ServerGuid`, `Capabilities`, `MaxTransactSize`, `SystemTime`, and `SecurityBuffer` (NTLMSSP blob). Print every field |

```
NEGOTIATE Request fields:
  ├── StructureSize   = 36
  ├── DialectCount    = N
  ├── SecurityMode    = 0x0001 (signing enabled) / 0x0002 (signing required)
  ├── Capabilities    = DFS, Leasing, LargeMTU, MultiChannel, Encryption...
  ├── ClientGuid      = random 16 bytes
  └── Dialects[]      = 0x0202, 0x0210, 0x0300, 0x0302, 0x0311

NEGOTIATE Response fields:
  ├── DialectRevision = dialect chosen by server
  ├── ServerGuid
  ├── Capabilities
  ├── MaxTransactSize / MaxReadSize / MaxWriteSize
  ├── SystemTime      = FILETIME (100-nanosecond intervals since 1601)
  └── SecurityBuffer  = NTLMSSP / Kerberos SPNEGO blob
```

---

### Step 34.4 — SESSION SETUP (Anonymous)
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Build the first SESSION_SETUP request — embed a minimal NTLMSSP NEGOTIATE blob (hand-crafted bytes, no library). Send it. Receive the server challenge — parse `STATUS_MORE_PROCESSING_REQUIRED` and extract the `SessionId` and NTLMSSP CHALLENGE blob |
| 2 | Build the second SESSION_SETUP request — embed an NTLMSSP AUTHENTICATE message with empty credentials (anonymous). Send it. Receive `STATUS_SUCCESS` and confirm the session is established |

```
SESSION SETUP — 2-round NTLM handshake:

  Round 1:
    Client → SESSION_SETUP (NTLMSSP_NEGOTIATE blob)
    Server → STATUS_MORE_PROCESSING_REQUIRED
              SessionId assigned
              SecurityBuffer = NTLMSSP_CHALLENGE

  Round 2:
    Client → SESSION_SETUP (NTLMSSP_AUTHENTICATE, empty credentials)
    Server → STATUS_SUCCESS
              Session is open
```

---

### Step 34.5 — TREE CONNECT & TREE DISCONNECT
**Duration: 1 day**

| Day | Goal |
|-----|------|
| 1 | Build a TREE_CONNECT request. Encode the share path `\\host\IPC$` as a Unicode (UTF-16LE) string in the packet body. Send it. Parse the response and store the `TreeId`. Then send TREE_DISCONNECT and verify the server accepts it |

```csharp
// Share path encoding — pure .NET, cross-platform
string sharePath = $@"\\{host}\IPC$";
byte[] pathBytes = Encoding.Unicode.GetBytes(sharePath); // UTF-16LE, no P/Invoke
```

---

### Step 34.6 — Packet Builder & Parser Classes
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Write `SMB2PacketBuilder` — one method per command. Each method fills the SMB2 header automatically, auto-increments `MessageId`, and returns a `byte[]` ready to send |
| 2 | Write `SMB2PacketParser` — takes a `byte[]`, reads the header, dispatches to a per-command parse method, and returns a typed response object. Handle unknown `Status` codes without crashing |

```
SMB2PacketBuilder
  ├── BuildNegotiate(dialects[])         → byte[]
  ├── BuildSessionSetup(stage, blob)     → byte[]
  ├── BuildTreeConnect(sessionId, path)  → byte[]
  └── BuildTreeDisconnect(sessionId, treeId) → byte[]

SMB2PacketParser
  ├── ParseHeader(byte[])                → SMB2Header
  ├── ParseNegotiateResponse(byte[])     → NegotiateResponse
  ├── ParseSessionSetupResponse(byte[])  → SessionSetupResponse
  └── ParseTreeConnectResponse(byte[])   → TreeConnectResponse
```

---

### Step 34.7 — CREATE & File Operations
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Build a CREATE request to open a file on a share. Understand `DesiredAccess` (read/write/both), `CreateDisposition` (open existing vs create new), and `CreateOptions` flags. Parse the response and store the `FileId` (persistent + volatile handle pair) |
| 2 | Build READ and CLOSE requests. Read a block of data from the opened file using `FileId`. Then send CLOSE. Complete the full `Open → Read → Close` cycle against a Samba share on Linux |

---

### Step 34.8 — Named Pipe Operations
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Tree-connect to `IPC$`. Open the `\srvsvc` named pipe using a CREATE request — named pipes are opened exactly like files, with the pipe name as the path. Store the `FileId` |
| 2 | Write raw bytes to the pipe using a WRITE request. Read the response using a READ request. This is the foundation for every RPC call in later stages. Verify round-trip with Wireshark |

```
Full path to a named pipe over SMB2:

  TCP 445
    └── NetBIOS frame (4-byte length prefix)
          └── SMB2 NEGOTIATE
                └── SESSION SETUP  (2 rounds)
                      └── TREE CONNECT  → \\host\IPC$  (TreeId)
                              └── CREATE  → \srvsvc     (FileId)
                                    └── WRITE raw bytes
                                          └── READ response
```

---

### Step 34.9 — NT Status Codes & Error Handling
**Duration: 1 day**

| Day | Goal |
|-----|------|
| 1 | Learn the most common `STATUS_*` codes returned in the SMB2 header. Write a `NtStatus` static class with named constants. Wrap every send/receive call so that unexpected status codes throw a typed `SmbException` with a human-readable message |

```csharp
public static class NtStatus
{
    public const uint SUCCESS                   = 0x00000000;
    public const uint MORE_PROCESSING_REQUIRED  = 0xC0000016;
    public const uint ACCESS_DENIED             = 0xC0000022;
    public const uint LOGON_FAILURE             = 0xC000006D;
    public const uint BAD_NETWORK_NAME          = 0xC00000CC;
    public const uint NOT_SUPPORTED             = 0xC00000BB;
    public const uint OBJECT_NAME_NOT_FOUND     = 0xC0000034;
    public const uint USER_SESSION_DELETED      = 0xC0000203;

    public static string Describe(uint code) => code switch
    {
        SUCCESS                  => "Success",
        MORE_PROCESSING_REQUIRED => "More processing required (NTLM round 2)",
        ACCESS_DENIED            => "Access denied",
        LOGON_FAILURE            => "Logon failure — bad credentials",
        BAD_NETWORK_NAME         => "Share not found",
        _                        => $"Unknown status 0x{code:X8}"
    };
}
```

---

### Step 34.10 — SMBClient Class (Bring It All Together)
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Write the `SMBClient` class — `ConnectAsync`, `NegotiateAsync`, `SessionSetupAnonymousAsync`, `TreeConnectAsync`. Each method stores its result (dialect, sessionId, treeId) as instance state |
| 2 | Add `OpenPipeAsync`, `WritePipeAsync`, `ReadPipeAsync`, `CloseAsync`. Each method uses the stored state so callers never pass IDs manually |
| 3 | Run the full flow against a Samba server on Linux: connect → negotiate → session → `IPC$` → open `\srvsvc` → write → read → close. Confirm every step in Wireshark |

```csharp
// Usage after Step 34.10 — this client is the foundation
// all pentest tools in later stages are built on top of it.
// Runs on Linux and Windows — zero P/Invoke.
var client = new SMBClient("192.168.1.10");
await client.ConnectAsync();
await client.NegotiateAsync();
await client.SessionSetupAnonymousAsync();
await client.TreeConnectAsync("IPC$");
var pipe = await client.OpenPipeAsync("srvsvc");
var response = await client.WritePipeAsync(pipe, requestBytes);
await client.CloseAsync(pipe);
```

---

> **After Step 34.10:** SMB Tools 1–10 (Common SMB) can be built directly on top of `SMBClient`.
> Tools 11–49 (RPC-based) require Steps 35–36 first — P/Invoke is needed for Windows API calls in those tools,
> but `SMBClient` itself stays cross-platform throughout.

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

## 🟣 Stage 11 — Credential Access (Steps 60–63)

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

## ⚪ Stage 12 — AppLocker & CLM Bypass (Steps 64–66)

### Step 64 — AppLocker Enumeration
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Read AppLocker policy from the registry. Check enforcement mode |
| 2 | Find writable paths (`%TEMP%`, `%APPDATA%`). Identify gaps in the whitelist |

---

### Step 65 — MSBuild Inline Task Execution
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Write a `.csproj` file with an inline C# task. Execute it with `MSBuild.exe` |
| 2 | Embed shellcode execution inside the MSBuild task. Test under AppLocker enforcement |

---

### Step 66 — CLM Detection & Bypass
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | What CLM is, the `__PSLockdownPolicy` env var. Check `LanguageMode` |
| 2 | Force `FullLanguage` mode via a custom runspace |
| 3 | Test PSv2 downgrade bypass. Configure `InitialSessionState` |

---

## 🟢 Stage 13 — C2 Hardening (Steps 67–69)

### Step 67 — Registry Persistence
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | HKCU `Run` key — user-level persistence. Verify it survives a reboot |
| 2 | HKLM `Run` key — system-level. Add WMI subscription persistence |

---

### Step 68 — Situational Awareness
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `IsSystem`, domain check, AV process detection — consolidate everything into a single class |
| 2 | Sandbox detection — RAM, CPU cores, username checks. `SeDebugPrivilege` verification |

---

### Step 69 — Agent Hardening & Operational Security
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Sleep masking — encrypt agent in memory during sleep intervals. Detect and evade common sandbox triggers |
| 2 | Malleable C2 profiles — randomize beacon intervals, URI paths, headers. String obfuscation at compile time |

---

## 🔐 Stage 14 — Windows Privilege Escalation (Steps 70–90)

> ⚠️ C2 infrastructure is fully operational from this point. All techniques are implemented as agent modules.

### Step 70 — Privilege Enumeration Module
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Enumerate all token privileges with `GetTokenInformation`. List enabled, disabled, and removed privileges |
| 2 | Build a reusable `PrivilegeChecker` class — check for specific privileges by name. Output as structured JSON for C2 |

---

### Step 71 — SeImpersonatePrivilege & Potato Attacks
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Understand `SeImpersonatePrivilege` — why IIS/SQL service accounts have it. How Potato attacks abuse NTLM relay |
| 2 | Implement a named pipe server that captures SYSTEM token via `ImpersonateNamedPipeClient`. Verify SYSTEM context |
| 3 | Study PrintSpoofer technique — `SpoolSample` triggers authentication to attacker-controlled pipe. Implement in C# |

---

### Step 72 — SeDebugPrivilege Abuse
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Enable `SeDebugPrivilege` programmatically. Open a handle to `lsass` — verify elevated access |
| 2 | Migrate into a SYSTEM process by injecting shellcode via `SeDebug`. Full SYSTEM shell via process injection |

---

### Step 73 — SeBackupPrivilege & SeRestorePrivilege
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `SeBackupPrivilege` — read any file regardless of DACL. Extract SAM/SYSTEM hives using backup APIs (`BackupRead`) |
| 2 | `SeRestorePrivilege` — write any file regardless of DACL. Overwrite a privileged binary or service executable |

---

### Step 74 — SeTakeOwnershipPrivilege & SeAssignPrimaryTokenPrivilege
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `SeTakeOwnershipPrivilege` — take ownership of any object. Change DACL on a privileged file/registry key |
| 2 | `SeAssignPrimaryTokenPrivilege` — assign a primary token to a process. Spawn a process under SYSTEM token |

---

### Step 75 — SeTCBPrivilege
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `SeTCBPrivilege` — Act as part of the operating system. Understand what this enables over SeImpersonate |
| 2 | Use `LsaLogonUser` with `SeTCBPrivilege` to create a logon session for any user without credentials. Spawn shell |

---

### Step 76 — UAC Bypass — COM Elevation
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | UAC internals — consent.exe, auto-elevation, manifest flags. How COM objects bypass UAC |
| 2 | Abuse `ICMLuaUtil` COM interface via `CoCreateInstance` — execute a command as elevated without UAC prompt |

---

### Step 77 — UAC Bypass — Token Manipulation
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Duplicate an elevated token from a high-integrity process. Use `CreateProcessWithTokenW` to spawn elevated shell |
| 2 | `fodhelper.exe` registry hijack — write payload to `HKCU\Software\Classes\ms-settings\shell\open\command`. Trigger elevation |

---

### Step 78 — UAC Bypass — DLL Hijack in Auto-Elevate Binary
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Find auto-elevate binaries (`requestedExecutionLevel=highestAvailable`). Identify DLL load order in `eventvwr.exe` |
| 2 | Drop a malicious DLL into the search path. Trigger `eventvwr.exe` — verify elevated execution |

---

### Step 79 — DLL Hijacking — Basics
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | DLL search order — `KnownDLLs`, application directory, `PATH`. Use `Procmon` to find missing DLL loads |
| 2 | Write a proxy DLL — export the same functions as the original, add payload in `DllMain`. Drop and trigger |

---

### Step 80 — DLL Sideloading
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Difference between hijacking and sideloading — legitimate signed binary loads attacker DLL from same directory |
| 2 | Find a vulnerable signed binary with `Sigcheck` + `Procmon`. Build a sideloading DLL. Execute payload under signed process |

---

### Step 81 — DLL Proxying
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Full proxy DLL — forward all exports to the original DLL using `#pragma comment(linker, "/export=...")`. Build with C# + embedded native DLL |
| 2 | Replace a legitimate DLL in a service directory. Service loads proxy → payload runs → original functions still work |

---

### Step 82 — DLL Injection via Reflective Loading
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Reflective DLL injection concept — DLL maps itself into memory without `LoadLibrary`. Study the bootstrap stub |
| 2 | Implement a C# loader that injects a reflective DLL into a remote process. No disk write required |

---

### Step 83 — Registry PrivEsc — AlwaysInstallElevated
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Check `AlwaysInstallElevated` in `HKLM` and `HKCU`. Understand why MSI packages run as SYSTEM when enabled |
| 2 | Generate a malicious `.msi` with `msfvenom`. Execute — verify SYSTEM shell. Implement the check in C# agent module |

---

### Step 84 — Registry PrivEsc — Service Binary Path & ImagePath
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Enumerate services with weak registry permissions — `HKLM\SYSTEM\CurrentControlSet\Services`. Find writable keys |
| 2 | Modify `ImagePath` to point to payload. Restart the service — verify SYSTEM execution |

---

### Step 85 — Registry PrivEsc — Autorun Keys & Weak Permissions
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Enumerate `Run`, `RunOnce`, `RunServices` keys. Check write permissions with `RegGetKeySecurity` |
| 2 | Write payload path to a writable autorun key. Verify execution on next login/boot |

---

### Step 86 — Scheduled Task PrivEsc — Weak Permissions
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Enumerate scheduled tasks with `ITaskService`. Find tasks running as SYSTEM with user-writable action paths |
| 2 | Replace the target binary with payload. Wait for task trigger — verify SYSTEM execution |

---

### Step 87 — Scheduled Task PrivEsc — Task XML Manipulation
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Read and parse task XML definitions from `C:\Windows\System32\Tasks`. Identify misconfigured triggers and principals |
| 2 | Modify task XML to inject a new action or change the principal to SYSTEM. Re-register with `RegisterTask` |

---

### Step 88 — Service Misconfigurations — Unquoted Service Path
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Understand unquoted path vulnerability — Windows resolves `C:\Program Files\Service\svc.exe` as `C:\Program.exe` first |
| 2 | Enumerate unquoted service paths in C#. Drop payload at the winning resolution path. Restart service |

---

### Step 89 — Service Misconfigurations — Weak Service DACL
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Query service DACLs with `QueryServiceObjectSecurity`. Find services where low-priv users have `SERVICE_CHANGE_CONFIG` |
| 2 | Use `ChangeServiceConfig` to redirect `BinaryPathName` to payload. Start service — verify SYSTEM shell |

---

### Step 90 — Token Impersonation via Named Pipe (Advanced)
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Trigger a SYSTEM process to connect to an attacker-controlled named pipe using print spooler or other primitives |
| 2 | Capture and duplicate the SYSTEM token. Spawn a new process under SYSTEM using `CreateProcessWithTokenW` |

---

## 🏰 Stage 15 — Active Directory Enumeration (Steps 91–95)

### Step 91 — LDAP Enumeration Module
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | `DirectoryEntry` + `DirectorySearcher` — enumerate all users, computers, groups. Understand LDAP filter syntax |
| 2 | Query `AdminCount=1`, disabled accounts, password-never-expires, users with SPNs set |
| 3 | Build a structured AD enumeration class — output results as JSON to C2. Add domain info: DC, forest, trusts |

---

### Step 92 — ACE & ACL Enumeration (ACE Collector)
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | ACL/DACL/ACE concepts — `ObjectSecurity`, `ActiveDirectorySecurity`. Read ACEs on AD objects |
| 2 | Enumerate dangerous ACEs: `GenericAll`, `GenericWrite`, `WriteDACL`, `WriteOwner`, `AllExtendedRights` |
| 3 | Build an ACE Collector module — scan all users and groups, output attack paths. Find shortest path to DA |

---

### Step 93 — Group Policy Enumeration
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Enumerate GPOs via LDAP — `(objectClass=groupPolicyContainer)`. Read `gPCFileSysPath`, parse `GptTmpl.inf` |
| 2 | Find GPOs with weak permissions — users who can modify a GPO linked to privileged OUs |

---

### Step 94 — Domain Trust Enumeration
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Query domain trusts via LDAP `(objectClass=trustedDomain)`. Understand trust direction, transitivity, trust type |
| 2 | Identify exploitable trusts — external trusts, forest trusts with SID filtering disabled. Map attack paths |

---

### Step 95 — AD Situational Awareness Module
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Combine all enumeration — domain info, users, groups, computers, GPOs, trusts into a single `ADRecon` module |
| 2 | Output structured JSON report to C2. Integrate as an agent command: `adrecon full` |

---

## ⚔️ Stage 16 — Kerberos Attacks (Steps 96–104)

### Step 96 — Kerberoasting
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | SPN, TGS, RC4 encryption — why service account hashes are crackable. Find Kerberoastable accounts via LDAP |
| 2 | Request TGS tickets with `KerberosRequestorSecurityToken`. Extract raw ticket bytes from memory |
| 3 | Format as `$krb5tgs$` hash. Crack offline with `hashcat -m 13100`. Implement as C# agent module |

---

### Step 97 — AS-REP Roasting
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Understand `DONT_REQUIRE_PREAUTH` flag. Find affected accounts via LDAP. AS-REQ without pre-auth returns encrypted data |
| 2 | Send raw AS-REQ with `KerberosClient`. Extract `$krb5asrep$` hash. Crack with `hashcat -m 18200` |

---

### Step 98 — Pass-the-Hash
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | NTLM auth flow — challenge/response. `LogonUser` with `LOGON32_LOGON_NEW_CREDENTIALS` and NTLM hash |
| 2 | Authenticate to SMB and WinRM via PTH in C#. Implement as agent module: `pth <user> <hash> <cmd>` |

---

### Step 99 — Pass-the-Ticket
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Extract TGT from memory with `KERB_RETRIEVE_TKT_REQUEST` via LSA. Export as `.kirbi` bytes |
| 2 | Inject a ticket with `KERB_SUBMIT_TKT_REQUEST`. Verify — access resources as the ticket owner |

---

### Step 100 — Silver Ticket
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Silver Ticket concept — forge a TGS using the service account NTLM hash. No DC contact required |
| 2 | Build the forged TGS structure — PAC, authorization data, encrypted part using RC4/AES |
| 3 | Inject the Silver Ticket and access the target service (CIFS, HTTP, MSSQL). Verify offline forgery works |

---

### Step 101 — Golden Ticket
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Golden Ticket concept — forge a TGT using `krbtgt` NTLM hash. Grants access to any service in the domain |
| 2 | Build the forged TGT structure. Requires: domain SID, `krbtgt` hash, arbitrary username and RID |
| 3 | Inject the Golden Ticket via LSA. Verify access to domain resources. Implement as C# agent module |

---

### Step 102 — Diamond Ticket
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Diamond Ticket concept — request a legitimate TGT, then decrypt and modify the PAC. More stealthy than Golden Ticket |
| 2 | Modify PAC group memberships (add Domain Admins SID). Re-encrypt with `krbtgt` key |
| 3 | Inject and verify — compare with Golden Ticket detection signatures. Understand why Diamond evades some detections |

---

### Step 103 — Sapphire Ticket
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Sapphire Ticket concept — obtain a privileged user's TGT via S4U2self + U2U, copy PAC into a new ticket |
| 2 | Implement the S4U2self + U2U flow in C#. Verify the resulting ticket grants DA-level access |

---

### Step 104 — Kerberos Delegation Attacks
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Delegation types — Unconstrained, Constrained, Resource-Based Constrained (RBCD). Enumerate via LDAP |
| 2 | Unconstrained delegation — capture TGTs from connecting users/computers via `TGT Delegation` flag |
| 3 | RBCD abuse — write `msDS-AllowedToActOnBehalfOfOtherIdentity`. S4U2proxy to impersonate domain admin |

---

## 🗝️ Stage 17 — ACL & GPO Abuse (Steps 105–110)

### Step 105 — GenericAll Abuse
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `GenericAll` on a user — reset password via `SetPassword` LDAP extended operation. Take over the account |
| 2 | `GenericAll` on a group — add self to Domain Admins with `AddMember` LDAP operation |

---

### Step 106 — WriteDACL & WriteOwner Abuse
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `WriteDACL` — grant self `GenericAll` on an object. Modify the DACL using `ActiveDirectorySecurity` |
| 2 | `WriteOwner` — take ownership of an AD object. Then grant self full control. Chain into DA escalation |

---

### Step 107 — GenericWrite & AllExtendedRights Abuse
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `GenericWrite` on a user — write `scriptPath` (logon script), `msDS-KeyCredentialLink` (Shadow Credentials) |
| 2 | `AllExtendedRights` — includes `User-Force-Change-Password` and `DS-Replication-Get-Changes`. Abuse each |

---

### Step 108 — Shadow Credentials
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Shadow Credentials concept — write a Key Credential to `msDS-KeyCredentialLink`. PKINIT authenticates with it |
| 2 | Generate a certificate key pair in C#. Construct the `KeyCredential` structure and write it via LDAP |
| 3 | Authenticate using the certificate via PKINIT. Retrieve NTLM hash from the AS-REP. Full account takeover |

---

### Step 109 — GPO Abuse — Immediate Scheduled Task
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Find GPOs where current user has `CreateChild` or `GenericWrite`. Understand GPO file system structure in SYSVOL |
| 2 | Add an immediate scheduled task to a GPO via `ScheduledTasks.xml` in the GPO SYSVOL path |
| 3 | Force GPO refresh on target with `gpupdate`. Verify payload executes under SYSTEM on all affected machines |

---

### Step 110 — GPO Abuse — Logon Script & Registry Policy
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Write a malicious logon script to a GPO's `Scripts\Logon` path. Script runs as the logging-in user |
| 2 | Inject a registry Run key via GPO `Registry.pol` file. Parse and write `.pol` format in C# |

---

## 🏅 Stage 18 — ADCS Certificate Attacks (Steps 111–120)

### Step 111 — ADCS Enumeration
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Enumerate Certificate Authorities via LDAP — `(objectClass=pKIEnrollmentService)`. Find CA name, DNS, templates |
| 2 | Enumerate certificate templates — permissions, flags, EKUs, `msPKI-Certificate-Name-Flag`. Build full ADCS map |

---

### Step 112 — ESC1 — SAN Spoofing
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | ESC1 — template allows `ENROLLEE_SUPPLIES_SUBJECT` + any user can enroll + authentication EKU. Identify via enum |
| 2 | Request a certificate with arbitrary SAN (Domain Admin UPN) using `CertRequest` COM interface in C#. Authenticate as DA |

---

### Step 113 — ESC2 — Any Purpose EKU
**Duration: 1 day**

| Day | Goal |
|-----|------|
| 1 | ESC2 — template has `Any Purpose` or no EKU. Can be used as a SubCA to sign subordinate certs. Enroll and abuse |

---

### Step 114 — ESC3 — Enrollment Agent Abuse
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | ESC3 — two templates: one grants Enrollment Agent rights, one allows agent to enroll on behalf of another user |
| 2 | Enroll as Enrollment Agent. Use agent cert to request a certificate on behalf of Domain Admin. Authenticate as DA |

---

### Step 115 — ESC4 — Template Write Permissions
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | ESC4 — low-priv user has `WriteProperty` or `GenericWrite` on a certificate template object |
| 2 | Modify the template to add `ENROLLEE_SUPPLIES_SUBJECT` flag and authentication EKU. Exploit as ESC1 |

---

### Step 116 — ESC5 — PKI Object Control
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | ESC5 — control over the CA object, NTAuthCertificates, or Root CA itself. Understand the PKI object hierarchy |
| 2 | Modify `NTAuthCertificates` to trust an attacker-controlled CA. Issue arbitrary certs that authenticate to AD |

---

### Step 117 — ESC6 — EDITF_ATTRIBUTESUBJECTALTNAME2
**Duration: 1 day**

| Day | Goal |
|-----|------|
| 1 | ESC6 — CA has `EDITF_ATTRIBUTESUBJECTALTNAME2` flag set. Any template with authentication EKU becomes ESC1-vulnerable. Exploit |

---

### Step 118 — ESC7 — CA Officer/Manager Rights
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | ESC7 — attacker has `ManageCA` or `ManageCertificates` rights on the CA. Enable `EDITF_ATTRIBUTESUBJECTALTNAME2` flag |
| 2 | Approve pending certificate requests as CA Manager. Issue arbitrary certs without template restrictions |

---

### Step 119 — ESC8 — NTLM Relay to AD CS HTTP Endpoint
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | ESC8 — AD CS web enrollment (`/certsrv`) does not require HTTPS or extended protection. NTLM relay is possible |
| 2 | Set up NTLM relay with `ntlmrelayx` targeting `/certsrv/certfnsh.asp`. Coerce DC authentication via print spooler |
| 3 | Relay DC$ machine account auth → obtain DC certificate → use PKINIT to get DC TGT → DCSync |

---

### Step 120 — ESC9 & ESC10 — Certificate Mapping Attacks
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | ESC9 — `CT_FLAG_NO_SECURITY_EXTENSION` on template. Certificate does not embed SID — allows UPN mapping abuse |
| 2 | ESC10 — weak certificate mapping in `CertificateMappingMethods` registry. Map a cert to any account via UPN |
| 3 | Combine ESC9/ESC10 with `GenericWrite` on a user to change UPN, enroll cert, reset UPN, authenticate as victim |

---

## 🌐 Stage 19 — AD Lateral Movement & Domain Dominance (Steps 121–130)

### Step 121 — DCSync
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | DCSync principle — MS-DRSR protocol (`IDL_DRSGetNCChanges`). Required rights: `DS-Replication-Get-Changes-All` |
| 2 | Implement DCSync in C# using `DsGetDcName` + `DsBind` + `DsGetNCChanges` P/Invoke calls |
| 3 | Extract `krbtgt`, Administrator, and all user hashes. Implement as C2 agent module: `dcsync <domain> <user>` |

---

### Step 122 — Pass-the-Hash Lateral Movement
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Use extracted NTLM hashes to authenticate over SMB — list shares, read files, execute commands via SCM |
| 2 | WinRM lateral movement with PTH — `WSManConnectionInfo` with NTLM hash. Execute commands on remote host |

---

### Step 123 — OverPass-the-Hash (Pass-the-Key)
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Use AES256 Kerberos key instead of NTLM hash — request TGT with AES key via `KERB_AS_REQ`. Avoids NTLM traffic |
| 2 | Implement in C# — extract AES key from LSASS, request TGT, inject. Compare with PTH in terms of detection |

---

### Step 124 — Remote Service Creation (SCM Lateral Movement)
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Connect to remote SCM with `OpenSCManager`. Create and start a service that runs payload on the remote host |
| 2 | Clean up — delete the service after execution. Implement as agent module: `scm-exec <host> <cmd>` |

---

### Step 125 — WMI Lateral Movement
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `ManagementScope` with credentials — connect to remote WMI. `Win32_Process.Create` to run a command |
| 2 | WMI event subscription for persistence — `__EventFilter` + `__EventConsumer` + `__FilterToConsumerBinding` |

---

### Step 126 — DCOM Lateral Movement
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `MMC20.Application` — `Type.GetTypeFromProgID` with remote host. `ExecuteShellCommand` to run payload |
| 2 | `ShellWindows` and `ShellBrowserWindow` COM objects — navigate to UNC path to trigger execution |

---

### Step 127 — Domain Persistence — AdminSDHolder
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | AdminSDHolder concept — `SDProp` process resets DACLs on protected accounts every 60 minutes |
| 2 | Write a backdoor ACE to `AdminSDHolder` object — `GenericAll` for a low-priv account. Wait for SDProp — verify propagation |

---

### Step 128 — Domain Persistence — DSRM Account
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | DSRM (Directory Services Restore Mode) account — local admin on every DC. Dump hash with `lsadump::lsa` |
| 2 | Enable remote DSRM login via registry: `DsrmAdminLogonBehavior = 2`. PTH with DSRM hash to DC |

---

### Step 129 — SID History Injection
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | SID History concept — extra SIDs in a user's token grant access to resources of the historical domain |
| 2 | Inject Enterprise Admins SID into a user's `SIDHistory` via `DsAddSidHistory` — verify cross-domain DA access |

---

### Step 130 — Cross-Forest Trust Attacks
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Forest trust internals — inter-realm TGT referral, SID filtering, `quarantine` flag. Enumerate via LDAP |
| 2 | Trust key extraction — get the inter-realm trust key with DCSync (`[domain]\[target]$` account) |
| 3 | Forge an inter-realm TGT with the trust key. Access resources in the trusted forest. SID filtering bypass if disabled |

---

## 🧹 Stage 20 — Operational Security & Cleanup (Steps 131–135)

### Step 131 — Event Log Manipulation
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Clear Security, System, and PowerShell Operational logs with `EventLog.Clear()` and `EvtClearLog` |
| 2 | Selectively delete specific event IDs using `EvtQuery` + `EvtFormatMessage`. Avoid clearing all logs (noisy) |

---

### Step 132 — Anti-Forensics — Timestomping
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | `SetFileTime` via P/Invoke — modify `CreationTime`, `LastWriteTime`, `LastAccessTime` on dropped files |
| 2 | Copy timestamps from a legitimate system file to the payload. Verify with forensic tools that timestamp looks native |

---

### Step 133 — Anti-Forensics — Secure Delete & Memory Cleanup
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Secure file delete — overwrite with random bytes N times before deletion. `File.WriteAllBytes` + `File.Delete` |
| 2 | Clear sensitive strings from managed memory using `SecureString` and `Marshal.ZeroFreeGlobalAllocUnicode` |

---

### Step 134 — Indicator Removal — Persistence Cleanup
**Duration: 2 days**

| Day | Goal |
|-----|------|
| 1 | Remove all registry run keys, scheduled tasks, and WMI subscriptions created during the operation |
| 2 | Build a `Cleanup` module for the C2 agent — single command removes all indicators. `cleanup --all` |

---

### Step 135 — Full Operation Simulation
**Duration: 3 days**

| Day | Goal |
|-----|------|
| 1 | Simulate a full attack chain: initial access → PrivEsc → credential dump → lateral movement → DA |
| 2 | Use only C# agent modules — no external tools. Document every step with C2 task logs |
| 3 | Run cleanup module — verify no indicators remain. Review what a defender would see in logs |

---

> **Note:** All tools must be used only in environments you are authorized to test — your own lab, a CTF, or systems covered by a formal penetration-testing agreement.
