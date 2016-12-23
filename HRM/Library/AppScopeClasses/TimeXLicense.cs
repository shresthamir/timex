using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SKGL;
namespace HRM.Library.AppScopeClasses
{
    static class TimeXLicense
    {
        static Validate ProductLicense;
        static string DeviceSerial = "1234567890";
        static string LicenseKey;
        static string LicensePath = "SOFTWARE\\National IT Service\\TimeX";
        static TimeXLicense()
        {
            ProductLicense = new Validate();
            var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(LicensePath);
            if (key != null)
            {
                DeviceSerial = key.GetValue("DeviceSerial").ToString();
                LicenseKey = key.GetValue("License").ToString();
                ProductLicense.Key = LicenseKey;
                ProductLicense.secretPhase = DeviceSerial;
                ValidateLicense();
            }
        }

        public static bool ValidateLicense()
        {
            ProductLicense.secretPhase = DeviceSerial;
            return ProductLicense.IsValid;
        }
        public static bool IsLicenseExpired()
        {
            return ProductLicense.IsExpired;
        }

        public static bool ValidateLicense(string _Key, string _DeviceSerial)
        {
            Validate val = new Validate();
            val.Key = _Key;
            val.secretPhase = _DeviceSerial;
            return val.IsValid;
        }

        public static int RemDays()
        {
            return ProductLicense.DaysLeft;
        }



        public static bool UpdateLicense(string _NewLicense, string _DeviceSerial)
        {
            try
            {
                Microsoft.Win32.RegistryKey key;
                key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(LicensePath, true);
                if (key == null)
                {
                    key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(LicensePath, Microsoft.Win32.RegistryKeyPermissionCheck.ReadWriteSubTree);
                }
                key.SetValue("License", _NewLicense);
                key.SetValue("DeviceSerial", _DeviceSerial);                
                LicenseKey = ProductLicense.Key = _NewLicense;
                DeviceSerial = ProductLicense.secretPhase = _DeviceSerial;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
