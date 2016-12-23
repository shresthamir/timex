using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.ComponentModel;
using System.Reflection;
namespace HRM.Models
{
    class UserManagement : RootModel, IDataErrorInfo
    {
        #region Members
        private string _UNAME;
        private string _DESCRIPTION;
        private string _PWD;
        private string _CATEGORY;
        private bool _USERMANAGEMENT;
        private bool _BRANCH;
        private bool _DEPARTMENT;
        private bool _DESIGNATION;
        private bool _LEAVE;
        private bool _HOLIDAY;
        private bool _WORKHOUR;
        private bool _EMPLOYEE_REGISTRATION;
        private bool _MANUAL_ATTENDANCE;
        private bool _MONTHLY_LEAVE;
        private bool _ADD_LEAVE;
        private bool _LEAVE_APPLICATION;
        private bool _ATTENDANCE_REPORTS;
        private bool _MONTHLY_ATTENDANCE_DETAIL;
        private bool _MONTHLY_ATTENDANCE_SUMMARY;
        private bool _ATTENDANCE_BOOK;
        private bool _DAILY_ATTENDANCE;
        private bool _ANNUAL_ATTENDANCE;
        private bool _DAILY_ABSENT;
        private bool _MANUAL_ATTENDANCE_REPORT;
        private bool _LEAVE_REPORTS;
        private bool _LEAVE_STATUS;
        private bool _LEAVE_STATEMENT;
        private bool _MONTHLY_LEAVE_REPORT;
        private bool _ANNUAL_LEAVE;
        private bool _EMPLOYEE_LIST;
        private bool _EMPLOYEE_DETAIL;
        private bool _MONTHLY_LATE_REPORT;
        private bool _ANNUAL_HOLIDAY_LIST;
        private bool _ATTENDANCE_LOG;
        private bool _DAILY_ATTENDANCE_LOG;
        private bool _MONTHLY_ATTENDANCE_LOG;
        private bool _COMPANY_INFO;
        private bool _DOWNLOAD_ATTENDANCE_DATA;
        private bool _DATA_BACKUP;
        private bool _DATA_RESTORE;
        private bool _DEVICE_SETTING;
        private string _CPWD;
        private bool _INACTIVE;
        private bool _ASSIGN_WORKHOUR;
        #endregion

        #region Properties
        public string UNAME { get { return _UNAME; } set { _UNAME = value; OnPropertyChanged("UNAME"); } }
        public string DESCRIPTION { get { return _DESCRIPTION; } set { _DESCRIPTION = value; OnPropertyChanged("DESCRIPTION"); } }
        public string PWD { get { return _PWD; } set { _PWD = value; OnPropertyChanged("PWD"); } }
        public string CPWD { get { return _CPWD; } set { _CPWD = value; OnPropertyChanged("CPWD"); } }
        public string CATEGORY { get { return _CATEGORY; } set { _CATEGORY = value; OnPropertyChanged("CATEGORY"); } }
        public bool USERMANAGEMENT { get { return _USERMANAGEMENT; } set { _USERMANAGEMENT = value; OnPropertyChanged("USERMANAGEMENT"); } }
        public bool INACTIVE { get { return _INACTIVE; } set { _INACTIVE = value; OnPropertyChanged("INACTIVE"); } }
        public bool MASTER { get { return _BRANCH || _DEPARTMENT || _DESIGNATION || _HOLIDAY || _LEAVE || _WORKHOUR; } }
        public bool BRANCH { get { return _BRANCH; } set { _BRANCH = value; OnPropertyChanged("BRANCH"); } }
        public bool DEPARTMENT { get { return _DEPARTMENT; } set { _DEPARTMENT = value; OnPropertyChanged("DEPARTMENT"); } }
        public bool DESIGNATION { get { return _DESIGNATION; } set { _DESIGNATION = value; OnPropertyChanged("DESIGNATION"); } }
        public bool LEAVE { get { return _LEAVE; } set { _LEAVE = value; OnPropertyChanged("LEAVE"); } }
        public bool HOLIDAY { get { return _HOLIDAY; } set { _HOLIDAY = value; OnPropertyChanged("HOLIDAY"); } }
        public bool WORKHOUR { get { return _WORKHOUR; } set { _WORKHOUR = value; OnPropertyChanged("WORKHOUR"); } }
        public bool TASKS { get { return _MANUAL_ATTENDANCE || _EMPLOYEE_REGISTRATION || _LEAVE_APPLICATION || MONTHLY_LEAVE || _ADD_LEAVE; } }
        public bool EMPLOYEE_REGISTRATION { get { return _EMPLOYEE_REGISTRATION; } set { _EMPLOYEE_REGISTRATION = value; OnPropertyChanged("EMPLOYEE_REGISTRATION"); } }
        public bool MANUAL_ATTENDANCE { get { return _MANUAL_ATTENDANCE; } set { _MANUAL_ATTENDANCE = value; OnPropertyChanged("MANUAL_ATTENDANCE"); } }
        public bool MONTHLY_LEAVE { get { return _MONTHLY_LEAVE; } set { _MONTHLY_LEAVE = value; OnPropertyChanged("MONTHLY_LEAVE"); } }
        public bool ADD_LEAVE { get { return _ADD_LEAVE; } set { _ADD_LEAVE = value; OnPropertyChanged("ADD_LEAVE"); } }
        public bool LEAVE_APPLICATION { get { return _LEAVE_APPLICATION; } set { _LEAVE_APPLICATION = value; OnPropertyChanged("LEAVE_APPLICATION"); } }
        public bool REPORTS { get { return _ATTENDANCE_REPORTS || _LEAVE_REPORTS || _EMPLOYEE_LIST || EMPLOYEE_DETAIL || MONTHLY_LATE_REPORT || ATTENDANCE_LOG || ANNUAL_HOLIDAY_LIST; } }
        public bool ATTENDANCE_REPORTS { get { return _ATTENDANCE_REPORTS; } set { _ATTENDANCE_REPORTS = value; OnPropertyChanged("ATTENDANCE_REPORTS"); } }
        public bool MONTHLY_ATTENDANCE_DETAIL { get { return _MONTHLY_ATTENDANCE_DETAIL; } set { _MONTHLY_ATTENDANCE_DETAIL = value; OnPropertyChanged("MONTHLY_ATTENDANCE_DETAIL"); } }
        public bool MONTHLY_ATTENDANCE_SUMMARY { get { return _MONTHLY_ATTENDANCE_SUMMARY; } set { _MONTHLY_ATTENDANCE_SUMMARY = value; OnPropertyChanged("MONTHLY_ATTENDANCE_SUMMARY"); } }
        public bool ATTENDANCE_BOOK { get { return _ATTENDANCE_BOOK; } set { _ATTENDANCE_BOOK = value; OnPropertyChanged("ATTENDANCE_BOOK"); } }
        public bool DAILY_ATTENDANCE { get { return _DAILY_ATTENDANCE; } set { _DAILY_ATTENDANCE = value; OnPropertyChanged("DAILY_ATTENDANCE"); } }
        public bool ANNUAL_ATTENDANCE { get { return _ANNUAL_ATTENDANCE; } set { _ANNUAL_ATTENDANCE = value; OnPropertyChanged("ANNUAL_ATTENDANCE"); } }
        public bool DAILY_ABSENT { get { return _DAILY_ABSENT; } set { _DAILY_ABSENT = value; OnPropertyChanged("DAILY_ABSENT"); } }
        public bool MANUAL_ATTENDANCE_REPORT { get { return _MANUAL_ATTENDANCE_REPORT; } set { _MANUAL_ATTENDANCE_REPORT = value; OnPropertyChanged("MANUAL_ATTENDANCE_REPORT"); } }
        public bool LEAVE_REPORTS { get { return _LEAVE_REPORTS; } set { _LEAVE_REPORTS = value; OnPropertyChanged("LEAVE_REPORTS"); } }
        public bool LEAVE_STATUS { get { return _LEAVE_STATUS; } set { _LEAVE_STATUS = value; OnPropertyChanged("LEAVE_STATUS"); } }
        public bool LEAVE_STATEMENT { get { return _LEAVE_STATEMENT; } set { _LEAVE_STATEMENT = value; OnPropertyChanged("LEAVE_STATEMENT"); } }
        public bool MONTHLY_LEAVE_REPORT { get { return _MONTHLY_LEAVE_REPORT; } set { _MONTHLY_LEAVE_REPORT = value; OnPropertyChanged("MONTHLY_LEAVE_REPORT"); } }
        public bool ANNUAL_LEAVE { get { return _ANNUAL_LEAVE; } set { _ANNUAL_LEAVE = value; OnPropertyChanged("ANNUAL_LEAVE"); } }
        public bool EMPLOYEE_LIST { get { return _EMPLOYEE_LIST; } set { _EMPLOYEE_LIST = value; OnPropertyChanged("EMPLOYEE_LIST"); } }
        public bool EMPLOYEE_DETAIL { get { return _EMPLOYEE_DETAIL; } set { _EMPLOYEE_DETAIL = value; OnPropertyChanged("EMPLOYEE_DETAIL"); } }
        public bool MONTHLY_LATE_REPORT { get { return _MONTHLY_LATE_REPORT; } set { _MONTHLY_LATE_REPORT = value; OnPropertyChanged("MONTHLY_LATE_REPORT"); } }
        public bool ANNUAL_HOLIDAY_LIST { get { return _ANNUAL_HOLIDAY_LIST; } set { _ANNUAL_HOLIDAY_LIST = value; OnPropertyChanged("ANNUAL_HOLIDAY_LIST"); } }
        public bool ATTENDANCE_LOG { get { return _ATTENDANCE_LOG; } set { _ATTENDANCE_LOG = value; OnPropertyChanged("ATTENDANCE_LOG"); } }
        public bool DAILY_ATTENDANCE_LOG { get { return _DAILY_ATTENDANCE_LOG; } set { _DAILY_ATTENDANCE_LOG = value; OnPropertyChanged("DAILY_ATTENDANCE_LOG"); } }
        public bool MONTHLY_ATTENDANCE_LOG { get { return _MONTHLY_ATTENDANCE_LOG; } set { _MONTHLY_ATTENDANCE_LOG = value; OnPropertyChanged("MONTHLY_ATTENDANCE_LOG"); } }
        public bool UTILITIES { get { return _DOWNLOAD_ATTENDANCE_DATA || _DATA_BACKUP || _DATA_RESTORE || _DEVICE_SETTING; } }
        public bool COMPANY_INFO { get { return _COMPANY_INFO; } set { _COMPANY_INFO = value; OnPropertyChanged("COMPANY_INFO"); } }
        public bool DOWNLOAD_ATTENDANCE_DATA { get { return _DOWNLOAD_ATTENDANCE_DATA; } set { _DOWNLOAD_ATTENDANCE_DATA = value; OnPropertyChanged("DOWNLOAD_ATTENDANCE_DATA"); } }
        public bool DATA_BACKUP { get { return _DATA_BACKUP; } set { _DATA_BACKUP = value; OnPropertyChanged("DATA_BACKUP"); } }
        public bool DATA_RESTORE { get { return _DATA_RESTORE; } set { _DATA_RESTORE = value; OnPropertyChanged("DATA_RESTORE"); } }
        public bool DEVICE_SETTING { get { return _DEVICE_SETTING; } set { _DEVICE_SETTING = value; OnPropertyChanged("DEVICE_SETTING"); } }
        public bool ASSIGN_WORKHOUR { get { return _ASSIGN_WORKHOUR; } set { _ASSIGN_WORKHOUR = value; OnPropertyChanged("ASSIGN_WORKHOUR"); } }
        #endregion

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute(
@"INSERT INTO UserManagement (UNAME, [DESCRIPTION], PWD, CATEGORY, USERMANAGEMENT, BRANCH, DEPARTMENT, DESIGNATION, LEAVE, HOLIDAY, WORKHOUR, EMPLOYEE_REGISTRATION, MANUAL_ATTENDANCE, 
MONTHLY_LEAVE, ADD_LEAVE, LEAVE_APPLICATION, ATTENDANCE_REPORTS, MONTHLY_ATTENDANCE_DETAIL, MONTHLY_ATTENDANCE_SUMMARY, ATTENDANCE_BOOK, DAILY_ATTENDANCE, ANNUAL_ATTENDANCE, 
DAILY_ABSENT, MANUAL_ATTENDANCE_REPORT, LEAVE_REPORTS, LEAVE_STATUS, LEAVE_STATEMENT, MONTHLY_LEAVE_REPORT, ANNUAL_LEAVE, EMPLOYEE_LIST, EMPLOYEE_DETAIL, MONTHLY_LATE_REPORT, 
ANNUAL_HOLIDAY_LIST, ATTENDANCE_LOG, DAILY_ATTENDANCE_LOG, MONTHLY_ATTENDANCE_LOG, COMPANY_INFO, DOWNLOAD_ATTENDANCE_DATA, DATA_BACKUP, DATA_RESTORE, DEVICE_SETTING, INACTIVE, ASSIGN_WORKHOUR)
VALUES (@UNAME, @DESCRIPTION, @PWD, @CATEGORY, @USERMANAGEMENT, @BRANCH, @DEPARTMENT, @DESIGNATION, @LEAVE, @HOLIDAY, @WORKHOUR, @EMPLOYEE_REGISTRATION, @MANUAL_ATTENDANCE, @MONTHLY_LEAVE, 
@ADD_LEAVE, @LEAVE_APPLICATION, @ATTENDANCE_REPORTS, @MONTHLY_ATTENDANCE_DETAIL, @MONTHLY_ATTENDANCE_SUMMARY, @ATTENDANCE_BOOK, @DAILY_ATTENDANCE, @ANNUAL_ATTENDANCE, @DAILY_ABSENT, 
@MANUAL_ATTENDANCE_REPORT, @LEAVE_REPORTS, @LEAVE_STATUS, @LEAVE_STATEMENT, @MONTHLY_LEAVE_REPORT, @ANNUAL_LEAVE, @EMPLOYEE_LIST, @EMPLOYEE_DETAIL, @MONTHLY_LATE_REPORT, 
@ANNUAL_HOLIDAY_LIST, @ATTENDANCE_LOG, @DAILY_ATTENDANCE_LOG, @MONTHLY_ATTENDANCE_LOG, @COMPANY_INFO, @DOWNLOAD_ATTENDANCE_DATA, @DATA_BACKUP, @DATA_RESTORE, @DEVICE_SETTING, @INACTIVE, @ASSIGN_WORKHOUR)", this, tran) == 1;
        }

        public override bool Update(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute(
@"UPDATE UserManagement SET [DESCRIPTION] = @DESCRIPTION, CATEGORY = @CATEGORY, USERMANAGEMENT = @USERMANAGEMENT, BRANCH = @BRANCH, DEPARTMENT = @DEPARTMENT, DESIGNATION = @DESIGNATION, 
LEAVE = @LEAVE, HOLIDAY = @HOLIDAY, WORKHOUR = @WORKHOUR, EMPLOYEE_REGISTRATION = @EMPLOYEE_REGISTRATION, MANUAL_ATTENDANCE = @MANUAL_ATTENDANCE, INACTIVE = @INACTIVE,
MONTHLY_LEAVE = @MONTHLY_LEAVE, ADD_LEAVE = @ADD_LEAVE, LEAVE_APPLICATION = @LEAVE_APPLICATION, ATTENDANCE_REPORTS = @ATTENDANCE_REPORTS, MONTHLY_ATTENDANCE_DETAIL = @MONTHLY_ATTENDANCE_DETAIL, 
MONTHLY_ATTENDANCE_SUMMARY = @MONTHLY_ATTENDANCE_SUMMARY, ATTENDANCE_BOOK = @ATTENDANCE_BOOK, DAILY_ATTENDANCE = @DAILY_ATTENDANCE, ANNUAL_ATTENDANCE = @ANNUAL_ATTENDANCE, 
DAILY_ABSENT = @DAILY_ABSENT, MANUAL_ATTENDANCE_REPORT = @MANUAL_ATTENDANCE_REPORT, LEAVE_REPORTS = @LEAVE_REPORTS, LEAVE_STATUS = @LEAVE_STATUS, LEAVE_STATEMENT = @LEAVE_STATEMENT, 
MONTHLY_LEAVE_REPORT = @MONTHLY_LEAVE_REPORT, ANNUAL_LEAVE = @ANNUAL_LEAVE, EMPLOYEE_LIST = @EMPLOYEE_LIST, EMPLOYEE_DETAIL = @EMPLOYEE_DETAIL, MONTHLY_LATE_REPORT = @MONTHLY_LATE_REPORT, 
ANNUAL_HOLIDAY_LIST = @ANNUAL_HOLIDAY_LIST, ATTENDANCE_LOG = @ATTENDANCE_LOG, DAILY_ATTENDANCE_LOG = @DAILY_ATTENDANCE_LOG, MONTHLY_ATTENDANCE_LOG = @MONTHLY_ATTENDANCE_LOG, 
COMPANY_INFO = @COMPANY_INFO, DOWNLOAD_ATTENDANCE_DATA = @DOWNLOAD_ATTENDANCE_DATA, DATA_BACKUP = @DATA_BACKUP, DATA_RESTORE = @DATA_RESTORE, DEVICE_SETTING = @DEVICE_SETTING, ASSIGN_WORKHOUR = @ASSIGN_WORKHOUR
WHERE UNAME = @UNAME", this, tran) == 1;
        }

        public override bool Delete(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("DELETE FROM USERMANAGEMENT WHERE UNAME = @UNAME", this, tran) == 1;
        }

        public override bool IsValid
        {
            get
            {
                return string.IsNullOrEmpty(Error);
            }
        }

        public string Error
        {
            get
            {
                foreach (PropertyInfo pi in this.GetType().GetProperties())
                {
                    if (!string.IsNullOrEmpty(this[pi.Name]))
                        return this[pi.Name];
                }
                return string.Empty;
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "UNAME":
                        if (string.IsNullOrEmpty(UNAME))
                            return "User Name cannot be empty";
                        else if (UNAME.Length > 20)
                            return "User Name cannot be longer than 20 characters";
                        break;
                    case "DESCRIPTION":
                        if (string.IsNullOrEmpty(DESCRIPTION))
                            return "Description cannot be empty";
                        else if (DESCRIPTION.Length > 250)
                            return "Description cannot be longer than 250 characters";
                        break;
                    case "PWD":
                        //if (PWD.Length < 4)
                        //    return "Password must be at least 4 character long";
                        //else if (PWD.Length > 250)
                        //    return "Password cannot be longer than 50 characters";
                        break;
                    case "CPWD":
                        //if (!PWD.Equals(CPWD))
                        //    return "Password does not match";
                        break;
                }
                return string.Empty;
            }
        }
    }
}
