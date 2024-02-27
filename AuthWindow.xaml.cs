using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Товары_школы_Кравец.Classes;

namespace Товары_школы_Кравец
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public bool AuthExit = true;

        string captchaValues = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890";
        Random random = new Random();
        string captchaTrueValue = "";

        public AuthWindow()
        {
            InitializeComponent();
        }

        public string CaptchaTextMaker(int lenght)
        {
            return string.Concat(Enumerable.Range(0, lenght).Select(_ => captchaValues[random.Next(captchaValues.Length)]));
        }

        public void CreateCaptcha()
        {
            Bitmap bitmap = new Bitmap(100, 30);

            Graphics _graphics = Graphics.FromImage(bitmap);
            _graphics.Clear(System.Drawing.Color.Gray);
            captchaTrueValue = CaptchaTextMaker(4);

            for (int i = 0; i < 4; i++)
                _graphics.DrawString(captchaTrueValue[i].ToString(), new Font("Arial", 18), System.Drawing.Brushes.Green, random.Next(0 + 15 * i, 15 + 15*i), random.Next(-5, 5 ));

            _graphics.DrawLine(Pens.Black,
              new System.Drawing.Point(0, 0),
              new System.Drawing.Point(99, 29));
            _graphics.DrawLine(Pens.Black,
                       new System.Drawing.Point(0, 29),
                       new System.Drawing.Point(99, 0));

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                captcha1.Source = bitmapImage;
            }
        }

        private void LogInClick(object sender, RoutedEventArgs e)
        {
            if (loginTxt.Text == "" || passwordTxt.Password == "")
            {
                ProjectManager.ShowError("Введите данные!");
                if (captchaTxt.Visibility == Visibility.Visible)
                    CreateCaptcha();
                return;
            }
            if (captchaTxt.Visibility == Visibility.Visible)
            {
                if (captchaTxt.Text != captchaTrueValue)
                {
                    ProjectManager.ShowError("Неверные символы!");
                    CreateCaptcha();
                    return;
                }
            }
            var userRole = ProjectManager.Context.UserLogin.Where(u => u.Login == loginTxt.Text && u.Password == passwordTxt.Password).FirstOrDefault();
            if (userRole == null)
            {
                ProjectManager.ShowError("Неверные данные!");
                if (captchaTxt.Visibility == Visibility.Visible)
                {
                    ProjectManager.ShowError("Временная блокировка на 10 секунд");
                    Thread.Sleep(10000);
                }
                captcha1.Visibility = Visibility.Visible;
                captcha2.Visibility = Visibility.Visible;
                captchaTxt.Visibility = Visibility.Visible;
                CreateCaptcha();

                return;
            }
            ProjectManager.UserRole = userRole.UserRole;
            AuthExit = false;
            Close();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void GuestLogIn(object sender, RoutedEventArgs e)
        {
            ProjectManager.UserRole = 0;
            AuthExit = false;
            Close();
            MainWindow mainWindow = new MainWindow();
            ProjectManager.MainFrame.Navigate(new ProductPage());
            mainWindow.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.isExit = true;
            if (AuthExit)
                Application.Current.Shutdown();
        }
    }
}
