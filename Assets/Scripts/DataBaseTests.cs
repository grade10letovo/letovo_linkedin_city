using UnityEngine;
using Npgsql;
using System.Data;
using System.Threading.Tasks;

public class PostgresTest : MonoBehaviour
{
    public string connectionString = "Host=localhost;Username=postgres;Password=123;Database=test_db";

    async void Start()
    {
        Debug.Log("Пытаюсь подключиться к PostgreSQL...");
        
        try
        {
            await using (var conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();
                Debug.Log("Подключено к PostgreSQL!");

                // Чтение данных
                await using (var cmd = new NpgsqlCommand("SELECT * FROM users", conn))
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Debug.Log($"ID: {reader.GetInt32(0)}, Name: {reader.GetString(1)}, Score: {reader.GetInt32(2)}");
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Ошибка: {ex.Message}");
        }
    }
}