# 🐍 Python `socket` Library — Complete Cheat Sheet

---

## 📦 Import

```python
import socket
```

---

## 🔑 Key Concepts

| Term | Description |
|------|-------------|
| **Socket** | An endpoint for sending/receiving data across a network |
| **AF_INET** | Address Family — IPv4 |
| **AF_INET6** | Address Family — IPv6 |
| **AF_UNIX** | Address Family — Unix domain sockets (IPC) |
| **SOCK_STREAM** | TCP — reliable, connection-oriented |
| **SOCK_DGRAM** | UDP — fast, connectionless |
| **SOCK_RAW** | Raw socket — direct access to lower-level protocols |

---

## 🏗️ Creating a Socket

```python
# TCP Socket (IPv4)
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# UDP Socket (IPv4)
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# TCP Socket (IPv6)
sock = socket.socket(socket.AF_INET6, socket.SOCK_STREAM)

# Using context manager (auto-close)
with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    ...
```

---

## ⚙️ Socket Options

```python
# Allow reuse of address (avoid "Address already in use")
sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

# Allow reuse of port
sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEPORT, 1)

# Set send/receive buffer size
sock.setsockopt(socket.SOL_SOCKET, socket.SO_SNDBUF, 4096)
sock.setsockopt(socket.SOL_SOCKET, socket.SO_RCVBUF, 4096)

# Keep-alive
sock.setsockopt(socket.SOL_SOCKET, socket.SO_KEEPALIVE, 1)

# Get an option
val = sock.getsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR)
```

---

## ⏱️ Timeouts & Blocking

```python
sock.settimeout(5.0)        # 5 second timeout (non-blocking after timeout)
sock.setblocking(True)      # Blocking mode (default)
sock.setblocking(False)     # Non-blocking mode
sock.settimeout(None)       # Block indefinitely (same as setblocking(True))
sock.settimeout(0)          # Non-blocking (same as setblocking(False))

timeout = sock.gettimeout() # Get current timeout
```

---

## 🖥️ TCP Server — Step by Step

```python
import socket

HOST = '127.0.0.1'
PORT = 65432

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as server:
    server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

    # 1. Bind to address and port
    server.bind((HOST, PORT))

    # 2. Listen for incoming connections (backlog=5)
    server.listen(5)
    print(f"[*] Listening on {HOST}:{PORT}")

    # 3. Accept a connection
    conn, addr = server.accept()
    with conn:
        print(f"[+] Connected by {addr}")

        while True:
            # 4. Receive data
            data = conn.recv(1024)   # buffer size in bytes
            if not data:
                break

            # 5. Send data back
            conn.sendall(data)

print("[*] Server closed")
```

---

## 💻 TCP Client — Step by Step

```python
import socket

HOST = '127.0.0.1'
PORT = 65432

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as client:
    # 1. Connect to server
    client.connect((HOST, PORT))

    # 2. Send data
    client.sendall(b'Hello, Server!')

    # 3. Receive response
    data = client.recv(1024)
    print(f"Received: {data.decode()}")
```

---

## 📡 UDP Server & Client

```python
# ── UDP SERVER ──────────────────────────────────────────
import socket

with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as server:
    server.bind(('127.0.0.1', 9999))
    print("[*] UDP Server listening...")

    while True:
        data, addr = server.recvfrom(1024)   # returns (data, address)
        print(f"[+] From {addr}: {data.decode()}")
        server.sendto(b'ACK', addr)          # send reply


# ── UDP CLIENT ──────────────────────────────────────────
import socket

with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as client:
    client.sendto(b'Hello UDP', ('127.0.0.1', 9999))
    data, server_addr = client.recvfrom(1024)
    print(f"Reply: {data.decode()}")
```

---

## 📤 Sending & Receiving Methods

| Method | Description |
|--------|-------------|
| `send(data)` | Send bytes; may not send all — returns bytes sent |
| `sendall(data)` | Send all bytes; loops internally until done |
| `recv(bufsize)` | Receive up to `bufsize` bytes |
| `recvfrom(bufsize)` | UDP receive — returns `(data, addr)` |
| `sendto(data, addr)` | UDP send to specific address |
| `recv_into(buffer)` | Receive into a pre-allocated buffer |
| `makefile(mode)` | Wrap socket as a file-like object |

```python
# Safe receive — keep reading until full message
def recv_all(sock, length):
    data = b''
    while len(data) < length:
        chunk = sock.recv(length - len(data))
        if not chunk:
            raise ConnectionError("Socket closed prematurely")
        data += chunk
    return data
```

---

## 🔄 Multi-Client Server with `threading`

```python
import socket
import threading

def handle_client(conn, addr):
    print(f"[+] New connection from {addr}")
    with conn:
        while True:
            data = conn.recv(1024)
            if not data:
                break
            conn.sendall(data)
    print(f"[-] Disconnected: {addr}")

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as server:
    server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    server.bind(('0.0.0.0', 8080))
    server.listen()

    while True:
        conn, addr = server.accept()
        t = threading.Thread(target=handle_client, args=(conn, addr))
        t.daemon = True
        t.start()
```

---

## ⚡ Non-Blocking Server with `select`

```python
import socket
import select

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
server.bind(('0.0.0.0', 8080))
server.listen(5)
server.setblocking(False)

inputs  = [server]   # Sockets to read from
outputs = []         # Sockets to write to

while inputs:
    readable, writable, exceptional = select.select(inputs, outputs, inputs, 1.0)

    for s in readable:
        if s is server:
            conn, addr = s.accept()
            conn.setblocking(False)
            inputs.append(conn)
        else:
            data = s.recv(1024)
            if data:
                s.sendall(data)
            else:
                inputs.remove(s)
                s.close()

    for s in exceptional:
        inputs.remove(s)
        s.close()
```

---

## 🌐 Async Server with `asyncio` (Python 3.7+)

```python
import asyncio

async def handle_client(reader, writer):
    addr = writer.get_extra_info('peername')
    print(f"[+] Connected: {addr}")

    while True:
        data = await reader.read(1024)
        if not data:
            break
        writer.write(data)
        await writer.drain()

    writer.close()
    await writer.wait_closed()
    print(f"[-] Disconnected: {addr}")

async def main():
    server = await asyncio.start_server(handle_client, '127.0.0.1', 8888)
    async with server:
        await server.serve_forever()

asyncio.run(main())
```

---

## 🌍 DNS & Address Utilities

```python
# Get hostname of local machine
hostname = socket.gethostname()

# Get IP from hostname
ip = socket.gethostbyname('www.google.com')

# Get all IPs for a hostname
results = socket.getaddrinfo('www.google.com', 80)
# Returns list of (family, type, proto, canonname, sockaddr)

# Reverse DNS lookup
host, aliases, ips = socket.gethostbyaddr('8.8.8.8')

# Get FQDN
fqdn = socket.getfqdn()

# Get service port by name
port = socket.getservbyname('http')   # → 80
port = socket.getservbyname('https')  # → 443

# Get service name by port
name = socket.getservbyport(80)       # → 'http'
```

---

## 🔢 Byte Order Conversion (Network ↔ Host)

```python
# Host to Network (big-endian)
socket.htons(port)    # 16-bit short
socket.htonl(value)   # 32-bit long

# Network to Host
socket.ntohs(port)    # 16-bit short
socket.ntohl(value)   # 32-bit long

# IP address ↔ binary
packed   = socket.inet_aton('192.168.1.1')   # → 4-byte binary
ip_str   = socket.inet_ntoa(packed)           # → '192.168.1.1'

# IPv6 support
packed6  = socket.inet_pton(socket.AF_INET6, '::1')
ip6_str  = socket.inet_ntop(socket.AF_INET6, packed6)
```

---

## 🔌 Socket Info & Inspection

```python
sock.getsockname()     # Returns (host, port) of the socket itself
sock.getpeername()     # Returns (host, port) of the remote end
sock.fileno()          # Returns the socket's file descriptor
sock.family            # AF_INET, AF_INET6, etc.
sock.type              # SOCK_STREAM, SOCK_DGRAM, etc.
sock.proto             # Protocol number

# Check if port is open
def is_port_open(host, port, timeout=1):
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.settimeout(timeout)
        return s.connect_ex((host, port)) == 0
```

---

## 🔐 SSL / TLS Wrapping

```python
import ssl
import socket

context = ssl.SSLContext(ssl.PROTOCOL_TLS_CLIENT)
context.load_verify_locations('/etc/ssl/certs/ca-certificates.crt')

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as raw_sock:
    with context.wrap_socket(raw_sock, server_hostname='example.com') as tls_sock:
        tls_sock.connect(('example.com', 443))
        tls_sock.sendall(b'GET / HTTP/1.0\r\nHost: example.com\r\n\r\n')
        data = tls_sock.recv(4096)
        print(data.decode())
```

---

## 🚫 Common Exceptions

| Exception | Cause |
|-----------|-------|
| `socket.timeout` | Operation timed out |
| `socket.gaierror` | Address-related error (DNS failure) |
| `socket.herror` | Host-related error |
| `ConnectionRefusedError` | No server listening on that port |
| `ConnectionResetError` | Remote host forcibly closed the connection |
| `BrokenPipeError` | Sending to a closed socket |
| `OSError` | General OS-level socket error |

```python
try:
    sock.connect(('example.com', 80))
except socket.timeout:
    print("Connection timed out")
except socket.gaierror as e:
    print(f"DNS error: {e}")
except ConnectionRefusedError:
    print("Connection refused")
except OSError as e:
    print(f"Socket error: {e}")
finally:
    sock.close()
```

---

## 📋 Socket Constants Quick Reference

```python
# Address Families
socket.AF_INET        # IPv4
socket.AF_INET6       # IPv6
socket.AF_UNIX        # Unix domain sockets

# Socket Types
socket.SOCK_STREAM    # TCP
socket.SOCK_DGRAM     # UDP
socket.SOCK_RAW       # Raw sockets

# Protocol Levels
socket.SOL_SOCKET     # Socket-level options
socket.IPPROTO_TCP    # TCP options
socket.IPPROTO_UDP    # UDP options

# Socket Options
socket.SO_REUSEADDR   # Reuse address
socket.SO_REUSEPORT   # Reuse port
socket.SO_KEEPALIVE   # Enable keep-alive
socket.SO_SNDBUF      # Send buffer size
socket.SO_RCVBUF      # Receive buffer size
socket.SO_BROADCAST   # Enable broadcast (UDP)
socket.SO_LINGER      # Linger on close
socket.TCP_NODELAY    # Disable Nagle algorithm

# Shutdown constants
socket.SHUT_RD        # Shutdown read
socket.SHUT_WR        # Shutdown write
socket.SHUT_RDWR      # Shutdown both
```

---

## 🔌 Graceful Shutdown

```python
# Notify peer before closing
sock.shutdown(socket.SHUT_RDWR)
sock.close()

# Or simply use context manager — it calls close() automatically
with socket.socket(...) as sock:
    ...  # sock.close() is called at block exit
```

---

## 🛠️ Useful Patterns

### Simple Port Scanner
```python
import socket

def scan_ports(host, ports):
    open_ports = []
    for port in ports:
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
            s.settimeout(0.5)
            if s.connect_ex((host, port)) == 0:
                open_ports.append(port)
    return open_ports

print(scan_ports('127.0.0.1', range(1, 1025)))
```

### Message Framing (Length-Prefix Protocol)
```python
import struct

def send_msg(sock, msg: bytes):
    """Prefix each message with a 4-byte big-endian length."""
    msg = struct.pack('>I', len(msg)) + msg
    sock.sendall(msg)

def recv_msg(sock) -> bytes:
    """Read a length-prefixed message."""
    raw_len = recv_all(sock, 4)
    msg_len = struct.unpack('>I', raw_len)[0]
    return recv_all(sock, msg_len)
```

### Broadcast UDP
```python
import socket

with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as s:
    s.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
    s.sendto(b'Hello LAN!', ('<broadcast>', 9999))
```

---

## 📊 TCP vs UDP — Quick Comparison

| Feature | TCP (`SOCK_STREAM`) | UDP (`SOCK_DGRAM`) |
|---------|--------------------|--------------------|
| Connection | Required (`connect`) | Not required |
| Reliability | Guaranteed delivery | Best-effort |
| Order | Preserved | Not guaranteed |
| Speed | Slower | Faster |
| Use Case | HTTP, SSH, FTP | DNS, Video, Games |
| Header Size | 20–60 bytes | 8 bytes |

---

*Generated for Python 3.7+ · `import socket` · stdlib only*
