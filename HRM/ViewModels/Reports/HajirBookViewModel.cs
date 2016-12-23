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

    class HajirBookViewModel : ReportViewModel
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


        public HajirBookViewModel()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    DepartmentList = conn.Query<Department>("SELECT DEPARTMENT_ID, DEPARTMENT FROM DEPARTMENT");
                    _All_Months = conn.Query<Month>("SELECT MID, MTYPE, MNAME FROM tblMonthNames");
                    MonthList = _All_Months.Where(x => x.MTYPE == SETTING.DEFAULT_CALENDAR);
                    //BranchList = conn.Query<Branch>("SELECT BRANCH_ID, BRANCH_NAME FROM BRANCH");
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
                <TextBlock Grid.Row=""1"" FontWeight=""SemiBold"" FontSize=""14"" Text=""Daily Attendance Report""/>                
                <StackPanel Orientation=""Horizontal"" Grid.Row=""2"">
                    <TextBlock Width=""70"" Text=""Date :""/>
                    <TextBlock Text=""" + TDate.ToString("MM/dd/yyyy") + @"""/>                    
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

                    string strSql =string.Format(
 @"SELECT ED.ENO, ED.FULLNAME ENAME, ED.DEPARTMENT, [1], [2], [3], [4], [5], [6], [7], [8], [9], [10], 
    [11], [12], [13], [14], [15], [16], [17], [18], [19], [20], [21], [22], [23], [24], [25], [26], 
    [27], [28], [29], [30], [31], [32] FROM
    (
        SELECT MC.ENO, ATTDATE, 
        CASE WHEN CHECKIN IS NOT NULL THEN 'P' 
             WHEN LEAVE_ID IS NOT NULL THEN 'L' 
             WHEN HOLIDAY_ID IS NOT NULL THEN 'H' 
             WHEN IsWeekend = 1 THEN 'W'
             ELSE 'A' END [ASTATUS] FROM ATTENDANCE A 
        RIGHT JOIN 
        (
            SELECT ENO, {0} ATTDATE, AD FROM EMPLOYEE, MITICONVERTER WHERE AD BETWEEN @FDate AND @TDate
        ) MC ON A.ATT_DATE = MC.AD AND A.ENO = MC.ENO  WHERE AD BETWEEN @FDATE AND @TDATE
    ) A PIVOT 
    ( 
        MIN(ASTATUS) FOR ATTDATE IN ( [1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12], [13], [14],
        [15], [16], [17], [18], [19], [20], [21], [22], [23], [24], [25], [26], [27], [28], [29], [30], [31], [32])) AS PVT
    JOIN vwEmpDetail ED ON PVT.ENO = ED.ENO WHERE 0 = 0 " + 
(AllDepartments ? string.Empty : " AND ED.DEPARTMENT_ID = @DEPARTMENT_ID"), "DATEPART(d,AD)");
                    DateFunctions.GetFirstAndLastDayOfMonth(SelectedMonth, CurYear, ref _FDate, ref _TDate);
                    PreReport = conn.Query<dynamic>(strSql, 
                        new 
                        { 
                            DEPARTMENT_ID = AllDepartments ? 0 : SelectedDepartment.DEPARTMENT_ID,
                            FDATE = FDate,
                            TDate = TDate
                        });

                    if (PreReport.Count() > 0)
                    {
                        ReportSource = new ObservableCollection<dynamic>(PreReport);
                        SetAction(ButtonAction.Selected);
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
