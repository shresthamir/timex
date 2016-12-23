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
    [ValueConversion(typeof(DateTime),typeof(string))]
    class MitiConverter:IValueConverter
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
            using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
            {
                return conn.ExecuteScalar<DateTime>("SELECT AD FROM MITICONVERTER WHERE BS = @BS", new { BS = value });
            }
        }
    }
}
