using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
namespace HRM.Library.ValueConverter
{
    class AttendanceColorIndicator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.Black;
            ViewModels.MonthlyAttendance model = value as ViewModels.MonthlyAttendance;
            if (parameter.ToString() == "CHECKIN" && model.CHECKIN != null)
            {
                TimeSpan checkin = model.CHECKIN.Value.TimeOfDay;
                TimeSpan intime = model.INTIME.TimeOfDay;
                TimeSpan ingracetime = model.INGRACETIME.TimeOfDay;
                if (checkin < intime)
                    return Brushes.Green;
                else if (checkin > ingracetime)
                    return Brushes.Red;
            }

            if (parameter.ToString() == "CHECKOUT" && model.CHECKOUT != null)
            {
                TimeSpan checkout = model.CHECKOUT.Value.TimeOfDay;
                TimeSpan outtime = model.OUTTIME.TimeOfDay;
                TimeSpan outgracetime = model.OUTGRACETIME.TimeOfDay;
                if (checkout < outgracetime)
                    return Brushes.Red;
                else if (checkout > outtime)
                    return Brushes.Green;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
