using System;
using System.Globalization;
using System.Windows.Data;

namespace Robock.Converters
{
    /// <summary>
    ///     Absolute Value to Relative Value Converter
    /// </summary>
    internal class AbsoluteToRelativeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var value = double.Parse(values[0]?.ToString() ?? throw new InvalidOperationException());
            var scale = double.Parse(values[1]?.ToString() ?? throw new InvalidOperationException());

            return value / scale;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}