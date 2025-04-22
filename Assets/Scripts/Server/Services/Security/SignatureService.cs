// Сервис для проверки подписей
using System.Threading.Tasks;
using System;

public class SignatureService : ISignatureService
{
    /// <summary>
    /// Проверяет подпись данных
    /// </summary>
    public async Task<bool> ValidateWorldSignature(string worldId, string platform, string signature, string payload)
    {
        // Проверка подписи
        var expectedSignature = GenerateSignature(worldId, platform, payload);
        return signature == expectedSignature;
    }

    private string GenerateSignature(string worldId, string platform, string payload)
    {
        string secretKey = Environment.GetEnvironmentVariable("HMAC_SECRET");
        // Логика генерации подписи, например, с использованием HMAC
        using (var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secretKey)))
        {
            var data = worldId + platform + payload;
            var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }
    }
}
