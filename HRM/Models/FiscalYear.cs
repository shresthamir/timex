using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Models
{
    class FiscalYear : RootModel
    {
        #region Members

        private byte _FYID;
        private string _FYNAME;
        private DateTime _STARTDATE;
        private DateTime _ENDDATE;
        private bool _ISACTIVE;

        #endregion

        #region Properties

        public byte FYID { get { return _FYID; } set { _FYID = value; OnPropertyChanged("FYID"); } }

        public string FYNAME { get { return _FYNAME; } set { _FYNAME = value; OnPropertyChanged("FYNAME"); } }

        public DateTime STARTDATE { get { return _STARTDATE; } set { _STARTDATE = value; OnPropertyChanged("STARTDATE"); } }

        public DateTime ENDDATE { get { return _ENDDATE; } set { _ENDDATE = value; OnPropertyChanged("ENDDATE"); } }

        public bool ISACTIVE { get { return _ISACTIVE; } set { _ISACTIVE = value; OnPropertyChanged("ISACTIVE"); } }

        #endregion
    }

}
