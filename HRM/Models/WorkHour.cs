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
    class WorkHour : RootModel, IDataErrorInfo
    {
        #region Members

        private short _WORKHOUR_ID;
        private string _DESCRIPTION;
        private bool _ISDEFAULT;
        private DateTime _INTIME = new DateTime(1900, 1, 1);
        private DateTime _INGRACETIME = new DateTime(1900, 1, 1);
        private DateTime _OUTTIME = new DateTime(1900, 1, 1);
        private DateTime _OUTGRACETIME = new DateTime(1900, 1, 1);
        private short _LUNCHDURATION;
        private short _BREAKDURATION;
        private short _TOTALDURATION;

        #endregion

        #region Properties

        public short WORKHOUR_ID { get { return _WORKHOUR_ID; } set { _WORKHOUR_ID = value; OnPropertyChanged("WORKHOUR_ID"); } }

        public string DESCRIPTION { get { return _DESCRIPTION; } set { _DESCRIPTION = value; OnPropertyChanged("DESCRIPTION"); } }

        public bool ISDEFAULT { get { return _ISDEFAULT; } set { _ISDEFAULT = value; OnPropertyChanged("ISDEFAULT"); } }

        public DateTime INTIME { get { return _INTIME; } set { _INTIME = value; OnPropertyChanged("INTIME"); } }

        public DateTime INGRACETIME { get { return _INGRACETIME; } set { _INGRACETIME = value; OnPropertyChanged("INGRACETIME"); } }

        public DateTime OUTTIME { get { return _OUTTIME; } set { _OUTTIME = value; OnPropertyChanged("OUTTIME"); } }

        public DateTime OUTGRACETIME { get { return _OUTGRACETIME; } set { _OUTGRACETIME = value; OnPropertyChanged("OUTGRACETIME"); } }

        public short LUNCHDURATION { get { return _LUNCHDURATION; } set { _LUNCHDURATION = value; OnPropertyChanged("LUNCHDURATION"); } }

        public short BREAKDURATION { get { return _BREAKDURATION; } set { _BREAKDURATION = value; OnPropertyChanged("BREAKDURATION"); } }

        public short TOTALDURATION { get { return _TOTALDURATION; } set { _TOTALDURATION = value; OnPropertyChanged("TOTALDURATION"); } }

        #endregion

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute(@"INSERT INTO WorkHour(WORKHOUR_ID, DESCRIPTION, ISDEFAULT, INTIME, INGRACETIME, OUTTIME, OUTGRACETIME, LUNCHDURATION, BREAKDURATION, TOTALDURATION) 
			        VALUES (@WORKHOUR_ID, @DESCRIPTION, @ISDEFAULT, @INTIME, @INGRACETIME, @OUTTIME, @OUTGRACETIME, @LUNCHDURATION, @BREAKDURATION, @TOTALDURATION)", this, tran) == 1;
        }

        public override bool Update(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute(@"UPDATE WorkHour SET DESCRIPTION = @DESCRIPTION, ISDEFAULT = @ISDEFAULT,
							                        INTIME = @INTIME, INGRACETIME = @INGRACETIME, OUTTIME = @OUTTIME, OUTGRACETIME = @OUTGRACETIME,
							                        LUNCHDURATION = @LUNCHDURATION, BREAKDURATION = @BREAKDURATION, TOTALDURATION = @TOTALDURATION WHERE WORKHOUR_ID = @WORKHOUR_ID",this, tran) == 1;
        }

        public override bool Delete(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("DELETE FROM WORKHOUR WHERE WORKHOUR_ID = @WORKHOUR_ID", this, tran) == 1;
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
                    case "DESCRIPTION":
                        if (string.IsNullOrEmpty(DESCRIPTION))
                            return "Workhour name cannot be Empty";
                        else if (DESCRIPTION.Length > 100)
                            return "Workhour name cannot be longer than 100 characters.";
                        break;
                    case "INGRACETIME":
                        if (INTIME > INGRACETIME)
                            return "In Grace time cannot be less than In Time";
                        break;
                    case "OUTTIME":
                        if (INTIME > OUTTIME)
                            return "Out Time cannot be less than In Time";
                        break;
                    case "OUTGRACETIME":
                        if (INTIME > OUTGRACETIME)
                            return "Out Grace Time cannot be less than In Time";
                        else if (OUTTIME < OUTGRACETIME)
                            return "Out Grace Time cannot be greater than Out Time";
                        break;
                    case "LUNCHDURATION":
                        if (LUNCHDURATION < 0)
                            return "Lunch Duration cannot be less than zero";
                        break;
                    case "BREAKDURATION":
                        if (BREAKDURATION < 0)
                            return "Break Duration cannot be less than zero";
                        break;
                }
                return string.Empty;
            }
        }
    }

    class Day : RootModel
    {
        private byte _DayId;
        private string _DayName;
        private bool _IsChecked;
        private bool _IsEnabled = true;
        public byte DayId { get { return _DayId; } set { _DayId = value; OnPropertyChanged("DayId"); } }
        public string DayName { get { return _DayName; } set { _DayName = value; OnPropertyChanged("DayName"); } }
        public bool IsChecked { get { return _IsChecked; } set { _IsChecked = value; OnPropertyChanged("IsChecked"); } }
        public bool IsEnabled { get { return _IsEnabled; } set { _IsEnabled = value; OnPropertyChanged("IsEnabled"); } }
    }

    class DBEmployeeWorkhour : RootModel
    {
        private int _ENO;
        private int _DAY_ID;
        private int _WORKHOUR_ID;
        private DateTime _EFFECTIVE_DATE;
        private DateTime _LAST_DATE;
        

        public int ENO { get { return _ENO; } set { _ENO = value; OnPropertyChanged("ENO"); } }
        public int DAY_ID { get { return _DAY_ID; } set { _DAY_ID = value; OnPropertyChanged("DAY_ID"); } }
        public int WORKHOUR_ID { get { return _WORKHOUR_ID; } set { _WORKHOUR_ID = value; OnPropertyChanged("WORKHOUR_ID"); } }
        public DateTime EFFECTIVE_DATE { get { return _EFFECTIVE_DATE; } set { _EFFECTIVE_DATE = value; OnPropertyChanged("EFFECTIVE_DATE"); } }
        public DateTime LAST_DATE { get { return _LAST_DATE; } set { _LAST_DATE = value; OnPropertyChanged("LAST_DATE"); } }
       
    }

    class EmployeeWorkhour : DBEmployeeWorkhour
    {
        private string _Days;
        private string _ENAME;
        private string _WHName;
        public string Days { get { return _Days; } set { _Days = value; OnPropertyChanged("Days"); } }
        public string ENAME { get { return _ENAME; } set { _ENAME = value; OnPropertyChanged("ENAME"); } }
        public string WHName { get { return _WHName; } set { _WHName = value; OnPropertyChanged("WHName"); } }
    }
}
