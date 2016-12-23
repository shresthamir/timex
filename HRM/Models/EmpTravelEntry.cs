using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Models
{
    class Emp_Travel_Entry : RootModel
    {
        #region Members

        private short _TRAVEL_ID;
        private short _ENO;
        private DateTime _STARTDATE;
        private DateTime _ENDDATE;
        private string _REMARKS;

        #endregion

        #region Properties

        public short TRAVEL_ID { get { return _TRAVEL_ID; } set { _TRAVEL_ID = value; OnPropertyChanged("TRAVEL_ID"); } }

        public short ENO { get { return _ENO; } set { _ENO = value; OnPropertyChanged("ENO"); } }

        public DateTime STARTDATE { get { return _STARTDATE; } set { _STARTDATE = value; OnPropertyChanged("STARTDATE"); } }

        public DateTime ENDDATE { get { return _ENDDATE; } set { _ENDDATE = value; OnPropertyChanged("ENDDATE"); } }

        public string REMARKS { get { return _REMARKS; } set { _REMARKS = value; OnPropertyChanged("REMARKS"); } }

        #endregion
    }

}
