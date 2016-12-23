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
    /// Interaction logic for ucCompany.xaml
    /// </summary>
    public partial class ucBranch : UserControl
    {
        public ucBranch()
        {
            InitializeComponent();
            this.DataContext = new BranchViewModel();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HRM.Library.AppScopeClasses.KeyHandler.NumericOnly(e);
        }
    }
}
