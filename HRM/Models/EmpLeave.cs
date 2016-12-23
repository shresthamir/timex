using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace HRM.Models
{
    class LEAVE_LEDGER:RootModel
    {
        #region Members

        private int _TRANID;
        private DateTime _TRANDATE;
        private short _ENO;
        private short _LEAVE_ID;
        private decimal _Dr;
        private decimal _Cr;
        private string _TRAN_USER;
        private DateTime? _APPLIED_DATE;
        private short _APPROVEDBY;
        private string _REMARKS;
        private short _MID;
        private string _ENAME;
        private string _LNAME;
        private string _APPROVEDBYNAME;

        #endregion

        #region Properties

        public int TRANID { get { return _TRANID; } set { _TRANID = value; OnPropertyChanged("TRANID"); } }

        public DateTime TRANDATE { get { return _TRANDATE; } set { _TRANDATE = value; OnPropertyChanged("TRANDATE"); } }

        public short ENO { get { return _ENO; } set { _ENO = value; OnPropertyChanged("ENO"); } }

        public string ENAME { get { return _ENAME; } set { _ENAME = value; OnPropertyChanged("ENAME"); } }

        public string APPROVEDBYNAME { get { return _APPROVEDBYNAME; } set { _APPROVEDBYNAME = value; OnPropertyChanged("APPROVEDBYNAME"); } }

        public short LEAVE_ID { get { return _LEAVE_ID; } set { _LEAVE_ID = value; OnPropertyChanged("LEAVE_ID"); } }

        public string LNAME { get { return _LNAME; } set { _LNAME = value; OnPropertyChanged("LNAME"); } }

        public decimal Dr { get { return _Dr; } set { _Dr = value; OnPropertyChanged("Dr"); } }

        public decimal Cr { get { return _Cr; } set { _Cr = value; OnPropertyChanged("Cr"); } }

        public string TRAN_USER { get { return _TRAN_USER; } set { _TRAN_USER = value; OnPropertyChanged("TRAN_USER"); } }

        public DateTime? APPLIED_DATE { get { return _APPLIED_DATE; } set { _APPLIED_DATE = value; OnPropertyChanged("APPLIED_DATE"); } }

        public short APPROVEDBY { get { return _APPROVEDBY; } set { _APPROVEDBY = value; OnPropertyChanged("APPROVEDBY"); } }

        public string REMARKS { get { return _REMARKS; } set { _REMARKS = value; OnPropertyChanged("REMARKS"); } }

        public short MID { get { return _MID; } set { _MID = value; OnPropertyChanged("MID"); } }

        #endregion

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute(@"INSERT INTO LEAVE_LEDGER(TRANID, TRANDATE, ENO, LEAVE_ID, Dr, Cr, TRAN_USER, APPLIED_DATE, APPROVEDBY, REMARKS, MID) 
			VALUES (@TRANID, @TRANDATE, @ENO, @LEAVE_ID, @Dr, @Cr, @TRAN_USER, @APPLIED_DATE, @APPROVEDBY, @REMARKS, @MID)", this, tran) == 1;
        }
    }


}
