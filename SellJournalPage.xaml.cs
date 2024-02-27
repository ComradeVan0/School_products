using System;
using System.Collections.Generic;
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
using Товары_школы_Кравец.Classes;

namespace Товары_школы_Кравец
{
    /// <summary>
    /// Логика взаимодействия для SellJournalPage.xaml
    /// </summary>
    public partial class SellJournalPage : Page
    {
        public Product Product { get; set; }
        public SellJournalPage()
        {
            InitializeComponent();
        }

        private void GoBack_Click(object sender, RoutedEventArgs e) => ProjectManager.MainFrame.GoBack();


        private void productsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Product product = productsComboBox.SelectedItem as Product;
                if (product is null) return;

                productSalesDataGrid.ItemsSource = product.ProductSale.OrderByDescending(p => p.SaleDate);
                if (product.ProductSale.Count == 0)
                    ProjectManager.ShowWarning("Этот товар ещё не продавался!");
            }
            catch (Exception ex)
            {
                ProjectManager.ShowError(ex.Message);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Product is null)
                {
                    productsComboBox.ItemsSource = ProjectManager.Context.Product.ToList();
                    return;
                }
                productsComboBox.ItemsSource = ProjectManager.Context.Product.ToList();
                productsComboBox.SelectedItem = Product;
            }
            catch (Exception ex)
            {
                ProjectManager.ShowError(ex.Message);
            }
        }
    }
}
