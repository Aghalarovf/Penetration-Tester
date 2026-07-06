# For
```powershell

```

# While
```powershell
#include <stdio.h>

int main() {
    int ədəd, cəm = 0;
    printf("Ədədləri daxil edin (bitirmək üçün 0 daxil edin):\n");

    while (1) {
        printf("Ədəd: ");
        scanf("%d", &ədəd);

        if (ədəd == 0)
            break;
        cəm += ədəd;
    }
    printf("Cəm: %d\n", cəm);
    return 0;
}
```
