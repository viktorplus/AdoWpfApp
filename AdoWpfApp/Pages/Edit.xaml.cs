using DBEntity;
using System;
using System.Collections;
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
                    MessageBox.Show(productQuery, "query", MessageBoxButton.OK, MessageBoxImage.Information);


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



        private async void UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(TextBoxProductId.Text, out int productId))
                {
                    // Получение данных из полей
                    string productName = TextBoxProductName.Text;
                    int productTypeId = await GetProductTypeIdByName(ComboBoxProductType.SelectedItem as string);
                    int quantity = int.Parse(TextBoxQuantity.Text);
                    decimal cost = decimal.Parse(TextBoxCost.Text);

                    string query = "UPDATE Stationery SET Name = @productName, " +
                                   "Type_ID = @productTypeId, Cost = @cost, " +
                                   "Quantity = @quantity " +
                                   "WHERE ID = @productId";

                    var parameters = new Dictionary<string, object>
            {
                { "@productId", productId },
                { "@productName", productName },
                { "@productTypeId", productTypeId },
                { "@cost", cost },
                { "@quantity", quantity }
            };

                    MessageBox.Show(query, "query", MessageBoxButton.OK, MessageBoxImage.Information);
                    await databaseManager.ExecuteNonQuery(query, parameters);

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
            string query = $"SELECT Top 1 ID FROM Stationery_Type WHERE Name = '{productTypeName}'";
            DataTable resultTable = await databaseManager.ExecuteQuery(query);

            if (resultTable.Rows.Count > 0)
            {
                return resultTable.Rows[0].Field<int>("ID");
            }

            return -1;
        }

        private async void UpdateProductType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedProductType = ComboBoxCurrentProductType.SelectedItem as string;
                string newProductTypeValue = TextBoxNewProductTypeValue.Text;

                if (!string.IsNullOrEmpty(selectedProductType) && !string.IsNullOrEmpty(newProductTypeValue))
                {
                    // Ваш запрос на обновление типа продукта
                    string updateProductTypeQuery = $"UPDATE Stationery_Type SET Name = '{newProductTypeValue}' WHERE Name = '{selectedProductType}'";

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
        private async void ComboBoxCurrentProductType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ComboBoxCurrentProductType.SelectedItem != null)
                {
                    string selectedProductType = ComboBoxCurrentProductType.SelectedItem as string;

                    string productTypeQuery = $"SELECT Quantity FROM Stationery_Type WHERE Name = '{selectedProductType}'";
                    DataTable productTypeData = await databaseManager.ExecuteQuery(productTypeQuery);

                    if (productTypeData.Rows.Count > 0)
                    {
                        int typeQuantity = productTypeData.Rows[0].Field<int>("Quantity");
                        TextBoxTypeQuantity.Text = typeQuantity.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Данные о типе продукта не найдены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных о типе продукта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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



        private async void UpdateManager_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(TextBoxManagerId.Text, out int supplierId))
                {
                    // Получение данных из UI элементов
                    string supplierName = TextBoxManagerName.Text;
                    string LastName = TextBoxManagerSurName.Text;
                    string supplierPhone = TextBoxManagerPhone.Text;

                    // Ваш запрос на обновление данных поставщика
                    string updateSupplierQuery = $"UPDATE Sales_Manager SET First_Name = '{supplierName}', Last_Name = '{LastName}', Phone = '{supplierPhone}' WHERE ID = {supplierId}";

                    // Выполнение запроса на обновление данных
                    await databaseManager.ExecuteNonQuery(updateSupplierQuery);

                    MessageBox.Show("Данные менеджера успешно обновлены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Введите корректный ID менеджера.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных менеджера: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private async void UpdateCompany_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(TextBoxCompanyId.Text, out int companyId))
                {
                    // Получение данных из UI элементов
                    string companyName = TextBoxCompanyName.Text;
                    string companyPhone = TextBoxCompanyPhone.Text;

                    // Ваш запрос на обновление данных компании
                    string updateCompanyQuery = $@"
                UPDATE Buyer_Company 
                SET Company_Name = '{companyName}', 
                    Phone = '{companyPhone}' 
                WHERE ID = {companyId}";

                    // Выполнение запроса на обновление данных
                    await databaseManager.ExecuteNonQuery(updateCompanyQuery);

                    MessageBox.Show("Данные компании успешно обновлены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Введите корректный ID компании.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных компании: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
