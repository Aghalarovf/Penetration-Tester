# SmbRecon Python — 100-Step Implementation Roadmap (Steps 1–10)

### Professional Python Toolkit Build Plan · Foundation & Architecture

---

## Table of Contents

- [Step 1 — Repository & Package Layout](#step-1--repository--package-layout)
- [Step 2 — Core: Primitive Models](#step-2--core-primitive-models)
- [Step 3 — Core: ISmbTool Protocol & Base Class](#step-3--core-ismbtool-protocol--base-class)
- [Step 4 — Core: Transport Abstraction](#step-4--core-transport-abstraction)
- [Step 5 — Protocol/RPC Layer](#step-5--protocolrpc-layer)
- [Step 6 — Module Registration System](#step-6--module-registration-system)
- [Step 7 — CLI Skeleton & Flag Map](#step-7--cli-skeleton--flag-map)
- [Step 8 — Safety Pipeline](#step-8--safety-pipeline)
- [Step 9 — Output & Reporting Contracts](#step-9--output--reporting-contracts)
- [Step 10 — Test Harness & Golden Files](#step-10--test-harness--golden-files)

---

## Step 1 — Repository & Package Layout

**Goal:** Establish a clean, scalable package structure where every sub-package has a single responsibility and inter-package imports are strictly enforced via module boundaries.

### Directory tree

```
smbrecon/
├── smbrecon/
│   ├── __init__.py
│   ├── core/                        # Abstractions, models — no I/O
│   │   ├── __init__.py
│   │   ├── models/
│   │   │   ├── __init__.py
│   │   │   ├── smb_target.py
│   │   │   ├── finding.py
│   │   │   ├── tool_result.py
│   │   │   └── enums.py
│   │   └── abstractions/
│   │       ├── __init__.py
│   │       ├── smb_tool.py          # ISmbTool Protocol + base class
│   │       └── smb_transport.py     # ISmbTransport Protocol
│   ├── protocol/                    # Packet encoding/decoding, RPC stubs
│   │   ├── __init__.py
│   │   ├── smb/
│   │   │   ├── smb_negotiate.py
│   │   │   ├── smb_session_setup.py
│   │   │   ├── smb_packet_parser.py
│   │   │   └── smb_constants.py
│   │   ├── rpc/
│   │   │   ├── rpc_bind.py
│   │   │   ├── rpc_channel.py
│   │   │   └── stubs/
│   │   │       ├── srvsvc.py
│   │   │       ├── samr.py
│   │   │       ├── lsarpc.py
│   │   │       ├── wkssvc.py
│   │   │       ├── svcctl.py
│   │   │       └── winreg.py
│   │   └── transport/
│   │       └── tcp_smb_transport.py
│   ├── modules/
│   │   ├── common/                  # Tools 1–10
│   │   ├── system_info/             # Tools 11–24
│   │   ├── domain/                  # Tools 25–31
│   │   ├── dc/                      # Tools 32–39
│   │   ├── enumeration/             # Tools 40–45
│   │   ├── objects/                 # Tools 46–49
│   │   ├── vulns/                   # Tools 50–53
│   │   ├── share_recon/             # Tools 54–61
│   │   ├── auth_relay/              # Tools 62–67
│   │   ├── post_ex/                 # Tools 68–77
│   │   └── analysis/
│   ├── output/
│   │   ├── reporter.py
│   │   ├── console_reporter.py
│   │   ├── json_reporter.py
│   │   └── markdown_reporter.py
│   └── cli/
│       ├── main.py
│       ├── flag_map.py
│       ├── cli_options.py
│       ├── module_loader.py
│       └── pipeline/
│           └── safety_filter.py
├── tests/
│   ├── core/
│   ├── protocol/
│   │   └── fixtures/
│   └── modules/common/
├── docs/
│   ├── TOOLS.md
│   ├── ARCHITECTURE.md
│   └── CONTRIBUTING.md
├── scripts/
│   └── validate_tools.py
├── pyproject.toml
├── .editorconfig
├── .gitignore
└── tox.ini
```

### Import rules

| Package | May import from |
|---|---|
| `core` | stdlib only |
| `protocol` | `core` only |
| `modules.*` | `core` + `protocol` only |
| `output` | `core` only |
| `cli` | all of the above |

> **Rule:** No module package may ever import from another module package. Violations are caught by a custom `flake8` plugin added in Step 10.

### Tasks

- [ ] Create the full directory tree with all `__init__.py` stubs
- [ ] Set up `pyproject.toml` with `[project]`, `[project.optional-dependencies]` (dev, test)
- [ ] Pin `python_requires = ">=3.11"`
- [ ] Configure `ruff` for linting + formatting
- [ ] Add `mypy` with `strict = true` in `pyproject.toml`
- [ ] Add `.editorconfig`: 4-space indent, UTF-8, LF line endings
- [ ] Add `.gitignore` (Python template + `__pycache__/`, `.mypy_cache/`, `.ruff_cache/`)
- [ ] Create `docs/ARCHITECTURE.md` summarising the import rules above

---

## Step 2 — Core: Primitive Models

**Goal:** Define all value types and dataclasses that flow through every layer without importing any I/O or network dependency.

### Files under `smbrecon/core/models/`

```python
# enums.py
from enum import Enum, auto

class Severity(Enum):
    INFO     = auto()
    LOW      = auto()
    MEDIUM   = auto()
    HIGH     = auto()
    CRITICAL = auto()

class ImpactLevel(Enum):
    PASSIVE    = auto()   # read-only — included in --safe runs
    ACTIVE     = auto()   # writes test artefacts, attempts auth
    INTRUSIVE  = auto()   # may cause disruption — --allow-intrusive required

class SmbDialect(Enum):
    UNKNOWN = auto()
    SMB1    = auto()
    SMB202  = auto()
    SMB210  = auto()
    SMB300  = auto()
    SMB302  = auto()
    SMB311  = auto()
```

```python
# smb_target.py
from __future__ import annotations
from dataclasses import dataclass, field
from datetime import timedelta

DEFAULT_TIMEOUT = timedelta(seconds=5)

@dataclass(frozen=True)
class SmbTarget:
    host:      str
    port:      int           = 445
    username:  str | None    = None
    password:  str | None    = None
    ntlm_hash: str | None    = None   # "LMHASH:NTHASH" or 32-hex NT only
    domain:    str | None    = None
    timeout:   timedelta     = field(default_factory=lambda: DEFAULT_TIMEOUT)

    @property
    def effective_timeout(self) -> timedelta:
        return self.timeout if self.timeout else DEFAULT_TIMEOUT
```

```python
# finding.py
from __future__ import annotations
from dataclasses import dataclass, field
from typing import Any
from .enums import Severity

@dataclass(frozen=True)
class Finding:
    severity: Severity
    key:      str                          # machine-readable: "smb.dialect.max"
    summary:  str                          # human-readable one-liner
    value:    Any                   = None
    detail:   dict[str, Any] | None = None
```

```python
# tool_result.py
from __future__ import annotations
from dataclasses import dataclass, field
from datetime import timedelta
from .finding import Finding

@dataclass(frozen=True)
class ToolResult:
    tool_name: str
    success:   bool
    findings:  tuple[Finding, ...]  = field(default_factory=tuple)
    raw_error: str | None           = None
    elapsed:   timedelta            = field(default_factory=timedelta)
```

### Design constraints

- All models use `@dataclass(frozen=True)` — immutable, hashable, value-equality via `__eq__`.
- `value` on `Finding` is `Any` intentionally; the reporter layer handles formatting.
- `tuple` is used instead of `list` for `findings` to enforce immutability.
- No `socket`, `asyncio`, or any I/O import anywhere in `core`.

### Tasks

- [ ] Create all four model files above
- [ ] Add `__all__` exports to `core/models/__init__.py`
- [ ] Add JSON serialization helpers using `dataclasses.asdict` + a custom `Enum` encoder
- [ ] Write unit tests asserting frozen equality, `dataclasses.replace()` cloning, and `Enum` values
- [ ] Add docstrings to every field (`"""machine-readable key, e.g. smb.dialect.max"""`)

---

## Step 3 — Core: ISmbTool Protocol & Base Class

**Goal:** Define the single `Protocol` that every one of the 77 tools implements, plus a base class to reduce per-tool boilerplate.

### Protocol (structural subtyping)

```python
# smbrecon/core/abstractions/smb_tool.py
from __future__ import annotations
from typing import Protocol, runtime_checkable
from ..models import SmbTarget, ToolResult, ImpactLevel

@runtime_checkable
class ISmbTool(Protocol):
    """Single interface every tool implements. Matches TOOLS.md entry."""

    @property
    def name(self) -> str:
        """Unique identifier — matches TOOLS.md entry."""
        ...

    @property
    def category(self) -> str:
        """Category slug — maps to --flag in CLI."""
        ...

    @property
    def description(self) -> str:
        """One-sentence description (<=120 chars) for --list output."""
        ...

    @property
    def impact(self) -> ImpactLevel:
        ...

    @property
    def requires_authentication(self) -> bool:
        ...

    async def run(self, target: SmbTarget, /) -> ToolResult:
        ...
```

### Base class (reduces per-tool boilerplate)

```python
# smbrecon/core/abstractions/smb_tool_base.py
from __future__ import annotations
import asyncio
from datetime import timedelta
from time import perf_counter
from ..models import SmbTarget, ToolResult, Finding, ImpactLevel

class SmbToolBase:
    """Wraps _run_core, measures elapsed time, handles all unhandled exceptions."""

    requires_authentication: bool = False

    async def run(self, target: SmbTarget, /) -> ToolResult:
        start = perf_counter()
        try:
            findings = await self._run_core(target)
            return ToolResult(
                tool_name=self.name,
                success=True,
                findings=tuple(findings),
                elapsed=timedelta(seconds=perf_counter() - start),
            )
        except asyncio.CancelledError:
            return ToolResult(
                tool_name=self.name, success=False,
                raw_error="Cancelled",
                elapsed=timedelta(seconds=perf_counter() - start),
            )
        except Exception as exc:  # noqa: BLE001
            return ToolResult(
                tool_name=self.name, success=False,
                raw_error=str(exc),
                elapsed=timedelta(seconds=perf_counter() - start),
            )

    async def _run_core(self, target: SmbTarget) -> list[Finding]:
        raise NotImplementedError
```

### Contract invariants

| Invariant | Enforcement |
|---|---|
| `name` must match a `TOOLS.md` entry | CI `validate_tools.py` script |
| `category` must match a known slug | unit test in `test_module_registration.py` |
| `PASSIVE` tools must not write to the target | Code-review checklist |
| `run()` must never raise — only return `success=False` | `SmbToolBase` wrapper |

### Tasks

- [ ] Create `smb_tool.py` (`ISmbTool` Protocol) and `smb_tool_base.py` (`SmbToolBase`)
- [ ] Write unit tests: `SmbToolBase` exception → `ToolResult(success=False)` conversion
- [ ] Write unit tests: `CancelledError` → `ToolResult(success=False, raw_error="Cancelled")`
- [ ] Add a `@tool_metadata` dataclass decorator (optional) for reflection-based doc generation
- [ ] Create `docs/TOOLS.md` stub listing all 77 tool names; CI script validates `name` consistency

---

## Step 4 — Core: Transport Abstraction

**Goal:** Decouple every tool from real TCP sockets so the entire module layer can be unit-tested with mock transports and golden packet files.

### Protocol

```python
# smbrecon/core/abstractions/smb_transport.py
from __future__ import annotations
from typing import Protocol, runtime_checkable
from ..models import SmbTarget

@runtime_checkable
class ISmbTransport(Protocol):

    @property
    def is_connected(self) -> bool: ...

    async def connect(self, target: SmbTarget, /) -> None: ...

    async def send_receive(self, request: bytes, /) -> bytes:
        """Send raw bytes; return the server's complete response frame."""
        ...

    async def disconnect(self) -> None: ...

    async def __aenter__(self) -> ISmbTransport: ...
    async def __aexit__(self, *_: object) -> None: ...


class ISmbTransportFactory(Protocol):
    def create(self, target: SmbTarget) -> ISmbTransport: ...
```

### Why this boundary exists

```
modules/common/smb_dialect_prober.py
    ├── builds raw NEGOTIATE bytes          (protocol layer)
    ├── calls ISmbTransport.send_receive()  (core abstraction)
    └── parses response bytes               (protocol layer)
```

The tool never touches `asyncio.open_connection` directly. In production the factory injects `TcpSmbTransport`; in tests it injects `MockSmbTransport`.

### TcpSmbTransport (lives in `protocol/`, not `core/`)

```python
# smbrecon/protocol/transport/tcp_smb_transport.py
from __future__ import annotations
import asyncio
import struct
from smbrecon.core.models import SmbTarget

class TcpSmbTransport:
    """Real TCP transport using asyncio streams."""

    def __init__(self) -> None:
        self._reader: asyncio.StreamReader | None = None
        self._writer: asyncio.StreamWriter | None = None

    @property
    def is_connected(self) -> bool:
        return self._writer is not None and not self._writer.is_closing()

    async def connect(self, target: SmbTarget, /) -> None:
        self._reader, self._writer = await asyncio.wait_for(
            asyncio.open_connection(target.host, target.port),
            timeout=target.effective_timeout.total_seconds(),
        )

    async def send_receive(self, request: bytes, /) -> bytes:
        assert self._writer and self._reader
        self._writer.write(request)
        await self._writer.drain()
        return await self._read_frame()

    async def _read_frame(self) -> bytes:
        """Read 4-byte NetBIOS length prefix then the frame."""
        assert self._reader
        header = await self._reader.readexactly(4)
        length = struct.unpack(">I", header)[0]
        return await self._reader.readexactly(length)

    async def disconnect(self) -> None:
        if self._writer:
            self._writer.close()
            await self._writer.wait_closed()

    async def __aenter__(self) -> TcpSmbTransport:
        return self

    async def __aexit__(self, *_: object) -> None:
        await self.disconnect()
```

### Tasks

- [ ] Define `ISmbTransport` and `ISmbTransportFactory` in `core/abstractions/`
- [ ] Implement `TcpSmbTransport` in `protocol/transport/`
- [ ] Implement `MockSmbTransport` in `tests/core/` (injects raw `bytes` responses from files)
- [ ] Write a `TransportFactory` that reads `SmbTarget.port` to choose 445 vs 139 framing
- [ ] Write tests asserting `MockSmbTransport` dequeues responses in order

---

## Step 5 — Protocol/RPC Layer

**Goal:** Centralise all byte-level SMB packet construction, parsing, and RPC stub generation so no module ever hard-codes a magic byte offset.

### Key design decisions

1. **No code generation.** RPC stubs are handwritten using `struct` and `memoryview`. Full NDR serializers are overkill for 77 focused tools.
2. **Packet objects are frozen dataclasses** with a `to_bytes() -> bytes` method and a `@classmethod parse(data: bytes)` factory — matching the Core model pattern.
3. **Constants file** — every flag, capability bit, and error code lives in `smb_constants.py` with the MS-SMB2 / MS-SRVS spec reference as a comment.

### Package structure

```
smbrecon/protocol/
├── smb/
│   ├── smb_constants.py          # All spec-defined flags and capability bits
│   ├── smb_negotiate.py          # NegotiateRequest + NegotiateResponse
│   ├── smb_session_setup.py      # SessionSetupRequest + SessionSetupResponse
│   └── smb_packet_parser.py      # Entry point: bytes → ISmbPacket
├── rpc/
│   ├── rpc_bind.py               # DCE/RPC Bind PDU
│   ├── rpc_channel.py            # Binds a named pipe over ISmbTransport
│   └── stubs/
│       ├── srvsvc.py             # NetShareEnumAll, NetSessionEnum
│       ├── samr.py               # EnumerateUsersInDomain
│       ├── lsarpc.py             # LsarQueryInformationPolicy
│       ├── wkssvc.py             # NetWkstaGetInfo
│       ├── svcctl.py             # EnumServicesStatusExW
│       └── winreg.py             # RegConnectRegistry
└── transport/
    └── tcp_smb_transport.py
```

### Constants file

```python
# smbrecon/protocol/smb/smb_constants.py

# MS-SMB2 §2.2.3  NEGOTIATE Response SecurityMode
NEGOTIATE_SIGNING_ENABLED  = 0x0001
NEGOTIATE_SIGNING_REQUIRED = 0x0002

# MS-SMB2 §2.2.3  GlobalCapabilities
CAP_DFS           = 0x00000001
CAP_LEASING       = 0x00000002
CAP_LARGE_MTU     = 0x00000004
CAP_MULTI_CHAN    = 0x00000008
CAP_PERSISTENT    = 0x00000010
CAP_DIR_LEASING   = 0x00000020
CAP_ENCRYPTION    = 0x00000040
CAP_NOTIFICATIONS = 0x00000080
CAP_COMPRESSION   = 0x00000100   # SMBv3.1.1

# MS-SMB2 §2.2.4  Dialects
DIALECT_SMB1   = 0x0002
DIALECT_SMB202 = 0x0202
DIALECT_SMB210 = 0x0210
DIALECT_SMB300 = 0x0300
DIALECT_SMB302 = 0x0302
DIALECT_SMB311 = 0x0311
```

### Negotiate packet sketch

```python
# smbrecon/protocol/smb/smb_negotiate.py
import struct
from dataclasses import dataclass
from .smb_constants import *

_SMB2_HEADER = b'\xfeSMB'   # SMB2+ magic
_SMB1_HEADER = b'\xffSMB'   # SMBv1 magic

@dataclass(frozen=True)
class NegotiateRequest:
    dialects: tuple[int, ...]   # e.g. (DIALECT_SMB311, DIALECT_SMB302, ...)

    def to_bytes(self) -> bytes:
        # Build SMB2 NEGOTIATE request — MS-SMB2 §2.2.3
        dialect_bytes = struct.pack(f'<{len(self.dialects)}H', *self.dialects)
        # ... header + struct_size + dialect_count + ...
        return b''  # full implementation in Step 11

@dataclass(frozen=True)
class NegotiateResponse:
    dialect_revision: int
    security_mode:    int
    capabilities:     int
    server_guid:      bytes

    @classmethod
    def parse(cls, data: bytes) -> NegotiateResponse:
        # Parse SMB2 NEGOTIATE response — MS-SMB2 §2.2.4
        # Handle both SMBv1 and SMBv2+ response shapes
        ...
        return cls(dialect_revision=0, security_mode=0,
                   capabilities=0, server_guid=b'')
```

### Tasks

- [ ] Create `smb_constants.py` covering all flags used by the 77 tools
- [ ] Create `NegotiateRequest` with dialect list builder (all dialects, or filtered)
- [ ] Create `NegotiateResponse` parser covering SMBv1/v2/v3 response shapes
- [ ] Create `RpcChannel` that binds a named pipe (`\\pipe\\svcctl` etc.) over `ISmbTransport`
- [ ] Add `protocol` unit tests: `to_bytes()` → `parse()` round-trip on captured packet fixtures
- [ ] Create stub files for all 6 RPC stubs (empty but importable)

---

## Step 6 — Module Registration System

**Goal:** Each module package exposes its tools through a `get_tools()` function; the CLI discovers all modules at startup via package scanning, with **zero changes** to `cli/` when a new module is added.

### Per-module registration file

```python
# smbrecon/modules/common/module.py
from smbrecon.core.abstractions import ISmbTool

FLAG         = "--common"
CATEGORY     = "common"
DISPLAY_NAME = "Common SMB"

def get_tools() -> list[ISmbTool]:
    # Imported lazily to avoid circular imports at scan time
    from .smb_dialect_prober      import SmbDialectProber
    from .smb_signing_auditor     import SmbSigningAuditor
    from .smb_capability_scanner  import SmbCapabilityScanner
    from .smbv1_detector          import Smbv1Detector
    from .smb_encryption_checker  import SmbEncryptionChecker
    from .smb_port_mapper         import SmbPortMapper
    from .smb_dialect_downgrade   import SmbDialectDowngradeChecker
    from .guest_access_prober     import GuestAccessProber
    from .smb_server_string       import SmbServerStringExtractor
    from .smb_timeout_prober      import SmbTimeoutBehaviorProber
    return [
        SmbDialectProber(), SmbSigningAuditor(), SmbCapabilityScanner(),
        Smbv1Detector(), SmbEncryptionChecker(), SmbPortMapper(),
        SmbDialectDowngradeChecker(), GuestAccessProber(),
        SmbServerStringExtractor(), SmbTimeoutBehaviorProber(),
    ]
```

### CLI-side auto-discovery

```python
# smbrecon/cli/module_loader.py
from __future__ import annotations
import importlib
import pkgutil
import smbrecon.modules as _modules_pkg
from smbrecon.core.abstractions import ISmbTool

def discover_all() -> list[ISmbTool]:
    tools: list[ISmbTool] = []
    for module_info in pkgutil.iter_modules(_modules_pkg.__path__):
        mod = importlib.import_module(
            f"smbrecon.modules.{module_info.name}.module"
        )
        if hasattr(mod, "get_tools"):
            tools.extend(mod.get_tools())
    return tools
```

### Registration contract

Every module **must** declare three public module-level constants:

| Constant | Example | Used by |
|---|---|---|
| `FLAG` | `"--common"` | CLI flag parser |
| `CATEGORY` | `"common"` | `ISmbTool.category` cross-validation |
| `DISPLAY_NAME` | `"Common SMB"` | `--list` output |

A CI test asserts that every discovered `ISmbTool.category` matches the `CATEGORY` constant of its hosting module.

### Tasks

- [ ] Create `module.py` for all 11 module packages (stub `get_tools()` returning empty lists initially)
- [ ] Create `smbrecon/modules/__init__.py` with `__path__` set correctly for `pkgutil`
- [ ] Write `discover_all()` as above
- [ ] Write a CI test: `discover_all()` returns exactly 77 tools; no duplicate `name`
- [ ] Document the three-constant contract in `docs/CONTRIBUTING.md`

---

## Step 7 — CLI Skeleton & Flag Map

**Goal:** Build a complete, production-quality CLI surface with all flags wired to module selectors before a single real tool is implemented.

### Flag map

```python
# smbrecon/cli/flag_map.py
FLAG_TO_CATEGORY: dict[str, str] = {
    "--common":      "common",
    "--system-info": "system-info",
    "--domain":      "domain",
    "--dc":          "dc",
    "--enum":        "enumeration",
    "--objects":     "objects",
    "--vulns":       "vulnerabilities",
    "--shares":      "share-recon",
    "--auth":        "auth-relay",
    "--postex":      "post-ex",
    "--analysis":    "analysis",
}
```

### CLI options dataclass

```python
# smbrecon/cli/cli_options.py
from __future__ import annotations
from dataclasses import dataclass, field
from enum import Enum, auto

class OutputFormat(Enum):
    CONSOLE  = auto()
    JSON     = auto()
    MARKDOWN = auto()

@dataclass
class CliOptions:
    # Target
    target:   str  | None = None
    port:     int         = 445
    username: str  | None = None
    password: str  | None = None
    hash:     str  | None = None      # NTLM hash for PtH
    domain:   str  | None = None
    timeout:  int         = 5

    # Module selectors
    all:        bool        = False
    categories: list[str]   = field(default_factory=list)
    only:       list[str]   = field(default_factory=list)  # tool names

    # Safety
    safe:            bool = True    # default
    allow_intrusive: bool = False

    # Output
    format:   OutputFormat = OutputFormat.CONSOLE
    out_file: str | None   = None
    threads:  int          = 1

    # Meta
    list_tools: bool = False
    help:       bool = False
```

### Usage surface

```
smbrecon --list
smbrecon --common --target 10.0.0.5
smbrecon --common --system-info --target 10.0.0.5
smbrecon --only SmbDialectProber,Smbv1Detector --target 10.0.0.5
smbrecon --all --target 10.0.0.5 --safe
smbrecon --all --target 10.0.0.5 --allow-intrusive
smbrecon --vulns --target 10.0.0.5 --json --out report.json
smbrecon --common --target 10.0.0.5 --threads 10
smbrecon --postex --target 10.0.0.5 --hash aad3b435b51404ee:8846f7eaee8fb117
```

### Implementation note

Use Python's built-in `argparse` (zero extra dependencies) with a custom `Action` for multi-flag category accumulation. `click` or `typer` are acceptable alternatives if the team prefers them — add to `[project.dependencies]` in that case.

### Tasks

- [ ] Implement `CliParser` using `argparse` (or `click`/`typer`)
- [ ] Implement `flag_map.py` as above
- [ ] Wire `--list` to print a formatted table from `discover_all()`
- [ ] Implement `--only` filter (comma-separated `ISmbTool.name` values)
- [ ] Validate that `--allow-intrusive` is present when `INTRUSIVE` tools are selected
- [ ] Add `--help` with full usage text and examples
- [ ] Add integration test: `--list` produces exactly 77 lines (once all stubs exist)

---

## Step 8 — Safety Pipeline

**Goal:** Make it architecturally impossible to run an `INTRUSIVE` tool without an explicit flag, and impossible to run `ACTIVE` tools in the default `--safe` mode — enforced at the pipeline level, not inside each tool.

### Safety filter

```python
# smbrecon/cli/pipeline/safety_filter.py
from __future__ import annotations
from smbrecon.core.abstractions import ISmbTool
from smbrecon.core.models import ImpactLevel
from ..cli_options import CliOptions

def apply(
    tools: list[ISmbTool],
    opts: CliOptions,
) -> tuple[list[ISmbTool], list[tuple[ISmbTool, str]]]:
    """Return (allowed, blocked) tool lists."""
    allowed: list[ISmbTool] = []
    blocked: list[tuple[ISmbTool, str]] = []

    for tool in tools:
        reason = _evaluate(tool, opts)
        if reason is None:
            allowed.append(tool)
        else:
            blocked.append((tool, reason))

    return allowed, blocked


def _evaluate(tool: ISmbTool, opts: CliOptions) -> str | None:
    match tool.impact:
        case ImpactLevel.INTRUSIVE if not opts.allow_intrusive:
            return "Intrusive — pass --allow-intrusive to enable"
        case ImpactLevel.ACTIVE if opts.safe:
            return "Active — pass --no-safe to enable"
        case _:
            return None
```

### Impact matrix

| `ImpactLevel` | Default (`--safe`) | `--no-safe` | `--allow-intrusive` |
|---|---|---|---|
| `PASSIVE` | ✅ runs | ✅ runs | ✅ runs |
| `ACTIVE` | ❌ blocked | ✅ runs | ✅ runs |
| `INTRUSIVE` | ❌ blocked | ❌ blocked | ✅ runs |

### Warning output (console)

```
[SAFETY] 2 tool(s) blocked by current safety settings:
  · SmbLorisDetector   [INTRUSIVE]  — pass --allow-intrusive to enable
  · ShareWriteProber   [ACTIVE]     — pass --no-safe to enable
```

### Tasks

- [ ] Implement `safety_filter.apply()` as above
- [ ] Wire `SafetyFilter` into the main execution pipeline (before `asyncio.gather` dispatch)
- [ ] Write unit tests: `INTRUSIVE` tool + `--safe` → blocked; `PASSIVE` + any flag → runs
- [ ] Write unit test: `ACTIVE` tool + `--allow-intrusive` but not `--no-safe` → still blocked
- [ ] Document the impact classification in `docs/TOOLS.md` for each of the 77 tools

---

## Step 9 — Output & Reporting Contracts

**Goal:** Define the output contract so that every reporter (console, JSON, Markdown) receives the same `ToolResult` stream and the module layer never formats its own output.

### Reporter Protocol

```python
# smbrecon/output/reporter.py
from __future__ import annotations
from typing import Protocol
from smbrecon.core.models import SmbTarget, ToolResult
from smbrecon.core.abstractions import ISmbTool
from datetime import timedelta

class IReporter(Protocol):

    def begin(self, target: SmbTarget, planned: list[ISmbTool]) -> None:
        """Called once before any tools run."""
        ...

    def report(self, result: ToolResult) -> None:
        """Called immediately when a tool finishes."""
        ...

    def end(self, total_elapsed: timedelta) -> None:
        """Called once after all tools complete."""
        ...
```

### Console reporter output

```
[✔] SmbDialectProber          (42ms)
    smb.dialect.max      : SMBv3.1.1
    smb.dialect.list     : SMBv1, SMBv2.1, SMBv3.0, SMBv3.1.1

[✔] SmbSigningAuditor          (18ms)
    smb.signing.required : false    [HIGH]
    smb.signing.enabled  : true

[✗] Smbv1Detector              (7ms)  ERROR: Connection refused
```

### Reporter factory

```python
# smbrecon/output/__init__.py
import sys
from typing import TextIO
from .console_reporter  import ConsoleReporter
from .json_reporter     import JsonReporter
from .markdown_reporter import MarkdownReporter
from smbrecon.cli.cli_options import OutputFormat

def create_reporter(
    fmt: OutputFormat,
    writer: TextIO | None = None,
) -> IReporter:
    out = writer or sys.stdout
    match fmt:
        case OutputFormat.JSON:     return JsonReporter(out)
        case OutputFormat.MARKDOWN: return MarkdownReporter(out)
        case _:                     return ConsoleReporter()
```

### Reporter notes

- `ConsoleReporter` uses ANSI colour codes: green `✔` / red `✗`. Detect `sys.stdout.isatty()` and disable colour when piping.
- `JsonReporter` emits a single JSON array of `ToolResult` objects using `dataclasses.asdict` + a custom encoder for `Enum` and `timedelta`.
- `MarkdownReporter` produces GitHub-flavored Markdown: each tool result is a `## Heading` with a findings table; severity badges are emoji (`🔴 CRITICAL`, `🟠 HIGH`, `🟡 MEDIUM`, `🔵 LOW`, `⚪ INFO`).

### Tasks

- [ ] Define `IReporter` Protocol in `smbrecon/output/reporter.py`
- [ ] Implement `ConsoleReporter` with ANSI colour output
- [ ] Implement `JsonReporter` using `json.dumps` + custom encoder
- [ ] Implement `MarkdownReporter` with severity-badge mapping
- [ ] Implement `create_reporter()` factory
- [ ] Write snapshot tests: given a fixed `ToolResult`, assert exact console/JSON/Markdown output

---

## Step 10 — Test Harness & Golden Files

**Goal:** Establish the testing infrastructure so every tool can be developed and validated without a live SMB server, using captured packet fixtures.

### Test structure

```
tests/
├── core/
│   ├── models/
│   │   ├── test_smb_target.py
│   │   ├── test_tool_result.py
│   │   └── test_finding.py
│   └── abstractions/
│       └── test_smb_tool_base.py     # exception → ToolResult(success=False)
├── protocol/
│   ├── fixtures/
│   │   ├── negotiate_smb311_response.bin
│   │   ├── negotiate_smb1_response.bin
│   │   └── session_setup_ntlmssp.bin
│   ├── test_smb_negotiate_request.py
│   └── test_smb_negotiate_response.py
└── modules/
    └── common/
        ├── test_smb_dialect_prober.py   # uses MockSmbTransport + fixture
        └── test_smbv1_detector.py
```

### MockSmbTransport

```python
# tests/core/mocks/mock_smb_transport.py
from __future__ import annotations
from collections import deque
from smbrecon.core.models import SmbTarget

class MockSmbTransport:
    """Injects pre-loaded byte[] responses; no real network traffic."""

    def __init__(self, *responses: bytes) -> None:
        self._responses: deque[bytes] = deque(responses)
        self._connected = False

    @property
    def is_connected(self) -> bool:
        return self._connected

    async def connect(self, target: SmbTarget, /) -> None:
        self._connected = True

    async def send_receive(self, request: bytes, /) -> bytes:
        return self._responses.popleft()

    async def disconnect(self) -> None:
        self._connected = False

    async def __aenter__(self) -> MockSmbTransport:
        return self

    async def __aexit__(self, *_: object) -> None:
        await self.disconnect()
```

### Golden-file test pattern

```python
# tests/modules/common/test_smb_dialect_prober.py
import pytest
from pathlib import Path
from smbrecon.core.models import SmbTarget, SmbDialect
from smbrecon.modules.common.smb_dialect_prober import SmbDialectProber
from tests.core.mocks.mock_smb_transport import MockSmbTransport

FIXTURES = Path(__file__).parent.parent.parent / "protocol" / "fixtures"

@pytest.mark.asyncio
async def test_detects_smb311_from_captured_frame() -> None:
    response = (FIXTURES / "negotiate_smb311_response.bin").read_bytes()
    transport = MockSmbTransport(response)

    def factory(_target: SmbTarget) -> MockSmbTransport:
        return transport

    tool = SmbDialectProber(transport_factory=factory)
    result = await tool.run(SmbTarget("127.0.0.1"))

    assert result.success
    dialect = next(f for f in result.findings if f.key == "smb.dialect.max")
    assert dialect.value == SmbDialect.SMB311
```

### Custom flake8 plugin (import-rule enforcement)

Write a `flake8` plugin (`smbrecon_flake8.py`) that raises `SMB001` when a `modules.*` package imports from another `modules.*` package, and `SMB002` when `core` imports from `socket` or `asyncio`.

### CI pipeline (`ci.yml` in GitHub Actions)

```yaml
jobs:
  test:
    steps:
      - run: pip install -e ".[dev,test]"
      - run: ruff check .
      - run: mypy smbrecon
      - run: pytest --cov=smbrecon --cov-fail-under=80
      - run: python scripts/validate_tools.py
```

### Tasks

- [ ] Add `MockSmbTransport` to `tests/core/mocks/`
- [ ] Capture and commit at least 5 real SMB negotiate frames as `.bin` fixtures
- [ ] Write round-trip tests for `NegotiateRequest` ↔ `NegotiateResponse`
- [ ] Write the first golden-file test for `SmbDialectProber`
- [ ] Set up GitHub Actions CI pipeline with 80% coverage gate
- [ ] Write `scripts/validate_tools.py` that diffs `TOOLS.md` against `discover_all()` output
- [ ] Write the `flake8` plugin for import-rule enforcement
- [ ] Add `pytest-asyncio` and `pytest-cov` to `[project.optional-dependencies.test]`

---

## Foundation Checkpoint

Before proceeding to Steps 11–20 (implementing the 10 Common SMB tools), verify every item in this checklist:

- [ ] `ruff check .` and `mypy smbrecon` pass with zero errors
- [ ] `pytest` passes — all Core + Protocol + stub module tests green
- [ ] `--list` outputs exactly the expected module/tool structure
- [ ] `--safe` blocks all `ACTIVE` and `INTRUSIVE` tools
- [ ] `--allow-intrusive` unblocks `INTRUSIVE` tools but `ACTIVE` still requires `--no-safe`
- [ ] `--json` produces valid JSON; `--markdown` produces valid GitHub-flavored Markdown
- [ ] `MockSmbTransport` + golden-file pattern established in at least one test
- [ ] CI pipeline green on `main`
- [ ] `docs/ARCHITECTURE.md` and `docs/TOOLS.md` stubs committed
- [ ] Zero cross-module imports (`SMB001` flake8 plugin clean)

---

*Steps 11–20 (Common SMB Module — all 10 tools fully implemented) will be covered in the next batch.*
