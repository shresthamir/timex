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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HRM.UI.Master
{
    /// <summary>
    /// Interaction logic for ucCompany.xaml
    /// </summary>
    public partial class ucDesignation : UserControl
    {
        public ucDesignation()
        {
            InitializeComponent();
            this.DataContext = new DesignationViewModel();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Library.AppScopeClasses.KeyHandler.NumericOnly(e);
        }
    }
}
