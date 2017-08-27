using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace Webrestful.Models
{
    public class DBHelper
    {
        public static MySqlConnection ConnectDatabase(string szHotelName)
        {
            return ConnectDatabase(szHotelName, "master", "ihotel");
        }
        public static MySqlConnection ConnectDatabase(string szDbName, string szIPServer)
        {
            string szConnString = "database={0}; server={1}; user id={2}; password={3}; pooling=true; Port=3307; Max Pool Size=200;Connect Timeout=200";
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(string.Format(szConnString, szDbName, szIPServer, "master", "ihotel"));
                conn.Open();
                return conn;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //public static MySqlConnection ConnectDatabase(string szDbName, string szIPServer)
        //{
        //    return ConnectDatabase(szDbName, szIPServer, "master", "ihotel");
        //}
        public static MySqlConnection ConnectDatabase(string szDbName, string szUserName, string szPassword)
        {
            string szConnString = "database={0}; server={1}; user id={2}; password={3}; pooling=false; Port=3307";
            MySqlConnection conn = null;
            try
            {
                string szDBServer = ConfigurationManager.AppSettings["DBServer"];
                conn = new MySqlConnection(string.Format(szConnString, szDbName, szDBServer, szUserName, szPassword));
                conn.Open();
                return conn;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static MySqlConnection ConnectDatabase(string szDbName, string szIPServer, string szUserName, string szPassword)
        {
            string szConnString = "database={0}; server={1}; user id={2}; password={3}; pooling=true; Port=3307; Max Pool Size=200;Connect Timeout=200";
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(string.Format(szConnString, szDbName, szIPServer, szUserName, szPassword));
                conn.Open();
                return conn;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static MySqlConnection ConnectDatabaseWebBooking(string szHotelName, string szWebBookingLicense)
        {
            MySqlConnection conn = ConnectDatabase(szHotelName, "master", "ihotel");
            return conn;
        }


        public static void CloseConnection(MySqlConnection conn)
        {
            conn.Close();
        }

        public static int ExecuteNonQuery(MySqlConnection conn, string szQuery)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(szQuery, conn);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                return -1;
            }
        }

        public static DataTable QueryListData(MySqlConnection conn, string szQuery)
        {
            try
            {
                DataTable dt = new DataTable();
                MySqlCommand cmd = new MySqlCommand(szQuery, conn);
                MySqlDataAdapter myDA = new MySqlDataAdapter(cmd);
                myDA.SelectCommand.CommandTimeout = 0;
                myDA.Fill(dt);
                return dt;
            }
            catch (Exception err)
            {
                return null;
            }
        }
    }
    public static class DateTimeLib
    {
        public static string szFmtDateTime = "yyyy-MM-dd HH:mm:ss";
        public static string szFmtDate = "yyyy-MM-dd";
        //------------------------------------------------------------
        //---- Date Time Function (Use in query) ----
        public static string FormatDateTime(DateTime dt)
        {
            System.Globalization.CultureInfo InvC = System.Globalization.CultureInfo.InvariantCulture;
            return "{ ts '" + dt.ToString("yyyy-MM-dd HH:mm:ss", InvC) + "' }";
        }

        public static string FormatDate(DateTime dt)
        {
            System.Globalization.CultureInfo InvC = System.Globalization.CultureInfo.InvariantCulture;
            return "{ d '" + dt.ToString("yyyy-MM-dd", InvC) + "' }";
        }

        public static string FormatDateTime(string szDateTime)
        {
            System.Globalization.CultureInfo InvC = System.Globalization.CultureInfo.InvariantCulture;
            return "{ ts '" + szDateTime + "' }";
        }

        public static string FormatDate(string szDate)
        {
            System.Globalization.CultureInfo InvC = System.Globalization.CultureInfo.InvariantCulture;
            return "{ d '" + szDate + "' }";
        }
    }

    //========================================================================================
    public class Response
    {
        public int status;
        public object dataResult;
        public object dataExtra;
    }
}