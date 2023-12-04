using DBEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdoWpfApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для Delete.xaml
    /// </summary>
    public partial class Delete : UserControl
    {
        private readonly DatabaseManager databaseManager;
        public Delete(DatabaseManager dbManager)
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
        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(TextBoxProductId.Text, out int productId))
                {
                    // Запрос для удаления продукта
                    string deleteProductQuery = $"DELETE FROM Products WHERE ID = {productId}";

                    await databaseManager.ExecuteNonQuery(deleteProductQuery);

                    MessageBox.Show("Продукт успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Введите корректный ID продукта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении продукта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteProductType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получение выбранного типа продукта
                string selectedProductType = ComboBoxProductType.SelectedItem as string;

                if (!string.IsNullOrEmpty(selectedProductType))
                {
                    // Запрос для удаления типа продукта
                    string deleteProductTypeQuery = $"DELETE FROM ProductTypes WHERE TypeName = '{selectedProductType}'";

                    await databaseManager.ExecuteNonQuery(deleteProductTypeQuery);

                    MessageBox.Show("Тип продукта успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Выберите тип продукта для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении типа продукта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(TextBoxSupplierId.Text, out int supplierId))
                {
                    // Запрос для удаления поставщика
                    string deleteSupplierQuery = $"DELETE FROM Suppliers WHERE ID = {supplierId}";

                    await databaseManager.ExecuteNonQuery(deleteSupplierQuery);

                    MessageBox.Show("Поставщик успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Введите корректный ID поставщика.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении поставщика: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
