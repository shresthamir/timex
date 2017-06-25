﻿using HRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HRM.UI.Utilities
{
    /// <summary>
    /// Interaction logic for wDeviceSetting.xaml
    /// </summary>
    public partial class wDeviceSetting : Window
    {
        public wDeviceSetting()
        {
            InitializeComponent();
            this.DataContext = new DeviceSettingViewModel();
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
