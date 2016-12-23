using HRM.Library.AppScopeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HRM.UI.Misc
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        string LicenseKey { get { return txtKey1.Text + "-" + txtKey2.Text + "-" + txtKey3.Text + "-" + txtKey4.Text; } }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tBox = (sender as TextBox);
            if (tBox.CaretIndex == 5)
            {
                switch (tBox.Name)
                {
                    case "txtKey1":
                        txtKey2.Focus();
                        break;
                    case "txtKey2":
                        txtKey3.Focus();
                        break;
                    case "txtKey3":
                        txtKey4.Focus();
                        break;
                }
            }
            if (tBox.Text.Length > 5)
                tBox.Text = tBox.Text.Substring(0, 5);
            //imgValidate.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(GlobalClass.ValidateLicense(txtKey.Text) ? "pack://application:,,,/Images/Yes.png" : "pack://application:,,,/Images/Cancel.png"));
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (TimeXLicense.ValidateLicense(LicenseKey, txtDeviceSerial.Text))
            {
                TimeXLicense.UpdateLicense(LicenseKey, txtDeviceSerial.Text);
                MessageBox.Show("License successfully update", "Activation", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
