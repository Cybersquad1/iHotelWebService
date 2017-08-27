using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Webrestful.Models
{
    public class RoomAvailableForecast
    {
        public static List<RoomForecastOverall> GetOverall(MySqlConnection conn, ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<RoomForecastOverall>();
                string OverallQuery = "SELECT A.RoomTypeID AS id, A.RoomTypeName AS name, COUNT(RoomNoID) AS TotalRoom FROM RoomType A JOIN RoomNo B ON A.RoomTypeID=B.RoomTypeID WHERE A.RoomTypeCategoryID=0 AND B.Deleted=0 AND B.Activate=1 GROUP BY id";
                DataTable dtFIT = DBHelper.QueryListData(conn, OverallQuery);
                for (int i = 0; i < dtFIT.Rows.Count; i++){
                    var xTrans = new RoomForecastOverall();
                    xTrans.RoomType = dtFIT.Rows[i]["name"].ToString();
                    xTrans.RoomTypeId = dtFIT.Rows[i]["id"].ToString();
                    xTrans.TotalRoom = dtFIT.Rows[i]["TotalRoom"].ToString();
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
        public static List<RoomForecastEach> GetEach(MySqlConnection conn, string szDate, string id,ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<RoomForecastEach>();
                string OverallQuery = "SELECT SUM(TotalRoom) AS totalroom, SUM(TotalPrice) AS totalprice FROM (SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice, A.Trans_date AS date FROM HotelTransRoomInfo A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID JOIN RoomNo C ON A.Trans_RoomID=C.RoomNoID WHERE B.Trans_StatusID<90 AND A.Trans_Date='"+szDate+"' AND C.RoomTypeID="+id+" UNION ALL SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice, A.Trans_date AS date FROM HotelTransGroupDetail A JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID JOIN HotelTransaction C ON B.TransactionID=C.TransactionID JOIN RoomNo D ON A.Trans_RoomID=D.RoomNoID WHERE C.Trans_StatusID<90 AND B.Trans_CheckOutStatus=0 AND A.Trans_Date='"+szDate+"' AND D.RoomTypeID="+id+" UNION ALL SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice, A.Rsvn_Date AS date FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID WHERE B.Rsvn_StatusID<10 AND A.Rsvn_Date='"+szDate+"' AND B.Rsvn_RoomTypeID="+id+" UNION ALL SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice, A.Rsvn_Date as date FROM RsvnGroupDetail A JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID WHERE C.Rsvn_StatusID<10 AND A.Rsvn_Date='"+szDate+"' AND B.Rsvn_RoomTypeID="+id+" ) AS A;";
                DataTable dtFIT = DBHelper.QueryListData(conn, OverallQuery);
                for (int i = 0; i < dtFIT.Rows.Count; i++)
                {
                    var xTrans = new RoomForecastEach();
                    xTrans.TotalRoom = dtFIT.Rows[i]["totalroom"].ToString();
                    xTrans.TotalPrice = dtFIT.Rows[i]["totalprice"].ToString();
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
        public static List<RoomForecastStat> GetStat(MySqlConnection conn, string szDate, ref string szErrMsg)
        {
            try
            {
                var xAryTransCheckIn = new List<RoomForecastStat>();
                string ArrivalQuery = "SELECT SUM(TotalRoom) AS arrivalroom FROM (SELECT COUNT(*) AS TotalRoom FROM RsvnTransaction WHERE Rsvn_StatusID<90 AND Rsvn_ArrivalDate='" + szDate + "' UNION ALL SELECT COUNT(*) AS TotalRoom FROM RsvnGroupInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID WHERE B.Rsvn_StatusID<90 AND A.Rsvn_ArrivalDate='" + szDate + "') AS A;";
                DataTable dtFIT = DBHelper.QueryListData(conn, ArrivalQuery);
                string arrival,departure,ooo = "";
                arrival = dtFIT.Rows[0]["arrivalroom"].ToString();

                string DepartureQuery = "SELECT SUM(TotalRoom) AS departureroom FROM (SELECT COUNT(*) AS TotalRoom FROM HotelTransaction WHERE Trans_StatusID<90 AND Trans_DepartureDate='" + szDate + "' UNION SELECT COUNT(*) AS TotalRoom FROM HotelTransGroupInfo A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID WHERE B.Trans_StatusID<90 AND A.Trans_DepartureDate='" + szDate + "' UNION SELECT COUNT(*) AS TotalRoom FROM RsvnTransaction WHERE Rsvn_StatusID<90 AND Rsvn_DepartureDate='" + szDate + "' UNION SELECT COUNT(*) AS TotalRoom FROM RsvnGroupInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID WHERE B.Rsvn_StatusID<90 AND A.Rsvn_DepartureDate='" + szDate + "') AS A ;";
                DataTable dt6 = DBHelper.QueryListData(conn, DepartureQuery);
                departure = dt6.Rows[0]["departureroom"].ToString();

                string OOOQuery = "SELECT COUNT(*) AS ooo FROM RoomMaintenance A JOIN RoomNo B ON A.RoomNoID=B.RoomNoID WHERE A.ChangeRoomStatusTo=5 AND A.MaintenanceStatusID<90 AND A.MaintenanceStatusID=1 AND MaintenanceDate<='" + szDate + "' AND MaintenanceFinishDate>'" + szDate + "';";
                DataTable dt7 = DBHelper.QueryListData(conn, OOOQuery);
                ooo= dt7.Rows[0]["ooo"].ToString();
                var xTrans = new RoomForecastStat();
                xTrans.Arrival = arrival;
                xTrans.Departure = departure;
                xTrans.ooo = ooo;
                xAryTransCheckIn.Add(xTrans);

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
