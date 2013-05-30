namespace Microsoft.WPSync.UI
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class FloatToStarLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new GridLength((double) ((float) value), GridUnitType.Star);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

