using HRM.Library.AppScopeClasses;
using HRM.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Windows;
namespace HRM.ViewModels
{

    class EmployeeListViewModel : ReportViewModel
    {
        private bool _AllBranch = true;
        private bool _AllStatus = true;
        private IEnumerable<Branch> _BranchList;
        private IEnumerable<string> _StatusList;
        private Branch _SelectedBranch;
        private string _SelectedStatus;

        public bool AllBranch { get { return _AllBranch; } set { _AllBranch = value; OnPropertyChanged("AllBranch"); } }
        public bool AllStatus { get { return _AllStatus; } set { _AllStatus = value; OnPropertyChanged("AllStatus"); } }

        public Branch SelectedBranch { get { return _SelectedBranch; } set { _SelectedBranch = value; OnPropertyChanged("SelectedBranch"); } }
        public string SelectedStatus { get { return _SelectedStatus; } set { _SelectedStatus = value; OnPropertyChanged("SelectedStatus"); } }

        public IEnumerable<Branch> BranchList { get { return _BranchList; } set { _BranchList = value; OnPropertyChanged("BranchList"); } }
        public IEnumerable<string> StatusList { get { return _StatusList; } set { _StatusList = value; OnPropertyChanged("StatusList"); } }


        public EmployeeListViewModel()
        {
            try
            {
                ReportName = "Employye List";
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    DepartmentList = conn.Query<Department>("SELECT DEPARTMENT_ID, DEPARTMENT FROM DEPARTMENT");
                    BranchList = conn.Query<Branch>("SELECT BRANCH_ID, BRANCH_NAME FROM BRANCH");
                    StatusList = conn.Query<string>("SELECT STATUS FROM WORKINGSTATUS");             
                }
                LoadData = new Library.Helpers.RelayCommand(LoadReport);
                PrintCommand = new Library.Helpers.RelayCommand(PrintReport);
            }
            catch (Exception Ex)
            {
                ShowError(Ex.Message);
            }
        }

        private void PrintReport(object obj)
        {
            string HeaderTemplate =
@"<ResourceDictionary xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
        <DataTemplate x:Key=""HeaderTemplate"">
            <Grid HorizontalAlignment=""Center"">
                <Grid.RowDefinitions>
                    <RowDefinition Height=""25""/>
                    <RowDefinition Height=""25""/>
                    <RowDefinition Height=""20""/>
                </Grid.RowDefinitions>
                <TextBlock FontWeight=""SemiBold"" FontSize=""14""  Text=""" + AppVariables.CompanyInfo.COMPANY_NAME + @"""/>
                <TextBlock Grid.Row=""1"" FontWeight=""SemiBold"" FontSize=""14"" Text=""Employee List""/>                
                <StackPanel Orientation=""Horizontal"" Grid.Row=""2"">
                    <TextBlock Width=""70"" Margin=""20 0 0 0"" Text=""Department :""/>
                    <TextBlock Text=""" + ((AllDepartments) ? "All" : SelectedDepartment.DEPARTMENT) + @"""/>                    
                </StackPanel>
            </Grid>  
        </DataTemplate>
</ResourceDictionary>
";

            sfGrid.PrintSettings.PrintPageHeaderTemplate = (XamlReader.Parse(HeaderTemplate) as ResourceDictionary)["HeaderTemplate"] as DataTemplate;
            sfGrid.Print();

            //System.IO.Stream st = new System.IO.MemoryStream();
            //PdfExportingOptions options = new PdfExportingOptions();
            //options.ExportMergedCells = true;            
            //var document = sfGrid.ExportToPdf(options);
            //document.Save(st);

            //new HRM.UI.Reports.PdfViewer(st).ShowDialog();

        }

        private void LoadReport(object obj)
        {            
            IEnumerable<dynamic> PreReport;
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {

                    string strSql =
 @"SELECT E.ENO, ECODE, TITLE + ' ' + FULLNAME ENAME, BRANCH_NAME, DEPARTMENT, DESIGNATION, MARITAL_STATUS, CMODE, [STATUS]  FROM Employee E
JOIN EMPLOYEE_DETAIL ED ON E.ENO = ED.ENO
JOIN BRANCH B ON ED.BRANCH_ID = B.BRANCH_ID
JOIN DEPARTMENT D ON ED.DEPARTMENT_ID = D.DEPARTMENT_ID
JOIN DESIGNATION DS ON ED.DESIGNATION_ID = DS.DESIGNATION_ID
JOIN (SELECT ENO, MAX(EMP_TRANID) TRANID FROM EMPLOYEE_DETAIL GROUP BY ENO) EDD ON ED.EMP_TRANID = EDD.TRANID WHERE 0 = 0 " + 
(AllDepartments ? string.Empty : " AND ED.DEPARTMENT_ID = @DEPARTMENT_ID") +
(AllBranch ? string.Empty : " AND ED.BRANCH_ID = @BRANCH_ID") +
(AllStatus ? string.Empty : " AND ED.[STATUS] = @STATUS");
                    PreReport = conn.Query<dynamic>(strSql, 
                        new 
                        { 
                            DEPARTMENT_ID = AllDepartments ? 0 : SelectedDepartment.DEPARTMENT_ID,
                            BRANCH_ID = AllBranch ? 0 : SelectedBranch.BRANCH_ID,
                            STATUS = SelectedStatus, 
                        });

                    if (PreReport.Count() > 0)
                    {
                        ReportSource = new ObservableCollection<dynamic>(PreReport);
                        SetAction(ButtonAction.Selected);
                        ReportParameter = "Department : " + ((AllDepartments) ? "All" : SelectedDepartment.DEPARTMENT);
                    }
                    else
                        ShowWarning("Record does not exists for entered criteria");
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }
}
