using HRM.ViewModels;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HRM.Library.Helper
{
    class MonthlyAttendanceStyleSelector:StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {

            var row = item as DataRowBase;

            var collection = row.RowData as MonthlyAttendance;


            if (collection.IsAbsent)
                return App.Current.Resources["AbsentRow"] as Style;
            else if(!string.IsNullOrEmpty(collection.LEAVE_NAME))
                return App.Current.Resources["LeaveRow"] as Style;
            else if(!string.IsNullOrEmpty(collection.HOLIDAY_NAME))
                return App.Current.Resources["HolidayRow"] as Style;
            else if(collection.IsWeekend)
                return App.Current.Resources["WeekendRow"] as Style;
            return base.SelectStyle(item, container);

        }
    }
}
