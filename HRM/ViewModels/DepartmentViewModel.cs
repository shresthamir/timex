using HRM.Library.BaseClasses;
using HRM.Library.Helpers;
using HRM.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using HRM.Library.AppScopeClasses;
using System.Windows;
using HRM.UI.Misc;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections;
using System.Collections.ObjectModel;

namespace HRM.ViewModels
{
    class DepartmentViewModel : RootViewModel
    {
        Department Root;
        private IEnumerable<Department> TreeSource;
        private Department _department;
        private ObservableCollection<Department> _TreeModel;
        private Department _SelectedDepartment;
        public Department department { get { return _department; } set { _department = value; OnPropertyChanged("department"); } }
        public Department SelectedDepartment { get { return _SelectedDepartment; } set { _SelectedDepartment = value; OnPropertyChanged("SelectedDepartment"); } }
        public ObservableCollection<Department> TreeModel { get { return _TreeModel; } set { _TreeModel = value; OnPropertyChanged("TreeModel"); } }
        public DepartmentViewModel()
        {
            try
            {
                HasGroup = true;
                MessageBoxCaption = "Department Setup";
                department = new Department();
                
                NewCommand = new RelayCommand(NewAction);
                DeleteCommand = new RelayCommand(DeleteDepartment);
                ClearCommand = new RelayCommand(ClearInterface);
                SaveCommand = new RelayCommand(SaveDepartment);
                LoadData = new RelayCommand(LoadBranchInfo);
                TreeInit();
            }
            catch (Exception ex)
            {

            }
        }

        void TreeInit()
        {
            TreeModel = new ObservableCollection<Department>();
            Root = new Department() { DEPARTMENT_ID = 0, DEPARTMENT = "Departments" };
            TreeModel.Add(Root);
            using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
            {
                TreeSource = conn.Query<Department>("SELECT DEPARTMENT_ID, DEPARTMENT, PARENT, LEVEL FROM DEPARTMENT");
            }
            LoadTree(Root);
            Root.IsExpanded = true;
        }

        void LoadTree(Department Parent)
        {
            foreach (Department d in TreeSource.Where(x => x.PARENT == Parent.DEPARTMENT_ID))
            {
                d.Parent = Parent;
                LoadTree(d);
                d.PropertyChanged += d_PropertyChanged;
                Parent.Children.Add(d);
            }
        }

        void d_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected" && (sender as ITreeItem).IsSelected)
            {
                SelectedDepartment = sender as Department;
                department.DEPARTMENT_ID = SelectedDepartment.DEPARTMENT_ID;
                department.DEPARTMENT = SelectedDepartment.DEPARTMENT;
                department.Parent = SelectedDepartment.Parent;
                department.LEVEL = SelectedDepartment.LEVEL;
                SetAction(ButtonAction.Selected);
            }
        }

        private void ClearInterface(object obj)
        {
            department = new Department();
            SetAction(ButtonAction.Init);
        }

        private void NewAction(object obj)
        {
            try
            {
                ClearInterface(null);
                if (obj != null && obj.ToString() == "I")
                {
                    if (SelectedDepartment.LEVEL <= 0)
                    {
                        ShowWarning("Cannot create Sub-Department here. Please select a parent department first and try again");
                        return;
                    }
                    else if (SelectedDepartment.LEVEL == 1)
                    {
                        department.Parent = SelectedDepartment;
                    }
                    else
                    {
                        department.Parent = SelectedDepartment.Parent;
                    }
                    department.LEVEL = 2;
                }
                else
                {
                    if (SelectedDepartment!=null && SelectedDepartment.LEVEL > 1)
                    {
                        ShowWarning("Cannot create Department here. Please select a root node and try again");
                        return;
                    }
                    department.Parent = Root;
                    department.LEVEL = 1;
                }
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    department.DEPARTMENT_ID = conn.ExecuteScalar<short>("SELECT ISNULL(MAX(DEPARTMENT_ID),0) + 1 FROM DEPARTMENT");
                }
                SetAction(ButtonAction.New);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void SaveDepartment(object obj)
        {
            try
            {
                if (ShowConfirmation("You are going to " + ((_action == ButtonAction.New) ? "save new" : "update selected") + " Department. Do you want to Continue?") && department.IsValid)
                {
                    DBDepartment d = new DBDepartment()
                    {
                        DEPARTMENT_ID = department.DEPARTMENT_ID,
                        PARENT = department.PARENT,
                        DEPARTMENT = department.DEPARTMENT,
                        LEVEL = department.LEVEL
                    };
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            if (_action == ButtonAction.New)
                                d.Save(tran);
                            else if (_action == ButtonAction.Edit)
                                d.Update(tran);
                            tran.Commit();
                        }
                    }
                    TreeInit();
                    SelectedDepartment = null;
                    ShowInformation("Department successfully saved.");
                    ClearInterface(null);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void DeleteDepartment(object obj)
        {
            if (ShowConfirmation("You are going to delete selected Department. Do you want to continue?"))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            department.Delete(tran);
                            tran.Commit();
                        }
                    }
                    ShowInformation("Department successfully deleted.");
                    ClearInterface(null);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        private void LoadBranchInfo(object obj)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    if (obj == null)
                    {
                        var branches = conn.Query<Department>("SELECT DEPARTMENT_ID, DEPARTMENT, FROM DEPARTMENT");
                        BrowseViewModel bvm = new BrowseViewModel(branches, "DEPARTMENT_ID", "DEPARTMENT");
                        Browse browse = new Browse() { DataContext = bvm };
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("DEPARTMENT_ID"), Header = "Id", Width = 100 });
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("DEPARTMENT"), Header = "Department Name", Width = 300 });
                        browse.ShowDialog();
                        if (browse.DialogResult != true)
                            return;
                        department.DEPARTMENT_ID = (browse.SearchGrid.SelectedItem as Department).DEPARTMENT_ID;
                    }
                    else
                    {
                        department.DEPARTMENT_ID = short.Parse(obj.ToString());
                    }

                    var b = conn.Query<Department>("SELECT DEPARTMENT_ID, DEPARTMENT, PARENT, LEVEL FROM DEPARTMENT WHERE DEPARTMENT_ID = @DEPARTMENT_ID", department);
                    if (b != null && b.Count() > 0)
                    {
                        department = b.FirstOrDefault();
                        SetAction(ButtonAction.Selected);
                    }
                    else
                        ShowWarning("Invalid Department Id");
                }
            }
            catch (Exception Ex)
            {
                ShowError(Ex.Message);
            }

        }
    }
}
