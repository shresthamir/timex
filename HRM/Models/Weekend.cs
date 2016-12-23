using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace HRM.Models
{
    class Weekend : RootModel
    {
        private short _ENO;
        private byte _DAY_ID;
        private DateTime _EFFECTIVE_DATE;
        private DateTime _LATE_DATE;

        public short ENO { get { return _ENO; } set { _ENO = value; OnPropertyChanged("ENO"); } }
        public byte DAY_ID { get { return _DAY_ID; } set { _DAY_ID = value; OnPropertyChanged("DAY_ID"); } }
        public DateTime EFFECTIVE_DATE { get { return _EFFECTIVE_DATE; } set { _EFFECTIVE_DATE = value; OnPropertyChanged("EFFECTIVE_DATE"); } }
        public DateTime LAST_DATE { get { return _LATE_DATE; } set { _LATE_DATE = value; OnPropertyChanged("LAST_DATE"); } }

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("INSERT INTO EMPLOYEE_WEEKEND (ENO, DAY_ID, EFFECTIVE_DATE) VALUES (@ENO, @DAY_ID, @EFFECTIVE_DATE)", this, tran) == 1;
        }



    }

    class WeekDay : RootModel
    {
        private byte _DAY_ID;
        private string _WEEKDAY;
        public byte DAY_ID { get { return _DAY_ID; } set { _DAY_ID = value; OnPropertyChanged("DAY_ID"); } }
        public string WEEKDAY { get { return _WEEKDAY; } set { _WEEKDAY = value; OnPropertyChanged("WEEKDAY"); } }
    }

}
