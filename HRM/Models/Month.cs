using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Models
{
    class Month : RootModel
    {
        private short _MID;
        private string _MTYPE;
        private string _MNAME;
        public short MID { get { return _MID; } set { _MID = value; OnPropertyChanged("MID"); } }
        public string MTYPE { get { return _MTYPE; } set { _MTYPE = value; OnPropertyChanged("MTYPE"); } }
        public string MNAME { get { return _MNAME; } set { _MNAME = value; OnPropertyChanged("MNAME"); } }
    }
}
