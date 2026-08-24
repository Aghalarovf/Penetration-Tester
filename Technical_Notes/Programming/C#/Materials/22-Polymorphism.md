# Step 20 — Polymorphism & Virtual Methods

---

## What is Polymorphism?

Polymorphism means one type reference can point to different object types at runtime, each behaving differently.

```csharp
C2Channel channel = new HttpChannel();   // base type, child object
channel.Receive();                       // runs HttpChannel's version

channel = new TcpChannel();             // swapped at runtime
channel.Receive();                       // runs TcpChannel's version
```

Same call, different behaviour — that's polymorphism.

---

## `virtual` and `override`

- `virtual` — base class marks a method as overridable
- `override` — child class replaces the base implementation

```csharp
class C2Channel
{
    public virtual string Receive()
    {
        return "";   // default — does nothing
    }
}

class HttpChannel : C2Channel
{
    public override string Receive()
    {
        return PollHttp();   // replaces the base version
    }
}

class TcpChannel : C2Channel
{
    public override string Receive()
    {
        return ReadSocket();   // replaces the base version
    }
}
```

Usage:
```csharp
C2Channel ch = new HttpChannel();
ch.Receive();   // calls HttpChannel.Receive()

ch = new TcpChannel();
ch.Receive();   // calls TcpChannel.Receive()
```

---

## Calling the Base Method (`base`)

Override the method but still run the parent's logic.

```csharp
class C2Channel
{
    public virtual void Send(string data)
    {
        Console.WriteLine($"[base] sending {data.Length} bytes");
    }
}

class HttpChannel : C2Channel
{
    public override void Send(string data)
    {
        base.Send(data);              // runs parent's logging first
        PostHttp(data);              // then does its own work
    }
}
```

---

## `abstract` — Force Child to Implement

`abstract` is like `virtual` but with no default implementation. The base class cannot be instantiated directly.

```csharp
abstract class C2Channel
{
    // must be implemented — no body here
    public abstract string Receive();
    public abstract void   Send(string data);

    // non-abstract — shared logic, no override required
    public void Heartbeat()
    {
        Console.WriteLine("ping");
    }
}

class HttpChannel : C2Channel
{
    public override string Receive()         => PollHttp();
    public override void   Send(string data) => PostHttp(data);
}

// C2Channel ch = new C2Channel();   // error — abstract class
C2Channel ch = new HttpChannel();    // ok
```

---

## `virtual` vs `abstract` vs `interface`

| | `virtual` | `abstract` | `interface` |
|---|---|---|---|
| Has default body | Yes | No | No (C# 8+ optionally yes) |
| Must override | No | Yes | Yes |
| Can instantiate | Yes | No | No |
| Multiple inheritance | No | No | Yes |
| Use when | Optional override | Forced override, shared base | Unrelated types, same contract |

---

## Polymorphism with a List

Store different types behind the base type — call them all the same way.

```csharp
List<C2Channel> channels = new List<C2Channel>
{
    new HttpChannel(),
    new TcpChannel(),
    new DnsChannel()
};

foreach (C2Channel ch in channels)
{
    string task = ch.Receive();    // each runs its own version
    Console.WriteLine(task);
}
```

---

## `sealed` — Prevent Further Overriding

```csharp
class HttpChannel : C2Channel
{
    public sealed override string Receive() => PollHttp();
    // no subclass of HttpChannel can override Receive() anymore
}
```

---

## Full Example

```csharp
abstract class C2Channel
{
    public string Name { get; protected set; }

    public abstract string Receive();
    public abstract void   Send(string data);

    public virtual void Heartbeat()
    {
        Console.WriteLine($"[{Name}] heartbeat");
    }
}

class HttpChannel : C2Channel
{
    public HttpChannel() { Name = "HTTP"; }

    public override string Receive()
    {
        Console.WriteLine("[HTTP] polling...");
        return "run whoami";
    }

    public override void Send(string data)
    {
        Console.WriteLine($"[HTTP] POST {data}");
    }
}

class TcpChannel : C2Channel
{
    public TcpChannel() { Name = "TCP"; }

    public override string Receive()
    {
        Console.WriteLine("[TCP] reading socket...");
        return "run ipconfig";
    }

    public override void Send(string data)
    {
        Console.WriteLine($"[TCP] write {data}");
    }

    public override void Heartbeat()   // optionally override
    {
        base.Heartbeat();
        Console.WriteLine("[TCP] keepalive sent");
    }
}

List<C2Channel> channels = new() { new HttpChannel(), new TcpChannel() };

foreach (C2Channel ch in channels)
{
    ch.Heartbeat();
    string task = ch.Receive();
    ch.Send($"result of: {task}");
    Console.WriteLine("---");
}
```

---

## Quick Reference

| Keyword | Where | Meaning |
|---|---|---|
| `virtual` | Base class method | Can be overridden |
| `override` | Child class method | Replaces base version |
| `abstract` | Base class method | Must be overridden, no body |
| `abstract class` | Class declaration | Cannot instantiate directly |
| `sealed` | Override method | No further overriding allowed |
| `base.Method()` | Inside override | Call parent's version |
