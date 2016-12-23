using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HRM.Library.ValueConverter
{
    class HideZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (decimal.Parse(value.ToString()) == 0) ? (object)string.Empty : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.IsNullOrEmpty(value.ToString()) ? 0 : value;
        }
    }

    class TextboxToPasswordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                return string.Empty.PadLeft(value.ToString().Length, '*');
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
        }
    }
}
