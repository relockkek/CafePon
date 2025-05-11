using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CafeAutomation.Models;
using MySqlConnector;

namespace CafeAutomation.DB
{
    internal class DishesDB
    {
        private DbConnection connection;

        private DishesDB(DbConnection db)
        {
            this.connection = db;
        }

        public bool Insert(Dishes dish)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "INSERT INTO Dishes (Name, Price, Category, Description, IsAvailable) VALUES (@name, @price, @category, @desc, @available); SELECT LAST_INSERT_ID();";

            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("name", dish.Name));
                cmd.Parameters.Add(new MySqlParameter("price", dish.Price));
                cmd.Parameters.Add(new MySqlParameter("category", dish.Category));
                cmd.Parameters.Add(new MySqlParameter("desc", dish.Description));
                cmd.Parameters.Add(new MySqlParameter("available", dish.IsAvailable));

                try
                {
                    var id = cmd.ExecuteScalar();
                    if (id != null)
                    {
                        dish.ID = Convert.ToInt32(id);
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении блюда: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        public async Task<List<Dishes>> SelectAllAsync()
        {
            List<Dishes> list = new List<Dishes>();
            if (connection == null || !connection.OpenConnection())
                return list;

            string query = "SELECT ID, Name, Price, Category, Description, IsAvailable FROM Dishes";
            using (var cmd = connection.CreateCommand(query))
            {
                try
                {
                    var reader = await Task.Run(() => cmd.ExecuteReader());

                    while (reader.Read())
                    {
                        list.Add(new Dishes
                        {
                            ID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDecimal(2),
                            Category = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Description = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            IsAvailable = reader.GetBoolean(5)
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки блюд: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return list;
        }

        public async Task<bool> UpdateAsync(Dishes dish)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "UPDATE Dishes SET Name=@name, Price=@price, Category=@category, Description=@desc, IsAvailable=@available WHERE ID=@id";
            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("name", dish.Name));
                cmd.Parameters.Add(new MySqlParameter("price", dish.Price));
                cmd.Parameters.Add(new MySqlParameter("category", dish.Category));
                cmd.Parameters.Add(new MySqlParameter("desc", dish.Description));
                cmd.Parameters.Add(new MySqlParameter("available", dish.IsAvailable));
                cmd.Parameters.Add(new MySqlParameter("id", dish.ID));

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка обновления блюда: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        public async Task<bool> DeleteAsync(Dishes dish)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "DELETE FROM Dishes WHERE ID=@id";
            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("id", dish.ID));

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления блюда: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }
        public async Task<Dishes> GetMostPopularDishAsync()
        {
            Dishes mostPopular = null;
            if (connection == null || !connection.OpenConnection())
                return mostPopular;

            string query = "SELECT DishID, COUNT(*) AS Count FROM OrderItems GROUP BY DishID ORDER BY Count DESC LIMIT 1";
            using (var cmd = connection.CreateCommand(query))
            {
                try
                {
                    var reader = await Task.Run(() => cmd.ExecuteReader());

                    if (reader.Read())
                    {
                        int dishId = reader.GetInt32(0);
                        mostPopular = new Dishes { ID = dishId };
                    }

                    reader.Close();
                }
                catch { }
            }

            connection.CloseConnection();
            return mostPopular;
        }


        static DishesDB instance;
        public static DishesDB GetDb()
        {
            if (instance == null)
                instance = new DishesDB(DbConnection.GetDbConnection());
            return instance;
        }
    }
}