```csharp
using System;
using System.Text;

class Program
{
    static void Main()
    {
        string exampleText = "Confidential Payload Data 2026";
        byte xorKey = 0x5A;

        byte[] originalBytes = Encoding.UTF8.GetBytes(exampleText);

        byte[] xorEncryptedBytes = new byte[originalBytes.Length];
        for (int i = 0; i < originalBytes.Length; i++)
        {
            xorEncryptedBytes[i] = (byte)(originalBytes[i] ^ xorKey);
        }

        string base64Encoded = Convert.ToBase64String(xorEncryptedBytes);

        Console.WriteLine($"Original Text: {exampleText}");
        Console.WriteLine($"Original Bytes (Hex): {BitConverter.ToString(originalBytes)}");
        Console.WriteLine($"XOR Encrypted Bytes (Hex): {BitConverter.ToString(xorEncryptedBytes)}");
        Console.WriteLine($"Final Base64 Result: {base64Encoded}");

        Console.WriteLine("\n--- Decryption Process ---");

        byte[] decodedBase64Bytes = Convert.FromBase64String(base64Encoded);

        byte[] xorDecryptedBytes = new byte[decodedBase64Bytes.Length];
        for (int i = 0; i < decodedBase64Bytes.Length; i++)
        {
            xorDecryptedBytes[i] = (byte)(decodedBase64Bytes[i] ^ xorKey);
        }

        string decryptedText = Encoding.UTF8.GetString(xorDecryptedBytes);

        Console.WriteLine($"Restored Text: {decryptedText}");
    }
}
```
