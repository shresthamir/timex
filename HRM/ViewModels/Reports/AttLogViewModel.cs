using HRM.Library.AppScopeClasses;
using HRM.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Windows;
namespace HRM.ViewModels
{

    class AttLogViewModel : ReportViewModel
    {
        public AttLogViewModel()
        {
            try
            {
                ReportName = "Employye List";
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    EmpList = conn.Query<Employee>("SELECT ENO, FULLNAME, CALENDAR_TYPE FROM EMPLOYEE");
                }
                LoadData = new Library.Helpers.RelayCommand(LoadReport);
                PrintCommand = new Library.Helpers.RelayCommand(PrintReport);
            }
            catch (Exception Ex)
            {
                ShowError(Ex.Message);
            }
        }

        private void PrintReport(object obj)
        {
            string HeaderTemplate =
@"<ResourceDictionary xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
        <DataTemplate x:Key=""HeaderTemplate"">
            <Grid HorizontalAlignment=""Center"">
                <Grid.RowDefinitions>
                    <RowDefinition Height=""25""/>
                    <RowDefinition Height=""25""/>
                    <RowDefinition Height=""20""/>
                </Grid.RowDefinitions>
                <TextBlock FontWeight=""SemiBold"" FontSize=""14""  Text=""" + AppVariables.CompanyInfo.COMPANY_NAME + @"""/>
                <TextBlock Grid.Row=""1"" FontWeight=""SemiBold"" FontSize=""14"" Text=""Employee List""/>                
                <StackPanel Orientation=""Horizontal"" Grid.Row=""2"">
                    <TextBlock Width=""70"" Margin=""20 0 0 0"" Text=""Department :""/>
                    <TextBlock Text=""" + ((AllDepartments) ? "All" : SelectedDepartment.DEPARTMENT) + @"""/>                    
                </StackPanel>
            </Grid>  
        </DataTemplate>
</ResourceDictionary>
";

            sfGrid.PrintSettings.PrintPageHeaderTemplate = (XamlReader.Parse(HeaderTemplate) as ResourceDictionary)["HeaderTemplate"] as DataTemplate;
            sfGrid.Print();

            //System.IO.Stream st = new System.IO.MemoryStream();
            //PdfExportingOptions options = new PdfExportingOptions();
            //options.ExportMergedCells = true;            
            //var document = sfGrid.ExportToPdf(options);
            //document.Save(st);

            //new HRM.UI.Reports.PdfViewer(st).ShowDialog();

        }

        private void LoadReport(object obj)
        {            
            IEnumerable<dynamic> PreReport;
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {

                    string strSql =
 @"SELECT L.ENO, L.ATT_DATE, MC.BS MITI, L.ATT_TIME, L.DEVICE_NAME, E.FULLNAME ENAME  FROM ATT_LOG L JOIN EMPLOYEE E ON L.ENO = E.ENO JOIN MITICONVERTER MC ON L.ATT_DATE = MC.AD WHERE ATT_DATE =  @FDate " + 
(AllEmployee ? string.Empty : " AND L.ENO = @ENO") + " ORDER BY ENO, ATT_DATE, ATT_TIME";
                    PreReport = conn.Query<dynamic>(strSql, 
                        new 
                        { 
                            ENO = AllEmployee ? 0 : SelectedEmployee.ENO,
                            FDate = TDate
                        });

                    if (PreReport.Count() > 0)
                    {
                        ReportSource = new ObservableCollection<dynamic>(PreReport);
                        SetAction(ButtonAction.Selected);
                        ReportParameter = "Employee : " + ((AllEmployee) ? "All" : SelectedEmployee.FULLNAME);
                    }
                    else
                        ShowWarning("Record does not exists for entered criteria");
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }
}
