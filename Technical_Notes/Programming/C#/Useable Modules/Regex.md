## Regex (System.Text.RegularExpressions)

```
using System;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        Console.Write("Enter the PIN code: ");
        string pinCode = Console.ReadLine();

        Console.WriteLine(
            Regex.IsMatch(pinCode, @"^\d{4}$")
                ? "Valid PIN code."
                : "Invalid PIN code. Please enter a 4-digit number."
        );
    }
}
```

### Core Methods

```csharp
Regex.IsMatch(input, pattern)                              // Returns true if pattern matches anywhere in the string.
Regex.Match(input, pattern).Value                          // Returns the first matched substring.
Regex.Matches(input, pattern)                              // Returns all matches as a MatchCollection.
Regex.Replace(input, pattern, replacement)                 // Replaces all matches with the given value.
Regex.Split(input, pattern)                                // Splits the string on every match.
```

### Compiled Regex (reuse for performance)

```csharp
var re = new Regex(@"\d+", RegexOptions.Compiled);
re.IsMatch("abc123");                                      // Faster when the same pattern is used many times.
```

---

### Pattern Reference

| Pattern   | Meaning                              | Example match       |
|-----------|--------------------------------------|---------------------|
| `\d`      | Any digit (0-9)                      | `"5"` in `"a5b"`    |
| `\D`      | Any non-digit                        | `"a"` in `"a5b"`    |
| `\w`      | Word character (letter/digit/`_`)    | `"h"` in `"hi_1"`   |
| `\W`      | Non-word character                   | `" "` in `"a b"`    |
| `\s`      | Whitespace (space, tab, newline)     | `" "` in `"a b"`    |
| `\S`      | Non-whitespace                       | `"a"` in `"a b"`    |
| `.`       | Any character except newline         | `"x"` in `"axb"`    |
| `^`       | Start of string                      | `^Hello`            |
| `$`       | End of string                        | `world$`            |
| `*`       | 0 or more                            | `\d*`               |
| `+`       | 1 or more                            | `\d+`               |
| `?`       | 0 or 1 (optional)                    | `colou?r`           |
| `{n,m}`   | Between n and m repetitions          | `\d{2,4}`           |
| `[abc]`   | Any one of these characters          | `[aeiou]`           |
| `[^abc]`  | Any character NOT in the set         | `[^0-9]`            |
| `(a\|b)`  | Either a or b                        | `(cat\|dog)`        |
| `()`      | Capture group                        | `(\d+)`             |
| `(?:)`    | Non-capturing group                  | `(?:\d+)`           |

---

### Real-World Examples

#### Integer
```csharp
Regex.IsMatch("42",   @"^\d+$")                            // true  - whole string is digits
Regex.IsMatch("-42",  @"^-?\d+$")                          // true  - optional minus sign
Regex.IsMatch("3.14", @"^\d+$")                            // false - dot is not a digit
```

#### Decimal / Float
```csharp
Regex.IsMatch("3.14",  @"^\d+(\.\d+)?$")                  // true  - optional decimal part
Regex.IsMatch("-3.14", @"^-?\d+(\.\d+)?$")                // true  - negative decimal
Regex.IsMatch("3.",    @"^\d+(\.\d+)?$")                   // false - dot must be followed by digits
```

#### Email
```csharp
Regex.IsMatch("user@example.com", @"^[\w\.-]+@[\w\.-]+\.\w{2,}$")   // true
Regex.IsMatch("user@",            @"^[\w\.-]+@[\w\.-]+\.\w{2,}$")   // false
```

#### Phone Number
```csharp
Regex.IsMatch("+994501234567",    @"^\+?\d{7,15}$")                  // true  - international format
Regex.IsMatch("(050) 123-45-67", @"^[\d\s\(\)\-\+]{7,20}$")         // true  - flexible format
```

#### URL
```csharp
Regex.IsMatch("https://example.com", @"^https?://[\w\-]+(\.[\w\-]+)+(\/\S*)?$")  // true
Regex.IsMatch("ftp://example.com",   @"^https?://[\w\-]+(\.[\w\-]+)+(\/\S*)?$")  // false
```

#### IPv4 Address
```csharp
Regex.IsMatch("192.168.1.1",      @"^(\d{1,3}\.){3}\d{1,3}$")       // true
// Note: use byte.Parse() to also validate range (0-255)
```

#### Date (YYYY-MM-DD)
```csharp
Regex.IsMatch("2024-08-10", @"^\d{4}-\d{2}-\d{2}$")       // true
Regex.IsMatch("10/08/2024", @"^\d{4}-\d{2}-\d{2}$")       // false
```

#### Time (HH:mm or HH:mm:ss)
```csharp
Regex.IsMatch("14:30",    @"^\d{2}:\d{2}(:\d{2})?$")      // true
Regex.IsMatch("9:5",      @"^\d{2}:\d{2}(:\d{2})?$")      // false - must be zero-padded
```

#### Hex Color
```csharp
Regex.IsMatch("#FF5733", @"^#([0-9A-Fa-f]{3}|[0-9A-Fa-f]{6})$")    // true  - #RGB or #RRGGBB
Regex.IsMatch("#GGG",    @"^#([0-9A-Fa-f]{3}|[0-9A-Fa-f]{6})$")    // false
```

#### UUID / GUID
```csharp
Regex.IsMatch("550e8400-e29b-41d4-a716-446655440000",
    @"^[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}$")       // true
```

#### Password Strength
```csharp
// At least 8 chars, 1 uppercase, 1 lowercase, 1 digit, 1 special char
Regex.IsMatch("Secret@1", @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$")   // true
Regex.IsMatch("password", @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$")   // false
```

#### Only Letters
```csharp
Regex.IsMatch("Hello",  @"^[a-zA-Z]+$")                    // true
Regex.IsMatch("Hello1", @"^[a-zA-Z]+$")                    // false - contains digit
```

#### Only Alphanumeric
```csharp
Regex.IsMatch("User123",  @"^\w+$")                         // true
Regex.IsMatch("User 123", @"^\w+$")                         // false - contains space
```

#### Extract All Numbers from Text
```csharp
var numbers = Regex.Matches("Price: 100, Tax: 15, Total: 115", @"\d+")
                   .Select(m => int.Parse(m.Value))
                   .ToList();
// → [100, 15, 115]
```

#### Named Capture Groups
```csharp
var m = Regex.Match("2024-08-10", @"^(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})$");
string year  = m.Groups["year"].Value;                      // "2024"
string month = m.Groups["month"].Value;                     // "08"
string day   = m.Groups["day"].Value;                       // "10"
```

#### Replace with Group Reference
```csharp
Regex.Replace("John Smith", @"(\w+) (\w+)", "$2, $1")      // → "Smith, John"
```

---
