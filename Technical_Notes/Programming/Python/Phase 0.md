# OSED Prep — Phase 0: Python Fundamentals

## Overview

Phase 0 builds the minimum Python fluency required before OSED-specific scripting (byte handling, sockets, fuzzing, debugger automation) begins in Phase 1. OffSec's own prerequisite for EXP-301 is "familiarity with writing Python3 code" — not computer-science depth. This phase is deliberately scoped to procedural, script-level Python: no object-oriented design, no advanced language features.

**Target duration:** 1 week at 2–3 hours/day.

## 1. Environment & Execution

- Installing Python 3 and verifying the interpreter version
- Running a script from the command line (`python3 script.py`)
- Basic script structure and the `if __name__ == "__main__":` entry point
- Using a text editor / IDE (VS Code recommended) with a terminal side by side

## 2. Variables & Scalar Types

- Core types: `int`, `str`, `float`, `bool`
- Type conversion: `int()`, `str()`, `float()`, `hex()`, `bin()`
- Arithmetic operators: `+ - * / // % **`
- Comparison and boolean operators: `== != < > <= >=`, `and or not`
- Variable naming and dynamic typing basics

Note: this is the grammar layer only. `bytes`/`bytearray` — the type actually used to build exploit payloads — is deferred to Phase 1, where it belongs alongside `struct` and endianness.

## 3. Strings

- Indexing and slicing (`s[0:4]`, `s[::-1]`)
- Concatenation and repetition (`"A" * 100` — the core buffer-building idiom)
- `len()`
- f-string formatting (`f"offset: {x}"`)
- Common methods: `.strip()`, `.replace()`, `.split()`, `.join()`, `.upper()/.lower()`
- Recognizing where `str` stops being sufficient (anything sent over a socket or written as raw data needs `bytes`, covered in Phase 1)

## 4. Control Flow

- Conditionals: `if` / `elif` / `else`
- `for` loops with `range()`
- `while` loops (needed later for reading from a socket until a condition is met)
- `break` and `continue`
- Building a simple incrementing loop (the conceptual seed of a fuzzing loop: grow a buffer size each iteration)

## 5. Data Structures

- `list`: creation, indexing, `.append()`, iteration — used to accumulate payload pieces or scan results
- `dict`: key-value pairs, `.get()`, iteration — used for offset tables and configuration data
- `tuple`: recognizing the form, since `socket.connect((host, port))` requires one
- Deliberately skipped: `set` and comprehensions. They rarely appear in exploit scripts and are not worth the time here.

## 6. Functions

- `def`, parameters, `return`
- Default parameter values
- Returning multiple values (`return a, b`)
- Refactoring a script into functions (e.g., turning the control-flow fuzzing loop into a reusable function that accepts min/max/step)

## 7. File I/O

- `open()`, `.read()`, `.write()`, the `with` block
- Text mode vs. binary mode (`"rb"` / `"wb"`) — binary mode is required for shellcode files and any raw byte output
- Writing simple logs (useful for recording fuzzing results and crash offsets)

## 8. Error Handling

- `try` / `except` / `finally`
- `except Exception as e:` to catch and inspect errors
- Why this matters specifically: a target crashing mid-exploit must not also crash the exploit script — socket errors and connection resets need to be caught gracefully

## 9. Modules & Imports

- `import socket`, `import struct`, `import sys`, `import time`
- `sys.argv` for reading command-line parameters (target IP, port, offsets)
- Recognizing these four modules now so Phase 1 (bytes/sockets) starts from familiar syntax, not new syntax plus new concepts at once

