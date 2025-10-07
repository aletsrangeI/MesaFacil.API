using System.Security.Cryptography;
using Interface.UseCases;

namespace Persistence.Security;

/// <summary>Verifica hashes PBKDF2 (Rfc2898) almacenados como Base64; salt opcional en Base64.</summary>
public class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int Iterations = 100_000;
    private const int KeySize = 32;
    private static readonly HashAlgorithmName Algo = HashAlgorithmName.SHA256;

    public bool Verify(string plaintext, string hashBase64, string? saltBase64)
    {
        if (string.IsNullOrEmpty(hashBase64)) return false;

        byte[] stored = Convert.FromBase64String(hashBase64);
        byte[] salt = string.IsNullOrEmpty(saltBase64) ? Array.Empty<byte>() : Convert.FromBase64String(saltBase64);

        using var pbkdf2 = new Rfc2898DeriveBytes(plaintext, salt, Iterations, Algo);
        var computed = pbkdf2.GetBytes(KeySize);

        // Tiempo constante
        return CryptographicOperations.FixedTimeEquals(stored, computed);
    }
}