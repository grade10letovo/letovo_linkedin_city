using System;
using Npgsql;
using UnityEngine;

public class TestNpgsql : MonoBehaviour
{
    void Start()
    {
        try
        {
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=code1234;Database=city"))
            {
                conn.Open();
                Debug.Log("✅ Npgsql успешно работает и подключился к PostgreSQL!");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Ошибка подключения к PostgreSQL: {ex.Message}");
        }
    }
}
