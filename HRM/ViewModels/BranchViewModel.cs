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

namespace HRM.ViewModels
{
    class BranchViewModel : RootViewModel
    {
        private Branch _branch;
        public Branch branch { get { return _branch; } set { _branch = value; OnPropertyChanged("branch"); } }

        public BranchViewModel()
        {
            try
            {
                MessageBoxCaption = "Branch Setup";
                branch = new Branch();

                NewCommand = new RelayCommand(NewAction);
                DeleteCommand = new RelayCommand(DeleteBranch);
                ClearCommand = new RelayCommand(ClearInterface);
                SaveCommand = new RelayCommand(SaveBranch);
                LoadData = new RelayCommand(LoadBranchInfo);
            }
            catch (Exception ex)
            {

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
                        var branches = conn.Query<Branch>("SELECT BRANCH_ID, BRANCH_NAME, BRANCH_ADDRESS, BRANCH_PHONENO FROM BRANCH");
                        BrowseViewModel bvm = new BrowseViewModel(branches, "BRANCH_ID", "BRANCH_NAME");
                        Browse browse = new Browse() { DataContext = bvm };
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("BRANCH_ID"), Header = "Id", Width = 100 });
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("BRANCH_NAME"), Header = "Branch Name", Width = 300 });
                        browse.ShowDialog();
                        if (browse.DialogResult != true)
                            return;
                        branch.BRANCH_ID = (browse.SearchGrid.SelectedItem as Branch).BRANCH_ID;
                    }
                    else
                    {
                        branch.BRANCH_ID = short.Parse(obj.ToString());
                    }

                    var b = conn.Query<Branch>("SELECT BRANCH_ID, BRANCH_NAME, BRANCH_ADDRESS, BRANCH_PHONENO FROM BRANCH WHERE BRANCH_ID = @BRANCH_ID", branch);
                    if (b != null && b.Count() > 0)
                    {
                        branch = b.FirstOrDefault();
                        SetAction(ButtonAction.Selected);
                    }
                    else
                        ShowWarning("Invalid Branch Id");
                }
            }
            catch (Exception Ex)
            {
                ShowError(Ex.Message);
            }

        }

        private void DeleteBranch(object obj)
        {
            if (ShowConfirmation("You are going to delete selected Branch. Do you want to continue?"))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            branch.Delete(tran);
                            tran.Commit();
                        }
                    }
                    ShowInformation("Branch successfully deleted.");
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
            branch = new Branch();
            SetAction(ButtonAction.Init);
        }

        private void NewAction(object obj)
        {
            try
            {
                ClearInterface(null);
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    branch.BRANCH_ID = conn.ExecuteScalar<short>("SELECT ISNULL(MAX(BRANCH_ID),0) + 1 FROM BRANCH");
                }
                SetAction(ButtonAction.New);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void SaveBranch(object obj)
        {
            try
            {
                if (ShowConfirmation("You are going to " + ((_action == ButtonAction.New) ? "save new" : "update selected") + " Branch. Do you want to Continue?") && branch.IsValid)
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            if (_action == ButtonAction.New)
                                branch.Save(tran);
                            else if (_action == ButtonAction.Edit)
                                branch.Update(tran);
                            tran.Commit();
                        }
                    }
                    ShowInformation("Branch successfully saved.");
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
