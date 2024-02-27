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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isExit = false;
        public MainWindow()
        {
            InitializeComponent();
            GetUserRole();
            if (ProjectManager.UserRole != 0)
                UserName.Content = $"User: { ProjectManager.Context.UserLogin.Where(u => u.UserRole == ProjectManager.UserRole).FirstOrDefault().Login}";
            else UserName.Content = "User: Гость";
            ProjectManager.MainFrame = mainFrame;
            ProjectManager.MainFrame.Navigate(new ProductsViewPage());
        }

        private void GetUserRole()
        {
            switch (ProjectManager.UserRole)
            {
                case 0: SellJournalButton.Visibility = Visibility.Collapsed; break;
                case 1: SellJournalButton.Visibility = Visibility.Visible; break;
                case 2: SellJournalButton.Visibility = Visibility.Visible; break;
                case 3: SellJournalButton.Visibility = Visibility.Collapsed; break;
            }
        }

        private void ClosingMainWIndow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isExit)
                return;

            MessageBoxResult result = ProjectManager.ShowQuestion("Вы действительно хотите закрыть программу?");
            if (result == MessageBoxResult.No)
                e.Cancel = true;
            else Application.Current.Shutdown();
        }

        private void Loading(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ProjectManager.Context.Database.Exists())
                    throw new Exception("Не удалось подключиться к базе данных." +
                        "\nДальнейшая работа приложения невозможна.");

                mainFrame.Navigate(new ProductsViewPage());
                ProjectManager.MainFrame = mainFrame;
            }
            catch (Exception ex)
            {
                isExit = true;
                ProjectManager.ShowError(ex.Message);
                Application.Current.Shutdown();
            }
        }

        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            ProjectManager.MainFrame.Navigate(new ProductPage());
        }

        private void SellJournalButton_Click(object sender, RoutedEventArgs e)
        {
            ProjectManager.MainFrame.Navigate(new SellJournalPage());
        }

        private void ProductViewButton_Click(object sender, RoutedEventArgs e)
        {
            ProjectManager.MainFrame.Navigate(new ProductsViewPage());
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            AuthWindow authWindow = new AuthWindow();
            isExit = true;
            Close();
            authWindow.Show();
        }
    }
}
