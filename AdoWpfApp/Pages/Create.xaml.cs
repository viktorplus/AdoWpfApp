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

        }
        private async Task LoadProductTypes()
        {
            try
            {
                string query = "SELECT ID, Name FROM Stationery_Type";
                DataTable productTypesTable = await databaseManager.ExecuteQuery(query);

                List<string> productTypeNames = productTypesTable.AsEnumerable()
                    .Select(row => row.Field<string>("Name"))
                    .ToList();

                ComboBoxProductType.ItemsSource = productTypeNames;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке типов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<int> GetProductTypeIdByName(string productTypeName)
        {
            string query = $"SELECT Top 1 ID FROM Stationery_Type WHERE Name = '{productTypeName}'";
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
                if (ComboBoxProductType.SelectedItem != null)
                {
                    string selectedProductTypeName = ComboBoxProductType.SelectedItem.ToString();
                    int productTypeId = await GetProductTypeIdByName(selectedProductTypeName);
                        int quantity = Convert.ToInt32(TextBoxQuantity.Text);
                        decimal cost = Convert.ToDecimal(TextBoxCost.Text);

                        string query = $"INSERT INTO Stationery (Name, Type_ID, Cost, Quantity) VALUES " +
                            $"('{productName}', {productTypeId}, {quantity}, {cost})";

                    TextBoxProductName.Clear();
                    TextBoxQuantity.Clear();
                    TextBoxCost.Clear();

                    await databaseManager.ExecuteNonQuery(query);
                        MessageBox.Show($"Продукт добавлен успешно: {query}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении продукта: {ex.ToString()}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void AddProductType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string productTypeName = TextBoxNewProductType.Text;
                int typeQuantity = Convert.ToInt32(TextBoxTypeQuantity.Text);
                if (!string.IsNullOrEmpty(productTypeName))
                {
                    string query = $"INSERT INTO Stationery_Type (Name, Quantity) VALUES ('{productTypeName}', {typeQuantity})";
                    await databaseManager.ExecuteNonQuery(query);

                    TextBoxNewProductType.Clear();

                    await LoadProductTypes();
                    MessageBox.Show($"Тип товара добавлен успешно: {query}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Введите название типа товара", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении типа товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddManager_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ManagerName = TextBoxName.Text;
                string ManagerLastName = TextBoxLastName.Text;
                string ManagerPhone = TextBoxManagerPhone.Text;
                string query = $"INSERT INTO Sales_Manager (First_Name, Last_Name, Phone) VALUES ('{ManagerName}', '{ManagerLastName}', {ManagerPhone})";
                await databaseManager.ExecuteNonQuery(query);

                TextBoxName.Clear();
                TextBoxLastName.Clear();
                TextBoxManagerPhone.Clear();
                MessageBox.Show($"Тип товара добавлен успешно: {query}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении типа товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddCompany_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Company_Name = TextBoxCompanyName.Text;
                string Phone = TextBoxCompanyPhone.Text;
                string query = $"INSERT INTO Buyer_Company (Company_Name, Phone) VALUES ('{Company_Name}', {Phone})";
                await databaseManager.ExecuteNonQuery(query);

                TextBoxCompanyName.Clear();
                TextBoxCompanyPhone.Clear();
                MessageBox.Show($"Тип товара добавлен успешно: {query}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении типа товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}

