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
                ReportName = "Attendance Book Report";
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
                <TextBlock Grid.Row=""1"" FontWeight=""SemiBold"" FontSize=""14"" Text=""Attendance Book Report""/>                
                <StackPanel Orientation=""Horizontal"" Grid.Row=""2"">
                    <TextBlock Width=""70"" Text=""Month :""/>
                    <TextBlock Text=""" + SelectedMonth.MNAME + @"""/>                    
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

            string PresentDayColumn = SETTING.ABSENT_IF_NO_CHECK_OUT ? "CHECKOUT" : "CHECKIN";
            string WeekendCondition = SETTING.COUNT_AS_PRESENT_ON_WEEKEND ? "" : " AND ISNULL(ISWEEKEND, 0) = 0 ";
            string HolidayCondition = SETTING.COUNT_AS_PRESENT_ON_HOLIDAY ? "" : " AND ISNULL(HOLIDAY_ID, 0 ) = 0 ";
            string PresentDayCondition = PresentDayColumn + " IS NOT NULL " + WeekendCondition + HolidayCondition;
            IEnumerable<HajirBookModel> PreReport;
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    string strSql = string.Format(
  @"SELECT ED.ENO, ED.FULLNAME ENAME, ED.DEPARTMENT, [1] d1, [2] d2, [3] d3, [4] d4, [5] d5, [6] d6, [7] d7, [8] d8, [9] d9, [10] d10, 
    [11] d11, [12] d12, [13] d13, [14] d14, [15] d15, [16] d16, [17] d17, [18] d18, [19] d19, [20] d20, [21] d21, [22] d22, [23] d23, [24] d24, [25] d25, [26] d26, 
    [27] d27, [28] d28, ISNULL([29], '') D29, ISNULL([30], '' ) D30, ISNULL([31], '') D31, ISNULL([32], '') D32 FROM
    (
        SELECT MC.ENO, ATTDATE, 
        CASE WHEN " + PresentDayCondition + @" THEN 'P' 
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
(AllDepartments ? string.Empty : " AND (ED.DEPARTMENT_ID = @DEPARTMENT_ID OR ED.DEPARTMENT_ID IN (SELECT DEPARTMENT_ID FROM DEPARTMENT WHERE PARENT = @DEPARTMENT_ID))"), ((SETTING.DEFAULT_CALENDAR == "AD") ? "DATEPART(d,AD)" : "CAST(SUBSTRING(BS,1,2) AS INT)"));
                    DateFunctions.GetFirstAndLastDayOfMonth(SelectedMonth, CurYear, ref _FDate, ref _TDate);
                    PreReport = conn.Query<HajirBookModel>(strSql,
                        new
                        {
                            DEPARTMENT_ID = AllDepartments ? 0 : SelectedDepartment.DEPARTMENT_ID,
                            FDATE = FDate,
                            TDate = TDate
                        });
                    foreach (var model in PreReport)
                    {
                        var days = model.GetType().GetProperties().Where(x => x.Name.StartsWith("d"));
                        model.PresentDays = days.Where(x => x.GetValue(model).ToString() == "P").Count();
                        model.AbsentDays = days.Where(x => x.GetValue(model).ToString() == "A").Count();
                    }
                    if (PreReport.Count() > 0)
                    {
                        ReportSource = new ObservableCollection<dynamic>(PreReport);
                        SetAction(ButtonAction.Selected);
                        ReportParameter = "Month : " + SelectedMonth.MNAME + "     Department : " + ((AllDepartments) ? "All" : SelectedDepartment.DEPARTMENT);
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

    class HajirBookModel
    {
        public int ENO { get; set; }
        public string ENAME { get; set; }
        public string DEPARTMENT { get; set; }
        public string d1 { get; set; }
        public string d2 { get; set; }
        public string d3 { get; set; }
        public string d4 { get; set; }
        public string d5 { get; set; }
        public string d6 { get; set; }
        public string d7 { get; set; }
        public string d8 { get; set; }
        public string d9 { get; set; }
        public string d10 { get; set; }
        public string d11 { get; set; }
        public string d12 { get; set; }
        public string d13 { get; set; }
        public string d14 { get; set; }
        public string d15 { get; set; }
        public string d16 { get; set; }
        public string d17 { get; set; }
        public string d18 { get; set; }
        public string d19 { get; set; }
        public string d20 { get; set; }
        public string d21 { get; set; }
        public string d22 { get; set; }
        public string d23 { get; set; }
        public string d24 { get; set; }
        public string d25 { get; set; }
        public string d26 { get; set; }
        public string d27 { get; set; }
        public string d28 { get; set; }
        public string d29 { get; set; }
        public string d30 { get; set; }
        public string d31 { get; set; }
        public string d32 { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
    }

}
