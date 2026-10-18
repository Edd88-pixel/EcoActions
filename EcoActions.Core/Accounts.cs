using System.Net.Mail;
using System.Security.Cryptography;

namespace EcoActions.Core;

public static class Accounts
{
    public static string NormalizeEmail(string email)
    {
        var value = email.Trim().ToLowerInvariant();
        if (!MailAddress.TryCreate(value, out var parsed) || parsed.Address != value || !parsed.Host.Contains('.'))
            throw new InputError("email", "Vérifiez cette adresse e-mail.");
        return value;
    }

    public static (Account Account, string RecoveryCode) Register(AppData data, string name,
        string email, string password, string confirmation)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 50)
            throw new InputError("name", "Indiquez un prénom de 1 à 50 caractères.");
        email = NormalizeEmail(email);
        ValidatePassword(password);
        if (password != confirmation) throw new InputError("confirmation", "Les mots de passe ne correspondent pas.");
        if (data.Accounts.Any(a => a.Email == email)) throw new InputError("email", "Un compte utilise déjà cette adresse.");
        var code = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
        var account = new Account { Name = name.Trim(), Email = email,
            Password = Hash(password), Recovery = Hash(code) };
        data.Accounts.Add(account);
        data.ActiveAccountId = account.Id;
        return (account, code);
    }

    public static Account Login(AppData data, string email, string password)
    {
        email = NormalizeEmail(email);
        var account = data.Accounts.SingleOrDefault(a => a.Email == email);
        if (account is null || !Verify(password, account.Password))
            throw new InputError("password", "L’adresse ou le mot de passe est incorrect.");
        data.ActiveAccountId = account.Id;
        return account;
    }

    public static void ResetPassword(AppData data, string email, string code, string password)
    {
        email = NormalizeEmail(email);
        ValidatePassword(password);
        var account = data.Accounts.SingleOrDefault(a => a.Email == email);
        if (account is null || !Verify(code.Trim().ToUpperInvariant(), account.Recovery))
            throw new InputError("code", "L’adresse ou le code de récupération est incorrect.");
        account.Password = Hash(password);
        data.ActiveAccountId = null;
    }

    private static void ValidatePassword(string password)
    {
        if (password.Length is < 10 or > 128)
            throw new InputError("password", "Utilisez un mot de passe de 10 à 128 caractères.");
    }

    private static PasswordSecret Hash(string value)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(value, salt, 600_000, HashAlgorithmName.SHA256, 32);
        return new(Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    private static bool Verify(string value, PasswordSecret secret)
    {
        var expected = Convert.FromBase64String(secret.Hash);
        var actual = Rfc2898DeriveBytes.Pbkdf2(value, Convert.FromBase64String(secret.Salt),
            600_000, HashAlgorithmName.SHA256, expected.Length);
        return CryptographicOperations.FixedTimeEquals(expected, actual);
    }
}
