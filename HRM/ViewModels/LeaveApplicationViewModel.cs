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
    class LeaveApplicationViewModel : RootViewModel
    {
        private ObservableCollection<Employee> _EmpList;
        private LEAVE_LEDGER _LLedger;
        private IEnumerable<Leaves> Leaves;
        private ObservableCollection<Models.Leaves> _LeaveList;
        private IEnumerable<AllowedLeaves> EmpLeaves;
        private DateTime _FDate = DateTime.Today;
        private DateTime _TDate = DateTime.Today;
        private decimal _TotalDays = 1;
        private bool _IsHalfDay;
        private decimal _Entitled;
        private decimal _Taken;
        private decimal _Remaining;

        public ObservableCollection<Leaves> LeaveList { get { return _LeaveList; } set { _LeaveList = value; OnPropertyChanged("LeaveList"); } }
        public LEAVE_LEDGER LLedger { get { return _LLedger; } set { _LLedger = value; OnPropertyChanged("LLedger"); } }
        public ObservableCollection<Employee> EmpList { get { return _EmpList; } set { _EmpList = value; OnPropertyChanged("EmpList"); } }
        public DateTime FDate { get { return _FDate; } set { _FDate = value; OnPropertyChanged("FDate"); } }
        public DateTime TDate { get { return _TDate; } set { _TDate = value; OnPropertyChanged("TDate"); } }
        public decimal TotalDays { get { return _TotalDays; } set { _TotalDays = value; OnPropertyChanged("TotalDays"); } }
        public bool IsHalfDay { get { return _IsHalfDay; } set { _IsHalfDay = value; OnPropertyChanged("IsHalfDay"); } }
        public decimal Entitled { get { return _Entitled; } set { _Entitled = value; OnPropertyChanged("Entitled"); } }
        public decimal Taken { get { return _Taken; } set { _Taken = value; OnPropertyChanged("Taken"); } }
        public decimal Remaining { get { return _Remaining; } set { _Remaining = value; OnPropertyChanged("Remaining"); } }
        public LeaveApplicationViewModel()
        {
            try
            {
                MessageBoxCaption = "Leave Application";
                this.PropertyChanged += LeaveApplicationViewModel_PropertyChanged;
                LLedger = new LEAVE_LEDGER();
                LLedger.PropertyChanged += LLedger_PropertyChanged;
                LeaveList = new ObservableCollection<Models.Leaves>();
                NewCommand = new RelayCommand(NewAction);
                SaveCommand = new RelayCommand(SaveLeaveApplication);
                ClearCommand = new RelayCommand(ClearInterface);
                DeleteCommand = new RelayCommand(DeleteLeave);
                LoadAllList();

            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void DeleteLeave(object obj)
        {
            throw new NotImplementedException();
        }

        void LeaveApplicationViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FDate" || e.PropertyName == "TDate" || e.PropertyName == "IsHalfDay")
            {
                TotalDays = (TDate.Subtract(FDate).Days + 1) * (_IsHalfDay ? 0.5m : 1m);
            }
        }

        private void ClearInterface(object obj)
        {
            LLedger = new LEAVE_LEDGER();
            LLedger.PropertyChanged += LLedger_PropertyChanged;
            SetAction(ButtonAction.Init);
        }

        void LLedger_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == "ENO")
                {
                    LeaveList.Clear();
                    foreach (AllowedLeaves al in EmpLeaves.Where(x => x.ENO == LLedger.ENO))
                    {
                        LeaveList.Add(Leaves.First(x => x.LEAVE_ID == al.LEAVE_ID));
                    }
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
                else if (e.PropertyName == "APPROVEDBY")
                {
                    Employee emp = EmpList.FirstOrDefault(x => x.ENO == LLedger.APPROVEDBY);
                    if (emp != null && LLedger.APPROVEDBYNAME != emp.FULLNAME)
                        LLedger.APPROVEDBYNAME = EmpList.FirstOrDefault(x => x.ENO == LLedger.APPROVEDBY).FULLNAME;
                }
                else if (e.PropertyName == "APPROVEDBYNAME")
                {
                    Employee emp = EmpList.FirstOrDefault(x => x.FULLNAME == LLedger.APPROVEDBYNAME);
                    if (emp != null && LLedger.APPROVEDBY != emp.ENO)
                        LLedger.APPROVEDBY = EmpList.FirstOrDefault(x => x.FULLNAME == LLedger.APPROVEDBYNAME).ENO;
                }

                if (e.PropertyName == "ENO" || e.PropertyName == "LEAVE_ID")
                {
                    if (LLedger.ENO > 0 && LLedger.LEAVE_ID > 0)
                    {
                        LeaveStatus ls = LeaveFunctions.GetLeaveStatus(LLedger.ENO, LLedger.LEAVE_ID).FirstOrDefault();
                        if (ls != null)
                        {
                            Entitled = ls.Entitled;
                            Taken = ls.Taken;
                            Remaining = ls.Remaining;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void SaveLeaveApplication(object obj)
        {
            DateTime date;
            try
            {
                if (!Validate())
                    return;
                if (ShowConfirmation("You are going to save new Leave Application.\nDo you want to Continue?"))
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            date = FDate;
                            LLedger.TRANID = conn.ExecuteScalar<int>("SELECT ISNULL(MAX(TRANID),0) + 1 FROM LEAVE_LEDGER", transaction: tran);
                            LLedger.TRANDATE = conn.ExecuteScalar<DateTime>("SELECT GETDATE()", transaction: tran).Date;
                            LLedger.TRAN_USER = AppVariables.LoggedUser;
                            while (date <= TDate)
                            {
                                LLedger.APPLIED_DATE = date;
                                LLedger.Cr = IsHalfDay ? 0.5m : 1m;
                                LLedger.Save(tran);
                                date = date.AddDays(1);
                            }
                            DataDownloadViewModel.RefreshLeaveData(tran);
                            tran.Commit();
                        }
                    }
                    ShowInformation("Leave Application successfully saved.");
                    ClearInterface(null);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private bool Validate()
        {
            using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
            {
                if (FDate > TDate)
                {
                    ShowWarning("From date cannot be later than To Date.");
                    return false;
                }

                //Check if leave is already given on or between selected dates.
                var dates = conn.Query<DateTime>("SELECT APPLIED_DATE FROM LEAVE_LEDGER WHERE ENO = @ENO AND APPLIED_DATE BETWEEN @FDATE AND @TDATE", new { ENO = LLedger.ENO, FDate = FDate, TDate = TDate });
                if (dates.Count() > 0)
                {
                    string Leave_Dates = string.Empty;
                    foreach (DateTime date in dates)
                        Leave_Dates += (string.IsNullOrEmpty(Leave_Dates) ? string.Empty : ", ") + date.ToString("MM/dd/yyyy");
                    ShowWarning("Selected Employee has already approved leave/s on " + Leave_Dates);
                    return false;
                }

                //Check if applied leaves is greater than remaining leave
                LeaveStatus ls = LeaveFunctions.GetLeaveStatus(LLedger.ENO, LLedger.LEAVE_ID).First();
                if (ls == null)
                {
                    ShowWarning("Selected leave is not yet given to the employee");
                    return false;
                }
                else if (TotalDays > ls.Remaining)
                {
                    ShowWarning("Applied leave days is greater than remaining leave days");
                    return false;
                }

                //Check if approved user is applying user
                if (LLedger.ENO == LLedger.APPROVEDBY)
                {
                    ShowWarning("Applying employee cannot approve his own leave application");
                    return false;
                }
            }
            return true;
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
                    Leaves = conn.Query<Leaves>("SELECT LEAVE_ID, LEAVE_NAME FROM LEAVES");
                    EmpLeaves = conn.Query<AllowedLeaves>("SELECT ENO, LEAVE_ID, ALLOWED_DAYS FROM EMP_ALLOWED_LEAVE");
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }
}
