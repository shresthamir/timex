using HRM.Library.BaseClasses;
using HRM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using HRM.Library.AppScopeClasses;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.Windows.PdfViewer;
using System.Windows.Markup;
using System.Windows;
using Syncfusion.XlsIO;
using Microsoft.Win32;
using System.IO;

namespace HRM.ViewModels
{
    class ReportViewModel : RootViewModel
    {
        public SfDataGrid sfGrid;
        public string ReportName;
        public string ReportParameter;
        protected string RecordNotFoundMessage = "Record does not exists.";
        private ObservableCollection<dynamic> _ReportSource;
        private IEnumerable<Employee> _EmpList;
        private IEnumerable<Leaves> _LeaveList;
        private Employee _SelectedEmployee;
        private Leaves _SelectedLeave;
        private bool _AllEmployee = true;
        private bool _AllLeaves = true;
        private IEnumerable<Month> _MonthList;
        protected IEnumerable<Month> _All_Months;
        private Month _SelectedMonth;
        private int _CurYear;
        private IEnumerable<Department> _DepartmentList;
        private bool _AllDepartments = true;
        private Department _SelectedDepartment;
        protected DateTime _FDate = DateTime.Today;
        protected DateTime _TDate = DateTime.Today;
        private IEnumerable<EmployeeAllDetail> _EmpDList;
        private bool _AD;

        public bool AllEmployee
        {
            get
            {
                return _AllEmployee;
            }
            set
            {
                _AllEmployee = value; OnPropertyChanged("AllEmployee");
                SelectedEmployee = new Employee();
                SelectedEmployee.PropertyChanged += SelectedEmployee_PropertyChanged;
            }
        }
        public Month SelectedMonth { get { return _SelectedMonth; } set { _SelectedMonth = value; OnPropertyChanged("SelectedMonth"); } }

        public bool AllLeaves { get { return _AllLeaves; } set { _AllLeaves = value; OnPropertyChanged("AllLeaves"); } }
        public bool AllDepartments { get { return _AllDepartments; } set { _AllDepartments = value; OnPropertyChanged("AllDepartments"); } }

        public DateTime FDate { get { return _FDate; } set { _FDate = value; OnPropertyChanged("FDate"); } }
        public DateTime TDate { get { return _TDate; } set { _TDate = value; OnPropertyChanged("TDate"); } }

        public int CurYear { get { return _CurYear; } set { _CurYear = value; OnPropertyChanged("CurYear"); } }


        public Employee SelectedEmployee { get { return _SelectedEmployee; } set { _SelectedEmployee = value; OnPropertyChanged("SelectedEmployee"); } }
        public Leaves SelectedLeave { get { return _SelectedLeave; } set { _SelectedLeave = value; OnPropertyChanged("SelectedLeave"); } }
        public Department SelectedDepartment { get { return _SelectedDepartment; } set { _SelectedDepartment = value; OnPropertyChanged("SelectedDepartment"); } }


        public IEnumerable<Leaves> LeaveList { get { return _LeaveList; } set { _LeaveList = value; OnPropertyChanged("LeaveList"); } }
        public IEnumerable<Employee> EmpList { get { return _EmpList; } set { _EmpList = value; OnPropertyChanged("EmpList"); } }
        public IEnumerable<EmployeeAllDetail> EmpDList { get { return _EmpDList; } set { _EmpDList = value; OnPropertyChanged("EmpDList"); } }
        public IEnumerable<Month> MonthList { get { return _MonthList; } set { _MonthList = value; OnPropertyChanged("MonthList"); } }
        public IEnumerable<Department> DepartmentList { get { return _DepartmentList; } set { _DepartmentList = value; OnPropertyChanged("DepartmentList"); } }
        public ObservableCollection<dynamic> ReportSource { get { return _ReportSource; } set { _ReportSource = value; OnPropertyChanged("ReportSource"); } }
        public bool AD
        {
            get { return _AD; }
            set
            {
                _AD = value;
                OnPropertyChanged("AD");
                MonthList = _All_Months.Where(x => x.MTYPE == ((value) ? "AD" : "BS"));
                CurYear = (value) ? DateTime.Today.Year : DateFunctions.GetBsYear(DateTime.Today);
            }
        }

        public ReportViewModel()
        {            
            SelectedEmployee = new Employee();
            SelectedEmployee.PropertyChanged += SelectedEmployee_PropertyChanged;
            CurYear = (SETTING.DEFAULT_CALENDAR == "AD") ? DateTime.Today.Year : DateFunctions.GetBsYear(DateTime.Today);
            ExportCommand = new Library.Helpers.RelayCommand(ExportReport);
        }

        protected virtual void ExportReport(object obj)
        {
            //string opt = new UI.Misc.wExport().GetExportOption();
            ExcelExportingOptions option = new ExcelExportingOptions();
            option.ExportMode = ExportMode.Text;
            option.ExcludeColumns.Add("DEPARTMENT");
            var Engine = sfGrid.ExportToExcel(sfGrid.View, option);
            var workBook = Engine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                FilterIndex = 2,
                Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx",
                FileName = "Book1"
            };

            if (sfd.ShowDialog() == true)
            {
                using (Stream stream = sfd.OpenFile())
                {
                    if (sfd.FilterIndex == 1)
                        workBook.Version = ExcelVersion.Excel97to2003;
                    else
                        workBook.Version = ExcelVersion.Excel2007;
                    var sheet = workBook.ActiveSheet;
                    sheet.InsertRow(1, 5);
                    var CNAME = sheet.Range[1, 1, 1, option.Columns.Count - option.ExcludeColumns.Count + 1];
                    CNAME.Merge();
                    CNAME.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    CNAME.CellStyle.Font.Size = 12;
                    CNAME.CellStyle.Font.Bold = true;

                    var CADDRESS = sheet.Range[2, 1, 2, option.Columns.Count - option.ExcludeColumns.Count + 1];
                    CADDRESS.Merge();
                    CADDRESS.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                    var RPTNAME = sheet.Range[3, 1, 3, option.Columns.Count - option.ExcludeColumns.Count + 1];
                    RPTNAME.Merge();
                    RPTNAME.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    RPTNAME.CellStyle.Font.Bold = true;

                    var RPTPARAM = sheet.Range[4, 1, 4, option.Columns.Count - option.ExcludeColumns.Count + 1];
                    RPTPARAM.Merge();
                    RPTPARAM.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    RPTPARAM.CellStyle.Font.Italic = true;
                    //sheet.MergeRanges(sheet.Range[1, 1], sheet.Range[1, 10]);
                    sheet.Range[1, 1].Value = AppVariables.CompanyInfo.COMPANY_NAME;
                    sheet.Range[2, 1].Value = AppVariables.CompanyInfo.COMPANY_ADDRESS;
                    sheet.Range[3, 1].Value = ReportName;
                    sheet.Range[4, 1].Value = ReportParameter;
                    workBook.SaveAs(stream);
                }

                //Message box confirmation to view the created spreadsheet.
                if (MessageBox.Show("Do you want to view the workbook?", "Workbook has been created",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }

        void SelectedEmployee_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ENO")
            {
                Employee emp = EmpList.FirstOrDefault(x => x.ENO == SelectedEmployee.ENO);
                if (emp != null && SelectedEmployee.FULLNAME != emp.FULLNAME)
                    SelectedEmployee.FULLNAME = EmpList.FirstOrDefault(x => x.ENO == SelectedEmployee.ENO).FULLNAME;
            }
            else if (e.PropertyName == "FULLNAME")
            {
                Employee emp = EmpList.FirstOrDefault(x => x.FULLNAME == SelectedEmployee.FULLNAME);
                if (emp != null && SelectedEmployee.ENO != emp.ENO)
                    SelectedEmployee.ENO = EmpList.FirstOrDefault(x => x.FULLNAME == SelectedEmployee.FULLNAME).ENO;
            }
            if (e.PropertyName == "ENO" || e.PropertyName == "FULLNAME")
            {
                Employee E = EmpList.FirstOrDefault(y => y.ENO == SelectedEmployee.ENO);
                if (_All_Months != null && E != null)
                {
                    AD = E.CALENDAR_TYPE == "AD";
                }

            }
        }


    }

    class LeaveStatusViewModel : ReportViewModel
    {
        public LeaveStatusViewModel()
        {
            try
            {
                ReportName = "Leave Status Report";
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    EmpList = conn.Query<Employee>("SELECT 0 ENO, '' FULLNAME UNION ALL SELECT ENO, FULLNAME FROM EMPLOYEE");

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
                <TextBlock Grid.Row=""1"" FontWeight=""SemiBold"" FontSize=""14"" Text=""Leave Status Report""/>                
                <StackPanel Orientation=""Horizontal"" Grid.Row=""2"">                                      
                    <TextBlock Width=""70"" Margin=""20 0 0 0"" Text=""Employee :""/>
                     <TextBlock Text=""" + ((AllEmployee) ? "All" : SelectedEmployee.FULLNAME) + @"""/>                      
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
            IEnumerable<LeaveStatus> Source;
            try
            {
                if (AllEmployee)
                    Source = LeaveFunctions.GetLeaveStatus();
                else
                    Source = LeaveFunctions.GetLeaveStatus(SelectedEmployee.ENO);
                if (Source.Count() <= 0)
                {
                    ShowWarning(RecordNotFoundMessage);
                }
                ReportSource = new ObservableCollection<dynamic>(Source);
                SetAction(ButtonAction.Selected);
                ReportParameter = "Employee : " + ((AllEmployee) ? "All" : SelectedEmployee.FULLNAME);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }

    class LeaveLedgerViewModel : ReportViewModel
    {
        public LeaveLedgerViewModel()
        {
            try
            {
                ReportName = "Leave Ledger Report";
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    EmpList = conn.Query<Employee>("SELECT ENO, FULLNAME FROM EMPLOYEE");
                    LeaveList = conn.Query<Leaves>("SELECT 0 LEAVE_ID, '' LEAVE_NAME UNION ALL SELECT LEAVE_ID, LEAVE_NAME FROM LEAVES");
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
                <TextBlock Grid.Row=""1"" FontWeight=""SemiBold"" FontSize=""14"" Text=""Leave Ledger Report""/>                
                <StackPanel Orientation=""Horizontal"" Grid.Row=""2"">                                      
                     <TextBlock Width=""70"" Text=""Employee :""/>
                     <TextBlock Text=""" + SelectedEmployee.FULLNAME + @"""/>                      
                     <TextBlock Width=""50"" Margin=""20 0 0 0"" Text=""Leave :""/>
                     <TextBlock Text=""" + ((AllLeaves) ? "All" : SelectedLeave.LEAVE_NAME) + @"""/>  
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
            IEnumerable<LeaveLedger> Source;
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    string strSql =
@"SELECT LNAME, LEAVE_DATE, MC.BS, SUM(Dr) Given, SUM(Cr) Taken, R.Balance from vwLeaveLedger LL
CROSS APPLY (SELECT SUM(dr-cr) Balance FROM vwLeaveLedger WHERE ENO=LL.ENO AND LEAVE_ID = LL.LEAVE_ID AND LEAVE_DATE <= LL.LEAVE_DATE) R
JOIN MITICONVERTER MC ON LL.LEAVE_DATE = MC.AD 
WHERE ENO = @ENO " + (AllLeaves ? string.Empty : "AND LEAVE_ID = @LEAVE_ID \n") +
@"GROUP BY LNAME,LEAVE_DATE,MC.BS,Balance
ORDER BY LNAME, LEAVE_DATE";

                    Source = conn.Query<LeaveLedger>(strSql, new { ENO = SelectedEmployee.ENO, LEAVE_ID = (SelectedLeave == null) ? 0 : SelectedLeave.LEAVE_ID });
                }
                if (Source.Count() <= 0)
                {
                    ShowWarning(RecordNotFoundMessage);
                }
                ReportSource = new ObservableCollection<dynamic>(Source);
                SetAction(ButtonAction.Selected);
                ReportParameter = "Employee : " + SelectedEmployee.NAME + "   Leaves : " + ((AllLeaves) ? "All" : SelectedLeave.LEAVE_NAME);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }

}
