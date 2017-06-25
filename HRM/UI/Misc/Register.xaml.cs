using HRM.Library.AppScopeClasses;
using HRM.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using Dapper;
namespace HRM.UI.Misc
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {

        public Register()
        {
            try
            {
                InitializeComponent();
                string SerialPhrase = string.Empty;
                bool GenNewSerialPhrase = false;
                txtDeviceSerial.IsReadOnly = false;
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    var DeviceList = new List<AttDevice>(conn.Query<AttDevice>("SELECT DEVICE_MODEL, DISPLAY_NAME, DEVICE_IP FROM ATT_DEVICE"));
                    if(DeviceList.Count == 0)
                    {
                        txtDeviceSerial.IsReadOnly = false;                        
                        return;
                    }
                    foreach (AttDevice ad in DeviceList)
                    {
                        System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
                        if (p.Send(ad.DEVICE_IP, 1000).Status != System.Net.NetworkInformation.IPStatus.Success)
                        {
                            txtDeviceSerial.IsReadOnly = false;
                            //MessageBox.Show("All Device must be online to Activate Timex. Make sure all devices are active and try again");
                            return;
                        }
                        if (!GetDeviceSerial(ad))
                        {
                            txtDeviceSerial.IsReadOnly = false;
                            //MessageBox.Show("All Device must be online to Activate Timex. Make sure all devices are active and try again");
                            return;
                        }
                    }
                    if (!string.IsNullOrEmpty(TimeXLicense.DeviceSerial))
                    {
                        foreach (AttDevice ad in DeviceList)
                        {
                            if (!TimeXLicense.Devices.Contains(ad.DEVICE_SERIAL))
                            {
                                GenNewSerialPhrase = true;
                                break;
                            }

                        }
                    }
                    else
                    {
                        GenNewSerialPhrase = true;
                    }
                    if (GenNewSerialPhrase)
                    {
                        foreach (AttDevice ad in DeviceList.OrderBy(x => x.DEVICE_SERIAL))
                        {
                            SerialPhrase += ad.DEVICE_SERIAL + "[#]";
                        }
                        txtDeviceSerial.Text = StringCipher.Encrypt(SerialPhrase, "12151990");
                        TimeXLicense.UpdateLicense("AAAA-AAAA-BBBB-CCCC", txtDeviceSerial.Text);
                    }
                    else
                        txtDeviceSerial.Text = TimeXLicense.DeviceSerial;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private bool GetDeviceSerial(AttDevice device)
        {
            string DeviceSerial = string.Empty;
            var zkem = new zkemkeeper.CZKEM();            
            if (zkem.Connect_Net(device.DEVICE_IP, 4370))
            {
                zkem.GetSerialNumber(zkem.MachineNumber, out DeviceSerial);
                device.DEVICE_SERIAL = DeviceSerial;
                zkem.Disconnect();
                return true;
            }
            return false;
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
                if (TimeXLicense.UpdateLicense(LicenseKey, txtDeviceSerial.Text))
                {
                    MessageBox.Show("License successfully update", "Activation", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtDeviceSerial.Text);
            MessageBox.Show("Serial Copied to Clipboard", "Activation", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
