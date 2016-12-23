using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Models
{
    class Attendance : RootModel
    {
        #region Members

        private short _ENO;
        private byte _FYID;
        private short _WORKHOUR_ID;
        private DateTime _ATT_DATE;
        private DateTime _CHECKIN;
        private string _CHECKINMODE;
        private string _CHECKINREMARKS;
        private DateTime? _CHECKOUT;
        private string _CHECKOUTMODE;
        private string _CHECKOUTREMARKS;
        private DateTime? _BREAKOUT;
        private string _BREAKOUTMODE;
        private string _BREAKOUTREMARKS;
        private DateTime? _BREAKIN;
        private string _BREAKINMODE;
        private string _BREAKINREMARKS;
        private DateTime? _POUT;
        private string _POUTMODE;
        private string _POUTREMARKS;
        private DateTime? _PIN;
        private string _PINMODE;
        private string _PINREMARKS;
        private DateTime? _OFFICEOUT;
        private string _OFFICEOUTMODE;
        private string _OFFICEOUTREMARKS;
        private DateTime? _OFFICEIN;
        private string _OFFICEINMODE;
        private string _OFFICEINREMARKS;
        private DateTime? _LUNCHOUT;
        private string _LUNCHOUTMODE;
        private string _LUNCHOUTREMARKS;
        private DateTime? _LUNCHIN;
        private string _LUNCHINMODE;
        private string _LUNCHINREMARKS;
        private short _LEAVE_ID;

        #endregion

        #region Properties

        public short ENO { get { return _ENO; } set { _ENO = value; OnPropertyChanged("ENO"); } }

        public byte FYID { get { return _FYID; } set { _FYID = value; OnPropertyChanged("FYID"); } }

        public short WORKHOUR_ID { get { return _WORKHOUR_ID; } set { _WORKHOUR_ID = value; OnPropertyChanged("WORKHOUR_ID"); } }

        public DateTime ATT_DATE { get { return _ATT_DATE; } set { _ATT_DATE = value; OnPropertyChanged("WORKDATE"); } }

        public DateTime CHECKIN { get { return _CHECKIN; } set { _CHECKIN = value; OnPropertyChanged("CHECKIN"); } }

        public string CHECKINMODE { get { return _CHECKINMODE; } set { _CHECKINMODE = value; OnPropertyChanged("CHECKINMODE"); } }

        public string CHECKINREMARKS { get { return _CHECKINREMARKS; } set { _CHECKINREMARKS = value; OnPropertyChanged("CHECKINREMARKS"); } }

        public DateTime? CHECKOUT { get { return _CHECKOUT; } set { _CHECKOUT = value; OnPropertyChanged("CHECKOUT"); } }

        public string CHECKOUTMODE { get { return _CHECKOUTMODE; } set { _CHECKOUTMODE = value; OnPropertyChanged("CHECKOUTMODE"); } }

        public string CHECKOUTREMARKS { get { return _CHECKOUTREMARKS; } set { _CHECKOUTREMARKS = value; OnPropertyChanged("CHECKOUTREMARKS"); } }

        public DateTime? BREAKOUT { get { return _BREAKOUT; } set { _BREAKOUT = value; OnPropertyChanged("BREAKOUT"); } }

        public string BREAKOUTMODE { get { return _BREAKOUTMODE; } set { _BREAKOUTMODE = value; OnPropertyChanged("BREAKOUTMODE"); } }

        public string BREAKOUTREMARKS { get { return _BREAKOUTREMARKS; } set { _BREAKOUTREMARKS = value; OnPropertyChanged("BREAKOUTREMARKS"); } }

        public DateTime? BREAKIN { get { return _BREAKIN; } set { _BREAKIN = value; OnPropertyChanged("BREAKIN"); } }

        public string BREAKINMODE { get { return _BREAKINMODE; } set { _BREAKINMODE = value; OnPropertyChanged("BREAKINMODE"); } }

        public string BREAKINREMARKS { get { return _BREAKINREMARKS; } set { _BREAKINREMARKS = value; OnPropertyChanged("BREAKINREMARKS"); } }


        public DateTime? LUNCHOUT { get { return _LUNCHOUT; } set { _LUNCHOUT = value; OnPropertyChanged("LUNCHOUT"); } }

        public string LUNCHOUTMODE { get { return _LUNCHOUTMODE; } set { _LUNCHOUTMODE = value; OnPropertyChanged("LUNCHOUTMODE"); } }

        public string LUNCHOUTREMARKS { get { return _LUNCHOUTREMARKS; } set { _LUNCHOUTREMARKS = value; OnPropertyChanged("LUNCHOUTREMARKS"); } }

        public DateTime? LUNCHIN { get { return _LUNCHIN; } set { _LUNCHIN = value; OnPropertyChanged("LUNCHIN"); } }

        public string LUNCHINMODE { get { return _LUNCHINMODE; } set { _LUNCHINMODE = value; OnPropertyChanged("LUNCHINMODE"); } }

        public string LUNCHINREMARKS { get { return _LUNCHINREMARKS; } set { _LUNCHINREMARKS = value; OnPropertyChanged("LUNCHINREMARKS"); } }

        public DateTime? POUT { get { return _POUT; } set { _POUT = value; OnPropertyChanged("POUT"); } }

        public string POUTMODE { get { return _POUTMODE; } set { _POUTMODE = value; OnPropertyChanged("POUTMODE"); } }

        public string POUTREMARKS { get { return _POUTREMARKS; } set { _POUTREMARKS = value; OnPropertyChanged("POUTREMARKS"); } }

        public DateTime? PIN { get { return _PIN; } set { _PIN = value; OnPropertyChanged("PIN"); } }

        public string PINMODE { get { return _PINMODE; } set { _PINMODE = value; OnPropertyChanged("PINMODE"); } }

        public string PINREMARKS { get { return _PINREMARKS; } set { _PINREMARKS = value; OnPropertyChanged("PINREMARKS"); } }

        public DateTime? OFFICEOUT { get { return _OFFICEOUT; } set { _OFFICEOUT = value; OnPropertyChanged("OFFICEOUT"); } }

        public string OFFICEOUTMODE { get { return _OFFICEOUTMODE; } set { _OFFICEOUTMODE = value; OnPropertyChanged("OFFICEOUTMODE"); } }

        public string OFFICEOUTREMARKS { get { return _OFFICEOUTREMARKS; } set { _OFFICEOUTREMARKS = value; OnPropertyChanged("OFFICEOUTREMARKS"); } }

        public DateTime? OFFICEIN { get { return _OFFICEIN; } set { _OFFICEIN = value; OnPropertyChanged("OFFICEIN"); } }

        public string OFFICEINMODE { get { return _OFFICEINMODE; } set { _OFFICEINMODE = value; OnPropertyChanged("OFFICEINMODE"); } }

        public string OFFICEINREMARKS { get { return _OFFICEINREMARKS; } set { _OFFICEINREMARKS = value; OnPropertyChanged("OFFICEINREMARKS"); } }

        public short LEAVE_ID { get { return _LEAVE_ID; } set { _LEAVE_ID = value; OnPropertyChanged("LEAVE_ID"); } }

        #endregion
    }


}
