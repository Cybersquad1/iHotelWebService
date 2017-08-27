using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Webrestful.Models
{
    public class RoomForecast
    {
        public static List<SetRoomForecast> GetForecast(MySqlConnection conn, DateTime dtHotelDate, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                string enddate = dtHotelDate.AddDays(7).ToString(format, cur);

                int month = dtHotelDate.Month;
                int year = dtHotelDate.Year;
                int days = System.DateTime.DaysInMonth(year, month);

                var xAryTransCheckIn = new List<SetRoomForecast>();
                string OverallQuery = "SELECT COUNT(RoomNoID) AS TotalRoom FROM RoomType A JOIN RoomNo B ON A.RoomTypeID=B.RoomTypeID WHERE A.RoomTypeCategoryID=0 AND B.Deleted=0 AND B.Activate=1  ;";
                DataTable dtFIT = DBHelper.QueryListData(conn, OverallQuery);
                int TotalRoom = 0;
                TotalRoom = int.Parse(dtFIT.Rows[0]["TotalRoom"].ToString());
                for (int i = 0; i < 7; i++)
                {
                    ////////////GET ARRIVAL////////////
                    string startdate = dtHotelDate.ToString(format, cur);
                    string ArrivalQuery = "SELECT TotalRoom FROM ( SELECT COUNT(*) AS TotalRoom FROM RsvnTransaction WHERE Rsvn_StatusID<90 AND Rsvn_ArrivalDate='" + startdate + "' GROUP BY Rsvn_ArrivalDate UNION ALL SELECT COUNT(*) AS TotalRoom FROM RsvnGroupInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID WHERE B.Rsvn_StatusID<90 AND A.Rsvn_ArrivalDate='" + startdate + "' GROUP BY A.Rsvn_ArrivalDate) AS A";
                    DataTable dt = DBHelper.QueryListData(conn, ArrivalQuery);
                    var xTrans = new SetRoomForecast();
                    int Arr = int.Parse(dt.Rows[0]["TotalRoom"].ToString());
                    xTrans.Arrival = dt.Rows[0]["TotalRoom"].ToString();
                    ////////////GET OOO////////////
                    string OOOQuery = "SELECT Count(*) as TotalOOO FROM RoomMaintenance A JOIN RoomNo B ON A.RoomNoID=B.RoomNoID WHERE A.ChangeRoomStatusTo=5 AND A.MaintenanceStatusID<90 AND A.MaintenanceStatusID=1 AND MaintenanceFinishDate>='" + startdate + "';";
                    DataTable dt2 = DBHelper.QueryListData(conn, OOOQuery);
                    int ooo = int.Parse(dt2.Rows[0]["TotalOOO"].ToString());
                    ////////////GET AVAILABLE////////////
                    int available = 0;
                    string AvaiQuery = "SELECT SUM(TotalRoom) as TotalRoom FROM (SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice FROM HotelTransRoomInfo A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID JOIN RoomNo C ON A.Trans_RoomID=C.RoomNoID WHERE B.Trans_StatusID<90 AND A.Trans_Date='" + startdate + "' AND C.RoomTypeID<99999 UNION SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice FROM HotelTransGroupDetail A JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID JOIN HotelTransaction C ON B.TransactionID=C.TransactionID JOIN RoomNo D ON A.Trans_RoomID=D.RoomNoID WHERE C.Trans_StatusID<90 AND B.Trans_CheckOutStatus=0 AND A.Trans_Date='" + startdate + "' AND D.RoomTypeID<99999 UNION SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID WHERE B.Rsvn_StatusID<10 AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID<99999 UNION SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnGroupDetail A JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID WHERE C.Rsvn_StatusID<10 AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID<99999 ) AS A;";
                    DataTable dt3 = DBHelper.QueryListData(conn, AvaiQuery);
                    int get_Av = int.Parse(dt3.Rows[0]["TotalRoom"].ToString());
                    available = (TotalRoom) - (get_Av + ooo);
                    xTrans.Available = available.ToString();
                    ////////////GET ROOM DATE//////////// 
                    string getDate = dtHotelDate.Day.ToString();
                    xTrans.RoomDate = getDate;
                    ////////////GET DEPARTURE////////////
                    string DepartQuery = "SELECT SUM(TotalRoom) as Departure FROM (  SELECT COUNT(*) AS TotalRoom FROM HotelTransaction  WHERE Trans_StatusID<90  AND Trans_DepartureDate='" + startdate + "' UNION  SELECT COUNT(*) AS TotalRoom FROM HotelTransGroupInfo A  JOIN HotelTransaction B ON A.TransactionID=B.TransactionID  WHERE B.Trans_StatusID<90  AND A.Trans_DepartureDate='" + startdate + "' UNION SELECT COUNT(*) AS TotalRoom FROM RsvnTransaction  WHERE Rsvn_StatusID<90  AND Rsvn_DepartureDate = '" + startdate + "' UNION  SELECT COUNT(*) AS TotalRoom FROM RsvnGroupInfo A  JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID  WHERE B.Rsvn_StatusID<90  AND A.Rsvn_DepartureDate='" + startdate + "') AS A; ";
                    DataTable dt4 = DBHelper.QueryListData(conn, DepartQuery);
                    xTrans.Departure = dt4.Rows[0]["Departure"].ToString();
                    ////////////GET ROOMUSED////////////
                    string RUsedQuery = "SELECT SUM(TotalRoom) FROM (   SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice FROM HotelTransRoomInfo A   JOIN HotelTransaction B ON A.TransactionID=B.TransactionID JOIN RoomNo C ON A.Trans_RoomID=C.RoomNoID WHERE B.Trans_StatusID<90 AND A.Trans_Date='" + startdate + "' AND C.RoomTypeID>=0 UNION SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice FROM HotelTransGroupDetail A JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID JOIN HotelTransaction C ON B.TransactionID=C.TransactionID JOIN RoomNo D ON A.Trans_RoomID=D.RoomNoID WHERE C.Trans_StatusID<90 AND B.Trans_CheckOutStatus=0 AND A.Trans_Date='" + startdate + "' AND D.RoomTypeID>=0 UNION SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID WHERE B.Rsvn_StatusID<10 AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID>=0 UNION SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnGroupDetail A JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID WHERE C.Rsvn_StatusID<10 AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID>=0 ) AS A;";
                    DataTable dt5 = DBHelper.QueryListData(conn, RUsedQuery);
                    int RoomUsed = int.Parse(dt5.Rows[0]["SUM(TotalRoom)"].ToString());
                    xTrans.RoomUsed = dt5.Rows[0]["SUM(TotalRoom)"].ToString();
                    ////////////GET OCC////////////
                    int occ = (RoomUsed * 100) / TotalRoom;
                    xTrans.Occ = occ.ToString();
                    ////////////GET ABF////////////
                    int abf = Arr + RoomUsed;
                    xTrans.Abf = abf.ToString();
                    ////////////GET TREVENUE////////////
                    string TRevenueQuery = "SELECT SUM(TotalPrice) FROM ( SELECT SUM(Trans_RoomPrice) AS TotalPrice  FROM HotelTransRoomInfo A   JOIN HotelTransaction B ON A.TransactionID=B.TransactionID JOIN RoomNo C ON A.Trans_RoomID=C.RoomNoID  WHERE B.Trans_StatusID<90 AND  A.Trans_Date='" + startdate + "' AND C.RoomTypeID>=0  UNION  SELECT SUM(Trans_RoomPrice) AS TotalPrice   FROM HotelTransGroupDetail A  JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND A.Trans_GroupID=B.Trans_GroupID  JOIN HotelTransaction C ON B.TransactionID=C.TransactionID JOIN RoomNo D ON A.Trans_RoomID=D.RoomNoID  WHERE C.Trans_StatusID<90 AND B.Trans_CheckOutStatus=0  AND A.Trans_Date='" + startdate + "' AND D.RoomTypeID>=0 UNION  SELECT SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnRoomInfo A  JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID WHERE B.Rsvn_StatusID<10  AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID>=0 UNION SELECT SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnGroupDetail A  JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID  JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID WHERE C.Rsvn_StatusID<10  AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID>=0 ) AS A;";
                    DataTable dt6 = DBHelper.QueryListData(conn, TRevenueQuery);
                    float TR = float.Parse(dt6.Rows[0]["SUM(TotalPrice)"].ToString());
                    xTrans.tRevenue = TR.ToString("N2");
                    ////////////GET AVGRATE////////////
                    float avg = TR / (float)RoomUsed;
                    xTrans.AvgRate = avg.ToString("N2");
                    ////////////ADD DATALIST////////////
                    xAryTransCheckIn.Add(xTrans);
                    dtHotelDate = dtHotelDate.AddDays(1);
                }
                return xAryTransCheckIn;
            }
            catch (Exception err)
            {
                szErrMsg = err.Message;
                return null;
            }
        }

        public static List<SetRoomTypeForecast> GetRoomType(MySqlConnection conn, DateTime dtHotelDate, ref string szErrMsg)
        {
            try
            {
                string format = "yyyy-MM-dd";
                CultureInfo cur = new CultureInfo("en-US");
                string enddate = dtHotelDate.AddDays(7).ToString(format, cur);

                int month = dtHotelDate.Month;
                int year = dtHotelDate.Year;
                int days = System.DateTime.DaysInMonth(year, month);

                var xAryTransCheckIn = new List<SetRoomTypeForecast>();
                int[] eachRoom = new int[7];
                string EachQuery = "SELECT COUNT(RoomNoID) AS TotalRoom FROM RoomType A JOIN RoomNo B ON A.RoomTypeID=B.RoomTypeID WHERE A.RoomTypeCategoryID=0 AND B.Deleted=0 AND B.Activate=1 GROUP BY A.RoomTypeID ;";
                DataTable dtF = DBHelper.QueryListData(conn, EachQuery);
                for (int i = 0; i < 6; i++)
                {
                    eachRoom[i] = int.Parse(dtF.Rows[i]["TotalRoom"].ToString());
                }
                    for (int i = 0; i < 7; i++)
                {
                    string startdate = dtHotelDate.ToString(format, cur);
                    var xTrans = new SetRoomTypeForecast();
                    ////////////ADD Beach////////////
                    string BeachQuery = "SELECT sum(TotalRoom) FROM ( SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice FROM HotelTransRoomInfo A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID JOIN RoomNo C ON A.Trans_RoomID=C.RoomNoID WHERE B.Trans_StatusID<90 AND A.Trans_Date='" + startdate + "' AND C.RoomTypeID=1 UNION SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice FROM HotelTransGroupDetail A JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND " + "A.Trans_GroupID=B.Trans_GroupID JOIN HotelTransaction C ON B.TransactionID=C.TransactionID JOIN RoomNo D ON A.Trans_RoomID=D.RoomNoID WHERE C.Trans_StatusID<90 AND B.Trans_CheckOutStatus=0 AND A.Trans_Date='" + startdate + "' AND D.RoomTypeID=1 UNION SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID WHERE B.Rsvn_StatusID<10 AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID=1 UNION SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnGroupDetail A JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID WHERE C.Rsvn_StatusID<10 AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID=1) AS A;";
                    DataTable dt = DBHelper.QueryListData(conn, BeachQuery);
                    int Beach = int.Parse(dt.Rows[0]["sum(TotalRoom)"].ToString());
                    xTrans.BeachBung = (eachRoom[0] - Beach).ToString();
                    ////////////ADD Garden////////////
                    string GardenQuery = "SELECT sum(TotalRoom) FROM ( SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice FROM HotelTransRoomInfo A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID JOIN RoomNo C ON A.Trans_RoomID=C.RoomNoID WHERE B.Trans_StatusID<90 AND A.Trans_Date='" + startdate + "' AND C.RoomTypeID=2 UNION SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice FROM HotelTransGroupDetail A JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND " + "A.Trans_GroupID=B.Trans_GroupID JOIN HotelTransaction C ON B.TransactionID=C.TransactionID JOIN RoomNo D ON A.Trans_RoomID=D.RoomNoID WHERE C.Trans_StatusID<90 AND B.Trans_CheckOutStatus=0 AND A.Trans_Date='" + startdate + "' AND D.RoomTypeID=2 UNION SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID WHERE B.Rsvn_StatusID<10 AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID=2 UNION SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnGroupDetail A JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID WHERE C.Rsvn_StatusID<10 AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID=2) AS A;";
                    DataTable dt2 = DBHelper.QueryListData(conn, GardenQuery);
                    int Garden = int.Parse(dt2.Rows[0]["sum(TotalRoom)"].ToString());
                    xTrans.GardenBung = (eachRoom[1] - Garden).ToString();
                    ////////////ADD SupSea////////////
                    string SupSeaQuery = "SELECT sum(TotalRoom) FROM ( SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice FROM HotelTransRoomInfo A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID JOIN RoomNo C ON A.Trans_RoomID=C.RoomNoID WHERE B.Trans_StatusID<90 AND A.Trans_Date='" + startdate + "' AND C.RoomTypeID=3 UNION SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice FROM HotelTransGroupDetail A JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND " + "A.Trans_GroupID=B.Trans_GroupID JOIN HotelTransaction C ON B.TransactionID=C.TransactionID JOIN RoomNo D ON A.Trans_RoomID=D.RoomNoID WHERE C.Trans_StatusID<90 AND B.Trans_CheckOutStatus=0 AND A.Trans_Date='" + startdate + "' AND D.RoomTypeID=3 UNION SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID WHERE B.Rsvn_StatusID<10 AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID=3 UNION SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnGroupDetail A JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID WHERE C.Rsvn_StatusID<10 AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID=3) AS A;";
                    DataTable dt3 = DBHelper.QueryListData(conn, SupSeaQuery);
                    int SupSea = int.Parse(dt3.Rows[0]["sum(TotalRoom)"].ToString());
                    xTrans.SupSea = (eachRoom[2] - SupSea).ToString();
                    ////////////ADD SupPool////////////
                    string SupPoolQuery = "SELECT sum(TotalRoom) FROM ( SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice FROM HotelTransRoomInfo A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID JOIN RoomNo C ON A.Trans_RoomID=C.RoomNoID WHERE B.Trans_StatusID<90 AND A.Trans_Date='" + startdate + "' AND C.RoomTypeID=4 UNION SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice FROM HotelTransGroupDetail A JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND " + "A.Trans_GroupID=B.Trans_GroupID JOIN HotelTransaction C ON B.TransactionID=C.TransactionID JOIN RoomNo D ON A.Trans_RoomID=D.RoomNoID WHERE C.Trans_StatusID<90 AND B.Trans_CheckOutStatus=0 AND A.Trans_Date='" + startdate + "' AND D.RoomTypeID=4 UNION SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID WHERE B.Rsvn_StatusID<10 AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID=4 UNION SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnGroupDetail A JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID WHERE C.Rsvn_StatusID<10 AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID=4) AS A;";
                    DataTable dt4 = DBHelper.QueryListData(conn, SupPoolQuery);
                    int SupPool = int.Parse(dt4.Rows[0]["sum(TotalRoom)"].ToString());
                    xTrans.SupPool = (eachRoom[3] - SupPool).ToString();
                    ////////////ADD SupGarden////////////
                    string SupGardenQuery = "SELECT sum(TotalRoom) FROM ( SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice FROM HotelTransRoomInfo A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID JOIN RoomNo C ON A.Trans_RoomID=C.RoomNoID WHERE B.Trans_StatusID<90 AND A.Trans_Date='" + startdate + "' AND C.RoomTypeID=5 UNION SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice FROM HotelTransGroupDetail A JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND " + "A.Trans_GroupID=B.Trans_GroupID JOIN HotelTransaction C ON B.TransactionID=C.TransactionID JOIN RoomNo D ON A.Trans_RoomID=D.RoomNoID WHERE C.Trans_StatusID<90 AND B.Trans_CheckOutStatus=0 AND A.Trans_Date='" + startdate + "' AND D.RoomTypeID=5 UNION SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID WHERE B.Rsvn_StatusID<10 AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID=5 UNION SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnGroupDetail A JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID WHERE C.Rsvn_StatusID<10 AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID=5) AS A;";
                    DataTable dt5 = DBHelper.QueryListData(conn, SupGardenQuery);
                    int SupGarden = int.Parse(dt5.Rows[0]["sum(TotalRoom)"].ToString());
                    xTrans.SupGarden = (eachRoom[4] - SupGarden).ToString();
                    ////////////ADD SupBung////////////
                    string SupBungQuery = "SELECT sum(TotalRoom) FROM ( SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice FROM HotelTransRoomInfo A JOIN HotelTransaction B ON A.TransactionID=B.TransactionID JOIN RoomNo C ON A.Trans_RoomID=C.RoomNoID WHERE B.Trans_StatusID<90 AND A.Trans_Date='" + startdate + "' AND C.RoomTypeID=7 UNION SELECT COUNT(*) AS TotalRoom, SUM(Trans_RoomPrice) AS TotalPrice FROM HotelTransGroupDetail A JOIN HotelTransGroupInfo B ON A.TransactionID=B.TransactionID AND " + "A.Trans_GroupID=B.Trans_GroupID JOIN HotelTransaction C ON B.TransactionID=C.TransactionID JOIN RoomNo D ON A.Trans_RoomID=D.RoomNoID WHERE C.Trans_StatusID<90 AND B.Trans_CheckOutStatus=0 AND A.Trans_Date='" + startdate + "' AND D.RoomTypeID=7 UNION SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnRoomInfo A JOIN RsvnTransaction B ON A.Rsvn_TransID=B.Rsvn_TransID WHERE B.Rsvn_StatusID<10 AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID=7 UNION SELECT COUNT(*) AS TotalRoom, SUM(Rsvn_RoomPrice) AS TotalPrice FROM RsvnGroupDetail A JOIN RsvnGroupInfo B ON A.Rsvn_TransID=B.Rsvn_TransID AND A.Rsvn_GroupID=B.Rsvn_GroupID JOIN RsvnTransaction C ON B.Rsvn_TransID=C.Rsvn_TransID WHERE C.Rsvn_StatusID<10 AND A.Rsvn_Date='" + startdate + "' AND B.Rsvn_RoomTypeID=7) AS A;";
                    DataTable dt6 = DBHelper.QueryListData(conn, SupBungQuery);
                    int SupBung = int.Parse(dt6.Rows[0]["sum(TotalRoom)"].ToString());
                    xTrans.SupBung = (eachRoom[5]- SupBung).ToString();
                    ////////////ADD ooo////////////
                    string OOOQuery = "SELECT Count(*) as TotalOOO FROM RoomMaintenance A JOIN RoomNo B ON A.RoomNoID=B.RoomNoID WHERE A.ChangeRoomStatusTo=5 AND A.MaintenanceStatusID<90 AND A.MaintenanceStatusID=1 AND MaintenanceFinishDate>='" + startdate + "';";
                    DataTable dtO = DBHelper.QueryListData(conn, OOOQuery);
                    xTrans.OOO = dtO.Rows[0]["TotalOOO"].ToString();
                    xTrans.EachRoom = eachRoom[i].ToString();
                    xAryTransCheckIn.Add(xTrans);
                    dtHotelDate = dtHotelDate.AddDays(1);
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