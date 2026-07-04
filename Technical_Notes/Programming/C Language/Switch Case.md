```
#include <stdio.h>

int main() {
    int ay;

    printf("Ayin nomresini daxil edin (1-12): ");
    scanf("%d", &ay);

    switch (ay) {
        case 12:
        case 1:
        case 2:
            printf("Movsum: Qis\n");
            break;

        case 3:
        case 4:
        case 5:
            printf("Movsum: Yaz\n");
            break;

        case 6:
        case 7:
        case 8:
            printf("Movsum: Yay\n");
            break;

        case 9:
        case 10:
        case 11:
            printf("Movsum: Payiz\n");
            break;

        default:
            printf("Yanlis ay nomresi! 1-12 arasinda deger daxil edin.\n");
    }

    return 0;
}
```
