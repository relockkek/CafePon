using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CafeAutomation.Models;
using MySqlConnector;

namespace CafeAutomation.DB
{
    internal class EmployeesDB
    {
        private DbConnection connection;

        private EmployeesDB(DbConnection db)
        {
            this.connection = db;
        }

        public bool Insert(Employees employee)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "INSERT INTO Employees (FirstName, LastName, Patronymic, Position, Phone, HireDate, Salary) VALUES (@fname, @lname, @patr, @pos, @phone, @hire, @salary); SELECT LAST_INSERT_ID();";

            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("fname", employee.FirstName));
                cmd.Parameters.Add(new MySqlParameter("lname", employee.LastName));
                cmd.Parameters.Add(new MySqlParameter("patr", employee.Patronymic));
                cmd.Parameters.Add(new MySqlParameter("pos", employee.Position));
                cmd.Parameters.Add(new MySqlParameter("phone", employee.Phone));
                cmd.Parameters.Add(new MySqlParameter("hire", employee.HireDate));
                cmd.Parameters.Add(new MySqlParameter("salary", employee.Salary));

                try
                {
                    var id = cmd.ExecuteScalar();
                    if (id != null)
                    {
                        employee.ID = Convert.ToInt32(id);
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении сотрудника: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        public async Task<List<Employees>> SelectAllAsync()
        {
            List<Employees> list = new List<Employees>();
            if (connection == null || !connection.OpenConnection())
                return list;

            string query = "SELECT ID, FirstName, LastName, Patronymic, Position, Phone, HireDate, Salary FROM Employees";
            using (var cmd = connection.CreateCommand(query))
            {
                try
                {
                    var reader = await Task.Run(() => cmd.ExecuteReader());

                    while (reader.Read())
                    {
                        list.Add(new Employees
                        {
                            ID = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Patronymic = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Position = reader.GetString(4),
                            Phone = reader.GetString(5),
                            HireDate = reader.GetDateTime(6),
                            Salary = reader.GetDecimal(7)
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки сотрудников: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return list;
        }

        public async Task<bool> UpdateAsync(Employees employee)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "UPDATE Employees SET FirstName=@fname, LastName=@lname, Patronymic=@patr, Position=@pos, Phone=@phone, HireDate=@hire, Salary=@salary WHERE ID=@id";
            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("fname", employee.FirstName));
                cmd.Parameters.Add(new MySqlParameter("lname", employee.LastName));
                cmd.Parameters.Add(new MySqlParameter("patr", employee.Patronymic));
                cmd.Parameters.Add(new MySqlParameter("pos", employee.Position));
                cmd.Parameters.Add(new MySqlParameter("phone", employee.Phone));
                cmd.Parameters.Add(new MySqlParameter("hire", employee.HireDate));
                cmd.Parameters.Add(new MySqlParameter("salary", employee.Salary));
                cmd.Parameters.Add(new MySqlParameter("id", employee.ID));

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка обновления сотрудника: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        public async Task<bool> DeleteAsync(Employees employee)
        {
            bool result = false;
            if (connection == null || !connection.OpenConnection())
                return result;

            string query = "DELETE FROM Employees WHERE ID=@id";
            using (var cmd = connection.CreateCommand(query))
            {
                cmd.Parameters.Add(new MySqlParameter("id", employee.ID));

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления сотрудника: " + ex.Message);
                }
            }

            connection.CloseConnection();
            return result;
        }

        static EmployeesDB instance;
        public static EmployeesDB GetDb()
        {
            if (instance == null)
                instance = new EmployeesDB(DbConnection.GetDbConnection());
            return instance;
        }
    }
}