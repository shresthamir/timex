using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Reflection;
namespace HRM.Models
{
    class EmployeeDetail : RootModel, IDataErrorInfo
    {
        #region Members

        private int _EMP_TRANID;
        private short _ENO;
        private short _DEPARTMENT_ID;
        private short _DESIGNATION_ID;
        private short _WORKHOUR_ID;
        private string _MARITAL_STATUS;
        private DateTime _TRNDATE;
        private short _BRANCH_ID;
        private string _CMODE;
        private string _STATUS;

        #endregion

        #region Properties

        public int EMP_TRANID { get { return _EMP_TRANID; } set { _EMP_TRANID = value; OnPropertyChanged("EMP_TRANID"); } }

        public short ENO { get { return _ENO; } set { _ENO = value; OnPropertyChanged("ENO"); } }

        public short DEPARTMENT_ID { get { return _DEPARTMENT_ID; } set { _DEPARTMENT_ID = value; OnPropertyChanged("DEPARTMENT_ID"); } }

        public short DESIGNATION_ID { get { return _DESIGNATION_ID; } set { _DESIGNATION_ID = value; OnPropertyChanged("DESIGNATION_ID"); } }

        public short WORKHOUR_ID { get { return _WORKHOUR_ID; } set { _WORKHOUR_ID = value; OnPropertyChanged("WORKHOUR_ID"); } }

        public string MARITAL_STATUS { get { return _MARITAL_STATUS; } set { _MARITAL_STATUS = value; OnPropertyChanged("MARITAL_STATUS"); } }

        public DateTime TRNDATE { get { return _TRNDATE; } set { _TRNDATE = value; OnPropertyChanged("TRNDATE"); } }

        public short BRANCH_ID { get { return _BRANCH_ID; } set { _BRANCH_ID = value; OnPropertyChanged("BRANCH_ID"); } }

        public string CMODE { get { return _CMODE; } set { _CMODE = value; OnPropertyChanged("CMODE"); } }
        public string STATUS { get { return _STATUS; } set { _STATUS = value; OnPropertyChanged("STATUS"); } }

        #endregion

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute(@"INSERT INTO EMPLOYEE_DETAIL(EMP_TRANID, ENO, DEPARTMENT_ID, DESIGNATION_ID, MARITAL_STATUS, TRNDATE, BRANCH_ID, CMODE, [STATUS]) 
			VALUES ((SELECT ISNULL(MAX(EMP_TRANID),0) +1 FROM EMPLOYEE_DETAIL), @ENO, @DEPARTMENT_ID, @DESIGNATION_ID, @MARITAL_STATUS, GETDATE(), @BRANCH_ID, @CMODE, @STATUS)", this, tran) > 0;
        }

        public bool Compare(EmployeeDetail Obj)
        {

            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {

                if (pi.DeclaringType == typeof(EmployeeDetail) && pi.CanWrite && !pi.GetValue(this).Equals(Obj.GetType().GetProperty(pi.Name).GetValue(Obj)))
                    return false;
            }
            return true;
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
                    case "DEPARTMENT_ID":
                        if (DEPARTMENT_ID <= 0)
                            return "Please select a Department.";
                        break;
                    case "BRANCH_ID":
                        if (BRANCH_ID <= 0)
                            return "Please select a Branch.";
                        break;
                    case "MARITAL_STATUS":
                        if (string.IsNullOrEmpty(MARITAL_STATUS))
                            return "Please select a Marital Status.";
                        break;
                    case "DESIGNATION_ID":
                        if (DESIGNATION_ID <= 0)
                            return "Please select a Designation.";
                        break;
                    case "WORKHOUR_ID":
                        if (WORKHOUR_ID <= 0)
                            return "Please select a Working Hour.";
                        break;
                    case "CMODE":
                        if (string.IsNullOrEmpty(CMODE))
                            return "Please select a Contract Mode.";
                        break;
                    case "STATUS":
                        if (string.IsNullOrEmpty(STATUS))
                            return "Please select a Status.";
                        break;
                }
                return string.Empty;
            }
        }
    }

}
