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
    class WorkHourViewModel : RootViewModel
    {
        private WorkHour _wHour;
        private ObservableCollection<WorkHour> _WorkHourList;
        public WorkHour wHour { get { return _wHour; } set { _wHour = value; OnPropertyChanged("wHour"); } }
        public ObservableCollection<WorkHour> WorkHourList { get { return _WorkHourList; } set { _WorkHourList = value; OnPropertyChanged("WorkHourList"); } }

        public WorkHourViewModel()
        {
            try
            {
                MessageBoxCaption = "Working Hour Setup";
                wHour = new WorkHour();
                wHour.PropertyChanged += wHour_PropertyChanged;
                NewCommand = new RelayCommand(NewAction);
                DeleteCommand = new RelayCommand(DeleteWorkhour);
                ClearCommand = new RelayCommand(ClearInterface);
                SaveCommand = new RelayCommand(SaveWorkhour);
                LoadData = new RelayCommand(LoadBranchInfo);
                RefreshWorkingHour();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        void wHour_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TOTALDURATION" || e.PropertyName == "INTIME")
            {

                if (wHour.INTIME.AddMinutes(wHour.TOTALDURATION) != wHour.OUTTIME && wHour.TOTALDURATION > 0)
                {
                    wHour.OUTTIME = wHour.OUTGRACETIME = wHour.INTIME.AddMinutes(wHour.TOTALDURATION);
                }
            }
            else if (e.PropertyName == "OUTTIME")
            {
                if (wHour.INTIME.AddMinutes(wHour.TOTALDURATION) != wHour.OUTTIME)
                    wHour.TOTALDURATION = (short)wHour.OUTTIME.Subtract(wHour.INTIME).TotalMinutes;
            }
        }

        void RefreshWorkingHour()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    WorkHourList = new ObservableCollection<WorkHour>(conn.Query<WorkHour>("SELECT WORKHOUR_ID, [DESCRIPTION], INTIME, OUTTIME, ISDEFAULT, INGRACETIME, OUTGRACETIME, LUNCHDURATION, BREAKDURATION, TOTALDURATION FROM WORKHOUR"));
                }
            }
            catch (Exception ex)
            {

                ShowError(ex.Message);
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
                        var wHours = conn.Query<WorkHour>("SELECT WORKHOUR_ID, [DESCRIPTION], INTIME, OUTTIME FROM WORKHOUR");
                        BrowseViewModel bvm = new BrowseViewModel(wHours, "WORKHOUR_ID", "DESCRIPTION");
                        Browse browse = new Browse() { DataContext = bvm };
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("WORKHOUR_ID"), Header = "Id", Width = 50 });
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("DESCRIPTION"), Header = "Work Hour Name", Width = 180 });
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("INTIME") { StringFormat = "hh:mm tt" }, Header = "In Time", Width = 80 });
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("OUTTIME") { StringFormat = "hh:mm tt" }, Header = "Out Time", Width = 80 });
                        browse.ShowDialog();
                        if (browse.DialogResult != true)
                            return;
                        wHour.WORKHOUR_ID = (browse.SearchGrid.SelectedItem as WorkHour).WORKHOUR_ID;
                    }
                    else
                    {
                        wHour.WORKHOUR_ID = short.Parse(obj.ToString());
                    }

                    var b = conn.Query<WorkHour>("SELECT WORKHOUR_ID, [DESCRIPTION], INTIME, OUTTIME, ISDEFAULT, INGRACETIME, OUTGRACETIME, LUNCHDURATION, BREAKDURATION, TOTALDURATION FROM WORKHOUR WHERE WORKHOUR_ID = @WORKHOUR_ID", wHour);
                    if (b != null && b.Count() > 0)
                    {
                        wHour = b.FirstOrDefault();
                        wHour.PropertyChanged+=wHour_PropertyChanged;
                        SetAction(ButtonAction.Selected);
                    }
                    else
                        ShowWarning("Invalid Workhour Id");
                }
            }
            catch (Exception Ex)
            {
                ShowError(Ex.Message);
            }

        }

        private void DeleteWorkhour(object obj)
        {
            if (ShowConfirmation("You are going to delete selected Workhour. Do you want to continue?"))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            wHour.Delete(tran);
                            tran.Commit();
                        }
                    }
                    ShowInformation("Workhour successfully deleted.");
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
            wHour = new WorkHour();
            wHour.PropertyChanged+=wHour_PropertyChanged;
            SetAction(ButtonAction.Init);
            RefreshWorkingHour();
        }

        private void NewAction(object obj)
        {
            try
            {
                ClearInterface(null);
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    wHour.WORKHOUR_ID = conn.ExecuteScalar<short>("SELECT ISNULL(MAX(WORKHOUR_ID),0) + 1 FROM WORKHOUR");
                }
                SetAction(ButtonAction.New);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void SaveWorkhour(object obj)
        {
            try
            {
                if (ShowConfirmation("You are going to " + ((_action == ButtonAction.New) ? "save new" : "update selected") + " Workhour. Do you want to Continue?") && wHour.IsValid)
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            if (_action == ButtonAction.New)
                                wHour.Save(tran);
                            else if (_action == ButtonAction.Edit)
                            {
                                if (conn.ExecuteScalar<int>("SELECT COUNT(*) FROM ATTENDANCE WHERE WORKHOUR_ID = @WORKHOUR_ID", wHour, transaction:tran) > 0)
                                {
                                    ShowWarning("Workhour already assigned to Attendance. Cannot update workhour");
                                    return;
                                }
                                if (wHour.ISDEFAULT)
                                    conn.Execute("UPDATE WORKHOUR SET ISDEFAULT = 0",transaction:tran);
                                wHour.Update(tran);
                            }
                            tran.Commit();
                        }
                    }
                    ShowInformation("Workhour successfully saved.");
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
