using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HRM.Library.ValueConverter
{
    class MinuteToTimeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {           
            decimal min = System.Convert.ToDecimal(value);
            if (min <= 0)
                return string.Empty;
            int hour = System.Convert.ToInt32(Math.Floor(min / 60));
            int mins = System.Convert.ToInt32(min % 60);

            return ((hour > 0) ? hour.ToString() + " Hrs " : string.Empty) + mins.ToString() + " Mins";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class DateToDayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((DateTime)value).DayOfWeek.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //class CheckInToAttStatusConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
            
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    
}
