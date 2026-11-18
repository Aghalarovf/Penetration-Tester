# OSED Prep — Phase 3: Fuzzing & Automation Basics

**Target duration:** ~1 week at 2–3 hours/day.

## 1. Incremental Fuzzing Loops

- The core pattern: a loop that sends a growing buffer size each iteration (e.g., 100, 200, 300… bytes) against a target over a socket
- Reusing the Phase 2 `send_payload()` function as the transport layer, so the fuzzer doesn't duplicate socket logic
- Detecting a crash indirectly: a failed connection, a timeout, or no response where one was previously expected
- Deciding step size and starting point based on the target (a web-facing buffer vs. a tiny protocol field needs different ranges)

## 2. Logging & Result Tracking

- Writing each attempt's buffer size and outcome (sent / no response / connection dropped) to a log file as the fuzzer runs
- Why logging matters here specifically: a crash found at 2:00 AM during an unattended run is useless if there's no record of which input caused it
- Structuring log entries so the exact buffer size and timestamp of the first failure are easy to find later

## 3. Finding the Exact Offset: Cyclic Patterns

- The problem: once a crash is found, you know a buffer of roughly size N causes it, but not which byte lands in the instruction pointer
- Building a non-repeating cyclic pattern (a unique sequence long enough that no 4-byte or 8-byte window repeats) — the same principle behind tools like `pattern_create`/`pattern_offset`
- Writing your own pattern generator (e.g., cycling through letter/number combinations) instead of relying on a prebuilt tool, to build real understanding of why it works
- Matching the value found in the crashed instruction pointer back against the generated pattern to compute the exact offset

## 4. Bad Character Identification

- Why certain byte values corrupt a payload in transit or in memory (e.g., `\x00` often terminates a string early, `\x0a`/`\x0d` can be treated as line endings by a protocol)
- Building a test buffer containing every byte value from `\x00` to `\xff`
- Sending that buffer to the target and comparing what the debugger/memory shows against what was sent, to identify which bytes were altered, dropped, or stopped the transfer
- Iteratively removing a confirmed bad byte and re-testing, until only clean bytes remain — producing the exclusion list later exploit payloads must avoid

## 5. Making It Reusable: `argparse` and `subprocess`

- Using `argparse` to accept target IP, port, buffer range, and log file path as command-line arguments, instead of hardcoding them per target
- Turning a one-off fuzzing script into a tool that can be pointed at a new target without editing the code
- `subprocess` basics for the case where the target is a local application rather than a networked service (launching the process, monitoring whether it's still alive)

## 6. Structuring the Fuzzer as Functions

- Splitting the fuzzer into clear functions: `build_buffer(size)`, `send_payload(ip, port, payload)` (from Phase 2), `log_attempt(size, result)`
- Why this structure matters: the same functions get reused, almost unchanged, when the offset-finding and bad-character scripts are written next
