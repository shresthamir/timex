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
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using HRM.UI.Misc;
using System.Windows.Controls;
using System.Windows.Data;
namespace HRM.ViewModels
{
    class EmployeeViewModel : RootViewModel
    {
        private ObservableCollection<Branch> _BranchList;
        private ObservableCollection<Department> _DepartmentList;
        private ObservableCollection<Designation> _DesignationList;
        private ObservableCollection<string> _ModeList;
        private ObservableCollection<string> _MaritalStatusList;
        private ObservableCollection<string> _StatusList;
        private ObservableCollection<string> _TitleList;
        private Employee _emp;
        private EmployeePersonalInfo _empPInfo;
        private EmployeeAcademics _empAcademics;
        private EmployeeTraining _empTraining;
        private EmployeeExperience _empExp;
        private ObservableCollection<EmployeeAcademics> _Academics;

        public Employee emp { get { return _emp; } set { _emp = value; OnPropertyChanged("emp"); } }
        public DateTime EffectiveDate { get { return _EffectiveDate; } set { _EffectiveDate = value; OnPropertyChanged("EffectiveDate"); } }
        public EmployeeDetail empDetail
        {
            get { return _empDetail; }
            set
            {
                if (value != null)
                    _empDetail = value;
                else
                    _empDetail = new EmployeeDetail();
                OnPropertyChanged("empDetail");
            }
        }
        public bool IsDate { get { return _IsDate; } set { _IsDate = value; OnPropertyChanged("IsDate"); } }
        public EmployeePersonalInfo empPInfo { get { return _empPInfo; } set { _empPInfo = value; OnPropertyChanged("empPInfo"); } }
        public EmployeeAcademics empAcademics { get { return _empAcademics; } set { _empAcademics = value; OnPropertyChanged("empAcademics"); } }
        public EmployeeTraining empTraining { get { return _empTraining; } set { _empTraining = value; OnPropertyChanged("empTraining"); } }
        public EmployeeExperience empExp { get { return _empExp; } set { _empExp = value; OnPropertyChanged("empExp"); } }
        public BitmapImage Photo
        {
            get
            {
                var image = new BitmapImage();
                if (emp.PHOTO == null)
                    return image;
                using (var mem = new MemoryStream(emp.PHOTO))
                {
                    mem.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = null;
                    image.StreamSource = mem;
                    image.EndInit();
                }
                image.Freeze();
                return image;
            }
        }

        public ObservableCollection<Branch> BranchList { get { return _BranchList; } set { _BranchList = value; OnPropertyChanged("BranchList"); } }
        public ObservableCollection<Department> DepartmentList { get { return _DepartmentList; } set { _DepartmentList = value; OnPropertyChanged("DepartmentList"); } }
        public ObservableCollection<Designation> DesignationList { get { return _DesignationList; } set { _DesignationList = value; OnPropertyChanged("Designation"); } }
        public ObservableCollection<string> ModeList { get { return _ModeList; } set { _ModeList = value; OnPropertyChanged("ModeList"); } }
        public ObservableCollection<string> StatusList { get { return _StatusList; } set { _StatusList = value; OnPropertyChanged("StatusList"); } }
        public ObservableCollection<string> MaritalStatusList { get { return _MaritalStatusList; } set { _MaritalStatusList = value; OnPropertyChanged("MaritalStatusList"); } }
        public ObservableCollection<string> TitleList { get { return _TitleList; } set { _TitleList = value; OnPropertyChanged("TitleList"); } }
        public ObservableCollection<string> GenderList { get { return _GenderList; } set { _GenderList = value; OnPropertyChanged("GenderList"); } }
        public ObservableCollection<WeekDay> Weekdays { get { return _Weekdays; } set { _Weekdays = value; OnPropertyChanged("WeekDays"); } }
        public ObservableCollection<Leaves> LeaveList { get { return _LeaveList; } set { _LeaveList = value; OnPropertyChanged("LeaveList"); } }
        public ObservableCollection<EmployeeAcademics> Academics
        {
            get
            {
                if (_Academics == null)
                    _Academics = new ObservableCollection<EmployeeAcademics>();
                return _Academics;
            }
            set { _Academics = value; OnPropertyChanged("Academics"); }
        }
        public ObservableCollection<EmployeeTraining> Trainings
        {
            get
            {
                if (_Trainings == null)
                    _Trainings = new ObservableCollection<EmployeeTraining>();
                return _Trainings;
            }
            set { _Trainings = value; OnPropertyChanged("Trainings"); }
        }
        public ObservableCollection<EmployeeExperience> Experiences
        {
            get
            {
                if (_Experiences == null)
                    _Experiences = new ObservableCollection<EmployeeExperience>();
                return _Experiences;
            }
            set { _Experiences = value; OnPropertyChanged("Experiences"); }
        }

        public RelayCommand AddAcademicsCommand { get { return new RelayCommand(AddAcademics, CanAddAcademics); } }
        public RelayCommand AddExpCommand { get { return new RelayCommand(AddExperience, CanAddExperience); } }
        public RelayCommand AddTrainingCommand { get { return new RelayCommand(AddTraining, CanAddTraining); } }
        public RelayCommand BrowseImageCommand { get { return new RelayCommand(BrowseImage); } }
        public RelayCommand RefreshLeaveCommand { get { return new RelayCommand(RefreshLeave); } }

        private void RefreshLeave(object obj)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    LeaveList = new ObservableCollection<Leaves>(conn.Query<Leaves>("SELECT LEAVE_ID, LEAVE_NAME, ANNUALLEAVECOUNT, ANNUALLEAVECOUNT AllowedDays FROM LEAVES"));
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void BrowseImage(object obj)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.ShowDialog();
            if (!string.IsNullOrEmpty(ofd.FileName))
            {
                Stream reader = File.OpenRead(ofd.FileName);
                byte[] bytes = new byte[reader.Length];
                reader.Read(bytes, 0, (int)reader.Length);
                emp.PHOTO = bytes;
                OnPropertyChanged("Photo");
            }
        }

        private bool CanAddTraining(object obj)
        {
            return empTraining.IsValid;
        }

        private void AddTraining(object obj)
        {
            Trainings.Add(new EmployeeTraining { COURSE = empTraining.COURSE, INSTITUTE = empTraining.INSTITUTE, DURATION = empTraining.DURATION });
            empTraining = new EmployeeTraining();
        }

        private bool CanAddExperience(object obj)
        {
            return empExp.IsValid;
        }

        private void AddExperience(object obj)
        {
            Experiences.Add(
                new EmployeeExperience
                {
                    CNAME = empExp.CNAME,
                    CADDRESS = empExp.CADDRESS,
                    DESIGNATION = empExp.DESIGNATION,
                    FDATE = empExp.FDATE,
                    TDATE = empExp.TDATE
                });
            empExp = new EmployeeExperience();
        }

        private bool CanAddAcademics(object obj)
        {
            return empAcademics.IsValid;
        }

        private void AddAcademics(object obj)
        {
            Academics.Add(
                new EmployeeAcademics
                {
                    BOARD = empAcademics.BOARD,
                    INSTITUTE = empAcademics.INSTITUTE,
                    DEGREENAME = empAcademics.DEGREENAME,
                    PASSEDYEAR = empAcademics.PASSEDYEAR
                });
            empAcademics = new EmployeeAcademics();
        }




        public EmployeeViewModel()
        {
            MessageBoxCaption = "Employee Registration";
            emp = new Employee();
            empDetail = new EmployeeDetail();
            empPInfo = new EmployeePersonalInfo();
            empAcademics = new EmployeeAcademics();
            empTraining = new EmployeeTraining();
            empExp = new EmployeeExperience();
            LoadAllLists();
            NewCommand = new RelayCommand(ExecuteNew);
            SaveCommand = new RelayCommand(ExecuteSave);
            ClearCommand = new RelayCommand(ClearForm);
            LoadData = new RelayCommand(LoadEmpInfo);
            EditCommand = new RelayCommand(ExecuteEdit);
            DeleteCommand = new RelayCommand(ExecuteDelete);
        }

        private void ExecuteDelete(object obj)
        {
            if (ShowConfirmation("You are going to delete selected Employee. Do you want to continue?"))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            empPInfo.Delete(tran);
                            conn.Execute
                                  (
                                      @"DELETE FROM EMPLOYEE_ACADEMICS WHERE ENO = @ENO;
                                          DELETE FROM EMPLOYEE_TRANING WHERE ENO = @ENO;
                                          DELETE FROM EMPLOYEE_DETAIL WHERE ENO = @ENO                                            
                                          DELETE FROM EMPLOYEE_EMPLOYMENT_HISTORY WHERE ENO = @ENO", emp, tran
                                  );
                            tran.Commit();
                            emp.Delete(tran);
                        }
                    }
                    ShowInformation("Employee successfully deleted.");
                    ClearForm(null);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        private void ExecuteEdit(object obj)
        {
            SetAction(ButtonAction.Edit);
        }

        private void LoadEmpInfo(object obj)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    if (obj == null)
                    {
                        var employee = conn.Query<Employee>("SELECT ENO, ECODE, FULLNAME FROM EMPLOYEE");
                        BrowseViewModel bvm = new BrowseViewModel(employee, "ENO", "ECODE", "FULLNAME");
                        Browse browse = new Browse() { DataContext = bvm };
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("ENO"), Header = "ENO", Width = 100 });
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("ECODE"), Header = "Employee Code", Width = 120 });
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("ECODE"), Header = "Employee Name", Width = 200 });
                        browse.ShowDialog();
                        if (browse.DialogResult != true)
                            return;
                        emp.ENO = (browse.SearchGrid.SelectedItem as Employee).ENO;
                    }
                    else
                    {
                        emp.ENO = short.Parse(obj.ToString());
                    }

                    var b = conn.Query<Employee>("SELECT ENO, ECODE, FULLNAME, PHOTO, TITLE, CALENDAR_TYPE FROM EMPLOYEE WHERE ENO = @ENO", emp);
                    if (b != null && b.Count() > 0)
                    {
                        emp = b.FirstOrDefault();
                        IsDate = emp.CALENDAR_TYPE == "AD";
                        empDetail = conn.Query<EmployeeDetail>("SELECT EMP_TRANID, DEPARTMENT_ID, DESIGNATION_ID, BRANCH_ID, MARITAL_STATUS, CMODE, [STATUS], ENO FROM EMPLOYEE_DETAIL WHERE ENO = @ENO AND EMP_TRANID = (SELECT ISNULL(MAX(EMP_TRANID),0) FROM EMPLOYEE_DETAIL WHERE ENO = @ENO)", emp).FirstOrDefault();
                        empPInfo = conn.Query<EmployeePersonalInfo>("SELECT ENO, P_ADDRESS, T_ADDRESS, MOBILE, PHONE, CTZNO, DOB, EMAIL, GENDER, JOINDATE, CPERSON, CCONTACTNO, CRELATION FROM EMPLOYEE_PERSONAL_INFO WHERE ENO = @ENO", emp).FirstOrDefault();
                        Academics = new ObservableCollection<EmployeeAcademics>(conn.Query<EmployeeAcademics>("SELECT ENO, BOARD, INSTITUTE, PASSEDYEAR, DEGREENAME FROM EMPLOYEE_ACADEMICS WHERE ENO = @ENO", emp));
                        Trainings = new ObservableCollection<EmployeeTraining>(conn.Query<EmployeeTraining>("SELECT ENO, COURSE, INSTITUTE, DURATION FROM EMPLOYEE_TRAINING WHERE ENO = @ENO", emp));
                        Experiences = new ObservableCollection<EmployeeExperience>(conn.Query<EmployeeExperience>("SELECT ENO, CNAME, CADDRESS, FDATE, TDATE, DESIGNATION FROM EMPLOYEE_EMPLOYMENT_HISTORY WHERE ENO = @ENO", emp));
                        foreach (Weekend w in conn.Query<Weekend>("SELECT ENO, DAY_ID, EFFECTIVE_DATE FROM EMPLOYEE_WEEKEND WHERE ENO = @ENO AND LAST_DATE IS NULL", emp))
                        {
                            Weekdays.First(x => x.DAY_ID == w.DAY_ID).IsSelected = true;
                        }
                        foreach (AllowedLeaves al in conn.Query<AllowedLeaves>("SELECT ENO, LEAVE_ID, ALLOWED_DAYS FROM EMP_ALLOWED_LEAVE WHERE ENO = @ENO", emp))
                        {
                            var l = LeaveList.First(x => x.LEAVE_ID == al.LEAVE_ID);
                            l.IsSelected = true;
                            l.AllowedDays = al.ALLOWED_DAYS;
                        }
                        OnPropertyChanged("Photo");
                        SetAction(ButtonAction.Selected);
                    }
                    else
                        ShowWarning("Invalid Employee No");
                }
            }
            catch (Exception Ex)
            {
                ShowError(Ex.Message);
            }
        }

        private void ClearForm(object obj)
        {
            emp = new Employee();
            empDetail = new EmployeeDetail();
            empPInfo = new EmployeePersonalInfo();
            empExp = new EmployeeExperience();
            empAcademics = new EmployeeAcademics();
            empTraining = new EmployeeTraining();
            foreach (WeekDay w in Weekdays)
                w.IsSelected = false;
            foreach (Leaves l in LeaveList)
            {
                l.IsSelected = false;
                l.AllowedDays = l.ANNUALLEAVECOUNT;
            }
            Academics.Clear();
            Trainings.Clear();
            Experiences.Clear();
            SetAction(ButtonAction.Init);
            OnPropertyChanged("Photo");
        }

        private void ExecuteSave(object obj)
        {
            try
            {
                if (!ShowConfirmation("You are going to " + ((_action == ButtonAction.New) ? "save new" : "update selected") + " Employee. Do you want to Continue?"))
                    return;
                if (emp.IsValid && empPInfo.IsValid)
                {
                    emp.CALENDAR_TYPE = IsDate ? "AD" : "BS";
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            if (_action == ButtonAction.New)
                            {
                                SetEmpNo(conn, tran);
                                emp.Save(tran);
                                empDetail.ENO = emp.ENO;
                                empDetail.Save(tran);
                                empPInfo.ENO = emp.ENO;
                                empPInfo.Save(tran);
                            }
                            else
                            {
                                var empDetailDb = conn.Query<EmployeeDetail>("SELECT EMP_TRANID, DEPARTMENT_ID, DESIGNATION_ID, BRANCH_ID, MARITAL_STATUS, CMODE, [STATUS], ENO FROM EMPLOYEE_DETAIL WHERE ENO = @ENO AND EMP_TRANID = (SELECT ISNULL(MAX(EMP_TRANID),0) FROM EMPLOYEE_DETAIL WHERE ENO = @ENO)", emp, tran).FirstOrDefault();
                                if (empDetailDb == null || !empDetail.Compare(empDetailDb))
                                {
                                    empDetail.ENO = emp.ENO;
                                    empDetail.Save(tran);
                                }
                                emp.Update(tran);
                                empPInfo.Update(tran);
                                conn.Execute
                                    (
                                        @"DELETE FROM EMPLOYEE_ACADEMICS WHERE ENO = @ENO;
                                          DELETE FROM EMPLOYEE_TRAINING WHERE ENO = @ENO;
                                          DELETE FROM EMPLOYEE_EMPLOYMENT_HISTORY WHERE ENO = @ENO
                                          DELETE FROM EMP_ALLOWED_LEAVE WHERE ENO =@ENO", emp, tran
                                    );


                                if (conn.ExecuteScalar<int>("SELECT COUNT(*) FROM EMPLOYEE_WEEKEND WHERE ENO = @ENO AND EFFECTIVE_DATE = @EFFECTIVE_DATE", new Weekend { ENO = emp.ENO, EFFECTIVE_DATE = EffectiveDate }, tran) > 0)
                                {
                                    conn.Execute("DELETE FROM EMPLOYEE_WEEKEND WHERE ENO = @ENO AND EFFECTIVE_DATE = @EFFECTIVE_DATE", new Weekend { ENO = emp.ENO, EFFECTIVE_DATE = EffectiveDate }, tran);
                                }
                                else
                                    conn.Execute("UPDATE EMPLOYEE_WEEKEND SET LAST_DATE = @LAST_DATE WHERE ENO = @ENO AND LAST_DATE IS NULL", new Weekend { ENO = emp.ENO, LAST_DATE = EffectiveDate.Subtract(new TimeSpan(1, 0, 0, 0)) }, tran);
                            }
                            foreach (Leaves l in LeaveList.Where(x => x.IsSelected))
                            {
                                if (!l.IsValid)
                                {
                                    ShowError(l.Error);
                                    return;
                                }
                                AllowedLeaves al = new AllowedLeaves { ENO = emp.ENO, LEAVE_ID = l.LEAVE_ID, ALLOWED_DAYS = l.AllowedDays };
                                al.Save(tran);
                            }

                            foreach (WeekDay wd in Weekdays.Where(x => x.IsSelected))
                            {
                                Weekend w = new Weekend { ENO = emp.ENO, DAY_ID = wd.DAY_ID, EFFECTIVE_DATE = EffectiveDate };
                                w.Save(tran);
                            }

                            foreach (EmployeeAcademics ea in Academics)
                            {
                                ea.ENO = emp.ENO;
                                ea.Save(tran);
                            }
                            foreach (EmployeeTraining et in Trainings)
                            {
                                et.ENO = emp.ENO;
                                et.Save(tran);
                            }
                            foreach (EmployeeExperience ee in Experiences)
                            {
                                ee.ENO = emp.ENO;
                                ee.Save(tran);
                            }

                            tran.Commit();
                            ClearForm(null);
                            ShowInformation("Employee Information successfully Saved.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        void SetEmpNo(SqlConnection conn, SqlTransaction Tran = null)
        {
            if (emp.ENO > 0)
            {
                if (conn.ExecuteScalar<int>("SELECT COUNT(*) FROM EMPLOYEE WHERE ENO = @ENO", emp, transaction: Tran) > 0)
                {
                    if (!ShowConfirmation("Employee Number already exists. Would you like the software to generate Employee Number for you?"))
                        return;
                }
                else
                {
                    SetAction(ButtonAction.New);
                    return;
                }
            }
            emp.ENO = conn.ExecuteScalar<short>("SELECT CAST(ISNULL(MAX(ENO),0) + 1 AS SMALLINT) FROM EMPLOYEE", emp, transaction: Tran);
        }

        private void ExecuteNew(object obj)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    foreach (string dw in AppVariables.DefaultWeekend)
                        Weekdays.First(x => x.DAY_ID.ToString() == dw).IsSelected = true;
                    SetEmpNo(conn);
                    SetAction(ButtonAction.New);
                }
            }
            catch (Exception Ex)
            {
                ShowError(Ex.Message);
            }
        }

        void LoadAllLists(SqlConnection conn = null)
        {
            bool IsConnSupplied = conn != null;
            if (!IsConnSupplied)
            {
                conn = new SqlConnection(AppVariables.ConnectionString);
                conn.Open();
            }

            BranchList = new ObservableCollection<Branch>(conn.Query<Branch>("SELECT BRANCH_ID, BRANCH_NAME FROM BRANCH"));
            DepartmentList = new ObservableCollection<Department>(conn.Query<Department>("SELECT DEPARTMENT_ID, DEPARTMENT FROM DEPARTMENT"));
            DesignationList = new ObservableCollection<Designation>(conn.Query<Designation>("SELECT DESIGNATION_ID, DESIGNATION FROM DESIGNATION"));
            ModeList = new ObservableCollection<string>(conn.Query<string>("SELECT MODE FROM CONTRACTMODE"));
            StatusList = new ObservableCollection<string>(conn.Query<string>("SELECT STATUS FROM WORKINGSTATUS"));
            TitleList = new ObservableCollection<string>(conn.Query<string>("SELECT TITLE FROM TITLES"));
            GenderList = new ObservableCollection<string>(conn.Query<string>("SELECT GENDER FROM GENDER"));
            MaritalStatusList = new ObservableCollection<string>(conn.Query<string>("SELECT STATUS FROM MARITALSTATUS"));
            Weekdays = new ObservableCollection<WeekDay>(conn.Query<WeekDay>("SELECT DAY_ID, [WEEKDAY] FROM WEEKDAYS"));
            LeaveList = new ObservableCollection<Leaves>(conn.Query<Leaves>("SELECT LEAVE_ID, LEAVE_NAME, ANNUALLEAVECOUNT, ANNUALLEAVECOUNT AllowedDays FROM LEAVES"));

            if (!IsConnSupplied)
                conn.Close();
        }


        public ObservableCollection<string> _GenderList;
        private ObservableCollection<EmployeeTraining> _Trainings;
        private ObservableCollection<EmployeeExperience> _Experiences;
        private EmployeeDetail _empDetail;
        private DateTime _EffectiveDate = DateTime.Today;
        private ObservableCollection<WeekDay> _Weekdays;
        private ObservableCollection<Leaves> _LeaveList;
        private bool _IsDate = (SETTING.DEFAULT_CALENDAR == "AD");

    }
}
