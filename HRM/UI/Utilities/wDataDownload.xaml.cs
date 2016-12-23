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
using System.Windows.Shapes;

namespace HRM.UI.Utilities
{
    /// <summary>
    /// Interaction logic for wDataDownload.xaml
    /// </summary>
    public partial class wDataDownload : Window
    {
        public wDataDownload()
        {
            InitializeComponent();
            this.DataContext = new DataDownloadViewModel();
        }
    }
}
