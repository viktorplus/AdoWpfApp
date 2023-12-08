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
                string productTypeQuery = "SELECT ID, Name FROM Stationery_Type";
                DataTable productTypesTable = await databaseManager.ExecuteQuery(productTypeQuery);
                List<string> productTypeNames = productTypesTable.AsEnumerable()
                    .Select(typeRow => typeRow.Field<string>("Name"))
                    .ToList();

                ComboBoxProductType.ItemsSource = productTypeNames;
                //ComboBoxCurrentProductType.ItemsSource = productTypeNames;
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
                        P.Name, 
                        P.Quantity, 
                        P.Cost, 
                        PT.Name AS ProductTypeName
                    FROM 
                        Stationery P
                        INNER JOIN Stationery_Type PT ON P.Type_ID = PT.ID
                    WHERE 
                        P.ID = " + productId;

                    DataTable productData = await databaseManager.ExecuteQuery(productQuery);


                    if (productData.Rows.Count > 0)
                    {
                        DataRow row = productData.Rows[0];

                        TextBoxProductName.Text = row.Field<string>("ProductTypeName");
                        TextBoxQuantity.Text = row.Field<int>("Quantity").ToString();
                        TextBoxCost.Text = row.Field<decimal>("Cost").ToString();

                        // Установим выбранное значение вручную
                        string selectedProductTypeName = row.Field<string>("ProductTypeName");
                        ComboBoxProductType.SelectedItem = selectedProductTypeName;
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

        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(TextBoxProductId.Text, out int productId))
                {
                    string deleteProductQuery = $"DELETE FROM Stationery WHERE ID = {productId}";

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
                    string deleteProductTypeQuery = $"DELETE FROM Stationery_Type WHERE Name = '{selectedProductType}'";

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

        private void DeleteCompany_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteManager_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadCompanyData_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadManagerData_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
