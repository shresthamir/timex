using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Library.BaseClasses
{
    class RootModel : INotifyPropertyChanged
    {
        public bool Validate;
        private bool _IsSelected;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propname));
            }
        }

        public virtual void Clear()
        {

        }

        public virtual bool IsValid { get { return false; } }

        public virtual string ID { get{ return string.Empty;} }
        public virtual string NAME { get { return string.Empty; } }

        public bool IsSelected { get { return _IsSelected; } set { _IsSelected = value; OnPropertyChanged("IsSelected"); } }

        public virtual bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return true;
        }

        public virtual bool Delete(System.Data.SqlClient.SqlTransaction tran)
        {
            return  true;
        }


        public virtual bool Update(System.Data.SqlClient.SqlTransaction tran)
        {
            return true;
        }

        public virtual object GetObject()
        {
            return this;
        }

        public void ValidateModel()
        {
            Validate = true;
            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {
                OnPropertyChanged(pi.Name);
            }
        }
    }
}
