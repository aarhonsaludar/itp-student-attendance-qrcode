using System;

// Compile: csc /reference:packages\BCrypt.Net-Next.4.0.3\lib\net472\BCrypt.Net-Next.dll GenerateHash.cs
// Run: GenerateHash.exe

class GenerateHash
{
    static void Main()
    {
        string password = "admin123";

        // Generate hash
        string hash = BCrypt.Net.BCrypt.HashPassword(password);

        Console.WriteLine("=== BCrypt Hash Generator ===");
        Console.WriteLine("Password: " + password);
        Console.WriteLine("Hash: " + hash);
        Console.WriteLine();

        // Verify it works
        bool isValid = BCrypt.Net.BCrypt.Verify(password, hash);
        Console.WriteLine("Verification: " + (isValid ? "SUCCESS" : "FAILED"));
        Console.WriteLine();

        // SQL Update statement
        Console.WriteLine("SQL Update Statement:");
        Console.WriteLine("UPDATE users SET password_hash = '" + hash + "' WHERE username = 'admin';");

        Console.WriteLine("\nPress Enter to exit...");
        Console.ReadLine();
    }
}
