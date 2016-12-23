using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using HRM.Library.Helpers;
using System.Windows;
using System.Windows.Input;
namespace HRM.Library.BaseClasses
{
    public class RootViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public ButtonAction _action;
        private bool _EntryPanelEnabled;
        private bool _NewEnabled;
        private bool _EditEnabled;
        private bool _SaveEnabled;
        private bool _DeleteEnabled;
        private string _FailureText;
        private bool _PrintEnabled;
        private bool _ExportEnabled;
        private bool _KeyEnabled;
        private bool _HasGroup = false;

        protected string MessageBoxCaption { get; set; }
        public string FailureText
        {
            get { return _FailureText; }
            set { _FailureText = value; OnPropertyChanged("FailureText"); }
        }

        public enum ButtonAction
        {
            New = 1, Edit = 2, Init = 0, Selected = 3
        }

        public void OnPropertyChanged(string propname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propname));
            }
        }

        public RelayCommand NewCommand { get; set; }
        public RelayCommand EditCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand LoadData { get; set; }
        public RelayCommand PrintCommand { get; set; }
        public RelayCommand ExportCommand { get; set; }

        //public RelayCommand NumericField_GotFocus { get; set; }
        public bool KeyEnabled { get { return _KeyEnabled; } set { _KeyEnabled = value; OnPropertyChanged("KeyEnabled"); } }
        public bool EntryPanelEnabled { get { return _EntryPanelEnabled; } set { _EntryPanelEnabled = value; OnPropertyChanged("EntryPanelEnabled"); } }
        public bool NewEnabled { get { return _NewEnabled ; } set { _NewEnabled = value; OnPropertyChanged("NewEnabled"); } }
        public bool EditEnabled { get { return _EditEnabled ; } set { _EditEnabled = value; OnPropertyChanged("EditEnabled"); } }
        public bool SaveEnabled { get { return _SaveEnabled ; } set { _SaveEnabled = value; OnPropertyChanged("SaveEnabled"); } }
        public bool DeleteEnabled { get { return _DeleteEnabled ; } set { _DeleteEnabled = value; OnPropertyChanged("DeleteEnabled"); } }
        public bool PrintEnabled { get { return _PrintEnabled; } set { _PrintEnabled = value; OnPropertyChanged("PrintEnabled"); } }
        public bool ExportEnabled { get { return _ExportEnabled; } set { _ExportEnabled = value; OnPropertyChanged("ExportEnabled"); } }
        public bool HasGroup { get { return _HasGroup; } set { _HasGroup = value; OnPropertyChanged("HasGroup"); } }

        public RootViewModel()
        {            
               
            SetAction(ButtonAction.Init);
            EditCommand = new RelayCommand(EditAction);
            MessageBoxCaption = "TimeX Attendance & Leave Management Software";
            //NumericField_GotFocus = new RelayCommand(ChangeInputLanguageToDefault);
        }


        protected virtual void EditAction(object obj)
        {
            SetAction(ButtonAction.Edit);
        }

        protected void SetAction(ButtonAction action)
        {
            SaveEnabled = false;
            EditEnabled = false;
            NewEnabled = false;
            EntryPanelEnabled = false;
            DeleteEnabled = false;
            PrintEnabled = false;
            ExportEnabled = false;
            KeyEnabled = false;
            switch (action)
            {

                case ButtonAction.New:
                    KeyEnabled = true;
                    SaveEnabled = true;
                    EntryPanelEnabled = true;
                    break;
                case ButtonAction.Edit:
                    SaveEnabled = true;
                    EntryPanelEnabled = true;
                    break;
                case ButtonAction.Init:
                    KeyEnabled = true;
                    NewEnabled = true;
                    FailureText = string.Empty;
                    break;
                case ButtonAction.Selected:                    
                    NewEnabled = true;
                    EditEnabled = true;
                    PrintEnabled = true;
                    ExportEnabled = true;
                    DeleteEnabled = true;
                    break;
            }
            _action = action;
        }

        protected void ShowError(string Message)
        {
            MessageBox.Show(Message, MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            //MessageBox.Show("Company Info updated successfully", MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected void ShowInformation(string Message)
        {
            MessageBox.Show(Message, MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected void ShowWarning(string Message)
        {
            MessageBox.Show(Message, MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        protected bool ShowConfirmation(string Message)
        {
            return MessageBox.Show(Message, MessageBoxCaption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

    }



}
