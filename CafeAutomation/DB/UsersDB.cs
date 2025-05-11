using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CafeAutomation.Models;
using MySqlConnector;

namespace CafeAutomation.DB
{
    internal class UsersDB
    {
        private DbConnection connection;

        private UsersDB(DbConnection db)
        {
            this.connection = db;
        }

        public bool Insert(Users user)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "INSERT INTO Users (EmployeeID, Username, Password, Role) VALUES (@empId, @username, @password, @role); SELECT LAST_INSERT_ID();";

            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("empId", user.EmployeeID));
                cmd.Parameters.Add(new MySqlParameter("username", user.Username));
                cmd.Parameters.Add(new MySqlParameter("password", user.Password));
                cmd.Parameters.Add(new MySqlParameter("role", user.Role));

                try
                {
                    var id = cmd.ExecuteScalar();
                    if (id != null)
                    {
                        user.ID = Convert.ToInt32(id);
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении пользователя: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        public async Task<List<Users>> SelectAllAsync()
        {
            List<Users> list = new List<Users>();
            if (connection == null || !connection.OpenConnection())
                return list;

            string query = "SELECT ID, EmployeeID, Username, Password, Role FROM Users";
            using (var cmd = connection.CreateCommand(query))
            {
                try
                {
                    var reader = await Task.Run(() => cmd.ExecuteReader());

                    while (reader.Read())
                    {
                        list.Add(new Users
                        {
                            ID = reader.GetInt32(0),
                            EmployeeID = reader.GetInt32(1),
                            Username = reader.GetString(2),
                            Password = reader.GetString(3),
                            Role = reader.GetString(4)
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки пользователей: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return list;
        }

        public async Task<bool> UpdateAsync(Users user)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "UPDATE Users SET EmployeeID=@empId, Username=@user, Password=@pass, Role=@role WHERE ID=@id";
            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("empId", user.EmployeeID));
                cmd.Parameters.Add(new MySqlParameter("user", user.Username));
                cmd.Parameters.Add(new MySqlParameter("pass", user.Password));
                cmd.Parameters.Add(new MySqlParameter("role", user.Role));
                cmd.Parameters.Add(new MySqlParameter("id", user.ID));

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка обновления пользователя: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        public async Task<bool> DeleteAsync(Users user)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "DELETE FROM Users WHERE ID=@id";
            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("id", user.ID));

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления пользователя: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }
        public Users FindByUsernameAndPassword(string username, string password)
        {
            Users user = null;
            if (connection == null || !connection.OpenConnection())
                return user;

            string query = "SELECT ID, EmployeeID, Username, Password, Role FROM Users WHERE Username=@username AND Password=@password";
            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("username", username));
                cmd.Parameters.Add(new MySqlParameter("password", password));

                try
                {
                    var reader = Task.Run(() => cmd.ExecuteReader()).Result;

                    if (reader.Read())
                    {
                        user = new Users
                        {
                            ID = reader.GetInt32(0),
                            EmployeeID = reader.GetInt32(1),
                            Username = reader.GetString(2),
                            Password = reader.GetString(3),
                            Role = reader.GetString(4)
                        };
                    }

                    reader.Close();
                }
                catch { }
            }

            connection.CloseConnection();
            return user;
        }
        public bool CheckAdminExists()
        {
            if (connection == null || !connection.OpenConnection())
                return false;

            string query = "SELECT COUNT(*) FROM Users WHERE Username = 'admin'";
            using (var cmd = connection.CreateCommand(query))
            {
                try
                {
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
                catch { }
            }

            connection.CloseConnection();
            return false;
        }
        static UsersDB instance;
        public static UsersDB GetDb()
        {
            if (instance == null)
                instance = new UsersDB(DbConnection.GetDbConnection());
            return instance;
        }
    }
}