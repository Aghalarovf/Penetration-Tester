# Step 18 — Constructors

---

## What is a Constructor?

A constructor is a special method that runs automatically when an object is created with `new`. Used to set the object's initial state.

Rules:
- Same name as the class
- No return type (not even `void`)
- Can be overloaded (multiple constructors with different parameters)

---

## No-Arg Constructor (Default)

If you don't define any constructor, C# provides one automatically. If you define any constructor yourself, the default is removed.

```csharp
class ReverseShell
{
    public string Host { get; set; }
    public int    Port { get; set; }

    public ReverseShell()           // no-arg constructor
    {
        Host = "127.0.0.1";
        Port = 4444;
    }
}

ReverseShell shell = new ReverseShell();
Console.WriteLine(shell.Host);   // 127.0.0.1
```

---

## Parameterised Constructor

Accept values at creation time.

```csharp
class ReverseShell
{
    private string _host;
    private int    _port;

    public ReverseShell(string host, int port)
    {
        _host = host;
        _port = port;
    }

    public void Connect()
    {
        Console.WriteLine($"Connecting to {_host}:{_port}");
    }
}

ReverseShell shell = new ReverseShell("10.0.0.1", 4444);
shell.Connect();   // Connecting to 10.0.0.1:4444
```

---

## Constructor Overloading

Multiple constructors with different signatures.

```csharp
class BeaconConfig
{
    public string C2Host { get; set; }
    public int    Port   { get; set; }
    public int    Sleep  { get; set; }

    // minimum required
    public BeaconConfig(string host)
    {
        C2Host = host;
        Port   = 443;
        Sleep  = 60;
    }

    // full control
    public BeaconConfig(string host, int port, int sleep)
    {
        C2Host = host;
        Port   = port;
        Sleep  = sleep;
    }
}

BeaconConfig cfg1 = new BeaconConfig("10.0.0.1");
BeaconConfig cfg2 = new BeaconConfig("10.0.0.1", 8443, 30);
```

---

## Constructor Chaining (`this(...)`)

Avoid repeating initialization logic — one constructor calls another.

```csharp
class BeaconConfig
{
    public string C2Host { get; set; }
    public int    Port   { get; set; }
    public int    Sleep  { get; set; }

    public BeaconConfig(string host) : this(host, 443, 60) { }

    public BeaconConfig(string host, int port, int sleep)
    {
        C2Host = host;
        Port   = port;
        Sleep  = sleep;
    }
}

// Both end up running the same core logic
BeaconConfig cfg = new BeaconConfig("10.0.0.1");
Console.WriteLine(cfg.Port);    // 443
Console.WriteLine(cfg.Sleep);   // 60
```

---

## Read-Only Fields via Constructor

`readonly` fields can only be set in the constructor. Useful for values that must never change after creation.

```csharp
class Implant
{
    public readonly string AgentId;

    public Implant()
    {
        AgentId = Guid.NewGuid().ToString();   // set once, never changes
    }
}

Implant implant = new Implant();
Console.WriteLine(implant.AgentId);
// implant.AgentId = "other";   // error — readonly
```

---

## Object Initializer (No Custom Constructor Needed)

Set public properties at creation using `{ }` syntax.

```csharp
class BeaconConfig
{
    public string C2Host { get; set; }
    public int    Port   { get; set; }
}

BeaconConfig cfg = new BeaconConfig
{
    C2Host = "10.0.0.1",
    Port   = 443
};
```

Works alongside constructors — if a constructor is defined, it runs first, then the initializer sets properties.

---

## Quick Reference

| Pattern | Syntax | Use when |
|---|---|---|
| No-arg constructor | `public Foo() { }` | Sensible defaults |
| Parameterised | `public Foo(string x) { }` | Required values at creation |
| Overloading | Multiple `public Foo(...)` | Optional parameters |
| Chaining | `: this(...)` | Avoid duplicate logic |
| Read-only field | `public readonly T X;` | Value must never change |
| Object initializer | `new Foo { X = 1 }` | Quick setup, no custom constructor |
