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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Dapper;
using HRM.Models;
namespace HRM.UI.User
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((sender as Button).Content.ToString() == "Login")
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        IEnumerable<UserManagement> users = conn.Query<UserManagement>("SELECT * FROM UserManagement WHERE UNAME = @UNAME AND PWD = @PWD", new { UNAME = txtUserName.Text, PWD = txtPassword.Password });
                        if (users.Count() <= 0)
                        {
                            MessageBox.Show("Invalid Username or password", "Login", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        UserManagement user = users.First();
                        if (user.INACTIVE)
                        {
                            MessageBox.Show("You no longer have access to TimeX Application. Please contact system administrator", "Login", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        AppVariables.LoggedUser = user.UNAME;
                        new MainWindow(user).Show();
                        this.Close();
                    }
                }
                else
                {
                    App.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(GetRootEx(ex).Message, "TimeX", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        Exception GetRootEx(Exception ex)
        {
            while (ex.InnerException != null)
            {
                return GetRootEx(ex.InnerException);
            }
            return ex;
        }
    }
}
