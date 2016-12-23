using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HRM.Library.CustomControls
{
    class WatermarkComboBox : ComboBox
    {

        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(string), typeof(WatermarkComboBox), new UIPropertyMetadata(string.Empty));

        public string Watermark
        {
            get
            {
                return (string)GetValue(WatermarkProperty);
            }
            set
            {
                SetValue(WatermarkProperty, value);
            }
        }        

        public WatermarkComboBox()
        {
            this.IsEditable = true;
        }
    }
}
