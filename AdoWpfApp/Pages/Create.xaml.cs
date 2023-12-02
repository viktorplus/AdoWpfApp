using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using DBEntity;

namespace AdoWpfApp.Pages
{
    public partial class Create : UserControl
    {
        private readonly DatabaseManager databaseManager;

        public Create(DatabaseManager dbManager)
        {
            InitializeComponent();
            databaseManager = dbManager;
            LoadProductTypes();
            LoadSupplierNames();

        }
        private async void LoadProductTypes()
        {
            try
            {
                string query = "SELECT ID, TypeName FROM ProductTypes";
                DataTable productTypesTable = await databaseManager.ExecuteQuery(query);

                List<string> productTypeNames = productTypesTable.AsEnumerable()
                    .Select(row => row.Field<string>("TypeName"))
                    .ToList();

                ComboBoxProductType.ItemsSource = productTypeNames;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке типов продуктов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadSupplierNames()
        {
            try
            {
                string query = "SELECT SupplierName FROM Suppliers";
                DataTable suppliersTable = await databaseManager.ExecuteQuery(query);

                List<string> supplierNames = suppliersTable.AsEnumerable()
                    .Select(row => row.Field<string>("SupplierName"))
                    .ToList();

                ComboBoxSupplier.ItemsSource = supplierNames;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке поставщиков: {ex.Message}\nStackTrace: {ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private async Task<int> GetSupplierIdByName(string supplierName)
        {
            string query = $"SELECT Top 1 ID FROM Suppliers WHERE SupplierName = '{supplierName}'";
            DataTable resultTable = await databaseManager.ExecuteQuery(query);

            if (resultTable.Rows.Count > 0)
            {
                int supplierId = resultTable.Rows[0].Field<int>("ID");
                return supplierId;
            }
            return -1;
        }

        private async Task<int> GetProductTypeIdByName(string productTypeName)
        {
            string query = $"SELECT Top 1 ID FROM ProductTypes WHERE TypeName = '{productTypeName}'";
            DataTable resultTable = await databaseManager.ExecuteQuery(query);

            if (resultTable.Rows.Count > 0)
            {
                int productTypeId = resultTable.Rows[0].Field<int>("ID");
                return productTypeId;
            }
            return -1;
        }



        private async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string productName = TextBoxProductName.Text;
                MessageBox.Show($"productName: {productName}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                if (ComboBoxProductType.SelectedItem != null)
                {
                    string selectedProductTypeName = ComboBoxProductType.SelectedItem.ToString();
                    int productTypeId = await GetProductTypeIdByName(selectedProductTypeName);

                    if (ComboBoxSupplier.SelectedItem != null)
                    {
                        string selectedSupplierName = ComboBoxSupplier.SelectedItem.ToString();
                        int supplierId = await GetSupplierIdByName(selectedSupplierName);
                        int quantity = Convert.ToInt32(TextBoxQuantity.Text);
                        decimal cost = Convert.ToDecimal(TextBoxCost.Text);
                        DateTime supplyDate = DatePickerSupplyDate.SelectedDate ?? DateTime.Now;

                        string query = $"INSERT INTO Products (ProductName, ProductTypeID, SupplierID, Quantity, Cost, SupplyDate) VALUES " +
                            $"('{productName}', {productTypeId}, {supplierId}, {quantity}, {cost}, '{supplyDate.ToString("yyyy-MM-dd")}')";
                        // Используем ExecuteNonQuery синхронно
                       databaseManager.ExecuteNonQuery(query);
                        MessageBox.Show($"Продукт добавлен успешно: {query}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении продукта: {ex.ToString()}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

    }
}

