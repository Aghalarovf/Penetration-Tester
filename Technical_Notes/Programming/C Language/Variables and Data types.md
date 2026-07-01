# Variables and Data types
```powershell
<data_type> <identifier> = <value>
```

```powershell
#include <stdio.h>
#include <stdbool.h>   // required for _Bool / bool

int main() {
    // Basic integer types
    short int shortNum = 100;
    int integer = 10;
    long int longNum = 100000L;
    long long int veryLongNum = 9000000000LL;

    // Unsigned integer types
    unsigned short int uShortNum = 200;
    unsigned int uInteger = 20;
    unsigned long int uLongNum = 200000UL;
    unsigned long long int uVeryLongNum = 18000000000ULL;

    // Floating-point types
    float decimal = 3.14f;
    double largeDecimal = 2.71828;
    long double veryLargeDecimal = 3.14159265358979L;

    // Character types
    char character = 'A';
    char name[] = "Olivia";
    unsigned char uCharacter = 250;
    signed char sCharacter = -100;

    // Boolean type (C99+, with stdbool.h)
    bool isTrue = true;
    bool isFalse = false;

    // ---- Printing ----
    printf("Short: %hd\n", shortNum);
    printf("Integer: %d\n", integer);
    printf("Long: %ld\n", longNum);
    printf("Long Long: %lld\n", veryLongNum);

    printf("Unsigned Short: %hu\n", uShortNum);
    printf("Unsigned Integer: %u\n", uInteger);
    printf("Unsigned Long: %lu\n", uLongNum);
    printf("Unsigned Long Long: %llu\n", uVeryLongNum);

    printf("Float: %f\n", decimal);
    printf("Double: %lf\n", largeDecimal);
    printf("Long double: %Lf\n", veryLargeDecimal);

    printf("Character: %c\n", character);
    printf("Unsigned Character: %u\n", uCharacter);
    printf("Signed Character: %d\n", sCharacter);
    printf("Size of Char array: %zu bytes\n", sizeof(name));

    printf("Bool (true): %d\n", isTrue);
    printf("Bool (false): %d\n", isFalse);

    return 0;
}
```
