using UnityEngine;
using MySqlConnector;
using System;
using System.Threading.Tasks;

public class MySQLManagement : MonoBehaviour
{
    public static MySQLManagement Instance;
    private MySqlConnection connection;
    
    private string connectionString = "Server=10.100.101.80;Port=3306;Database=pontos;UserID=mqtt2db;Pwd=BakkenTeigen er PizzaFredag;SslMode=None;Pooling=true;";

    async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            await OpenConnectionAsync();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public async Task OpenConnectionAsync()
    {
        try
        {
            connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            Debug.Log("MySQL/MariaDB connection opened and persistent.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to open MySQL connection: " + ex.ToString());
        }
    }
    
    public void CloseConnection()
    {
        if (connection != null && connection.State == System.Data.ConnectionState.Open)
        {
            connection.Close();
            Debug.Log("MySQL connection closed.");
        }
    }

    public MySqlConnection GetConnection()
    {
        return connection;
    }

    public async Task<string> GetSensorValueAsync(string name)
    {
        if (connection == null || connection.State != System.Data.ConnectionState.Open)
        {
            Debug.LogError("Connection is not open.");
            return "N/A";
        }
        try
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Value FROM Readings WHERE EquipmentID = @name LIMIT 1";
            cmd.Parameters.AddWithValue("@name", name ?? string.Empty);
            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value) return "N/A";
            return result.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error executing query: " + ex.ToString());
            return "N/A";
        }
    }

public async Task<string> GetSensorTypeAsync(string name)
    {
        if (connection == null || connection.State != System.Data.ConnectionState.Open)
        {
            Debug.LogError("Connection is not open.");
            return "N/A";
        }
        try
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT EquipmentType FROM Equipment WHERE EquipmentID = @name LIMIT 1";
            cmd.Parameters.AddWithValue("@name", name ?? string.Empty);
            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value) return "N/A";
            return result.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error executing query: " + ex.ToString());
            return "N/A";
        }
    }

    void OnApplicationQuit()
    {
        CloseConnection();
    }
}