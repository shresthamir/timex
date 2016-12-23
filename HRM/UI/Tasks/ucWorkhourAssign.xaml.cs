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
    /// Interaction logic for ucWorkhourAssign.xaml
    /// </summary>
    public partial class ucWorkhourAssign : UserControl
    {
        public ucWorkhourAssign()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.WorkHourAssignViewModel();
        }
    }
}
