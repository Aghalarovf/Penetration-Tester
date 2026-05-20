# 🗺️ AD Enumeration Agent — C++ Development Roadmap
### 100 Addımlı Professional İnkişaf Planı

---

## 📋 Ümumi Arxitektura

```
ad-enum/
├── CMakeLists.txt
├── src/
│   ├── main.cpp
│   ├── cli/
│   │   ├── args_parser.cpp / .h
│   │   └── banner.cpp / .h
│   ├── core/
│   │   ├── ldap_client.cpp / .h
│   │   ├── kerberos_auth.cpp / .h
│   │   └── ntlm_auth.cpp / .h
│   ├── modules/
│   │   ├── users.cpp / .h
│   │   ├── computers.cpp / .h
│   │   ├── groups.cpp / .h
│   │   ├── gpo.cpp / .h
│   │   ├── ou.cpp / .h
│   │   ├── trusts.cpp / .h
│   │   └── dacl.cpp / .h
│   ├── output/
│   │   ├── formatter.cpp / .h
│   │   ├── json_writer.cpp / .h
│   │   └── csv_writer.cpp / .h
│   ├── generator/
│   │   ├── build_config.cpp / .h
│   │   └── output_wrapper.cpp / .h
│   └── utils/
│       ├── thread_pool.cpp / .h
│       ├── logger.cpp / .h
│       └── network.cpp / .h
└── tests/
    └── ...
```

---

## 🔵 PHASE 1 — Layihə Əsası və Mühit Qurulması (Addım 1–10)

### Addım 1 — CMake Layihə Skeleti
- `CMakeLists.txt` yarat (minimum_required VERSION 3.20)
- `project(ad-enum VERSION 0.1.0 LANGUAGES CXX)` təyin et
- `set(CMAKE_CXX_STANDARD 20)` ilə C++20 standartını məcburi et
- Debug/Release build tiplərini konfiqurasiya et

### Addım 2 — Qovluq Strukturu
- `src/`, `include/`, `tests/`, `docs/`, `scripts/` qovluqlarını yarat
- `src/` altında `cli/`, `core/`, `modules/`, `output/`, `generator/`, `utils/` alt-qovluqları əlavə et
- `.gitignore` faylını yarat (build/, *.o, *.a, CMakeCache.txt)

### Addım 3 — Asılılıq Menecmenti
- `vcpkg.json` manifest faylı yarat
- Lazım olan kitabxanaları əlavə et:
  - `openldap` — LDAP protokolu üçün
  - `nlohmann-json` — JSON output üçün
  - `cxxopts` — CLI argument parsing üçün
  - `spdlog` — structured logging üçün
  - `gtest` — unit testlər üçün
- CMakeLists-ə `find_package()` çağırışlarını əlavə et

### Addım 4 — Kompilyator Flagları
```cmake
add_compile_options(
  -Wall -Wextra -Wpedantic   # Bütün xəbərdarlıqlar
  -O2                         # Release-də optimallaşdırma
  -fstack-protector-strong    # Stack qoruması
  -D_FORTIFY_SOURCE=2         # Buffer overflow qoruması
)
```
- Address Sanitizer (Debug) konfiqurasiyası əlavə et

### Addım 5 — Minimum `main.cpp` Skeleti
```cpp
#include <iostream>

int main(int argc, char* argv[]) {
    std::cout << "AD Enum Agent v0.1" << std::endl;
    return 0;
}
```
- CMake ilə build et, uğurla compile olduğunu yoxla

### Addım 6 — Logger Modulu (`utils/logger.h/.cpp`)
- `spdlog` kitabxanası ilə wrapper sinif yarat
- Log səviyyələri: `DEBUG`, `INFO`, `WARN`, `ERROR`, `CRITICAL`
- Fayl + konsol çıxışı konfiqurasiyası
- `--verbose` flag ilə debug logu aktiv et

### Addım 7 — Xəta İdarəetmə Sistemi
- `ADEnumException` base sinfi yarat
- Alt siniflər: `LDAPException`, `AuthException`, `NetworkException`, `ConfigException`
- RAII prinsipinə uyğun exception-safe kod qaydaları müəyyənləşdir

### Addım 8 — Konfiqurasiya Strukturları (`core/config.h`)
```cpp
struct AgentConfig {
    std::string domain;
    std::string dc_ip;
    std::string ldap_user;
    std::string ldap_pass;
    AuthType    auth_type;  // BASIC / KERBEROS / NTLM
    uint32_t    threads;
    uint32_t    timeout;
    uint32_t    page_size;
    bool        no_ping;
    // ... sair parametrlər
};

struct GeneratorConfig {
    OutputType  output_type;  // EXE / DLL / POWERSHELL
    std::string agent_name;
    std::string export_path;
    Architecture arch;        // X64 / X86 / ARM64
    std::string icon_path;
};
```

### Addım 9 — İlk Unit Test Infrastrukturu
- `tests/CMakeLists.txt` yarat
- `test_logger.cpp` ilə spdlog inteqrasiyasını test et
- `test_config.cpp` ilə konfiq struct-larını test et
- `ctest` ilə test pipeline-ı qur

### Addım 10 — Banner və Version Modulu (`cli/banner.cpp`)
- ASCII art banner funksiyası yarat
- Version bilgisi: `v0.1.0-dev`
- Build tarixi/vaxtı: `__DATE__` / `__TIME__` macro-ları ilə
- `--version` flag-i ilə çap et

---

## 🟢 PHASE 2 — CLI Argument Parser (Addım 11–20)

### Addım 11 — `cxxopts` İnteqrasiyası
- `cli/args_parser.h` faylı yarat
- `ArgsParser` sinfi müəyyənləşdir
- `parse(int argc, char* argv[])` metodu yaz

### Addım 12 — Generator Flagları — Output Növü
```cpp
options.add_options("Generator")
    ("d,delivery", "Output type: powershell|exe|dll",
        cxxopts::value<std::string>()->default_value("exe"))
```
- Enum: `OutputType { EXE, DLL, POWERSHELL }`
- Yanlış dəyər daxil edilərsə xəta mesajı ver

### Addım 13 — Generator Flagları — Ad, Path, Arxitektura
```cpp
("n,name",        "Agent name",        cxxopts::value<std::string>())
("export-path",   "Export path",       cxxopts::value<std::string>())
("a,arch",        "Architecture x64|x86|arm64",
                  cxxopts::value<std::string>()->default_value("x64"))
("ic,icon",       "Icon path (.ico)",  cxxopts::value<std::string>())
```
- Path validasiyası: mövcud qovluq yoxla

### Addım 14 — Agent Flagları — Domain və DC
```cpp
options.add_options("Agent")
    ("domain",    "Target domain",      cxxopts::value<std::string>())
    ("dc",        "DC IP/hostname",     cxxopts::value<std::string>())
```
- Domain format yoxlaması: regex ilə (`[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}`)
- IP format yoxlaması: IPv4 regex

### Addım 15 — Agent Flagları — LDAP Credentials
```cpp
("ldap-user",  "LDAP username",   cxxopts::value<std::string>())
("ldap-pass",  "LDAP password",   cxxopts::value<std::string>())
("ldap-auth",  "Auth: basic|kerberos|ntlm",
               cxxopts::value<std::string>()->default_value("basic"))
```
- `AuthType` enum: `BASIC`, `KERBEROS`, `NTLM`

### Addım 16 — Agent Flagları — Performans Parametrləri
```cpp
("threads",   "Thread count",   cxxopts::value<uint32_t>()->default_value("4"))
("timeout",   "LDAP timeout",   cxxopts::value<uint32_t>()->default_value("30"))
("page-size", "LDAP page size", cxxopts::value<uint32_t>()->default_value("1000"))
("no-ping",   "Skip ping check", cxxopts::value<bool>()->default_value("false"))
```

### Addım 17 — Enum Modulu Seçimləri (Boolean Flaglar)
```cpp
("user",     "Enumerate users")
("computer", "Enumerate computers")
("gpo",      "Enumerate GPOs")
("group",    "Enumerate groups and members")
("trust",    "Enumerate trusts")
("ou",       "Enumerate OUs")
("dacl",     "Enumerate ACE/ACL")
("snapshot", "Full AD snapshot")
```
- `--snapshot` flag-i aktiv edildikdə bütün flagları `true` et

### Addım 18 — Validasiya Məntiqi
- `validate()` metodu yarat
- Məcburi field yoxlaması: `--domain`, `--dc`, `--ldap-user`
- Ən az bir enum flag-inin aktiv olmasını yoxla (`--snapshot` istisna)
- Yol mövcudluğu yoxlaması (`--export-path`, `--icon`)

### Addım 19 — Help Mətninin Formatlanması
- Hər flag qrupu üçün ayrı section göstər
- Nümunə istifadə göstər:
```
Nümunə:
  ad-enum --domain corp.local --dc 192.168.1.1 \
          --ldap-user admin --ldap-pass P@ss! \
          --user --group --threads 8
```

### Addım 20 — ArgParser Unit Testləri
- Düzgün flagların parse edilməsini test et
- Yanlış flagların exception atmasını test et
- `--snapshot` flag-inin bütün flagları aktiv etməsini test et
- Boş argument-in help çap etməsini test et

---

## 🟡 PHASE 3 — Şəbəkə və LDAP Əsası (Addım 21–35)

### Addım 21 — Ping Yoxlaması (`utils/network.cpp`)
```cpp
bool ping_host(const std::string& host, uint32_t timeout_ms);
```
- ICMP socket ilə ping at (Windows: `IcmpSendEcho`, Linux: raw socket)
- `--no-ping` flag-i aktiv olduqda bu addımı keç

### Addım 22 — DNS Resolve
```cpp
std::string resolve_hostname(const std::string& hostname);
```
- `getaddrinfo()` ilə hostname → IP çevirmə
- IPv4 və IPv6 dəstəyi
- Cache mexanizmi: `std::unordered_map<string,string>`

### Addım 23 — LDAP Client Sinfi (`core/ldap_client.h`)
```cpp
class LDAPClient {
public:
    LDAPClient(const AgentConfig& config);
    ~LDAPClient();  // RAII: LDAP bağlantısını bağla

    bool connect();
    bool authenticate();
    LDAPResult search(const std::string& base_dn,
                      int scope,
                      const std::string& filter,
                      const std::vector<std::string>& attrs);
    void disconnect();
private:
    LDAP* ld_;
    AgentConfig config_;
};
```

### Addım 24 — LDAP Bağlantısı
```cpp
bool LDAPClient::connect() {
    ld_ = ldap_init(config_.dc_ip.c_str(), LDAP_PORT);
    int version = LDAP_VERSION3;
    ldap_set_option(ld_, LDAP_OPT_PROTOCOL_VERSION, &version);
    ldap_set_option(ld_, LDAP_OPT_NETWORK_TIMEOUT, &tv);
    // TLS/SSL seçimi
}
```
- Port: LDAP=389, LDAPS=636
- `--timeout` dəyəri ilə `timeval` struct-ı qur

### Addım 25 — Simple/Basic Auth
```cpp
bool LDAPClient::authenticate_simple() {
    struct berval cred;
    cred.bv_val = const_cast<char*>(config_.ldap_pass.c_str());
    cred.bv_len = config_.ldap_pass.length();
    return ldap_sasl_bind_s(ld_, dn.c_str(),
                            LDAP_SASL_SIMPLE, &cred,
                            nullptr, nullptr, nullptr) == LDAP_SUCCESS;
}
```

### Addım 26 — NTLM Auth
```cpp
bool LDAPClient::authenticate_ntlm() {
    // SASL NTLM mexanizmi
    return ldap_sasl_interactive_bind_s(ld_, nullptr, "NTLM",
                                        nullptr, nullptr,
                                        LDAP_SASL_QUIET,
                                        sasl_interact_callback,
                                        nullptr) == LDAP_SUCCESS;
}
```

### Addım 27 — Kerberos Auth
```cpp
bool LDAPClient::authenticate_kerberos() {
    // GSSAPI / Kerberos5 SASL mexanizmi
    return ldap_sasl_interactive_bind_s(ld_, nullptr, "GSSAPI",
                                        nullptr, nullptr,
                                        LDAP_SASL_QUIET,
                                        sasl_interact_callback,
                                        nullptr) == LDAP_SUCCESS;
}
```
- `KRB5_KTNAME` environment variable dəstəyi

### Addım 28 — LDAP Search Wrapper
```cpp
LDAPResult LDAPClient::search(const std::string& base_dn,
                               int scope,
                               const std::string& filter,
                               const std::vector<std::string>& attrs) {
    LDAPMessage* result = nullptr;
    char** attr_array = build_attr_array(attrs);
    int rc = ldap_search_ext_s(ld_, base_dn.c_str(), scope,
                                filter.c_str(), attr_array,
                                0, nullptr, nullptr,
                                &timeout_, 0, &result);
    // Nəticəni parse et, LDAPResult-a çevir
    ldap_msgfree(result);
    return parsed_result;
}
```

### Addım 29 — Paged Results (Böyük Domainlər)
```cpp
void LDAPClient::search_paged(const std::string& base_dn,
                               const std::string& filter,
                               const std::vector<std::string>& attrs,
                               std::function<void(LDAPEntry)> callback) {
    // ldap_create_page_control() ilə --page-size addım-addım oxu
    // Cookie-ni saxla, növbəti səhifəni çəkməyə davam et
}
```

### Addım 30 — `LDAPEntry` Data Strukturu
```cpp
struct LDAPEntry {
    std::string dn;
    std::unordered_map<std::string, std::vector<std::string>> attributes;
    std::string get(const std::string& attr) const;
    std::vector<std::string> get_all(const std::string& attr) const;
    bool has(const std::string& attr) const;
};
```

### Addım 31 — Base DN Avtomatik Alınması
```cpp
std::string LDAPClient::get_root_dse() {
    // RootDSE-dən defaultNamingContext oxu
    // "DC=corp,DC=local" formatında qaytar
}
std::string domain_to_dn(const std::string& domain);
// "corp.local" → "DC=corp,DC=local"
```

### Addım 32 — LDAP Xəta Kodları
- Bütün LDAP xəta kodlarını enum-a çevir
- `ldap_err2string()` ilə human-readable mesajlar
- Retry məntiqi: `LDAP_SERVER_DOWN`, `LDAP_TIMEOUT` üçün 3 cəhd

### Addım 33 — Bağlantı Unit Testləri
- Mock LDAP server ilə test et (OpenLDAP test server)
- Uğurlu autentifikasiya scenariosunu test et
- Yanlış credentials ilə xəta senaryosu
- Timeout senaryosu

### Addım 34 — TLS/LDAPS Dəstəyi
```cpp
// ldap_start_tls_s() ilə STARTTLS
// LDAP_OPT_X_TLS_REQUIRE_CERT seçimi
// Sertifikat yoxlamasını söndürmə (--no-tls-verify flag)
```

### Addım 35 — LDAP Connection Pool
```cpp
class LDAPPool {
    std::vector<std::unique_ptr<LDAPClient>> pool_;
    std::mutex mutex_;
    std::condition_variable cv_;
public:
    std::shared_ptr<LDAPClient> acquire();
    void release(std::shared_ptr<LDAPClient>);
};
```

---

## 🟠 PHASE 4 — Thread Pool və Paralel Enum (Addım 36–45)

### Addım 36 — Thread Pool İmplementasiyası (`utils/thread_pool.h`)
```cpp
class ThreadPool {
    std::vector<std::thread> workers_;
    std::queue<std::function<void()>> tasks_;
    std::mutex queue_mutex_;
    std::condition_variable condition_;
    std::atomic<bool> stop_{false};
public:
    explicit ThreadPool(size_t threads);
    ~ThreadPool();
    template<class F, class... Args>
    auto enqueue(F&& f, Args&&... args)
        -> std::future<std::invoke_result_t<F, Args...>>;
};
```

### Addım 37 — Work Stealing Queue (Performans)
- Thread pool-a work stealing əlavə et
- Lock-free deque ilə (mümkünsə)
- Thread sayını `--threads` parametri ilə dinamik tənzimlə

### Addım 38 — Progress İndikatoru
```cpp
class ProgressBar {
    std::atomic<uint64_t> current_{0};
    uint64_t total_;
    std::mutex print_mutex_;
public:
    void increment();
    void render();  // \r ilə konsola yaz
    void finish();
};
```
- TTY yoxlaması: `isatty(STDOUT_FILENO)` — redirect edilərsə gizlət

### Addım 39 — Rate Limiting
```cpp
class RateLimiter {
    std::chrono::steady_clock::time_point last_request_;
    std::mutex mutex_;
    uint32_t max_rps_;  // Max requests per second
public:
    void acquire();  // Lazım olsa gözlə
};
```
- Böyük domainlərdə DC-ni yükləməmək üçün

### Addım 40 — Paralel Enum Koordinatoru
```cpp
class EnumCoordinator {
    ThreadPool pool_;
    LDAPPool   ldap_pool_;
    ProgressBar progress_;
public:
    void run(const AgentConfig& config);
    void enqueue_module(std::unique_ptr<IEnumModule> module);
};
```

### Addım 41 — `IEnumModule` İnterfeysi
```cpp
class IEnumModule {
public:
    virtual ~IEnumModule() = default;
    virtual std::string name() const = 0;
    virtual std::vector<LDAPEntry> enumerate(LDAPClient& client) = 0;
    virtual std::string ldap_filter() const = 0;
    virtual std::vector<std::string> attributes() const = 0;
};
```

### Addım 42 — Modul Qeydiyyatı (Factory Pattern)
```cpp
class ModuleRegistry {
    std::unordered_map<std::string,
        std::function<std::unique_ptr<IEnumModule>()>> registry_;
public:
    void register_module(const std::string& name, ...);
    std::unique_ptr<IEnumModule> create(const std::string& name);
    std::vector<std::string> available_modules();
};
```

### Addım 43 — Thread-Safe Nəticə Kollektoru
```cpp
class ResultCollector {
    std::unordered_map<std::string, std::vector<LDAPEntry>> results_;
    std::shared_mutex mutex_;
public:
    void add(const std::string& module, LDAPEntry entry);
    std::vector<LDAPEntry> get(const std::string& module) const;
    size_t total_count() const;
};
```

### Addım 44 — İptal Mexanizmi (Graceful Shutdown)
```cpp
std::atomic<bool> g_cancel_requested{false};
// SIGINT / SIGTERM signal handler
void signal_handler(int sig) {
    g_cancel_requested.store(true);
}
// Hər enum dövrü `g_cancel_requested` yoxlasın
```

### Addım 45 — Thread Pool Unit Testləri
- 100 tapşırıq ilə thread pool-u test et
- Thread count dəyişikliyini test et
- Xəta halında exception propagation-ı test et

---

## 🔴 PHASE 5 — Enum Modulları (Addım 46–65)

### Addım 46 — User Enum Modulu (`modules/users.cpp`)
- LDAP filter: `(&(objectClass=user)(objectCategory=person))`
- Atributlar: `sAMAccountName`, `displayName`, `mail`, `userPrincipalName`,
  `memberOf`, `userAccountControl`, `lastLogon`, `pwdLastSet`,
  `adminCount`, `description`, `distinguishedName`

### Addım 47 — User Flag Analizi
```cpp
struct UserFlags {
    bool account_disabled;
    bool password_never_expires;
    bool password_not_required;
    bool account_locked;
    bool dont_require_preauth;  // AS-REP Roasting üçün maraqlı
    bool trusted_for_delegation;
};
UserFlags parse_uac(uint32_t uac);
```

### Addım 48 — Computer Enum Modulu (`modules/computers.cpp`)
- Filter: `(&(objectClass=computer)(objectCategory=computer))`
- Atributlar: `name`, `dNSHostName`, `operatingSystem`,
  `operatingSystemVersion`, `lastLogonTimestamp`,
  `servicePrincipalName`, `userAccountControl`
- OS statistikası: versiyona görə qruplaşdırma

### Addım 49 — Group Enum Modulu (`modules/groups.cpp`)
- Filter: `(objectClass=group)`
- Atributlar: `sAMAccountName`, `description`, `member`,
  `memberOf`, `groupType`, `adminCount`
- Nested member yükləmə: recursive yox, iterative BFS

### Addım 50 — Group Növ Analizi
```cpp
enum class GroupType {
    DOMAIN_LOCAL_SECURITY,
    GLOBAL_SECURITY,
    UNIVERSAL_SECURITY,
    DOMAIN_LOCAL_DISTRIBUTION,
    GLOBAL_DISTRIBUTION,
    UNIVERSAL_DISTRIBUTION
};
GroupType parse_group_type(int32_t type_flag);
```

### Addım 51 — GPO Enum Modulu (`modules/gpo.cpp`)
- Filter: `(objectClass=groupPolicyContainer)`
- Atributlar: `displayName`, `gPCFileSysPath`, `versionNumber`,
  `flags`, `whenCreated`, `whenChanged`
- GPO linkləri: OU-lara bağlı GPO-ları göstər

### Addım 52 — OU Enum Modulu (`modules/ou.cpp`)
- Filter: `(objectClass=organizationalUnit)`
- Atributlar: `name`, `description`, `distinguishedName`,
  `gpLink`, `gPOptions`
- Ağac strukturu çıxışı: DN-ə görə iyerarxiya

### Addım 53 — Trust Enum Modulu (`modules/trusts.cpp`)
- Filter: `(objectClass=trustedDomain)`
- Atributlar: `name`, `trustType`, `trustAttributes`,
  `trustDirection`, `flatName`, `securityIdentifier`
- Trust növü: `EXTERNAL`, `FOREST`, `SHORTCUT`, `MIT`

### Addım 54 — Trust Atribut Analizi
```cpp
struct TrustInfo {
    std::string trusted_domain;
    TrustType   type;        // EXTERNAL / FOREST / ...
    TrustDir    direction;   // INBOUND / OUTBOUND / BIDIRECTIONAL
    bool        transitive;
    bool        selective_auth;
    bool        sid_history_enabled;
};
```

### Addım 55 — DACL Enum Modulu (`modules/dacl.cpp`)
- Filter: Bütün obyektlər üçün
- Security Descriptor atributu: `nTSecurityDescriptor`
- `ParseSecurityDescriptor()` ilə DACL oxu

### Addım 56 — ACE Analizi
```cpp
struct ACE {
    std::string trustee;        // Kimə verilir
    std::string object_dn;      // Hansı obyektə
    uint32_t    access_mask;    // Hansı icazə
    ACEType     type;           // ALLOW / DENY
    bool        inherited;
    std::string object_type;    // Xüsusi hüquq növü (GUID)
};
```
- İnteresant ACE-lər: `GenericAll`, `WriteDACL`, `WriteOwner`,
  `DCSync` hüquqları (`DS-Replication-*`)

### Addım 57 — Snapshot Modu
```cpp
void EnumCoordinator::run_snapshot() {
    // Bütün modulları paralel işlət
    // Nəticəni vahid formatda topla
    // Timestamp ilə fayla yaz
}
```

### Addım 58 — SPN Enum (Service Principal Names)
- User və Computer obyektlərindən `servicePrincipalName` çəkir
- Kerberoastable hesabları işarələ (SPN olan user-lər)
- Format: `service/hostname:port@REALM`

### Addım 59 — Disabled/Inactive Hesab Filtrəsi
```cpp
// Son 90 gün aktiv olmayan hesablar
std::string inactive_filter(int days) {
    // lastLogonTimestamp < now - days * 864000000000LL
    // Windows FILETIME: 100-nanosaniyəlik interval
}
```

### Addım 60 — AdminCount Analizi
- `adminCount=1` olan hesabları tapır
- Protected Users qrubu üzvlərini göstər
- Privileged hesab siyahısı

### Addım 61 — Delegation Analizi
```cpp
// Unconstrained Delegation
"(&(userAccountControl:1.2.840.113556.1.4.803:=524288)(!(primaryGroupID=516)))"
// Constrained Delegation
"(msDS-AllowedToDelegateTo=*)"
// Resource-Based Constrained Delegation
"(msDS-AllowedToActOnBehalfOfOtherIdentity=*)"
```

### Addım 62 — Password Policy Enum
- Default domain password policy: `objectClass=domainDNS`
- Fine-Grained Password Policies: `msDS-PasswordSettings` container
- Atributlar: minLength, lockoutThreshold, lockoutDuration

### Addım 63 — Schema Version Tespiti
- `CN=Schema,CN=Configuration` obyektindən `objectVersion` oxu
- Windows Server versiyasını map et (87=2019, 88=2022, v.s.)

### Addım 64 — AS-REP Roastable Hesablar
- Filter: `(&(objectClass=user)(userAccountControl:1.2.840.113556.1.4.803:=4194304))`
- `DONT_REQUIRE_PREAUTH` flag-i aktiv olan hesablar

### Addım 65 — Enum Modulu Unit Testləri
- Hər modul üçün mock LDAP nəticəsi ilə test
- UAC flag parsing testləri
- Trust atribut parsing testləri
- ACE parsing testləri

---

## 🟣 PHASE 6 — Output Sistemi (Addım 66–75)

### Addım 66 — Output Formatter İnterfeysi (`output/formatter.h`)
```cpp
class IFormatter {
public:
    virtual ~IFormatter() = default;
    virtual void begin() = 0;
    virtual void write_module(const std::string& name,
                              const std::vector<LDAPEntry>& entries) = 0;
    virtual void end() = 0;
    virtual void flush() = 0;
};
```

### Addım 67 — JSON Formatter (`output/json_writer.cpp`)
- `nlohmann::json` ilə structured JSON
- Hər modul ayrı JSON array-i
- Metadata: timestamp, domain, dc_ip, total_count
- Pretty-print seçimi: `--pretty` flag-i

### Addım 68 — CSV Formatter (`output/csv_writer.cpp`)
- Hər modul üçün ayrı CSV fayl
- Header sətiri avtomatik
- Xüsusi simvolların escape edilməsi (vergül, dırnaq)
- BOM marker (Excel uyumluluğu üçün)

### Addım 69 — Konsol Formatter
- Rəngli çıxış: `\033[` ANSI kodları ilə
- TTY yoxlaması: pipe edilərsə rəngləri söndür
- Kəsilmiş uzun dəyərlər: `--wide` flag-i ilə tam göstər

### Addım 70 — XML Formatter (BloodHound uyumlu)
- BloodHound ingest formatına uyğun çıxış (opsional)
- `computers.json`, `users.json`, `groups.json` strukturu

### Addım 71 — Export Path Meneceri
```cpp
class ExportManager {
public:
    explicit ExportManager(const std::string& base_path);
    std::filesystem::path get_path(const std::string& module,
                                    const std::string& ext);
    void ensure_directory();
    void write_summary();
};
```

### Addım 72 — Şifrəli Çıxış (Opsional)
- AES-256-GCM ilə output faylını şifrələ
- `--encrypt` flag-i ilə aktiv et
- `--encrypt-key <key>` ilə açar qoy

### Addım 73 — Çıxış Statistikası
```cpp
struct EnumStats {
    uint64_t users_found;
    uint64_t computers_found;
    uint64_t groups_found;
    uint64_t gpos_found;
    uint64_t ous_found;
    uint64_t trusts_found;
    uint64_t dacl_entries;
    double   elapsed_seconds;
    uint64_t ldap_queries;
};
```
- Enum bitdikdə konsola özet cədvəl çap et

### Addım 74 — Timestamp və Metadata
```cpp
std::string iso8601_now();  // "2025-01-15T14:30:00Z"
// Hər çıxış faylına header əlavə et:
// # ad-enum v0.1.0 | Domain: corp.local | Enum Date: 2025-01-15
```

### Addım 75 — Output Unit Testləri
- JSON output strukturunun doğruluğunu test et
- CSV escape məntiqini test et
- Export path yaradılmasını test et

---

## ⚪ PHASE 7 — Generator Modulu (Addım 76–82)

### Addım 76 — Generator Konfiqurasiya Doğrulaması
- Icon faylının `.ico` formatını yoxla
- Export path-in yazılabilir olduğunu yoxla
- Agent name-in etibarlı fayl adı olduğunu yoxla

### Addım 77 — EXE Build Konfiqurasiyası
- Windows üçün: `rc.exe` ilə manifest əlavə et
- Icon linki: CMake-dəki `target_sources()` ilə `.rc` faylı
- Version info resursu: `FILEVERSION`, `PRODUCTVERSION`

### Addım 78 — DLL Wrapper Modu
```cpp
// dllmain.cpp skeleti generasiyası
// Export funksiyaları: RunEnum(), GetResults(), FreeMemory()
// __declspec(dllexport) ilə
```

### Addım 79 — PowerShell Module Wrapper
- C++ binary-ni base64 encode et
- `.psm1` faylına yerləştir
- `Invoke-ADEnum` funksiyası yazan PS wrapper

### Addım 80 — Cross-Compilation Dəstəyi
- x64, x86, ARM64 target konfiqurasiyası
- MinGW cross-compile (Linux → Windows)
- `CMAKE_TOOLCHAIN_FILE` ilə toolchain seçimi

### Addım 81 — Build Script (`scripts/build.sh`)
```bash
#!/bin/bash
cmake -B build -DCMAKE_BUILD_TYPE=Release \
      -DTARGET_ARCH=${ARCH:-x64} \
      -DOUTPUT_TYPE=${OUTPUT:-exe}
cmake --build build --parallel $(nproc)
```

### Addım 82 — Generator Unit Testləri
- Konfiqurasiya doğrulamasını test et
- Fayl adı validasiyasını test et

---

## ⚫ PHASE 8 — Performans Optimallaşdırması (Addım 83–92)

### Addım 83 — LDAP Sorğu Keşi
```cpp
class LDAPCache {
    std::unordered_map<std::string, CachedResult> cache_;
    std::shared_mutex mutex_;
    std::chrono::seconds ttl_{300};  // 5 dəqiqə
public:
    std::optional<std::vector<LDAPEntry>> get(const std::string& key);
    void put(const std::string& key, std::vector<LDAPEntry> result);
};
```

### Addım 84 — Atribut Filtrəsi Optimallaşdırması
- Lazımsız atributları sorğudan çıxar
- Server-side sort: `ldap_create_sort_control()`
- VLV (Virtual List View) böyük nəticə setləri üçün

### Addım 85 — String İnternalizasiyası
```cpp
// Təkrarlanan DN və atribut adları üçün shared_ptr pool
class StringPool {
    std::unordered_set<std::string> pool_;
    std::mutex mutex_;
public:
    const std::string& intern(const std::string& s);
};
```

### Addım 86 — Memory Pool (Arena Allocator)
```cpp
// Qısa ömürlü LDAPEntry obyektləri üçün
class ArenaAllocator {
    std::vector<std::byte> buffer_;
    size_t offset_{0};
public:
    void* allocate(size_t size);
    void reset();  // Bütün arenı sil
};
```

### Addım 87 — Profiling İnteqrasiyası
- `perf` / Valgrind uyumlu build konfiqurasiyası
- Daxili timer: `std::chrono::high_resolution_clock`
- `--profile` flag-i ilə detallı zamanlama çıxışı

### Addım 88 — I/O Optimallaşdırması
- Fayl yazma: buffered I/O (`std::ofstream` ilə `sync_with_stdio(false)`)
- Böyük JSON üçün streaming yazma (bütünü yaddaşda saxlama)
- `mmap` ilə sürətli fayl oxuma (config, icon)

### Addım 89 — SIMD Optimizasiya (Opsional)
- String axtarışı üçün SSE4.2 `_mm_cmpestri`
- DN parsing-i üçün vektorizasiya
- Compiler flag: `-msse4.2`

### Addım 90 — Benchmark Suite
```cpp
// Google Benchmark inteqrasiyası
BENCHMARK(BM_LDAPFilter_Parse)->Iterations(10000);
BENCHMARK(BM_UserEntry_Parse)->Iterations(10000);
BENCHMARK(BM_JSONSerialize)->Iterations(1000);
```

### Addım 91 — Memory Leak Yoxlaması
- Valgrind: `valgrind --leak-check=full ./ad-enum --help`
- AddressSanitizer: `-fsanitize=address`
- CI pipeline-a leak check əlavə et

### Addım 92 — Performans Hədəfləri
- 10,000 user < 30 saniyə (4 thread, page-size=1000)
- Memory: < 100MB (10,000 obyekt üçün)
- JSON serialization: > 50,000 entry/saniyə

---

## 🏁 PHASE 9 — Test, Sənəd, CI/CD (Addım 93–100)

### Addım 93 — İnteqrasiya Testləri
- Docker ilə OpenLDAP test serveri qur
- Gerçək LDAP sorğularını test et
- 1000+ test entry əlavə et (Python skript ilə)

### Addım 94 — End-to-End Test
```bash
# Test scripti
./ad-enum --domain test.local --dc 127.0.0.1 \
          --ldap-user admin --ldap-pass test \
          --snapshot --export-path /tmp/test-output
# Çıxışın doğruluğunu yoxla
```

### Addım 95 — CI/CD Pipeline (GitHub Actions)
```yaml
# .github/workflows/build.yml
jobs:
  build-linux:
    runs-on: ubuntu-latest
    steps:
      - cmake configure
      - cmake build
      - ctest --output-on-failure
      - valgrind leak check

  build-windows:
    runs-on: windows-latest
    steps:
      - cmake configure (MSVC)
      - cmake build
      - ctest
```

### Addım 96 — README.md
- Quraşdırma təlimatı (Ubuntu, Windows, macOS)
- Bütün flag-ların sənədləşdirilməsi
- Nümunə istifadə ssenariləri
- Output format nümunələri

### Addım 97 — Man Page (`docs/ad-enum.1`)
```
.TH AD-ENUM 1 "2025" "v0.1.0" "AD Enumeration Tool"
.SH NAME
ad-enum \- Active Directory enumeration agent
.SH SYNOPSIS
ad-enum [GENERATOR OPTIONS] [AGENT OPTIONS] [MODULES]
...
```

### Addım 98 — Changelog və Versiyonlama
- `CHANGELOG.md` — Semantic Versioning 2.0.0
- Git tag: `git tag -a v0.1.0 -m "Initial release"`
- CMake versiyonu ilə sinxronizasiya

### Addım 99 — Security Review
- Credential-ların yaddaşda təmizlənməsi: `SecureZeroMemory()` / `memset_s()`
- Bütün xarici input-ların sanitize edilməsi (DN injection)
- Stack canary yoxlaması
- Symbol stripping (Release): `-s` linker flag

### Addım 100 — v0.1.0 Release
- Bütün testlər keçir
- Sənədlər tamamdır
- Binary release: Linux x64, Windows x64
- Release notes yazılır
- `git tag v0.1.0` ilə etiketlənir

---

## 📊 Addım Xülasəsi

| Phase | Addımlar | Mövzu |
|-------|----------|-------|
| 1 | 1–10 | Layihə skeleti, build sistemi, logger |
| 2 | 11–20 | CLI argument parser |
| 3 | 21–35 | Şəbəkə, LDAP bağlantısı, auth |
| 4 | 36–45 | Thread pool, paralel enum |
| 5 | 46–65 | Enum modulları (User, Computer, Group...) |
| 6 | 66–75 | Output sistemi (JSON, CSV, konsol) |
| 7 | 76–82 | Generator modu (EXE, DLL, PS) |
| 8 | 83–92 | Performans optimallaşdırması |
| 9 | 93–100 | Test, sənəd, CI/CD, release |

---

## 🔧 Tövsiyə Edilən Alətlər

| Kateqoriya | Alət |
|------------|------|
| Build | CMake 3.20+, Ninja |
| Compiler | GCC 13+ / Clang 17+ / MSVC 2022 |
| Package manager | vcpkg |
| LDAP | OpenLDAP (libldap) |
| JSON | nlohmann/json |
| CLI | cxxopts |
| Logging | spdlog |
| Testing | Google Test + Google Benchmark |
| Profiling | Valgrind, perf, AddressSanitizer |
| CI/CD | GitHub Actions |

---

*AD Enum Agent Roadmap — v1.0 | 100 Addım | C++20*
