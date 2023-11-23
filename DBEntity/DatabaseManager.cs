using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        //public async Task<List<dynamic>> ExecuteReaderDynamic(string query)
        //{
        //    if (sqlConnection.State != System.Data.ConnectionState.Open)
        //    {
        //        OpenConnection();
        //    }

        //    List<dynamic> result = new List<dynamic>();

        //    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
        //    using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
        //    {
        //        while (reader.Read())
        //        {
        //            var row = new System.Dynamic.ExpandoObject() as IDictionary<string, Object>;

        //            for (int i = 0; i < reader.FieldCount; i++)
        //            {
        //                string columnName = reader.GetName(i);
        //                object value = reader.GetValue(i);
        //                row[columnName] = value;
        //            }

        //            result.Add(row);
        //        }
        //    }

        //    CloseConnection();
        //    return result;
        //}



        // Метод для выполнения запроса с возвращаемым результатом


        //// Метод для выполнения запроса с возвращаемым скалярным результатом
        //public async Task<T> ExecuteScalar<T>(string query)
        //{
        //    if (sqlConnection.State != System.Data.ConnectionState.Open)
        //    {
        //        OpenConnection();
        //    }

        //    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
        //    {
        //        object result = await sqlCommand.ExecuteScalarAsync();
        //        CloseConnection();

        //        if (result != null && result != DBNull.Value)
        //        {
        //            return (T)result;
        //        }

        //        return default(T);
        //    }
        //}
    }
}
