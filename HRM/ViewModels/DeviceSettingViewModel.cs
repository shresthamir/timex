using HRM.Library.BaseClasses;
using System;
using System.Collections.Generic;
using HRM.Models;
using System.Collections.ObjectModel;
using HRM.Library.Helpers;
using HRM.Library.AppScopeClasses;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ZkSoftwareEU;
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
        public bool SyncAllDevices { get { return _SyncAllDevices; } set { _SyncAllDevices = value; OnPropertyChanged("SyncAllDevices"); } }
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
                        SetDeviceTime(Device);
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

        public static void SyncName(bool Old)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    var EmployeeList = conn.Query<Employee>("SELECT ENO , TITLE + ' ' + FULLNAME FULLNAME FROM EMPLOYEE");
                    var DeviceList = conn.Query<AttDevice>("SELECT DEVICE_MODEL, DISPLAY_NAME, DEVICE_IP FROM ATT_DEVICE");
                    foreach (AttDevice ad in DeviceList)
                    {
                        var zkem = new zkemkeeper.CZKEM();
                        //if (ad.DEVICE_MODEL == "U160")
                        //    continue;
                        if (zkem.Connect_Net(ad.DEVICE_IP, 4370))
                        {
                            foreach (Employee e in EmployeeList)
                            {
                                int refs = 0;
                                DeviceUser du = new DeviceUser(true);
                                if (ad.DEVICE_MODEL == "U160")
                                {
                                    
                                    //zkem.SSR_GetAllUserInfo()
                                    if (zkem.SSR_GetUserInfo(zkem.MachineNumber, e.ENO.ToString(), out du.Name, out du.Password, out du.Privilage, out du.Enabled))
                                        zkem.SSR_SetUserInfo(zkem.MachineNumber, e.ENO.ToString(), e.FULLNAME, du.Password, du.Privilage, du.Enabled);
                                    zkem.GetLastError(ref refs);
                                }
                                else if (ad.DEVICE_MODEL == "SC105")
                                {
                                    int vs = 0;
                                    byte res = 0;

                                    //zkem.GetEnrollData(zkem.MachineNumber, e.ENO, zkem.MachineNumber, 0, ref priv, ref edata, ref pwd);
                                    if (zkem.GetUserInfo(zkem.MachineNumber, e.ENO, ref du.Name, ref du.Password, ref du.Privilage, ref du.Enabled))
                                        zkem.SetUserInfo(zkem.MachineNumber, e.ENO, e.FULLNAME, du.Password, du.Privilage, du.Enabled);
                                    //zkem.SetEnrollData(zkem.MachineNumber, e.ENO, zkem.MachineNumber, 0, priv, ref edata, pwd);

                                }
                            }

                            //zkem.SetUserInfo(zkem.MachineNumber, 9999, "Admin", "9999", 1, true);
                            zkem.Disconnect();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {

            }
        }

        public static void SyncName()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    var EmployeeList = new List<Employee>(conn.Query<Employee>("SELECT ENO , TITLE + ' ' + FULLNAME FULLNAME FROM EMPLOYEE"));
                    var DeviceList = conn.Query<AttDevice>("SELECT DEVICE_MODEL, DISPLAY_NAME, DEVICE_IP FROM ATT_DEVICE");
                    foreach (AttDevice ad in DeviceList)
                    {
                        var zkem = new CZKEUEMNetClass();
                        //if (ad.DEVICE_MODEL == "U160")
                        //    continue;
                        
                        if (zkem.Connect_Net_Tcp(ad.DEVICE_IP, 4370))
                        {
                            int refs = 0;
                            DeviceUser du = new DeviceUser(true);
                            if (ad.DEVICE_MODEL == "U160")
                            {
                                do
                                {
                                    if (EmployeeList.Any(x => x.ENO == du.ENO))
                                    {
                                        Employee e = EmployeeList.First(x => x.ENO == du.ENO);
                                        zkem.SSR_SetUserInfo(zkem.MachineNumber, e.ENO.ToString(), e.FULLNAME, du.Password, du.Privilage, du.Enabled);
                                    }
                                }
                                while (zkem.GetAllUserInfo(zkem.MachineNumber, ref du.ENO, ref du.Name, ref du.Password, ref du.Privilage, ref du.Enabled));


                                zkem.GetLastError(ref refs);
                            }
                            //else if (ad.DEVICE_MODEL == "SC105")
                            
                            //    int vs = 0;
                            //    byte res = 0;

                            //    //zkem.GetEnrollData(zkem.MachineNumber, e.ENO, zkem.MachineNumber, 0, ref priv, ref edata, ref pwd);
                            //    if (zkem.GetUserInfo(zkem.MachineNumber, e.ENO, ref du.Name, ref du.Password, ref du.Privilage, ref du.Enabled))
                            //        zkem.SetUserInfo(zkem.MachineNumber, e.ENO, e.FULLNAME, du.Password, du.Privilage, du.Enabled);
                            //    //zkem.SetEnrollData(zkem.MachineNumber, e.ENO, zkem.MachineNumber, 0, priv, ref edata, pwd);

                            //}


                            //zkem.SetUserInfo(zkem.MachineNumber, 9999, "Admin", "9999", 1, true);
                            zkem.Disconnect();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {

            }
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
    struct DeviceUser
    {
        public int ENO;
        public string Name;
        public string Password;
        public int Privilage;
        public bool Enabled;


        public DeviceUser(bool de)
        {
            ENO = 0;
            Name = string.Empty;
            Password = string.Empty;
            Privilage = 0;
            Enabled = true;

        }
    }
}
