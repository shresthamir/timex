using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Reflection;
using HRM.Library.AppScopeClasses;
using System.Collections.ObjectModel;

namespace HRM.Models
{
    class Employee : RootModel, IDataErrorInfo
    {
        #region Members

        private short _ENO;
        private string _TITLE;
        private string _FULLNAME;
        private byte[] _PHOTO;
        private string _ECODE;
        private string _CALENDAR_TYPE = SETTING.DEFAULT_CALENDAR;

        #endregion

        #region Properties

        public short ENO { get { return _ENO; } set { _ENO = value; OnPropertyChanged("ENO"); } }

        public string TITLE { get { return _TITLE; } set { _TITLE = value; OnPropertyChanged("TITLE"); } }

        public string FULLNAME { get { return _FULLNAME; } set { _FULLNAME = value; OnPropertyChanged("FULLNAME"); } }

        public byte[] PHOTO { get { return _PHOTO; } set { _PHOTO = value; OnPropertyChanged("PHOTO"); } }

        public string ECODE { get { return _ECODE; } set { _ECODE = value; OnPropertyChanged("ECODE"); } }
        public string CALENDAR_TYPE { get { return _CALENDAR_TYPE; } set { _CALENDAR_TYPE = value; OnPropertyChanged("CALENDAR_TYPE"); } }

        #endregion

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("INSERT INTO EMPLOYEE(ENO, FULLNAME, TITLE, PHOTO, ECODE, CALENDAR_TYPE) VALUES (@ENO, @FULLNAME, @TITLE, @PHOTO, @ECODE, @CALENDAR_TYPE)", this, tran) == 1;
        }

        public override bool Update(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("UPDATE EMPLOYEE SET FULLNAME = @FULLNAME, TITLE = @TITLE, PHOTO = @PHOTO, ECODE = @ECODE WHERE ENO = @ENO", this, tran) == 1;
        }

        public override bool Delete(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("DELETE FROM EMPLOYEE WHERE ENO = @ENO", this, tran) == 1;
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
                    case "FULLNAME":
                        if (string.IsNullOrEmpty(FULLNAME))
                            return "Employee Name cannot be empty.";
                        break;
                    case "ECODE":
                        if (!string.IsNullOrEmpty(ECODE) && ECODE.Length > 50)
                            return "Employee Code cannot be longer than 50 characters long.";
                        break;
                }
                return string.Empty;
            }
        }
    }

    class EmployeePersonalInfo : RootModel, IDataErrorInfo
    {
        #region Members

        private short _ENO;
        private string _P_ADDRESS;
        private string _T_ADDRESS;
        private DateTime _DOB = new DateTime(2000, 1, 1);
        private string _MOBILE;
        private string _PHONE;
        private string _CTZNO;
        private string _EMAIL;
        private DateTime _JOINDATE = DateTime.Today;
        private string _CPERSON;
        private string _CCONTACTNO;
        private string _CRELATION;
        private string _GENDER;
        #endregion

        #region Properties

        public short ENO { get { return _ENO; } set { _ENO = value; OnPropertyChanged("ENO"); } }

        public string P_ADDRESS { get { return _P_ADDRESS; } set { _P_ADDRESS = value; OnPropertyChanged("P_ADDRESS"); } }

        public string T_ADDRESS { get { return _T_ADDRESS; } set { _T_ADDRESS = value; OnPropertyChanged("T_ADDRESS"); } }

        public string MOBILE { get { return _MOBILE; } set { _MOBILE = value; OnPropertyChanged("MOBILE"); } }

        public string PHONE { get { return _PHONE; } set { _PHONE = value; OnPropertyChanged("PHONE"); } }

        public string CTZNO { get { return _CTZNO; } set { _CTZNO = value; OnPropertyChanged("CTZNO"); } }

        public DateTime DOB { get { return _DOB; } set { _DOB = value; OnPropertyChanged("DOB"); } }

        public DateTime JOINDATE { get { return _JOINDATE; } set { _JOINDATE = value; OnPropertyChanged("JOINDATE"); } }

        public string EMAIL { get { return _EMAIL; } set { _EMAIL = value; OnPropertyChanged("EMAIL"); } }
        public string CPERSON { get { return _CPERSON; } set { _CPERSON = value; OnPropertyChanged("CPERSON"); } }
        public string CCONTACTNO { get { return _CCONTACTNO; } set { _CCONTACTNO = value; OnPropertyChanged("CCONTACTNO"); } }
        public string CRELATION { get { return _CRELATION; } set { _CRELATION = value; OnPropertyChanged("CRELATION"); } }
        public string GENDER { get { return _GENDER; } set { _GENDER = value; OnPropertyChanged("GENDER"); } }
        #endregion

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("INSERT INTO EMPLOYEE_PERSONAL_INFO(ENO, P_ADDRESS, T_ADDRESS, MOBILE, PHONE, CTZNO, DOB, EMAIL, GENDER, JOINDATE, CPERSON, CCONTACTNO, CRELATION) VALUES (@ENO, @P_ADDRESS, @T_ADDRESS, @MOBILE, @PHONE, @CTZNO, @DOB, @EMAIL, @GENDER, @JOINDATE, @CPERSON, @CCONTACTNO, @CRELATION)", this, tran) == 1;
        }

        public override bool Update(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("UPDATE EMPLOYEE_PERSONAL_INFO SET P_ADDRESS = @P_ADDRESS, T_ADDRESS = @T_ADDRESS, DOB = @DOB, MOBILE = @MOBILE, PHONE = @PHONE, CTZNO = @CTZNO, EMAIL = @EMAIL, JOINDATE = @JOINDATE, GENDER = @GENDER, CPERSON = @CPERSON, CCONTACTNO = @CCONTACTNO, CRELATION = @CRELATION WHERE ENO = @ENO", this, tran) == 1;
        }

        public override bool Delete(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("DELETE FROM EMPLOYEE_PERSONAL_INFO WHERE ENO = @ENO", this, tran) == 1;
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
                    case "P_ADDRESS":
                        if (!string.IsNullOrEmpty(P_ADDRESS) && P_ADDRESS.Length > 250)
                            return "Address cannot be longer than 250 characters.";
                        break;
                    case "T_ADDRESS":
                        if (!string.IsNullOrEmpty(T_ADDRESS) && T_ADDRESS.Length > 250)
                            return "Address cannot be longer than 250 characters.";
                        break;
                    case "MOBILE":
                        if (!string.IsNullOrEmpty(MOBILE) && MOBILE.Length > 20)
                            return "Mobile No cannot be longer than 20 digits.";
                        break;
                    case "PHONE":
                        if (!string.IsNullOrEmpty(PHONE) && PHONE.Length > 20)
                            return "Phone No cannot be longer than 20 digits.";
                        break;
                    case "CTZNO":
                        if (!string.IsNullOrEmpty(CTZNO) && CTZNO.Length > 50)
                            return "Citizenship No cannot be longer than 50 characters.";
                        break;
                    case "EMAIL":
                        if (!string.IsNullOrEmpty(EMAIL) && EMAIL.Length > 50)
                            return "Email cannot be longer than 50 characters.";
                        break;
                    case "CPERSON":
                        if (!string.IsNullOrEmpty(CPERSON) && CPERSON.Length > 50)
                            return "Contact Person cannot be longer than 50 characters.";
                        break;
                    case "CCONTACTNO":
                        if (!string.IsNullOrEmpty(CPERSON) && string.IsNullOrEmpty(CCONTACTNO))
                            return "Contact Number of Contact Person is mandatory.";
                        else if (!string.IsNullOrEmpty(CCONTACTNO) && CCONTACTNO.Length > 20)
                            return "Contact No cannot be longer than 20 digits.";
                        break;
                    case "CRELATION":
                        if (!string.IsNullOrEmpty(CPERSON) && string.IsNullOrEmpty(CRELATION))
                            return "Employee and Contact Person Relation is mandatory.";
                        else if (!string.IsNullOrEmpty(CRELATION) && CRELATION.Length > 50)
                            return "Contact No cannot be longer than 50 Characters.";
                        break;
                }
                return string.Empty;
            }
        }
    }

    class EmployeeAcademics : RootModel, IDataErrorInfo
    {
        private string _BOARD;
        private string _INSTITUTE;
        private string _DEGREENAME;
        private short _PASSEDYEAR;
        private short _ENO;

        public short ENO { get { return _ENO; } set { _ENO = value; OnPropertyChanged("ENO"); } }
        public string BOARD { get { return _BOARD; } set { _BOARD = value; OnPropertyChanged("BOARD"); } }
        public string INSTITUTE { get { return _INSTITUTE; } set { _INSTITUTE = value; OnPropertyChanged("INSTITUTE"); } }
        public string DEGREENAME { get { return _DEGREENAME; } set { _DEGREENAME = value; OnPropertyChanged("DEGREENAME"); } }
        public short PASSEDYEAR { get { return _PASSEDYEAR; } set { _PASSEDYEAR = value; OnPropertyChanged("PASSEDYEAR"); } }

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("INSERT INTO EMPLOYEE_ACADEMICS(ENO, BOARD, INSTITUTE, DEGREENAME, PASSEDYEAR) VALUES (@ENO, @BOARD, @INSTITUTE, @DEGREENAME, @PASSEDYEAR)", this, tran) == 1;
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
                    case "BOARD":
                        if (string.IsNullOrEmpty(BOARD))
                            return "Board/University cannot be empty";
                        else if (BOARD.Length > 250)
                            return "Board/University cannot be longer than 250 Characters";
                        break;
                    case "INSTITUTE":
                        if (string.IsNullOrEmpty(INSTITUTE))
                            return "Institute cannot be empty";
                        else if (INSTITUTE.Length > 250)
                            return "Institute name cannot be longer than 250 Characters";
                        break;
                    case "DEGREENAME":
                        if (string.IsNullOrEmpty(DEGREENAME))
                            return "Board/University cannot be empty";
                        else if (DEGREENAME.Length > 100)
                            return "Name of Degree cannot be longer than 100 Characters";
                        break;
                    case "PASSEDYEAR":
                        if (PASSEDYEAR < 1900 || PASSEDYEAR > DateTime.Today.Year)
                            return "Passed Year must be between 1900 and current year";
                        break;
                }
                return string.Empty;
            }
        }
    }

    class EmployeeTraining : RootModel, IDataErrorInfo
    {
        private string _COURSE;
        private string _INSTITUTE;
        private short _DURATION;
        private short _ENO;

        public short ENO { get { return _ENO; } set { _ENO = value; OnPropertyChanged("ENO"); } }
        public string COURSE { get { return _COURSE; } set { _COURSE = value; OnPropertyChanged("COURSE"); } }
        public string INSTITUTE { get { return _INSTITUTE; } set { _INSTITUTE = value; OnPropertyChanged("INSTITUTE"); } }
        public short DURATION { get { return _DURATION; } set { _DURATION = value; OnPropertyChanged("DURATION"); } }

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("INSERT INTO EMPLOYEE_TRAINING(ENO, COURSE, INSTITUTE, DURATION) VALUES (@ENO, @COURSE, @INSTITUTE, @DURATION)", this, tran) == 1;
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
                    case "COURSE":
                        if (string.IsNullOrEmpty(COURSE))
                            return "Course Name cannot be empty";
                        else if (COURSE.Length > 250)
                            return "Course Name cannot be longer than 250 Characters";
                        break;
                    case "INSTITUTE":
                        if (string.IsNullOrEmpty(INSTITUTE))
                            return "Institute cannot be empty";
                        else if (INSTITUTE.Length > 250)
                            return "Institute name cannot be longer than 250 Characters";
                        break;
                    case "DURATION":
                        if (DURATION <= 0)
                            return "Course Duration must be greater than Zero";
                        break;
                }
                return string.Empty;
            }
        }
    }

    class EmployeeExperience : RootModel, IDataErrorInfo
    {
        private string _CNAME;
        private string _CADDRESS;
        private string _DESIGNATION;
        private DateTime _FDATE = DateTime.Today;
        private short _ENO;
        private DateTime _TDATE = DateTime.Today;

        public short ENO { get { return _ENO; } set { _ENO = value; OnPropertyChanged("ENO"); } }
        public string CNAME { get { return _CNAME; } set { _CNAME = value; OnPropertyChanged("CNAME"); } }
        public string CADDRESS { get { return _CADDRESS; } set { _CADDRESS = value; OnPropertyChanged("CADDRESS"); } }
        public string DESIGNATION { get { return _DESIGNATION; } set { _DESIGNATION = value; OnPropertyChanged("DESIGNATION"); } }
        public DateTime FDATE { get { return _FDATE; } set { _FDATE = value; OnPropertyChanged("FDATE"); } }
        public DateTime TDATE { get { return _TDATE; } set { _TDATE = value; OnPropertyChanged("TDATE"); } }

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("INSERT INTO EMPLOYEE_EMPLOYMENT_HISTORY(ENO, CNAME, CADDRESS, DESIGNATION, FDATE, TDATE) VALUES (@ENO, @CNAME, @CADDRESS, @DESIGNATION, @FDATE, @TDATE)", this, tran) == 1;
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
                    case "CNAME":
                        if (string.IsNullOrEmpty(CNAME))
                            return "Organization cannot be empty";
                        else if (CNAME.Length > 250)
                            return "Organization Name cannot be longer than 250 Characters";
                        break;
                    case "CADDRESS":
                        if (!string.IsNullOrEmpty(CADDRESS) && CADDRESS.Length > 250)
                            return "Address cannot be longer than 250 Characters";
                        break;
                    case "DESIGNATION":
                        if (string.IsNullOrEmpty(DESIGNATION))
                            return "Designaiton cannot be empty";
                        else if (DESIGNATION.Length > 50)
                            return "Designation cannot be longer than 50 Characters";
                        break;
                    case "FDATE":
                        if (FDATE.Year < 1900 || FDATE > DateTime.Today)
                            return "Start Date must be between 1900 and current Date";
                        break;
                    case "TDATE":
                        if (TDATE.Year < 1900 || TDATE > DateTime.Today)
                            return "End Date must be between 1900 and current Date";
                        break;
                }
                return string.Empty;
            }
        }
    }

    class EmployeeAllDetail : Employee, ITreeItem
    {
        private short _BRANCH_ID;
        private short _DEPARTMENT_ID;
        private short _DESIGNATION_ID;
        private string _DEPARTMENT;
        private string _BRANCH_NAME;
        private string _DESIGNATION;
        private string _STATUS;
        private string _MARITAL_STATUS;
        private string _CMODE;

        public short BRANCH_ID { get { return _BRANCH_ID; } set { _BRANCH_ID = value; OnPropertyChanged("BRANCH_ID"); } }
        public short DEPARTMENT_ID { get { return _DEPARTMENT_ID; } set { _DEPARTMENT_ID = value; OnPropertyChanged("DEPARTMENT_ID"); } }
        public short DESIGNATION_ID { get { return _DESIGNATION_ID; } set { _DESIGNATION_ID = value; OnPropertyChanged("DESIGNATION_ID"); } }
        public string BRANCH_NAME { get { return _BRANCH_NAME; } set { _BRANCH_NAME = value; OnPropertyChanged("BRANCH_NAME"); } }
        public string DEPARTMENT { get { return _DEPARTMENT; } set { _DEPARTMENT = value; OnPropertyChanged("DEPARTMENT"); } }
        public string DESIGNATION { get { return _DESIGNATION; } set { _DESIGNATION = value; OnPropertyChanged("DESIGNATION"); } }
        public string STATUS { get { return _STATUS; } set { _STATUS = value; OnPropertyChanged("STATUS"); } }
        public string MARITAL_STATUS { get { return _MARITAL_STATUS; } set { _MARITAL_STATUS = value; OnPropertyChanged("MARITAL_STATUS"); } }
        public string CMODE { get { return _CMODE; } set { _CMODE = value; OnPropertyChanged("CMODE"); } }


        #region TreeNode
        public void ExpandTree(ITreeItem Branch)
        {
        }
        public string NodeID { get { return ENO.ToString(); } }
        public string NodeName { get { return FULLNAME; } }
        public bool IsExpanded { get; set; }
        public string ParentID { get { return DEPARTMENT_ID.ToString(); } }
        public ObservableCollection<ITreeItem> Children { get; set; }

        public ITreeItem Parent
        {
            get;
            set;
        }
        #endregion
    }

}
