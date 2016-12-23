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

namespace HRM.UI.Tasks
{
    /// <summary>
    /// Interaction logic for ucMonthlyLeave.xaml
    /// </summary>
    public partial class ucAddLeave : UserControl
    {
        public ucAddLeave()
        {
            InitializeComponent();
            this.DataContext = new AddLeaveViewModel();
        }
    }
}
