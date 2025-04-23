using System;
using System.IO;
using UnityEngine;

public class SecureStorage : MonoBehaviour
{
    private static string filePath = Application.persistentDataPath + "/authToken.json";

    public static void SaveAuthToken(string token)
    {
        string encryptedToken = EncryptToken(token); // Применение шифрования
        File.WriteAllText(filePath, encryptedToken);
    }

    public static string LoadAuthToken()
    {
        if (File.Exists(filePath))
        {
            string encryptedToken = File.ReadAllText(filePath);
            return DecryptToken(encryptedToken); // Дешифровка
        }
        return null;
    }

    private static string EncryptToken(string token)
    {
        // Пример простого шифрования
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));
    }

    private static string DecryptToken(string encryptedToken)
    {
        // Пример дешифрования
        return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encryptedToken));
    }
}
