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

        public async Task<bool> OpenConnection()
        {
            try
            {
                await sqlConnection.OpenAsync();
                return true;
            }
            catch (SqlException sql_ex)
            {
                throw new Exception(sql_ex.Message);
            }
        }

        public async Task<bool> CloseConnection()
        {
            try
            {
                await sqlConnection.CloseAsync();
                return true;
            }
            catch (SqlException sql_ex)
            {
                throw new Exception(sql_ex.Message);
            }
        }


        public async Task<int> ExecuteNonQuery(string query)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                OpenConnection();
            }

            using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
            {
                int affectedRows = await sqlCommand.ExecuteNonQueryAsync();
                CloseConnection();
                return affectedRows;
            }
        }

        public async Task<DataTable> ExecuteQuery(string query)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        await Task.Run(() => adapter.Fill(dataTable));
                        return dataTable;
                    }
                }
            }
        }

        public async Task<object> ExecuteScalar(string query)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                await OpenConnection();
            }

            using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
            {
                object result = await sqlCommand.ExecuteScalarAsync();
                await CloseConnection();
                return result;
            }
        }


        public async Task InsertProduct(string productName, int productTypeId, int supplierId, int quantity, decimal cost, DateTime supplyDate)
        {
            string query = $"INSERT INTO Products (ProductName, ProductTypeID, SupplierID, Quantity, Cost, SupplyDate) VALUES " +
                           $"('{productName}', {productTypeId}, {supplierId}, {quantity}, {cost}, '{supplyDate.ToString("yyyy-MM-dd")}')";

            await ExecuteNonQuery(query);
        }

        public async Task InsertProduct2(string productName, int productTypeId, int supplierId, int quantity, decimal cost, DateTime supplyDate)
        {
            string query = "INSERT INTO Products (ProductName, ProductTypeID, SupplierID, Quantity, Cost, SupplyDate) VALUES " +
                           $"(@ProductName, @ProductTypeID, @SupplierID, @Quantity, @Cost, @SupplyDate)";

            using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@ProductName", productName);
                sqlCommand.Parameters.AddWithValue("@ProductTypeID", productTypeId);
                sqlCommand.Parameters.AddWithValue("@SupplierID", supplierId);
                sqlCommand.Parameters.AddWithValue("@Quantity", quantity);
                sqlCommand.Parameters.AddWithValue("@Cost", cost);
                sqlCommand.Parameters.AddWithValue("@SupplyDate", supplyDate);

                await sqlCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task InsertProductType(string typeName)
        {
            string query = $"INSERT INTO ProductTypes (TypeName) VALUES ('{typeName}')";

            await ExecuteNonQuery(query);
        }

        public async Task InsertSupplier(string supplierName, string address, string phone)
        {
            string query = $"INSERT INTO Suppliers (SupplierName, Address, Phone) VALUES " +
                           $"('{supplierName}', '{address}', '{phone}')";

            await ExecuteNonQuery(query);
        }

        public async Task UpdateProduct(int productId, string productName, int productTypeId, int supplierId, int quantity, decimal cost, DateTime supplyDate)
        {
            string query = $"UPDATE Products SET ProductName = '{productName}', ProductTypeID = {productTypeId}, SupplierID = {supplierId}, " +
                           $"Quantity = {quantity}, Cost = {cost}, SupplyDate = '{supplyDate.ToString("yyyy-MM-dd")}' WHERE ID = {productId}";

            await ExecuteNonQuery(query);
        }

        public async Task UpdateProductType(int productTypeId, string typeName)
        {
            string query = $"UPDATE ProductTypes SET TypeName = '{typeName}' WHERE ID = {productTypeId}";

            await ExecuteNonQuery(query);
        }

        public async Task UpdateSupplier(int supplierId, string supplierName, string address, string phone)
        {
            string query = $"UPDATE Suppliers SET SupplierName = '{supplierName}', Address = '{address}', Phone = '{phone}' WHERE ID = {supplierId}";

            await ExecuteNonQuery(query);
        }

        public async Task DeleteProduct(int productId)
        {
            string query = $"DELETE FROM Products WHERE ID = {productId}";

            await ExecuteNonQuery(query);
        }

        public async Task DeleteProductType(int productTypeId)
        {
            string query = $"DELETE FROM ProductTypes WHERE ID = {productTypeId}";

            await ExecuteNonQuery(query);
        }

        public async Task DeleteSupplier(int supplierId)
        {
            string query = $"DELETE FROM Suppliers WHERE ID = {supplierId}";

            await ExecuteNonQuery(query);
        }

    }
}
/*MultipleActiveResultSets=True*/