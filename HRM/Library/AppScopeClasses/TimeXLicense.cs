using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SKGL;
using System.IO;

namespace HRM.Library.AppScopeClasses
{
    static class TimeXLicense
    {
        static Validate ProductLicense;
        public static string[] Devices;
        public static string DeviceSerial = string.Empty;
        static string LicenseKey;
        static string LicensePath = "SOFTWARE\\National IT Service\\TimeX";
        static TimeXLicense()
        {
            ProductLicense = new Validate();
            var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(LicensePath);
            if (key != null)
            {
                DeviceSerial = key.GetValue("DeviceSerial").ToString();
                if (DeviceSerial.Length > 50)
                    Devices = StringCipher.Decrypt(DeviceSerial, "12151990").Split(new string[] { "[#]" }, StringSplitOptions.RemoveEmptyEntries);
                else
                    Devices = new string[] { DeviceSerial };
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

        public static bool DeleteLicense()
        {
            try
            {
                Microsoft.Win32.RegistryKey key;
                key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\National IT Service", true);                
                if (key != null)
                {
                    foreach(string sk in key.GetSubKeyNames())
                        key.DeleteSubKey(sk);
                    
                }
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message == "Requested registry access is not allowed.")
                    System.Windows.MessageBox.Show("TimeX activation failed. Please run the program as administrator.", "TimeX Activation", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
                return false;
            }
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
                if (ex.Message == "Requested registry access is not allowed.")
                    System.Windows.MessageBox.Show("TimeX activation failed. Please run the program as administrator.", "TimeX Activation", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
                return false;
            }
        }
    }

    static class StringCipher
    {
        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}
