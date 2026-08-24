# C# Classes & Objects Cheat Sheet

---

## Class vs Object

| Term | What it is | Analogy |
|---|---|---|
| `class` | Blueprint / template | Cookie cutter |
| `object` | Instance created from class | The actual cookie |

```csharp
class PortScanner { }              // blueprint
PortScanner scanner = new PortScanner();  // object (instance)
```

---

## Basic Class Structure

```csharp
class PortScanner
{
    // Fields — raw variables (usually private)
    private int _timeout = 3000;

    // Properties — controlled access to data
    public string Target { get; set; }
    public List<int> OpenPorts { get; set; } = new();

    // Constructor — runs when object is created
    public PortScanner(string target)
    {
        Target = target;
    }

    // Method — action the class can perform
    public void AddPort(int port)
    {
        OpenPorts.Add(port);
    }
}
```

---

## Properties

Controlled access to a class's data. Safer than public fields.

```csharp
public string Target { get; set; }        // read + write
public string Host   { get; private set; } // read publicly, write only inside class
public int Count     { get; }              // read-only (set only in constructor)
```

Default value on declaration:
```csharp
public List<int> OpenPorts { get; set; } = new();
public int Timeout { get; set; } = 3000;
```

---

## Constructor

Runs automatically when an object is created. Used to set initial state.

```csharp
class PortScanner
{
    public string Target { get; set; }
    public List<int> OpenPorts { get; set; } = new();

    // No-arg constructor
    public PortScanner()
    {
        Target = "127.0.0.1";
    }

    // Parameterised constructor
    public PortScanner(string target)
    {
        Target = target;
    }
}

PortScanner s1 = new PortScanner();             // Target = "127.0.0.1"
PortScanner s2 = new PortScanner("10.0.0.1");   // Target = "10.0.0.1"
```

---

## Methods

Actions the class can perform. Can read/modify the object's own data.

```csharp
class PortScanner
{
    public string Target { get; set; }
    public List<int> OpenPorts { get; set; } = new();

    public PortScanner(string target) { Target = target; }

    // void — returns nothing
    public void AddPort(int port)
    {
        OpenPorts.Add(port);
    }

    // returns a value
    public bool IsOpen(int port)
    {
        return OpenPorts.Contains(port);
    }

    // reads own data
    public void PrintResults()
    {
        Console.WriteLine($"Target: {Target}");
        foreach (int port in OpenPorts)
            Console.WriteLine($"  Open: {port}");
    }
}
```

Usage:
```csharp
PortScanner scanner = new PortScanner("192.168.1.1");
scanner.AddPort(80);
scanner.AddPort(443);
bool found = scanner.IsOpen(80);   // true
scanner.PrintResults();
// Target: 192.168.1.1
//   Open: 80
//   Open: 443
```

---

## Multiple Instances

Each object is independent — changing one does not affect another.

```csharp
PortScanner s1 = new PortScanner("10.0.0.1");
PortScanner s2 = new PortScanner("10.0.0.2");

s1.AddPort(22);
s2.AddPort(3389);

// s1.OpenPorts → [22]
// s2.OpenPorts → [3389]   completely separate
```

---

## Access Modifiers

| Modifier | Accessible from |
|---|---|
| `public` | Anywhere |
| `private` | Inside the class only |
| `protected` | Inside class + subclasses |
| `internal` | Same project only |

Convention: fields are `private`, properties and methods are `public`.

```csharp
class Beacon
{
    private string _secretKey;          // only Beacon can touch this
    public  string C2Server { get; set; } // anyone can read/write
    
    private void Encrypt() { }          // internal helper
    public  void Send()    { }          // callable from outside
}
```

---

## `this` Keyword

Refers to the current object. Useful when parameter names clash with property names.

```csharp
class Implant
{
    public string Host { get; set; }

    public Implant(string host)
    {
        this.Host = host;   // "this.Host" = property, "host" = parameter
    }
}
```

---

## Static Members

Belong to the class itself, not to any instance. Shared across all objects.

```csharp
class PortScanner
{
    public static int TotalScans { get; private set; } = 0;  // shared counter

    public PortScanner(string target)
    {
        TotalScans++;   // increments for every new scanner created
    }
}

PortScanner s1 = new PortScanner("10.0.0.1");
PortScanner s2 = new PortScanner("10.0.0.2");
Console.WriteLine(PortScanner.TotalScans);   // 2
```

---

## Object Initializer Syntax

Set properties at creation without a custom constructor.

```csharp
PortScanner scanner = new PortScanner
{
    Target    = "192.168.1.1",
    Timeout   = 5000
};
```

---

## Full Example

```csharp
class Implant
{
    public string Host       { get; set; }
    public int    Port       { get; set; }
    public bool   IsActive   { get; private set; }

    private string _sessionId;

    public Implant(string host, int port)
    {
        Host      = host;
        Port      = port;
        IsActive  = false;
        _sessionId = Guid.NewGuid().ToString();
    }

    public void Connect()
    {
        IsActive = true;
        Console.WriteLine($"[+] Connected to {Host}:{Port} (session: {_sessionId})");
    }

    public void Disconnect()
    {
        IsActive = false;
        Console.WriteLine($"[-] Disconnected from {Host}");
    }
}

Implant implant = new Implant("10.0.0.1", 4444);
implant.Connect();
Console.WriteLine(implant.IsActive);   // true
implant.Disconnect();
```

---

## Quick Reference

| Concept | Keyword / Syntax | Purpose |
|---|---|---|
| Define a class | `class Foo { }` | Create a blueprint |
| Create an object | `new Foo()` | Instantiate the blueprint |
| Property (read+write) | `public T Name { get; set; }` | Expose data safely |
| Property (read-only) | `public T Name { get; }` | Prevent external writes |
| Constructor | Same name as class, no return type | Initialize on creation |
| Method | `public void DoSomething() { }` | Define an action |
| Current object | `this` | Refer to own members |
| Shared member | `static` | One copy for all instances |
