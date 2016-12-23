using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Models
{
    class AttDevice:RootModel
    {
        #region Members

        private string _DISPLAY_NAME;
        private string _DEVICE_MODEL;
        private string _DEVICE_IP;
        private byte _DEVICE_MODE;
        private byte _STATUS;

        #endregion

        #region Properties

        public string DISPLAY_NAME { get { return _DISPLAY_NAME; } set { _DISPLAY_NAME = value; OnPropertyChanged("DISPLAY_NAME"); } }

        public string DEVICE_MODEL { get { return _DEVICE_MODEL; } set { _DEVICE_MODEL = value; OnPropertyChanged("DEVICE_MODEL"); } }

        public string DEVICE_IP { get { return _DEVICE_IP; } set { _DEVICE_IP = value; OnPropertyChanged("DEVICE_IP"); } }

        public byte DEVICE_MODE { get { return _DEVICE_MODE; } set { _DEVICE_MODE = value; OnPropertyChanged("DEVICE_MODE"); } }

        public byte STATUS { get { return _STATUS; } set { _STATUS = value; OnPropertyChanged("STATUS"); } }

        #endregion
    }
}
