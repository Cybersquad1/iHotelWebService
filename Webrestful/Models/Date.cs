using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Webrestful.Models
{
    public class Date
    {
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