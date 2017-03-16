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
using HRM.Library.ValueConverter;
using PdfSharp.Pdf;
using PdfSharp;
using PdfSharp.Drawing;
using System.Windows.Forms;
using HRM.Controls;
using HRM.Library.BaseClasses;
using Syncfusion.UI.Xaml.Grid.Converter;
using System.IO;
using Syncfusion.XlsIO;

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
        public RelayCommand ShowWorkhourCommand { get { return new RelayCommand(ShowWorkhour); } }
        public RelayCommand AddRemarksCommand { get { return new RelayCommand(AddRemarks, CanAddRemarks); } }

        private bool CanAddRemarks(object obj)
        {
            if (obj != null && obj is Syncfusion.UI.Xaml.Grid.GridRecordContextMenuInfo)
            {
                Syncfusion.UI.Xaml.Grid.GridRecordContextMenuInfo cmi = obj as Syncfusion.UI.Xaml.Grid.GridRecordContextMenuInfo;
                if (cmi.Record is MonthlyAttendance)
                {
                    MonthlyAttendance ma = cmi.Record as MonthlyAttendance;
                    return ma.IsAbsent;
                }
            }
            return false;
        }

        private void AddRemarks(object obj)
        {
            try
            {
                Syncfusion.UI.Xaml.Grid.GridRecordContextMenuInfo cmi = obj as Syncfusion.UI.Xaml.Grid.GridRecordContextMenuInfo;
                MonthlyAttendance ma = cmi.Record as MonthlyAttendance;
                wAbsentRemarks window = new wAbsentRemarks();
                window.txtRemarks.Text = ma.ATT_REMARKS.Replace("Absent", "").Trim(' ', '-');
                window.ShowDialog();
                if (window.DialogResult.Value)
                {
                    ma.ATT_REMARKS = window.txtRemarks.Text;
                    ma.ENO = SelectedEmployee.ENO;
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Execute(@"IF NOT EXISTS (SELECT * FROM ATT_ABSENT_REMARKS WHERE ENO = @ENO AND ATT_DATE = @ATT_DATE)
INSERT INTO ATT_ABSENT_REMARKS(ENO, ATT_DATE, REMARKS) VALUES (@ENO, @ATT_DATE, @ATT_REMARKS)
ELSE 
UPDATE ATT_ABSENT_REMARKS SET REMARKS = @ATT_REMARKS WHERE ENO = @ENO AND ATT_DATE = @ATT_DATE", ma);
                    }
                    ma.ATT_REMARKS = "Absent - " + ma.ATT_REMARKS;
                    ma.OnPropertyChanged("ATT_REMARKS");
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }

        }

        private void ShowWorkhour(object obj)
        {
            WorkHour wh;
            Syncfusion.UI.Xaml.Grid.GridRecordContextMenuInfo cmi = obj as Syncfusion.UI.Xaml.Grid.GridRecordContextMenuInfo;
            MonthlyAttendance ma = cmi.Record as MonthlyAttendance;
            if (ma.WORKHOUR_ID == 0)
            {
                ShowWarning("No Attendance Data!");
                return;
            }
            using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
            {
                wh = conn.Query<WorkHour>("SELECT WORKHOUR_ID, DESCRIPTION, ISDEFAULT, INTIME, INGRACETIME, OUTTIME, OUTGRACETIME, LUNCHDURATION, BREAKDURATION, TOTALDURATION FROM WORKHOUR WHERE WORKHOUR_ID = " + ma.WORKHOUR_ID).First();
            }
            wWorkingHourDetail w = new wWorkingHourDetail() { Title = "Workhour Detail" };
            w.DataContext = wh;
            w.ShowDialog();
        }

        private void OpenLeaveApplication(object obj)
        {
        }


        public MonthlyAttandanceViewModel()
        {
            try
            {
                ReportName = "Monthly Attendance Report";
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    EmpList = conn.Query<Employee>("SELECT ENO, FULLNAME, CALENDAR_TYPE FROM EMPLOYEE");
                    _All_Months = conn.Query<Month>("SELECT MID, MTYPE, MNAME FROM tblMonthNames");

                }
                LoadData = new Library.Helpers.RelayCommand(LoadReport);
                PrintCommand = new Library.Helpers.RelayCommand(PrintReport);
                //ExportCommand = new RelayCommand(ExportToPDF);
                PD = new PrintDocument();
                PD.PrinterSettings.DefaultPageSettings.Landscape = true;
                PD.PrintPage += PD_PrintPage;

            }
            catch (Exception Ex)
            {
                ShowError(Ex.Message);
            }
        }

        void ExportToPDF()
        {
            PdfDocument document = new PdfDocument();
            document.Info.Author = "Rolf Baxter";
            document.Info.Keywords = "PdfSharp, Examples, C#";

            PdfPage page = document.AddPage();
            page.Size = PageSize.A4;
            page.Orientation = PageOrientation.Landscape;

            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont font = new XFont("Courier New", 10, XFontStyle.Regular);
            XFont CNameFont = new XFont("Courier New", 16, XFontStyle.Bold);
            XFont CAddressFont = new XFont("Courier New", 12, XFontStyle.BoldItalic);
            XFont RptNameFont = new XFont("Courier New", 14, XFontStyle.Bold);
            XFont Empfont = new XFont("Courier New", 10, XFontStyle.BoldItalic);




            short PrintLen = 125;
            string strReport = string.Empty;
            string strLeftMargin = "   ";
            string ReportName = "Monthly Attendance Report";
            string EmpInfo = string.Format("Employee : {0} Year : {1} Month : {2}", SelectedEmployee.FULLNAME, CurYear, SelectedMonth.MNAME);

            gfx.DrawString(AppVariables.CompanyInfo.COMPANY_NAME, CNameFont, XBrushes.Black, new XRect(10, 30, page.Width, 15), XStringFormats.Center);
            gfx.DrawString(AppVariables.CompanyInfo.COMPANY_ADDRESS, CAddressFont, XBrushes.Black, new XRect(10, 45, page.Width, 15), XStringFormats.Center);
            gfx.DrawString(ReportName, RptNameFont, XBrushes.Black, new XRect(10, 60, page.Width, 15), XStringFormats.Center);
            gfx.DrawString(EmpInfo, Empfont, XBrushes.Black, new XRect(10, 75, page.Width, 15), XStringFormats.Center);

            gfx.DrawString(strLeftMargin + string.Empty.PadLeft(PrintLen, '-'), RptNameFont, XBrushes.Black, new XRect(10, 85, page.Width, 10), XStringFormats.Center);
            gfx.DrawString(strLeftMargin + "Date".PadRight(10, ' ') + " | " + "Day".PadRight(4, ' ') + " | " + "Check In".PadRight(8, ' ') + " | " + "Mode".PadRight(12, ' ') + " | " + "Remarks".PadRight(16, ' ') +
                " | " + "Check Out".PadRight(9, ' ') + " | " + "Mode".PadRight(12, ' ') + " | " + "Remarks".PadRight(16, ' ') + " | " + "T. Duration".PadRight(16, ' '), Empfont, XBrushes.Black, new XRect(10, 95, page.Width, 10), XStringFormats.TopLeft);

            gfx.DrawString(strLeftMargin + string.Empty.PadLeft(PrintLen, '-'), RptNameFont, XBrushes.Black, new XRect(10, 105, page.Width, 10), XStringFormats.Center);
            int y = 115;
            foreach (MonthlyAttendance ma in ReportSource)
            {
                string Day = ma.ATT_DATE.DayOfWeek.ToString().Substring(0, 3);
                string AttDate = GetDate(ma.TDATE);
                if (ma.CHECKIN == null && ma.ATT_REMARKS != null)
                {
                    gfx.DrawString(strLeftMargin + PaddedString(AttDate, 10) + " | " + PaddedString(Day, 4) + " | " + ma.ATT_REMARKS.PadLeft((PrintLen - 31 + ma.ATT_REMARKS.Length) / 2, ' '), font, XBrushes.Black, new XRect(10, y, page.Width, 10), XStringFormats.TopLeft);
                }
                else if (ma.CHECKIN == null && ma.ATT_REMARKS == null)
                {
                    gfx.DrawString(strLeftMargin + PaddedString(AttDate, 10) + " | " + PaddedString(Day, 4) + " | ", font, XBrushes.Black, new XRect(10, y, page.Width, 10), XStringFormats.Default);
                }
                else
                    gfx.DrawString(strLeftMargin + PaddedString(AttDate, 10) + " | " + PaddedString(Day, 4) + " | " + PaddedString(ma.CHECKIN, 8) + " | " +
                        PaddedString(ma.CHECKINMODE, 12) + " | " + PaddedString(ma.CHECKINREMARKS, 16) + " | " + PaddedString(ma.CHECKOUT, 9) + " | " + PaddedString(ma.CHECKOUTMODE, 12) + " | " +
                        PaddedString(ma.CHECKOUTREMARKS, 16) + " | " + ToPeriod(ma.TotalDuration).PadRight(16, ' '), font, XBrushes.Black, new XRect(10, y, page.Width, 10), XStringFormats.TopLeft);
                y += 12;
            }
            gfx.DrawString(strLeftMargin + string.Empty.PadLeft(PrintLen, '-'), RptNameFont, XBrushes.Black, new XRect(10, y, page.Width, 10), XStringFormats.Center);
            y += 12;
            gfx.DrawString(string.Empty.PadLeft(20, ' ') + "Total Days".PadRight(15, ' ') + TotalDays.ToString().PadLeft(10, ' ') + " | " + "Weekends".PadRight(15, ' ') + Weekend.ToString().PadLeft(10, ' ') + " | " + "Holidays".PadRight(15, ' ') + Holidays.ToString().PadLeft(10, ' '), font, XBrushes.Black, new XRect(10, y, page.Width, 10), XStringFormats.TopLeft);
            y += 12;
            gfx.DrawString(string.Empty.PadLeft(20, ' ') + "Working Days".PadRight(15, ' ') + WorkingDays.ToString().PadLeft(10, ' ') + " | " + "Present Days".PadRight(15, ' ') + PresentDays.ToString().PadLeft(10, ' ') + " | " + "Absent Days".PadRight(15, ' ') + AbsentDays.ToString().PadLeft(10, ' '), font, XBrushes.Black, new XRect(10, y, page.Width, 10), XStringFormats.TopLeft);
            y += 12;
            gfx.DrawString(string.Empty.PadLeft(20, ' ') + "Paid Leaves".PadRight(15, ' ') + PaidLeaveDays.ToString("#0.00").PadLeft(10, ' ') + " | " + "Unpaid Leaves".PadRight(15, ' ') + UnpaidLeaveDays.ToString("#0.00").PadLeft(10, ' ') + " | " + "Hours Worked".PadRight(15, ' ') + new MinuteToTimeConverter().Convert(HoursWorked, typeof(string), null, System.Globalization.CultureInfo.CurrentCulture).ToString().PadLeft(10, ' '), font, XBrushes.Black, new XRect(10, y, page.Width, 10), XStringFormats.TopLeft);
            document.Save("Export Documents\\" + ReportName + " - " + SelectedEmployee.FULLNAME + " - " + CurYear + " - " + SelectedMonth.MNAME + ".pdf");
            //System.Diagnostics.Process.Start("TestDocument.pdf");
        }

        void PD_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(GetPrintString(), new System.Drawing.Font("Courier New", 10, System.Drawing.FontStyle.Bold), System.Drawing.Brushes.Black, 0, 0);
        }

        string GetPrintString()
        {
            short PrintLen = 125;
            string strReport = string.Empty;
            string strLeftMargin = "   ";
            string ReportName = "Monthly Attendance Report";
            string EmpInfo = string.Format("Employee : {0} Year : {1} Month : {2}", SelectedEmployee.FULLNAME, CurYear, SelectedMonth.MNAME);
            strReport += Environment.NewLine;
            strReport += AppVariables.CompanyInfo.COMPANY_NAME.PadLeft((PrintLen + AppVariables.CompanyInfo.COMPANY_NAME.Length) / 2, ' ') + Environment.NewLine;
            strReport += AppVariables.CompanyInfo.COMPANY_ADDRESS.PadLeft((PrintLen + AppVariables.CompanyInfo.COMPANY_ADDRESS.Length) / 2, ' ') + Environment.NewLine;
            strReport += ReportName.PadLeft((PrintLen + ReportName.Length) / 2, ' ') + Environment.NewLine;
            strReport += EmpInfo.PadLeft((PrintLen + EmpInfo.Length) / 2, ' ') + Environment.NewLine;
            strReport += Environment.NewLine;
            strReport += strLeftMargin + string.Empty.PadLeft(PrintLen, '-') + Environment.NewLine;
            strReport += strLeftMargin + "Date".PadRight(10, ' ') + " | " + "Day".PadRight(4, ' ') + " | " + "Check In".PadRight(8, ' ') + " | " + "Mode".PadRight(12, ' ') + " | " + "Remarks".PadRight(16, ' ') +
                " | " + "Check Out".PadRight(9, ' ') + " | " + "Mode".PadRight(12, ' ') + " | " + "Remarks".PadRight(16, ' ') + " | " + "T. Duration".PadRight(16, ' ') +
                //"|A. Duration".PadRight(12, ' ') + 
                Environment.NewLine;
            strReport += strLeftMargin + string.Empty.PadLeft(PrintLen, '-') + Environment.NewLine;
            foreach (MonthlyAttendance ma in ReportSource)
            {
                string Day = ma.ATT_DATE.DayOfWeek.ToString().Substring(0, 3);
                string AttDate = GetDate(ma.TDATE);
                if (ma.CHECKIN == null && ma.ATT_REMARKS != null)
                {
                    strReport += strLeftMargin + PaddedString(AttDate, 10) + " | " + PaddedString(Day, 4) + " | " + ma.ATT_REMARKS.PadLeft((PrintLen - 31 + ma.ATT_REMARKS.Length) / 2, ' ') + Environment.NewLine;
                }
                else if (ma.CHECKIN == null && ma.ATT_REMARKS == null)
                {
                    strReport += strLeftMargin + PaddedString(AttDate, 10) + " | " + PaddedString(Day, 4) + " | " + Environment.NewLine;
                }
                else
                    strReport += strLeftMargin + PaddedString(AttDate, 10) + " | " + PaddedString(Day, 4) + " | " + PaddedString(ma.CHECKIN, 8) + " | " +
                        PaddedString(ma.CHECKINMODE, 12) + " | " + PaddedString(ma.CHECKINREMARKS, 16) + " | " + PaddedString(ma.CHECKOUT, 9) + " | " + PaddedString(ma.CHECKOUTMODE, 12) + " | " +
                        PaddedString(ma.CHECKOUTREMARKS, 16) + " | " + ToPeriod(ma.TotalDuration).PadRight(16, ' ') +
                        //ToPeriod(ma.ActualDuration).PadRight(12, ' ') + 
                        Environment.NewLine;
            }
            strReport += strLeftMargin + string.Empty.PadLeft(PrintLen, '-') + Environment.NewLine;
            strReport += string.Empty.PadLeft(20, ' ') + "Total Days".PadRight(15, ' ') + TotalDays.ToString().PadLeft(10, ' ') + " | " + "Weekends".PadRight(15, ' ') + Weekend.ToString().PadLeft(10, ' ') + " | " + "Holidays".PadRight(15, ' ') + Holidays.ToString().PadLeft(10, ' ') + Environment.NewLine;
            strReport += string.Empty.PadLeft(20, ' ') + "Working Days".PadRight(15, ' ') + WorkingDays.ToString().PadLeft(10, ' ') + " | " + "Present Days".PadRight(15, ' ') + PresentDays.ToString().PadLeft(10, ' ') + " | " + "Absent Days".PadRight(15, ' ') + AbsentDays.ToString().PadLeft(10, ' ') + Environment.NewLine;
            strReport += string.Empty.PadLeft(20, ' ') + "Paid Leaves".PadRight(15, ' ') + PaidLeaveDays.ToString("#0.00").PadLeft(10, ' ') + " | " + "Unpaid Leaves".PadRight(15, ' ') + UnpaidLeaveDays.ToString("#0.00").PadLeft(10, ' ') + " | " + "Hours Worked".PadRight(15, ' ') + new MinuteToTimeConverter().Convert(HoursWorked, typeof(string), null, System.Globalization.CultureInfo.CurrentCulture).ToString().PadLeft(10, ' ') + Environment.NewLine;
            return strReport;
        }

        private void PrintReport(object obj)
        {
            PD.DefaultPageSettings.Landscape = true;
            PD.Print();
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
            IEnumerable<MonthlyAttendance> AbsentRemarks;
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
 @"SELECT ATT_DATE, CONVERT(VARCHAR,ATT_DATE,101) + ' ('+ MC.BS + ')' TDATE, A.WORKHOUR_ID,  CHECKIN, CHECKINREMARKS, CHECKINMODE, CHECKOUT, CHECKOUTMODE, 
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

                    AbsentRemarks = conn.Query<MonthlyAttendance>("SELECT ENO, ATT_DATE, REMARKS ATT_REMARKS FROM ATT_ABSENT_REMARKS WHERE ENO  = @ENO AND ATT_DATE BETWEEN @FDATE AND @TDATE", new { ENO = SelectedEmployee.ENO, FDATE = FDate, TDate = TDate });

                    while (FDate <= TDate)
                    {
                        if (!Source.Any(x => x.ATT_DATE == FDate))
                        {
                            if (FDate <= DateTime.Today)
                            {
                                if (AbsentRemarks.Any(x => x.ATT_DATE == FDate))
                                    PreReport.Add(new MonthlyAttendance { ATT_DATE = FDate, TDATE = FDate.ToString("MM/dd/yyyy") + " (" + DateFunctions.GetBsDate(FDate) + ")", IsAbsent = true, ATT_REMARKS = "Absent - " + AbsentRemarks.First(x => x.ATT_DATE == FDate).ATT_REMARKS });
                                else
                                    PreReport.Add(new MonthlyAttendance { ATT_DATE = FDate, TDATE = FDate.ToString("MM/dd/yyyy") + " (" + DateFunctions.GetBsDate(FDate) + ")", IsAbsent = true, ATT_REMARKS = "Absent" });
                            }
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
                    TotalDays = TDate.Subtract(SDate).Days + 1;
                    Weekend = PreReport.Where(x => x.IsWeekend).Count();
                    Holidays = PreReport.Where(x => !string.IsNullOrEmpty(x.HOLIDAY_NAME)).Count();
                    WorkingDays = TotalDays - (Weekend + Holidays);
                    if (!SETTING.COUNT_AS_PRESENT_ON_HOLIDAY)
                        PreReport = PreReport.Where(x => string.IsNullOrEmpty(x.HOLIDAY_NAME)).ToList();
                    if (!SETTING.COUNT_AS_PRESENT_ON_WEEKEND)
                        PreReport = PreReport.Where(x => !x.IsWeekend).ToList();
                    if (SETTING.ABSENT_IF_NO_CHECK_OUT)
                        PresentDays = PreReport.Where(x => x.CHECKIN != null && x.CHECKOUT != null).Count();
                    else
                        PresentDays = PreReport.Where(x => x.CHECKIN != null).Count();
                    AbsentDays = (WorkingDays > (PresentDays + PaidLeaveDays)) ? WorkingDays - (PresentDays + Convert.ToInt16(Math.Floor(PaidLeaveDays))) : 0;
                    HoursWorked = PreReport.Sum(x => x.ActualDuration);
                    SetAction(ButtonAction.Selected);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        protected override void ExportReport(object obj)
        {
            string opt = new UI.Misc.wExport().GetExportOption();
            if (opt.Equals("Excel"))
                ExportToExcel();
            else if (opt.Equals("PDF"))
                ExportToPDF();

        }

        void ExcelExport_CellExporting(object sender, GridCellExcelExportingEventArgs e)
        {
            //if(e.CellType == ExportCellType.RecordCell && e.ColumnName == "TDATE" )
            //{

            //    if (SETTING.DEFAULT_CALENDAR == "AD")
            //       e.Range.Value  = e.CellValue.ToString().Substring(0, 10);
            //    else
            //        e.Range.Value = e.CellValue.ToString().Substring(12, 10);
            //    e.Handled = true;
            //}
            if (e.CellType == ExportCellType.RecordCell && e.ColumnName == "ATT_REMARKS" && e.CellValue != null && !e.CellValue.Equals("Present"))
            {
                var sheet = e.Range.Worksheet;
                var Remarks = sheet.Range[e.Range.Row, 3, e.Range.Row, 10];
                Remarks.CellStyle.FillPattern = ExcelPattern.Solid;
                if (e.CellValue.ToString().Contains("Absent"))
                    Remarks.CellStyle.FillBackground = ExcelKnownColors.Red;
                else if (e.CellValue.ToString().Contains("Leave"))
                    Remarks.CellStyle.FillBackgroundRGB = System.Drawing.Color.FromArgb(112, 252, 160);
                else if (e.CellValue.Equals("Weekend"))
                    Remarks.CellStyle.FillBackground = ExcelKnownColors.Orange;
                else if (e.CellValue.Equals("Holiday"))
                    Remarks.CellStyle.FillBackgroundRGB = System.Drawing.Color.FromArgb(255, 211, 86);
                if (string.IsNullOrEmpty(sheet.Range[e.Range.Row, 3].Value))
                {
                    Remarks.Merge();
                    Remarks.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    Remarks.Value = e.CellValue.ToString();
                }
            }
        }

        void ExportToExcel()
        {
            try
            {
                //string opt = new UI.Misc.wExport().GetExportOption();
                ReportParameter = "Employee : " + SelectedEmployee.FULLNAME + "  Month : " + SelectedMonth.MNAME;
                ExcelExportingOptions option = new ExcelExportingOptions();
                option.ExportMode = ExportMode.Text;

                option.CellsExportingEventHandler = new GridCellExcelExportingEventHandler(ExcelExport_CellExporting);
                //option.ExportingEventHandler = new GridExcelExportingEventhandler(ExcelExport_CellExporting);

                option.ExcludeColumns.Add("DEPARTMENT");
                var Engine = sfGrid.ExportToExcel(sfGrid.View, option);
                var workBook = Engine.Excel.Workbooks[0];

                Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog
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
                    if (System.Windows.MessageBox.Show("Do you want to view the workbook?", "Workbook has been created",
                                        MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ExceptionHandler.GetException(ex).Message);
                throw;
            }
        }
    }

    class MonthlyAttendance : RootModel
    {
        public short WORKHOUR_ID { get; set; }
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

        public DateTime INTIME { get; set; }
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
