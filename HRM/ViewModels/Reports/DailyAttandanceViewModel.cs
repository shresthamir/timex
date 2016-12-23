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
    class DailyAttandanceViewModel : ReportViewModel
    {
        private bool _PresentOnly =true;
        public bool PresentOnly { get { return _PresentOnly; } set { _PresentOnly = value; OnPropertyChanged("PresentOnly"); } }

        public DailyAttandanceViewModel()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    DepartmentList = conn.Query<Department>("SELECT DEPARTMENT_ID, DEPARTMENT FROM DEPARTMENT");
                    EmpDList = conn.Query<EmployeeAllDetail>("SELECT * FROM vwEmpDetail");
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
            IEnumerable<MonthlyAttendance> Source;
            List<MonthlyAttendance> PreReport = new List<MonthlyAttendance>();
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {

                    string strSql =
 @"SELECT A.ENO, ED.FULLNAME ENAME, ED.DEPARTMENT, CHECKIN, CHECKINREMARKS, CHECKINMODE, CHECKOUT, CHECKOUTMODE, 
CHECKOUTREMARKS, LUNCHOUT, LUNCHOUTMODE, LUNCHOUTREMARKS, LUNCHIN, LUNCHINMODE, LUNCHINREMARKS, DATEDIFF(MI,LUNCHOUT,LUNCHIN) LunchTime,  
CASE WHEN DATEDIFF(MI,LUNCHOUT,LUNCHIN)>LUNCHDURATION THEN DATEDIFF(MI,LUNCHOUT,LUNCHIN) - LUNCHDURATION ELSE 0 END ExcessLunchTime,
DATEDIFF(MI,CHECKIN,CHECKOUT) TotalDuration, A.HOLIDAY_ID, A.LEAVE_ID, A.IsWeekend, WH.INTIME, WH.INGRACETIME, WH.OUTTIME, 
WH.OUTGRACETIME, WH.LUNCHDURATION, L.LEAVE_NAME, H.HOLIDAY_NAME,
CASE WHEN LEAVE_NAME IS NOT NULL THEN LEAVE_NAME
WHEN HOLIDAY_NAME IS NOT NULL THEN HOLIDAY_NAME
WHEN IsWeekend = 1 THEN 'Weekend' 
WHEN CHECKIN IS NULL THEN 'Absent'
ELSE 'Present' END ATT_REMARKS
FROM ATTENDANCE A JOIN WORKHOUR WH ON A.WORKHOUR_ID = WH.WORKHOUR_ID
JOIN vwEmpDetail ED ON A.ENO = ED.ENO
LEFT JOIN HOLIDAYS H ON H.HOLIDAY_ID = A.HOLIDAY_ID
LEFT JOIN LEAVES L ON A.LEAVE_ID = L.LEAVE_ID WHERE ATT_DATE = @ATT_DATE" + (AllDepartments ? string.Empty : " AND ED.DEPARTMENT_ID = @DEPARTMENT_ID");
                    Source = conn.Query<MonthlyAttendance>(strSql, new { DEPARTMENT_ID = AllDepartments ? 0 : SelectedDepartment.DEPARTMENT_ID, ATT_DATE = TDate });

                    foreach (EmployeeAllDetail emp in EmpDList)
                    {
                        if (!AllDepartments && emp.DEPARTMENT_ID != SelectedDepartment.DEPARTMENT_ID)
                            continue;
                        if (!Source.Any(x => x.ENO == emp.ENO))
                        {
                            if (PresentOnly)
                                continue;
                            PreReport.Add(new MonthlyAttendance { ENAME = emp.FULLNAME, DEPARTMENT = emp.DEPARTMENT, IsAbsent = true, ATT_REMARKS = "Absent" });
                        }
                        else
                        {
                            if (PresentOnly && Source.FirstOrDefault(x => x.ENO == emp.ENO).CHECKIN == null)
                                continue;
                            PreReport.Add(Source.First(x => x.ENO == emp.ENO));
                        }
                    }
                    if (PreReport.Count > 0)
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

    class DailyAbsentViewModel : ReportViewModel
    {
        public DailyAbsentViewModel()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    DepartmentList = conn.Query<Department>("SELECT DEPARTMENT_ID, DEPARTMENT FROM DEPARTMENT");
                    EmpDList = conn.Query<EmployeeAllDetail>("SELECT * FROM vwEmpDetail");
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
            IEnumerable<MonthlyAttendance> Source;
            List<MonthlyAttendance> PreReport = new List<MonthlyAttendance>();
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {

                    string strSql =
 @"SELECT A.ENO, ED.FULLNAME ENAME, ED.DEPARTMENT, CHECKIN, CHECKINREMARKS, CHECKINMODE, CHECKOUT, CHECKOUTMODE, 
CHECKOUTREMARKS, LUNCHOUT, LUNCHOUTMODE, LUNCHOUTREMARKS, LUNCHIN, LUNCHINMODE, LUNCHINREMARKS, DATEDIFF(MI,LUNCHOUT,LUNCHIN) LunchTime,  
CASE WHEN DATEDIFF(MI,LUNCHOUT,LUNCHIN)>LUNCHDURATION THEN DATEDIFF(MI,LUNCHOUT,LUNCHIN) - LUNCHDURATION ELSE 0 END ExcessLunchTime,
DATEDIFF(MI,CHECKIN,CHECKOUT) TotalDuration, A.HOLIDAY_ID, A.LEAVE_ID, A.IsWeekend, WH.INTIME, WH.INGRACETIME, WH.OUTTIME, 
WH.OUTGRACETIME, WH.LUNCHDURATION, L.LEAVE_NAME, H.HOLIDAY_NAME,
CASE WHEN LEAVE_NAME IS NOT NULL THEN LEAVE_NAME
WHEN HOLIDAY_NAME IS NOT NULL THEN HOLIDAY_NAME
WHEN IsWeekend = 1 THEN 'Weekend' 
WHEN CHECKIN IS NULL THEN 'Absent'
ELSE 'Present' END ATT_REMARKS
FROM ATTENDANCE A JOIN WORKHOUR WH ON A.WORKHOUR_ID = WH.WORKHOUR_ID
JOIN vwEmpDetail ED ON A.ENO = ED.ENO
LEFT JOIN HOLIDAYS H ON H.HOLIDAY_ID = A.HOLIDAY_ID
LEFT JOIN LEAVES L ON A.LEAVE_ID = L.LEAVE_ID WHERE ATT_DATE = @ATT_DATE" + (AllDepartments ? string.Empty : " AND ED.DEPARTMENT_ID = @DEPARTMENT_ID");
                    Source = conn.Query<MonthlyAttendance>(strSql, new { DEPARTMENT_ID = AllDepartments ? 0 : SelectedDepartment.DEPARTMENT_ID, ATT_DATE = TDate });

                    foreach (EmployeeAllDetail emp in EmpDList)
                    {
                        if (!AllDepartments && emp.DEPARTMENT_ID != SelectedDepartment.DEPARTMENT_ID)
                            continue;
                        if (!Source.Any(x => x.ENO == emp.ENO))
                        {
                            PreReport.Add(new MonthlyAttendance { ENAME = emp.FULLNAME, DEPARTMENT = emp.DEPARTMENT, IsAbsent = true, ATT_REMARKS = "Absent" });
                        }
                        else if(Source.First(x=>x.ENO ==emp.ENO).CHECKIN == null)
                        {
                            PreReport.Add(Source.First(x => x.ENO == emp.ENO));
                        }
                    }
                    if (PreReport.Count > 0)
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
