using Npgsql;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using System;
// Реализация источника данных для PostgreSQL
public class PostgresDataReader
{
    private readonly string connectionString;

    public PostgresDataReader(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public List<T> ExecuteQuery<T>(string query, Func<NpgsqlDataReader, T> mapFunction)
    {
        List<T> results = new List<T>();
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand(query, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    results.Add(mapFunction(reader));
                }
            }
        }
        return results;
    }
}