using MySql.Data.MySqlClient;
using SpaApp.Models;
using System;
using System.Collections.Generic;

namespace SpaApp.Database
{
    public class MySqlUserRepository
    {
        private string connectionString;

        public MySqlUserRepository()
        {
            connectionString = "Server=localhost;Database=spa_app;Uid=root;Pwd=your_password;Port=3306;";
        }

        public User Authenticate(string username, string password)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM users WHERE username = @username AND password = @password";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32("id"),
                                Username = reader.GetString("username"),
                                Password = reader.GetString("password"),
                                Email = reader.GetString("email"),
                                FullName = reader.GetString("full_name"),
                                Role = reader.GetString("role")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public User Register(string username, string password, string email, string fullName)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Проверяем, существует ли пользователь
                string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
                using (var checkCommand = new MySqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@username", username);
                    long count = (long)checkCommand.ExecuteScalar();
                    if (count > 0)
                    {
                        return null; // Пользователь уже существует
                    }
                }

                // Создаем нового пользователя
                string insertQuery = @"INSERT INTO users (username, password, email, full_name, role) 
                                     VALUES (@username, @password, @email, @fullName, 'User');
                                     SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@fullName", fullName);

                    int newId = Convert.ToInt32(command.ExecuteScalar());

                    return new User
                    {
                        Id = newId,
                        Username = username,
                        Password = password,
                        Email = email,
                        FullName = fullName,
                        Role = "User"
                    };
                }
            }
        }
    }
}