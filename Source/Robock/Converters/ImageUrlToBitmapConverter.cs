using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Robock.Converters
{
    /// <summary>
    ///     for avoiding memory leak of Image control
    /// </summary>
    internal class ImageUrlToBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = value as string;
            if (string.IsNullOrWhiteSpace(path))
                return null;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.None;
            bitmap.UriSource = new Uri(path);
            bitmap.DecodePixelWidth = 960;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}