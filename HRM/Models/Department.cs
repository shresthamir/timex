using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.ComponentModel;
using System.Collections.ObjectModel;
namespace HRM.Models
{
    class DBDepartment : RootModel, IDataErrorInfo
    {
        #region Members

        private short _DEPARTMENT_ID;
        private string _DEPARTMENT;
        private short _PARENT;
        private byte _LEVEL;

        #endregion

        #region Properties

        public short DEPARTMENT_ID { get { return _DEPARTMENT_ID; } set { _DEPARTMENT_ID = value; OnPropertyChanged("DEPARTMENT_ID"); } }

        public string DEPARTMENT { get { return _DEPARTMENT; } set { _DEPARTMENT = value; OnPropertyChanged("DEPARTMENT"); } }

        public short PARENT { get { return _PARENT; } set { _PARENT = value; OnPropertyChanged("PARENT"); } }

        public byte LEVEL { get { return _LEVEL; } set { _LEVEL = value; OnPropertyChanged("LEVEL"); } }

        #endregion

        public override bool Save(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("INSERT INTO DEPARTMENT(DEPARTMENT_ID, DEPARTMENT, PARENT, [LEVEL]) VALUES (@DEPARTMENT_ID, @DEPARTMENT, @PARENT, @LEVEL)", this, tran) == 0;
        }

        public override bool Update(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("UPDATE DEPARTMENT SET  DEPARTMENT = @DEPARTMENT, PARENT = @PARENT, [LEVEL] = @LEVEL WHERE DEPARTMENT_ID = @DEPARTMENT_ID", this, tran) == 1;
        }

        public override bool Delete(System.Data.SqlClient.SqlTransaction tran)
        {
            return tran.Connection.Execute("DELETE FROM DEPARTMENT WHERE DEPARTMENT_ID = @DEPARTMENT_ID", this, tran) == 1;
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
                if (string.IsNullOrEmpty(DEPARTMENT))
                    return "Department Name cannot be Empty";
                return string.Empty;
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "DEPARTMENT":
                        if (string.IsNullOrEmpty(DEPARTMENT))
                            return "Department Name cannot be Empty";
                        break;
                    default:
                        return string.Empty;
                }
                return string.Empty;
            }
        }
    }

    class Department : DBDepartment, ITreeItem
    {
        private ITreeItem _Parent;
        private ObservableCollection<ITreeItem> _Children;
        private bool _IsExpanded;
        private bool _IsNodeSelected;
        public ITreeItem Parent { get { return _Parent; } set { _Parent = value; PARENT = (value as Department).DEPARTMENT_ID; OnPropertyChanged("Parent"); } }

        public ObservableCollection<ITreeItem> Children
        {
            get
            {
                if (_Children == null)
                    _Children = new ObservableCollection<ITreeItem>();
                return _Children;
            }
            set
            {
                _Children = value; OnPropertyChanged("Children");
            }
        }



        public string NodeName { get { return this.DEPARTMENT; } }
        public string NodeID { get { return this.DEPARTMENT_ID.ToString(); } }
        public bool IsNodeSelected { get { return _IsNodeSelected; } set { _IsNodeSelected = value; OnPropertyChanged("IsNodeSelected"); } }
        public bool IsNodeExpanded { get { return _IsExpanded; } set { _IsExpanded = value; OnPropertyChanged("IsExpanded"); } }
        public void ExpandTree(ITreeItem Branch)
        {
            foreach (ITreeItem ti in Branch.Children)
            {
                ExpandTree(ti);
            }
            Branch.IsNodeExpanded = true;


        }



        
    }

}
