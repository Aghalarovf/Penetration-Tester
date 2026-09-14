```csharp
using System;
using System.IO;

class Program
{
    static void Main()
    {
        string rootPath = AppDomain.CurrentDomain.BaseDirectory;
        string wordlistPath = Path.Combine(rootPath, "wordlist.txt");
        string outputPath = Path.Combine(rootPath, "filtered_results.txt");

        CreateSampleWordlist(wordlistPath);

        try
        {
            using (StreamReader reader = new StreamReader(wordlistPath))
            using (StreamWriter writer = new StreamWriter(outputPath, append: false))
            {
                string? currentLine;
                int totalProcessed = 0;
                int matchesFound = 0;

                while ((currentLine = reader.ReadLine()) != null)
                {
                    totalProcessed++;

                    if (IsTargetMatch(currentLine))
                    {
                        writer.WriteLine(currentLine);
                        matchesFound++;
                    }
                }

                Console.WriteLine($"Total Lines Analyzed: {totalProcessed}");
                Console.WriteLine($"Filtered Matches Saved: {matchesFound}");
            }

            DisplayPathMetadata(wordlistPath);
            DisplayPathMetadata(outputPath);
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: File missing -> {ex.FileName}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"I/O Exception: {ex.Message}");
        }
    }

    static bool IsTargetMatch(string entry)
    {
        return entry.Length >= 8 && entry.StartsWith("admin", StringComparison.OrdinalIgnoreCase);
    }

    static void DisplayPathMetadata(string path)
    {
        Console.WriteLine($"\nFile Info:");
        Console.WriteLine($"Full Directory: {Path.GetDirectoryName(path)}");
        Console.WriteLine($"Filename: {Path.GetFileName(path)}");
        Console.WriteLine($"Filename Without Extension: {Path.GetFileNameWithoutExtension(path)}");
        Console.WriteLine($"Extension: {Path.GetExtension(path)}");
    }

    static void CreateSampleWordlist(string path)
    {
        if (!File.Exists(path))
        {
            string[] items = 
            {
                "admin",
                "administrator_pass_2026",
                "user123",
                "admin_root_token",
                "guest_account",
                "admin_secure_99"
            };

            File.WriteAllLines(path, items);
        }
    }
}
```
