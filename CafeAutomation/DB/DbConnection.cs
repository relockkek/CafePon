using System;
using System.Data;
using MySqlConnector; // Используем MySqlConnector вместо MySql.Data.MySqlClient
using System.Windows;

namespace CafeAutomation.DB
{
    internal class DbConnection
    {
        private MySqlConnection _connection;

        public void Config()
        {
            var sb = new MySqlConnectionStringBuilder
            {
                Server = "95.154.107.102",
                UserID = "student",
                Password = "student",
                Database = "CafeAutomation",
                CharacterSet = "utf8mb4"
            };

            _connection = new MySqlConnection(sb.ConnectionString);
        }

        public bool OpenConnection()
        {
            if (_connection == null)
            {
                Config();
            }

            try
            {
                // Проверяем, открыто ли соединение
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
                return true;
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Ошибка подключения к базе данных: " + e.Message);
                return false;
            }
        }

        public void CloseConnection()
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                try
                {
                    _connection.Close();
                }
                catch (MySqlException e)
                {
                    MessageBox.Show("Ошибка закрытия соединения: " + e.Message);
                }
            }
        }

        public MySqlCommand CreateCommand(string sql)
        {
            return new MySqlCommand(sql, _connection);
        }

        static DbConnection dbConnection;
        private DbConnection() { }

        public static DbConnection GetDbConnection()
        {
            if (dbConnection == null)
                dbConnection = new DbConnection();

            return dbConnection;
        }
    }
}