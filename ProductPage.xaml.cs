using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Товары_школы_Кравец.Classes;

namespace Товары_школы_Кравец
{
    /// <summary>
    /// Логика взаимодействия для Product.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        ObservableCollection<Product> prods = new ObservableCollection<Product>();
        public ProductPage()
        {
            InitializeComponent();
            GetUserRole();
            DataContext = ProjectManager.Context.Product;
            var allManuf = ProjectManager.Context.Manufacturer.ToList();
            allManuf.Insert(0, new Manufacturer
            {
                Name = "Все производители"
            });
            manufacturerBox.ItemsSource = allManuf;
            manufacturerBox.SelectedIndex = 0;
            prods.CollectionChanged += ProductsListChanged;
            UpdateProducts();
        }

        private void GetUserRole()
        {
            switch (ProjectManager.UserRole)
            {
                case 0:
                    AddButton.Visibility = Visibility.Collapsed;
                    EditButton.Visibility = Visibility.Collapsed;
                    DeleteBtn.Visibility = Visibility.Collapsed;
                    SellJournalBtn.Visibility = Visibility.Collapsed; break;
                case 1:
                    AddButton.Visibility = Visibility.Visible;
                    EditButton.Visibility = Visibility.Visible;
                    DeleteBtn.Visibility = Visibility.Visible;
                    SellJournalBtn.Visibility = Visibility.Visible; break;
                case 2:
                    AddButton.Visibility = Visibility.Collapsed;
                    EditButton.Visibility = Visibility.Collapsed;
                    DeleteBtn.Visibility = Visibility.Collapsed;
                    SellJournalBtn.Visibility = Visibility.Visible; break;
                case 3:
                    AddButton.Visibility = Visibility.Collapsed;
                    EditButton.Visibility = Visibility.Collapsed;
                    DeleteBtn.Visibility = Visibility.Collapsed;
                    SellJournalBtn.Visibility = Visibility.Collapsed; break;
            }
        }

        private void UpdateProducts()
        {
            try
            {
                noResultTxb.Visibility = Visibility.Hidden;
                productsLView.Visibility = Visibility.Visible;
                var products = ProjectManager.Context.Product.ToList();
                if (manufacturerBox.SelectedIndex > 0)
                    products = (manufacturerBox.SelectedItem as Manufacturer).Product.ToList();
                products = products.Where(p => p.Title.ToLower().Contains(searchingBox.Text.ToLower()) ||
                        (p.Description != null && p.Description.ToLower().Contains(searchingBox.Text.ToLower()))).ToList();
                if (string.IsNullOrWhiteSpace(searchingBox.Text))
                {
                    if (manufacturerBox.SelectedIndex > 0)
                        products = (manufacturerBox.SelectedItem as Manufacturer).Product.ToList();
                    prods = new ObservableCollection<Product>(products);
                    productsLView.ItemsSource = prods;
                    UpdateCollection();
                    return;
                }
                prods = new ObservableCollection<Product>(products);
                productsLView.ItemsSource = prods;
                UpdateCollection();
            }
            catch (Exception ex)
            {
                ProjectManager.ShowError(ex.Message);
            }
        }

        private void UpdateCollection()
        {
            try
            {
                statsBlock.Text = $"Показано {productsLView.Items.Count} из {ProjectManager.Context.Product.Count()} элементов";
                NoResultReply();
            }
            catch (Exception ex)
            {
                ProjectManager.ShowError(ex.Message);
                statsBlock.Text = string.Empty;
            }
        }
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (Product.ID != 0)
                //ProjectManager.Context.Entry(Product).Reload();
            }
            catch (Exception ex)
            {
                ProjectManager.ShowError(ex.Message);
            }
            finally
            {
                ProjectManager.MainFrame.GoBack();
            }
        }

        private void SearchingByTitleOrDescription(object sender, TextChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void FilteringByManufacturer(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void NoResultReply()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (productsLView.Items.Count <= 0)
                {
                    noResultTxb.Visibility = Visibility.Visible;
                    productsLView.Visibility = Visibility.Hidden;
                }
            }), System.Windows.Threading.DispatcherPriority.Background);

        }

        private void productsLView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ProductsListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void SortByCost(object sender, SelectionChangedEventArgs e)
        {
            var products = productsLView.ItemsSource.Cast<Product>();
            switch (sortBox.SelectedIndex)
            {
                default:
                    productsLView.ItemsSource = products.OrderBy(p => p.ID).ToList();
                    break;
                case 1:
                    productsLView.ItemsSource = products.OrderBy(p => p.Cost).ToList();
                    break;
                case 2:
                    productsLView.ItemsSource = products.OrderByDescending(p => p.Cost).ToList();
                    break;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Product selectedProduct = productsLView.SelectedItem as Product;
                if (selectedProduct.ProductSale.Count != 0)
                {
                    ProjectManager.ShowWarning("Нельзя удалить уже проданный товар!");
                    return;
                }

                MessageBoxResult result = ProjectManager.ShowQuestion("Вы действительно хотите удалить выбранный товар?");

                if (result == MessageBoxResult.No)
                    return;

                selectedProduct.Product1.Clear();
                if (!string.IsNullOrWhiteSpace(selectedProduct.MainImagePath))
                    File.Delete(selectedProduct.MainImagePath);
                ProjectManager.Context.Product.Remove(selectedProduct);
                ProjectManager.Context.SaveChanges();
                ProjectManager.ShowInformation("Товар и информация по прикрепленным товарам успешно удалена!");
                UpdateProducts();
            }
            catch (Exception ex)
            {
                ProjectManager.ShowError(ex.Message);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e) => ProjectManager.MainFrame.Navigate(new ProductAddPage(false));

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Product selectedProduct = productsLView.SelectedItem as Product;
            ProductAddPage productAddPage = new ProductAddPage(false) { DataContext = selectedProduct, Product = selectedProduct };
            ProjectManager.MainFrame.Navigate(productAddPage);
        }

        private void SellJournalBtn_Click(object sender, RoutedEventArgs e)
        {
            ProjectManager.MainFrame.Navigate(new SellJournalPage() { Product = productsLView.SelectedItem as Product });
        }
    }
}
