using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using SpaApp.Models;

namespace SpaApp.Repositories
{
    public class SubscriptionRepository
    {
        public List<Subscription> GetAllSubscriptions()
        {
            var subscriptions = new List<Subscription>();

            try
            {
                using (var connection = Database.DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM subscriptions WHERE is_active = 1";

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            subscriptions.Add(new Subscription
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                TypeName = reader.GetString("type_name"),
                                Rating = reader.GetDouble("rating"),
                                FacilityName = reader.GetString("facility_name"),
                                Description = reader.GetString("description"),
                                Features = reader.GetString("features"),
                                Address = reader.GetString("address"),
                                Phone = reader.GetString("phone"),
                                Price = reader.GetDecimal("price"),
                                DurationDays = reader.GetInt32("duration_days"),
                                VisitsCount = reader.GetInt32("visits_count")
                            });
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при загрузке абонементов: {ex.Message}", "Ошибка БД",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            return subscriptions;
        }

        public List<SubscriptionType> GetSubscriptionTypes()
        {
            var types = new List<SubscriptionType>();

            try
            {
                using (var connection = Database.DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM subscription_types";

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            types.Add(new SubscriptionType
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                Description = reader.GetString("description"),
                                Color = reader.GetString("color")
                            });
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при загрузке типов: {ex.Message}", "Ошибка БД",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            return types;
        }
    }
}