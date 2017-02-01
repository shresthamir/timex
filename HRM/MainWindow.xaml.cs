using HRM.Library.AppScopeClasses;
using HRM.Library.BaseClasses;
using HRM.Models;
using HRM.UI.Master;
using HRM.UI.Reports.AttendanceReports;
using HRM.UI.Reports.LeaveReports;
using HRM.UI.User;
using HRM.UI.Utilities;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;
using Dapper;
using HRM.UI.Reports;
namespace HRM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsAppRunningInServer;
        public MainWindow()
        {
            InitializeComponent();
            var version =System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Title = "TimeX - Attendance & Leave Management Software v " + version.Major + "."  + version.Minor + "." + version.Build;            
            this.Closing+=MainWindow_Closing;
        }

        internal MainWindow(UserManagement user)
        {
            InitializeComponent();
            this.Closing+=MainWindow_Closing;
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Title = "TimeX - Attendance & Leave Management Software v " + version.Major + "." + version.Minor + "." + version.Build;
            this.MainMenu.DataContext = user;
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    IsAppRunningInServer = (Environment.MachineName == conn.ExecuteScalar<string>("SELECT SERVERPROPERTY('MachineName') "));
                }
            }
            catch (Exception ex)
            {
                IsAppRunningInServer = false;
            }
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Do you want to close TimeX Attendance Software?", "TimeX", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
            else
                Application.Current.Shutdown();
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Title = (sender as MenuItem).Header.ToString();
            Binding b = new Binding();
            switch ((sender as MenuItem).Header.ToString())
            {
                case "Leave Status Report":
                    la.Content = new ucLeaveStatus();
                    break;
                case "Leave Statement":
                    la.Content = new ucLeaveLedger();
                    break;
                case "Monthly Attendance - Detail":
                    la.Content = new ucMonthlyAttendance();
                    break;
                case "Monthly Attendance - Summary":
                    la.Content = new ucMonthlyAttendanceSummary();
                    break;
                case "Attendance Book Report":
                    la.Content = new ucHajirBook();
                    break;
                case "Daily Attendance Report":
                    la.Content = new ucDailyAttendance();
                    break;
                case "Daily Absent Report":
                    la.Content = new ucDailyAbsent();
                    break;
                case "Employee List":
                    la.Content = new ucEmployeeList();
                    break;
            }
            //b.Converter = new Library.Converters.LanguageConverter();
            //b.ConverterParameter = (sender as MenuItem).Header.ToString();
            //SetBinding(LayoutAnchorable.TitleProperty, b);
            la.PropertyChanged += la_PropertyChanged;
            la.IsSelected = true;
            LayDocPane.Children.Add(la);
        }

        private void Master_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Title = (sender as MenuItem).Header.ToString();
            Binding b = new Binding();
            switch ((sender as MenuItem).Header.ToString())
            {
                case "Branch Setup":
                    la.Content = new ucBranch();
                    break;
                case "Holiday Setup":
                    la.Content = new ucHoliday();
                    break;
                case "Leave Setup":
                    la.Content = new ucLeave();
                    break;
                case "Departments":
                    la.Content = new ucDepartment();
                    break;
                case "Designation":
                    la.Content = new ucDesignation();
                    break;
                case "Working Hour":
                    la.Content = new ucWorkHour();
                    break;
            }
            //b.Converter = new Library.Converters.LanguageConverter();
            //b.ConverterParameter = (sender as MenuItem).Header.ToString();
            //SetBinding(LayoutAnchorable.TitleProperty, b);
            la.PropertyChanged += la_PropertyChanged;
            la.IsSelected = true;
            LayDocPane.Children.Add(la);
        }

        private void Task_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Title = (sender as MenuItem).Header.ToString();
            Binding b = new Binding();
            switch ((sender as MenuItem).Header.ToString())
            {
                case "Employee Registration":
                    la.Content = new UI.Master.EmployeeRegistration();
                    break;
                case "Manual Attendance":
                    la.Content = new UI.Tasks.ucForceAttendance();
                    break;
                case "Monthly Leave":
                    la.Content = new UI.Tasks.ucMonthlyLeave();
                    break;
                case "Add Leave":
                    la.Content = new UI.Tasks.ucAddLeave();
                    break;
                case "Leave Application":
                    la.Content = new UI.Tasks.ucLeaveApplication();
                    break;
                case "Assign Workhour":
                    la.Content = new UI.Tasks.ucWorkhourAssign();
                    break;
            }
            //b.Converter = new Library.Converters.LanguageConverter();
            //b.ConverterParameter = (sender as MenuItem).Header.ToString();
            //SetBinding(LayoutAnchorable.TitleProperty, b);
            la.PropertyChanged += la_PropertyChanged;
            la.IsSelected = true;
            LayDocPane.Children.Add(la);
        }


        private void Utilities_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Title = (sender as MenuItem).Header.ToString();
            Binding b = new Binding();
            switch ((sender as MenuItem).Header.ToString())
            {
                case "Company Info":
                    la.Content = new ucCompany();
                    break;
                case "Download Attendance Data":
                    new wDataDownload().Show();
                    return;
                case "Database Backup":
                    if (IsAppRunningInServer)
                        new wDataBackup().Show();
                    return;
                case "Database Restore":
                    if (IsAppRunningInServer)
                        new wDataRestore().Show();
                    return;
                case "Device Setting":
                    new wDeviceSetting().Show();
                    return;

            }
            //b.Converter = new Library.Converters.LanguageConverter();
            //b.ConverterParameter = (sender as MenuItem).Header.ToString();
            //SetBinding(LayoutAnchorable.TitleProperty, b);

            la.PropertyChanged += la_PropertyChanged;
            la.IsSelected = true;
            LayDocPane.Children.Add(la);
        }

        void la_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            LayoutAnchorable la = (sender as LayoutAnchorable);
            if (e.PropertyName == "IsSelected" && la.IsSelected)
            {
                this.DataContext = (la.Content as FrameworkElement).DataContext;
            }
            else if (e.PropertyName == "IsActive" && la.IsActive)
            {
                this.DataContext = (la.Content as FrameworkElement).DataContext;
            }
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {

            LayoutAnchorable la = new LayoutAnchorable();
            la.Title = (sender as MenuItem).Header.ToString();
            switch ((sender as MenuItem).Header.ToString())
            {
                case "User Management":
                    la.Content = new ucUserManagement();
                    break;
                case "Log Out":
                    AppVariables.LoggedUser = null;
                    this.Close();
                    new UI.User.Login().Show();
                    return;
                case "Exit":
                    this.Close();
                    return;
                case "Change Password":
                    new UI.User.ChangePassword().ShowDialog();
                    return;
            }
            la.PropertyChanged += la_PropertyChanged;
            la.IsSelected = true;
            LayDocPane.Children.Add(la);
        }

        private void LayDocPane_ChildrenCollectionChanged(object sender, EventArgs e)
        {
            if (((LayoutDocumentPane)sender).Children.Count > 0)
            {
            }
            else
            {
            }
        }

        private void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            LayoutContent la = LayDocPane.Children.FirstOrDefault(x => x.IsSelected);
            if (la == null || la.Content == null)
                return;
            RootViewModel bvm = (RootViewModel)(la.Content as FrameworkElement).DataContext;

            switch ((sender as Button).Tag.ToString())
            {
                case "NEW":
                    bvm.NewCommand.Execute(null);
                    break;
                case "EDIT":
                    bvm.EditCommand.Execute(null);
                    break;
                case "DELETE":
                    bvm.DeleteCommand.Execute(null);
                    break;
                case "SAVE":
                    bvm.SaveCommand.Execute(null);
                    break;
                case "UNDO":
                    bvm.ClearCommand.Execute(null);
                    break;
                case "PRINT":
                    bvm.PrintCommand.Execute(null);
                    break;
                case "EXPORT":
                    bvm.ExportCommand.Execute(null);
                    break;
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

    }
}
