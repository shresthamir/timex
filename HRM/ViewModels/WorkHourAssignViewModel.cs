using HRM.Library.AppScopeClasses;
using HRM.Library.BaseClasses;
using HRM.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Windows;
using System.Collections.ObjectModel;
namespace HRM.ViewModels
{
    class WorkHourAssignViewModel : RootViewModel
    {
        private IEnumerable<Branch> _BranchList;
        private IEnumerable<Department> _DepartmentList;
        private IEnumerable<WorkHour> _WorkHourList;
        private IEnumerable<Employee> _EmpList;
        private DateTime _EffectiveDate = DateTime.Today;
        private Employee _SelectedEmployee;
        private bool _BranchWise = true;
        private bool _DepartmentWise;
        private bool _EmployeeWise;
        private Branch _SelectedBranch;
        private Department _SelectedDepartment;
        private List<Day> _Days;
        private bool _AllDays;
        private WorkHour _SelectedWorkhour;
        private ObservableCollection<EmployeeWorkhour> _EmployeeWorkhourList;
        private List<DBEmployeeWorkhour> EWList;

        public Department SelectedDepartment { get { return _SelectedDepartment; } set { _SelectedDepartment = value; OnPropertyChanged("SelectedDepartment"); } }
        public Branch SelectedBranch { get { return _SelectedBranch; } set { _SelectedBranch = value; OnPropertyChanged("SelectedBranch"); } }
        public bool BranchWise { get { return _BranchWise; } set { _BranchWise = value; OnPropertyChanged("BranchWise"); } }
        public bool DepartmentWise { get { return _DepartmentWise; } set { _DepartmentWise = value; OnPropertyChanged("DepartmentWise"); } }
        public bool EmployeeWise { get { return _EmployeeWise; } set { _EmployeeWise = value; OnPropertyChanged("EmployeeWise"); } }
        public DateTime EffectiveDate { get { return _EffectiveDate; } set { _EffectiveDate = value; OnPropertyChanged("EffectiveDate"); } }
        public IEnumerable<Branch> BranchList { get { return _BranchList; } set { _BranchList = value; OnPropertyChanged("BranchList"); } }
        public IEnumerable<Department> DepartmentList { get { return _DepartmentList; } set { _DepartmentList = value; OnPropertyChanged("DepartmentList"); } }
        public IEnumerable<WorkHour> WorkHourList { get { return _WorkHourList; } set { _WorkHourList = value; OnPropertyChanged("WorkHourList"); } }
        public IEnumerable<Employee> EmpList { get { return _EmpList; } set { _EmpList = value; OnPropertyChanged("EmpList"); } }
        public Employee SelectedEmployee { get { return _SelectedEmployee; } set { _SelectedEmployee = value; OnPropertyChanged("SelectedEmployee"); } }
        public WorkHour SelectedWorkhour { get { return _SelectedWorkhour; } set { _SelectedWorkhour = value; OnPropertyChanged("SelectedWorkhour"); } }
        public ObservableCollection<EmployeeWorkhour> EmployeeWorkhourList { get { return _EmployeeWorkhourList; } set { _EmployeeWorkhourList = value; OnPropertyChanged("EmployeeWorkhourList"); } }
        public bool AllDays
        {
            get { return _AllDays; }
            set
            {
                _AllDays = value;
                OnPropertyChanged("AllDays");
                if (Days != null)
                {
                    foreach (Day d in Days)
                    {
                        d.IsChecked = value;
                    }
                }

            }
        }

        public IEnumerable<Day> Days
        {
            get
            {
                if (_Days == null)
                {
                    _Days = new List<Day>();
                    _Days.Add(new Day { DayId = 1, DayName = "Sunday" });
                    _Days.Add(new Day { DayId = 2, DayName = "Monday" });
                    _Days.Add(new Day { DayId = 3, DayName = "Tuesday" });
                    _Days.Add(new Day { DayId = 4, DayName = "Wednesday" });
                    _Days.Add(new Day { DayId = 5, DayName = "Thursday" });
                    _Days.Add(new Day { DayId = 6, DayName = "Friday" });
                    _Days.Add(new Day { DayId = 7, DayName = "Saturday" });
                }
                foreach (Day d in _Days)
                {
                    d.PropertyChanged += d_PropertyChanged;
                }
                return _Days;
            }
        }

        public HRM.Library.Helpers.RelayCommand AddCommand { get { return new Library.Helpers.RelayCommand(ExecuteAdd); } }



        void d_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked" && !(sender as Day).IsChecked && AllDays)
            {
                _AllDays = false;
                OnPropertyChanged("AllDays");
            }
        }

        public WorkHourAssignViewModel()
        {
            try
            {
                MessageBoxCaption = "Working Hour Assignment";
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    EmpList = conn.Query<Employee>("SELECT ENO, FULLNAME, CALENDAR_TYPE FROM EMPLOYEE");
                    WorkHourList = conn.Query<WorkHour>("SELECT WORKHOUR_ID, [DESCRIPTION], INTIME, OUTTIME, ISDEFAULT, INGRACETIME, OUTGRACETIME, LUNCHDURATION, BREAKDURATION, TOTALDURATION FROM WORKHOUR");
                    BranchList = conn.Query<Branch>("SELECT BRANCH_ID, BRANCH_NAME FROM BRANCH");
                    DepartmentList = conn.Query<Department>("SELECT DEPARTMENT_ID, DEPARTMENT FROM DEPARTMENT");
                }
                NewCommand = new Library.Helpers.RelayCommand(ExecuteNew);
                EWList = new List<DBEmployeeWorkhour>();
                EmployeeWorkhourList = new ObservableCollection<EmployeeWorkhour>();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteNew(object obj)
        {
            SetAction(ButtonAction.New);
        }

        private void ExecuteAdd(object obj)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    if (BranchWise || DepartmentWise)
                    {
                        string strEmpQry =
@"SELECT ENO, ENAME FROM
(
	SELECT ED.ENO, FULLNAME ENAME, BRANCH_ID, DEPARTMENT_ID FROM  EMPLOYEE_DETAIL ED 
	JOIN EMPLOYEE E ON ED.ENO = E.ENO
	JOIN 
	(
		SELECT ENO,  MAX(EMP_TRANID) EMP_TRANID FROM EMPLOYEE_DETAIL GROUP BY ENO
	) ED1 ON ED.ENO = ED1.ENO AND ED.EMP_TRANID = ED1.EMP_TRANID	
) A WHERE DEPARTMENT_ID LIKE '" + (DepartmentWise ? SelectedDepartment.DEPARTMENT_ID.ToString() : "%") + "' AND BRANCH_ID LIKE '" + (BranchWise ? SelectedBranch.BRANCH_ID.ToString() : "%") + "'";

                        foreach (var Emp in conn.Query<dynamic>(strEmpQry))
                        {
                            EmployeeWorkhour ew = new EmployeeWorkhour()
                            {
                                ENO = Emp.ENO,
                                WORKHOUR_ID = SelectedWorkhour.WORKHOUR_ID,
                                EFFECTIVE_DATE = EffectiveDate,
                                Days =string.Empty,
                                ENAME = Emp.ENAME,
                                WHName = SelectedWorkhour.DESCRIPTION
                            };
                            foreach (Day d in Days.Where(x => x.IsChecked))
                            {
                                EWList.Add(new DBEmployeeWorkhour()
                                {
                                    ENO = Emp.ENO,
                                    DAY_ID = d.DayId,
                                    WORKHOUR_ID = SelectedWorkhour.WORKHOUR_ID,
                                    EFFECTIVE_DATE = EffectiveDate
                                });
                                ew.Days+=d.DayName + ";";
                            }
                            EmployeeWorkhourList.Add(ew);
                        }
                    }

                }
            }
            catch (Exception)
            {

                
            }
        }
    }
}
