using System;
using System.Text;

class Exercise11_XOREncrypt
{
    static void Main()
    {
        Console.Write("Enter payload (text): ");
        string input = Console.ReadLine() ?? "";

        if (input == "")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: Payload cannot be empty.");
            Console.ResetColor();
            return;
        }

        byte key = 0xAA;
        byte[] original  = Encoding.UTF8.GetBytes(input);
        byte[] encrypted = new byte[original.Length];
        byte[] decrypted = new byte[original.Length];

        for (int i = 0; i < original.Length; i++)
            encrypted[i] = (byte)(original[i] ^ key);

        for (int i = 0; i < encrypted.Length; i++)
            decrypted[i] = (byte)(encrypted[i] ^ key);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Original  : " + ToHex(original));
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Encrypted : " + ToHex(encrypted));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Decrypted : " + ToHex(decrypted));

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nVerify: " + (input == Encoding.UTF8.GetString(decrypted) ? "OK" : "FAIL"));
        Console.ResetColor();
    }

    static string ToHex(byte[] data) =>
        string.Join(" ", Array.ConvertAll(data, b => $"0x{b:X2}"));
}
