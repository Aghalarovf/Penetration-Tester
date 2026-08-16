# C# Fundamentals Roadmap — 40 Steps to Master the Basics

> **Goal:** Strengthen your C# foundation step by step — variables, loops, LINQ, collections, OOP, and more. No advanced modules, just solid fundamentals.

---

## 🟢 Stage 1: Language Basics (Steps 1–10)

### Step 1 — Hello World & Program Structure
Understand the entry point of a C# program. Learn what `namespace`, `class`, and `Main()` mean. Run your first console app.
```csharp
Console.WriteLine("Hello, World!");
```

### Step 2 — Variables & Data Types
Learn the core types: `int`, `double`, `float`, `decimal`, `bool`, `char`, `string`. Understand value types vs reference types.
```csharp
int age = 25;
string name = "Anar";
bool isActive = true;
```

### Step 3 — Type Conversion
Understand implicit vs explicit casting, `Convert.ToInt32()`, `int.Parse()`, `int.TryParse()`. Know when each is appropriate.
```csharp
string input = "42";
int number = int.Parse(input);
```

### Step 4 — Operators
Arithmetic (`+`, `-`, `*`, `/`, `%`), comparison (`==`, `!=`, `>`, `<`), logical (`&&`, `||`, `!`), and assignment operators (`+=`, `-=`).

### Step 5 — String Operations
Learn `string.Length`, `ToUpper()`, `ToLower()`, `Trim()`, `Replace()`, `Contains()`, `Split()`, `Substring()`, and string interpolation.
```csharp
string full = $"Hello, {name}! You are {age} years old.";
```

### Step 6 — Conditional Statements (if / else / switch)
Control the flow of your program using `if`, `else if`, `else`, and `switch` statements.
```csharp
if (age >= 18) Console.WriteLine("Adult");
else Console.WriteLine("Minor");
```

### Step 7 — Ternary Operator & Null Coalescing
Use `? :` for short conditionals and `??` for null-safe defaults.
```csharp
string result = age >= 18 ? "Adult" : "Minor";
string display = name ?? "Anonymous";
```

### Step 8 — Loops: for & while
Master `for` and `while` loops. Understand loop variables, conditions, and increments. Use `break` and `continue`.
```csharp
for (int i = 0; i < 10; i++) Console.WriteLine(i);
```

### Step 9 — Loops: foreach & do-while
Use `foreach` to iterate over collections. Use `do-while` when the body must run at least once.
```csharp
foreach (var item in myList) Console.WriteLine(item);
```

### Step 10 — Methods (Functions)
Define and call methods. Understand parameters, return types, `void`, and `return`. Learn method overloading.
```csharp
int Add(int a, int b) => a + b;
```

---

## 🔵 Stage 2: Collections & Data Structures (Steps 11–20)

### Step 11 — Arrays
Declare and use single-dimensional arrays. Access elements by index. Understand `array.Length`.
```csharp
int[] numbers = { 1, 2, 3, 4, 5 };
```

### Step 12 — Multi-dimensional & Jagged Arrays
Learn 2D arrays (`int[,]`) and jagged arrays (`int[][]`). Know when to use each.

### Step 13 — List\<T\>
Use `List<T>` for dynamic-size collections. Methods: `Add()`, `Remove()`, `Contains()`, `Count`, `Clear()`, `Sort()`.
```csharp
var names = new List<string> { "Ali", "Veli" };
names.Add("Anar");
```

### Step 14 — Dictionary\<TKey, TValue\>
Store key-value pairs. Use `Add()`, `Remove()`, `ContainsKey()`, `TryGetValue()`, iterate with `foreach`.
```csharp
var ages = new Dictionary<string, int> { { "Ali", 30 } };
```

### Step 15 — HashSet\<T\>
Unique element collections. Understand `Add()`, `Contains()`, `UnionWith()`, `IntersectWith()`.

### Step 16 — Stack\<T\> & Queue\<T\>
Understand LIFO (`Stack`) and FIFO (`Queue`) data structures. Use `Push/Pop` and `Enqueue/Dequeue`.

### Step 17 — Tuple & ValueTuple
Return multiple values from a method using `Tuple` or named `ValueTuple`.
```csharp
(string Name, int Age) GetPerson() => ("Anar", 25);
```

### Step 18 — IEnumerable\<T\> & ICollection\<T\>
Understand the collection interfaces. Know why `IEnumerable` is the base for all iteration in C#.

### Step 19 — Sorting & Searching Collections
Use `List.Sort()`, `Array.Sort()`, `Array.BinarySearch()`. Understand custom sorting with `IComparer<T>`.

### Step 20 — Collection Initialization Patterns
Master object and collection initializers for cleaner, more readable code.
```csharp
var person = new Person { Name = "Anar", Age = 25 };
```

---

## 🟣 Stage 3: Object-Oriented Programming (Steps 21–28)

### Step 21 — Classes & Objects
Define a class with fields, properties, and constructors. Create objects with `new`.
```csharp
class Car { public string Brand { get; set; } }
```

### Step 22 — Properties & Access Modifiers
Use `public`, `private`, `protected`, `internal`. Understand auto-properties vs full properties with getters/setters.

### Step 23 — Constructors & Destructors
Parameterless and parameterized constructors. Constructor chaining with `this(...)`.

### Step 24 — Inheritance
Extend a base class with `: BaseClass`. Use `base` keyword to call parent members.
```csharp
class ElectricCar : Car { public int BatteryCapacity { get; set; } }
```

### Step 25 — Polymorphism & Virtual Methods
Use `virtual` and `override` to allow child classes to redefine behavior.

### Step 26 — Abstract Classes & Interfaces
`abstract` forces subclasses to implement. `interface` defines a contract. Understand when to use each.

### Step 27 — Static Members & Static Classes
Fields and methods that belong to the class, not instances. Utility classes pattern.

### Step 28 — Records & Structs
Use `record` for immutable data objects. Use `struct` for lightweight value types.
```csharp
record Person(string Name, int Age);
```

---

## 🟡 Stage 4: LINQ (Steps 29–34)

### Step 29 — What is LINQ?
Language Integrated Query — query collections using SQL-like syntax. Understand deferred execution.

### Step 30 — LINQ: Where & Select
Filter with `Where()`, transform with `Select()`.
```csharp
var adults = people.Where(p => p.Age >= 18).Select(p => p.Name);
```

### Step 31 — LINQ: OrderBy, GroupBy, Distinct
Sort with `OrderBy/OrderByDescending`, group with `GroupBy`, remove duplicates with `Distinct`.

### Step 32 — LINQ: First, Single, Any, All, Count
Use aggregation and existence checks: `FirstOrDefault()`, `SingleOrDefault()`, `Any()`, `All()`, `Count()`.

### Step 33 — LINQ: Sum, Min, Max, Average
Numeric aggregations on collections.
```csharp
double avg = scores.Average();
int total = scores.Sum();
```

### Step 34 — LINQ Query Syntax vs Method Syntax
Know both styles. Method syntax is more common; query syntax is more readable for complex joins.
```csharp
// Method syntax
var result = list.Where(x => x > 5).OrderBy(x => x);

// Query syntax
var result = from x in list where x > 5 orderby x select x;
```

---

## 🔴 Stage 5: Error Handling & Other Essentials (Steps 35–40)

### Step 35 — Exception Handling (try / catch / finally)
Wrap risky code in `try`. Catch specific exceptions. Use `finally` for cleanup.
```csharp
try { int.Parse("abc"); }
catch (FormatException ex) { Console.WriteLine(ex.Message); }
finally { Console.WriteLine("Done"); }
```

### Step 36 — Custom Exceptions
Create your own exception classes inheriting from `Exception`.
```csharp
class AgeException : Exception { public AgeException(string msg) : base(msg) {} }
```

### Step 37 — Nullable Types & Null Safety
Use `int?`, `string?`. Understand null-conditional (`?.`), null-coalescing (`??`), and null-forgiving (`!`) operators.
```csharp
int? age = null;
int result = age ?? 0;
```

### Step 38 — Delegates & Events (Basics)
Understand what a delegate is — a type-safe function pointer. Learn basic event wiring with `+=` and `-=`.
```csharp
Action<string> greet = name => Console.WriteLine($"Hello {name}");
```

### Step 39 — Lambda Expressions & Func/Action
Write concise anonymous functions with `=>`. Use `Func<T, TResult>` and `Action<T>` as parameter types.
```csharp
Func<int, int, int> add = (a, b) => a + b;
```

### Step 40 — var, const, readonly & Pattern Matching
Use `var` for type inference. Distinguish `const` vs `readonly`. Master `switch` pattern matching with `is` and `when`.
```csharp
if (obj is string s && s.Length > 0) Console.WriteLine(s);
```

---

## 📌 Recommended Practice Order

| Priority | Topic |
|---|---|
| 🔥 First | Steps 1–10 (Language Basics) |
| 🔥 Second | Steps 11–14 (List, Dictionary) |
| 🔥 Third | Steps 21–26 (OOP) |
| 🔥 Fourth | Steps 29–34 (LINQ) |
| 🔥 Fifth | Steps 35–40 (Exceptions, Delegates) |

---

## 📚 Recommended Resources

- [Microsoft C# Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [C# Fundamentals for Absolute Beginners – Channel 9](https://learn.microsoft.com/en-us/shows/csharp-fundamentals-for-absolute-beginners/)
- [LeetCode Easy problems](https://leetcode.com) — practice with C#
- [dotnetfiddle.net](https://dotnetfiddle.net) — run C# in browser

---

*Good luck! 🚀 Master the fundamentals and everything else becomes much easier.*
