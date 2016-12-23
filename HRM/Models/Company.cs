using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace HRM.Models
{
    class Company : RootModel
    {
        #region Members

        private byte _COMPANY_ID;
        private string _COMPANY_NAME;
        private string _COMPANY_ADDRESS;
        private string _COMPANY_PHONENO;
        private string _COMPANY_EMAIL;
        private string _COMPANY_PAN;
        private string _COMPANY_ADDRESS_LINE2;

        #endregion

        #region Properties

        public byte COMPANY_ID { get { return _COMPANY_ID; } set { _COMPANY_ID = value; OnPropertyChanged("COMPANY_ID"); } }

        public string COMPANY_NAME { get { return _COMPANY_NAME; } set { _COMPANY_NAME = value; OnPropertyChanged("COMPANY_NAME"); } }

        public string COMPANY_ADDRESS { get { return _COMPANY_ADDRESS; } set { _COMPANY_ADDRESS = value; OnPropertyChanged("COMPANY_ADDRESS"); } }

        public string COMPANY_PHONENO { get { return _COMPANY_PHONENO; } set { _COMPANY_PHONENO = value; OnPropertyChanged("COMPANY_PHONENO"); } }

        public string COMPANY_EMAIL { get { return _COMPANY_EMAIL; } set { _COMPANY_EMAIL = value; OnPropertyChanged("COMPANY_EMAIL"); } }

        public string COMPANY_PAN { get { return _COMPANY_PAN; } set { _COMPANY_PAN = value; OnPropertyChanged("COMPANY_PAN"); } }

        public string COMPANY_ADDRESS_LINE2 { get { return _COMPANY_ADDRESS_LINE2; } set { _COMPANY_ADDRESS_LINE2 = value; OnPropertyChanged("COMPANY_ADDRESS_LINE2"); } }

        #endregion

        public override bool IsValid
        {
            get
            {
                return true;
            }
        }

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            string strSql = "INSERT INTO COMPANY(COMPANY_ID, COMPANY_NAME, COMPANY_ADDRESS, COMPANY_PHONENO, COMPANY_EMAIL, COMPANY_PAN, COMPANY_ADDRESS_LINE2)";
            strSql += Environment.NewLine + "VALUES (1, 'DEMO COMPANY PVT. LTD.', 'Buddha Nagar - 10', '9841000000', 'demo@democompany.com.np','123456789', 'Kathmandu, Nepal')";
            return tran.Connection.Execute(strSql, transaction: tran) == 1;
        }

        public override bool Update(System.Data.SqlClient.SqlTransaction tran)
        {
            string strSql = "UPDATE COMPANY SET COMPANY_NAME = @COMPANY_NAME, COMPANY_ADDRESS = @COMPANY_ADDRESS, COMPANY_PHONENO = @COMPANY_PHONENO, ";
            strSql += Environment.NewLine + "COMPANY_EMAIL = @COMPANY_EMAIL, COMPANY_PAN = @COMPANY_PAN, COMPANY_ADDRESS_LINE2 = @COMPANY_ADDRESS_LINE2 WHERE COMPANY_ID = @COMPANY_ID";
            return tran.Connection.Execute(strSql, this, transaction: tran) == 1;
        }
    }

}
