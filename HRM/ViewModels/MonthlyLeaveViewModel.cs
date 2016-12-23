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
using HRM.Library.Helpers;
namespace HRM.ViewModels
{
    class MonthlyLeaveViewModel : RootViewModel
    {
        private ObservableCollection<Month> _MonthList;
        private ObservableCollection<Employee> _EmpList;
        private bool _IsAllEmployee;
        private LEAVE_LEDGER _LLedger;
        private ObservableCollection<LEAVE_LEDGER> _EmployeeLeaveList;

        public bool MonthEnabled { get { return EmployeeLeaveList.Count == 0; } }
        public LEAVE_LEDGER LLedger { get { return _LLedger; } set { _LLedger = value; OnPropertyChanged("LLedger"); } }
        public bool IsAllEmployee { get { return _IsAllEmployee; } set { _IsAllEmployee = value; OnPropertyChanged("IsAllEmployee"); } }
        public ObservableCollection<Month> MonthList { get { return _MonthList; } set { _MonthList = value; OnPropertyChanged("MonthList"); } }
        public ObservableCollection<Employee> EmpList { get { return _EmpList; } set { _EmpList = value; OnPropertyChanged("EmpList"); } }
        public ObservableCollection<LEAVE_LEDGER> EmployeeLeaveList { get { return _EmployeeLeaveList; } set { _EmployeeLeaveList = value; OnPropertyChanged("EmployeeLeaveList"); } }
        public RelayCommand LoadLeaveCommand { get { return new RelayCommand(LoadLeave); } }

        private void LoadLeave(object obj)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    foreach (Employee emp in EmpList)
                    {
                        if (!IsAllEmployee && emp.ENO != LLedger.ENO)
                            continue;
                        if (conn.ExecuteScalar<int>("SELECT COUNT(*) FROM LEAVE_LEDGER WHERE ENO = @ENO AND MID = @MID", new { ENO = emp.ENO, MID = LLedger.MID }) > 0)
                        {
                            ShowWarning("Montly Leave for " + emp.FULLNAME + " has already been added.");
                            continue;
                        }

                        var empLeaves = conn.Query<LEAVE_LEDGER>(@"SELECT AL.ENO, E.FULLNAME ENAME, AL.LEAVE_ID, L.LEAVE_NAME LNAME, AL.ALLOWED_DAYS/12 DR, 0 Cr, 'admin' TRAN_USER, NULL APPLIED_DATE, NULL APPROVEDBY, NULL REMARKS, @MID MID FROM EMP_ALLOWED_LEAVE AL
                                        JOIN EMPLOYEE E ON AL.ENO = E.ENO
                                        JOIN LEAVES L ON AL.LEAVE_ID = L.LEAVE_ID WHERE E.ENO = @ENO", new { MID = LLedger.MID, ENO = emp.ENO });
                        foreach (LEAVE_LEDGER empLeave in empLeaves)
                        {
                            if (EmployeeLeaveList.Any(x => x.ENO == empLeave.ENO && x.LEAVE_ID == empLeave.LEAVE_ID))
                                continue;
                            EmployeeLeaveList.Add(empLeave);
                        }
                    }
                }
                OnPropertyChanged("MonthEnabled");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public MonthlyLeaveViewModel()
        {
            try
            {
                LLedger = new LEAVE_LEDGER();
                LLedger.PropertyChanged += LLedger_PropertyChanged;
                EmployeeLeaveList = new ObservableCollection<LEAVE_LEDGER>();
                NewCommand = new RelayCommand(NewAction);
                SaveCommand = new RelayCommand(SaveMonthlyLeave);
                ClearCommand = new RelayCommand(ClearInterface);
                LoadAllList();

            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void ClearInterface(object obj)
        {
            LLedger = new LEAVE_LEDGER();
            LLedger.PropertyChanged += LLedger_PropertyChanged;
            EmployeeLeaveList.Clear();
            OnPropertyChanged("MonthEnabled");
            SetAction(ButtonAction.Init);
        }

        void LLedger_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ENO")
            {
                Employee emp = EmpList.FirstOrDefault(x => x.ENO == LLedger.ENO);
                if (emp != null && LLedger.ENAME != emp.FULLNAME)
                    LLedger.ENAME = EmpList.FirstOrDefault(x => x.ENO == LLedger.ENO).FULLNAME;
            }
            else if (e.PropertyName == "ENAME")
            {
                Employee emp = EmpList.FirstOrDefault(x => x.FULLNAME == LLedger.ENAME);
                if (emp != null && LLedger.ENO != emp.ENO)
                    LLedger.ENO = EmpList.FirstOrDefault(x => x.FULLNAME == LLedger.ENAME).ENO;
            }
        }

        private void SaveMonthlyLeave(object obj)
        {
            try
            {
                if (!EmployeeLeaveList.Any(x => x.IsSelected))
                {
                    ShowWarning("Please select at least one leave from the grid");
                    return;
                }
                if (ShowConfirmation("You are going to save monthly Leave for month of " + MonthList.First(x => x.MID == LLedger.MID && x.MTYPE == SETTING.DEFAULT_CALENDAR).MNAME + ".\nMonthly Leave Transactions cannot be edited once saved.\nDo you want to Continue?"))
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            LLedger.TRANID = conn.ExecuteScalar<int>("SELECT ISNULL(MAX(TRANID),0) + 1 FROM LEAVE_LEDGER", transaction: tran);
                            LLedger.TRANDATE = conn.ExecuteScalar<DateTime>("SELECT GETDATE()", transaction: tran).Date;
                            foreach (LEAVE_LEDGER ll in EmployeeLeaveList)
                            {
                                if (ll.IsSelected)
                                {
                                    ll.TRANID = LLedger.TRANID;
                                    ll.TRANDATE = LLedger.TRANDATE;
                                    ll.TRAN_USER = AppVariables.LoggedUser;
                                    ll.Save(tran);
                                }
                            }
                            tran.Commit();
                        }
                    }
                    ShowInformation("Monthly Leave successfully saved.");
                    ClearInterface(null);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void NewAction(object obj)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    LLedger.TRANID = conn.ExecuteScalar<int>("SELECT ISNULL(MAX(TRANID),0) + 1 FROM LEAVE_LEDGER");
                SetAction(ButtonAction.New);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void LoadAllList()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    EmpList = new ObservableCollection<Employee>(conn.Query<Employee>("SELECT 0 ENO, '' ECODE, '' FULLNAME UNION ALL SELECT ENO, ECODE, FULLNAME FROM EMPLOYEE"));
                    MonthList = new ObservableCollection<Month>(conn.Query<Month>("SELECT MID, MTYPE, MNAME FROM tblMonthNames WHERE MTYPE = @MTYPE", new { MTYPE = SETTING.DEFAULT_CALENDAR }));

                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }


    }
}
