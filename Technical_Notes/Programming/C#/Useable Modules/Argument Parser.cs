using System;

class Exercise10_CLIArgumentReader
{
    static void Main(string[] args)
    {
        string target = "";
        string portStr = "";

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--target" && i + 1 < args.Length)
                target = args[++i];
            else if (args[i] == "--port" && i + 1 < args.Length)
                portStr = args[++i];
        }

        if (string.IsNullOrWhiteSpace(target))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: --target must not be empty.");
            Console.ResetColor();
            return;
        }

        if (!int.TryParse(portStr, out int port) || port < 1 || port > 65535)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: --port '{portStr}' is invalid. Must be 1–65535.");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Target : {target}");
        Console.WriteLine($"Port   : {port}");
        Console.ResetColor();
    }
}
