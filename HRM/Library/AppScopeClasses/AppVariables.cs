using HRM.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Reflection;
namespace HRM.Library.AppScopeClasses
{
    static class AppVariables
    {
        public static string ConnectionString = "SERVER = DELL-PC; DATABASE = HRMDB; UID = sa; PWD = tebahal";
        public static Company CompanyInfo;
        public static byte MaxDepartmentLevel;
        public static string LoggedUser;
        public static string[] DefaultWeekend;
        public static byte FYID { get; set; }

        static AppVariables()
        {
            if (System.IO.File.Exists(Environment.CurrentDirectory + "\\conn.txt"))
                ConnectionString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "\\conn.txt");
            
            FYID = 1;            
            DefaultWeekend = SETTING.DEFAULT_WEEKEND.Split(';');
            using(SqlConnection conn = new SqlConnection(ConnectionString))
            {
                CompanyInfo = conn.Query<Company>("SELECT * FROM COMPANY").First();
            }
        }
    }

    static class SETTING
    {
        struct Setting
        {
            public string SKEY { get; set; }
            public string VALUE { get; set; }
        }
        public static string DEFAULT_CALENDAR { get; set; }
        public static bool USE_MULTI_CALENDAR { get; set; }
        public static string DEFAULT_WEEKEND { get; set; }
        static SETTING()
        {
            Type t = typeof(SETTING);
            using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
            {
                var settings = conn.Query<Setting>("SELECT SKEY, VALUE FROM SETTING");
                foreach (Setting s in settings)
                {
                    try
                    {
                        PropertyInfo pi = t.GetProperty(s.SKEY);
                        switch (pi.PropertyType.Name.ToLower())
                        {
                            case "boolean":
                                pi.SetValue(null, (s.VALUE == "TRUE"));
                                break;
                            case "datetime":
                                pi.SetValue(null, DateTime.Parse(s.VALUE));
                                break;
                            case "int":
                                pi.SetValue(null, Convert.ToInt32(s.VALUE));
                                break;
                            default:
                                pi.SetValue(null, s.VALUE);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
    }
}
