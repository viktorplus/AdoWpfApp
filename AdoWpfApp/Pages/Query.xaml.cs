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
    /// Логика взаимодействия для Query.xaml
    /// </summary>
    public partial class Query : UserControl
    {
        private readonly DatabaseManager databaseManager;

        public Query(DatabaseManager dbManager)
        {
            InitializeComponent();
            databaseManager = dbManager;
            LoadData();
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
