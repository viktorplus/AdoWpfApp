using Microsoft.Data.SqlClient;
using System.Data;

namespace DBEntity
{
    public class DatabaseManager
    {
        public string ConnectionString { get; set; }
        private SqlConnection sqlConnection { get; set; }

        public DatabaseManager(string connectionString)
        {
            ConnectionString = connectionString;
            sqlConnection = new SqlConnection(ConnectionString);
        }

        public bool OpenConnection()
        {
            try
            {
                sqlConnection.Open();
                return true;
            }
            catch (SqlException sql_ex)
            {
                throw new Exception(sql_ex.Message);
            }
        }

        public bool CloseConnection()
        {
            try
            {
                sqlConnection.Close();
                return true;
            }
            catch (SqlException sql_ex)
            {
                throw new Exception(sql_ex.Message);
            }
        }
        // Метод для выполнения запроса и возврата результата в виде DataTable
        public async Task<DataTable> ExecuteQuery(string query)
        {
            if (sqlConnection.State != System.Data.ConnectionState.Open)
            {
                OpenConnection();
            }

            DataTable resultTable = new DataTable();

            using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
            using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
            {
                // Заполняем DataTable данными из результата запроса
                resultTable.Load(reader);
            }

            CloseConnection();
            return resultTable;
        }



    }
}
