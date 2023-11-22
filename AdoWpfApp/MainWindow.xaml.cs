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
        const string ConnectionString = @"Data Source=HomeDE\SQLEXPRESS;Initial Catalog=Warehouse;Integrated Security=True;Encrypt=False";
        public MainWindow()

        {
            InitializeComponent();
            databaseManager = new DatabaseManager(ConnectionString);
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //DG_Table.ItemsSource = await databaseManager.ExecuteReaderProducts();
        }
    }
}