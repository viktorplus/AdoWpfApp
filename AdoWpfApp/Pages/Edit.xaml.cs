using DBEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AdoWpfApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для Edit.xaml
    /// </summary>
    public partial class Edit : UserControl
    {
        private readonly DatabaseManager databaseManager;

        public Edit(DatabaseManager dbManager)
        {
            InitializeComponent();
            databaseManager = dbManager;
            LoadComboBoxData();
        }

        private async void LoadComboBoxData()
        {
            try
            {
                // Загрузка данных о типах продуктов
                string productTypeQuery = "SELECT ID, TypeName FROM ProductTypes";
                DataTable productTypesTable = await databaseManager.ExecuteQuery(productTypeQuery);
                List<string> productTypeNames = productTypesTable.AsEnumerable()
                    .Select(typeRow => typeRow.Field<string>("TypeName"))
                    .ToList();

                ComboBoxProductType.ItemsSource = productTypeNames;
                ComboBoxCurrentProductType.ItemsSource = productTypeNames;

                // Загрузка данных о поставщиках
                string supplierQuery = "SELECT ID, SupplierName FROM Suppliers";
                DataTable suppliersTable = await databaseManager.ExecuteQuery(supplierQuery);
                List<string> supplierNames = suppliersTable.AsEnumerable()
                    .Select(supplierRow => supplierRow.Field<string>("SupplierName"))
                    .ToList();

                ComboBoxSupplier.ItemsSource = supplierNames;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных для комбобоксов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void LoadProductData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(TextBoxProductId.Text, out int productId))
                {
                    // Запрос для получения данных о продукте с информацией о типе продукта и поставщике
                    string productQuery = @"
                    SELECT 
                        P.ID, 
                        P.ProductName, 
                        P.Quantity, 
                        P.Cost, 
                        P.SupplyDate, 
                        PT.TypeName AS ProductTypeName, 
                        S.SupplierName
                    FROM 
                        Products P
                        INNER JOIN ProductTypes PT ON P.ProductTypeID = PT.ID
                        LEFT JOIN Suppliers S ON P.SupplierID = S.ID
                    WHERE 
                        P.ID = " + productId;

                    DataTable productData = await databaseManager.ExecuteQuery(productQuery);


                    if (productData.Rows.Count > 0)
                    {
                        DataRow row = productData.Rows[0];

                        TextBoxProductName.Text = row.Field<string>("ProductName");
                        TextBoxQuantity.Text = row.Field<int>("Quantity").ToString();
                        TextBoxCost.Text = row.Field<decimal>("Cost").ToString();
                        DatePickerSupplyDate.SelectedDate = row.Field<DateTime>("SupplyDate");

                        // Установим выбранное значение вручную
                        string selectedProductTypeName = row.Field<string>("ProductTypeName");
                        ComboBoxProductType.SelectedItem = selectedProductTypeName;

                        // Установим выбранное значение вручную
                        string selectedSupplierName = row.Field<string>("SupplierName");
                        //int selectedSupplierIndex = supplierNames.IndexOf(selectedSupplierName);
                        //ComboBoxSupplier.SelectedIndex = selectedSupplierIndex;
                        ComboBoxSupplier.SelectedItem = selectedSupplierName;
                    }
                    else
                    {
                        MessageBox.Show("Продукт с указанным ID не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Введите корректный ID продукта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных продукта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private async void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(TextBoxProductId.Text, out int productId))
                {
                    // Получение данных из полей
                    string productName = TextBoxProductName.Text;
                    int productTypeId = await GetProductTypeIdByName(ComboBoxProductType.SelectedItem as string);
                    int supplierId = await GetSupplierIdByName(ComboBoxSupplier.SelectedItem as string);
                    int quantity = int.Parse(TextBoxQuantity.Text);
                    decimal cost = decimal.Parse(TextBoxCost.Text);
                    DateTime supplyDate = DatePickerSupplyDate.SelectedDate ?? DateTime.Now;

                    string query = "UPDATE Products SET ProductName = @productName, " +
                                   "ProductTypeID = @productTypeId, SupplierID = @supplierId, " +
                                   "Quantity = @quantity, Cost = @cost, SupplyDate = @supplyDate " +
                                   "WHERE ID = @productId";

                    var parameters = new Dictionary<string, object>
                        {
                            { "@productName", productName },
                            { "@productTypeId", productTypeId },
                            { "@supplierId", supplierId },
                            { "@quantity", quantity },
                            { "@cost", cost },
                            { "@supplyDate", supplyDate },
                            { "@productId", productId }
                        };

                    await databaseManager.ExecuteNonQuery(query, parameters);




                    //MessageBox.Show(query, "query", MessageBoxButton.OK, MessageBoxImage.Information);
                    //await databaseManager.ExecuteNonQuery(query);

                    MessageBox.Show("Данные продукта успешно обновлены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Введите корректный ID продукта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных продукта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<int> GetProductTypeIdByName(string productTypeName)
        {
            string query = $"SELECT Top 1 ID FROM ProductTypes WHERE TypeName = '{productTypeName}'";
            DataTable resultTable = await databaseManager.ExecuteQuery(query);

            if (resultTable.Rows.Count > 0)
            {
                return resultTable.Rows[0].Field<int>("ID");
            }

            return -1;
        }

        private async Task<int> GetSupplierIdByName(string supplierName)
        {
            string query = $"SELECT ID FROM Suppliers WHERE SupplierName = '{supplierName}'";
            DataTable resultTable = await databaseManager.ExecuteQuery(query);

            if (resultTable.Rows.Count > 0)
            {
                return resultTable.Rows[0].Field<int>("ID");
            }

            return -1;
        }

        private async void SaveProductType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedProductType = ComboBoxCurrentProductType.SelectedItem as string;
                string newProductTypeValue = TextBoxNewProductTypeValue.Text;

                if (!string.IsNullOrEmpty(selectedProductType) && !string.IsNullOrEmpty(newProductTypeValue))
                {
                    // Ваш запрос на обновление типа продукта
                    string updateProductTypeQuery = $"UPDATE ProductTypes SET TypeName = '{newProductTypeValue}' WHERE TypeName = '{selectedProductType}'";

                    await databaseManager.ExecuteNonQuery(updateProductTypeQuery);

                    MessageBox.Show("Изменения типа продукта успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Выберите текущий тип продукта и введите новое значение.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений типа продукта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadSupplierData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(TextBoxSupplierId.Text, out int supplierId))
                {
                    // Запрос для получения данных о поставщике
                    string supplierQuery = $@"
                SELECT 
                    S.ID, 
                    S.SupplierName, 
                    S.Address, 
                    S.Phone
                FROM 
                    Suppliers S
                WHERE 
                    S.ID = {supplierId}";

                    DataTable supplierData = await databaseManager.ExecuteQuery(supplierQuery);

                    if (supplierData.Rows.Count > 0)
                    {
                        DataRow row = supplierData.Rows[0];

                        TextBoxSupplierName.Text = row.Field<string>("SupplierName");
                        TextBoxSupplierAddress.Text = row.Field<string>("Address");
                        TextBoxSupplierPhone.Text = row.Field<string>("Phone");
                    }
                    else
                    {
                        MessageBox.Show("Поставщик с указанным ID не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Введите корректный ID поставщика.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных поставщика: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void SaveSupplierData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(TextBoxSupplierId.Text, out int supplierId))
                {
                    // Получение данных из UI элементов
                    string supplierName = TextBoxSupplierName.Text;
                    string supplierAddress = TextBoxSupplierAddress.Text;
                    string supplierPhone = TextBoxSupplierPhone.Text;

                    // Ваш запрос на обновление данных поставщика
                    string updateSupplierQuery = $"UPDATE Suppliers SET SupplierName = '{supplierName}', Address = '{supplierAddress}', Phone = '{supplierPhone}' WHERE ID = {supplierId}";

                    // Выполнение запроса на обновление данных
                    await databaseManager.ExecuteNonQuery(updateSupplierQuery);

                    MessageBox.Show("Данные поставщика успешно обновлены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Введите корректный ID поставщика.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных поставщика: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
