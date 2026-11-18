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


# OSED Phase 0 — Practical Tasks (10 Tasks)

Each task combines several Phase 0 topics at once. Ordered from easy to hard. Follow each task's "Requirements" section exactly — grading is based on those conditions.

---

## Task 1 — Number Analysis Script
**Topics:** variables/types, operators, conditionals, f-strings

Take one integer from the user. The program should:
- determine whether the number is even or odd
- determine whether the number is negative, zero, or positive
- print the number's hex and binary representation

**Requirements:**
- Output must be a single f-string printed on one line: `"Number: {x} | Type: even/odd | Sign: ... | Hex: ... | Bin: ..."`
- Use `int(input())`; no extra libraries

---

## Task 2 — String Manipulation Tool
**Topics:** strings, slicing, loops, conditionals

Take a word from the user. The program should:
- reverse the word
- check whether the word is a palindrome (case-insensitive)
- build a new word by repeating each letter 3 times (e.g., "ab" → "aaabbb")

**Requirements:**
- All three results printed on separate lines
- The palindrome check must be written manually with a `for` loop — use `[::-1]` slicing ONLY for the reversal, not for the palindrome comparison itself; write the comparison logic yourself with a loop

---

## Task 3 — Buffer Simulation
**Topics:** strings, loops, functions, f-strings

Write a function `build_buffer(char, count)` that returns a string with the given character repeated `count` times.

Then build a loop from 100 to 500 in steps of 50. At each step:
- call `build_buffer("A", size)`
- check the result's length (`len()`) against the expected size
- print `"Size: {X}, Built buffer length: {Y}, Matches: {True/False}"`

**Requirements:**
- `build_buffer` must be a separate `def`; inline repetition is not allowed
- The match check must use a plain `if`, not `assert`

---

## Task 4 — Configuration Table
**Topics:** dict, list, tuple, loops, functions

Build a `dict` with this structure (key: service name, value: a `(port, protocol)` tuple):
```
{"HTTP": (80, "TCP"), "DNS": (53, "UDP"), "FTP": (21, "TCP"), "HTTPS": (443, "TCP")}
```

Write a function `find_by_protocol(services, protocol)` that returns a `list` of all service names matching the given protocol.

The program should list all services, then use `find_by_protocol` to find and print only the "TCP" ones.

**Requirements:**
- Iterate the dict as `for key, value in services.items():`
- Inside `find_by_protocol`, access the tuple elements by index (`value[0]`, `value[1]`)

---

## Task 5 — File Logging
**Topics:** file I/O, loops, f-strings, functions

Write a function `log_attempt(filename, size, status)` that appends a line to the given file in the format `"[size={size}] status={status}"` (append mode, `"a"`).

Then build a loop from 1 to 10, calling `log_attempt` at each step (status: "OK" for even numbers, "FAIL" for odd numbers).

After the loop finishes, read the file back and count how many "OK" entries there are.

**Requirements:**
- File opening/closing must use a `with` block
- The counting logic must be its own function: `count_status(filename, status)`

---

## Task 6 — Error-Resilient Processor
**Topics:** try/except, functions, list, dict

Write a program that processes this list:
```
inputs = ["42", "abc", "17", "", "99", "xyz"]
```

Write a function `safe_convert(value)` that converts a string to `int`, returning `None` on failure (the program must never crash, regardless of input).

Process every element and collect the results into a `dict`: `{"succeeded": [...], "failed": [...]}`. Print the count and contents of each category at the end.

**Requirements:**
- `try`/`except` must live only inside `safe_convert`, not in the main loop
- The `except` block must be specific (`except ValueError:`) — do not use a bare `except Exception`

---

## Task 7 — Command-Line Configurator
**Topics:** sys.argv, conditionals, functions, file writing

The program should accept two command-line parameters: a target name and a repeat count (`python script.py "test" 5`).

- If the parameter count is wrong, show the correct usage and stop the program (`sys.exit()`)
- Converting the second parameter to `int` can fail — catch this with `try`/`except`
- Repeat the target name the given number of times, writing each numbered repetition to a file (`1: test`, `2: test`, ...)

**Requirements:**
- Use `sys.argv`; do NOT use `argparse` (not covered yet)
- The output filename is fixed: `"output.txt"`

---

## Task 8 — Multi-Dimensional Inventory System
**Topics:** list, dict, tuple, functions, loops, conditionals

Build this data structure — a list of dicts, each with `name`, `size`, and `tags` (a tuple) fields:
```
records = [
    {"name": "payload_a", "size": 128, "tags": ("test", "small")},
    {"name": "payload_b", "size": 512, "tags": ("prod", "large")},
    {"name": "payload_c", "size": 256, "tags": ("test", "medium")},
]
```

Write these functions:
- `filter_by_tag(records, tag)` — return all records containing the given tag
- `total_size(records)` — return the sum of all records' `size` values
- `largest_record(records)` — return the record with the largest `size`

The program should call all three functions and print the results in a formatted way.

**Requirements:**
- Each function must have a single responsibility (one function, one job)
- `largest_record` must use a manual comparison loop — the built-in `max()` function is NOT allowed

---

## Task 9 — Simulated "Fuzzing" Logger
**Topics:** loops, functions, file I/O, try/except, string/list

Simulate fuzzing logic without any real network:

Write a function `simulate_send(size)` that raises `Exception("crash simulated")` if `size > 350`, otherwise returns `"OK"`.

The main program builds a loop from 50 to 450 in steps of 25. At each step:
- call `simulate_send(size)`, catching the result with `try`/`except`
- log the result (success, or the "CRASH" message) to a file
- as SOON as a crash occurs, stop the loop (`break`) and separately print at which size it happened

**Requirements:**
- `simulate_send` must be written as if it were an importable, standalone module function — it must not depend on any global variable
- Logging must use a function similar to Task 5's `log_attempt` (rewrite it here)

---

## Task 10 — Capstone Project: Mini Audit Tool
**Topics:** ALL of the above (sys.argv, dict/list/tuple, functions, loops, try/except, file I/O, f-strings)

Take a filename from the command line (`python script.py "report.txt"`). The program works on this fixed data:
```
checks = [
    {"name": "port_scan", "result": "open", "severity": 3},
    {"name": "ssl_check", "result": "weak", "severity": 2},
    {"name": "auth_check", "result": "ok", "severity": 0},
    {"name": "header_check", "result": "missing", "severity": 1},
]
```

Required functions:
- `risk_score(checks)` — return the sum of all `severity` values
- `group_by_result(checks)` — return a `dict` grouped by `result` value (key: result, value: list of names)
- `write_report(filename, checks, score)` — write a formatted report to file: one line per check, ending with the total risk score

Program flow:
1. Read the filename from `sys.argv`; if not provided, default to `"audit_report.txt"`
2. Calculate the risk score and print it to the console
3. Print the `group_by_result` output to the console
4. Write the results to file using `write_report`
5. Wrap all file operations in `try`/`except` — if writing fails, show a clear error message without crashing

**Requirements:**
- All three functions must be independently testable (no global variables — everything flows through parameters and return values)
- The main logic must live inside an `if __name__ == "__main__":` block
- If you can explain every part of this code without help once it's done, Phase 0 is considered fully mastered

---

