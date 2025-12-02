using System;
using MySql.Data.MySqlClient;
using System.Data;

namespace SpaApp.Database
{
    public class DatabaseHelper
    {
        private static string _connectionString = "Server=localhost;Database=spa_app;Uid=root;Pwd=;";

        public static string ConnectionString
        {
            get => _connectionString;
            set => _connectionString = value;
        }

        public static bool TestConnection()
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    return connection.State == ConnectionState.Open;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения: {ex.Message}");
                return false;
            }
        }

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}