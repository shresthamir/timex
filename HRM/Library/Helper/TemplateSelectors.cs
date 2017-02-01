using HRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HRM.Library.Helper
{
    class DownloadStatusTemplateSelector: DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ContentPresenter presenter = container as ContentPresenter;
            DataGridCell cell = presenter.Parent as DataGridCell;
            AttDevice device = cell.DataContext as AttDevice;
            if (device.STATUS != 100)
            {
                return App.Current.Resources["DownloadDeviceStatus"] as DataTemplate;
            }
            else
                return App.Current.Resources["DownloadingDeviceStatus"] as DataTemplate;
        }
    }
}
