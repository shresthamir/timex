using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using HRM.Models;
using System.Collections.ObjectModel;
using HRM.Library.Helpers;
using HRM.Library.AppScopeClasses;
using System.Data.SqlClient;
using Dapper;
namespace HRM.ViewModels
{
    class DeviceSettingViewModel : RootViewModel
    {
        private AttDevice _Device;
        private List<string> _ModelList;
        private ObservableCollection<AttDevice> _DeviceList;

        public List<string> ModelList { get { return _ModelList; } set { _ModelList = value; OnPropertyChanged("ModelList"); } }
        public AttDevice Device { get { return _Device; } set { _Device = value; OnPropertyChanged("Device"); } }
        public ObservableCollection<AttDevice> DeviceList { get { return _DeviceList; } set { _DeviceList = value; OnPropertyChanged("DeviceList"); } }

        public RelayCommand AddCommand { get { return new RelayCommand(SaveDeviceSetting); } }
        public RelayCommand TestConnectionCommand { get { return new RelayCommand(TestConnection); } }

        private void TestConnection(object obj)
        {
            if (GetDeviceSerial(Device))
                ShowInformation("Connection Successfull");
            else
                ShowWarning("Connection Failed");
        }

        private void SaveDeviceSetting(object obj)
        {
            if (ShowConfirmation("You are going to save new Device. Do you want to Continue?") && Device.IsValid)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        if (conn.ExecuteScalar<int>("SELECT COUNT(*) FROM ATT_DEVICE WHERE DEVICE_IP = @DEVICE_IP", Device) > 0)
                        {
                            ShowError("Device Setting with IP '" + Device.DEVICE_IP + "' already exists. Please enter another ip and try again");
                            return;
                        }
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            Device.Save(tran);
                            tran.Commit();
                        }
                    }
                    ShowInformation("Device Setting saved successfully.");
                    GetDeviceSerial(Device);
                    DeviceList.Add(new AttDevice
                    {
                        DEVICE_MODEL = Device.DEVICE_MODEL,
                        DISPLAY_NAME = Device.DISPLAY_NAME,
                        DEVICE_IP = Device.DEVICE_IP,
                        DEVICE_SERIAL = Device.DEVICE_SERIAL
                    });
                    Device = new AttDevice();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("PRIMARY KEY"))
                        ShowError("Device Setting with display name '" + Device.DISPLAY_NAME + "' already exists. Please enter another display name and try again.");
                    else
                        ShowError(ex.Message);
                }
            }
        }

        public DeviceSettingViewModel()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    ModelList = new List<string>(conn.Query<string>("SELECT DEVICE_MODEL FROM DEVICE_MODELS"));
                    DeviceList = new ObservableCollection<AttDevice>(conn.Query<AttDevice>("SELECT DEVICE_MODEL, DISPLAY_NAME, DEVICE_IP FROM ATT_DEVICE"));
                    foreach (AttDevice ad in DeviceList)
                    {
                        GetDeviceSerial(ad);
                    }
                }
                Device = new AttDevice();
            }
            catch (Exception Ex)
            {
                ShowError(Ex.Message);
            }
        }

        private bool GetDeviceSerial(AttDevice device)
        {
            string DeviceSerial = string.Empty;
            var zkem = new zkemkeeper.CZKEM();
            if (zkem.Connect_Net(device.DEVICE_IP, 4370))
            {
                zkem.GetSerialNumber(zkem.MachineNumber, out DeviceSerial);
                device.DEVICE_SERIAL = DeviceSerial;
                zkem.Disconnect();
                return true;
            }
            return false;
        }
    }

    class DeviceDateTimeSyncViewModel : RootViewModel
    {
        private AttDevice _Device;
        private ObservableCollection<AttDevice> _DeviceList;
        private DateTime _SyncDate;
        private bool _SyncAllDevices;

        public AttDevice Device { get { return _Device; } set { _Device = value; OnPropertyChanged("Device"); } }
        public ObservableCollection<AttDevice> DeviceList { get { return _DeviceList; } set { _DeviceList = value; OnPropertyChanged("DeviceList"); } }
        public DateTime SyncDate { get { return _SyncDate; } set { _SyncDate = value; OnPropertyChanged("SyncDate"); } }
        public bool SyncAllDevices { get { return _SyncAllDevices; }  set { _SyncAllDevices = value; OnPropertyChanged("SyncAllDevices"); } }
        public RelayCommand SyncCommand { get { return new RelayCommand(SyncDateTime); } }

        private void SyncDateTime(object obj)
        {
            if (ShowConfirmation("You are going to Sync Date & Time on Device. Do you want to Continue?"))
            {
                try
                {
                    if (SyncAllDevices)
                    {
                        foreach (AttDevice ad in DeviceList)
                        {
                            SetDeviceTime(ad);
                        }
                    }
                    else
                        SetDeviceTime(Device)
                    ShowInformation("Device Time Synced Successfully");
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("PRIMARY KEY"))
                        ShowError("Device Setting with display name '" + Device.DISPLAY_NAME + "' already exists. Please enter another display name and try again.");
                    else
                        ShowError(ex.Message);
                }
            }
        }

        public DeviceDateTimeSyncViewModel()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    DeviceList = new ObservableCollection<AttDevice>(conn.Query<AttDevice>("SELECT DEVICE_MODEL, DISPLAY_NAME, DEVICE_IP FROM ATT_DEVICE"));
                    foreach (AttDevice ad in DeviceList)
                    {
                        GetDeviceSerial(ad);
                    }
                }
                SyncDate = DateTime.Now;                
            }
            catch (Exception Ex)
            {
                ShowError(Ex.Message);
            }
        }

        private void SetDeviceTime(AttDevice device)
        {
            var zkem = new zkemkeeper.CZKEM();
            DeviceTime DTime = new DeviceTime(SyncDate);
            if (zkem.Connect_Net(device.DEVICE_IP, 4370))
            {
                zkem.SetDeviceTime2(zkem.MachineNumber, DTime.dwYear, DTime.dwMonth, DTime.dwDay, DTime.dwHour, DTime.dwMinute, DTime.dwSecond);
                zkem.Disconnect();                
            }
        }

        private bool GetDeviceSerial(AttDevice device)
        {
            DeviceTime DTime = new DeviceTime();
            string DeviceSerial = string.Empty;
            var zkem = new zkemkeeper.CZKEM();
            if (zkem.Connect_Net(device.DEVICE_IP, 4370))
            {
                zkem.GetSerialNumber(zkem.MachineNumber, out DeviceSerial);
                zkem.GetDeviceTime(zkem.MachineNumber, ref DTime.dwYear, ref DTime.dwMonth, ref DTime.dwDay, ref DTime.dwHour, ref DTime.dwMinute, ref DTime.dwSecond);
                device.DEVICE_DATE = DTime.GetDate();
                device.DEVICE_SERIAL = DeviceSerial;
                zkem.Disconnect();
                return true;
            }
            return false;
        }
    }

    struct DeviceTime
    {
        public int dwYear;
        public int dwMonth;
        public int dwDay;
        public int dwHour;
        public int dwMinute;
        public int dwSecond;
        

        public DeviceTime(DateTime date)
        {
            dwYear = date.Year;
            dwMonth = date.Month;
            dwDay = date.Day;
            dwHour = date.Hour;
            dwMinute = date.Minute;
            dwSecond = date.Second;
        }

        public DateTime GetDate()
        {
            return new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond);
        }
    }
}
