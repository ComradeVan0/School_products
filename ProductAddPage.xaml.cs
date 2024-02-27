using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Логика взаимодействия для ProductAddPage.xaml
    /// </summary>
    public partial class ProductAddPage : Page
    {
        public Product Product { get; set; } = new Product() { IsActive = true };
        OpenFileDialog fileDialog = new OpenFileDialog()
        {
            CheckFileExists = true,
            CheckPathExists = true,
            Filter = "JPG|*.jpg|JPEG|*.jpeg|PNG|*.png|Все файлы|*.*"
        };
        string oldMainImagePath;
        bool IsBlocked;
        public ProductAddPage(bool IsNavigationBlocked)
        {
            InitializeComponent();
            IsBlocked = IsNavigationBlocked;
        }

        //private void GoBack_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (Product.ID != 0)
        //            ProjectManager.Context.Entry(Product).Reload();
        //    }
        //    catch (Exception ex)
        //    {
        //        ProjectManager.ShowError(ex.Message);
        //    }
        //    finally
        //    {
        //        ProjectManager.MainFrame.GoBack();
        //    }
        //}

        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void Action_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Product.Cost = costNumeric.Value;
                Product.Product1.Clear();
                List<Product> prodList = new List<Product>();
                for (int i = 0; i < extraProductList.Items.Count; i++)
                {
                    ListViewItem lvi = extraProductList.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                    if (lvi != null)
                    {
                        ContentPresenter cp = FindVisualChild<ContentPresenter>(lvi);
                        CheckBox ckbx = (CheckBox)cp.ContentTemplate.FindName("extraItem", cp);
                        if (ckbx.IsChecked == true)
                            prodList.Add(extraProductList.Items[i] as Product);
                    }
                }
                Product.Product1 = prodList;
                IDTextbox.IsEnabled = false;

                string text = string.Empty;
                if (Product.ID == 0)
                {
                    ProjectManager.Context.Product.Add(Product);
                    text = "Добавлен!";
                }
                else
                {
                    ProjectManager.Context.Entry(Product).State = System.Data.Entity.EntityState.Modified;
                    text = "Изменен!";
                }
                ProjectManager.Context.SaveChanges();

                if (!string.IsNullOrWhiteSpace(oldMainImagePath))
                    File.Delete(oldMainImagePath);

                if (!string.IsNullOrWhiteSpace(fileDialog.FileName))
                {
                    if (!string.IsNullOrWhiteSpace(Product.MainImagePath))
                        File.Delete(Product.MainImagePath);

                    string format = fileDialog.FileName.Split('.').LastOrDefault();
                    string photoPath = $@"Товарышколы\photo_{Product.ID}.{format}";
                    File.Copy(fileDialog.FileName, photoPath, true);
                    Product.MainImagePath = photoPath;
                    ProjectManager.Context.SaveChanges();
                }
                ProjectManager.ShowInformation("Товар успешно " + text);
            }
            catch (Exception ex)
            {
                ProjectManager.ShowError(ex.Message);
            }
            finally
            {
                ProjectManager.MainFrame.Navigate(new ProductPage());
            }

        }

        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!fileDialog.ShowDialog().Value)
                    return;

                if (new FileInfo(fileDialog.FileName).Length > 1024 * 1024 * 2)
                {
                    ProjectManager.ShowWarning("Размер изображения не должен превышать 2 МБ!");
                    fileDialog.FileName = null;
                    return;
                }

                mainImage.Source = new BitmapImage(new Uri(fileDialog.FileName));
            }
            catch (Exception ex)
            {
                ProjectManager.ShowError(ex.Message);
                ProjectManager.MainFrame.Navigate(new ProductPage());
            }
        }

        //private void DeleteImage_Click(object sender, RoutedEventArgs e)
        //{
        //    mainImage.Source = null;
        //    Product.MainImagePath = null;
        //}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                oldMainImagePath = Product.MainImagePath;
                manufacturerComboBox.ItemsSource = ProjectManager.Context.Manufacturer.ToList();
                extraProductList.ItemsSource = ProjectManager.Context.Product.Where(p => p.ID != Product.ID && p.IsActive).ToArray();
                propGrid.DataContext = Product;
                if (Product.ID != 0)
                {
                    costNumeric.Value = Product.Cost;

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        for (int i = 0; i < extraProductList.Items.Count; i++)
                        {
                            ListViewItem lvi = extraProductList.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                            if (lvi != null)
                            {
                                Product currentProduct = extraProductList.Items[i] as Product;
                                if (Product.Product1.Contains(currentProduct))
                                {
                                    ContentPresenter cp = FindVisualChild<ContentPresenter>(lvi);
                                    CheckBox ckbx = (CheckBox)cp.ContentTemplate.FindName("extraItem", cp);
                                    ckbx.IsChecked = true;
                                }
                            }
                        }
                    }), System.Windows.Threading.DispatcherPriority.Background);
                    IDTextbox.IsEnabled = false;
                }
                else
                    IDLabel.Visibility = IDTextbox.Visibility = isActiveLabel.Visibility = isActiveCheckBox.Visibility = Visibility.Collapsed;
                titleBlock.Text = Product.ID == 0 ? "Добавление товара" : "Изменение";
                actionBtn.Content = Product.ID == 0 ? "Добавить" : "Изменить";
            }
            catch (Exception ex)
            {
                ProjectManager.ShowError(ex.Message);
                ProjectManager.MainFrame.Navigate(new ProductPage());
            }
        }

        private void extraProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Product selectedProduct = extraProductList.SelectedItem as Product;
            if (!IsBlocked)
                ProjectManager.MainFrame.Navigate(new ProductAddPage(true) { DataContext = selectedProduct, Product = selectedProduct });
        }
    }
}
