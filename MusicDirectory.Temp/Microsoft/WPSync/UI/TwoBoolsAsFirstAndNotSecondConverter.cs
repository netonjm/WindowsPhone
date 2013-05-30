namespace Microsoft.WPSync.UI
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows.Data;

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Bools")]
    public class TwoBoolsAsFirstAndNotSecondConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((values == null) || (values.Length != 2))
            {
                return false;
            }
            if (!(values[0] is bool))
            {
                return false;
            }
            if (!(values[1] is bool))
            {
                return false;
            }
            bool flag = (bool) values[0];
            bool flag2 = (bool) values[1];
            return (flag && !flag2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

