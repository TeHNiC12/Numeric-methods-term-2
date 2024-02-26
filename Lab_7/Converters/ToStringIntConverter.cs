using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lab_7.Converters
{
    internal class ToStringIntConverter : IValueConverter
    {
        public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            int x = (int) value;
            return x.ToString();
        }

        public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = (string) value;
            int resultVal;
            if (Int32.TryParse(strValue, out resultVal))
            {
                return resultVal;
            }
            return DependencyProperty.UnsetValue;
            ;
        }
    }
}
