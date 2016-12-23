using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using HRM.Models;
namespace HRM.Library.AppScopeClasses
{
    class LeaveStatus
    {
        public short ENO { get; set; }
        public short LEAVE_ID { get; set; }
        public string ENAME { get; set; }
        public string LNAME { get; set; }
        public decimal Entitled { get; set; }
        public decimal Taken { get; set; }
        public decimal Remaining { get; set; }
    }

    class LeaveLedger
    {
        public string LNAME { get; set; }
        public DateTime LEAVE_DATE { get; set; }
        public decimal Given { get; set; }
        public decimal Taken { get; set; }
        public decimal Balance { get; set; }
    }

    static class LeaveFunctions
    {
        public static IEnumerable<LeaveStatus> GetLeaveStatus(short ENO = 0, short LEAVE_ID = 0)
        {

            string strSql = @"SELECT LL.*, E.FULLNAME ENAME, L.LEAVE_NAME LNAME FROM 
                                (
	                                SELECT ENO, LEAVE_ID, SUM(Dr) Entitled, Sum(Cr) Taken, Sum(Dr-Cr) Remaining from LEAVE_LEDGER GROUP BY ENO, LEAVE_ID
                                ) LL JOIN EMPLOYEE E ON LL.ENO = E.ENO
                                JOIN LEAVES L ON LL.LEAVE_ID = L.LEAVE_ID";
            if (ENO > 0 || LEAVE_ID > 0)
            {
                if (ENO > 0 && LEAVE_ID > 0)
                    strSql += "\nWHERE LL.ENO = @ENO AND LL.LEAVE_ID = @LEAVE_ID";
                else if (ENO > 0)
                    strSql += "\nWHERE LL.ENO = @ENO";
                else if (LEAVE_ID > 0)
                    strSql += "\nWHERE LL.LEAVE_ID = @LEAVE_ID";

            }
            strSql += "\nORDER BY ENAME, LNAME";
            using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
            {
                return conn.Query<LeaveStatus>(strSql, new { ENO = ENO, LEAVE_ID = LEAVE_ID });
            }

        }


    }

    static class DateFunctions
    {
        public static int GetBsYear(DateTime AD)
        {
            using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
            {
                return conn.ExecuteScalar<int>("SELECT CAST(RIGHT(BS,4) AS INT) FROM MITICONVERTER WHERE AD = @AD", new { AD = AD });
            }
        }

        public static DateTime GetFirstDayOfBSMonth(short MID, int Year)
        {
            using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
            {
                return conn.ExecuteScalar<DateTime>("SELECT MIN(AD) FROM MITICONVERTER WHERE RIGHT(BS,7) =  @BS", new { BS = MID.ToString().PadLeft(2, '0') + "/" + Year.ToString() });
            }
        }

        public static DateTime GetLastDayOfBSMonth(short MID, int Year)
        {
            using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
            {
                return conn.ExecuteScalar<DateTime>("SELECT MAX(AD) FROM MITICONVERTER WHERE RIGHT(BS,7) =  @BS", new { BS = MID.ToString().PadLeft(2, '0') + "/" + Year.ToString() });
            }
        }



        internal static string GetBsDate(DateTime FDate)
        {
            using (SqlConnection conn = new SqlConnection(AppVariables.ConnectionString))
            {
                return conn.ExecuteScalar<string>("SELECT BS FROM MITICONVERTER WHERE AD =  @AD", new { AD = FDate });
            }
        }

        public static void GetFirstAndLastDayOfMonth(Month month, int Year, ref DateTime FDate, ref DateTime TDate)
        {
            if (month.MTYPE == "AD")
            {
                FDate = DateTime.Parse(month.MID.ToString().PadLeft(2, '0') + "/01/" + Year.ToString());
                TDate = DateTime.Parse(month.MID.ToString().PadLeft(2, '0') + "/" + DateTime.DaysInMonth(Year, month.MID).ToString() + "/" + Year.ToString());
            }
            else
            {
                FDate = GetFirstDayOfBSMonth(month.MID, Year);
                TDate = GetLastDayOfBSMonth(month.MID, Year);
            }
            
        }
    }


}
