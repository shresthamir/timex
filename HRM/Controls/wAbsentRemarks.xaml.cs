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

namespace HRM.Controls
{
    /// <summary>
    /// Interaction logic for wAbsentRemarks.xaml
    /// </summary>
    public partial class wAbsentRemarks : Window
    {
        public wAbsentRemarks()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Content.Equals("Ok"))
                this.DialogResult = true;
            this.Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                this.DialogResult = true;
                this.Close();
            }
            else if(e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
