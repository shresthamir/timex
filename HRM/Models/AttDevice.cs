using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
namespace HRM.Models
{
    class AttDevice : RootModel
    {
        #region Members

        private string _DISPLAY_NAME;
        private string _DEVICE_MODEL;
        private string _DEVICE_IP;
        private byte _DEVICE_MODE;
        private byte _STATUS;
        private int _LogCount = 100;
        private int _Counter;
        private string _DEVICE_SERIAL;
        private DateTime _DEVICE_DATE;

        #endregion

        #region Properties

        public string DISPLAY_NAME { get { return _DISPLAY_NAME; } set { _DISPLAY_NAME = value; OnPropertyChanged("DISPLAY_NAME"); } }

        public string DEVICE_MODEL { get { return _DEVICE_MODEL; } set { _DEVICE_MODEL = value; OnPropertyChanged("DEVICE_MODEL"); } }

        public string DEVICE_IP { get { return _DEVICE_IP; } set { _DEVICE_IP = value; OnPropertyChanged("DEVICE_IP"); } }

        public string DEVICE_SERIAL { get { return _DEVICE_SERIAL; } set { _DEVICE_SERIAL = value; OnPropertyChanged("DEVICE_SERIAL"); } }

        public byte DEVICE_MODE { get { return _DEVICE_MODE; } set { _DEVICE_MODE = value; OnPropertyChanged("DEVICE_MODE"); } }

        public byte STATUS { get { return _STATUS; } set { _STATUS = value; OnPropertyChanged("STATUS"); } }

        public int LogCount { get { return _LogCount; } set { _LogCount = value; OnPropertyChanged("LogCount"); } }

        public int Counter { get { return _Counter; } set { _Counter = value; OnPropertyChanged("Counter"); } }

        public DateTime DEVICE_DATE { get { return _DEVICE_DATE; } set { _DEVICE_DATE = value; OnPropertyChanged("DEVICE_DATE"); } }

        #endregion

        public override bool Save(SqlTransaction tran)
        {
            return tran.Connection.Execute("INSERT INTO ATT_DEVICE(DISPLAY_NAME, DEVICE_MODEL, DEVICE_IP, DEVICE_MODE) VALUES (@DISPLAY_NAME, @DEVICE_MODEL, @DEVICE_IP, 0)", this, tran) == 1;
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
                    case "DEVICE_NAME":
                        if (string.IsNullOrEmpty(DISPLAY_NAME))
                            return "Display Name cannot be empty.";
                        break;
                    case "DEVICE_MODEL":
                        if (string.IsNullOrEmpty(DEVICE_MODEL))
                            return "Device Model cannot be empty.";
                        break;
                    case "DEVICE_IP":
                        if (string.IsNullOrEmpty(DEVICE_IP))
                            return "Device IP cannot be empty.";
                        else if (!Regex.Match(DEVICE_IP, "^(([1-9]?\\d|1\\d\\d|2[0-5][0-5]|2[0-4]\\d)\\.){3}([1-9]?\\d|1\\d\\d|2[0-5][0-5]|2[0-4]\\d)$").Success)
                            return "Please enter valid IP Address";
                        break;
                }
                return string.Empty;
            }
        }
    }
}
