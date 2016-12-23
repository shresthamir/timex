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
using System.Drawing.Printing;
using HRM.Library.Helpers;
namespace HRM.ViewModels
{
    class MonthlyAttandanceViewModel : ReportViewModel
    {
        private int _TotalDays;
        private int _Weekend;
        private int _Holidays;
        private int _WorkingDays;
        private int _PresentDays;
        private decimal _PaidLeaveDays;
        private decimal _UnpaidLeaveDays;
        private int _AbsentDays;
        private int _HoursWorked;
        PrintDocument PD;

        public int TotalDays { get { return _TotalDays; } set { _TotalDays = value; OnPropertyChanged("TotalDays"); } }
        public int Weekend { get { return _Weekend; } set { _Weekend = value; OnPropertyChanged("Weekend"); } }
        public int Holidays { get { return _Holidays; } set { _Holidays = value; OnPropertyChanged("Holidays"); } }
        public int WorkingDays { get { return _WorkingDays; } set { _WorkingDays = value; OnPropertyChanged("WorkingDays"); } }
        public int PresentDays { get { return _PresentDays; } set { _PresentDays = value; OnPropertyChanged("PresentDays"); } }
        public decimal UnpaidLeaveDays { get { return _UnpaidLeaveDays; } set { _UnpaidLeaveDays = value; OnPropertyChanged("UnpaidLeaveDays"); } }
        public decimal PaidLeaveDays { get { return _PaidLeaveDays; } set { _PaidLeaveDays = value; OnPropertyChanged("PaidLeaveDays"); } }
        public int AbsentDays { get { return _AbsentDays; } set { _AbsentDays = value; OnPropertyChanged("AbsentDays"); } }
        public int HoursWorked { get { return _HoursWorked; } set { _HoursWorked = value; OnPropertyChanged("HoursWorked"); } }

        public RelayCommand LeaveCommand { get { return new RelayCommand(OpenLeaveApplication); } }

        private void OpenLeaveApplication(object obj)
        {
        }


        public MonthlyAttandanceViewModel()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    EmpList = conn.Query<Employee>("SELECT ENO, FULLNAME, CALENDAR_TYPE FROM EMPLOYEE");
                    _All_Months = conn.Query<Month>("SELECT MID, MTYPE, MNAME FROM tblMonthNames");
                    
                }
                LoadData = new Library.Helpers.RelayCommand(LoadReport);
                PrintCommand = new Library.Helpers.RelayCommand(PrintReport);
                PD = new PrintDocument();
                PD.PrinterSettings.DefaultPageSettings.Landscape = true;
                PD.PrintPage += PD_PrintPage;
            }
            catch (Exception Ex)
            {
                ShowError(Ex.Message);
            }
        }

        void PD_PrintPage(object sender, PrintPageEventArgs e)
        {
            short PrintLen = 122;
            string strReport = string.Empty;
            string ReportName = "Monthly Attendance Report";
            string EmpInfo = string.Format("Employee : {0} Year : {1} Month : {2}", SelectedEmployee.FULLNAME, CurYear, SelectedMonth.MNAME);
            strReport += Environment.NewLine;
            strReport += AppVariables.CompanyInfo.COMPANY_NAME.PadLeft((PrintLen + AppVariables.CompanyInfo.COMPANY_NAME.Length) / 2, ' ') + Environment.NewLine;
            strReport += AppVariables.CompanyInfo.COMPANY_ADDRESS.PadLeft((PrintLen + AppVariables.CompanyInfo.COMPANY_ADDRESS.Length) / 2, ' ') + Environment.NewLine;
            strReport += ReportName.PadLeft((PrintLen + ReportName.Length) / 2, ' ') + Environment.NewLine;
            strReport += EmpInfo.PadLeft((PrintLen + EmpInfo.Length) / 2, ' ') + Environment.NewLine;
            strReport += Environment.NewLine;
            strReport += string.Empty.PadLeft(PrintLen, '-') + Environment.NewLine;
            strReport += "Date".PadRight(11, ' ') + "|Day".PadRight(4, ' ') + "|Check In".PadRight(9, ' ') + "|Mode".PadRight(15, ' ') + "|Remarks".PadRight(15, ' ') +
                "|Check Out".PadRight(9, ' ') + "|Mode".PadRight(15, ' ') + "|Remarks".PadRight(15, ' ') + "|T. Duration".PadRight(12, ' ') +
                "|A. Duration".PadRight(12, ' ') + Environment.NewLine;
            strReport += string.Empty.PadLeft(PrintLen, '-') + Environment.NewLine;
            foreach (MonthlyAttendance ma in ReportSource)
            {
                if (ma.CHECKIN == null && ma.ATT_REMARKS != null)
                {
                    strReport += GetDate(ma.TDATE).PadRight(12, ' ') + ma.ATT_DATE.DayOfWeek.ToString().Substring(0, 3).PadRight(4, ' ') + ma.ATT_REMARKS.PadLeft((PrintLen - 31 + ma.ATT_REMARKS.Length) / 2, ' ') + Environment.NewLine;
                }
                else if (ma.CHECKIN == null && ma.ATT_REMARKS == null)
                {
                    strReport += GetDate(ma.TDATE).PadRight(12, ' ') + ma.ATT_DATE.DayOfWeek.ToString().Substring(0, 3).PadRight(4, ' ') + Environment.NewLine;
                }
                else
                    strReport += GetDate(ma.TDATE).PadRight(12, ' ') + ma.ATT_DATE.DayOfWeek.ToString().Substring(0, 3).PadRight(4, ' ') + ma.CHECKIN.Value.ToString("hh:mm tt").PadRight(9, ' ') +
                        PaddedString(ma.CHECKINMODE, 15) + PaddedString(ma.CHECKINREMARKS, 15) + PaddedString(ma.CHECKOUT, 9) + PaddedString(ma.CHECKOUTMODE, 15) +
                        PaddedString(ma.CHECKOUTREMARKS, 15) + ToPeriod(ma.TotalDuration).PadRight(12, ' ') + ToPeriod(ma.ActualDuration).PadRight(12, ' ') + Environment.NewLine;
            }
            strReport += string.Empty.PadLeft(PrintLen, '-') + Environment.NewLine;
            strReport += string.Empty.PadLeft(20, ' ') + "Total Days".PadRight(15, ' ') + TotalDays.ToString().PadLeft(10, ' ') + "|" + "Weekends".PadRight(15, ' ') + Weekend.ToString().PadLeft(10, ' ') + "|" + "Holidays".PadRight(15, ' ') + Holidays.ToString().PadLeft(10, ' ') + Environment.NewLine;
            strReport += string.Empty.PadLeft(20, ' ') + "Working Days".PadRight(15, ' ') + WorkingDays.ToString().PadLeft(10, ' ') + "|" + "Present Days".PadRight(15, ' ') + PresentDays.ToString().PadLeft(10, ' ') + "|" + "Absent Days".PadRight(15, ' ') + AbsentDays.ToString().PadLeft(10, ' ') + Environment.NewLine;
            strReport += string.Empty.PadLeft(20, ' ') + "Paid Leaves".PadRight(15, ' ') + PaidLeaveDays.ToString("#0.00").PadLeft(10, ' ') + "|" + "Unpaid Leaves".PadRight(15, ' ') + UnpaidLeaveDays.ToString("#0.00").PadLeft(10, ' ') + "|" + "Hours Worked".PadRight(15, ' ') + HoursWorked.ToString().PadLeft(10, ' ') + Environment.NewLine;
            e.Graphics.DrawString(strReport, new System.Drawing.Font("Courier New", 9), System.Drawing.Brushes.Black,0,0);
        }

        private void PrintReport(object obj)
        {
            
            ////            string HeaderTemplate =
            ////@"<ResourceDictionary xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
            ////                    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
            ////        <DataTemplate x:Key=""HeaderTemplate"">
            ////            <Grid HorizontalAlignment=""Center"">
            ////                <Grid.RowDefinitions>
            ////                    <RowDefinition Height=""25""/>
            ////                    <RowDefinition Height=""25""/>
            ////                    <RowDefinition Height=""20""/>
            ////                </Grid.RowDefinitions>
            ////                <TextBlock FontWeight=""SemiBold"" FontSize=""14""  Text="""+AppVariables.CompanyInfo.COMPANY_NAME+@"""/>
            ////                <TextBlock Grid.Row=""1"" FontWeight=""SemiBold"" FontSize=""14"" Text=""Monthly Attendance Report - Detail""/>                
            ////                <StackPanel Orientation=""Horizontal"" Grid.Row=""2"">
            ////                    <TextBlock Width=""70"" Text=""Employee :""/>
            ////                    <TextBlock Text="""+SelectedEmployee.FULLNAME+@"""/>                    
            ////                    <TextBlock Width=""50"" Margin=""20 0 0 0"" Text=""Year :""/>
            ////                    <TextBlock Text=""" + CurYear + @"""/>                    
            ////                    <TextBlock Width=""50"" Margin=""20 0 0 0"" Text=""Month :""/>
            ////                    <TextBlock Text=""" + SelectedMonth.MNAME + @"""/>                    
            ////                </StackPanel>
            ////            </Grid>  
            ////        </DataTemplate>
            ////</ResourceDictionary>
            ////";

            ////            sfGrid.PrintSettings.PrintPageHeaderTemplate = (XamlReader.Parse(HeaderTemplate) as ResourceDictionary)["HeaderTemplate"] as DataTemplate;
            ////            sfGrid.Print();

            //System.IO.Stream st = new System.IO.MemoryStream();
            //PdfExportingOptions options = new PdfExportingOptions();
            //options.ExportMergedCells = true;            
            //var document = sfGrid.ExportToPdf(options);
            //document.Save(st);

            //new HRM.UI.Reports.PdfViewer(st).ShowDialog();
            PD.DefaultPageSettings.Landscape = true;
            PD.Print();
            //RawPrintFunctions.RawPrinterHelper.SendStringToPrinter(new PrinterSettings().PrinterName, strReport, "Monthly Attendance Report");
        }

        string GetDate(string TDATE)
        {
            if (SelectedEmployee.CALENDAR_TYPE == "AD")
                return TDATE.Substring(0, 10);
            else
                return TDATE.Substring(12, 10);
        }

        string PaddedString(DateTime? value, int TotLength)
        {
            string strval = string.Empty;
            if (value != null)
                strval = value.Value.ToString("hh:mm tt");
            return strval.PadRight(TotLength, ' ');

        }

        string PaddedString(string value, int TotLength)
        {
            if (string.IsNullOrEmpty(value))
                value = string.Empty;
            else if (value.Length > TotLength)
                value = value.Substring(0, TotLength);
            return value.PadRight(TotLength, ' ');
        }

        string ToPeriod(int min)
        {
            if (min <= 0)
                return string.Empty;
            int hour = System.Convert.ToInt32(Math.Floor((decimal)min / 60));
            int mins = System.Convert.ToInt32(min % 60);
            return ((hour > 0) ? hour.ToString() + " Hrs " : string.Empty) + mins.ToString() + " Mins";
        }

        private void LoadReport(object obj)
        {

            DateTime FDate, TDate, SDate;
            IEnumerable<MonthlyAttendance> Source;
            List<MonthlyAttendance> PreReport = new List<MonthlyAttendance>();
            try
            {

                Employee E = EmpList.FirstOrDefault(x => x.ENO == SelectedEmployee.ENO);
                SDate = FDate = (E.CALENDAR_TYPE == "AD") ? new DateTime(CurYear, SelectedMonth.MID, 1) : DateFunctions.GetFirstDayOfBSMonth(SelectedMonth.MID, CurYear);
                TDate = (E.CALENDAR_TYPE == "AD") ? new DateTime(CurYear, SelectedMonth.MID, DateTime.DaysInMonth(CurYear, SelectedMonth.MID)) : DateFunctions.GetLastDayOfBSMonth(SelectedMonth.MID, CurYear);
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {

                    string strSql =
 @"SELECT ATT_DATE, CONVERT(VARCHAR,ATT_DATE,101) + ' ('+ MC.BS + ')' TDATE, CHECKIN, CHECKINREMARKS, CHECKINMODE, CHECKOUT, CHECKOUTMODE, 
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
JOIN MITICONVERTER MC ON A.ATT_DATE = MC.AD
LEFT JOIN HOLIDAYS H ON H.HOLIDAY_ID = A.HOLIDAY_ID
LEFT JOIN LEAVES L ON A.LEAVE_ID = L.LEAVE_ID WHERE ATT_DATE BETWEEN @FDATE AND @TDATE AND ENO = @ENO";
                    Source = conn.Query<MonthlyAttendance>(strSql, new { ENO = SelectedEmployee.ENO, FDATE = FDate, TDate = TDate });

                    while (FDate <= TDate)
                    {
                        if (!Source.Any(x => x.ATT_DATE == FDate))
                        {
                            if (FDate <= DateTime.Today)
                                PreReport.Add(new MonthlyAttendance { ATT_DATE = FDate, TDATE = FDate.ToString("MM/dd/yyyy") + " (" + DateFunctions.GetBsDate(FDate) + ")", IsAbsent = true, ATT_REMARKS = "Absent" });
                            else
                                PreReport.Add(new MonthlyAttendance { ATT_DATE = FDate, TDATE = FDate.ToString("MM/dd/yyyy") + " (" + DateFunctions.GetBsDate(FDate) + ")" });
                        }
                        else
                        {
                            PreReport.Add(Source.First(x => x.ATT_DATE == FDate));
                        }
                        FDate = FDate.AddDays(1);
                    }

                    ReportSource = new ObservableCollection<dynamic>(PreReport);
                    PaidLeaveDays = conn.ExecuteScalar<Decimal>("SELECT SUM(Cr) FROM LEAVE_LEDGER LL JOIN LEAVES L ON LL.LEAVE_ID = L.LEAVE_ID WHERE ENO = @ENO AND ISPAIDLEAVE = 1 AND APPLIED_DATE BETWEEN @FDate AND @TDate", new { FDate = SDate, TDate = TDate, ENO = SelectedEmployee.ENO });
                    UnpaidLeaveDays = conn.ExecuteScalar<Decimal>("SELECT SUM(Cr) FROM LEAVE_LEDGER LL JOIN LEAVES L ON LL.LEAVE_ID = L.LEAVE_ID WHERE ENO = @ENO AND ISPAIDLEAVE = 0 AND APPLIED_DATE BETWEEN @FDate AND @TDate", new { FDate = SDate, TDate = TDate, ENO = SelectedEmployee.ENO });
                    TotalDays = TDate.Subtract(SDate).Days;
                    Weekend = PreReport.Where(x => x.IsWeekend).Count();
                    Holidays = PreReport.Where(x => !string.IsNullOrEmpty(x.HOLIDAY_NAME)).Count();
                    WorkingDays = TotalDays - (Weekend + Holidays);
                    PresentDays = PreReport.Where(x => x.CHECKIN != null && x.CHECKOUT != null).Count();
                    AbsentDays = WorkingDays - (PresentDays + Convert.ToInt16(Math.Floor(PaidLeaveDays)));
                    HoursWorked = PreReport.Sum(x => x.ActualDuration);
                    SetAction(ButtonAction.Selected);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }

    class MonthlyAttendance
    {
        public short ENO { get; set; }
        public string ENAME { get; set; }
        public string DEPARTMENT { get; set; }
        public DateTime ATT_DATE { get; set; }
        public string TDATE { get; set; }
        public string DAYOFWEEK { get; set; }
        public DateTime? CHECKIN { get; set; }
        public string CHECKINMODE { get; set; }
        public string CHECKINREMARKS { get; set; }
        public DateTime? CHECKOUT { get; set; }
        public string CHECKOUTMODE { get; set; }
        public string CHECKOUTREMARKS { get; set; }
        public DateTime? LUNCHOUT { get; set; }
        public string LUNCHOUTMODE { get; set; }
        public string LUNCHOUTREMARKS { get; set; }
        public DateTime? LUNCHIN { get; set; }
        public string LUNCHINMODE { get; set; }
        public string LUNCHINREMARKS { get; set; }
        public string ATT_REMARKS { get; set; }
        public string LEAVE_NAME { get; set; }
        public string HOLIDAY_NAME { get; set; }
        public bool IsWeekend { get; set; }
        public bool IsAbsent { get; set; }
        public bool LEAVE_ID { get; set; }

        public DateTime INDATE { get; set; }
        public DateTime INGRACETIME { get; set; }
        public DateTime OUTTIME { get; set; }
        public DateTime OUTGRACETIME { get; set; }
        public short LUNCHDURATION { get; set; }

        public int TotalDuration { get; set; }
        public int LunchTime { get; set; }
        public int ExcessLunchTime { get; set; }

        public int ActualDuration
        {
            get
            {
                return TotalDuration - ExcessLunchTime;
            }
        }
    }

}
