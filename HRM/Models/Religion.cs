using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Models
{
    class Religions:RootModel
    {
        string _Religion;
        public string RELIGION { get { return _Religion; } set { _Religion = value; OnPropertyChanged("RELIGION"); } }
    }
}
