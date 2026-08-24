# Step 17 — Properties & Access Modifiers

---

## Access Modifiers

| Modifier | Accessible from |
|---|---|
| `public` | Anywhere |
| `private` | Inside the class only |
| `internal` | Same project (assembly) only |
| `protected` | Inside class + subclasses |
| `private protected` | Subclasses in the same assembly only |

```csharp
class BeaconConfig
{
    public  string C2Host  = "10.0.0.1";   // anyone can read/write
    private int    _port   = 4444;          // only BeaconConfig can touch
    internal string AgentId = "abc123";     // accessible within same project
}
```

---

## Properties

A property is a controlled wrapper around a field. It looks like a field from outside but lets you control read/write access.

### Full property (with backing field)

```csharp
class BeaconConfig
{
    private int _sleepInterval = 60;

    public int SleepInterval
    {
        get { return _sleepInterval; }
        set
        {
            if (value > 0)
                _sleepInterval = value;   // validation before writing
        }
    }
}

BeaconConfig cfg = new BeaconConfig();
cfg.SleepInterval = 30;    // calls set
int s = cfg.SleepInterval; // calls get → 30
cfg.SleepInterval = -5;    // silently rejected (validation)
```

### Auto-property (no backing field needed)

```csharp
public string C2Host { get; set; }            // read + write
public string AgentId { get; private set; }   // read publicly, write inside class only
public int    Port    { get; }                // read-only (set only in constructor)
```

### Expression-bodied property (read-only shorthand)

```csharp
private int _sleepInterval = 60;
public int SleepInterval => _sleepInterval;   // same as { get { return _sleepInterval; } }
```

---

## Encapsulation Pattern

Hide internal state, expose only what's needed.

```csharp
class BeaconConfig
{
    // public — callers need this
    public string C2Host { get; set; }

    // private — internal detail, not exposed directly
    private int _sleepInterval = 60;

    // read-only property — callers can read, not write
    public int SleepInterval => _sleepInterval;

    // controlled setter — callers go through this
    public void SetSleepInterval(int seconds)
    {
        if (seconds >= 10 && seconds <= 3600)
            _sleepInterval = seconds;
    }
}

BeaconConfig cfg = new BeaconConfig();
cfg.C2Host = "10.0.0.1";
cfg.SetSleepInterval(120);
Console.WriteLine(cfg.SleepInterval);   // 120
// cfg._sleepInterval = 5;             // error — private
```

---

## Default Values

```csharp
public string C2Host   { get; set; } = "127.0.0.1";
public int    Port     { get; set; } = 4444;
public bool   UseHttps { get; set; } = true;
```

---

## Init-Only Property (C# 9+)

Can be set at creation only (object initializer), then becomes read-only.

```csharp
public string AgentId { get; init; }

BeaconConfig cfg = new BeaconConfig { AgentId = "abc123" };
// cfg.AgentId = "xyz";   // error after init
```

---

## Quick Reference

| Syntax | Read | Write |
|---|---|---|
| `{ get; set; }` | Anyone | Anyone |
| `{ get; private set; }` | Anyone | Class only |
| `{ get; init; }` | Anyone | Object initializer only |
| `{ get; }` | Anyone | Constructor only |
| `=> _field` | Anyone | Never (computed) |
