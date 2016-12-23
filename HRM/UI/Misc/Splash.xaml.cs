using HRM.Library.AppScopeClasses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Dapper;
namespace HRM.UI.Misc
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        Timer t;
        public Splash()
        {
            InitializeComponent();
            this.Loaded += Splash_Loaded;

        }

        private bool CheckConnection()
        {
            try
            {
                SqlConnectionStringBuilder sbr = new SqlConnectionStringBuilder(AppVariables.ConnectionString);
                sbr.ConnectTimeout = 5;
                using (SqlConnection conn = new SqlConnection(sbr.ConnectionString))
                    conn.Open();
                return true;
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Connection Failed", "TimeX", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        void Splash_Loaded(object sender, RoutedEventArgs e)
        {

            t = new Timer();
            t.Interval = 3000;
            t.Tick += t_Tick;
            t.Start();

        }

        void t_Tick(object sender, EventArgs e)
        {
            t.Stop();
            if (!TimeXLicense.ValidateLicense())
            {
                System.Windows.MessageBox.Show("Your License for TimeX Attendance Software has Expired. Please contact your software vendor for more information.", "TimeX", MessageBoxButton.OK, MessageBoxImage.Information);
                new About().Show();
                this.Close();                
                return;
            }
            if (CheckConnection())
            {
                try
                {
                    //using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    //{
                    //    if (conn.ExecuteScalar<String>("SELECT TOP 1 COMPANY_NAME FROM COMPANY") != "ALKA HOSPITAL")
                    //    {
                    //        System.Windows.MessageBox.Show("Invalid License. Please contact your system provider. 9851155543 Suraj Neupane");
                    //        this.Close();
                    //        return;
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "nuTimeX", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                new UI.User.Login().Show();
                this.Close();
            }
            else
                App.Current.Shutdown();
        }

    }
}
