using DBEntity;
using System.Data;
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
            //try
            //{
            //    if (result)
            //    {
            //        MessageBox.Show("Connection opened successfully!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("Connection opened failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}

            await LoadData();
        }

        private async void ComboBoxQueries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxQueries.SelectedItem == null)
                return;

            string selectedQuery = (string)((ComboBoxItem)ComboBoxQueries.SelectedItem).Tag;
            DataTable resultTable = await databaseManager.ExecuteQuery(selectedQuery);
            DG_Table.ItemsSource = resultTable.DefaultView;
        }

        private async Task LoadData()
        {
            ComboBoxQueries.SelectedIndex = 0;
        }


    }
}
