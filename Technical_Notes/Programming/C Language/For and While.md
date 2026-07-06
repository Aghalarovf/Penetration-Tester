# For
```powershell
#include <stdio.h>

int main() {
    int say, i;
    long long faktorial = 1;

    printf("Ədəd daxil edin: ");
    scanf("%d", &say);

    if (say < 0) {
        printf("Mənfi ədədlərin faktorialı yoxdur!\n");
    } else {
        for (i = 1; i <= say; i++) {
            faktorial *= i;
        }
        printf("%d! = %lld\n", say, faktorial);
    }

    return 0;
}
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
