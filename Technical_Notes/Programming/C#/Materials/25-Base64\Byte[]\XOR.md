### BASE64
```csharp
using System;
using System.Text;

class Program
{
    static void Main()
    {
        string exampleText = "Confidential Payload Data 2026";

        byte[] originalBytes = Encoding.UTF8.GetBytes(exampleText);
        string base64Encoded = Convert.ToBase64String(originalBytes);

        byte[] decodedBytes = Convert.FromBase64String(base64Encoded);
        string base64Decoded = Encoding.UTF8.GetString(decodedBytes);

        Console.WriteLine($"Original Text: {exampleText}");
        Console.WriteLine($"Base64 Encoded: {base64Encoded}");
        Console.WriteLine($"Base64 Decoded: {base64Decoded}");
    }
}
```

### XOR
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

        byte[] xorDecryptedBytes = new byte[xorEncryptedBytes.Length];
        for (int i = 0; i < xorEncryptedBytes.Length; i++)
        {
            xorDecryptedBytes[i] = (byte)(xorEncryptedBytes[i] ^ xorKey);
        }
        string xorDecryptedText = Encoding.UTF8.GetString(xorDecryptedBytes);

        Console.WriteLine($"Original Text: {exampleText}");
        Console.WriteLine($"XOR Encrypted (Hex): {BitConverter.ToString(xorEncryptedBytes)}");
        Console.WriteLine($"XOR Decrypted: {xorDecryptedText}");
    }
}
```
