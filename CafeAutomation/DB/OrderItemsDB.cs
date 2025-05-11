using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CafeAutomation.Models;
using MySqlConnector;

namespace CafeAutomation.DB
{
    internal class OrderItemsDB
    {
        private DbConnection connection;

        private OrderItemsDB(DbConnection db)
        {
            this.connection = db;
        }

        public bool Insert(OrderItems item)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "INSERT INTO OrderItems (OrderID, DishID, Amount, PriceAtOrderTime) VALUES (@orderId, @dishId, @amount, @price); SELECT LAST_INSERT_ID();";

            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("orderId", item.OrderID));
                cmd.Parameters.Add(new MySqlParameter("dishId", item.DishID));
                cmd.Parameters.Add(new MySqlParameter("amount", item.Amount));
                cmd.Parameters.Add(new MySqlParameter("price", item.PriceAtOrderTime));

                try
                {
                    var id = cmd.ExecuteScalar();
                    if (id != null)
                    {
                        item.ID = Convert.ToInt32(id);
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении элемента заказа: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        public async Task<List<OrderItems>> SelectAllAsync()
        {
            List<OrderItems> list = new List<OrderItems>();
            if (connection == null || !connection.OpenConnection())
                return list;

            string query = "SELECT ID, OrderID, DishID, Amount, PriceAtOrderTime FROM OrderItems";
            using (var cmd = connection.CreateCommand(query))
            {
                try
                {
                    var reader = await Task.Run(() => cmd.ExecuteReader());

                    while (reader.Read())
                    {
                        list.Add(new OrderItems
                        {
                            ID = reader.GetInt32(0),
                            OrderID = reader.GetInt32(1),
                            DishID = reader.GetInt32(2),
                            Amount = reader.GetInt32(3),
                            PriceAtOrderTime = reader.GetDecimal(4)
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки элементов заказа: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return list;
        }

        public async Task<bool> UpdateAsync(OrderItems item)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "UPDATE OrderItems SET OrderID=@orderId, DishID=@dishId, Amount=@amount, PriceAtOrderTime=@price WHERE ID=@id";
            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("orderId", item.OrderID));
                cmd.Parameters.Add(new MySqlParameter("dishId", item.DishID));
                cmd.Parameters.Add(new MySqlParameter("amount", item.Amount));
                cmd.Parameters.Add(new MySqlParameter("price", item.PriceAtOrderTime));
                cmd.Parameters.Add(new MySqlParameter("id", item.ID));

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка обновления элемента заказа: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        public async Task<bool> DeleteAsync(OrderItems item)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "DELETE FROM OrderItems WHERE ID=@id";
            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("id", item.ID));

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления элемента заказа: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        static OrderItemsDB instance;
        public static OrderItemsDB GetDb()
        {
            if (instance == null)
                instance = new OrderItemsDB(DbConnection.GetDbConnection());
            return instance;
        }
    }
}