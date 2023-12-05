using AdoWpfApp.Pages;
using DBEntity;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace AdoWpfApp
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseManager databaseManager;
        private const string ConnectionString = @"Data Source=HomeDE\SQLEXPRESS;Initial Catalog=Kanztovar;Integrated Security=True;Encrypt=False";

        public MainWindow()
        {
            InitializeComponent();
            databaseManager = new DatabaseManager(ConnectionString);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var result = await databaseManager.OpenConnection();
            try
            {
                if (result)
                {
                    //MessageBox.Show("Connection opened successfully!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Connection opened failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //await LoadData();
            MainFrame.Content = new Query(databaseManager);

        }


        private void Click_Create(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Create(databaseManager);
        }


        private void Query_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Query(databaseManager);
        }

        private void Click_Delete(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Delete(databaseManager);
        }

        private void Click_Update(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Edit(databaseManager);
        }
    }
}
