# Variables and Data types
```powershell
<data_type> <identifier> = <value>
```

```powershell
#include <stdio.h>
int main() {
    int yash = 20;
    float boy = 1.75;
    double dunya_nufusu = 8000000000.0;
    char herf = 'C';
    printf("Yas: %d\n", yash);
    printf("Boy: %.2f\n", boy);
    printf("Dunya nufusu: %.0f\n", dunya_nufusu);
    printf("Herf: %c\n", herf);
    return 0;
}
```

```powershell
#include <stdio.h>

int main() {
    int eded = 25;
    float kesr = 3.14159;
    char simvol = 'A';

    printf("%d\n", eded);      // Çıxış: 25
    printf("%.2f\n", kesr);    // Çıxış: 3.14
    printf("%.0f\n", kesr);    // Çıxış: 3
    printf("%c\n", simvol);    // Çıxış: A

    return 0;
}
```

## Type Modifiers
```powershell
unsigned int yash = 25;       // only positive
short int kicik_eded = 100; 
long int boyuk_eded = 123456789;
long long int cox_boyuk = 9223372036854775807;
```
