using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lab_8.Converters
{
    public class ToStringRoundingConverter : IValueConverter
    {
        public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            double x = (double) value;
            return Math.Round(x, 5).ToString();
        }

        public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = (string) value;
            double resultVal;
            if (Double.TryParse(strValue, out resultVal))
            {
                return resultVal;
            }
            return DependencyProperty.UnsetValue;
            ;
        }
    }
}
