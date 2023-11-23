using DBEntity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace AdoWpfApp
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseManager databaseManager;
        private const string ConnectionString = @"Data Source=HOMEDE\SQLEXPRESS;Initial Catalog=Student;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public MainWindow()
        {
            InitializeComponent();
            databaseManager = new DatabaseManager(ConnectionString);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var result = databaseManager.OpenConnection();
            try
            {
                if (result)
                {
                    MessageBox.Show("Connection opened successfully!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Connection opened failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            await LoadData();
        }

        private async void ComboBoxQueries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxQueries.SelectedItem == null)
                return;

            string selectedQuery = ((ComboBoxItem)ComboBoxQueries.SelectedItem).Content.ToString();
            string query = "";

            switch (selectedQuery)
            {
                case "Show All Students and Grades":
                    query = "SELECT * FROM Students";
                    break;
                case "Show All Student Names":
                    query = "SELECT Surname, Name, Vatername FROM Students";
                    break;
                case "Show Average Grades":
                    query = "SELECT AVG(Gradeavr) AS AverageGrade FROM Students";
                    break;
                case "Show Students with Minimum Grade":
                    query = "SELECT * FROM Students WHERE Gradeavr > 9.0";
                    break;
                case "Show Subjects with Minimum Average Grades":
                    query = "SELECT DISTINCT Subjectmin FROM Students WHERE Gradeavr = (SELECT MIN(Gradeavr) FROM Students)";
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(query))
            {

                var result = await databaseManager.ExecuteReader(query);
                DG_Table.ItemsSource = await databaseManager.ExecuteReader(query);


                // Вывести результат в консоль для отладки
                foreach (var item in result)
                {
                    string output = "";

                    switch (selectedQuery)
                    {
                        case "Show All Students and Grades":
                            output = $"{item.StudentID}, {item.Surname}, {item.Name}, {item.Vatername}, {item.GroupName}, {item.Gradeavr}, {item.Subjectmin}, {item.Subjectmax}";
                            break;
                        case "Show All Student Names":
                            output = $"{item.Surname}, {item.Name}, {item.Vatername}";
                            break;
                        case "Show Average Grades":
                            output = $"Average Grade: {item.AverageGrade}";
                            break;
                        case "Show Students with Minimum Grade":
                            output = $"{item.StudentID}, {item.Surname}, {item.Name}, {item.Vatername}, {item.GroupName}, {item.Gradeavr}, {item.Subjectmin}, {item.Subjectmax}";
                            break;
                        case "Show Subjects with Minimum Average Grades":
                            output = $"Subject with Minimum Average Grade: {item.Subjectmin}";
                            break;
                        default:
                            break;
                    }

                    Debug.WriteLine(output); 

                }
            }
        }

        private async Task LoadData()
        {
            // По умолчанию загружаем данные для первого запроса
            ComboBoxQueries.SelectedIndex = 0;
        }
    }
}
