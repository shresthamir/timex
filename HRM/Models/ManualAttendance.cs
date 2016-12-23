using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Reflection;
using System.Data.SqlClient;
namespace HRM.Models
{
    class ManualAttendance : RootModel, IDataErrorInfo
    {
        #region Members

        private int _TRANID;
        private short _ENO;
        private DateTime _TRANDATE = DateTime.Today;
        private string _ATT_TYPE = "Sign In";
        private DateTime _ATT_TIME = DateTime.Now;
        private string _TRAN_USER;
        private string _REMARKS;
        private DateTime _ATT_DATE = DateTime.Today;
        private string _ENAME;

        #endregion

        #region Properties

        public int TRANID { get { return _TRANID; } set { _TRANID = value; OnPropertyChanged("TRANID"); } }

        public short ENO { get { return _ENO; } set { _ENO = value; OnPropertyChanged("ENO"); } }

        public string ENAME { get { return _ENAME; } set { _ENAME = value; OnPropertyChanged("ENAME"); } }

        public DateTime TRANDATE { get { return _TRANDATE; } set { _TRANDATE = value; OnPropertyChanged("TRANDATE"); } }

        public string ATT_TYPE { get { return _ATT_TYPE; } set { _ATT_TYPE = value; OnPropertyChanged("ATT_TYPE"); } }

        public DateTime ATT_TIME { get { return _ATT_TIME; } set { _ATT_TIME = value; OnPropertyChanged("ATT_TIME"); } }

        public string TRAN_USER { get { return _TRAN_USER; } set { _TRAN_USER = value; OnPropertyChanged("TRAN_USER"); } }

        public string REMARKS { get { return _REMARKS; } set { _REMARKS = value; OnPropertyChanged("REMARKS"); } }

        public DateTime ATT_DATE { get { return _ATT_DATE; } set { _ATT_DATE = value; ATT_TIME = value; OnPropertyChanged("ATT_DATE"); } }

        #endregion

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute(@"INSERT INTO MANUAL_ATT_LOG(TRANID, ENO, TRANDATE, ATT_TYPE, ATT_TIME, TRAN_USER, REMARKS, ATT_DATE) 
			VALUES (@TRANID, @ENO, GETDATE(), @ATT_TYPE, @ATT_TIME, @TRAN_USER, @REMARKS, @ATT_DATE)", this, tran) == 1;
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
                if (Validate)
                    switch (columnName)
                    {
                        case "ENO":
                            if (ENO <= 0)
                                return "Please Select an Employee";
                            break;
                        case "ATT_DATE":
                            if (ATT_DATE > DateTime.Today)
                                return "Attendance date cannot be Future Dates";
                            break;
                        case "REMARKS":
                            if (string.IsNullOrEmpty(REMARKS))
                                return "Remarks cannot be empty";
                            else if (REMARKS.Length > 200)
                                return "Remarks cannot be longer than 200 Characters";
                            break;
                    }
                return string.Empty;
            }
        }
    }

    class AttLog
    {
        public short ENO { get; set; }
        public DateTime ATT_DATE { get; set; }
        public DateTime ATT_TIME { get; set; }
        public byte VerifyMode { get; set; }
        public byte InOutMode { get; set; }

        public bool Save(SqlTransaction tran)
        {
            return tran.Connection.Execute("INSERT INTO ATT_LOG(ENO, ATT_DATE, ATT_TIME, VerifyMode, InOutMode) VALUES (@ENO, @ATT_DATE, @ATT_TIME, @VerifyMode, @InOutMode)", this, tran) == 1;
        }
    }

}
