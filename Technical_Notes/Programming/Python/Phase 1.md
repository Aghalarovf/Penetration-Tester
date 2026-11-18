# OSED Prep — Phase 1: Binary Data & Byte Manipulation

## 1. `str` vs. `bytes`

- Why Python 3 keeps text (`str`) and binary data (`bytes`) as separate, non-interchangeable types
- Byte literals: `b"\x41\x42\x43"`, `b"ABC"`
- Why `"A" + b"A"` raises `TypeError`, and how to avoid the mix (encode/decode when unavoidable: `.encode()`, `.decode()`)
- `bytearray`: a mutable byte buffer, used when a payload needs to be modified in place after construction

## 2. Building Byte Buffers

- Concatenation and repetition: `b"A" * 100`, `payload += b"\x90\x90"`
- Slicing a byte buffer the same way as a string (`buf[0:4]`)
- `bytes.fromhex()` and `.hex()` — converting between a hex string and raw bytes
- `len()` on a byte buffer, and why exact buffer length matters (off-by-one offsets are the most common exploit-dev bug)

## 3. Number Systems & Bitwise Operations

- Converting between decimal, hexadecimal, and binary: `hex()`, `bin()`, `int(x, 16)`, `int(x, 2)`
- Bitwise operators: `&`, `|`, `^`, `<<`, `>>`, `~`
- Practical uses: masking an address (`addr & 0xFFFFFFFF`), isolating a byte from a DWORD, shifting values when building bitmasks

## 4. Endianness

- What little-endian means in memory layout, and why x86/x64 store multi-byte values with the least-significant byte first
- Why a return address must be written into a buffer in the correct byte order to work once popped into EIP/RIP
- Spotting endianness visually in a hex dump (an address like `0x401020` appears as `20 10 40 00`)

## 5. Manual Packing & the `struct` Module

- `int.to_bytes()` / `int.from_bytes()` — manual conversion, useful for understanding what `struct` does under the hood
- `struct.pack()` / `struct.unpack()`: format characters `<` (little-endian), `I` (4-byte unsigned), `Q` (8-byte unsigned)
- Packing a 32-bit address (`struct.pack("<I", addr)`) vs. a 64-bit address (`struct.pack("<Q", addr)`)
- Unpacking a received value back into an integer for inspection or validation

## 6. Payload Construction Patterns

- The standard exploit buffer skeleton: filler/junk bytes → offset to overwrite → controlled value (return address) → NOP sled → shellcode placeholder
- Assembling a payload as one concatenated `bytes` object from these labeled pieces, so each part stays independently editable
- Keeping payload-building code in a function that returns `bytes`, so it can be reused across fuzzing, offset-finding, and the final exploit script

## Practical Exercises

1. Write a script that takes a decimal address, converts it to a little-endian 4-byte sequence, and prints both its hex representation and raw bytes. Extend it to also produce the 8-byte (x64) version.
2. Write a function `build_payload(junk_len, ret_addr, nop_len, shellcode)` that returns a single `bytes` object assembled from those labeled pieces, using `struct.pack` for the return address.
