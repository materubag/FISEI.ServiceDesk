using System.Security.Cryptography;
using System.Text;

namespace FISEI.ServiceDesk.Infrastructure.Security;

public static class PasswordHasher
{
    public static (string hash, string salt) HashPassword(string password, int iterations = 150000)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(16);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256);
        var hashBytes = pbkdf2.GetBytes(32);
        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    public static bool Verify(string password, string storedHash, string storedSalt, int iterations = 150000)
    {
        var saltBytes = Convert.FromBase64String(storedSalt);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256);
        var hashBytes = pbkdf2.GetBytes(32);
        var calcHash = Convert.ToBase64String(hashBytes);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(calcHash),
            Encoding.UTF8.GetBytes(storedHash));
    }
}