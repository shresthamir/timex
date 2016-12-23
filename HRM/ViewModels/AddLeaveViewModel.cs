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
    class AddLeaveViewModel : RootViewModel
    {
        private ObservableCollection<Employee> _EmpList;
        private LEAVE_LEDGER _LLedger;
        private ObservableCollection<LEAVE_LEDGER> _EmployeeLeaveList;
        private IEnumerable<Leaves> Leaves;
        private ObservableCollection<Models.Leaves> _LeaveList;
        private IEnumerable<AllowedLeaves> EmpLeaves;

        public ObservableCollection<Leaves> LeaveList { get { return _LeaveList; } set { _LeaveList = value; OnPropertyChanged("LeaveList"); } }
        public LEAVE_LEDGER LLedger { get { return _LLedger; } set { _LLedger = value; OnPropertyChanged("LLedger"); } }
        public ObservableCollection<Employee> EmpList { get { return _EmpList; } set { _EmpList = value; OnPropertyChanged("EmpList"); } }
        public ObservableCollection<LEAVE_LEDGER> EmployeeLeaveList { get { return _EmployeeLeaveList; } set { _EmployeeLeaveList = value; OnPropertyChanged("EmployeeLeaveList"); } }
        public RelayCommand AddLeaveCommand { get { return new RelayCommand(AddLeave); } }

        private void AddLeave(object obj)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {

                    if (EmployeeLeaveList.Any(x => x.ENO == LLedger.ENO && x.LEAVE_ID == LLedger.LEAVE_ID))
                        return;
                    EmployeeLeaveList.Add(new LEAVE_LEDGER
                    {
                        ENO = LLedger.ENO,
                        ENAME = LLedger.ENAME,
                        LEAVE_ID = LLedger.LEAVE_ID,
                        LNAME = Leaves.First(x => x.LEAVE_ID == LLedger.LEAVE_ID).LEAVE_NAME,
                        Dr = LLedger.Dr,
                        REMARKS = LLedger.REMARKS
                    });

                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public AddLeaveViewModel()
        {
            try
            {
                LLedger = new LEAVE_LEDGER();
                LLedger.PropertyChanged += LLedger_PropertyChanged;
                LeaveList = new ObservableCollection<Models.Leaves>();
                EmployeeLeaveList = new ObservableCollection<LEAVE_LEDGER>();
                NewCommand = new RelayCommand(NewAction);
                SaveCommand = new RelayCommand(SaveAddLeave);
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

            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void SaveAddLeave(object obj)
        {
            try
            {
                if (ShowConfirmation("You are going to save Add Leave for month of.\nAdd Leave Transactions cannot be edited once saved.\nDo you want to Continue?"))
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
                                ll.TRANID = LLedger.TRANID;
                                ll.TRANDATE = LLedger.TRANDATE;
                                ll.TRAN_USER = AppVariables.LoggedUser;
                                ll.Save(tran);
                            }
                            tran.Commit();
                        }
                    }
                    ShowInformation("Add Leave successfully saved.");
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
