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
using HRM.Library.Helpers;
using HRM.UI.Misc;
using System.Windows.Controls;
using System.Windows.Data;
namespace HRM.ViewModels
{
    class GenderStruct:RootModel
    {
        public string Gender { get; set; }
    }


    class HolidayViewModel : RootViewModel
    {
        private Holidays _holiday;
        private ObservableCollection<GenderStruct> _GenderList;
        private ObservableCollection<Branch> _BranchList;
        private ObservableCollection<Religions> _ReligionList;
        private bool _IsGenderWise;
        private bool _IsBranchWise;
        private bool _IsReligionWise;
        private DateTime _FDate = DateTime.Today;
        private DateTime _TDate = DateTime.Today;
        private bool _IsMultipleDays;
        
        public Holidays holiday { get { return _holiday; } set { _holiday = value; OnPropertyChanged("holiday"); } }
        public bool IsMultipleDays { get { return _IsMultipleDays; } set { _IsMultipleDays = value; OnPropertyChanged("IsMultipleDays"); } }
        public bool IsGenderWise { get { return _IsGenderWise; } set { _IsGenderWise = value; OnPropertyChanged("IsGenderWise"); } }
        public bool IsBranchWise { get { return _IsBranchWise; } set { _IsBranchWise = value; OnPropertyChanged("IsBranchWise"); } }
        public bool IsReligionWise { get { return _IsReligionWise; } set { _IsReligionWise = value; OnPropertyChanged("IsReligionWise"); } }
        public DateTime FDate 
        { 
            get { return _FDate; } 
            set { _FDate = value; OnPropertyChanged("FDate"); if (!IsMultipleDays || FDate>TDate) TDate = value;}
        }
        public DateTime TDate { get { return _TDate; } set { _TDate = value; OnPropertyChanged("TDate"); } }

        public ObservableCollection<GenderStruct> GenderList
        {
            get
            {
                if (_GenderList == null)
                {
                    _GenderList = new ObservableCollection<GenderStruct>();
                    _GenderList.Add(new GenderStruct() { Gender = "Male" });
                    _GenderList.Add(new GenderStruct() { Gender = "Female" });
                    _GenderList.Add(new GenderStruct() { Gender = "Other" });
                }
                return _GenderList;
            }
            set { _GenderList = value; OnPropertyChanged("GenderList"); }
        }

        public ObservableCollection<Branch> BranchList
        {
            get
            {
                if (_BranchList == null)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                        {
                            _BranchList = new ObservableCollection<Branch>(conn.Query<Branch>("SELECT BRANCH_ID, BRANCH_NAME FROM BRANCH"));
                        }
                    }
                    catch (Exception Ex)
                    {
                        ShowError(Ex.Message);
                        return null;
                    }
                }
                return _BranchList;
            }
            set { _BranchList = value; OnPropertyChanged("BranchList"); }
        }

        public ObservableCollection<Religions> ReligionList
        {
            get
            {
                if (_ReligionList == null)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                        {
                            _ReligionList = new ObservableCollection<Religions>(conn.Query<Religions>("SELECT RELIGION FROM RELIGION"));
                        }
                    }
                    catch (Exception Ex)
                    {
                        ShowError(Ex.Message);
                        return null;
                    }
                }
                return _ReligionList;
            }
            set { _ReligionList = value; OnPropertyChanged("ReligionList"); }
        }

        public HolidayViewModel()
        {
            try
            {
                MessageBoxCaption = "Holiday Setup";
                holiday = new Holidays();

                NewCommand = new RelayCommand(NewAction);
                DeleteCommand = new RelayCommand(DeleteHoliday);
                ClearCommand = new RelayCommand(ClearInterface);
                SaveCommand = new RelayCommand(SaveHoliday);
                LoadData = new RelayCommand(LoadHolidayInfo);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void ClearInterface(object obj)
        {
            holiday = new Holidays();
            ClearSelection(GenderList);
            ClearSelection(BranchList);
            ClearSelection(ReligionList);
            SetAction(ButtonAction.Init);
        }

        void ClearSelection(IEnumerable<RootModel> list)
        {
            foreach (RootModel bm in list)
            {
                bm.IsSelected = false;
            }
        }

       

        private void NewAction(object obj)
        {
            try
            {
                ClearInterface(null);
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    holiday.HOLIDAY_ID = conn.ExecuteScalar<short>("SELECT ISNULL(MAX(HOLIDAY_ID),0) + 1 FROM HOLIDAYS");
                }
                SetAction(ButtonAction.New);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void SaveHoliday(object obj)
        {
            try
            {
                if (ShowConfirmation("You are going to " + ((_action == ButtonAction.New) ? "save new" : "update selected") + " Holiday. Do you want to Continue?"))
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            if (_action == ButtonAction.New)
                            {
                                holiday.HOLIDAY_ID = conn.ExecuteScalar<short>("SELECT ISNULL(MAX(HOLIDAY_ID),0) + 1 FROM HOLIDAYS",transaction:tran);
                                holiday.Save(tran);
                            }
                            else if (_action == ButtonAction.Edit)
                            {
                                holiday.Update(tran);
                                conn.Execute("UPDATE ATTENDANCE SET HOLIDAY_ID = NULL WHERE HOLIDAY_ID = @HOLIDAY_ID", holiday, tran);
                                ClearHolidayAdditionalTable(tran, holiday.HOLIDAY_ID);
                            }                            
                            DateTime fdate = FDate;
                            while (TDate >= fdate)
                            {
                                new Holiday_Date() { HOLIDAY_ID = holiday.HOLIDAY_ID, FYID = AppVariables.FYID, HOLIDAY_DATE = fdate }.Save(tran);
                                fdate = fdate.AddDays(1);
                            }

                            if (IsGenderWise)
                            {
                                foreach (GenderStruct gs in GenderList)
                                {
                                    if (gs.IsSelected)
                                    {
                                        new Holiday_Gender() { HOLIDAY_ID = holiday.HOLIDAY_ID, GENDER = gs.Gender }.Save(tran);
                                    }
                                }
                            }

                            if (IsBranchWise)
                            {
                                foreach (Branch b in BranchList)
                                {
                                    if (b.IsSelected)
                                    {
                                        new Holiday_Branch() { HOLIDAY_ID = holiday.HOLIDAY_ID, BRANCH_ID = b.BRANCH_ID }.Save(tran);
                                    }
                                }
                            }

                            if (IsReligionWise)
                            {
                                foreach (Religions r in ReligionList)
                                {
                                    if (r.IsSelected)
                                    {
                                        new Holiday_Religion() { HOLIDAY_ID = holiday.HOLIDAY_ID, RELIGION = r.RELIGION }.Save(tran);
                                    }
                                }
                            }
                            DataDownloadViewModel.RefreshHolidayData(tran, conn.Query<Employee>("SELECT * FROM EMPLOYEE",transaction:tran));
                            tran.Commit();
                        }
                    }
                    ShowInformation("Holiday successfully saved.");
                    ClearInterface(null);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void ClearHolidayAdditionalTable(SqlTransaction tran, short HOLIDAY_ID)
        {
            SqlConnection conn = tran.Connection;
            conn.Execute("DELETE FROM HOLIDAY_DATE WHERE HOLIDAY_ID = " + HOLIDAY_ID + " AND FYID = " + AppVariables.FYID,transaction:tran);
            conn.Execute("DELETE FROM HOLIDAY_GENDER WHERE HOLIDAY_ID = " + HOLIDAY_ID, transaction:tran);
            conn.Execute("DELETE FROM HOLIDAY_BRANCH WHERE HOLIDAY_ID = " + HOLIDAY_ID, transaction: tran);
            conn.Execute("DELETE FROM HOLIDAY_RELIGION WHERE HOLIDAY_ID = " + HOLIDAY_ID, transaction: tran);
        }

        private void DeleteHoliday(object obj)
        {
            if (ShowConfirmation("You are going to delete selected Holiday. Do you want to continue?"))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            ClearHolidayAdditionalTable(tran, holiday.HOLIDAY_ID);
                            holiday.Delete(tran);
                            conn.Execute("UPDATE ATTENDANCE SET HOLIDAY_ID = NULL WHERE HOLIDAY_ID = @HOLIDAY_ID", holiday, tran);
                            tran.Commit();
                        }
                    }
                    ShowInformation("Holiday successfully deleted.");
                    ClearInterface(null);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        private void LoadHolidayInfo(object obj)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    if (obj == null)
                    {
                        var branches = conn.Query<Holidays>("SELECT HOLIDAY_ID, HOLIDAY_NAME FROM HOLIDAYS");
                        BrowseViewModel bvm = new BrowseViewModel(branches, "HOLIDAY_ID", "HOLIDAY_NAME");
                        Browse browse = new Browse() { DataContext = bvm };
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("HOLIDAY_ID"), Header = "Id", Width = 100 });
                        browse.SearchGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("HOLIDAY_NAME"), Header = "Name", Width = 300 });
                        browse.ShowDialog();
                        if (browse.DialogResult != true)
                            return;
                        holiday.HOLIDAY_ID = (browse.SearchGrid.SelectedItem as Holidays).HOLIDAY_ID;
                    }
                    else
                    {
                        holiday.HOLIDAY_ID = short.Parse(obj.ToString());
                    }

                    var b = conn.Query<Holidays>("SELECT HOLIDAY_ID, HOLIDAY_NAME FROM HOLIDAYS WHERE HOLIDAY_ID = @HOLIDAY_ID", holiday);
                    if (b != null && b.Count() > 0)
                    {
                        holiday = b.FirstOrDefault();
                        var holiday_date = conn.Query("SELECT MIN(HOLIDAY_DATE) FDate, MAX(HOLIDAY_DATE) TDate FROM HOLIDAY_DATE WHERE HOLIDAY_ID = @HOLIDAY_ID", holiday).FirstOrDefault();
                        FDate = holiday_date.FDate;
                        TDate = holiday_date.TDate;
                        IsMultipleDays = TDate > FDate;

                        IEnumerable<Holiday_Gender> Genders = conn.Query<Holiday_Gender>("SELECT HOLIDAY_ID, GENDER FROM HOLIDAY_GENDER WHERE HOLIDAY_ID = @HOLIDAY_ID", holiday);
                        if (Genders.Count() > 0)
                        {
                            IsGenderWise = true;
                            foreach (Holiday_Gender hg in Genders)
                            {
                                GenderList.FirstOrDefault(x => x.Gender == hg.GENDER).IsSelected = true;
                            }
                        }

                        IEnumerable<Holiday_Branch> Branches = conn.Query<Holiday_Branch>("SELECT HOLIDAY_ID, BRANCH_ID FROM HOLIDAY_BRANCH WHERE HOLIDAY_ID = @HOLIDAY_ID", holiday);
                        if (Branches.Count() > 0)
                        {
                            IsBranchWise = true;
                            foreach (Holiday_Branch branch in Branches)
                            {
                                BranchList.FirstOrDefault(x => x.BRANCH_ID == branch.BRANCH_ID).IsSelected = true;
                            }
                        }

                        IEnumerable<Holiday_Religion> Religions = conn.Query<Holiday_Religion>("SELECT HOLIDAY_ID, RELIGION FROM HOLIDAY_RELIGION WHERE HOLIDAY_ID = @HOLIDAY_ID", holiday);
                        if (Religions.Count() > 0)
                        {
                            IsReligionWise = true;
                            foreach (var r in Religions)
                            {
                                ReligionList.FirstOrDefault(x => x.RELIGION == r.RELIGION).IsSelected = true;
                            }
                        }

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
    }
}
