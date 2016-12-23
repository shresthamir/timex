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
    class Leaves : RootModel, IDataErrorInfo
    {
        #region Members

        private short _LEAVE_ID;
        private string _LEAVE_NAME;
        private bool _ISPAIDLEAVE;
        private short _MAXALLOWEDLEAVES;
        private short _ANNUALLEAVECOUNT;
        private bool _EXPIRE;
        private decimal _EQUIVALENTWORKINGDAYS;
        private short _AllowedDays;

        #endregion

        #region Properties

        public short LEAVE_ID { get { return _LEAVE_ID; } set { _LEAVE_ID = value; OnPropertyChanged("LEAVE_ID"); } }
        public string LEAVE_NAME { get { return _LEAVE_NAME; } set { _LEAVE_NAME = value; OnPropertyChanged("LEAVE_NAME"); } }
        public bool ISPAIDLEAVE { get { return _ISPAIDLEAVE; } set { _ISPAIDLEAVE = value; OnPropertyChanged("ISPAIDLEAVE"); } }
        public short MAXALLOWEDLEAVES { get { return _MAXALLOWEDLEAVES; } set { _MAXALLOWEDLEAVES = value; OnPropertyChanged("MAXALLOWEDLEAVES"); } }
        public short ANNUALLEAVECOUNT { get { return _ANNUALLEAVECOUNT; } set { _ANNUALLEAVECOUNT = value; OnPropertyChanged("ANNUALLEAVECOUNT"); } }
        public bool EXPIRE { get { return _EXPIRE; } set { _EXPIRE = value; OnPropertyChanged("EXPIRE"); } }
        public decimal EQUIVALENTWORKINGDAYS { get { return _EQUIVALENTWORKINGDAYS; } set { _EQUIVALENTWORKINGDAYS = value; OnPropertyChanged("EQUIVALENTWORKINGDAYS"); } }
        public short AllowedDays { get { return _AllowedDays; } set { _AllowedDays = value; OnPropertyChanged("AllowedDays"); } }
        #endregion

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute(@"INSERT INTO LEAVES(LEAVE_ID, LEAVE_NAME, ISPAIDLEAVE, MAXALLOWEDLEAVES, ANNUALLEAVECOUNT, EXPIRE, EQUIVALENTWORKINGDAYS) 
			VALUES (@LEAVE_ID, @LEAVE_NAME, @ISPAIDLEAVE, @MAXALLOWEDLEAVES, @ANNUALLEAVECOUNT, @EXPIRE, @EQUIVALENTWORKINGDAYS)", this, tran) == 1;
        }

        public override bool Update(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute(@"UPDATE LEAVES SET LEAVE_NAME = @LEAVE_NAME, ISPAIDLEAVE = @ISPAIDLEAVE, MAXALLOWEDLEAVES = @MAXALLOWEDLEAVES,
							ANNUALLEAVECOUNT = @ANNUALLEAVECOUNT, EXPIRE = @EXPIRE, EQUIVALENTWORKINGDAYS = @EQUIVALENTWORKINGDAYS WHERE LEAVE_ID = @LEAVE_ID", this, tran) == 1;
        }

        public override bool Delete(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("DELETE FROM LEAVES WHERE LEAVE_ID = @LEAVE_ID", this, tran) == 1;
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
                    case "LEAVE_NAME":
                        if (string.IsNullOrEmpty(LEAVE_NAME))
                            return "Leave Name cannot be empty.";
                        break;
                    case "MAXALLOWEDLEAVES":
                        if (MAXALLOWEDLEAVES < 0)
                            return "Maximum Leave days cannot be less than zero.";
                        break;
                    case "ANNUALLEAVECOUNT":
                        if (ANNUALLEAVECOUNT <= 0)
                            return "Annual Leave days must be greater than zero.";
                        break;
                    case "EQUIVALENTWORKINGDAYS":
                        if (EQUIVALENTWORKINGDAYS > 1)
                            return "Equivalent Working Days cannot be greater than One.";
                        break;
                    case "AllowedDays":
                        if (AllowedDays > ANNUALLEAVECOUNT)
                            return "Allowed leave days cannot be greater than Annual Leave days";
                        break;
                }
                return string.Empty;
            }
        }
    }

    class AllowedLeaves : RootModel
    {

        private short _ENO;
        private short _LEAVE_ID;
        private short _ALLOWED_DAYS;
        public short ENO { get { return _ENO; } set { _ENO = value; OnPropertyChanged("ENO"); } }
        public short LEAVE_ID { get { return _LEAVE_ID; } set { _LEAVE_ID = value; OnPropertyChanged("LEAVE_ID"); } }
        public short ALLOWED_DAYS { get { return _ALLOWED_DAYS; } set { _ALLOWED_DAYS = value; OnPropertyChanged("ALLOWED_DAYS"); } }

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("INSERT INTO EMP_ALLOWED_LEAVE(ENO, LEAVE_ID, ALLOWED_DAYS) VALUES (@ENO, @LEAVE_ID, @ALLOWED_DAYS)",this, tran) == 1;
        }

    }

}
