namespace Interface.UseCases;

public interface IPasswordHasher
{
    bool Verify(string plaintext, string hashBase64, string? saltBase64);
}