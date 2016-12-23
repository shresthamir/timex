using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.ComponentModel;
namespace HRM.Models
{
    class Branch : RootModel, IDataErrorInfo
    {

        private short _BRANCH_ID;
        private string _BRANCH_NAME;
        private string _BRANCH_ADDRESS;
        private string _BRANCH_PHONENO;
        

        public short BRANCH_ID { get { return _BRANCH_ID; } set { _BRANCH_ID = value; OnPropertyChanged("BRANCH_ID"); } }
        public string BRANCH_NAME { get { return _BRANCH_NAME; } set { _BRANCH_NAME = value; OnPropertyChanged("BRANCH_NAME"); } }
        public string BRANCH_ADDRESS { get { return _BRANCH_ADDRESS; } set { _BRANCH_ADDRESS = value; OnPropertyChanged("BRANCH_ADDRESS"); } }
        public string BRANCH_PHONENO { get { return _BRANCH_PHONENO; } set { _BRANCH_PHONENO = value; OnPropertyChanged("BRANCH_PHONENO"); } }


        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("INSERT INTO BRANCH (BRANCH_ID, BRANCH_NAME, BRANCH_ADDRESS, BRANCH_PHONENO) VALUES ((SELECT ISNULL(MAX(BRANCH_ID),0) + 1 FROM BRANCH), @BRANCH_NAME, @BRANCH_ADDRESS, @BRANCH_PHONENO)", this, tran) == 1;
        }

        public override bool Update(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("UPDATE BRANCH SET BRANCH_NAME = @BRANCH_NAME,BRANCH_ADDRESS = @BRANCH_ADDRESS,BRANCH_PHONENO = @BRANCH_PHONENO WHERE BRANCH_ID = @BRANCH_ID ", this, tran) == 1;
        }

        public override bool Delete(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("DELETE FROM BRANCH WHERE BRANCH_ID = @BRANCH_ID", this, tran) == 1;
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
                if (string.IsNullOrEmpty(BRANCH_NAME))
                    return "Branch Name cannot be Empty";
                else
                    return string.Empty;
            }
        }

        public string this[string columnName]
        {
            get 
            {
                switch (columnName)
                {
                    case "BRANCH_NAME":
                        if (string.IsNullOrEmpty(BRANCH_NAME))
                            return "Branch Name cannot be Empty";
                        break;
                    default :
                        return string.Empty;
                }
                return string.Empty;
            }
        }
    }
}
