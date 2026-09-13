```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class TargetHost
{
    public string IpAddress { get; set; }
    public int Port { get; set; }
    public string Service { get; set; }
    public bool IsCritical { get; set; }
}

class Program
{
    static void Main()
    {
        List<TargetHost> scanData = new List<TargetHost>
        {
            new TargetHost { IpAddress = "10.0.0.5", Port = 445, Service = "SMB", IsCritical = true },
            new TargetHost { IpAddress = "10.0.0.2", Port = 80, Service = "HTTP", IsCritical = false },
            new TargetHost { IpAddress = "10.0.0.5", Port = 445, Service = "SMB", IsCritical = true },
            new TargetHost { IpAddress = "10.0.0.10", Port = 22, Service = "SSH", IsCritical = false },
            new TargetHost { IpAddress = "10.0.0.2", Port = 443, Service = "HTTP", IsCritical = false },
            new TargetHost { IpAddress = "10.0.0.12", Port = 3389, Service = "RDP", IsCritical = true }
        };

        var filteredMethod = scanData.Where(h => h.IsCritical);
        var filteredQuery = from h in scanData where h.IsCritical select h;

        var transformedHosts = scanData.Select(h => $"{h.IpAddress}:{h.Port}");

        TargetHost[] criticalArray = scanData.Where(h => h.IsCritical).ToArray();
        List<TargetHost> criticalList = scanData.Where(h => h.IsCritical).ToList();

        var sortedPorts = scanData.OrderBy(h => h.Port).ToList();

        var uniqueIps = scanData.Select(h => h.IpAddress).Distinct().ToList();

        var groupedServices = scanData.GroupBy(h => h.Service);

        bool hasCritical = scanData.Any(h => h.IsCritical);
        bool allHttp = scanData.All(h => h.Service == "HTTP");
        int totalTargets = scanData.Count(h => h.IsCritical);

        TargetHost firstSmb = scanData.FirstOrDefault(h => h.Service == "SMB");
        TargetHost singleRdp = scanData.SingleOrDefault(h => h.Service == "RDP");

        Console.WriteLine($"Has Critical: {hasCritical}");
        Console.WriteLine($"All HTTP: {allHttp}");
        Console.WriteLine($"Total Critical Count: {totalTargets}");
        Console.WriteLine($"First SMB Target: {firstSmb?.IpAddress}");
        Console.WriteLine($"Single RDP Target: {singleRdp?.IpAddress}\n");

        Console.WriteLine("Unique IPs:");
        foreach (var ip in uniqueIps)
        {
            Console.WriteLine(ip);
        }

        Console.WriteLine("\nGrouped Services:");
        foreach (var group in groupedServices)
        {
            Console.WriteLine($"Service: {group.Key}");
            foreach (var item in group)
            {
                Console.WriteLine($" - {item.IpAddress}:{item.Port}");
            }
        }
    }
}
```
