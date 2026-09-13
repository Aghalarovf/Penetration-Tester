```csharp
using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        List<int> ports = new List<int> { 21, 22, 80, 443, 8080, 3389, 25 };

        var webPortsMethod = ports.Where(p => p == 80 || p == 443 || p == 8080);

        var webPortsQuery = from p in ports
                            where p == 80 || p == 443 || p == 8080
                            select p;

        List<string> hosts = new List<string> { "192.168.1.1", "10.0.0.1", "172.16.0.1" };

        var webUrlsMethod = hosts.Select(ip => $"https://{ip}");

        var webUrlsQuery = from ip in hosts
                   select $"https://{ip}";

        foreach (var port in webPortsMethod)
        {
            Console.WriteLine(port);
        }

        foreach (var url in webUrlsMethod)
        {
            Console.WriteLine(url);
        }
    }
}
```
