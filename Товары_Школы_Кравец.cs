using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.IO;

namespace Товары_школы_Кравец.Classes
{
    public static class ProjectManager
    {
        public static AM_КравецEntities Context = new AM_КравецEntities();

        public static int UserRole { get; set; }
        public static Frame MainFrame { get; set; }
        public static bool IsNavigationBlocked { get; set; }

        public static MessageBoxResult ShowQuestion(string a)
        {
            return MessageBox.Show(a, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }
        public static MessageBoxResult ShowError(string message)
        {
            return MessageBox.Show(message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Question);
        }
        public static MessageBoxResult ShowWarning(string message)
        {
            return MessageBox.Show(message, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Question);
        }
        public static MessageBoxResult ShowInformation(string message)
        {
            return MessageBox.Show(message, "Уведомление", MessageBoxButton.OK, MessageBoxImage.Question);
        }

    }

    public class FromBoolToStringActiveConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return "В наличии";
            else
                return "Нет в наличии";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class FromStringToImageConverter : IValueConverter
    {
        private string imageDirectory = Directory.GetCurrentDirectory();
        public string ImageDirectory
        {
            get { return imageDirectory; }
            set { imageDirectory = value; }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((string)value != null)
            {
                string imagePath = Path.Combine(ImageDirectory, (string)value);
                if (File.Exists(imagePath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bitmap.EndInit();
                    return bitmap;
                }
                return null;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("The method or operation is not implemented.");
        }

    }
}
