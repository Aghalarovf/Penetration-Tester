### Write File
```csharp
using System;
using System.IO;

class Program
{
    static void Main()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample.txt");

        Console.WriteLine($"Directory: {Path.GetDirectoryName(filePath)}");
        Console.WriteLine($"Filename Without Extension: {Path.GetFileNameWithoutExtension(filePath)}");
        Console.WriteLine($"Extension: {Path.GetExtension(filePath)}");

        File.WriteAllText(filePath, "Hello, World!");
        Console.WriteLine("File written successfully.");
    }
}
```

### Reading from File
```csharp
using System;
using System.IO;

class Program
{
    static void Main()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample.txt");

        Console.WriteLine($"Directory: {Path.GetDirectoryName(filePath)}");
        Console.WriteLine($"Filename Without Extension: {Path.GetFileNameWithoutExtension(filePath)}");
        Console.WriteLine($"Extension: {Path.GetExtension(filePath)}");

        if (File.Exists(filePath))
        {
            string content = File.ReadAllText(filePath);
            Console.WriteLine($"Content: {content}");
        }
    }
}
```

### File Existence Check
```csharp
using System;
using System.IO;

class Program
{
    static void Main()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample.txt");

        Console.WriteLine($"Directory: {Path.GetDirectoryName(filePath)}");
        Console.WriteLine($"Filename Without Extension: {Path.GetFileNameWithoutExtension(filePath)}");
        Console.WriteLine($"Extension: {Path.GetExtension(filePath)}");

        if (File.Exists(filePath))
        {
            Console.WriteLine("File exists.");
        }
        else
        {
            Console.WriteLine("File does not exist.");
        }
    }
}
```

### Directory Existence Check
```csharp
using System;
using System.IO;

class Program
{
    static void Main()
    {
        string dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyFolder");
        string dummyPath = Path.Combine(dirPath, "data.txt");

        Console.WriteLine($"Directory: {Path.GetDirectoryName(dummyPath)}");
        Console.WriteLine($"Filename Without Extension: {Path.GetFileNameWithoutExtension(dummyPath)}");
        Console.WriteLine($"Extension: {Path.GetExtension(dummyPath)}");

        if (Directory.Exists(dirPath))
        {
            Console.WriteLine("Directory exists.");
        }
        else
        {
            Directory.CreateDirectory(dirPath);
            Console.WriteLine("Directory did not exist, so it was created.");
        }
    }
}
```
