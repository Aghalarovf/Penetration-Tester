# OSED Prep — Phase 4: ctypes & Windows API Interaction

## 1. Loading and Calling DLLs

- The `ctypes` module and why it exists: calling functions in compiled Windows DLLs directly from Python, with no wrapper library needed
- Loading core DLLs: `ctypes.windll.kernel32`, `ctypes.windll.user32`, `ctypes.windll.ntdll`
- Calling a simple function with no arguments first, to confirm the call mechanism works (`kernel32.GetCurrentProcessId()`)
- A visible test case: calling `user32.MessageBoxW()` to pop up a message box — useful as an immediate, visual confirmation that the Python-to-WinAPI call succeeded

## 2. Argument and Return Types

- Why `ctypes` needs explicit type information to call a C function correctly: `argtypes` and `restype`
- Common `ctypes` types: `ctypes.c_int`, `ctypes.c_uint`, `ctypes.c_void_p`, `ctypes.c_char_p`, `ctypes.c_wchar_p`
- `ctypes.create_string_buffer()` — allocating a buffer in Python that a WinAPI call can write into
- Passing pointers: `ctypes.byref()` for output parameters (functions that write a result back through a pointer argument)

## 3. Memory-Related API Calls

- `VirtualAlloc` — allocating memory with a specified protection level (e.g., `PAGE_EXECUTE_READWRITE`), and why this matters conceptually for shellcode staging
- `VirtualProtect` — changing the protection of an existing memory region (the Python-level signature mirrors what later shows up as a ROP-chain target)
- `ReadProcessMemory` / `WriteProcessMemory` — signatures and parameter order; full practical use comes later once debugger tooling is introduced, but the calling pattern (handle, address, buffer, size, bytes-transferred pointer) is learned here
- `OpenProcess` / `CreateProcess` — obtaining or creating a process handle, a prerequisite for several of the above calls

## 4. Error Handling

- `ctypes.GetLastError()` — retrieving the Windows error code after a failed API call
- `ctypes.WinError()` — converting that code into a readable exception/message
- Why checking return values matters: a `ctypes` call that "succeeds" syntactically can still fail at the Windows API level (e.g., `VirtualAlloc` returning `NULL`)

## 5. Why This Matters for OSED

- Validating that a crafted shellcode stub behaves as expected in isolation, without needing a full exploit chain
- Scripting small Windows-API-driven checks (e.g., confirming a target process's memory protections) directly from Python
- Building intuition for the C function signatures that will be read constantly once C and Windows API work begins in the next phase — `ctypes` argument/return typing is a soft preview of C's own type system
