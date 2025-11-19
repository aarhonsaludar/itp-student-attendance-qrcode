using System;

// TEST: Verify BCrypt is working with the database hash
// This is a standalone test - not part of the main project
class TestBCrypt
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== BCrypt Password Testing ===\n");

        // Test 1: Generate new hash
        string password = "admin123";
        string newHash = BCrypt.Net.BCrypt.HashPassword(password);
        Console.WriteLine("Test 1: Generate new hash for 'admin123'");
        Console.WriteLine("New Hash: " + newHash);
        Console.WriteLine("Verify: " + (BCrypt.Net.BCrypt.Verify(password, newHash) ? "SUCCESS" : "FAILED"));
        Console.WriteLine();

        // Test 2: Verify existing database hash
        string dbHash = "$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy";
        Console.WriteLine("Test 2: Verify database hash");
        Console.WriteLine("DB Hash: " + dbHash);
        Console.WriteLine("Password: " + password);
        Console.WriteLine("Verify: " + (BCrypt.Net.BCrypt.Verify(password, dbHash) ? "SUCCESS" : "FAILED"));
        Console.WriteLine();

        // Test 3: Try different passwords
        Console.WriteLine("Test 3: Try wrong passwords");
        Console.WriteLine("Verify 'Admin123': " + (BCrypt.Net.BCrypt.Verify("Admin123", dbHash) ? "SUCCESS" : "FAILED"));
        Console.WriteLine("Verify 'admin': " + (BCrypt.Net.BCrypt.Verify("admin", dbHash) ? "SUCCESS" : "FAILED"));
        Console.WriteLine("Verify 'admin123 ': " + (BCrypt.Net.BCrypt.Verify("admin123 ", dbHash) ? "SUCCESS" : "FAILED"));

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}