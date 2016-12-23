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
    class LeaveViewModel : RootViewModel
    {
        private Leaves _leave;
        public Leaves leave { get { return _leave; } set { _leave = value; OnPropertyChanged("leave"); } }

        public LeaveViewModel()
        {
            try
            {
                MessageBoxCaption = "Leave Setup";
                leave = new Leaves();

                NewCommand = new RelayCommand(NewAction);
                DeleteCommand = new RelayCommand(DeleteLeave);
                ClearCommand = new RelayCommand(ClearInterface);
                SaveCommand = new RelayCommand(SaveLeave);
                LoadData = new RelayCommand(LoadLeaveInfo);
            }
            catch (Exception ex)
            {

            }
        }

        private void LoadLeaveInfo(object obj)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    if (obj == null)
                    {
                        var leaves = conn.Query<Leaves>("SELECT LEAVE_ID, LEAVE_NAME FROM LEAVES");
                        BrowseViewModel bvm = new BrowseViewModel(leaves, "LEAVE_ID", "LEAVE_NAME");
                        Browse browse = new Browse() { DataContext = bvm };
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("LEAVE_ID"), Header = "Id", Width = 100 });
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("LEAVE_NAME"), Header = "Leaves Name", Width = 300 });
                        browse.ShowDialog();
                        if (browse.DialogResult != true)
                            return;
                        leave.LEAVE_ID = (browse.SearchGrid.SelectedItem as Leaves).LEAVE_ID;
                    }
                    else
                    {
                        leave.LEAVE_ID = short.Parse(obj.ToString());
                    }

                    var b = conn.Query<Leaves>("SELECT LEAVE_ID, LEAVE_NAME, ISPAIDLEAVE, MAXALLOWEDLEAVES, ANNUALLEAVECOUNT, EXPIRE, EQUIVALENTWORKINGDAYS FROM LEAVES WHERE LEAVE_ID = @LEAVE_ID", leave);
                    if (b != null && b.Count() > 0)
                    {
                        leave = b.FirstOrDefault();
                        SetAction(ButtonAction.Selected);
                    }
                    else
                        ShowWarning("Invalid Leave Id");
                }
            }
            catch (Exception Ex)
            {
                ShowError(Ex.Message);
            }

        }

        private void DeleteLeave(object obj)
        {
            if (ShowConfirmation("You are going to delete selected Leave. Do you want to continue?"))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            leave.Delete(tran);
                            tran.Commit();
                        }
                    }
                    ShowInformation("Leaves successfully deleted.");
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
            leave = new Leaves();
            SetAction(ButtonAction.Init);
        }

        private void NewAction(object obj)
        {
            try
            {
                ClearInterface(null);
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    leave.LEAVE_ID = conn.ExecuteScalar<short>("SELECT ISNULL(MAX(LEAVE_ID),0) + 1 FROM LEAVES");
                }
                SetAction(ButtonAction.New);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void SaveLeave(object obj)
        {
            try
            {
                if (ShowConfirmation("You are going to " + ((_action == ButtonAction.New) ? "save new" : "update selected") + " Leave. Do you want to Continue?") && leave.IsValid)
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            if (_action == ButtonAction.New)
                                leave.Save(tran);
                            else if (_action == ButtonAction.Edit)
                                leave.Update(tran);
                            tran.Commit();
                        }
                    }
                    ShowInformation("Leave successfully saved.");
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
