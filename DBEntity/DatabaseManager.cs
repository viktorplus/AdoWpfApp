using Microsoft.Data.SqlClient;

namespace DBEntity
{
    public class DatabaseManager
    {

        // Строка подключения к базе данных
        public string ConnectionString { get; set; }

        // Объект для работы с соединением с базой данных
        private SqlConnection sqlConnection { get; set; }

        // Конструктор класса, принимающий строку подключения и инициализирующий объект SqlConnection
        public DatabaseManager(string connectionString)
        {
            ConnectionString = connectionString;
            sqlConnection = new SqlConnection(ConnectionString);
        }

        // Метод для открытия соединения с базой данных (асинхронная версия)
        public bool OpenConnection()
        {
            try
            {
                // Открываем соединение асинхронно
                sqlConnection.OpenAsync();
                return true;
            }
            catch (SqlException sql_ex)
            {
                // Если произошла ошибка при открытии соединения, бросаем исключение с сообщением об ошибке
                throw new Exception(sql_ex.Message);
            }
        }

        // Метод для закрытия соединения с базой данных (асинхронная версия)
        public bool CloseConnection()
        {
            try
            {
                // Закрываем соединение асинхронно
                sqlConnection.CloseAsync();
                return true;
            }
            catch (SqlException sql_ex)
            {
                // Если произошла ошибка при закрытии соединения, бросаем исключение с сообщением об ошибке
                throw new Exception(sql_ex.Message);
            }
        }

        // Метод для выполнения запроса без возвращаемого результата (асинхронная версия)
        public void ExecuteNonQuery(string query)
        {
            // Если соединение не открыто, открываем его
            if (sqlConnection.State != System.Data.ConnectionState.Open)
            {
                OpenConnection();
            }

            // Создаем SqlCommand для выполнения запроса
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            // Выполняем запрос
            sqlCommand.ExecuteNonQuery();

            // Закрываем соединение после выполнения запроса
            CloseConnection();
        }

        public async Task<List<Student>> ExecuteReader(string query)
        {
            if (sqlConnection.State != System.Data.ConnectionState.Open)
            {
                OpenConnection();
            }

            List<Student> result = new List<Student>();

            using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
            using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    result.Add(new Student
                    {
                        Surname = reader["Surname"].ToString(),
                        Name = reader["Name"].ToString(),
                        Vatername = reader["Vatername"].ToString(),
                        GroupName = reader["GroupName"].ToString(),
                        Gradeavr = Convert.ToDecimal(reader["Gradeavr"]),
                        Subjectmin = reader["Subjectmin"].ToString(),
                        Subjectmax = reader["Subjectmax"].ToString()
                    });
                }
            }

            CloseConnection();
            return result;
        }


        // Метод для выполнения запроса с возвращаемым результатом (асинхронная версия)
        //public async Task<List<MainWindow.Product>> ExecuteReaderProducts()
        //{
        //    if (sqlConnection.State != System.Data.ConnectionState.Open)
        //    {
        //        OpenConnection();
        //    }
        //    string query = "SELECT ID, ProductName, Quantity FROM [Products]";
        //    List<MainWindow.Product> result = new List<MainWindow.Product>();

        //    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
        //    using (SqlDataReader reader = sqlCommand.ExecuteReader())
        //    {
        //        while (reader.Read())
        //        {
        //            result.Add(new MainWindow.Product(
        //                (int)reader[0],
        //                reader[1].ToString(),
        //                (int)reader[2]));
        //        }
        //    }
        //    CloseConnection();
        //    return result;
        //}
    }

}
