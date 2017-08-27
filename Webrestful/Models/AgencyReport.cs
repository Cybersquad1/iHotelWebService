using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Webrestful.Models
{
    public class AgencyReport
    {
        public static List<AgencyName> GetAgencyName(MySqlConnection conn, ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<AgencyName>();
                string AgencyName = "SELECT * FROM AgencyInfo ORDER BY AgencyName";
                DataTable dtFIT = DBHelper.QueryListData(conn, AgencyName);

                for (int i = 0; i < dtFIT.Rows.Count; i++)
                {
                    var xTrans = new AgencyName();
                    xTrans.id = dtFIT.Rows[i]["AgencyID"].ToString();
                    xTrans.name = dtFIT.Rows[i]["AgencyName"].ToString();
                    xAryTransCheckIn.Add(xTrans);
                }
                return xAryTransCheckIn;
            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }
       public static List<AgencyObject> GetAgencyNight(MySqlConnection conn, string szDate, string szDate2, string mode, string id, ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<AgencyObject>();
                string AgencyNight = "SELECT "+ mode +"(A.Trans_date) as AtDay, SUM(SumRoom) AS SumRoom FROM( SELECT (A.Trans_Date), B.Trans_AgencyID, COUNT(A.Trans_Date) AS SumRoom FROM HotelTransRoomInfo A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID WHERE B.Trans_StatusID<90 AND B.Trans_AgencyID="+id+" AND A.Trans_Date>='"+szDate+"' AND A.Trans_Date<'"+szDate2+"' GROUP BY A.Trans_date, B.Trans_AgencyID UNION ALL SELECT (A.Trans_Date), C.Trans_AgencyID, COUNT(A.Trans_Date) AS SumRoom FROM HotelTransGroupDetail A JOIN HotelTransaction C ON A.TransactionID=C.TransactionID WHERE C.Trans_StatusID<90 AND C.Trans_AgencyID="+id+" AND A.Trans_Date>='"+szDate+"' AND A.Trans_Date<'"+szDate2+"' GROUP BY A.Trans_date, C.Trans_AgencyID) AS A GROUP BY "+mode+"(A.Trans_date)";
                DataTable dtFIT = DBHelper.QueryListData(conn, AgencyNight);

                for (int i = 0; i < dtFIT.Rows.Count; i++)
                {
                    var xTrans = new AgencyObject();
                    xTrans.total = dtFIT.Rows[i]["SumRoom"].ToString();
                    xTrans.date = dtFIT.Rows[i]["AtDay"].ToString();
                    xAryTransCheckIn.Add(xTrans);
                }
                return xAryTransCheckIn;
            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }
        public static List<AgencyObject> GetAgencyCharge(MySqlConnection conn, string szDate, string szDate2, string mode, string id, ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<AgencyObject>();
                string AgencyCharge = "SELECT "+mode+"(FolioDetailDate) AS AtDay, SUM(TotalPrice) AS TotalPrice FROM (SELECT FolioDetailDate, B.Trans_AgencyID, SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS TotalPrice FROM HotelFolioDetail A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID WHERE A.FolioStatusID<>99 AND FolioItemID IN (1001,2001) AND B.Trans_AgencyID="+id+" AND A.FolioDetailDate>='"+szDate+"' AND A.FolioDetailDate<'"+szDate2+"' GROUP BY FolioDetailDate, B.Trans_AgencyID UNION ALL SELECT FolioDetailDate, B.Trans_AgencyID, SUM(TotalItemPrice+TotalItemVat+TotalServiceCharge+TotalServiceChargeVat) AS TotalPrice FROM HotelFolioGroupDetail A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID WHERE A.FolioStatusID<>99 AND FolioItemID IN (1001,2001) AND B.Trans_AgencyID="+id+" AND A.FolioDetailDate>='"+szDate+"' AND A.FolioDetailDate<'"+szDate2+"' GROUP BY FolioDetailDate, B.Trans_AgencyID) AS A GROUP BY "+mode+"(FolioDetailDate)";
                DataTable dtFIT = DBHelper.QueryListData(conn, AgencyCharge);

                for (int i = 0; i < dtFIT.Rows.Count; i++)
                {
                    var xTrans = new AgencyObject();
                    xTrans.total = dtFIT.Rows[i]["TotalPrice"].ToString();
                    xTrans.date = dtFIT.Rows[i]["AtDay"].ToString();
                    xAryTransCheckIn.Add(xTrans);
                }
                return xAryTransCheckIn;
            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }      
    }     
}