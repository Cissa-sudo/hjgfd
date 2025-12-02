using System;
using MySql.Data.MySqlClient;
using SpaApp.Models;
using System.Windows;

namespace SpaApp.Repositories
{
    public class UserRepository
    {
        public bool ValidateUser(string username, string password)
        {
            try
            {
                using (var connection = Database.DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password AND is_active = 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);

                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке пользователя: {ex.Message}", "Ошибка БД",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Метод Register (синоним для CreateUser)
        public bool Register(User user)
        {
            return CreateUser(user);
        }

        public bool CreateUser(User user)
        {
            try
            {
                using (var connection = Database.DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = @"INSERT INTO users (username, password, full_name, email, role, is_active, created_at) 
                                   VALUES (@username, @password, @fullName, @email, @role, @isActive, @createdAt)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", user.Username);
                        command.Parameters.AddWithValue("@password", user.Password);
                        command.Parameters.AddWithValue("@fullName", user.FullName);
                        command.Parameters.AddWithValue("@email", user.Email);
                        command.Parameters.AddWithValue("@role", user.Role);
                        command.Parameters.AddWithValue("@isActive", user.IsActive);
                        command.Parameters.AddWithValue("@createdAt", DateTime.Now);

                        int result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании пользователя: {ex.Message}", "Ошибка БД",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool UserExists(string username)
        {
            try
            {
                using (var connection = Database.DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE username = @username AND is_active = 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке пользователя: {ex.Message}", "Ошибка БД",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Метод для получения пользователя по имени
        public User GetUserByUsername(string username)
        {
            try
            {
                using (var connection = Database.DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT id, username, full_name, email, role, is_active, created_at FROM users WHERE username = @username AND is_active = 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    Id = reader.GetInt32("id"),
                                    Username = reader.GetString("username"),
                                    FullName = reader.GetString("full_name"),
                                    Email = reader.GetString("email"),
                                    Role = reader.GetString("role"),
                                    IsActive = reader.GetBoolean("is_active"),
                                    CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ?
                                               DateTime.Now : reader.GetDateTime("created_at")
                                    // updated_at не запрашиваем, так как его может не быть в базе
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении пользователя: {ex.Message}", "Ошибка БД",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return null;
        }

        public bool EmailExists(string email)
        {
            try
            {
                using (var connection = Database.DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE email = @email AND is_active = 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке email: {ex.Message}", "Ошибка БД",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Метод для обновления данных пользователя
        public bool UpdateUser(User user)
        {
            try
            {
                using (var connection = Database.DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    // Проверяем, есть ли столбец updated_at в таблице
                    string checkColumnQuery = "SHOW COLUMNS FROM users LIKE 'updated_at'";
                    bool hasUpdatedAtColumn = false;

                    using (var checkCommand = new MySqlCommand(checkColumnQuery, connection))
                    using (var reader = checkCommand.ExecuteReader())
                    {
                        hasUpdatedAtColumn = reader.HasRows;
                        reader.Close();
                    }

                    string query;
                    if (hasUpdatedAtColumn)
                    {
                        query = @"UPDATE users 
                                SET username = @username, 
                                    full_name = @fullName, 
                                    email = @email,
                                    updated_at = @updatedAt
                                WHERE id = @id AND is_active = 1";
                    }
                    else
                    {
                        query = @"UPDATE users 
                                SET username = @username, 
                                    full_name = @fullName, 
                                    email = @email
                                WHERE id = @id AND is_active = 1";
                    }

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", user.Username);
                        command.Parameters.AddWithValue("@fullName", user.FullName);
                        command.Parameters.AddWithValue("@email", user.Email);
                        command.Parameters.AddWithValue("@id", user.Id);

                        if (hasUpdatedAtColumn)
                        {
                            command.Parameters.AddWithValue("@updatedAt", DateTime.Now);
                        }

                        int result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении пользователя: {ex.Message}", "Ошибка БД",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Метод для удаления пользователя (мягкое удаление)
        public bool DeleteUser(int userId)
        {
            try
            {
                using (var connection = Database.DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    // Проверяем, есть ли столбец updated_at в таблице
                    string checkColumnQuery = "SHOW COLUMNS FROM users LIKE 'updated_at'";
                    bool hasUpdatedAtColumn = false;

                    using (var checkCommand = new MySqlCommand(checkColumnQuery, connection))
                    using (var reader = checkCommand.ExecuteReader())
                    {
                        hasUpdatedAtColumn = reader.HasRows;
                        reader.Close();
                    }

                    string query;
                    if (hasUpdatedAtColumn)
                    {
                        query = @"UPDATE users 
                                SET is_active = 0, 
                                    updated_at = @updatedAt
                                WHERE id = @id";
                    }
                    else
                    {
                        query = @"UPDATE users 
                                SET is_active = 0
                                WHERE id = @id";
                    }

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", userId);

                        if (hasUpdatedAtColumn)
                        {
                            command.Parameters.AddWithValue("@updatedAt", DateTime.Now);
                        }

                        int result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}", "Ошибка БД",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Метод для получения пользователя по ID
        public User GetUserById(int userId)
        {
            try
            {
                using (var connection = Database.DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT id, username, full_name, email, role, is_active, created_at FROM users WHERE id = @id AND is_active = 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", userId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    Id = reader.GetInt32("id"),
                                    Username = reader.GetString("username"),
                                    FullName = reader.GetString("full_name"),
                                    Email = reader.GetString("email"),
                                    Role = reader.GetString("role"),
                                    IsActive = reader.GetBoolean("is_active"),
                                    CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ?
                                               DateTime.Now : reader.GetDateTime("created_at")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении пользователя: {ex.Message}", "Ошибка БД",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return null;
        }

        // Метод для проверки существования username исключая текущего пользователя
        public bool UserExistsExcludingCurrent(string username, int currentUserId)
        {
            try
            {
                using (var connection = Database.DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE username = @username AND id != @currentUserId AND is_active = 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@currentUserId", currentUserId);
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке пользователя: {ex.Message}", "Ошибка БД",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Метод для проверки существования email исключая текущего пользователя
        public bool EmailExistsExcludingCurrent(string email, int currentUserId)
        {
            try
            {
                using (var connection = Database.DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE email = @email AND id != @currentUserId AND is_active = 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@currentUserId", currentUserId);
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке email: {ex.Message}", "Ошибка БД",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Метод для изменения пароля
        public bool ChangePassword(int userId, string newPassword)
        {
            try
            {
                using (var connection = Database.DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    // Проверяем, есть ли столбец updated_at в таблице
                    string checkColumnQuery = "SHOW COLUMNS FROM users LIKE 'updated_at'";
                    bool hasUpdatedAtColumn = false;

                    using (var checkCommand = new MySqlCommand(checkColumnQuery, connection))
                    using (var reader = checkCommand.ExecuteReader())
                    {
                        hasUpdatedAtColumn = reader.HasRows;
                        reader.Close();
                    }

                    string query;
                    if (hasUpdatedAtColumn)
                    {
                        query = @"UPDATE users 
                                SET password = @password,
                                    updated_at = @updatedAt
                                WHERE id = @id AND is_active = 1";
                    }
                    else
                    {
                        query = @"UPDATE users 
                                SET password = @password
                                WHERE id = @id AND is_active = 1";
                    }

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@password", newPassword);
                        command.Parameters.AddWithValue("@id", userId);

                        if (hasUpdatedAtColumn)
                        {
                            command.Parameters.AddWithValue("@updatedAt", DateTime.Now);
                        }

                        int result = command.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении пароля: {ex.Message}", "Ошибка БД",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}