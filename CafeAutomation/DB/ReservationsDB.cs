using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CafeAutomation.Models;
using MySqlConnector;

namespace CafeAutomation.DB
{
    internal class ReservationsDB
    {
        private DbConnection connection;

        private ReservationsDB(DbConnection db)
        {
            this.connection = db;
        }

        public bool Insert(Reservations reservation)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "INSERT INTO Reservations (TableID, CustomerName, CustomerPhone, GuestsCount, ReservationDate, Status) VALUES (@tableId, @name, @phone, @guests, @date, @status); SELECT LAST_INSERT_ID();";

            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("tableId", reservation.TableID));
                cmd.Parameters.Add(new MySqlParameter("name", reservation.CustomerName));
                cmd.Parameters.Add(new MySqlParameter("phone", reservation.CustomerPhone));
                cmd.Parameters.Add(new MySqlParameter("guests", reservation.GuestsCount));
                cmd.Parameters.Add(new MySqlParameter("date", reservation.ReservationDate));
                cmd.Parameters.Add(new MySqlParameter("status", reservation.Status));

                try
                {
                    var id = cmd.ExecuteScalar();
                    if (id != null)
                    {
                        reservation.ID = Convert.ToInt32(id);
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении бронирования: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        public async Task<List<Reservations>> SelectAllAsync()
        {
            List<Reservations> list = new List<Reservations>();
            if (connection == null || !connection.OpenConnection())
                return list;

            string query = "SELECT ID, TableID, CustomerName, CustomerPhone, GuestsCount, ReservationDate, Status FROM Reservations";
            using (var cmd = connection.CreateCommand(query))
            {
                try
                {
                    var reader = await Task.Run(() => cmd.ExecuteReader());

                    while (reader.Read())
                    {
                        list.Add(new Reservations
                        {
                            ID = reader.GetInt32(0),
                            TableID = reader.GetInt32(1),
                            CustomerName = reader.GetString(2),
                            CustomerPhone = reader.GetString(3),
                            GuestsCount = reader.GetInt32(4),
                            ReservationDate = reader.GetDateTime(5),
                            Status = reader.GetString(6)
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки бронирований: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return list;
        }

        public async Task<bool> UpdateAsync(Reservations reservation)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "UPDATE Reservations SET TableID=@tableId, CustomerName=@name, CustomerPhone=@phone, GuestsCount=@guests, ReservationDate=@date, Status=@status WHERE ID=@id";
            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("tableId", reservation.TableID));
                cmd.Parameters.Add(new MySqlParameter("name", reservation.CustomerName));
                cmd.Parameters.Add(new MySqlParameter("phone", reservation.CustomerPhone));
                cmd.Parameters.Add(new MySqlParameter("guests", reservation.GuestsCount));
                cmd.Parameters.Add(new MySqlParameter("date", reservation.ReservationDate));
                cmd.Parameters.Add(new MySqlParameter("status", reservation.Status));
                cmd.Parameters.Add(new MySqlParameter("id", reservation.ID));

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка обновления бронирования: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        public async Task<bool> DeleteAsync(Reservations reservation)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "DELETE FROM Reservations WHERE ID=@id";
            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("id", reservation.ID));

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления бронирования: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        static ReservationsDB instance;
        public static ReservationsDB GetDb()
        {
            if (instance == null)
                instance = new ReservationsDB(DbConnection.GetDbConnection());
            return instance;
        }
    }
}