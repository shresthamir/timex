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

namespace HRM.UI.Master
{
    /// <summary>
    /// Interaction logic for ucLeave.xaml
    /// </summary>
    public partial class ucLeave : UserControl
    {
        public ucLeave()
        {
            InitializeComponent();
            this.DataContext = new LeaveViewModel();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Library.AppScopeClasses.KeyHandler.NumericOnly(e);
        }
    }
}
