# Swith Statement
```csharp
using System;

class PortServiceMapper
{
    static void Main(string[] args)
    {
        Console.Write("Enter the Port: ");
        string Port = Console.ReadLine();

        switch(Port)
        {
            case "21": Console.WriteLine("This port is FTP");       break;
            case "22": Console.WriteLine("This port is SSH");       break;
            case "23": Console.WriteLine("This port is Telnet");    break;
            case "25": Console.WriteLine("This port is SMTP");      break;
            case "53": Console.WriteLine("This port is DNS");       break;
            case "80": Console.WriteLine("This port is HTTP");      break;
            case "443": Console.WriteLine("This port is HTTPs");    break;
            case "3306": Console.WriteLine("This port is MySQL");   break;
            case "3389": Console.WriteLine("This port is RDP");     break;
            default:
                 Console.WriteLine("Unknown Port");
                 break;
        }
    }
}
```

# Switch Expression
```csharp
using System;

class PortServiceMapper
{
    static string GetService(int port)
    {
        return port switch
        {
            21   => "FTP",
            22   => "SSH",
            23   => "Telnet",
            25   => "SMTP",
            53   => "DNS",
            80   => "HTTP",
            443  => "HTTPS",
            3306 => "MySQL",
            3389 => "RDP",
            _    => "UNKNOWN"
        };
    }

    static void Main(string[] args)
    {
        int[] ports = { 22, 80, 443, 1337, 3389, 53 };

        foreach (int port in ports)
        {
            Console.WriteLine($"{port,-4} → {GetService(port)}");
        }
    }
}
```
