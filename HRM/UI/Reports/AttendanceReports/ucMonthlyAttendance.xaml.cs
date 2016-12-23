using HRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HRM.UI.Reports.AttendanceReports
{
    /// <summary>
    /// Interaction logic for ucLeaveStatus.xaml
    /// </summary>
    public partial class ucMonthlyAttendance : UserControl
    {
        public ucMonthlyAttendance()
        {
            InitializeComponent();
            this.DataContext = new HRM.ViewModels.MonthlyAttandanceViewModel() {sfGrid = sfgrid };
            
        }

        private void sfgrid_QueryCoveredRange(object sender, Syncfusion.UI.Xaml.Grid.GridQueryCoveredRangeEventArgs e)
        {
            if (e.Record is MonthlyAttendance)
            {
                if (e.RowColumnIndex.ColumnIndex == 2)
                {
                    MonthlyAttendance ma = e.Record as MonthlyAttendance;
                    if (ma.CHECKIN == null)
                    {   
                        e.Range = new Syncfusion.UI.Xaml.Grid.CoveredCellInfo(2, 10, e.RowColumnIndex.RowIndex, e.RowColumnIndex.RowIndex);   
                        
                        e.GridColumn.TextAlignment = TextAlignment.Center;
                        e.Handled = true;
                    }
                }

            }
        }

        private void sfgrid_ResizingColumns(object sender, Syncfusion.UI.Xaml.Grid.ResizingColumnsEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                e.Width = 0.1;
                e.Cancel = true;
            }
        }
    }
}
