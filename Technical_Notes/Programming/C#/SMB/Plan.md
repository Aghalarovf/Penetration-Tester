# SmbRecon — Professional C# Toolkit Build Plan
### 100-Step Implementation Roadmap · Steps 1–10 · Foundation & Architecture

---

## Table of Contents

- [Step 1 — Repository & Solution Layout](#step-1--repository--solution-layout)
- [Step 2 — Core: Primitive Models](#step-2--core-primitive-models)
- [Step 3 — Core: ISmbTool Contract](#step-3--core-ismbtool-contract)
- [Step 4 — Core: Transport Abstraction](#step-4--core-transport-abstraction)
- [Step 5 — Protocol/RPC Layer](#step-5--protocolrpc-layer)
- [Step 6 — Module Registration System](#step-6--module-registration-system)
- [Step 7 — CLI Skeleton & Flag Map](#step-7--cli-skeleton--flag-map)
- [Step 8 — Safety Pipeline](#step-8--safety-pipeline)
- [Step 9 — Output & Reporting Contracts](#step-9--output--reporting-contracts)
- [Step 10 — Test Harness & Golden Files](#step-10--test-harness--golden-files)

---

## Step 1 — Repository & Solution Layout

**Goal:** Establish a clean, scalable solution structure where every project has a
single responsibility and inter-project references are strictly enforced.

### Directory tree

```
SmbRecon/
├── src/
│   ├── SmbRecon.Core/               # Abstractions, models, no I/O
│   ├── SmbRecon.Protocol/           # SMB packet encoding/decoding, RPC stubs
│   ├── SmbRecon.Modules.Common/     # Tools 1–10  (dialect, signing, …)
│   ├── SmbRecon.Modules.SystemInfo/ # Tools 11–24
│   ├── SmbRecon.Modules.Domain/     # Tools 25–31
│   ├── SmbRecon.Modules.DC/         # Tools 32–39
│   ├── SmbRecon.Modules.Enumeration/# Tools 40–45
│   ├── SmbRecon.Modules.Objects/    # Tools 46–49
│   ├── SmbRecon.Modules.Vulns/      # Tools 50–53
│   ├── SmbRecon.Modules.ShareRecon/ # Tools 54–61
│   ├── SmbRecon.Modules.AuthRelay/  # Tools 62–67
│   ├── SmbRecon.Modules.PostEx/     # Tools 68–77
│   ├── SmbRecon.Modules.Analysis/   # Intelligence aggregation
│   ├── SmbRecon.Output/             # Console / JSON / Markdown reporters
│   └── SmbRecon.Cli/                # Entry point — references everything
├── tests/
│   ├── SmbRecon.Core.Tests/
│   ├── SmbRecon.Protocol.Tests/
│   └── SmbRecon.Modules.Common.Tests/
├── docs/
│   ├── TOOLS.md                     # Canonical tool reference (77 tools)
│   ├── ARCHITECTURE.md
│   └── CONTRIBUTING.md
├── SmbRecon.sln
├── .editorconfig
├── .gitignore
└── global.json                      # Pin SDK version
```

### Reference rules (enforced via `<ProjectReference>`)

| Referencing project | May reference |
|---|---|
| `Core` | nothing (pure abstractions) |
| `Protocol` | `Core` only |
| `Modules.*` | `Core` + `Protocol` only |
| `Output` | `Core` only |
| `Cli` | all of the above |

> **Rule:** No module project may ever reference another module project.
> Violations are caught by a custom Roslyn analyzer added in Step 10.

### Tasks

- [ ] Run `dotnet new sln -n SmbRecon`
- [ ] Scaffold all projects with `dotnet new classlib` / `dotnet new console`
- [ ] Add `global.json` pinning `net8.0`
- [ ] Add `<Nullable>enable</Nullable>` and `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` globally via `Directory.Build.props`
- [ ] Configure `.editorconfig`: 4-space indent, `file_header_template` with license block
- [ ] Add `.gitignore` (Visual Studio template + `*.user`, `launchSettings.json`)
- [ ] Create `docs/ARCHITECTURE.md` summarising the reference rules above

---

## Step 2 — Core: Primitive Models

**Goal:** Define all value types and data transfer objects (DTOs) that flow
through every layer without importing any I/O or network dependency.

### Files to create under `SmbRecon.Core/Models/`

```csharp
// SmbTarget.cs
public sealed record SmbTarget(
    string Host,           // IP or hostname
    int    Port    = 445,  // 445 (direct) or 139 (NetBIOS)
    string? Username = null,
    string? Password = null,
    string? NtlmHash = null,  // Pass-the-Hash: 32-hex or LMHASH:NTHASH
    string? Domain   = null,
    TimeSpan Timeout = default)
{
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);
    public TimeSpan EffectiveTimeout => Timeout == default ? DefaultTimeout : Timeout;
}
```

```csharp
// ToolResult.cs
public sealed record ToolResult(
    string          ToolName,
    bool            Success,
    IReadOnlyList<Finding> Findings,
    string?         RawError   = null,
    TimeSpan        Elapsed    = default);
```

```csharp
// Finding.cs
public sealed record Finding(
    Severity          Severity,    // Info | Low | Medium | High | Critical
    string            Key,         // machine-readable: "smb.dialect.max"
    string            Summary,     // human-readable one-liner
    object?           Value  = null,
    IReadOnlyDictionary<string, object>? Detail = null);
```

```csharp
// Severity.cs
public enum Severity { Info, Low, Medium, High, Critical }
```

```csharp
// ImpactLevel.cs
public enum ImpactLevel
{
    Passive,    // read-only observation — included in --safe runs
    Active,     // writes test artefacts, attempts auth — opt-in flag
    Intrusive   // may cause disruption (DoS-class) — --allow-intrusive required
}
```

```csharp
// SmbDialect.cs
public enum SmbDialect
{
    Unknown,
    Smb1,
    Smb202,
    Smb210,
    Smb300,
    Smb302,
    Smb311
}
```

### Design constraints

- All models are `record` types — immutable, value-equality, easy to serialize.
- `Value` on `Finding` is `object?` intentionally; the reporter layer handles formatting.
- No `System.Net` references anywhere in `Core`.

### Tasks

- [ ] Create all model files above
- [ ] Add `SmbRecon.Core` unit tests asserting record equality and `with`-expression cloning
- [ ] Add JSON serialization attributes (`[JsonConverter]`) for `SmbDialect` and `Severity` enums
- [ ] Document each model's fields in XML doc-comments (`<summary>`, `<param>`)

---

## Step 3 — Core: ISmbTool Contract

**Goal:** Define the single interface that every one of the 77 tools implements,
plus helper base class to reduce boilerplate.

### Interface

```csharp
// Abstractions/ISmbTool.cs
namespace SmbRecon.Core.Abstractions;

public interface ISmbTool
{
    /// <summary>Unique identifier — matches TOOLS.md entry.</summary>
    string Name { get; }

    /// <summary>Category slug — maps to --flag in CLI.</summary>
    string Category { get; }

    /// <summary>One-sentence description (≤ 120 chars) for --list output.</summary>
    string Description { get; }

    ImpactLevel Impact { get; }

    bool RequiresAuthentication { get; }

    Task<ToolResult> RunAsync(SmbTarget target, CancellationToken ct);
}
```

### Base class (reduces per-tool boilerplate)

```csharp
// Abstractions/SmbToolBase.cs
public abstract class SmbToolBase : ISmbTool
{
    public abstract string      Name        { get; }
    public abstract string      Category    { get; }
    public abstract string      Description { get; }
    public abstract ImpactLevel Impact      { get; }
    public virtual  bool        RequiresAuthentication => false;

    // Wraps RunCoreAsync, handles Elapsed measurement + unhandled exceptions
    public async Task<ToolResult> RunAsync(SmbTarget target, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var findings = await RunCoreAsync(target, ct);
            return new ToolResult(Name, true, findings, Elapsed: sw.Elapsed);
        }
        catch (OperationCanceledException)
        {
            return new ToolResult(Name, false, Array.Empty<Finding>(),
                "Cancelled", sw.Elapsed);
        }
        catch (Exception ex)
        {
            return new ToolResult(Name, false, Array.Empty<Finding>(),
                ex.Message, sw.Elapsed);
        }
    }

    protected abstract Task<IReadOnlyList<Finding>> RunCoreAsync(
        SmbTarget target, CancellationToken ct);
}
```

### Contract invariants

| Invariant | Enforcement |
|---|---|
| `Name` must match an entry in `docs/TOOLS.md` | CI script diff-checks the list |
| `Category` must match a known slug | Roslyn analyzer |
| `Passive` tools must not write to the target | Code-review checklist |
| `RunAsync` must never throw — only return `Success = false` | `SmbToolBase` wrapper |

### Tasks

- [ ] Create `ISmbTool.cs` and `SmbToolBase.cs`
- [ ] Write unit tests that verify `SmbToolBase` exception → `ToolResult(Success=false)` conversion
- [ ] Add a `[ToolMetadata]` attribute (optional) for reflection-based documentation generation
- [ ] Create `docs/TOOLS.md` stub with all 77 tool names; CI script validates `Name` consistency

---

## Step 4 — Core: Transport Abstraction

**Goal:** Decouple every tool from real TCP sockets so that the entire module
layer can be unit-tested with mock transports and golden packet files.

### Interface

```csharp
// Abstractions/ISmbTransport.cs
namespace SmbRecon.Core.Abstractions;

public interface ISmbTransport : IAsyncDisposable
{
    bool IsConnected { get; }

    Task ConnectAsync(SmbTarget target, CancellationToken ct);

    /// <summary>Send raw bytes; return the server's complete response frame.</summary>
    Task<byte[]> SendReceiveAsync(byte[] request, CancellationToken ct);

    Task DisconnectAsync();
}
```

```csharp
// Abstractions/ISmbTransportFactory.cs
public interface ISmbTransportFactory
{
    ISmbTransport Create(SmbTarget target);
}
```

### Why this boundary exists

```
Modules.Common
    └── SmbDialectProber
           ├── builds raw NEGOTIATE bytes          (Protocol layer)
           ├── calls ISmbTransport.SendReceiveAsync (Core abstraction)
           └── parses response bytes               (Protocol layer)
```

The tool never touches `TcpClient` directly. In production the factory injects
`TcpSmbTransport`; in tests it injects `MockSmbTransport`.

### TcpSmbTransport (lives in Protocol, not Core)

```csharp
// SmbRecon.Protocol/Transport/TcpSmbTransport.cs
public sealed class TcpSmbTransport : ISmbTransport
{
    private TcpClient?    _tcp;
    private NetworkStream? _stream;

    public bool IsConnected => _tcp?.Connected ?? false;

    public async Task ConnectAsync(SmbTarget target, CancellationToken ct)
    {
        _tcp = new TcpClient();
        await _tcp.ConnectAsync(target.Host, target.Port, ct);
        _stream = _tcp.GetStream();
    }

    public async Task<byte[]> SendReceiveAsync(byte[] request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(_stream);
        await _stream.WriteAsync(request, ct);
        return await ReadFrameAsync(_stream, ct); // reads 4-byte NetBIOS length prefix
    }

    public async Task DisconnectAsync() => _tcp?.Close();
    public async ValueTask DisposeAsync() => await DisconnectAsync();

    private static async Task<byte[]> ReadFrameAsync(
        NetworkStream stream, CancellationToken ct) { /* … */ }
}
```

### Tasks

- [ ] Define `ISmbTransport` and `ISmbTransportFactory` in `Core`
- [ ] Implement `TcpSmbTransport` in `Protocol`
- [ ] Implement `MockSmbTransport` in `Core.Tests` (injects raw `byte[]` responses from files)
- [ ] Write a `TransportFactory` that reads `SmbTarget.Port` to choose 445 vs 139 framing

---

## Step 5 — Protocol/RPC Layer

**Goal:** Centralise all byte-level SMB packet construction, parsing, and RPC
stub generation so that no module ever hard-codes a magic byte offset.

### Project structure

```
SmbRecon.Protocol/
├── Smb/
│   ├── SmbNegotiateRequest.cs      # Dialect list builder
│   ├── SmbNegotiateResponse.cs     # Parser: dialect, SecurityMode, Capabilities
│   ├── SmbSessionSetupRequest.cs
│   ├── SmbSessionSetupResponse.cs
│   └── SmbPacketParser.cs          # Entry point: byte[] → ISmbPacket
├── Rpc/
│   ├── RpcBindRequest.cs           # DCE/RPC Bind PDU
│   ├── RpcBindAck.cs
│   ├── RpcRequestPdu.cs
│   ├── Stubs/
│   │   ├── SrvsvcStub.cs           # NetShareEnumAll, NetSessionEnum
│   │   ├── SamrStub.cs             # EnumerateUsersInDomain
│   │   ├── LsarpcStub.cs           # LsarQueryInformationPolicy
│   │   ├── WkssvcStub.cs           # NetWkstaGetInfo
│   │   ├── SvcctlStub.cs           # EnumServicesStatusExW
│   │   └── WinregStub.cs           # RegConnectRegistry
│   └── RpcChannel.cs               # Binds a named pipe over ISmbTransport
└── Transport/
    └── TcpSmbTransport.cs          # (from Step 4)
```

### Key design decisions

1. **No code generation.** RPC stubs are handwritten; full NDR serializers are
   overkill for 77 focused tools. Use `BinaryPrimitives` + `Span<byte>`.

2. **Packet objects are immutable records** with a `ToBytes()` method and a
   static `Parse(ReadOnlySpan<byte>)` factory — matching the Core model pattern.

3. **Constants file** — every flag, capability bit, and error code lives in
   `SmbConstants.cs` with the MS-SMB2 / MS-SRVS spec reference as a comment.

```csharp
// Protocol/Smb/SmbConstants.cs
public static class SmbConstants
{
    // MS-SMB2 §2.2.3  NEGOTIATE Response SecurityMode
    public const ushort NEGOTIATE_SIGNING_ENABLED  = 0x0001;
    public const ushort NEGOTIATE_SIGNING_REQUIRED = 0x0002;

    // MS-SMB2 §2.2.3  GlobalCapabilities
    public const uint CAP_DFS         = 0x00000001;
    public const uint CAP_LEASING     = 0x00000002;
    public const uint CAP_LARGE_MTU   = 0x00000004;
    public const uint CAP_MULTI_CHAN  = 0x00000008;
    public const uint CAP_PERSISTENT  = 0x00000010;
    public const uint CAP_DIR_LEASING = 0x00000020;
    public const uint CAP_ENCRYPTION  = 0x00000040;
    public const uint CAP_NOTIFICATIONS = 0x00000080;
    public const uint CAP_COMPRESSION  = 0x00000100; // SMBv3.1.1
}
```

### Tasks

- [ ] Create `SmbNegotiateRequest` with dialect list builder (all dialects, or filtered)
- [ ] Create `SmbNegotiateResponse` parser covering SMBv1/v2/v3 response shapes
- [ ] Create `RpcChannel` that binds a named pipe (`\pipe\svcctl` etc.) over `ISmbTransport`
- [ ] Create `SmbConstants.cs` covering all flags used by the 77 tools
- [ ] Add `Protocol` unit tests: serialise → parse round-trip on captured packet fixtures

---

## Step 6 — Module Registration System

**Goal:** Each module project exposes its tools through a static `GetTools()`
method; the CLI discovers all modules at startup via assembly scanning, with
**zero changes** to `Cli` when a new module is added.

### Per-module registration file

```csharp
// SmbRecon.Modules.Common/CommonModule.cs
namespace SmbRecon.Modules.Common;

public static class CommonModule
{
    public const string Flag     = "--common";
    public const string Category = "common";

    public static IEnumerable<ISmbTool> GetTools() =>
        new ISmbTool[]
        {
            new SmbDialectProber(),
            new SmbSigningAuditor(),
            new SmbCapabilityScanner(),
            new Smbv1Detector(),
            new SmbEncryptionChecker(),
            new SmbPortMapper(),
            new SmbDialectDowngradeChecker(),
            new GuestAccessProber(),
            new SmbServerStringExtractor(),
            new SmbTimeoutBehaviorProber(),
        };
}
```

### CLI-side auto-discovery

```csharp
// SmbRecon.Cli/ModuleLoader.cs
public static class ModuleLoader
{
    private const string ModuleAssemblyPrefix = "SmbRecon.Modules";

    public static IReadOnlyList<ISmbTool> DiscoverAll()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()                                           // already-loaded
            .Concat(                                                    // + referenced
                typeof(ModuleLoader).Assembly
                    .GetReferencedAssemblies()
                    .Where(a => a.Name!.StartsWith(ModuleAssemblyPrefix))
                    .Select(Assembly.Load))
            .Distinct()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && t.IsAbstract == false == false     // static classes
                        && t.Name.EndsWith("Module")
                        && t.GetMethod("GetTools", BindingFlags.Public | BindingFlags.Static) != null)
            .SelectMany(m =>
                (IEnumerable<ISmbTool>)m.GetMethod("GetTools")!.Invoke(null, null)!)
            .ToList();
    }
}
```

### Registration contract

Every module **must** declare three public `const string` fields:

| Field | Example | Used by |
|---|---|---|
| `Flag` | `"--common"` | CLI flag parser |
| `Category` | `"common"` | `ISmbTool.Category` cross-validation |
| `DisplayName` | `"Common SMB"` | `--list` output |

A CI test asserts that every discovered `ISmbTool.Category` matches the `Category`
constant of its hosting module.

### Tasks

- [ ] Create `CommonModule.cs` (stub — all 10 tools as `new SmbDialectProber()` placeholders)
- [ ] Create identical stubs for the remaining 10 modules
- [ ] Write `ModuleLoader` as above
- [ ] Write a CI test: `DiscoverAll()` returns exactly 77 tools; no duplicate `Name`
- [ ] Document the three-constant contract in `docs/CONTRIBUTING.md`

---

## Step 7 — CLI Skeleton & Flag Map

**Goal:** Build a complete, production-quality CLI surface with all flags wired
to module selectors before a single real tool is implemented.

### Flag map (`FlagMap.cs`)

```csharp
// SmbRecon.Cli/FlagMap.cs
public static class FlagMap
{
    public static readonly IReadOnlyDictionary<string, string> FlagToCategory =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["--common"]      = "common",
            ["--system-info"] = "system-info",
            ["--domain"]      = "domain",
            ["--dc"]          = "dc",
            ["--enum"]        = "enumeration",
            ["--objects"]     = "objects",
            ["--vulns"]       = "vulnerabilities",
            ["--shares"]      = "share-recon",
            ["--auth"]        = "auth-relay",
            ["--postex"]      = "post-ex",
            ["--analysis"]    = "analysis",
        };
}
```

### CLI argument model (`CliOptions.cs`)

```csharp
public sealed class CliOptions
{
    // Target
    public string?   Target   { get; init; }
    public int       Port     { get; init; } = 445;
    public string?   Username { get; init; }
    public string?   Password { get; init; }
    public string?   Hash     { get; init; }   // NTLM hash for PtH
    public string?   Domain   { get; init; }
    public int       Timeout  { get; init; } = 5;

    // Module selectors
    public bool      All       { get; init; }
    public List<string> Categories { get; init; } = new();
    public List<string> Only    { get; init; } = new();  // --only Tool1,Tool2

    // Safety
    public bool      Safe            { get; init; } = true;   // default
    public bool      AllowIntrusive  { get; init; }

    // Output
    public OutputFormat Format  { get; init; } = OutputFormat.Console;
    public string?      OutFile { get; init; }
    public int          Threads { get; init; } = 1;

    // Meta
    public bool List { get; init; }
    public bool Help { get; init; }
}

public enum OutputFormat { Console, Json, Markdown }
```

### Usage surface

```
SmbRecon --list
SmbRecon --common --target 10.0.0.5
SmbRecon --common --system-info --target 10.0.0.5
SmbRecon --only SMBDialectProber,SMBv1Detector --target 10.0.0.5
SmbRecon --all --target 10.0.0.5 --safe
SmbRecon --all --target 10.0.0.5 --allow-intrusive
SmbRecon --vulns --target 10.0.0.5 --json --out report.json
SmbRecon --common --target 10.0.0.5 --threads 10
SmbRecon --postex --target 10.0.0.5 --hash aad3b435b51404eeaad3b435b51404ee:8846f7eaee8fb117ad06bdd830b7586c
```

### Tasks

- [ ] Implement `CliParser` using `System.CommandLine` (or manual arg parsing if preferred)
- [ ] Implement `FlagMap.cs` as above
- [ ] Wire `--list` to print a formatted table from `ModuleLoader.DiscoverAll()`
- [ ] Implement `--only` filter (comma-separated `ISmbTool.Name` values)
- [ ] Validate that `--allow-intrusive` is present when `Intrusive` tools are selected
- [ ] Add `--help` with full usage text and examples
- [ ] Add integration test: `--list` produces exactly 77 lines (once all stubs exist)

---

## Step 8 — Safety Pipeline

**Goal:** Make it architecturally impossible to run an `Intrusive` tool without
an explicit flag, and impossible to run `Active` tools in the default `--safe`
mode — enforced at the pipeline level, not inside each tool.

### Safety filter

```csharp
// SmbRecon.Cli/Pipeline/SafetyFilter.cs
public static class SafetyFilter
{
    public static IReadOnlyList<ISmbTool> Apply(
        IEnumerable<ISmbTool> tools,
        CliOptions opts)
    {
        var allowed = new List<ISmbTool>();
        var blocked = new List<(ISmbTool Tool, string Reason)>();

        foreach (var tool in tools)
        {
            var reason = Evaluate(tool, opts);
            if (reason is null) allowed.Add(tool);
            else                blocked.Add((tool, reason));
        }

        if (blocked.Count > 0)
            PrintBlockedWarning(blocked);

        return allowed;
    }

    private static string? Evaluate(ISmbTool tool, CliOptions opts) =>
        tool.Impact switch
        {
            ImpactLevel.Intrusive when !opts.AllowIntrusive =>
                "Intrusive — pass --allow-intrusive to enable",
            ImpactLevel.Active when opts.Safe =>
                "Active — pass --no-safe or remove --safe to enable",
            _ => null
        };
}
```

### Impact matrix

| `ImpactLevel` | Default (`--safe`) | `--no-safe` | `--allow-intrusive` |
|---|---|---|---|
| `Passive` | ✅ runs | ✅ runs | ✅ runs |
| `Active` | ❌ blocked | ✅ runs | ✅ runs |
| `Intrusive` | ❌ blocked | ❌ blocked | ✅ runs |

### Warning output (console)

```
[SAFETY] 2 tool(s) blocked by current safety settings:
  · SMBLorisDetector   [Intrusive]  — pass --allow-intrusive to enable
  · ShareWriteProber   [Active]     — pass --no-safe to enable
```

### Tasks

- [ ] Implement `SafetyFilter.Apply()` as above
- [ ] Wire `SafetyFilter` into the main execution pipeline (before thread dispatch)
- [ ] Write unit tests: Intrusive tool + `--safe` → blocked; Passive + any flag → runs
- [ ] Document the impact classification table in `docs/TOOLS.md` for each of the 77 tools

---

## Step 9 — Output & Reporting Contracts

**Goal:** Define the output contract so that every reporter (console, JSON,
Markdown) receives the same `ToolResult` stream and the module layer never
formats its own output.

### Reporter interface

```csharp
// SmbRecon.Output/IReporter.cs
namespace SmbRecon.Output;

public interface IReporter
{
    /// <summary>Called once before any tools run.</summary>
    void Begin(SmbTarget target, IReadOnlyList<ISmbTool> planned);

    /// <summary>Called immediately when a tool finishes.</summary>
    void Report(ToolResult result);

    /// <summary>Called once after all tools complete.</summary>
    void End(TimeSpan totalElapsed);
}
```

### Console reporter (production default)

```
[✔] SMBDialectProber          (42ms)
    smb.dialect.max      : SMBv3.1.1
    smb.dialect.list     : SMBv1, SMBv2.1, SMBv3.0, SMBv3.1.1

[✔] SMBSigningAuditor          (18ms)
    smb.signing.required : false    [HIGH]
    smb.signing.enabled  : true

[✗] SMBv1Detector              (7ms)  ERROR: Connection refused
```

### JSON reporter

Emits a single JSON array of `ToolResult` objects. Each `Finding.Value` is
serialised to its native JSON type (string, number, boolean, array).

### Markdown reporter

Suitable for including in pentest reports. Each tool result is a `## Heading`
with a findings table. Severity badges render as GitHub-flavored Markdown emoji.

### Reporter factory

```csharp
// SmbRecon.Output/ReporterFactory.cs
public static class ReporterFactory
{
    public static IReporter Create(OutputFormat format, TextWriter? writer = null)
        => format switch
        {
            OutputFormat.Json     => new JsonReporter(writer ?? Console.Out),
            OutputFormat.Markdown => new MarkdownReporter(writer ?? Console.Out),
            _                     => new ConsoleReporter()
        };
}
```

### Tasks

- [ ] Define `IReporter` in `SmbRecon.Output`
- [ ] Implement `ConsoleReporter` with ANSI colour output (green ✔ / red ✗)
- [ ] Implement `JsonReporter` using `System.Text.Json`
- [ ] Implement `MarkdownReporter` with severity-badge mapping
- [ ] Implement `ReporterFactory`
- [ ] Write snapshot tests: given a fixed `ToolResult`, assert exact console/JSON/Markdown output

---

## Step 10 — Test Harness & Golden Files

**Goal:** Establish the testing infrastructure so that every tool can be
developed and validated without a live SMB server, using captured packet fixtures.

### Test infrastructure

```
tests/
├── SmbRecon.Core.Tests/
│   ├── Models/
│   │   ├── SmbTargetTests.cs
│   │   ├── ToolResultTests.cs
│   │   └── FindingTests.cs
│   └── Abstractions/
│       └── SmbToolBaseTests.cs      # exception → ToolResult(false) contract
├── SmbRecon.Protocol.Tests/
│   ├── Fixtures/
│   │   ├── negotiate_smb311_response.bin   # real captured frame
│   │   ├── negotiate_smb1_response.bin
│   │   └── session_setup_ntlmssp.bin
│   ├── SmbNegotiateRequestTests.cs
│   └── SmbNegotiateResponseTests.cs
└── SmbRecon.Modules.Common.Tests/
    ├── SmbDialectProberTests.cs      # uses MockSmbTransport + fixture above
    └── Smbv1DetectorTests.cs
```

### Mock transport

```csharp
// SmbRecon.Core.Tests/Mocks/MockSmbTransport.cs
public sealed class MockSmbTransport : ISmbTransport
{
    private readonly Queue<byte[]> _responses;

    public MockSmbTransport(params byte[][] responses)
        => _responses = new Queue<byte[]>(responses);

    public bool IsConnected { get; private set; }

    public Task ConnectAsync(SmbTarget target, CancellationToken ct)
    { IsConnected = true; return Task.CompletedTask; }

    public Task<byte[]> SendReceiveAsync(byte[] request, CancellationToken ct)
        => Task.FromResult(_responses.Dequeue());

    public Task DisconnectAsync()
    { IsConnected = false; return Task.CompletedTask; }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
```

### Golden-file pattern

```csharp
[Fact]
public async Task SmbDialectProber_DetectsSmb311_FromCapturedFrame()
{
    // Arrange
    var response = File.ReadAllBytes("Fixtures/negotiate_smb311_response.bin");
    var transport = new MockSmbTransport(response);
    var factory   = Mock.Of<ISmbTransportFactory>(f =>
        f.Create(It.IsAny<SmbTarget>()) == transport);

    var tool = new SmbDialectProber(factory);

    // Act
    var result = await tool.RunAsync(
        new SmbTarget("127.0.0.1"), CancellationToken.None);

    // Assert
    Assert.True(result.Success);
    var dialect = result.Findings.Single(f => f.Key == "smb.dialect.max");
    Assert.Equal(SmbDialect.Smb311, dialect.Value);
}
```

### Roslyn analyzer (reference-rule enforcement)

Add `Microsoft.CodeAnalysis.Analyzers` and write a simple analyzer that:
- Errors when a `Modules.*` project references another `Modules.*` project
- Errors when a `Core` project references any `System.Net` type

### CI pipeline (`ci.yml` in GitHub Actions)

```yaml
jobs:
  build:
    steps:
      - dotnet restore
      - dotnet build --no-restore -warnaserror
      - dotnet test --no-build --collect:"XPlat Code Coverage"
      - dotnet tool run reportgenerator  # coverage gate ≥ 80 %
      - python scripts/validate_tools.py # TOOLS.md ↔ GetTools() diff
```

### Tasks

- [ ] Add `MockSmbTransport` to `Core.Tests`
- [ ] Capture and commit at least 5 real SMB negotiate frames as `.bin` fixtures
- [ ] Write round-trip tests for `SmbNegotiateRequest` ↔ `SmbNegotiateResponse`
- [ ] Write the first golden-file test for `SmbDialectProber`
- [ ] Set up GitHub Actions (or equivalent) CI pipeline with coverage gate
- [ ] Write the `validate_tools.py` script that diffs `TOOLS.md` against `GetTools()` output
- [ ] Add Roslyn analyzer for reference-rule enforcement

---

## Foundation Checkpoint

Before proceeding to Steps 11–20 (implementing the 10 Common SMB tools),
verify every item in this checklist:

- [ ] Solution builds with zero warnings (`-warnaserror`)
- [ ] `dotnet test` passes — all Core + Protocol + stub module tests green
- [ ] `--list` outputs exactly the expected module/tool structure
- [ ] `--safe` blocks all `Active` and `Intrusive` tools
- [ ] `--allow-intrusive` unblocks `Intrusive` tools but `Active` still needs `--no-safe`
- [ ] `--json` produces valid JSON; `--markdown` produces valid GitHub-flavored Markdown
- [ ] `MockSmbTransport` + golden-file pattern established in at least one test
- [ ] CI pipeline green on `main`
- [ ] `docs/ARCHITECTURE.md` and `docs/TOOLS.md` stubs committed
- [ ] Zero cross-module references (Roslyn analyzer clean)

---

*Steps 11–20 (Common SMB Module — all 10 tools implemented) will be covered in the next batch.*
