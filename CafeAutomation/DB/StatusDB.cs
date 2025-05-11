using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CafeAutomation.Models;
using MySqlConnector;

namespace CafeAutomation.DB
{
    internal class StatusDB
    {
        private DbConnection connection;

        private StatusDB(DbConnection db)
        {
            this.connection = db;
        }

        public bool Insert(Status status)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "INSERT INTO Status (Title) VALUES (@title); SELECT LAST_INSERT_ID();";

            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("title", status.Title));

                try
                {
                    var id = cmd.ExecuteScalar();
                    if (id != null)
                    {
                        status.ID = Convert.ToInt32(id);
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении статуса: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        public async Task<List<Status>> SelectAllAsync()
        {
            List<Status> list = new List<Status>();
            if (connection == null || !connection.OpenConnection())
                return list;

            string query = "SELECT ID, Title FROM Status";
            using (var cmd = connection.CreateCommand(query))
            {
                try
                {
                    var reader = await Task.Run(() => cmd.ExecuteReader());

                    while (reader.Read())
                    {
                        list.Add(new Status
                        {
                            ID = reader.GetInt32(0),
                            Title = reader.GetString(1)
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки статусов: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return list;
        }

        public async Task<bool> UpdateAsync(Status status)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "UPDATE Status SET Title=@title WHERE ID=@id";
            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("title", status.Title));
                cmd.Parameters.Add(new MySqlParameter("id", status.ID));

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка обновления статуса: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        public async Task<bool> DeleteAsync(Status status)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "DELETE FROM Status WHERE ID=@id";
            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("id", status.ID));

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления статуса: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        static StatusDB instance;
        public static StatusDB GetDb()
        {
            if (instance == null)
                instance = new StatusDB(DbConnection.GetDbConnection());
            return instance;
        }
    }
}