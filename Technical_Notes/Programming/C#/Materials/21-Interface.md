# Step 19 — Interfaces

---

## What is an Interface?

An interface defines **what** a class must do, not **how**. It's a contract: any class that implements it must provide all the listed methods/properties.

- No implementation inside the interface (only signatures)
- A class can implement multiple interfaces
- By convention, interface names start with `I`

---

## Defining an Interface

```csharp
interface IC2Channel
{
    string Receive();           // must be implemented
    void   Send(string data);  // must be implemented
}
```

---

## Implementing an Interface

Every method in the interface must be implemented — otherwise the code won't compile.

```csharp
class HttpChannel : IC2Channel
{
    public string Receive()          => PollHttp();
    public void   Send(string data)  => PostHttp(data);
}

class TcpChannel : IC2Channel
{
    public string Receive()          => ReadSocket();
    public void   Send(string data)  => WriteSocket(data);
}

class DnsChannel : IC2Channel
{
    public string Receive()          => PollDns();
    public void   Send(string data)  => EncodeDns(data);
}
```

---

## Using an Interface as a Type

The key benefit: code that uses `IC2Channel` doesn't care which channel it is.

```csharp
IC2Channel channel = new HttpChannel();
channel.Send("checkin");
string task = channel.Receive();

// swap to TCP — calling code stays identical
channel = new TcpChannel();
channel.Send("checkin");
```

---

## Interface with Properties

```csharp
interface IImplant
{
    string AgentId  { get; }
    bool   IsActive { get; set; }
    void   Connect();
    void   Disconnect();
}

class Implant : IImplant
{
    public string AgentId  { get; } = Guid.NewGuid().ToString();
    public bool   IsActive { get; set; }

    public void Connect()    => IsActive = true;
    public void Disconnect() => IsActive = false;
}
```

---

## Multiple Interfaces

A class can implement more than one interface.

```csharp
interface IC2Channel
{
    void Send(string data);
    string Receive();
}

interface ILoggable
{
    void Log(string message);
}

class HttpChannel : IC2Channel, ILoggable
{
    public void   Send(string data)  => PostHttp(data);
    public string Receive()          => PollHttp();
    public void   Log(string msg)    => Console.WriteLine($"[LOG] {msg}");
}
```

---

## Interface vs Inheritance

| | Interface | Inheritance (`class : BaseClass`) |
|---|---|---|
| Defines | What to do (contract) | What to do + how (base behaviour) |
| Multiple allowed | Yes (many interfaces) | No (one base class only) |
| Has implementation | No (signatures only) | Yes (can have default code) |
| Use when | Unrelated classes share a contract | Classes share common behaviour |

```csharp
// Interface — HttpChannel and TcpChannel are unrelated, but both are C2 channels
class HttpChannel : IC2Channel { ... }
class TcpChannel  : IC2Channel { ... }

// Inheritance — HttpChannel IS a C2Channel and reuses its base logic
class HttpChannel : C2Channel { ... }
```

---

## Default Interface Methods (C# 8+)

Interfaces can have a default implementation — implementing classes can override it but don't have to.

```csharp
interface IC2Channel
{
    void Send(string data);
    string Receive();

    void Heartbeat()   // default — optional to override
    {
        Console.WriteLine("ping");
    }
}
```

---

## Full Example

```csharp
interface IC2Channel
{
    string Name    { get; }
    string Receive();
    void   Send(string data);
}

class HttpChannel : IC2Channel
{
    public string Name => "HTTP";
    public string Receive()         => "[http] task received";
    public void   Send(string data) => Console.WriteLine($"[http] sending: {data}");
}

class DnsChannel : IC2Channel
{
    public string Name => "DNS";
    public string Receive()         => "[dns] task received";
    public void   Send(string data) => Console.WriteLine($"[dns] sending: {data}");
}

class Implant
{
    private IC2Channel _channel;

    public Implant(IC2Channel channel)
    {
        _channel = channel;
    }

    public void CheckIn()
    {
        _channel.Send("checkin");
        string task = _channel.Receive();
        Console.WriteLine($"Task via {_channel.Name}: {task}");
    }
}

Implant implant = new Implant(new HttpChannel());
implant.CheckIn();
// [http] sending: checkin
// Task via HTTP: [http] task received

implant = new Implant(new DnsChannel());
implant.CheckIn();
// [dns] sending: checkin
// Task via DNS: [dns] task received
```

---

## Quick Reference

| Concept | Syntax |
|---|---|
| Define interface | `interface IFoo { void Bar(); }` |
| Implement interface | `class Foo : IFoo { public void Bar() { } }` |
| Multiple interfaces | `class Foo : IFoo, IBar { }` |
| Use as type | `IFoo obj = new Foo();` |
| Property in interface | `string Name { get; }` |
| Default method (C# 8+) | Method with body inside interface |
