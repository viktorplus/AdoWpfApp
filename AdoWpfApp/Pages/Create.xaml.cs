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
        private async Task LoadProductTypes()
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

        private async Task LoadSupplierNames()
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

        private async void AddProductType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получение данных из текстового поля
                string productTypeName = TextBoxNewProductType.Text;

                // Проверка, что поле не пусто
                if (!string.IsNullOrEmpty(productTypeName))
                {
                    // Формирование и выполнение запроса на добавление типа товара
                    string query = $"INSERT INTO ProductTypes (TypeName) VALUES ('{productTypeName}')";
                    await databaseManager.ExecuteNonQuery(query);

                    // Очистка текстового поля после успешного добавления
                    TextBoxNewProductType.Clear();

                    // Обновление списка типов продуктов
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
                // Обработка ошибок
                MessageBox.Show($"Ошибка при добавлении типа товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddSupplier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получение данных из текстовых полей
                string supplierName = TextBoxNewSupplier.Text;
                string supplierAddress = TextBoxSupplierAddress.Text;
                string supplierPhone = TextBoxSupplierPhone.Text;

                // Проверка, что поля не пусты
                if (!string.IsNullOrEmpty(supplierName) && !string.IsNullOrEmpty(supplierAddress) && !string.IsNullOrEmpty(supplierPhone))
                {
                    // Формирование и выполнение запроса на добавление поставщика
                    string query = $"INSERT INTO Suppliers (SupplierName, Address, Phone) VALUES ('{supplierName}', '{supplierAddress}', '{supplierPhone}')";
                    await databaseManager.ExecuteNonQuery(query);

                    // Очистка текстовых полей после успешного добавления
                    TextBoxNewSupplier.Clear();
                    TextBoxSupplierAddress.Clear();
                    TextBoxSupplierPhone.Clear();

                    // Обновление списка поставщиков
                    await LoadSupplierNames();
                    MessageBox.Show($"Поставщик добавлен успешно: {query}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    MessageBox.Show("Введите все данные поставщика", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                MessageBox.Show($"Ошибка при добавлении поставщика: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




    }
}

