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
            var zkem = new zkemkeeper.CZKEM();
            ShowInformation(zkem.Connect_Net(Device.DEVICE_IP, 4370) ? "Connection Successfull" : "Connection Failed");
            zkem.Disconnect();
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
                    DeviceList.Add(new AttDevice
                    {
                        DEVICE_MODEL = Device.DEVICE_MODEL,
                        DISPLAY_NAME = Device.DISPLAY_NAME,
                        DEVICE_IP = Device.DEVICE_IP
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
                }
                Device = new AttDevice();
            }
            catch (Exception Ex)
            {
                ShowError(Ex.Message);
            }
        }
    }
}
