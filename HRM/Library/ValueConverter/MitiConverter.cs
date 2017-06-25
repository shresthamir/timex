using HRM.Library.AppScopeClasses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Dapper;
namespace HRM.Library.ValueConverter
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    class MitiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
            {
                return conn.ExecuteScalar<string>("SELECT BS FROM MITICONVERTER WHERE AD = @AD", new { AD = (DateTime)value });
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
                {
                    string[] MitiPart = value.ToString().Split('/');
                    int day = System.Convert.ToInt32(MitiPart[0]);
                    int month = System.Convert.ToInt32(MitiPart[1]);
                    int year = System.Convert.ToInt32(MitiPart[2]);
                    if (MitiPart.Count() != 3 || day <= 0 || day > 32 || month <= 0 || month>12 || year< 1940 || year > 2080)
                        return DateTime.Today;
                    string Miti = MitiPart[0].PadLeft(2, '0') + "/" + MitiPart[1].PadLeft(2, '0') + "/" + MitiPart[2];
                    return conn.ExecuteScalar<DateTime>("SELECT AD FROM MITICONVERTER WHERE BS = @BS", new { BS = Miti });
                }
            }
            catch (Exception)
            {
                return DateTime.Today;
            }
        }
    }
}
