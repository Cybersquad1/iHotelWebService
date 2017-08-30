using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Webrestful.Models
{
    public class HotelAuthenAPI
    {
        //public static string CheckHotelLicense(string szHotelLicense, string szHotelPassword, ref int iHotelID)
        //{
        //    try
        //    {
        //        var conn = DBHelper.ConnectDatabase("iHotelMaster");
        //        string szFmt = "SELECT * FROM HotelMaster WHERE LicenseValue='{0}' AND LicensePassword='{1}'";
        //        string szQuery = string.Format(szFmt, szHotelLicense.Trim().Replace("'", "''"), szHotelPassword.Trim().Replace("'", "''"));
        //        DataTable dt = DBHelper.QueryListData(conn, szQuery);
        //        iHotelID = (dt.Rows.Count == 0) ? 0 : int.Parse(dt.Rows[0]["HotelID"].ToString());
        //        conn.Close();
        //        return (dt.Rows.Count == 0) ? "" : dt.Rows[0]["DatabaseName"].ToString();
        //    }
        //    catch (Exception)
        //    {
        //        iHotelID = 0;
        //        return null;
        //    }
        //}

        public static int VerifyUserNamePassword(MySqlConnection conn, string szUserLogin, string szPassword, ref string szUserFullName)
        {
            try
            {
                string szFmt = "SELECT * FROM Users WHERE UserLogin='{0}' AND UserPassword='{1}'";
                string szQuery = string.Format(szFmt, szUserLogin, szPassword);
                DataTable dt = DBHelper.QueryListData(conn, szQuery);
                if (dt.Rows.Count == 0)
                    return 0;
                else
                {
                    szUserFullName = dt.Rows[0]["UserFirstName"] + " " + dt.Rows[0]["UserLastName"];
                    return int.Parse(dt.Rows[0]["UserID"].ToString());
                }
            }
            catch (Exception)
            {
                szUserFullName = "";
                return 0;
            }
        }

        public static DateTime GetCurrentHotelDate(MySqlConnection conn)
        {
            string szQuery = "SELECT MAX(HotelDate) FROM HotelEndDayInfo WHERE CloseShiftDate is null";
            DataTable dt = DBHelper.QueryListData(conn, szQuery);
            if (dt.Rows[0][0] != DBNull.Value)
                return Convert.ToDateTime(dt.Rows[0][0].ToString());
            else
            {
                szQuery = "SELECT MAX(HotelDate) FROM HotelEndDayInfo";
                dt = DBHelper.QueryListData(conn, szQuery);
                if (dt.Rows[0][0] == DBNull.Value)
                {
                    szQuery = "SELECT MAX(HotelDate) FROM HotelUserShiftInfo";
                    dt = DBHelper.QueryListData(conn, szQuery);
                    if (dt.Rows[0][0] == DBNull.Value)
                        return DateTime.Today;
                    else
                        return Convert.ToDateTime(dt.Rows[0][0].ToString());
                }
                else
                {
                    DateTime dtDate = Convert.ToDateTime(dt.Rows[0][0].ToString());
                    szQuery = "SELECT * FROM HotelEndDayInfo WHERE HotelDate=" + DateTimeLib.FormatDate(dtDate);
                    dt = DBHelper.QueryListData(conn, szQuery);
                    if (dt.Rows.Count == 0)
                        return dtDate;
                    else
                        return dtDate.AddDays(1);
                }
            }
        }


        public static string CheckDate(MySqlConnection conn, ref string szErrMsg)
        {
            string date = "";
            string szQuery2 = "SELECT MAX(CloseShiftDate) FROM HotelEndDayInfo";
            DataTable dt2 = DBHelper.QueryListData(conn, szQuery2);
            if (dt2.Rows[0][0] != DBNull.Value)
            {
                DateTime dtEndDay = Convert.ToDateTime(dt2.Rows[0][0].ToString());
                date = dtEndDay.ToString("yyyy/MM/dd");
            }
            else
            {
                DateTime dtEndDay2 = DateTime.Now;
                date = dtEndDay2.ToString("yyyy/MM/dd");
            }

            return date;
        }
    }
}