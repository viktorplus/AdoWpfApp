using DBEntity;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdoWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DatabaseManager databaseManager;
        const string ConnectionString = @"Data Source=HOMEDE\SQLEXPRESS;Initial Catalog=Student;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
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
            catch (System.Exception)
            {
                MessageBox.Show("Connection opened failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //DG_Table.ItemsSource = await databaseManager.ExecuteReaderProducts();
        }
    }
}