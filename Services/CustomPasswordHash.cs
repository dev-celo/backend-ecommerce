using System;
using System.Security.Cryptography;
using System.Text;

public class CustomPasswordHasher
{
    public static string HashPassword(string password, out string salt)
    {
        // Gerar um novo salt
        salt = GenerateSalt();
        // Hash a senha usando o salt
        return HashPasswordWithSalt(password, salt);
    }

    private static string GenerateSalt(int size = 16)
    {
        var salt = new byte[size];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }
        return Convert.ToBase64String(salt);
    }

    private static string HashPasswordWithSalt(string password, string salt)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 10000))
        {
            var hash = pbkdf2.GetBytes(20);
            return Convert.ToBase64String(hash);
        }
    }

    public static bool VerifyPassword(string enteredPassword, string storedSalt, string storedHash)
    {
        // hash da senha digitada usando o salt
        var hashOfEnteredPassword = HashPasswordWithSalt(enteredPassword, storedSalt);
        // Comparar o hash da senha digitada com o hash armazenado
        return hashOfEnteredPassword == storedHash;
    }
}
