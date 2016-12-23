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
    class Designation : RootModel, IDataErrorInfo
    {
        #region Members

        private short _DESIGNATION_ID;
        private string _DESIGNATION;
        private byte _LEVEL;

        #endregion

        #region Properties

        public short DESIGNATION_ID { get { return _DESIGNATION_ID; } set { _DESIGNATION_ID = value; OnPropertyChanged("DESIGNATION_ID"); } }

        public string DESIGNATION { get { return _DESIGNATION; } set { _DESIGNATION = value; OnPropertyChanged("DESIGNATION"); } }

        public byte LEVEL { get { return _LEVEL; } set { _LEVEL = value; OnPropertyChanged("DEPARTMENT_ID"); } }

        #endregion

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("INSERT INTO DESIGNATION(DESIGNATION_ID, DESIGNATION, LEVEL) VALUES ((SELECT ISNULL(MAX(DESIGNATION_ID),0) + 1 FROM DESIGNATION), @DESIGNATION, @LEVEL)", this, tran) == 1;
        }

        public override bool Update(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("UPDATE DESIGNATION SET DESIGNATION = @DESIGNATION, LEVEL = @LEVEL WHERE DESIGNATION_ID = @DESIGNATION_ID", this, tran) == 1;
        }

        public override bool Delete(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("DELETE FROM DESIGNATION WHERE DESIGNATION_ID = @DESIGNATION_ID", this, tran) == 1;
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
                    case "DESIGNATION":
                        if (string.IsNullOrEmpty(DESIGNATION))
                            return "Designation cannot be empty.";
                        break;
                    case "LEVEL":
                        if (LEVEL < 0)
                            return "LEVEL cannot be less than zero.";
                        break;                   
                }
                return string.Empty;
            }
        }
    }

}
