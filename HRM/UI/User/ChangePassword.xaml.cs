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
using Dapper;
using System.Data.SqlClient;
using HRM.Library.AppScopeClasses;
namespace HRM.UI.User
{
    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        public ChangePassword()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtNewPass.Password == txtCPass.Password)
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    if (conn.ExecuteScalar<int>("SELECT COUNT(*) FROM UserManagement WHERE UNAME = @UNAME AND PWD = @PWD", new { UNAME = AppVariables.LoggedUser, PWD = txtOldPass.Password }) == 0)
                    {
                        MessageBox.Show("Old password does not match.", "Change Password", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    conn.Execute("UPDATE UserManagement SET PWD = @PWD WHERE UNAME = @UNAME", new { UNAME = AppVariables.LoggedUser, PWD = txtNewPass.Password });
                    MessageBox.Show("Password successfully changed.", "Change Password", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
