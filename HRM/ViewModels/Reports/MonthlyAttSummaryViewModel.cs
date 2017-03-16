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

    class MonthlyAttSummaryViewModel : ReportViewModel
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


        public MonthlyAttSummaryViewModel()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    ReportName = "Monthly Attendance Summary Report";
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
            <Grid >
                <Grid.RowDefinitions>
                    <RowDefinition Height=""25""/>
                    <RowDefinition Height=""25""/>
                    <RowDefinition Height=""20""/>
                </Grid.RowDefinitions>
                <TextBlock HorizontalAlignment=""Center"" FontWeight=""SemiBold"" FontSize=""14""  Text=""" + AppVariables.CompanyInfo.COMPANY_NAME + @"""/>
                <TextBlock HorizontalAlignment=""Center"" Grid.Row=""1"" FontWeight=""SemiBold"" FontSize=""14"" Text=""Monthly Attendance Summary Report""/>                
                <StackPanel HorizontalAlignment=""Center"" Orientation=""Horizontal"" Grid.Row=""2"">
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
            //sfGrid.ShowPrintPreview();
            sfGrid.PrintSettings.PrintPageOrientation = Syncfusion.UI.Xaml.Grid.PrintOrientation.Landscape;
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
            string PresentDayCondition = "SUM(CASE WHEN 0 = 0" + WeekendCondition + HolidayCondition + " AND " + PresentDayColumn + " IS NOT NULL THEN 1 ELSE 0 END)";
            IEnumerable<dynamic> PreReport;
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {

                    string strSql =
 @"SELECT A.ENO, ED.FULLNAME ENAME, ED.DEPARTMENT, TotalDays , Weekends, Holidays, PaidLeaves, UnPaidLeaves, TotalDays - (Weekends + Holidays) WorkingDays, PresentDays, 
Weekends + Holidays + PaidLeaves + PresentDays PayableDays,
TotalDays - (Weekends + Holidays + FLOOR(PaidLeaves) + PresentDays) AbsentDays,  PresentOnNonWorkingDay, TotalDuration FROM
(
	SELECT A.ENO, 
	(
		SELECT COUNT(" + PresentDayColumn + @") FROM ATTENDANCE WHERE (ATT_DATE BETWEEN @FDATE AND @TDATE) AND ENO = A.ENO AND 
		(
			ISNULL(IsWeekend,0) > 0 OR ISNULL(LEAVE_ID, 0) > 0 OR ISNULL(HOLIDAY_ID, 0) > 0
		)
	) PresentOnNonWorkingDay, DATEDIFF(d,@FDATE,@TDATE) + 1 TotalDays, " + PresentDayCondition + @" PresentDays, ISNULL(SUM(CAST(IsWeekend AS INT)), 0) Weekends, Count(Holiday_ID) Holidays, 
	ISNULL(AVG(PaidLeaves),0) PaidLeaves, ISNULL(AVG(UnPaidLeaves),0) UnPaidLeaves, 
    SUM(CASE WHEN CHECKIN IS NOT NULL AND CHECKOUT IS NOT NULL THEN DATEDIFF(MI,CHECKIN,CHECKOUT) ELSE 0 END) TotalDuration FROM ATTENDANCE A 
	LEFT JOIN 
	(
		SELECT DISTINCT ENO, L.LEAVE_ID, 
		(
			SELECT SUM(Cr) FROM LEAVE_LEDGER WHERE ENO = LL.ENO AND LEAVE_ID = LL.LEAVE_ID AND Cr>0 AND APPLIED_DATE BETWEEN @FDATE AND @TDATE  GROUP BY ENO, LEAVE_ID
		) PaidLeaves  FROM LEAVE_LEDGER LL JOIN LEAVES L ON LL.LEAVE_ID = L.LEAVE_ID WHERE ISPAIDLEAVE = 1 AND Cr>0 AND APPLIED_DATE BETWEEN @FDATE AND @TDATE
	) PL ON PL.LEAVE_ID = A.LEAVE_ID AND A.ENO = PL.ENO
	LEFT JOIN 
	(	
		SELECT DISTINCT ENO, L.LEAVE_ID, 
		(
			SELECT SUM(Cr) FROM LEAVE_LEDGER WHERE ENO = LL.ENO AND LEAVE_ID = LL.LEAVE_ID AND Cr>0 AND APPLIED_DATE BETWEEN @FDATE AND @TDATE  GROUP BY ENO, LEAVE_ID
		) UnPaidLeaves  FROM LEAVE_LEDGER LL JOIN LEAVES L ON LL.LEAVE_ID = L.LEAVE_ID WHERE ISPAIDLEAVE = 0 AND Cr>0 AND APPLIED_DATE BETWEEN @FDATE AND @TDATE
	) UL ON UL.LEAVE_ID = A.LEAVE_ID AND A.ENO = UL.ENO
	WHERE ATT_DATE BETWEEN @FDATE AND @TDATE GROUP BY A.ENO
) A JOIN vwEmpDetail ED ON A.ENO = ED.ENO WHERE 0 = 0 " +
(AllDepartments ? string.Empty : " AND (ED.DEPARTMENT_ID = @DEPARTMENT_ID OR ED.DEPARTMENT_ID IN (SELECT DEPARTMENT_ID FROM DEPARTMENT WHERE PARENT = @DEPARTMENT_ID))");
                    DateFunctions.GetFirstAndLastDayOfMonth(SelectedMonth, CurYear, ref _FDate, ref _TDate);
                    PreReport = conn.Query<MonthlyAttSummary>(strSql,
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

    class MonthlyAttSummary
    {
        public int ENO { get; set; }
        public string ENAME { get; set; }
        public string DEPARTMENT { get; set; }
        public int TotalDays { get; set; }
        public int Weekends { get; set; }
        public int Holidays { get; set; }
        public double PaidLeaves { get; set; }
        public double UnPaidLeaves { get; set; }
        public int WorkingDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int PresentOnNonWorkingDay { get; set; }
        public int TotalDuration { get; set; }
        public double PayableDays { get; set; }
    }
}
