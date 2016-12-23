using HRM.Library.BaseClasses;
using HRM.Library.Helpers;
using HRM.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using HRM.Library.AppScopeClasses;
using System.Windows;

namespace HRM.ViewModels
{
    class CompanyViewModel : RootViewModel
    {
        private Company _company;
        public Company company { get { return _company; } set { _company = value; OnPropertyChanged("company"); } }

        public CompanyViewModel()
        {
            try
            {
                MessageBoxCaption = "Company Information";
                company = new Company();
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    var c = conn.Query<Company>("SELECT COMPANY_ID, COMPANY_NAME, COMPANY_ADDRESS, COMPANY_PHONENO, COMPANY_EMAIL, COMPANY_PAN, COMPANY_ADDRESS_LINE2 FROM COMPANY").FirstOrDefault();
                    if (c == null)
                        company.Save(conn.BeginTransaction());
                    else
                        company = c;
                }
                SaveCommand = new RelayCommand(UpdateCompany);
            }
            catch (Exception ex)
            {

            }
        }

        private void UpdateCompany(object obj)
        {
            try
            {
                if (ShowConfirmation("You are going to update Company Information. Do you want to Proceed?") && company.IsValid)
                {
                    using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            company.Update(tran);
                            tran.Commit();
                        }
                    }
                    ShowInformation("Company Info updated successfully");
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }
}
