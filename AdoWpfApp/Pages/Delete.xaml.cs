using DBEntity;
using System.Data;
using System.Windows;
using System.Windows.Controls;

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
                ComboBoxCurrentProductType.ItemsSource = productTypeNames;
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

        private async void DeleteCompany_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(TextBoxCompanyId.Text, out int companyId))
                {
                    string deleteCompanyQuery = $"DELETE FROM Buyer_Company WHERE ID = {companyId}";

                    await databaseManager.ExecuteNonQuery(deleteCompanyQuery);

                    MessageBox.Show("Компания успешно удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Введите корректный ID компании.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении компании: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteManager_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(TextBoxManagerId.Text, out int managerId))
                {
                    string deleteManagerQuery = $"DELETE FROM Sales_Manager WHERE ID = {managerId}";

                    await databaseManager.ExecuteNonQuery(deleteManagerQuery);

                    MessageBox.Show("Менеджер успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Введите корректный ID менеджера.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении менеджера: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void LoadCompanyData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(TextBoxCompanyId.Text, out int companyId))
                {
                    // Запрос для получения данных о компании
                    string companyQuery = $@"
                SELECT 
                    BC.ID, 
                    BC.Company_Name, 
                    BC.Phone
                FROM 
                    Buyer_Company BC
                WHERE 
                    BC.ID = {companyId}";

                    DataTable companyData = await databaseManager.ExecuteQuery(companyQuery);

                    if (companyData.Rows.Count > 0)
                    {
                        DataRow row = companyData.Rows[0];

                        TextBoxCompanyName.Text = row.Field<string>("Company_Name");
                        TextBoxCompanyPhone.Text = row.Field<string>("Phone");
                    }
                    else
                    {
                        MessageBox.Show("Компания с указанным ID не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Введите корректный ID компании.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных компании: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadManagerData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(TextBoxManagerId.Text, out int managerId))
                {
                    // Запрос для получения данных о менеджере
                    string managerQuery = $@"
                    SELECT 
                        S.ID, 
                        S.First_Name AS FirstName, 
                        S.Last_Name AS LastName, 
                        S.Phone
                    FROM 
                        Sales_Manager S
                    WHERE 
                        S.ID = {managerId}";


                    DataTable managerData = await databaseManager.ExecuteQuery(managerQuery);

                    if (managerData.Rows.Count > 0)
                    {
                        DataRow row = managerData.Rows[0];

                        TextBoxManagerName.Text = row.Field<string>("FirstName");
                        TextBoxManagerSurName.Text = row.Field<string>("LastName");
                        TextBoxManagerPhone.Text = row.Field<string>("Phone");
                    }
                    else
                    {
                        MessageBox.Show("Менеджер с указанным ID не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Введите корректный ID менеджера.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных менеджера: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ComboBoxCurrentProductType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
