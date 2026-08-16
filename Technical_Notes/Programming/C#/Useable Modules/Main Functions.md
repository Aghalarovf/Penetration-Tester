# C# String Cheat Sheet

---

## Environment Variables

```csharp
IDictionary envVars = Environment.GetEnvironmentVariables();
foreach (DictionaryEntry kv in envVars)
{
    string key   = kv.Key.ToString();
    string value = kv.Value?.ToString() ?? "";
}
```
Iterates all environment variables and reads each key-value pair.

---

## Search

```csharp
s.Contains("World")                                        // Returns true if the substring exists.
s.StartsWith("Hello")                                      // Returns true if the string starts with the value.
s.EndsWith("Hello")                                        // Returns true if the string ends with the value.
s.IndexOf("Hello")                                         // Returns the index of the first occurrence (-1 if not found).
s.LastIndexOf("Hello")                                     // Returns the index of the last occurrence (-1 if not found).
s.Count(c => c == 'l')                                     // Counts how many times a character appears.
```

---

## Cleaning

```csharp
s.Trim()                                                   // Removes whitespace from both ends.
s.TrimStart()                                              // Removes whitespace from the start only.
s.TrimEnd()                                                // Removes whitespace from the end only.
s.Trim('!')                                                // Removes a specific character from both ends.
```

---

## Modification

```csharp
s.Replace("Hello", "Hi")                                   // Replaces all occurrences of a substring.
s.ToLower()                                                // Converts all characters to lowercase.
s.ToUpper()                                                // Converts all characters to uppercase.
s.Remove(2, 5)                                             // Removes 5 characters starting at index 2.
```

---

## Slicing

```csharp
s.Substring(2, 5)                                          // Extracts 5 characters starting at index 2.
s.Trim()[^5..]                                             // Takes the last 5 characters (Range syntax).
s.Trim()[..5]                                              // Takes the first 5 characters (Range syntax).
```

---

## Splitting & Joining

```csharp
"a,b,c".Split(',')                                         // Splits by delimiter into an array.
"a,,b".Split(',', StringSplitOptions.RemoveEmptyEntries)   // Splits and skips empty entries.
string.Join(" | ", new[] { "a", "b", "c" })                // Joins an array into a single string.
string.Concat("Hello", " ", "World")                       // Concatenates multiple strings.
```

---

## Validation

```csharp
string.IsNullOrEmpty("")                                   // Returns true if null or empty string.
string.IsNullOrWhiteSpace("   ")                           // Returns true if null, empty, or only whitespace.
s.Length                                                   // Returns the number of characters.
char.IsDigit('5')                                          // Returns true if the character is a digit.
char.IsLetter('A')                                         // Returns true if the character is a letter.
char.IsWhiteSpace(' ')                                     // Returns true if the character is whitespace.
```

---

## Formatting & Interpolation

```csharp
$"Name: {name}, Age: {age}"                                // Embeds variables directly into a string.
$"Age: {age:D3}"                                           // Formats an integer with leading zeros (e.g. 025).
$"Amount: {3.14159:F2}"                                    // Formats a float to 2 decimal places.
string.Format("{0} + {1} = {2}", 1, 2, 3)                  // Classic positional string formatting.
```

---

## Padding & Repeating

```csharp
"42".PadLeft(6)                                            // Pads with spaces on the left to reach length 6.
"42".PadRight(6)                                           // Pads with spaces on the right to reach length 6.
"42".PadLeft(6, '0')                                       // Pads with zeros on the left (e.g. "000042").
new string('=', 20)                                        // Creates a string by repeating a character N times.
```

---

## Comparison

```csharp
string.Compare("a", "b")                                                    // Returns -1, 0, or 1 (less, equal, greater).
"hello" == "hello"                                                           // Simple equality check.
"hello".Equals("HELLO", StringComparison.OrdinalIgnoreCase)                 // Case-insensitive equality check.
```

---

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

## StringBuilder (for heavy concatenation)

```csharp
var sb = new StringBuilder();
sb.Append("Hello");                                        // Appends a string without allocating a new object.
sb.AppendLine(" World");                                   // Appends a string followed by a newline.
sb.Insert(5, ",");                                         // Inserts a string at the specified index.
sb.Replace("Hello", "Hi");                                 // Replaces all occurrences within the builder.
sb.Remove(0, 3);                                           // Removes characters from the given index.
sb.ToString();                                             // Converts the builder back to a plain string.
sb.Length;                                                 // Gets the current character count.
sb.Clear();                                                // Resets the builder to empty.
```

---

## Type Conversion

```csharp
int.Parse("42")                                            // Converts a string to int; throws if invalid.
int.TryParse("42", out int n)                              // Safe parse — returns false instead of throwing.
double.Parse("3.14")                                       // Converts a string to double.
bool.Parse("true")                                         // Converts "true"/"false" string to bool.
Convert.ToInt32("42")                                      // Converts a string to int via Convert class.
42.ToString()                                              // Converts any value to its string representation.
42.ToString("X")                                           // Converts int to hex string (e.g. "2A").
```

---

## Encoding & Hashing

```csharp
Convert.ToBase64String(Encoding.UTF8.GetBytes("hello"))    // Encodes a string to Base64.
Encoding.UTF8.GetString(Convert.FromBase64String(b64))     // Decodes a Base64 string back to text.
Encoding.UTF8.GetBytes("hello")                            // Converts a string to a UTF-8 byte array.
Encoding.UTF8.GetByteCount("hello")                        // Returns the byte size without allocating.
```

---

## Span\<char\> / Memory (zero-allocation slicing)

```csharp
s.AsSpan(2, 5)                                             // Slices without allocating a new string.
s.AsSpan().Trim()                                          // Trims whitespace without allocation.
MemoryExtensions.Equals(s.AsSpan(), "hi", StringComparison.OrdinalIgnoreCase) // Allocation-free comparison.
```

---

## Char Utilities

```csharp
char.IsDigit(c)                                            // True if the character is 0–9.
char.IsLetter(c)                                           // True if the character is a letter.
char.IsLetterOrDigit(c)                                    // True if letter or digit.
char.IsUpper(c)                                            // True if uppercase letter.
char.IsLower(c)                                            // True if lowercase letter.
char.IsPunctuation(c)                                      // True if punctuation character.
char.IsSymbol(c)                                           // True if a symbol (e.g. $, ©).
char.ToUpper(c)                                            // Converts a char to uppercase.
char.ToLower(c)                                            // Converts a char to lowercase.
(int)c                                                     // Gets the Unicode code point of a char.
```

---

## Path & String Utilities

```csharp
Path.Combine("folder", "file.txt")                         // Safely joins path segments with the OS separator.
Path.GetFileName("C:/dir/file.txt")                        // Extracts just the filename from a path.
Path.GetExtension("file.txt")                              // Returns the file extension (e.g. ".txt").
Path.GetFileNameWithoutExtension("file.txt")               // Returns the filename without its extension.
Path.GetDirectoryName("C:/dir/file.txt")                   // Returns the directory part of a path.
Uri.EscapeDataString("hello world")                        // URL-encodes a string (e.g. "hello%20world").
Uri.UnescapeDataString("hello%20world")                    // Decodes a URL-encoded string.
```

---

## LINQ on Strings

```csharp
s.Where(char.IsDigit)                                      // Filters characters by a condition.
s.Select(char.ToUpper)                                     // Projects each character (e.g. to uppercase).
s.All(char.IsLetter)                                       // Returns true if every character satisfies the condition.
s.Any(char.IsDigit)                                        // Returns true if any character satisfies the condition.
s.First()                                                  // Returns the first character.
s.Last()                                                   // Returns the last character.
s.Reverse()                                                // Returns characters in reverse order (returns IEnumerable<char>).
new string(s.Reverse().ToArray())                          // Reverses and converts back to a string.
```

---

## Ternary Operators

```csharp
string root = Environment.OSVersion.Platform == PlatformID.Win32NT
      ? @"C:\Users"
      : Environement.GetFolderPath
```

---

## StringComparison Options

| Option                                    | Description                                      |
|-------------------------------------------|--------------------------------------------------|
| `StringComparison.Ordinal`                | Byte-by-byte comparison. Fastest.                |
| `StringComparison.OrdinalIgnoreCase`      | Same, but ignores letter casing.                 |
| `StringComparison.CurrentCulture`         | Compares using the system's current locale.      |
| `StringComparison.CurrentCultureIgnoreCase` | Same, ignoring casing.                         |
| `StringComparison.InvariantCulture`       | Uses fixed English culture rules.                |
| `StringComparison.InvariantCultureIgnoreCase` | Same, ignoring casing.                       |
