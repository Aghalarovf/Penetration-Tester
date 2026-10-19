## Semgrep Installation
```powershell
docker pull returntocorp/semgrep
docker run --rm -v "${PWD}:/src" returntocorp/semgrep semgrep scan --config=auto
```

## Semgrep Configs
```powershell
semgrep --version

semgrep login
```

## Semgrep Using
```powershell
# Scan a specific path
semgrep scan --config=auto src/

# Scan with a specific local rule file or directory
semgrep scan --config=rules/
semgrep scan --config=my-rule.yaml

# Scan with a Registry ruleset
semgrep scan --config "p/python"
```

# Top Semgrep Rulesets

## A. Core security
| Config | URL |
| --- | --- |
| p/security-audit | https://semgrep.dev/p/security-audit |
| p/owasp-top-ten | https://semgrep.dev/p/owasp-top-ten |
| p/cwe-top-25 | https://semgrep.dev/p/cwe-top-25 |
| p/secrets | https://semgrep.dev/p/secrets |
| p/ci | https://semgrep.dev/p/ci |
| p/default | https://semgrep.dev/p/default |

## B. Languages
| Config | URL |
| --- | --- |
| p/python | https://semgrep.dev/p/python |
| p/javascript | https://semgrep.dev/p/javascript |
| p/typescript | https://semgrep.dev/p/typescript |
| p/golang | https://semgrep.dev/p/golang |
| p/java | https://semgrep.dev/p/java |
| p/csharp | https://semgrep.dev/p/csharp |
| p/ruby | https://semgrep.dev/p/ruby |
| p/php | https://semgrep.dev/p/php |
| p/kotlin | https://semgrep.dev/p/kotlin |
| p/rust | https://semgrep.dev/p/rust |
| p/c | https://semgrep.dev/p/c |
| p/swift | https://semgrep.dev/p/swift |
| p/scala | https://semgrep.dev/p/scala |

## C. Frameworks
| Config | URL |
| --- | --- |
| p/django | https://semgrep.dev/p/django |
| p/flask | https://semgrep.dev/p/flask |
| p/expressjs | https://semgrep.dev/p/expressjs |
| p/react | https://semgrep.dev/p/react |

## D. Vulnerability class (əsas)
| Config | URL |
| --- | --- |
| p/sql-injection | https://semgrep.dev/p/sql-injection |
| p/xss | https://semgrep.dev/p/xss |
| p/command-injection | https://semgrep.dev/p/command-injection |
| p/jwt | https://semgrep.dev/p/jwt |
| p/insecure-transport | https://semgrep.dev/p/insecure-transport |
| p/xxe | https://semgrep.dev/p/xxe |
| p/path-traversal | https://semgrep.dev/p/path-traversal |
| p/ssrf | https://semgrep.dev/p/ssrf |
| p/csrf | https://semgrep.dev/p/csrf |
| p/open-redirect | https://semgrep.dev/p/open-redirect |
| p/deserialization | https://semgrep.dev/p/deserialization |
| p/hardcoded-secrets | https://semgrep.dev/p/hardcoded-secrets |
| p/insecure-cookie | https://semgrep.dev/p/insecure-cookie |
| p/supply-chain | https://semgrep.dev/p/supply-chain |

## E. Ported linter / external-tool
| Config | URL |
| --- | --- |
| p/eslint-plugin-security | https://semgrep.dev/p/eslint-plugin-security |
| p/findsecbugs | https://semgrep.dev/p/findsecbugs |
| p/trailofbits | https://semgrep.dev/p/trailofbits |
| p/smart-contracts | https://semgrep.dev/p/smart-contracts |
| p/gitlab-bandit | https://semgrep.dev/p/gitlab-bandit |

## F. Third-party GitHub repos
| Repo | URL |
| --- | --- |
| trailofbits/semgrep-rules | https://github.com/trailofbits/semgrep-rules |
| 0xdea/semgrep-rules | https://github.com/0xdea/semgrep-rules |
| Decurity/semgrep-smart-contracts | https://github.com/Decurity/semgrep-smart-contracts |
| elttam/semgrep-rules | https://github.com/elttam/semgrep-rules |
| dgryski/semgrep-go | https://github.com/dgryski/semgrep-go |
| MobSF/mobsfscan | https://github.com/MobSF/mobsfscan |
| j3ssie/curated-semgrep-rules | https://github.com/j3ssie/curated-semgrep-rules |
| Shopify/semgrep-rules-1 | https://github.com/Shopify/semgrep-rules-1 |
| PyCQA/bandit | https://github.com/PyCQA/bandit |
