using HRM.Library.AppScopeClasses;
using HRM.Library.BaseClasses;
using HRM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace HRM.ViewModels
{
    class EmployeeTreeViewModel:RootViewModel
    {
        IEnumerable<Department> DepartmentList;
        IEnumerable<EmployeeAllDetail> EmployeeList;
        private ObservableCollection<ITreeItem> _TreeSource;

        public ObservableCollection<ITreeItem> TreeSource { get { return _TreeSource; } set { _TreeSource = value; OnPropertyChanged("TreeSource"); } }

        public EmployeeTreeViewModel()
        {
            PopulateTree();
        }

        void PopulateTree()
        {
            try
            {
                TreeSource = new ObservableCollection<ITreeItem>();
                var Root = new Department() { DEPARTMENT_ID = 0, DEPARTMENT = "Employee List" };
                TreeSource.Add(Root);
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    DepartmentList = conn.Query<Department>("SELECT DEPARTMENT_ID, DEPARTMENT, PARENT, LEVEL FROM DEPARTMENT");
                    EmployeeList = conn.Query<EmployeeAllDetail>("SELECT * FROM vwEmpDetail");
                }
                LoadTree(Root);
                Root.IsExpanded = true;
            }
            catch (Exception ex)
            {
                ShowError(ExceptionHandler.GetException(ex).Message);
            }
        }

        void LoadTree(ITreeItem Parent)
        {
            foreach (Department d in DepartmentList.Where(x => x.ParentID == Parent.NodeID))
            {
                d.Parent = Parent;
                LoadTree(d);
                //d.PropertyChanged += d_PropertyChanged;
                Parent.Children.Add(d);
                d.IsExpanded = true;
            }
            foreach (EmployeeAllDetail e in EmployeeList.Where(x => x.ParentID == Parent.NodeID))
            {
                e.Parent = Parent;                
                //d.PropertyChanged += d_PropertyChanged;
                Parent.Children.Add(e);
            }
        }
    }
}
