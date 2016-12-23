using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace HRM.Models
{
    class Holidays : RootModel
    {
        #region Members

        private short _HOLIDAY_ID;
        private string _HOLIDAY_NAME;
             

        #endregion

        #region Properties
        public short HOLIDAY_ID { get { return _HOLIDAY_ID; } set { _HOLIDAY_ID = value; OnPropertyChanged("HOLIDAY_ID"); } }        
        public string HOLIDAY_NAME { get { return _HOLIDAY_NAME; } set { _HOLIDAY_NAME = value; OnPropertyChanged("HOLIDAY_NAME"); } }
        
        #endregion

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("INSERT INTO HOLIDAYS(HOLIDAY_ID, HOLIDAY_NAME) VALUES (@HOLIDAY_ID, @HOLIDAY_NAME)", this, tran) == 1;
        }

        public override bool Update(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("UPDATE HOLIDAYS SET HOLIDAY_NAME = @HOLIDAY_NAME WHERE HOLIDAY_ID = @HOLIDAY_ID", this, tran) == 1;
        }

        public override bool Delete(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("DELETE FROM HOLIDAYS WHERE HOLIDAY_ID = @HOLIDAY_ID", this, tran) == 1;
        }
    }

    class Holiday_Date : RootModel
    {
        private short _HOLIDAY_ID;
        private byte _FYID;
        private DateTime _HOLIDAY_DATE;

        public short HOLIDAY_ID { get { return _HOLIDAY_ID; } set { _HOLIDAY_ID = value; OnPropertyChanged("HOLIDAY_ID"); } }        
        public byte FYID { get { return _FYID; } set { _FYID = value; OnPropertyChanged("FYID"); } }
        public DateTime HOLIDAY_DATE { get { return _HOLIDAY_DATE; } set { _HOLIDAY_DATE = value; OnPropertyChanged("HOLIDAY_DATE"); } }

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("INSERT INTO HOLIDAY_DATE(HOLIDAY_ID, FYID, HOLIDAY_DATE) VALUES (@HOLIDAY_ID, @FYID, @HOLIDAY_DATE)", this, tran) == 1;
        }
    }

    class Holiday_Gender : RootModel
    {
        #region Members
        private short _HOLIDAY_ID;
        private string _GENDER;
        #endregion

        #region Properties

        public short HOLIDAY_ID { get { return _HOLIDAY_ID; } set { _HOLIDAY_ID = value; OnPropertyChanged("HOLIDAY_ID"); } }
        public string GENDER { get { return _GENDER; } set { _GENDER = value; OnPropertyChanged("GENDER"); } }

        #endregion

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("INSERT INTO HOLIDAY_GENDER(HOLIDAY_ID, GENDER) VALUES (@HOLIDAY_ID, @GENDER)", this, tran) == 1;
        }
    }

    class Holiday_Branch : RootModel
    {
        #region Members
        private short _HOLIDAY_ID;
        private short _BRANCH_ID;
        #endregion

        #region Properties
        public short HOLIDAY_ID { get { return _HOLIDAY_ID; } set { _HOLIDAY_ID = value; OnPropertyChanged("HOLIDAY_ID"); } }
        public short BRANCH_ID { get { return _BRANCH_ID; } set { _BRANCH_ID = value; OnPropertyChanged("BRANCH_ID"); } }
        #endregion

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("INSERT INTO HOLIDAY_BRANCH(HOLIDAY_ID, BRANCH_ID) VALUES (@HOLIDAY_ID, @BRANCH_ID)", this, tran) == 1;
        }
    }

    class Holiday_Religion : RootModel
    {
        #region Members
        private short _HOLIDAY_ID;
        private string _RELIGION;
        #endregion

        #region Properties

        public short HOLIDAY_ID { get { return _HOLIDAY_ID; } set { _HOLIDAY_ID = value; OnPropertyChanged("HOLIDAY_ID"); } }
        public string RELIGION { get { return _RELIGION; } set { _RELIGION = value; OnPropertyChanged("RELIGION"); } }

        #endregion

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("INSERT INTO HOLIDAY_RELIGION(HOLIDAY_ID, RELIGION) VALUES (@HOLIDAY_ID, @RELIGION)", this, tran) == 1;
        }
    }
}
