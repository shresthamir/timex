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
    class DesignationViewModel : RootViewModel
    {
        private Designation _designation;
        public Designation designation { get { return _designation; } set { _designation = value; OnPropertyChanged("designation"); } }

        public DesignationViewModel()
        {
            try
            {
                MessageBoxCaption = "Designation Setup";
                designation = new Designation();

                NewCommand = new RelayCommand(NewAction);
                DeleteCommand = new RelayCommand(DeleteDesignation);
                ClearCommand = new RelayCommand(ClearInterface);
                SaveCommand = new RelayCommand(SaveDesignation);
                LoadData = new RelayCommand(LoadDesignationInfo);
            }
            catch (Exception ex)
            {

            }
        }

        private void LoadDesignationInfo(object obj)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    if (obj == null)
                    {
                        var designations = conn.Query<Designation>("SELECT DESIGNATION_ID, DESIGNATION, LEVEL FROM DESIGNATION");
                        BrowseViewModel bvm = new BrowseViewModel(designations, "DESIGNATION_ID", "DESIGNATION");
                        Browse browse = new Browse() { DataContext = bvm };
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("DESIGNATION_ID"), Header = "Id", Width = 100 });
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("DESIGNATION"), Header = "Designation", Width = 300 });
                        browse.ShowDialog();
                        if (browse.DialogResult != true)
                            return;
                        designation.DESIGNATION_ID = (browse.SearchGrid.SelectedItem as Designation).DESIGNATION_ID;
                    }
                    else
                    {
                        designation.DESIGNATION_ID = short.Parse(obj.ToString());
                    }

                    var b = conn.Query<Designation>("SELECT DESIGNATION_ID, DESIGNATION, LEVEL FROM DESIGNATION WHERE DESIGNATION_ID = @DESIGNATION_ID", designation);
                    if (b != null && b.Count() > 0)
                    {
                        designation = b.FirstOrDefault();
                        SetAction(ButtonAction.Selected);
                    }
                    else
                        ShowWarning("Invalid Designation Id");
                }
            }
            catch (Exception Ex)
            {
                ShowError(Ex.Message);
            }

        }

        private void DeleteDesignation(object obj)
        {
            if (ShowConfirmation("You are going to delete selected Designation. Do you want to continue?"))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            designation.Delete(tran);
                            tran.Commit();
                        }
                    }
                    ShowInformation("Designation successfully deleted.");
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
            designation = new Designation();
            SetAction(ButtonAction.Init);
        }

        private void NewAction(object obj)
        {
            try
            {
                ClearInterface(null);
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    designation.DESIGNATION_ID = conn.ExecuteScalar<short>("SELECT ISNULL(MAX(DESIGNATION_ID),0) + 1 FROM DESIGNATION");
                }
                SetAction(ButtonAction.New);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void SaveDesignation(object obj)
        {
            try
            {
                if (ShowConfirmation("You are going to " + ((_action == ButtonAction.New) ? "save new" : "update selected") + " Designation. Do you want to Continue?") && designation.IsValid)
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            if (_action == ButtonAction.New)
                                designation.Save(tran);
                            else if (_action == ButtonAction.Edit)
                                designation.Update(tran);
                            tran.Commit();
                        }
                    }
                    ShowInformation("Designation successfully saved.");
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
