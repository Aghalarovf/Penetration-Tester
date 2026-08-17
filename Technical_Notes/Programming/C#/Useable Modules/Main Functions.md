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
