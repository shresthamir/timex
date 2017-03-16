using HRM.Library.BaseClasses;
using HRM.Library.Helpers;
using HRM.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using HRM.Library.AppScopeClasses;
using System.Windows;
using HRM.UI.Misc;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace HRM.ViewModels
{
    class ManualAttendanceViewModel : RootViewModel
    {
        private ManualAttendance _mAtt;
        private Employee _SelectedEmployee;
        private ObservableCollection<Employee> _EmpList;
        private ObservableCollection<ManualAttendance> _mAttList;
        private ManualAttendance _SelectedMAtt;
        public ManualAttendance mAtt { get { return _mAtt; } set { _mAtt = value; OnPropertyChanged("mAtt"); } }
        public ManualAttendance SelectedMAtt { get { return _SelectedMAtt; } set { _SelectedMAtt = value; } }
        public Employee SelectedEmployee { get { return _SelectedEmployee; } set { _SelectedEmployee = value; OnPropertyChanged("SelectedEmployee"); } }
        public ObservableCollection<Employee> EmpList { get { return _EmpList; } set { _EmpList = value; OnPropertyChanged("EmpList"); } }
        public ObservableCollection<ManualAttendance> mAttList { get { if (_mAttList == null) _mAttList = new ObservableCollection<ManualAttendance>(); return _mAttList; } set { _mAttList = value; OnPropertyChanged("mAttList"); } }

        public RelayCommand AddCommand { get { return new RelayCommand(ExecuteAdd, CanExecuteAdd); } }
        public RelayCommand EditGridItemCommand { get { return new RelayCommand(ExecuteGridItemEdit); } }

        private void ExecuteGridItemEdit(object obj)
        {
            //if (SelectedMAtt != null)
            //{
            //    ShowWarning("Cannot edit selected data until current data is processed. Please Re-add current entry.");
            //    return;
            //}
            mAtt = new ManualAttendance { 
                TRANID = SelectedMAtt.TRANID,
                ENO = SelectedMAtt.ENO,
                ENAME = SelectedMAtt.ENAME,
                ATT_DATE = SelectedMAtt.ATT_DATE,
                ATT_TYPE = SelectedMAtt.ATT_TYPE,
                ATT_TIME = SelectedMAtt.ATT_TIME,
                REMARKS = SelectedMAtt.REMARKS
            };
            mAttList.Remove(SelectedMAtt);
        }

        private bool CanExecuteAdd(object obj)
        {
            return mAtt.IsValid;
        }

        private void ExecuteAdd(object obj)
        {
            mAtt.ATT_TIME = mAtt.ATT_DATE.Add(mAtt.ATT_TIME.TimeOfDay);
            mAtt.ValidateModel();
            if (!mAtt.IsValid)
                return;
            string AttColumn;
            Attendance empAtt;
            try
            {
                if (mAttList.Any(x => x.ENO == mAtt.ENO && x.ATT_DATE == mAtt.ATT_DATE))
                {
                    ShowWarning("Only one transaction is allowed per employee per day. Please select another Employee or Attendance Date");
                    return;
                }
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    if (conn.ExecuteScalar<int>("SELECT COUNT(*) FROM MANUAL_ATT_LOG WHERE ENO = @ENO AND ATT_DATE = @ATT_DATE AND ATT_TYPE = @ATT_TYPE AND TRANID <> @TRANID", mAtt) > 0)
                    {
                        ShowWarning("Manual " + mAtt.ATT_TYPE + " for Selected Employe for selected date already exist. Cannot add " + mAtt.ATT_TYPE);
                        return;
                    }
                    empAtt = conn.Query<Attendance>("SELECT ENO, FYID, WORKHOUR_ID, ATT_DATE, CHECKIN, CHECKINMODE, CHECKINREMARKS, CHECKOUT, CHECKOUTMODE, CHECKOUTREMARKS, BREAKOUT, BREAKOUTMODE, BREAKOUTREMARKS, BREAKIN, BREAKINMODE, BREAKINREMARKS, POUT, POUTMODE, POUTREMARKS, PIN, PINMODE, PINREMARKS, OFFICEOUT, OFFICEOUTMODE, OFFICEOUTREMARKS, OFFICEIN, OFFICEINMODE, OFFICEINREMARKS FROM ATTENDANCE WHERE ENO = @ENO AND ATT_DATE = @ATT_DATE", mAtt).FirstOrDefault();
                }
                if (empAtt == null)
                {
                    if (mAtt.ATT_TYPE != "Sign In")
                    {
                        ShowWarning("Employe Sign in detail not found. Cannot add without Sign In detail.");
                        return;
                    }
                }
                else
                {
                    switch (mAtt.ATT_TYPE)
                    {
                        case "Sign In":
                            AttColumn = "CHECKIN";
                            break;
                        case "Lunch Out":
                            AttColumn = "BREAKOUT";
                            break;
                        case "Lunch In":
                            AttColumn = "BREAKIN";
                            break;
                        case "Break Out":
                            AttColumn = "BREAK1OUT";
                            break;
                        case "Break In":
                            AttColumn = "BREAK1IN";
                            break;
                        case "Sign Out":
                            AttColumn = "CHECKOUT";
                            break;
                        default:
                            AttColumn = string.Empty;
                            break;
                    }
                    if (!string.IsNullOrEmpty(AttColumn))
                    {
                        if (empAtt.GetType().GetProperty(AttColumn).GetValue(empAtt) != null)
                            if (!ShowConfirmation("Employee " + mAtt.ATT_TYPE + " already exists. Would you like to Override existing data?"))
                                return;
                    }

                    if (mAtt.ATT_TYPE != "Sign In" && mAtt.ATT_TIME < empAtt.CHECKIN)
                    {
                        ShowWarning(mAtt.ATT_TYPE + " cannot be early than Sign In time");
                        return;
                    }
                    else if (mAtt.ATT_TYPE == "Lunch In" && empAtt.LUNCHOUT == null)
                    {
                        ShowWarning("Employe Lunch Out information not found. Cannot add Lunch In without  Lunch Out.");
                        return;
                    }
                    else if (mAtt.ATT_TYPE == "Break In" && empAtt.BREAKOUT == null)
                    {
                        ShowWarning("Employee Break Out information not found. Cannot add Break In without Break Out.");
                        return;
                    }
                }
                mAttList.Add(new ManualAttendance { 
                    ENO = mAtt.ENO,
                    ENAME =mAtt.ENAME,
                    ATT_DATE = mAtt.ATT_DATE,
                    ATT_TIME = mAtt.ATT_TIME,
                    ATT_TYPE = mAtt.ATT_TYPE,
                    REMARKS =mAtt.REMARKS
                });
                SelectedMAtt = null;
                mAtt = new ManualAttendance();
                mAtt.PropertyChanged+=mAtt_PropertyChanged;

            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        public ManualAttendanceViewModel()
        {
            try
            {
                MessageBoxCaption = "Manual Attendance";
                mAtt = new ManualAttendance();
                mAtt.PropertyChanged += mAtt_PropertyChanged;
                NewCommand = new RelayCommand(NewAction);
                DeleteCommand = new RelayCommand(DeleteManualAttendance);
                ClearCommand = new RelayCommand(ClearInterface);
                SaveCommand = new RelayCommand(SaveManualAttendance);
                LoadData = new RelayCommand(LoadManualAttendanceInfo);
                LoadEmployeeList();
                this.PropertyChanged += ManualAttendanceViewModel_PropertyChanged;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        void mAtt_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ENO")
            {
                Employee emp = EmpList.FirstOrDefault(x => x.ENO == mAtt.ENO);
                if (emp != null && mAtt.ENAME != emp.FULLNAME)
                    mAtt.ENAME = EmpList.FirstOrDefault(x => x.ENO == mAtt.ENO).FULLNAME;               
            }
            else if (e.PropertyName == "ENAME")
            {
                Employee emp = EmpList.FirstOrDefault(x => x.FULLNAME == mAtt.ENAME);
                if (emp != null && mAtt.ENO != emp.ENO)
                    mAtt.ENO = EmpList.FirstOrDefault(x => x.FULLNAME == mAtt.ENAME).ENO;           
            }
        }

        void ManualAttendanceViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        private void LoadEmployeeList()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    EmpList = new ObservableCollection<Employee>(conn.Query<Employee>("SELECT 0 ENO, '' ECODE, '' FULLNAME UNION ALL SELECT ENO, ECODE, FULLNAME FROM EMPLOYEE"));

                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }


        private void LoadManualAttendanceInfo(object obj)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    if (obj == null)
                    {
                        var mAttes = conn.Query<ManualAttendance>("SELECT DISTINCT TRANID, TRANDATE, TRANUSER FROM MANUAL_ATT_LOG");
                        BrowseViewModel bvm = new BrowseViewModel(mAttes, "TRANID", "TRANDATE", "TRANUSER");
                        Browse browse = new Browse() { DataContext = bvm };
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("TRANID"), Header = "Tran Id", Width = 100 });
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("TRAN_USER"), Header = "Tran User", Width = 200 });
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("TRANDATE") {StringFormat="MM/dd/yyyy" }, Header = "Tran Date", Width = 100 });
                        browse.ShowDialog();
                        if (browse.DialogResult != true)
                            return;
                        mAtt.TRANID = (browse.SearchGrid.SelectedItem as ManualAttendance).TRANID;
                    }
                    else
                    {
                        mAtt.TRANID = short.Parse(obj.ToString());
                    }

                    var b = conn.Query<ManualAttendance>("SELECT TRANID,E.ENO, E.FULLNAME ENAME, TRANDATE, ATT_TYPE, ATT_DATE, ATT_TIME, TRAN_USER, REMARKS FROM MANUAL_ATT_LOG L JOIN EMPLOYEE E ON E.ENO = L.ENO WHERE TRANID = @TRANID", mAtt);
                    if (b != null && b.Count() > 0)
                    {
                        mAttList = new ObservableCollection<ManualAttendance>(b);
                        SetAction(ButtonAction.Selected);
                    }
                    else
                        ShowWarning("Invalid ManualAttendance Id");
                }
            }
            catch (Exception Ex)
            {
                ShowError(Ex.Message);
            }

        }

        private void DeleteManualAttendance(object obj)
        {
            if (ShowConfirmation("You are going to delete selected ManualAttendance. Do you want to continue?"))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            mAtt.Delete(tran);
                            tran.Commit();
                        }
                    }
                    ShowInformation("Manual Attendance successfully deleted.");
                    ClearInterface(null);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        private void ClearInterface(object obj)
        {
            mAtt = new ManualAttendance();
            mAtt.PropertyChanged += mAtt_PropertyChanged;
            mAttList.Clear();
            SetAction(ButtonAction.Init);
        }

        private void NewAction(object obj)
        {
            try
            {
                ClearInterface(null);
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    mAtt.TRANID = conn.ExecuteScalar<short>("SELECT ISNULL(MAX(TRANID),0) + 1 FROM MANUAL_ATT_LOG");
                }
                SetAction(ButtonAction.New);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void SaveManualAttendance(object obj)
        {
            int Tranid = 0;
            try
            {
                if (ShowConfirmation("You are going to " + ((_action == ButtonAction.New) ? "save new" : "update selected") + " ManualAttendance. Do you want to Continue?"))
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            if (_action == ButtonAction.Edit)
                            {
                                Tranid = mAttList.First().TRANID;
                                conn.Execute("DELETE FROM MANUAL_ATT_LOG WHERE TRANID = @TRANID", new { TRANID = Tranid }, tran);
                                
                            }
                            else if (_action == ButtonAction.New)
                            {
                                Tranid = conn.ExecuteScalar<int>("SELECT ISNULL(MAX(TRANID),0) + 1 FROM MANUAL_ATT_LOG",transaction: tran);
                            }
                            foreach (ManualAttendance ma in mAttList)
                            {
                                ma.TRANID = Tranid;
                                ma.TRAN_USER = AppVariables.LoggedUser;
                                ma.Save(tran);
                            }
                            tran.Commit();
                        }
                    }
                    ShowInformation("Manual Attendance successfully saved.");
                    ClearInterface(null);
                    System.Threading.Thread T = new System.Threading.Thread(new System.Threading.ThreadStart(this.RefreshAttData));
                    T.Start();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        void RefreshAttData()
        {
            using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    DataDownloadViewModel.RefreshAttendanceData(tran, EmpList);
                    tran.Commit();
                }
            }
        }

    }
}
