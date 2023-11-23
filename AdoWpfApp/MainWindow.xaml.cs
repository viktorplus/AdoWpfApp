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

        private async void ComboBoxQueries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
                    // Предполагается, что вы установите минимальную оценку, например, 9.0
                    query = "SELECT Surname, Name, Vatername FROM Students WHERE Gradeavr > 9.0";
                    break;
                case "Show Subjects with Minimum Average Grades":
                    query = "SELECT DISTINCT Subjectmin FROM Students WHERE Gradeavr = (SELECT MIN(Gradeavr) FROM Students)";
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(query))
            {
                DG_Table.ItemsSource = await databaseManager.ExecuteReader(query);
            }
        }

    }
}