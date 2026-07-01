```powershell
scanf("format_specifier", &variable);
```

```powershell
#include <stdio.h>

int main() {
    char name[50];
    char surname[50];
    int age;

    printf("Enter the Name:\n");
    scanf("%s", name);

    printf("Enter the Surname: ");
    scanf("%s", surname);

    printf("Enter the Age: ");
    scanf("%d", &age);

    printf("Your name is: %s\n", name);
    printf("Your surname is: %s\n", surname);
    printf("Your age is: %d\n", age);

    return 0;
}
```
