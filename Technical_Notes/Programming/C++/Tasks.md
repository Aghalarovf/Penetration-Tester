# 🔐 500 Praktiki Tapşırıq: C++ ilə Kibertəhlükəsizlik
## Sıfırdan Xüsusi Protokol İmplementasiyasına

> Hər tapşırıq əvvəlki üzərində qurulur. Atlamayın.

---

# 🟢 MƏRHƏLƏ 1 — C++ Əsasları və Sistem Proqramlaması (1–50)

### Tapşırıq 1
**Kompilyasiya Pipeline Kəşfi**
`hello_security.cpp` yazın. İçərisində `std::cout << "Oxsium Security Framework v0.1" << std::endl;` olsun. Əl ilə bu ardıcıllıqla kompilyasiya edin: `g++ -E` (preprocessor), `g++ -S` (assembly), `g++ -c` (object), sonra link edin. Hər mərhələdə çıxış faylını `hexdump` ilə incələyin.

---

### Tapşırıq 2
**Bit Ölçüləri Xəritəsi**
`sizeof_map.cpp` yazın. `uint8_t`, `uint16_t`, `uint32_t`, `uint64_t`, `int8_t`, `int16_t`, `int32_t`, `int64_t`, `size_t`, `ptrdiff_t`, `uintptr_t`, `char`, `wchar_t`, `float`, `double`, `long double` tiplərinin hər birinin ölçüsünü, alignment-ini, minimum və maksimum dəyərini çap edin. Çıxışı cədvəl formatında formatlandırın.

---

### Tapşırıq 3
**Endianness Detektoru**
`endian_detect.cpp` yazın. Proqram işlədiyi sistemin little-endian yoxsa big-endian olduğunu `union` və ya pointer cast ilə müəyyən etsin. Sonra `uint16_t`, `uint32_t`, `uint64_t` üçün byte-swap funksiyaları yazın — heç bir standart kitabxana funksiyası istifadə etmədən, yalnız bit əməliyyatları ilə.

---

### Tapşırıq 4
**Hexdump Aləti**
`hexdump.cpp` yazın. Funksiya imzası: `void hexdump(const uint8_t* buf, size_t len, const char* label)`. Çıxış formatı: sol tərəfdə offset (8 hex rəqəm), ortada 16 byte hex (iki qrupda 8-lik), sağda ASCII göstəricisi (çap edilə bilməyən simvollar nöqtə ilə). Tam `xxd` çıxışını simulyasiya edin.

---

### Tapşırıq 5
**Pointer Arifmetikası Araşdırması**
`ptr_arithmetic.cpp` yazın. 256 baytlıq bir massiv yaradın, onu `0x41`-dən `0x80`-ə qədər doldurun. Yalnız pointer arifmetikası istifadə edərək: massivi tərsinə çevirin, hər ikinci baytı `0x00` ilə əvəz edin, sonra `uint32_t*` pointer kimi şərh edərək hər 4 baytı bir tam ədəd kimi oxuyun. Nəticəni hexdump edin.

---

### Tapşırıq 6
**Stack və Heap Yaddaş Araşdırması**
`memory_layout.cpp` yazın. Stack dəyişənlərinin, heap obyektlərinin, qlobal dəyişənlərin, `static` dəyişənlərin, funksiya göstəricilərinin ünvanlarını çap edin. Onları müqayisə edərək yaddaş xəritəsini (text, data, bss, stack, heap) vizual olaraq göstərin.

---

### Tapşırıq 7
**Bit Manipulyasiya Kitabxanası**
`bitlib.cpp` yazın. Bu funksiyaları implement edin: `set_bit(n, pos)`, `clear_bit(n, pos)`, `toggle_bit(n, pos)`, `test_bit(n, pos)`, `count_ones(n)` (popcount), `count_zeros(n)`, `highest_set_bit(n)`, `lowest_set_bit(n)`, `rotate_left32(n, k)`, `rotate_right32(n, k)`. Hər birini 20 test vektoru ilə sınayın.

---

### Tapşırıq 8
**Raw String Kitabxanası**
`strlib.cpp` yazın. Standart kitabxana istifadə etmədən implement edin: `my_strlen`, `my_strcpy`, `my_strncpy`, `my_strcmp`, `my_strncmp`, `my_strcat`, `my_strchr`, `my_strstr`, `my_memset`, `my_memcpy`, `my_memmove` (overlap-i idarə etsin). Hər birini edge case-lərlə sınayın.

---

### Tapşırıq 9
**Hex Çevirmə Kitabxanası**
`hex_convert.cpp` yazın. İmplementasiya edin: `bytes_to_hex(buf, len) -> string`, `hex_to_bytes(hex_str) -> vector<uint8_t>`, `hex_to_uint32(str) -> uint32_t`, `hex_to_uint64(str) -> uint64_t`. Böyük/kiçik hərf dəstəyi olsun. Etibarsız simvollar üçün müvafiq xəta qaytarsın.

---

### Tapşırıq 10
**Tam Ədəd Overflow Detektoru**
`overflow_detect.cpp` yazın. `safe_add`, `safe_sub`, `safe_mul` funksiyaları yazın — `int32_t`, `uint32_t`, `int64_t`, `uint64_t` üçün. Overflow baş verdikdə `false` qaytarsın. İmplementasiya `__builtin_add_overflow` istifadə etməsin — əl ilə yoxlasın. Kibertəhlükəsizlikdə integer overflow-un necə exploit olunduğunu şərh edən comment-lər əlavə edin.

---

### Tapşırıq 11
**Struct Yaddaş Layout Analizi**
`struct_layout.cpp` yazın. Müxtəlif padding-ə malik struct-lar yaradın. `offsetof()` istifadə edərək hər sahənin ofsetini çap edin. `__attribute__((packed))` ilə eyni struct-u yaradın, ölçü fərqini müqayisə edin. Şəbəkə protokol başlıqları üçün packed struct-ların niyə kritik olduğunu izah edin.

---

### Tapşırıq 12
**Yaddaş Zəifliyi Demonstrasiyası (Nəzarətli)**
`vuln_demo.cpp` yazın. Üç klassik zəifliyi ayrı funksiyalarda göstərin: stack buffer overflow (əl ilə), use-after-free (əl ilə), double-free (əl ilə). HƏR BİRİNİ `#ifdef SHOW_VULN` ilə qoruyun ki, təsadüfən işlənməsin. Hər zəifliyin necə exploit oluna biləcəyini şərh edin.

---

### Tapşırıq 13
**RAII Guard Kitabxanası**
`raii_guards.cpp` yazın. Bunları implement edin: `FileGuard` (faylı avtomatik bağlayan), `SocketGuard` (socket-i bağlayan), `MemoryGuard` (yaddaşı boşaldan), `LockGuard` (mutex-i açan). Hamısı copy-edilə bilməz, yalnız move edilə bilər. Destructor-lar exception atmamalıdır.

---

### Tapşırıq 14
**Custom Smart Pointer**
`smart_ptr.cpp` yazın. `UniquePtr<T>` implement edin — `std::unique_ptr` kimi amma sıfırdan. Daxil olsun: `operator*`, `operator->`, `operator bool`, `get()`, `release()`, `reset()`, move constructor, move assignment. Copy constructor və copy assignment `= delete` edilsin. 15 test vektoru ilə sınayın.

---

### Tapşırıq 15
**Compile-Time Sabitlər**
`constexpr_crypto.cpp` yazın. `constexpr` istifadə edərək compile-time-da hesablayın: 2-nin ilk 64 qüvvəti, Fibonacci-nin ilk 50 ədədi, ilk 100 sadə ədəd (Sieve of Eratosthenes kimi), SHA-256 sabit cədvəlinin ilk 64 elementi (kubik köklər). Hamısı compile-time-da hesablanır, runtime-da heç nə yoxdur.

---

### Tapşırıq 16
**Error Handling Sistemi**
`error_system.cpp` yazın. `Result<T, E>` template class-ı implement edin. Daxil olsun: `Ok(value)`, `Err(error)`, `is_ok()`, `is_err()`, `unwrap()` (panic edir), `unwrap_or(default)`, `map(fn)`, `and_then(fn)`. Sonra `CryptoError` enum class-ı yaradın: `InvalidKey`, `InvalidInput`, `BufferTooSmall`, `AuthenticationFailed`, `NotImplemented`.

---

### Tapşırıq 17
**Platform Abstraksiya Qatı**
`platform.h` yazın. Linux, macOS, Windows-u dəstəkləsin. Xüsusi macros: `PLATFORM_LINUX`, `PLATFORM_MACOS`, `PLATFORM_WINDOWS`, `PLATFORM_64BIT`, `PLATFORM_LITTLE_ENDIAN`. Funksiyalar: `get_cpu_count()`, `get_page_size()`, `get_random_bytes(buf, len)` (platformaya görə `/dev/urandom` vs `CryptGenRandom`), `get_monotonic_time_ns()`.

---

### Tapşırıq 18
**Yaddaş Sıfırlama Funksiyası**
`secure_zero.cpp` yazın. `secure_memzero(void* ptr, size_t len)` implement edin — kompilator tərəfindən optimize edilib silinməyəcək şəkildə. Üç üsuldan birini istifadə edin: `volatile` pointer, `memset_s` (C11), `SecureZeroMemory` (Windows). Sonra kompilatorun adi `memset`-i optimize edib silmədiyini assembly-də yoxlayın. Kriptografiyada bu niyə kritikdir?

---

### Tapşırıq 19
**Proses Mühit Analizi**
`proc_info.cpp` yazın. Çap etsin: proses ID-si, valideyn proses ID-si, istifadəçi ID-si, qrup ID-si, işçi qovluq, mühit dəyişənləri (filtr edərək), komanda sətri arqumentləri, açıq fayl deskriptorları sayı (`/proc/self/fd` ilə Linux-da). Cross-platform iş görməsi üçün conditional compilation istifadə edin.

---

### Tapşırıq 20
**Vaxt Ölçmə Kitabxanası**
`timer.cpp` yazın. `HighResTimer` class-ı implement edin: `start()`, `stop()`, `elapsed_ns()`, `elapsed_us()`, `elapsed_ms()`. Sonra `benchmark(fn, iterations)` funksiyası yazın: funksiyanı N dəfə işlədib nanosaniyə başına ortalama, minimum, maksimum, standart sapma çap etsin. Bunu sıfırdan implement edin.

---

### Tapşırıq 21
**Dinamik Massiv (Vector) Sıfırdan**
`my_vector.cpp` yazın. `MyVector<T>` implement edin. Daxil olsun: `push_back`, `pop_back`, `operator[]`, `at()` (bounds check ilə), `size()`, `capacity()`, `resize()`, `reserve()`, `clear()`, `begin()`, `end()`. Amortized O(1) push_back üçün 2x böyütmə strategiyası istifadə edin. `T` non-trivial destructor-a malik ola bilər.

---

### Tapşırıq 22
**Hash Cədvəli Sıfırdan**
`my_hashmap.cpp` yazın. `MyHashMap<K, V>` implement edin. Open addressing + linear probing istifadə edin. Daxil olsun: `insert`, `find`, `erase`, `operator[]`, `contains`, `size`, `load_factor`. Load factor 0.75-dən keçdikdə avtomatik resize edin. String açarlar üçün FNV-1a hash funksiyası istifadə edin.

---

### Tapşırıq 23
**Linked List Zəiflikləri**
`list_vulns.cpp` yazın. Doubly linked list implement edin. Sonra aşağıdakıları demonstrasiya edin: `use-after-free` hücumu (pointer-ı free-dən sonra istifadə), `heap spray` konsepti (eyni ölçülü çoxlu allokasiya), `heap overflow` (node içindəki buffer-a yazma). Valgrind ilə hər birini aşkarlayın.

---

### Tapşırıq 24
**Funksiya Pointer Cədvəli**
`func_table.cpp` yazın. Funksiya pointer cədvəli (virtual dispatch-ın manual simulyasiyası) implement edin. `CryptoAlgorithm` struct-u yaradın: `encrypt_fn`, `decrypt_fn`, `key_schedule_fn` funksiya pointerlər ilə. XOR, Caesar cipher, ROT13 üçün konkret implementasiyalar yazın. `vtable hijacking` hücumunu şərh edən comment-lər əlavə edin.

---

### Tapşırıq 25
**Stack Frames Analizi**
`stack_frames.cpp` yazın. Rekursiv funksiya yazın. `__builtin_frame_address(0)`, `__builtin_return_address(0)` istifadə edərək stack frames-i iz edin. Hər frame-in ünvanını, return address-ini, frame pointer-ini çap edin. Stack-in necə böyüdüyünü (yuxarıdan aşağı) vizual göstərin. Stack overflow-un nə zaman baş verdiyini müəyyən etmək üçün rekursiya limitini tapın.

---

### Tapşırıq 26
**Namespace və Modul Sistemi**
`oxsium_core.hpp` yazın. `oxsium::crypto::symmetric`, `oxsium::crypto::hash`, `oxsium::network::tcp`, `oxsium::network::tls`, `oxsium::protocol::ldap` namespace-lərini yaradın. Hər namespace-də placeholder funksiyalar olsun. `inline namespace` istifadə edərək versiyalanma göstərin: `oxsium::v1` vs `oxsium::v2`.

---

### Tapşırıq 27
**Makros vs constexpr Müqayisəsi**
`macro_vs_constexpr.cpp` yazın. Eyni funksiyanı həm makro həm `constexpr` kimi implement edin: `MAX(a,b)`, `MIN(a,b)`, `ALIGN_UP(x, align)`, `ALIGN_DOWN(x, align)`, `IS_POWER_OF_2(n)`, `ARRAY_SIZE(arr)`. Makroların double-evaluation bug-unu, scope problemlərini, type-unsafety-liyini göstərin.

---

### Tapşırıq 28
**Fayl I/O: Binary Oxuma/Yazma**
`bin_io.cpp` yazın. Binary formatda fayllar yaradın: magic number (4 bayt), versiyon (2 bayt), payload uzunluğu (4 bayt, big-endian), payload, CRC32 checksum (son 4 bayt). Faylı oxuyun, hər sahəni yoxlayın. Dəyişdirilmiş faylı aşkarlayın. Bu, sadə fayl formatının necə strukturlaşdırıldığını öyrədir.

---

### Tapşırıq 29
**Environment Variable Injection**
`env_inject.cpp` yazın. `getenv`, `setenv`, `unsetenv` istifadə edin. `PATH` dəyişəninin necə manipulyasiya oluna biləcəyini göstərin. Xüsusi bir proqram üçün güvənli mühit əldə etmək üçün funksiya yazın: bütün potensial təhlükəli mühit dəyişənlərini sil. Privilege escalation üçün mühit dəyişənlərinin necə istifadə edilə biləcəyini şərh edin.

---

### Tapşırıq 30
**Sadə Test Framework**
`test_framework.cpp` yazın. Heç bir kənar kitabxana olmadan: `TEST(name, body)` makrosu, `EXPECT_EQ(a, b)`, `EXPECT_NE(a, b)`, `EXPECT_TRUE(cond)`, `EXPECT_FALSE(cond)`, `EXPECT_THROW(expr, exception_type)`, `EXPECT_NO_THROW(expr)`. Rəngli terminal çıxışı: yaşıl PASS, qırmızı FAIL. Ümumi statistika.

---

### Tapşırıq 31
**Bit Field Protokol Başlığı**
`protocol_header.cpp` yazın. IPv4 başlığını bit field struct kimi modelləşdirin: Version (4 bit), IHL (4 bit), DSCP (6 bit), ECN (2 bit), Total Length (16 bit), vb. `packed` struct ilə networkdən gələn raw bytes-ı parse edin. Hər sahəni hexdump ilə göstərin.

---

### Tapşırıq 32
**Rekursiv Descent Parser**
`expr_parser.cpp` yazın. Sadə ifadə parser-ı implement edin: `+`, `-`, `*`, `/`, mötərizə dəstəyi. Daha sonra bu parser-ı LDAP filter-larını parse etmək üçün istifadə edəcəksiniz. Grammar:
```
expr   = term (('+' | '-') term)*
term   = factor (('*' | '/') factor)*
factor = NUMBER | '(' expr ')'
```

---

### Tapşırıq 33
**String View Implementasiyası**
`string_view.cpp` yazın. `MyStringView` implement edin — `std::string_view` kimi amma sıfırdan. Daxil olsun: `data()`, `size()`, `empty()`, `operator[]`, `substr()`, `find()`, `starts_with()`, `ends_with()`, `trim_left()`, `trim_right()`. Heç bir allokasiya etməsin. Protokol parsing üçün niyə kritik olduğunu izah edin.

---

### Tapşırıq 34
**Command Line Parser**
`cli_parser.cpp` yazın. Security tool üçün CLI parser implement edin. Dəstəkləsin: `-h/--help`, `-v/--verbose`, `-o/--output FILE`, `-p/--port PORT`, `-t/--timeout SECONDS`, birlikdə istifadə edilə bilən və ya xarici olan flaglar. Etibarsız arqumentlər üçün xəta mesajları. Heç bir kənar kitabxana yox.

---

### Tapşırıq 35
**Shared Library Yaratmaq**
`libsecutils.cpp` yazın. Paylaşılan kitabxana kimi kompilyasiya edin (`-shared -fPIC`). Public API header (`secutils.h`) yazın. Sadə bir C proqramında istifadə edin. `nm -D libsecutils.so` ilə export edilmiş simvolları yoxlayın. `LD_PRELOAD` mexanizmini şərh edin.

---

### Tapşırıq 36
**Inline Assembly**
`asm_basics.cpp` yazın. x86-64 inline assembly istifadə edərək implement edin: `cpuid` ilə CPU xüsusiyyətlərini oxuma (AES-NI dəstəyi varmı?), `rdtsc` ilə yüksək dəqiqlikli vaxt oxuma, `xchg` ilə atomic swap, `bswap` ilə 32-bit byte swap. Hər birinin C++ ekvivalenti ilə müqayisə edin.

---

### Tapşırıq 37
**Object File Analizi**
Tapşırıq 35-dəki `.o` faylını analiz edin. İstifadə edin: `objdump -d` (disassembly), `nm` (symbol table), `readelf -S` (section headers), `strings` (embedded strings), `file` (format). Bu məlumatı bir security researcher kimi necə istifadə edərdiniz? Reverse engineering üçün əhəmiyyətini şərh edin.

---

### Tapşırıq 38
**Sadə Allocator**
`bump_allocator.cpp` yazın. Bump (arena) allocator implement edin: böyük bir buffer saxlayır, `allocate(size, align)` sadəcə pointerı irəli aparır, `reset()` hamısını birdən boşaldır. `free()` yoxdur. Sonra `PoolAllocator<T, N>`: N sayda T obyekti üçün pool, O(1) alloc/free. Network paket parsing üçün niyə müvafiqdir?

---

### Tapşırıq 39
**Sistem Çağırışları Birbaşa**
`direct_syscall.cpp` yazın. Libc istifadə etmədən birbaşa syscall-lar edin: `write` (stdout-a yazmaq), `read` (stdin-dən oxumaq), `open`/`close` (fayl açmaq), `getpid`. Linux-da `syscall()` funksiyası ilə. Antivirus proqramlarının niyə libc hooking-ini izlədiyini şərh edin.

---

### Tapşırıq 40
**Fayl Deskriptor Cədvəli**
`fd_table.cpp` yazın. `/proc/self/fd` (Linux) yaxud `fcntl` istifadə edərək bütün açıq fayl deskriptorlarını siyahılayın. Hər birinin tipini (file, pipe, socket, device) müəyyən edin. Proqramın başlanğıcında stdout, stderr, stdin-dən əlavə hansı fd-ların açıq olduğunu yoxlayın. FD leak-inin niyə təhlükəsizlik riski olduğunu şərh edin.

---

### Tapşırıq 41
**Mutex və Condition Variable Sıfırdan**
`sync_primitives.cpp` yazın. `pthread_mutex_t` üzərindən wrapper (C++ style): `Mutex`, `LockGuard`, `ConditionVariable`. Sonra bunlarla `BoundedQueue<T, N>` implement edin — N-lik məhdud sabaşlı növbə. Producer threads dolduruyor, consumer threads boşaldır, tam sınaq proqramı ilə.

---

### Tapşırıq 42
**Atomic Counter və Spinlock**
`atomics.cpp` yazın. `std::atomic<uint64_t>` istifadə edərək: lock-free counter, lock-free stack (Treiber stack), CAS (Compare-And-Swap) əsaslı spinlock. 8 thread paralel şəkildə counter-ı artırsın. ThreadSanitizer ilə data race olmadığını yoxlayın.

---

### Tapşırıq 43
**Shared Memory IPC**
`shm_ipc.cpp` yazın. İki proses arasında paylaşılan yaddaş istifadə edərək kommunikasiya: `shmget`/`shmat` (POSIX) yaxud `shm_open`/`mmap`. Bir proses mesaj yazır, digəri oxuyur. Semaphore ilə sinxronizasiya. Bu mexanizmin təhlükəsizlik implikasiyalarını şərh edin.

---

### Tapşırıq 44
**Proses Fork və Exec**
`fork_exec.cpp` yazın. `fork()`, `exec()`, `wait()` istifadə edərək: başqa bir proqramı child process kimi işlədən wrapper, stdout/stderr-ini pipe ilə tutan, timeout-dan sonra kill edən funksiya yazın. `ptrace` sistemini qısaca izah edin.

---

### Tapşırıq 45
**Signal Handler**
`signal_handler.cpp` yazın. `SIGINT`, `SIGTERM`, `SIGSEGV`, `SIGALRM` üçün signal handler-lar yazın. `SIGSEGV` handler-da crash zamanı stack trace çap etsin (`backtrace` funksiyası ilə). Alarm-based timeout implement edin. Reentrancy problemlərini şərh edin.

---

### Tapşırıq 46
**Memory Mapping**
`mmap_demo.cpp` yazın. `mmap` istifadə edərək: böyük faylı yaddaşa map etmək, map edilmiş yaddaşda axtarış (`memmem` əvəzinə sıfırdan), `mprotect` ilə sahəni read-only etmək, `msync` ilə dəyişiklikləri diska yazmaq. Fayl-əsaslı I/O ilə performans müqayisəsi.

---

### Tapşırıq 47
**Yaddaş Korrupsiyası Aşkarlaması**
`canary.cpp` yazın. Stack canary mexanizmini simulate edin: hər funksiyada müvafiq yerdə magic value yerləşdirin, funksiya çıxışında yoxlayın. Həmçinin heap canary: hər allokasiyadan əvvəl/sonra magic bytes əlavə edin, free zamanı yoxlayın. AddressSanitizer-in bunu necə etdiyini şərh edin.

---

### Tapşırıq 48
**ELF Fayl Parser**
`elf_parser.cpp` yazın. ELF header-ı (e_ident, e_type, e_machine, e_entry, e_phoff, e_shoff) oxuyun. Program headers-i parse edin. Section headers-i parse edin. Heç bir xarici kitabxana istifadə etmədən `/usr/bin/ls`-in ELF strukturunu göstərin. Bu, malware analizi üçün əsas bacarıqdır.

---

### Tapşırıq 49
**Exception Safety Zəmanətləri**
`exception_safety.cpp` yazın. Üç funksiya yazın — eyni funksiyanı implement edən, amma fərqli exception safety zəmanətləri ilə: basic guarantee, strong guarantee (commit-or-rollback), nothrow guarantee. `std::vector::push_back` üçün strong exception safety-nin necə implement olunduğunu izah edin.

---

### Tapşırıq 50
**Compilation Database**
`compile_commands.json` yazın (əl ilə). Bütün yazdığınız faylları əhatə etsin. `clang-tidy -p compile_commands.json *.cpp` işlədin. `clangd` ilə IDE dəstəyi konfiqurasiya edin. `bear` aləti ilə avtomatik generate etməyi öyrənin.

---

# 🟡 MƏRHƏLƏ 2 — Şəbəkə Proqramlaması Əsasları (51–100)

### Tapşırıq 51
**Raw TCP Client Sıfırdan**
`tcp_client.cpp` yazın. `socket()`, `connect()`, `send()`, `recv()`, `close()` istifadə edərək TCP client implement edin. `google.com:80`-ə qoşulun, `GET / HTTP/1.0\r\n\r\n` göndərin, cavabı oxuyun. Heç bir HTTP kitabxanası yox. Əlçatılan bytes-ı hexdump edin.

---

### Tapşırıq 52
**Raw TCP Server**
`tcp_server.cpp` yazın. `socket()`, `bind()`, `listen()`, `accept()` ilə sadə TCP server: bir anda bir client qəbul edir, aldığı hər şeyi geri göndərir (echo server), client `quit` yazanda bağlayır. Hər addımda tam error checking.

---

### Tapşırıq 53
**Socket Ünvanı İşlənməsi**
`addr_utils.cpp` yazın. `getaddrinfo` ilə hostname-i IPv4 və IPv6 ünvanlarına çevirin. `inet_pton`/`inet_ntop` istifadə edin. `sockaddr_storage` ilə hər iki ailəni idarə edin. Port validasiyası (1-65535). `getnameinfo` ilə reverse lookup. IP-dən ölkəni müəyyən etmək üçün GeoIP konseptini şərh edin.

---

### Tapşırıq 54
**Tam send/recv Loop**
`reliable_io.cpp` yazın. `send_all(fd, buf, len)` — hər byte göndərilənə qədər loop edir, EINTR-i idarə edir. `recv_exactly(fd, buf, len)` — tam olaraq N byte alır, EINTR-i idarə edir. `recv_line(fd, buf, max_len)` — `\n`-ə qədər oxuyur. Hər birini test edin.

---

### Tapşırıq 55
**Non-blocking Socket**
`nonblocking.cpp` yazın. `fcntl(fd, F_SETFL, O_NONBLOCK)` ilə non-blocking socket yaradın. Non-blocking `connect()` implement edin (EINPROGRESS-i idarə edin). `send()`/`recv()` EAGAIN qaytardıqda nə etmək lazımdır. Timeout əsaslı polling: `poll()` ilə timeout.

---

### Tapşırıq 56
**poll() əsaslı Multiplexer**
`poll_server.cpp` yazın. `poll()` ilə çoxlu client-ə xidmət edən server: eyni anda 100 client dəstəkləsin. Hər client öz state-ini saxlasın. Client `hello`yazanda `world` cavabı alsın, `quit` yazanda bağlansın. CPU istifadəsini ölçün.

---

### Tapşırıq 57
**epoll Server (Linux)**
`epoll_server.cpp` yazın. `epoll_create1`, `epoll_ctl`, `epoll_wait` ilə yüksək performanslı server. Level-triggered vs edge-triggered fərqini izah edin. `EPOLLIN`, `EPOLLOUT`, `EPOLLERR`, `EPOLLHUP` hadisələrini idarə edin. 1000 eyni vaxtlı client ilə sınayın.

---

### Tapşırıq 58
**UDP Socket Proqramlaması**
`udp_tools.cpp` yazın. UDP client: `sendto` ilə paket göndər. UDP server: `recvfrom` ilə al, sender-ə cavab ver. UDP-nin niyə "connectionless" olduğunu praktiki olaraq göstərin (paket itkisi, sıra dəyişikliyi). DNS sorğusu üçün bir UDP client yazın (port 53-ə manual DNS sorğusu).

---

### Tapşırıq 59
**Raw Socket (ICMP Ping)**
`raw_ping.cpp` yazın. `AF_INET, SOCK_RAW, IPPROTO_ICMP` ilə raw socket yaradın (root lazımdır). ICMP Echo Request paketi əl ilə qurun. IP checksum hesablayın. ICMP checksum hesablayın. Cavabı parse edin. Round-trip time ölçün. `ping` əmrinin necə işlədiyini anlamaq.

---

### Tapşırıq 60
**IP Header Qurmaq**
`ip_builder.cpp` yazın. IPv4 header-ı əl ilə qurun: Version, IHL, TOS, Total Length, ID, Flags, Fragment Offset, TTL, Protocol, Checksum, Source IP, Dest IP. Header checksum-u RFC 791-ə görə hesablayın. `IP_HDRINCL` socket seçimi ilə göndərin.

---

### Tapşırıq 61
**TCP Handshake Analizi**
`tcp_handshake.cpp` yazın. `tcpdump`-un çıxışını simulyasiya edin: raw socket ilə paket tutan proqram yazın (promisc mode). Hər paketin: Ethernet, IP, TCP başlıqlarını parse edin. TCP flags-ı dekodlaşdırın (SYN, ACK, FIN, RST, PSH, URG). Connection state machine-i implement edin.

---

### Tapşırıq 62
**Port Scanner**
`port_scanner.cpp` yazın. TCP connect scanner: IP ünvanında 1-1024 portları yoxlayın. Non-blocking connect + poll/select ilə paralel yoxlama (100 eyni anda). Timeout dəstəyi. Açıq/bağlı/filtreli portları fərqləndirin. `nmap`-ın sadə versiyasını implement edin.

---

### Tapşırıq 63
**Şəbəkə Araçları Kitabxanası**
`netutils.cpp` yazın. `get_local_interfaces()` — sistem interfeyslərini siyahıla. `get_public_ip()` — HTTPəsaslı sınaq (example.com:80). `is_port_open(host, port, timeout_ms)`. `resolve_all(hostname)` — bütün A/AAAA record-ları qaytarsın. `traceroute_hop(host, ttl)` — bir hop üçün TTL=1 paketi göndər.

---

### Tapşırıq 64
**Protokol Buffer Yazma**
`proto_buffer.cpp` yazın. Şəbəkə paketlərini qurmaq üçün `PacketBuilder` class-ı implement edin: `write_u8(v)`, `write_u16_be(v)`, `write_u32_be(v)`, `write_u64_be(v)`, `write_bytes(buf, len)`, `write_string(s)`, `get_bytes()`. `PacketReader` da yazın — eyni operasiyalar amma oxuma üçün, bounds checking ilə.

---

### Tapşırıq 65
**Şəbəkə Timeout Kitabxanası**
`net_timeout.cpp` yazın. `connect_with_timeout(fd, addr, timeout_ms)` implement edin. `send_with_timeout(fd, buf, len, timeout_ms)`. `recv_with_timeout(fd, buf, len, timeout_ms)`. SO_RCVTIMEO/SO_SNDTIMEO ilə fərqini şərh edin. Hər biri `ETIMEDOUT` error-u qaytarmalıdır.

---

### Tapşırıq 66
**DNS Sorğusu Sıfırdan**
`dns_query.cpp` yazın. DNS query paketini əl ilə qurun: Header (ID, flags, QDCOUNT), Question section (qname, qtype=A, qclass=IN). UDP ilə 8.8.8.8:53-ə göndərin. Cavabı parse edin: Answer section-dan IP ünvanlarını çıxarın. `google.com` üçün sınayın.

---

### Tapşırıq 67
**HTTP/1.0 Client Sıfırdan**
`http_client.cpp` yazın. Heç bir HTTP kitabxanası olmadan: `GET` request-i göndər, response header-larını parse et (Status-Line, Content-Type, Content-Length), body-ni oku. HTTP redirect-ləri idarə et (301, 302). HTTPS üçün nə lazımdır?

---

### Tapşırıq 68
**Bannər Oxuma (Banner Grabbing)**
`banner_grab.cpp` yazın. Verilmiş host:port-a qoşulun. Bir saniyə gözləyin. Gələn hər şeyi oxuyun. Bunu SSH (port 22), SMTP (port 25), FTP (port 21), HTTP (port 80), LDAP (port 389) üçün sınayın. Bannər məlumatından xidməti və versiyasını identifikasiya etmək üçün heuristics yazın.

---

### Tapşırıq 69
**TCP Keepalive İmplementasiyası**
`tcp_keepalive.cpp` yazın. `SO_KEEPALIVE`, `TCP_KEEPIDLE`, `TCP_KEEPINTVL`, `TCP_KEEPCNT` socket seçimlərini konfiqurasiya edin. "Half-open" TCP bağlantılarını aşkarlayın. `heartbeat` protokolu implement edin: hər 30 saniyədən bir `PING` göndər, `PONG` gözlə, cavab gəlməsə yenidən qoşul.

---

### Tapşırıq 70
**Platform Abstraksion: Windows + Linux Socket**
`cross_socket.cpp` yazın. Eyni kod həm Linux-da həm Windows-da kompilyasiya olunur. Fərqlər: `SOCKET` tipi, `WSAStartup`/`WSACleanup`, `closesocket` vs `close`, `WSAGetLastError` vs `errno`. `Socket` wrapper class-ı yazın ki, fərqlər gizlənsin.

---

### Tapşırıq 71
**Network Byte Order Kitabxanası**
`netorder.cpp` yazın. Bütün `hton*`/`ntoh*` funksiyalarını sıfırdan implement edin: `my_htons`, `my_htonl`, `my_htonll`, `my_ntohs`, `my_ntohl`, `my_ntohll`. Sonra generic template: `to_network_order<T>(val)`, `from_network_order<T>(val)`. Assembly-də sonucun doğru olduğunu yoxlayın.

---

### Tapşırıq 72
**Prosesin Şəbəkə Bağlantılarını Siyahılamaq**
`netstat_clone.cpp` yazın. `/proc/net/tcp` və `/proc/net/tcp6` (Linux) oxuyaraq cari prosesin TCP bağlantılarını siyahılayın. Hər bağlantı üçün: local addr:port, remote addr:port, state (ESTABLISHED, LISTEN, TIME_WAIT, vb.), inode, proses ID-si. `netstat -an` çıxışını simulyasiya edin.

---

### Tapşırıq 73
**Packet Capture (libpcap olmadan)**
`raw_capture.cpp` yazın. `AF_PACKET, SOCK_RAW, htons(ETH_P_ALL)` ilə (Linux, root) bütün şəbəkə trafikini tutun. Ethernet, IPv4, IPv6, TCP, UDP başlıqlarını parse edin. Paket sayını, məcmu byte-ı, ən aktiv host cütlüklərini çap edin. Bu, Wireshark-ın necə işlədiyinin əsasıdır.

---

### Tapşırıq 74
**ARP Cache Zəhərlənməsi Demonstrasiyası (Nəzarətli)**
`arp_concepts.cpp` yazın. ARP paket strukturunu (hardware type, protocol type, operation, sender MAC, sender IP, target MAC, target IP) modelləşdirin. ARP request və reply paketlərini əl ilə qurun (göndərməyin — yalnız qurun və hexdump edin). ARP cache poisoning hücumunun necə işlədiyini şərh edin.

---

### Tapşırıq 75
**Şəbəkə Proqramlama: Əsas Təhlükəsizlik Yoxlamaları**
`net_security_checks.cpp` yazın. Aşağıdakı yoxlamaları implement edin: IP ünvanının private range-də olub olmadığını yoxla (RFC 1918), loopback-i yoxla, multicast-ı yoxla, port-un məhdudlaşdırılmış diapazonlarda olub olmadığını yoxla (< 1024 root tələb edir), URL-i parse et və komponent validasiya et.

---

### Tapşırıq 76
**Sadə SOCKS5 Proxy Client**
`socks5_client.cpp` yazın. SOCKS5 protokolunu implement edin (RFC 1928): greeting, authentication, request (CONNECT metodu), host:port-a tunnel yaratmaq. Bağlantıdan sonra bütün trafiki proxy-dən keçirin. `127.0.0.1:1080`-dəki lokal SOCKS5 proxy-si vasitəsilə `example.com:80`-ə qoşulun.

---

### Tapşırıq 77
**Şəbəkə Fuzzer**
`net_fuzzer.cpp` yazın. Protokol fuzzer-i implement edin: random bytes göndərin, truncated messages göndərin, çox uzun fields göndərin, sıfır uzunluqlu messages göndərin, geçerli amma unexpected sequence-lər göndərin. Hər test case-i log-layın. Server-in crash etdiyini aşkarlayın. Bu, protocol implementation-ı test etməyin əsas üsuludur.

---

### Tapşırıq 78
**Connection State Machine**
`conn_state.cpp` yazın. TCP connection state machine-i implement edin: CLOSED, LISTEN, SYN_SENT, SYN_RECEIVED, ESTABLISHED, FIN_WAIT_1, FIN_WAIT_2, TIME_WAIT, CLOSE_WAIT, LAST_ACK. Hər state transition-ı log-layın. Bu state machine-i bir protokol parser-a inteqrasiya edin.

---

### Tapşırıq 79
**Sadə Load Balancer**
`load_balancer.cpp` yazın. Round-robin load balancer implement edin: client bağlantısını N backend server-dən birinə yönləndir. Health checking: hər 5 saniyədə bir sağlamlıq yoxla, sağlam olmayan server-i çıxar. Connection tracking: neçə bağlantı hər backend-ə gedir.

---

### Tapşırıq 80
**Şəbəkə Kitabxanası v1.0**
`oxsium_net_v1.hpp` yazın. Bütün indiyə qədər yazdıqlarınızı bir clean API-yə birləşdirin:
```cpp
namespace oxsium::net {
  class TcpSocket;
  class UdpSocket;
  class Address;
  class Poller;
  class Listener;
}
```
Tam dokumentasiya (Doxygen style), unit testlər.

---

### Tapşırıq 81
**HTTP Server Sıfırdan**
`http_server.cpp` yazın. Heç bir HTTP kitabxanası olmadan sadə HTTP/1.1 server: GET, POST dəstəyi, request parser (method, path, headers, body), response builder, static file serving, query string parsing. `/health` endpoint-i `{"status":"ok"}` qaytarsın.

---

### Tapşırıq 82
**WebSocket Handshake**
`websocket_handshake.cpp` yazın. WebSocket upgrade handshake-i implement edin (RFC 6455): HTTP Upgrade request-ini parse edin, `Sec-WebSocket-Key`-i oxuyun, `SHA-1(key + magic) |> base64` hesablayın, HTTP 101 response-u göndərin. Şifrelənmiş WebSocket frame-i göndərin.

---

### Tapşırıq 83
**Reverse Shell Mexanizmi (Nəzarətli)**
`rev_shell_concepts.cpp` yazın. Reverse shell-in konseptual arxitekturasını göstərin: client outbound TCP bağlantısı açır, server komandaları göndərir, client işlədir, nəticəni qaytarır. Yalnız lokal loopback üzərində sınayın. Bu, penetration testing-in əsas konseptidir. Real deployment etməyin.

---

### Tapşırıq 84
**Network Namespace (Linux)**
`net_namespace.cpp` yazın. `clone()` ilə yeni network namespace yaradın. İçəridə yeni virtual interface qurun. Namespace-dən kənara paket göndərin. Docker-in network izolasiyasını bu şəkildə implement etdiyini göstərin. Container şəbəkəsinin əsaslarını anlayın.

---

### Tapşırıq 85
**Protokol Fuzzing Framework**
`fuzzing_framework.cpp` yazın. Strukturlu fuzzer: başlanğıc nümunəsi al, bit-flip et, byte dəyişdir, field-ləri şişirt/sıfırla, çarpaz nümunə yarat. Crash-ları `/tmp/crashes/` qovluğuna saxla. İşləmə sürəti (executions/second) ölç. AFL ilhamına əsaslı amma sıfırdan.

---

### Tapşırıq 86
**TLS Record Layer (Strukturu)**
`tls_record.cpp` yazın. TLS record-u parse edin (göndərməyin): Content Type (1 byte), Version (2 byte), Length (2 byte), Fragment. Content type-ları: 20=ChangeCipherSpec, 21=Alert, 22=Handshake, 23=ApplicationData. Bir TLS sessiondan kaydedilmiş hex dump-u parse edin.

---

### Tapşırıq 87
**X.509 Sertifikat Parser (DER)**
`cert_parser.cpp` yazın. DER encoded X.509 sertifikatını parse edin. Çıxarın: Subject CN, Issuer CN, Validity (notBefore, notAfter), Subject Alternative Names, Public Key algorithm, Serial number. `/etc/ssl/certs/`-dən bir sertifikat istifadə edin. OpenSSL istifadə etmədən.

---

### Tapşırıq 88
**SSL/TLS Fingerprinting**
`tls_fingerprint.cpp` yazın. TLS ClientHello-nu parse edin: supported cipher suites, extensions, elliptic curves, signature algorithms. JA3 fingerprint hesablayın: `MD5(SSLVersion,Ciphers,Extensions,EllipticCurves,EllipticCurvePointFormats)`. Bu, client-i identity etmək üçün istifadə olunur.

---

### Tapşırıq 89
**Certificates Zəncirini Yoxlamak**
`cert_chain.cpp` yazın. Sertifikat zəncirini validate edin: leaf → intermediate → root CA. Hər sertifikatın imzasını əvvəlki ilə yoxlayın. Etibarlılıq müddətini yoxlayın. Subject/Issuer uyğunluğunu yoxlayın. OpenSSL `SSL_CTX_set_verify` ilə nə etdiyini anlayın.

---

### Tapşırıq 90
**Şəbəkə Performance Benchmark**
`net_benchmark.cpp` yazın. Benchmark edin: latency (bir döngünün RTT-si), throughput (MB/s), connection setup time (TCP handshake), concurrent connections sayı, qəbul edə biləcəyiniz max paket/saniyə. `localhost` ilə həm server həm client işlədərək real nəticələr alın.

---

### Tapşırıq 91
**SMTP Client Sıfırdan**
`smtp_client.cpp` yazın. SMTP protokolunu implement edin (RFC 5321): `EHLO`, `MAIL FROM`, `RCPT TO`, `DATA`, `QUIT`. `localhost:25`-ə (lokal mail server) email göndərin. SMTP injection hücumlarını şərh edin. `From` başlığını saxtalaşdırmanın niyə mümkün olduğunu izah edin.

---

### Tapşırıq 92
**IRC Client Sıfırdan**
`irc_client.cpp` yazın. IRC protokolunu implement edin (RFC 1459): `NICK`, `USER`, `JOIN`, `PRIVMSG`, `QUIT`. IRC-in botnet command-and-control üçün tarixən necə istifadə edildiyini şərh edin. Sadə bir IRC bot yazın — yalnız lokal test serveri üçün.

---

### Tapşırıq 93
**FTP Client Sıfırdan**
`ftp_client.cpp` yazın. FTP protokolunu implement edin (RFC 959): control channel (port 21) + data channel (passive mode PASV). Komandalar: `USER`, `PASS`, `PASV`, `LIST`, `RETR`, `STOR`, `QUIT`. FTP-nin niyə inherently insecure olduğunu şərh edin (cleartext credentials).

---

### Tapşırıq 94
**SSH-nin Konseptual Arxitekturası**
`ssh_concepts.cpp` yazın. SSH protocol stack-ini modelləşdirin (implement etmədən): Transport Layer Protocol, User Authentication Protocol, Connection Protocol. SSH-nin niyə telnet-dən üstün olduğunu texniki olaraq şərh edin. SSH key exchange-in addımlarını (Diffie-Hellman əsaslı) step-by-step sənəkləşdirin.

---

### Tapşırıq 95
**Şəbəkə Trafik Analyzer**
`traffic_analyzer.cpp` yazın. Loopback interfeysdən gələn trafiki analiz edin. Protokol dağılımını hesablayın (TCP vs UDP, port-lara görə). Top 10 "danışıqlı" host cütlüklərini tapın. Uncommon portları işarələyin. Real-time statistika çap edin.

---

### Tapşırıq 96
**Protokol State Machine Framework**
`state_machine.cpp` yazın. Generic state machine framework-u implement edin: `StateMachine<State, Event>`, transition table, guard funksiyaları, entry/exit actions. HTTP parser üçün istifadə edin: REQUEST_LINE → HEADERS → BODY → COMPLETE. Sonra LDAP üçün istifadə edəcəksiniz.

---

### Tapşırıq 97
**Şəbəkə Güvənlik Skaneri**
`sec_scanner.cpp` yazın. Aşağıdakıları skan edin: açıq portlar, xidmət versiyaları (banner grabbing ilə), zəif şifrə dəstəyi (TLS skan), default credentials (ümumi username/password kombinasiyaları). Nəticəni JSON formatında çap edin.

---

### Tapşırıq 98
**Firewall Qaydaları Analizi**
`firewall_rules.cpp` yazın. `iptables -L -n` çıxışını parse edin. Qaydaları data strukturunda saxlayın. Verilmiş src IP, dst IP, port, protokol üçün paket ACCEPT mi DROP mu olur, simulyasiya edin. Firewall bypass texnikalarını şərh edin.

---

### Tapşırıq 99
**Şəbəkə Kitabxanası v2.0**
`oxsium_net_v2.hpp` yazın. v1-i genişləndirin: async/callback-based API əlavə edin, TLS support (OpenSSL C API üzərindən wrapper), connection pool, retry logic ilə exponential backoff, metrics collection. Tam API sənədləşdirməsi.

---

### Tapşırıq 100
**Mini VPN Tunnel (Konseptual)**
`vpn_concepts.cpp` yazın. Sadə tunnel protokolu implement edin: UDP üzərindən encapsulation, paketləri əl ilə wrap etmək (outer header + inner packet), TUN/TAP interface yaratmaq (Linux). Həqiqi şifrəlemə əlavə etmədən belə, konseptin necə işlədiyini göstərin. WireGuard-ın arxitekturasını şərh edin.

---

# 🟠 MƏRHƏLƏ 3 — Kriptografi Sıfırdan (101–200)

### Tapşırıq 101
**XOR Şifrəsi və Frequency Analysis**
`xor_cipher.cpp` yazın. Single-byte XOR şifrəsi implement edin. Şifrəli mətnə frequency analysis hücumu yazın: hər byte-la XOR edin, İngilis hərflərinə ən yaxın olanı seçin (İngilis dilinin hərflik tezliyi cədvəli ilə). Multi-byte XOR için Kasiski testi implement edin.

---

### Tapşırıq 102
**Vigenère Şifrəsi**
`vigenere.cpp` yazın. Vigenère şifrəsini implement edin. Index of Coincidence ilə açar uzunluğunu tapın. Açar uzunluğunu tapdıqdan sonra hər offset üçün single-byte XOR kimi çözün. Tarixdə "le chiffre indéchiffrable" adlanan bu şifrənin niyə qırıldığını anlayın.

---

### Tapşırıq 103
**One-Time Pad**
`otp.cpp` yazın. Mükəmməl gizlilik nəzəriyyəsini demonstrate edin: OTP ilə şifrələnmiş mətnin frequency analysis-ə niyə müqavimət etdiyini göstərin. İki fərqli mesajı eyni key ilə şifrələdikdə nə olur — "two-time pad" hücumunu implementasiya edin. Shannon-un information-theoretic security anlayışını şərh edin.

---

### Tapşırıq 104
**LFSR (Linear Feedback Shift Register)**
`lfsr.cpp` yazın. n-bit LFSR implement edin (tap positions konfiqurasiya edilə bilən). 16-bit, 32-bit variantları yazın. Maksimal period sequence generasiya edin. Berlekamp-Massey alqoritmi ilə LFSR-i qırmağı simulate edin. A5/1 (GSM şifrəsi) üçün istifadə olunan LFSR strukturunu şərh edin.

---

### Tapşırıq 105
**Feistel Network**
`feistel.cpp` yazın. Ümumi Feistel network implement edin: konfigurasiya edilə bilən blok ölçüsü, round sayı, round funksiyası (callback kimi). 16-roundlu, 64-bit blok ölçülü nümunə göstərin. DES-in Feistel network üzərində qurulduğunu göstərin. Şifrəlmə = tərsinə çevrilmiş açarlarla şifrəçözümü olan gözəlliyi göstərin.

---

### Tapşırıq 106
**MD5 Sıfırdan**
`md5.cpp` yazın. MD5-i (RFC 1321) tam implement edin: mesaj padding, 512-bit blok emalı, 4 round × 16 əməliyyat, cədvəl sabitləri (sin funksiyasından). Sıfırdan hesablayın. Test vektorları: `md5("") = d41d8cd98f00b204e9800998ecf8427e`, `md5("abc") = 900150983cd24fb0d6963f7d28e17f72`.

---

### Tapşırıq 107
**SHA-1 Sıfırdan**
`sha1.cpp` yazın. SHA-1-i (RFC 3174) tam implement edin: 80 round, 4 stage, başlanğıc hash dəyərləri, sabitlər. Sıfırdan, yalnız 32-bit əməliyyatlar. Test: `sha1("abc") = a9993e364706816aba3e25717850c26c9cd0d89d`, `sha1("") = da39a3ee5e6b4b0d3255bfef95601890afd80709`.

---

### Tapşırıq 108
**SHA-256 Sıfırdan**
`sha256.cpp` yazın. SHA-256-ı (FIPS 180-4) tam implement edin: 64 round, 8 working variable, 64 sabit (küb köklərindən). `sha256("") = e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855`. NIST test vektorlarının hamısını keçin.

---

### Tapşırıq 109
**HMAC Sıfırdan**
`hmac.cpp` yazın. HMAC-ı (RFC 2104) implement edin. Template-based: `HMAC<SHA256>`, `HMAC<SHA1>`, `HMAC<MD5>`. `HMAC-SHA256("key", "message")` test vektoru ilə yoxlayın. Timing attack-dan qorunmaq üçün constant-time müqayisə əlavə edin.

---

### Tapşırıq 110
**PBKDF2 Sıfırdan**
`pbkdf2.cpp` yazın. PBKDF2-ı (RFC 2898) implement edin. `PBKDF2-HMAC-SHA256(password, salt, iterations, dkLen)`. 100,000 iterasiya ilə test edin. Brute force-u yavaşlatmaq üçün niyə yüksək iterasiya sayı lazımdır? GPU-əsaslı PBKDF2 cracking-in sürəti barəsində hesablayın.

---

### Tapşırıq 111
**bcrypt Konseptual İmplementasiya**
`bcrypt_concepts.cpp` yazın. Blowfish şifrəsinin key schedule-ını implement edin (bcrypt-in əsasını). `cost factor`-un niyə adaptiv olduğunu göstərin: cost artdıqca hesablama vaxtı ikiqat artır. bcrypt hash-inin formatını (`$2b$12$...`) parse edin.

---

### Tapşırıq 112
**Base64 Sıfırdan**
`base64.cpp` yazın. Base64-ü tam implement edin: encode, decode, URL-safe variant (+ → -, / → _), padding ilə və adsız. Test vektorları RFC 4648-dən. `base64url` da implement edin. Performans: 1GB/s-dən çox encode sürəti əldə edin.

---

### Tapşırıq 113
**Base32 Sıfırdan**
`base32.cpp` yazın. Base32-ni (RFC 4648) implement edin. TOTP (Time-based One-Time Password) üçün istifadə olunur. Encode/decode, padding. Test: `base32("foo") = MZXW6===`. Google Authenticator-ın seed-ini Base32 formatında saxladığını şərh edin.

---

### Tapşırıq 114
**CRC32 Sıfırdan**
`crc32.cpp` yazın. CRC32-ni (IEEE 802.3 polynomial: 0xEDB88320) implement edin. Lookup cədvəli əsaslı optimallaşdırma. `crc32("123456789") = 0xCBF43926`. Ethernet frames, ZIP faylları, PNG fayllarında CRC32-nin necə istifadə edildiyini şərh edin.

---

### Tapşırıq 115
**Adler-32 Sıfırdan**
`adler32.cpp` yazın. Adler-32 checksum implement edin (zlib-də istifadə olunur). `adler32("Wikipedia") = 0x11E60398`. CRC32 ilə sürət müqayisəsi edin. zlib-in Deflate compression-da Adler-32-dən necə istifadə etdiyini şərh edin.

---

### Tapşırıq 116
**Modular Exponentiation**
`modexp.cpp` yazın. Square-and-multiply alqoritmini implement edin: `pow_mod(base, exp, mod)`. 1024-bit ədədlər üçün (BigInteger class-ınızla). Timing side-channel-ı önləmək üçün Montgomery ladder implement edin. Bu, RSA və DH-nin əsasıdır.

---

### Tapşırıq 117
**Miller-Rabin Primality Test**
`primality.cpp` yazın. Miller-Rabin probabilistik primality test-i implement edin. 20 witness ilə: `is_prime_miller_rabin(n, 20)`. Sieve of Eratosthenes ilə ilk 10^6 sadə ədədi hesablayın. 512-bit, 1024-bit, 2048-bit sadə ədəd generasiya edin. RSA üçün niyə güclü primality test lazımdır?

---

### Tapşırıq 118
**Extended Euclidean Algorithm**
`euclid.cpp` yazın. GCD, Extended GCD implement edin: `(d, x, y)` qaytarsın, burada `d = gcd(a,b)` və `ax + by = d`. Modular inverse: `mod_inverse(a, m)` — Extended Euclidean ilə. Bu, RSA key generation üçün `e` və `d`-ni hesablamaq üçün lazımdır.

---

### Tapşırıq 119
**BigInteger Kitabxanası**
`bigint.cpp` yazın. 2048-bit ədədlər üçün BigInteger implement edin: `uint32_t` massivi üzərindən. Əməliyyatlar: `add`, `sub`, `mul` (school method), `div` (long division), `mod`, `pow_mod`, `compare`, `to_hex`, `from_hex`. Bütün kriptografik alqoritmləriniz bu üzərindən işləcəkdir.

---

### Tapşırıq 120
**Diffie-Hellman Key Exchange**
`dh_kex.cpp` yazın. DH key exchange-i implement edin: 2048-bit MODP group (RFC 3526 Group 14 parametrləri), `g=2`, `p` (2048-bit prime). Alice: random `a`, `A = g^a mod p`. Bob: random `b`, `B = g^b mod p`. Shared secret: `s = B^a mod p = A^b mod p`. Verify edir.

---

### Tapşırıq 121
**RSA Sıfırdan**
`rsa.cpp` yazın. RSA tam implement edin: `key_gen(bits)` — p, q seç, n=pq, φ(n)=(p-1)(q-1), e=65537, d=e⁻¹ mod φ(n). `encrypt(msg, e, n)` = `msg^e mod n`. `decrypt(cipher, d, n)` = `cipher^d mod n`. 512-bit (test üçün), 1024-bit, 2048-bit dəstəyi.

---

### Tapşırıq 122
**RSA PKCS#1 v1.5 Padding**
`rsa_pkcs1.cpp` yazın. PKCS#1 v1.5 padding scheme implement edin: `0x00 0x02 PS 0x00 M` (encryption), `0x00 0x01 PS 0x00 M` (signature). `PS` minimum 8 byte random (non-zero) baytdır. Bleichenbacher oracle attack-ını (1998) niyə mümkün etdiyini şərh edin.

---

### Tapşırıq 123
**RSA-OAEP Padding**
`rsa_oaep.cpp` yazın. OAEP (Optimal Asymmetric Encryption Padding) implement edin: MGF1 (Mask Generation Function), seed masklaması, data block masklaması. Bu, Bleichenbacher hücumunu necə önlədiyi barədə texniki şərh əlavə edin.

---

### Tapşırıq 124
**Elliptic Curve Kriptografiyası: Əsaslar**
`ecc_basics.cpp` yazın. Elliptic curve üzərindəki nöqtə əməliyyatlarını implement edin: `point_add(P, Q)`, `point_double(P)`, `scalar_multiply(k, P)` (double-and-add). Curve: `y² = x³ - 3x + b (mod p)` — secp256r1 (P-256) parametrləri ilə. Nöqtə sonsuzluqda (identity element) idarəsi.

---

### Tapşırıq 125
**ECDH Sıfırdan**
`ecdh.cpp` yazın. P-256 əyrisindəki ECDH implement edin: private key generasiyası (random), public key = `private * G`, shared secret = `Alice_priv * Bob_pub = Bob_priv * Alice_pub`. Shared secret-dən AES açarı türet: `SHA-256(x-coordinate)`.

---

### Tapşırıq 126
**AES S-Box Sıfırdan**
`aes_sbox.cpp` yazın. AES S-box-ı GF(2⁸) hesablamaqla sıfırdan generasiya edin (cədvəli hardcode etmədən): multiplicative inverse in GF(2⁸), affine transformation. Hər ikisini (hardcoded cədvəl vs hesablanmış) müqayisə edin. Sıfır üçün edge case.

---

### Tapşırıq 127
**AES-128 Sıfırdan**
`aes128.cpp` yazın. AES-128-i tam implement edin: AddRoundKey, SubBytes (S-box), ShiftRows, MixColumns, key expansion (10 round key). NIST FIPS 197 test vektorları ilə yoxlayın: `Plaintext: 00112233...eeff, Key: 000102...0f → Cipher: 69c4e0d8...5aee`.

---

### Tapşırıq 128
**AES-256 Sıfırdan**
`aes256.cpp` yazın. AES-256-yı implement edin: 14 round, 256-bit key schedule. AES-128 ilə eyni struktur, fərqli key expansion. Performance benchmark: GB/s throughput ölçün. NIST test vektorları ilə yoxlayın.

---

### Tapşırıq 129
**AES-CBC Mode**
`aes_cbc.cpp` yazın. CBC mode implement edin: `encrypt(key, iv, plaintext)`, `decrypt(key, iv, ciphertext)`. PKCS#7 padding. IV niyə random olmalıdır? Padding oracle attack-ını şərh edin. `openssl enc -aes-128-cbc` ilə çıxışınızı müqayisə edin.

---

### Tapşırıq 130
**AES-CTR Mode**
`aes_ctr.cpp` yazın. CTR mode implement edin: counter bloku `nonce||counter`, paralel şifrəlmə/açma. CTR mode-un CBC-dən niyə üstün olduğunu göstərin: seek qabiliyyəti, paralellik, padding yoxdur. Nonce təkrarlanmasının nə üçün katastrofik olduğunu şərh edin.

---

### Tapşırıq 131
**AES-GCM Sıfırdan**
`aes_gcm.cpp` yazın. Galois/Counter Mode implement edin: CTR mode şifrələmə + GHASH authentication tag. `GHASH(H, A, C)` funksiyası GF(2¹²⁸) üzərindədir. Authenticated Encryption əldə etmek üçün: şifrəlmə + bütövlük yoxlaması birlikdə. Tampered ciphertext-i aşkar edin.

---

### Tapşırıq 132
**ChaCha20 Sıfırdan**
`chacha20.cpp` yazın. ChaCha20 stream cipher-ı (RFC 7539) implement edin: 20 round, quarter-round funksiyası, 512-bit state, counter mode. `ChaCha20(key, nonce, counter, plaintext)`. Test vektorları RFC 7539-dan. AES-CTR ilə sürət müqayisəsi.

---

### Tapşırıq 133
**Poly1305 MAC Sıfırdan**
`poly1305.cpp` yazın. Poly1305 MAC-ı (RFC 7539) implement edin: GF(2¹³⁰-5) üzərindəki polinom hesablaması. `Poly1305(key, msg)` → 16-byte tag. ChaCha20-Poly1305 AEAD-i birləşdirin. Test vektorları RFC 7539-dan.

---

### Tapşırıq 134
**Salsa20 Sıfırdan**
`salsa20.cpp` yazın. Salsa20 stream cipher-ı implement edin: 20 round, column/row round, 64-byte output block. ChaCha20-nun sələfi. Hər ikisini benchmark edin. eStream portfolio contest-inin nə olduğunu şərh edin.

---

### Tapşırıq 135
**RC4 Sıfırdan**
`rc4.cpp` yazın. RC4-ü (ARCFOUR) implement edin: KSA (Key Scheduling Algorithm), PRGA (Pseudo-Random Generation Algorithm). `RC4("Key", "Plaintext") = BB F3 16 E8 D9 40 AF 0A D3`. RC4-ün niyə qırıldığını göstərin: ilk çıxış baytlarındakı bias (Fluhrer-Mantin-Shamir hücumu).

---

### Tapşırıq 136
**Blowfish Sıfırdan**
`blowfish.cpp` yazın. Blowfish blok şifrəsini implement edin: 64-bit blok ölçüsü, variable key uzunluğu (32-448 bit), 16 round Feistel network, P-array (18 uint32) + 4 S-box (her biri 256 uint32). Key expansion: P-array + S-box-ları key-ilə inisializasiya edin.

---

### Tapşırıq 137
**3DES (Triple DES) Sıfırdan**
`3des.cpp` yazın. 3DES-i implement edin: `E(k3, D(k2, E(k1, plaintext)))`. EDE (Encrypt-Decrypt-Encrypt) modu. İki açar (k1=k3) vs üç açar variantları. Meet-in-the-middle hücumunun niyə double DES-i zəiflətdiyini şərh edin. 3DES-in niyə köhnəlmiş sayıldığını izah edin.

---

### Tapşırıq 138
**Keccak/SHA-3 Sıfırdan**
`sha3.cpp` yazın. Keccak sponge construction implement edin: 5×5 lane state (64-bit), θ, ρ, π, χ, ι permutation round-ları. SHA3-256, SHA3-512, SHAKE128, SHAKE256 çıxış funksiyaları. NIST test vektorları ilə yoxlayın. SHA-2-dən fərqini şərh edin.

---

### Tapşırıq 139
**BLAKE2 Sıfırdan**
`blake2.cpp` yazın. BLAKE2b-ni implement edin: 12 round, 8×64-bit working variables, G mixing function, sigma permutation. BLAKE2b-256 için test vektorları. SHA-256 ilə sürət müqayisəsi (BLAKE2 adətən daha sürətlidir). Password hashing üçün BLAKE2-nin üstünlüklərini şərh edin.

---

### Tapşırıq 140
**Argon2 Sıfırdan (Sadələşdirilmiş)**
`argon2_simple.cpp` yazın. Argon2id-nin sadələşdirilmiş versiyasını implement edin: memory-hard function, pass sayı, memory ölçüsü, parallelism faktoru. Bu, GPU-əsaslı cracking-i niyə çətinləşdirir? PHC (Password Hashing Competition) qalib alqoritmi olaraq seçilmə səbəbini şərh edin.

---

### Tapşırıq 141
**Kriptografik Təsadüfi Ədəd Generatoru**
`csprng.cpp` yazın. CSPRNG implement edin: `/dev/urandom` seed-dən alınan AES-256-CTR əsaslı. ChaCha20-əsaslı variant da yazın. Diehard/NIST statistical test-lərindən bəzilərini implement edin: frequency test, runs test, autocorrelation test. `rand()` ilə müqayisə edin.

---

### Tapşırıq 142
**Digital Signature (DSA) Sıfırdan**
`dsa.cpp` yazın. DSA-ı implement edin: key generation (p, q, g parametrləri, x, y), imza (r, s hesablanması), yoxlama. RFC 6979 ilə deterministic k generasiyası (Sony PS3 hücumunu önləmək üçün). Test: `sign(key, msg)` → `verify(pubkey, msg, sig)`.

---

### Tapşırıq 143
**ECDSA Sıfırdan**
`ecdsa.cpp` yazın. P-256 üzərindəki ECDSA-ı implement edin: imza yaratma `(r, s)`, yoxlama. RFC 6979 deterministic nonce. Etibarsız nonce təkrarlanmasının niyə private key-i ortaya çıxardığını göstərin (Bitcoin wallet-larının bu şəkildə oğurlandığı hadisələri şərh edin).

---

### Tapşırıq 144
**Ed25519 Sıfırdan**
`ed25519.cpp` yazın. Ed25519 (Edwards-curve DSA) implement edin: Curve25519 twisted Edwards forması, point arithmetic, hash-based challenges. Deterministik imza — heç bir random nonce yoxdur. Bitcoin Taproot-da, SSH-də, TLS 1.3-də istifadəsini şərh edin.

---

### Tapşırıq 145
**Kriptografik Hash-in Collision Tapmaq (Toy Example)**
`birthday_attack.cpp` yazın. Doğum günü paradoksunu demonstrasiya edin: 32-bit hash funksiyası üçün ~65536 sorğudan sonra collision tapın. `O(2^(n/2))` komplekslikini praktiki olaraq göstərin. MD5 collision-larının (Wang 2004) niyə mümkün olduğunu şərh edin.

---

### Tapşırıq 146
**Length Extension Attack**
`length_extension.cpp` yazın. MD5/SHA-1/SHA-256 üzərindəki length extension hücumunu implement edin: `H(key || msg)` bildiyinizdə `H(key || msg || pad || extension)`-ı hesablayın key-i bilmədən. SHA-3-ün niyə bu hücuma qarşı immune olduğunu şərh edin. Flickr API-sinin bu hücumdan necə istifadə edildiyini şərh edin.

---

### Tapşırıq 147
**Padding Oracle Attack**
`padding_oracle.cpp` yazın. AES-CBC-yə qarşı padding oracle attack-ı implement edin: CBC decryption mexanizmindən istifadə edərək şifrəli mətni açın, yalnız "valid padding" / "invalid padding" oracle funksiyası var. POODLE hücumunu şərh edin.

---

### Tapşırıq 148
**Timing Attack**
`timing_attack.cpp` yazın. Non-constant-time HMAC müqayisəsinə qarşı timing attack-ı simulate edin: tərəf kanalından `correct_byte_at_pos()` funksiyası. `std::chrono::high_resolution_clock` ilə nanosaniyə fərqlərini ölçün. Constant-time müqayisəni implement edin və statistik fərqi göstərin.

---

### Tapşırıq 149
**Stream Cipher Reuse Attack**
`stream_reuse.cpp` yazın. Eyni nonce/key ilə iki mesajı şifrəldiyinizdə: `C1 = P1 XOR K`, `C2 = P2 XOR K` → `C1 XOR C2 = P1 XOR P2`. Mətnin crib-dragging ilə necə bərpa ediləcəyini göstərin. WEP-in (Wi-Fi şifrəsi) niyə qırıldığını şərh edin.

---

### Tapşırıq 150
**Kriptografi Kitabxanası v1.0**
`oxsium_crypto_v1.hpp` yazın. Bütün kriptografik primitivi clean API-yə birləşdirin:
```cpp
namespace oxsium::crypto {
  class AES256GCM;
  class ChaCha20Poly1305;
  class SHA256;
  class SHA3_256;
  class HMAC_SHA256;
  class PBKDF2;
  class ECDH_P256;
  class ECDSA_P256;
  class CSPRNG;
}
```
Tam test suite.

---

### Tapşırıq 151
**Password Hashing Kitabxanası**
`password_hash.cpp` yazın. LDAP password formatlarını implement edin: `{CLEARTEXT}`, `{MD5}`, `{SHA}`, `{SSHA}`, `{SHA256}`, `{SSHA256}`, `{SHA512}`, `{SSHA512}`, `{CRYPT}`. `verify_password(stored, cleartext)` funksiyası bütün formatları dəstəkləsin. Test suite.

---

### Tapşırıq 152
**Secure Key Storage**
`key_storage.cpp` yazın. Kriptografik açarları güvənli saxlamaq üçün: AES-GCM ilə şifrələnmiş fayl formatı, PBKDF2 ilə master password-dən açar türetmə, secure memory (mlock, memset after use), anti-debugging (ptrace yoxlaması). Keyring API konseptini şərh edin.

---

### Tapşırıq 153
**Kriptografik Protokol: Challenge-Response**
`challenge_response.cpp` yazın. SCRAM (Salted Challenge Response Authentication Mechanism) implement edin: server random challenge göndərir, client `HMAC(key, challenge)` hesablar. Replay hücumuna qarşı nonce əlavə edin. Session key-i birlikdə generasiya edin.

---

### Tapşırıq 154
**Sertifikat Validator**
`cert_validator.cpp` yazın. X.509 sertifikatını tam yoxlayın: imza (RSA-SHA256), etibarlılıq müddəti, Subject Alternative Names, Basic Constraints, Key Usage, Extended Key Usage. Sertifikat pinning implement edin. Zəif sertifikat parametrlərini (1024-bit RSA, SHA-1 imza) aşkarlayın.

---

### Tapşırıq 155
**Authenticated Key Exchange**
`ake.cpp` yazın. Authenticated ECDH implement edin: ECDHE-ECDSA. Alice public key-ini ECDSA ilə imzalayır, Bob yoxlayır. Uzaqdan kim olduğunu bilmədən key exchange-in niyə man-in-the-middle hücumuna açıq olduğunu şərh edin.

---

### Tapşırıq 156
**Key Derivation Framework**
`kdf.cpp` yazın. HKDF-i (RFC 5869) implement edin: Extract + Expand. `HKDF(IKM, salt, info, len)`. TLS session key-lərinin HKDF ilə necə türədildiyini göstərin: `HKDF-Expand-Label(secret, label, context, length)`. Test vektorları RFC 5869-dan.

---

### Tapşırıq 157
**Kriptografik Random Sayların Keyfiyyəti**
`rng_quality.cpp` yazın. NIST SP 800-22 statistical test suite-dən implement edin: Frequency (monobit) test, Block frequency test, Runs test, Longest run test, Binary matrix rank test. `rand()`, `/dev/urandom`, AES-CTR DRBG çıxışlarını müqayisə edin.

---

### Tapşırıq 158
**Zero-Knowledge Proof (Toy)**
`zkp_toy.cpp` yazın. Feige-Fiat-Shamir protokolunu implement edin: Peggy bir sirri (gizli faktorizasiya) bilir, Victor-a bunu sübut edir, amma sirri açmadan. Protokolun 20 roundunu simulate edin. ZKP-nin real dünya tətbiqlərini (Zcash, STARK) şərh edin.

---

### Tapşırıq 159
**Shamir's Secret Sharing**
`secret_sharing.cpp` yazın. Shamir's (k, n) threshold secret sharing implement edin: sirri n payçaya böl, k-sı ilə yenidən birləşdir. Lagrange interpolation GF(p) üzərindədir. Test: `split(secret, k=3, n=5)` → herhangi 3 pay ilə `reconstruct()`.

---

### Tapşırıq 160
**Homomorphic Encryption (Toy)**
`homomorphic_toy.cpp` yazın. Sadə additive homomorphic encryption implement edin (Paillier criptosisteminin oyuncaq versiyası): `Enc(a) + Enc(b) = Enc(a+b)`. Real dünyada bu texnologiyanın niyə hələ geniş istifadə olunmadığını şərh edin.

---

### Tapşırıq 161
**Steganography: LSB**
`steganography.cpp` yazın. BMP şəkil faylına mətn gizlədin: hər pikselin least significant bit-ini istifadə edin. `hide(image, message)`, `extract(image)`. Statistik aşkarlama üçün chi-square test yazın. RS (Regular-Singular) steganalysis metodunu şərh edin.

---

### Tapşırıq 162
**Password Strength Analyzer**
`password_strength.cpp` yazın. Shannon entropy hesablayın. Şifrə gücü metriklərini hesablayın: karakter sinifləri, uzunluq, ortak şifrələrə qarşı yoxlama (Have I Been Pwned API konsepti), keyboard walk aşkarlama, dictionary word aşkarlama. zxcvbn alqoritminə əsaslı scoring.

---

### Tapşırıq 163
**Brute Force / Dictionary Attack Simulyatoru**
`cracker.cpp` yazın. MD5/SHA-1 hash-larını sındırın: dictionary attack (wordlist), rule-based mutation (l33tspeak, append numbers), hybrid attack. Sürəti ölçün: hashes/second. GPU-əsaslı cracking ilə müqayisə edin. Bu tapşırıq yalnız sizin yaratdığınız test hash-ları üçündür.

---

### Tapşırıq 164
**Rainbow Table**
`rainbow_table.cpp` yazın. Sadə rainbow table implement edin: reduction funksiyası (hash → password), chain generasiyası, lookup. MD5 üçün 6 simvol lowercase alfasayısal şifrələr üçün table yaradın. Salting-in rainbow table-ları niyə istifadəsiz etdiyini göstərin.

---

### Tapşırıq 165
**Kriptoanalizin Əsas Alətləri**
`cryptanalysis_tools.cpp` yazın. Implement edin: frequency analysis (hər byte, bigram, trigram tezliyi), index of coincidence, chi-square test, Kasiski test (Vigenère key uzunluğunu tapmaq), Hamming distance (XOR key uzunluğunu tapmaq). Bu alətlərin hamısı çağdaş CTF-lərdə işlənir.

---

### Tapşırıq 166
**Kriptografik Protokol: Needham-Schroeder**
`needham_schroeder.cpp` yazın. Needham-Schroeder Symmetric Key Protocol-u implement edin. Lowe hücumunu (1996) simulate edin — protokoldakı məşhur bug. TLS-in bu hücumlardan necə qorunduğunu şərh edin.

---

### Tapşırıq 167
**TLS 1.3 Handshake Simulyasiyası**
`tls13_handshake.cpp` yazın. TLS 1.3 handshake-i simulyasiya edin: ClientHello, ServerHello, Handshake keys hesablanması (HKDF-dən), Certificate + CertificateVerify, Finished (MAC yoxlaması). Hər mesajın bayt formatını şərh edin.

---

### Tapşırıq 168
**Forward Secrecy Demonstrasiyası**
`forward_secrecy.cpp` yazın. Ephemeral DH ilə perfect forward secrecy-ni demonstrasiya edin: hər session üçün yeni DH key pair. Static RSA encryption ilə müqayisə edin: biri açarı qırıldıqda, bütün keçmiş trafiki açmağın mümkün olduğunu göstərin.

---

### Tapşırıq 169
**Kriptografik API Dizaynı**
`crypto_api_design.cpp` yazın. Misuse-resistant kriptografi API-si dizayn edin: yanlış istifadəni compile-time-da imkansız edin. Məsələn: IV/nonce avtomatik generasiya, key strength yoxlaması, authenticated encryption yalnız, raw cipher məcralarının bloklanması. Cryptographic API dizayn prinsiplərini şərh edin.

---

### Tapşırıq 170
**Kriptografik Kitabxana v2.0**
`oxsium_crypto_v2.hpp` yazın. v1-i genişləndirin: key management (generation, storage, rotation), protocol-level abstractions (TLS-like handshake, AEAD messaging), certificate handling, full test suite. Performans benchmarks.

---

### Tapşırıq 171
**Hardware Security Module Simulatoru**
`hsm_sim.cpp` yazın. HSM-in (Hardware Security Module) davranışını simulate edin: açarlar heç vaxt çıxmır, yalnız kriptografik əməliyyatlar içəridə edilir. API: `generate_key()`, `sign(data)`, `decrypt(ciphertext)` — açarı heç vaxt açmır. PKCS#11 interfeys konseptini şərh edin.

---

### Tapşırıq 172
**Post-Quantum Kriptografiya: Əsaslar**
`pqc_basics.cpp` yazın. CRYSTALS-Kyber (ML-KEM) alqoritm strukturunu implement edin: module lattice üzərindəki key encapsulation. Learning With Errors (LWE) problemini şərh edin. Kvant kompüterlərin RSA/ECC-ni nəyə görə qırabiləcəyini (Shor alqoritmi) izah edin.

---

### Tapşırıq 173
**Kriptografik Hash Funksiyaları Müqayisəsi**
`hash_benchmark.cpp` yazın. Benchmark edin: MD5, SHA-1, SHA-256, SHA-512, SHA3-256, BLAKE2b, BLAKE3. Ölçün: MB/s throughput, latency (1 byte input), latency (1 MB input), parallelism imkanı. Nəticəni cədvəl formatında çap edin. Hansı use case üçün hansı hash tövsiyə olunur?

---

### Tapşırıq 174
**Side-Channel Attack Azaldılması**
`side_channel_mitigation.cpp` yazın. Aşağıdakı texnikaları implement edin: constant-time table lookup (secret-dependent memory access-dən qaçmaq), masking (əməliyyatları random mask ilə örtmək), blinding (RSA üçün random blinding factor). Cache-timing hücumlarını şərh edin.

---

### Tapşırıq 175
**Fuzzing Kriptografik Primitivi**
`crypto_fuzzer.cpp` yazın. Kriptografik implementasiyalarınızı fuzz edin: random giriş ilə `sha256()`, `aes128_encrypt()`, `rsa_decrypt()` çağırın. Heç birinin crash etmədiyini, invalid input-da paniklamamasını yoxlayın. Mutation-based fuzzer ilə structure-aware fuzzer arasındakı fərqi şərh edin.

---

### Tapşırıq 176
**Kriptografik Primitiv Unit Test Suite**
`crypto_tests.cpp` yazın. Bütün kriptografik implementasiyalarınız üçün 300+ test vektoru: NIST CAVP test vektorları, RFC test vektorları, Wycheproof test vektorları (Google-ın kriptografik test layihəsi). 100% test keçidi məcburidir.

---

### Tapşırıq 177
**Encoded Data Format Parser**
`encoded_parser.cpp` yazın. Parse edin: PEM format (`-----BEGIN CERTIFICATE-----`), DER binary format, PKCS#12 (.pfx) struktur, JWK (JSON Web Key) format. Hər formatdan public/private key-i çıxarın.

---

### Tapşırıq 178
**JWT (JSON Web Token) Sıfırdan**
`jwt.cpp` yazın. JWT-i implement edin: header + payload + signature. `HS256` (HMAC-SHA256) və `RS256` (RSA-SHA256) imza alqoritmləri. Verify edin. `alg: none` hücumunu demonstrasiya edin — bəzi kitabxanaların bu hücuma necə zəifliyi.

---

### Tapşırıq 179
**OAuth2 / OpenID Connect Sıfırdan**
`oauth2_concepts.cpp` yazın. Authorization Code Flow-u implement edin: authorization request, token exchange, JWT verify. PKCE (Proof Key for Code Exchange) əlavə edin. Müxtəlif grant type-ları şərh edin. Token hijacking hücumlarını şərh edin.

---

### Tapşırıq 180
**Kriptografik Protokol Test Framework**
`crypto_protocol_test.cpp` yazın. Protocol-level test-lər yazın: man-in-the-middle aşkarlama (sertifikat yoxlaması olmadan), replay hücumu aşkarlama, downgrade hücumu aşkarlama (weaker cipher seçilmənin qarşısı), forward secrecy yoxlaması.

---

### Tapşırıq 181
**Kriptografik Standartlar Compliance**
`fips_compliance.cpp` yazın. FIPS 140-2 Level 1 tələblərini yoxlayın: təsadüfi bit generatoru, zayıf alqoritm (DES, MD5) istifadəsini bloklayın, key strength yoxlaması, approved algorithms siyahısı. Self-test suite yazın.

---

### Tapşırıq 182
**Wireless Protokol Şifrəsi (WEP Analysis)**
`wep_analysis.cpp` yazın. WEP şifrəsini implement edin: RC4 + CRC32. WEP-in zəifliklərini demonstrasiya edin: zəif IV, keystream reuse. FMS hücumunu (Fluhrer-Mantin-Shamir) şərh edin. WPA2-CCMP-nin bu problemləri necə həll etdiyini izah edin.

---

### Tapşırıq 183
**Disk Şifrəlmə Konsepti**
`disk_encrypt.cpp` yazın. XTS-AES (XEX-based Tweaked CodeBook mode with ciphertext Stealing) implement edin: disk sektorlarının şifrəlməsi üçün. Tweakable block cipher konseptini şərh edin. LUKS (Linux Unified Key Setup) formatının strukturunu sənəkləşdirin.

---

### Tapşırıq 184
**Kriptografik Protokol: Signal Protokolu (Konseptual)**
`signal_protocol.cpp` yazın. Signal protokolunun əsas komponentlərini implement edin: X3DH (Extended Triple Diffie-Hellman) key agreement, Double Ratchet Algorithm (message key rotation). End-to-end şifrəlmənin niyə WhatsApp/Signal-da istifadə edildiyini şərh edin.

---

### Tapşırıq 185
**Kriptografik Hash Zənciri**
`hash_chain.cpp` yazın. Hash zənciri implement edin: `h_0 = hash(seed)`, `h_i = hash(h_{i-1})`. One-time password generasiya (Lamport OTP). Blockchain-in əsas konseptini demonstrasiya edin. Hash zəncirlərinin niyə tamperproof olduğunu göstərin.

---

### Tapşırıq 186
**Kriptografik Protokol Logging**
`crypto_audit_log.cpp` yazın. Kriptografik əməliyyatlar üçün audit log: hər şifrəlmə/açma, key generation, imza — müvafiq context ilə log-layın. Log-ların özlərinin dəyişdirilməyə qarşı qorunması: HMAC chain (hər log entry əvvəlkinin HMAC-ını ehtiva edir).

---

### Tapşırıq 187
**Elliptic Curve Seçimi**
`curve_selection.cpp` yazın. Müxtəlif EC curve-ları implement edin: P-256, P-384, P-521, Curve25519, secp256k1 (Bitcoin). Hər birinin parametrlərini (prime, a, b, G, n) sənəkləşdirin. NIST curve-larının Curve25519-la müqayisəsi: Bernstein-in etirazlarını şərh edin.

---

### Tapşırıq 188
**Kriptografik Protokol Versiyalanması**
`crypto_versioning.cpp` yazın. Kriptografik aqility implement edin: alqoritm identifikatorları, versiyon başlıqları, köhnə format dəstəyi. Deprecation mexanizmi: köhnə alqoritm istifadəsini xəbərdar edib blok edin. TLS cipher suite negotiation konseptini modellləşdirin.

---

### Tapşırıq 189
**OCSP (Online Certificate Status Protocol)**
`ocsp_client.cpp` yazın. OCSP request-i əl ilə qurun: CertID hesablanması (SHA-1 of issuer key hash + serial number), DER encoded request. OCSP responder-ə HTTP ilə göndərin. Response-u parse edin: good/revoked/unknown. Certificate Revocation List (CRL) ilə müqayisə edin.

---

### Tapşırıq 190
**Kriptografik Primitiv Composition**
`crypto_composition.cpp` yazın. Kriptografik primitivi düzgün birləşdirin: Encrypt-then-MAC (doğru), MAC-then-Encrypt (zəif), MAC-and-Encrypt (zəif). Hər birini implement edin, yanlış birləşdirmənin niyə təhlükəli olduğunu demonstrasiya edin.

---

### Tapşırıq 191
**Nonce Menecmenti**
`nonce_manager.cpp` yazın. Nonce/IV menecmenti sistemi yazın: counter-əsaslı (deterministik), random nonce (stateless), SIV (Synthetic IV - nonce misuse resistant). Nonce tekrarının nəticəsini demonstrasiya edin (AES-CTR-da). Nonce menecmentinin niyə kritik olduğunu şərh edin.

---

### Tapşırıq 192
**TOTP (Time-based One-Time Password)**
`totp.cpp` yazın. TOTP-u (RFC 6238) implement edin: `HOTP(K, T)` — K açar, T vaxt adımı (30 saniyə). `HOTP = Truncate(HMAC-SHA1(K, T))`. 6 rəqəmli OTP generasiya edin. Google Authenticator uyğun olduğunu `oathtool` ilə yoxlayın.

---

### Tapşırıq 193
**FIDO2/WebAuthn Əsasları**
`webauthn_basics.cpp` yazın. WebAuthn əsas əməliyyatlarını modelləşdirin: attestation (credential yaradılması), assertion (authentication). COSE (CBOR Object Signing and Encryption) key formatını parse edin. Passkey-in necə işlədiyini şərh edin.

---

### Tapşırıq 194
**Kriptografik Kitabxana: Performans Profili**
`crypto_profile.cpp` yazın. Kriptografik kitabxananızı profilə edin: `perf record` + flame graph. Bottleneck-ları tapın. Aşağıdakıları optimallaşdırın: AES S-box lookup (cache-friendly), modular exponentiation (Montgomery form), hash compression function (unrolling). 30%+ sürət artımı əldə edin.

---

### Tapşırıq 195
**Kriptografik Zəiflik Scanner**
`crypto_vuln_scanner.cpp` yazın. Kod bazasında kriptografik zəifliklər axtarın: MD5/SHA1 istifadəsi (deprecated), ECB mode, statik IV, hardcoded key, insufficient key size (< 2048 bit RSA), weak PRNG (rand(), srand(time())), non-constant-time comparison.

---

### Tapşırıq 196
**Secure Communication Channel**
`secure_channel.cpp` yazın. Tam şifrələnmiş kommunikasiya kanalı implement edin: ECDHE key exchange, AES-256-GCM şifrəlmə, ECDSA authentication, replay attack prevention (sequence numbers), perfect forward secrecy. Bir loopback üzərindəki bağlantı ilə sınayın.

---

### Tapşırıq 197
**Kriptografik Protokol Analizi**
`protocol_analysis.cpp` yazın. Tamarin/ProVerif formal verification alətlərinin çıxışını manual olaraq simulyasiya edin: protokol addımlarını state machine kimi modelləşdirin, potensial attack path-ları tapın. Kriptografik protokolların formal olaraq analiz edilməsinin niyə vacib olduğunu şərh edin.

---

### Tapşırıq 198
**Kriptografik Hash Tabanlı Data Structure**
`hash_structures.cpp` yazın. Implement edin: Merkle tree (hash-based authentication), Bloom filter (probabilistic set membership), Cuckoo filter (false positive rate), Hash map with cryptographic hash (HMAC əsaslı, DoS-resistant). Bunların kibertəhlükəsizlikdəki tətbiqlərini şərh edin.

---

### Tapşırıq 199
**Secure Coding Standards**
`secure_coding_audit.cpp` yazın. Aşağıdakı standartlara uyğun kod yazmaq üçün checker funksiyaları yazın: CERT C Secure Coding (INT30-C, STR31-C, MEM30-C), OWASP Secure Coding Practices, CWE Top 25. Öncəki tapşırıqlardakı kodunuzu bu standartlara görə analiz edin.

---

### Tapşırıq 200
**Kriptografi Kitabxanası: Final v3.0**
`oxsium_crypto_v3.hpp` yazın. Hər şeyi birləşdirin. Tam API sənədləşdirməsi, 500+ test vektoru, cross-platform support, FIPS compliance mode, misuse-resistant API, performance benchmarks. `README.md` yazın: "Bu kitabxana niyə mövcuddur?" sualını cavablandırın.

# 🛡️ C++ & KİBERTƏHLÜKƏSİZLİK — Praktiki Tapşırıqlar (201–500)

> **Tapşırıqlar 201–500** | Növbəti 300 Praktiki Məşğələ  
> Şəbəkə Proqramlaşdırması • Kriptoqrafiya • Sistem Proqramlaşdırması • Protokol İmplementasiyası • Kibertəhlükəsizlik • Production Sistemlər

---


## MƏRHƏLƏ 5: Şəbəkə Proqramlaşdırması (201–240)
> Socket API, TCP/UDP, Asinxron I/O

| # | Tapşırıq Adı | Təsvir / Məqsəd | Səviyyə |
|---|---|---|---|
| 201 | BSD Socket Yaratmaq | `socket()` sistemi çağırışı ilə TCP socket yarat, nəticəni ekrana çap et | ⚙️ ORTA |
| 202 | Bind və Listen | Socket-ı porta bind et, `listen()` ilə bağlantı gözlə | ⚙️ ORTA |
| 203 | Echo Server | Bağlanan clientdən mətn al, eyni mətni geri göndər | ⚙️ ORTA |
| 204 | Multi-Client Server (fork) | Hər client üçün `fork()` ilə ayrı proses yarat | ⚙️ ORTA |
| 205 | UDP Datagram Göndər | `sendto()`/`recvfrom()` istifadə edərək UDP mesaj göndər/al | ⚙️ ORTA |
| 206 | Getaddrinfo() İstifadəsi | DNS adını IP-yə çevir, IPv4/IPv6 dəstəyi əlavə et | ⚙️ ORTA |
| 207 | Non-blocking Socket | `fcntl()` ilə socket-ı non-blocking rejiminə keçir | ⚙️ ORTA |
| 208 | select() Multiplexing | `select()` ilə eyni anda çoxlu socket-ı izlə | ⚙️ ORTA |
| 209 | poll() Multiplexing | `poll()` API ilə fərqi göstər, `select()` ilə müqayisə et | ⚙️ ORTA |
| 210 | epoll Edge-Triggered | `epoll()` EPOLLET rejimini istifadə et, yüksək performans göstər | ⚙️ ORTA |
| 211 | HTTP GET Sorğusu | Raw socket ilə HTTP/1.1 GET sorğusu göndər, cavabı parse et | 🔥 ÇƏTİN |
| 212 | Çoxlu Client Thread Pool | Thread pool server yarat, hər bağlantıya thread ver | 🔥 ÇƏTİN |
| 213 | SO_REUSEADDR/PORT | Socket seçənəklərini tənzimlə, TIME_WAIT problemini həll et | 🔥 ÇƏTİN |
| 214 | TCP Keepalive | `SO_KEEPALIVE`, `TCP_KEEPIDLE`, `TCP_KEEPINTVL` ayarla | 🔥 ÇƏTİN |
| 215 | Scatter-Gather I/O | `readv()`/`writev()` ilə çoxlu buffer istifadə et | 🔥 ÇƏTİN |
| 216 | sendfile() Optimizasiyası | Fayl göndərmək üçün `sendfile()` zero-copy texnikası | 🔥 ÇƏTİN |
| 217 | Unix Domain Socket | `AF_UNIX` socket ilə eyni maşında proses arası rabitə | 🔥 ÇƏTİN |
| 218 | Multicast UDP | `IP_ADD_MEMBERSHIP` ilə UDP multicast qrupu yarat | 🔥 ÇƏTİN |
| 219 | Raw Socket (ICMP) | `SOCK_RAW` ilə ICMP echo (ping) paketi hazırla və göndər | 🔥 ÇƏTİN |
| 220 | TCP Nagle Alqoritmi | `TCP_NODELAY` ilə Nagle-ı söndür, gecikməni ölç | 🔥 ÇƏTİN |
| 221 | Async I/O (io_uring) | Linux `io_uring` API ilə asinxron fayl/socket əməliyyatları | 🔥 ÇƏTİN |
| 222 | QUIC Emülyasiyası | UDP üzərindən QUIC-bənzər çoxlu axın idarəsi simul et | 🔥 ÇƏTİN |
| 223 | TCP State Machine | SYN, SYN-ACK, ACK, FIN state-lərini əl ilə izlə | 🔥 ÇƏTİN |
| 224 | Bandwidth Throttling | Token bucket alqoritmi ilə göndərmə sürətini məhdudlaşdır | 🔥 ÇƏTİN |
| 225 | Socket Timeout İdarəsi | `SO_RCVTIMEO`/`SO_SNDTIMEO`, retry mexanizmi qur | 🔥 ÇƏTİN |
| 226 | TLS Handshake Simulasiyası | OpenSSL ilə TLS1.3 el-shake addımlarını idarə et | 💀 EKSPERT |
| 227 | HTTP/2 Frame Parse | HTTP/2 binary frame-lərini decode et, HEADERS/DATA ayır | 💀 EKSPERT |
| 228 | SOCKS5 Proxy Server | SOCKS5 protokolunu sıfırdan implement et (RFC 1928) | 💀 EKSPERT |
| 229 | TCP Congestion Control | CUBIC/BBR alqoritmini simul et, cwnd dinamikasını göstər | 💀 EKSPERT |
| 230 | Reliable UDP Kitabxanası | Paket itkisi, sıra, yenidən göndərmə olan UDP kitabxanası yaz | 💀 EKSPERT |
| 231 | DNS Resolver Sıfırdan | DNS sorğu paketi qur, UDP/TCP üzərindən göndər, cavabı parse et | 💀 EKSPERT |
| 232 | DHCP Client Simulasiyası | DISCOVER, OFFER, REQUEST, ACK addımlarını DHCP protokolunda keç | 💀 EKSPERT |
| 233 | WebSocket Handshake | HTTP Upgrade sorğusu + Sec-WebSocket-Accept hesabla, frame göndər | 💀 EKSPERT |
| 234 | SCTP Socket İstifadəsi | Multi-stream SCTP bağlantısı qur, TCP ilə fərqini göstər | 💀 EKSPERT |
| 235 | Network Namespace | Linux network namespace-ləri yarat, virtual şəbəkə qur | 💀 EKSPERT |
| 236 | DPDK Konsepti İmplementasiyası | Kernel bypass ilə yüksək performanslı paket prosesləmə sim. | 💀 EKSPERT |
| 237 | BGP Mesaj Parser | BGP OPEN/UPDATE/KEEPALIVE mesajlarını parse et (RFC 4271) | 💀 EKSPERT |
| 238 | OSPF Hello Packet | OSPFv2 Hello paketi hazırla, multicast adresə göndər | 💀 EKSPERT |
| 239 | NetFlow v9 Kollektoru | NetFlow v9 paketlərini dinlə, flow-ları parse et | 💀 EKSPERT |
| 240 | Şəbəkə Protokol Fuzzer | Şəbəkə protokollarına mutasiya edilmiş paketlər göndər | 💀 EKSPERT |

---

## MƏRHƏLƏ 6: Kriptoqrafiya Əsasları (241–275)
> Şifrələmə, Hash, PKI, Protokol Kriptoqrafiyası

| # | Tapşırıq Adı | Təsvir / Məqsəd | Səviyyə |
|---|---|---|---|
| 241 | XOR Şifrələmə | Sadə XOR şifrəsi yaz, açar uzunluğu ilə şifrəni sındır | ⚙️ ORTA |
| 242 | Caesar Cipher | Caesar şifrəsi + brute force deşifrə + frequency analysis | ⚙️ ORTA |
| 243 | Vigenere Cipher | Vigenere şifrəsi yaz, Kasiski testi ilə açar uzunluğunu tap | 🔥 ÇƏTİN |
| 244 | MD5 Sıfırdan | MD5 alqoritmini standartdan oxuyaraq C++ ilə implement et | 🔥 ÇƏTİN |
| 245 | SHA-256 Sıfırdan | SHA-256 bütün addımları (padding, round, digest) implement et | 🔥 ÇƏTİN |
| 246 | HMAC-SHA256 | HMAC konstruksiyasını SHA-256 üzərindən sıfırdan yaz | 🔥 ÇƏTİN |
| 247 | AES-128 ECB Modu | AES SubBytes, ShiftRows, MixColumns, AddRoundKey sıfırdan | 🔥 ÇƏTİN |
| 248 | AES CBC Modu | AES-CBC şifrələmə/deşifrə + PKCS#7 padding implement et | 🔥 ÇƏTİN |
| 249 | AES-GCM AEAD | Authenticated encryption: AES-CTR + GHASH birləşdir | 🔥 ÇƏTİN |
| 250 | RSA Açar Generasiyası | Miller-Rabin ilə böyük prime tap, e,d hesabla, N qur | 🔥 ÇƏTİN |
| 251 | RSA Şifrələmə/Deşifrə | RSA OAEP padding ilə şifrələ, side-channel qarşısı al | 🔥 ÇƏTİN |
| 252 | Diffie-Hellman | DH açar mübadiləsini sıfırdan implement et, MITM göstər | 🔥 ÇƏTİN |
| 253 | Elliptic Curve Basics | secp256k1 üzərindən nöqtə toplama/skalyar vurmə implement et | 💀 EKSPERT |
| 254 | ECDH Açar Mübadiləsi | ECDH protokolunu P-256 əyrisi üzərindən sıfırdan yaz | 💀 EKSPERT |
| 255 | Ed25519 İmza | Ed25519 imzalama/doğrulama (RFC 8032) implement et | 💀 EKSPERT |
| 256 | ChaCha20 Axın Şifrəsi | ChaCha20 quarter-round funksiyası sıfırdan (RFC 8439) | 💀 EKSPERT |
| 257 | Poly1305 MAC | Poly1305 mesaj autentifikasiya kodu implement et | 💀 EKSPERT |
| 258 | ChaCha20-Poly1305 AEAD | ChaCha20 + Poly1305 birləşdirərək tam AEAD hazırla | 💀 EKSPERT |
| 259 | Merkle Tree | Kriptoqrafik Merkle ağacı qur, proof-of-inclusion yoxla | 💀 EKSPERT |
| 260 | Blind Signature Protokolu | RSA əsaslı kor imza protokolu implement et | 💀 EKSPERT |
| 261 | Zero-Knowledge Proof (Schnorr) | Schnorr protokolunu sıfırdan implement et | 💀 EKSPERT |
| 262 | Shamir Sirrini Bölmə | SSS (k,n) threshold şemasını Galois sahəsi üzərindən yaz | 💀 EKSPERT |
| 263 | Homomorphic Encryption Sim. | Paillier şifrələməsini implement et, şifrəli toplama göstər | 💀 EKSPERT |
| 264 | Salted Password Hashing | bcrypt/Argon2 parametrlərini manual həyata keçir | 💀 EKSPERT |
| 265 | Timing Attack Demo | HMAC müqayisəsindəki vaxt sızmasını göstər, sabit vaxt düzəlt | 💀 EKSPERT |
| 266 | Padding Oracle Attack | CBC padding oracle hücumunu implement et, plaintext çıxar | 👑 MASTER |
| 267 | RC4 Bias Hücumu | RC4 statistik zəifliyini göstər, real paket şifrəsi qır | 👑 MASTER |
| 268 | Meet-in-the-Middle | 2DES-ə qarşı MITM hücumu implement et | 👑 MASTER |
| 269 | Pohlig-Hellman Alqoritmi | Aşağı mürəkkəblikli DL problemi üçün alqoritmi implement et | 👑 MASTER |
| 270 | Lattice-Based Kriptoqrafiya | LWE problemini implement et, kvant-rezistant şemayı göstər | 👑 MASTER |
| 271 | Kriptoqrafik RNG Testi | NIST SP 800-22 statistik testlərini implement et | 👑 MASTER |
| 272 | X.509 Sertifikat Parse | DER/PEM sertifikatı ASN.1 ilə parse et, sahələri çıxar | 👑 MASTER |
| 273 | Sertifikat Zənciri Doğrulama | Root CA-dan leaf-ə imza zəncirini doğrula | 👑 MASTER |
| 274 | OCSP Responderi | Online Certificate Status Protocol cavabvericiyi yaz | 👑 MASTER |
| 275 | TLS 1.3 Rekord Layer | TLS 1.3 record layer protokolunu sıfırdan implement et | 👑 MASTER |

---

## MƏRHƏLƏ 7: Sistem Proqramlaşdırması və OS Daxilişləri (276–310)
> Proses, Thread, Kernel, Sistem Çağırışları

| # | Tapşırıq Adı | Təsvir / Məqsəd | Səviyyə |
|---|---|---|---|
| 276 | ptrace ilə Debugger | `ptrace()` API ilə sadə debugger yaz, breakpoint qur | 🔥 ÇƏTİN |
| 277 | Shared Memory IPC | `shmget`/`shmat` ilə paylaşılan yaddaş idarəsi + semafor sync | 🔥 ÇƏTİN |
| 278 | Signal Handler Zənciri | Çoxlu signal handler yaz, `sigaction()` ilə alt-üst etmə et | 🔥 ÇƏTİN |
| 279 | Proses Prioritet İdarəsi | `nice()`/`setpriority()`/`sched_setscheduler()` istifadə et | 🔥 ÇƏTİN |
| 280 | Cgroup v2 İdarəsi | cgroup v2 ilə CPU/Yaddaş limitini proqramla tənzimlə | 🔥 ÇƏTİN |
| 281 | Seccomp Filter | BPF proqramı ilə sistem çağırışı filtri yaz | 🔥 ÇƏTİN |
| 282 | eBPF Xüsusi Proqram | Kernel space-də işləyən eBPF proqramı yaz + userspace kommunikasiyası | 💀 EKSPERT |
| 283 | Kernel Modul Yazı | Sadə Linux kernel modulu yaz, `/proc` faylı yarat | 💀 EKSPERT |
| 284 | Xüsusi Sistem Çağırışı | Kernel-ə xüsusi syscall əlavə et, userspace-dən test et | 💀 EKSPERT |
| 285 | Memory-mapped I/O | `mmap()` ilə fayl I/O + xüsusi yaddaş bölgəsi idarəsi | 🔥 ÇƏTİN |
| 286 | Huge Pages İstifadəsi | HugeTLB sayfalarını allok et, TLB miss azalmasını ölç | 💀 EKSPERT |
| 287 | NUMA Yaddaş İdarəsi | `numa_alloc_onnode()` ilə NUMA-aware yaddaş allokasiyası | 💀 EKSPERT |
| 288 | Proses Sandbox | Namespace+seccomp+cgroup ilə tam sandboxlanmış proses yarat | 💀 EKSPERT |
| 289 | Xüsusi Yükləyici (Loader) | ELF binary-ni disk-dən oxu, yaddaşa map et, execute et | 👑 MASTER |
| 290 | Dynamic Linker Emülasiyası | Shared library-ləri tapıb yükləyən mini `ld.so` yaz | 👑 MASTER |
| 291 | Yaddaş Ayrılma (malloc) Sıfırdan | `sbrk`/`mmap` istifadə edərək `malloc`/`free` implement et | 💀 EKSPERT |
| 292 | Lock-free Queue | CAS (compare-and-swap) ilə lock-free MPMC queue yaz | 💀 EKSPERT |
| 293 | Wait-free Stack | Wait-free push/pop olan stack implement et | 👑 MASTER |
| 294 | Hazard Pointer GC | Lock-free strukturlar üçün hazard pointer GC yaz | 👑 MASTER |
| 295 | Coroutine Kitabxanası | `setjmp`/`ucontext` ilə C++ coroutine kitabxanası yaz | 💀 EKSPERT |
| 296 | Fiber Zamanlayıcısı | Kooperativ fiber zamanlayıcısı implement et | 💀 EKSPERT |
| 297 | Kernel Thread Zamanlayıcısı | CFS bənzəri proses zamanlayıcısını userspace-də simul et | 👑 MASTER |
| 298 | Virtual Yaddaş İdarəsi | Öz page table-ını idarə edən virtual yaddaş sistemi yaz | 👑 MASTER |
| 299 | Fayl Sistemi FUSE | FUSE ilə xüsusi fayl sistemi implement et (şifrəli FS) | 👑 MASTER |
| 300 | Mini Hypervisor | KVM API istifadə edərək sadə VM çalışdıran hypervisor yaz | 👑 MASTER |
| 301 | Inter-VM Kommunikasiya | virtio-net bənzəri virtual şəbəkə interfeysi impl et | 👑 MASTER |
| 302 | CPU Emülatoru | Sadə RISC ISA üçün bytecode interpreter/JIT yaz | 👑 MASTER |
| 303 | ELF Injector | Mövcud ELF binary-yə yeni kod section əlavə et | 👑 MASTER |
| 304 | GOT/PLT Hook | Runtime-da PLT table-ı dəyişdirərək funksiya yönləndir | 👑 MASTER |
| 305 | Inline Hook Engine | x86-64 üçün inline trampoline hook mexanizmi yaz | 👑 MASTER |
| 306 | Kernel Stack Overflow | Stack canary bypass + ret2libc chain (CTF kontekstdə) | 👑 MASTER |
| 307 | ROP Chain Qurma | libc gadget-lər ilə ROP zənciri, ASLR-i bypass et | 👑 MASTER |
| 308 | Format String Exploit | Format string açığından istifadə edərək GOT yaz | 👑 MASTER |
| 309 | Heap Grooming | Tcache/fastbin zəifliyi ilə heap exploit (CTF) | 👑 MASTER |
| 310 | Kernel Exploit (ret2kernel) | Kernel module-dakı bof ilə root privilege escalation | 👑 MASTER |

---

## MƏRHƏLƏ 8: Protokolları Sıfırdan Qurmaq (311–375)
> Layer 2–7 Protokollar — Tam Implementation

| # | Tapşırıq Adı | Təsvir / Məqsəd | Səviyyə |
|---|---|---|---|
| 311 | Ethernet Frame Qurucu | Ethernet II frame-ini bit-bit qur, CRC32 hesabla, raw socket-a göndər | 👑 MASTER |
| 312 | ARP Protokolu | ARP request/reply paketlərini sıfırdan qur, ARP cədvəlini idarə et | 👑 MASTER |
| 313 | IPv4 Paket Qurucu | IPv4 başlığı, IP checksum, TTL azaldma sıfırdan implement et | 👑 MASTER |
| 314 | IPv4 Fragmentasiya | Böyük paketləri MTU-ya görə parça, fragment ID/offset hesabla | 👑 MASTER |
| 315 | IPv6 Paket Qurucu | IPv6 başlığı, uzantı başlıqları, Flow Label sıfırdan qur | 👑 MASTER |
| 316 | ICMPv4 Sıfırdan | ICMP echo/reply + time-exceeded + port-unreachable impl et | 👑 MASTER |
| 317 | ICMPv6 + NDP | Neighbor Discovery Protocol, Router Solicitation/Advertisement | 👑 MASTER |
| 318 | TCP Sıfırdan (3-Way Handshake) | SYN/SYN-ACK/ACK state machine raw socket-la impl et | 👑 MASTER |
| 319 | TCP Axın İdarəsi | Sliding window, cumulative ACK, retransmission timer impl et | 👑 MASTER |
| 320 | TCP Tıxanma İdarəsi | Slow start, congestion avoidance, fast retransmit implement et | 👑 MASTER |
| 321 | UDP Sıfırdan + Checksum | UDP datagram qurucu, pseudo-header checksum impl et | 👑 MASTER |
| 322 | DNS Query/Response | DNS sorğu paketi qur (RFC 1035), cavabı parse et, cache et | 👑 MASTER |
| 323 | DNS Recursive Resolver | Root-dan authority-yə iterativ/rekursiv DNS resolver yaz | 👑 MASTER |
| 324 | DHCP Server Sıfırdan | DHCP DISCOVER→OFFER→REQUEST→ACK dövrünü tam impl et | 👑 MASTER |
| 325 | HTTP/1.1 Server Sıfırdan | GET/POST/HEAD dəstəyi, Keep-Alive, chunked transfer impl | 👑 MASTER |
| 326 | HTTP Parser Sıfırdan | State machine ilə HTTP başlıq + gövdə parser yaz (RFC 7230) | 👑 MASTER |
| 327 | HTTP/2 Multiplexing | HTTP/2 stream multiplexing, HPACK başlıq sıxışdırması impl | 👑 MASTER |
| 328 | WebSocket Protokolu | Handshake, frame encoding/decoding, masking sıfırdan impl | 👑 MASTER |
| 329 | SMTP Müştəri Sıfırdan | EHLO, AUTH LOGIN, MAIL FROM, DATA, QUIT impl et | 👑 MASTER |
| 330 | IMAP4 Parser | IMAP4 literal string, response parser, UID FETCH impl et | 👑 MASTER |
| 331 | FTP Server Sıfırdan | Active/passive mode, PORT/PASV, LIST/RETR/STOR impl et | 👑 MASTER |
| 332 | SSH Transport Layer | SSH-2 transport: version exchange, kex_init, DH KEX impl et | 👑 MASTER |
| 333 | SSH Userauth Protokolu | SSH password + publickey auth protokolunu impl et | 👑 MASTER |
| 334 | SSH Kanal Protokolu | SSH channel open/data/eof/close, exec subsystem impl et | 👑 MASTER |
| 335 | TLS 1.3 Handshake Sıfırdan | ClientHello, ServerHello, Certificate, Finished impl et | 👑 MASTER |
| 336 | TLS 1.3 Record Layer | TLS AEAD encryption, sequence number, alert impl et | 👑 MASTER |
| 337 | DTLS 1.3 (UDP üzərindən TLS) | Datagram TLS, fragment, retransmission, epoch impl et | 👑 MASTER |
| 338 | QUIC Transport Sıfırdan | QUIC initial packet, connection ID, stream impl et (RFC 9000) | 👑 MASTER |
| 339 | WireGuard Protokolu | Noise_IK handshake + ChaCha20-Poly1305 tunnel impl et | 👑 MASTER |
| 340 | OpenVPN Protokolu Emülasiyası | TLS kontrol kanal + data kanal sıfırdan impl et | 👑 MASTER |
| 341 | BGP-4 Tam Implementasiyası | FSM, OPEN, UPDATE, path attribute, prefix filter impl et | 👑 MASTER |
| 342 | OSPF Tam Implementasiyası | Hello, DBD, LSR, LSU, LSAck, SPF hesablaması impl et | 👑 MASTER |
| 343 | IS-IS Protokolu | IS-IS PDU-ları, adjacency, LSP flooding impl et | 👑 MASTER |
| 344 | MPLS Paket Yönləndirmə | MPLS label push/pop/swap, LDP protokolunu impl et | 👑 MASTER |
| 345 | VXLAN Overlay Şəbəkəsi | VXLAN encapsulation/decapsulation, VTEP impl et | 👑 MASTER |
| 346 | RADIUS Server Sıfırdan | Access-Request, Access-Accept, AVP encode/decode impl et | 👑 MASTER |
| 347 | Diameter Protokolu | Diameter base protocol (RFC 6733) state machine impl et | 👑 MASTER |
| 348 | SNMP v3 Agent | USM autentifikasiya, priv şifrələmə, MIB walk impl et | 👑 MASTER |
| 349 | NTP Server Sıfırdan | NTP v4 paket qurucu, Stratum idarəsi, offset hesabla | 👑 MASTER |
| 350 | LDAP Server Sıfırdan | ASN.1 BER encode/decode, Bind, Search, Add impl et | 👑 MASTER |
| 351 | SIP Proxy Server | SIP INVITE, 100 Trying, 180 Ringing, 200 OK impl et | 👑 MASTER |
| 352 | RTP/RTCP Protokolu | Real-time Transport Protocol, SSRC, seqnum, jitter buffer | 👑 MASTER |
| 353 | STUN/TURN Server | NAT traversal üçün STUN/TURN protokolunu impl et | 👑 MASTER |
| 354 | ICE Alqoritmi | Interactive Connectivity Establishment sıfırdan impl et | 👑 MASTER |
| 355 | DTLS-SRTP Tunnel | WebRTC media encryption üçün DTLS-SRTP impl et | 👑 MASTER |
| 356 | Modbus TCP/RTU | Sənaye SCADA protokolu Modbus-u sıfırdan impl et | 👑 MASTER |
| 357 | DNP3 Protokolu | DNP3 frame layer, transport layer, app layer impl et | 👑 MASTER |
| 358 | IEC 61850 GOOSE | Enerji sistemi protokolu GOOSE mesaj impl et | 👑 MASTER |
| 359 | CAN Bus Protokolu | CAN frame qurucu, arbitrasiya, error frame impl et | 👑 MASTER |
| 360 | OPC-UA Xüsusi Stack | OPC-UA binary encoding, session, subscription impl et | 👑 MASTER |
| 361 | MQTT Broker Sıfırdan | MQTT 5.0: CONNECT, SUBSCRIBE, PUBLISH, QoS 0/1/2 impl et | 👑 MASTER |
| 362 | CoAP Server Sıfırdan | RFC 7252: CON/NON/ACK/RST, observe, block-wise impl et | 👑 MASTER |
| 363 | Tor Circuit Protokolu | Tor OR protokolu: cell qurucu, CREATE/EXTEND impl et | 👑 MASTER |
| 364 | I2P Tunnel Protokolu | I2P garlic routing, ElGamal + AES256 tunnel impl et | 👑 MASTER |
| 365 | Custom Mesh Routing | Öz mesh şəbəkə routing protokolunu sıfırdan yaz | 👑 MASTER |
| 366 | Custom P2P Protokolu | DHT əsaslı peer-to-peer file sharing protokolu yaz | 👑 MASTER |
| 367 | Custom RPC Framework | Öz Remote Procedure Call framework-ünü binary serialization ilə yaz | 👑 MASTER |
| 368 | Xüsusi Multiplexing Protokolu | Tək TCP bağlantısı üzərindən N virtual kanal impl et | 👑 MASTER |
| 369 | Xüsusi Şifrəli Tunnel | ECDH key exchange + ChaCha20-Poly1305 xüsusi VPN impl et | 👑 MASTER |
| 370 | Adaptive Bitrate Protokolu | DASH/HLS bənzəri ABR media streaming protokolu yaz | 👑 MASTER |
| 371 | Xüsusi Gossip Protokolu | Epidemic information spreading protokolu impl et | 👑 MASTER |
| 372 | Raft Konsensus Protokolu | Distributed consensus: leader election, log replication impl | 👑 MASTER |
| 373 | Paxos Protokolu | Multi-Paxos phase 1/2, leader, promise, accept impl et | 👑 MASTER |
| 374 | Xüsusi Blockchain Protokolu | Öz blockchain: block qurucu, PoW, peer sync impl et | 👑 MASTER |
| 375 | Kvant-Rezistant Protokol | Kyber KEM + Dilithium imza ilə post-kvant protokol impl et | 👑 MASTER |

---

## MƏRHƏLƏ 9: İleri Kibertəhlükəsizlik (376–425)
> Hücum, Müdafiə, Forensika, Red Team

| # | Tapşırıq Adı | Təsvir / Məqsəd | Səviyyə |
|---|---|---|---|
| 376 | Port Scanner Sıfırdan | SYN scan, version detection, service fingerprint yaz | 🔥 ÇƏTİN |
| 377 | Vulnerability Scanner | CVE bazası ilə xidmətlərə qarşı avtomatik zəiflik testi | 💀 EKSPERT |
| 378 | Exploit Framework Mini | Metasploit-bənzər modul sistemi: payload + exploit + post | 💀 EKSPERT |
| 379 | Shellcode Encoder | x86-64 shellcode-u null-byte-siz encode et | 💀 EKSPERT |
| 380 | Polymorphic Shellcode | Hər çalışdırmada fərqli görünən shellcode generatoru | 👑 MASTER |
| 381 | Staged Payload Delivery | Stager → stage → meterpreter-bənzər payload zənciri | 👑 MASTER |
| 382 | C2 Framework Sıfırdan | HTTP/DNS üzərindən komanda-nəzarət çərçivəsi yaz | 👑 MASTER |
| 383 | DNS Covert Channel | DNS TXT/CNAME sorğularında məlumat gizlət/çıxar | 👑 MASTER |
| 384 | ICMP Tunnel | ICMP echo payload-ında TCP tuneli impl et | 👑 MASTER |
| 385 | HTTP Covert Channel | HTTP başlıqlarında gizli məlumat kanalı qur | 👑 MASTER |
| 386 | Şifrəli C2 Protokolu | mTLS + certificate pinning ilə C2 rabitəsi | 👑 MASTER |
| 387 | Anti-forensic Texnikalar | Yaddaşda iz silmə, timestamp tamper, slack space gizlətmə | 👑 MASTER |
| 388 | Kernel Rootkit | Syscall hook ilə proses/fayl gizlədən kernel rootkit | 👑 MASTER |
| 389 | Userspace Rootkit | `LD_PRELOAD` ilə libc funksiyalarını hook et | 💀 EKSPERT |
| 390 | Memory Forensik Tool | `/proc/pid/mem`-dən canlı yaddaş dump + artifact çıxar | 💀 EKSPERT |
| 391 | Volatility Plugin Yaz | Volatility3 üçün xüsusi yaddaş analiz plugin yaz | 💀 EKSPERT |
| 392 | Network Forensik | PCAP-ı oxu, session reconstruct, artefakt çıxar | 💀 EKSPERT |
| 393 | Malware Analiz Sandbox | Syscall log + şəbəkə davranışı + fayl dəyişiklik sandbox | 👑 MASTER |
| 394 | Fuzzer Sıfırdan (Coverage) | AFL bənzəri coverage-guided fuzzer yaz | 👑 MASTER |
| 395 | libFuzzer İstifadəsi | libFuzzer target + corpus + sanitizer ilə bug tap | 💀 EKSPERT |
| 396 | Symbolic Execution | Z3 solver ilə sadə proqram analizi (KLEE konsepti) | 👑 MASTER |
| 397 | Taint Analysis | Dataflow taint tracking ilə input-a bağlı hər yolu izlə | 👑 MASTER |
| 398 | Binary Diffing | İki binary-nin fərqini tap, patch analizi et | 💀 EKSPERT |
| 399 | IDA/Ghidra Plugin | RE tool üçün xüsusi analiz plugin yaz | 💀 EKSPERT |
| 400 | Heap Spray Texnikası | ASLR bypass üçün heap spray + use-after-free impl et | 👑 MASTER |
| 401 | Browser Exploit Konsepti | V8/SpiderMonkey JIT heap grooming konsseptini impl et | 👑 MASTER |
| 402 | SMB Exploit Sıfırdan | SMBv1 protokolu ilə EternalBlue-bənzər exploit yaz | 👑 MASTER |
| 403 | Active Directory Attack | Kerberoasting, Pass-the-Hash, DCSync impl et | 👑 MASTER |
| 404 | OAuth Token Hijacking | Authorization code flow-da açıq redirect istismarı | 💀 EKSPERT |
| 405 | JWT Algorithm Confusion | RS256→HS256 keçid ilə JWT imzasını saxtalaşdır | 💀 EKSPERT |
| 406 | GraphQL Introspection Attack | Schema extraction + batch query + injection impl et | 💀 EKSPERT |
| 407 | gRPC Security Test | Protobuf fuzzing + authentication bypass test | 💀 EKSPERT |
| 408 | Supply Chain Attack Sim. | Paket registry-yə trojanized paket inject sim et | 👑 MASTER |
| 409 | Firmware Analysis | IoT firmware-ini unpack, hardcoded cred tap, CVE scan | 💀 EKSPERT |
| 410 | Hardware Debug Interface | JTAG/UART ilə embedded cihazdan debug məlumatı çıxar | 👑 MASTER |
| 411 | Side-Channel (Cache) | Flush+Reload cache timing hücumunu implement et | 👑 MASTER |
| 412 | Spectre v1 Demo | Spectre variant 1 PoC C++ kodu yaz, izolasiya test et | 👑 MASTER |
| 413 | Rowhammer Demo | DRAM bit-flip hücumunu sənədləşdir, PoC yaz | 👑 MASTER |
| 414 | Power Analysis Sim. | Simulated power trace ilə AES açar recover et | 👑 MASTER |
| 415 | DMA Attack Sim. | Thunderbolt/PCIe DMA üzərindən yaddaş oxuma sim | 👑 MASTER |
| 416 | RF Signal Capture | RTL-SDR ilə 433MHz remote signal capture + decode | 💀 EKSPERT |
| 417 | Bluetooth LE Sniffer | BTLE paket sniffer yaz, pairing process analiz et | 💀 EKSPERT |
| 418 | Wi-Fi Deauth Sim. | 802.11 management frame injection PoC (test mühiti) | 💀 EKSPERT |
| 419 | Cellular SS7 Attack Sim. | SS7 MAP protocol açıq: SMS intercept konsepti | 👑 MASTER |
| 420 | RFID Clone Simulasiyası | MIFARE Classic sektoru oxu + klon simulasiyası | 💀 EKSPERT |
| 421 | PKI Sertifikat Saxtalaması | Zəif CA imzası ilə sertifikat zəncirini klon et | 👑 MASTER |
| 422 | BGP Hijack Simulasiyası | Exabgp ilə BGP prefix hijack demo (izolə mühit) | 👑 MASTER |
| 423 | ARP Spoofing Framework | ARP spoof + MITM + SSL stripping aracı yaz | 💀 EKSPERT |
| 424 | DNS Rebinding Attack | DNS TTL manipulyasiyası ilə same-origin bypass impl et | 👑 MASTER |
| 425 | Xüsusi Exploit Mitigation | CFI, CET, Shadow Stack, PIE+ASLR mitigation testi | 👑 MASTER |

---

## MƏRHƏLƏ 10: Tam Protokol + Sistem Arxitekturası (426–500)
> 🛡️ Production Səviyyəli Tam Sistemlər

| # | Tapşırıq Adı | Təsvir / Məqsəd | Səviyyə |
|---|---|---|---|
| 426 | Tam TLS Stack Sıfırdan | TLS 1.3 + TLS 1.2 fallback, session resumption, 0-RTT impl et | 🛡️ PROTOKOL |
| 427 | HTTPS Server Sıfırdan | TLS stack + HTTP/2 multiplex + gzip + HTTP/3 üzərindən | 🛡️ PROTOKOL |
| 428 | VPN Server+Müştəri | WireGuard bənzəri tam VPN: tun/tap, routing, NAT impl et | 🛡️ PROTOKOL |
| 429 | Tam SSH Server Sıfırdan | Autentifikasiya, kanal multiplex, port forwarding impl et | 🛡️ PROTOKOL |
| 430 | Tam SMTP/IMAP/POP3 | Mail server: TLS, SPF/DKIM/DMARC doğrulama impl et | 🛡️ PROTOKOL |
| 431 | DNS Authoritative Server | DNSSEC imzalama, zone transfer, dynamic DNS impl et | 🛡️ PROTOKOL |
| 432 | HTTP Proxy Sıfırdan | CONNECT, cache, SSL inspection, transparent proxy impl et | 🛡️ PROTOKOL |
| 433 | Reverse Proxy + LB | L7 load balancer, health check, sticky session impl et | 🛡️ PROTOKOL |
| 434 | API Gateway Sıfırdan | Rate limiting, auth, routing, mTLS, plugin sistem impl et | 🛡️ PROTOKOL |
| 435 | Service Mesh Protokolu | Envoy-bənzər sidecar: mTLS, telemetri, circuit breaker | 🛡️ PROTOKOL |
| 436 | Custom Message Broker | Kafka-bənzər: topic, partition, consumer group, replication | 🛡️ PROTOKOL |
| 437 | Distributed Cache Protokolu | Redis-bənzər: RESP protokolu, cluster, sentinel impl et | 🛡️ PROTOKOL |
| 438 | Tam Raft Implementasiyası | Leader seçim + log replication + snapshotting + cluster join | 🛡️ PROTOKOL |
| 439 | Distributed Lock Manager | ETCD-bənzər: lease, watch, distributed lock impl et | 🛡️ PROTOKOL |
| 440 | Container Network Plugin | CNI plugin yaz: veth pair, bridge, iptables rules impl et | 🛡️ PROTOKOL |
| 441 | Kubernetes Şəbəkə Modeli | kube-proxy-bənzər: iptables/ipvs service routing impl et | 🛡️ PROTOKOL |
| 442 | eBPF Network Accelerator | XDP/TC eBPF proqramı ilə paket prosesləmə bypass kernel | 🛡️ PROTOKOL |
| 443 | Tam IDS/IPS Sistemi | Snort-bənzər: rule engine, protocol decode, alert impl et | 🛡️ PROTOKOL |
| 444 | Network Anomaly Detector | ML əsaslı şəbəkə anomaly detection sistemi yaz | 🛡️ PROTOKOL |
| 445 | SIEM Toplama Motoru | Log toplama, correlation, alert, normalization impl et | 🛡️ PROTOKOL |
| 446 | Zero Trust Şəbəkə | SPIFFE/SPIRE bənzəri ID + mTLS + policy engine impl et | 🛡️ PROTOKOL |
| 447 | HSM Emülatoru | Hardware Security Module-u yumşaq implement et | 🛡️ PROTOKOL |
| 448 | PKI CA Sistemi | Root CA, intermediate CA, CRL, OCSP tam impl et | 🛡️ PROTOKOL |
| 449 | PKCS#11 Provider | Kriptoqrafik token interfeysi standartını impl et | 🛡️ PROTOKOL |
| 450 | Tam Firewall Yazı | Stateful firewall: conntrack, NAT, L7 inspection impl et | 🛡️ PROTOKOL |
| 451 | DPI (Deep Packet Inspection) | Hyperscan əsaslı pattern matching + protocol decode | 🛡️ PROTOKOL |
| 452 | Honeynet Sistemi | Birdən çox fake xidmət + attacker davranış analizi | 🛡️ PROTOKOL |
| 453 | Threat Intelligence Platform | IOC toplama, STIX/TAXII, feed korrelyasiyası impl et | 🛡️ PROTOKOL |
| 454 | Tam SOC Avtomatlaşdırması | Alert triage, playbook icra, ticket yaratma impl et | 🛡️ PROTOKOL |
| 455 | Xüsusi Antivirus Motoru | Signature + heuristic + sandbox + ML detect engine | 🛡️ PROTOKOL |
| 456 | EDR Agent Sıfırdan | Syscall hook + file/proc/net telemetri + C2 comm impl et | 🛡️ PROTOKOL |
| 457 | Custom Hypervisor İntrospection | VMI ilə guest OS yaddaş analizi impl et | 🛡️ PROTOKOL |
| 458 | Firmware Güvənlik Çərçivəsi | Secure boot + attestation + update signing impl et | 🛡️ PROTOKOL |
| 459 | HSM Protocol Bridge | PKCS#11 ↔ TPM 2.0 ↔ custom protocol bridge yaz | 🛡️ PROTOKOL |
| 460 | Kvant Kriptoqrafiya Keçişi | Hybrid klasik+post-kvant TLS extension impl et | 🛡️ PROTOKOL |
| 461 | MPC Protokolu | Secure multi-party computation protokolu impl et | 🛡️ PROTOKOL |
| 462 | Oblivious RAM (ORAM) | Path ORAM protokolunu sıfırdan implement et | 🛡️ PROTOKOL |
| 463 | Private Information Retrieval | PIR protokolunu implement et, query gizliliyini göstər | 🛡️ PROTOKOL |
| 464 | Fully Homomorphic Encryption | BFV/CKKS şeması üzərindən FHE hesablamaları impl et | 🛡️ PROTOKOL |
| 465 | Tam Blockchain Node | P2P, transaction pool, block mine, fork resolution impl et | 🛡️ PROTOKOL |
| 466 | Smart Contract VM | Öz bytecode VM-ni yaz, smart contract execute et | 🛡️ PROTOKOL |
| 467 | Lightning Network Kanali | Payment channel: HTLC, revocation, routing impl et | 🛡️ PROTOKOL |
| 468 | FIDO2/WebAuthn Server | Authenticator registration + assertion protokolu impl et | 🛡️ PROTOKOL |
| 469 | OAuth 2.1 Server | Authorization server: PKCE, refresh, token introspection | 🛡️ PROTOKOL |
| 470 | OpenID Connect Provider | OIDC discovery, userinfo, JWKS, ID token impl et | 🛡️ PROTOKOL |
| 471 | SAML 2.0 IdP Sıfırdan | XML imzalama, assertion, artifact binding impl et | 🛡️ PROTOKOL |
| 472 | Kerberos v5 Tam Impl. | KDC, TGT, TGS, ST, PKINIT impl et (RFC 4120) | 🛡️ PROTOKOL |
| 473 | NTLM Challenge-Response | NTLMv2 protokolunu sıfırdan implement et | 🛡️ PROTOKOL |
| 474 | S/MIME Tam Implementasiyası | CMS imzalama + şifrələmə + sertifikat doğrulama impl et | 🛡️ PROTOKOL |
| 475 | PGP/OpenPGP Protokolu | OpenPGP packet format, keyring, WoT impl et (RFC 4880) | 🛡️ PROTOKOL |
| 476 | Secure Messaging (Signal) | X3DH + Double Ratchet protokolunu sıfırdan impl et | 🛡️ PROTOKOL |
| 477 | Onion Routing Sistemi | Tor-bənzər: directory server + circuit + cell routing impl | 🛡️ PROTOKOL |
| 478 | Mixnet Protokolu | Loopix/Sphinx packet format ilə mixnet impl et | 🛡️ PROTOKOL |
| 479 | Anonim Kimlik Doğrulama | DAA (Direct Anonymous Attestation) protokolu impl et | 🛡️ PROTOKOL |
| 480 | Group Signature Sxemi | BBS+ group signature protokolunu sıfırdan implement et | 🛡️ PROTOKOL |
| 481 | Tam IKEv2/IPsec Stack | IKEv2 SA negotiation + ESP/AH transform impl et | 🛡️ PROTOKOL |
| 482 | SCEP Sertifikat Enrolment | Simple Certificate Enrollment Protocol impl et | 🛡️ PROTOKOL |
| 483 | EST (RFC 7030) Protokolu | Enrollment over Secure Transport protokolu impl et | 🛡️ PROTOKOL |
| 484 | CMP Protokolu | Certificate Management Protocol (RFC 4210) impl et | 🛡️ PROTOKOL |
| 485 | ACME Protokolu Sıfırdan | Let's Encrypt ACME (RFC 8555): challenge, order, cert impl | 🛡️ PROTOKOL |
| 486 | Xüsusi HSM Protokolu | Öz HSM protokolunu yaz: əmr seti, session, audit impl et | 🛡️ PROTOKOL |
| 487 | Xüsusi Audit Protokolu | Tamper-evident, verifiable audit log protokolu impl et | 🛡️ PROTOKOL |
| 488 | Xüsusi Attestation Protokolu | Remote attestation: nonce, quote, verify impl et | 🛡️ PROTOKOL |
| 489 | Tam QUIC Tətbiqi | QUIC + HTTP/3 + 0-RTT + connection migration impl et | 🛡️ PROTOKOL |
| 490 | Custom SDN Protokolu | OpenFlow-bənzər SDN kontrol protokolu impl et | 🛡️ PROTOKOL |
| 491 | Xüsusi P4 Pipeline | P4 proqramlanabilir data plane impl et (P4Runtime) | 🛡️ PROTOKOL |
| 492 | Tam Network Emülatoru | Mininet-bənzər: virtual topo, link delay, packet loss | 🛡️ PROTOKOL |
| 493 | Xüsusi Fuzzing Framework | Coverage+mutation+grammar-based fuzzer impl et | 🛡️ PROTOKOL |
| 494 | Xüsusi Binary Analysis | angr-bənzər: CFG, taint, constraint solving impl et | 🛡️ PROTOKOL |
| 495 | Production C2 Framework | Malleable C2, domain fronting, modullar implant impl et | 🛡️ PROTOKOL |
| 496 | Tam EDR + XDR Sistemi | Agent + Server + analitika + MITRE ATT&CK mapping | 🛡️ PROTOKOL |
| 497 | Sıfır Günlük Araşdırma Aləti | Fuzzing + taint + SMT solver birləşdirən 0-day finder | 🛡️ PROTOKOL |
| 498 | Tam Kibertəhlükəsizlik Platforması | SIEM + SOAR + TI + EDR birləşdirən inteqrasiya qatı yaz | 🛡️ PROTOKOL |
| 499 | Öz Kriptoqrafik Protokol Standarti | RFC formatında xüsusi kriptoqrafik protokol dizayn et + impl | 🛡️ PROTOKOL |
| 500 | Tam Şifrəli Şəbəkə OS | Miniature güvənli OS: bootloader, network stack, crypto impl | 🛡️ PROTOKOL |

---

## 📊 Xülasə Statistikası

| Mərhələ | Tapşırıqlar | Mövzu | Min Səviyyə |
|---|---|---|---|
| 5 | 201–240 | Şəbəkə Proqramlaşdırması | ⚙️ ORTA |
| 6 | 241–275 | Kriptoqrafiya | ⚙️ ORTA → 👑 MASTER |
| 7 | 276–310 | Sistem Proqramlaşdırması | 🔥 ÇƏTİN → 👑 MASTER |
| 8 | 311–375 | Protokolları Sıfırdan | 👑 MASTER |
| 9 | 376–425 | İleri Kibertəhlükəsizlik | 🔥 ÇƏTİN → 👑 MASTER |
| 10 | 426–500 | Production Sistemlər | 🛡️ PROTOKOL |

> **💡 Tövsiyə:** Hər tapşırığı tamamladıqdan sonra kodu GitHub-a yükləyin və README-yə texniki qeydlər əlavə edin. Bu, portfelunuzu gücləndirir.

---

*C++ & Kibertəhlükəsizlik Roadmap — Tapşırıqlar 201–500*
