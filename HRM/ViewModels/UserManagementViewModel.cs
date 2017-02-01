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
using System.Collections.ObjectModel;

namespace HRM.ViewModels
{
    class UserManagementViewModel : RootViewModel
    {
        private UserManagement _User;
        private ObservableCollection<UserManagement> _UserList;
        public UserManagement User { get { return _User; } set { _User = value; OnPropertyChanged("User"); } }
        public ObservableCollection<UserManagement> UserList { get { return _UserList; } set { _UserList = value; OnPropertyChanged("UserList"); } }

        public UserManagementViewModel()
        {
            try
            {
                MessageBoxCaption = "User Management";
                User = new UserManagement();
                NewCommand = new RelayCommand(NewAction);
                DeleteCommand = new RelayCommand(DeleteUser);
                ClearCommand = new RelayCommand(ClearInterface);
                SaveCommand = new RelayCommand(SaveUser);
                LoadData = new RelayCommand(LoadUserInfo);
                RefreshUsers();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
       

        void RefreshUsers()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    UserList = new ObservableCollection<UserManagement>(conn.Query<UserManagement>("SELECT UNAME, [DESCRIPTION], CATEGORY FROM UserManagement"));
                }
            }
            catch (Exception ex)
            {

                ShowError(ex.Message);
            }
        }

        private void LoadUserInfo(object obj)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    if (obj == null)
                    {
                        var wHours = conn.Query<UserManagement>("SELECT UNAME, [DESCRIPTION], CATEGORY FROM UserManagement");
                        BrowseViewModel bvm = new BrowseViewModel(wHours, "UNAME", "DESCRIPTION");
                        Browse browse = new Browse() { DataContext = bvm };
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("UNAME"), Header = "User Name", Width = 120 });
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("DESCRIPTION"), Header = "Description", Width = 180 });
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("CATEGORY") , Header = "Category", Width = 100 });                        
                        browse.ShowDialog();
                        if (browse.DialogResult != true)
                            return;
                        User.UNAME = (browse.SearchGrid.SelectedItem as UserManagement).UNAME;
                    }
                    else
                    {
                        User.UNAME = (obj as UserManagement).UNAME;
                    }

                    var b = conn.Query<UserManagement>("SELECT * FROM UserManagement WHERE UNAME = @UNAME", User);
                    if (b != null && b.Count() > 0)
                    {
                        User = b.FirstOrDefault();                        
                        SetAction(ButtonAction.Selected);
                    }
                    else
                        ShowWarning("Invalid Username");
                }
            }
            catch (Exception Ex)
            {
                ShowError(Ex.Message);
            }

        }

        private void DeleteUser(object obj)
        {
            if (User.UNAME == "admin")
                ShowWarning("User cannot be deleted");
            if (ShowConfirmation("You are going to delete selected User. Do you want to continue?"))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            User.Delete(tran);
                            tran.Commit();
                        }
                    }
                    ShowInformation("UserManagement successfully deleted.");
                    ClearInterface(null);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        private void ClearInterface(object obj)
        {
            User = new UserManagement();
            SetAction(ButtonAction.Init);
            RefreshUsers();
        }

        private void NewAction(object obj)
        {
            try
            {
                ClearInterface(null);                
                SetAction(ButtonAction.New);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void SaveUser(object obj)
        {
            try
            {
                User.PWD = string.Empty;
                if (!User.IsValid)
                {
                    ShowWarning(User.Error);
                    return;
                }
                if (ShowConfirmation("You are going to " + ((_action == ButtonAction.New) ? "save new" : "update selected") + " User. Do you want to Continue?") && User.IsValid)
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            if (_action == ButtonAction.New)
                                User.Save(tran);
                            else if (_action == ButtonAction.Edit)
                            {
                                User.Update(tran);
                            }
                            tran.Commit();
                        }
                    }
                    ShowInformation("User successfully saved.");
                    ClearInterface(null);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }
}
