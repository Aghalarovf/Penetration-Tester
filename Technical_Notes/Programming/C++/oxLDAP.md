# 🚀 100-Step Roadmap: Mastering C++ & Building Your Own LDAP Library from Scratch

> **Goal:** Learn C++ from zero to expert level, then build a fully self-contained LDAP library (Oxsium Framework) without any third-party dependencies — down to the raw bits.

---

## 📌 HOW TO USE THIS ROADMAP

- Every step builds on the previous one. Do **not** skip steps.
- Each step includes: **what to learn**, **what to implement/practice**, and **why it matters for LDAP**.
- By Step 100, you will have: deep C++ mastery + a working, production-quality LDAP library written entirely by you.

---

## 🧱 PHASE 1 — C++ FOUNDATIONS (Steps 1–20)

### Step 1 — The C++ Compilation Pipeline
**Learn:**
- How `.cpp` → preprocessor → compiler → assembler → linker → binary works
- The difference between `g++`, `clang++`, and `MSVC`
- Compilation flags: `-std=c++17`, `-Wall`, `-Wextra`, `-O2`, `-g`
- Object files (`.o`), static libraries (`.a`), shared libraries (`.so` / `.dll`)

**Practice:**
- Write a "Hello, LDAP World!" program and compile it manually step by step using `g++ -E`, `g++ -S`, `g++ -c`, then link
- Inspect the resulting assembly with `objdump -d`

**LDAP relevance:** Your library will be compiled as a static `.a` archive that others link against — understanding this pipeline is non-negotiable.

---

### Step 2 — Types, Memory Layout, and Sizes
**Learn:**
- Primitive types: `char`, `short`, `int`, `long`, `long long`, `float`, `double`, `bool`
- Unsigned variants: `uint8_t`, `uint16_t`, `uint32_t`, `uint64_t` (from `<cstdint>`)
- `sizeof()`, `alignof()`, `offsetof()`
- Stack vs heap memory, memory segments (text, data, bss, stack, heap)
- Endianness: big-endian vs little-endian, how to detect and convert

**Practice:**
- Write a program that prints the size and alignment of every primitive type on your system
- Write `to_big_endian_32(uint32_t val)` and `from_big_endian_32(uint32_t val)` without any library

**LDAP relevance:** LDAP packets are big-endian BER encoded. Every integer you send over the wire must be byte-swapped correctly.

---

### Step 3 — Pointers, References, and Raw Memory
**Learn:**
- Pointer arithmetic, pointer-to-pointer, `void*`
- References vs pointers: when to use each
- `const` correctness: `const int*`, `int* const`, `const int* const`
- `nullptr` vs `NULL` vs `0`
- `reinterpret_cast`, `static_cast`, `const_cast`, `dynamic_cast`
- Memory model: stack allocation vs `new`/`delete`

**Practice:**
- Implement a function that reverses an array of bytes in-place using only pointer arithmetic
- Write a `hexdump(const uint8_t* buf, size_t len)` function — you will use this constantly to debug BER packets

**LDAP relevance:** Raw byte buffers are the heart of your BER encoder/decoder.

---

### Step 4 — Arrays, Strings, and C-Style I/O
**Learn:**
- C-style arrays, array decay to pointer
- C-style strings: `char*`, `strlen`, `strcpy`, `strcmp`, `strncpy` — and why they are dangerous
- `std::string` internals: SSO (Small String Optimization), `c_str()`, `data()`
- `printf` / `snprintf` format specifiers including `%zu`, `%02x`, `%s`

**Practice:**
- Implement your own `my_strlen`, `my_strcpy`, `my_strcmp` from scratch
- Write `bytes_to_hex_string(const uint8_t* buf, size_t len) -> std::string`

**LDAP relevance:** LDAP strings (Distinguished Names, attribute values) must be handled carefully — both as raw bytes and as UTF-8 text.

---

### Step 5 — Control Flow and Error Handling Philosophy
**Learn:**
- `if/else`, `switch`, `for`, `while`, `do-while`, `goto` (when it is acceptable)
- C++ exceptions: `throw`, `try`, `catch`, stack unwinding, `noexcept`
- Return-code error handling: `enum class Error { Ok, Timeout, ... }`
- `std::optional<T>` and `std::variant<T, Error>` for modern error handling
- The tradeoffs between exceptions and error codes in library design

**Practice:**
- Design an `LdapError` enum class with 20+ meaningful error codes
- Write a `Result<T>` template class that wraps either a value or an error — you will use this throughout your library

**LDAP relevance:** Network operations fail. Your library must have a consistent, predictable error model.

---

### Step 6 — Functions: Signatures, Overloading, and Calling Conventions
**Learn:**
- Function signatures, default arguments, function overloading
- Pass by value vs reference vs pointer vs `const` reference
- Inline functions and when the compiler ignores `inline`
- Calling conventions: `cdecl`, `stdcall`, `fastcall` (platform-specific)
- Stack frames: how arguments, return addresses, and local variables are laid out

**Practice:**
- Write a `parse_ldap_dn(const std::string& dn)` function stub that validates DN format
- Use `gdb` or a debugger to inspect stack frames while stepping through function calls

**LDAP relevance:** Good function design is the foundation of a clean public API.

---

### Step 7 — Structs, Classes, and Object-Oriented Basics
**Learn:**
- `struct` vs `class` (only difference: default access)
- Constructors, destructors, copy constructor, copy assignment operator
- The Rule of Three / Rule of Five / Rule of Zero
- Member initialization lists vs assignment in constructor body
- `explicit` keyword to prevent implicit conversions

**Practice:**
- Design an `LdapAttribute` class: holds a name (string) and a list of values (vector of byte arrays)
- Implement all five special member functions manually

**LDAP relevance:** Every piece of LDAP data — attributes, entries, messages — will be a class.

---

### Step 8 — Operator Overloading and User-Defined Types
**Learn:**
- Overloading `==`, `!=`, `<`, `>`, `<<`, `[]`, `()`
- When **not** to overload operators
- Comparison operators and `std::strong_ordering` (C++20 spaceship `<=>`)
- Stream insertion `operator<<` for debugging

**Practice:**
- Add `operator==` and `operator<<` to `LdapAttribute`
- Write an `OctetString` class (a named wrapper around `std::vector<uint8_t>`) with full operator support

**LDAP relevance:** LDAP attribute values are octet strings (arbitrary byte sequences). You need a clean type for this.

---

### Step 9 — Inheritance and Polymorphism
**Learn:**
- Single inheritance, multiple inheritance, virtual inheritance (diamond problem)
- `virtual` functions, `override`, `final`, pure virtual (`= 0`)
- `vtable` internals: how the compiler implements virtual dispatch
- Abstract base classes and interfaces
- Object slicing: why polymorphism requires pointers/references

**Practice:**
- Design a hierarchy: `LdapMessage` (abstract) → `LdapBindRequest`, `LdapSearchRequest`, `LdapAddRequest`, etc.
- Implement `virtual std::vector<uint8_t> encode() const = 0;` on the base

**LDAP relevance:** Every LDAP operation is a message type. Polymorphism lets you handle them uniformly.

---

### Step 10 — Templates and Generic Programming Basics
**Learn:**
- Function templates and class templates
- Template instantiation: implicit vs explicit
- Template specialization (full and partial)
- Non-type template parameters
- `typename` vs `class` in template parameters

**Practice:**
- Write a generic `Buffer<N>` class: a fixed-size byte buffer on the stack (no heap allocation) for small LDAP messages
- Implement `encode_integer<T>(T value, uint8_t* out)` as a template function

**LDAP relevance:** Templates let you write zero-overhead abstractions — critical for a high-performance network library.

---

### Step 11 — STL Containers: Vector, Map, List, Array
**Learn:**
- `std::vector<T>`: dynamic array, amortized O(1) push_back, memory layout (pointer + size + capacity)
- `std::array<T, N>`: fixed-size, stack-allocated, zero overhead
- `std::map<K,V>` (red-black tree) vs `std::unordered_map<K,V>` (hash table)
- `std::list<T>`: doubly linked list (cache-unfriendly — use rarely)
- `std::deque<T>`, `std::set<T>`, `std::multimap<K,V>`
- Iterator categories: input, forward, bidirectional, random-access

**Practice:**
- Implement `LdapEntry`: a Distinguished Name + `std::map<std::string, std::vector<OctetString>>` of attributes
- Benchmark `std::map` vs `std::unordered_map` for attribute lookup by name (1000 entries, 10 attributes each)

**LDAP relevance:** LDAP search results are collections of entries with attribute maps.

---

### Step 12 — Memory Management: new/delete, RAII, Smart Pointers
**Learn:**
- `new` / `delete`, `new[]` / `delete[]`, placement `new`
- RAII: Resource Acquisition Is Initialization — the most important C++ idiom
- `std::unique_ptr<T>`: sole ownership, zero overhead
- `std::shared_ptr<T>`: shared ownership, reference counting overhead
- `std::weak_ptr<T>`: break cycles in shared_ptr graphs
- `make_unique`, `make_shared`
- Custom deleters for file descriptors and socket handles

**Practice:**
- Write an `LdapConnection` class that owns a socket file descriptor using `unique_ptr` with a custom deleter (`close(fd)`)
- Deliberately create a memory leak, then find it with `valgrind --leak-check=full`

**LDAP relevance:** Network sockets and buffer allocations must never leak, even on error paths.

---

### Step 13 — Move Semantics and Perfect Forwarding
**Learn:**
- Lvalues vs rvalues vs xvalues (value categories)
- Move constructor and move assignment operator
- `std::move()` — what it really does (just a cast)
- Perfect forwarding with `std::forward<T>`
- Return Value Optimization (RVO) and Named RVO (NRVO)
- `noexcept` and its effect on move semantics

**Practice:**
- Add move constructor and move assignment to `LdapEntry`
- Write a `ByteBuffer` class (owned heap byte array) with move-only semantics — no copying allowed

**LDAP relevance:** Large LDAP responses (search results with thousands of entries) must be moved, not copied.

---

### Step 14 — Lambda Expressions and std::function
**Learn:**
- Lambda syntax: capture by value `[=]`, by reference `[&]`, specific variables `[x, &y]`
- Mutable lambdas, generic lambdas (C++14), lambda templates (C++20)
- `std::function<R(Args...)>`: type-erased callable, its overhead
- Function pointers vs `std::function` vs templates: when to use each

**Practice:**
- Add a callback system to `LdapConnection`: `void on_message(std::function<void(LdapMessage&)> cb)`
- Write a `search()` function that takes a filter predicate lambda: `search("dc=example,dc=com", [](const LdapEntry& e) { return e.has_attr("mail"); })`

**LDAP relevance:** Asynchronous LDAP operations (search results arriving over time) are naturally expressed as callbacks.

---

### Step 15 — Namespaces, Headers, and Project Structure
**Learn:**
- Namespace declaration, `using` declarations vs `using namespace` (avoid in headers!)
- Anonymous namespaces for internal linkage
- Include guards (`#pragma once` vs `#ifndef GUARD_H`)
- Forward declarations and why they speed up compilation
- Separation of interface (`.h`) and implementation (`.cpp`)
- Organizing a multi-file C++ project

**Practice:**
- Restructure all your code so far into:
  ```
  oxsium/
    include/oxsium/      ← public headers
    src/                 ← implementation files
    tests/               ← test programs
    Makefile
  ```
- Write the `Makefile` from scratch (no CMake yet)

**LDAP relevance:** A professional library needs a clean public API surface separated from internal implementation.

---

### Step 16 — Preprocessor, Macros, and Conditional Compilation
**Learn:**
- `#define`, `#ifdef`, `#ifndef`, `#if defined()`, `#elif`, `#endif`
- Platform detection macros: `_WIN32`, `__linux__`, `__APPLE__`, `__x86_64__`
- Macro pitfalls: no type safety, no scope, double-evaluation bugs
- `constexpr` and `if constexpr` as safer alternatives
- `__FILE__`, `__LINE__`, `__func__` for logging/debugging

**Practice:**
- Write platform abstraction macros for socket types (`SOCKET` on Windows, `int` on POSIX)
- Create a `OXSIUM_ASSERT(cond, msg)` macro that prints file/line and aborts in debug mode, does nothing in release

**LDAP relevance:** Your library must compile on Linux, macOS, and Windows — conditional compilation handles platform differences.

---

### Step 17 — File I/O, Streams, and Binary Data
**Learn:**
- `std::fstream`, `std::ifstream`, `std::ofstream`
- Text mode vs binary mode (`std::ios::binary`)
- Reading/writing raw bytes: `read(char*, streamsize)`, `write(const char*, streamsize)`
- `std::stringstream` for in-memory byte manipulation
- `std::istream` / `std::ostream` operator overloading
- Seeking: `seekg`, `seekp`, `tellg`, `tellp`

**Practice:**
- Write `save_ldif(const std::vector<LdapEntry>&, const std::string& filename)` — exports entries in LDIF format
- Write `load_ldif(const std::string& filename) -> std::vector<LdapEntry>` — parses LDIF files

**LDAP relevance:** LDIF (LDAP Data Interchange Format) is the standard way to export/import directory data. You need to parse it.

---

### Step 18 — Exception Safety and Error Propagation Patterns
**Learn:**
- Exception safety guarantees: no-throw, strong, basic, none
- How to write strongly exception-safe code: commit-or-rollback pattern
- `std::exception`, `std::runtime_error`, `std::logic_error`, custom exception classes
- `std::error_code` and `std::error_category` (C++11 system error model)
- When to use exceptions vs error codes in library design

**Practice:**
- Redesign your `Result<T>` class to wrap `std::error_code`
- Write a custom `LdapErrorCategory` that maps your `LdapError` enum to human-readable strings

**LDAP relevance:** A library used by other people must have a well-defined, documented error model that integrates with the C++ ecosystem.

---

### Step 19 — Build Systems: Makefile Deep Dive and Introduction to CMake
**Learn:**
- Makefile: rules, prerequisites, automatic variables (`$@`, `$<`, `$^`), pattern rules, phony targets
- How `make` decides what to rebuild (timestamps + dependency graph)
- CMake basics: `CMakeLists.txt`, `add_library`, `add_executable`, `target_include_directories`, `target_link_libraries`
- CMake modern style: target-based (not global variables)
- `cmake --build`, out-of-source builds

**Practice:**
- Write a complete `CMakeLists.txt` for your Oxsium library that builds a static library and a test executable
- Add an `install` target that copies headers to `/usr/local/include/oxsium/` and the lib to `/usr/local/lib/`

**LDAP relevance:** Users of your library need to build and integrate it easily. A proper CMake setup is the standard.

---

### Step 20 — Debugging, Testing, and Profiling
**Learn:**
- `gdb` basics: breakpoints, watchpoints, stepping, inspecting memory with `x/16xb`
- `valgrind`: `--leak-check=full`, `--track-origins=yes`, `helgrind` for threading
- Address Sanitizer (`-fsanitize=address`), UB Sanitizer (`-fsanitize=undefined`)
- Writing unit tests without a framework: simple `assert`-based test runner
- `gprof` and `perf` basics for profiling

**Practice:**
- Write a `run_tests()` function that runs 20 test cases on everything you've built so far
- Use AddressSanitizer to verify no memory errors in your code
- Profile your `hexdump` function with `perf stat`

**LDAP relevance:** A network library has edge cases everywhere. Testing and sanitizers catch bugs before users do.

---

## 🌐 PHASE 2 — NETWORKING FUNDAMENTALS FROM SCRATCH (Steps 21–40)

### Step 21 — The OSI Model and TCP/IP Stack Internals
**Learn:**
- All 7 OSI layers and what happens at each one
- How an IP packet is structured: version, IHL, TTL, protocol, source/dest IP, checksum
- How a TCP segment is structured: source/dest port, sequence number, ACK number, flags, window size, checksum
- The 3-way handshake (SYN, SYN-ACK, ACK) and 4-way teardown
- What "connection" means at the kernel level

**Practice:**
- Capture LDAP traffic with `tcpdump -i lo -w ldap.pcap port 389` and open it in Wireshark
- Annotate the bytes of one complete LDAP packet by hand: Ethernet header, IP header, TCP header, LDAP payload

**LDAP relevance:** LDAP runs on TCP port 389 (LDAPS on 636). You must understand the transport layer to diagnose connection issues.

---

### Step 22 — POSIX Sockets API: The Complete Foundation
**Learn:**
- `socket()`, `bind()`, `listen()`, `accept()`, `connect()`
- `send()`, `recv()`, `sendto()`, `recvfrom()`
- Socket address structures: `sockaddr`, `sockaddr_in`, `sockaddr_in6`, `sockaddr_storage`
- `htons()`, `htonl()`, `ntohs()`, `ntohl()` — host-to-network byte order
- `inet_pton()`, `inet_ntop()` — IP address string conversion
- Socket options: `SO_REUSEADDR`, `SO_KEEPALIVE`, `TCP_NODELAY`

**Practice:**
- Write a TCP client from scratch that connects to `ldap.example.com:389` and immediately closes
- Write a tiny TCP echo server from scratch (no `select`, just one client at a time)

**LDAP relevance:** Your LDAP library's transport layer is literally this — `connect()`, `send()`, `recv()`, `close()`.

---

### Step 23 — Non-blocking I/O and the select/poll/epoll Model
**Learn:**
- Blocking vs non-blocking sockets: `fcntl(fd, F_SETFL, O_NONBLOCK)`
- `EAGAIN` / `EWOULDBLOCK` — what they mean and how to handle them
- `select()`: `fd_set`, `FD_SET`, `FD_ISSET`, timeout, the 1024 fd limit
- `poll()`: `struct pollfd`, `POLLIN`, `POLLOUT`, `POLLERR`, `POLLHUP`
- `epoll()` (Linux): `epoll_create1`, `epoll_ctl`, `epoll_wait`, edge-triggered vs level-triggered
- The C10K problem and why blocking-per-thread doesn't scale

**Practice:**
- Rewrite your TCP client using non-blocking `connect()` and `poll()` with a configurable timeout
- Write an `EventLoop` class that wraps `epoll` and dispatches read/write callbacks

**LDAP relevance:** LDAP clients may handle many connections at once (connection pooling). Non-blocking I/O is essential for this.

---

### Step 24 — Socket Timeouts, Keepalive, and Reliability
**Learn:**
- `SO_RCVTIMEO` and `SO_SNDTIMEO` socket options (timeout at kernel level)
- `SO_KEEPALIVE` + `TCP_KEEPIDLE`, `TCP_KEEPINTVL`, `TCP_KEEPCNT`
- Detecting half-open connections (the `recv()` returns 0 problem)
- `MSG_WAITALL` flag to receive exactly N bytes
- Partial reads/writes: why `send()` may not send all bytes at once

**Practice:**
- Write `send_all(int fd, const uint8_t* buf, size_t len)` that loops until all bytes are sent
- Write `recv_exactly(int fd, uint8_t* buf, size_t len)` that loops until exactly `len` bytes are received

**LDAP relevance:** LDAP over TCP requires reliable, ordered delivery of complete messages. `recv_exactly` is the foundation of your message framing.

---

### Step 25 — DNS Resolution from Scratch
**Learn:**
- `getaddrinfo()` and `freeaddrinfo()`: the modern POSIX name resolution API
- `struct addrinfo` fields: `ai_family`, `ai_socktype`, `ai_addr`, `ai_next`
- IPv4 vs IPv6: `AF_INET` vs `AF_INET6`, dual-stack sockets
- DNS query packet structure: header, questions, answers, authority, additional
- DNS record types: A, AAAA, CNAME, SRV (important for LDAP service discovery), TXT

**Practice:**
- Write `resolve_host(const std::string& hostname, uint16_t port) -> std::vector<sockaddr_storage>` that returns all addresses
- Manually construct a DNS query packet for `_ldap._tcp.example.com SRV` and send it to `8.8.8.8:53` via UDP

**LDAP relevance:** SRV records are how enterprise LDAP clients discover domain controllers. Your library should support this.

---

### Step 26 — TLS/SSL Concepts (Before Implementation)
**Learn:**
- Symmetric vs asymmetric cryptography: the conceptual difference
- What TLS does: handshake, key exchange, cipher negotiation, certificate verification, bulk encryption
- X.509 certificates: structure, fields (CN, SAN, issuer, validity, public key, signature)
- Certificate chains, root CAs, intermediate CAs
- TLS record protocol structure: content type, version, length, data
- TLS 1.2 vs TLS 1.3 handshake flow differences

**Practice:**
- Use `openssl s_client -connect ldap.example.com:636 -showcerts` and parse the output by hand
- Draw the complete TLS 1.3 handshake on paper with every message type labeled

**LDAP relevance:** LDAPS (port 636) and StartTLS (port 389 + upgrade) both require TLS. You must understand it to implement your own or integrate correctly.

---

### Step 27 — Writing a Raw TLS Client Using OS APIs
**Learn:**
- On Linux: `/dev/urandom` for random bytes, how to read it
- How to parse DER-encoded X.509 certificates byte by byte
- The structure of a `ClientHello` TLS record
- Using the kernel's `TCP_MD5SIG` or understanding where TLS sits relative to the kernel
- Why implementing TLS from scratch is a 6-month project (and why most people use a library)
- The decision: implement TLS yourself OR write a thin C++ wrapper around OS TLS APIs (`Secure Transport` on macOS, `SChannel` on Windows, or use raw `openssl` C API at the syscall level without C++ wrappers)

**Practice:**
- Implement `get_random_bytes(uint8_t* buf, size_t len)` using `/dev/urandom`
- Write a DER parser that extracts the Subject CN from a raw certificate file
- Decide your TLS strategy for Oxsium (document this decision)

**LDAP relevance:** LDAP security depends on TLS. Your library needs to handle it, even if you use OS-provided TLS at the lowest level.

---

### Step 28 — Platform Abstraction Layer for Sockets
**Learn:**
- Winsock2 vs POSIX: what is different (`SOCKET` type, `WSAStartup/WSACleanup`, `closesocket` vs `close`, error codes)
- How to write a `Socket` class that hides platform differences
- `#ifdef _WIN32` patterns for cross-platform code
- Handling `EINTR`: `recv()` can be interrupted by signals; retry loop pattern

**Practice:**
- Write `platform/socket.h` and `platform/socket.cpp` that provide:
  - `Socket::connect(host, port, timeout_ms)`
  - `Socket::send(buf, len)`
  - `Socket::recv(buf, len)`
  - `Socket::close()`
  - Works on Linux, macOS, and Windows

**LDAP relevance:** Enterprise software runs on all three platforms. Your library must too.

---

### Step 29 — Implementing a Connection Pool
**Learn:**
- What a connection pool is and why it exists (LDAP connections are expensive to establish)
- Thread-safe data structures: mutex, condition variable, lock guard
- Pool states: idle, in-use, failed, reconnecting
- Health checking: how to test if an idle connection is still alive
- Maximum pool size, minimum pool size, wait timeout

**Practice:**
- Write `LdapConnectionPool`:
  - Fixed maximum connections
  - `acquire(timeout_ms) -> LdapConnection`
  - `release(LdapConnection)`
  - Background thread that pings idle connections every 30 seconds

**LDAP relevance:** Production LDAP clients (authentication systems, directory services) pool connections for performance.

---

### Step 30 — Network Packet Capture and Analysis
**Learn:**
- `libpcap` concepts (even though you won't use it — understand what it does)
- How to send a raw Ethernet frame using `AF_PACKET` sockets (Linux)
- Understanding the loopback interface
- Wireshark LDAP dissector: how it parses LDAP packets
- `strace` for tracing system calls: `strace -e trace=network ./your_ldap_client`

**Practice:**
- Run a test LDAP server (`slapd`) locally
- Connect your raw socket client to it and capture the exchange
- Annotate every byte of the captured LDAP `BindRequest` packet

**LDAP relevance:** You will use `strace` and Wireshark constantly while debugging your library. Get comfortable now.

---

## 🔐 PHASE 3 — CRYPTOGRAPHY FROM SCRATCH (Steps 31–40)

### Step 31 — Bitwise Operations and Binary Math
**Learn:**
- AND `&`, OR `|`, XOR `^`, NOT `~`, left shift `<<`, right shift `>>`
- Arithmetic shift vs logical shift
- Setting, clearing, toggling, testing bits
- Bit fields in structs
- Modular arithmetic: `(a + b) % n`, why it works
- Big number concepts: why 256-bit integers need special handling

**Practice:**
- Implement `rotate_left_32(uint32_t x, int n)` and `rotate_right_32(uint32_t x, int n)` — used in SHA-1
- Write a `BigInteger` class that supports 256-bit numbers using `uint64_t[4]` — with addition, subtraction, comparison

**LDAP relevance:** Hash functions (for password hashing) and random number generation require deep bitwise fluency.

---

### Step 32 — Implementing MD5 from Scratch
**Learn:**
- MD5 algorithm: 4 rounds of 16 operations each, the constants table, the rotation amounts
- Merkle-Damgård construction: why MD5 has length extension vulnerabilities
- Why MD5 is broken for security (collision attacks) but still found in legacy LDAP password schemes (`{MD5}`)
- How to read an RFC specification (RFC 1321 — MD5)

**Practice:**
- Implement MD5 from scratch using only `uint32_t` operations and your bitwise primitives
- Test: `md5("") == "d41d8cd98f00b204e9800998ecf8427e"`
- Test: `md5("The quick brown fox jumps over the lazy dog") == "9e107d9d372bb6826bd81d3542a419d6"`

**LDAP relevance:** LDAP directories still store passwords as `{MD5}base64hash`. Your library needs to verify these.

---

### Step 33 — Implementing SHA-1 from Scratch
**Learn:**
- SHA-1 algorithm: 80 rounds, 4 stages, the initial hash values
- Why SHA-1 is broken (SHAttered collision attack, 2017) but still used in many certificates
- Reading RFC 3174 (SHA-1 specification)
- HMAC: Hash-based Message Authentication Code — wraps any hash function

**Practice:**
- Implement SHA-1 from scratch
- Test: `sha1("abc") == "a9993e364706816aba3e25717850c26c9cd0d89d"`
- Implement `HMAC-SHA1(key, message)` — used in SASL DIGEST-MD5

**LDAP relevance:** LDAP SASL mechanisms use HMAC-SHA1 for authentication challenge responses.

---

### Step 34 — Implementing SHA-256 from Scratch
**Learn:**
- SHA-256: 64 rounds, 8 working variables, the constants (first 32 bits of fractional parts of cube roots of first 64 primes)
- Reading FIPS 180-4 (the SHA standard)
- SHA-224, SHA-384, SHA-512 as variants
- How to implement SHA-256 for `{SHA}` and `{SSHA}` LDAP password schemes

**Practice:**
- Implement SHA-256 from scratch
- Test: `sha256("") == "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"`
- Implement `{SSHA}` password verification: `SHA1(password + salt)` where salt is appended to the hash

**LDAP relevance:** `{SSHA}` (Salted SHA-1) is the most common LDAP password scheme in OpenLDAP.

---

### Step 35 — Base64 Encoding/Decoding from Scratch
**Learn:**
- Base64 alphabet: A-Z, a-z, 0-9, +, / (and URL-safe variant: +→-, /→_)
- Encoding: every 3 bytes → 4 characters, padding with `=`
- Decoding: every 4 characters → 3 bytes
- Base64 in LDAP: attribute values containing binary data are Base64-encoded in LDIF
- Base64 in SASL: authentication tokens are Base64-encoded over the wire

**Practice:**
- Implement `base64_encode(const uint8_t* data, size_t len) -> std::string`
- Implement `base64_decode(const std::string& input) -> std::vector<uint8_t>`
- Test with the standard test vectors from RFC 4648

**LDAP relevance:** You will use Base64 constantly — LDIF files, SASL auth, and certificate encoding all use it.

---

### Step 36 — Random Number Generation
**Learn:**
- Why `rand()` is not cryptographically secure
- `/dev/urandom` vs `/dev/random`: the difference and why `/dev/urandom` is fine
- `getrandom()` syscall (Linux 3.17+)
- `CryptGenRandom` on Windows
- A simple CSPRNG (Cryptographically Secure PRNG): ChaCha20 or AES-CTR mode

**Practice:**
- Write `CryptoRandom` class with `fill(uint8_t* buf, size_t len)` using `/dev/urandom`
- Generate 16 random bytes and format as a UUID (version 4)
- Implement a simple linear congruential generator (bad PRNG) so you understand what "not secure" means

**LDAP relevance:** SASL DIGEST-MD5 requires a server nonce (a random number). Your SASL implementation needs secure random bytes.

---

### Step 37 — Implementing RC4 (ARCFOUR) from Scratch
**Learn:**
- RC4 key scheduling algorithm (KSA) — the 256-byte state array initialization
- RC4 pseudo-random generation algorithm (PRGA)
- Why RC4 is broken and deprecated
- Where RC4 appears in legacy LDAP environments (SASL DIGEST-MD5 uses RC4 as an option)
- Reading RFC 4757 (Kerberos RC4-HMAC)

**Practice:**
- Implement RC4: `rc4_init(key, keylen)`, `rc4_encrypt(input, output, len)`
- Test: `RC4("Key", "Plaintext") == "\xBB\xF3\x16\xE8\xD9\x40\xAF\x0A\xD3"`

**LDAP relevance:** Some legacy SASL mechanisms use RC4. Understanding it helps you recognize (and avoid) insecure configurations.

---

### Step 38 — Implementing AES-128 from Scratch
**Learn:**
- AES key expansion (key schedule): how a 16-byte key becomes 11 round keys
- AES encryption: SubBytes (S-box), ShiftRows, MixColumns, AddRoundKey — 10 rounds
- AES decryption: inverse operations
- Block cipher modes: ECB (bad), CBC, CTR, GCM — how they work
- Why AES-128 in GCM mode is the standard today

**Practice:**
- Implement AES-128 encryption/decryption from scratch using the published S-box table
- Test with NIST test vectors
- Implement AES-128-CBC with PKCS#7 padding

**LDAP relevance:** AES-128-CBC is used in Kerberos (which underlies LDAP SASL/GSSAPI in Active Directory). Understanding it is essential.

---

### Step 39 — Implementing Diffie-Hellman Key Exchange
**Learn:**
- Modular exponentiation: `g^a mod p` — implemented efficiently with binary exponentiation
- The discrete logarithm problem: why DH is secure
- DH parameter selection: prime `p`, generator `g`
- Ephemeral DH (DHE) vs static DH
- Elliptic Curve DH (ECDH) conceptually

**Practice:**
- Implement `mod_exp(BigInteger base, BigInteger exp, BigInteger mod) -> BigInteger`
- Simulate a DH key exchange between "Alice" and "Bob" in a single program
- Verify both arrive at the same shared secret

**LDAP relevance:** TLS uses DHE or ECDHE for perfect forward secrecy. Understanding this helps you configure TLS correctly for LDAPS.

---

### Step 40 — Implementing a Password Hashing Verification Engine
**Learn:**
- LDAP password storage schemes: `{CLEARTEXT}`, `{MD5}`, `{SHA}`, `{SSHA}`, `{CRYPT}`, `{PBKDF2-SHA256}`
- How `userPassword` attribute stores hashed passwords
- `crypt(3)` function and its variants: DES crypt, MD5 crypt `$1$`, SHA-512 crypt `$6$`
- PBKDF2: Password-Based Key Derivation Function 2 (RFC 2898)
- bcrypt and scrypt conceptually

**Practice:**
- Implement `verify_ldap_password(const std::string& stored_value, const std::string& cleartext) -> bool`
- Support: `{CLEARTEXT}`, `{MD5}`, `{SHA}`, `{SSHA}`, `{SHA256}`, `{SSHA256}`
- Write test cases for each scheme

**LDAP relevance:** Any LDAP library doing authentication needs to verify passwords stored in any of these formats.

---

## 📦 PHASE 4 — ASN.1 AND BER ENCODING (Steps 41–55)

### Step 41 — ASN.1 Concepts and Type System
**Learn:**
- ASN.1 (Abstract Syntax Notation One): a language for defining data structures
- Primitive types: INTEGER, OCTET STRING, BOOLEAN, NULL, ENUMERATED, BIT STRING
- Constructed types: SEQUENCE, SEQUENCE OF, SET, SET OF, CHOICE
- OPTIONAL and DEFAULT modifiers
- ASN.1 tagging: IMPLICIT vs EXPLICIT, Universal / Application / Context / Private class
- How LDAP protocol is defined in ASN.1 (RFC 4511 Section 4)

**Practice:**
- Read RFC 4511 Section 4 and draw the ASN.1 structure of `BindRequest` on paper
- Map every LDAP message type to its ASN.1 definition
- Identify every IMPLICIT vs EXPLICIT tag in the LDAP ASN.1 module

**LDAP relevance:** LDAP is defined entirely in ASN.1. You cannot encode or decode LDAP without mastering this.

---

### Step 42 — BER Encoding Rules: Tag Byte
**Learn:**
- BER (Basic Encoding Rules): the binary serialization of ASN.1
- Tag byte structure: bits 8-7 (class), bit 6 (primitive/constructed), bits 5-1 (tag number)
- Tag classes: Universal (00), Application (01), Context-specific (10), Private (11)
- Long-form tags: when tag number ≥ 31, multi-byte encoding
- Common universal tags: BOOLEAN=1, INTEGER=2, BIT STRING=3, OCTET STRING=4, NULL=5, OBJECT IDENTIFIER=6, SEQUENCE=16 (0x30), SET=17 (0x31)

**Practice:**
- Write `encode_tag(uint8_t tag_class, bool constructed, uint32_t tag_number, uint8_t* out) -> size_t`
- Write `decode_tag(const uint8_t* buf, size_t len, uint8_t& tag_class, bool& constructed, uint32_t& tag_number) -> size_t` (returns bytes consumed)
- Test with every LDAP message tag from RFC 4511

**LDAP relevance:** Every byte in an LDAP packet starts with a tag. This is the most fundamental operation.

---

### Step 43 — BER Encoding Rules: Length Encoding
**Learn:**
- Short form: single byte, length 0–127
- Long form: first byte = `0x80 | number_of_length_bytes`, followed by that many bytes
- Indefinite form (only in BER, not DER): `0x80` ... content ... `00 00` — NOT used in LDAP
- Why LDAP uses definite-length BER only
- Length limits: practical maximum LDAP message size (default 4MB in most servers)

**Practice:**
- Write `encode_length(uint32_t length, uint8_t* out) -> size_t`
- Write `decode_length(const uint8_t* buf, size_t buf_len, uint32_t& length) -> size_t`
- Verify: length 127 → `0x7F` (1 byte), length 128 → `0x81 0x80` (2 bytes), length 65535 → `0x82 0xFF 0xFF` (3 bytes)

**LDAP relevance:** Every TLV element's length field uses this encoding. Getting it wrong corrupts every packet.

---

### Step 44 — BER Encoding Rules: Primitive Type Values
**Learn:**
- INTEGER encoding: big-endian two's complement, minimum bytes, leading zero for positive values with high bit set
- BOOLEAN: `0xFF` for TRUE, `0x00` for FALSE (DER rule)
- OCTET STRING: raw bytes, no transformation
- NULL: zero bytes of content
- ENUMERATED: same as INTEGER
- BIT STRING: first byte is "unused bits" count, then the bits

**Practice:**
- Write `encode_integer(int64_t val, uint8_t* out) -> size_t`
- Write `decode_integer(const uint8_t* buf, size_t len) -> int64_t`
- Write `encode_octet_string(const uint8_t* data, size_t len, uint8_t* out) -> size_t`
- Test: INTEGER 0 → `02 01 00`, INTEGER 128 → `02 02 00 80`, INTEGER -1 → `02 01 FF`

**LDAP relevance:** LDAP message IDs, result codes, and scope values are all BER INTEGERs.

---

### Step 45 — BER Encoding Rules: Constructed Types
**Learn:**
- SEQUENCE: tag `0x30`, length, then concatenated TLV elements
- SET: tag `0x31`, same structure but elements in sorted order (DER rule)
- Context-specific constructed: used heavily in LDAP for `[N]` tagged structures
- CHOICE: no wrapper tag — the content is just the chosen alternative
- How nested structures are encoded: outer length includes all inner TLVs

**Practice:**
- Write a `BerWriter` class with a builder pattern:
  - `begin_sequence()` / `end_sequence()`
  - `write_integer(int64_t val)`
  - `write_octet_string(const uint8_t* data, size_t len)`
  - `write_null()`
  - `get_bytes() -> std::vector<uint8_t>`
- Handle the "deferred length" problem: you don't know a SEQUENCE's length until you've encoded all its children

**LDAP relevance:** Every LDAP request and response is a nested SEQUENCE structure.

---

### Step 46 — BER Decoder: Building a TLV Parser
**Learn:**
- Recursive descent parsing of BER/TLV
- Safety: always check buffer bounds before reading
- Handling malformed input gracefully (return error, never crash)
- The "TLV cursor" pattern: a struct holding `{tag, tag_class, constructed, content_ptr, content_len}`
- How to iterate over children of a SEQUENCE without recursion

**Practice:**
- Write a `BerReader` class:
  - `BerElement parse_element(const uint8_t* buf, size_t len)`
  - `std::vector<BerElement> parse_children(const BerElement& seq)`
  - `int64_t read_integer(const BerElement& e)`
  - `std::vector<uint8_t> read_octet_string(const BerElement& e)`
- Fuzz it: feed it 10,000 random byte sequences and verify it never crashes

**LDAP relevance:** Every incoming LDAP response must be parsed by this decoder.

---

### Step 47 — BER Codec: Complete Bidirectional Test
**Learn:**
- Round-trip testing: encode then decode must produce identical data
- Property-based testing: generate random valid ASN.1 values, encode, decode, verify
- Comparing your encoder output against `openssl asn1parse` or `dumpasn1`
- Performance: how many BER encode/decode operations per second can you do?

**Practice:**
- Write 50 round-trip tests covering all types and edge cases:
  - INTEGER: 0, 1, -1, 127, 128, -128, -129, INT32_MAX, INT32_MIN, INT64_MAX
  - OCTET STRING: empty, 1 byte, 127 bytes, 128 bytes, 65535 bytes
  - Nested SEQUENCE 10 levels deep
  - BOOLEAN: true and false
- Benchmark: encode + decode 1 million simple SEQUENCE elements, measure throughput

**LDAP relevance:** A buggy BER codec will produce invalid packets that servers silently reject or crash on. 100% test coverage here is mandatory.

---

### Step 48 — OBJECT IDENTIFIER (OID) Encoding
**Learn:**
- OID structure: a sequence of non-negative integers separated by dots (e.g., `1.3.6.1.4.1.1466`)
- BER OID encoding: first two components merged as `40*X + Y`, then each subsequent component base-128 encoded
- Base-128 encoding: `0x80 | bits[13:7]`, `0x80 | bits[6:0]` ... `bits[6:0]` (last byte has high bit clear)
- Important LDAP OIDs: `1.3.6.1.4.1.1466.115.121.1.*` (LDAP syntax OIDs), `2.5.*` (X.500 OIDs)

**Practice:**
- Write `encode_oid(const std::string& oid_str, uint8_t* out) -> size_t` (e.g., "1.3.6.1.4.1.1466" → bytes)
- Write `decode_oid(const uint8_t* buf, size_t len) -> std::string`
- Test: OID `1.2.840.113549` (RSA) encodes to `2A 86 48 86 F7 0D`

**LDAP relevance:** LDAP extensions, SASL mechanisms, and schema elements are all identified by OIDs.

---

### Step 49 — Implementing LDAPMessage Envelope
**Learn:**
- The `LDAPMessage` envelope (RFC 4511 Section 4.1.1):
  ```
  LDAPMessage ::= SEQUENCE {
    messageID  MessageID,
    protocolOp CHOICE { ... },
    controls   [0] Controls OPTIONAL
  }
  ```
- `MessageID`: 1 to 2,147,483,647 (must be unique per connection)
- Managing message IDs: atomic counter per connection
- The `controls` field: extended request/response metadata

**Practice:**
- Write `LdapMessageEncoder` that takes a messageID + operation bytes and wraps them in the `LDAPMessage` SEQUENCE
- Write `LdapMessageDecoder` that parses the envelope and returns messageID + raw operation bytes

**LDAP relevance:** Every LDAP packet, request or response, is wrapped in this envelope.

---

### Step 50 — Implementing BindRequest / BindResponse
**Learn:**
- `BindRequest` (Application tag 0):
  ```
  BindRequest ::= [APPLICATION 0] SEQUENCE {
    version    INTEGER (1..127),
    name       LDAPDN,
    authentication AuthenticationChoice
  }
  AuthenticationChoice ::= CHOICE {
    simple  [0] OCTET STRING,
    sasl    [3] SaslCredentials
  }
  ```
- Simple bind: send DN + cleartext password (must use TLS!)
- `BindResponse` (Application tag 1): contains `LDAPResult` + optional `serverSaslCreds`
- `LDAPResult`: `resultCode`, `matchedDN`, `diagnosticMessage`
- Result codes: 0 (success), 49 (invalidCredentials), 32 (noSuchObject)

**Practice:**
- Write `encode_bind_request(int msgid, const std::string& dn, const std::string& password) -> std::vector<uint8_t>`
- Write `decode_bind_response(const std::vector<uint8_t>& data) -> BindResponse`
- Test against a real OpenLDAP server: perform a simple bind and verify success

**LDAP relevance:** This is the first real LDAP operation. Getting it working end-to-end is a major milestone.

---

## 📋 PHASE 5 — THE LDAP PROTOCOL (Steps 51–70)

### Step 51 — LDAP URLs and Distinguished Names
**Learn:**
- LDAP URL format: `ldap://host:port/dn?attrs?scope?filter`
- DN (Distinguished Name) structure: `cn=John Doe,ou=users,dc=example,dc=com`
- RDN (Relative Distinguished Name): one component of a DN
- DN canonicalization: case folding, whitespace normalization
- Escaping special characters in DN values: `, + " \ < > ;` and non-ASCII

**Practice:**
- Write `LdapUrl` parser: `LdapUrl::parse("ldap://ldap.example.com:389/dc=example,dc=com?cn,mail?sub?(objectClass=person)")`
- Write `Dn` class: parses, normalizes, compares, and modifies DNs
- Write `Dn::append_rdn(const std::string& rdn) -> Dn`

**LDAP relevance:** DNs appear in every LDAP operation. A correct DN parser is the foundation of your public API.

---

### Step 52 — LDAP Search Filters: Parsing and Encoding
**Learn:**
- Filter types (RFC 4511 Section 4.5.1):
  - `(attr=value)` — equality
  - `(attr~=value)` — approximate match
  - `(attr>=value)` — greater-or-equal
  - `(attr<=value)` — less-or-equal
  - `(attr=*)` — presence
  - `(attr=val*ue*)` — substring
  - `(&(...)(...)...)` — AND
  - `(|(...)(...)...)` — OR
  - `(!(...)` — NOT
  - `(attr:dn:1.2.3.4:=value)` — extensible match
- Filter ASN.1 encoding (Context-specific tags)
- Filter string parsing: recursive descent parser

**Practice:**
- Write a `Filter` AST (Abstract Syntax Tree) with 9 node types
- Write `Filter::parse(const std::string& filter_str) -> Filter`
- Write `Filter::encode() -> std::vector<uint8_t>` (BER encoding)
- Test: `(&(objectClass=person)(|(cn=John*)(mail=*@example.com)))` must round-trip correctly

**LDAP relevance:** Search filters are the most complex part of the LDAP protocol to implement correctly.

---

### Step 53 — SearchRequest and SearchResponse
**Learn:**
- `SearchRequest` fields: baseObject, scope (base/one/sub), derefAliases, sizeLimit, timeLimit, typesOnly, filter, attributes
- Scope values: `baseObject(0)`, `singleLevel(1)`, `wholeSubtree(2)`, `subordinateSubtree(3)`
- `SearchResultEntry`: DN + list of attributes + values
- `SearchResultReference`: referral URLs
- `SearchResultDone`: final response with result code
- The "incomplete results" problem: servers may return partial results

**Practice:**
- Write `encode_search_request(...)` for all parameters
- Write `decode_search_result_entry(...)` → `LdapEntry`
- Write `LdapConnection::search(...)` that reads multiple responses until `SearchResultDone`

**LDAP relevance:** Search is the most frequently used LDAP operation. Most LDAP traffic is searches.

---

### Step 54 — Add, Delete, Modify, ModifyDN Operations
**Learn:**
- `AddRequest`: DN + list of attributes — create a new entry
- `DeleteRequest`: DN — remove an entry
- `ModifyRequest`: DN + list of `ModifyOperation` (`add`, `delete`, `replace`)
- `ModifyDNRequest`: DN + new RDN + delete old RDN + new superior — move/rename an entry
- All four return `LDAPResult` with result code

**Practice:**
- Implement encode + decode for all four request and response types
- Write a test that: adds an entry, modifies an attribute, moves it to a new DN, then deletes it
- Handle all RFC-defined result codes for each operation (at least 20 codes)

**LDAP relevance:** Without write operations, your library is read-only. Most real applications need to create/modify/delete entries.

---

### Step 55 — Compare Operation and Abandon
**Learn:**
- `CompareRequest`: DN + `AttributeValueAssertion` (attribute name + value) — returns `compareTrue(6)` or `compareFalse(5)`
- `AbandonRequest`: messageID — asks server to stop processing an in-flight request
- Why Abandon is "best effort" — the server may have already processed it
- Use case for Compare: verify a password without retrieving `userPassword`

**Practice:**
- Implement Compare request + response
- Implement Abandon request (no response expected)
- Write `verify_password_via_compare(dn, password)` using the Compare operation

**LDAP relevance:** Compare is used for access control checks. Abandon is used for large searches that need to be cancelled.

---

### Step 56 — Extended Operations
**Learn:**
- `ExtendedRequest`: OID + optional value — the LDAP extension mechanism
- `ExtendedResponse`: OID + optional value + `LDAPResult`
- StartTLS: OID `1.3.6.1.4.1.1466.20037` — upgrade plaintext to TLS mid-connection
- Password Modify: OID `1.3.6.1.4.1.4203.1.11.1` (RFC 3062) — change password
- Who Am I: OID `1.3.6.1.4.1.4203.1.11.3` (RFC 4532) — get current identity

**Practice:**
- Implement generic `ExtendedRequest` / `ExtendedResponse` encode/decode
- Implement the StartTLS extended operation (initiate + TLS upgrade sequence)
- Implement the Password Modify extended operation

**LDAP relevance:** StartTLS is critical for secure connections on port 389. Password Modify is used by every web application with a "change password" feature.

---

### Step 57 — LDAP Controls
**Learn:**
- Controls mechanism: optional per-request metadata
- Control structure: OID + criticality + optional value
- Common controls:
  - Paged Results: `1.2.840.113556.1.4.319` — get results in pages
  - Sort: `1.2.840.113556.1.4.473` — request sorted results
  - VLV: `2.16.840.1.113730.3.4.9` — Virtual List View (offset-based pagination)
  - ManageDsaIT: `2.16.840.1.113730.3.4.2` — prevent chasing referrals

**Practice:**
- Implement the Paged Results control: encode request, decode response cookie, loop until no cookie
- Write `search_all_paged(base, filter, page_size)` that internally uses paged results and returns all entries

**LDAP relevance:** Without paged results, you can only retrieve 1000 entries at once from most servers. Paging is mandatory for large directories.

---

### Step 58 — SASL Authentication: PLAIN and EXTERNAL
**Learn:**
- SASL (Simple Authentication and Security Layer) framework: RFC 4422
- How SASL is layered over LDAP BindRequest/BindResponse
- SASL PLAIN mechanism (RFC 4616): `\0username\0password` — simple, requires TLS
- SASL EXTERNAL mechanism: identity derived from TLS client certificate
- Challenge-response pattern: server sends challenge, client sends response

**Practice:**
- Implement SASL PLAIN bind: encode the `\0user\0pass` OCTET STRING, wrap in SASL BindRequest
- Implement SASL EXTERNAL bind: empty credentials, identity from TLS cert
- Test both against OpenLDAP with appropriate TLS configuration

**LDAP relevance:** SASL is how enterprise LDAP clients authenticate without sending cleartext passwords (except PLAIN over TLS, which is acceptable).

---

### Step 59 — SASL DIGEST-MD5 Mechanism
**Learn:**
- DIGEST-MD5 (RFC 2831): challenge-response mechanism using MD5
- Server sends a challenge: `realm`, `nonce`, `qop`, `charset`, `algorithm`
- Client computes: `HA1 = MD5(username:realm:password)`, `HA2 = MD5(method:digestURI)`, `response = MD5(HA1:nonce:nc:cnonce:qop:HA2)`
- Client sends: `username`, `realm`, `nonce`, `cnonce`, `nc`, `qop`, `response`, `digest-uri`
- Two-step: BindRequest → BindResponse(saslCreds=challenge) → BindRequest(credentials=response) → BindResponse(result)

**Practice:**
- Parse the DIGEST-MD5 challenge string (comma-separated `key=value` pairs, with quoting)
- Implement the full response computation using your MD5 from Step 32
- Test against an LDAP server configured for DIGEST-MD5

**LDAP relevance:** DIGEST-MD5 is common in older enterprise LDAP deployments. Your library must support it.

---

### Step 60 — LDIF Format: Full Parser and Generator
**Learn:**
- LDIF (LDAP Data Interchange Format): RFC 2849
- Content records: `dn:` + attribute lines
- Change records: `changetype: add/modify/delete/modrdn`
- Base64 values: `attr:: base64value` (double colon means Base64)
- URL values: `attr:< url` (less-than means fetch from URL)
- Line folding: continuation lines start with a single space
- Comments: lines starting with `#`

**Practice:**
- Write a complete LDIF parser: handle all record types, Base64 decoding, line folding, comments
- Write a complete LDIF generator: proper line folding at 76 chars, Base64 for non-UTF-8 values
- Test: round-trip 1000 random LDAP entries through your parser and generator

**LDAP relevance:** LDIF is the standard backup/restore format for all LDAP directories. A library without LDIF support is incomplete.

---

## 🏗️ PHASE 6 — ADVANCED C++ (Steps 61–75)

### Step 61 — Advanced Templates: Variadic Templates and Parameter Packs
**Learn:**
- Variadic templates: `template<typename... Args>`
- Parameter pack expansion: `sizeof...(Args)`, `Args...`, `args...`
- Fold expressions (C++17): `(args + ...)`, `(... && args)`
- `std::tuple<T...>` and `std::get<N>(tuple)`
- Template metaprogramming basics: compile-time computation

**Practice:**
- Write `encode_sequence(const Ber& writer, Args&&... elements)` that encodes any number of BER elements into a SEQUENCE
- Write a type-safe `LdapFilter::make_and(Filter f1, Filter f2, MoreFilters... more)` using variadic templates

**LDAP relevance:** Variadic templates enable expressive, type-safe APIs for building LDAP requests without boilerplate.

---

### Step 62 — SFINAE, Type Traits, and Concepts (C++20)
**Learn:**
- `std::enable_if<condition, T>` — SFINAE (Substitution Failure Is Not An Error)
- Type traits: `std::is_integral<T>`, `std::is_same<T,U>`, `std::is_convertible<From,To>`
- `constexpr if` (C++17) as a cleaner alternative to SFINAE
- C++20 Concepts: `template<typename T> requires std::integral<T>`
- Writing your own concept: `template<typename T> concept LdapEncodable = requires(T t) { t.encode(); };`

**Practice:**
- Write a `BerEncoder` template that only accepts types satisfying `LdapEncodable` concept
- Add `static_assert` messages that give helpful errors when wrong types are used

**LDAP relevance:** Clean compile-time errors help users of your library fix mistakes quickly.

---

### Step 63 — Multithreading: Threads, Mutexes, and Condition Variables
**Learn:**
- `std::thread`: creation, `join()`, `detach()`
- `std::mutex`, `std::recursive_mutex`, `std::shared_mutex`
- `std::lock_guard<T>`, `std::unique_lock<T>`, `std::scoped_lock` (C++17 multi-mutex)
- `std::condition_variable`: `wait()`, `notify_one()`, `notify_all()`
- Deadlock: causes, detection, prevention (lock ordering, RAII)
- Data races: undefined behavior, detection with ThreadSanitizer

**Practice:**
- Add thread safety to `LdapConnectionPool`: multiple threads acquiring/releasing connections simultaneously
- Write a test using 8 threads each doing 100 LDAP searches concurrently via the pool
- Run with `-fsanitize=thread` to detect any races

**LDAP relevance:** Multi-threaded web servers use your library from many threads simultaneously. Thread safety is not optional.

---

### Step 64 — Atomics and Lock-Free Programming
**Learn:**
- `std::atomic<T>`: load, store, exchange, compare_exchange_weak/strong
- Memory ordering: `memory_order_relaxed`, `memory_order_acquire`, `memory_order_release`, `memory_order_seq_cst`
- The happens-before relationship and the C++ memory model
- Lock-free data structures: when they are faster (rarely!) and when they are not
- ABA problem in lock-free algorithms

**Practice:**
- Implement the message ID counter in `LdapConnection` using `std::atomic<uint32_t>`
- Write a lock-free SPSC (Single-Producer Single-Consumer) queue for passing received bytes from the network thread to the processing thread

**LDAP relevance:** The network receive loop and the message dispatch loop run in different threads. A lock-free queue avoids mutex contention on the hot path.

---

### Step 65 — Async I/O: Implementing an Async LDAP Client
**Learn:**
- The difference between synchronous, asynchronous, and non-blocking I/O
- Completion callbacks vs `std::future<T>` vs coroutines
- `std::promise<T>` and `std::future<T>`: setting a value from one thread, getting it from another
- `std::packaged_task<F>`: wraps a callable for async execution
- The reactor pattern: one thread handles I/O, dispatches to worker threads

**Practice:**
- Implement `LdapConnection::async_search(params, callback)` that returns immediately
- Implement `LdapConnection::sync_search(params) -> std::future<SearchResult>` using the async version internally
- Write an event loop that handles responses for multiple in-flight requests simultaneously using message IDs

**LDAP relevance:** High-performance LDAP clients pipeline multiple requests on one connection. Async is the architecture that enables this.

---

### Step 66 — C++20 Coroutines
**Learn:**
- What coroutines are: functions that can be suspended and resumed
- `co_await`, `co_yield`, `co_return`
- Promise types, awaitables, and the coroutine frame
- Writing a simple `Task<T>` coroutine type
- Integrating coroutines with `epoll`/`select` for async network I/O

**Practice:**
- Write a coroutine-based LDAP client:
  ```cpp
  Task<SearchResult> search(const SearchParams& p) {
    co_await send(encode_search_request(p));
    co_return co_await recv_search_result();
  }
  ```
- Compare the readability vs the callback-based version from Step 65

**LDAP relevance:** Coroutines are the future of async C++. Modern LDAP libraries will use them.

---

### Step 67 — Custom Memory Allocators
**Learn:**
- How `malloc`/`free` work: the heap, free lists, bins, `sbrk`/`mmap`
- Memory allocator design: bump allocator, pool allocator, arena allocator
- `std::allocator<T>`: the standard allocator interface
- Custom allocators for STL containers: `std::vector<T, MyAlloc<T>>`
- Memory pools: why they are faster for fixed-size objects (no fragmentation)

**Practice:**
- Write `ArenaAllocator`: allocate from a large buffer, free everything at once
- Write `PoolAllocator<T, N>`: a pool of N pre-allocated T objects
- Measure: parse 100,000 LDAP responses using the default allocator vs ArenaAllocator

**LDAP relevance:** When parsing thousands of search results, allocation overhead adds up. A custom allocator speeds up parsing significantly.

---

### Step 68 — SIMD and CPU Optimization
**Learn:**
- SIMD (Single Instruction, Multiple Data): SSE2, AVX2 intrinsics
- Cache lines: 64 bytes, how data layout affects cache performance
- Branch prediction: why predictable branches are faster
- Compiler auto-vectorization: when `g++ -O2 -mavx2` vectorizes your loops
- `__builtin_expect(cond, likely)` for branch prediction hints
- `constexpr` functions and compile-time evaluation

**Practice:**
- Use `__builtin_expect` on the "length fits in short form" check in your BER length encoder
- Implement `find_ber_element_fast(const uint8_t* buf, size_t len)` using SIMD to scan for tag bytes
- Profile before and after with `perf stat` and measure the improvement

**LDAP relevance:** Searching through a large LDAP response buffer benefits from SIMD scanning.

---

### Step 69 — Writing a Complete Test Framework from Scratch
**Learn:**
- Unit testing concepts: isolation, determinism, repeatability
- Test runner architecture: test registry, test runner, reporter
- Assertion macros: `EXPECT_EQ`, `EXPECT_NE`, `EXPECT_THROW`
- Property-based testing: generate random inputs, find edge cases
- Fuzz testing with libFuzzer: `LLVMFuzzerTestOneInput(const uint8_t* data, size_t size)`

**Practice:**
- Write your own test framework (no Google Test, no Catch2):
  - Test registration via a macro that fills a global registry
  - Colored output: green PASS, red FAIL
  - Exception catching per test
  - Summary: X passed, Y failed, Z skipped
- Write 200 test cases covering the entire library so far

**LDAP relevance:** A library without tests is a liability. 200+ tests give you confidence to refactor and optimize.

---

### Step 70 — Documentation, API Design, and Versioning
**Learn:**
- Doxygen: `@brief`, `@param`, `@return`, `@throws`, `@example`
- Semantic versioning: `MAJOR.MINOR.PATCH`
- ABI (Application Binary Interface) stability: what breaks ABI, how to maintain it
- The PIMPL idiom (Pointer to IMPLementation) for ABI stability
- `[[nodiscard]]`, `[[deprecated]]`, `[[maybe_unused]]` attributes
- Writing a `CHANGELOG.md`

**Practice:**
- Add Doxygen comments to every public function and class
- Generate HTML documentation with `doxygen`
- Apply the PIMPL idiom to `LdapConnection` so its internal implementation can change without breaking users

**LDAP relevance:** An undocumented library is unused. Professional documentation is what separates a personal project from a public framework.

---

## 🏢 PHASE 7 — LDAP ADVANCED FEATURES (Steps 71–85)

### Step 71 — Schema: Object Classes and Attribute Types
**Learn:**
- LDAP schema: the rules for what attributes an entry can have
- Object classes: `STRUCTURAL`, `AUXILIARY`, `ABSTRACT`
- Required attributes (`MUST`) vs optional attributes (`MAY`)
- Attribute types: syntax, equality rule, ordering rule, substring rule, single-value flag
- `subschemaSubentry`: where the server stores its schema
- Reading schema via `(objectClass=*)` search at the schema DN

**Practice:**
- Write `LdapSchema` class that retrieves and parses schema from a live server
- Parse `attributeTypes` and `objectClasses` from the server's schema entry
- Write `schema.validate(LdapEntry&) -> std::vector<SchemaError>` that checks an entry against schema

**LDAP relevance:** Without schema awareness, your library can create invalid entries that servers reject.

---

### Step 72 — Referrals and Chaining
**Learn:**
- LDAP referrals: when a server doesn't have an entry, it points you to another server
- `SearchResultReference`: contains LDAP URLs to follow
- `LDAP_REFERRAL (10)` result code in `LDAPResult`
- Chasing referrals: automatically following them vs returning them to the caller
- Referral loops: detecting and preventing infinite referral chains

**Practice:**
- Implement automatic referral chasing in `search()` (configurable: on/off)
- Implement loop detection: track visited URLs, stop after 10 hops
- Test with an LDAP server that returns referrals (set up two OpenLDAP instances)

**LDAP relevance:** Active Directory uses referrals extensively for cross-domain queries.

---

### Step 73 — Server-Side Sorting and VLV (Virtual List View)
**Learn:**
- Server-Side Sort (SSS) control: OID `1.2.840.113556.1.4.473`
  - Request: list of `SortKey` (attribute + ordering rule + reverse flag)
  - Response: sort result code
- VLV control: OID `2.16.840.1.113730.3.4.9`
  - Request: beforeCount, afterCount, offset or assertionValue, contextID
  - Response: targetPosition, contentCount, virtualListViewResult
- How VLV enables efficient "jump to page N" in huge result sets

**Practice:**
- Implement SSS control encoding and response decoding
- Implement VLV control: request by offset and by assertionValue
- Write `paged_search_by_offset(base, filter, sort_attr, page_size, page_number) -> SearchPage`

**LDAP relevance:** Any UI showing a sortable, pageable list of directory entries needs SSS + VLV.

---

### Step 74 — LDAP Transactions (RFC 5805) and Batch Operations
**Learn:**
- RFC 5805: LDAP Transactions (extension OID `1.3.6.1.1.21.1`)
- Start Transaction → get transaction ID → operations with Transaction control → End Transaction (commit/rollback)
- Why LDAP transactions are not widely supported (only OpenLDAP)
- Simulating transactions: optimistic locking with compare-and-swap using the Compare operation

**Practice:**
- Implement the Start Transaction extended operation
- Implement the Transaction control (attach transaction ID to operations)
- Implement the End Transaction extended operation (commit or abort)

**LDAP relevance:** Applications needing atomic multi-entry updates require transaction support.

---

### Step 75 — Active Directory Specifics
**Learn:**
- Active Directory OID extensions: `1.2.840.113556.1.4.*`
- `userAccountControl` bitmask attribute: enabled/disabled/locked/password-never-expires flags
- `objectGUID` and `objectSid`: binary attributes requiring special decoding
- `unicodePwd`: setting passwords in Active Directory (UTF-16LE, Kerberos-encrypted)
- AD-specific controls: LDAP_SERVER_SHOW_DELETED_OID, LDAP_SERVER_DIRSYNC_OID
- NETLOGON service and ping via CLDAP (connectionless LDAP over UDP)

**Practice:**
- Write `decode_object_guid(const std::vector<uint8_t>& bytes) -> std::string` (format: `{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}`)
- Write `decode_object_sid(const std::vector<uint8_t>& bytes) -> std::string` (format: `S-1-5-21-...`)
- Write `parse_user_account_control(uint32_t uac) -> UserAccountFlags`

**LDAP relevance:** Any Windows enterprise environment uses Active Directory. Your library must handle AD peculiarities.

---

## 🔒 PHASE 8 — TLS INTEGRATION AND SECURITY (Steps 76–85)

### Step 76 — Integrating OS-Level TLS: OpenSSL C API
**Learn:**
- `SSL_CTX`, `SSL`, `SSL_library_init`, `SSL_CTX_new`, `TLS_client_method`
- `SSL_new`, `SSL_set_fd`, `SSL_connect`, `SSL_read`, `SSL_write`, `SSL_shutdown`
- Certificate verification: `SSL_CTX_set_verify`, `SSL_CTX_load_verify_locations`
- Client certificates: `SSL_CTX_use_certificate_file`, `SSL_CTX_use_PrivateKey_file`
- Error handling: `SSL_get_error`, `ERR_get_error`, `ERR_error_string`

**Practice:**
- Wrap OpenSSL into a `TlsSocket` class that satisfies the same interface as your plain `Socket` from Step 28
- Implement LDAPS: connect to port 636 with TLS from the start
- Implement StartTLS: upgrade an existing plaintext connection

**LDAP relevance:** Without TLS, passwords sent over the network are readable by anyone. TLS is required for production use.

---

### Step 77 — Certificate Validation and Pinning
**Learn:**
- Full chain validation: root CA → intermediate CA → server cert
- Common Name vs Subject Alternative Name (SAN): why CN is deprecated for hostname verification
- `SSL_CTX_set1_host()` (OpenSSL 1.0.2+) for automatic hostname verification
- Certificate pinning: accepting only specific certificates or public keys
- CRL (Certificate Revocation List) and OCSP (Online Certificate Status Protocol)

**Practice:**
- Implement `TlsSocket::set_verify_hostname(const std::string& hostname)`
- Implement `TlsSocket::pin_certificate(const std::string& pem_cert)`
- Test: verify that your client rejects a server with an expired or self-signed certificate

**LDAP relevance:** Accepting any certificate defeats the purpose of TLS. Proper validation is mandatory.

---

### Step 78 — SASL GSSAPI / Kerberos
**Learn:**
- Kerberos concepts: KDC, TGT, service tickets, Kerberos realm
- GSSAPI (Generic Security Services API): the abstraction layer over Kerberos
- SASL/GSSAPI mechanism: multi-step token exchange
- `gss_init_sec_context()` and `gss_accept_sec_context()`
- Kerberos in Active Directory: every AD authentication uses Kerberos
- SPN (Service Principal Name): `ldap/ldap.example.com@EXAMPLE.COM`

**Practice:**
- Implement SASL/GSSAPI bind using the GSSAPI C API (no Kerberos from scratch — use OS krb5 library)
- Acquire a Kerberos ticket and authenticate to an Active Directory LDAP server
- Implement `get_kerberos_ticket(principal, keytab_file)`

**LDAP relevance:** In enterprise environments, Kerberos/GSSAPI is the expected authentication method. NTLM and simple bind are considered insecure.

---

### Step 79 — Input Validation and Security Hardening
**Learn:**
- LDAP injection: how unescaped user input in filter strings creates vulnerabilities
- Proper escaping for DN values: RFC 4514 escaping rules
- Proper escaping for filter values: RFC 4515 escaping rules (`\xx` for special bytes)
- DoS protection: maximum message size, maximum nesting depth, timeout enforcement
- Buffer overflow prevention: always use length-checked copies

**Practice:**
- Write `escape_dn_value(const std::string& val) -> std::string` (RFC 4514)
- Write `escape_filter_value(const std::string& val) -> std::string` (RFC 4515)
- Write `safe_filter(const std::string& attr, const std::string& user_input) -> Filter` — always safe

**LDAP relevance:** A library that allows LDAP injection is a security vulnerability waiting to be exploited.

---

### Step 80 — Logging, Tracing, and Observability
**Learn:**
- Log levels: TRACE, DEBUG, INFO, WARN, ERROR, FATAL
- Structured logging: JSON log lines with fields
- Log sinks: stderr, file, syslog
- Packet tracing: logging raw BER bytes for debugging
- Performance metrics: operations/second, latency percentiles (p50, p95, p99)
- OpenTelemetry concepts for distributed tracing

**Practice:**
- Write a `Logger` class with configurable levels and sinks
- Add packet-level tracing: log every BER packet sent and received (hex dump)
- Add metrics: count operations, measure latency with `std::chrono::high_resolution_clock`
- Format latency report: `search p50=1.2ms p95=8.5ms p99=23ms`

**LDAP relevance:** When your LDAP library misbehaves in production, logs are all you have. Good observability is essential.

---

### Step 81 — Rate Limiting and Retry Logic
**Learn:**
- Exponential backoff with jitter: why it prevents thundering herd
- Retry-able vs non-retry-able errors: `BUSY(51)`, `UNAVAILABLE(52)` → retry; `INVALID_DN(34)` → don't retry
- Rate limiting: token bucket algorithm, leaky bucket algorithm
- Circuit breaker pattern: stop sending requests to a failing server

**Practice:**
- Write `ExponentialBackoff(base_ms=100, max_ms=30000, jitter=true)`
- Add retry logic to all LDAP operations with configurable max attempts
- Implement a circuit breaker for `LdapConnectionPool`

**LDAP relevance:** LDAP servers go down. Your library must handle this gracefully without hammering a failing server.

---

### Step 82 — Caching Layer
**Learn:**
- Cache invalidation strategies: TTL (Time To Live), LRU (Least Recently Used), event-driven
- LDAP persistent search (OID `2.16.840.1.113730.3.4.3`): receive notifications when entries change
- Thread-safe LRU cache: `std::unordered_map` + `std::list` + `std::shared_mutex`
- Cache key design for LDAP: hash of `(base_dn, scope, filter, attributes)`

**Practice:**
- Implement a thread-safe LRU cache: `LruCache<Key, Value, MaxSize>`
- Add search result caching to `LdapConnection` with configurable TTL
- Implement cache invalidation using LDAP persistent search (if supported by server)

**LDAP relevance:** LDAP authentication servers often get thousands of identical queries per second. Caching reduces server load by 90%+.

---

### Step 83 — Implementing a Simple LDAP Server
**Learn:**
- Server-side LDAP: binding, parsing requests, sending responses
- The LDAP server state machine: per-connection state (anonymous, authenticated, etc.)
- A simple in-memory directory: tree of entries, indexed by DN
- Access Control: who can read/write which attributes
- Implementing the `slapd` wire protocol from the server side

**Practice:**
- Write a minimal LDAP server that handles: Bind, Search, Unbind
- Store data in a `std::map<Dn, LdapEntry>` in memory
- Test it with `ldapsearch` command-line tool: `ldapsearch -H ldap://localhost:3890 -x -b "dc=test,dc=com" "(objectClass=*)"`

**LDAP relevance:** Implementing a server forces you to understand the protocol from the other side. It also gives you a test target for your client library.

---

### Step 84 — Replication and Synchronization Protocol (RFC 4533)
**Learn:**
- LDAP Content Synchronization Protocol (RFC 4533 / "syncrepl")
- Sync modes: `refreshOnly` vs `refreshAndPersist`
- `syncUUID` and `entryCSN`: how changes are tracked
- `syncInfoValue` control: present, sync-id-set, new-cookie, refresh-done, refresh-delete
- Using syncrepl to keep a local cache synchronized with a remote directory

**Practice:**
- Implement `LdapSyncClient` that performs an initial bulk load then listens for changes
- Store changes in a `std::queue<SyncEvent>` for the application to process
- Test: add/modify/delete entries on the server, verify your sync client receives them

**LDAP relevance:** Applications needing real-time directory state (authentication caches, group membership caches) use syncrepl.

---

### Step 85 — Performance Benchmarking and Optimization
**Learn:**
- LDAP performance metrics: binds/second, searches/second, latency distribution
- Profiling with `perf record` + `perf report` and flame graphs
- Bottleneck identification: is the bottleneck in BER encoding? Network? DNS? TLS handshake?
- Connection multiplexing: many requests per connection (pipelining)
- Request batching: combining multiple operations into one TCP segment

**Practice:**
- Write a benchmark suite:
  - 10,000 simple binds (cold): measure bind latency
  - 100,000 searches (warm, same filter): measure search throughput
  - 1,000 concurrent operations via the connection pool
- Generate a flame graph and identify the top 3 hotspots
- Optimize at least one hotspot by 30%+

**LDAP relevance:** Production LDAP clients must handle thousands of authentications per second. Performance is a feature.

---

## 🎯 PHASE 9 — PACKAGING, DISTRIBUTION, AND COMPLETION (Steps 86–100)

### Step 86 — Static Analysis and Code Quality
**Learn:**
- `clang-tidy`: modernize-*, readability-*, bugprone-*, performance-* checks
- `cppcheck`: null pointer, buffer overflow, uninitialized variable detection
- `include-what-you-use`: reduce unnecessary #includes for faster compilation
- Code coverage: `gcov` + `lcov` → HTML coverage report
- Cyclomatic complexity: measure function complexity, refactor functions over 10

**Practice:**
- Run `clang-tidy` on the entire codebase and fix all warnings
- Achieve 90%+ test coverage measured by `gcov`
- Fix every `cppcheck` error

**LDAP relevance:** Static analysis finds bugs before users do.

---

### Step 87 — Cross-Platform Build and CI/CD
**Learn:**
- GitHub Actions / GitLab CI: YAML pipeline definition
- Building on Linux (GCC + Clang), macOS (Clang), Windows (MSVC + MinGW)
- Docker: building a Linux container for reproducible builds
- `cmake --preset`: CMake preset system for multiple build configurations
- `ctest`: test runner integrated with CMake

**Practice:**
- Write a GitHub Actions workflow that:
  - Builds on Linux (GCC 12, Clang 15), macOS, and Windows
  - Runs all tests on each platform
  - Runs AddressSanitizer and UBSanitizer builds
  - Uploads coverage reports
- Create a Docker image for the Linux build environment

**LDAP relevance:** A library only you can build isn't a library. CI/CD ensures it works everywhere.

---

### Step 88 — Package Management and Distribution
**Learn:**
- `pkg-config`: `.pc` files, `pkg-config --cflags --libs oxsium`
- Conan package manager: `conanfile.py`, `conan create`
- vcpkg: `portfile.cmake`, `vcpkg.json`
- CPack: generating `.tar.gz`, `.deb`, `.rpm`, `.msi` packages
- Header-only library option: pros and cons

**Practice:**
- Write `oxsium.pc` for pkg-config
- Write a `conanfile.py` that packages your library for Conan
- Use CPack to generate `.deb` and `.tar.gz` packages
- Write a "Getting Started in 5 minutes" guide that uses these packages

**LDAP relevance:** If other developers can't easily add your library to their projects, they won't use it.

---

### Step 89 — Writing Language Bindings: C API
**Learn:**
- Why C APIs are the lingua franca of library interoperability
- `extern "C"`: preventing C++ name mangling
- Opaque handle pattern: `typedef struct OxsiumConnection* OxsiumConnectionHandle;`
- Error codes vs exceptions at the C boundary
- Memory ownership conventions in C APIs: who allocates, who frees

**Practice:**
- Write a C API wrapper for your library:
  ```c
  OxsiumConnection* oxsium_connect(const char* host, int port, OxsiumError* err);
  int oxsium_bind(OxsiumConnection* conn, const char* dn, const char* password);
  OxsiumResult* oxsium_search(OxsiumConnection* conn, const char* base, ...);
  void oxsium_free_result(OxsiumResult* result);
  void oxsium_disconnect(OxsiumConnection* conn);
  ```
- Test the C API from a pure C program

**LDAP relevance:** A C API allows Python, Go, Rust, Java (JNI), and any other language to use your library via FFI.

---

### Step 90 — Writing Language Bindings: Python
**Learn:**
- Python C extension API: `PyObject*`, `PyArg_ParseTuple`, `Py_BuildValue`
- `ctypes`: calling your C API from Python without a C extension
- `cffi`: alternative to ctypes with better ergonomics
- `pybind11`: C++ library for writing Python bindings (but you already understand the C approach)

**Practice:**
- Write a Python module using `ctypes` that wraps your C API:
  ```python
  import oxsium
  conn = oxsium.connect("ldap.example.com", 389)
  conn.bind("cn=admin,dc=example,dc=com", "password")
  results = conn.search("dc=example,dc=com", oxsium.SCOPE_SUB, "(objectClass=person)")
  ```
- Write 20 integration tests using your Python binding

**LDAP relevance:** Most DevOps tooling and automation scripts are written in Python. A Python binding dramatically increases adoption.

---

### Step 91 — RFC Compliance Verification
**Learn:**
- RFC 4510: LDAP Technical Specification Road Map
- RFC 4511: LDAP Protocol
- RFC 4512: Directory Information Models
- RFC 4513: Authentication Methods
- RFC 4514: String Representation of Distinguished Names
- RFC 4515: String Representation of Search Filters
- RFC 4516: Uniform Resource Locator
- RFC 4517: Attribute Syntaxes
- RFC 4518: String Preparation
- RFC 4519: User Schema

**Practice:**
- Read each RFC and create a checklist: which features are implemented, which are missing
- Implement the top 5 missing mandatory features
- Run your library against the OpenLDAP test suite

**LDAP relevance:** RFC compliance is what makes your library interoperable with all LDAP servers, not just OpenLDAP.

---

### Step 92 — Security Audit and Penetration Testing
**Learn:**
- OWASP Top 10 for APIs: how they apply to an LDAP library
- LDAP injection: fuzz your filter builder with malicious strings
- Buffer overflow: fuzz your BER decoder with malformed input
- Integer overflow: check all length calculations for overflow
- Timing attacks: ensure password comparison is constant-time

**Practice:**
- Write a fuzz target for BER decoder: `LLVMFuzzerTestOneInput` that feeds random bytes to your parser
- Write a fuzz target for the filter parser
- Implement `constant_time_compare(const uint8_t* a, const uint8_t* b, size_t len) -> bool` using `volatile` to prevent compiler optimization
- Run `afl-fuzz` for 24 hours and fix every crash

**LDAP relevance:** Security libraries must be provably secure. Fuzzing finds bugs that unit tests miss.

---

### Step 93 — Writing Comprehensive Examples
**Learn:**
- Example-driven documentation: working code is the best documentation
- Error handling in example code: show all error paths, not just the happy path
- "Cookbook" style documentation: recipes for common tasks

**Practice:**
- Write 10 complete, standalone example programs:
  1. Simple bind and search
  2. Paged search of 100,000+ entries
  3. Adding/modifying/deleting entries
  4. LDAPS connection with certificate verification
  5. StartTLS upgrade
  6. SASL DIGEST-MD5 authentication
  7. Connection pool with 8 worker threads
  8. LDIF export and import
  9. Schema inspection
  10. Active Directory user enumeration

**LDAP relevance:** Developers evaluate libraries by running examples first. Good examples = adoption.

---

### Step 94 — Performance Comparison with Other Libraries
**Learn:**
- OpenLDAP's `libldap`: its API, its performance characteristics
- Apache LDAP API (Java): design patterns worth emulating
- `ldap3` (Python): pure-Python LDAP, design philosophy
- How to write a fair benchmark: same server, same data, same queries

**Practice:**
- Benchmark Oxsium vs `libldap` (C):
  - Binds/second
  - Searches/second
  - Memory usage per 10,000 entries parsed
  - Binary size of a simple program using each library
- Write a `BENCHMARKS.md` documenting the results honestly

**LDAP relevance:** Knowing where your library stands helps you set expectations and find areas to improve.

---

### Step 95 — Versioning, Changelog, and Semantic Versioning
**Learn:**
- Git tagging: `git tag v1.0.0 -m "Initial release"`
- Conventional Commits: `feat:`, `fix:`, `breaking:`, `docs:`
- Automated changelog generation from commit messages
- Git branching strategy: `main` (stable), `develop` (next), `release/x.y` branches
- `git bisect` for finding the commit that introduced a bug

**Practice:**
- Write your `CHANGELOG.md` following "Keep a Changelog" format
- Tag v0.1.0 through v1.0.0 milestones as you complete each phase
- Set up `git-cliff` or `standard-version` for automated changelog generation

**LDAP relevance:** Version discipline is how you communicate stability guarantees to users.

---

### Step 96 — Writing the README and Architecture Documentation
**Learn:**
- README structure: badges, description, installation, quickstart, API reference link, license
- Architecture decision records (ADRs): document why you made each major design choice
- `CONTRIBUTING.md`: how others can contribute
- `SECURITY.md`: how to report vulnerabilities
- Diagrams: C4 model (Context, Container, Component, Code)

**Practice:**
- Write a complete `README.md` with: build badge, one-command install, 20-line quickstart, feature list
- Write ADRs for: BER encoder design, connection pool design, error model choice, TLS integration approach
- Draw the system architecture using a C4 Context diagram

**LDAP relevance:** Open-source success depends on documentation quality. This is often what makes or breaks adoption.

---

### Step 97 — Open Source Release Preparation
**Learn:**
- Choosing a license: MIT (permissive), Apache 2.0 (patent grant), LGPL (copyleft for libraries)
- `SPDX` license identifiers in source files
- CLA (Contributor License Agreement): do you need one?
- GitHub: repository setup, issue templates, PR templates, discussions
- `CODEOWNERS` file for automatic review assignment

**Practice:**
- Choose your license and add headers to every source file
- Create GitHub issue templates for: bug report, feature request, security vulnerability
- Write `CONTRIBUTING.md` with: code style guide, how to run tests, PR process

**LDAP relevance:** A proper open-source release is what turns a personal project into the "Oxsium Framework" used by others.

---

### Step 98 — Community and Ecosystem
**Learn:**
- Writing a blog post announcing your library: problem statement, solution, benchmarks, call to action
- Submitting to package registries: vcpkg, Conan Center, AUR (Arch Linux)
- Responding to GitHub issues: triage, labels, milestones
- Collecting and prioritizing feature requests
- Maintaining backward compatibility as you add features

**Practice:**
- Write a 2000-word blog post: "I wrote an LDAP library from scratch in C++ — here's what I learned"
- Submit a vcpkg port PR
- Create a GitHub Discussions board for questions and RFCs

**LDAP relevance:** A library without a community doesn't survive. Building an audience is part of building a successful open-source project.

---

### Step 99 — Reflection: What You've Built
**Milestone Review:**

At this point, you have built from scratch:

**C++ Mastery:**
- Complete understanding of compilation, linking, memory, types, templates, move semantics, coroutines, atomics, threading, SIMD, and custom allocators

**Cryptography (from bits):**
- MD5, SHA-1, SHA-256, Base64, RC4, AES-128, Diffie-Hellman, CSPRNG, password verification

**Networking (from syscalls):**
- TCP sockets, non-blocking I/O, epoll event loop, TLS integration, DNS resolution, connection pooling

**BER/ASN.1 (from the RFC):**
- Complete bidirectional BER codec: tag, length, integer, octet string, sequence, OID

**LDAP Protocol (from RFC 4511):**
- All 9 operations: Bind, Search, Add, Delete, Modify, ModifyDN, Compare, Abandon, Extended
- Controls: Paged, Sort, VLV, Transaction
- SASL: PLAIN, EXTERNAL, DIGEST-MD5, GSSAPI
- LDIF: full parser and generator
- Schema: retrieval and validation

**Library Quality:**
- Cross-platform (Linux, macOS, Windows)
- Thread-safe
- Fuzz-tested
- RFC-compliant
- Documented
- Packaged

**Write a personal retrospective:** What was hardest? What would you do differently? What did you learn about LDAP that surprised you?

---

### Step 100 — What Comes Next
**Your library is done. Here is what you can do next:**

**Extend the library:**
- LDAP over QUIC (future RFC)
- Full TLS 1.3 implementation from scratch (replacing the OS TLS dependency)
- GraphQL-to-LDAP query translator
- An LDAP proxy server built on Oxsium

**Deeper C++ mastery:**
- Compiler development: write a simple C compiler
- Operating systems: write a simple kernel
- Embedded systems: use your BER codec on a microcontroller

**Contribute to the ecosystem:**
- Contribute improvements back to OpenLDAP
- Write an IETF Internet-Draft proposing an LDAP extension
- Present at a C++ conference (CppCon, Meeting C++)

**The meta-lesson:**
You did not just learn C++ and LDAP. You learned how to read RFCs and turn them into working code. You learned how to implement cryptography from mathematical definitions. You learned how to build a production-quality library from nothing. This skill — reading a specification and implementing it — is the most valuable thing a software engineer can possess.

**Oxsium Framework v1.0.0 is yours. Ship it.**

---

## 📚 ESSENTIAL REFERENCES

| Resource | Why |
|----------|-----|
| RFC 4511 | The LDAP protocol specification — read it 10 times |
| RFC 5280 | X.509 certificates — needed for TLS |
| X.680 / X.690 | ASN.1 and BER — the formal specification |
| FIPS 180-4 | SHA hash functions |
| RFC 4648 | Base64 |
| RFC 2849 | LDIF format |
| RFC 4422 | SASL framework |
| "The C++ Programming Language" — Bjarne Stroustrup | Language reference from the creator |
| "Effective Modern C++" — Scott Meyers | C++11/14 best practices |
| "C++ Concurrency in Action" — Anthony Williams | Everything about threading |
| cppreference.com | The definitive C++ standard library reference |
| Beej's Guide to Network Programming | POSIX socket API guide |

---

*Built with ❤️ by you — from raw bits to Oxsium Framework*
