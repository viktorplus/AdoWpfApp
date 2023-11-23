using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoWpfApp
{
    class Class1
    {
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
//public async Task<List<Dictionary<string, object>>> Q1(string query)
//{
//    if (sqlConnection.State != System.Data.ConnectionState.Open)
//    {
//        OpenConnection();
//    }

//    List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

//    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
//    using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
//    {
//        while (reader.Read())
//        {
//            Dictionary<string, object> row = new Dictionary<string, object>();
//            for (int i = 0; i < reader.FieldCount; i++)
//            {
//                row[reader.GetName(i)] = reader[i];
//            }
//            result.Add(row);
//        }
//    }

//    CloseConnection();
//    return result;
//}


//public async Task<DataTable> ExecuteQuery(string query)
//{
//    if (sqlConnection.State != System.Data.ConnectionState.Open)
//    {
//        OpenConnection();
//    }

//    DataTable result = new DataTable();

//    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
//    using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
//    {
//        result.Load(reader);
//    }

//    CloseConnection();
//    return result;
//}