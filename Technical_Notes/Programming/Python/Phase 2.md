# OSED Prep — Phase 2: Network Programming

## 1. The `socket` Module Basics

- Creating a TCP socket: `socket.socket(socket.AF_INET, socket.SOCK_STREAM)`
- `connect()`, and why it takes a `(host, port)` tuple — tying back to Phase 0's `tuple` coverage
- Sending data: `send()` vs. `sendall()` (why `sendall()` is generally safer — it guarantees the full buffer is transmitted)
- Receiving data: `recv(bufsize)` and choosing a reasonable buffer size
- Closing a connection cleanly with `close()`

## 2. Reliability & Error Handling

- Setting a timeout with `settimeout()` and catching `socket.timeout`
- Catching `ConnectionRefusedError` and `ConnectionResetError` — a crashing target disconnects abruptly, and the script must not crash alongside it
- Wrapping connect/send/recv in `try`/`except` so a single failed attempt doesn't kill an entire fuzzing or exploit run
- Retrying a connection after a target restarts (relevant once fuzzing starts crashing services)

## 3. Reading Full Responses

- Why a single `recv()` call does not guarantee the full response (TCP is a stream, not a message protocol)
- Looping `recv()` until a known delimiter, an expected length, or a timeout is reached
- Accumulating partial reads into one buffer (`response += chunk`)

## 4. UDP Sockets

- `socket.SOCK_DGRAM` for UDP-based targets
- Key differences from TCP: no `connect()`-based handshake required, no delivery guarantee, message boundaries are preserved per `sendto()`/`recvfrom()` call
- When a target uses UDP instead of TCP (common for some network services and custom protocols)

## 5. Talking to Real Protocols by Hand

- Constructing a raw HTTP request manually (`b"GET / HTTP/1.1\r\nHost: target\r\n\r\n"`) to understand what a library like `requests` normally hides
- Reading a banner from a service on connect (common with FTP, SMTP, custom TCP services) — the first `recv()` often happens before you send anything
- Why understanding the raw protocol matters for OSED: exploit targets are rarely using a convenient Python library on the other end

## 6. Combining Phase 1 + Phase 2

- Sending a `bytes` payload built with `struct.pack` (Phase 1) directly over a live socket connection
- Structuring an exploit script's shape: build payload → connect → send → (optionally) receive confirmation or crash evidence → close
- Wrapping the connect/send sequence in a function (e.g., `send_payload(target_ip, port, payload)`) so it can be reused by the fuzzer in Phase 3 without rewriting socket logic

