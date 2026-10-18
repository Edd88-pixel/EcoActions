using System.Security.Cryptography;
using System.Text.Json;
using EcoActions.Core;

namespace EcoActions.Services;

public sealed class SecureVault
{
    private readonly string _path = Path.Combine(FileSystem.AppDataDirectory, "ecoactions.vault");
    private byte[]? _key;

    public async Task<AppData> OpenAsync()
    {
        var key = await SecureStorage.Default.GetAsync("ecoactions-vault-key");
        if (key is null)
        {
            if (File.Exists(_path)) throw new CryptographicException("La clé des données locales est indisponible.");
            key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            await SecureStorage.Default.SetAsync("ecoactions-vault-key", key);
        }
        _key = Convert.FromBase64String(key);
        if (!File.Exists(_path)) return new AppData();
        var bytes = await File.ReadAllBytesAsync(_path);
        if (bytes.Length < 28) throw new InvalidDataException("Le fichier de données est incomplet.");
        var plain = new byte[bytes.Length - 28];
        using var aes = new AesGcm(_key, 16);
        aes.Decrypt(bytes.AsSpan(0, 12), bytes.AsSpan(28), bytes.AsSpan(12, 16), plain);
        var data = JsonSerializer.Deserialize(plain, AppJsonContext.Default.AppData)
            ?? throw new InvalidDataException("Le fichier de données est invalide.");
        if (data.Version != 1) throw new InvalidDataException("Cette version des données n’est pas prise en charge.");
        return data;
    }

    public async Task SaveAsync(AppData data)
    {
        var key = _key ?? throw new InvalidOperationException("Le stockage n’est pas ouvert.");
        var plain = JsonSerializer.SerializeToUtf8Bytes(data, AppJsonContext.Default.AppData);
        var bytes = new byte[plain.Length + 28];
        RandomNumberGenerator.Fill(bytes.AsSpan(0, 12));
        using var aes = new AesGcm(key, 16);
        aes.Encrypt(bytes.AsSpan(0, 12), plain, bytes.AsSpan(28), bytes.AsSpan(12, 16));
        var temporary = _path + ".new";
        await File.WriteAllBytesAsync(temporary, bytes);
        File.Move(temporary, _path, true);
        CryptographicOperations.ZeroMemory(plain);
    }
}
