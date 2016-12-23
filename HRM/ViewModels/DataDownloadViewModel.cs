using HRM.Library.AppScopeClasses;
using HRM.Library.BaseClasses;
using HRM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.ComponentModel;
using HRM.Library.Helpers;


namespace HRM.ViewModels
{
    class DataDownloadViewModel : RootViewModel
    {        
        IEnumerable<Employee> EmpList;
        static zkemkeeper.CZKEM zkem;
        private ObservableCollection<AttDevice> _DeviceList;
        public ObservableCollection<AttDevice> DeviceList { get { return _DeviceList; } set { _DeviceList = value; OnPropertyChanged("DeviceList"); } }

        public RelayCommand DownloadCommand { get { return new RelayCommand(DownloadData); } }
        public RelayCommand RefreshCommand { get { return new RelayCommand(RefreshData); } }

        private void RefreshData(object obj)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    conn.Open();
                    EmpList = conn.Query<Employee>("SELECT * FROM EMPLOYEE");
                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        RefreshAllData(tran);                       
                        tran.Commit();
                    }

                }
            }
            catch (Exception Ex)
            {

                ShowError(Ex.Message);
            }
        }

        private void DownloadData(object obj)
        {
            List<AttLog> AttData = new List<AttLog>();
            try
            {
                zkem = new zkemkeeper.CZKEM();
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    conn.Open();
                    foreach (AttDevice ad in DeviceList)
                    {
                        if (!ad.IsSelected || ad.STATUS != 1)
                            continue;
                        zkem.Connect_Net(ad.DEVICE_IP, 4370);

                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            if (zkem.ReadGeneralLogData(zkem.MachineNumber))
                            {
                                LogData ld = new LogData();
                                while (zkem.SSR_GetGeneralLogData(zkem.MachineNumber, out ld.dwEnrollNumber, out ld.dwVerifyMode, out ld.dwInOutMode, out ld.dwYear, out ld.dwMonth, out ld.dwDay, out ld.dwHour, out ld.dwMinute, out ld.dwSecond, ref ld.dwWorkCode))
                                {
                                    AttLog al = new AttLog
                                    {
                                        ENO = Convert.ToInt16(ld.dwEnrollNumber),
                                        ATT_DATE = new DateTime(ld.dwYear, ld.dwMonth, ld.dwDay),
                                        ATT_TIME = new DateTime(ld.dwYear, ld.dwMonth, ld.dwDay, ld.dwHour, ld.dwMinute, ld.dwSecond),
                                        VerifyMode = Convert.ToByte(ld.dwVerifyMode),
                                        InOutMode = Convert.ToByte(ld.dwInOutMode)
                                    };
                                    al.Save(tran);
                                }
                            }
                            tran.Commit();
                         //   zkem.ClearGLog(zkem.MachineNumber);
                            zkem.Disconnect();
                        }
                        EmpList = conn.Query<Employee>("SELECT * FROM EMPLOYEE");
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            RefreshAllData(tran);
                            tran.Commit();
                        }
                    }

                    
                }
                ShowInformation("Attendance Data Downloaded successfully.");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        public DataDownloadViewModel()
        {
            MessageBoxCaption = "Data Download";
            RefreshDeviceList();
        }        

        void RefreshDeviceList()
        {
            try
            {
                DeviceList = new ObservableCollection<AttDevice>();
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    var dlist = conn.Query<AttDevice>("SELECT * FROM ATT_DEVICE");
                    foreach (AttDevice ad in dlist)
                    {
                        ad.PropertyChanged += ad_PropertyChanged;
                        DeviceList.Add(ad);
                    }
                }
            }
            catch (Exception Ex)
            {
                ShowError(Ex.Message);
            }
        }


        void ad_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            AttDevice ad = sender as AttDevice;
            if (e.PropertyName == "IsSelected")
            {
                if (ad.IsSelected)
                {

                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += worker_DoWork;
                    worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                    worker.RunWorkerAsync(ad);

                }
                else
                    ad.STATUS = 0;
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {

            zkem = new zkemkeeper.CZKEM();
            AttDevice ad = e.Argument as AttDevice;
            ad.STATUS = 2;
            ad.STATUS = (byte)(zkem.Connect_Net(ad.DEVICE_IP, 4370) ? 1 : 0);
            zkem.Disconnect();

            //System.Threading.Thread.Sleep(5000);
            //ad.STATUS = 1;
        }


        public static void RefreshLeaveData(SqlTransaction tran)
        {
            var EmpLeaves = tran.Connection.Query<Attendance>("SELECT ENO, dbo.GetEmpWorkHour(ENO,APPLIED_DATE) WORKHOUR_ID, APPLIED_DATE ATT_DATE, LEAVE_ID FROM LEAVE_LEDGER LL WHERE NOT EXISTS (SELECT ENO, ATT_DATE FROM ATTENDANCE A WHERE LL.ENO = A.ENO AND A.ATT_DATE = LL.APPLIED_DATE) AND Cr>0", transaction: tran);
            foreach (Attendance ll in EmpLeaves)
            {
                ll.FYID = AppVariables.FYID;
                tran.Connection.Execute("INSERT INTO ATTENDANCE(ENO, FYID, WORKHOUR_ID, ATT_DATE, LEAVE_ID) VALUES (@ENO, @FYID, @WORKHOUR_ID, @ATT_DATE, @LEAVE_ID)",
                        ll, tran);
            }

        }

        public static void RefreshHolidayData(SqlTransaction tran, IEnumerable<Employee> EmpList)
        {
            IEnumerable<dynamic> EmpHolidaySettingList;
            IEnumerable<dynamic> HolidayList;

            EmpHolidaySettingList = tran.Connection.Query
                (
@"SELECT E.ENO, ISNULL(EP.GENDER, 'Male') GENDER, ISNULL(EP.RELIGION, 'Hindu') RELIGION , ED.BRANCH_ID, EP.JOINDATE FROM EMPLOYEE E
JOIN 
(
	EMPLOYEE_DETAIL ED JOIN
	(
		SELECT ENO, MAX(EMP_TRANID) EMP_TRANID FROM EMPLOYEE_DETAIL GROUP BY ENO
	) EID ON ED.EMP_TRANID = EID.EMP_TRANID
) ON ED.ENO = E.ENO
LEFT JOIN EMPLOYEE_PERSONAL_INFO EP ON E.ENO = EP.ENO", transaction: tran
                );

            HolidayList = tran.Connection.Query
                (
@"SELECT HD.HOLIDAY_ID, HOLIDAY_DATE, ISNULL(BRANCH_ID, 0) BRANCHID, ISNULL(GENDER,'') GENDER, ISNULL(RELIGION,'') RELIGION FROM HOLIDAY_DATE HD 
LEFT JOIN HOLIDAY_BRANCH HB ON HD.HOLIDAY_ID = HB.HOLIDAY_ID
LEFT JOIN HOLIDAY_GENDER HG ON HD.HOLIDAY_ID = HB.HOLIDAY_ID
LEFT JOIN HOLIDAY_RELIGION HR ON HD.HOLIDAY_ID = HB.HOLIDAY_ID
WHERE FYID = " + AppVariables.FYID + " ORDER BY HOLIDAY_DATE", transaction: tran
                );
            foreach (Employee e in EmpList)
            {
                var HolidayAlreadygiven = tran.Connection.Query<short>("SELECT DISTINCT HOLIDAY_ID FROM ATTENDANCE WHERE ENO = @ENO AND FYID = @FYID AND HOLIDAY_ID IS NOT NULL", new { ENO = e.ENO, FYID = AppVariables.FYID }, tran);
                var empHSetting = EmpHolidaySettingList.First(x => x.ENO == e.ENO);
                foreach (dynamic holiday in HolidayList)
                {
                    if (HolidayAlreadygiven.Any(x => x == holiday.HOLIDAY_ID))
                        continue;
                    else if (holiday.BRANCHID > 0 && empHSetting.BRANCHID != holiday.BRANCHID)
                        continue;
                    else if (!string.IsNullOrEmpty(holiday.GENDER) && empHSetting.GENDER != holiday.GENDER)
                        continue;
                    else if (!string.IsNullOrEmpty(holiday.RELIGION) && empHSetting.GENDER != holiday.RELIGION)
                        continue;
                    else if ((DateTime)holiday.HOLIDAY_DATE < (DateTime)empHSetting.JOINDATE)
                        continue;

                    if (tran.Connection.ExecuteScalar<int>("SELECT COUNT(*) FROM ATTENDANCE WHERE ENO = @ENO AND ATT_DATE = @ATT_DATE", new { ENO = e.ENO, ATT_DATE = holiday.HOLIDAY_DATE }, tran) == 0)
                    {
                        tran.Connection.Execute("INSERT INTO ATTENDANCE(ENO, FYID, WORKHOUR_ID, ATT_DATE, HOLIDAY_ID) VALUES (@ENO, @FYID, dbo.GetEmpWorkHour(@ENO,@ATT_DATE), @ATT_DATE, @HOLIDAY_ID)",
                                new { ENO = e.ENO, FYID = AppVariables.FYID, ATT_DATE = holiday.HOLIDAY_DATE, HOLIDAY_ID = holiday.HOLIDAY_ID }, tran);
                    }
                    else
                    {
                        tran.Connection.Execute("UPDATE ATTENDANCE SET HOLIDAY_ID = @HOLIDAY_ID WHERE ENO = @ENO AND ATT_DATE = @ATT_DATE",
                             new { ENO = e.ENO, ATT_DATE = holiday.HOLIDAY_DATE, HOLIDAY_ID = holiday.HOLIDAY_ID }, tran);
                    }
                }
            }
        }

        void RefreshWeekendData(SqlTransaction tran)
        {
            IEnumerable<Weekend> EmpWeekend = tran.Connection.Query<Weekend>("SELECT ENO, DAY_ID, EFFECTIVE_DATE, ISNULL(LAST_DATE, GETDATE()) LAST_DATE FROM EMPLOYEE_WEEKEND", transaction: tran);
            DateTime Today = tran.Connection.ExecuteScalar<DateTime>("SELECT GETDATE()", transaction: tran).Date;
            foreach (Employee e in EmpList)
            {
                DateTime LastWeekend = tran.Connection.ExecuteScalar<DateTime>("SELECT ISNULL(MAX(ATT_DATE), (SELECT JOINDATE FROM EMPLOYEE_PERSONAL_INFO WHERE ENO = @ENO)) FROM ATTENDANCE WHERE ENO = @ENO AND ISNULL(IsWeekend,0) = 1", e, tran);
                while (LastWeekend <= Today)
                {
                    LastWeekend = LastWeekend.AddDays(1);
                    if (EmpWeekend.Any(x => x.ENO == e.ENO && x.EFFECTIVE_DATE <= LastWeekend && x.LAST_DATE >= LastWeekend && x.DAY_ID == ((byte)LastWeekend.DayOfWeek + 1)))
                    {
                        if (tran.Connection.ExecuteScalar<int>("SELECT COUNT(*) FROM ATTENDANCE WHERE ENO = @ENO AND ATT_DATE = @ATT_DATE", new { ENO = e.ENO, ATT_DATE = LastWeekend }, tran) == 0)
                        {
                            tran.Connection.Execute("INSERT INTO ATTENDANCE(ENO, FYID, WORKHOUR_ID, ATT_DATE, IsWeekend) VALUES (@ENO, @FYID, dbo.GetEmpWorkHour(@ENO, @ATT_DATE), @ATT_DATE, @IsWeekend)",
                                    new { ENO = e.ENO, FYID = AppVariables.FYID, ATT_DATE = LastWeekend, IsWeekend = true }, tran);
                        }
                        else
                        {
                            tran.Connection.Execute("UPDATE ATTENDANCE SET IsWeekend = @IsWeekend WHERE ENO = @ENO AND ATT_DATE = @ATT_DATE",
                                 new { ENO = e.ENO, ATT_DATE = LastWeekend, IsWeekend = true }, tran);
                        }
                    }
                }
            }
        }

        void RefreshAllData(SqlTransaction tran)
        {
            try
            {
                RefreshLeaveData(tran);
                RefreshHolidayData(tran,EmpList);
                RefreshWeekendData(tran);
                RefreshAttendanceData(tran, EmpList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void RefreshAttendanceData(SqlTransaction tran, IEnumerable<Employee> EmpList)
        {
            IEnumerable<Attendance> CheckInOutData;
            IEnumerable<Attendance> LunchOutInData;
            IEnumerable<Attendance> BreakOutInData;


            CheckInOutData = tran.Connection.Query<Attendance>
                (
@"SELECT [IN].*, dbo.GetEmpWorkHour([IN].ENO,[IN].ATT_DATE) WORKHOUR_ID, InMode.MODE CHECKINMODE, InMode.REMARKS CHECKINREMARKS,  [OUT].CHECKOUT, OutMode.MODE CHECKOUTMODE, OutMode.REMARKS CHECKINREMARKS FROM
(
	SELECT L.ENO, L.ATT_DATE, 
	CASE WHEN A.CHECKIN IS NULL THEN MIN(ATT_TIME) 
	WHEN A.CHECKIN>MIN(ATT_TIME) THEN MIN(ATT_TIME) 
	ELSE A.CHECKIN END CHECKIN  
	FROM vwAttLogDetail L LEFT JOIN ATTENDANCE A ON A.ENO = L.ENO AND A.ATT_DATE = L.ATT_DATE  
	WHERE ISNULL(FLAG,0) = 0  GROUP BY L.ENO, L.ATT_DATE, A.CHECKIN
) [IN] 
JOIN vwAttLogDetail INMODE ON [IN].ENO = INMODE.ENO AND [IN].ATT_DATE = INMODE.ATT_DATE AND [IN].CHECKIN = INMODE.ATT_TIME
LEFT JOIN 
(
	(
		SELECT L.ENO, L.ATT_DATE, 
		CASE WHEN A.CHECKOUT IS NULL THEN MAX(ATT_TIME)  
		WHEN A.CHECKOUT<MAX(ATT_TIME) THEN MAX(ATT_TIME) 
		ELSE A.CHECKOUT END CHECKOUT 
		FROM vwAttLogDetail L LEFT JOIN ATTENDANCE A ON A.ENO = L.ENO AND A.ATT_DATE = L.ATT_DATE  
		WHERE ISNULL(FLAG,0) = 0 GROUP BY L.ENO, L.ATT_DATE, A.CHECKOUT
	) [OUT] 
	JOIN vwAttLogDetail OUTMODE ON [OUT].ENO = OUTMODE.ENO AND [OUT].ATT_DATE = OUTMODE.ATT_DATE AND [OUT].CHECKOUT = OUTMODE.ATT_TIME
) ON [IN].ENO = [OUT].ENO AND [IN].ATT_DATE = [OUT].ATT_DATE AND DATEDIFF(MINUTE, [IN].CHECKIN, [OUT].CHECKOUT) > 0", transaction: tran
                );

            foreach (Attendance att in CheckInOutData)
            {
                Attendance ExistingAttData = tran.Connection.Query<Attendance>("SELECT * FROM ATTENDANCE WHERE ENO = @ENO AND ATT_DATE = @ATT_DATE", new { ENO = att.ENO, ATT_DATE = att.ATT_DATE }, tran).FirstOrDefault();
                if (ExistingAttData == null)
                {
                    if (EmpList.Any(x => x.ENO == att.ENO))
                    {                        
                        att.FYID = AppVariables.FYID;
                        tran.Connection.Execute("INSERT INTO ATTENDANCE(ENO, FYID, WORKHOUR_ID, ATT_DATE, CHECKIN, CHECKINMODE, CHECKINREMARKS, CHECKOUT, CHECKOUTMODE, CHECKOUTREMARKS) VALUES (@ENO, @FYID, @WORKHOUR_ID, @ATT_DATE, @CHECKIN, @CHECKINMODE, @CHECKINREMARKS, @CHECKOUT, @CHECKOUTMODE, @CHECKOUTREMARKS)",
                                att, tran);
                    }
                }
                else
                {
                    tran.Connection.Execute("UPDATE ATTENDANCE SET CHECKIN = @CHECKIN, CHECKINMODE = @CHECKINMODE, CHECKINREMARKS = @CHECKINREMARKS, CHECKOUT = @CHECKOUT, CHECKOUTMODE = @CHECKOUTMODE, CHECKOUTREMARKS = @CHECKOUTREMARKS WHERE ENO = @ENO AND ATT_DATE = @ATT_DATE",
                         att, tran);
                }
            }


            LunchOutInData = tran.Connection.Query<Attendance>
                (
@"SELECT [OUT].*, OUTMODE.MODE LUNCHOUTMODE, OUTMODE.REMARKS LUNCHOUTREMARKS, [IN].LUNCHIN, INMODE.MODE LUNCHINMODE, INMODE.REMARKS LUNCHINREMARKS FROM
(
    SELECT L.ENO, L.ATT_DATE, 
	CASE WHEN A.LUNCHOUT IS NULL THEN MIN(ATT_TIME) 
	WHEN A.LUNCHOUT>MIN(ATT_TIME) THEN MIN(ATT_TIME) 
	ELSE A.LUNCHOUT END LUNCHOUT  
	FROM vwAttLogDetail L LEFT JOIN ATTENDANCE A ON A.ENO = L.ENO AND A.ATT_DATE = L.ATT_DATE  
	WHERE InOut = 'Lunch Out' AND ISNULL(FLAG,0) = 0  GROUP BY L.ENO, L.ATT_DATE, A.LUNCHOUT
) [OUT] 
JOIN vwAttLogDetail OUTMODE ON [OUT].ENO = OUTMODE.ENO AND [OUT].ATT_DATE = OUTMODE.ATT_DATE AND [OUT].LUNCHOUT = OUTMODE.ATT_TIME
LEFT JOIN 
(
	(
	    SELECT L.ENO, L.ATT_DATE, 
		CASE WHEN A.LUNCHIN IS NULL THEN MAX(ATT_TIME)  
		WHEN A.LUNCHIN<MAX(ATT_TIME) THEN MAX(ATT_TIME) 
		ELSE A.LUNCHIN END LUNCHIN 
		FROM vwAttLogDetail L LEFT JOIN ATTENDANCE A ON A.ENO = L.ENO AND A.ATT_DATE = L.ATT_DATE  
		WHERE InOut = 'Lunch In' AND ISNULL(FLAG,0) = 0 GROUP BY L.ENO, L.ATT_DATE, A.LUNCHIN
	) [IN] 
	JOIN vwAttLogDetail INMODE ON [IN].ENO = INMODE.ENO AND [IN].ATT_DATE = INMODE.ATT_DATE AND [IN].LUNCHIN = INMODE.ATT_TIME
) ON [IN].ENO = [OUT].ENO AND [IN].ATT_DATE = [OUT].ATT_DATE AND DATEDIFF(MINUTE, [IN].LUNCHIN, [OUT].LUNCHOUT) > 0", transaction: tran
                );

            foreach (Attendance att in LunchOutInData)
            {
                if (att.LUNCHIN > att.LUNCHOUT)
                {
                    tran.Connection.Execute("UPDATE ATTENDANCE SET LUNCHOUT = @LUNCHOUT, LUNCHOUTMODE = @LUNCHOUTMODE, LUNCHOUTREMARKS = @LUNCHOUTREMARKS, LUNCHIN = @LUNCHIN, LUNCHIN = @LUNCHINMODE, LUNCHINREMARKS = @LUNCHINREMARKS  WHERE ENO = @ENO AND ATT_DATE = @ATT_DATE",
                         att, tran);
                }
            }

            BreakOutInData = tran.Connection.Query<Attendance>
                (
@"SELECT [OUT].*, OUTMODE.MODE BREAKOUTMODE, OUTMODE.REMARKS BREAKOUTREMARKS, [IN].BREAKIN, INMODE.MODE BREAKINMODE, INMODE.REMARKS BREAKINREMARKS FROM
(
	 SELECT L.ENO, L.ATT_DATE, 
	CASE WHEN A.BREAKOUT IS NULL THEN MIN(ATT_TIME) 
	WHEN A.BREAKOUT>MIN(ATT_TIME) THEN MIN(ATT_TIME) 
	ELSE A.BREAKOUT END BREAKOUT  
	FROM vwAttLogDetail L LEFT JOIN ATTENDANCE A ON A.ENO = L.ENO AND A.ATT_DATE = L.ATT_DATE  
	WHERE InOut = 'Break Out' AND ISNULL(FLAG,0) = 0  GROUP BY L.ENO, L.ATT_DATE, A.BREAKOUT
) [OUT] 
JOIN vwAttLogDetail OUTMODE ON [OUT].ENO = OUTMODE.ENO AND [OUT].ATT_DATE = OUTMODE.ATT_DATE AND [OUT].BREAKOUT = OUTMODE.ATT_TIME
LEFT JOIN 
(
	(
		SELECT L.ENO, L.ATT_DATE, 
		CASE WHEN A.BREAKIN IS NULL THEN MAX(ATT_TIME)  
		WHEN A.BREAKIN<MAX(ATT_TIME) THEN MAX(ATT_TIME) 
		ELSE A.BREAKIN END BREAKIN 
		FROM vwAttLogDetail L LEFT JOIN ATTENDANCE A ON A.ENO = L.ENO AND A.ATT_DATE = L.ATT_DATE  
		WHERE InOut = 'BREAK In' AND ISNULL(FLAG,0) = 0 GROUP BY L.ENO, L.ATT_DATE, A.BREAKIN
	) [IN] 
	JOIN vwAttLogDetail INMODE ON [IN].ENO = INMODE.ENO AND [IN].ATT_DATE = INMODE.ATT_DATE AND [IN].BREAKIN = INMODE.ATT_TIME
) ON [IN].ENO = [OUT].ENO AND [IN].ATT_DATE = [OUT].ATT_DATE AND DATEDIFF(MINUTE, [IN].BREAKIN, [OUT].BREAKOUT) > 0", transaction: tran
                );

            foreach (Attendance att in BreakOutInData)
            {
                if (att.BREAKIN > att.BREAKOUT)
                {
                    tran.Connection.Execute("UPDATE ATTENDANCE SET BREAKOUT = @BREAKOUT, BREAKOUTMODE = @BREAKOUTMODE, BREAKOUTREMARKS = @BREAKOUTREMARKS, BREAKIN = @BREAKIN, BREAKIN = @BREAKINMODE, BREAKINREMARKS = @BREAKINREMARKS  WHERE ENO = @ENO AND ATT_DATE = @ATT_DATE",
                         att, tran);
                }
            }

            tran.Connection.Execute("UPDATE MANUAL_ATT_LOG SET FLAG = 1 WHERE ISNULL(FLAG,0) = 0", transaction: tran);
            tran.Connection.Execute("UPDATE ATT_LOG SET FLAG = 1 WHERE ISNULL(FLAG,0) = 0 AND ENO IN(SELECT ENO FROM EMPLOYEE)", transaction: tran);
        }
    }

    class LogData
    {
        public int dwTMachineNumber = 0;
        public string dwEnrollNumber = "";
        public int dwEMachineNumber = 0;
        public int dwVerifyMode = 0;
        public int dwInOutMode = 0;
        public int dwYear = 0;
        public int dwMonth = 0;
        public int dwDay = 0;
        public int dwHour = 0;
        public int dwMinute = 0;
        public int dwSecond = 0;
        public int dwWorkCode = 0;
    }


}
