# Passive Scanner: Module Check Plan

This document lists what each module inspects. The scanner is **passive**: it reads captured HTTP traffic (Burp JSON, HAR, ZAP JSON, intercept JSON/JSONL) and never sends requests. Every finding is a *candidate* that needs manual confirmation, except where noted as "direct evidence".

## Conventions

| Item | Detail |
|------|--------|
| Input | `HttpRecord` = one request + one response |
| Output | `Finding` with module, severity, title, location, evidence, vector hint, confidence |
| Severities | `critical`, `high`, `medium`, `low`, `info` |
| Default output | `medium` and above; `--verbose` adds `low` and `info`; `--severity` overrides both |
| Deduplication | Findings with the same module, severity, title and evidence are merged; the URLs are listed as occurrences |
| Module selection | No flags = all modules; one or more flags = only those modules |

## Module Overview

| # | Module | CLI flag | Analyzers |
|---|--------|----------|-----------|
| 1 | XML | `--xml` | 3 |
| 2 | LFI / Path Traversal | `--lfi` | 5 |
| 3 | IDOR | `--idor` | 3 |
| 4 | JWT | `--jwt` | 1 |
| 5 | CORS | `--cors` | 2 |
| 6 | Cache Poisoning | `--cache` | 1 |
| 7 | File Upload | `--file-upload` | 3 |
| 8 | CSRF | `--csrf` | 4 |
| 9 | Clickjacking | `--clickjacking` | 2 |
| 10 | Insecure Deserialization | `--deserialization` | 3 |
| 11 | OAuth | `--oauth` | 3 |
| 12 | WebSocket | `--websocket` | 2 |
| 13 | Information Disclosure | `--info-disclosure` | 5 |
| 14 | Cookie Security | `--cookie-security` | 3 |
| 15 | Open Redirect | `--open-redirect` | 3 |

---

## 1. XML (`modules/xml/`)

Goal: find endpoints that parse XML, which may be exposed to XXE.

| Check | Inspected | Severity |
|-------|-----------|----------|
| XML declaration in request body | Request body | medium |
| `DOCTYPE` declaration in request body | Request body | high |
| `ENTITY` declaration in request body | Request body | high |
| XML structure in request body | Request body | medium |
| XML-type request Content-Type (`application/xml`, `text/xml`, `application/soap+xml`, `application/svg+xml`, `application/xhtml+xml`, RSS, Atom, XSLT, MathML) | Request Content-Type | high for SOAP, otherwise medium |
| SOAP and WSDL markers (`<soap:`, `SOAP-ENV`, `xmlns:soap`, `SOAPAction`, `.asmx`, `.wsdl`) | Request body, URL | high |

## 2. LFI / Path Traversal (`modules/lfi_path_traversal/`)

Goal: find parameters and URLs that load files from the server.

| Check | Inspected | Severity |
|-------|-----------|----------|
| File-style parameter names (`file`, `path`, `page`, `template`, `include`, `load`, `doc`, `src`, `resource`, `view`, `dir`, `lang`, `theme`, `url`, ...) | Request parameters | high |
| File-serving URL patterns (`/download?file=`, `/view?path=`, `/image?src=`, `/include?`, `/render?`, `/export?`, direct `/uploads/...` file paths) | Request URL | high |
| Interesting file extensions in the URL path | Request URL | medium, higher for sensitive extensions |
| Hidden file-serving patterns inside responses | Response body | medium |
| Traversal sequences: `../`, `..\`, `%2e%2e%2f`, `..%2f`, `%252e%252e`, `..%5c`, `%c0%af`, `..;/` | Request | high |
| Sensitive file paths: `/etc/passwd`, `/etc/shadow`, `win.ini`, `boot.ini`, `proc/self/environ`, `wp-config.php`, `.env` | Request | critical |

## 3. IDOR (`modules/idor/`)

Goal: find direct object references that may lack authorization checks.

| Check | Inspected | Severity |
|-------|-----------|----------|
| Object-reference parameter names (`id`, `user_id`, `order_id`, `account`, `invoice_id`, `doc_id`, ...) | Request parameters | high if numeric, otherwise medium |
| UUID in the URL | Request URL | medium |
| Numeric ID (3+ digits) in the URL path | Request URL | high |
| Sequential or guessable ID values | Request parameters | medium |

## 4. JWT (`modules/jwt/`)

Goal: find JSON Web Tokens and flag weak configurations.

| Check | Inspected | Severity |
|-------|-----------|----------|
| JWT present, decoded header and payload, `alg` reported | URL, request and response headers, cookies, request and response bodies | medium |
| `alg: none` (unsigned token) | Decoded header | critical |
| Empty signature segment | Token | high |
| Symmetric algorithm (`HS256`, `HS384`, `HS512`), secret strength matters | Decoded header | medium |
| Unrecognized algorithm | Decoded header | medium |
| Header not decodable | Token | medium |

## 5. CORS (`modules/cors/`)

Goal: find cross-origin policies that let other sites read responses.

| Check | Inspected | Severity |
|-------|-----------|----------|
| `Access-Control-Allow-Origin: *` | Response headers | medium |
| `Access-Control-Allow-Origin: null` | Response headers | high |
| Specific origin with `Access-Control-Allow-Credentials: true` | Response headers | high |
| `DELETE` in `Access-Control-Allow-Methods` | Response headers | medium |
| Request `Origin` reflected exactly in ACAO | Request and response headers | high with credentials, otherwise medium |
| Subdomain wildcard behavior | Request and response headers | by pattern |

## 6. Cache Poisoning (`modules/cache/`)

Goal: find responses cached by a CDN or proxy that depend on unkeyed inputs.

| Check | Inspected | Severity |
|-------|-----------|----------|
| CDN or cache presence signals (`CF-Cache-Status`, `X-Cache`, `Age`, `Via`, `X-Varnish`, ...) | Response headers | scoring input, no finding on its own |
| `no-cache` present but CDN still active | Response headers | low |
| Heuristic caching: no `Cache-Control` but `Last-Modified` present | Response headers | medium |
| Surrogate headers overriding `Cache-Control` (`Surrogate-Control`, `CDN-Cache-Control`, ...) | Response headers | high |
| No `Vary` header: with suspicious request headers present / response cached without any `Vary` | Request and response headers | high / medium |
| `Vary: Accept-Encoding` only: with unkeyed headers present / without | Request and response headers | high / medium |
| Suspicious unkeyed request headers (`X-Forwarded-Host`, `X-Original-URL`, `X-Rewrite-URL`, `X-HTTP-Method-Override`, ...) not covered by `Vary` | Request and response headers | high |
| Safe cases reported as not vulnerable (no-cache or bypass, all headers keyed) | Response headers | info |

The module combines weighted signals (CDN presence, cache directives, TTL, `Vary` coverage, surrogate headers) to decide which of the findings above applies.

## 7. File Upload (`modules/file_upload/`)

Goal: find upload endpoints and risky upload content.

| Check | Inspected | Severity |
|-------|-----------|----------|
| Multipart upload request | Request Content-Type and body | medium |
| File input field in HTML | Response body | medium |
| Dangerous extension in `filename=` (`.php`, `.phtml`, `.phar`, `.jsp`, `.aspx`, `.asp`, `.svg`, `.html`, `.py`, `.sh`, `.exe`, ...) | Multipart body | critical, high or medium by extension |
| Declared image MIME with executable extension | Multipart body | critical |
| Executable extension without a part Content-Type | Multipart body | high |
| Path traversal in the upload filename (`../`, `..\`) | Multipart body | critical |

## 8. CSRF (`modules/csrf/`)

Goal: find state-changing actions that cross-site requests could trigger.

| Check | Inspected | Severity |
|-------|-----------|----------|
| POST, PUT, PATCH or DELETE with cookie auth and no CSRF token or custom header | Request method, cookie, parameters, headers, body | medium for form and text/plain, low for JSON |
| HTML form with a state-changing method and no token field | Response HTML | medium, low for password forms |
| Session cookie without `SameSite` | `Set-Cookie` | low |
| Session cookie with `SameSite=None` | `Set-Cookie` | medium |
| State-changing action over GET (`delete`, `transfer`, `revoke`, `cancel`, ...) | Request URL and parameters | medium |

Skipped on purpose: requests with an `Authorization` header, requests with no cookie, login, logout, register and OAuth paths, and static files.

## 9. Clickjacking (`modules/clickjacking/`)

Goal: find pages that can be embedded in a hostile frame.

| Check | Inspected | Severity |
|-------|-----------|----------|
| No `X-Frame-Options` and no CSP `frame-ancestors` on an HTML page | Response headers | medium if the page is sensitive (cookie, account, admin or payment path, password field), otherwise low |
| JavaScript frame-busting as the only protection | Response body | low |
| `X-Frame-Options: ALLOW-FROM` (obsolete) | Response headers | low |
| Invalid or multi-value `X-Frame-Options` | Response headers | medium |
| CSP `frame-ancestors` with `*`, `http:`, `https:` or `data:` | Response headers | medium |
| `X-Frame-Options` set through `<meta>` (ignored by browsers) | Response body | low |

Only `200` HTML responses to GET navigations are checked; XHR and sub-resource requests are skipped.

## 10. Insecure Deserialization (`modules/deserialization/`)

Goal: find serialized objects crossing the client boundary.

| Check | Inspected | Severity |
|-------|-----------|----------|
| Java serialized object (`aced0005`, base64 `rO0AB`), .NET BinaryFormatter, Python pickle, Ruby Marshal, PHP object or array, node-serialize payload, Java XMLDecoder XML; plain, URL-encoded, hex, base64 and gzip forms | Request parameters, cookies, headers, body | high |
| Serialization Content-Types (`application/x-java-serialized-object`, `application/x-python-pickle`, ...) | Request Content-Type | high |
| Same Content-Types in responses | Response Content-Type | medium |
| Serialized objects inside `Set-Cookie` values | Response cookies | medium |
| ASP.NET `__VIEWSTATE` with plain `/wE` prefix | Request parameters, response form fields | medium |
| ASP.NET `__VIEWSTATE` present otherwise | Same | low |
| ViewState without `__VIEWSTATEGENERATOR` | Same | info |
| Polymorphic type names in request JSON (`$type`, `@type`, `@class`, `__type`, `javaClass`) | Request JSON body | medium |
| Type metadata in response JSON | Response JSON body | low |

## 11. OAuth (`modules/oauth/`)

Goal: find weak OAuth 2.0 and OpenID Connect flows.

| Check | Inspected | Severity |
|-------|-----------|----------|
| Authorization request without `state` | Request parameters | medium |
| Implicit flow (`response_type=token` or `id_token`) | Request parameters | medium |
| Code flow without PKCE (`code_challenge`) | Request parameters | low |
| `code_challenge_method=plain` | Request parameters | low |
| `redirect_uri` with a wildcard | Request parameters | high |
| `redirect_uri` over `http://` | Request parameters | medium |
| `redirect_uri` pointing to localhost | Request parameters | low |
| `redirect_uri` present (review validation) | Request parameters | info |
| `offline_access` scope | Request parameters | low |
| Callback with `code` but no `state` | Callback request | medium |
| `access_token`, `id_token` or `refresh_token` in a URL | Request URL | medium |
| Credentials leaked through `Referer` | Request headers | medium |
| Token returned in a redirect `Location` | Response headers | low |
| `client_secret` in a URL | Request URL | high |
| `grant_type=password` | Token request | medium |
| Code exchange without `code_verifier` | Token request | low |
| `client_secret` in a request body | Token request | info |
| `http://` `redirect_uri` on a token request | Token request | medium |
| Token response without `Cache-Control: no-store` | Token response | low |
| Token response without `expires_in` when a refresh token is issued | Token response | low |
| Token response without `scope` | Token response | info |

## 12. WebSocket (`modules/websocket/`)

Goal: find weak WebSocket handshakes and client code. Only the HTTP handshake and page code are visible; frame contents are not.

| Check | Inspected | Severity |
|-------|-----------|----------|
| Unencrypted `ws://` handshake | Handshake URL | medium |
| Foreign `Origin` accepted with status `101` (cross-site hijacking) | Handshake headers and status | high |
| Credential-like query parameter in the handshake URL | Handshake URL | medium |
| Cookie-only handshake authentication, no token or CSRF-style parameter | Handshake headers | medium |
| Handshake with cookies but no `Origin` header | Handshake headers | low |
| `new WebSocket("ws://...")` in client code | HTML and JavaScript responses | medium |
| Credential in a client-side WebSocket URL | HTML and JavaScript responses | medium |
| WebSocket endpoint discovered in client code | HTML and JavaScript responses | info |

## 13. Information Disclosure (`modules/info_disclosure/`)

Goal: find data that helps attackers fingerprint or break the application.

| Check | Inspected | Severity |
|-------|-----------|----------|
| Debug pages: Django debug, Werkzeug debugger, Laravel and Symfony debug | Response body | high |
| Stack traces and exceptions: Java, Python, PHP, ASP.NET, Node.js, Rails | Response body | medium |
| SQL error messages (MySQL, Oracle, PostgreSQL, SQLite, MSSQL) | Response body | medium |
| Spring Whitelabel error page, server file paths | Response body | low |
| Version in `Server` header | Response headers | low |
| `X-Powered-By`, `X-AspNet-Version`, `X-Generator`, `X-Runtime` | Response headers | low |
| Debug headers (`X-Debug-Token`, `X-Symfony-Profiler`, ...) | Response headers | medium |
| Internal IP in response headers or body | Response headers and body | low |
| Private key block, Stripe live key | Response body | critical |
| AWS key, Slack, GitHub and SendGrid tokens | Response body | high |
| Google, Twilio and Mailgun keys, hardcoded `password` or `api_key` assignments | Response body | medium |
| `.env` file reachable | URL with status 200 | critical |
| `.git`, `.svn`, `.htpasswd`, config backups, database dumps, sensitive Actuator endpoints, debug console | URL with status 200 | high |
| Backup or temporary files, `phpinfo`, `server-status`, directory listing, debug endpoints | URL with status 200 and body markers | medium |
| Exposed API docs, basic Actuator, `.DS_Store`, source map file | URL with status 200 | low |
| `sourceMappingURL` reference, `SourceMap` header | JavaScript, response headers | info |
| Sensitive HTML comments (`password`, `TODO`, `admin`, internal IPs) | Response HTML | low |

Secret values are redacted in evidence (first 4 characters only). Known placeholders and `EXAMPLE` keys are ignored.

## 14. Cookie Security (`modules/cookie_security/`)

Goal: find weak session cookie handling.

| Check | Inspected | Severity |
|-------|-----------|----------|
| Session cookie without `Secure` | `Set-Cookie` | medium |
| Other cookie set over HTTP without `Secure` | `Set-Cookie` | low |
| Session cookie without `HttpOnly` | `Set-Cookie` | medium |
| Invalid `__Host-` or `__Secure-` prefix usage | `Set-Cookie` | low |
| Session cookie scoped to a parent domain | `Set-Cookie` | low |
| Session cookie with `Max-Age` over 30 days | `Set-Cookie` | low |
| Numeric session ID | `Set-Cookie` value | medium |
| Session ID shorter than 16 characters | `Set-Cookie` value | medium |
| Session value that base64-decodes to readable text | `Set-Cookie` value | low |
| Session ID in the URL (`?sid=`, `;jsessionid=`) | Request URL | medium |
| Session ID not rotated after a successful login (candidate fixation) | Login request and response, across records | medium |

`SameSite` checks live in the CSRF module to avoid duplicate findings.

## 15. Open Redirect (`modules/open_redirect/`)

Goal: find redirects whose target a user can control.

| Check | Inspected | Severity |
|-------|-----------|----------|
| `javascript:` or `data:` value in a redirect-style parameter | Request parameters | medium |
| External or absolute URL in a redirect-style parameter (`next`, `url`, `redirect`, `return`, `goto`, `dest`, ...) | Request parameters | low |
| Relative-path value in a redirect-style parameter | Request parameters | info |
| Parameter value reflected into a `Location` header pointing to another host (direct evidence) | Request and 3xx response | high |
| DOM redirect sink fed by `location.search`, `location.hash`, `document.URL`, `document.referrer` or `window.name` | Response JavaScript | medium |
| Parameter reflected in `<meta http-equiv="refresh">` | Response HTML | medium |
| Parameter reflected in a script `location` assignment | Response HTML and JavaScript | medium |
| Parameter reflected in a `Refresh` header | Response headers | medium |

OAuth authorization requests are skipped here because the OAuth module covers `redirect_uri`.

---

## Known Limits of Passive Analysis

- A finding shows where to test, not that the target is vulnerable. Blind and server-side logic issues need active testing.
- WebSocket frame contents, request timing and response differences between two requests are not available.
- Business logic flaws, race conditions, prototype pollution and subdomain takeover are not covered.
- Severity reflects potential impact if confirmed, not confirmed impact.

## Not Yet Covered (Candidates for Next Modules)

| Area | Typical passive checks |
|------|------------------------|
| Security headers and transport | Missing HSTS, weak CSP, mixed content, missing `X-Content-Type-Options` and `Referrer-Policy` |
| GraphQL | Endpoint discovery, introspection responses, batching, GET mutations |
| XSS reflection | Parameter reflection without encoding, dangerous DOM sinks |
| Host header and smuggling | Reflected `Host` and `X-Forwarded-Host`, conflicting `Content-Length` and `Transfer-Encoding` |
| Authentication flow | User enumeration through response differences, reset tokens in URLs, missing rate limit headers |
| API security | Mass assignment hints, legacy API versions, method override headers |
| SAML and SSO | `SAMLResponse` handling, `RelayState` issues |
