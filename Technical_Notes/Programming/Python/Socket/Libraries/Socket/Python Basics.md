# Python Fundamentals Roadmap — 40 Steps to Master the Basics

> **Goal:** Strengthen your Python foundation step by step — variables, loops, comprehensions, collections, OOP, and more. No advanced modules, just solid fundamentals.

---

## 🟢 Stage 1: Language Basics (Steps 1–10)

### Step 1 — Hello World & Program Structure
Understand the entry point of a Python script. Learn how Python executes top-to-bottom. Run your first script.
```python
print("Hello, World!")
```

### Step 2 — Variables & Data Types
Learn the core types: `int`, `float`, `complex`, `bool`, `str`, `NoneType`. Understand dynamic typing — no need to declare types explicitly.
```python
age = 25
name = "Anar"
is_active = True
nothing = None
```

### Step 3 — Type Conversion
Understand explicit casting with `int()`, `float()`, `str()`, `bool()`. Use `type()` to inspect types at runtime.
```python
text = "42"
number = int(text)
pi_str = str(3.14)
```

### Step 4 — Operators
Arithmetic (`+`, `-`, `*`, `/`, `//`, `%`, `**`), comparison (`==`, `!=`, `>`, `<`), logical (`and`, `or`, `not`), and assignment operators (`+=`, `-=`, `*=`).
```python
result = 2 ** 8      # 256
quotient = 17 // 3   # 5 (floor division)
```

### Step 5 — String Operations
Learn `len()`, `.upper()`, `.lower()`, `.strip()`, `.replace()`, `.split()`, `.join()`, slicing (`s[1:5]`), and f-strings.
```python
full = f"Hello, {name}! You are {age} years old."
words = "hello world".split()   # ['hello', 'world']
```

### Step 6 — Conditional Statements (if / elif / else)
Control the flow of your program using `if`, `elif`, and `else`. Python uses indentation — no braces.
```python
if age >= 18:
    print("Adult")
elif age >= 13:
    print("Teen")
else:
    print("Child")
```

### Step 7 — Ternary Expression & Short-Circuit Evaluation
Use Python's one-line conditional expression. Understand how `and`/`or` short-circuit for safe defaults.
```python
label = "Adult" if age >= 18 else "Minor"
display = name or "Anonymous"
```

### Step 8 — Loops: for & while
Master `for` loops with `range()`. Use `while` for condition-based loops. Control flow with `break` and `continue`.
```python
for i in range(10):
    print(i)

count = 0
while count < 5:
    count += 1
```

### Step 9 — Loops: Nested Loops & else Clause
Use nested loops for 2D iteration. Learn the unusual `else` block on loops — runs when no `break` occurred.
```python
for i in range(3):
    for j in range(3):
        print(i, j)
```

### Step 10 — Functions
Define and call functions with `def`. Understand parameters, return values, default arguments, and `*args` / `**kwargs`.
```python
def add(a, b=0):
    return a + b

def greet(*names):
    for name in names:
        print(f"Hello, {name}!")
```

---

## 🔵 Stage 2: Collections & Data Structures (Steps 11–20)

### Step 11 — Lists
Create and use lists — Python's dynamic array. Index, slice, and mutate them. Core methods: `append()`, `remove()`, `pop()`, `sort()`, `len()`.
```python
numbers = [1, 2, 3, 4, 5]
numbers.append(6)
numbers.sort(reverse=True)
```

### Step 12 — List Slicing & Copying
Use slicing `[start:stop:step]` to extract sublists. Understand shallow vs deep copying with `copy()` and `copy.deepcopy()`.
```python
evens = numbers[::2]
reversed_list = numbers[::-1]
```

### Step 13 — Tuples
Immutable ordered sequences. Use for fixed data, multiple return values, and as dictionary keys. Unpack with ease.
```python
point = (3, 7)
x, y = point

def get_person():
    return "Anar", 25

name, age = get_person()
```

### Step 14 — Dictionaries
Store key-value pairs. Use `get()`, `keys()`, `values()`, `items()`, `update()`, `pop()`. Iterate safely with `.items()`.
```python
ages = {"Ali": 30, "Veli": 25}
ages["Anar"] = 28
print(ages.get("Unknown", 0))   # 0 (default)
```

### Step 15 — Sets
Unique element collections. Perform set operations: `union()`, `intersection()`, `difference()`, `issubset()`.
```python
a = {1, 2, 3}
b = {2, 3, 4}
print(a & b)   # {2, 3} — intersection
print(a | b)   # {1, 2, 3, 4} — union
```

### Step 16 — Stack & Queue Patterns
Implement LIFO stacks using a `list` (`append`/`pop`). Use `collections.deque` for efficient FIFO queues (`appendleft`/`pop`).
```python
from collections import deque
queue = deque()
queue.append("first")
queue.appendleft("zero")
```

### Step 17 — List Comprehensions
Create new lists in one concise line. Filter and transform simultaneously. More Pythonic than manual `for` loops.
```python
squares = [x ** 2 for x in range(10)]
evens = [x for x in range(20) if x % 2 == 0]
```

### Step 18 — Dict & Set Comprehensions
Apply the same comprehension syntax to dictionaries and sets for compact, readable data transformations.
```python
word_lengths = {word: len(word) for word in ["apple", "banana"]}
unique_squares = {x ** 2 for x in range(-3, 4)}
```

### Step 19 — Sorting & Searching Collections
Use `sorted()` and `list.sort()` with `key=` and `reverse=`. Search with `in`, `index()`, and `bisect` for sorted lists.
```python
people = [("Ali", 30), ("Veli", 25)]
people.sort(key=lambda p: p[1])   # sort by age
```

### Step 20 — `collections` Module
Master `Counter`, `defaultdict`, `OrderedDict`, `namedtuple`, and `deque` — built-in power tools for real-world data handling.
```python
from collections import Counter, defaultdict
counts = Counter("mississippi")
dd = defaultdict(list)
dd["key"].append(1)   # no KeyError
```

---

## 🟣 Stage 3: Object-Oriented Programming (Steps 21–28)

### Step 21 — Classes & Objects
Define a class with `class`. Create objects with `ClassName()`. Understand `self` — the reference to the current instance.
```python
class Car:
    def __init__(self, brand):
        self.brand = brand

my_car = Car("Toyota")
```

### Step 22 — Instance vs Class vs Static Methods
Understand `self` (instance method), `cls` (class method with `@classmethod`), and `@staticmethod` for utility functions.
```python
class Counter:
    count = 0

    @classmethod
    def increment(cls):
        cls.count += 1
```

### Step 23 — Properties & Encapsulation
Use `@property`, `@setter`, and `@deleter` for controlled attribute access. Prefix private attributes with `_` or `__`.
```python
class Person:
    def __init__(self, age):
        self._age = age

    @property
    def age(self):
        return self._age

    @age.setter
    def age(self, value):
        if value < 0:
            raise ValueError("Age cannot be negative")
        self._age = value
```

### Step 24 — Inheritance
Extend a base class using `class Child(Parent)`. Use `super()` to call parent methods. Override methods in the child class.
```python
class Animal:
    def speak(self):
        return "..."

class Dog(Animal):
    def speak(self):
        return "Woof!"
```

### Step 25 — Polymorphism & Duck Typing
Python's polymorphism comes from duck typing — if it walks like a duck and quacks like a duck, it is a duck. No explicit interface needed.
```python
def make_sound(animal):
    print(animal.speak())   # works for any object with speak()
```

### Step 26 — Abstract Classes & Interfaces
Use `abc.ABC` and `@abstractmethod` to enforce method implementation in subclasses. Python's equivalent of interfaces.
```python
from abc import ABC, abstractmethod

class Shape(ABC):
    @abstractmethod
    def area(self) -> float:
        pass

class Circle(Shape):
    def __init__(self, r): self.r = r
    def area(self): return 3.14 * self.r ** 2
```

### Step 27 — Magic / Dunder Methods
Customize class behavior with `__str__`, `__repr__`, `__len__`, `__eq__`, `__lt__`, `__add__`, and more.
```python
class Vector:
    def __init__(self, x, y):
        self.x, self.y = x, y

    def __add__(self, other):
        return Vector(self.x + other.x, self.y + other.y)

    def __repr__(self):
        return f"Vector({self.x}, {self.y})"
```

### Step 28 — Dataclasses & NamedTuples
Use `@dataclass` for clean, auto-generated `__init__`, `__repr__`, and `__eq__`. Use `NamedTuple` for lightweight immutable records.
```python
from dataclasses import dataclass

@dataclass
class Person:
    name: str
    age: int
```

---

## 🟡 Stage 4: Functional Tools & Comprehensions (Steps 29–34)

### Step 29 — Lambda Functions
Write short anonymous functions with `lambda`. Use for simple one-liners passed as arguments.
```python
square = lambda x: x ** 2
numbers.sort(key=lambda x: -x)
```

### Step 30 — `map()`, `filter()`, `reduce()`
Apply functions to collections. `map()` transforms, `filter()` selects, `reduce()` (from `functools`) folds.
```python
from functools import reduce
doubled = list(map(lambda x: x * 2, [1, 2, 3]))
evens = list(filter(lambda x: x % 2 == 0, range(10)))
total = reduce(lambda a, b: a + b, [1, 2, 3, 4])
```

### Step 31 — Generators & `yield`
Use `yield` to create lazy iterators that produce values one at a time — memory-efficient for large sequences.
```python
def countdown(n):
    while n > 0:
        yield n
        n -= 1

for num in countdown(5):
    print(num)
```

### Step 32 — Generator Expressions
Like list comprehensions, but lazy — values are generated on demand, not stored in memory all at once.
```python
total = sum(x ** 2 for x in range(1000000))   # no list created
```

### Step 33 — `enumerate()`, `zip()`, `any()`, `all()`
Essential built-ins for Pythonic loops and aggregation checks.
```python
for i, val in enumerate(["a", "b", "c"]):
    print(i, val)

pairs = list(zip([1, 2, 3], ["a", "b", "c"]))
print(all(x > 0 for x in [1, 2, 3]))   # True
```

### Step 34 — Decorators
Functions that wrap other functions to add behavior (logging, timing, authentication). Understand `@functools.wraps`.
```python
import functools

def logger(func):
    @functools.wraps(func)
    def wrapper(*args, **kwargs):
        print(f"Calling {func.__name__}")
        return func(*args, **kwargs)
    return wrapper

@logger
def greet(name):
    print(f"Hello, {name}!")
```

---

## 🔴 Stage 5: Error Handling & Other Essentials (Steps 35–40)

### Step 35 — Exception Handling (try / except / finally)
Wrap risky code in `try`. Catch specific exceptions with `except`. Use `finally` for cleanup, `else` when no error occurs.
```python
try:
    result = int("abc")
except ValueError as e:
    print(f"Error: {e}")
finally:
    print("Done")
```

### Step 36 — Custom Exceptions
Create your own exception classes by inheriting from `Exception`. Add custom attributes and messages.
```python
class AgeError(Exception):
    def __init__(self, age):
        super().__init__(f"Invalid age: {age}")
        self.age = age

raise AgeError(-5)
```

### Step 37 — `with` Statement & Context Managers
Use `with` for automatic resource management (files, locks, DB connections). Create your own with `__enter__`/`__exit__` or `contextlib`.
```python
with open("data.txt", "r") as f:
    content = f.read()   # file closes automatically
```

### Step 38 — File I/O
Read and write text and binary files. Use `open()` with modes `r`, `w`, `a`, `rb`. Work with `pathlib.Path` for modern file handling.
```python
from pathlib import Path

path = Path("output.txt")
path.write_text("Hello, file!")
content = path.read_text()
```

### Step 39 — Modules & Packages
Organize code into modules (`.py` files) and packages (directories with `__init__.py`). Understand `import`, `from ... import`, and `__name__ == "__main__"`.
```python
# mymodule.py
def greet(name):
    return f"Hello, {name}!"

# main.py
from mymodule import greet
print(greet("Anar"))
```

### Step 40 — Type Hints & `match` Statement
Add type hints for readability and IDE support (not enforced at runtime). Use Python 3.10+ `match` for structural pattern matching.
```python
def add(a: int, b: int) -> int:
    return a + b

match command:
    case "quit":
        print("Exiting...")
    case "help":
        print("Commands: quit, help")
    case _:
        print("Unknown command")
```

---

## 📌 Recommended Practice Order

| Priority | Topic |
|---|---|
| 🔥 First | Steps 1–10 (Language Basics) |
| 🔥 Second | Steps 11–15 (List, Dict, Set) |
| 🔥 Third | Steps 21–26 (OOP) |
| 🔥 Fourth | Steps 29–34 (Functional Tools) |
| 🔥 Fifth | Steps 35–40 (Exceptions, Modules) |

---

## 📚 Recommended Resources

- [Official Python Documentation](https://docs.python.org/3/)
- [Python Tutorial – docs.python.org](https://docs.python.org/3/tutorial/)
- [Real Python – realpython.com](https://realpython.com) — beginner to advanced guides
- [LeetCode Easy problems](https://leetcode.com) — practice with Python
- [Python Tutor](https://pythontutor.com) — visualize code execution in the browser

---

*Good luck! 🚀 Master the fundamentals and everything else becomes much easier.*
