using HRM.Library.AppScopeClasses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
namespace HRM.UI.Utilities
{
    /// <summary>
    /// Interaction logic for wDataRestore.xaml
    /// </summary>
    public partial class wDataBackup : Window
    {
        public wDataBackup()
        {
            InitializeComponent();
            txtDirectory.Text = Environment.CurrentDirectory;
            txtFile.Text = "HRMDB - " + DateTime.Today.ToString("MM-dd-yy");
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn.Content.ToString() == "...")
                {
                    System.Windows.Forms.FolderBrowserDialog ofd = new System.Windows.Forms.FolderBrowserDialog();
                    ofd.ShowDialog();
                    if (!string.IsNullOrEmpty(ofd.SelectedPath))
                        txtDirectory.Text = ofd.SelectedPath;
                }
                else if (btn.Content.ToString() == "Backup")
                {
                    if (!Directory.Exists(txtDirectory.Text))
                    {
                        MessageBox.Show("Selected Directory does not Exists. Please select valid directory and try again", "Data Backup", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        txtDirectory.Focus();
                        return;
                    }
                    if (File.Exists(txtDirectory.Text + "\\" + txtFile.Text + txtExt.Text))
                    {
                        if (MessageBox.Show("A Backup file with same name already Exists. Would you like to overwrite old backup with new one?", "Data Backup", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            File.Delete(txtDirectory.Text + "\\" + txtFile.Text + txtExt.Text);
                        else
                        {
                            txtFile.Focus();
                            return;
                        }
                    }
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Execute(string.Format("Backup Database {0} TO DISK = '{1}'", conn.Database, txtDirectory.Text + "\\" + txtFile.Text + txtExt.Text));
                        MessageBox.Show("Database backup successfully completed.", "Data Backup", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                }
                else
                {
                    this.Close();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Data Backup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
