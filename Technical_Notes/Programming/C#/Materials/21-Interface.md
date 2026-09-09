# Basic Interface
```csharp
public interface IScanner
{
    string Target { get; set; }
    bool Scan();
    string GetResult();
}

class PortScanner : IScanner
{
    public string Target { get; set; }

    public PortScanner(string target)
    {
        Target = target;
    }
    public bool Scan()
    {
        return true;
    }

    public string GetResult()
    {
        return $"Port 80 is open";
    }
}
```
