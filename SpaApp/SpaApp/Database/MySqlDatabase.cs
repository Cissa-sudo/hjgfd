using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SpaApp.Database
{
    public class MySqlDatabase : IDisposable
    {
        private MySqlConnection connection;
        private string connectionString;

        public MySqlDatabase()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            connection = new MySqlConnection(connectionString);
        }

        public void OpenConnection()
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (connection.State != System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        public MySqlConnection GetConnection()
        {
            return connection;
        }

        public void Dispose()
        {
            connection?.Dispose();
        }
    }
}