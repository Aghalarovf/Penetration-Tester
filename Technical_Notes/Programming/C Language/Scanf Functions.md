```powershell
scanf("format_specifier", &variable);
```

```powershell
#include <stdio.h>

int main() {
    int wholeNum;
    float decimal;
    double bigDecimal;
    char letter;

    printf("Enter an integer: ");
    scanf("%d", &wholeNum);

    printf("Enter a decimal number: ");
    scanf("%f", &decimal);

    printf("Enter a large decimal number: ");
    scanf("%lf", &bigDecimal);   // %lf for double!

    printf("Enter a single character: ");
    scanf(" %c", &letter);        // note: starts with a space

    printf("Integer: %d\n", wholeNum);
    printf("Decimal: %f\n", decimal);
    printf("Large decimal: %lf\n", bigDecimal);
    printf("Character: %c\n", letter);

    return 0;
}
```
