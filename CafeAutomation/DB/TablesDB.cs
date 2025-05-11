using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CafeAutomation.Models;
using MySqlConnector;

namespace CafeAutomation.DB
{
    internal class TablesDB
    {
        private DbConnection connection;

        private TablesDB(DbConnection db)
        {
            this.connection = db;
        }

        public bool Insert(Tables table)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "INSERT INTO Tables (TableNumber, Capacity, Zone, IsActive) VALUES (@number, @capacity, @zone, @active); SELECT LAST_INSERT_ID();";

            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("number", table.TableNumber));
                cmd.Parameters.Add(new MySqlParameter("capacity", table.Capacity));
                cmd.Parameters.Add(new MySqlParameter("zone", table.Zone));
                cmd.Parameters.Add(new MySqlParameter("active", table.IsActive));

                try
                {
                    var id = cmd.ExecuteScalar();
                    if (id != null)
                    {
                        table.ID = Convert.ToInt32(id);
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении стола: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        public async Task<List<Tables>> SelectAllAsync()
        {
            List<Tables> list = new List<Tables>();
            if (connection == null || !connection.OpenConnection())
                return list;

            string query = "SELECT ID, TableNumber, Capacity, Zone, IsActive FROM Tables";
            using (var cmd = connection.CreateCommand(query))
            {
                try
                {
                    var reader = await Task.Run(() => cmd.ExecuteReader());

                    while (reader.Read())
                    {
                        list.Add(new Tables
                        {
                            ID = reader.GetInt32(0),
                            TableNumber = reader.GetInt32(1),
                            Capacity = reader.GetInt32(2),
                            Zone = reader.GetString(3),
                            IsActive = reader.GetBoolean(4)
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки столов: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return list;
        }

        public async Task<bool> UpdateAsync(Tables table)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "UPDATE Tables SET TableNumber=@number, Capacity=@capacity, Zone=@zone, IsActive=@active WHERE ID=@id";
            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("number", table.TableNumber));
                cmd.Parameters.Add(new MySqlParameter("capacity", table.Capacity));
                cmd.Parameters.Add(new MySqlParameter("zone", table.Zone));
                cmd.Parameters.Add(new MySqlParameter("active", table.IsActive));
                cmd.Parameters.Add(new MySqlParameter("id", table.ID));

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка обновления стола: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        public async Task<bool> DeleteAsync(Tables table)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "DELETE FROM Tables WHERE ID=@id";
            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("id", table.ID));

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления стола: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        static TablesDB instance;
        public static TablesDB GetDb()
        {
            if (instance == null)
                instance = new TablesDB(DbConnection.GetDbConnection());
            return instance;
        }
    }
}